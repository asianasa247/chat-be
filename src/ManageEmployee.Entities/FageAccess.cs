using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.Entities
{
    public class FaceAccess
    {
        public Guid Id { get; set; }
        public string PageId { get; set; }
        public string PageAccessToken { get; set; }
        public bool IsActive { get; set; }
    }
}
