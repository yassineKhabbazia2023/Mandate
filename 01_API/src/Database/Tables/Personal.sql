CREATE TABLE [Mandate].[Personal]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[CompanyId] INT NULL,
	[CollectionId] UNIQUEIDENTIFIER NULL,
	[Title] NVARCHAR(10) NULL, 
	[FirstName] NVARCHAR(100) NULL, 
	[LastName] NVARCHAR(100) NULL, 
	[Email] NVARCHAR(100) NULL, 
	[Street] NVARCHAR(100) NULL, 
	[Complements] NVARCHAR(100) NULL, 
	[ZipCode] NVARCHAR(100) NULL, 
	[City] NVARCHAR(100) NULL, 
	[Country] NVARCHAR(100) NULL, 

    CONSTRAINT [PK_Personal] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_Personal_Company] FOREIGN KEY ([CompanyId]) REFERENCES [Mandate].[Company]([Id]),
	CONSTRAINT [FK_Personal_Collection] FOREIGN KEY ([CollectionId]) REFERENCES [Mandate].[Collection]([Id])
)
