using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TasksManagementServer.Models;

public partial class TaskComment
{
    [Key]
    public int CommentId { get; set; }

    public int? TaskId { get; set; }

    [StringLength(100)]
    public string Comment { get; set; } = null!;

    public DateOnly CommentDate { get; set; }

    [ForeignKey("TaskId")]
    [InverseProperty("TaskComments")]
    public virtual UserTask? Task { get; set; }
}
