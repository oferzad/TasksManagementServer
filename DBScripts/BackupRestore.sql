-- REPLACE YOUR DATABASE NAME, LOGIN AND PASSWORD IN THE SCRIPT BELOW

USE master;
GO

-- Declare the database name
DECLARE @DatabaseName NVARCHAR(255) = 'TasksManagementDB';

-- Generate and execute the kill commands for all active connections
DECLARE @KillCommand NVARCHAR(MAX);

SET @KillCommand = (
    SELECT STRING_AGG('KILL ' + CAST(session_id AS NVARCHAR), '; ')
    FROM sys.dm_exec_sessions
    WHERE database_id = DB_ID(@DatabaseName)
);

IF @KillCommand IS NOT NULL
BEGIN
    EXEC sp_executesql @KillCommand;
    PRINT 'All connections to the database have been terminated.';
END
ELSE
BEGIN
    PRINT 'No active connections to the database.';
END
Go

IF EXISTS (SELECT * FROM sys.databases WHERE name = N'TasksManagementDB')
BEGIN
    DROP DATABASE TasksManagementDB;
END
Go
-- Create a login for the admin user
CREATE LOGIN [TaskAdminLogin] WITH PASSWORD = 'kukuPassword';
Go

--so user can restore the DB!
ALTER SERVER ROLE sysadmin ADD MEMBER [TaskAdminLogin];
Go

CREATE Database TasksManagementDB;
Go

