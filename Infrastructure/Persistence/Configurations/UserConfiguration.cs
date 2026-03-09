using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UsersAPI.Domain.Entities;

namespace UsersAPI.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.OwnsOne(u => u.Email, e =>
        {
            e.Property(x => x.Address)
                .HasColumnName("Email")
                .IsRequired();
            e.HasIndex(x => x.Address).IsUnique();
        });

        builder.OwnsOne(u => u.Password, p =>
        {
            p.Property(x => x.Value)
                .HasColumnName("Password")
                .IsRequired()
                .HasMaxLength(256);
        });

        builder.OwnsOne(u => u.Role, r =>
        {
            r.Property(x => x.Value)
                .HasColumnName("Role")
                .HasMaxLength(20)
                .IsRequired();
        });
    }
}
