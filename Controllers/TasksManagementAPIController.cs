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

                DTO.AppUser dtoUser = new DTO.AppUser(modelsUser);
                dtoUser.ProfileImagePath = GetProfileImageVirtualPath(dtoUser.Id);
                return Ok(dtoUser);
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
                DTO.AppUser dtoUser = new DTO.AppUser(modelsUser);
                dtoUser.ProfileImagePath = GetProfileImageVirtualPath(dtoUser.Id);
                return Ok(dtoUser);
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
        //Get api/getUrgencyLevels
        //This method is used to get all urgency levels from the database and return a list of DTO.UrgencyLevel
        [HttpGet("getUrgencyLevels")]
        public IActionResult GetUrgencyLevels()
        {
            try
            {
                List<DTO.UrgencyLevel> dtoLevels = new List<DTO.UrgencyLevel>();
                List<UrgencyLevel> modelLevels = context.UrgencyLevels.ToList();
                foreach (UrgencyLevel level in modelLevels)
                {
                    dtoLevels.Add(new DTO.UrgencyLevel()
                    {
                        UrgencyLevelId = level.UrgencyLevelId,
                        UrgencyLevelName = level.UrgencyLevelName
                    });
                }
                return Ok(dtoLevels);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("UploadProfileImage")]
        public async Task<IActionResult> UploadProfileImageAsync(IFormFile file)
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

            if (user == null)
            {
                return Unauthorized("User is not found in the database");
            }


            //Read all files sent
            long imagesSize = 0;
            
            if (file.Length > 0)
            {
                //Check the file extention!
                string[] allowedExtentions = { ".png", ".jpg" };
                string extention = "";
                if (file.FileName.LastIndexOf(".") > 0)
                {
                    extention = file.FileName.Substring(file.FileName.LastIndexOf(".")).ToLower();
                }
                if (!allowedExtentions.Where(e => e == extention).Any())
                {
                    //Extention is not supported
                    return BadRequest("File sent with non supported extention");
                }

                //Build path in the web root (better to a specific folder under the web root
                string filePath = $"{this.webHostEnvironment.WebRootPath}\\profileImages\\{user.Id}{extention}";

                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);

                    if (IsImage(stream))
                    {
                        imagesSize += stream.Length;
                    }
                    else
                    {
                        //Delete the file if it is not supported!
                        System.IO.File.Delete(filePath);
                    }

                }

            }

            DTO.AppUser dtoUser = new DTO.AppUser(user);
            dtoUser.ProfileImagePath = GetProfileImageVirtualPath(dtoUser.Id);
            return Ok(dtoUser);
        }

        //Helper functions

        //this function gets a file stream and check if it is an image
        private static bool IsImage(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);

            List<string> jpg = new List<string> { "FF", "D8" };
            List<string> bmp = new List<string> { "42", "4D" };
            List<string> gif = new List<string> { "47", "49", "46" };
            List<string> png = new List<string> { "89", "50", "4E", "47", "0D", "0A", "1A", "0A" };
            List<List<string>> imgTypes = new List<List<string>> { jpg, bmp, gif, png };

            List<string> bytesIterated = new List<string>();

            for (int i = 0; i < 8; i++)
            {
                string bit = stream.ReadByte().ToString("X2");
                bytesIterated.Add(bit);

                bool isImage = imgTypes.Any(img => !img.Except(bytesIterated).Any());
                if (isImage)
                {
                    return true;
                }
            }

            return false;
        }

        //this function check which profile image exist and return the virtual path of it.
        //if it does not exist it returns the default profile image virtual path
        private string GetProfileImageVirtualPath(int userId)
        {
            string virtualPath = $"/profileImages/{userId}";
            string path = $"{this.webHostEnvironment.WebRootPath}\\profileImages\\{userId}.png";
            if (System.IO.File.Exists(path))
            {
                virtualPath += ".png";
            }
            else
            {
                path = $"{this.webHostEnvironment.WebRootPath}\\profileImages\\{userId}.jpg";
                if (System.IO.File.Exists(path))
                {
                    virtualPath += ".jpg";
                }
                else
                {
                    virtualPath = $"/profileImages/default.png";
                }
            }
            
            return virtualPath;
        }
    }
}
