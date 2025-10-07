using ManageEmployee.Entities.LotteryEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.DataTransferObject.SettingsSpinModels
{
    public class SettingsSpinDTO
    {
        public string? Code { get; set; }
        public string? Name { get; set; }

        public int IdCustomerClassification { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public DateTime TimeStartSpin { get; set; }
        public int TimeStartPerSpin { get; set; }
        public int TimeStopPerSpin { get; set; }
        public DateTime AwarDay { get; set; }
        public string? Note { get; set; }

        public SettingsSpin ConvertToSettingsSpin()
        {
            return new SettingsSpin()
            {
                Code = this.Code,
                Name = this.Name,
                IdCustomerClassification = this.IdCustomerClassification,
                TimeStart = this.TimeStart,
                TimeEnd = this.TimeEnd,
                TimeStartSpin = this.TimeStartSpin,
                TimeStartPerSpin = this.TimeStartPerSpin,
                TimeStopPerSpin = this.TimeStopPerSpin,
                AwarDay = this.AwarDay,
                Note = this.Note
            };
        }
    }
}
