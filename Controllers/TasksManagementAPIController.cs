using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TasksManagementServer.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
namespace TasksManagementServer.Controllers
{
    [Route("api")]
    [ApiController]
    public class TasksManagementAPIController : ControllerBase
    {
        //a variable to hold a reference to the db context!
        private TasksManagementDbContext context;
        //a variable that hold a reference to web hosting interface (that provide information like the folder on which the server runs etc...)
        private IWebHostEnvironment webHostEnvironment;
        //Use dependency injection to get the db context and web host into the constructor
        public TasksManagementAPIController(TasksManagementDbContext context, IWebHostEnvironment env)
        {
            this.context = context;
            this.webHostEnvironment = env;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] DTO.LoginInfo loginDto)
        {
            try
            {
                HttpContext.Session.Clear(); //Logout any previous login attempt

                //Get model user class from DB with matching email. 
                Models.AppUser? modelsUser = context.GetUser(loginDto.Email);

                //Check if user exist for this email and if password match, if not return Access Denied (Error 403) 
                if (modelsUser == null || modelsUser.UserPassword != loginDto.Password)
                {
                    return Unauthorized();
                }

                //Login suceed! now mark login in session memory!
                HttpContext.Session.SetString("loggedInUser", modelsUser.UserEmail);

                return Ok(new DTO.AppUser(modelsUser));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] DTO.AppUser userDto)
        {
            try
            {
                HttpContext.Session.Clear(); //Logout any previous login attempt

                //Get model user class from DB with matching email. 
                Models.AppUser modelsUser = new AppUser()
                {
                    UserName = userDto.UserName,
                    UserLastName = userDto.UserLastName,
                    UserEmail = userDto.UserEmail,
                    UserPassword = userDto.UserPassword,
                    IsManager = userDto.IsManager
                };

                context.AppUsers.Add(modelsUser);
                context.SaveChanges();

                //User was added!
                return Ok(new DTO.AppUser(modelsUser));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("addTask")]
        public IActionResult AddTask([FromBody] DTO.UserTask userTaskDto)
        {
            try
            {
                //Check if who is logged in
                string? userEmail = HttpContext.Session.GetString("loggedInUser");
                if (string.IsNullOrEmpty(userEmail)) 
                { 
                    return Unauthorized("User is not logged in"); 
                }

                //Get model user class from DB with matching email. 
                Models.AppUser? user = context.GetUser(userEmail);
                //Clear the tracking of all objects to avoid double tracking
                context.ChangeTracker.Clear();

                //Check if the user that is logged in is the same user of the task
                //this situation is ok only if the user is a manager
                if (user == null || (user.IsManager == false && userTaskDto.UserId != user.Id))
                {
                    return Unauthorized("Non Manager User is trying to add a task for a different user");
                }

                Models.UserTask task = new UserTask()
                {
                    UserId = userTaskDto.UserId,
                    UrgencyLevelId = userTaskDto.UrgencyLevelId,
                    TaskDescription = userTaskDto.TaskDescription,
                    TaskDueDate = userTaskDto.TaskDueDate,
                    TaskActualDate = userTaskDto.TaskActualDate
                };

                context.Entry(task).State = EntityState.Added;
                context.SaveChanges();

                //Task was added!
                return Ok(new DTO.UserTask(task));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [HttpPost("updateTask")]
        public IActionResult UpdateTask([FromBody] DTO.UserTask userTaskDto)
        {
            try
            {
                //Check if who is logged in
                string? userEmail = HttpContext.Session.GetString("loggedInUser");
                if (string.IsNullOrEmpty(userEmail))
                {
                    return Unauthorized("User is not logged in");
                }

                //Get model user class from DB with matching email. 
                Models.AppUser? user = context.GetUser(userEmail);
                //Clear the tracking of all objects to avoid double tracking
                context.ChangeTracker.Clear();
                //Check if the user that is logged in is the same user of the task
                //this situation is ok only if the user is a manager
                if (user == null || (user.IsManager == false && userTaskDto.UserId != user.Id))
                {
                    return Unauthorized("Non Manager User is trying to update a task for a different user");
                }

                Models.UserTask task = new UserTask()
                {
                    TaskId = userTaskDto.TaskId,
                    UserId = userTaskDto.UserId,
                    UrgencyLevelId = userTaskDto.UrgencyLevelId,
                    TaskDescription = userTaskDto.TaskDescription,
                    TaskDueDate = userTaskDto.TaskDueDate,
                    TaskActualDate = userTaskDto.TaskActualDate
                };

                context.Entry(task).State = EntityState.Modified;

                //Now loop through the comments and update / add all of them
                foreach(var comment in userTaskDto.TaskComments)
                {
                    //check if comment is new or not
                    if (comment.CommentId == 0)
                    {
                        //New comment
                        Models.TaskComment newTaskComment = new TaskComment()
                        {
                            TaskId = task.TaskId,
                            CommentDate = comment.CommentDate,
                            Comment = comment.Comment
                        };

                        context.Entry(newTaskComment).State = EntityState.Added;
                    }
                    else
                    {
                        //Update the existing comment
                        Models.TaskComment taskComment = new TaskComment()
                        {
                            TaskId = task.TaskId,
                            CommentId = comment.CommentId,
                            CommentDate = comment.CommentDate,
                            Comment = comment.Comment
                        };

                        context.Entry(taskComment).State = EntityState.Modified;
                    }
                    
                }
                context.SaveChanges();

                //Task was updated!
                return Ok(new DTO.UserTask(task));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("updateUser")]
        public IActionResult UpdateUser([FromBody] DTO.AppUser userDto)
        {
            try
            {
                //Check if who is logged in
                string? userEmail = HttpContext.Session.GetString("loggedInUser");
                if (string.IsNullOrEmpty(userEmail))
                {
                    return Unauthorized("User is not logged in");
                }

                //Get model user class from DB with matching email. 
                Models.AppUser? user = context.GetUser(userEmail);
                //Clear the tracking of all objects to avoid double tracking
                context.ChangeTracker.Clear();

                //Check if the user that is logged in is the same user of the task
                //this situation is ok only if the user is a manager
                if (user == null || (user.IsManager == false && userDto.Id != user.Id))
                {
                    return Unauthorized("Non Manager User is trying to update a different user");
                }

                Models.AppUser appUser = new AppUser()
                {
                    Id = userDto.Id,
                    UserName = userDto.UserName,
                    UserLastName = userDto.UserLastName,
                    UserEmail = userDto.UserEmail,
                    UserPassword = userDto.UserPassword,
                    IsManager = userDto.IsManager
                };

                context.Entry(appUser).State = EntityState.Modified;

                context.SaveChanges();

                //Task was updated!
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        // Get api/check
        //This method is only used for testing the server. It tests if the server recognizes a logged in user
        [HttpGet("check")]
        public IActionResult Check()
        {
            try
            {
                //Check if user is logged in 
                string? userEmail = HttpContext.Session.GetString("loggedInUser");

                if (string.IsNullOrEmpty(userEmail))
                {
                    return Unauthorized("User is not logged in");
                }


                string str = userEmail + "  Is Logged in!";
                return Ok(str);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


    }
}
