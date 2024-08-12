using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TasksManagementServer.Models;

public partial class UrgencyLevel
{
    [Key]
    public int UrgencyLevelId { get; set; }

    [StringLength(50)]
    public string UrgencyLevelName { get; set; } = null!;

    [InverseProperty("UrgencyLevel")]
    public virtual ICollection<UserTask> UserTasks { get; set; } = new List<UserTask>();
}
