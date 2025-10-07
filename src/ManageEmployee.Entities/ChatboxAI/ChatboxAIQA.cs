using ManageEmployee.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManageEmployee.Entities.ChatboxAI
{
    public class ChatboxAIQA : BaseEntity
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int TopicId { get; set; }           // FK → ChatboxAITopic.Id

        [Required]
        [MaxLength(2048)]
        public string Question { get; set; } = "";

        [Required]
        [MaxLength(4000)]
        public string Answer { get; set; } = "";

        [ForeignKey(nameof(TopicId))]
        public ChatboxAITopic? Topic { get; set; }
    }
}
