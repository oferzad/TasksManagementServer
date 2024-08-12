using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TasksManagementServer.Models;

public partial class TasksManagementDbContext : DbContext
{
    public TasksManagementDbContext()
    {
    }

    public TasksManagementDbContext(DbContextOptions<TasksManagementDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AppUser> AppUsers { get; set; }

    public virtual DbSet<TaskComment> TaskComments { get; set; }

    public virtual DbSet<UrgencyLevel> UrgencyLevels { get; set; }

    public virtual DbSet<UserTask> UserTasks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server = (localdb)\\MSSQLLocalDB;Initial Catalog=TasksManagementDB;User ID=TaskAdminLogin;Password=kukuPassword;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AppUsers__3214EC07AC0B8EE5");
        });

        modelBuilder.Entity<TaskComment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__TaskComm__C3B4DFCA05B219BC");

            entity.Property(e => e.CommentDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskComments).HasConstraintName("FK__TaskComme__TaskI__2E1BDC42");
        });

        modelBuilder.Entity<UrgencyLevel>(entity =>
        {
            entity.HasKey(e => e.UrgencyLevelId).HasName("PK__UrgencyL__0CA733F9EE6A578A");

            entity.Property(e => e.UrgencyLevelId).ValueGeneratedNever();
        });

        modelBuilder.Entity<UserTask>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__UserTask__7C6949D1749A1D78");

            entity.HasOne(d => d.UrgencyLevel).WithMany(p => p.UserTasks).HasConstraintName("FK__UserTasks__Urgen__2B3F6F97");

            entity.HasOne(d => d.User).WithMany(p => p.UserTasks).HasConstraintName("FK__UserTasks__UserI__2A4B4B5E");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
