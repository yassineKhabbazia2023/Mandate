CREATE TABLE [Mandate].[Collection]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[CompanyId] UNIQUEIDENTIFIER NOT NULL,
	[BankCode] CHAR(5) NULL, 
	[BranchCode] CHAR(5) NULL, 
	[AccountNumber] CHAR(11) NULL, 
	[CheckDigits] CHAR(2) NULL, 
	[LinkType] INT NULL, 
	[RejectReason] NVARCHAR(100) NULL, 

    CONSTRAINT [PK_Collection] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_Collection_Company] FOREIGN KEY ([CompanyId]) REFERENCES [Mandate].[Company]([Id])
)
