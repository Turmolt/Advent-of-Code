using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Solutions
{
    public class Day15 : IChallenge
    {
        private (int risk, int id)[][] riskMap;
        private (int risk, int id, (int x, int y) coordinates)[] allNodes;
        private int[,] graph;
        private int yLength => riskMap.Length;
        private int xLength => riskMap[0].Length;
        private int tiles = 5;
        public void Solve(string[] data)
        {
            ParseData(data);
            Dijkstras(0);
        }

        int GetTiledRisk(int x, int y)
        {
            var nx = x % xLength;
            var ny = y % yLength;

            var xa = x / xLength;
            var ya = y / yLength;
            
            var risk = riskMap[ny][nx].risk + xa + ya;
            if (risk > 9) risk %= 9;
            
            return risk;
        }
        
        string[] TileMap(string[] data)
        {
            var xLength = data[0].Length;
            var yLength = data.Length;
            var tiled = new string[yLength * 5];
            for (int y = 0; y < yLength*5; y++)
            {
                var yAddition = y / yLength;
                var yIndex = (y % yLength);
                for (int x = 0; x < xLength * 5; x++)
                {
                    var xAddition = x / xLength;
                    var xIndex = (x % xLength);
                    var targetNum = int.Parse($"{data[yIndex][xIndex]}");
                    var num = (targetNum + xAddition + yAddition);
                    if (num >= 10) num %= 9;
                    tiled[y] += $"{num}";
                }
            }

            return tiled;
        }

        int FindMinimum(int[] dist, bool[] sptSet)
        {
            int min = int.MaxValue, min_index = -1;
            for (int v = 0; v < allNodes.Length * tiles * tiles; v++)
            {
                if (sptSet[v]) continue;
                if (dist[v] >= min) continue;

                min = dist[v];
                min_index = v;
            }

            return min_index;
        }

        int GetGraphValue(int u, int v)
        {
            int maxX = tiles * xLength;
            int x = (u % maxX);
            int y = (u / maxX);
            
            int x2 = (v % maxX);
            int y2 = (v / maxX);

            if (!IsValidMoveTiled(x2, y2)) return 0;
            
            var deltaTiles = Math.Abs(x2 - x) + Math.Abs(y2 - y);
            if (deltaTiles > 1) return 0;
            
            return GetTiledRisk(x2, y2);
        }

        int[] GetNeighbors(int id)
        {
            List<int> available = new List<int>();
            int maxX = 5 * xLength;
            int maxY = 5 * yLength;
            int xVal = (id % maxX);
            int yVal = (id / maxX);
            int lookupX = xVal % xLength;
            int lookupY = yVal % yLength;
            int additionalRisk = (xVal / xLength) + (yVal / yLength);
            
            for (int dy = -1; dy <= 1; dy++)
            {
                var ny = yVal + dy;
                if(ny < 0 || ny >= maxY) continue;
                
                for (int dx = -1; dx <= 1; dx++)
                {
                    var nx = xVal + dx;
                    if(nx < 0 || nx >= maxX) continue;

                    var targetId = (ny * xLength) + nx;
                    available.Add(targetId);
                }
            }

            return available.ToArray();
        }

        void Dijkstras(int src)
        {
            int[] dist = new int[allNodes.Length * tiles * tiles];
            bool[] sptSet = new bool[allNodes.Length * tiles * tiles];

            for (int i = 0; i < allNodes.Length * tiles * tiles; i++)
            {
                dist[i] = int.MaxValue;
                sptSet[i] = false;
            }

            dist[src] = 0;

            for (int count = 0; count < allNodes.Length * tiles * tiles - 1; count++)
            {
                int u = FindMinimum(dist, sptSet);

                sptSet[u] = true;

                if (count % 1000 == 0) Console.WriteLine(count);
                
                for (int v = 0; v < allNodes.Length * tiles * tiles; v++)
                {
                    if (!sptSet[v] && GetGraphValue(u, v) != 0 &&
                        dist[u] != int.MaxValue && dist[u] + GetGraphValue(u, v) < dist[v])
                    {
                        dist[v] = dist[u] + GetGraphValue(u, v);
                    }

                }
            }
            
            Console.WriteLine($"Total Cost: {dist[allNodes.Length * tiles * tiles-1]}");
        }
        
        void ParseData(string[] data)
        {
            allNodes = new (int risk, int id, (int x, int y) coordinates)[data.Length * data[0].Length];
            riskMap = new (int risk, int id)[data.Length][];
            graph = new int[allNodes.Length,allNodes.Length];
            
            for (int y = 0; y < data.Length; y++)
            {
                var line = data[y];
                riskMap[y] = new (int risk, int id)[line.Length];
                for (int x = 0; x < line.Length; x++)
                {
                    var id = (y * line.Length) + x;
                    var risk = int.Parse($"{line[x]}");
                    riskMap[y][x] = (risk, id);

                    allNodes[id] = (risk, id, (x, y));
                }
            }

            for (int y = 0; y < data.Length; y++)
            {
                for (int x = 0; x < riskMap[y].Length; x++)
                {
                    AddConnections(x, y);
                }
            }
        }

        void AddConnections(int x, int y)
        {
            var target = riskMap[y][x];
            
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    var ty = y + dy;
                    var tx = x + dx;
                    if (!IsValidMove(tx, ty, dx, dy)) continue;
                    graph[target.id, riskMap[ty][tx].id] = riskMap[ty][tx].risk;
                }
            }
        }

        bool IsValidMoveTiled(int x, int y)
        { 
            if(y < 0 || y >= xLength * tiles) return false;
            if(x < 0 || x >= yLength * tiles) return false;
            return true;
        }
        
        bool IsValidMove(int x, int y, int dx, int dy)
        {
            if(Math.Abs(dy) == Math.Abs(dx)) return false; //no diagonals
            if(y < 0 || y >= riskMap.Length) return false;
            if(x < 0 || x >= riskMap[y].Length) return false;
            return true;
        }

    }
}