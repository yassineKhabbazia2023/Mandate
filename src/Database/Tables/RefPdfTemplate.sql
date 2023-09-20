CREATE TABLE [Mandate].[RefPdfTemplate]
(
	[BankCode] CHAR(5) NOT NULL,
	[PdfFile] VARBINARY(MAX) NOT NULL,

	CONSTRAINT [PK_RefPdfTemplate] PRIMARY KEY ([BankCode])
)
