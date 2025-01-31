


---- FRANCHISE TABLE ====================================================================================================================

	--Franchises
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Franchises_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Franchises_Notes varchar(MAX) null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Franchises_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Franchises_Add bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Franchises_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Franchises_View bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Franchises_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Franchises_Edit bit default 0 not null;
	GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Franchises' AND TABLE_SCHEMA='dbo') 
BEGIN
	CREATE TABLE [dbo].[Franchises]
	(
		[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
		[Name] VARCHAR(MAX) NOT NULL, 
		[Active] BIT NOT NULL DEFAULT 1,
		[Notes] VARCHAR(MAX) NULL
	)
END
GO

INSERT INTO Franchises(Id, Name, Active) VALUES(NEWID(), 'MAIN', 1);

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Franchises_Id' AND TABLE_NAME = 'Branches' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE Branches ADD Franchises_Id UNIQUEIDENTIFIER NULL;
GO

UPDATE Branches SET Franchises_Id = (SELECT TOP 1 (Id) FROM Franchises) 

ALTER TABLE Branches ALTER COLUMN Franchises_Id UNIQUEIDENTIFIER NOT NULL;



---- LANDING PAGE UPDATE ================================================================================================================

	------LandingPageUpdate
	--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'LandingPageUpdate_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	--ALTER TABLE UserAccountRoles ADD LandingPageUpdate_Notes varchar(MAX) null;
	--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'LandingPageUpdate_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	--ALTER TABLE UserAccountRoles ADD LandingPageUpdate_Edit bit default 0 not null;
	--GO

---- ACTIVITY LOGS ======================================================================================================================

	----ActivityLogs
	--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'ActivityLogs_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	--ALTER TABLE UserAccountRoles ADD ActivityLogs_Notes varchar(MAX) null;
	--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'ActivityLogs_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	--ALTER TABLE UserAccountRoles ADD ActivityLogs_View bit default 0 not null;
	--GO

---- CLUB CLASS ONLINE LINKS ============================================================================================================
--CREATE TABLE [dbo].[ClubClassOnlineLinks]
--(
--	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
--    [ClubClasses_Id] UNIQUEIDENTIFIER NOT NULL, 
--    [Name] VARCHAR(MAX) NOT NULL, 
--    [OnlineLink] VARCHAR(MAX) NOT NULL, 
--    [WeekNo] TINYINT NOT NULL DEFAULT 0, 
--    [DurationDays] TINYINT NOT NULL DEFAULT 0,
--    [Active] BIT NOT NULL DEFAULT 1, 
--    [Notes] VARCHAR(MAX) NULL
--)
--GO

--	--ClubSchedules
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'ClubClassOnlineLinks_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD ClubClassOnlineLinks_Notes varchar(MAX) null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'ClubClassOnlineLinks_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD ClubClassOnlineLinks_Add bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'ClubClassOnlineLinks_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD ClubClassOnlineLinks_View bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'ClubClassOnlineLinks_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD ClubClassOnlineLinks_Edit bit default 0 not null;
--	GO

--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ClubClasses' AND COLUMN_NAME = 'PeriodStartDate' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE ClubClasses ADD PeriodStartDate datetime NULL 
--GO
--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ClubClasses' AND COLUMN_NAME = 'PeriodAdjustmentDayCount' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE ClubClasses ADD PeriodAdjustmentDayCount int NOT NULL DEFAULT 0 
--GO
--UPDATE ClubClasses SET PeriodStartDate = '8/15/2022', PeriodAdjustmentDayCount=0
--GO


---- CLUB CLASSES ============================================================================================================

	--ClubSchedules
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'ClubClasses_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD ClubClasses_Notes varchar(MAX) null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'ClubClasses_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD ClubClasses_Add bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'ClubClasses_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD ClubClasses_View bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'ClubClasses_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD ClubClasses_Edit bit default 0 not null;
--	GO

--UPDATE ClubSchedules SET [Description] = REPLACE([Description],'Kindergaten','Kindergarten')

--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ClubSchedules' AND COLUMN_NAME = 'Onsite' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE ClubSchedules ADD Onsite bit NOT NULL DEFAULT 0
--GO

--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ClubSchedules' AND COLUMN_NAME = 'ClubClasses_Id' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE ClubSchedules ADD ClubClasses_Id uniqueidentifier NULL
--GO

--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ClubClasses' AND TABLE_SCHEMA='dbo') 
--BEGIN
--	CREATE TABLE [dbo].[ClubClasses]
--	(
--		[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
--		[Name] VARCHAR(MAX) NOT NULL, 
--		[Languages_Id] UNIQUEIDENTIFIER NOT NULL, 
--		[Active] BIT NOT NULL DEFAULT 1,
--		[Notes] VARCHAR(MAX) NULL
--	)
--END
--GO
--DELETE ClubClasses
--GO

--	IF(SELECT object_id('TempDB..#TEMP_INPUTARRAY')) IS NOT NULL
--		DROP TABLE #TEMP_INPUTARRAY
		
--	SELECT * INTO #TEMP_INPUTARRAY FROM (SELECT * FROM ClubSchedules) AS x
	
--	DECLARE @Iteration_Id uniqueidentifier
--	DECLARE @Description varchar(MAX)
--	DECLARE @Languages_Id uniqueidentifier
--	DECLARE @NewId uniqueidentifier
--	WHILE EXISTS(SELECT * FROM #TEMP_INPUTARRAY)
--	BEGIN
--		SELECT TOP 1 @Iteration_Id = Id, @Description = [Description], @Languages_Id=Languages_Id FROM #TEMP_INPUTARRAY

--		-- update onsite column
--		IF CHARINDEX('(onsite)', @Description) > 0
--		BEGIN
--			UPDATE ClubSchedules SET Onsite=1, [Description]=RTRIM(LTRIM(REPLACE([Description],'(onsite)',''))) WHERE Id=@Iteration_Id
--			SELECT @Description = [Description] FROM ClubSchedules WHERE Id=@Iteration_Id
--		END

--		-- create clubclasses
--		IF NOT EXISTS (SELECT [Name] FROM ClubClasses WHERE [Name] = @Description)
--		BEGIN
--			INSERT INTO ClubClasses(Id, Name, Languages_Id) VALUES(NEWID(), @Description, @Languages_Id)
--		END
--		UPDATE ClubSchedules SET ClubClasses_Id=(SELECT Id FROM ClubClasses WHERE Name=@Description) WHERE Id = @Iteration_Id
		
--		-- remove row to iterate to the next row
--		DELETE #TEMP_INPUTARRAY WHERE Id = @Iteration_Id
--	END
	
--	-- clean up
--	DROP TABLE #TEMP_INPUTARRAY
--GO

--ALTER TABLE ClubSchedules DROP COLUMN Languages_Id
--ALTER TABLE ClubSchedules DROP COLUMN Description
--UPDATE ClubSchedules SET Notes=null

---- Kasih role access ke club classes

--SELECT * FROM ClubClasses ORDER BY Name ASC
--select * from ClubSchedules


---- CLUB SCHEDULES ==========================================================================================================

--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'SaleInvoiceItems' AND COLUMN_NAME = 'IsClubSubscription' AND TABLE_SCHEMA='dbo') 
--    ALTER TABLE SaleInvoiceItems ADD IsClubSubscription bit NULL DEFAULT 0 

--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'StartingDate' AND TABLE_NAME = 'SaleInvoiceItems' AND TABLE_SCHEMA='dbo') 
--    ALTER TABLE SaleInvoiceItems ADD StartingDate datetime NULL 
--GO

--UPDATE SaleInvoiceItems 
--SET StartingDate = CAST(SaleInvoices.Timestamp AS Date)
--FROM SaleInvoiceItems 
--LEFT JOIN SaleInvoices ON SaleInvoices.Id = SaleInvoiceItems.SaleInvoices_Id


--ALTER TABLE LessonPackages ADD IsClubSubscription bit NOT NULL DEFAULT 0
--GO

	--alter table SaleInvoiceItems add ExpirationMonth tinyint NOT NULL default 0
	--GO
	
--DROP TABLE ClubSchedules;
--GO
--CREATE TABLE [dbo].[ClubSchedules]
--(
--	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
--    [Description] VARCHAR(MAX) NOT NULL, 
--    [LessonPackages_Id] UNIQUEIDENTIFIER NOT NULL, 
--    [Languages_Id] UNIQUEIDENTIFIER NOT NULL, 
--    [Branches_Id] UNIQUEIDENTIFIER NULL,
--    [DayOfWeek] TINYINT NOT NULL DEFAULT 0, 
--    [StartTime] DATETIME NOT NULL, 
--    [EndTime] DATETIME NOT NULL, 
--    [OnlineLink] VARCHAR(MAX) NULL, 
--    [Active] BIT NOT NULL DEFAULT 1, 
--    [Notes] VARCHAR(MAX) NULL
--)
--GO

--	--ClubSchedules
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'ClubSchedules_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD ClubSchedules_Notes varchar(MAX) null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'ClubSchedules_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD ClubSchedules_Add bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'ClubSchedules_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD ClubSchedules_View bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'ClubSchedules_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD ClubSchedules_Edit bit default 0 not null;
--	GO

--	alter table lessonpackages add ExpirationMonth tinyint NOT NULL default 0
--	GO

---- REPORTS =================================================================================================================

--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'IncomeStatement_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--ALTER TABLE UserAccountRoles ADD IncomeStatement_Notes varchar(MAX) null;
--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'IncomeStatement_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--ALTER TABLE UserAccountRoles ADD IncomeStatement_ViewProfit bit default 0 not null;
--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'IncomeStatement_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--ALTER TABLE UserAccountRoles ADD IncomeStatement_View bit default 0 not null;

---- FILES ===================================================================================================================

--CREATE TABLE [dbo].[GoogleTokens]
--(
--	[Id] VARCHAR(50) NOT NULL PRIMARY KEY, 
--    [token] NVARCHAR(MAX) NOT NULL
--)
--GO

--CREATE TABLE [dbo].[Files]
--(
--	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
--    [OnlineFileId] VARCHAR(MAX) NULL, 
--    [ParentId] UNIQUEIDENTIFIER NOT NULL, 
--    [Branches_Id] UNIQUEIDENTIFIER NULL,
--    [No] VARCHAR(MAX) NOT NULL, 
--    [FileName] VARCHAR(MAX) NULL,
--    [DirectoryName] VARCHAR(MAX) NULL, 
--    [Notes] VARCHAR(MAX) NULL, 
--    [UserAccounts_Id] UNIQUEIDENTIFIER NOT NULL, 
--    [Timestamp] DATETIME NOT NULL, 
--    [IsDeleted] BIT NOT NULL DEFAULT 0, 
--    [Approved] BIT NOT NULL DEFAULT 0
--)

----Files
--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'IncomeStatement_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--ALTER TABLE UserAccountRoles ADD IncomeStatement_Notes varchar(MAX) null;
--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'IncomeStatement_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--ALTER TABLE UserAccountRoles ADD IncomeStatement_Add bit default 0 not null;
--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'IncomeStatement_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--ALTER TABLE UserAccountRoles ADD IncomeStatement_View bit default 0 not null;
--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'IncomeStatement_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--ALTER TABLE UserAccountRoles ADD IncomeStatement_Edit bit default 0 not null;
--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'IncomeStatement_EditGlobal' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--ALTER TABLE UserAccountRoles ADD IncomeStatement_EditGlobal bit default 0 not null;
--GO

---- CLEANUP ASPNET USER TABLES ==============================================================================================

--DROP TABLE __MigrationHistory;
--GO
--DROP TABLE AspNetRoles
--GO
--DROP TABLE AspNetUserClaims
--GO
--DROP TABLE AspNetUserLogins
--GO
--DROP TABLE AspNetUsers
--GO
--DROP TABLE AspNetUserRoles
--GO
--DROP TABLE RoleAccessMenu
--GO

--DROP TABLE Logs;
--GO
--DROP TABLE MasterMenu;
--GO

---- CLEANUP =================================================================================================================

--if not exists (select 1 from information_schema.columns where column_name = 'ReferenceId' and table_name = 'ActivityLogs' and table_schema='dbo') 
--	exec sp_rename 'ActivityLogs.ReffId' , 'ReferenceId', 'column'
--GO

--if not exists (select 1 from information_schema.columns where column_name = 'ReferenceId' and table_name = 'PettyCashRecords' and table_schema='dbo') 
--	exec sp_rename 'PettyCashRecords.RefId' , 'ReferenceId', 'column'
--GO

--if not exists (select 1 from information_schema.columns where column_name = 'Approved' and table_name = 'PettyCashRecords' and table_schema='dbo') 
--	exec sp_rename 'PettyCashRecords.IsChecked' , 'Approved', 'column'
--GO
--if not exists (select 1 from information_schema.columns where column_name = 'Approved' and table_name = 'PayrollPayments' and table_schema='dbo') 
--	exec sp_rename 'PayrollPayments.IsChecked' , 'Approved', 'column'
--GO
--if not exists (select 1 from information_schema.columns where column_name = 'Approved' and table_name = 'SaleInvoices' and table_schema='dbo') 
--	exec sp_rename 'SaleInvoices.IsChecked' , 'Approved', 'column'
--GO
--if not exists (select 1 from information_schema.columns where column_name = 'Approved' and table_name = 'Payments' and table_schema='dbo') 
--	exec sp_rename 'Payments.Confirmed' , 'Approved', 'column'
--GO
--if not exists (select 1 from information_schema.columns where column_name = 'Cancelled' and table_name = 'LessonSessions' and table_schema='dbo') 
--	exec sp_rename 'LessonSessions.Deleted' , 'Cancelled', 'column'
--GO

--if not exists (select 1 from information_schema.columns where column_name = 'CancelNotes' and table_name = 'LessonSessions' and table_schema='dbo') 
--	exec sp_rename 'LessonSessions.Notes_Cancel' , 'CancelNotes', 'column'
--GO
--if not exists (select 1 from information_schema.columns where column_name = 'CancelNotes' and table_name = 'PayrollPayments' and table_schema='dbo') 
--	exec sp_rename 'PayrollPayments.Notes_Cancel' , 'CancelNotes', 'column'
--GO

--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Branches_Id' AND TABLE_NAME = 'PayrollPayments' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE PayrollPayments ADD Branches_Id uniqueidentifier NULL
--GO
---- populate value
--	IF(SELECT object_id('TempDB..#TEMP_INPUTARRAY')) IS NOT NULL
--		DROP TABLE #TEMP_INPUTARRAY
		
--	SELECT * INTO #TEMP_INPUTARRAY FROM (SELECT * FROM PayrollPayments) AS x
	
--	DECLARE @Iteration_Id uniqueidentifier
--	DECLARE @Branches_Id uniqueidentifier
--	WHILE EXISTS(SELECT * FROM #TEMP_INPUTARRAY)
--	BEGIN
--		SELECT TOP 1 @Iteration_Id = Id FROM #TEMP_INPUTARRAY
--		SELECT TOP 1 @Branches_Id = PayrollPaymentItems.Branches_Id FROM PayrollPaymentItems WHERE PayrollPaymentItems.PayrollPayments_Id = @Iteration_Id

--		-- add operation here
--		UPDATE PayrollPayments SET Branches_Id = @Branches_Id WHERE Id = @Iteration_Id
		
--		-- remove row to iterate to the next row
--		DELETE #TEMP_INPUTARRAY WHERE Id = @Iteration_Id
--	END
	
--	-- clean up
--	DROP TABLE #TEMP_INPUTARRAY
--GO
--UPDATE PayrollPayments SET Branches_Id = NULL WHERE Cancelled=1
--GO


---- UPDATE STUDENT SCHEDULES ================================================================================================

--UPDATE StudentSchedules SET Notes = 'ONLINE' WHERE UPPER(Notes) collate SQL_Latin1_General_CP1_CS_AS = 'ONLINE'
--UPDATE StudentSchedules SET Notes = 'ONSITE' WHERE UPPER(Notes) collate SQL_Latin1_General_CP1_CS_AS = 'ONSITE'
--GO

--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'LessonLocation' AND TABLE_NAME = 'StudentSchedules' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE StudentSchedules ADD LessonLocation varchar(MAX) NULL
--GO

--UPDATE StudentSchedules SET LessonLocation = Notes WHERE Notes = 'ONLINE' OR Notes = 'ONSITE'
--GO
--UPDATE StudentSchedules SET Notes = '' WHERE Notes = 'ONLINE' OR Notes = 'ONSITE'
--GO

---- STUDENT SCHEDULES =======================================================================================================

--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'UserAccounts_ViewAllRoles' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD UserAccounts_ViewAllRoles bit default 0 not null;

--CREATE TABLE [dbo].[StudentSchedules] (
--    [Id]                      UNIQUEIDENTIFIER NOT NULL,
--    [DayOfWeek]               TINYINT          NOT NULL,
--    [StartTime]               DATETIME         NOT NULL,
--    [EndTime]                 DATETIME         NOT NULL,
--    [SaleInvoiceItems_Id]     UNIQUEIDENTIFIER NOT NULL,
--    [Notes]                   VARCHAR (MAX)    NULL,
--    [Active]                  BIT              NOT NULL,
--    [Tutor_UserAccounts_Id]   UNIQUEIDENTIFIER NULL,
--    [Student_UserAccounts_Id] UNIQUEIDENTIFIER NULL
--);
--GO
--DROP TABLE TutorStudentSchedules
--GO
--DELETE StudentSchedules
--GO
--DELETE TutorSchedules
--GO

------StudentSchedules
--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'StudentSchedules_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	EXEC sp_RENAME 'UserAccountRoles.TutorStudentSchedules_Notes' , 'StudentSchedules_Notes', 'COLUMN'
--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'StudentSchedules_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	EXEC sp_RENAME 'UserAccountRoles.TutorStudentSchedules_Add' , 'StudentSchedules_Add', 'COLUMN'
--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'StudentSchedules_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	EXEC sp_RENAME 'UserAccountRoles.TutorStudentSchedules_View' , 'StudentSchedules_View', 'COLUMN'
--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'StudentSchedules_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	EXEC sp_RENAME 'UserAccountRoles.TutorStudentSchedules_Edit' , 'StudentSchedules_Edit', 'COLUMN'
--GO



---- TUTOR SCHEDULES =========================================================================================================

--UPDATE UserAccounts set Fullname = REPLACE(REPLACE(Fullname, '   ', ' '), '  ', ' ')
--GO

--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Tutor_UserAccounts_Id_TEMP' AND TABLE_NAME = 'TutorSchedules' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE TutorSchedules ADD Tutor_UserAccounts_Id_TEMP uniqueidentifier NULL;
--GO	
--UPDATE TutorSchedules SET Tutor_UserAccounts_Id_TEMP = CONVERT(UNIQUEIDENTIFIER, Tutor_UserAccounts_Id)
--GO	
--ALTER TABLE TutorSchedules DROP COLUMN Tutor_UserAccounts_Id;
--EXEC sp_RENAME 'TutorSchedules.Tutor_UserAccounts_Id_TEMP' , 'Tutor_UserAccounts_Id', 'COLUMN'
--GO
--EXEC sp_RENAME 'TutorSchedules.IsActive' , 'Active', 'COLUMN'
--GO

----TutorSchedules
--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'TutorSchedules_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--ALTER TABLE UserAccountRoles ADD TutorSchedules_Notes varchar(MAX) null;
--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'TutorSchedules_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--ALTER TABLE UserAccountRoles ADD TutorSchedules_Add bit default 0 not null;
--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'TutorSchedules_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--ALTER TABLE UserAccountRoles ADD TutorSchedules_View bit default 0 not null;
--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'TutorSchedules_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--ALTER TABLE UserAccountRoles ADD TutorSchedules_Edit bit default 0 not null;
--GO

--delete TutorSchedules where TutorSchedules.Tutor_UserAccounts_Id is null
--GO

---- TUTOR STUDENT SCHEDULES =================================================================================================

--IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'IsActive' AND TABLE_NAME = 'StudentSchedules' AND TABLE_SCHEMA='dbo') 
--	EXEC sp_RENAME 'StudentSchedules.IsActive' , 'Active', 'COLUMN'
--GO
--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Tutor_UserAccounts_Id_TEMP' AND TABLE_NAME = 'StudentSchedules' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE StudentSchedules ADD Tutor_UserAccounts_Id_TEMP uniqueidentifier NULL;
--GO	
--UPDATE StudentSchedules SET Tutor_UserAccounts_Id_TEMP = CONVERT(UNIQUEIDENTIFIER, Tutor_UserAccounts_Id)
--GO	
--ALTER TABLE StudentSchedules DROP COLUMN Tutor_UserAccounts_Id;
--EXEC sp_RENAME 'StudentSchedules.Tutor_UserAccounts_Id_TEMP' , 'Tutor_UserAccounts_Id', 'COLUMN'
--GO
--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Student_UserAccounts_Id_TEMP' AND TABLE_NAME = 'StudentSchedules' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE StudentSchedules ADD Student_UserAccounts_Id_TEMP uniqueidentifier NULL;
--GO	
--UPDATE StudentSchedules SET Student_UserAccounts_Id_TEMP = CONVERT(UNIQUEIDENTIFIER, Student_UserAccounts_Id)
--GO	
--ALTER TABLE StudentSchedules DROP COLUMN Student_UserAccounts_Id;
--EXEC sp_RENAME 'StudentSchedules.Student_UserAccounts_Id_TEMP' , 'Student_UserAccounts_Id', 'COLUMN'
--GO
--EXEC sp_RENAME 'StudentSchedules.InvoiceItems_Id' , 'SaleInvoiceItems_Id', 'COLUMN'
--GO

----StudentSchedules
--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'StudentSchedules_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--ALTER TABLE UserAccountRoles ADD StudentSchedules_Notes varchar(MAX) null;
--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'StudentSchedules_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--ALTER TABLE UserAccountRoles ADD StudentSchedules_Add bit default 0 not null;
--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'StudentSchedules_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--ALTER TABLE UserAccountRoles ADD StudentSchedules_View bit default 0 not null;
--IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'StudentSchedules_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--ALTER TABLE UserAccountRoles ADD StudentSchedules_Edit bit default 0 not null;
--GO








-- Add the new accesses to user roles!!






---- NEED TO EXECUTE AFTER MIGRATION COMPLETE ==============================================================================


----DROP TABLE AspNetUserRoles
----DROP TABLE AspNetUserLogins
----DROP TABLE AspNetUserClaims
----DROP TABLE __MigrationHistory
----DROP TABLE AspNetUsers
----DROP TABLE MasterMenu
----DROP TABLE RoleAccessMenu
----DROP TABLE SaleInvoiceItems_Vouchers


---- NEED TO DO AFTER SCRIPT IS RUN =======================================================================================

---- all users will be prompted for password change

---- CREATE NEW USER ACCOUNTS TABLE =======================================================================================

--	-- drop table if already exists
--	IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'UserAccounts'))
--		DROP TABLE UserAccounts;

