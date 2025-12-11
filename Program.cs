// Online C# Editor for free
// Write, Edit and Run your C# code using C# Online Compiler

using System;

namespace SingletonDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- Testing EAGER Singleton ---");

            // 1. Get the instance 
            // (Matching the class name defined below)
            var eager1 = NumberGeneratorEagerInstanciation.Instance;
            var eager2 = NumberGeneratorEagerInstanciation.Instance;

            // 2. Prove state is shared
            // (Fixed method casing to GetNextNumber)
            Console.WriteLine($"Eager Client 1 gets: {eager1.GetNextNumber()}");
            Console.WriteLine($"Eager Client 2 gets: {eager2.GetNextNumber()}");

            // 3. Prove they are literally the same object in memory
            bool isSameEager = ReferenceEquals(eager1, eager2);
            Console.WriteLine($"Are eager1 and eager2 the same object? {isSameEager}");


            Console.WriteLine("\n--- Testing LAZY Singleton ---");

            // 1. Get the instance
            var lazy1 = NumberGeneratorLazyInstanciation.Instance;
            var lazy2 = NumberGeneratorLazyInstanciation.Instance;

            // 2. Prove state is shared
            Console.WriteLine($"Lazy Client 1 gets: {lazy1.GetNextNumber()}");
            Console.WriteLine($"Lazy Client 2 gets: {lazy2.GetNextNumber()}");

            // 3. Prove they are the same object
            bool isSameLazy = ReferenceEquals(lazy1, lazy2);
            Console.WriteLine($"Are lazy1 and lazy2 the same object? {isSameLazy}");
        }
    }
    public class NumberGeneratorEagerInstanciation
    {
        private static readonly NumberGeneratorEagerInstanciation _instance = new();
        private int number = 1;

        private NumberGeneratorEagerInstanciation() { }
        public static NumberGeneratorEagerInstanciation Instance
        {
            get
            {
                return _instance;
            }
        }
        public int GetNextNumber()
        {
            return number++;
        }
    }
    public class NumberGeneratorLazyInstanciation
    {
        private static NumberGeneratorLazyInstanciation _instance;
        private static readonly object _lock = new object();
        private int number = 1;
        private NumberGeneratorLazyInstanciation() { }
        public static NumberGeneratorLazyInstanciation Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new();
                        }
                    }
                }
                _instance = new();
                return _instance;
            }
        }
        public int GetNextNumber()
        {
            return number++;
        }
    }
}
