using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UsersAPI.Domain.UserAggregate;

namespace UsersAPI.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            builder.OwnsOne(u => u.Email, email =>
            {
                email.Property(e => e.Address)
                     .HasColumnName("Email")
                     .IsRequired();

                email.HasIndex(e => e.Address)
                   .IsUnique();
            });

            builder.Property(u => u.Balance)
                   .IsRequired()
                   .HasColumnType("decimal(10,2)")
                   .HasDefaultValue(0);

            builder.OwnsOne(u => u.Password, password =>
            {
                password.Property(p => p.Value)
                        .HasColumnName("Password")
                        .IsRequired()
                        .HasMaxLength(256);
            });

            builder.OwnsOne(u => u.Role, role =>
            {
                role.Property(r => r.Value)
                    .HasColumnName("Role")
                    .HasMaxLength(20)
                    .IsRequired();
            });

            builder.Property(u => u.Name)
                   .IsRequired()
                   .HasMaxLength(150);

        }
    }
}
