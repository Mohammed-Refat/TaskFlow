using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Core.Entities;
using TaskFlow.Core.Enums;

namespace TaskFlow.Infrastructure.Data.Configurations;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .HasMaxLength(2000);

        builder.Property(t => t.Status)
            .HasConversion<string>()  // store enum as string not int
            .HasDefaultValue(TaskItemStatus.ToDo);

        builder.Property(t => t.Priority)
            .HasConversion<string>()
            .HasDefaultValue(TaskItemPriority.Medium);

        builder.Property(t => t.IsDeleted)
            .HasDefaultValue(false);

        // Relationship: one AppUser has many TaskItems
        builder.HasOne(t => t.User)
            .WithMany(u => u.Tasks)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index on UserId — most queries filter by user
        builder.HasIndex(t => t.UserId);

        // Index on Status — filtering by status is common
        builder.HasIndex(t => t.Status);
    }
}