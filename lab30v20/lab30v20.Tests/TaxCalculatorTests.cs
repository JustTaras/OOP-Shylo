using Xunit;
using lab30vN;

namespace lab30vN.Tests
{
    public class TaxCalculatorTests
    {
        private readonly TaxCalculator calculator;

        public TaxCalculatorTests()
        {
            calculator = new TaxCalculator();
        }

        // Fact tests

        [Fact]
        public void CalculateTax_For1000_Returns180()
        {
            double result = calculator.CalculateTax(1000);

            Assert.Equal(180, result);
        }

        [Fact]
        public void NetIncome_For1000_Returns820()
        {
            double result = calculator.NetIncome(1000);

            Assert.Equal(820, result);
        }

        [Fact]
        public void CalculateTax_ForZero_ReturnsZero()
        {
            double result = calculator.CalculateTax(0);

            Assert.Equal(0, result);
        }

        [Fact]
        public void NetIncome_ForZero_ReturnsZero()
        {
            double result = calculator.NetIncome(0);

            Assert.Equal(0, result);
        }

        [Fact]
        public void CalculateTax_NegativeIncome_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => calculator.CalculateTax(-100));
        }

        [Fact]
        public void NetIncome_NegativeIncome_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => calculator.NetIncome(-100));
        }

        // Theory tests (параметризовані)

        [Theory]
        [InlineData(1000, 180)]
        [InlineData(2000, 360)]
        [InlineData(500, 90)]
        public void CalculateTax_VariousIncome_ReturnsCorrectTax(double income, double expected)
        {
            double result = calculator.CalculateTax(income);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(1000, 820)]
        [InlineData(2000, 1640)]
        [InlineData(500, 410)]
        public void NetIncome_VariousIncome_ReturnsCorrectNet(double income, double expected)
        {
            double result = calculator.NetIncome(income);

            Assert.Equal(expected, result);
        }
    }
}