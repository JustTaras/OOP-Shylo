using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab6_LambdaDelegates
{
    // Клас Car (модель, пробіг, витрата пального)
    class Car
    {
        public string Model { get; set; }
        public int Mileage { get; set; }              // Пробіг у км
        public double FuelConsumption { get; set; }   // Витрата, л/100 км

        public Car(string model, int mileage, double fuelConsumption)
        {
            Model = model;
            Mileage = mileage;
            FuelConsumption = fuelConsumption;
        }

        public override string ToString()
        {
            return $"{Model} | Пробіг: {Mileage} км | Витрата: {FuelConsumption} л/100км";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Початковий список автомобілів
            List<Car> cars = new List<Car>
            {
                new Car("Toyota Corolla", 85000, 6.8),
                new Car("Honda Civic", 120000, 7.2),
                new Car("Volkswagen Golf", 195000, 6.5),
                new Car("BMW 320d", 210000, 5.9),
                new Car("Ford Focus", 99000, 7.8)
            };

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Початковий список автомобілів ===");
            cars.ForEach(c => Console.WriteLine(c));
            Console.WriteLine();

            // === ПРИКЛАД 1: Predicate<T> + анонімний метод ===
            // Predicate<Car> - ВБУДОВАНИЙ ДЕЛЕГАТ, АНОНІМНИЙ МЕТОД
            Predicate<Car> highMileagePredicate = delegate (Car c)
            {
                return c.Mileage > 100000;
            };

            List<Car> highMileageCars = cars.FindAll(highMileagePredicate);
            Console.WriteLine("=== Автомобілі з пробігом > 100000 км (Predicate + anonymous) ===");
            highMileageCars.ForEach(c => Console.WriteLine(c));
            Console.WriteLine();

            // === ПРИКЛАД 2: Predicate<T> з лямбда-виразом ===
            // Predicate<Car> - ВБУДОВАНИЙ ДЕЛЕГАТ, ЛЯМБДА-ВИРАЗ
            Predicate<Car> highMileageLambda = c => c.Mileage > 100000;
            var highMileageCars2 = cars.FindAll(highMileageLambda);

            Console.WriteLine("=== Ті ж авто з пробігом > 100000 км (Predicate + lambda) ===");
            highMileageCars2.ForEach(c => Console.WriteLine(c));
            Console.WriteLine();

            // === ПРИКЛАД 3: Func<T, TResult> для обчислення середньої витрати ===
            // Func<List<Car>, double> - ВБУДОВАНИЙ ДЕЛЕГАТ, ЛЯМБДА-ВИРАЗ
            Func<List<Car>, double> averageFuelConsumption = list =>
                list.Average(c => c.FuelConsumption);

            double avgConsumption = averageFuelConsumption(cars);
            Console.WriteLine($"Середня витрата пального по всіх авто: {avgConsumption:F2} л/100км");
            Console.WriteLine();

            // === ПРИКЛАД 4: Func<T, TResult> для пошуку авто з мінімальною витратою ===
            // Func<List<Car>, Car> - ВБУДОВАНИЙ ДЕЛЕГАТ, ЛЯМБДА-ВИРАЗ + LINQ OrderBy
            Func<List<Car>, Car> minConsumptionCarFunc = list =>
                list.OrderBy(c => c.FuelConsumption).First();

            Car minConsumptionCar = minConsumptionCarFunc(cars);
            Console.WriteLine("Автомобіль з мінімальною витратою пального:");
            Console.WriteLine(minConsumptionCar);
            Console.WriteLine();

            // === ПРИКЛАД 5: Action<T> для виводу інформації про авто ===
            // Action<Car> - ВБУДОВАНИЙ ДЕЛЕГАТ, ЛЯМБДА-ВИРАЗ
            Action<Car> printCarAction = car =>
            {
                Console.WriteLine($"[INFO] {car.Model}: {car.FuelConsumption} л/100км, {car.Mileage} км");
            };

            Console.WriteLine("=== Вивід авто за допомогою Action<Car> ===");
            cars.ForEach(c => printCarAction(c));
            Console.WriteLine();

            // === LINQ: Where, Select, OrderBy, Aggregate ===

            // Where: відбір авто з пробігом > 100000 км
            var filteredByMileage = cars.Where(c => c.Mileage > 100000);
            Console.WriteLine("=== Where: авто з пробігом > 100000 км ===");
            foreach (var car in filteredByMileage)
            {
                Console.WriteLine(car);
            }
            Console.WriteLine();

            // Select: спроєктуємо тільки модель та витрату пального
            var modelAndConsumption = cars
                .Select(c => new { c.Model, c.FuelConsumption });

            Console.WriteLine("=== Select: тільки модель та витрата пального ===");
            foreach (var item in modelAndConsumption)
            {
                Console.WriteLine($"{item.Model} -> {item.FuelConsumption} л/100км");
            }
            Console.WriteLine();

            // OrderBy: сортування авто за витратою пального
            var orderedByConsumption = cars
                .OrderBy(c => c.FuelConsumption);

            Console.WriteLine("=== OrderBy: авто за зростанням витрати пального ===");
            foreach (var car in orderedByConsumption)
            {
                Console.WriteLine(car);
            }
            Console.WriteLine();

            // Aggregate: сумарний пробіг усіх авто
            // Aggregate - LINQ-операція агрегації
            int totalMileage = cars
                .Select(c => c.Mileage)
                .Aggregate(0, (acc, current) => acc + current);

            Console.WriteLine($"Сумарний пробіг усіх автомобілів: {totalMileage} км");
            Console.WriteLine();

            Console.WriteLine("Роботу програми завершено. Натисніть будь-яку клавішу...");
            Console.ReadKey();
        }
    }
}
