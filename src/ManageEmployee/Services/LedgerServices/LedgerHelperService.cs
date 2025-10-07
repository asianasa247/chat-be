using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities.Enumerations;
using Microsoft.EntityFrameworkCore;


public class LedgerHelperService : ILedgerHelperService
{
    private readonly ApplicationDbContext _context;
    private int _iLastOrderNumber;
    private AssetsType _eEntryType;
    private DateTime _dtEntryDate;
    private int _iSeed;

    public LedgerHelperService(ApplicationDbContext context)
    {
        _context = context;

        _iLastOrderNumber = 0;
        _eEntryType = AssetsType.KC;
        _dtEntryDate = DateTime.Now;
    }

    public void SetAutoIncrement(DateTime dtEntryDate, AssetsType eType, int isInternal, int year, int? iSeed = 1)
    {
        _dtEntryDate = dtEntryDate;
        _eEntryType = eType;
        _iSeed = iSeed ?? 1;
        _iLastOrderNumber = GetLastOrder(_eEntryType, isInternal, year);
    }

    private int GetLastOrder(AssetsType type, int isInternal, int year)
    {
        var query = _context.GetLedger(year, isInternal).Where(t => t.Type == type.ToString());
        if (query.Any())
            return query.Max(t => t.Order);
        return _iLastOrderNumber;
    }

    private int Increment()
    {
        _iLastOrderNumber = _iLastOrderNumber + _iSeed;
        return _iLastOrderNumber;
    }

    private int Decrement()
    {
        if (_iLastOrderNumber <= _iSeed)
            return _iLastOrderNumber;

        _iLastOrderNumber = _iLastOrderNumber + _iSeed;
        return _iLastOrderNumber;
    }


    public string OrginalVoucherNumber
    {
        get
        {
            Increment();
            string strPadNumber = _iLastOrderNumber.ToString().PadLeft(3, '0');
            string strYearFormat = _dtEntryDate.ToString("MM");
            string strEntryType = _eEntryType.ToString();

            string strCode = $"{strEntryType}{strYearFormat}-{DateTime.Now.Year.ToString().Substring(2, 2)}-{strPadNumber}";
            return strCode;
        }
    }

    public string VoucherNumber
    {
        get
        {
            string strYearFormat = _dtEntryDate.ToString("MM");
            string strEntryType = _eEntryType.ToString();

            string strCode = $"{strYearFormat}/{strEntryType}";
            return strCode;
        }
    }

    public async Task<int> GetOriginalVoucher(bool isNotInternal, string type, int year)
    {
        int maxOriginalVoucher = 0;
        var ledgerExist = await _context.GetLedger(year, isNotInternal ? 1 : 3).AsNoTracking().Where(x => !x.IsDelete && x.Type == type
                                                                && x.OrginalBookDate.Value.Year == DateTime.Today.Year && x.OrginalBookDate.Value.Month == DateTime.Today.Month).ToListAsync();
        if (ledgerExist.Any())
        {
            maxOriginalVoucher = ledgerExist.Max(x => Int32.Parse(x.OrginalVoucherNumber.Split('-').Last()));
        }
        maxOriginalVoucher++;
        return maxOriginalVoucher;
    }

}