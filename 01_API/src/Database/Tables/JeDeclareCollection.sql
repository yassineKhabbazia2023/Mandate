CREATE TABLE [Mandate].[JeDeclareCollection]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[CollectionId] UNIQUEIDENTIFIER NOT NULL,
	[JdcReleveId] VARCHAR(50) NULL, 
	[JdcRibId] VARCHAR(50) NULL, 

    CONSTRAINT [PK_JeDeclareCollection] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_JeDeclareCollection_Collection] FOREIGN KEY ([CollectionId]) REFERENCES [Mandate].[Collection]([Id])
)
