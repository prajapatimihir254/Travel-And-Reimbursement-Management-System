-- BizTravel Database Setup Script for SQL Server Management Studio (SSMS)
-- Run this script in SSMS to create the database

-- Step 1: Create the database
CREATE DATABASE biztravel;
GO

-- Step 2: Use the database
USE biztravel;
GO

-- Step 3: Create the Users table (matching your Entity Framework migration)
CREATE TABLE [dbo].[Users] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Fullname] nvarchar(max) NOT NULL,
    [EmployeeID] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Department] nvarchar(max) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

-- Step 4: Insert some sample data for testing
INSERT INTO [dbo].[Users] ([Fullname], [EmployeeID], [Email], [Department], [Password])
VALUES 
    ('John Doe', 'EMP001', 'john.doe@biztravel.com', 'IT', 'password123'),
    ('Jane Smith', 'EMP002', 'jane.smith@biztravel.com', 'HR', 'password123'),
    ('Mike Johnson', 'EMP003', 'mike.johnson@biztravel.com', 'Sales', 'password123');
GO

-- Step 5: Create the __EFMigrationsHistory table (required by Entity Framework)
CREATE TABLE [dbo].[__EFMigrationsHistory] (
    [MigrationId] nvarchar(150) NOT NULL,
    [ProductVersion] nvarchar(32) NOT NULL,
    CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
);
GO

-- Step 6: Insert the migration record
INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES ('20260106075938_InitialCreate', '8.0.0');
GO

-- Verify the setup
SELECT 'Database setup completed successfully!' as Status;
SELECT COUNT(*) as UserCount FROM Users;
SELECT * FROM Users;