Порушення принципу підстановки Лісков (LSP)

Принцип підстановки Лісков (LSP) стверджує, що об’єкти підкласів повинні бути взаємозамінними з об’єктами базового класу без зміни коректності роботи програми. Якщо підклас змінює очікувану поведінку базового класу — це порушення LSP.

Нижче наведено 3 приклади порушення LSP, окрім класичного прикладу з квадратом і прямокутником.

1. Птах → Пінгвін
Порушення
class Bird
{
    public virtual void Fly() { }
}

class Penguin : Bird
{
    public override void Fly()
    {
        throw new NotSupportedException();
    }
}

Чому це порушує LSP

Код, який працює з Bird, очікує, що будь-який птах може літати. Але Penguin не може виконати метод Fly, що призводить до помилок під час виконання.

Проблеми

Винятки в несподіваних місцях

Порушення контракту базового класу

Необхідність перевірок типів (if (bird is Penguin))

Як виправити

Розділити поведінки:

interface IFlyable
{
    void Fly();
}

class Sparrow : IFlyable { }
class Penguin { }

2. Базовий клас File → ReadOnlyFile
Порушення
class File
{
    public virtual void Write(string text) { }
}

class ReadOnlyFile : File
{
    public override void Write(string text)
    {
        throw new InvalidOperationException();
    }
}

Чому це порушує LSP

Код очікує, що будь-який File можна змінювати. Підклас звужує можливості базового класу, що заборонено LSP.

Проблеми

Падіння програми

Порушення відкритості системи

Приховані обмеження

Як виправити

Використати інтерфейси:

interface IReadableFile { }
interface IWritableFile { }

3. Банк → Заморожений рахунок
Порушення
class BankAccount
{
    public virtual void Withdraw(decimal amount) { }
}

class FrozenAccount : BankAccount
{
    public override void Withdraw(decimal amount)
    {
        throw new InvalidOperationException();
    }
}

Чому це порушує LSP

FrozenAccount не підтримує базову поведінку Withdraw, але використовується як BankAccount.

Проблеми

Логіка з перевірками стану рахунку

Складність підтримки

Порушення контрактів

Як виправити

Розділити стани або поведінки:

interface IWithdrawable { }
class ActiveAccount : IWithdrawable { }

Висновок

Порушення LSP виникає тоді, коли підклас:

викидає винятки замість коректної реалізації,

звужує поведінку базового класу,

змінює очікуваний контракт.