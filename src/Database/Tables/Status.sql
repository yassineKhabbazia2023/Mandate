CREATE TABLE [Mandate].[Status]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[CollectionId] UNIQUEIDENTIFIER NOT NULL,
	[StatusCode] INT NOT NULL,
	[CollectionStatusCode] INT NULL,
	[IsCurrent] BIT NOT NULL DEFAULT 0, 
	[StatusDate] DATETIME2 NULL, 
	[MandateFile] VARBINARY(MAX) NULL, 
	[CreatedBy] NVARCHAR(100) NULL, 

    CONSTRAINT [PK_Status] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_Status_Collection] FOREIGN KEY ([CollectionId]) REFERENCES [Mandate].[Collection]([Id]),
	CONSTRAINT [FK_Status_RefStatusCode] FOREIGN KEY ([StatusCode]) REFERENCES [Mandate].[RefStatusCode]([StatusCode])
)
