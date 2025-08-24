namespace Cdm.Data.Models.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasIndex(u => u.UserEmail).IsUnique().HasDatabaseName("IX_Users_UserEmail");

        builder.Property(u => u.UserName)
               .IsRequired()
               .HasMaxLength(20);

        builder.Property(u => u.UserEmail)
               .IsRequired()
               .HasMaxLength(255);

        builder.Property(u => u.Password)
               .IsRequired();
    }
}
