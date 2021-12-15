using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Solutions
{
    public class Day9
    {
        private int[][] map;
        private List<(int x, int y)> lowPoints;
        private List<int> basinSizes;
        public void Solve(string[] data)
        {
            lowPoints = new List<(int x, int y)>();
            basinSizes = new List<int>();
            
            map = new int[data.Length][];
            for (int y = 0; y < data.Length; y++)
            {
                map[y] = new int[data[y].Length];
                var charArray = data[y].ToCharArray();
                for (int x = 0; x < charArray.Length; x++)
                {
                    map[y][x] = int.Parse($"{charArray[x]}");
                }
            }

            var sum = 0;

            for (int y = 0; y < map.Length; y++)
            {
                for (int x = 0; x < map[y].Length; x++)
                {
                    if (lowerThanSurroundings(x, y))
                    {
                        sum += 1 + map[y][x];
                    }
                }
            }
            
            Console.WriteLine($"The answer to part one is {sum}");
            
            Console.WriteLine($"{lowPoints.Count} low points found");
            
            
            foreach (var low in lowPoints)
            {
                var pointMap = new List<(int, int)>();
                MapPoint(low, ref pointMap);
                basinSizes.Add(pointMap.Count);
            }
            
            Console.WriteLine($"I have found {basinSizes.Count} basins");

            basinSizes.Sort();
            basinSizes.Reverse();

            
            Console.WriteLine($"The answer to part two is {basinSizes[0]*basinSizes[1]*basinSizes[2]}");
        }

        private int depth;
        public void MapPoint((int x, int y) coord, ref List<(int x, int y)> targetMap)
        {
            var newlyFound = new List<(int x, int y)>();
            var targetPoint = map[coord.y][coord.x];
            if (targetPoint < 9 && !targetMap.Contains(coord))
            {
                targetMap.Add(coord);
            }

            for (var dy = -1; dy <= 1; dy++)
            {
                for (var dx = -1; dx <= 1; dx++)
                {
                    if(Math.Abs(dx)==Math.Abs(dy)) continue;
                    var ny = coord.y + dy;
                    var nx = coord.x + dx;
                    if(ny < 0 || ny >= map.Length) continue;
                    if(nx < 0 || nx >= map[ny].Length) continue;
                    var foundPoint = map[ny][nx];
                    if (foundPoint < 9 && !targetMap.Contains((nx, ny)))
                    {
                        targetMap.Add((nx, ny));
                        if (!newlyFound.Contains((nx, ny)))
                        {
                            newlyFound.Add((nx, ny));
                        }
                    }
                }
            }

            foreach (var newPair in newlyFound)
            {
                MapPoint(newPair, ref targetMap);
            }
        }



        bool lowerThanSurroundings(int x, int y)
        {
            var me = map[y][x];
            for(int dy = -1; dy <= 1; dy++){
                for (int dx = -1; dx <= 1; dx++)
                {
                     if(dy == dx && dx == 0) continue;
                     var cy = y + dy;
                     var cx = x + dx;
                    if (cx < 0 || cy < 0 || cy >= map.Length || cx >= map[cy].Length)
                         continue;
                    
                     var target = map[y + dy][x + dx];
                     if (target < me) return false;
                }
            }

            lowPoints.Add((x, y));
            return true;
        }
    }
}