namespace KPMG.Pulse.Back.Accounting.Mandate.Sql;

public record MandateIdsAndStatus(Guid Id, string AccountNumber, string JdcDossierId, string JdcRibId, int StatusCode, int? CreatedBy, int AccountId);