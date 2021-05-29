
-- NEED TO EXECUTE AFTER MIGRATION COMPLETE ==============================================================================

--DROP TABLE AspNetUsers
--DROP TABLE AspNetUserRoles
--DROP TABLE AspNetUserLogins
--DROP TABLE AspNetUserClaims
--DROP TABLE AspNetUserRoles
--DROP TABLE __MigrationHistory
--DROP TABLE MasterMenu
--DROP TABLE RoleAccessMenu
-- ALTER TABLE SaleInvoiceItems DROP SaleInvoiceItems_Vouchers_Id;
-- DROP TABLE SaleInvoiceItems_Vouchers
-- ALTER TABLE Payments DROP Notes_Cancel;
-- ALTER TABLE Inventory DROP AvailableQty;

-- MANUAL TABLE MODIFICATIONS ===========================================================================================

-- TABLE PettyCashRecords: need to remove UserAccounts_Id (nvarchar) and rename UserAccounts_Id_TEMP(uniqueidentifier) to UserAccounts_Id
-- TABLE LessonSessions: need to remove Tutor_UserAccounts_Id (nvarchar) and rename Tutor_UserAccounts_Id_TEMP(uniqueidentifier) to Tutor_UserAccounts_Id
-- TABLE PayrollPaymentItems: need to remove UserAccounts_Id (nvarchar) and rename UserAccounts_Id_TEMP(uniqueidentifier) to UserAccounts_Id
-- TABLE HourlyRates: need to remove UserAccounts_Id (nvarchar) and rename UserAccounts_Id_TEMP(uniqueidentifier) to UserAccounts_Id

-- NEED TO DO AFTER SCRIPT IS RUN =======================================================================================

-- assign roles to users. currently all get student role
-- all users will be prompted for password change

-- CREATE NEW USER ACCOUNTS TABLE =======================================================================================

	-- drop table if already exists
	IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'UserAccounts'))
		DROP TABLE UserAccounts;

	CREATE TABLE [dbo].[UserAccounts] (
		[Id]                   UNIQUEIDENTIFIER NOT NULL,
		[Username]             VARCHAR (MAX)   NOT NULL,
		[Password]			   VARCHAR (MAX)   NOT NULL,
		[Fullname]             VARCHAR (MAX)   NOT NULL,
		[Roles]				   VARCHAR (MAX)   NULL,
		[Birthday]             DATE             NOT NULL,
		[Branches_Id]  UNIQUEIDENTIFIER NOT NULL,
		[Active]               BIT              DEFAULT ((1)) NOT NULL,
		[ResetPassword]		   BIT				DEFAULT ((0)) NOT NULL,
		[Address]              VARCHAR (MAX)   NULL,
		[Phone1]               VARCHAR (MAX)   NULL,
		[Phone2]               VARCHAR (MAX)   NULL,
		[Notes]                VARCHAR (MAX)   NULL,
		[Email]                VARCHAR (MAX)   NULL,
		[Interest]             VARCHAR (MAX)   NULL,
		[PromotionEvents_Id]   UNIQUEIDENTIFIER NULL
	);
	GO

