using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.Entities
{
    public class ProductOfEmployee
    {
        public int Id { get; set; }
        public int GoodId { get; set; }
        public int EmployeeId { get; set; }
        public int CommissionId { get; set; }
        public double Discount { get; set; }
        public string detail1 { get; set; }
        public string detailName1 { get; set; }
        public string detail2 { get; set; }
        public string detailName2 { get; set; }
        public string account { get; set; }
        public string accountName { get; set; }
    }
}
