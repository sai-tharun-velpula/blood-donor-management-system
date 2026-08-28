/*
 BloodCare Professional Edition - SQL Server setup / migration
 Creates: Users, Donors, DonationHistory
 Supports existing BloodCare databases by adding missing security/profile columns.
*/
IF DB_ID(N'BloodDonorManagementDB') IS NULL CREATE DATABASE BloodDonorManagementDB;
GO
USE BloodDonorManagementDB;
GO

IF OBJECT_ID('dbo.Users','U') IS NULL
BEGIN
 CREATE TABLE dbo.Users(
  UserId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
  Username NVARCHAR(80) NOT NULL CONSTRAINT UQ_Users_Username UNIQUE,
  Email NVARCHAR(150) NULL,
  PasswordHash NVARCHAR(200) NOT NULL,
  PasswordSalt NVARCHAR(200) NOT NULL,
  FullName NVARCHAR(120) NULL,
  RoleName NVARCHAR(40) NOT NULL CONSTRAINT DF_Users_RoleName DEFAULT('Admin'),
  IsActive BIT NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT(1),
  MustChangePassword BIT NOT NULL CONSTRAINT DF_Users_MustChange DEFAULT(0),
  CreatedDate DATETIME2 NOT NULL CONSTRAINT DF_Users_Created DEFAULT(SYSDATETIME()),
  UpdatedDate DATETIME2 NOT NULL CONSTRAINT DF_Users_Updated DEFAULT(SYSDATETIME())
 );
END
GO

IF COL_LENGTH('dbo.Users','Email') IS NULL ALTER TABLE dbo.Users ADD Email NVARCHAR(150) NULL;
IF COL_LENGTH('dbo.Users','MustChangePassword') IS NULL ALTER TABLE dbo.Users ADD MustChangePassword BIT NOT NULL CONSTRAINT DF_Users_MustChange DEFAULT(0);
IF COL_LENGTH('dbo.Users','UpdatedDate') IS NULL ALTER TABLE dbo.Users ADD UpdatedDate DATETIME2 NOT NULL CONSTRAINT DF_Users_Updated DEFAULT(SYSDATETIME());
IF COL_LENGTH('dbo.Users','RoleName') IS NULL ALTER TABLE dbo.Users ADD RoleName NVARCHAR(40) NOT NULL CONSTRAINT DF_Users_RoleName DEFAULT('Admin');
GO

IF OBJECT_ID('dbo.Donors','U') IS NULL
BEGIN
 CREATE TABLE dbo.Donors(
  DonorId INT IDENTITY(1001,1) NOT NULL CONSTRAINT PK_Donors PRIMARY KEY,
  FullName NVARCHAR(120) NOT NULL,
  Gender NVARCHAR(20) NULL,
  DateOfBirth DATE NULL,
  BloodGroup NVARCHAR(5) NOT NULL,
  Mobile NVARCHAR(20) NOT NULL,
  Email NVARCHAR(150) NULL,
  Address NVARCHAR(300) NULL,
  City NVARCHAR(80) NOT NULL,
  State NVARCHAR(80) NOT NULL,
  Pincode NVARCHAR(10) NULL,
  LastDonationDate DATE NULL,
  IsAvailable BIT NOT NULL CONSTRAINT DF_Donors_Available DEFAULT(1),
  CreatedDate DATETIME2 NOT NULL CONSTRAINT DF_Donors_Created DEFAULT(SYSDATETIME()),
  UpdatedDate DATETIME2 NOT NULL CONSTRAINT DF_Donors_Updated DEFAULT(SYSDATETIME()),
  UserId INT NULL
 );
 CREATE INDEX IX_Donors_BloodGroup ON dbo.Donors(BloodGroup);
 CREATE INDEX IX_Donors_City ON dbo.Donors(City);
 CREATE INDEX IX_Donors_Available ON dbo.Donors(IsAvailable);
 CREATE UNIQUE INDEX UX_Donors_UserId ON dbo.Donors(UserId) WHERE UserId IS NOT NULL;
END
GO

IF COL_LENGTH('dbo.Donors','Email') IS NULL ALTER TABLE dbo.Donors ADD Email NVARCHAR(150) NULL;
IF COL_LENGTH('dbo.Donors','Address') IS NULL ALTER TABLE dbo.Donors ADD Address NVARCHAR(300) NULL;
IF COL_LENGTH('dbo.Donors','Pincode') IS NULL ALTER TABLE dbo.Donors ADD Pincode NVARCHAR(10) NULL;
IF COL_LENGTH('dbo.Donors','UserId') IS NULL ALTER TABLE dbo.Donors ADD UserId INT NULL;
IF COL_LENGTH('dbo.Donors','UpdatedDate') IS NULL ALTER TABLE dbo.Donors ADD UpdatedDate DATETIME2 NOT NULL CONSTRAINT DF_Donors_Updated DEFAULT(SYSDATETIME());
IF COL_LENGTH('dbo.Donors','Gender') IS NOT NULL ALTER TABLE dbo.Donors ALTER COLUMN Gender NVARCHAR(20) NULL;
IF COL_LENGTH('dbo.Donors','DateOfBirth') IS NOT NULL ALTER TABLE dbo.Donors ALTER COLUMN DateOfBirth DATE NULL;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username='admin')
BEGIN
 INSERT INTO dbo.Users(Username,Email,PasswordHash,PasswordSalt,FullName,RoleName,IsActive,MustChangePassword)
 VALUES('admin','admin@bloodcare.local','5ff3e7580b6c33f19c2d472f315519ad5cb9628a691bb38e0c235adbf598a003','BloodCareAdminSalt2026','System Administrator','Admin',1,0);
END
ELSE
BEGIN
 UPDATE dbo.Users SET PasswordHash='5ff3e7580b6c33f19c2d472f315519ad5cb9628a691bb38e0c235adbf598a003',PasswordSalt='BloodCareAdminSalt2026',RoleName='Admin',IsActive=1,MustChangePassword=0 WHERE Username='admin';
END
GO

IF OBJECT_ID('dbo.DonationHistory','U') IS NULL
BEGIN
 CREATE TABLE dbo.DonationHistory(
  DonationId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_DonationHistory PRIMARY KEY,
  DonorId INT NOT NULL,
  DonationDate DATE NOT NULL,
  BloodGroup NVARCHAR(5) NULL,
  Units DECIMAL(4,2) NOT NULL CONSTRAINT DF_DonationHistory_Units DEFAULT(1),
  Location NVARCHAR(150) NULL,
  Notes NVARCHAR(500) NULL,
  RecordedByUserId INT NULL,
  CreatedDate DATETIME2 NOT NULL CONSTRAINT DF_DonationHistory_Created DEFAULT(SYSDATETIME())
 );
 CREATE INDEX IX_DonationHistory_DonorId ON dbo.DonationHistory(DonorId);
 CREATE INDEX IX_DonationHistory_Date ON dbo.DonationHistory(DonationDate);
END
GO

PRINT 'BloodCare Professional database setup/migration completed.';
PRINT 'Admin username: admin';
PRINT 'Admin password: Admin@123';
GO
