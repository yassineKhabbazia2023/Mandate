CREATE TABLE [Mandate].[CompanyCollaborator]
(
	[CompanyId] UNIQUEIDENTIFIER NOT NULL,
	[CollaboratorId] UNIQUEIDENTIFIER NOT NULL,

	CONSTRAINT [PK_CompanyCollaborator] PRIMARY KEY ([CompanyId], [CollaboratorId]),
	CONSTRAINT [FK_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Mandate].[Company]([Id]),
	CONSTRAINT [FK_CollaboratorId] FOREIGN KEY ([CollaboratorId]) REFERENCES [Mandate].[Collaborator]([Id])
)
