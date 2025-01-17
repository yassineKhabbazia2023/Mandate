namespace KPMG.Pulse.Back.Accounting.Mandate.Sql;

public record MandateIdsAndStatus(Guid Id, string JdcDossierId, string JdcRibId, int StatusCode);