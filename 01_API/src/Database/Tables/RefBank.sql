CREATE TABLE [Mandate].[RefBank]
(
	[BankCode] CHAR(5) NOT NULL,
	[BankName] NVARCHAR(250) NULL,
	[BankCommercialName] NVARCHAR(250) NULL,
	[BankCategory] NVARCHAR(250) NULL,
	[BankGroup] NVARCHAR(250) NULL,
	[IsJdcScrapable] BIT NOT NULL DEFAULT 0,
	[IsJdcPartner] BIT NOT NULL DEFAULT 0,
	[HasReleveAgreement] BIT NULL,
	[HasLiasseAgreement] BIT NULL,
	[AllowsDemat] BIT NULL,
	[JdcPartnership] TINYINT NOT NULL DEFAULT 0,
	[EbicsCardId] VARCHAR(50) NULL,

	CONSTRAINT [PK_RefBank] PRIMARY KEY ([BankCode])
)
