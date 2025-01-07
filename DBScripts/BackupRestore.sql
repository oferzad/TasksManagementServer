-- REPLACE YOUR DATABASE LOGIN AND PASSWORD IN THE SCRIPT BELOW

Use master
Go

-- Create a login for the admin user
CREATE LOGIN [TaskAdminLogin] WITH PASSWORD = 'kukuPassword';
Go

--so user can restore the DB!
ALTER SERVER ROLE sysadmin ADD MEMBER [TaskAdminLogin];
Go
