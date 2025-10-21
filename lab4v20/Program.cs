using System;
using System.Collections.Generic;
using System.Linq;

// Інтерфейс сигналу — контракт, який визначає базові властивості та методи
public interface ISignal
{
    double[] Generate(int samples);     // Згенерувати сигнал
    double GetAverage(double[] signal); // Обчислити середнє значення
    double GetAmplitude(double[] signal); // Обчислити амплітуду
    double GetEnergy(double[] signal);  // Обчислити енергію
}

// Абстрактний клас, який реалізує спільну логіку для всіх типів сигналів
public abstract class SignalBase : ISignal
{
    protected double frequency;
    protected double amplitude;

    public SignalBase(double frequency, double amplitude)
    {
        this.frequency = frequency;
        this.amplitude = amplitude;
    }

    public abstract double[] Generate(int samples);

    // Середнє значення
    public double GetAverage(double[] signal) => signal.Average();

    // Амплітуда — максимальне значення
    public double GetAmplitude(double[] signal) => signal.Max();

    // Енергія сигналу — сума квадратів
    public double GetEnergy(double[] signal)
        => signal.Select(x => x * x).Sum();
}

// Реалізація 1: Синусоїдальний сигнал
public class SineSignal : SignalBase
{
    public SineSignal(double frequency, double amplitude)
        : base(frequency, amplitude) { }

    public override double[] Generate(int samples)
    {
        double[] signal = new double[samples];
        for (int i = 0; i < samples; i++)
            signal[i] = amplitude * Math.Sin(2 * Math.PI * frequency * i / samples);
        return signal;
    }
}

// Реалізація 2: Прямокутний сигнал
public class SquareSignal : SignalBase
{
    public SquareSignal(double frequency, double amplitude)
        : base(frequency, amplitude) { }

    public override double[] Generate(int samples)
    {
        double[] signal = new double[samples];
        for (int i = 0; i < samples; i++)
        {
            double value = Math.Sin(2 * Math.PI * frequency * i / samples);
            signal[i] = value >= 0 ? amplitude : -amplitude;
        }
        return signal;
    }
}

// Клас Analyzer використовує композицію — він має посилання на ISignal
public class SignalAnalyzer
{
    private readonly ISignal signal;

    public SignalAnalyzer(ISignal signal)
    {
        this.signal = signal;
    }

    public void Analyze(int samples)
    {
        var data = signal.Generate(samples);
        Console.WriteLine($"Середнє значення: {signal.GetAverage(data):F3}");
        Console.WriteLine($"Амплітуда: {signal.GetAmplitude(data):F3}");
        Console.WriteLine($"Енергія: {signal.GetEnergy(data):F3}");
        Console.WriteLine();
    }
}

class Program
{
    static void Main()
    {
        // Демонстрація роботи
        ISignal sine = new SineSignal(frequency: 2, amplitude: 1);
        ISignal square = new SquareSignal(frequency: 2, amplitude: 1);

        // Композиція: Analyzer "володіє" сигналом
        var sineAnalyzer = new SignalAnalyzer(sine);
        var squareAnalyzer = new SignalAnalyzer(square);

        Console.WriteLine("=== Синусоїдальний сигнал ===");
        sineAnalyzer.Analyze(100);

        Console.WriteLine("=== Прямокутний сигнал ===");
        squareAnalyzer.Analyze(100);
    }
}