using Hangfire;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Services.Interfaces.Mails;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities;

namespace ManageEmployee.Services;
public class SendMailService : ISendMailService
{
    private readonly ApplicationDbContext _context;
    private ILogger<SendMailService> _logger;
    public SendMailService(ApplicationDbContext context, ILogger<SendMailService> logger)
    {
        _context = context;
        _logger = logger;
    }
    public void SendEmail(SendMail request, string customerEmail)
    {
        try
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("kimjaewoongshop@gmail.com");
                mail.To.Add(customerEmail);
                mail.Subject = request.Title;
                mail.Body = request.Content;
                mail.IsBodyHtml = true;
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential("kimjaewoongshop@gmail.com", "guqomrdtbhhpuqlu");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email. To: {CustomerEmail}, Subject: {Subject}, Error: {ErrorMessage}", 
                customerEmail, 
                request.Title, 
                ex.Message);
        }
    }
    
    public async Task<IEnumerable<SendMail>> GetAll(PagingRequestModel request)
    {
        return await _context.SendMails.Where(x => x.Title == request.SearchText || string.IsNullOrEmpty(request.SearchText)).OrderByDescending(x => x.CreateAt).ToListAsync();
    }
    public async Task Create(SendMail request)
    {
        var customer = _context.Customers.Find(request.CustomerId);
        if (string.IsNullOrEmpty(customer.Email))
            throw new Exception("Khách hàng chưa có email");
        request.CreateSend = DateTime.Now;
        request.CreateAt = DateTime.Now;
        _context.SendMails.Add(request);
        await _context.SaveChangesAsync();
        BackgroundJob.Schedule(
       () => SendEmail(request, customer.Email), request.CreateSend ?? DateTime.Now);
    }
    public async Task Update(SendMail request)
    {
        _context.SendMails.Update(request);
        await _context.SaveChangesAsync();
    }
    public async Task Delete(int id)
    {
        var item = await _context.SendMails.FindAsync(id);
        _context.SendMails.Remove(item);
        await _context.SaveChangesAsync();
    }
}
