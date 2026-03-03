Юніт-тестування
1. Що таке юніт-тестування

Юніт-тестування (Unit Testing) — це рівень тестування, при якому перевіряється окремий модуль програми: клас, метод або функція. Головна ідея — ізолювати одиницю коду та переконатися, що вона працює правильно незалежно від інших компонентів системи.

У C# для юніт-тестування часто використовують фреймворки:

NUnit

xUnit

MSTest

Юніт-тести зазвичай:

швидкі;

автоматизовані;

ізольовані від зовнішніх залежностей;

запускаються часто (перед комітом, у CI/CD).

2. Переваги та обмеження юніт-тестування порівняно з інтеграційним
Переваги юніт-тестування:

Швидкість виконання — тестуються окремі методи без підключення БД чи мережі.

Локалізація помилки — якщо тест падає, легко знайти проблемний метод.

Підтримка рефакторингу — дозволяє безпечно змінювати код.

Документація поведінки — тест показує, як повинен працювати метод.

Обмеження:

Не перевіряє взаємодію між модулями.

Не виявляє проблеми конфігурації.

Може створювати хибне відчуття повної надійності системи.

Інтеграційне тестування

Інтеграційні тести перевіряють, як компоненти працюють разом (наприклад: сервіс + база даних + API).

Переваги:

Виявляють проблеми взаємодії.

Перевіряють реальну роботу системи.

Недоліки:

Повільніші.

Складніші в налаштуванні.

Важче локалізувати помилку.

3. Приклад класу та юніт-тести (C#)
Основний клас
public class Calculator
{
    public int Add(int a, int b)
    {
        return a + b;
    }

    public int Divide(int a, int b)
    {
        if (b == 0)
            throw new ArgumentException("Division by zero");

        return a / b;
    }

    public bool IsEven(int number)
    {
        return number % 2 == 0;
    }
}
Юніт-тести (NUnit)
using NUnit.Framework;
using System;

[TestFixture]
public class CalculatorTests
{
    private Calculator _calculator;

    [SetUp]
    public void Setup()
    {
        _calculator = new Calculator();
    }

    // ===== Add =====

    [Test]
    public void Add_ShouldReturnCorrectSum()
    {
        int result = _calculator.Add(2, 3);
        Assert.AreEqual(5, result);
    }

    [Test]
    public void Add_WithZero_ShouldReturnSameNumber()
    {
        int result = _calculator.Add(5, 0);
        Assert.AreEqual(5, result);
    }

    // ===== Divide =====

    [Test]
    public void Divide_ShouldReturnCorrectResult()
    {
        int result = _calculator.Divide(10, 2);
        Assert.AreEqual(5, result);
    }

    [Test]
    public void Divide_ByZero_ShouldThrowException()
    {
        Assert.Throws<ArgumentException>(() => _calculator.Divide(10, 0));
    }

    // ===== IsEven =====

    [Test]
    public void IsEven_WithEvenNumber_ShouldReturnTrue()
    {
        bool result = _calculator.IsEven(4);
        Assert.IsTrue(result);
    }

    [Test]
    public void IsEven_WithZero_ShouldReturnTrue()
    {
        bool result = _calculator.IsEven(0);
        Assert.IsTrue(result);
    }
}

У кожного методу:

1 тест — звичайний сценарій

1 тест — граничний випадок

4. Коли використовувати mock-об’єкти

Mock-об’єкти застосовуються, коли клас залежить від:

бази даних;

зовнішнього API;

файлової системи;

часу (DateTime.Now);

складних сервісів.

Наприклад:

public class OrderService
{
    private readonly IEmailService _emailService;

    public OrderService(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public void CreateOrder()
    {
        // логіка створення замовлення
        _emailService.Send("Order created");
    }
}

Тут IEmailService краще замокати, щоб:

не відправляти реальні листи;

перевірити, чи був викликаний метод Send().

Для цього часто використовують бібліотеку Moq.

Коли mock не потрібен

Mock можна не використовувати, якщо:

метод не має зовнішніх залежностей;

клас працює лише з простими даними;

це чиста бізнес-логіка (наприклад, математичні обчислення).

У нашому прикладі Calculator mock не потрібен, бо він не має залежностей.

Висновок

Юніт-тестування — це основа якісної розробки програмного забезпечення. Воно забезпечує:

стабільність коду;

безпечний рефакторинг;

швидке виявлення помилок.