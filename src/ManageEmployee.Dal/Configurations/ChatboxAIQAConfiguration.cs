using ManageEmployee.Entities.ChatboxAI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManageEmployee.Dal.Configurations;

internal class ChatboxAIQAConfiguration : IEntityTypeConfiguration<ChatboxAIQA>
{
    public void Configure(EntityTypeBuilder<ChatboxAIQA> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Question).HasMaxLength(2048).IsRequired();
        builder.Property(p => p.Answer).HasMaxLength(4000).IsRequired();

        builder.HasIndex(p => new { p.TopicId, p.Question }).IsUnique();

        builder.HasOne(p => p.Topic)
               .WithMany() // nếu muốn, có thể thêm ICollection<ChatboxAIQA> ở Topic
               .HasForeignKey(p => p.TopicId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