--	CREATE TABLE [dbo].[UserAccounts] (
--		[Id]                   UNIQUEIDENTIFIER NOT NULL,
--		[Username]             VARCHAR (MAX)   NOT NULL,
--		[Password]			   VARCHAR (MAX)   NOT NULL,
--		[Fullname]             VARCHAR (MAX)   NOT NULL,
--		[Roles]				   VARCHAR (MAX)   NULL,
--		[Birthday]             DATE             NOT NULL,
--		[Branches_Id]		   UNIQUEIDENTIFIER NOT NULL,
--		[Active]               BIT              DEFAULT ((1)) NOT NULL,
--		[ResetPassword]		   BIT				DEFAULT ((0)) NOT NULL,
--		[Address]              VARCHAR (MAX)   NULL,
--		[Phone1]               VARCHAR (MAX)   NULL,
--		[Phone2]               VARCHAR (MAX)   NULL,
--		[Notes]                VARCHAR (MAX)   NULL,
--		[Email]                VARCHAR (MAX)   NULL,
--		[Interest]             VARCHAR (MAX)   NULL,
--		[Branches]             VARCHAR (MAX)   NULL,
--		[PromotionEvents_Id]   UNIQUEIDENTIFIER NULL
--	);
--	GO

---- MIGRATE USER ACCOUNTS DATA TO THE NEW USER ACCOUNTS TABLE ============================================================

