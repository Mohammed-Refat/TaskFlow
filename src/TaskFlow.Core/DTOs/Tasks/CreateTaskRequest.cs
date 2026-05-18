using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Core.Enums;

namespace TaskFlow.Core.DTOs.Tasks
{
    public class CreateTaskRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskItemPriority Priority { get; set; } = TaskItemPriority.Medium;
        public DateTime? DueDate { get; set; }

    }
}
