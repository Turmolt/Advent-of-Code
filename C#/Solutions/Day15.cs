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

        void Dijkstras(int src)
        {
            int[] dist = new int[allNodes.Length * tiles * tiles];
            bool[] visited = new bool[allNodes.Length * tiles * tiles];

            PriorityQueue<int, int> queue = new PriorityQueue<int, int>();
            
            for (int i = 0; i < allNodes.Length * tiles * tiles; i++)
            {
                dist[i] = int.MaxValue;
                visited[i] = false;
            }

            dist[src] = 0;
            queue.Enqueue(src, 0);

            while (queue.Count > 0)
            {
                int u = queue.Dequeue();

                var neighbors = GetNeighbors(u);

                if (u == (allNodes.Length * tiles * tiles - 1))
                {
                    Console.WriteLine($"Path found! Shortest cost is {dist[u]}");
                    return;
                }
                
                for (int i = 0; i < neighbors.Length; i++)
                {
                    var v = neighbors[i];
                    var edge = GetGraphValue(u, v);
                    if (!visited[v] && edge != 0 &&
                        dist[u] != int.MaxValue && dist[u] + edge < dist[v])
                    {
                        dist[v] = dist[u] + edge;
                        visited[v] = true;
                        queue.Enqueue(v, dist[v]);
                    }
                }
            }
            Console.WriteLine("No path found!");
        }
        
        int GetGraphValue(int u, int v)
        {
            int maxX = tiles * xLength;
            int x = (u % maxX);
            int y = (u / maxX);
            
            int x2 = (v % maxX);
            int y2 = (v / maxX);

            if (!IsValidMoveTiled(x2, y2))
            {
                Console.WriteLine("Not valid Tile");
                return 0;
            }
            
            var deltaTiles = Math.Abs(x2 - x) + Math.Abs(y2 - y);
            if (deltaTiles > 1)
            {
                Console.WriteLine($"IDs: {u} and {v}\n{x2},{y2} - {x},{y}");
                return 0;
            }
            
            return GetTiledRisk(x2, y2);
        }

        int[] GetNeighbors(int id)
        {
            List<int> available = new List<int>();
            
            int maxX = tiles * xLength;
            int maxY = tiles * yLength;
            int xVal = (id % maxX);
            int yVal = (id / maxX);
            
            for (int dy = -1; dy <= 1; dy++)
            {
                var ny = yVal + dy;
                if(ny < 0 || ny >= maxY) continue;
                
                for (int dx = -1; dx <= 1; dx++)
                {
                    var nx = xVal + dx;
                    if(nx < 0 || nx >= maxX) continue;
                    if (Math.Abs(dx) == Math.Abs(dy)) continue;
                    
                    var targetId = (ny * maxX) + nx;

                    available.Add(targetId);
                }
            }
            
            return available.ToArray();
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