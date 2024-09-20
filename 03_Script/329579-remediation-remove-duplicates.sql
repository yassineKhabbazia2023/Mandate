-- Script de suppression des doublons

-- Dans ce script,on va supprimer deux types de doublons : 
-- 1) les doublons identiques (JdcDossierId, City, Complements, Country, Email, FirstName, LastName, Street, Title, ZipCode, BankCode, BranchCode, AccountNumber, CheckDigits, CompanyId, StatusCode)
-- 2) les doublons "non identiques" qui ont un statut égal à "-1" (Incident) seulement si leur doublon associé est dans un statut différent de "-1".

-- Déclaration des tables temporaires
DECLARE @TousLesDoublons TABLE (
    Rank INT,
    CollectionId VARCHAR(MAX),
    JdcDossierId VARCHAR(MAX),
    JdcReleveId INT, 
    JdcRibId INT,
    City VARCHAR(MAX), 
    Complements VARCHAR(MAX), 
    Country VARCHAR(MAX), 
    Email VARCHAR(MAX), 
    FirstName VARCHAR(MAX), 
    LastName VARCHAR(MAX), 
    Street VARCHAR(MAX), 
    Title VARCHAR(MAX),
    ZipCode VARCHAR(MAX), 
    BankCode VARCHAR(MAX), 
    BranchCode VARCHAR(MAX), 
    AccountNumber VARCHAR(MAX), 
    CheckDigits VARCHAR(MAX), 
    CompanyId INT,
    ErpId VARCHAR(MAX),
    SiretNumber VARCHAR(MAX),
    StatusCode INT, 
    IsCurrent BIT
);
DECLARE @DoublonsIdentiques TABLE (
    Rank INT,
    CollectionId VARCHAR(MAX),
    JdcDossierId VARCHAR(MAX),
    JdcReleveId INT, 
    JdcRibId INT,
    City VARCHAR(MAX), 
    Complements VARCHAR(MAX), 
    Country VARCHAR(MAX), 
    Email VARCHAR(MAX), 
    FirstName VARCHAR(MAX), 
    LastName VARCHAR(MAX), 
    Street VARCHAR(MAX), 
    Title VARCHAR(MAX),
    ZipCode VARCHAR(MAX), 
    BankCode VARCHAR(MAX), 
    BranchCode VARCHAR(MAX), 
    AccountNumber VARCHAR(MAX), 
    CheckDigits VARCHAR(MAX), 
    CompanyId INT,
    ErpId VARCHAR(MAX),
    SiretNumber VARCHAR(MAX),
    StatusCode INT, 
    IsCurrent BIT
);
DECLARE @DoublonsASupprimer TABLE (
    CollectionId VARCHAR(MAX),
    BankCode VARCHAR(MAX), 
    BranchCode VARCHAR(MAX), 
    AccountNumber VARCHAR(MAX), 
    CheckDigits VARCHAR(MAX), 
    CompanyId INT
);

-- Insérer les doublons dans @TousLesDoublons
INSERT INTO @TousLesDoublons
SELECT ROW_NUMBER() OVER (
        PARTITION BY BankCode, BranchCode, AccountNumber, CheckDigits, CompanyId 
        ORDER BY JdcReleveId, JdcRibId, BankCode, BranchCode, AccountNumber, CheckDigits, CompanyId
    ) AS Rank, *
FROM (
    SELECT
        COL.Id, JDF.JdcDossierId, JDC.JdcReleveId, JDC.JdcRibId, P.City, P.Complements, P.Country, 
        P.Email, P.FirstName, P.LastName, P.Street, P.Title, P.ZipCode, COL.BankCode, COL.BranchCode, 
        COL.AccountNumber, COL.CheckDigits, COL.CompanyId, CNY.ErpId, CNY.SiretNumber, S.StatusCode, S.IsCurrent
    FROM [Mandate].[Collection] COL
    INNER JOIN (
        SELECT COUNT(*) AS Count, C.BankCode, C.BranchCode, C.AccountNumber, c.CheckDigits, c.CompanyId
        FROM [Mandate].[Collection] C 
        GROUP BY C.BankCode, C.BranchCode, C.AccountNumber, c.CheckDigits, c.CompanyId
        HAVING COUNT(*) > 1
    ) D ON COL.AccountNumber = D.AccountNumber 
        AND COL.BankCode = D.BankCode 
        AND COL.BranchCode = D.BranchCode 
        AND COL.CheckDigits = D.CheckDigits 
        AND COL.CompanyId = D.CompanyId
    INNER JOIN Mandate.Company CNY ON CNY.Id = COL.CompanyId
    INNER JOIN Mandate.Personal P ON P.CollectionId = COL.Id
    INNER JOIN Mandate.JeDeclareCollection JDC ON JDC.CollectionId = COL.Id
    INNER JOIN Mandate.JeDeclareFolder JDF ON JDF.CompanyId = CNY.Id
    INNER JOIN Mandate.Status S ON S.CollectionId = COL.Id AND S.IsCurrent = 1
) T;

