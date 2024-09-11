CREATE TABLE [Mandate].[MandateCreationLogMessage]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [CollectionId] UNIQUEIDENTIFIER NOT NULL,
    [MessageContent] NVARCHAR(MAX) NOT NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT [PK_MandateCreationLogMessage] PRIMARY KEY ([Id]),
    
    CONSTRAINT [FK_MandateCreationLogMessage_Collection] FOREIGN KEY ([CollectionId]) REFERENCES [Mandate].[Collection]([Id])
);
