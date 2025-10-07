namespace ManageEmployee.Entities.CompanyEntities;

public class Company
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? MST { get; set; }
    public string? Email { get; set; }
    public string? Fax { get; set; }
    public string? WebsiteName { get; set; }
    public string? NameOfCEO { get; set; }
    public string? NoteOfCEO { get; set; }
    public string? NameOfChiefAccountant { get; set; }
    public string? NoteOfChiefAccountant { get; set; }
    public string? NameOfTreasurer { get; set; }
    public string? NameOfStorekeeper { get; set; }
    public string? NameOfChiefSupplier { get; set; }
    public string? NoteOfChiefSupplier { get; set; }
    public string? AssignPerson { get; set; }
    public double CharterCapital { get; set; } = 0;
    public string? FileOfBusinessRegistrationCertificate { get; set; }
    public DateTime SignDate { get; set; } = DateTime.Now;
    public string? FileLogo { get; set; }
    public int BusinessType { get; set; }
    public int AccordingAccountingRegime { get; set; }
    public int MethodCalcExportPrice { get; set; } = 0;//1: tai thoi diem xuat kho, 2:cuoi ki
    public int? UserUpdated { get; set; }
    public DateTime UpdateAt { get; set; } = DateTime.Now;
    public string? Note { get; set; }

    //
    public int Quantity { get; set; }
    public int UnitCost { get; set; }
    public int Money { get; set; }
    public int Currency { get; set; }
    public string DayType { get; set; } = "";
    public string DecimalUnit { get; set; } = "";
    public string DecimalRate { get; set; } = "";
    public string ThousandUnit { get; set; } = "";
    public bool IsShowBarCode { get; set; } = false;
    public string Defautl_themeweb { get; set; }
    public bool Signature {  get; set; }    
    public bool IsImage {  get; set; }
    public DateTime? SyncInvoiceAt { get; set; }
    public string? FacebookAppId { get; set; }
    public string? GoogleAppId { get; set; }

    public string? PasswordAccountTax { get; set; }


    public string? ClientID { get; set; }
    public string? ClientSecret { get; set; }
    public string? InvSerial { get; set; }
    public string? InvName { get; set; }

}