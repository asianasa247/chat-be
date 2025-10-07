using Common.Helpers;
using Emgu.CV;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.Email;
using ManageEmployee.Entities.Email;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;
using ManageEmployee.Helpers;

namespace ManageEmployee.JobSchedules
{
    public class SendMailCronJob : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public SendMailCronJob(IServiceScopeFactory _scopeFactory)
        {
            this._scopeFactory = _scopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var dn = DateTime.Now.Date;
                    var timer = db.MailTimers.Where(x => x.Status).ToList();
                    //mail config
                    var email = timer.Select(s => s.EmailSend).ToList();
                    var mailConfig = db.MailSends.Where(x => email.Contains(x.Email)).ToList();
                    //smtp
                    var smtpIds = mailConfig.Select(s => s.SmtpId).Concat(mailConfig.Select(s => s.ImapId)).ToList();
                    var smtps = db.Smtps.Where(x => smtpIds.Contains(x.Id)).ToList();
                    //template
                    var templateIds = timer.Select(s => s.MailTemplateId).ToList();
                    var templates = db.MailTemplates.Where(x => templateIds.Contains(x.Id)).ToList();
                    foreach (var item in timer)
                    {
                        switch (item.TypeRepeat)
                        {
                            case 0://One time
                                {
                                    if (item.LastSent == null || item.LastSent.Value == DateTime.MinValue)
                                    {
                                        var template = templates.FirstOrDefault(x => x.Id == item.MailTemplateId);
                                        var emailSend = mailConfig.FirstOrDefault(x => x.Email == item.EmailSend);
                                        var smtpSend = smtps.FirstOrDefault(x => x.Id == emailSend.SmtpId);
                                        if (template == null || emailSend == null || smtpSend == null) break;
                                        var mailto = item.MailTo.Split(',').Select(s => new MailInput
                                        {
                                            MailTo = s,
                                            Subject = template.Subject,
                                            Content = template.Content,
                                            DisplayName = emailSend.DisplayName,
                                            Email = emailSend.Email,
                                            MailBcc = item.MailBcc,
                                            MailCc = item.MailCc,
                                        });
                                        foreach (var mail in mailto)
                                        {
                                            var log = await SendingEmailAsync(mail, smtpSend, emailSend.Password);
                                            var _log = new MailLog
                                            {
                                                MailTo = log.MailTo,
                                                MailCc = log.MailCc,
                                                MailBcc = log.MailBcc,
                                                Content = log.Content,
                                                Success = log.Success,
                                                Status = log.Status,
                                                CreatedAt = DateTime.Now
                                            };
                                            db.MailLogs.Add(_log);
                                            db.Entry(_log).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                                        }
                                    }
                                    else
                                    {
                                        item.Status = false;
                                        item.IsRunning = false;
                                    }
                                    break;
                                }
                            case 1: //Daily
                                {
                                    if (item.Begin > dn.TimeOfDay && (item.LastSent == null || dn.Subtract(item.LastSent ?? dn).TotalDays > 1))
                                    {
                                        var template = templates.FirstOrDefault(x => x.Id == item.MailTemplateId);
                                        var emailSend = mailConfig.FirstOrDefault(x => x.Email == item.EmailSend);
                                        var smtpSend = smtps.FirstOrDefault(x => x.Id == emailSend.SmtpId);
                                        if (template == null || emailSend == null || smtpSend == null) break;
                                        var mailto = item.MailTo.Split(',').Select(s => new MailInput
                                        {
                                            MailTo = s,
                                            Subject = template.Subject,
                                            Content = template.Content,
                                            DisplayName = emailSend.DisplayName,
                                            Email = emailSend.Email,
                                            MailBcc = item.MailBcc,
                                            MailCc = item.MailCc,
                                        });
                                        foreach (var mail in mailto)
                                        {
                                            var log = await SendingEmailAsync(mail, smtpSend, emailSend.Password);
                                            if (log.Success)
                                            {
                                                item.LastSent = DateTime.Now;
                                            }
                                            else
                                            {
                                                if (item.LastSent != null)
                                                {
                                                    item.LastSent = item.LastSent.Value.AddMinutes(15);
                                                }
                                                else
                                                {
                                                    item.LastSent = DateTime.Now.AddDays(-1).AddMinutes(15);
                                                }
                                            }
                                            var _log = new MailLog
                                            {
                                                MailTo = log.MailTo,
                                                MailCc = log.MailCc,
                                                MailBcc = log.MailBcc,
                                                Content = log.Content,
                                                Success = log.Success,
                                                Status = log.Status,
                                                CreatedAt = DateTime.Now
                                            };
                                            db.MailLogs.Add(_log);
                                            db.Entry(_log).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                                        }
                                    }
                                    break;
                                }
                            case 2: //Weekly
                                {
                                    if (item.Begin > dn.TimeOfDay && (item.LastSent == null || dn.Subtract(item.LastSent ?? dn).TotalDays > 7))
                                    {
                                        var template = templates.FirstOrDefault(x => x.Id == item.MailTemplateId);
                                        var emailSend = mailConfig.FirstOrDefault(x => x.Email == item.EmailSend);
                                        var smtpSend = smtps.FirstOrDefault(x => x.Id == emailSend.SmtpId);
                                        if (template == null || emailSend == null || smtpSend == null) break;
                                        var mailto = item.MailTo.Split(',').Select(s => new MailInput
                                        {
                                            MailTo = s,
                                            Subject = template.Subject,
                                            Content = template.Content,
                                            DisplayName = emailSend.DisplayName,
                                            Email = emailSend.Email,
                                            MailBcc = item.MailBcc,
                                            MailCc = item.MailCc,
                                        });
                                        foreach (var mail in mailto)
                                        {
                                            var log = await SendingEmailAsync(mail, smtpSend, emailSend.Password);
                                            if (log.Success)
                                            {
                                                item.LastSent = DateTime.Now;
                                            }
                                            else
                                            {
                                                if (item.LastSent != null)
                                                {
                                                    item.LastSent = item.LastSent.Value.AddMinutes(15);
                                                }
                                                else
                                                {
                                                    item.LastSent = DateTime.Now.AddDays(-1).AddMinutes(15);
                                                }
                                            }
                                            var _log = new MailLog
                                            {
                                                MailTo = log.MailTo,
                                                MailCc = log.MailCc,
                                                MailBcc = log.MailBcc,
                                                Content = log.Content,
                                                Success = log.Success,
                                                Status = log.Status,
                                                CreatedAt = DateTime.Now
                                            };
                                            db.MailLogs.Add(_log);
                                            db.Entry(_log).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                                        }
                                    }
                                    break;
                                }
                            default:
                                break;
                        }
                        db.MailTimers.Add(item);
                        db.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    }
                    db.SaveChanges();
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
        }

