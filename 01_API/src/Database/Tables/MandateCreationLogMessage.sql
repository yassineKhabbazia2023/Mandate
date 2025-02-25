CREATE TABLE [Mandate].[MandateCreationLogMessage]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [CollectionId] UNIQUEIDENTIFIER NOT NULL,
    [MessageContent] NVARCHAR(MAX) NOT NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedById] INT NULL, 

    CONSTRAINT [PK_MandateCreationLogMessage] PRIMARY KEY ([Id]),    

    CONSTRAINT [FK_MandateCreationLogMessage_Collection] FOREIGN KEY ([CollectionId]) REFERENCES [Mandate].[Collection]([Id]),
    CONSTRAINT [FK_MandateCreationLogMessage_Collaborator] FOREIGN KEY ([CreatedById]) REFERENCES [Mandate].[Collaborator]([Id])
);
