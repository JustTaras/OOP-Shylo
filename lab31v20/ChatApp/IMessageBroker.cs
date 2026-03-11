public interface IMessageBroker
{
    void Send(string user, string message);
}