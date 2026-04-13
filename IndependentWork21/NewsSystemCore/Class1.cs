using System;
using System.Collections.Generic;

namespace NewsSystemCore
{
    // ==========================================
    // 1. STRATEGY (Стратегії обробки новин)
    // ==========================================
    public interface INewsStrategy
    {
        string Process(string news);
    }

    public class PublishStrategy : INewsStrategy
    {
        public string Process(string news) => $"PUBLISHED: {news}";
    }

    public class ArchiveStrategy : INewsStrategy
    {
        public string Process(string news) => $"ARCHIVED: {news}";
    }

    // ==========================================
    // 2. FACTORY (Фабрика стратегій)
    // ==========================================
    public class StrategyFactory
    {
        public INewsStrategy CreateStrategy(string strategyType)
        {
            return strategyType.ToLower() switch
            {
                "publish" => new PublishStrategy(),
                "archive" => new ArchiveStrategy(),
                _ => throw new ArgumentException($"Unknown strategy type: {strategyType}")
            };
        }
    }

    // ==========================================
    // 3. OBSERVER (Видавець та Спостерігачі)
    // ==========================================
    public class NewsPublisher
    {
        public event Action<string> OnNewsProcessed;

        public void Notify(string data)
        {
            OnNewsProcessed?.Invoke(data);
        }
    }

    // Тестовий спостерігач, який зберігає історію подій
    public class TestObserver
    {
        public List<string> History { get; } = new List<string>();

        public void ReceiveUpdate(string data)
        {
            History.Add(data);
        }
    }

    // ==========================================
    // 4. SINGLETON (Головний менеджер системи)
    // ==========================================
    public class NewsSystemManager
    {
        private static NewsSystemManager _instance;
        private static readonly object _lock = new object();

        public StrategyFactory Factory { get; }
        public NewsPublisher Publisher { get; }

        private NewsSystemManager()
        {
            Factory = new StrategyFactory();
            Publisher = new NewsPublisher();
        }

        public static NewsSystemManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new NewsSystemManager();
                    }
                }
                return _instance;
            }
        }

        // Інтеграційний метод: створює стратегію, обробляє новину і сповіщає підписників
        public string ExecuteWorkflow(string strategyType, string newsContent)
        {
            if (string.IsNullOrWhiteSpace(newsContent))
                throw new ArgumentNullException(nameof(newsContent), "News content cannot be empty");

            INewsStrategy strategy = Factory.CreateStrategy(strategyType);
            string processedData = strategy.Process(newsContent);
            
            Publisher.Notify(processedData);
            
            return processedData;
        }

        // Метод для скидання стану (корисно для незалежності тестів)
        public static void Reset()
        {
            _instance = null;
        }
    }
}