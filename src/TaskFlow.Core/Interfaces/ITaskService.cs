using TaskFlow.Core.DTOs.Tasks;

namespace TaskFlow.Core.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskResponse>> GetAllAsync(string userId, CancellationToken cancellationToken = default);
    Task<TaskResponse?> GetByIdAsync(Guid id, string userId, CancellationToken cancellationToken = default);
    Task<TaskResponse> CreateAsync(string userId, CreateTaskRequest request, CancellationToken cancellationToken = default);
    Task<TaskResponse?> UpdateAsync(Guid id, string userId, UpdateTaskRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, string userId, CancellationToken cancellationToken = default);
}