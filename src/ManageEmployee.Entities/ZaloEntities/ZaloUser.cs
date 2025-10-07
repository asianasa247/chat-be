using ManageEmployee.Entities.BaseEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace ManageEmployee.Entities.ZaloEntities
{
    public class ZaloUser : BaseEntity
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public long AppId { get; set; }
        public long UserId { get; set; }
        public long UserIdByApp { get; set; }
        public string UserExternalId { get; set; }
        public string DisplayName { get; set; }
        public string UserAlias { get; set; }
        public string IsSensitive { get; set; }
        public string UserLastInteractionDate { get; set; }
        public bool UserIsFollower { get; set; }
        public string Avatar { get; set; }
        public string Avatars { get; set; }
        public string TagsAndNotesInfo { get; set; }
        public string SharedInfo { get; set; }

    }
}
