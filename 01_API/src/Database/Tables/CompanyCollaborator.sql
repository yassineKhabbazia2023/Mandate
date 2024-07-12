CREATE TABLE [Mandate].[CompanyCollaborator]
(
	[CompanyId] INT NOT NULL,
	[CollaboratorId] INT NOT NULL,

	CONSTRAINT [PK_CompanyCollaborator] PRIMARY KEY ([CollaboratorId], [CompanyId]),
	CONSTRAINT [FK_CollaboratorId] FOREIGN KEY ([CollaboratorId]) REFERENCES [Mandate].[Collaborator]([Id]),
	CONSTRAINT [FK_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Mandate].[Company]([Id])
)