-- Sélection des doublons "identiques" et insertion dans @DoublonsIdentiques
WITH IdenticalDuplicates AS (
    SELECT *,
        COUNT(*) OVER (PARTITION BY JdcDossierId, City, Complements, Country, Email, FirstName, LastName, Street, Title, ZipCode, BankCode, BranchCode, AccountNumber, CheckDigits, CompanyId, StatusCode) AS DuplicateCount
    FROM @TousLesDoublons
)
INSERT INTO @DoublonsIdentiques
SELECT Rank, CollectionId, JdcDossierId, JdcReleveId, JdcRibId, City, Complements, Country, Email, FirstName, LastName, Street, Title, ZipCode, BankCode, BranchCode, AccountNumber, CheckDigits, CompanyId, ErpId, SiretNumber, StatusCode, IsCurrent
FROM IdenticalDuplicates
WHERE DuplicateCount > 1;

-- Sélection des doublons "identiques" à supprimer et insertion dans @DoublonsASupprimer
WITH MinRank AS (
    SELECT BankCode, BranchCode, AccountNumber, CheckDigits, CompanyId, MIN(Rank) AS MinRank 
    FROM @DoublonsIdentiques
    GROUP BY JdcDossierId, City, Complements, Country, Email, FirstName, LastName, Street, Title, ZipCode, BankCode, BranchCode, AccountNumber, CheckDigits, CompanyId
)
INSERT INTO @DoublonsASupprimer
SELECT d.CollectionId, d.BankCode, d.BranchCode, d.AccountNumber, d.CheckDigits, d.CompanyId
FROM @DoublonsIdentiques d
INNER JOIN MinRank m
    ON d.BankCode = m.BankCode 
    AND d.BranchCode = m.BranchCode 
    AND d.AccountNumber = m.AccountNumber 
    AND d.CheckDigits = m.CheckDigits 
    AND d.CompanyId = m.CompanyId
WHERE d.Rank > m.MinRank;

-- Déclaration des tables temporaires pour les doublons "non identiques"
DECLARE @DoublonsNonIdentiques TABLE (
    CollectionId VARCHAR(MAX),
    JdcDossierId VARCHAR(MAX),
    City VARCHAR(MAX), 
    Complements VARCHAR(MAX), 
    Country VARCHAR(MAX), 
    Email VARCHAR(MAX), 
    FirstName VARCHAR(MAX), 
    LastName VARCHAR(MAX), 
    Street VARCHAR(MAX), 
    Title VARCHAR(MAX),
    ZipCode VARCHAR(MAX), 
    BankCode VARCHAR(MAX), 
    BranchCode VARCHAR(MAX), 
    AccountNumber VARCHAR(MAX), 
    CheckDigits VARCHAR(MAX), 
    CompanyId INT,
    StatusCode INT
);
DECLARE @GroupeDoublonsNonIdentiquesAvecUnSeulEgalAMoinsUN TABLE (
    CollectionId VARCHAR(MAX),
    JdcDossierId VARCHAR(MAX),
    City VARCHAR(MAX), 
    Complements VARCHAR(MAX), 
    Country VARCHAR(MAX), 
    Email VARCHAR(MAX), 
    FirstName VARCHAR(MAX), 
    LastName VARCHAR(MAX), 
    Street VARCHAR(MAX), 
    Title VARCHAR(MAX),
    ZipCode VARCHAR(MAX), 
    BankCode VARCHAR(MAX), 
    BranchCode VARCHAR(MAX), 
    AccountNumber VARCHAR(MAX), 
    CheckDigits VARCHAR(MAX), 
    CompanyId INT,
    StatusCode INT
);
DECLARE @GroupeDoublonsNonIdentiquesSansAucunEgalAMoinsUN TABLE (
    CollectionId VARCHAR(MAX),
    JdcDossierId VARCHAR(MAX),
    City VARCHAR(MAX), 
    Complements VARCHAR(MAX), 
    Country VARCHAR(MAX), 
    Email VARCHAR(MAX), 
    FirstName VARCHAR(MAX), 
    LastName VARCHAR(MAX), 
    Street VARCHAR(MAX), 
    Title VARCHAR(MAX),
    ZipCode VARCHAR(MAX), 
    BankCode VARCHAR(MAX), 
    BranchCode VARCHAR(MAX), 
    AccountNumber VARCHAR(MAX), 
    CheckDigits VARCHAR(MAX), 
    CompanyId INT,
    StatusCode INT
);
DECLARE @GroupeDoublonsNonIdentiquesTousAvecAuMoinsUnStatusEgalAMoinsUN TABLE (
    CollectionId VARCHAR(MAX),
    JdcDossierId VARCHAR(MAX),
    City VARCHAR(MAX), 
    Complements VARCHAR(MAX), 
    Country VARCHAR(MAX), 
    Email VARCHAR(MAX), 
    FirstName VARCHAR(MAX), 
    LastName VARCHAR(MAX), 
    Street VARCHAR(MAX), 
    Title VARCHAR(MAX),
    ZipCode VARCHAR(MAX), 
    BankCode VARCHAR(MAX), 
    BranchCode VARCHAR(MAX), 
    AccountNumber VARCHAR(MAX), 
    CheckDigits VARCHAR(MAX), 
    CompanyId INT,
    StatusCode INT
);

