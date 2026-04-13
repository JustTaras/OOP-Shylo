using System;
using System.Collections.Generic;
using System.Text;

namespace IndependentWork23
{
    // ==========================================
    // Допоміжні класи (Моделі)
    // ==========================================
    public class Message
    {
        public string Text { get; set; }
    }

    // ==========================================
    // 1. ПАТЕРН ADAPTER
    // ==========================================
    
    // Target: Сучасний інтерфейс, який очікує наша система
    public interface IMessageQueue
    {
        void Send(string data);
    }

    // Adaptee: Стара система (застаріла шина повідомлень з іншим інтерфейсом та типами даних)
    public class OldMessageBus
    {
        public void PublishMessage(Message msg)
        {
            Console.WriteLine($"[OldMessageBus] Публікація повідомлення: {msg.Text}");
        }
    }

    // Adapter: Адаптує виклик IMessageQueue.Send() до OldMessageBus.PublishMessage()
    public class MessageBusAdapter : IMessageQueue
    {
        private readonly OldMessageBus _oldBus;

        public MessageBusAdapter(OldMessageBus oldBus)
        {
            _oldBus = oldBus;
        }

        public void Send(string data)
        {
            // Перетворення рядка у формат, який розуміє стара система
            Message msg = new Message { Text = data };
            _oldBus.PublishMessage(msg);
        }
    }

    // ==========================================
    // 2. ПАТЕРН FACADE
    // ==========================================

    // Subsystem 1
    public class MessageSerializer
    {
        public string Serialize(string content)
        {
            Console.WriteLine("[Serializer] Перетворення тексту в JSON...");
            return $"{{ \"content\": \"{content}\" }}";
        }
    }

    // Subsystem 2
    public class MessageEncryptor
    {
        public string Encrypt(string data)
        {
            Console.WriteLine("[Encryptor] Шифрування даних (Base64)...");
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
        }
    }

    // Facade: Спрощує використання підсистем і відправку повідомлень
    public class MessageQueueFacade
    {
        private readonly MessageSerializer _serializer;
        private readonly MessageEncryptor _encryptor;
        private readonly IMessageQueue _queue;

        public MessageQueueFacade(IMessageQueue queue)
        {
            _serializer = new MessageSerializer();
            _encryptor = new MessageEncryptor();
            _queue = queue;
        }

        // Клієнту достатньо викликати лише один метод
        public void SendEncryptedMessage(string text)
        {
            Console.WriteLine("\n--- Facade: Початок відправки повідомлення ---");
            string serialized = _serializer.Serialize(text);
            string encrypted = _encryptor.Encrypt(serialized);
            _queue.Send(encrypted); // Використовує Adapter під капотом
            Console.WriteLine("--- Facade: Відправка завершена ---\n");
        }
    }

    // ==========================================
    // 3. ПАТЕРН PROXY
    // ==========================================

    // Subject
    public interface IMessageConsumer
    {
        void Consume(string messageId);
    }

    // RealSubject: Виконує реальну ресурсоємну або ризиковану роботу
    public class RealMessageConsumer : IMessageConsumer
    {
        public void Consume(string messageId)
        {
            Console.WriteLine($"[RealConsumer] Спроба обробки повідомлення ID: {messageId}...");
            
            // Імітація помилки для певного повідомлення
            if (messageId == "msg_error_99")
            {
                throw new Exception("Збій підключення до бази даних!");
            }
            
            Console.WriteLine($"[RealConsumer] Повідомлення ID: {messageId} успішно оброблено.");
        }
    }

    // Proxy: Додає обробку помилок та кешування (щоб не обробляти двічі)
    public class ErrorHandlingMessageConsumerProxy : IMessageConsumer
    {
        private readonly RealMessageConsumer _realConsumer;
        // Кеш для збереження вже оброблених повідомлень (ліміти/кеш)
        private readonly HashSet<string> _processedMessagesCache = new HashSet<string>();

        public ErrorHandlingMessageConsumerProxy()
        {
            _realConsumer = new RealMessageConsumer();
        }

        public void Consume(string messageId)
        {
            Console.WriteLine($"[Proxy] Перехоплено запит на обробку '{messageId}'.");

            // 1. Перевірка кешу (Лімітування доступу до RealConsumer)
            if (_processedMessagesCache.Contains(messageId))
            {
                Console.WriteLine($"[Proxy] ПОПЕРЕДЖЕННЯ: Повідомлення '{messageId}' вже було оброблено. Ігнорую запит.\n");
                return;
            }

            // 2. Безпечний виклик з обробкою помилок
            try
            {
                _realConsumer.Consume(messageId);
                _processedMessagesCache.Add(messageId); // Зберігаємо успішний результат у кеш
                Console.WriteLine("[Proxy] Операція успішна.\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Proxy] ПЕРЕХОПЛЕНО ПОМИЛКУ: {ex.Message}. Падіння системи попереджено.\n");
            }
        }
    }

    // ==========================================
    // 4. ДЕМОНСТРАЦІЯ (Клієнтський код)
    // ==========================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Інтеграція: Adapter + Facade + Proxy (Варіант 20) ===\n");

            // --- Демонстрація Adapter та Facade ---
            Console.WriteLine(">>> 1. Відправка повідомлень (Facade + Adapter)");
            OldMessageBus oldBus = new OldMessageBus();
            IMessageQueue adaptedQueue = new MessageBusAdapter(oldBus);
            MessageQueueFacade facade = new MessageQueueFacade(adaptedQueue);

            facade.SendEncryptedMessage("Секретні дані користувача");

            // --- Демонстрація Proxy ---
            Console.WriteLine(">>> 2. Обробка повідомлень (Proxy з кешем та обробкою помилок)");
            IMessageConsumer consumerProxy = new ErrorHandlingMessageConsumerProxy();

            // Успішний сценарій
            consumerProxy.Consume("msg_ok_01");

            // Сценарій з кешуванням (спроба повторної обробки того ж повідомлення)
            consumerProxy.Consume("msg_ok_01");

            // Сценарій з помилкою (RealSubject викине Exception, але Proxy його перехопить)
            consumerProxy.Consume("msg_error_99");

            Console.WriteLine("Роботу системи завершено.");
        }
    }
}