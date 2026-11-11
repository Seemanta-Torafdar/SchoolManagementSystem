using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.DAL.Data.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.AdmissionNumber).IsRequired().HasMaxLength(30);
        builder.HasIndex(s => s.AdmissionNumber).IsUnique();

        builder.Property(s => s.FirstName).IsRequired().HasMaxLength(50);
        builder.Property(s => s.LastName).IsRequired().HasMaxLength(50);
        builder.Property(s => s.GuardianPhone).IsRequired().HasMaxLength(20);
        builder.Property(s => s.VerificationCode).IsRequired().HasMaxLength(30);

        builder.HasIndex(s => new { s.SchoolClassId, s.SectionId, s.AcademicSessionId, s.RollNumber }).IsUnique();

        builder.HasOne(s => s.SchoolClass)
            .WithMany(c => c.Students)
            .HasForeignKey(s => s.SchoolClassId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Section)
            .WithMany(sec => sec.Students)
            .HasForeignKey(s => s.SectionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.AcademicSession)
            .WithMany(a => a.Students)
            .HasForeignKey(s => s.AcademicSessionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

