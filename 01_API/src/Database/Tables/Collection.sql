CREATE TABLE [Mandate].[Collection]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[CompanyId] INT NOT NULL,
	[BankCode] CHAR(5) NOT NULL, 
	[BranchCode] CHAR(5) NOT NULL, 
	[AccountNumber] CHAR(11) NOT NULL, 
	[CheckDigits] CHAR(2) NOT NULL, 
	[LinkType] INT NULL, 
	[RejectReason] NVARCHAR(100) NULL,
	CreatedById INT NULL,

    CONSTRAINT [PK_Collection] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_Collection_Company] FOREIGN KEY ([CompanyId]) REFERENCES [Mandate].[Company]([Id]),
	CONSTRAINT [FK_Collection_Bank] FOREIGN KEY ([BankCode]) REFERENCES [Mandate].[RefBank]([BankCode]),
	CONSTRAINT [FK_CreatedById] FOREIGN KEY (CreatedById) REFERENCES [Mandate].[Collaborator]([Id]),
)
