using System;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace lab7v20
{
    public class FileProcessor
    {
        private int _attempts = 0;

        public string GetNotificationPayload(string path)
        {
            _attempts++;

            Console.WriteLine($"[FileProcessor] Виклик #{_attempts} для шляху: {path}");

            if (_attempts <= 3)
            {
                throw new IOException("Тимчасова помилка доступу до файлу (симуляція).");
            }

            return $"{{ \"title\": \"Hello\", \"body\": \"Test push from file '{path}'\" }}";
        }
    }

    public class NetworkClient
    {
        private int _sendAttempts = 0;

        public void SendPushNotification(string deviceId, string payload)
        {
            _sendAttempts++;

            Console.WriteLine($"[NetworkClient] Спроба відправки #{_sendAttempts} на пристрій {deviceId}");

            if (_sendAttempts <= 2)
            {
                throw new HttpRequestException("Тимчасова мережевa помилка під час надсилання push (симуляція).");
            }

            Console.WriteLine($"[NetworkClient] Push успішно надіслано на {deviceId} з payload:");
            Console.WriteLine(payload);
        }
    }

    public static class RetryHelper
    {
        public static T ExecuteWithRetry<T>(
    Func<T> operation,
    int retryCount = 3,
    TimeSpan initialDelay = default,
    Func<Exception, bool>? shouldRetry = null)
        {
            if (initialDelay == default)
            {
                initialDelay = TimeSpan.FromMilliseconds(500);
            }

            if (shouldRetry == null)
            {
                shouldRetry = _ => true;
            }

            int attempt = 0;

            while (true)
            {
                try
                {
                    attempt++;
                    Console.WriteLine($"[RetryHelper] Спроба #{attempt}...");

                    T result = operation();

                    Console.WriteLine("[RetryHelper] Операція виконана успішно.");
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[RetryHelper] Помилка на спробі #{attempt}: {ex.GetType().Name} - {ex.Message}");

                    if (attempt >= retryCount || !shouldRetry(ex))
                    {
                        Console.WriteLine("[RetryHelper] Ліміт спроб вичерпано або shouldRetry = false. Повторна спроба не буде виконана.");
                        throw;
                    }

                    double multiplier = Math.Pow(2, attempt - 1);
                    var delay = TimeSpan.FromMilliseconds(initialDelay.TotalMilliseconds * multiplier);

                    Console.WriteLine($"[RetryHelper] Очікування перед наступною спробою: {delay.TotalMilliseconds} мс.");
                    Thread.Sleep(delay);
                }
            }
        }

        public static void ExecuteWithRetry(
    Action operation,
    int retryCount = 3,
    TimeSpan initialDelay = default,
    Func<Exception, bool>? shouldRetry = null)
        {
            ExecuteWithRetry(
                () =>
                {
                    operation();
                    return true;
                },
                retryCount,
                initialDelay,
                shouldRetry);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var fileProcessor = new FileProcessor();
            var networkClient = new NetworkClient();

            string path = "notification.json";
            string deviceId = "DEVICE-12345";

            Func<Exception, bool> shouldRetry = ex =>
                ex is IOException || ex is HttpRequestException;

            Console.WriteLine("===== СЦЕНАРІЙ 1: Отримання payload з файлу з Retry =====");

            try
            {
                string payload = RetryHelper.ExecuteWithRetry(
                    operation: () => fileProcessor.GetNotificationPayload(path),
                    retryCount: 5,
                    initialDelay: TimeSpan.FromMilliseconds(500),
                    shouldRetry: shouldRetry
                );

                Console.WriteLine("Отриманий payload:");
                Console.WriteLine(payload);

                Console.WriteLine();
                Console.WriteLine("===== СЦЕНАРІЙ 2: Надсилання push-сповіщення з Retry =====");

                RetryHelper.ExecuteWithRetry(
                    operation: () => networkClient.SendPushNotification(deviceId, payload),
                    retryCount: 4,
                    initialDelay: TimeSpan.FromMilliseconds(500),
                    shouldRetry: shouldRetry
                );

                Console.WriteLine("Усі операції завершено успішно.");
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("=== КРИТИЧНА ПОМИЛКА ПІСЛЯ ВСІХ СПРОБ ===");
                Console.WriteLine($"{ex.GetType().Name}: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("Натисніть будь-яку клавішу, щоб вийти...");
            Console.ReadKey();
        }
    }
}
