using Moq;
using Xunit;

public class ChatServiceTests
{
    private readonly Mock<IChatRepository> repoMock;
    private readonly Mock<IMessageBroker> brokerMock;
    private readonly ChatService service;

    public ChatServiceTests()
    {
        repoMock = new Mock<IChatRepository>();
        brokerMock = new Mock<IMessageBroker>();

        service = new ChatService(repoMock.Object, brokerMock.Object);
    }

    [Fact]
    public void SendMessage_ShouldSaveMessage()
    {
        service.SendMessage("Taras", "Hello");

        repoMock.Verify(r => r.SaveMessage("Taras", "Hello"), Times.Once);
    }

    [Fact]
    public void SendMessage_ShouldSendToBroker()
    {
        service.SendMessage("Taras", "Hello");

        brokerMock.Verify(b => b.Send("Taras", "Hello"), Times.Once);
    }

    [Fact]
    public void GetMessages_ReturnsMessages()
    {
        var list = new List<string> { "Hi", "Hello" };

        repoMock.Setup(r => r.GetMessages("Taras")).Returns(list);

        var result = service.GetUserMessages("Taras");

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetMessages_CallsRepository()
    {
        repoMock.Setup(r => r.GetMessages("Taras")).Returns(new List<string>());

        service.GetUserMessages("Taras");

        repoMock.Verify(r => r.GetMessages("Taras"), Times.Once);
    }

    [Fact]
    public void SendMessage_UserEmpty_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            service.SendMessage("", "Hello"));
    }

    [Fact]
    public void SendMessage_MessageEmpty_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            service.SendMessage("Taras", ""));
    }

    [Fact]
    public void SendMessage_SaveCalledOnce()
    {
        service.SendMessage("User", "Test");

        repoMock.Verify(r => r.SaveMessage("User", "Test"), Times.Once);
    }

    [Fact]
    public void SendMessage_BrokerCalledOnce()
    {
        service.SendMessage("User", "Test");

        brokerMock.Verify(b => b.Send("User", "Test"), Times.Once);
    }
}