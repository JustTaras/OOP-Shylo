## Принципи ISP та DIP (SOLID)

### 1. Принцип ISP (Interface Segregation Principle)

**ISP** стверджує: *клієнти не повинні залежати від інтерфейсів, які вони не використовують*. Іншими словами, краще мати кілька **вузьких (малих)** інтерфейсів, ніж один великий і універсальний.

#### Приклад інтерфейсу, що порушує ISP

```csharp
public interface IWorker
{
    void Work();
    void Eat();
    void Sleep();
}
```

Проблема: якщо класу потрібен лише метод `Work()`, він все одно змушений реалізовувати `Eat()` і `Sleep()`.

```csharp
public class Robot : IWorker
{
    public void Work() { /* працює */ }
    public void Eat() { throw new NotImplementedException(); }
    public void Sleep() { throw new NotImplementedException(); }
}
```

#### Вирішення проблеми (застосування ISP)

```csharp
public interface IWorkable
{
    void Work();
}

public interface IEatable
{
    void Eat();
}

public interface ISleepable
{
    void Sleep();
}
```

```csharp
public class Human : IWorkable, IEatable, ISleepable
{
    public void Work() { }
    public void Eat() { }
    public void Sleep() { }
}

public class Robot : IWorkable
{
    public void Work() { }
}
```

Тепер кожен клас реалізує лише те, що йому дійсно потрібно.

---

### 2. Принцип DIP (Dependency Inversion Principle)

**DIP** говорить: *високорівневі модулі не повинні залежати від низькорівневих; обидва мають залежати від абстракцій*. Також абстракції не повинні залежати від деталей реалізації.

#### Порушення DIP

```csharp
public class EmailSender
{
    public void Send(string message) { }
}

public class NotificationService
{
    private EmailSender sender = new EmailSender();

    public void Notify(string msg)
    {
        sender.Send(msg);
    }
}
```

Клас `NotificationService` жорстко залежить від `EmailSender`.

#### Застосування DIP через Dependency Injection

```csharp
public interface IMessageSender
{
    void Send(string message);
}

public class EmailSender : IMessageSender
{
    public void Send(string message) { }
}
```

```csharp
public class NotificationService
{
    private readonly IMessageSender _sender;

    public NotificationService(IMessageSender sender)
    {
        _sender = sender;
    }

    public void Notify(string msg)
    {
        _sender.Send(msg);
    }
}
```

Тепер залежність передається ззовні (через конструктор) — це і є **Dependency Injection**.

---

### 3. Переваги застосування DIP

* зменшення жорстких звʼязків між класами;
* легка заміна реалізацій (Email, SMS, Push);
* краща масштабованість системи;
* значно простіше писати модульні тести.

---

### 4. Як ISP допомагає DI та тестуванню

Вузькі інтерфейси:

* простіше реалізувати **mock** або **stub** для тестів;
* зменшують кількість непотрібних залежностей;
* роблять Dependency Injection зрозумілішим і чистішим.

#### Приклад для тестування

```csharp
public class FakeSender : IMessageSender
{
    public void Send(string message)
    {
        // імітація відправки
    }
}
```

Завдяки малому інтерфейсу `IMessageSender` тестування не потребує реальної відправки повідомлень.

---

### Висновок

ISP і DIP тісно повʼязані між собою: **вузькі інтерфейси (ISP)** роблять **інʼєкцію залежностей (DIP)** простою та ефективною, а код — гнучким, підтримуваним і зручним для тестування.