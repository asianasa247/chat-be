using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.DataTransferObject
{
    public class EmployeeByOrderModels
    {
        public int EmployeeId { get; set; } = 0;
        public string PosisionWorking { get; set; }
        public int OrderId { get; set; } = 0;
        public double Salary { get; set; }
        public string SalaryProduct { get; set; }
        public double TotalSalary { get; set; }
        public double TipMonney { get; set; }
        public string Note { get; set; }
    }
}
