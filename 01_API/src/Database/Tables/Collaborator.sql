CREATE TABLE [Mandate].[Collaborator]
(
	[Id] INT NOT NULL,
	[Email] NVARCHAR(255) NOT NULL,
	[FirstName] NVARCHAR(255) NULL,
	[LastName] NVARCHAR(255) NULL,

	CONSTRAINT [PK_Collaborator] PRIMARY KEY ([Id]),
	CONSTRAINT [UQ_Collaborator_Email] UNIQUE ([Email])
)
