using Emgu.CV.Structure;
using Emgu.CV;
using System.Drawing;
using Common.Constants;
using Emgu.CV.Face;
using Emgu.CV.Util;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ManageEmployee.Services.Interfaces.FaceRecognitions;
using ManageEmployee.Services.Interfaces.Assets;
using ManageEmployee.Entities.UserEntites;
using ManageEmployee.DataTransferObject.FileModels;

namespace ManageEmployee.Services;

public class FaceRecognitionService : IFaceRecognitionService
{
    private readonly ApplicationDbContext _context;
    private readonly IFileService _fileService;
    public FaceRecognitionService(
        ApplicationDbContext context, 
        IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public Rectangle[] FaceDetect(string imgPath)
    {
        // Load image from path
        var img = new Image<Bgr, byte>(imgPath);
        var grayImg = img.Convert<Gray, byte>();
        return FaceDetect(grayImg);
    }

    private Rectangle[] FaceDetect(Image<Gray, byte> grayImg)
    {
        // Create a CascadeClassifier and read the image
        var faceCascade = new CascadeClassifier("haarcascade_frontalface_default.xml");

        // Detect faces
        var faces = faceCascade.DetectMultiScale(grayImg, 1.1, 10, Size.Empty, Size.Empty);
        return faces;
    }

    public (string msg, Rectangle[] faceResult) FaceDetectAndValidate(string imgPath)
    {
        // Load image from path
        var grayImg = GetGrayImgFromImgPath(imgPath);
        return FaceDetectAndValidate(grayImg);
    }

    private (string msg, Rectangle[] faceResult) FaceDetectAndValidate(Image<Gray, byte> grayImg)
    {
        var faces = FaceDetect(grayImg);
        if (faces == null || !faces.Any())
        {
            return ("Ảnh không hợp lệ. Không tìm thấy khuôn mặt nào trong ảnh", null);
        }

        if (faces.Length > 1)
        {
            return ("Ảnh không hợp lệ. Có quá nhiều khuôn mặt trong ảnh. Vui lòng chọn ảnh chỉ có khuôn mặt bạn.",
                null);
        }

        return (string.Empty, faces);
    }

    /// <summary>
    /// Get absolute path of trained model folder
    /// </summary>
    /// <param name="dbName"></param>
    /// <returns>Absolute path of trained model folder</returns>
    private string GetTrainedFolderPath(string dbName)
    {
        var dbNameFormated = dbName.Replace('.', '_');
        var folderPath = AppConstant.FACE_DETECTOR_MODEL_TRAINED_FOLDER.GetAbsolutePath().CreateDirectoryIfNotExists();
        var fileName = $"trained_model_{dbNameFormated}.xml";
        return @$"{folderPath}\{fileName}";
    }
    private List<string> _notValue => new List<string> { "null", "[]" };
    /// <summary>
    /// Training multiple face from user table
    /// </summary>
    /// <param name="dbName"></param>
    public async Task Training(string dbName)
    {
        // Get images need to training from db
        var users = await _context.Users
            .Where(x => !string.IsNullOrWhiteSpace(x.Images) && !_notValue.Contains(x.Images))
            .Select(x => new
            {
                x.Images,
                x.Id,
            }).ToListAsync();

        if (!users.Any())
        {
            return;
        }

        var userImages = new List<(int userId, Image<Gray, byte> img)>().ToList();
        foreach (var user in users)
        {
            var images = JsonConvert.DeserializeObject<List<FileDetailModel>>(user.Images);
            if (!images.Any()) 
            {
                continue;
            }

            var userImage = images
                .Select(x =>
                    (
                        user.Id,
                        new Image<Gray, byte>(x.FileUrl.GetAbsolutePath())
                    )
                );

            userImages.AddRange(userImage);
        }

        if (!userImages.Any()) 
        {
            return;
        }

        // Prepare data
        var trainingImages = userImages.Select(x => x.img).ToList();
        var labels = userImages.Select(x => x.userId).ToList();

        // Convert the List<Image<Gray, byte>> to VectorOfMat (IInputArrayOfArrays)
        using var vectorOfMats = new VectorOfMat();
        foreach (var img in trainingImages)
        {
            vectorOfMats.Push(img.Mat);
        }
        _fileService.DeleteFileUpload(GetTrainedFolderPath(dbName));

        // Convert the List<int> to Mat (IInputArray)
        var labelsMat = new Mat(labels.Count, 1, Emgu.CV.CvEnum.DepthType.Cv32S, 1);
        labelsMat.SetTo(labels.ToArray());

        // Create the recognizer
        var faceRecognizer = new LBPHFaceRecognizer();

        // Train the recognizer 
        faceRecognizer.Train(vectorOfMats, labelsMat);

        // Store model into local storage
        faceRecognizer.Write(GetTrainedFolderPath(dbName));
    }

    /// <summary>
    /// Detect face from image
    /// Validate image: only include one face in the image
    /// Identify the user from the detected face
    /// If user not found from db. Then return exception
    /// </summary>
    /// <param name="dbName"></param>
    /// <param name="imgPath"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<User> DetectAndIdentifyFacesAsync(string dbName, string imgPath)
    {
        // Load image from path
        var grayImg = GetGrayImgFromImgPath(imgPath);

        // Detect faces from images
        var (_, detectedFaces) = FaceDetectAndValidate(grayImg);

        // Load the face recognition model
        var faceRecognizer = new LBPHFaceRecognizer();
        faceRecognizer.Read(GetTrainedFolderPath(dbName));

        var face = detectedFaces.First();
        var faceImg = grayImg.Copy(face).Resize(100, 100, Emgu.CV.CvEnum.Inter.Linear);
        var predictedFace = faceRecognizer.Predict(faceImg);

        // Check the result
        if (predictedFace.Label != -1)
        {
            // Get user information from db
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == predictedFace.Label);
            if (user == null)
            {
                throw new Exception($"Không tìm thấy thông tin người dùng với id = {predictedFace.Label}");
            }

            return user;
        }

        throw new Exception($"Không thể xác định danh tính người dùng qua hình ảnh");
    }

