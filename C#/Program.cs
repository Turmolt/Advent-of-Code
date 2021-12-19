using System;
using System.Threading;
using System.Collections.Generic;
using Advent_of_Code.Solutions;

namespace Advent_of_Code
{
    class Program
    {
        static void Main(string[] args)
        {
            var data = System.IO.File.ReadAllLines("data.txt");
            var watch = new System.Diagnostics.Stopwatch();
            
            IChallenge d = new Day18();
            
            watch.Start();
            d.Solve(data);
            watch.Stop();
            
            Console.WriteLine($"Elapsed milliseconds: {watch.ElapsedMilliseconds}");

            Console.ReadKey();
        }

    }
}