-- Insérer les doublons "non identiques"
INSERT INTO @DoublonsNonIdentiques
SELECT
    CollectionId, JdcDossierId, City, Complements, Country, Email, FirstName, LastName, Street, Title, ZipCode, BankCode, BranchCode, AccountNumber, CheckDigits, CompanyId, StatusCode
FROM (
    SELECT 
        CollectionId, JdcDossierId, City, Complements, Country, Email, FirstName, LastName, Street, Title, ZipCode, BankCode, BranchCode, AccountNumber, CheckDigits, CompanyId, StatusCode,
        COUNT(*) OVER (PARTITION BY City, Complements, Country, Email, FirstName, LastName, Street, Title, ZipCode, BankCode, BranchCode, AccountNumber, CheckDigits, CompanyId, StatusCode) AS Cnt
    FROM @TousLesDoublons
) T
WHERE Cnt = 1;
--SELECT * FROM @DoublonsNonIdentiques;
-- Grouper les doublons "non identiques" SANS aucun d'entre eux ayant status égal à -1
INSERT INTO @GroupeDoublonsNonIdentiquesSansAucunEgalAMoinsUN
SELECT M.*
FROM @DoublonsNonIdentiques M
INNER JOIN (
    SELECT DISTINCT BankCode, BranchCode, AccountNumber, CheckDigits, CompanyId
    FROM @TousLesDoublons
    GROUP BY BankCode, BranchCode, AccountNumber, CheckDigits, CompanyId
    HAVING SUM(CASE WHEN StatusCode = -1 THEN 1 ELSE 0 END) = 0
) T ON T.AccountNumber = M.AccountNumber 
   AND T.BankCode = M.BankCode 
   AND T.BranchCode = M.BranchCode 
   AND T.CheckDigits = M.CheckDigits 
   AND T.CompanyId = M.CompanyId
ORDER BY BankCode, BranchCode, AccountNumber, CheckDigits, CompanyId;

-- Grouper les doublons "non identiques" avec au moins un d'entre eux ayant un statut égal à -1
INSERT INTO @GroupeDoublonsNonIdentiquesAvecUnSeulEgalAMoinsUN
SELECT M.* 
FROM @DoublonsNonIdentiques M
INNER JOIN ( 
    SELECT DISTINCT BankCode, BranchCode, AccountNumber, CheckDigits, CompanyId 
    FROM @TousLesDoublons 
    WHERE StatusCode != -1
) T ON T.AccountNumber = M.AccountNumber 
   AND T.BankCode = M.BankCode 
   AND T.BranchCode = M.BranchCode 
   AND T.CheckDigits = M.CheckDigits 
   AND T.CompanyId = M.CompanyId
ORDER BY BankCode, BranchCode, AccountNumber, CheckDigits, CompanyId;

-- Ajouter les doublons à supprimer
INSERT INTO @DoublonsASupprimer
SELECT CollectionId, BankCode, BranchCode, AccountNumber, CheckDigits, CompanyId
FROM @GroupeDoublonsNonIdentiquesAvecUnSeulEgalAMoinsUN 
WHERE StatusCode = -1;

-- Afficher les doublons à supprimer
SELECT * FROM @DoublonsASupprimer;

 -- Suppression des doublons
 DECLARE @CollectionsToDelete TABLE (CollectionId VARCHAR(MAX));
 INSERT INTO @CollectionsToDelete (CollectionId) 
 SELECT CollectionId FROM @DoublonsASupprimer;

 -- Suppression des enregistrements associés
 DELETE JDC
 FROM [Mandate].[JeDeclareCollection] JDC
 INNER JOIN @CollectionsToDelete CTD ON JDC.CollectionId = CTD.CollectionId;

 DELETE P
 FROM [Mandate].[Personal] P
 INNER JOIN @CollectionsToDelete CTD ON P.CollectionId = CTD.CollectionId;

 DELETE S
 FROM [Mandate].[Status] S
 INNER JOIN @CollectionsToDelete CTD ON S.CollectionId = CTD.CollectionId;

 DELETE C
 FROM [Mandate].[Collection] C
 INNER JOIN @CollectionsToDelete CTD ON C.Id = CTD.CollectionId;

 -- Vérification des suppressions (le résultat doit être vide pour les 4 SELECT suivant)
 SELECT * FROM [Mandate].[Collection] WHERE Id IN (SELECT CollectionId FROM @CollectionsToDelete);
 SELECT * FROM [Mandate].[JeDeclareCollection] WHERE CollectionId IN (SELECT CollectionId FROM @CollectionsToDelete);
 SELECT * FROM [Mandate].[Personal] WHERE CollectionId IN (SELECT CollectionId FROM @CollectionsToDelete);
 SELECT * FROM [Mandate].[Status] WHERE CollectionId IN (SELECT CollectionId FROM @CollectionsToDelete);
