using System;
using System.Collections.Generic;

class Program
{
    delegate bool CheckNumber(int number);

    static void Main()
    {
        List<int> numbers = new List<int> { 1, 4, 7, 10, 13, 16, 19, 22, 25, 28 };

        CheckNumber isEvenDelegate = (number) => number % 2 ==0 ;

        foreach (int num in numbers)
        {
            bool result = isEvenDelegate(num);

            if (result)
                Console.WriteLine($"{num} - парне число");
            else
                Console.WriteLine($"{num} - непарне число");
        }
    }

    static bool IsEven(int number)
    {
        return number % 2 == 0;
    }
}