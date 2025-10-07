namespace ManageEmployee.DataTransferObject.LedgerModels
{
    public class LedgerProjectReporterModel
    {
        public string ProjectCode { get; set; }
        public DateTime? OrginalBookDate { get; set; }
        public string Type { get; set; }
        public int Month { get; set; }
        public DateTime? BookDate { get; set; }
        public string VoucherNumber { get; set; }
        public string OrginalDescription { get; set; }
        public string DebitCode { get; set; }
        public string DebitWarehouse { get; set; }
        public string DebitDetailCodeFirst { get; set; }
        public string DebitDetailCodeSecond { get; set; }
        public string CreditCode { get; set; }
        public string CreditWarehouse { get; set; }
        public string CreditDetailCodeFirst { get; set; }
        public string CreditDetailCodeSecond { get; set; }
        public double Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double Amount { get; set; }
        public string DebitCodeName { get; set; }
        public string DebitDetailCodeFirstName { get; set; }
        public string DebitDetailCodeSecondName { get; set; }
        public string CreditCodeName { get; set; }
        public string CreditDetailCodeFirstName { get; set; }
        public string CreditDetailCodeSecondName { get; set; }
        public string DebitWarehouseName { get; set; }
        public string CreditWarehouseName { get; set; }
    }
}