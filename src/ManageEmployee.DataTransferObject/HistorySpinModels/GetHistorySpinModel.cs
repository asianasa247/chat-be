using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.DataTransferObject.HistorySpinModels
{
    public class GetHistorySpinModel
    {
        public int HistoryId { get; set; }
        public SettingSpinInfo SettingsSpinInfo { get; set; }
        public PrizeInfo Prize { get; set; }
        public CustomerInfo Customer { get; set; }
        public GoodInfo Good { get; set; }
        public DateTime WinTime { get; set; }
        public DateTime ReceivedDay { get; set; }
        public string? Image { get; set; }
    }
    public struct SettingSpinInfo
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
    public struct PrizeInfo
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public struct CustomerInfo
    {
        public int Id { get; set; }
        public string Code { get; set; }    
        public string Name { get; set; }
    }
    public struct GoodInfo
    {
        public int Id { get; set; }
        public string Account { get; set; }
       
        public string AccountName { get; set; }
        
        public string Warehouse { get; set; }
        
        public string WarehouseName { get; set; }
        
        public string Detail1 { get; set; }
       
        public string DetailName1 { get; set; }
        
        public string Detail2 { get; set; }
        
        public string DetailName2 { get; set; }
    }
}
