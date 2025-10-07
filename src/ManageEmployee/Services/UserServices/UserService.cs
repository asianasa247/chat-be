using AutoMapper;
using Common.Constants;
using ManageEmployee.Helpers;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Services.Interfaces.Users;
using ManageEmployee.Services.Interfaces.Ledgers.V2;
using ManageEmployee.Services.Interfaces.Excels;
using ManageEmployee.Services.Interfaces.FaceRecognitions;
using ManageEmployee.Services.Interfaces.Assets;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.UserModels;
using ManageEmployee.DataTransferObject.UserModels.SalaryModels;
using ManageEmployee.Entities.SalaryEntities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Entities.UserEntites;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.SelectModels;
using ManageEmployee.DataTransferObject.FileModels;
using ManageEmployee.Entities.ContractorEntities;
using System.Diagnostics;
using ManageEmployee.DataLayer.Service.Interfaces;
using ManageEmployee.DataTransferObject.Contractors;
using ManageEmployee.Entities;
using Microsoft.EntityFrameworkCore;
namespace ManageEmployee.Services.UserServices;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IChartOfAccountV2Service _chartOfAccountV2Service;
    private readonly IExcelService _excelService;
    private readonly IFileService _fileService;
    private readonly IFaceRecognitionService _faceRecognitionService;
    private readonly IContractorService _contractorService;
    public UserService(
        ApplicationDbContext context,
        IMapper mapper,
        IChartOfAccountV2Service chartOfAccountV2Service,
        IExcelService excelService,
        IFileService fileService,
        IFaceRecognitionService faceRecognitionService,
        IContractorService contractorService)
    {
        _context = context;
        _mapper = mapper;
        _chartOfAccountV2Service = chartOfAccountV2Service;
        _excelService = excelService;
        _fileService = fileService;
        _faceRecognitionService = faceRecognitionService;
        _contractorService = contractorService;
    }

    public async Task<UserMapper.Auth> Authenticate(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            return null;

        var user = await (from us in _context.Users
                          where us.Username == username && !us.IsDelete
                          select new UserMapper.Auth
                          {
                              PasswordHash = us.PasswordHash,
                              PasswordSalt = us.PasswordSalt,
                              FullName = us.FullName,
                              Id = us.Id,
                              LastLogin = us.LastLogin,
                              Avatar = us.Avatar,
                              Username = us.Username,
                              Status = us.Status,
                              UserRoleIds = us.UserRoleIds,
                              Timekeeper = us.Timekeeper ?? 0,
                              TargetId = us.TargetId ?? 0,
                              Language = us.Language,
                              
                          }
        ).SingleOrDefaultAsync();

        // check if username exists
        if (user == null)
            return null;

        // check if password is correct
        if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            return null;
        // authentication successful

        return user;
    }

    public async Task<IEnumerable<UserActiveModel>> GetAllUserActive(List<string> listRole, int userId)
    {
        var users = await GetListUserCommon(listRole, userId);

        return users.ToList().ConvertAll(x => _mapper.Map<UserActiveModel>(x));
    }

    public async Task<IEnumerable<UserActiveModel>> GetAllUserActive1(List<string> listRole, int userId)
    {
        var users = await GetListUserCommon1(listRole, userId);

        return users.ToList().ConvertAll(x => _mapper.Map<UserActiveModel>(x));
    }

    public async Task<User> GetByUserName(string username)
    {
        return await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username.Equals(username) && !x.IsDelete);
    }

    public async Task<IEnumerable<string>> GetAllUserName()
    {
        return await _context.Users.Where(x => !x.IsDelete).Select(x => x.Username).ToListAsync();
    }

    public async Task<IEnumerable<User>> GetMany(Expression<Func<User, bool>> where)
    {
        return await _context.Users.Where(where).ToListAsync();
    }

    public IQueryable<User> QueryUserForPermission(int userId, List<string> roles)
    {
        var query = _context.Users.Where(x => !x.IsDelete).AsQueryable();
        var user = _context.Users?.Find(userId);
        if (user != null && !roles.Contains(UserRoleConst.SuperAdmin))
        {
            if (roles.Contains(UserRoleConst.AdminBranch))
            {
                var roleAdmin = _context.UserRoles.FirstOrDefault(x => x.Code == UserRoleConst.SuperAdmin);
                query = query.Where(x => (x.BranchId == user.BranchId || x.BranchId == 0 || x.BranchId == null)
                && (!("," + x.UserRoleIds + ",").Contains("," + roleAdmin.Id.ToString() + ",") || x.BranchId == user.BranchId)
                );
            }
            else if (roles.Contains(UserRoleConst.TruongPhong))
            {
                query = query.Where(x => x.DepartmentId == user.DepartmentId);
            }
            else if (roles.Contains(UserRoleConst.NguoiChamCong))
            {
                query = query.Where(x => x.TargetId == user.TargetId);
            }
            else
            {
                query = query.Where(x => x.Id == user.Id);
            }
        }
        return query;
    }

    public async Task<BaseResponseModel> GetPaging(UserMapper.FilterParams param)
    {
        var query = QueryUserForPermission(param.UserId, param.roles);

        if (param.Ids != null && param.Ids.Any())
        {
            query = query.Where(x => param.Ids.Contains(x.Id));
        }

        if (!string.IsNullOrEmpty(param.Keyword))
        {
            query = query.Where(x => x.Username.ToLower().Contains(param.Keyword) ||
            x.Phone.Contains(param.Keyword)
            || x.FullName.ToLower().Contains(param.Keyword));
        }

        if (param.WarehouseId != null && param.WarehouseId.Value != 0)
        {
            query = query.Where(x => x.WarehouseId == param.WarehouseId.Value);
        }

        if (param.DepartmentId != null && param.DepartmentId.Value != 0)
        {
            query = query.Where(x => x.DepartmentId == param.DepartmentId.Value);
        }
        if (param.RequestPassword != null && param.RequestPassword.Value)
        {
            query = query.Where(x => x.RequestPassword == param.RequestPassword.Value);
        }
        if (param.Quit != null && param.Quit.HasValue)
        {
            query = query.Where(x => x.Quit == param.Quit.Value);
        }

        if (param.Gender != GenderEnum.All)
        {
            query = query.Where(x => x.Gender == param.Gender);
        }

        if (param.BirthDay.HasValue)
        {
            query = query.Where(x => x.BirthDay != null && x.BirthDay == param.BirthDay);
        }
        if (param.TargetId != 0)
        {
            query = query.Where(x => x.TargetId == param.TargetId);
        }
        if (param.Month.HasValue && param.Month.Value > 0 && param.Month.Value <= 12)
        {
            query = query.Where(x => x.BirthDay != null && x.BirthDay.Value.Month == param.Month);
        }

        if (param.StartDate != null && param.EndDate != null)
        {
            query = query.Where(x => x.BirthDay != null && x.BirthDay.Value >= param.StartDate && x.BirthDay.Value <= param.EndDate);
        }
        else if (param.StartDate != null)
        {
            query = query.Where(x => x.BirthDay != null && x.BirthDay.Value >= param.StartDate);
        }
        else if (param.EndDate != null)
        {
            query = query.Where(x => x.BirthDay != null && x.BirthDay.Value <= param.EndDate);
        }
        if (param.DegreeId > 0)
        {
            query = query.Where(x => string.IsNullOrEmpty(x.LiteracyDetail) || x.LiteracyDetail.Contains(param.DegreeId.ToString()));
        }

        if (!string.IsNullOrEmpty(param.SortField))
        {
            switch (param.SortField)
            {
                case "id":
                    query = param.isSort ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id);
                    break;

                case "fullName":
                    query = param.isSort ? query.OrderByDescending(x => x.FullName) : query.OrderBy(x => x.FullName);
                    break;

                case "birthDay":
                    query = param.isSort ? query.OrderByDescending(x => x.BirthDay) : query.OrderBy(x => x.BirthDay);
                    break;
            }
        }

        var totalCount = await query.CountAsync();

        var result = new BaseResponseModel()
        {
            CurrentPage = param.CurrentPage,
            PageSize = param.PageSize,
            DataTotal = totalCount,
        };

        result.TotalItems = totalCount;

        query = query.Skip(param.PageSize * (param.CurrentPage == 0 ? param.CurrentPage : param.CurrentPage - 1))
            .Take(param.PageSize);

        var users = from coa in query
                    join district in _context.Districts
                    on coa.DistrictId equals district.Id into empDistrict
                    from dis in empDistrict.DefaultIfEmpty()

                    join province in _context.Provinces
                    on coa.ProvinceId equals province.Id into empProvince
                    from pro in empProvince.DefaultIfEmpty()

                    join ward in _context.Wards
                    on coa.WardId equals ward.Id into empWard
                    from war in empWard.DefaultIfEmpty()

                    join districtNat in _context.Districts
                    on coa.NativeDistrictId equals districtNat.Id into empDistrictNat
                    from disNat in empDistrictNat.DefaultIfEmpty()

                    join provinceNat in _context.Provinces
                    on coa.NativeProvinceId equals provinceNat.Id into empProvinceNat
                    from proNat in empProvinceNat.DefaultIfEmpty()

                    join wardNat in _context.Wards
                    on coa.NativeWardId equals wardNat.Id into empWardNat
                    from warNat in empWardNat.DefaultIfEmpty()

                    join dep in _context.Departments
                    on coa.DepartmentId equals dep.Id into empDep
                    from empDepart in empDep.DefaultIfEmpty()
                    where !coa.IsDelete
                    select new UserModel()
                    {
                        Id = coa.Id,
                        Avatar = coa.Avatar,
                        FullName = coa.FullName ?? "",
                        Phone = coa.Phone ?? "",
                        Identify = coa.Identify,
                        Address = coa.Address,
                        BirthDay = coa.BirthDay,
                        Gender = coa.Gender,
                        RequestPassword = coa.RequestPassword,
                        Quit = coa.Quit,
                        TargetId = coa.TargetId ?? 0,
                        AddressFull = coa.Address + ", " + war.Name + ", " + dis.Name + ", " + pro.Name,
                        NativeAddressFull = warNat.Name + ", " + disNat.Name + ", " + proNat.Name,
                        Username = coa.Username ?? "",
                        Note = coa.Note,
                        UserRoleIds = coa.UserRoleIds,
                        Status = coa.Status,
                        PlaceOfPermanent = coa.PlaceOfPermanent,
                        Religion = coa.Religion,
                        EthnicGroup = coa.EthnicGroup,
                        UnionMember = coa.UnionMember ?? 0,
                        Nation = coa.Nation,
                        Literacy = coa.Literacy,
                        LiteracyDetail = coa.LiteracyDetail,
                        BankAccount = coa.BankAccount,
                        Bank = coa.Bank,
                        NoOfLeaveDate = coa.NoOfLeaveDate ?? 0,
                        ShareHolderCode = coa.ShareHolderCode,
                        Timekeeper = coa.Timekeeper ?? 0,
                        LicensePlates = coa.LicensePlates,
                        DepartmentName = empDepart.Name,
                        SalaryPercentage = coa.SalaryPercentage ?? 0,

                    };

        var listItems = await users.ToListAsync();

        var listRole = _context.UserRoles?.ToDictionary(x => x.Id.ToString())
           ?? new Dictionary<string, UserRole>();

        List<string> roles = new List<string>();

        foreach (var item in listItems)
        {
            var roleIds = item.UserRoleIds?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? new string[] { };

            roles.Clear();

            foreach (var roleId in roleIds)
            {
                if (listRole.ContainsKey(roleId))
                {
                    roles.Add(listRole[roleId].Title);
                }
            }

            item.UserRoleName = string.Join(",", roles);
        }

        result.Data = listItems;
        return result;
    }

    public async Task<User> GetByIdAsync(int id)
    {
        return await _context.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task Create(UserModel userParam, string password, string dbName = "")
    {
        // validation
        if (string.IsNullOrWhiteSpace(password))
            throw new ErrorException(ResultErrorConstants.USER_MISS_PASSWORD);
        if (string.IsNullOrWhiteSpace(userParam.Username))
            throw new ErrorException(ResultErrorConstants.USER_MISS_USERNAME);

        if (await _context.Users.AnyAsync(x => x.Username == userParam.Username && !x.IsDelete))
            throw new ErrorException(ResultErrorConstants.USER_USNEXIST);

        var user = _mapper.Map<User>(userParam);

        byte[] passwordHash, passwordSalt;
        CreatePasswordHash(password, out passwordHash, out passwordSalt);

        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;

        if (string.IsNullOrEmpty(user.Avatar))
        {
            var company = await _context.Companies.OrderByDescending(x => x.SignDate).FirstOrDefaultAsync();
            user.Avatar = company?.FileLogo;
        }
        //if (userParam.SignFile != null)
        //{
        //    var fileUrl = _fileService.Upload(userParam.SignFile, "User", userParam.SignFile.FileName);
        //    var file = new FileDetailModel
        //    {
        //        FileName = userParam.SignFile.FileName,
        //        FileUrl = fileUrl,
        //    };
        //    user.SignFile = JsonConvert.SerializeObject(file).ToString();
        //}

        await _context.Users.AddAsync(user);

        await _context.SaveChangesAsync();


        UpdateContractor(_context, user, userParam);
        await _faceRecognitionService.Training(dbName);
    }

    public async Task CreateExcel(List<UserModel> users, int userId)
    {
        var isCheckUserName = users.Any(x => string.IsNullOrWhiteSpace(x.Username));
        if (isCheckUserName)
            throw new ErrorException(ResultErrorConstants.USER_MISS_USERNAME);

        var userUpdates = new List<User>();
        var userAdds = new List<User>();
        var departments = await _context.Departments.Where(x => !x.isDelete).ToListAsync();
        var listDegree = await _context.Degrees.Where(x => !x.IsDelete).ToListAsync();

        foreach (var user in users)
        {
            User userCheck = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == user.Username && !x.IsDelete);
            if (userCheck != null)
            {
                userCheck.Avatar = user.Avatar;
            }
            else
            {
                userCheck = _mapper.Map<User>(user);
                userCheck.UserCreated = userId;
                userCheck.CreatedAt = DateTime.UtcNow;
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash("123456", out passwordHash, out passwordSalt);

                userCheck.PasswordHash = passwordHash;
                userCheck.PasswordSalt = passwordSalt;
                if (string.IsNullOrEmpty(user.Avatar))
                {
                    var company = await _context.Companies.OrderByDescending(x => x.SignDate).FirstOrDefaultAsync();
                    user.Avatar = company?.FileLogo;
                }
            }
            if (!string.IsNullOrEmpty(user.LiteracyDetail))
            {
                string[] LiteracyDetails = user.LiteracyDetail.Split(',');
                if (!LiteracyDetails.Any())
                    LiteracyDetails[0] = user.LiteracyDetail;
                userCheck.LiteracyDetail = "";
                foreach (var literacyDetail in LiteracyDetails)
                {
                    var degree = listDegree.Find(x => x.Name == literacyDetail.Trim());
                    if (degree != null)
                        userCheck.LiteracyDetail += degree.Id + ",";
                }
            }
            userCheck.UserUpdated = userId;
            userCheck.UpdatedAt = DateTime.UtcNow;
            userCheck.Username = user.Username;
            userCheck.FullName = user.FullName;
            userCheck.PositionDetailId = user.PositionDetailId;
            userCheck.DepartmentId = departments.Find(x => x.Name == user.DepartmentName)?.Id;
            userCheck.Gender = user.Gender;
            userCheck.BirthDay = user.BirthDay;
            userCheck.ProvinceId = await _context.Provinces.Where(x => x.Name == user.ProvinceName).Select(x => x.Id).FirstOrDefaultAsync();
            userCheck.DistrictId = await _context.Districts.Where(x => x.Name == user.DistrictName && x.ProvinceId == userCheck.ProvinceId).Select(x => x.Id).FirstOrDefaultAsync();
            userCheck.WardId = await _context.Wards.Where(x => x.Name == user.WardName && x.DistrictId == userCheck.DistrictId).Select(x => x.Id).FirstOrDefaultAsync();
            userCheck.Address = user.Address;
            userCheck.EthnicGroup = user.EthnicGroup;
            userCheck.Religion = user.Religion;
            userCheck.Phone = user.Phone;
            userCheck.SocialInsuranceCode = user.SocialInsuranceCode;
            userCheck.Identify = user.Identify;
            userCheck.IdentifyCreatedDate = user.IdentifyCreatedDate;
            userCheck.IdentifyCreatedPlace = user.IdentifyCreatedPlace;
            userCheck.Literacy = user.Literacy;
            userCheck.LiteracyDetail = user.LiteracyDetail;
            userCheck.SendSalaryDate = user.SendSalaryDate;
            userCheck.Salary = user.Salary;
            userCheck.Note = user.Note;
            

            if (userCheck.Id > 0)
            {
                userUpdates.Add(userCheck);
            }
            else
            {
                userAdds.Add(userCheck);
            }
        }
        _context.Users.UpdateRange(userUpdates);
        await _context.Users.AddRangeAsync(userAdds);
        await _context.SaveChangesAsync();
    }

    public async Task Update(UserModel userParam, string dbName = "")
    {
        if (string.IsNullOrWhiteSpace(userParam.Username))
            throw new ErrorException(ResultErrorConstants.USER_MISS_USERNAME);

        if (await _context.Users.AnyAsync(x => x.Username == userParam.Username && !x.IsDelete && x.Id != userParam.Id))
            throw new ErrorException(ResultErrorConstants.USER_USNEXIST);

        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userParam.Id);
        if (user == null)
            throw new ErrorException(ResultErrorConstants.USER_EMPTY_OR_DELETE);
        byte[] passwordHash = user.PasswordHash;
        byte[] passwordSalt = user.PasswordSalt;
        user = _mapper.Map<User>(userParam);

        user.PasswordSalt = passwordSalt;
        user.PasswordHash = passwordHash;
        if (string.IsNullOrWhiteSpace(userParam.FullName))
            userParam.FullName = user.FullName;

        user.UpdatedAt = DateTime.Now;
        //if (userParam.SignFile != null)
        //{
        //    var fileUrl = _fileService.Upload(userParam.SignFile, "User", userParam.SignFile.FileName);
        //    var file = new FileDetailModel
        //    {
        //        FileName = userParam.SignFile.FileName,
        //        FileUrl = fileUrl,
        //    };
        //    user.SignFile = JsonConvert.SerializeObject(file).ToString();
        //}

        _context.Users.Update(user);
        // kiểm tra đường dẫn của hình ảnh có tồn tại hay không, nếu tồn tại file thì nhận dạng lại
        
        //UpdateContractor(_context, user, userParam);

        await _context.SaveChangesAsync();
    
        // await _faceRecognitionService.Training(dbName);
    }
    public async Task ResetPasswordForMultipleUser(List<int> ids)
    {
        string password = "123456";
        var users = await _context.Users.Where(x => ids.Contains(x.Id)).ToListAsync();
        // update password if provided
        foreach (var user in users)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.RequestPassword = false;
        }

        _context.Users.UpdateRange(users);

        await _context.SaveChangesAsync();
    }
    public async Task ResetPassword(User userParam, string password = null)
    {
        var user = await _context.Users.AsNoTracking().SingleAsync(x => x.Id == userParam.Id);
        // update password if provided
        if (!string.IsNullOrWhiteSpace(password))
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
        }

        user.RequestPassword = userParam.RequestPassword;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> CheckPassword(int id, string oldPassword)
    {
        var user = await _context.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
        if (user == null) return false;
        return VerifyPasswordHash(oldPassword, user.PasswordHash, user.PasswordSalt);
    }

    public async Task UpdatePassword(PasswordModel passwordModel)
    {
        var user = await _context.Users.AsNoTracking().SingleAsync(x => x.Id == passwordModel.Id);
        // update password if provided
        if (!string.IsNullOrWhiteSpace(passwordModel.Password))
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(passwordModel.Password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateLastLogin(int userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
            throw new ErrorException(ResultErrorConstants.USER_EMPTY_OR_DELETE);
        user.LastLogin = DateTime.Now;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            user.IsDelete = true;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }

    private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        if (password == null) throw new ArgumentNullException("password");
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

        using (var hmac = new System.Security.Cryptography.HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }

    private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
    {
        if (password == null) throw new ArgumentNullException("password");
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
        if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
        if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

        using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
        {
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != storedHash[i]) return false;
            }
        }

        return true;
    }

    public async Task<List<UserModel>> GetForExcel(List<int> ids, int userId, List<string> roles)
    {
        var data = (await GetListUserCommon(userId, roles)).Where(x => ids.Contains(x.Id) || ids.Count == 0).ToList();
        return data;
    }

    public async Task<List<SelectListModel>> HeaderThongKeTongQuat()
    {
        var listDegree = await _context.Degrees.Where(x => !x.IsDelete).OrderBy(x => x.Order).ToListAsync();
        var listContractType = await _context.ContractTypes.ToListAsync();
        List<SelectListModel> listHeader = new List<SelectListModel>()
        {
            new SelectListModel()
            {
                Id = -3,
                Name = "Tổng số",
            },
            new SelectListModel()
            {
                Id = -1,
                Name = "Nam",
            },
            new SelectListModel()
            {
                Id = -2,
                Name = "Nữ",
            }
        };

        foreach (var item in listDegree)
        {
            SelectListModel itemAdd = new SelectListModel();
            itemAdd.Id = item.Id;
            itemAdd.Name = item.Name;
            itemAdd.Type = 1;

            listHeader.Add(itemAdd);
        }
        foreach (var item in listContractType)
        {
            SelectListModel itemAdd = new SelectListModel();
            itemAdd.Id = item.Id;
            itemAdd.Name = item.Name;
            itemAdd.Type = 2;

            listHeader.Add(itemAdd);
        }
        return listHeader;
    }

    public async Task<List<ThongKeTongQuat>> GetListThongKeTongQuat(List<string> listRole, int userId)
    {
        var listDepartment = await _context.Departments.Where(x => !x.isDelete).ToListAsync();
        List<SelectListModel> listHeader = await HeaderThongKeTongQuat();
        var listNhanSu = await GetListUserCommon(listRole, userId);
        List<ThongKeTongQuat> listOut = new List<ThongKeTongQuat>();
        foreach (var depatment in listDepartment)
        {
            ThongKeTongQuat item = new ThongKeTongQuat();
            item.Id = depatment.Id;
            item.Name = depatment.Name;
            item.listChildren = new List<ThongKeTongQuat>();

            foreach (var header in listHeader)
            {
                var listNhanSu_new = new List<User>();
                ThongKeTongQuat child = new ThongKeTongQuat();
                child.Name = header.Name;

                if (header.Id == -3)
                {
                    listNhanSu_new = listNhanSu.Where(x => x.DepartmentId == depatment.Id).ToList();
                    child.isBold = true;
                }
                else if (header.Id == -1)
                    listNhanSu_new = listNhanSu.Where(x => x.DepartmentId == depatment.Id && x.Gender == GenderEnum.Male).ToList();
                else if (header.Id == -2)
                    listNhanSu_new = listNhanSu.Where(x => x.DepartmentId == depatment.Id && x.Gender == GenderEnum.Female).ToList();
                else if (header.Type == 1)
                    listNhanSu_new = listNhanSu.Where(x => x.DepartmentId == depatment.Id &&
                    (string.IsNullOrEmpty(x.LiteracyDetail) || x.LiteracyDetail.Contains(header.Id.ToString()))).ToList();
                else if (header.Type == 2)
                    listNhanSu_new = listNhanSu.Where(x => x.DepartmentId == depatment.Id && x.ContractTypeId == header.Id).ToList();
                child.SoLuong = listNhanSu_new.Count;
                child.listChildren = new List<ThongKeTongQuat>();
                int yearCurrent = DateTime.Today.Year;
                for (int i = 20; i < 66; i = i + 5)
                {
                    ThongKeTongQuat chiTiet = new ThongKeTongQuat();
                    chiTiet.Id = i;
                    chiTiet.Name = "Tuổi từ " + i + " đến " + (i + 4);
                    chiTiet.listChildren = new List<ThongKeTongQuat>()
                    {
                         new ThongKeTongQuat()
                         {
                             SoLuong = listNhanSu_new.Count(x => x.BirthDay != null && x.BirthDay.Value.Year + i >= yearCurrent && x.BirthDay.Value.Year + i < yearCurrent + 5)
                         },
                         new ThongKeTongQuat()
                         {
                             SoLuong = listNhanSu_new.Count(x => x.Gender == GenderEnum.Male && x.BirthDay != null && x.BirthDay.Value.Year + i >= yearCurrent && x.BirthDay.Value.Year + i < yearCurrent + 5)
                         },
                         new ThongKeTongQuat()
                         {
                            SoLuong = listNhanSu_new.Count(x => x.Gender == GenderEnum.Female && x.BirthDay != null && x.BirthDay.Value.Year + i >= yearCurrent && x.BirthDay.Value.Year + i < yearCurrent + 5)
                         },
                    };

                    child.listChildren.Add(chiTiet);
                }

                item.listChildren.Add(child);
            }
            listOut.Add(item);
        }

        ThongKeTongQuat itemTong = new();
        itemTong.Id = -1;
        itemTong.Name = "TỔNG CỘNG";
        itemTong.isBold = true;
        itemTong.listChildren = new List<ThongKeTongQuat>();

        foreach (var header in listHeader)
        {
            ThongKeTongQuat child = new();
            child.Name = header.Name;
            child.isBold = true;

            if (header.Id == -3)
                child.SoLuong = listNhanSu.Count();
            else if (header.Id == -1)
                child.SoLuong = listNhanSu.Where(x => x.Gender == GenderEnum.Male).Count();
            else if (header.Id == -2)
                child.SoLuong = listNhanSu.Where(x => x.Gender == GenderEnum.Female).Count();
            else if (header.Type == 1)
                child.SoLuong = listNhanSu.Where(x => string.IsNullOrEmpty(x.LiteracyDetail) || x.LiteracyDetail.Contains(header.Id.ToString())).Count();
            else if (header.Type == 2)
                child.SoLuong = listNhanSu.Where(x => x.ContractTypeId == header.Id).Count();
            itemTong.listChildren.Add(child);
        }
        listOut.Add(itemTong);

        return listOut;
    }

    public async Task UpdateSalarySocial(SalarySocial data)
    {
        if (data.Id == 0)
            await _context.SalarySocials.AddAsync(data);
        else
            _context.SalarySocials.Update(data);
        await _context.SaveChangesAsync();
    }

    public async Task<string> GetUserName()
    {
        try
        {
            Regex r = new Regex(@"^NV-[0-9]{6}");
            var users = await _context.Users.Where(x => x.Username.Length == 9).ToListAsync();

            string stt = users.Where(x => r.IsMatch(x.Username.ToUpper())).OrderByDescending(x => x.Username).FirstOrDefault()?.Username;
            if (stt is null)
                return "NV-000001";
            if (stt.Contains("-"))
                stt = stt.Split("-")[1];
            int order = int.Parse(stt) + 1;
            string userName = order.ToString();
            while (userName.Length < 6)
            {
                userName = "0" + userName;
            }
            return "NV-" + userName;
        }
        catch
        {
            return "NV-000001";
        }
    }

    public async Task<List<SalarySocial>> GetListSalarySocial()
    {
        return await _context.SalarySocials.OrderBy(x => x.Order).ToListAsync();
    }

    public async Task<SalarySocialDetailModel> GetSalarySocialById(int id, int year)
    {
        var item = await _context.SalarySocials.FindAsync(id);
        if (item is null)
            return new SalarySocialDetailModel();
        var model = _mapper.Map<SalarySocialDetailModel>(item);
        model.Debit = await _chartOfAccountV2Service.FindAccount(item.AccountDebit, string.Empty, year);
        model.Credit = await _chartOfAccountV2Service.FindAccount(item.AccountCredit, string.Empty, year);
        model.DebitFirst = await _chartOfAccountV2Service.FindAccount(item.DetailDebit1, item.AccountDebit, year);
        model.CreditFirst = await _chartOfAccountV2Service.FindAccount(item.DetailCredit1, item.AccountCredit, year);
        model.DebitSecond = await _chartOfAccountV2Service.FindAccount(item.DetailDebit2, item.AccountDebit + ":" + item.DetailDebit1, year);
        model.CreditSecond = await _chartOfAccountV2Service.FindAccount(item.DetailCredit2, item.AccountCredit + ":" + item.DetailCredit1, year);

        return model;
    }

    public async Task<string> ExportExcel(List<int> ids, int userId, List<string> roles, bool allowImages)
    {
        var users = await GetForExcel(ids, userId, roles);

        if (!users.Any())
        {
            return string.Empty;
        }

        string fileMapServer = $"ThongTinNhanVien_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
        string folder = Path.Combine(Directory.GetCurrentDirectory(), @"ExportHistory\\EXCEL");
        string pathSave = Path.Combine(folder, fileMapServer);

        string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Excel/ThongTinNhanVien.xlsx");

        var listDepartment = await _context.Departments.Where(x => !x.isDelete).ToListAsync();
        var listPosition = await _context.Positions.Where(x => !x.isDelete).ToListAsync();
        var listDegree = await _context.Degrees.Where(x => !x.IsDelete).ToListAsync();
        var listMajor = await _context.Majors.Where(x => !x.isDelete).ToListAsync();

        using (FileStream templateDocumentStream = File.OpenRead(path))
        {
            using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets["Sheet1"];
                var departmentSheet = package.Workbook.Worksheets.Add("Department");

                int rowDepartmentIdx = 2;

                foreach (var department in listDepartment)
                {
                    departmentSheet.Cells[rowDepartmentIdx, 2].Value = department.Name;
                    rowDepartmentIdx++;
                }

                sheet.DefaultColWidth = 10.0;
                sheet.DefaultRowHeight = 39.00D;
                int nRowBegin = 11, nCol = 31;
                int rowIdx = 11;

                // Location
                var provinceColIndex = 13;
                var districtColIndex = 14;
                var wardColIndex = 15;
                await _excelService.PrepareLocationRawSheetDataExcel(package, sheet, nRowBegin, users.Count, provinceColIndex, districtColIndex, wardColIndex, isMinusIndex: true);

                var literacyDetailId = new Regex("^[0-9]+");

                foreach (UserModel lo in users)
                {
                    sheet.Cells.Style.WrapText = true;
                    sheet.Cells[rowIdx, 1].Value = rowIdx - 10;
                    try
                    {
                        if (!string.IsNullOrEmpty(lo.Avatar) && allowImages)
                        {
                            var pathAvatar = Path.Combine(Directory.GetCurrentDirectory(), lo.Avatar);
                            ExcelPicture pic =
                                sheet.Drawings.AddPicture("Avatar" + rowIdx, new FileInfo(pathAvatar));
                            pic.SetPosition(rowIdx - 1, 10, 1, 28);
                            pic.SetSize(80, 80);
                            sheet.Row(rowIdx).Height = 75;
                        }
                    }
                    catch
                    {
                        sheet.Cells[rowIdx, 2].Value = "Hình đã bị xóa";
                    }

                    sheet.Cells[rowIdx, 3].Value = lo.Username;
                    sheet.Cells[rowIdx, 4].Value = lo.FullName;

                    var pos = listPosition.Find(x => x.Id == lo.PositionDetailId);
                    if (pos != null)
                        sheet.Cells[rowIdx, 5].Value = pos.Name;

                    var dep = listDepartment.Find(x => x.Id == lo.DepartmentId);
                    if (dep != null)
                        sheet.Cells[rowIdx, 6].Value = dep.Name;

                  // sheet.Cells[rowIdx, 6].DataValidation.AddListDataValidation().Formula.ExcelFormula = $"Department!B1:B{rowDepartmentIdx}";

                    GenderEnum gioiTinh = lo.Gender;
                    if (gioiTinh == GenderEnum.Male)
                    {
                        sheet.Cells[rowIdx, 7].Value = "x";
                    }
                    else if (gioiTinh == GenderEnum.Female)
                    {
                        sheet.Cells[rowIdx, 8].Value = "x";
                    }

                    sheet.Cells[rowIdx, 9].Value = lo.BirthDay.HasValue ? lo.BirthDay.Value.ToString("dd") : "";
                    sheet.Cells[rowIdx, 10].Value = lo.BirthDay.HasValue ? lo.BirthDay.Value.ToString("MM") : "";
                    sheet.Cells[rowIdx, 11].Value = lo.BirthDay.HasValue ? lo.BirthDay.Value.ToString("yyyy") : "";
                    sheet.Cells[rowIdx, 12].Value = lo.NativeAddressFull;
                    sheet.Cells[rowIdx, provinceColIndex].Value = lo.ProvinceName;
                    sheet.Cells[rowIdx, districtColIndex].Value = lo.DistrictName;
                    sheet.Cells[rowIdx, wardColIndex].Value = lo.WardName;
                    sheet.Cells[rowIdx, 16].Value = lo.Address;
                    sheet.Cells[rowIdx, 17].Value = lo.EthnicGroup;
                    sheet.Cells[rowIdx, 18].Value = lo.Religion;
                    int unionMember = lo.UnionMember;
                    if (unionMember == 0)
                    {
                        sheet.Cells[rowIdx, 19].Value = "x";
                    }
                    else if (unionMember == 1)
                    {
                        sheet.Cells[rowIdx, 20].Value = "x";
                    }

                    sheet.Cells[rowIdx, 21].Value = lo.Phone;
                    sheet.Cells[rowIdx, 22].Value = lo.SocialInsuranceCode;
                    sheet.Cells[rowIdx, 23].Value = lo.Identify;
                    sheet.Cells[rowIdx, 24].Value = lo.IdentifyCreatedDate.HasValue
                        ? lo.IdentifyCreatedDate.Value.ToString("dd/MM/yyyy")
                        : "";
                    sheet.Cells[rowIdx, 25].Value = lo.IdentifyCreatedPlace;
                    sheet.Cells[rowIdx, 26].Value = lo.Literacy;

                    if (!string.IsNullOrEmpty(lo.LiteracyDetail))
                    {
                        List<string> degrees = lo.LiteracyDetail.Split(',').ToList();
                        lo.LiteracyDetail = "";
                        degrees = degrees.Where(x => !string.IsNullOrEmpty(x) && literacyDetailId.IsMatch(x)).ToList();
                        foreach (string degree in degrees)
                        {
                            int degreeID = int.Parse(degree);
                            var degr = listDegree.Find(x => x.Id == degreeID);
                            if (degr != null)
                                lo.LiteracyDetail += degr.Name + ", ";
                        }
                    }

                    sheet.Cells[rowIdx, 27].Value = lo.LiteracyDetail;

                   // var degree_excel = sheet.Cells[rowIdx, 27].DataValidation.AddListDataValidation();
                    //foreach (var type in listDegree)
                    //{
                    //    degree_excel.Formula.Values.Add(type.Name);
                    //}

                    //var major_excel = sheet.Cells[rowIdx, 28].DataValidation.AddListDataValidation();
                    //foreach (var type in listMajor)
                    //{
                    //    major_excel.Formula.Values.Add(type.Name);
                    //}

                    sheet.Cells[rowIdx, 29].Value = lo.SendSalaryDate.HasValue
                        ? lo.SendSalaryDate.Value.ToString("dd/MM/yyyy")
                        : "";
                    sheet.Cells[rowIdx, 30].Value = lo.Salary;
                    sheet.Cells[rowIdx, 31].Value = lo.Note;

                    rowIdx++;
                }

                rowIdx--;
                if (!allowImages)
                {
                    sheet.DeleteColumn(2);
                    nCol--;
                }

                if (rowIdx >= nRowBegin)
                {
                    sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Right.Style =
                        OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Left.Style =
                        OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Top.Style =
                        OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Bottom.Style =
                        OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                    sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Right.Style =
                        OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Left.Style =
                        OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Top.Style =
                        OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Bottom.Style =
                        OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                using (FileStream fs = new FileStream(pathSave, FileMode.Create))
                {
                    package.SaveAs(fs);
                }

                return fileMapServer;
            }
        }
    }

    public async Task UpdateCurrentYear(int year, int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
            user.YearCurrent = year;
        _context.Users.Update(user);

        var yearSale = await _context.YearSales.FirstOrDefaultAsync(x => x.Year == year);
        if(yearSale is null)
        {
            yearSale = new YearSale
            {
                Year = year,
                Note = $"Add by {userId}"
            };
            await _context.AddAsync(yearSale);
        }
        await _context.SaveChangesAsync();

    }

    public async Task<List<int>> GetYearSales()
    {
        return await _context.YearSales.Select(x => x.Year).ToListAsync();
    }

    public async Task<object> GetUserStatistics(List<string> listRole, int userId)
    {
        var users = await GetListUserCommon(listRole, userId);

        var birthDayOfUsers = users
            .GroupBy(x => x.BirthDay.HasValue ? x.BirthDay.Value.Month : -1)
            .Select(x => new
            {
                Month = x.Key,
                Male = x.Count(s => s.Gender == GenderEnum.Male),
                Female = x.Count(s => s.Gender == GenderEnum.Female)
            })
            .ToDictionary(x => x.Month);
        List<object> distributeByMonth = new();

        for (int i = 1; i <= 12; i++)
        {
            distributeByMonth.Add(new
            {
                Month = i,
                Male = birthDayOfUsers.ContainsKey(i) ? birthDayOfUsers[i].Male : 0,
                Female = birthDayOfUsers.ContainsKey(i) ? birthDayOfUsers[i].Female : 0,
            });
        }

        var result = new
        {
            TotalUsers = await users.CountAsync(),
            TotalMale = await users.Where(x => x.Gender == GenderEnum.Male).CountAsync(),
            TotalFemale = await users.Where(x => x.Gender == GenderEnum.Female).CountAsync(),
            BirthDayOfUsers = distributeByMonth
        };
        return result;
    }

    private async Task<IQueryable<User>> GetListUserCommon(List<string> listRole, int userId)
    {
        var users = _context.Users.Where(x => !x.Quit && !x.IsDelete);
        var user = await _context.Users.FindAsync(userId);

        if (!listRole.Contains(UserRoleConst.SuperAdmin))
        {
            if (listRole.Contains(UserRoleConst.AdminBranch))
            {
                var roleAdmin = await _context.UserRoles.FirstOrDefaultAsync(x => x.Code == UserRoleConst.SuperAdmin);
                users = users.Where(x => (x.BranchId == user.BranchId || x.BranchId == 0 || x.BranchId == null)
                && (!("," + x.UserRoleIds + ",").Contains("," + roleAdmin.Id.ToString() + ",") || x.BranchId == user.BranchId)
                );
            }
            else if (listRole.Contains(UserRoleConst.TruongPhong))
            {
                users = users.Where(x => x.DepartmentId == user.DepartmentId);
            }
            else if (listRole.Contains(UserRoleConst.NhanVien))
            {
                users = users.Where(x => x.Id == user.Id);
            }
        }
        return users;
    }

    public async Task<IQueryable<User>> GetListUserCommon1(List<string> listRole, int userId)
    {
        var users = _context.Users.Where(x => !x.Quit && !x.IsDelete);
        var user = await _context.Users.FindAsync(userId);

        if (!listRole.Contains(UserRoleConst.SuperAdmin))
        {
            var roleAdmin = await _context.UserRoles.FirstOrDefaultAsync(x => x.Code == UserRoleConst.SuperAdmin);
            users = users.Where(x => (x.BranchId == user.BranchId || x.BranchId == 0 || x.BranchId == null)
            && (!("," + x.UserRoleIds + ",").Contains("," + roleAdmin.Id.ToString() + ",") || x.BranchId == user.BranchId)
            );
        }
        return users;
    }

    private async Task<IEnumerable<UserModel>> GetListUserCommon(int userId, List<string> roles)
    {
        var usersQueryable = from users in _context.Users
                             join district in _context.Districts
                                 on users.DistrictId equals district.Id into empDistrict
                             from district in empDistrict.DefaultIfEmpty()

                             join province in _context.Provinces
                                 on users.ProvinceId equals province.Id into empProvince
                             from province in empProvince.DefaultIfEmpty()

                             join ward in _context.Wards
                                 on users.WardId equals ward.Id into empWard
                             from ward in empWard.DefaultIfEmpty()

                             join districtNat in _context.Districts
                                 on users.NativeDistrictId equals districtNat.Id into empDistrictNat
                             from districtNat in empDistrictNat.DefaultIfEmpty()

                             join provinceNat in _context.Provinces
                                 on users.NativeProvinceId equals provinceNat.Id into empProvinceNat
                             from provinceNat in empProvinceNat.DefaultIfEmpty()

                             join wardNat in _context.Wards
                                 on users.NativeWardId equals wardNat.Id into empWardNat
                             from wardNat in empWardNat.DefaultIfEmpty()

                             join dep in _context.Departments
                                 on users.DepartmentId equals dep.Id into empDep
                             from empDepart in empDep.DefaultIfEmpty()
                             where !users.IsDelete

                             select new UserModel()
                             {
                                 Id = users.Id,
                                 Avatar = users.Avatar,
                                 FullName = users.FullName ?? "",
                                 Phone = users.Phone ?? "",
                                 Identify = users.Identify,
                                 IdentifyCreatedDate = users.IdentifyCreatedDate,
                                 IdentifyCreatedPlace = users.IdentifyCreatedPlace,
                                 IdentifyExpiredDate = users.IdentifyExpiredDate,
                                 Email = users.Email ?? "",
                                 Address = users.Address,
                                 BirthDay = users.BirthDay,
                                 Gender = users.Gender,
                                 WarehouseId = users.WarehouseId ?? 0,
                                 DepartmentId = users.DepartmentId ?? 0,
                                 RequestPassword = users.RequestPassword,
                                 Quit = users.Quit,
                                 TargetId = users.TargetId ?? 0,
                                 AddressFull = users.Address + ", " + ward.Name + ", " + district.Name + ", " + province.Name,
                                 NativeAddressFull = wardNat.Name + ", " + districtNat.Name + ", " + provinceNat.Name,
                                 DistrictId = users.DistrictId,
                                 ProvinceId = users.ProvinceId,
                                 WardId = users.WardId,
                                 NativeDistrictId = users.NativeDistrictId,
                                 NativeProvinceId = users.NativeProvinceId,
                                 NativeWardId = users.NativeWardId,

                                 Username = users.Username ?? "",
                                 Language = users.Language,
                                 Note = users.Note,
                                 UserRoleIds = users.UserRoleIds,
                                 Facebook = users.Facebook,
                                 Salary = users.Salary ?? 0,
                                 SendSalaryDate = users.SendSalaryDate,
                                 Status = users.Status,
                                 PlaceOfPermanent = users.PlaceOfPermanent,
                                 Religion = users.Religion,
                                 EthnicGroup = users.EthnicGroup,
                                 UnionMember = users.UnionMember ?? 0,
                                 Nation = users.Nation,
                                 Literacy = users.Literacy,
                                 LiteracyDetail = users.LiteracyDetail,
                                 BankAccount = users.BankAccount,
                                 Bank = users.Bank,
                                 NoOfLeaveDate = users.NoOfLeaveDate ?? 0,
                                 ShareHolderCode = users.ShareHolderCode,
                                 PersonalTaxCode = users.PersonalTaxCode,
                                 SocialInsuranceCreated = users.SocialInsuranceCreated,
                                 SocialInsuranceCode = users.SocialInsuranceCode,
                                 LastLogin = users.LastLogin,
                                 Timekeeper = users.Timekeeper ?? 0,
                                 LicensePlates = users.LicensePlates,
                                 ContractTypeId = users.ContractTypeId,
                                 DepartmentName = empDepart.Name,
                                 ProvinceName = province != null ? province.Name : string.Empty,
                                 DistrictName = district != null ? district.Name : string.Empty,
                                 WardName = ward != null ? ward.Name : string.Empty,
                             };

        var user = await _context.Users.FindAsync(userId);
        var userOuts = await usersQueryable.ToListAsync();
        if (user != null && !roles.Contains(UserRoleConst.SuperAdmin))
        {
            if (roles.Contains(UserRoleConst.AdminBranch))
            {
                var roleAdminId = await _context.UserRoles.Where(x => x.Code == UserRoleConst.SuperAdmin).Select(x => x.Id.ToString()).FirstOrDefaultAsync();
                userOuts = userOuts.Where(x => (x.BranchId == user.BranchId || x.BranchId == 0 || x.BranchId == null)
                && (!("," + x.UserRoleIds + ",").Contains("," + roleAdminId + ",") || x.BranchId == user.BranchId)
                ).ToList();
            }
            else if (roles.Contains(UserRoleConst.TruongPhong))
            {
                userOuts = userOuts.Where(x => x.DepartmentId == user.DepartmentId).ToList();
            }
            else if (roles.Contains(UserRoleConst.NhanVien))
            {
                userOuts = userOuts.Where(x => x.Id == user.Id).ToList();
            }
        }

        return userOuts;
    }

    public async Task<IEnumerable<object>> GetAllUserNotRole()
    {
        return await _context.Users.Where(x => string.IsNullOrEmpty(x.UserRoleIds)).Select(x => new
        {
            x.Id,
            x.FullName,
            x.Username
        }).ToListAsync();
    }

    public FileDetailModel UploadFile(IFormFile file, string folder, string fileNameUpload)
    {
        var fileDetail = _fileService.UploadFile(file, "UserFaceImages", file.FileName);
        // Detect any face in image before upload
        var absolutePath = fileDetail.FileUrl.GetAbsolutePath();

        var (msg, _) = _faceRecognitionService.FaceDetectAndValidate(absolutePath);
        if (!string.IsNullOrEmpty(msg))
        {
            _fileService.DeleteFileUpload(absolutePath);
            throw new Exception(msg);
        }

        return fileDetail;
    }

    public void UpdateContractor(ApplicationDbContext context, User user, UserModel userParam)
    {
        try
        {
            var contractorResult = _contractorService.GetContractorByUserId(user.Id);

            if (contractorResult.IsSuccess)
            {
                var contractor = contractorResult.Data;

                _contractorService.UpdateContractor(
                    new UpdateUserToContractorDto(
                        contractor.UserToContractorId,
                        userParam.ContractorDomain,
                        !userParam.IsContractor
                    )
                );
            }
            else
            {
                _contractorService.AddContractor(new UserToContractorDto
                {
                    UserToContractorId = Guid.NewGuid(),
                    Domain = userParam.ContractorDomain,
                    IsDeleted = !userParam.IsContractor,
                    UserId = user.Id
                });
            }

            context.SaveChanges();
        }
        catch (Exception ex)
        {
            Debug.Write(ex);
        }
    }

    public async Task<List<UserModel>> GetUsersByReminderAsync(ReminderModel model)
    {
        var query = _context.Users.AsQueryable();
        var result = new List<UserModel>();

        // 1. Gửi toàn bộ người dùng
        if (model.IsAllUsers)
        {
            return await query.Select(u => new UserModel
            {
                Id = u.Id,
                FullName = u.FullName,
                Note = u.Note
            }).ToListAsync();
        }

        // 2. Gửi theo danh sách user tham gia
        if (model.Participants?.Any() == true)
        {
            var userIds = model.Participants.Select(p => p.UserId).ToList();

            var users = await query
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new UserModel
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Note = u.Note
                }).ToListAsync();

            result.AddRange(users);
        }

        // 3. Gửi theo phòng ban nếu bật IsAllDepartment
        if (model.IsAllDepartment && model.DepartmentId.HasValue)
        {
            var users = await query
                .Where(u => u.DepartmentId == model.DepartmentId.Value)
                .Select(u => new UserModel
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Note = u.Note
                }).ToListAsync();

            result.AddRange(users);
        }

        // 4. Trả về danh sách không trùng người dùng
        return result.GroupBy(u => u.Id).Select(g => g.First()).ToList();
    }
    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync(); // Tùy context
    }

}
