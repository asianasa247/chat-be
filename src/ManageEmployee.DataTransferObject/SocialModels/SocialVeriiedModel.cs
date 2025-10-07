using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageEmployee.DataTransferObject.SocialModels
{
    public class SocialVerifiedModel
    {
        public string Email { get; set; }
        public bool? EmailVerified { get; set; }
        public string Name { get; set; }
        public string? GivenName { get; set; }
        public string? FamilyName { get; set; }
        public string Picture { get; set; }
        public string Locale { get; set; }
        public string Id { get; set; }
        public string PhotoUrl{ get; set; }
    }
}