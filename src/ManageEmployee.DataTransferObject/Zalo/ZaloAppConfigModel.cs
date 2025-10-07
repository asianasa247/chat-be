using ManageEmployee.Entities.BaseEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.DataTransferObject.Zalo
{
    public class ZaloAppConfigModel
    {
        public int Id { get; set; }
        public string AppName { get; set; }
        public string AppId { get; set; }
        public string AppSecret { get; set; } // App secret
        public string CallbackUrl { get; set; }// Url để nhận code từ zalo
        public string OauthCode { get; set; }// Code dùng để lấy access token
        public string AccessToken { get; set; }// Token dùng để gọi api
        public string RefreshToken { get; set; }// Token dùng để lấy lại access token
        public DateTime ExpiredAt { get; set; }//Thời gian hết hạn asscess token
    }
}
