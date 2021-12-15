using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Solutions
{
    public class Day14 : IChallenge
    {
        private Dictionary<string, string> insertionTemplate = new Dictionary<string, string>();
        private Dictionary<string, ulong> pairCounter = new Dictionary<string, ulong>();
        private Dictionary<char, ulong> charCounter = new Dictionary<char, ulong>();

        public void Solve(string[] data)
        {
            ParseData(data);
            
            var steps = 40;
            while (steps-- > 0)
            {
                var currentPairs = new Dictionary<string, ulong>(pairCounter);
                pairCounter.Clear();
                foreach (var pair in currentPairs)
                {
                    SplitPair(pair.Key, pair.Value);
                }
            }

            var max = ulong.MinValue;
            var min = ulong.MaxValue;
            foreach (var entry in charCounter)
            {
                var val = entry.Value;
                if (val > max) max = val;
                if (val < min) min = val;
                Console.WriteLine($"{entry.Key} appeared {val} times");
            }

            Console.WriteLine($"The answer is {max - min}");
        }

        void SplitPair(string key, ulong amount)
        {
            if(!insertionTemplate.ContainsKey(key)) return;

            var insertion = insertionTemplate[key];
            var firstNew = key[0] + insertion;
            var secondNew = insertion + key[1];
            IncrementChar(insertion[0], amount);
            IncrementPair(firstNew, amount);
            IncrementPair(secondNew, amount);
        }

        void IncrementChar(char c, ulong amount)
        {
            if (charCounter.ContainsKey(c)) charCounter[c] += amount;
            else charCounter[c] = amount;
        }

        void IncrementPair(string pair, ulong amount)
        {
            if (pairCounter.ContainsKey(pair)) pairCounter[pair]+=amount;
            else pairCounter[pair] = amount;
        }

        void ParseData(string[] data)
        {
            var polymer = data[0];
            foreach (var c in polymer)
            {
                IncrementChar(c, 1);
            }
            
            for (int i = 2; i < data.Length; i++)
            {
                var line = data[i];
                var tokens = line.Split(" -> ");
                insertionTemplate[tokens[0]] = tokens[1];
            }

            for (int i = 0; i < polymer.Length - 1; i++)
            {
                var pair = $"{polymer[i]}{polymer[i + 1]}";
                IncrementPair(pair, 1);
            }
        }
    }
}