CREATE TABLE [Mandate].[RefStatusCode]
(
    [StatusCode] INT NOT NULL,
    [CollectionStatusCode] INT NULL,
    [PulseCode] INT NOT NULL,
    [StatusNameFr] NVARCHAR(100) NOT NULL,
    [StatusNameEn] NVARCHAR(100) NOT NULL,

    CONSTRAINT [PK_RefStatusCode] PRIMARY KEY ([StatusCode]),
    CONSTRAINT [UQ_CollectionStatusCode] UNIQUE ([CollectionStatusCode])
)
