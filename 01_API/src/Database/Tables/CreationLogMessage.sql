CREATE TABLE [Mandate].[MandateCreationLogMessages]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [CollectionId] UNIQUEIDENTIFIER NOT NULL,
    [MessageContent] NVARCHAR(MAX) NOT NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT [PK_MandateCreationLogMessages] PRIMARY KEY ([Id]),
    
    CONSTRAINT [FK_MandateCreationLogMessages_Collection] FOREIGN KEY ([CollectionId]) REFERENCES [Mandate].[Collection]([Id])
);
