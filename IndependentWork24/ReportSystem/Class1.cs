using System;
using System.Collections.Generic;
using System.Linq;

namespace ReportSystem
{
    // ==========================================
    // Спільний інтерфейс компонентів
    // ==========================================
    public interface IReportComponent
    {
        string Generate();
    }

    // ==========================================
    // 1. ПАТЕРН COMPOSITE
    // ==========================================
    
    // Leaf: Базовий текстовий вузол
    public class TextNode : IReportComponent
    {
        private readonly string _text;

        public TextNode(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Текст не може бути порожнім", nameof(text));
                
            _text = text;
        }

        public virtual string Generate()
        {
            // Імітація важкої операції (наприклад, форматування або запит до БД)
            return _text;
        }
    }

    // Composite: Секція, що містить інші вузли
    public class SectionNode : IReportComponent
    {
        private readonly List<IReportComponent> _children = new();

        public void Add(IReportComponent component)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));
            _children.Add(component);
        }

        public string Generate()
        {
            var results = _children.Select(c => c.Generate());
            return string.Join("\n", results);
        }
    }

    // ==========================================
    // 2. ПАТЕРН DECORATOR
    // ==========================================
    
    // Додає HTML-теги до будь-якого компонента
    public class HtmlTagDecorator : IReportComponent
    {
        private readonly IReportComponent _component;
        private readonly string _tag;

        public HtmlTagDecorator(IReportComponent component, string tag)
        {
            _component = component ?? throw new ArgumentNullException(nameof(component));
            _tag = tag ?? throw new ArgumentNullException(nameof(tag));
        }

        public string Generate()
        {
            return $"<{_tag}>{_component.Generate()}</{_tag}>";
        }
    }

    // ==========================================
    // 3. ПАТЕРН PROXY
    // ==========================================
    
    // Кешує результат генерації, щоб не обходити дерево повторно
    public class CachedReportProxy : IReportComponent
    {
        private readonly IReportComponent _realComponent;
        private string? _cachedResult;

        public CachedReportProxy(IReportComponent realComponent)
        {
            _realComponent = realComponent ?? throw new ArgumentNullException(nameof(realComponent));
        }

        public string Generate()
        {
            if (_cachedResult == null)
            {
                // Виклик реальної (важкої) генерації лише один раз
                _cachedResult = _realComponent.Generate();
            }
            return _cachedResult;
        }

        // Метод для примусового очищення кешу (корисно, якщо структура змінилася)
        public void InvalidateCache()
        {
            _cachedResult = null;
        }
    }
}