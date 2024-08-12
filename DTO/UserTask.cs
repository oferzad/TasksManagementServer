using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TasksManagementServer.Models;

namespace TasksManagementServer.DTO
{
    public class UserTask
    {
        public int TaskId { get; set; }

        public int? UserId { get; set; }

        public int? UrgencyLevelId { get; set; }

        public string TaskDescription { get; set; } = null!;

        public DateOnly TaskDueDate { get; set; }

        public DateOnly? TaskActualDate { get; set; }

        public virtual ICollection<TaskComment> TaskComments { get; set; } = new List<TaskComment>();

        public UserTask() { }
        public UserTask(Models.UserTask modelTask)
        {
            this.TaskId = modelTask.TaskId;
            this.UserId = modelTask.UserId;
            this.UrgencyLevelId = modelTask.UrgencyLevelId;
            this.TaskDescription = modelTask.TaskDescription;
            this.TaskDueDate = modelTask.TaskDueDate;
            this.TaskActualDate = modelTask.TaskActualDate;
            this.TaskComments = new List<TaskComment>();
            foreach (var comment in modelTask.TaskComments)
            {
                this.TaskComments.Add(new TaskComment(comment));
            }
        }
    }
}
