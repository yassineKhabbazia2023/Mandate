CREATE TABLE [Mandate].[RefBank]
(
	[BankCode] CHAR(5) NOT NULL,
	[BankName] NVARCHAR(50) NULL,
	[BankCommercialName] NVARCHAR(50) NULL,
	[BankCategory] NVARCHAR(50) NULL,
	[BankGroup] NVARCHAR(50) NULL,
	[IsJdcScrapable] BIT NOT NULL DEFAULT 0,
	[IsJdcPartner] BIT NOT NULL DEFAULT 0,
	[HasReleveAgreement] BIT NULL,
	[HasLiasseAgreement] BIT NULL,
	[AllowsDemat] BIT NULL,
	[JdcPartnership] TINYINT NOT NULL DEFAULT 0,

	CONSTRAINT [PK_RefBank] PRIMARY KEY ([BankCode])
)
