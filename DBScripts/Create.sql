Use master
Go
IF EXISTS (SELECT * FROM sys.databases WHERE name = N'TasksManagementDB')
BEGIN
    DROP DATABASE TasksManagementDB;
END
Go
Create Database TasksManagementDB
Go
Use TasksManagementDB
Go
Create Table AppUsers
(
	Id int Primary Key Identity,
	UserName nvarchar(50) Not Null,
	UserLastName nvarchar(50) Not Null,
	UserEmail nvarchar(50) Unique Not Null,
	UserPassword nvarchar(50) Not Null,
	IsManager bit Not Null Default 0
)

Create Table UrgencyLevels
(
	UrgencyLevelId int Primary Key,
	UrgencyLevelName nvarchar(50) Not Null
)
Create Table UserTasks
(
	TaskID int Primary Key Identity,
	UserId int Foreign Key References AppUsers(Id),
	UrgencyLevelId int Foreign Key References UrgencyLevels(UrgencyLevelId),
	TaskDescription nvarchar(100) Not Null,
	TaskDueDate date Not Null,
	TaskActualDate date Null,
)
Create Table TaskComments
(
	CommentId int Primary Key Identity,
	TaskId int Foreign Key References UserTasks(TaskID),
	Comment nvarchar(100) Not Null,
	CommentDate date Not Null default getdate()
)

Insert Into UrgencyLevels Values(50, 'Low')
Insert Into UrgencyLevels Values(75, 'Medium')
Insert Into UrgencyLevels Values(100, 'High')
Go

Insert Into AppUsers Values('admin', 'admin', 'ofer2@gmauil.com', '1234', 1)
Go

Insert Into AppUsers Values('admin', 'admin', 'ofer@gmail.com', '1234', 1)
Go

-- Create a login for the admin user
CREATE LOGIN [TaskAdminLogin] WITH PASSWORD = 'kukuPassword';
Go

-- Create a user in the TasksManagementDB database for the login
CREATE USER [TaskAdminUser] FOR LOGIN [TaskAdminLogin];
Go

-- Add the user to the db_owner role to grant admin privileges
ALTER ROLE db_owner ADD MEMBER [TaskAdminUser];
Go
--so user can restore the DB!
ALTER SERVER ROLE sysadmin ADD MEMBER [TaskAdminLogin];
Go



--EF Code
/*
scaffold-DbContext "Server = (localdb)\MSSQLLocalDB;Initial Catalog=TasksManagementDB;User ID=TaskAdminLogin;Password=kukuPassword;" Microsoft.EntityFrameworkCore.SqlServer -OutPutDir Models -Context TasksManagementDbContext -DataAnnotations -force
*/

select * from AppUsers
select * from TaskComments
select * from UserTasks

insert into UserTasks (UserId, UrgencyLevelId, TaskDescription, TaskDueDate, TaskActualDate) 
values(1, 50, 'Task 50', '2024-12-01', null)

insert into UserTasks (UserId, UrgencyLevelId, TaskDescription, TaskDueDate, TaskActualDate) 
values(1, 100, 'Task 100', '2024-12-01', '2024-08-15')


