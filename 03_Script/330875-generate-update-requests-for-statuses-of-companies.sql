-- Script pour remettre les status des companies à actif.
-- task : 330875
-- 1) Executer cette requête côté DB Account
-- 2) Récupérer le résultat (les requêtes UPDATE) et executer les dans la DB Mandate.
SELECT 
    'UPDATE [Mandate].[Company] SET [IsActive] = 1' +  
    ' WHERE [Id] = ' + CAST(a.[AccountId] AS VARCHAR) + ';' AS UpdateQuery
FROM 
    [account].[Account] a
WHERE 
    a.IsActive = 1;
 