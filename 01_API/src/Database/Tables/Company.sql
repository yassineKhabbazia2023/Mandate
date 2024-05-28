CREATE TABLE [Mandate].[Company]
(
	[Id] INT NOT NULL,
	[Name] NVARCHAR(100) NULL, 
	[SiretNumber] CHAR(14) NOT NULL, 
	[ErpId] VARCHAR(50) NULL,
	[IsActive] BIT NOT NULL DEFAULT 1,

    CONSTRAINT [PK_Company] PRIMARY KEY ([Id])
)
