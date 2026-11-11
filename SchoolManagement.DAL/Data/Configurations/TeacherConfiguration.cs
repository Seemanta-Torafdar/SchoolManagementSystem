using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.DAL.Data.Configurations;

public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
{
    public void Configure(EntityTypeBuilder<Teacher> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.TeacherIdNumber).IsRequired().HasMaxLength(30);
        builder.HasIndex(t => t.TeacherIdNumber).IsUnique();

        builder.Property(t => t.FirstName).IsRequired().HasMaxLength(50);
        builder.Property(t => t.LastName).IsRequired().HasMaxLength(50);
        builder.Property(t => t.Designation).IsRequired().HasMaxLength(100);
        builder.Property(t => t.Qualification).IsRequired().HasMaxLength(250);
        builder.Property(t => t.Phone).IsRequired().HasMaxLength(20);
        builder.Property(t => t.Email).IsRequired().HasMaxLength(100);

        builder.HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

