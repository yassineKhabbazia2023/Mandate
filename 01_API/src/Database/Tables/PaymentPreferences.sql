CREATE TABLE [Onboarding].[PaymentPreferences] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [AccountId] INT NOT NULL,
    [PaymentType] INT NULL,
    [CreatedAt] DATETIME2 NOT NULL,
    [CreatedBy] VARCHAR(255) NOT NULL,

    CONSTRAINT [PK_PaymentPreferences] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PaymentPreferences_Company]
        FOREIGN KEY ([AccountId])
        REFERENCES [Mandate].[Company]([Id])
);
