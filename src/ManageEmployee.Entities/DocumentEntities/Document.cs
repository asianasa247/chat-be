using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.DocumentEntities;

public class Document : BaseEntity
{
    public int Id { get; set; }
    public int Stt { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? DebitCode { get; set; }
    public string? DebitCodeFirst { get; set; }
    public string? DebitCodeSecond { get; set; }
    public string? CreditCode { get; set; }
    public string? CreditCodeFirst { get; set; }
    public string? CreditCodeSecond { get; set; }
    public bool AllowDelete { get; set; }
    public bool Check { get; set; }
    public string? NameDebitCode { get; set; }
    public string? NameCreditCode { get; set; }
    public string? DebitCodeFirstName { get; set; }
    public string? DebitCodeSecondName { get; set; }
    public string? CreditCodeFirstName { get; set; }
    public string? CreditCodeSecondName { get; set; }
    public string? UserId { get; set; }
    public string? UserCode { get; set; }
    public string? UserFullName { get; set; }
    public string? Title { get; set; }
}