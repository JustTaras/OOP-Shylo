using System;
using System.Collections.Generic;

namespace lab21v9
{
    // Strategy interface
    public interface IParkingStrategy
    {
        decimal CalculateCost(int amount);
    }

    // Concrete strategies
    public class HourlyParkingStrategy : IParkingStrategy
    {
        public decimal CalculateCost(int hours)
        {
            decimal ratePerHour = 20m;
            decimal cost = hours * ratePerHour;

            if (hours >= 10)
            {
                cost *= 0.9m; // 10% discount
            }

            return cost;
        }
    }

    public class DailyParkingStrategy : IParkingStrategy
    {
        public decimal CalculateCost(int days)
        {
            decimal ratePerDay = 300m;
            decimal cost = days * ratePerDay;

            if (days >= 7)
            {
                cost *= 0.85m; // 15% discount
            }

            return cost;
        }
    }

    public class MonthlySubscriptionStrategy : IParkingStrategy
    {
        public decimal CalculateCost(int months)
        {
            decimal monthlyRate = 4000m;
            decimal cost = months * monthlyRate;

            if (months >= 3)
            {
                cost *= 0.8m; // 20% discount
            }

            return cost;
        }
    }

    // OCP demonstration: new strategy
    public class NightParkingStrategy : IParkingStrategy
    {
        public decimal CalculateCost(int hours)
        {
            decimal baseCost = hours * 15m;
            decimal nightExtra = 50m;
            return baseCost + nightExtra;
        }
    }

    // Factory Method
    public static class ParkingStrategyFactory
    {
        private static readonly Dictionary<string, IParkingStrategy> Strategies =
            new Dictionary<string, IParkingStrategy>(StringComparer.OrdinalIgnoreCase)
            {
                { "Hourly", new HourlyParkingStrategy() },
                { "Daily", new DailyParkingStrategy() },
                { "Monthly", new MonthlySubscriptionStrategy() },
                { "Night", new NightParkingStrategy() }
            };

        public static IParkingStrategy CreateStrategy(string parkingType)
        {
            if (Strategies.TryGetValue(parkingType, out IParkingStrategy? strategy))
            {
                return strategy;
            }

            throw new ArgumentException("Unknown parking type");
        }
    }

    // Service class
    public class ParkingService
    {
        public decimal CalculateParkingCost(int amount, IParkingStrategy strategy)
        {
            return strategy.CalculateCost(amount);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Choose parking type: Hourly / Daily / Monthly / Night");
            string? parkingTypeInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(parkingTypeInput))
            {
                Console.WriteLine("Invalid parking type");
                return;
            }

            Console.WriteLine("Enter number of hours / days / months:");
            string? amountInput = Console.ReadLine();

            if (!int.TryParse(amountInput, out int amount) || amount <= 0)
            {
                Console.WriteLine("Invalid number");
                return;
            }

            IParkingStrategy strategy = ParkingStrategyFactory.CreateStrategy(parkingTypeInput);
            ParkingService service = new ParkingService();

            decimal cost = service.CalculateParkingCost(amount, strategy);

            Console.WriteLine($"Parking cost: {cost} UAH");
        }
    }
}