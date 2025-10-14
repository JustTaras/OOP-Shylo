using System;
using System.Collections.Generic;

namespace lab2v20
{
    class Dictionary
    {
        // Приватне поле — внутрішнє сховище пар "ключ-значення"
        private Dictionary<string, string> data = new Dictionary<string, string>();

        // Властивість для підрахунку кількості елементів
        public int Count
        {
            get { return data.Count; }
        }

        // Індексатор — доступ до елементів за ключем
        public string this[string key]
        {
            get
            {
                if (data.ContainsKey(key))
                    return data[key];
                else
                    return "Ключ не знайдено.";
            }
            set
            {
                data[key] = value;
            }
        }

        // Перевантаження оператора + (додає пару ключ-значення)
        public static Dictionary operator +(Dictionary dict, (string key, string value) pair)
        {
            dict.data[pair.key] = pair.value;
            Console.WriteLine($"Додано: {pair.key} = {pair.value}");
            return dict;
        }

        // Перевантаження оператора - (видаляє пару за ключем)
        public static Dictionary operator -(Dictionary dict, string key)
        {
            if (dict.data.Remove(key))
                Console.WriteLine($"Видалено ключ: {key}");
            else
                Console.WriteLine($"Ключ '{key}' не знайдено для видалення.");
            return dict;
        }

        // Метод для виведення всіх елементів
        public void ShowAll()
        {
            Console.WriteLine("\nВміст словника:");
            foreach (var item in data)
                Console.WriteLine($"{item.Key} = {item.Value}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Лабораторна робота №2 ===");

            Dictionary myDict = new Dictionary();

            // Додаємо елементи через оператор +
            myDict += ("apple", "яблуко");
            myDict += ("banana", "банан");
            myDict += ("orange", "апельсин");

            // Виводимо всі елементи
            myDict.ShowAll();

            // Звертаємося через індексатор
            Console.WriteLine($"\nПереклад слова 'apple': {myDict["apple"]}");

            // Змінюємо значення через індексатор
            myDict["banana"] = "жовтий фрукт";
            Console.WriteLine($"Оновлений переклад 'banana': {myDict["banana"]}");

            // Видаляємо елемент через оператор -
            myDict -= "orange";

            // Показуємо фінальний результат
            myDict.ShowAll();

            Console.WriteLine($"\nКількість елементів: {myDict.Count}");
        }
    }
}