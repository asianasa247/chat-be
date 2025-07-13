namespace ChatappLC.Application.DTOs.Auth;

public class MailboxValidatorResponse
{
    public string EmailAddress { get; set; }
    public string Domain { get; set; }
    public string IsFree { get; set; }
    public string IsSyntax { get; set; }
    public string IsDomain { get; set; }
    public string IsSmtp { get; set; }
    public string IsVerified { get; set; }
    public string IsServerDown { get; set; }
    public string IsGreylisted { get; set; }
    public string IsDisposable { get; set; }
    public string IsSuppressed { get; set; }
    public string IsRole { get; set; }
    public string IsHighRisk { get; set; }
    public string IsCatchall { get; set; }
    public string MailboxvalidatorScore { get; set; }
    public string TimeTaken { get; set; }
    public string Status { get; set; }
    public string CreditsAvailable { get; set; }
    public string ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
}
