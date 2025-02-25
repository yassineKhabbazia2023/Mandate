namespace KPMG.Pulse.Back.Accounting.Mandate.Function.Managers;

public record MandateToUpdate(Guid CollectionId, string AccountNumber, int NextStatus, int? CreatedById, int AccountId);