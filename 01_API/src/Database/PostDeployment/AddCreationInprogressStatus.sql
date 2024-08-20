IF NOT EXISTS (SELECT StatusCode FROM Mandate.RefStatusCode)
BEGIN
INSERT into Mandate.RefStatusCode values (99,null,99,'','')
END