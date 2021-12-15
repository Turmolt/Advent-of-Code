using System;
using System.Collections.Generic;

namespace Advent_of_Code.Solutions
{
    public class Day6
    {
        private Dictionary<ulong, ulong> fishMap;
        
        public void Solve(string[] data, int days)
        {
            fishMap = new Dictionary<ulong, ulong>();
            for (ulong i = 0; i <= 8; i++) fishMap[i] = 0;
            var rawFish = data[0].Split(',');
            foreach (var fish in rawFish)
            {
                var fishNumber = ulong.Parse(fish);
                if (fishMap.ContainsKey(fishNumber))
                {
                    fishMap[fishNumber]++;
                }
                else
                {
                    fishMap[fishNumber] = 1;
                }
            }

            
            for (int i = 0; i < days; i++)
            {
                UpdateFish();
            }

            var count = TallyFish();

            Console.WriteLine($"After {days} days there will be {count} fish");
        }

        ulong TallyFish()
        {
            ulong count = 0;
            for (ulong i = 0; i <= 8; i++)
            {
                count += fishMap[i];
            }

            return count;
        }

        void UpdateFish()
        {
            var tomorrowsFish = new Dictionary<ulong, ulong>();
            for (ulong i = 0; i <= 8; i++)
            {
                var todaysFish = fishMap[i];
                switch (i)
                {
                    case 0:
                        AddToDictionary(ref tomorrowsFish, 6, todaysFish);
                        AddToDictionary(ref tomorrowsFish, 8, todaysFish);
                        break;
                    default:
                        AddToDictionary(ref tomorrowsFish, i - 1, todaysFish);
                        break;
                }
            }

            fishMap = tomorrowsFish;
        }

        void AddToDictionary(ref Dictionary<ulong, ulong> target, ulong day, ulong number)
        {
            if (target.ContainsKey(day)) target[day] = target[day] + number;
            else target[day] = number;
        }
    }
}