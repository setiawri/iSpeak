
---- BILLS ==============================================================================================================================

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'BilliardEventBills' AND TABLE_SCHEMA='dbo') 
BEGIN
	CREATE TABLE [dbo].[BilliardEventBills]
	(
		[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
		[Pool1_Start] [datetime] NOT NULL,
		[Pool1_End] [datetime] NOT NULL,
		[Pool1_ChargeAmount] [int] NOT NULL DEFAULT 0,
		[Pool2_Start] [datetime] NOT NULL,
		[Pool2_End] [datetime] NOT NULL,
		[Pool2_ChargeAmount] [int] NOT NULL DEFAULT 0,
		[FoodAndBeveragesAmount] [int] NOT NULL DEFAULT 0,
		[FoodAndBeveragesDiscountAmount] [int] NOT NULL DEFAULT 0,
		[ServiceChargeAmount] [int] NOT NULL DEFAULT 0,
		[TaxAmount] [int] NOT NULL DEFAULT 0,
		[AdditionalChargeAmount] [int] NOT NULL DEFAULT 0,
		[TotalAmount] [int] NOT NULL DEFAULT 0,
		[TaxAndServiceChargePercentageRate] [decimal](5, 2) NOT NULL DEFAULT 0,
		[FoodAndBeveragesDiscountPercentageRate] [decimal](5, 2) NOT NULL DEFAULT 0,
		[Notes] VARCHAR(MAX) NULL
	)
END
GO

---- EVENT PLAYERS ======================================================================================================================

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'BilliardEventUsers' AND TABLE_SCHEMA='dbo') 
BEGIN
	CREATE TABLE [dbo].[BilliardEventUsers]
	(
		[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
		[BilliardEvents_Id] [uniqueidentifier] NOT NULL,
		[BilliardUsers_Id] [uniqueidentifier] NOT NULL,
		[Timestamp_Start] [datetime] NOT NULL,
		[Timestamp_End] [datetime] NOT NULL,
		[FoodAndBeverages1_Name] VARCHAR(MAX) NULL,
		[FoodAndBeverages1] [int] NOT NULL DEFAULT 0,
		[FoodAndBeverages2_Name] VARCHAR(MAX) NULL,
		[FoodAndBeverages2] [int] NOT NULL DEFAULT 0,
		[FoodAndBeverages3_Name] VARCHAR(MAX) NULL,
		[FoodAndBeverages3] [int] NOT NULL DEFAULT 0,
		[FoodAndBeverages4_Name] VARCHAR(MAX) NULL,
		[FoodAndBeverages4] [int] NOT NULL DEFAULT 0,
		[FoodAndBeverages5_Name] VARCHAR(MAX) NULL,
		[FoodAndBeverages5] [int] NOT NULL DEFAULT 0,
		[OtherCharges1_Name] VARCHAR(MAX) NULL,
		[OtherCharges1] [int] NOT NULL DEFAULT 0,
		[OtherCharges2_Name] VARCHAR(MAX) NULL,
		[OtherCharges2] [int] NOT NULL DEFAULT 0,
		[AmountOwe] [int] NOT NULL DEFAULT 0,
		[AmountDue] [int] NOT NULL DEFAULT 0,
		[Notes] VARCHAR(MAX) NULL
	)
END
GO

---- EVENT SCORES =======================================================================================================================

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'BilliardEventItemScores' AND TABLE_SCHEMA='dbo') 
BEGIN
	CREATE TABLE [dbo].[BilliardEventItemScores]
	(
		[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
		[Timestamp] [datetime] NOT NULL,
		[BilliardEvents_Id] [uniqueidentifier] NOT NULL,
		[BilliardUsers_Id_1] [uniqueidentifier] NOT NULL,
		[BilliardUser1_Score] [int] NOT NULL DEFAULT 0,
		[BilliardUsers_Id_2] [uniqueidentifier] NOT NULL,
		[BilliardUser2_Score] [int] NOT NULL DEFAULT 0,
		[Notes] VARCHAR(MAX) NULL
	)
END
GO

---- BILLIARD EVENTS ====================================================================================================================

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'BilliardEvents' AND TABLE_SCHEMA='dbo') 
BEGIN
	CREATE TABLE [dbo].[BilliardEvents]
	(
		[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
		[Timestamp] [datetime] NOT NULL,
		[Name] VARCHAR(MAX) NOT NULL, 
		[BilliardPlaces_Id] [uniqueidentifier] NOT NULL,
		[PoolRatePerMinute] [int] NOT NULL DEFAULT 0,
		[Notes] VARCHAR(MAX) NULL
	)
END
GO

---- BILLIARD PLACES ====================================================================================================================

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'BilliardPlaces' AND TABLE_SCHEMA='dbo') 
BEGIN
	CREATE TABLE [dbo].[BilliardPlaces]
	(
		[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
		[Name] VARCHAR(MAX) NOT NULL, 
		[Notes] VARCHAR(MAX) NULL
	)
END
GO

---- USERS TABLE ========================================================================================================================


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'BilliardUsers' AND TABLE_SCHEMA='dbo') 
BEGIN
	CREATE TABLE [dbo].[BilliardUsers]
	(
		[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
		[Name] VARCHAR(MAX) NOT NULL, 
		[UserIdentification] [int] NOT NULL DEFAULT 0, 
		[Notes] VARCHAR(MAX) NULL
	)
END
GO
