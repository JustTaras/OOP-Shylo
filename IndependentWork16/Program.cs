using System;

#region Models
public class UserActivity
{
    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
}
#endregion

#region Interfaces
public interface IActivityRecorder
{
    void Record(UserActivity activity);
}

public interface IActivityFilter
{
    bool IsAllowed(UserActivity activity);
}

public interface IFileLogger
{
    void Write(UserActivity activity);
}

public interface IServerLogger
{
    void Send(UserActivity activity);
}
#endregion

#region Implementations
public class SimpleActivityRecorder : IActivityRecorder
{
    public void Record(UserActivity activity)
    {
        Console.WriteLine($"Activity recorded: {activity.UserName} - {activity.Action}");
    }
}

public class LoginActivityFilter : IActivityFilter
{
    public bool IsAllowed(UserActivity activity)
    {
        return activity.Action.Contains("LOGIN");
    }
}

public class FileLogger : IFileLogger
{
    public void Write(UserActivity activity)
    {
        Console.WriteLine($"[File] {activity.UserName}: {activity.Action}");
    }
}

public class ServerLogger : IServerLogger
{
    public void Send(UserActivity activity)
    {
        Console.WriteLine($"[Server] {activity.UserName}: {activity.Action}");
    }
}
#endregion

#region Service
public class UserActivityService
{
    private readonly IActivityRecorder _recorder;
    private readonly IActivityFilter _filter;
    private readonly IFileLogger _fileLogger;
    private readonly IServerLogger _serverLogger;

    public UserActivityService(
        IActivityRecorder recorder,
        IActivityFilter filter,
        IFileLogger fileLogger,
        IServerLogger serverLogger)
    {
        _recorder = recorder;
        _filter = filter;
        _fileLogger = fileLogger;
        _serverLogger = serverLogger;
    }

    public void Process(UserActivity activity)
    {
        _recorder.Record(activity);

        if (!_filter.IsAllowed(activity))
        {
            Console.WriteLine("Activity rejected by filter");
            return;
        }

        _fileLogger.Write(activity);
        _serverLogger.Send(activity);
    }
}
#endregion

class Program
{
    static void Main()
    {
        var activity = new UserActivity
        {
            UserName = "Taras",
            Action = "LOGIN_SUCCESS"
        };

        var service = new UserActivityService(
            new SimpleActivityRecorder(),
            new LoginActivityFilter(),
            new FileLogger(),
            new ServerLogger()
        );

        service.Process(activity);
    }
}