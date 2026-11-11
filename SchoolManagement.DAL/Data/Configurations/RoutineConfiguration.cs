using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.DAL.Data.Configurations;

public class RoutineConfiguration : IEntityTypeConfiguration<ClassRoutine>
{
    public void Configure(EntityTypeBuilder<ClassRoutine> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.DayOfWeek).IsRequired().HasMaxLength(20);
        builder.Property(r => r.StartTime).IsRequired().HasMaxLength(20);
        builder.Property(r => r.EndTime).IsRequired().HasMaxLength(20);
        builder.Property(r => r.RoomNumber).HasMaxLength(30);

        builder.HasOne(r => r.SchoolClass)
            .WithMany(c => c.Routines)
            .HasForeignKey(r => r.SchoolClassId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Section)
            .WithMany(s => s.Routines)
            .HasForeignKey(r => r.SectionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Subject)
            .WithMany(sub => sub.Routines)
            .HasForeignKey(r => r.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Teacher)
            .WithMany(t => t.Routines)
            .HasForeignKey(r => r.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

