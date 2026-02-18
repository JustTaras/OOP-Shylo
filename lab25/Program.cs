using System;
using System.IO;

namespace Lab25
{
    // ==============================
    // LOGGER (Factory Method)
    // ==============================

    public interface ILogger
    {
        void Log(string message);
    }

    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine("[Console] " + message);
        }
    }

    public class FileLogger : ILogger
    {
        private readonly string _path;

        public FileLogger(string path)
        {
            _path = path;
        }

        public void Log(string message)
        {
            File.AppendAllText(_path, "[File] " + message + Environment.NewLine);
        }
    }

    public abstract class LoggerFactory
    {
        public abstract ILogger CreateLogger();
    }

    public class ConsoleLoggerFactory : LoggerFactory
    {
        public override ILogger CreateLogger()
        {
            return new ConsoleLogger();
        }
    }

    public class FileLoggerFactory : LoggerFactory
    {
        public override ILogger CreateLogger()
        {
            return new FileLogger("log.txt");
        }
    }

    // ==============================
    // Singleton
    // ==============================

    public class LoggerManager
    {
        private static LoggerManager? _instance;
        private LoggerFactory? _factory;

        private LoggerManager() { }

        public static LoggerManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LoggerManager();
                return _instance;
            }
        }

        public void SetFactory(LoggerFactory factory)
        {
            _factory = factory;
        }

        public ILogger GetLogger()
{
    if (_factory == null)
        throw new InvalidOperationException("LoggerFactory is not set.");

    return _factory.CreateLogger();
    }
    }

    // ==============================
    // Strategy
    // ==============================

    public interface IDataProcessorStrategy
    {
        string Process(string data);
    }

    public class EncryptDataStrategy : IDataProcessorStrategy
    {
        public string Process(string data)
        {
            return $"Encrypted({data})";
        }
    }

    public class CompressDataStrategy : IDataProcessorStrategy
    {
        public string Process(string data)
        {
            return $"Compressed({data})";
        }
    }

    public class DataContext
    {
        private IDataProcessorStrategy _strategy;

        public DataContext(IDataProcessorStrategy strategy)
        {
            _strategy = strategy;
        }

        public void SetStrategy(IDataProcessorStrategy strategy)
        {
            _strategy = strategy;
        }

        public string ProcessData(string data)
        {
            return _strategy.Process(data);
        }
    }

    // ==============================
    // Observer
    // ==============================

    public class DataPublisher
    {
        public event Action<string>? DataProcessed;

        public void Notify(string processedData)
        {
            DataProcessed?.Invoke(processedData);
        }
    }

    public class ProcessingLoggerObserver
    {
        public void Subscribe(DataPublisher publisher)
        {
            publisher.DataProcessed += OnDataProcessed;
        }

        private void OnDataProcessed(string data)
        {
            ILogger logger = LoggerManager.Instance.GetLogger();
            logger.Log("Processed data: " + data);
        }
    }

    // ==============================
    // Program (Demonstration)
    // ==============================

    class Program
    {
        static void Main(string[] args)
        {
            RunScenario1();
            RunScenario2();
            RunScenario3();
        }

        static void RunScenario1()
        {
            Console.WriteLine("========== SCENARIO 1 ==========");

            LoggerManager.Instance.SetFactory(new ConsoleLoggerFactory());

            DataContext context = new DataContext(new EncryptDataStrategy());
            DataPublisher publisher = new DataPublisher();

            ProcessingLoggerObserver observer = new ProcessingLoggerObserver();
            observer.Subscribe(publisher);

            string result = context.ProcessData("Hello");
            publisher.Notify(result);

            Console.WriteLine();
        }

        static void RunScenario2()
        {
            Console.WriteLine("========== SCENARIO 2 ==========");

            LoggerManager.Instance.SetFactory(new FileLoggerFactory());

            DataContext context = new DataContext(new EncryptDataStrategy());
            DataPublisher publisher = new DataPublisher();

            ProcessingLoggerObserver observer = new ProcessingLoggerObserver();
            observer.Subscribe(publisher);

            string result = context.ProcessData("SecondRun");
            publisher.Notify(result);

            Console.WriteLine("Перевір файл log.txt");
            Console.WriteLine();
        }

        static void RunScenario3()
        {
            Console.WriteLine("========== SCENARIO 3 ==========");

            LoggerManager.Instance.SetFactory(new ConsoleLoggerFactory());

            DataContext context = new DataContext(new EncryptDataStrategy());
            DataPublisher publisher = new DataPublisher();

            ProcessingLoggerObserver observer = new ProcessingLoggerObserver();
            observer.Subscribe(publisher);

            string result1 = context.ProcessData("BeforeChange");
            publisher.Notify(result1);

            context.SetStrategy(new CompressDataStrategy());

            string result2 = context.ProcessData("AfterChange");
            publisher.Notify(result2);

            Console.WriteLine();
        }
    }
}