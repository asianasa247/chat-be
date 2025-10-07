
using Common.Extensions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Gmail.v1;
using MailKit.Net.Imap;
using MailKit.Security;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.Email;
using ManageEmployee.Entities.Email;
using ManageEmployee.Helpers;
using ManageEmployee.Models;
using ManageEmployee.Services.Interfaces.Email;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;

namespace ManageEmployee.Controllers.EmailControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailSendController : ControllerBase
    {
        IMailSendService _MailSendService;
        IMailTemplateService _MailTemplateService;
        ISmtpService _SmtpService;
        IMailLogService _MailLogService;
        private IConfiguration config;
        private static readonly string[] Scopes = { GmailService.Scope.GmailReadonly, GmailService.Scope.GmailSend };
        private readonly GmailApiConfig _gmailApiConfig;
        public MailSendController(IMailSendService MailSendService, IMailTemplateService mailTemplateService, ISmtpService smtpService, IMailLogService mailLogService, IConfiguration config, IOptions<GmailApiConfig> gmailApiConfig)
        {
            _MailSendService = MailSendService;
            _MailTemplateService = mailTemplateService;
            _SmtpService = smtpService;
            _MailLogService = mailLogService;
            this.config = config;
            _gmailApiConfig = gmailApiConfig.Value;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            string roles = "";
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                roles = identity.FindFirst(x => x.Type == "RoleName").Value.ToString();
                userId = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
            }
            List<string> listRole = JsonConvert.DeserializeObject<List<string>>(roles);
            var result = await _MailSendService.GetAll();
            foreach (var item in result)
            {
                item.Password = "";
            }
            return Ok(new BaseResponseModel
            {
                Data = result
            });
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> GetMailConfigSmtp()
        {
            var mails = await _MailSendService.GetAll();
            var smtpIds = mails.Select(x => x.SmtpId).ToList();
            var smtps = await _SmtpService.GetByListId(smtpIds);
            var result = mails.Select(x =>
            {
                var smtp = smtps.FirstOrDefault(z => z.Id == x.SmtpId);
                //convert password to ****
                var pass = x.Password.Length.ToString().PadLeft(x.Password.Length, '*');
                var model = new MailSendSmtpModel
                {
                    Id = x.Id,
                    Email = x.Email,
                    DisplayName = x.DisplayName,
                    Password = pass,
                    SmtpId = x.SmtpId,
                    Smtp = new DataTransferObject.Smtp.SmtpModel
                    {
                        Id = smtp.Id,
                        SmtpServer = smtp.SmtpServer,
                        Port = smtp.Port,
                        Ssl = smtp.Ssl,
                        UseDefaultCredentials = smtp.UseDefaultCredentials,
                        EnableSsl = smtp.EnableSsl,
                        EnableTls = smtp.EnableTls,
                        RequiresAuthentication = smtp.RequiresAuthentication
                    }
                };
                return model;
            });
            return Ok(new BaseResponseModel
            {
                Data = result
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _MailSendService.GetById(id);
            result.Password = "";
            return Ok(new BaseResponseModel
            {
                Data = result
            });
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MailSendModel model)
        {
            var result = await _MailSendService.Create(model);
            return Ok(new BaseResponseModel
            {
                Data = result
            });
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] MailSendModel model)
        {
            var result = await _MailSendService.Update(model);
            return Ok(new BaseResponseModel
            {
                Data = result
            });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _MailSendService.Delete(id);
            return Ok(new BaseResponseModel
            {
                Data = result
            });
        }
        #region SendMail
        [HttpPost("SendMail")]
        public async Task<IActionResult> SendMailTemplate([FromBody] MailSendByTemplate model)
        {
            var email = await _MailSendService.GetByEmail(model.Email);
            if (email == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new BaseResponseModel
                {
                    Data = "Email not found"
                });
            }
            var smtp = await _SmtpService.GetById(email.SmtpId);
            var template = await _MailTemplateService.GetById(model.TemplateId);
            if (template == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new BaseResponseModel
                {
                    Data = "Template not found"
                });
            }
            var mailto = model.MailTo.Split(',').Select(s => new MailInput
            {
                MailTo = s,
                Subject = template.Subject,
                Content = template.Content,
                DisplayName = email.DisplayName,
                Email = email.Email,
                MailBcc = "",
                MailCc = "",
            });
            foreach (var item in mailto)
            {
                var result = await SendingEmailAsync(item, smtp, email.Password);
                if (!result)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponseModel
                    {
                        Data = "Send mail fail"
                    });
                }
            }
            return Ok(new BaseResponseModel
            {
                Data = mailto
            });
        }
        [HttpPost("FetchMail")]
        public async Task<IActionResult> FetchMail([FromBody] MailBoxReq model)
        {
            var emailConfig = await _MailSendService.GetByEmail(model.Email);
            if (emailConfig == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new BaseResponseModel
                {
                    Data = "Email not found"
                });
            }
            if (emailConfig.ImapId == 0)
            {
                return StatusCode(StatusCodes.Status404NotFound, new BaseResponseModel
                {
                    Data = "Imap not found"
                });
            }
            var imap = await _SmtpService.GetById(emailConfig.ImapId);
            if (imap == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new BaseResponseModel
                {
                    Data = "Imap not found"
                });
            }
            var lsmineMail = new List<MailBoxRes>();
            using (var client = new ImapClient())
            {
                client.Connect(imap.SmtpServer, imap.Port, SecureSocketOptions.SslOnConnect);
                client.Authenticate(emailConfig.Email, emailConfig.Password);
                var inbox = client.Inbox;
                inbox.Open(MailKit.FolderAccess.ReadOnly);
                //Xác định chỉ mục email mới nhất(Inbox.Count - 1)
                int maxIndex = inbox.Count - 1 - ((model.PageNo - 1) * model.ItemNo);

                //Xác định chỉ mục bắt đầu(tránh âm)
                int minIndex = Math.Max(maxIndex - (model.PageNo * model.ItemNo) + 1, 0);
                for (int i = maxIndex; i >= minIndex; i--)
                {
                    var message = inbox.GetMessage(i);
                    lsmineMail.Add(new MailBoxRes
                    {
                        Content = message.HtmlBody,
                        Date = message.Date.DateTime,
                        From = message.From.ToString(),
                        Subject = message.Subject,
                        To = message.To.ToString(),
                        TextContent = message.HtmlBody.StripHtml() ?? "",
                    });
                }
                client.Disconnect(true);
                return Ok(new BaseResponseModel
                {
                    Data = lsmineMail,
                    CurrentPage = model.PageNo,
                    DataTotal = lsmineMail.Count,
                    NextStt = minIndex,
                    PageSize = model.ItemNo,
                    TotalItems = inbox.Count
                });
            }
        }
        //[HttpPost("FetchMail")]
        //public async Task<IActionResult> FetchMail([FromBody] MailBoxReq model)
        //{
        //    var emailConfig = await _MailSendService.GetByEmail(model.Email);
        //    if (emailConfig == null || emailConfig.ImapId == 0)
        //        return NotFound(new BaseResponseModel { Data = "Email or IMAP not found" });

        //    var imap = await _SmtpService.GetById(emailConfig.ImapId);
        //    if (imap == null)
        //        return NotFound(new BaseResponseModel { Data = "IMAP config not found" });

        //    var result = new List<MailBoxRes>();

        //    using (var client = new ImapClient())
        //    {
        //        await client.ConnectAsync(imap.SmtpServer, imap.Port, SecureSocketOptions.SslOnConnect);
        //        await client.AuthenticateAsync(emailConfig.Email, emailConfig.Password);

        //        var inbox = client.Inbox;
        //        await inbox.OpenAsync(FolderAccess.ReadOnly);

        //        // Lọc các mail trong 3 tháng gần nhất
        //        var query = SearchQuery.DeliveredAfter(DateTime.UtcNow.AddMonths(-3));
        //        var uids = await inbox.SearchAsync(query);

        //        var totalItems = uids.Count;
        //        if (totalItems == 0)
        //        {
        //            await client.DisconnectAsync(true);
        //            return Ok(new BaseResponseModel
        //            {
        //                Data = result,
        //                CurrentPage = model.PageNo,
        //                PageSize = model.ItemNo,
        //                TotalItems = 0
        //            });
        //        }

        //        // Phân trang
        //        int skip = (model.PageNo - 1) * model.ItemNo;
        //        var pageUids = uids.Reverse().Skip(skip).Take(model.ItemNo).ToList();

        //        foreach (var uid in pageUids)
        //        {
        //            var message = await inbox.GetMessageAsync(uid);
        //            result.Add(new MailBoxRes
        //            {
        //                Subject = message.Subject,
        //                From = message.From?.ToString(),
        //                To = message.To?.ToString(),
        //                Date = message.Date.DateTime,
        //                Content = message.HtmlBody ?? "",
        //                TextContent = message.TextBody ?? message.HtmlBody?.StripHtml() ?? "",
        //                Uid = uid.Id
        //            });
        //        }

        //        await client.DisconnectAsync(true);

        //        return Ok(new BaseResponseModel
        //        {
        //            Data = result,
        //            CurrentPage = model.PageNo,
        //            PageSize = model.ItemNo,
        //            TotalItems = totalItems
        //        });
        //    }
        //}


        #region FetchMailDetail
        //[HttpPost("FetchMail")]
        //public async Task<IActionResult> FetchMail([FromBody] MailBoxReq model)
        //{
        //    var emailConfig = await _MailSendService.GetByEmail(model.Email);
        //    if (emailConfig == null || emailConfig.ImapId == 0)
        //        return NotFound(new BaseResponseModel { Data = "Email or IMAP not found" });

        //    var imap = await _SmtpService.GetById(emailConfig.ImapId);
        //    if (imap == null)
        //        return NotFound(new BaseResponseModel { Data = "IMAP config not found" });

        //    var result = new List<MailBoxRes>();

        //    using (var client = new ImapClient())
        //    {
        //        await client.ConnectAsync(imap.SmtpServer, imap.Port, SecureSocketOptions.SslOnConnect);
        //        await client.AuthenticateAsync(emailConfig.Email, emailConfig.Password);

        //        var inbox = client.Inbox;
        //        await inbox.OpenAsync(FolderAccess.ReadOnly);

        //        // Lọc các mail trong 3 tháng gần nhất
        //        var query = SearchQuery.DeliveredAfter(DateTime.UtcNow.AddMonths(-3));
        //        var uids = await inbox.SearchAsync(query);

        //        // Tổng số mail khớp
        //        var totalItems = uids.Count;
        //        if (totalItems == 0)
        //        {
        //            await client.DisconnectAsync(true);
        //            return Ok(new BaseResponseModel
        //            {
        //                Data = result,
        //                CurrentPage = model.PageNo,
        //                PageSize = model.ItemNo,
        //                TotalItems = 0
        //            });
        //        }

        //        // Tính chỉ mục phân trang
        //        int skip = (model.PageNo - 1) * model.ItemNo;
        //        var pageUids = uids.Reverse().Skip(skip).Take(model.ItemNo).ToList();

        //        // Dùng Fetch chỉ lấy metadata
        //        var summaries = await inbox.FetchAsync(pageUids, MessageSummaryItems.Envelope);

        //        foreach (var summary in summaries)
        //        {
        //            result.Add(new MailBoxRes
        //            {
        //                Subject = summary.Envelope?.Subject,
        //                From = summary.Envelope?.From?.ToString(),
        //                To = summary.Envelope?.To?.ToString(),
        //                Date = summary.Envelope?.Date?.DateTime ?? DateTime.MinValue,
        //                TextContent = "[Click để xem nội dung]", // Tùy chọn: hoặc để rỗng
        //                Uid = summary.UniqueId.Id
        //            });
        //        }

        //        await client.DisconnectAsync(true);

        //        return Ok(new BaseResponseModel
        //        {
        //            Data = result,
        //            CurrentPage = model.PageNo,
        //            PageSize = model.ItemNo,
        //            TotalItems = totalItems
        //        });
        //    }
        //}
        //[HttpPost("FetchMailDetail")]
        //public async Task<IActionResult> FetchMailDetail([FromBody] MailDetailReq model)
        //{
        //    var emailConfig = await _MailSendService.GetByEmail(model.Email);
        //    if (emailConfig == null || emailConfig.ImapId == 0)
        //        return NotFound(new BaseResponseModel { Data = "Email or IMAP not found" });

        //    var imap = await _SmtpService.GetById(emailConfig.ImapId);
        //    if (imap == null)
        //        return NotFound(new BaseResponseModel { Data = "IMAP config not found" });

        //    using (var client = new ImapClient())
        //    {
        //        await client.ConnectAsync(imap.SmtpServer, imap.Port, SecureSocketOptions.SslOnConnect);
        //        await client.AuthenticateAsync(emailConfig.Email, emailConfig.Password);

        //        var inbox = client.Inbox;
        //        await inbox.OpenAsync(FolderAccess.ReadOnly);

        //        // Convert System.Xml.UniqueId to MailKit.UniqueId
        //        if (!MailKit.UniqueId.TryParse(model.Uid.ToString(), out var mailKitUid))
        //        {
        //            return BadRequest(new BaseResponseModel { Data = "Invalid UID format" });
        //        }

        //        var message = await inbox.GetMessageAsync(mailKitUid);

        //        await client.DisconnectAsync(true);

        //        return Ok(new BaseResponseModel
        //        {
        //            Data = new
        //            {
        //                Content = message.HtmlBody ?? "",
        //                TextContent = message.TextBody ?? "",
        //                Subject = message.Subject ?? ""
        //            }
        //        });
        //    }
        //}
        #endregion

        [HttpGet("GetAuthorize")]
        public async Task<IActionResult> GetAuthorize()
        {
            // Lấy thông tin client từ file credentials hoặc khai báo trực tiếp
            var clientSecrets = new ClientSecrets
            {
                ClientId = _gmailApiConfig.web.client_id,       // Thay bằng Client ID từ credentials.json
                ClientSecret = _gmailApiConfig.web.client_secret  // Thay bằng Client Secret từ credentials.json
            };

            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = clientSecrets,
                Scopes = Scopes,
            });

            // Tạo URL đăng nhập (authorization URL)
            var authUrl = flow.CreateAuthorizationCodeRequest(_gmailApiConfig.web.redirect_uris.FirstOrDefault()).Build().AbsoluteUri;

            return Ok(authUrl);
        }
        [HttpGet("Callback")]
        public async Task<IActionResult> Callback(string code)
        {
            return Ok(code);
        }
        [HttpPost("SendMailSmtp")]
        public async Task<IActionResult> SendMailSmtp([FromBody] SendMailBody model)
        {
            var emailConfig = await _MailSendService.GetByEmail(model.Email);
            if (emailConfig == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new BaseResponseModel
                {
                    Data = "Email not found"
                });
            }
            var smtp = await _SmtpService.GetById(emailConfig.SmtpId);
            var mailto = new MailInput
            {
                MailTo = model.MailTo,
                Subject = model.Subject,
                Content = model.Content,
                DisplayName = emailConfig.DisplayName,
                Email = emailConfig.Email,
                MailBcc = model.MailBcc,
                MailCc = model.MailCc,
            };
            var result = await SendingEmailAsync(mailto, smtp, emailConfig.Password);
            if (!result)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponseModel
                {
                    Data = "Send mail fail"
                });
            }
            return Ok(new BaseResponseModel
            {
                Data = mailto
            });
        }
        #endregion
        #region Mail Helper

        private async Task<bool> SendingEmailAsync(MailInput input, Smtp _smtp, string password)
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
                await _MailLogService.Create(mailLog);
                return true;
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
                await _MailLogService.Create(mailLog);
                return false;
            }
        }
        #endregion
    }
}
