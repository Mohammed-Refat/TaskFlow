using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Core.Enums;

namespace TaskFlow.Core.DTOs.Tasks
{
    public class UpdateTaskRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public TaskItemStatus? Status { get; set; }
        public TaskItemPriority? Priority { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
