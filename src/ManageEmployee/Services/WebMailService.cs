using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.WebEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Mails;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;
public class WebMailService : IWebMailService
{
    private readonly ApplicationDbContext _context;

    public WebMailService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task CreateMail(string email,string? phoneNumber,  string? note, int? userId)
    {
        var isExistEmail = await _context.WebMails.AnyAsync(x => x.Email == email);
        if (isExistEmail)
        {
            throw new ErrorException(ErrorMessages.EmailAlreadyExist);
        }

        var webEmail = new WebMail
        {
            Email = email,
            CreatedAt = DateTime.Now,
            CustomerId = userId,
            PhoneNumber = phoneNumber,
            Note = note
        };
        await _context.AddAsync(webEmail);
        await _context.SaveChangesAsync();
    }

    public async Task<PagingResult<WebMail>> GetPaging(PagingRequestModel param)
    {
        if (param.PageSize <= 0)
            param.PageSize = 20;

        if (param.Page < 0)
            param.Page = 0;

        var query = _context.WebMails.Where(x => string.IsNullOrEmpty(param.SearchText) || x.Email.Contains(param.SearchText));
        return new PagingResult<WebMail>()
        {
            CurrentPage = param.Page,
            PageSize = param.PageSize,
            Data = await query.Skip((param.Page) * param.PageSize).Take(param.PageSize).ToListAsync(),
            TotalItems = await query.CountAsync()
        };
    }
}
