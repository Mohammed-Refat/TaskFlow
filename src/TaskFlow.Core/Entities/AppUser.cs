using Microsoft.AspNetCore.Identity;

namespace TaskFlow.Core.Entities;

public class AppUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property — one user has many tasks
    public ICollection<TaskItem> Tasks { get; set; } = [];
}