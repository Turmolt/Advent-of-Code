using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Solutions
{
    public class Day7
    {
        private List<int> crabs;
        
        public void Solve(string[] data)
        {
            var crabStrings = data[0].Split(',');
            crabs = new List<int>();
            for (int i = 0; i < crabStrings.Length; i++)
            {
                crabs.Add(int.Parse(crabStrings[i]));
            }

            var bestMovement = FindBestPosition();

            Console.WriteLine($"Position: {bestMovement.position}\nCost: {bestMovement.cost}");
        }

        (int position, int cost) FindBestPosition()
        {
            var position = 0;
            var lowestCost = int.MaxValue;
            var maxPosition = crabs.Max();
            var lowestPosition = crabs.Min();
            //for (int i = lowestPosition; i <= maxPosition; i++)
            {
                var i = GetMean(crabs.ToArray());
                var cost = 0;
                foreach (var crab in crabs)
                {
                    var distance = Math.Abs(crab - i);
                    var crabCost = (distance + distance * distance) / 2;
                    cost += crabCost;
                }

                if (lowestCost > cost)
                {
                    position = i;
                    lowestCost = cost;
                }
            }

            return (position, lowestCost);
        }

        int GetMean(int[] numbers)
        {
            var sum = 0;
            foreach (var n in numbers)
            {
                sum += n;
            }

            return sum / numbers.Length;
        }
        
        int GetMedian(int[] sourceNumbers) {
            //Framework 2.0 version of this method. there is an easier way in F4        
            if (sourceNumbers == null || sourceNumbers.Length == 0)
                throw new System.Exception("Median of empty array not defined.");

            //make sure the list is sorted, but use a new array
            int[] sortedPNumbers = (int[])sourceNumbers.Clone();
            Array.Sort(sortedPNumbers);

            //get the median
            int size = sortedPNumbers.Length;
            int mid = size / 2;
            int median = (size % 2 != 0) ? (int)sortedPNumbers[mid] : ((int)sortedPNumbers[mid] + (int)sortedPNumbers[mid - 1]) / 2;
            return median;
        }
    }
}