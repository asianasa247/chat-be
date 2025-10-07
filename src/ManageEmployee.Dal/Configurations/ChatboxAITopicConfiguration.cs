using ManageEmployee.Entities.ChatboxAI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManageEmployee.Dal.Configurations;

internal class ChatboxAITopicConfiguration : IEntityTypeConfiguration<ChatboxAITopic>
{
    public void Configure(EntityTypeBuilder<ChatboxAITopic> builder)
    {
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => new { p.TopicCode }).IsUnique();

        builder.Property(p => p.TopicName)
            .HasMaxLength(256);
        builder.Property(p => p.TopicCode)
            .HasMaxLength(256);
    }
}
