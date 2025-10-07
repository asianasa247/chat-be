using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.DataTransferObject.ConvertProductModels
{
    public class DTOConvert
    {
        public int Quantity { get; set; } //số lượng
        public int ConvertQuantity { get; set; } //số lượng quy đổi

        // 8 biến mã hàng
        [StringLength(36)]
        public string Account { get; set; }

        [StringLength(255)]
        public string AccountName { get; set; }

        [StringLength(36)]
        public string Warehouse { get; set; }

        [StringLength(255)]
        public string WarehouseName { get; set; }

        [StringLength(255)]
        public string Detail1 { get; set; }

        [StringLength(255)]
        public string DetailName1 { get; set; }

        [StringLength(255)]
        public string Detail2 { get; set; }

        [StringLength(255)]
        public string DetailName2 { get; set; }

        // 8 biến mã hàng đối diện
        [StringLength(36)]
        public string OppositeAccount { get; set; }

        [StringLength(255)]
        public string OppositeAccountName { get; set; }

        [StringLength(36)]
        public string OppositeWarehouse { get; set; }

        [StringLength(255)]
        public string OppositeWarehouseName { get; set; }

        [StringLength(255)]
        public string OppositeDetail1 { get; set; }

        [StringLength(255)]
        public string OppositeDetailName1 { get; set; }

        [StringLength(255)]
        public string OppositeDetail2 { get; set; }

        [StringLength(255)]
        public string OppositeDetailName2 { get; set; }
    }
}
