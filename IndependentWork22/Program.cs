using System;
using System.Collections.Generic;
using System.Text;

namespace IndependentWork22
{
    // ==========================================
    // 1. Спільний інтерфейс компонентів
    // ==========================================
    public interface IComponent
    {
        string GenerateHtml();
    }

    // ==========================================
    // 2. ПАТЕРН COMPOSITE (Leaf та Composite)
    // ==========================================

    // Leaf: Абзац тексту
    public class Paragraph : IComponent
    {
        private string _text;

        public Paragraph(string text)
        {
            _text = text;
        }

        public string GenerateHtml()
        {
            return $"<p>{_text}</p>";
        }
    }

    // Leaf: Таблиця
    public class Table : IComponent
    {
        private string _data;

        public Table(string data)
        {
            _data = data;
        }

        public string GenerateHtml()
        {
            return $"<table border=\"1\">\n  <tr><td>{_data}</td></tr>\n</table>";
        }
    }

    // Composite: Секція (містить інші компоненти)
    public class Section : IComponent
    {
        private string _title;
        private List<IComponent> _children = new List<IComponent>();

        public Section(string title)
        {
            _title = title;
        }

        public void Add(IComponent component)
        {
            _children.Add(component);
        }

        public void Remove(IComponent component)
        {
            _children.Remove(component);
        }

        public string GenerateHtml()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<section>");
            sb.AppendLine($"  <h2>{_title}</h2>");
            
            foreach (var child in _children)
            {
                // Додаємо відступ для наочності HTML
                sb.AppendLine($"  {child.GenerateHtml()}");
            }
            
            sb.Append("</section>");
            return sb.ToString();
        }
    }

    // ==========================================
    // 3. ПАТЕРН DECORATOR
    // ==========================================

    // Абстрактний декоратор
    public abstract class ReportDecorator : IComponent
    {
        protected IComponent _component;

        public ReportDecorator(IComponent component)
        {
            _component = component;
        }

        public virtual string GenerateHtml()
        {
            return _component.GenerateHtml();
        }
    }

    // Конкретний декоратор: Додає заголовок документа
    public class HeaderDecorator : ReportDecorator
    {
        private string _headerText;

        public HeaderDecorator(IComponent component, string headerText) : base(component)
        {
            _headerText = headerText;
        }

        public override string GenerateHtml()
        {
            return $"<header>\n  <h1>{_headerText}</h1>\n</header>\n" + base.GenerateHtml();
        }
    }

    // Конкретний декоратор: Додає нижній колонтитул
    public class FooterDecorator : ReportDecorator
    {
        private string _footerText;

        public FooterDecorator(IComponent component, string footerText) : base(component)
        {
            _footerText = footerText;
        }

        public override string GenerateHtml()
        {
            return base.GenerateHtml() + $"\n<footer>\n  <small>{_footerText}</small>\n</footer>";
        }
    }

    // ==========================================
    // 4. ДЕМОНСТРАЦІЯ (Клієнтський код)
    // ==========================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Генератор HTML звітів (Варіант 20) ===\n");

            // 1. Створюємо окремі елементи (Leaf)
            var p1 = new Paragraph("Це вступний абзац нашого фінансового звіту.");
            var p2 = new Paragraph("Детальна інформація наведена у таблиці нижче.");
            var table1 = new Table("Прибуток: $10,000 | Витрати: $4,000");

            // 2. Створюємо композитну ієрархію (Section містить Paragraph та Table)
            var mainSection = new Section("Фінансові результати Q3");
            mainSection.Add(p1);
            mainSection.Add(p2);
            mainSection.Add(table1);

            // Створюємо ще одну секцію та додаємо її до головної (Composite в Composite)
            var conclusionSection = new Section("Висновки");
            conclusionSection.Add(new Paragraph("Квартал завершено успішно."));
            mainSection.Add(conclusionSection);

            // 3. Застосовуємо декоратори
            // Декоруємо таблицю (Leaf) нижнім колонтитулом
            IComponent decoratedTable = new FooterDecorator(table1, "Дані актуальні на 01.10.2023");
            
            // Декоруємо весь звіт (Composite) заголовком і підвалом
            IComponent fullyDecoratedReport = new HeaderDecorator(
                new FooterDecorator(mainSection, "(c) 2023 Компанія ТОВ 'Рога і Копита'"), 
                "Офіційний Звіт Компанії"
            );

            // 4. Вивід результатів
            Console.WriteLine("--- 1. Недекорована базова секція ---");
            Console.WriteLine(mainSection.GenerateHtml() + "\n");

            Console.WriteLine("--- 2. Окремий декорований елемент (Таблиця з Footer) ---");
            Console.WriteLine(decoratedTable.GenerateHtml() + "\n");

            Console.WriteLine("--- 3. Повністю декорований ієрархічний звіт ---");
            Console.WriteLine(fullyDecoratedReport.GenerateHtml());
        }
    }
}