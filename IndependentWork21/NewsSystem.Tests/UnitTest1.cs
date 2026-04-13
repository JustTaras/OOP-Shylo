using System;
using Xunit;
using NewsSystemCore;

namespace NewsSystem.Tests
{
    public class IntegrationTests : IDisposable
    {
        // Скидаємо Singleton перед/після кожного тесту для чистоти
        public IntegrationTests() { NewsSystemManager.Reset(); }
        public void Dispose() { NewsSystemManager.Reset(); }

        // ====================================================
        // ПОЗИТИВНІ СЦЕНАРІЇ (3 мінімум)
        // ====================================================

        [Fact]
        public void Scenario1_Positive_FullWorkflowWithPublishStrategy()
        {
            // Arrange
            var manager = NewsSystemManager.Instance;
            var observer = new TestObserver();
            manager.Publisher.OnNewsProcessed += observer.ReceiveUpdate;
            string news = "AI takes over the world";

            // Act
            string result = manager.ExecuteWorkflow("publish", news);

            // Assert
            Assert.Equal("PUBLISHED: AI takes over the world", result); // Перевірка Strategy
            Assert.Single(observer.History); // Перевірка Observer
            Assert.Equal("PUBLISHED: AI takes over the world", observer.History[0]);
        }

        [Fact]
        public void Scenario2_Positive_SingletonStabilityAcrossCalls()
        {
            // Arrange
            var manager1 = NewsSystemManager.Instance;
            var manager2 = NewsSystemManager.Instance;
            var observer = new TestObserver();
            manager1.Publisher.OnNewsProcessed += observer.ReceiveUpdate;

            // Act
            manager1.ExecuteWorkflow("publish", "News 1");
            manager2.ExecuteWorkflow("archive", "News 2"); // Зміна стратегії в Runtime через Singleton

            // Assert
            Assert.Same(manager1, manager2); // Перевірка стабільності Singleton
            Assert.Equal(2, observer.History.Count); // Спостерігач отримав обидві події, бо це один і той самий екземпляр
            Assert.Equal("PUBLISHED: News 1", observer.History[0]);
            Assert.Equal("ARCHIVED: News 2", observer.History[1]);
        }

        [Fact]
        public void Scenario3_Positive_MultipleObserversReceiveUpdates()
        {
            // Arrange
            var manager = NewsSystemManager.Instance;
            var feedObserver = new TestObserver();
            var emailObserver = new TestObserver();
            
            manager.Publisher.OnNewsProcessed += feedObserver.ReceiveUpdate;
            manager.Publisher.OnNewsProcessed += emailObserver.ReceiveUpdate;

            // Act
            manager.ExecuteWorkflow("archive", "Old news");

            // Assert
            Assert.Single(feedObserver.History);
            Assert.Single(emailObserver.History);
            Assert.Equal("ARCHIVED: Old news", feedObserver.History[0]);
            Assert.Equal("ARCHIVED: Old news", emailObserver.History[0]);
        }

        // ====================================================
        // НЕГАТИВНІ ТА ГРАНИЧНІ СЦЕНАРІЇ (2 мінімум)
        // ====================================================

        [Fact]
        public void Scenario4_Negative_FactoryThrowsOnInvalidStrategy()
        {
            // Arrange
            var manager = NewsSystemManager.Instance;

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => 
                manager.ExecuteWorkflow("invalid_strategy", "Some news"));
            
            Assert.Contains("Unknown strategy type", ex.Message); // Перевірка безпеки Фабрики
        }

        [Fact]
        public void Scenario5_Boundary_NullOrEmptyNewsThrowsException()
        {
            // Arrange
            var manager = NewsSystemManager.Instance;
            var observer = new TestObserver();
            manager.Publisher.OnNewsProcessed += observer.ReceiveUpdate;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                manager.ExecuteWorkflow("publish", "")); // Порожній рядок

            Assert.Throws<ArgumentNullException>(() => 
                manager.ExecuteWorkflow("publish", null)); // Null

            // Спостерігачі не повинні отримати жодного сповіщення
            Assert.Empty(observer.History); 
        }
    }
}