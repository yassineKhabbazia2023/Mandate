CREATE TABLE [Onboarding].[SepaMandates] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [AccountId] INT NOT NULL,
    [AccountHolder] NVARCHAR(255) NOT NULL,
    [Address] NVARCHAR(500) NOT NULL,
    [Iban] VARCHAR(34) NOT NULL,
    [Bic] VARCHAR(11) NOT NULL,
    [RibDocumentId] INT NOT NULL,
    [SignatureRequestId] VARCHAR(100) NULL,
    [SignatureUrl] NVARCHAR(2048) NULL,
    [SignatureStatus] INT NOT NULL CONSTRAINT [DF_SepaMandates_SignatureStatus] DEFAULT 0,
    [IsSentToAkuiteo] BIT NOT NULL CONSTRAINT [DF_SepaMandates_IsSentToAkuiteo] DEFAULT 0,
    [SentToAkuiteoAt] DATETIME2 NULL,
    [SignedMandateDocumentId] NVARCHAR(100) NULL,
    [CreatedAt] DATETIME2 NOT NULL,
    [CreatedBy] VARCHAR(255) NOT NULL,

    CONSTRAINT [PK_SepaMandates] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SepaMandates_Company]
        FOREIGN KEY ([AccountId])
        REFERENCES [Mandate].[Company]([Id])
);
