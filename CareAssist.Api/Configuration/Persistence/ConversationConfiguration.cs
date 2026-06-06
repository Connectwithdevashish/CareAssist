using CareAssist.Api.Entities.Chat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareAssist.Api.Configuration.Persistence;

public sealed class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.UserId);

        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasOne(x => x.ApplicationUser)
            .WithMany()
            .HasForeignKey(x => x.UserId);

        builder.HasMany(x => x.Messages)
            .WithOne(x => x.Conversation)
            .HasForeignKey(x => x.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();
    }
}

