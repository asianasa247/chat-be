namespace ManageEmployee.Entities
{
    public class EmployeeByOrder
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; } = 0;
        public string PosisionWorking { get; set; }
        public int OrderId { get; set; } = 0;
        public double Salary { get; set; }
        public string SalaryProduct { get; set; }
        public double TotalSalary { get; set; }
        public double TipMonney { get; set; }
        public string Note { get; set; }
        public string ServiceCount { get; set; }
    }
}