    /// <summary>
    /// Store img from IFormFile to temporary folder
    /// Identify face
    /// Remove img after identified
    /// </summary>
    /// <param name="dbName"></param>
    /// <param name="file"></param>
    /// <returns></returns>
    public async Task<User> DetectAndIdentifyFacesAsync(string dbName, IFormFile file)
    {
        // Upload img to temporary folder
        var tmpFolder = $@"{AppConstant.FACE_DETECTOR_ROOT_FOLDER}\tmp".CreateDirectoryIfNotExists();
        var imgPath = _fileService.Upload(file, tmpFolder);
        
        var result = await DetectAndIdentifyFacesAsync(dbName, imgPath);
        
        // Delete after detected
        _fileService.DeleteFileUpload(imgPath);

        return result;
    }
    
    /// <summary>
    /// Get image from img path input
    /// Convert image to gray image
    /// </summary>
    /// <param name="imgPath"></param>
    /// <returns></returns>
    private Image<Gray, byte> GetGrayImgFromImgPath(string imgPath)
    {
        // Load image from path
        var img = new Image<Bgr, byte>(imgPath);
        return img.Convert<Gray, byte>();
    }

    public async Task TrainingForUser(string dbName, int userId)
    {
        if (string.IsNullOrEmpty(dbName))
        {
            return;
        }
        // Get images need to training from db
        var user = await _context.Users
            .Where(x => !string.IsNullOrWhiteSpace(x.Images) && !_notValue.Contains(x.Images) && x.Id == userId)
            .Select(x => new
            {
                x.Images,
                x.Id,
            }).FirstOrDefaultAsync();

        if (user is null)
        {
            return;
        }

        var userImages = new List<(int userId, Image<Gray, byte> img)>().ToList();
        var images = JsonConvert.DeserializeObject<List<FileDetailModel>>(user.Images);
        if (!images.Any())
        {
            return;
        }

        var userImage = images
            .Select(x =>
                (
                    user.Id,
                    new Image<Gray, byte>(x.FileUrl.GetAbsolutePath())
                )
            );

        userImages.AddRange(userImage);

        // Prepare data
        var trainingImages = userImages.Select(x => x.img).ToList();
        var labels = userImages.Select(x => x.userId).ToList();

        // Convert the List<Image<Gray, byte>> to VectorOfMat (IInputArrayOfArrays)
        using var vectorOfMats = new VectorOfMat();
        foreach (var img in trainingImages)
        {
            vectorOfMats.Push(img.Mat);
        }

        // Convert the List<int> to Mat (IInputArray)
        var labelsMat = new Mat(labels.Count, 1, Emgu.CV.CvEnum.DepthType.Cv32S, 1);
        labelsMat.SetTo(labels.ToArray());

        // Create the recognizer
        var faceRecognizer = new LBPHFaceRecognizer();

        // Train the recognizer 
        faceRecognizer.Train(vectorOfMats, labelsMat);

        // Store model into local storage
        faceRecognizer.Write(GetTrainedFolderPath(dbName));
    }
}