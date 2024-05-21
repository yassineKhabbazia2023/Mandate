CREATE TABLE [Mandate].[MandateLog]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[SiretNumber] CHAR(14) NOT NULL, 
	[ErpId] VARCHAR(50) NOT NULL, 
	[BankCode] CHAR(5) NOT NULL, 
	[BranchCode] CHAR(5) NOT NULL, 
	[AccountNumber] CHAR(11) NOT NULL, 
	[CheckDigits] CHAR(2) NOT NULL, 
	[JdcDossierId] VARCHAR(50) NOT NULL, 
	[JdcReleveId] VARCHAR(50) NOT NULL, 
	[JdcRibId] VARCHAR(50) NOT NULL, 
    [CreationDate] DATETIME NOT NULL, 
    [ExceptionType] INT NOT NULL, 
    [ExceptionMessage] VARCHAR(MAX) NULL, 
    [InnerExceptionMessage] VARCHAR(MAX) NULL, 

	CONSTRAINT [PK_MandateLog] PRIMARY KEY ([Id])
)
