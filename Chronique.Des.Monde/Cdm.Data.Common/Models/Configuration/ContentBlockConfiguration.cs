using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronique.Des.Mondes.Data.Models.Configuration;

public class ContentBlockConfiguration : IEntityTypeConfiguration<ContentBlock>
{
    public void Configure(EntityTypeBuilder<ContentBlock> builder)
    {
        // Primary key
        builder.HasKey(cb => cb.Id);

        // Indexes for performance
        builder.HasIndex(cb => cb.ChapterId)
               .HasDatabaseName("IX_ContentBlocks_ChapterId");

        builder.HasIndex(cb => cb.Type)
               .HasDatabaseName("IX_ContentBlocks_Type");

        builder.HasIndex(cb => cb.CharacterId)
               .HasDatabaseName("IX_ContentBlocks_CharacterId");

        builder.HasIndex(cb => new { cb.ChapterId, cb.Order })
               .HasDatabaseName("IX_ContentBlocks_ChapterId_Order")
               .IsUnique();

        // Foreign key relationships
        builder.HasOne(cb => cb.Chapter)
               .WithMany(c => c.ContentBlocks)
               .HasForeignKey(cb => cb.ChapterId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cb => cb.Character)
               .WithMany(c => c.ContentBlocks)
               .HasForeignKey(cb => cb.CharacterId)
               .OnDelete(DeleteBehavior.SetNull);

        // Constraints
        builder.Property(cb => cb.Type)
               .IsRequired()
               .HasMaxLength(20);

        builder.Property(cb => cb.Title)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(cb => cb.Content)
               .IsRequired();

        builder.Property(cb => cb.NpcMood)
               .HasMaxLength(20);

        builder.Property(cb => cb.CreatedDate)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");

        // Check constraints
        builder.HasCheckConstraint("CK_ContentBlocks_Type", 
            "[Type] IN ('Location', 'NpcDialogue', 'Description', 'Event')");

        // Complex check constraint for NPC dialogue validation
        builder.HasCheckConstraint("CK_ContentBlocks_NpcDialogue",
            "([Type] = 'NpcDialogue' AND [CharacterId] IS NOT NULL AND [NpcMood] IS NOT NULL) OR ([Type] != 'NpcDialogue' AND [CharacterId] IS NULL AND [NpcMood] IS NULL)");
    }
}