        private async Task<MailLogModel> SendingEmailAsync(MailInput input, Smtp _smtp, string password)
        {
            var mailLog = new MailLogModel
            {
                MailTo = input.MailTo,
                MailCc = input.MailCc,
                MailBcc = input.MailBcc,
                Content = input.Subject + "|" + input.Content,
                Success = true,
                Status = true
            };
            try
            {
                MailMessage mailMessage = new MailMessage();
                string mailSend = input.Email;
                string smtp = _smtp.SmtpServer;
                string titleSend = input.DisplayName;
                int port = _smtp.Port;
                bool enableSsl = _smtp.EnableSsl;

                mailMessage.From = new MailAddress("\"" + titleSend + "\" " + mailSend);
                mailMessage.Subject = input.Subject;
                mailMessage.Body = input.Content;
                mailMessage.IsBodyHtml = true;
                mailMessage.To.Add(input.MailTo);
                if (!string.IsNullOrEmpty(input.MailCc))
                {
                    var cc = input.MailCc.Split(',');
                    foreach (var ccEmail in cc)
                    {
                        mailMessage.CC.Add(ccEmail);
                    }
                }
                if (!string.IsNullOrEmpty(input.MailBcc))
                {
                    var bcc = input.MailBcc.Split(',');
                    foreach (var bccEmail in bcc)
                    {
                        mailMessage.Bcc.Add(bccEmail);
                    }
                }
                SmtpClient SmtpServer = new SmtpClient(smtp);
                SmtpServer.Port = port;
                //SmtpServer.Credentials = new NetworkCredential(_config["MailConfig:Email"], _config["MailConfig:Password"]);
                //SmtpServer.EnableSsl = enableSsl;
                //SmtpServer.UseDefaultCredentials = false;
                //SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                //SmtpServer.UseDefaultCredentials = false; 
                SmtpServer.Credentials = new NetworkCredential(input.Email, password);
                SmtpServer.EnableSsl = enableSsl;
                SmtpServer.Send(mailMessage);
                //Save log
                //await _MailLogService.Create(mailLog);
                return mailLog;
            }
            catch (Exception ex)
            {
                var messess = ex.Message;
                if (ex.InnerException != null)
                {
                    messess = ex.InnerException.Message;
                }
                mailLog.Success = false;
                mailLog.Error = messess;
                //await _MailLogService.Create(mailLog);
                return mailLog;
            }
        }
    }
}
