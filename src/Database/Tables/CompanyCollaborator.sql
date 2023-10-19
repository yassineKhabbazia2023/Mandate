CREATE TABLE [Mandate].[CompanyCollaborator]
(
	[CompanyId] UNIQUEIDENTIFIER NOT NULL,
	[CollaboratorId] UNIQUEIDENTIFIER NOT NULL,

	CONSTRAINT [FK_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Mandate].[Company]([Id]),
	CONSTRAINT [FK_CollaboratorId] FOREIGN KEY ([CollaboratorId]) REFERENCES [Mandate].[Collaborator]([Id])
)
