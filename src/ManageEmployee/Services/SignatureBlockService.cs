using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.Enums;
using ManageEmployee.DataTransferObject.FileModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.SignatureBlockModels;
using ManageEmployee.Entities;
using ManageEmployee.Entities.CompanyEntities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.ProcedureEntities;
using ManageEmployee.Entities.UserEntites;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.P_Procedures;
using ManageEmployee.Services.Interfaces.Signatures;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace ManageEmployee.Services;

public class SignatureBlockService : ISignatureBlockService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IProcedureHelperService _procedureHelperService;

    public SignatureBlockService(ApplicationDbContext context, IMapper mapper, IProcedureHelperService procedureHelperService)
    {
        _context = context;
        _mapper = mapper;
        _procedureHelperService = procedureHelperService;
    }

    public async Task<PagingResult<SignatureBlockPagingModel>> GetPaging(ProcedurePagingRequestModel param)
    {
        var query = _context.SignatureBlocks.Where(x => x.Id > 0);
        if (param.StatusTab == ProduceProductStatusTab.Done)
        {
            query = query.Where(x => x.IsFinished);
        }
        else if (param.StatusTab != ProduceProductStatusTab.All)
        {
            query = query.Where(x => !x.IsFinished);
        }

        if (param.UserId != null)
        {
            var userId0 = $",{param.UserId},";
            var userId1 = $"[{param.UserId},";
            var userId2 = $",{param.UserId}]";
            query = query.Where(x => x.UserIdStr.Contains(userId0) || x.UserIdStr.Contains(userId1) || x.UserIdStr.Contains(userId2));
        }
        var data = await query.Skip(param.Page * param.PageSize).Take(param.PageSize).ToListAsync();
        var dataOut = new List<SignatureBlockPagingModel>();
        foreach (var item in data)
        {
            var itemOut = _mapper.Map<SignatureBlockPagingModel>(item);
            if (!string.IsNullOrEmpty(item.UserIdStr))
            {
                itemOut.UserIds = JsonConvert.DeserializeObject<List<int>>(item.UserIdStr);
            }
            if (!string.IsNullOrEmpty(item.FileStr))
            {
                itemOut.File = JsonConvert.DeserializeObject<FileDetailModel>(item.FileStr);
            }
            dataOut.Add(itemOut);
        }
        return new PagingResult<SignatureBlockPagingModel>
        {
            Data = dataOut,
            CurrentPage = param.Page,
            PageSize = param.PageSize,
            TotalItems = await query.CountAsync()
        };
    }

    public async Task<SignatureBlockModel> GetById(int id)
    {
        var item = await _context.SignatureBlocks.FindAsync(id);
        if (item == null)
            throw new ErrorException(ErrorMessages.DataNotFound);

        var itemOut = _mapper.Map<SignatureBlockModel>(item);
        if (!string.IsNullOrEmpty(item.UserIdStr))
        {
            itemOut.UserIds = JsonConvert.DeserializeObject<List<int>>(item.UserIdStr);
        }
        if (!string.IsNullOrEmpty(item.FileStr))
        {
            itemOut.File = JsonConvert.DeserializeObject<FileDetailModel>(item.FileStr);
        }
        return itemOut;
    }

    public async Task Create(SignatureBlockModel form, int userId)
    {
        if (!form.File.FileName.Contains(".docx"))
        {
            throw new ErrorException("Định dạng file không đúng");
        }
        var item = _mapper.Map<SignatureBlock>(form);
        if (form.UserIds != null)
        {
            item.UserIdStr = JsonConvert.SerializeObject(form.UserIds);
        }
        if (form.File != null)
        {
            item.FileStr = JsonConvert.SerializeObject(form.File);
        }
        item.CreatedAt = DateTime.Now;
        item.UserCreated = userId;
        item.UpdatedAt = DateTime.Now;
        item.UserUpdated = userId;
        _context.SignatureBlocks.Add(item);
        await _context.SaveChangesAsync();

        await AddLogNotification(form.UserIds, item.Id, nameof(ProcedureEnum.SIGNATURE_BLOCK), userId);
    }

    private async Task AddLogNotification(IEnumerable<int> userIds, int procedureId, string procedureCode, int userCreated)
    {
        var logExists = await _context.ProcedureLogs.Where(x => x.ProcedureCode == procedureCode && x.ProcedureId == procedureId).ToListAsync();
        var log = new ProcedureLog()
        {
            ProcedureId = procedureId,
            ProcedureCode = procedureCode,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            IsSendNotification = true
        };
        if (!logExists.Any())
        {
            log.UserIds = ";" + userCreated + ";";
        }
        else
        {
            _context.ProcedureLogs.RemoveRange(logExists);
            log.UserIds = ";" + string.Join(";", userIds) + ";";
        }
        await _context.ProcedureLogs.AddAsync(log);
        await _context.SaveChangesAsync();
    }

    public async Task Update(SignatureBlockModel form, int userId)
    {
        if (!form.File.FileName.Contains(".docx"))
        {
            throw new ErrorException("Định dạng file không đúng");
        }

        var data = await _context.SignatureBlocks.FindAsync(form.Id);
        if (data == null)
            throw new ErrorException(ErrorMessages.DataNotFound);
        if (form.UserIds != null)
        {
            data.UserIdStr = JsonConvert.SerializeObject(form.UserIds);
        }
        if (form.File != null)
        {
            data.FileStr = JsonConvert.SerializeObject(form.File);
        }
        //if (!string.IsNullOrEmpty(data.UserIdAccepted) && )
        //{
        //}
        data.Note = form.Note;
        data.UpdatedAt = DateTime.Now;
        data.UserUpdated = userId;
        _context.SignatureBlocks.Update(data);
        await _context.SaveChangesAsync();
    }

    public async Task Accept(int id, int userId)
    {
        var data = await _context.SignatureBlocks.FindAsync(id);
        if (data == null)
            throw new ErrorException(ErrorMessages.DataNotFound);
        if (string.IsNullOrEmpty(data.UserIdStr))
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }

        var userAcceptIds = new List<int>();
        if (!string.IsNullOrEmpty(data.UserIdAccepted))
        {
            userAcceptIds = JsonConvert.DeserializeObject<List<int>>(data.UserIdAccepted);
        }

        var userRefusedIds = new List<int>();
        if (!string.IsNullOrEmpty(data.UserIdRefused))
        {
            userRefusedIds = JsonConvert.DeserializeObject<List<int>>(data.UserIdRefused);
        }

        if (userAcceptIds.Contains(userId) && !userRefusedIds.Contains(userId))
        {
            throw new ErrorException(ErrorMessages.CannotAccept);
        }
        if (userRefusedIds.Contains(userId))
        {
            userRefusedIds.Remove(userId);
            data.UserIdRefused = JsonConvert.SerializeObject(userRefusedIds);
        }

        userAcceptIds.Add(userId);

        data.UserIdAccepted = JsonConvert.SerializeObject(userAcceptIds);
        var userNameAccept = string.Join(";", await _context.Users.Where(x => userAcceptIds.Contains(x.Id)).Select(x => x.Username + " đã duyệt; ").ToListAsync());
        var userNameRefused = string.Join(";", await _context.Users.Where(x => userRefusedIds.Contains(x.Id)).Select(x => x.Username + " không duyệt; ").ToListAsync());

        data.Notification = userNameAccept + userNameRefused;
        var userIds = JsonConvert.DeserializeObject<List<int>>(data.UserIdStr);
        data.IsFinished = userIds.All(x => userAcceptIds.Contains(x));

        data.UpdatedAt = DateTime.Now;
        data.UserUpdated = userId;
        _context.SignatureBlocks.Update(data);
        await _context.SaveChangesAsync();

        await AddLogNotification(userIds.Where(x => !userAcceptIds.Contains(x)), data.Id, nameof(ProcedureEnum.SIGNATURE_BLOCK), userId);
    }

    public async Task NotAccept(int id, int userId)
    {
        var data = await _context.SignatureBlocks.FindAsync(id);
        if (data == null)
            throw new ErrorException(ErrorMessages.DataNotFound);

        var userAcceptIds = new List<int>();
        if (string.IsNullOrEmpty(data.UserIdAccepted))
        {
            userAcceptIds = JsonConvert.DeserializeObject<List<int>>(data.UserIdAccepted);
        }

        var userRefusedIds = new List<int>();
        if (string.IsNullOrEmpty(data.UserIdRefused))
        {
            userRefusedIds = JsonConvert.DeserializeObject<List<int>>(data.UserIdRefused);
        }
        if (!userAcceptIds.Contains(userId) && userRefusedIds.Contains(userId))
        {
            throw new ErrorException(ErrorMessages.CannotAccept);
        }
        if (userAcceptIds.Contains(userId))
        {
            userAcceptIds.Remove(userId);
            data.UserIdAccepted = JsonConvert.SerializeObject(userAcceptIds);
        }
        userRefusedIds.Add(userId);

        data.UserIdRefused = JsonConvert.SerializeObject(userRefusedIds);
        var userNameAccept = string.Join(";", await _context.Users.Where(x => userAcceptIds.Contains(x.Id)).Select(x => x.Username + " đã duyệt; ").ToListAsync());
        var userNameRefused = string.Join(";", await _context.Users.Where(x => userRefusedIds.Contains(x.Id)).Select(x => x.Username + " không duyệt; ").ToListAsync());

        data.Notification = userNameAccept + userNameRefused;

        data.UpdatedAt = DateTime.Now;
        data.UserUpdated = userId;
        _context.SignatureBlocks.Update(data);
        await _context.SaveChangesAsync();
        await AddLogNotificationNotAcceptAsync(data.Id, nameof(ProcedureEnum.SIGNATURE_BLOCK), data.UserCreated);
    }

    private async Task AddLogNotificationNotAcceptAsync(int procedureId, string procedureCode, int userCreated)
    {
        var logExists = await _context.ProcedureLogs.Where(x => x.ProcedureCode == procedureCode && x.ProcedureId == procedureId).ToListAsync();
        var log = new ProcedureLog()
        {
            ProcedureId = procedureId,
            ProcedureCode = procedureCode,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            IsSendNotification = true,
            UserIds = ";" + userCreated + ";"
        };

        _context.ProcedureLogs.RemoveRange(logExists);
        await _context.ProcedureLogs.AddAsync(log);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var data = await _context.SignatureBlocks.FindAsync(id);
        if (data == null)
            throw new ErrorException(ErrorMessages.DataNotFound);

        if (!string.IsNullOrEmpty(data.UserIdAccepted))
        {
            throw new ErrorException(ErrorMessages.ProcedureCannotDelete);
        }

        _context.SignatureBlocks.RemoveRange(data);
        await _context.SaveChangesAsync();

        await _procedureHelperService.DeleteLog(id, nameof(ProcedureEnum.SIGNATURE_BLOCK));
    }

    public async Task<string> Export(int id)
    {
        var data = await _context.SignatureBlocks.FindAsync(id);
        if (data == null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }

        if (!data.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureNotFinished);
        }
        var file = JsonConvert.DeserializeObject<FileDetailModel>(data.FileStr);
        if (!file.FileName.Contains(".docx"))
        {
            throw new ErrorException("Định dạng file không đúng");
        }

        string _templateOut = @"ExportHistory\DOC\" + file.FileName;
        string filePath = file.FileUrl;
        string filePathOut = Path.Combine(Directory.GetCurrentDirectory(), _templateOut);

        string folder = Path.Combine(Directory.GetCurrentDirectory(), @"ExportHistory\\DOC");

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        var userIds = JsonConvert.DeserializeObject<List<int>>(data.UserIdStr);
        using (var document = DocX.Load(filePath))
        {
            int colNumber = userIds.Count();
            Table t = document.AddTable(3, colNumber);
            t.Alignment = Alignment.center;
            t.Design = TableDesign.ColorfulList;

            int i = 0;
            var users = await _context.Users.Where(x => userIds.Contains(x.Id))
                .Select(x => new User
                {
                    Id = x.Id,
                    PositionDetailId = x.PositionDetailId,
                    FullName = x.FullName,
                    SignFile = x.SignFile,
                }).ToListAsync();
            var positionDetails = await _context.PositionDetails.Select(x => new PositionDetail
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();

            foreach (var userId in userIds)
            {
                var userPosition = "Người lập biểu";
                var user = users.FirstOrDefault(x => x.Id == userId);
                if (user is null)
                {
                    continue;
                }
                if (i > 0)
                {
                    userPosition = positionDetails.FirstOrDefault(x => x.Id == user.PositionDetailId)?.Name;
                }

                t.Rows[0].Cells[i].Paragraphs.First().Append(userPosition);
                i++;
            }
            i = 0;
            foreach (var userId in userIds)
            {
                var user = users.FirstOrDefault(x => x.Id == userId);
                if (user is null)
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(user.SignFile))
                {
                    var link = $"{Directory.GetCurrentDirectory()}/{user.SignFile}";
                    var img = document.AddImage(link); // Thêm hình ảnh vào tài liệu
                    var picture = img.CreatePicture(); // Tạo đối tượng Picture từ hình ảnh
                    t.Rows[1].Cells[i].Paragraphs.First().AppendPicture(picture);
                }

                i++;
            }

            i = 0;
            foreach (var userId in userIds)
            {
                var user = users.FirstOrDefault(x => x.Id == userId);
                if (user is null)
                {
                    continue;
                }

                t.Rows[2].Cells[i].Paragraphs.First().Append(user.FullName);
                i++;
            }

            document.InsertTable(t);

            // Save this document to disk.
            document.SaveAs(filePathOut);
        }
        return file.FileName;
    }
}