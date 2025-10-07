using ManageEmployee.Entities.ChatboxAI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManageEmployee.Dal.Configurations;

public class ChatboxAIScheduledMessageConfiguration : IEntityTypeConfiguration<ChatboxAIScheduledMessage>
{
    public void Configure(EntityTypeBuilder<ChatboxAIScheduledMessage> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Message).HasMaxLength(4000).IsRequired();
        builder.Property(p => p.DaysOfWeek).HasMaxLength(64);

        builder.HasIndex(p => new { p.TopicId, p.SendTime });

        builder.HasOne(p => p.Topic)
               .WithMany()
               .HasForeignKey(p => p.TopicId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
