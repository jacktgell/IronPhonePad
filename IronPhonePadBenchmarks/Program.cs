using System;
using System.Diagnostics; // Required for Stopwatch
using System.Text;        // Required for StringBuilder
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using PhonePadLibrary;

namespace IronPhonePadBenchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            // OPTION 1: Run the O(N) Proof Experiment (Generates CSV data)
            // Command: dotnet run --project IronPhonePadBenchmarks > results.csv
            ComplexityProof.RunProof();
        
            // OPTION 2: Run the Standard Benchmarks (Generates Report)
            // Command: dotnet run -c Release --project IronPhonePadBenchmarks
            // BenchmarkRunner.Run<PhonePadBenchmarks>();
        }
    }

    // ---------------------------------------------------------
    // MODULE 1: Algorithmic Complexity Verification
    // ---------------------------------------------------------
    public static class ComplexityProof
    {
        public static void RunProof()
        {
            Console.WriteLine("N,AverageTime(Ticks)"); // CSV Header

            // 1. Warmup Phase
            // Critical to allow the JIT compiler to optimize the method execution path before measuring.
            var warmupString = new string('2', 1000) + "#";
            PhoneKeypad.OldPhonePad(warmupString);

            // 2. The Experiment: Linearity Test (N=10 to N=10,000)
            for (int n = 10; n <= 10000; n += 10)
            {
                // Construct input once for this batch size
                string input = new string('2', n - 1) + "#";
                
                long totalTicks = 0;
                int iterations = 10;

                // 3. Batch Execution
                for (int i = 0; i < iterations; i++)
                {
                    // Force Garbage Collection to minimize memory noise during timing
                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    var stopwatch = Stopwatch.StartNew();
                    
                    PhoneKeypad.OldPhonePad(input);
                    
                    stopwatch.Stop();
                    totalTicks += stopwatch.ElapsedTicks;
                }

                // 4. Averaging
                double averageTicks = totalTicks / (double)iterations;

                // Output: Input Length vs. Average Ticks
                Console.WriteLine($"{n},{averageTicks}");
            }
        }
    }

    // ---------------------------------------------------------
    // MODULE 2: Standard Performance Benchmarks (BenchmarkDotNet)
    // ---------------------------------------------------------
    [MemoryDiagnoser] // Configures the runner to capture memory allocation stats (Gen 0/1/2, Heap Alloc).
    public class PhonePadBenchmarks
    {
        // 1. Short standard input
        private const string ShortInput = "4433555 555666#";

        // 2. Complex Turing input
        private const string ComplexInput = "8 88777444666*664#";

        // 3. Massive input (Stress Test)
        // Initialized to default to satisfy non-nullable field requirements.
        private string LongInput = string.Empty; 

        [GlobalSetup]
        public void Setup()
        {
            // Construct a large dataset to stress-test the O(N) performance.
            // Repeats the "TURING" sequence 100 times.
            var builder = new StringBuilder();
            for (int i = 0; i < 100; i++)
            {
                builder.Append("8 88777444666*664#");
            }
            LongInput = builder.ToString();
        }

        [Benchmark]
        public string Decode_Short() => PhoneKeypad.OldPhonePad(ShortInput);

        [Benchmark]
        public string Decode_Complex() => PhoneKeypad.OldPhonePad(ComplexInput);

        [Benchmark]
        public string Decode_StressTest() => PhoneKeypad.OldPhonePad(LongInput);
    }
}