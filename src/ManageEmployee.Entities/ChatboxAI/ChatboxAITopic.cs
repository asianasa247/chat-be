using ManageEmployee.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.ChatboxAI
{
    public class ChatboxAITopic : BaseEntity
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string TopicCode { get; set; } = "";

        [Required]
        public string TopicName { get; set; } = "";
    }
}
