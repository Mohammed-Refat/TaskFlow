using TaskFlow.Core.Entities;

namespace TaskFlow.Core.Interfaces;

public interface ITaskRepository
{
    Task<IEnumerable<TaskItem>> GetAllAsync(string userId, CancellationToken cancellationToken = default);
    Task<TaskItem?> GetByIdAsync(Guid id, string userId, CancellationToken cancellationToken = default);
    Task<TaskItem?> GetByIdTrackedAsync(Guid id, string userId, CancellationToken cancellationToken = default);
    Task<TaskItem> CreateAsync(TaskItem task, CancellationToken cancellationToken = default);
    Task<TaskItem> UpdateAsync(TaskItem task, CancellationToken cancellationToken = default);
    Task DeleteAsync(TaskItem task, CancellationToken cancellationToken = default);
}