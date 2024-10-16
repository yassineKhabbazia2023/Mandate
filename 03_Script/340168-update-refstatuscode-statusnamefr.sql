
UPDATE [Mandate].[RefStatusCode]
SET StatusNameFr = 'Activation demandée, collecte en attente'
WHERE StatusCode = 0

UPDATE [Mandate].[RefStatusCode]
SET StatusNameFr = 'Activation demandée, en attente de la carte EBICS'
WHERE StatusCode = 1

UPDATE [Mandate].[RefStatusCode]
SET StatusNameFr = 'Activation demandée, carte paramètre en cours d''installation'
WHERE StatusCode = 2

UPDATE [Mandate].[RefStatusCode]
SET StatusNameFr = 'Active, collecte effective'
WHERE StatusCode = 3

UPDATE [Mandate].[RefStatusCode]
SET StatusNameFr = 'Incident sur la collecte'
WHERE StatusCode = 4

UPDATE [Mandate].[RefStatusCode]
SET StatusNameFr = 'Désactivation demandée'
WHERE StatusCode = 5

UPDATE [Mandate].[RefStatusCode]
SET StatusNameFr = 'Désactivation effective'
WHERE StatusCode = 6

UPDATE [Mandate].[RefStatusCode]
SET StatusNameFr = 'Activation demandée, en attente du mandat'
WHERE StatusCode = 7

UPDATE [Mandate].[RefStatusCode]
SET StatusNameFr = 'Activation demandée mandate uploadé'
WHERE StatusCode = 8

UPDATE [Mandate].[RefStatusCode]
SET StatusNameFr = 'N/A'
WHERE StatusCode = 9

UPDATE [Mandate].[RefStatusCode]
SET StatusNameFr = 'Activation demandée, mandat transmis à la banque'
WHERE StatusCode = 10

UPDATE [Mandate].[RefStatusCode]
SET StatusNameFr = 'Mode de communication non validé (scraping)'
WHERE StatusCode = 11

UPDATE [Mandate].[RefStatusCode]
SET StatusNameFr = 'Activation demandée, identifiants à saisir par le client (scraping)'
WHERE StatusCode = 12

UPDATE [Mandate].[RefStatusCode]
SET StatusNameFr = 'Activation demandée, compte bancaire non disponible (scraping)'
WHERE StatusCode = 13

UPDATE [Mandate].[RefStatusCode]
SET StatusNameFr = 'Activation demandée, compte bancaire à confimer par le client (scraping)'
WHERE StatusCode = 14

UPDATE [Mandate].[RefStatusCode]
SET StatusNameFr = 'Incident sur la collecte : identifiants à ressaisir par le client (scraping)'
WHERE StatusCode = 15

UPDATE [Mandate].[RefStatusCode]
SET StatusNameFr = 'Annulé par votre client (scraping)'
WHERE StatusCode = 16

UPDATE [Mandate].[RefStatusCode]
SET StatusNameFr = 'Activation demandée, mandate rejeté'
WHERE StatusCode = 17

IF EXISTS (SELECT 1 FROM [Mandate].[RefStatusCode] WHERE StatusCode = 18)
	UPDATE [Mandate].[RefStatusCode]
	SET StatusNameFr = 'Réclamation en cours (n/a)'
	WHERE StatusCode = 18
ELSE
	INSERT INTO [Mandate].[RefStatusCode](StatusCode, PulseCode, StatusNameFr, StatusNameEn)
	VALUES(18, 18, 'Réclamation en cours (n/a)', '')

IF EXISTS (SELECT 1 FROM [Mandate].[RefStatusCode] WHERE StatusCode = 21)
	UPDATE [Mandate].[RefStatusCode]
	SET StatusNameFr = 'Activation demandée mais déjà collectée par un autre expert (en attente de sa désactivation)'
	WHERE StatusCode = 21
ELSE
	INSERT INTO [Mandate].[RefStatusCode](StatusCode, PulseCode, StatusNameFr, StatusNameEn)
	VALUES(21, 21, 'Activation demandée mais déjà collectée par un autre expert (en attente de sa désactivation)', '')

IF EXISTS (SELECT 1 FROM [Mandate].[RefStatusCode] WHERE StatusCode = 22)
	UPDATE [Mandate].[RefStatusCode]
	SET StatusNameFr = 'Activation demandée en attente de la lettre de résiliation et de l''upload du nouveau mandat'
	WHERE StatusCode = 22
ELSE
	INSERT INTO [Mandate].[RefStatusCode](StatusCode, PulseCode, StatusNameFr, StatusNameEn)
	VALUES(22, 22, 'Activation demandée en attente de la lettre de résiliation et de l''upload du nouveau mandat', '')
