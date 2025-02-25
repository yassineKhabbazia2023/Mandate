namespace KPMG.Pulse.Back.Accounting.Mandate.Function.Interfaces;

public interface IMandateEventPublisher
{
    Task PublishEventAsync<TEvent>(TEvent message);
}