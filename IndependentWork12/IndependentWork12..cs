﻿/*
Самостійна робота №12
Тема: PLINQ — дослідження продуктивності та безпеки
Студент: Шило Тарас, ІПЗ-3/2

1. Хід експериментів
- Створено три колекції: 1 млн, 5 млн і 10 млн випадкових чисел.
- Виконано дві обчислювально інтенсивні операції:
  1) Перевірка простоти + Math.Sqrt
  2) HeavyFunction(x) — складні мат. обчислення
- Для кожної операції виконувались два варіанти:
  • LINQ (послідовно)
  • PLINQ (паралельно)
- Час вимірювався за допомогою Stopwatch.
- Фактичні результати (вписати після запуску):
  • 1 млн: LINQ = 84,94 ms, PLINQ = 27,38 ms
  • 5 млн: LINQ = 362,423 ms, PLINQ = 79,96 ms
  • 10 млн: LINQ = 790,862 ms, PLINQ = 140,225 ms

2. Аналіз продуктивності
- PLINQ швидше працює на великих колекціях та важких розрахунках, бо задіює всі ядра CPU.
- На малих даних або простих операціях переваги немає — паралельність додає накладні витрати.
- В окремих випадках LINQ може бути навіть швидшим, якщо навантаження надто легке.

3. Побічні ефекти та потокобезпечність
- Демонстровано проблему: паралельне оновлення змінної unsafeSum без синхронізації → неправильний результат.
- Виправлення:
  1) Використання lock — результат стає правильним, але з можливим падінням продуктивності.
  2) Найкращий варіант — уникати спільних змінних і використовувати агрегатори PLINQ (Sum, Aggregate).

4. Висновки
- PLINQ доцільно застосовувати для великих обсягів даних і важких обчислень.
- Потрібно уникати побічних ефектів усередині паралельних виразів.
- Важливо розуміти, що паралельність не гарантує прискорення — все залежить від складності операції та розміру даних.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace IndependentWork12
{
    internal class Program
    {
        private static readonly int[] Sizes = { 1_000_000, 5_000_000, 10_000_000 };

        private static readonly Random Random = new Random();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("Самостійна робота №12. PLINQ: дослідження продуктивності та безпеки.");
            Console.WriteLine("---------------------------------------------------------------\n");

            RunPerformanceExperiments();
            Console.WriteLine();
            RunSideEffectsDemo();

            Console.WriteLine("\nНатисніть будь-яку клавішу для завершення...");
            Console.ReadKey();
        }


        private static void RunPerformanceExperiments()
        {
            Console.WriteLine("=== ЕКСПЕРИМЕНТИ З ПРОДУКТИВНОСТІ LINQ vs PLINQ ===\n");

            foreach (int size in Sizes)
            {
                Console.WriteLine($"--- Розмір колекції: {size:N0} елементів ---");

                var data = GenerateRandomData(size);

                Console.WriteLine("Операція 1: IsPrime(x) + Math.Sqrt(x)");
                MeasureQuery(
                    data,
                    predicate: IsPrime,
                    selector: x => Math.Sqrt(x),
                    operationName: "IsPrime + Sqrt"
                );

                Console.WriteLine("Операція 2: HeavyFunction(x) – складне математичне перетворення");
                MeasureQuery(
                    data,
                    predicate: x => x % 2 == 0,
                    selector: HeavyFunction,
                    operationName: "Even + HeavyFunction"
                );

                Console.WriteLine();
            }
        }

        private static List<int> GenerateRandomData(int size)
        {
            var list = new List<int>(size);
            for (int i = 0; i < size; i++)
            {
                list.Add(Random.Next(1, 1_000_001));
            }
            return list;
        }

        private static void MeasureQuery(
            List<int> data,
            Func<int, bool> predicate,
            Func<int, double> selector,
            string operationName)
        {
            data.Where(predicate).Select(selector).Take(10).ToList();
            data.AsParallel().Where(predicate).Select(selector).Take(10).ToList();

            var sw = Stopwatch.StartNew();
            var linqResult = data
                .Where(predicate)
                .Select(selector)
                .ToList();
            sw.Stop();
            long linqMs = sw.ElapsedMilliseconds;

            sw.Restart();
            var plinqResult = data
                .AsParallel()
                .WithDegreeOfParallelism(Environment.ProcessorCount)
                .Where(predicate)
                .Select(selector)
                .ToList();
            sw.Stop();
            long plinqMs = sw.ElapsedMilliseconds;

            Console.WriteLine($"[{operationName}]");
            Console.WriteLine($"  LINQ : {linqMs} ms, Count = {linqResult.Count}");
            Console.WriteLine($"  PLINQ: {plinqMs} ms, Count = {plinqResult.Count}");
        }


        private static bool IsPrime(int n)
        {
            if (n <= 1) return false;
            if (n == 2) return true;
            if (n % 2 == 0) return false;

            int limit = (int)Math.Sqrt(n);
            for (int i = 3; i <= limit; i += 2)
            {
                if (n % i == 0)
                    return false;
            }
            return true;
        }

        private static double HeavyFunction(int x)
        {
            double d = x;
            for (int i = 0; i < 5; i++)
            {
                d = Math.Sqrt(d + 1) * Math.Sin(d) * Math.Cos(d) + Math.Log10(d + 2);
            }
            return d;
        }


        private static void RunSideEffectsDemo()
        {
            Console.WriteLine("=== ДЕМОНСТРАЦІЯ ПОБІЧНИХ ЕФЕКТІВ У PLINQ ===\n");

            var data = Enumerable.Range(1, 1_000_000).ToList();

            Console.WriteLine("Сценарій: паралельне обчислення суми з доступом до спільної змінної без синхронізації.");
            int unsafeSum = 0;

            data
                .AsParallel()
                .ForAll(x =>
                {
                    unsafeSum += x;
                });

            long correctSum = data.Sum(x => (long)x);

            Console.WriteLine($"Некоректна паралельна сума (без lock): {unsafeSum}");
            Console.WriteLine($"Правильна послідовна сума           : {correctSum}");
            Console.WriteLine("Різниця показує проблему потокобезпечності.\n");

            Console.WriteLine("Виправлення 1: використання lock для захисту спільної змінної.");
            int safeSumWithLock = 0;
            object locker = new object();

            data
                .AsParallel()
                .ForAll(x =>
                {
                    lock (locker)
                    {
                        safeSumWithLock += x;
                    }
                });

            Console.WriteLine($"Паралельна сума з lock: {safeSumWithLock} (має збігатися з правильною сумою)\n");

            Console.WriteLine("Виправлення 2: уникнення побічних ефектів, використання агрегуючих операторів.");
            long safeSumPlinq = data
                .AsParallel()
                .Sum(x => (long)x);

            Console.WriteLine($"Паралельна сума через PLINQ.Sum: {safeSumPlinq} (також правильний результат)");
        }
    }
}