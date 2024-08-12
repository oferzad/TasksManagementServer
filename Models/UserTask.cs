using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TasksManagementServer.Models;

public partial class UserTask
{
    [Key]
    [Column("TaskID")]
    public int TaskId { get; set; }

    public int? UserId { get; set; }

    public int? UrgencyLevelId { get; set; }

    [StringLength(100)]
    public string TaskDescription { get; set; } = null!;

    public DateOnly TaskDueDate { get; set; }

    public DateOnly? TaskActualDate { get; set; }

    [InverseProperty("Task")]
    public virtual ICollection<TaskComment> TaskComments { get; set; } = new List<TaskComment>();

    [ForeignKey("UrgencyLevelId")]
    [InverseProperty("UserTasks")]
    public virtual UrgencyLevel? UrgencyLevel { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserTasks")]
    public virtual AppUser? User { get; set; }
}
