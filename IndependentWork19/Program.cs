using System;

namespace IndependentWork19
{
    // ==========================================
    // 1. Абстракція (Інтерфейс)
    // ==========================================
    public interface IResourceManager
    {
        void Manage(string resourceData);
    }

    // ==========================================
    // 2. Конкретні реалізації ресурсів
    // ==========================================
    public class FileResourceManager : IResourceManager
    {
        public void Manage(string resourceData)
        {
            Console.WriteLine($"[FILE] Обробка файлового ресурсу: {resourceData}");
        }
    }

    public class NetworkResourceManager : IResourceManager
    {
        public void Manage(string resourceData)
        {
            Console.WriteLine($"[NETWORK] Обробка мережевого запиту: {resourceData}");
        }
    }

    public class DatabaseResourceManager : IResourceManager
    {
        public void Manage(string resourceData)
        {
            Console.WriteLine($"[DB] Виконання запиту до бази даних: {resourceData}");
        }
    }

    // ==========================================
    // 3. Патерн Factory Method (Абстрактна фабрика)
    // ==========================================
    public abstract class ResourceManagerFactory
    {
        // Фабричний метод
        protected abstract IResourceManager CreateResourceManager();

        // Основна бізнес-логіка, яка використовує фабричний метод
        public void ProcessResource(string resourceData)
        {
            IResourceManager manager = CreateResourceManager();
            manager.Manage(resourceData);
        }
    }

    // ==========================================
    // 4. Конкретні фабрики
    // ==========================================
    public class FileResourceManagerFactory : ResourceManagerFactory
    {
        protected override IResourceManager CreateResourceManager()
        {
            return new FileResourceManager();
        }
    }

    public class NetworkResourceManagerFactory : ResourceManagerFactory
    {
        protected override IResourceManager CreateResourceManager()
        {
            return new NetworkResourceManager();
        }
    }

    public class DatabaseResourceManagerFactory : ResourceManagerFactory
    {
        protected override IResourceManager CreateResourceManager()
        {
            return new DatabaseResourceManager();
        }
    }

    // ==========================================
    // 5. Патерн Singleton (Менеджер сервісів)
    // ==========================================
    public class ResourceService
    {
        private static ResourceService _instance;
        private static readonly object _lock = new object();
        private ResourceManagerFactory _currentFactory;

        // Приватний конструктор, щоб запобігти створенню об'єктів через new
        private ResourceService() { }

        // Глобальна точка доступу до єдиного екземпляра
        public static ResourceService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock) // Потокобезпечна реалізація
                    {
                        if (_instance == null)
                        {
                            _instance = new ResourceService();
                        }
                    }
                }
                return _instance;
            }
        }

        // Встановлення поточної фабрики
        public void SetFactory(ResourceManagerFactory factory)
        {
            _currentFactory = factory;
        }

        // Делегування роботи поточній фабриці
        public void Manage(string resourceData)
        {
            if (_currentFactory == null)
            {
                Console.WriteLine("Помилка: Фабрика ресурсів не встановлена!");
                return;
            }
            _currentFactory.ProcessResource(resourceData);
        }
    }

    // ==========================================
    // 6. Демонстрація роботи (Клієнтський код)
    // ==========================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Система управління ресурсами (Варіант 20) ===\n");

            // Отримуємо екземпляр Singleton
            ResourceService service = ResourceService.Instance;

            // 1. Робота з файлами
            Console.WriteLine("--- Перемикання на FileResourceManager ---");
            service.SetFactory(new FileResourceManagerFactory());
            service.Manage("config.json");
            service.Manage("data_export.csv");

            // 2. Робота з мережею
            Console.WriteLine("\n--- Перемикання на NetworkResourceManager ---");
            service.SetFactory(new NetworkResourceManagerFactory());
            service.Manage("GET /api/users/1");
            service.Manage("POST /api/auth");

            // 3. Робота з базою даних
            Console.WriteLine("\n--- Перемикання на DatabaseResourceManager ---");
            service.SetFactory(new DatabaseResourceManagerFactory());
            service.Manage("SELECT * FROM Inventory");
            service.Manage("UPDATE Settings SET Theme='Dark'");

            Console.WriteLine("\nРоботу завершено.");
        }
    }
}