using ManageEmployee.Entities.BaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.Entities
{
    public class PostAi : BaseEntity
    {
        public int Id { get; set; } // Khóa chính
        public string ImageUrl { get; set; } = ""; // Đường dẫn hình ảnh
        public string Status { get; set; } = ""; // Trạng thái bài viết
    }
}
