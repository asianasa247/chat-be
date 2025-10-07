using System.Drawing;
using ManageEmployee.Entities.UserEntites;

namespace ManageEmployee.Services.Interfaces.FaceRecognitions;

public interface IFaceRecognitionService
{
    Rectangle[] FaceDetect(string imgPath);
    Task Training(string dbName);
    Task<User> DetectAndIdentifyFacesAsync(string dbName, string imgPath);
    Task<User> DetectAndIdentifyFacesAsync(string dbName, IFormFile file);
    (string msg, Rectangle[] faceResult) FaceDetectAndValidate(string imgPath);
    Task TrainingForUser(string dbName, int userId);
}
