using ManageEmployee.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManageEmployee.Entities.ChatboxAI
{
    public class ChatboxAIScheduledMessage : BaseEntity
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int TopicId { get; set; }                 // FK → ChatboxAITopic.Id

        [Required]
        [MaxLength(4000)]
        public string Message { get; set; } = "";

        /// <summary>Khung giờ gửi trong ngày (ví dụ 22:00:00)</summary>
        [Required]
        public TimeSpan SendTime { get; set; }

        /// <summary>"Mon,Tue,Wed" | "Daily" | null = gửi mỗi ngày</summary>
        [MaxLength(64)]
        public string? DaysOfWeek { get; set; }

        public DateTime? LastSentAt { get; set; }

        [ForeignKey(nameof(TopicId))]
        public ChatboxAITopic? Topic { get; set; }
    }
}
