using System;

namespace lab1v20
{
    class Figure
    {
        // Приватне поле
        private string name;

        // Публічна властивість (площа фігури)
        public double Area { get; set; }

        // Конструктор
        public Figure(string name, double area)
        {
            this.name = name;
            Area = area;
            Console.WriteLine($"Створено об'єкт: {name}");
        }

        // Метод, який виводить інформацію про фігуру
        public void GetFigure()
        {
            Console.WriteLine($"Фігура: {name}, Площа: {Area}");
        }

        // Деструктор (для демонстрації)
        ~Figure()
        {
            Console.WriteLine($"Об'єкт {name} видалено з пам’яті.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Лабораторна робота №1 ===");

            // Створюємо кілька об’єктів класу Figure
            Figure square = new Figure("Квадрат", 25.0);
            Figure circle = new Figure("Коло", 78.5);
            Figure triangle = new Figure("Трикутник", 36.2);

            // Викликаємо методи
            square.GetFigure();
            circle.GetFigure();
            triangle.GetFigure();

            Console.WriteLine("Роботу завершено!");
        }
    }
}