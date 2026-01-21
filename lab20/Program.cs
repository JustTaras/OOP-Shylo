using System;
using System.Collections.Generic;

namespace lab20
{
    // =========================
    // Модель замовлення
    // =========================
   public class Order
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
}

    // ==================================================
    // ПОЧАТКОВИЙ КЛАС (ПОРУШУЄ SRP)
    // ==================================================
    public class OrderProcessor
    {
        public void ProcessOrder(Order order)
        {
            // Валідація
            if (order.TotalAmount <= 0)
            {
                Console.WriteLine("Замовлення невалідне: сума повинна бути більшою за 0.");
                return;
            }

            // Збереження
            Console.WriteLine($"Замовлення #{order.Id} збережено в базу даних.");

            // Email
            Console.WriteLine($"Email надіслано клієнту {order.CustomerName}.");

            // Оновлення статусу
            order.Status = "Processed";
            Console.WriteLine($"Статус замовлення оновлено: {order.Status}");
        }
    }

    // ==================================================
    // ІНТЕРФЕЙСИ (SRP)
    // ==================================================
    public interface IOrderValidator
    {
        bool IsValid(Order order);
    }

   public interface IOrderRepository
{
    void Save(Order order);
    Order? GetById(int id);
}

    public interface IEmailService
    {
        void SendOrderConfirmation(Order order);
    }

    // ==================================================
    // РЕАЛІЗАЦІЇ (ЗАГЛУШКИ)
    // ==================================================
    public class OrderValidator : IOrderValidator
    {
        public bool IsValid(Order order)
        {
            return order.TotalAmount > 0;
        }
    }

    public class InMemoryOrderRepository : IOrderRepository
    {
        private readonly Dictionary<int, Order> _orders = new();

        public void Save(Order order)
        {
            _orders[order.Id] = order;
            Console.WriteLine($"Замовлення #{order.Id} збережено в памʼяті.");
        }

      public Order? GetById(int id)
{
    return _orders.ContainsKey(id) ? _orders[id] : null;
}
    }

    public class ConsoleEmailService : IEmailService
    {
        public void SendOrderConfirmation(Order order)
        {
            Console.WriteLine($"Email підтвердження надіслано клієнту {order.CustomerName}.");
        }
    }

    // ==================================================
    // КООРДИНАТОР (SRP + Dependency Injection)
    // ==================================================
    public class OrderService
    {
        private readonly IOrderValidator _validator;
        private readonly IOrderRepository _repository;
        private readonly IEmailService _emailService;

        public OrderService(
            IOrderValidator validator,
            IOrderRepository repository,
            IEmailService emailService)
        {
            _validator = validator;
            _repository = repository;
            _emailService = emailService;
        }

        public void ProcessOrder(Order order)
        {
            if (!_validator.IsValid(order))
            {
                Console.WriteLine("Замовлення невалідне.");
                return;
            }

            order.Status = "Processed";
            _repository.Save(order);
            _emailService.SendOrderConfirmation(order);

            Console.WriteLine($"Замовлення #{order.Id} успішно оброблено.");
        }
    }

    // ==================================================
    // DEMO
    // ==================================================
    class Program
    {
        static void Main()
        {
            Console.WriteLine("===== Варіант з порушенням SRP =====");
            var processor = new OrderProcessor();

            var order1 = new Order
            {
                Id = 1,
                CustomerName = "Taras",
                TotalAmount = 1200,
                Status = "New"
            };

            processor.ProcessOrder(order1);

            Console.WriteLine("\n===== Рефакторинг з SRP =====");

            IOrderValidator validator = new OrderValidator();
            IOrderRepository repository = new InMemoryOrderRepository();
            IEmailService emailService = new ConsoleEmailService();

            var orderService = new OrderService(
                validator,
                repository,
                emailService
            );

            Console.WriteLine("\n--- Валідне замовлення ---");
            var validOrder = new Order
            {
                Id = 2,
                CustomerName = "Ivan",
                TotalAmount = 800,
                Status = "New"
            };

            orderService.ProcessOrder(validOrder);

            Console.WriteLine("\n--- Невалідне замовлення ---");
            var invalidOrder = new Order
            {
                Id = 3,
                CustomerName = "Oleh",
                TotalAmount = 0,
                Status = "New"
            };

            orderService.ProcessOrder(invalidOrder);
        }
    }
}