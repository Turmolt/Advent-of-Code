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
        var useTestData = true;
        
        var data = System.IO.File.ReadAllLines(useTestData ? "testdata.txt" : "data.txt");
        var watch = new System.Diagnostics.Stopwatch();
            
        IChallenge d = new Day22();
  
        watch.Start();
        d.Solve(data);
        watch.Stop();
        
        Log($"Elapsed milliseconds: {watch.ElapsedMilliseconds}", force:true);
        
        Console.ReadKey();
    }

}