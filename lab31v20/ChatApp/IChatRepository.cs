public interface IChatRepository
{
    void SaveMessage(string user, string message);
    List<string> GetMessages(string user);
}