-- MIGRATE USER ACCOUNTS DATA TO THE NEW USER ACCOUNTS TABLE ============================================================

	DELETE UserAccounts;

	-- drop table if already exists
	IF(SELECT object_id('TempDB..#TEMP_INPUTARRAY')) IS NOT NULL
		DROP TABLE #TEMP_INPUTARRAY
		
	SELECT * INTO #TEMP_INPUTARRAY FROM (SELECT * FROM AspNetUsers) AS x
	
	DECLARE @Iteration_Id nvarchar(128);
	WHILE EXISTS(SELECT * FROM #TEMP_INPUTARRAY)
	BEGIN
		SELECT TOP 1 @Iteration_Id = Id FROM #TEMP_INPUTARRAY

		-- add operation here
		INSERT INTO UserAccounts(Id, Username, Password, Fullname, Birthday, Branches_Id, Active, ResetPassword,
								Address, Phone1, Phone2, Notes, Email, Interest, PromotionEvents_Id,Roles) 
			VALUES(
				(SELECT CONVERT(UNIQUEIDENTIFIER, Id) FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
				(SELECT UserName FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
				'b1b3773a05c0ed0176787a4f1574ff0075f7521e',
				(SELECT RTRIM(RTRIM(Firstname + ' ' + ISNULL(Middlename,'')) + ' ' + ISNULL(Lastname,'')) FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
				(SELECT Birthday FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
				(SELECT Branches_Id FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
				(SELECT Active FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
				1,
				(SELECT Address FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
				(SELECT Phone1 FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
				(SELECT Phone2 FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
				(SELECT Notes FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
				(SELECT Email FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
				(SELECT REPLACE(REPLACE(REPLACE(Interest, '[{"Languages_Id":"', ''), '"},{"Languages_Id":"', ','), '"}]', '') FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
				(SELECT PromotionEvents_Id FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
				'A6DCC946-9AC7-4C07-A367-23D5ED91493D'
			)

		
		-- remove row to iterate to the next row
		DELETE #TEMP_INPUTARRAY WHERE Id = @Iteration_Id
	END
	
	-- clean up
	DROP TABLE #TEMP_INPUTARRAY;
	GO

	update UserAccounts set Roles='7FFE2278-1C25-4FAC-80F4-74BD26A63D96', ResetPassword=0 where fullname = 'ricky'
	GO

-- USER ACCOUNT ROLES ===================================================================================================

	IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'UserAccountRoles'))
		DROP TABLE UserAccountRoles;
	
	CREATE TABLE [dbo].[UserAccountRoles] (
		[Id]                   UNIQUEIDENTIFIER NOT NULL,
		[Name]                 VARCHAR (MAX)   NOT NULL,
		[Notes]                VARCHAR (MAX)   NULL
	);
	GO
	
	-- drop table if already exists
	IF(SELECT object_id('TempDB..#TEMP_INPUTARRAY')) IS NOT NULL
		DROP TABLE #TEMP_INPUTARRAY
	SELECT * INTO #TEMP_INPUTARRAY FROM (SELECT * FROM AspNetRoles) AS x
	
	DECLARE @Iteration_Id nvarchar(128);
	WHILE EXISTS(SELECT * FROM #TEMP_INPUTARRAY)
	BEGIN
		SELECT TOP 1 @Iteration_Id = Id FROM #TEMP_INPUTARRAY

		-- add operation here
		INSERT INTO UserAccountRoles(Id, Name) 
			VALUES(
				(SELECT CONVERT(UNIQUEIDENTIFIER, Id) FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
				(SELECT Name FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id)
			)
		
		-- remove row to iterate to the next row
		DELETE #TEMP_INPUTARRAY WHERE Id = @Iteration_Id
	END
	
	-- clean up
	DROP TABLE #TEMP_INPUTARRAY;
	GO
	
-- ACTIVITY LOGS ========================================================================================================

	IF EXISTS (SELECT 1
				   FROM   INFORMATION_SCHEMA.COLUMNS
				   WHERE  TABLE_NAME = 'ActivityLogs'
						  AND COLUMN_NAME = 'TableName'
						  AND TABLE_SCHEMA='dbo')
	BEGIN
		EXEC sp_rename 'ActivityLogs', 'ActivityLogsToDelete'
			
		IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'ActivityLogs'))
			DROP TABLE ActivityLogs;
	
		CREATE TABLE [dbo].[ActivityLogs] (
			[Id] uniqueidentifier NOT NULL,
			[Timestamp] datetime NOT NULL,
			[ReffId] uniqueidentifier NOT NULL,
			[Description] varchar(max) NOT NULL,
			[UserAccounts_Id] uniqueidentifier NOT NULL,
			[UserAccounts_Fullname] varchar(MAX) NULL
		);
	
		-- drop table if already exists
		IF(SELECT object_id('TempDB..#TEMP_INPUTARRAY')) IS NOT NULL
			DROP TABLE #TEMP_INPUTARRAY
		SELECT * INTO #TEMP_INPUTARRAY FROM (SELECT * FROM ActivityLogsToDelete) AS x
	
		DECLARE @Iteration_Id uniqueidentifier;
		WHILE EXISTS(SELECT * FROM #TEMP_INPUTARRAY)
		BEGIN
			SELECT TOP 1 @Iteration_Id = Id FROM #TEMP_INPUTARRAY

			-- add operation here
			INSERT INTO ActivityLogs(Id, Timestamp, ReffId, Description, UserAccounts_Id, UserAccounts_Fullname) 
				VALUES(
					(SELECT Id FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
					(SELECT Timestamp FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
					(SELECT RefId FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
					(SELECT Description FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
					(SELECT UserAccounts_Id FROM #TEMP_INPUTARRAY WHERE Id=@Iteration_Id),
					null
				)
		
			-- remove row to iterate to the next row
			DELETE #TEMP_INPUTARRAY WHERE Id = @Iteration_Id
		END
	
		-- clean up
		DROP TABLE #TEMP_INPUTARRAY;

		DROP TABLE ActivityLogsToDelete;

	END
	GO

-- SERVICES =============================================================================================================

	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Name' AND TABLE_NAME = 'Services' AND TABLE_SCHEMA='dbo')
	BEGIN	
		ALTER TABLE Services ADD [Name] varchar(MAX) NULL;
		ALTER TABLE Services ALTER COLUMN Description varchar(MAX) NULL;
		UPDATE Services SET Name=Description, Description = NULL;
		ALTER TABLE Services ALTER COLUMN [Name] varchar(MAX) NOT NULL;
	END
	GO
	
-- PRODUCTS =============================================================================================================

	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Name' AND TABLE_NAME = 'Products' AND TABLE_SCHEMA='dbo') 
	BEGIN
		ALTER TABLE Products ADD [Name] varchar(MAX) NULL;
		ALTER TABLE Products ALTER COLUMN Description varchar(MAX) NULL;	
		UPDATE Products SET Name=Description, Description = NULL;	
		ALTER TABLE Products ALTER COLUMN [Name] varchar(MAX) NOT NULL;	
	END
	GO
	
-- SALE INVOICES AND SALE INVOICE ITEMS =================================================================================

	ALTER TABLE SaleInvoiceItems ALTER COLUMN RowNo int NOT NULL;
	ALTER TABLE SaleInvoiceItems ALTER COLUMN DiscountAmount int NOT NULL;
	ALTER TABLE SaleInvoiceItems ADD VouchersName varchar(MAX) NULL;
	ALTER TABLE Vouchers ALTER COLUMN Amount int NOT NULL;
	ALTER TABLE SaleInvoices ALTER COLUMN Customer_UserAccounts_Id uniqueidentifier NOT NULL;
	
	-- move all vouchers to column and delete table SaleInvoiceItems_Vouchers--------------------------------------------
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Vouchers' AND TABLE_NAME = 'SaleInvoiceItems' AND TABLE_SCHEMA='dbo') 
		ALTER TABLE SaleInvoiceItems ADD Vouchers varchar(MAX) NULL;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'VouchersAmount' AND TABLE_NAME = 'SaleInvoiceItems' AND TABLE_SCHEMA='dbo') 
		ALTER TABLE SaleInvoiceItems ADD VouchersAmount int NULL DEFAULT 0;

	UPDATE SaleInvoiceItems SET VouchersAmount=0;
	
	-- drop table if already exists
	IF(SELECT object_id('TempDB..#TEMP_INPUTARRAY')) IS NOT NULL
		DROP TABLE #TEMP_INPUTARRAY
	SELECT * INTO #TEMP_INPUTARRAY FROM (SELECT * FROM SaleInvoiceItems_Vouchers) AS x
	
	DECLARE @Iteration_Id uniqueidentifier;
	DECLARE @Vouchers varchar(MAX);
	DECLARE @VouchersName varchar(MAX);
	DECLARE @Amount int;
	WHILE EXISTS(SELECT * FROM #TEMP_INPUTARRAY)
	BEGIN
		SELECT TOP 1 @Iteration_Id = Id, @Vouchers = Voucher_Ids, @Amount = Amount FROM #TEMP_INPUTARRAY

		UPDATE SaleInvoiceItems 
		SET SaleInvoiceItems.Vouchers=@Vouchers, SaleInvoiceItems.VouchersAmount=@Amount
		WHERE SaleInvoiceItems.SaleInvoiceItems_Vouchers_Id = @Iteration_Id
		
		-- remove row to iterate to the next row
		DELETE #TEMP_INPUTARRAY WHERE Id = @Iteration_Id
	END
	
	-- clean up
	DROP TABLE #TEMP_INPUTARRAY;
	GO	

	--ALTER TABLE SaleInvoiceItems ALTER COLUMN RowNo tinyint NOT NULL;
	--ALTER TABLE SaleInvoiceItems ALTER COLUMN DiscountAmount decimal NOT NULL;
	--ALTER TABLE Vouchers ALTER COLUMN Amount decimal NOT NULL;
	--ALTER TABLE SaleInvoiceItems ALTER COLUMN Customer_UserAccounts_Id nvarchar(128) NOT NULL;
	
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'CancelNotes' AND TABLE_NAME = 'SaleInvoices' AND TABLE_SCHEMA='dbo') 
	BEGIN
		ALTER TABLE SaleInvoices ADD CancelNotes varchar(MAX) NULL;
	END

	--UPDATE SaleInvoices SET CancelNotes=Notes, Notes=NULL WHERE Cancelled=1;
	
-- PAYMENTS =============================================================================================================

	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'CancelNotes' AND TABLE_NAME = 'Payments' AND TABLE_SCHEMA='dbo') 
		ALTER TABLE Payments ADD CancelNotes varchar(MAX) NULL;
	GO
	UPDATE Payments SET CancelNotes=Notes_Cancel WHERE Cancelled=1;

	
-- PETTY CASH RECORDS ===================================================================================================

	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'UserAccounts_Id_TEMP' AND TABLE_NAME = 'PettyCashRecords' AND TABLE_SCHEMA='dbo') 
		ALTER TABLE PettyCashRecords ADD UserAccounts_Id_TEMP uniqueidentifier NULL;
	GO
	
	UPDATE PettyCashRecords SET UserAccounts_Id_TEMP = CONVERT(UNIQUEIDENTIFIER, UserAccounts_Id)
	
-- LESSON SESSIONS ======================================================================================================

	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Tutor_UserAccounts_Id_TEMP' AND TABLE_NAME = 'LessonSessions' AND TABLE_SCHEMA='dbo') 
		ALTER TABLE LessonSessions ADD Tutor_UserAccounts_Id_TEMP uniqueidentifier NULL;
	GO
	
	ALTER TABLE LessonSessions ALTER COLUMN Tutor_UserAccounts_Id nvarchar(128) NULL;	
	UPDATE LessonSessions SET Tutor_UserAccounts_Id_TEMP = CONVERT(UNIQUEIDENTIFIER, Tutor_UserAccounts_Id)
	ALTER TABLE LessonSessions ALTER COLUMN Tutor_UserAccounts_Id_TEMP uniqueidentifier NOT NULL;	
	
-- HOURLY PAYRATES ======================================================================================================

	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'UserAccounts_Id_TEMP' AND TABLE_NAME = 'HourlyRates' AND TABLE_SCHEMA='dbo') 
		ALTER TABLE HourlyRates ADD UserAccounts_Id_TEMP uniqueidentifier NULL;
	GO
	
	UPDATE HourlyRates SET UserAccounts_Id_TEMP = CONVERT(UNIQUEIDENTIFIER, UserAccounts_Id)
	ALTER TABLE HourlyRates ALTER COLUMN UserAccounts_Id nvarchar(128) NULL;	
	
-- PAYROLL PAYMENT ITEMS ================================================================================================

	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'UserAccounts_Id_TEMP' AND TABLE_NAME = 'PayrollPaymentItems' AND TABLE_SCHEMA='dbo') 
		ALTER TABLE PayrollPaymentItems ADD UserAccounts_Id_TEMP uniqueidentifier NULL;
	GO
	
	UPDATE PayrollPaymentItems SET UserAccounts_Id_TEMP = CONVERT(UNIQUEIDENTIFIER, UserAccounts_Id)
	ALTER TABLE PayrollPaymentItems ALTER COLUMN UserAccounts_Id nvarchar(128) NULL;	
	
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'IsFullTime' AND TABLE_NAME = 'PayrollPaymentItems' AND TABLE_SCHEMA='dbo') 
		ALTER TABLE PayrollPaymentItems ADD IsFullTime bit NULL DEFAULT 0;
	GO
	UPDATE PayrollPaymentItems SET IsFullTime = 1 WHERE Description LIKE '%Payroll%'
	UPDATE PayrollPaymentItems SET IsFullTime = 0 WHERE IsFullTime IS NULL
	
-- PAYROLL PAYMENTS =====================================================================================================

	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'UserAccounts_Id_TEMP' AND TABLE_NAME = 'PayrollPayments' AND TABLE_SCHEMA='dbo') 
		ALTER TABLE PayrollPayments ADD UserAccounts_Id_TEMP uniqueidentifier NULL;
	GO
	
	UPDATE PayrollPayments SET UserAccounts_Id_TEMP = CONVERT(UNIQUEIDENTIFIER, UserAccounts_Id)
	ALTER TABLE PayrollPayments ALTER COLUMN UserAccounts_Id nvarchar(128) NULL;	
	
-- ADD ROLE ACCESSES ====================================================================================================

	--Reminders
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Reminders_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Reminders_Notes varchar(MAX) null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Reminders_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Reminders_Add bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Reminders_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Reminders_View bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Reminders_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Reminders_Edit bit default 0 not null;
	GO
	
	--UserAccounts
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'UserAccounts_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD UserAccounts_Notes varchar(MAX) null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'UserAccounts_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD UserAccounts_Add bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'UserAccounts_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD UserAccounts_View bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'UserAccounts_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD UserAccounts_Edit bit default 0 not null;
	GO
	
	--UserAccountRoles
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'UserAccountRoles_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD UserAccountRoles_Notes varchar(MAX) null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'UserAccountRoles_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD UserAccountRoles_Add bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'UserAccountRoles_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD UserAccountRoles_View bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'UserAccountRoles_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD UserAccountRoles_Edit bit default 0 not null;
	GO
	
	--Settings
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Settings_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Settings_Notes varchar(MAX) null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Settings_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Settings_View bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Settings_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Settings_Edit bit default 0 not null;
	GO
	
	--Branches
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Branches_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Branches_Notes varchar(MAX) null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Branches_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Branches_Add bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Branches_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Branches_View bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Branches_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Branches_Edit bit default 0 not null;
	GO
	
	--PromotionEvents
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PromotionEvents_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD PromotionEvents_Notes varchar(MAX) null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PromotionEvents_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD PromotionEvents_Add bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PromotionEvents_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD PromotionEvents_View bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PromotionEvents_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD PromotionEvents_Edit bit default 0 not null;
	GO
	
	--PettyCashRecordsCategories
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PettyCashRecordsCategories_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD PettyCashRecordsCategories_Notes varchar(MAX) null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PettyCashRecordsCategories_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD PettyCashRecordsCategories_Add bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PettyCashRecordsCategories_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD PettyCashRecordsCategories_View bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PettyCashRecordsCategories_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD PettyCashRecordsCategories_Edit bit default 0 not null;
	GO
	
	--Languages
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Languages_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Languages_Notes varchar(MAX) null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Languages_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Languages_Add bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Languages_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Languages_View bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Languages_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Languages_Edit bit default 0 not null;
	GO
	
	--LessonTypes
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'LessonTypes_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD LessonTypes_Notes varchar(MAX) null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'LessonTypes_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD LessonTypes_Add bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'LessonTypes_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD LessonTypes_View bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'LessonTypes_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD LessonTypes_Edit bit default 0 not null;
	GO
	
	--LessonPackages
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'LessonPackages_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD LessonPackages_Notes varchar(MAX) null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'LessonPackages_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD LessonPackages_Add bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'LessonPackages_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD LessonPackages_View bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'LessonPackages_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD LessonPackages_Edit bit default 0 not null;
	GO
	
	--Consignments
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Consignments_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Consignments_Notes varchar(MAX) null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Consignments_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Consignments_Add bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Consignments_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Consignments_View bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Consignments_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Consignments_Edit bit default 0 not null;
	GO
	
	--Vouchers
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Vouchers_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Vouchers_Notes varchar(MAX) null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Vouchers_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Vouchers_Add bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Vouchers_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Vouchers_View bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Vouchers_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Vouchers_Edit bit default 0 not null;
	GO
	
	--Suppliers
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Suppliers_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Suppliers_Notes varchar(MAX) null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Suppliers_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Suppliers_Add bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Suppliers_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Suppliers_View bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Suppliers_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Suppliers_Edit bit default 0 not null;
	GO
	
	--Units
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Units_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Units_Notes varchar(MAX) null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Units_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Units_Add bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Units_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Units_View bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Units_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Units_Edit bit default 0 not null;
	GO
	
	--ExpenseCategories
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'ExpenseCategories_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD ExpenseCategories_Notes varchar(MAX) null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'ExpenseCategories_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD ExpenseCategories_Add bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'ExpenseCategories_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD ExpenseCategories_View bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'ExpenseCategories_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD ExpenseCategories_Edit bit default 0 not null;
	GO
	
	--Services
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Services_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Services_Notes varchar(MAX) null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Services_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Services_Add bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Services_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Services_View bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Services_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Services_Edit bit default 0 not null;
	GO
	
	--Products
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Products_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Products_Notes varchar(MAX) null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Products_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Products_Add bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Products_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Products_View bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Products_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Products_Edit bit default 0 not null;
	GO
	
	--SaleInvoices
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'SaleInvoices_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD SaleInvoices_Notes varchar(MAX) null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'SaleInvoices_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD SaleInvoices_Add bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'SaleInvoices_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD SaleInvoices_View bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'SaleInvoices_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD SaleInvoices_Edit bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'SaleInvoices_Approve' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD SaleInvoices_Approve bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'SaleInvoices_TutorTravelCost_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD SaleInvoices_TutorTravelCost_View bit default 0 not null;
	GO
	
	--Payments
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Payments_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Payments_Notes varchar(MAX) null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Payments_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Payments_Add bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Payments_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Payments_View bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Payments_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Payments_Edit bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Payments_Approve' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Payments_Approve bit default 0 not null;
	GO
	
	--PettyCashRecords
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PettyCashRecords_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD PettyCashRecords_Notes varchar(MAX) null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PettyCashRecords_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD PettyCashRecords_Add bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PettyCashRecords_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD PettyCashRecords_View bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PettyCashRecords_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD PettyCashRecords_Edit bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PettyCashRecords_Approve' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD PettyCashRecords_Approve bit default 0 not null;
	GO
	
	--Inventory
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Inventory_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Inventory_Notes varchar(MAX) null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Inventory_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Inventory_Add bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Inventory_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Inventory_View bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'Inventory_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD Inventory_Edit bit default 0 not null;
	GO
	
	--LessonSessions
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'LessonSessions_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD LessonSessions_Notes varchar(MAX) null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'LessonSessions_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD LessonSessions_Add bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'LessonSessions_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD LessonSessions_View bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'LessonSessions_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD LessonSessions_Edit bit default 0 not null;
	GO
	
	--HourlyRates
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'HourlyRates_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD HourlyRates_Notes varchar(MAX) null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'HourlyRates_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD HourlyRates_Add bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'HourlyRates_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD HourlyRates_View bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'HourlyRates_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD HourlyRates_Edit bit default 0 not null;
	GO
	
	--PayrollPayments
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PayrollPayments_Notes' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD PayrollPayments_Notes varchar(MAX) null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PayrollPayments_Add' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD PayrollPayments_Add bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PayrollPayments_View' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD PayrollPayments_View bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PayrollPayments_Edit' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD PayrollPayments_Edit bit default 0 not null;
	IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PayrollPayments_Approve' AND TABLE_NAME = 'UserAccountRoles' AND TABLE_SCHEMA='dbo') 
	ALTER TABLE UserAccountRoles ADD PayrollPayments_Approve bit default 0 not null;
	GO
	
-- ======================================================================================================================




