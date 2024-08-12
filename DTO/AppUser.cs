using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TasksManagementServer.Models;

namespace TasksManagementServer.DTO
{
    public class AppUser
    {
        public int Id { get; set; }

        public string UserName { get; set; } = null!;

        public string UserLastName { get; set; } = null!;

        public string UserEmail { get; set; } = null!;

        public string UserPassword { get; set; } = null!;

        public bool IsManager { get; set; }

        public virtual ICollection<UserTask> UserTasks { get; set; } = new List<UserTask>();

        public AppUser() { }
        public AppUser(Models.AppUser modelUser)
        {
            this.Id = modelUser.Id;
            this.UserName = modelUser.UserName;
            this.UserLastName = modelUser.UserLastName;
            this.UserEmail = modelUser.UserEmail;
            this.UserPassword = modelUser.UserPassword;
            this.IsManager = modelUser.IsManager;
            this.UserTasks = new List<UserTask>();
            foreach (var task in modelUser.UserTasks)
            {
                this.UserTasks.Add(new UserTask(task));
            }
        }
    }
}
