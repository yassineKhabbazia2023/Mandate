CREATE TABLE [Mandate].[Company]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[Name] NVARCHAR(100) NULL, 
	[SiretNumber] CHAR(14) NOT NULL, 
	[ErpId] VARCHAR(50) NULL, 
	[BankServicesProviderId] VARCHAR(50) NULL, 

    CONSTRAINT [PK_Company] PRIMARY KEY ([Id])
)
