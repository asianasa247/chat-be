using Common.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.RegularExpressions;
using ManageEmployee.Dal.DbContexts;
using Xceed.Document.NET;
using Xceed.Words.NET;
using ManageEmployee.Services.Interfaces.Users;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.UserEntites;
using ManageEmployee.Entities.CustomerEntities;

namespace ManageEmployee.Services.Contract;

public class UserContractService : IUserContractService
{
    private readonly ApplicationDbContext _context;
    private Dictionary<string, string> _replacePatterns = new Dictionary<string, string>();

    public UserContractService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> DownloadContract(int userId, int contractFileId)
    {
        var contractFile = await _context.ContractFiles.FindAsync(contractFileId);
        var contract = await _context.ContractTypes.FindAsync(contractFile.ContractTypeId);
        if (contract.TypeContract == TypeContractEnum.User)
            await InitData(userId);
        else if (contract.TypeContract == TypeContractEnum.Customer)
            await InitDataCustomer(userId);
        // Load a document.
        string _template = contractFile.LinkFile;
        var fileName = "Contrac_" + userId.ToString() + ".docx";
        string _templateOut = @"ExportHistory\Contract\" + fileName;
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), _template);
        string filePathOut = Path.Combine(Directory.GetCurrentDirectory(), _templateOut);

        string folder = Path.Combine(Directory.GetCurrentDirectory(), @"ExportHistory\\CONTRACT");

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        using (var document = DocX.Load(filePath))
        {
            // Check if some of the replace patterns are used in the loaded document.
            if (document.FindUniqueByPattern(@"<[\w \=]{4,}>", RegexOptions.IgnoreCase).Count > 0)
            {
                // Do the replacement of all the found tags and with green bold strings.
                var replaceTextOptions = new FunctionReplaceTextOptions()
                {
                    FindPattern = "<(.*?)>",
                    RegexMatchHandler = ReplaceFunc,
                    RegExOptions = RegexOptions.IgnoreCase,
                    NewFormatting = new Formatting() { Bold = true, FontColor = System.Drawing.Color.Green }
                };
                document.ReplaceText(replaceTextOptions);
            }
            // Save this document to disk.
            document.SaveAs(filePathOut);
        }

        // save history
        UserContractHistory userContractHistory = new UserContractHistory()
        {
            UserId = userId,
            ContractTypeId = contractFileId,
        };
        _context.UserContractHistories.Add(userContractHistory);
        await _context.SaveChangesAsync();

        return fileName;
    }

    private async Task InitData(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        var allowances = await _context.Allowances.Where(x => !x.IsDelete).ToListAsync();
        var allowanceUsers = await _context.AllowanceUsers.Where(x => !x.IsDelete && x.UserId == userId).ToListAsync();
        var allowanceUserString = new StringBuilder();
        foreach (var allowance in allowances)
        {
            var allowanceUser = allowanceUsers.Find(X => X.AllowanceId == allowance.Id);
            if (allowanceUser != null)
            {
                allowanceUserString.Append($"- {allowance.Name}: {String.Format("{0:N0}", allowanceUser.Value)} <br/>");
            }
        }
        var position = await _context.PositionDetails.FirstOrDefaultAsync(x => x.Id == user.PositionDetailId);
        _replacePatterns = new Dictionary<string, string>()
                        {
                            { "NgayHopDong", "Ngày " + DateTime.Now.Day + " tháng " +DateTime.Now.Month + " năm " + DateTime.Now.Year },
                            { "HoTenNhanVien", user.FullName },
                            { "NamSinhNhanVien", user.BirthDay?.ToString("dd/MM/yyyy") },
                            { "DiaChiNhanVien", user.Address},
                            { "CanCuocNhanVien", user.Identify },
                            { "NgayCanCuocNhanVien", user.IdentifyCreatedDate?.ToString("dd/MM/yyyy") },
                            { "NoiCapCanCuocNhanVien", user.IdentifyCreatedPlace },
                            { "ChucDanhNhanVien", position?.Name },
                            { "NgayBatDau", user.CreatedAt.ToString("dd/MM/yyyy")  },
                            { "MucLuongNhanVien",String.Format("{0:N2}", user.Salary)},
                            { "MucLuongNhanVienBangChu",AmountExtension.ConvertFromDecimal(user.Salary ?? 0)},
                            { "PhuCapNhanVien", allowanceUserString.ToString() },
                        };
    }

    private async Task InitDataCustomer(int userId)
    {
        var customer = await _context.Customers.FindAsync(userId);
        var customerTax = await _context.CustomerTaxInformations.FirstOrDefaultAsync(x=> x.CustomerId == userId);
        CustomerTaxInformationAccountant customerTaxAccount = new CustomerTaxInformationAccountant();
        if (customerTax != null)
            customerTaxAccount = await _context.CustomerTaxInformationAccountants.FirstOrDefaultAsync(x=> x.CustomerTaxInformationId == customerTax.Id);

        _replacePatterns = new Dictionary<string, string>()
                        {
                            { "TenKhachHang", customer.Name },
                            { "DiaChiKhachHang", customer.Address},
                            { "SoDienThoaiKH", customer.Phone},
                            { "MSTKhachHang", customerTax?.TaxCode },
                            { "TenNguoiDaiDienKH", customerTaxAccount?.Name},
                            { "ChucVuKH", customerTaxAccount?.Position },
                        };
    }

    private string ReplaceFunc(string findStr)
    {
        if (_replacePatterns.ContainsKey(findStr))
        {
            return _replacePatterns[findStr];
        }
        return findStr;
    }
}