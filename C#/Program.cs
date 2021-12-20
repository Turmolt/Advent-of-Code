using System;
using System.Threading;
using System.Collections.Generic;
using Advent_of_Code.Solutions;
using static Advent_of_Code.G8S;

namespace Advent_of_Code;

public static class Program
{
    static void Main(string[] args)
    {
        var data = System.IO.File.ReadAllLines("data.txt");
        var watch = new System.Diagnostics.Stopwatch();
            
        IChallenge d = new Day19();
  
        watch.Start();
        d.Solve(data);
        watch.Stop();
            
        Console.WriteLine($"Elapsed milliseconds: {watch.ElapsedMilliseconds}");

        Console.ReadKey();
    }

}