using System;
using System.Collections.Generic;

namespace lab24
{
    // ================= Strategy =================

    interface INumericOperationStrategy
    {
        double Execute(double value);
    }

    class SquareOperationStrategy : INumericOperationStrategy
    {
        public double Execute(double value)
        {
            return value * value;
        }
    }

    class CubeOperationStrategy : INumericOperationStrategy
    {
        public double Execute(double value)
        {
            return value * value * value;
        }
    }

    class SquareRootOperationStrategy : INumericOperationStrategy
    {
        public double Execute(double value)
        {
            return Math.Sqrt(value);
        }
    }

    class NumericProcessor
    {
        private INumericOperationStrategy strategy;

        public NumericProcessor(INumericOperationStrategy strategy)
        {
            this.strategy = strategy;
        }

        public void SetStrategy(INumericOperationStrategy strategy)
        {
            this.strategy = strategy;
        }

        public double Process(double value)
        {
            return strategy.Execute(value);
        }
    }

    // ================= Observer =================

    class ResultPublisher
    {
        public event Action<double, string> ResultCalculated;

        public void Publish(double result, string operationName)
        {
            if (ResultCalculated != null)
            {
                ResultCalculated(result, operationName);
            }
        }
    }

    class ConsoleLoggerObserver
    {
        public void Subscribe(ResultPublisher publisher)
        {
            publisher.ResultCalculated += OnResult;
        }

        private void OnResult(double result, string operationName)
        {
            Console.WriteLine("Operation: " + operationName + " Result: " + result);
        }
    }

    class HistoryLoggerObserver
    {
        private List<string> history = new List<string>();

        public void Subscribe(ResultPublisher publisher)
        {
            publisher.ResultCalculated += OnResult;
        }

        private void OnResult(double result, string operationName)
        {
            history.Add(operationName + " = " + result);
        }

        public void ShowHistory()
        {
            Console.WriteLine("\nHistory:");
            foreach (var item in history)
            {
                Console.WriteLine(item);
            }
        }
    }

    class ThresholdNotifierObserver
    {
        private double threshold;

        public ThresholdNotifierObserver(double threshold)
        {
            this.threshold = threshold;
        }

        public void Subscribe(ResultPublisher publisher)
        {
            publisher.ResultCalculated += OnResult;
        }

        private void OnResult(double result, string operationName)
        {
            if (result > threshold)
            {
                Console.WriteLine("Result is greater than threshold!");
            }
        }
    }

    // ================= Main =================

    class Program
    {
        static void Main(string[] args)
        {
            NumericProcessor processor = new NumericProcessor(new SquareOperationStrategy());
            ResultPublisher publisher = new ResultPublisher();

            ConsoleLoggerObserver consoleObserver = new ConsoleLoggerObserver();
            HistoryLoggerObserver historyObserver = new HistoryLoggerObserver();
            ThresholdNotifierObserver thresholdObserver = new ThresholdNotifierObserver(50);

            consoleObserver.Subscribe(publisher);
            historyObserver.Subscribe(publisher);
            thresholdObserver.Subscribe(publisher);

            double[] numbers = { 4, 5, 10 };

            Console.WriteLine("Square:");
            foreach (double n in numbers)
            {
                double result = processor.Process(n);
                publisher.Publish(result, "Square");
            }

            processor.SetStrategy(new CubeOperationStrategy());

            Console.WriteLine("\nCube:");
            foreach (double n in numbers)
            {
                double result = processor.Process(n);
                publisher.Publish(result, "Cube");
            }

            processor.SetStrategy(new SquareRootOperationStrategy());

            Console.WriteLine("\nSquare Root:");
            foreach (double n in numbers)
            {
                double result = processor.Process(n);
                publisher.Publish(result, "Square Root");
            }

            historyObserver.ShowHistory();

            Console.ReadKey();
        }
    }
}