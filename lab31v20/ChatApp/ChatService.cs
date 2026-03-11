public class ChatService
{
    private readonly IChatRepository _repository;
    private readonly IMessageBroker _broker;

    public ChatService(IChatRepository repository, IMessageBroker broker)
    {
        _repository = repository;
        _broker = broker;
    }

    public void SendMessage(string user, string message)
    {
        if (string.IsNullOrWhiteSpace(user))
            throw new ArgumentException("User empty");

        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message empty");

        _repository.SaveMessage(user, message);
        _broker.Send(user, message);
    }

    public List<string> GetUserMessages(string user)
    {
        return _repository.GetMessages(user);
    }
}