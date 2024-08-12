using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TasksManagementServer.Models;

public partial class TasksManagementDbContext : DbContext
{
    public AppUser? GetUser(string email)
    {
        return this.AppUsers.Where(u => u.UserEmail == email)
                            .Include(u => u.UserTasks)
                            .ThenInclude(t => t.TaskComments)
                            .FirstOrDefault();
    }
}

