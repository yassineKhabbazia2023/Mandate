IF NOT EXISTS (SELECT StatusCode FROM Mandate.RefStatusCode where StatusCode = 99)
BEGIN
INSERT into Mandate.RefStatusCode values (99,null,99,'','')
END

IF NOT EXISTS (SELECT StatusCode FROM Mandate.RefStatusCode where StatusCode = 98)
BEGIN
INSERT into Mandate.RefStatusCode values (98,null,10,'','')
END