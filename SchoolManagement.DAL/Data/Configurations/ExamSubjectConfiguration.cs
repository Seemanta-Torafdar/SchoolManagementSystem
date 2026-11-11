using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.DAL.Data.Configurations;

public class ExamSubjectConfiguration : IEntityTypeConfiguration<ExamSubject>
{
    public void Configure(EntityTypeBuilder<ExamSubject> builder)
    {
        builder.HasKey(es => es.Id);
        builder.Property(es => es.FullMarks).HasPrecision(5, 2);
        builder.Property(es => es.PassMarks).HasPrecision(5, 2);

        builder.HasOne(es => es.Examination)
            .WithMany(e => e.ExamSubjects)
            .HasForeignKey(es => es.ExaminationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(es => es.Subject)
            .WithMany(s => s.ExamSubjects)
            .HasForeignKey(es => es.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

