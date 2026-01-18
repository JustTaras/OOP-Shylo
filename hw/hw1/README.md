# Анти-патерн **God Object** та принцип єдиної відповідальності (SRP)

## 1. Анти-патерн God Object

**God Object** - це анти-патерн проєктування програмного забезпечення, за якого один клас або об’єкт зосереджує в собі надто багато обов’язків і контролює значну частину логіки системи.

### Основні характеристики God Object:
- **Надмірна кількість відповідальностей** - клас виконує багато різних задач.
- **Висока зв’язаність (High Coupling)** - об’єкт напряму залежить від багатьох інших класів.
- **Низька згуртованість (Low Cohesion)** - методи класу слабко пов’язані між собою.
- **Складність підтримки** - будь-яка зміна може вплинути на велику частину системи.
- **Погана тестованість** - важко ізолювати логіку для модульних тестів.

God Object часто виникає, коли вся логіка системи концентрується в одному «центральному» класі.

---

## 2. Приклад класу, який порушує SRP (C#)

**SRP (Single Responsibility Principle)** говорить, що клас повинен мати **тільки одну причину для зміни**.

### Приклад класу з порушенням SRP:

```csharp
public class UserManager
{
    public void CreateUser(string name)
    {
        // Бізнес-логіка створення користувача
        Console.WriteLine($"User created: {name}");
    }

    public void SaveToDatabase(string name)
    {
        // Робота з базою даних
        Console.WriteLine($"Saving user to database: {name}");
    }

    public void SendEmail(string name)
    {
        // Надсилання email
        Console.WriteLine($"Sending email to: {name}");
    }
}

---

## 3. Рефакторинг класу для дотримання SRP

Для дотримання принципу єдиної відповідальності необхідно розділити логіку класу на кілька окремих класів, кожен з яких відповідатиме лише за одну задачу.

### Клас для бізнес-логіки користувача
```csharp
public class UserService
{
    public void CreateUser(string name)
    {
        Console.WriteLine($"User created: {name}");
    }
}
Клас для роботи з базою даних
public class UserRepository
{
    public void Save(string name)
    {
        Console.WriteLine($"Saving user to database: {name}");
    }
}

Клас для надсилання повідомлень
public class EmailService
{
    public void Send(string name)
    {
        Console.WriteLine($"Sending email to: {name}");
    }
}