namespace lab30vN
{
    public class TaxCalculator
    {
        private const double TaxRate = 0.18; // 18% податок

        public double CalculateTax(double income)
        {
            if (income < 0)
                throw new ArgumentException("Income cannot be negative");

            return income * TaxRate;
        }

        public double NetIncome(double income)
        {
            if (income < 0)
                throw new ArgumentException("Income cannot be negative");

            return income - CalculateTax(income);
        }
    }
}