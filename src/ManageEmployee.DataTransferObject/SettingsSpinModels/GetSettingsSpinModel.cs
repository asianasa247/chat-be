using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.DataTransferObject.SettingsSpinModels
{
    public class GetSettingsSpinModel
    {
        public int SettingId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }

        public int IdCustomerClassification { get; set; }
        public string NameCustomerClassification { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public DateTime TimeStartSpin { get; set; }
        public int TimeStartPerSpin { get; set; }
        public int TimeStopPerSpin { get; set; }
        public DateTime AwarDay { get; set; }
        public string? Note { get; set; }
    }
}
