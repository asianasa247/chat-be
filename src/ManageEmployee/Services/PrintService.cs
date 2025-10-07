using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PrintViewModelModels;
using ManageEmployee.Entities.PrintEntities;
using ManageEmployee.Services.Interfaces.Prints;

namespace ManageEmployee.Services;

public class PrintService : IPrintService
{
    private ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public PrintService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public IEnumerable<Print> GetAll()
    {
        return _context.Prints;
    }

    public PagePrintViewModel GetPagePrint()
    {
        var page = _context.PagePrints.FirstOrDefault();
        if(page is null)
            return new PagePrintViewModel();

        PagePrintViewModel itemOut = new PagePrintViewModel();
        itemOut.Width = page.Width;
        itemOut.Height = page.Height;
        itemOut.Barcode = _mapper.Map<PrintViewModel>(_context.Prints.FirstOrDefault(x => x.Name == "Barcode"));
        itemOut.QrCode = _mapper.Map<PrintViewModel>(_context.Prints.FirstOrDefault(x => x.Name == "QrCode"));
        return itemOut;
    }

    public Print Create(Print param)
    {
        _context.Prints.Add(param);
        _context.SaveChanges();

        return param;
    }

    public void Update(PagePrintViewModel param)
    {
        var page = _context.PagePrints.FirstOrDefault();
        page.Width = param.Width;
        param.Height = param.Height;
        var barCode = _context.Prints.FirstOrDefault(x => x.Name == "Barcode");
        if (barCode != null)
        {
            barCode.Width = param.Barcode.Width;
            barCode.Height = param.Barcode.Height;
            barCode.MarginLeft = param.Barcode.MarginLeft;
            barCode.MarginRight = param.Barcode.MarginRight;
            barCode.MarginTop = param.Barcode.MarginTop;
            barCode.MarginBottom = param.Barcode.MarginBottom;
            barCode.Size = param.Barcode.Size;
        }
        var qrCode = _context.Prints.FirstOrDefault(x => x.Name == "QrCode");
        if (qrCode != null)
        {
            qrCode.Width = param.QrCode.Width;
            qrCode.Height = param.QrCode.Height;
            qrCode.MarginLeft = param.QrCode.MarginLeft;
            qrCode.MarginRight = param.QrCode.MarginRight;
            qrCode.MarginTop = param.QrCode.MarginTop;
            qrCode.MarginBottom = param.QrCode.MarginBottom;
            qrCode.Size = param.QrCode.Size;
        }

        _context.Prints.Update(qrCode);
        _context.Prints.Update(barCode);
        _context.PagePrints.Update(page);
        _context.SaveChanges();
    }
}