--	DELETE UserAccounts;

--	-- drop table if already exists
--	IF(SELECT object_id('TempDB..#TEMP_INPUTARRAY')) IS NOT NULL
--		DROP TABLE #TEMP_INPUTARRAY
		
--	SELECT * INTO #TEMP_INPUTARRAY FROM (SELECT * FROM AspNetUsers) AS x
	
--	DECLARE @Iteration_Id nvarchar(128);
--	WHILE EXISTS(SELECT * FROM #TEMP_INPUTARRAY)
--	BEGIN
--		SELECT TOP 1 @Iteration_Id = Id FROM #TEMP_INPUTARRAY

--		-- add operation here
--		INSERT INTO UserAccounts(Id, Username, Password, Fullname, Birthday, Branches_Id, Active, ResetPassword,
--								Address, Phone1, Phone2, Notes, Email, Interest, PromotionEvents_Id,Roles) 
--			VALUES(
--				(SELECT CONVERT(UNIQUEIDENTIFIER, Id) FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
--				(SELECT UserName FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
--				'b1b3773a05c0ed0176787a4f1574ff0075f7521e',
--				(SELECT RTRIM(RTRIM(Firstname + ' ' + ISNULL(Middlename,'')) + ' ' + ISNULL(Lastname,'')) FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
--				(SELECT Birthday FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
--				(SELECT Branches_Id FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
--				(SELECT Active FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
--				1,
--				(SELECT Address FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
--				(SELECT Phone1 FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
--				(SELECT Phone2 FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
--				(SELECT Notes FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
--				(SELECT Email FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
--				(SELECT REPLACE(REPLACE(REPLACE(Interest, '[{"Languages_Id":"', ''), '"},{"Languages_Id":"', ','), '"}]', '') FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
--				(SELECT PromotionEvents_Id FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
--				NULL
--			)

		
--		-- remove row to iterate to the next row
--		DELETE #TEMP_INPUTARRAY WHERE Id = @Iteration_Id
--	END
	
--	-- clean up
--	DROP TABLE #TEMP_INPUTARRAY;
--	GO

--	UPDATE UserAccounts SET Branches = Branches_Id
--	GO
--	UPDATE UserAccounts SET Interest = RTRIM(LTRIM(Interest))
--	GO
--	UPDATE UserAccounts SET Interest = NULL WHERE Interest = ''
--	GO
	
---- UPDATE USER ACCOUNT ROLES ============================================================================================

--	-- drop table if already exists
--	IF(SELECT object_id('TempDB..#TEMP_INPUTARRAY')) IS NOT NULL
--		DROP TABLE #TEMP_INPUTARRAY
		
--	SELECT * INTO #TEMP_INPUTARRAY FROM (
--			SELECT TOP 2000 AspNetUserRoles.*, 
--				ROW_NUMBER() OVER (ORDER BY AspNetUserRoles.UserId ASC) AS InitialRowNumber
--			FROM AspNetUserRoles
--			ORDER BY AspNetUserRoles.UserId ASC
--		) AS x
	
--	DECLARE @InitialRowNumber int;
--	DECLARE @UserId varchar(128);
--	DECLARE @RoleId varchar(128);
--	DECLARE @RolesList varchar(MAX);
--	WHILE EXISTS(SELECT * FROM #TEMP_INPUTARRAY)
--	BEGIN
--		SELECT TOP 1 @InitialRowNumber = InitialRowNumber FROM #TEMP_INPUTARRAY
--		SELECT @UserId = UserId, @RoleId = RoleId FROM #TEMP_INPUTARRAY WHERE InitialRowNumber = @InitialRowNumber

--		SELECT @RolesList = Roles FROM UserAccounts WHERE Id = CONVERT(UNIQUEIDENTIFIER, @UserId)
--		IF @RolesList = '' OR @RolesList IS NULL
--			SET @RolesList = @RoleId;
--		ELSE
--			SET @RolesList = @RolesList + ',' + @RoleId

--		--SELECT @UserId AS UserId, @RoleId AS RoleId, @RolesList AS RolesList
--		--BREAK;

--		-- add operation here
--		UPDATE UserAccounts SET Roles = @RolesList WHERE Id = CONVERT(UNIQUEIDENTIFIER, @UserId)
		
--		-- remove row to iterate to the next row
--		DELETE #TEMP_INPUTARRAY WHERE InitialRowNumber = @InitialRowNumber
--	END
	
--	-- clean up
--	DROP TABLE #TEMP_INPUTARRAY;
--	GO
	
---- UPDATE USER ACCOUNT BRANCHES =========================================================================================

--	-- drop table if already exists
--	IF(SELECT object_id('TempDB..#TEMP_INPUTARRAY')) IS NOT NULL
--		DROP TABLE #TEMP_INPUTARRAY
		
--	SELECT * INTO #TEMP_INPUTARRAY FROM (SELECT * FROM UserAccounts) AS x
	
--	DECLARE @Id uniqueidentifier;
--	DECLARE @SecondBranch uniqueidentifier;
--	DECLARE @Branches varchar(MAX);
--	WHILE EXISTS(SELECT * FROM #TEMP_INPUTARRAY)
--	BEGIN
--		SELECT TOP 1 @Id = Id, @Branches = Branches FROM #TEMP_INPUTARRAY
		
--		SELECT @SecondBranch = MAX(Branches_Id) from PayrollPaymentItems where UserAccounts_Id = @Id AND Branches_Id NOT IN (SELECT Branches_Id FROM UserAccounts WHERE Id = @Id)
--		IF @SecondBranch IS NOT NULL
--			SET @Branches = @Branches + ',' + CONVERT(VARCHAR(1000),@SecondBranch)

--		-- add operation here
--		UPDATE UserAccounts SET Branches = @Branches WHERE Id = @Id
		
--		-- remove row to iterate to the next row
--		DELETE #TEMP_INPUTARRAY WHERE Id = @Id
--	END
	
--	-- clean up
--	DROP TABLE #TEMP_INPUTARRAY;
--	GO
	
---- USER ACCOUNT ROLES MASTER DATA =======================================================================================

--	IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'UserAccountRoles'))
--		DROP TABLE UserAccountRoles;
	
--	CREATE TABLE [dbo].[UserAccountRoles] (
--		[Id]                   UNIQUEIDENTIFIER NOT NULL,
--		[Name]                 VARCHAR (MAX)   NOT NULL,
--		[Notes]                VARCHAR (MAX)   NULL
--	);
--	GO
	
--	-- drop table if already exists
--	IF(SELECT object_id('TempDB..#TEMP_INPUTARRAY')) IS NOT NULL
--		DROP TABLE #TEMP_INPUTARRAY
--	SELECT * INTO #TEMP_INPUTARRAY FROM (SELECT * FROM AspNetRoles) AS x
	
--	DECLARE @Iteration_Id nvarchar(128);
--	WHILE EXISTS(SELECT * FROM #TEMP_INPUTARRAY)
--	BEGIN
--		SELECT TOP 1 @Iteration_Id = Id FROM #TEMP_INPUTARRAY

--		-- add operation here
--		INSERT INTO UserAccountRoles(Id, Name) 
--			VALUES(
--				(SELECT CONVERT(UNIQUEIDENTIFIER, Id) FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
--				(SELECT Name FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id)
--			)
		
--		-- remove row to iterate to the next row
--		DELETE #TEMP_INPUTARRAY WHERE Id = @Iteration_Id
--	END
	
--	-- clean up
--	DROP TABLE #TEMP_INPUTARRAY;
--	GO
	
---- ACTIVITY LOGS ========================================================================================================

--	IF EXISTS (SELECT 1
--				   FROM   INFORMATION_SCHEMA.COLUMNS
--				   WHERE  TABLE_NAME = 'ActivityLogs'
--						  AND COLUMN_NAME = 'TableName'
--						  AND TABLE_SCHEMA='dbo')
--	BEGIN
--		EXEC sp_rename 'ActivityLogs', 'ActivityLogsToDelete'
			
--		IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'ActivityLogs'))
--			DROP TABLE ActivityLogs;
	
--		CREATE TABLE [dbo].[ActivityLogs] (
--			[Id] uniqueidentifier NOT NULL,
--			[Timestamp] datetime NOT NULL,
--			[ReferenceId] uniqueidentifier NOT NULL,
--			[Description] varchar(max) NOT NULL,
--			[UserAccounts_Id] uniqueidentifier NOT NULL,
--			[UserAccounts_Fullname] varchar(MAX) NULL
--		);
	
--		-- drop table if already exists
--		IF(SELECT object_id('TempDB..#TEMP_INPUTARRAY')) IS NOT NULL
--			DROP TABLE #TEMP_INPUTARRAY
--		SELECT * INTO #TEMP_INPUTARRAY FROM (SELECT * FROM ActivityLogsToDelete) AS x
	
--		DECLARE @Iteration_Id uniqueidentifier;
--		WHILE EXISTS(SELECT * FROM #TEMP_INPUTARRAY)
--		BEGIN
--			SELECT TOP 1 @Iteration_Id = Id FROM #TEMP_INPUTARRAY

--			-- add operation here
--			INSERT INTO ActivityLogs(Id, Timestamp, ReferenceId, Description, UserAccounts_Id, UserAccounts_Fullname) 
--				VALUES(
--					(SELECT Id FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
--					(SELECT Timestamp FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
--					(SELECT ReferenceId FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
--					(SELECT Description FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
--					(SELECT UserAccounts_Id FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
--					null
--				)
		
--			-- remove row to iterate to the next row
--			DELETE #TEMP_INPUTARRAY WHERE Id = @Iteration_Id
--		END
	
--		-- clean up
--		DROP TABLE #TEMP_INPUTARRAY;

--		DROP TABLE ActivityLogsToDelete;

--	END
--	GO

---- SERVICES =============================================================================================================

--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Name' AND TABLE_NAME = 'Services' AND TABLE_SCHEMA='dbo')
--	BEGIN	
--		ALTER TABLE Services ADD [Name] varchar(MAX) NULL;
--		ALTER TABLE Services ALTER COLUMN Description varchar(MAX) NULL;
--	END
--	GO

--	UPDATE Services SET Name=Description, Description = NULL;
--	ALTER TABLE Services ALTER COLUMN [Name] varchar(MAX) NOT NULL;
--	GO
	
---- PRODUCTS =============================================================================================================

--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Name' AND TABLE_NAME = 'Products' AND TABLE_SCHEMA='dbo') 
--	BEGIN
--		ALTER TABLE Products ADD [Name] varchar(MAX) NULL;
--		ALTER TABLE Products ALTER COLUMN Description varchar(MAX) NULL;	
--	END
--	GO

--	UPDATE Products SET Name=Description, Description = NULL;	
--	ALTER TABLE Products ALTER COLUMN [Name] varchar(MAX) NOT NULL;	
--	GO
		
---- SALE INVOICES AND SALE INVOICE ITEMS =================================================================================

--	ALTER TABLE SaleInvoiceItems ALTER COLUMN RowNo int NOT NULL;
--	ALTER TABLE SaleInvoiceItems ALTER COLUMN DiscountAmount int NOT NULL;
--	ALTER TABLE SaleInvoiceItems ADD Vouchers varchar(MAX) NULL;
--	ALTER TABLE SaleInvoiceItems ADD VouchersName varchar(MAX) NULL;
--	ALTER TABLE Vouchers ALTER COLUMN Amount int NOT NULL;
--	ALTER TABLE SaleInvoices ALTER COLUMN Customer_UserAccounts_Id uniqueidentifier NOT NULL;
	
--	-- move all vouchers to column and delete table SaleInvoiceItems_Vouchers--------------------------------------------
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Vouchers' AND TABLE_NAME = 'SaleInvoiceItems' AND TABLE_SCHEMA='dbo') 
--		ALTER TABLE SaleInvoiceItems ADD Vouchers varchar(MAX) NULL;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'VouchersAmount' AND TABLE_NAME = 'SaleInvoiceItems' AND TABLE_SCHEMA='dbo') 
--		ALTER TABLE SaleInvoiceItems ADD VouchersAmount int NULL DEFAULT 0;
--	GO

--	UPDATE SaleInvoiceItems SET VouchersAmount=0;
	
--	-- drop table if already exists
--	IF(SELECT object_id('TempDB..#TEMP_INPUTARRAY')) IS NOT NULL
--		DROP TABLE #TEMP_INPUTARRAY
--	SELECT * INTO #TEMP_INPUTARRAY FROM (SELECT * FROM SaleInvoiceItems_Vouchers) AS x
	
--	DECLARE @Iteration_Id uniqueidentifier;
--	DECLARE @Vouchers varchar(MAX);
--	DECLARE @VouchersName varchar(MAX);
--	DECLARE @Amount int;
--	WHILE EXISTS(SELECT * FROM #TEMP_INPUTARRAY)
--	BEGIN
--		SELECT TOP 1 @Iteration_Id = Id, @Vouchers = Voucher_Ids, @Amount = Amount FROM #TEMP_INPUTARRAY

--		UPDATE SaleInvoiceItems 
--		SET SaleInvoiceItems.Vouchers=@Vouchers, SaleInvoiceItems.VouchersAmount=@Amount
--		WHERE SaleInvoiceItems.SaleInvoiceItems_Vouchers_Id = @Iteration_Id
		
--		-- remove row to iterate to the next row
--		DELETE #TEMP_INPUTARRAY WHERE Id = @Iteration_Id
--	END
	
--	-- clean up
--	DROP TABLE #TEMP_INPUTARRAY;
--	GO	

--	--ALTER TABLE SaleInvoiceItems ALTER COLUMN RowNo tinyint NOT NULL;
--	--ALTER TABLE SaleInvoiceItems ALTER COLUMN DiscountAmount decimal NOT NULL;
--	--ALTER TABLE Vouchers ALTER COLUMN Amount decimal NOT NULL;
--	--ALTER TABLE SaleInvoiceItems ALTER COLUMN Customer_UserAccounts_Id nvarchar(128) NOT NULL;
	
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'CancelNotes' AND TABLE_NAME = 'SaleInvoices' AND TABLE_SCHEMA='dbo') 
--	BEGIN
--		ALTER TABLE SaleInvoices ADD CancelNotes varchar(MAX) NULL;
--	END

--	--UPDATE SaleInvoices SET CancelNotes=Notes, Notes=NULL WHERE Cancelled=1;
	
---- PAYMENTS =============================================================================================================

--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'CancelNotes' AND TABLE_NAME = 'Payments' AND TABLE_SCHEMA='dbo') 
--		ALTER TABLE Payments ADD CancelNotes varchar(MAX) NULL;
--	GO
--	UPDATE Payments SET CancelNotes=CancelNotes WHERE Cancelled=1;

	
---- PETTY CASH RECORDS ===================================================================================================

--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'UserAccounts_Id_TEMP' AND TABLE_NAME = 'PettyCashRecords' AND TABLE_SCHEMA='dbo') 
--		ALTER TABLE PettyCashRecords ADD UserAccounts_Id_TEMP uniqueidentifier NULL;
--	GO
	
--	UPDATE PettyCashRecords SET UserAccounts_Id_TEMP = CONVERT(UNIQUEIDENTIFIER, UserAccounts_Id)
	
---- LESSON SESSIONS ======================================================================================================

--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Tutor_UserAccounts_Id_TEMP' AND TABLE_NAME = 'LessonSessions' AND TABLE_SCHEMA='dbo') 
--		ALTER TABLE LessonSessions ADD Tutor_UserAccounts_Id_TEMP uniqueidentifier NULL;
--	GO
	
--	ALTER TABLE LessonSessions ALTER COLUMN Tutor_UserAccounts_Id nvarchar(128) NULL;	
--	UPDATE LessonSessions SET Tutor_UserAccounts_Id_TEMP = CONVERT(UNIQUEIDENTIFIER, Tutor_UserAccounts_Id)
--	ALTER TABLE LessonSessions ALTER COLUMN Tutor_UserAccounts_Id_TEMP uniqueidentifier NOT NULL;	
	
---- HOURLY PAYRATES ======================================================================================================

--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'UserAccounts_Id_TEMP' AND TABLE_NAME = 'HourlyRates' AND TABLE_SCHEMA='dbo') 
--		ALTER TABLE HourlyRates ADD UserAccounts_Id_TEMP uniqueidentifier NULL;
--	GO
	
--	UPDATE HourlyRates SET UserAccounts_Id_TEMP = CONVERT(UNIQUEIDENTIFIER, UserAccounts_Id)
--	ALTER TABLE HourlyRates ALTER COLUMN UserAccounts_Id nvarchar(128) NULL;	
	
---- PAYROLL PAYMENT ITEMS ================================================================================================

--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'UserAccounts_Id_TEMP' AND TABLE_NAME = 'PayrollPaymentItems' AND TABLE_SCHEMA='dbo') 
--		ALTER TABLE PayrollPaymentItems ADD UserAccounts_Id_TEMP uniqueidentifier NULL;
--	GO
	
--	UPDATE PayrollPaymentItems SET UserAccounts_Id_TEMP = CONVERT(UNIQUEIDENTIFIER, UserAccounts_Id)
--	ALTER TABLE PayrollPaymentItems ALTER COLUMN UserAccounts_Id nvarchar(128) NULL;	
	
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'IsFullTime' AND TABLE_NAME = 'PayrollPaymentItems' AND TABLE_SCHEMA='dbo') 
--		ALTER TABLE PayrollPaymentItems ADD IsFullTime bit NULL DEFAULT 0;
--	GO
--	UPDATE PayrollPaymentItems SET IsFullTime = 1 WHERE Description LIKE '%Payroll%'
--	UPDATE PayrollPaymentItems SET IsFullTime = 0 WHERE IsFullTime IS NULL
	
---- PAYROLL PAYMENTS =====================================================================================================

--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'UserAccounts_Id_TEMP' AND TABLE_NAME = 'PayrollPayments' AND TABLE_SCHEMA='dbo') 
--		ALTER TABLE PayrollPayments ADD UserAccounts_Id_TEMP uniqueidentifier NULL;
--	GO
	
--	UPDATE PayrollPayments SET UserAccounts_Id_TEMP = CONVERT(UNIQUEIDENTIFIER, UserAccounts_Id)
--	ALTER TABLE PayrollPayments ALTER COLUMN UserAccounts_Id nvarchar(128) NULL;	
	
---- ADD ROLE ACCESSES ====================================================================================================

--	--Reminders
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Reminders_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Reminders_Notes varchar(MAX) null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Reminders_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Reminders_Add bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Reminders_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Reminders_View bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Reminders_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Reminders_Edit bit default 0 not null;
--	GO
	
--	--Birthdays
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Birthdays_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Birthdays_Notes varchar(MAX) null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Birthdays_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Birthdays_View bit default 0 not null;
--	GO
	
--	--UserAccounts
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'UserAccounts_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD UserAccounts_Notes varchar(MAX) null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'UserAccounts_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD UserAccounts_Add bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'UserAccounts_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD UserAccounts_View bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'UserAccounts_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD UserAccounts_Edit bit default 0 not null;
--	GO
	
--	--UserAccountRoles
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'UserAccountRoles_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD UserAccountRoles_Notes varchar(MAX) null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'UserAccountRoles_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD UserAccountRoles_Add bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'UserAccountRoles_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD UserAccountRoles_View bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'UserAccountRoles_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD UserAccountRoles_Edit bit default 0 not null;
--	GO
	
--	--Settings
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Settings_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Settings_Notes varchar(MAX) null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Settings_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Settings_View bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Settings_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Settings_Edit bit default 0 not null;
--	GO
	
--	--Branches
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Branches_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Branches_Notes varchar(MAX) null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Branches_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Branches_Add bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Branches_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Branches_View bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Branches_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Branches_Edit bit default 0 not null;
--	GO
	
--	--PromotionEvents
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PromotionEvents_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD PromotionEvents_Notes varchar(MAX) null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PromotionEvents_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD PromotionEvents_Add bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PromotionEvents_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD PromotionEvents_View bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PromotionEvents_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD PromotionEvents_Edit bit default 0 not null;
--	GO
	
--	--PettyCashRecordsCategories
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PettyCashRecordsCategories_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD PettyCashRecordsCategories_Notes varchar(MAX) null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PettyCashRecordsCategories_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD PettyCashRecordsCategories_Add bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PettyCashRecordsCategories_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD PettyCashRecordsCategories_View bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PettyCashRecordsCategories_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD PettyCashRecordsCategories_Edit bit default 0 not null;
--	GO
	
--	--Languages
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Languages_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Languages_Notes varchar(MAX) null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Languages_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Languages_Add bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Languages_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Languages_View bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Languages_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Languages_Edit bit default 0 not null;
--	GO
	
--	--LessonTypes
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'LessonTypes_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD LessonTypes_Notes varchar(MAX) null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'LessonTypes_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD LessonTypes_Add bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'LessonTypes_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD LessonTypes_View bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'LessonTypes_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD LessonTypes_Edit bit default 0 not null;
--	GO
	
--	--LessonPackages
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'LessonPackages_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD LessonPackages_Notes varchar(MAX) null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'LessonPackages_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD LessonPackages_Add bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'LessonPackages_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD LessonPackages_View bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'LessonPackages_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD LessonPackages_Edit bit default 0 not null;
--	GO
	
--	--Consignments
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Consignments_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Consignments_Notes varchar(MAX) null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Consignments_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Consignments_Add bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Consignments_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Consignments_View bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Consignments_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Consignments_Edit bit default 0 not null;
--	GO
	
--	--Vouchers
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Vouchers_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Vouchers_Notes varchar(MAX) null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Vouchers_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Vouchers_Add bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Vouchers_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Vouchers_View bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Vouchers_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Vouchers_Edit bit default 0 not null;
--	GO
	
--	--Suppliers
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Suppliers_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Suppliers_Notes varchar(MAX) null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Suppliers_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Suppliers_Add bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Suppliers_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Suppliers_View bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Suppliers_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Suppliers_Edit bit default 0 not null;
--	GO
	
--	--Units
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Units_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Units_Notes varchar(MAX) null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Units_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Units_Add bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Units_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Units_View bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Units_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Units_Edit bit default 0 not null;
--	GO
	
--	--ExpenseCategories
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'ExpenseCategories_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD ExpenseCategories_Notes varchar(MAX) null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'ExpenseCategories_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD ExpenseCategories_Add bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'ExpenseCategories_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD ExpenseCategories_View bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'ExpenseCategories_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD ExpenseCategories_Edit bit default 0 not null;
--	GO
	
--	--Services
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Services_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Services_Notes varchar(MAX) null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Services_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Services_Add bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Services_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Services_View bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Services_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Services_Edit bit default 0 not null;
--	GO
	
--	--Products
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Products_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Products_Notes varchar(MAX) null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Products_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Products_Add bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Products_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Products_View bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Products_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Products_Edit bit default 0 not null;
--	GO
	
--	--SaleInvoices
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'SaleInvoices_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD SaleInvoices_Notes varchar(MAX) null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'SaleInvoices_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD SaleInvoices_Add bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'SaleInvoices_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD SaleInvoices_View bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'SaleInvoices_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD SaleInvoices_Edit bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'SaleInvoices_Approve' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD SaleInvoices_Approve bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'SaleInvoices_TutorTravelCost_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD SaleInvoices_TutorTravelCost_View bit default 0 not null;
--	GO
	
--	--Payments
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Payments_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Payments_Notes varchar(MAX) null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Payments_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Payments_Add bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Payments_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Payments_View bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Payments_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Payments_Edit bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Payments_Approve' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Payments_Approve bit default 0 not null;
--	GO
	
--	--PettyCashRecords
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PettyCashRecords_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD PettyCashRecords_Notes varchar(MAX) null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PettyCashRecords_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD PettyCashRecords_Add bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PettyCashRecords_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD PettyCashRecords_View bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PettyCashRecords_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD PettyCashRecords_Edit bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PettyCashRecords_Approve' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD PettyCashRecords_Approve bit default 0 not null;
--	GO
	
--	--Inventory
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Inventory_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Inventory_Notes varchar(MAX) null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Inventory_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Inventory_Add bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Inventory_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Inventory_View bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Inventory_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD Inventory_Edit bit default 0 not null;
--	GO
	
--	--LessonSessions
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'LessonSessions_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD LessonSessions_Notes varchar(MAX) null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'LessonSessions_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD LessonSessions_Add bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'LessonSessions_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD LessonSessions_View bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'LessonSessions_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD LessonSessions_Edit bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'LessonSessions_EditReviewAndInternalNotes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD LessonSessions_EditReviewAndInternalNotes bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'LessonSessions_InternalNotes_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD LessonSessions_InternalNotes_View bit default 0 not null;
--	GO
	
--	--HourlyRates
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'HourlyRates_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD HourlyRates_Notes varchar(MAX) null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'HourlyRates_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD HourlyRates_Add bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'HourlyRates_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD HourlyRates_View bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'HourlyRates_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD HourlyRates_Edit bit default 0 not null;
--	GO
	
--	--PayrollPayments
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PayrollPayments_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD PayrollPayments_Notes varchar(MAX) null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PayrollPayments_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD PayrollPayments_Add bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PayrollPayments_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD PayrollPayments_View bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PayrollPayments_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD PayrollPayments_Edit bit default 0 not null;
--	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PayrollPayments_Approve' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
--	ALTER TABLE UserAccountRoles ADD PayrollPayments_Approve bit default 0 not null;
--	GO
	
---- SET ACCESS TO ROLES ==================================================================================================

--	-- superuser
--	update UserAccountRoles set
--		Reminders_Add=1,
--		Reminders_Edit=1,
--		Reminders_View=1,
--		UserAccounts_Add=1,
--		UserAccounts_Edit=1,
--		UserAccounts_View=1,
--		UserAccountRoles_Add=1,
--		UserAccountRoles_Edit=1,
--		UserAccountRoles_View=1,
--		Settings_Edit=1,
--		Settings_View=1,
--		Branches_Add=1,
--		Branches_Edit=1,
--		Branches_View=1,
--		PromotionEvents_Add=1,
--		PromotionEvents_Edit=1,
--		PromotionEvents_View=1,
--		PettyCashRecords_Add=1,
--		PettyCashRecords_Approve=1,
--		PettyCashRecords_Edit=1,
--		PettyCashRecords_View=1,
--		PettyCashRecordsCategories_Add=1,
--		PettyCashRecordsCategories_Edit=1,
--		PettyCashRecordsCategories_View=1,
--		Languages_Add=1,
--		Languages_Edit=1,
--		Languages_View=1,
--		LessonTypes_Add=1,
--		LessonTypes_Edit=1,
--		LessonTypes_View=1,
--		LessonPackages_Add=1,
--		LessonPackages_Edit=1,
--		LessonPackages_View=1,
--		Consignments_Add=1,
--		Consignments_Edit=1,
--		Consignments_View=1,
--		Vouchers_Add=1,
--		Vouchers_Edit=1,
--		Vouchers_View=1,
--		Suppliers_Add=1,
--		Suppliers_Edit=1,
--		Suppliers_View=1,
--		Units_Add=1,
--		Units_Edit=1,
--		Units_View=1,
--		ExpenseCategories_Add=1,
--		ExpenseCategories_Edit=1,
--		ExpenseCategories_View=1,
--		Services_Add=1,
--		Services_Edit=1,
--		Services_View=1,
--		Products_Add=1,
--		Products_Edit=1,
--		Products_View=1,
--		SaleInvoices_Add=1,
--		SaleInvoices_Approve=1,
--		SaleInvoices_Edit=1,
--		SaleInvoices_TutorTravelCost_View=1,
--		SaleInvoices_View=1,
--		Payments_Add=1,
--		Payments_Approve=1,
--		Payments_Edit=1,
--		Payments_View=1,
--		Inventory_Add=1,
--		Inventory_Edit=1,
--		Inventory_View=1,
--		LessonSessions_Add=1,
--		LessonSessions_Edit=1,
--		LessonSessions_View=1,
--		LessonSessions_InternalNotes_View=1,
--		HourlyRates_Add=1,
--		HourlyRates_Edit=1,
--		HourlyRates_View=1,
--		PayrollPayments_Add=1,
--		PayrollPayments_Approve=1,
--		PayrollPayments_Edit=1,
--		PayrollPayments_View=1,
--		Birthdays_View=1,
--		Birthdays_Notes=1	
--	where id='7FFE2278-1C25-4FAC-80F4-74BD26A63D96'
--	GO

--	--tutor
--	update UserAccountRoles set
--		Reminders_Add=0,
--		Reminders_Edit=0,
--		Reminders_View=0,
--		UserAccounts_Add=0,
--		UserAccounts_Edit=0,
--		UserAccounts_View=0,
--		UserAccountRoles_Add=0,
--		UserAccountRoles_Edit=0,
--		UserAccountRoles_View=0,
--		Settings_Edit=0,
--		Settings_View=0,
--		Branches_Add=0,
--		Branches_Edit=0,
--		Branches_View=0,
--		PromotionEvents_Add=0,
--		PromotionEvents_Edit=0,
--		PromotionEvents_View=0,
--		PettyCashRecords_Add=0,
--		PettyCashRecords_Approve=0,
--		PettyCashRecords_Edit=0,
--		PettyCashRecords_View=0,
--		PettyCashRecordsCategories_Add=0,
--		PettyCashRecordsCategories_Edit=0,
--		PettyCashRecordsCategories_View=0,
--		Languages_Add=0,
--		Languages_Edit=0,
--		Languages_View=0,
--		LessonTypes_Add=0,
--		LessonTypes_Edit=0,
--		LessonTypes_View=0,
--		LessonPackages_Add=0,
--		LessonPackages_Edit=0,
--		LessonPackages_View=0,
--		Consignments_Add=0,
--		Consignments_Edit=0,
--		Consignments_View=0,
--		Vouchers_Add=0,
--		Vouchers_Edit=0,
--		Vouchers_View=0,
--		Suppliers_Add=0,
--		Suppliers_Edit=0,
--		Suppliers_View=0,
--		Units_Add=0,
--		Units_Edit=0,
--		Units_View=0,
--		ExpenseCategories_Add=0,
--		ExpenseCategories_Edit=0,
--		ExpenseCategories_View=0,
--		Services_Add=0,
--		Services_Edit=0,
--		Services_View=0,
--		Products_Add=0,
--		Products_Edit=0,
--		Products_View=0,
--		SaleInvoices_Add=0,
--		SaleInvoices_Approve=0,
--		SaleInvoices_Edit=0,
--		SaleInvoices_TutorTravelCost_View=0,
--		SaleInvoices_View=0,
--		Payments_Add=0,
--		Payments_Approve=0,
--		Payments_Edit=0,
--		Payments_View=0,
--		Inventory_Add=0,
--		Inventory_Edit=0,
--		Inventory_View=0,
--		LessonSessions_Add=0,
--		LessonSessions_Edit=0,
--		LessonSessions_View=1,
--		LessonSessions_InternalNotes_View=1,
--		HourlyRates_Add=0,
--		HourlyRates_Edit=0,
--		HourlyRates_View=0,
--		PayrollPayments_Add=0,
--		PayrollPayments_Approve=0,
--		PayrollPayments_Edit=0,
--		PayrollPayments_View=0,
--		Birthdays_View=0,
--		Birthdays_Notes=0		
--	where id='305678D6-7100-4D7E-8264-569E2491EB12'
--	GO
		
--	--assistant
--	update UserAccountRoles set
--		Reminders_Add=0,
--		Reminders_Edit=0,
--		Reminders_View=0,
--		UserAccounts_Add=0,
--		UserAccounts_Edit=0,
--		UserAccounts_View=0,
--		UserAccountRoles_Add=0,
--		UserAccountRoles_Edit=0,
--		UserAccountRoles_View=0,
--		Settings_Edit=0,
--		Settings_View=0,
--		Branches_Add=0,
--		Branches_Edit=0,
--		Branches_View=0,
--		PromotionEvents_Add=0,
--		PromotionEvents_Edit=0,
--		PromotionEvents_View=0,
--		PettyCashRecords_Add=0,
--		PettyCashRecords_Approve=0,
--		PettyCashRecords_Edit=0,
--		PettyCashRecords_View=0,
--		PettyCashRecordsCategories_Add=0,
--		PettyCashRecordsCategories_Edit=0,
--		PettyCashRecordsCategories_View=0,
--		Languages_Add=0,
--		Languages_Edit=0,
--		Languages_View=0,
--		LessonTypes_Add=0,
--		LessonTypes_Edit=0,
--		LessonTypes_View=0,
--		LessonPackages_Add=0,
--		LessonPackages_Edit=0,
--		LessonPackages_View=0,
--		Consignments_Add=0,
--		Consignments_Edit=0,
--		Consignments_View=0,
--		Vouchers_Add=0,
--		Vouchers_Edit=0,
--		Vouchers_View=0,
--		Suppliers_Add=0,
--		Suppliers_Edit=0,
--		Suppliers_View=0,
--		Units_Add=0,
--		Units_Edit=0,
--		Units_View=0,
--		ExpenseCategories_Add=0,
--		ExpenseCategories_Edit=0,
--		ExpenseCategories_View=0,
--		Services_Add=0,
--		Services_Edit=0,
--		Services_View=0,
--		Products_Add=0,
--		Products_Edit=0,
--		Products_View=0,
--		SaleInvoices_Add=0,
--		SaleInvoices_Approve=0,
--		SaleInvoices_Edit=0,
--		SaleInvoices_TutorTravelCost_View=0,
--		SaleInvoices_View=0,
--		Payments_Add=0,
--		Payments_Approve=0,
--		Payments_Edit=0,
--		Payments_View=0,
--		Inventory_Add=0,
--		Inventory_Edit=0,
--		Inventory_View=0,
--		LessonSessions_Add=1,
--		LessonSessions_Edit=1,
--		LessonSessions_View=1,
--		LessonSessions_InternalNotes_View=1,
--		HourlyRates_Add=0,
--		HourlyRates_Edit=0,
--		HourlyRates_View=0,
--		PayrollPayments_Add=0,
--		PayrollPayments_Approve=0,
--		PayrollPayments_Edit=0,
--		PayrollPayments_View=0,
--		Birthdays_View=0,
--		Birthdays_Notes=0	
--	where id='4D66868D-F84F-4BEA-AFE7-96D1CF250E4F'
--	GO

	
--	--admin
--	update UserAccountRoles set
--		Reminders_Add=1,
--		Reminders_Edit=1,
--		Reminders_View=1,
--		UserAccounts_Add=1,
--		UserAccounts_Edit=1,
--		UserAccounts_View=1,
--		UserAccountRoles_Add=0,
--		UserAccountRoles_Edit=0,
--		UserAccountRoles_View=0,
--		Settings_Edit=0,
--		Settings_View=0,
--		Branches_Add=0,
--		Branches_Edit=0,
--		Branches_View=0,
--		PromotionEvents_Add=0,
--		PromotionEvents_Edit=0,
--		PromotionEvents_View=0,
--		PettyCashRecords_Add=1,
--		PettyCashRecords_Approve=0,
--		PettyCashRecords_Edit=0,
--		PettyCashRecords_View=1,
--		PettyCashRecordsCategories_Add=0,
--		PettyCashRecordsCategories_Edit=0,
--		PettyCashRecordsCategories_View=0,
--		Languages_Add=0,
--		Languages_Edit=0,
--		Languages_View=0,
--		LessonTypes_Add=0,
--		LessonTypes_Edit=0,
--		LessonTypes_View=0,
--		LessonPackages_Add=0,
--		LessonPackages_Edit=0,
--		LessonPackages_View=0,
--		Consignments_Add=0,
--		Consignments_Edit=0,
--		Consignments_View=0,
--		Vouchers_Add=0,
--		Vouchers_Edit=0,
--		Vouchers_View=0,
--		Suppliers_Add=0,
--		Suppliers_Edit=0,
--		Suppliers_View=0,
--		Units_Add=0,
--		Units_Edit=0,
--		Units_View=0,
--		ExpenseCategories_Add=0,
--		ExpenseCategories_Edit=0,
--		ExpenseCategories_View=0,
--		Services_Add=0,
--		Services_Edit=0,
--		Services_View=0,
--		Products_Add=0,
--		Products_Edit=0,
--		Products_View=0,
--		SaleInvoices_Add=1,
--		SaleInvoices_Approve=0,
--		SaleInvoices_Edit=1,
--		SaleInvoices_TutorTravelCost_View=1,
--		SaleInvoices_View=1,
--		Payments_Add=1,
--		Payments_Approve=0,
--		Payments_Edit=1,
--		Payments_View=1,
--		Inventory_Add=1,
--		Inventory_Edit=0,
--		Inventory_View=1,
--		LessonSessions_Add=1,
--		LessonSessions_Edit=1,
--		LessonSessions_View=1,
--		LessonSessions_InternalNotes_View=1,
--		HourlyRates_Add=0,
--		HourlyRates_Edit=0,
--		HourlyRates_View=0,
--		PayrollPayments_Add=0,
--		PayrollPayments_Approve=0,
--		PayrollPayments_Edit=0,
--		PayrollPayments_View=0,
--		Birthdays_View=1,
--		Birthdays_Notes=1	
--	where id='9101FA61-3F6B-4A36-875E-E6283BD36FDF'
--	GO

	
--	--student
--	update UserAccountRoles set
--		Reminders_Add=0,
--		Reminders_Edit=0,
--		Reminders_View=0,
--		UserAccounts_Add=0,
--		UserAccounts_Edit=0,
--		UserAccounts_View=0,
--		UserAccountRoles_Add=0,
--		UserAccountRoles_Edit=0,
--		UserAccountRoles_View=0,
--		Settings_Edit=0,
--		Settings_View=0,
--		Branches_Add=0,
--		Branches_Edit=0,
--		Branches_View=0,
--		PromotionEvents_Add=0,
--		PromotionEvents_Edit=0,
--		PromotionEvents_View=0,
--		PettyCashRecords_Add=0,
--		PettyCashRecords_Approve=0,
--		PettyCashRecords_Edit=0,
--		PettyCashRecords_View=0,
--		PettyCashRecordsCategories_Add=0,
--		PettyCashRecordsCategories_Edit=0,
--		PettyCashRecordsCategories_View=0,
--		Languages_Add=0,
--		Languages_Edit=0,
--		Languages_View=0,
--		LessonTypes_Add=0,
--		LessonTypes_Edit=0,
--		LessonTypes_View=0,
--		LessonPackages_Add=0,
--		LessonPackages_Edit=0,
--		LessonPackages_View=0,
--		Consignments_Add=0,
--		Consignments_Edit=0,
--		Consignments_View=0,
--		Vouchers_Add=0,
--		Vouchers_Edit=0,
--		Vouchers_View=0,
--		Suppliers_Add=0,
--		Suppliers_Edit=0,
--		Suppliers_View=0,
--		Units_Add=0,
--		Units_Edit=0,
--		Units_View=0,
--		ExpenseCategories_Add=0,
--		ExpenseCategories_Edit=0,
--		ExpenseCategories_View=0,
--		Services_Add=0,
--		Services_Edit=0,
--		Services_View=0,
--		Products_Add=0,
--		Products_Edit=0,
--		Products_View=0,
--		SaleInvoices_Add=0,
--		SaleInvoices_Approve=0,
--		SaleInvoices_Edit=0,
--		SaleInvoices_TutorTravelCost_View=0,
--		SaleInvoices_View=1,
--		Payments_Add=0,
--		Payments_Approve=0,
--		Payments_Edit=0,
--		Payments_View=0,
--		Inventory_Add=0,
--		Inventory_Edit=0,
--		Inventory_View=0,
--		LessonSessions_Add=0,
--		LessonSessions_Edit=0,
--		LessonSessions_View=1,
--		LessonSessions_InternalNotes_View=0,
--		HourlyRates_Add=0,
--		HourlyRates_Edit=0,
--		HourlyRates_View=0,
--		PayrollPayments_Add=0,
--		PayrollPayments_Approve=0,
--		PayrollPayments_Edit=0,
--		PayrollPayments_View=0,
--		Birthdays_View=0,
--		Birthdays_Notes=0	
--	where id='A6DCC946-9AC7-4C07-A367-23D5ED91493D'
--	GO

	
--	--manager
--	update UserAccountRoles set
--		Reminders_Add=1,
--		Reminders_Edit=1,
--		Reminders_View=1,
--		UserAccounts_Add=1,
--		UserAccounts_Edit=1,
--		UserAccounts_View=1,
--		UserAccountRoles_Add=0,
--		UserAccountRoles_Edit=0,
--		UserAccountRoles_View=1,
--		Settings_Edit=0,
--		Settings_View=1,
--		Branches_Add=0,
--		Branches_Edit=0,
--		Branches_View=1,
--		PromotionEvents_Add=1,
--		PromotionEvents_Edit=1,
--		PromotionEvents_View=1,
--		PettyCashRecords_Add=1,
--		PettyCashRecords_Approve=1,
--		PettyCashRecords_Edit=1,
--		PettyCashRecords_View=1,
--		PettyCashRecordsCategories_Add=1,
--		PettyCashRecordsCategories_Edit=1,
--		PettyCashRecordsCategories_View=1,
--		Languages_Add=1,
--		Languages_Edit=1,
--		Languages_View=1,
--		LessonTypes_Add=1,
--		LessonTypes_Edit=1,
--		LessonTypes_View=1,
--		LessonPackages_Add=1,
--		LessonPackages_Edit=1,
--		LessonPackages_View=1,
--		Consignments_Add=1,
--		Consignments_Edit=1,
--		Consignments_View=1,
--		Vouchers_Add=1,
--		Vouchers_Edit=1,
--		Vouchers_View=1,
--		Suppliers_Add=1,
--		Suppliers_Edit=1,
--		Suppliers_View=1,
--		Units_Add=1,
--		Units_Edit=1,
--		Units_View=1,
--		ExpenseCategories_Add=1,
--		ExpenseCategories_Edit=1,
--		ExpenseCategories_View=1,
--		Services_Add=1,
--		Services_Edit=1,
--		Services_View=1,
--		Products_Add=1,
--		Products_Edit=1,
--		Products_View=1,
--		SaleInvoices_Add=1,
--		SaleInvoices_Approve=1,
--		SaleInvoices_Edit=1,
--		SaleInvoices_TutorTravelCost_View=1,
--		SaleInvoices_View=1,
--		Payments_Add=1,
--		Payments_Approve=1,
--		Payments_Edit=1,
--		Payments_View=1,
--		Inventory_Add=1,
--		Inventory_Edit=1,
--		Inventory_View=1,
--		LessonSessions_Add=1,
--		LessonSessions_Edit=1,
--		LessonSessions_View=1,
--		LessonSessions_InternalNotes_View=1,
--		HourlyRates_Add=1,
--		HourlyRates_Edit=1,
--		HourlyRates_View=1,
--		PayrollPayments_Add=1,
--		PayrollPayments_Approve=1,
--		PayrollPayments_Edit=1,
--		PayrollPayments_View=1,
--		Birthdays_View=1,
--		Birthdays_Notes=1	
--	where id='C8C12756-53B1-4D7E-802F-6A0C89F132DC'
--	GO

---- ======================================================================================================================

--update saleinvoices set CancelNotes=Notes where Cancelled = 1
--update saleinvoices set Notes=null where Cancelled = 1
--GO


--update UserAccounts set Fullname = TRIM(Fullname) 
--GO

--ALTER TABLE SaleInvoiceItems DROP COLUMN SaleInvoiceItems_Vouchers_Id;

--ALTER TABLE PettyCashRecords DROP COLUMN UserAccounts_Id;
--EXEC sp_RENAME 'PettyCashRecords.UserAccounts_Id_TEMP' , 'UserAccounts_Id', 'COLUMN'

--ALTER TABLE LessonSessions DROP COLUMN Tutor_UserAccounts_Id;
--EXEC sp_RENAME 'LessonSessions.Tutor_UserAccounts_Id_TEMP' , 'Tutor_UserAccounts_Id', 'COLUMN'

--ALTER TABLE PayrollPayments DROP COLUMN UserAccounts_Id;
--EXEC sp_RENAME 'PayrollPayments.UserAccounts_Id_TEMP' , 'UserAccounts_Id', 'COLUMN'

--ALTER TABLE PayrollPaymentItems DROP COLUMN UserAccounts_Id;
--EXEC sp_RENAME 'PayrollPaymentItems.UserAccounts_Id_TEMP' , 'UserAccounts_Id', 'COLUMN'

--ALTER TABLE HourlyRates DROP COLUMN UserAccounts_Id;
--EXEC sp_RENAME 'HourlyRates.UserAccounts_Id_TEMP' , 'UserAccounts_Id', 'COLUMN'

--GO

--ALTER TABLE Payments DROP COLUMN CancelNotes;
--ALTER TABLE Inventory DROP COLUMN AvailableQty;
--GO

--INSERT INTO Settings (Id, Value_Guid) VALUES('A94B2FFC-3547-40CB-96CD-F82729768926', 'A6DCC946-9AC7-4C07-A367-23D5ED91493D');
--INSERT INTO Settings (Id, Value_Guid) VALUES('20D2F1DA-2ACC-4E92-850C-2B260848FB8F', '305678D6-7100-4D7E-8264-569E2491EB12');
--GO

