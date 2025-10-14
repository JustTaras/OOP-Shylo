using System;
using System.Collections.Generic;
using System.Linq;

namespace lab3v20
{
    // 🔹 Базовий клас — Sensor
    class Sensor
    {
        public string Name { get; set; }

        public Sensor(string name)
        {
            Name = name;
        }

        // Віртуальний метод — кожен тип сенсора реалізує його по-своєму
        public virtual double ReadValue()
        {
            return 0.0;
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }

    // 🔹 Похідний клас TemperatureSensor
    class TemperatureSensor : Sensor
    {
        private Random rand = new Random();

        public TemperatureSensor(string name) : base(name) { }

        // Перевизначення методу ReadValue()
        public override double ReadValue()
        {
            // Випадкова температура від -10 до +40 °C
            return Math.Round(rand.NextDouble() * 50 - 10, 2);
        }
    }

    // 🔹 Похідний клас PressureSensor
    class PressureSensor : Sensor
    {
        private Random rand = new Random();

        public PressureSensor(string name) : base(name) { }

        // Перевизначення методу ReadValue()
        public override double ReadValue()
        {
            // Випадковий тиск від 700 до 800 мм рт. ст.
            return Math.Round(rand.NextDouble() * 100 + 700, 2);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Лабораторна робота №3: Наслідування ===\n");

            // 🔸 Створюємо список сенсорів різних типів (поліморфізм)
            List<Sensor> sensors = new List<Sensor>
            {
                new TemperatureSensor("Температурний датчик 1"),
                new TemperatureSensor("Температурний датчик 2"),
                new PressureSensor("Датчик тиску 1")
            };

            List<double> values = new List<double>();

            // 🔸 Зчитуємо показники
            foreach (var sensor in sensors)
            {
                double value = sensor.ReadValue();
                values.Add(value);
                Console.WriteLine($"{sensor}: {value}");
            }

            // 🔸 Обчислення середнього значення
            double avg = values.Average();
            Console.WriteLine($"\nСередній показник: {avg:F2}");

            // 🔸 Виявлення аномалій (вище або нижче середнього ±20%)
            double lowerBound = avg * 0.8;
            double upperBound = avg * 1.2;

            Console.WriteLine("\nАномальні значення:");
            foreach (var value in values)
            {
                if (value < lowerBound || value > upperBound)
                    Console.WriteLine($"⚠️  {value} — поза нормою");
            }

            Console.WriteLine("\nРоботу завершено.");
        }
    }
}