using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskFlow.Core.DTOs.Tasks;
using TaskFlow.Core.Interfaces;

namespace TaskFlow.Api.Controllers
{

    [ApiController]
    [Route("api/tasks")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var tasks = await _taskService.GetAllAsync(GetUserId(), cancellationToken);
            return Ok(tasks);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var task = await _taskService.GetByIdAsync(id, GetUserId(), cancellationToken);
            if (task is null)
                return NotFound();

            return Ok(task);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskRequest request, CancellationToken cancellationToken)
        {
            var createdTask = await _taskService.CreateAsync(GetUserId(), request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = createdTask.Id }, createdTask);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskRequest request, CancellationToken cancellationToken)
        {
            var updatedTask = await _taskService.UpdateAsync(id, GetUserId(), request, cancellationToken);
            if (updatedTask is null)
                return NotFound();
            return Ok(updatedTask);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var deleted = await _taskService.DeleteAsync(id, GetUserId(), cancellationToken);
            if (!deleted)
                return NotFound();
            return NoContent();
        }

        

    }
}
