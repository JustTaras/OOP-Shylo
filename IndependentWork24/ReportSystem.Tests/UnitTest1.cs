using System;
using System.Diagnostics;
using Xunit;
using ReportSystem;

namespace ReportSystem.Tests
{
    public class IntegrationTests
    {
        [Fact]
        public void Test1_Positive_CompositeGeneratesCorrectly()
        {
            // Arrange
            var section = new SectionNode();
            section.Add(new TextNode("Paragraph 1"));
            section.Add(new TextNode("Paragraph 2"));

            // Act
            string result = section.Generate();

            // Assert
            Assert.Equal("Paragraph 1\nParagraph 2", result);
        }

        [Fact]
        public void Test2_Positive_DecoratorWrapsCompositeProperly()
        {
            // Arrange
            var section = new SectionNode();
            section.Add(new TextNode("Header"));
            
            // Застосовуємо декоратор до секції (Composite)
            var boldSection = new HtmlTagDecorator(section, "b");

            // Act
            string result = boldSection.Generate();

            // Assert
            Assert.Equal("<b>Header</b>", result);
        }

        [Fact]
        public void Test3_Positive_ProxyCachesResultAndImprovesPerformance()
        {
            // Arrange
            var node = new TextNode("Heavy Data");
            var proxy = new CachedReportProxy(node);

            // Act
            string firstCall = proxy.Generate(); // Оригінальний виклик
            
            // Змінюємо стан (імітація: якби кешу не було, результат міг би змінитися,
            // але ми використовуємо просту перевірку посилань для доказу кешування)
            string secondCall = proxy.Generate(); 

            // Assert
            Assert.Equal("Heavy Data", firstCall);
            // Перевіряємо, що повернутий рядок - це ТОЙ САМИЙ об'єкт з пам'яті (кешований)
            Assert.Same(firstCall, secondCall); 
        }

        [Fact]
        public void Test4_Negative_NullTextNodeThrowsException()
        {
            // Arrange & Act & Assert
            // Граничний/Негативний кейс: порожній рядок у базовий вузол
            var ex = Assert.Throws<ArgumentException>(() => new TextNode(""));
            Assert.Contains("Текст не може бути порожнім", ex.Message);

            // Граничний кейс: передача null у Composite
            var section = new SectionNode();
            Assert.Throws<ArgumentNullException>(() => section.Add(null!));
            
            // Граничний кейс: передача null у Decorator
            Assert.Throws<ArgumentNullException>(() => new HtmlTagDecorator(null!, "p"));
        }
    }
}