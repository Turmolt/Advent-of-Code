using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Advent_of_Code.Solutions
{
    public class Day8
    {
        private Dictionary<string, int> panelMeaning = new Dictionary<string, int>()
        {
            { "abcefg",0},
            {"cf",1},
            {"acdeg",2},
            {"acdfg",3},
            {"bcdf",4},
            {"abdfg",5},
            {"abdefg",6},
            {"acf",7},
            {"abcdefg",8},
            {"abcdfg",9},
        };
        public void Solve(string[] data)
        {
            var sum = 0;
            foreach (var line in data)
            {
                var tokens = line.Split(" | ");
                var decodeData = tokens[0].Split(' ');
                var outputData = tokens[1].Split(' ');
                var translationKey = FindTranslation(decodeData);
                var number = "";
                foreach (var entry in outputData)
                {
                    var charArray = entry.ToCharArray();
                    var translated = "";
                    foreach (var c in charArray)
                    {
                        translated += translationKey[c];
                    }

                    translated = new string(translated.ToCharArray().OrderBy(x => (int)x).ToArray());
                    number += panelMeaning[translated];
                }
                
                int output = int.Parse(number);
                sum += output;
                Console.WriteLine($"{line} => {output}");

            }

            Console.WriteLine($"The sum of all outputs is {sum}");
        }
        
        Dictionary<char, char> FindTranslation(string[] codes)
        {
            var one = codes.Where(x => x.Length == 2).ToArray()[0];
            var seven = codes.Where(x => x.Length == 3).ToArray()[0];
            // in 7 but not 1 -> a
            var a = seven.ToCharArray().Except(one.ToCharArray()).ToArray()[0];
            // 5 letters and share 2 with 1 -> 3
            var three = codes.Where(x => x.Length == 5).Where(w => one.All(w.Contains)).ToArray()[0];
            var four = codes.Where(x => x.Length == 4).ToArray()[0];
            // 5 letters and share 3 with 4 and not 3 -> 5
            var five = codes.Where(x => x != three && x.Length == 5 && x.Intersect(four).Count() == 3).ToArray()[0];
            var two = codes.Where(x => x.Length == 5 && x != three && x != five).ToArray()[0];
            // in 2 but not 3 -> e
            var e = two.ToCharArray().Except(three.ToCharArray()).ToArray()[0];
            // in 5 and 1 -> f
            var f = three.ToCharArray().Except(two.ToCharArray()).ToArray()[0];
            // in 2 and 1 -> c
            var c = two.ToCharArray().Where(x => one.Contains(x)).ToArray()[0];
            // in 2 and 4 and not c -> d
            var d = two.ToCharArray().Where(x=>four.Contains(x) && x != c).ToArray()[0];
            // in 4 and not c f or d -> b
            var b = four.ToCharArray().Where(x=>x!=c&&x!=f&&x!=d).ToArray()[0];
            // take final unassigned -> g
            var g = "abcdefg".ToCharArray().Where(x=>x!=a&&x!=b&&x!=c&&x!=d&&x!=e&&x!=f).ToArray()[0];
            var translationKey = new Dictionary<char, char>();
            translationKey.Add(a, 'a');
            translationKey.Add(b, 'b');
            translationKey.Add(c, 'c');
            translationKey.Add(d, 'd');
            translationKey.Add(e, 'e');
            translationKey.Add(f, 'f');
            translationKey.Add(g, 'g');
            return translationKey;
        }
    }
}