namespace Reception.Application.Contracts;

public interface IHl7ClientService
{
    Task SendHl7Message(string hl7Message);
}