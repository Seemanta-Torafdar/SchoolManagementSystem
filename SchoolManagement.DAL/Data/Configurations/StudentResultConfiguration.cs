using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.DAL.Data.Configurations;

public class StudentResultConfiguration : IEntityTypeConfiguration<StudentResult>
{
    public void Configure(EntityTypeBuilder<StudentResult> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.MarksObtained).HasPrecision(5, 2);
        builder.Property(r => r.MaxMarks).HasPrecision(5, 2);
        builder.Property(r => r.Grade).HasMaxLength(5);
        builder.Property(r => r.GradePoint).HasPrecision(4, 2);

        builder.HasIndex(r => new { r.StudentId, r.ExaminationId, r.SubjectId }).IsUnique();

        builder.HasOne(r => r.Student)
            .WithMany(s => s.Results)
            .HasForeignKey(r => r.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Examination)
            .WithMany(e => e.StudentResults)
            .HasForeignKey(r => r.ExaminationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Subject)
            .WithMany(s => s.StudentResults)
            .HasForeignKey(r => r.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

