CREATE TABLE [Mandate].[JeDeclareFolder]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[CompanyId] UNIQUEIDENTIFIER NOT NULL,
	[JdcDossierId] VARCHAR(50) NULL, 

    CONSTRAINT [PK_JeDeclareFolder] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_JeDeclareFolder_Company] FOREIGN KEY ([CompanyId]) REFERENCES [Mandate].[Company]([Id])
)
