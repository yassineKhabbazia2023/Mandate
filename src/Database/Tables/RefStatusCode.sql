CREATE TABLE [Mandate].[RefStatusCode]
(
	[StatusCode] INT NOT NULL,
	[StatusName] NVARCHAR(100) NOT NULL,

	CONSTRAINT [PK_RefStatusCode] PRIMARY KEY ([StatusCode])
)
