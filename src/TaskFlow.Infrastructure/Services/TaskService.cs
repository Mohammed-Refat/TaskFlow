using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Core.DTOs.Tasks;
using TaskFlow.Core.Entities;
using TaskFlow.Core.Enums;
using TaskFlow.Core.Interfaces;

namespace TaskFlow.Infrastructure.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }


        public async Task<IEnumerable<TaskResponse>> GetAllAsync(string userId, CancellationToken cancellationToken = default)
        {
            var tasks = await _taskRepository.GetAllAsync(userId, cancellationToken);
            return tasks.Select(MapToResponse);
        }


        public async Task<TaskResponse?> GetByIdAsync(Guid id, string userId, CancellationToken cancellationToken = default)
        {
            var task = await _taskRepository.GetByIdAsync(id, userId, cancellationToken);
            return task is null ? null : MapToResponse(task);
        }


        public async Task<TaskResponse> CreateAsync(string userId, CreateTaskRequest request, CancellationToken cancellationToken = default)
        {
            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                Priority = request.Priority,
                DueDate = request.DueDate,
                Status = TaskItemStatus.ToDo,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            var createdTask = await _taskRepository.CreateAsync(task, cancellationToken);
            return MapToResponse(createdTask);
        }

        public async Task<bool> DeleteAsync(Guid id, string userId, CancellationToken cancellationToken = default)
        {
            var task = await _taskRepository.GetByIdAsync(id, userId, cancellationToken);
            if (task is null)
                return false;

            await _taskRepository.DeleteAsync(task, cancellationToken);
            return true;
        }

    
        public async Task<TaskResponse?> UpdateAsync(Guid id, string userId, UpdateTaskRequest request, CancellationToken cancellationToken = default)
        {
            var task = await _taskRepository.GetByIdTrackedAsync(id, userId, cancellationToken);
            if (task is null)
                return null;

            if (request.Title is not null)
                task.Title = request.Title;

            if (request.Description is not null)
                task.Description = request.Description;

            if (request.Status is not null)
                task.Status = request.Status.Value;

            if (request.Priority is not null)
                task.Priority = request.Priority.Value;

            if (request.DueDate is not null)
                task.DueDate = request.DueDate;

            task.UpdatedAt = DateTime.UtcNow;

            var updated = await _taskRepository.UpdateAsync(task, cancellationToken);
            return MapToResponse(updated);
        }


        private static TaskResponse MapToResponse(TaskItem task) => new()
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            DueDate = task.DueDate,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };

    }
}