using ManageEmployee.DataTransferObject.FileModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.DataTransferObject.HistorySpinModels
{
    public class HistorySpinDTO
    {
        public int IdSettingsSpin { get; set; }
        public int PrizeId { get; set; }
        public int CustomerId { get; set; }
        public int GoodId { get; set; }
        public DateTime? WinTime { get; set; }
        public DateTime? ReceivedDay { get; set; }
        public List<IFormFile>? File { get; set; }
        public List<FileDetailModel>? UploadedFiles { get; set; }
    }
}
