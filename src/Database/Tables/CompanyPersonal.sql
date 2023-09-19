CREATE TABLE [Mandate].[CompanyPersonal]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[CompanyId] UNIQUEIDENTIFIER NOT NULL,
	[Title] NVARCHAR(10) NULL, 
	[FirstName] NVARCHAR(100) NULL, 
	[LastName] NVARCHAR(100) NULL, 
	[Email] NVARCHAR(100) NULL, 
	[Street] NVARCHAR(100) NULL, 
	[Complements] NVARCHAR(100) NULL, 
	[ZipCode] NVARCHAR(100) NULL, 
	[City] NVARCHAR(100) NULL, 
	[Country] NVARCHAR(100) NULL, 

    CONSTRAINT [PK_CompanyPersonal] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_CompanyPersonal_Company] FOREIGN KEY ([CompanyId]) REFERENCES [Mandate].[Company]([Id])
)
