using System;

namespace IndependentWork20
{
    // ==========================================
    // ПАТЕРН STRATEGY
    // ==========================================

    // Інтерфейс стратегії
    public interface IDataProcessorStrategy
    {
        void Process(string data);
    }

    // Конкретні реалізації стратегій (Варіант 20)
    public class PublishNewsStrategy : IDataProcessorStrategy
    {
        public void Process(string data)
        {
            Console.WriteLine($"[Strategy: Publish] Публікація новини на головній сторінці: '{data}'");
        }
    }

    public class ArchiveNewsStrategy : IDataProcessorStrategy
    {
        public void Process(string data)
        {
            Console.WriteLine($"[Strategy: Archive] Переміщення новини до архіву бази даних: '{data}'");
        }
    }

    public class EditNewsStrategy : IDataProcessorStrategy
    {
        public void Process(string data)
        {
            Console.WriteLine($"[Strategy: Edit] Відправлення новини редактору на доопрацювання: '{data}'");
        }
    }

    // Контекст для використання стратегії
    public class DataContext
    {
        private IDataProcessorStrategy _strategy;

        // Встановлення початкової стратегії через конструктор
        public DataContext(IDataProcessorStrategy strategy)
        {
            _strategy = strategy;
        }

        // Зміна стратегії в рантаймі
        public void SetStrategy(IDataProcessorStrategy strategy)
        {
            _strategy = strategy;
            Console.WriteLine("--> Стратегію змінено.");
        }

        // Делегування виконання поточній стратегії
        public void ExecuteProcessing(string data)
        {
            if (_strategy == null)
            {
                Console.WriteLine("Помилка: Стратегія не встановлена.");
                return;
            }
            _strategy.Process(data);
        }
    }

    // ==========================================
    // ПАТЕРН OBSERVER
    // ==========================================

    // Subject (Видавець)
    public class DataPublisher
    {
        // Використання делегата Action для події
        public event Action<string> DataProcessed;

        public void PublishDataProcessed(string data)
        {
            Console.WriteLine($"\n[Publisher] Сповіщення спостерігачів про новину: '{data}'");
            // Безпечний виклик події (якщо є підписники)
            DataProcessed?.Invoke(data);
        }
    }

    // Класи-спостерігачі (Observers)
    public class NewsFeedObserver
    {
        public void OnDataProcessed(string data)
        {
            Console.WriteLine($"   - [Observer: NewsFeed] Оновлено стрічку новин користувачів. Новина: '{data}'");
        }
    }

    public class SocialMediaPublisherObserver
    {
        public void OnDataProcessed(string data)
        {
            Console.WriteLine($"   - [Observer: SocialMedia] Згенеровано пост для соцмереж (Facebook/Telegram). Новина: '{data}'");
        }
    }

    // ==========================================
    // ДЕМОНСТРАЦІЯ (Клієнтський код)
    // ==========================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Система обробки новин (Варіант 20) ===\n");

            // 1. Ініціалізація Publisher та Observers
            DataPublisher publisher = new DataPublisher();
            
            NewsFeedObserver feedObserver = new NewsFeedObserver();
            SocialMediaPublisherObserver socialObserver = new SocialMediaPublisherObserver();

            // Підписка спостерігачів на подію
            publisher.DataProcessed += feedObserver.OnDataProcessed;
            publisher.DataProcessed += socialObserver.OnDataProcessed;

            // 2. Ініціалізація Context зі стартовою стратегією
            DataContext context = new DataContext(new EditNewsStrategy());

            string news1 = "Вчені винайшли новий спосіб передачі енергії";
            string news2 = "Штучний інтелект написав симфонію";
            string news3 = "Стара стаття за 2020 рік про технології";

            // 3. Використання стратегії редагування
            context.ExecuteProcessing(news1);

            // Зміна стратегії на "Публікацію" та сповіщення підписників
            context.SetStrategy(new PublishNewsStrategy());
            context.ExecuteProcessing(news2);
            publisher.PublishDataProcessed(news2); // Сповіщаємо, що новина опублікована

            // Зміна стратегії на "Архівування"
            context.SetStrategy(new ArchiveNewsStrategy());
            context.ExecuteProcessing(news3);
            
            Console.WriteLine("\nРоботу завершено.");
        }
    }
}