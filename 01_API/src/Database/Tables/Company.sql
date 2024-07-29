CREATE TABLE [Mandate].[Company]
(
	[Id] INT NOT NULL,
	[Name] NVARCHAR(255) NULL, 
	[SiretNumber] VARCHAR(150) NULL, 
	[ErpId] VARCHAR(50) NULL,
	[IsActive] BIT NOT NULL DEFAULT 1,

    CONSTRAINT [PK_Company] PRIMARY KEY ([Id])
)
