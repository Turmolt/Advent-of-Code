using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Advent_of_Code.Solutions
{
    class Day3
    {
        public void Solve(string[] data)
        {
            var survivors = new List<string>();
            var idx = 0;
            foreach (var line in data) survivors.Add(line);

            while (survivors.Count > 1)
            {
                var mcb = TallyRemaining(survivors, 0);
                var seeking = int.Parse($"{mcb[idx]}");
                var targetSurvivors = survivors.ToArray();
                foreach (var line in targetSurvivors)
                {
                    var target = int.Parse($"{line[idx]}");
                    if (target != seeking)
                    {
                        survivors.Remove(line);
                        continue;
                    }
                    if (survivors.Count == 1) break;
                }

                idx++;

                if (idx >= mcb.Length)
                {
                    Console.WriteLine($"IDX Out of Range {idx}");
                    Console.WriteLine(survivors.Count);
                    break;
                }

                Thread.Sleep(1);
            }

            //first: 011000111111
            //second: 101011000100

            Console.WriteLine(survivors[0]);
        }

        string TallyRemaining(List<string> remaining, int seeking)
        {
            var idx = 0;
            var targetIdx = remaining[0].Length;
            var value = "";
            while (idx < targetIdx)
            {
                var tally = new[] { 0, 0 };
                foreach (var line in remaining)
                {
                    var target = int.Parse($"{line[idx]}");
                    tally[target]++;
                }
                if (tally[0] > tally[1])
                {
                    value += "0";
                }
                else if (tally[0] < tally[1])
                {
                    value += "1";
                }
                else
                {
                    value += $"{seeking}";
                }
                idx++;
            }
            return value;
        }
    }
}
