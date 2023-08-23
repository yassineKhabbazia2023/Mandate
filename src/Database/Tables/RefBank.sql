CREATE TABLE [Mandate].[RefBank]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[BankCode] NVARCHAR(MAX) NOT NULL,
	[BankName] NVARCHAR(MAX) NULL,
	[BankGroup] NVARCHAR(MAX) NULL,
	[IsJdcScrapable] BIT NOT NULL DEFAULT 0,
	[IsJdcPartner] BIT NOT NULL DEFAULT 0,
	[HasReleveAgreement] BIT NULL,
	[HasLiasseAgreement] BIT NULL,
	[AllowsDemat] BIT NULL,

	CONSTRAINT [PK_RefBank] PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_RefBank_BankCode] UNIQUE ([BankCode])
)
