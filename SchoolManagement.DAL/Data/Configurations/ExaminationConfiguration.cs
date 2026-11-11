using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.DAL.Data.Configurations;

public class ExaminationConfiguration : IEntityTypeConfiguration<Examination>
{
    public void Configure(EntityTypeBuilder<Examination> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(150);

        builder.HasOne(e => e.AcademicSession)
            .WithMany(a => a.Examinations)
            .HasForeignKey(e => e.AcademicSessionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.ExamSubjects)
            .WithOne(es => es.Examination)
            .HasForeignKey(es => es.ExaminationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

