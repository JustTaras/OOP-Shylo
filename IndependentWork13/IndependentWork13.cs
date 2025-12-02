using System;
using System.Threading;
using Polly;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace IndependentWork13
{
    internal class Program
    {
        private static readonly Random Random = new Random();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("Самостійна робота №13. Polly/Retry: дослідження кейсів відмовостійкості.\n");

            RunScenario1_ExternalApi();
            Separator();

            RunScenario2_DatabaseWithCircuitBreaker();
            Separator();

            RunScenario3_LongOperationWithTimeoutAndFallback();

            Console.WriteLine("\nНатисніть будь-яку клавішу для завершення...");
            Console.ReadKey();
        }

        private static void Separator()
        {
            Console.WriteLine("\n------------------------------------------------------------\n");
        }

        // =====================================================================
        // СЦЕНАРІЙ 1. Виклик зовнішнього API з тимчасовими помилками + Retry
        // =====================================================================
        /*
         * ПРОБЛЕМА:
         * Зовнішній HTTP-сервіс може повертати тимчасові помилки (наприклад, 500, 503),
         * бути недоступним через короткочасні проблеми в мережі.
         *
         * ВИБРАНА ПОЛІТИКА POLLY:
         *  - WaitAndRetry: кілька повторних спроб із затримкою між ними.
         * Обґрунтування: тимчасові помилки часто зникають самі по собі за 1-2 секунди,
         * тому повтор серед помірних затримок дозволяє уникнути падіння всієї операції.
         *
         * ОЧІКУВАНА ПОВЕДІНКА:
         *  - Перша(і) спроба(и) "API-виклику" кидають виняток.
         *  - Polly робить до 3 повторів із зростаючою затримкою.
         *  - У логах видно номер спроби та повідомлення про помилку.
         *  - Після успішної спроби виводиться повідомлення про успішний виклик API.
         */
        private static void RunScenario1_ExternalApi()
        {
            Console.WriteLine("Сценарій 1: Виклик зовнішнього API + Retry\n");

            int attempt = 0;

            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetry(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(retryAttempt),
                    onRetry: (exception, sleepDuration, retryNumber, context) =>
                    {
                        Console.WriteLine(
                            $"[Retry] Спроба #{retryNumber}, затримка {sleepDuration.TotalSeconds}с. " +
                            $"Помилка: {exception.Message}");
                    });

            try
            {
                retryPolicy.Execute(() =>
                {
                    attempt++;

                    Console.WriteLine($"[API] Виклик спроба #{attempt}...");

                    if (attempt <= 2)
                    {
                        throw new Exception("Імітація HTTP 503 Service Unavailable");
                    }

                    Console.WriteLine("[API] Успішна відповідь від сервера!");
                });

                Console.WriteLine("Результат: API виклик успішний після повторних спроб.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Результат: API виклик остаточно провалився. Помилка: {ex.Message}");
            }
        }

        // =====================================================================
        // СЦЕНАРІЙ 2. Доступ до БД + CircuitBreaker + Retry
        // =====================================================================
        /*
         * ПРОБЛЕМА:
         * База даних може час від часу “падати” або бути перевантаженою:
         * таймаути підключення, тимчасова відмова сервера і т.п.
         * Якщо продовжувати безкінечно "лупити" запити, це лише погіршить ситуацію.
         *
         * ВИБРАНА ПОЛІТИКА POLLY:
         *  - CircuitBreaker: "вибиває пробку" після кількох підряд помилок
         *    і на певний час блокує нові спроби.
         *  - Retry: повторює запит 1-2 рази перед тим, як віддати його в circuit breaker.
         *
         * Обґрунтування: Retry допомагає при одиничних збоях,
         * а CircuitBreaker захищає ресурс від лавинних навантажень, коли проблема постійна.
         *
         * ОЧІКУВАНА ПОВЕДІНКА:
         *  - Перші кілька звернень до БД завершуються помилкою (таймаут).
         *  - CircuitBreaker переходить у стан Open після 2 послідовних помилок.
         *  - Наступні виклики одразу відхиляються без спроб (BrokenCircuitException).
         *  - У логах видно стан breaker'а (Open, Half-Open, Reset).
         */
        private static void RunScenario2_DatabaseWithCircuitBreaker()
        {
            Console.WriteLine("Сценарій 2: Доступ до бази даних + CircuitBreaker + Retry\n");

            int dbCall = 0;

            var retryPolicy = Policy
                .Handle<TimeoutException>()
                .Retry(
                    retryCount: 1,
                    onRetry: (exception, retryNumber) =>
                    {
                        Console.WriteLine($"[DB-Retry] Повтор #{retryNumber}. Помилка: {exception.Message}");
                    });

            var circuitBreakerPolicy = Policy
                .Handle<TimeoutException>()
                .CircuitBreaker(
                    exceptionsAllowedBeforeBreaking: 2,
                    durationOfBreak: TimeSpan.FromSeconds(5),
                    onBreak: (exception, breakDelay) =>
                    {
                        Console.WriteLine($"[DB-CB] Перехід у стан Open на {breakDelay.TotalSeconds}с. Помилка: {exception.Message}");
                    },
                    onReset: () =>
                    {
                        Console.WriteLine("[DB-CB] Стан Reset (повернення до нормальної роботи).");
                    },
                    onHalfOpen: () =>
                    {
                        Console.WriteLine("[DB-CB] Стан Half-Open: тестовий запит до БД.");
                    });

            var combinedPolicy = Policy.Wrap(retryPolicy, circuitBreakerPolicy);

            for (int i = 1; i <= 6; i++)
            {
                Console.WriteLine($"\n[DB] Виклик #{i}");

                try
                {
                    combinedPolicy.Execute(() =>
                    {
                        dbCall++;

                        Console.WriteLine($"[DB] Спроба звернення до БД (dbCall={dbCall})...");

                        if (dbCall <= 4)
                        {
                            Thread.Sleep(200);
                            throw new TimeoutException("Імітація таймаута підключення до БД");
                        }

                        Console.WriteLine("[DB] Запит до БД виконано успішно!");
                    });
                }
                catch (BrokenCircuitException bce)
                {
                    Console.WriteLine($"[DB] Виклик заблоковано CircuitBreaker (Open). Повідомлення: {bce.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DB] Невідома помилка: {ex.Message}");
                }

                Thread.Sleep(1000);
            }
        }

        // =====================================================================
        // СЦЕНАРІЙ 3. Довга операція + Timeout + Fallback
        // =====================================================================
        /*
         * ПРОБЛЕМА:
         * Деякі операції можуть виконуватися занадто довго (зависати),
         * наприклад, важкий розрахунок або повільний зовнішній сервіс.
         * Краще обмежити час очікування й повернути "резервний" результат.
         *
         * ВИБРАНА ПОЛІТИКА POLLY:
         *  - Timeout: обмеження часу виконання операції.
         *  - Fallback: запасний сценарій, який повертає безпечний результат,
         *    якщо основна операція не встигла виконатися.
         *
         * Обґрунтування: замість того, щоб чекати "вічно" або падати з винятком,
         * застосунок повертає контрольований fallback-результат.
         *
         * ОЧІКУВАНА ПОВЕДІНКА:
         *  - Операція "працює" 3 секунди, але timeout = 1 секунда.
         *  - Політика Timeout кидає виняток TimeoutRejectedException.
         *  - Спрацьовує Fallback і повертає "резервний" результат.
         *  - У логах видно спрацювання Timeout і Fallback.
         */
        private static void RunScenario3_LongOperationWithTimeoutAndFallback()
        {
            Console.WriteLine("Сценарій 3: Довга операція + Timeout + Fallback\n");

            var timeoutPolicy = Policy
                .Timeout(
                    TimeSpan.FromSeconds(1),
                    TimeoutStrategy.Pessimistic,
                    onTimeout: (context, timespan, task, exception) =>
                    {
                        Console.WriteLine(
                            $"[Timeout] Операція перевищила ліміт {timespan.TotalSeconds}с. " +
                            $"Повідомлення: {exception?.Message}");
                    });

            var fallbackPolicy = Policy<string>
                .Handle<TimeoutRejectedException>()
                .Or<TimeoutException>()
                .Fallback(
                    fallbackValue: "Резервний результат: таймаут операції.",
                    onFallback: (result, context) =>
                    {
                        Console.WriteLine("[Fallback] Повертаємо резервний результат через таймаут.");
                    });

            var combinedPolicy = fallbackPolicy.Wrap(timeoutPolicy);

            string result = combinedPolicy.Execute(() =>
            {
                Console.WriteLine("[Operation] Старт довгої операції (3 секунди)...");
                Thread.Sleep(3000);
                Console.WriteLine("[Operation] Операція завершилась вчасно (але це не повинно статись при timeout=1с).");

                return "Основний результат: операція виконалась успішно.";
            });

            Console.WriteLine($"Результат для користувача: {result}");
        }
    }
}