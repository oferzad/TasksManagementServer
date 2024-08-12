using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TasksManagementServer.Models;

[Index("UserEmail", Name = "UQ__AppUsers__08638DF8E8100940", IsUnique = true)]
public partial class AppUser
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string UserName { get; set; } = null!;

    [StringLength(50)]
    public string UserLastName { get; set; } = null!;

    [StringLength(50)]
    public string UserEmail { get; set; } = null!;

    [StringLength(50)]
    public string UserPassword { get; set; } = null!;

    public bool IsManager { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<UserTask> UserTasks { get; set; } = new List<UserTask>();
}
