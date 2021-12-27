using System;

namespace Advent_of_Code.Solutions;
using static G8S;

class Day25 : IChallenge
{
    private char[,] map;
    
    public void Solve(string[] data)
    {
        ParseData(data);
        int steps = 0;
        int totalMoved = 1;
        int allMoved = 0;
        while (totalMoved > 0)
        {
            totalMoved = 0;
            
           // PrintMap();
            var nextMap = map.Clone() as char[,];
            totalMoved += MoveHorizontal(ref nextMap);
            
            map = nextMap;
            
            nextMap = map.Clone() as char[,];
            totalMoved += MoveVertical(ref nextMap);
            map = nextMap;
            
            allMoved += totalMoved;
            steps++;
           // Console.ReadKey();
            //Log("\n");
        }
        Log($"Cucumbers stopped moving after {steps} steps.");
    }

    void PrintMap()
    {
        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                Log(map[y,x],writeLine:false);
            }

            Log("");
        }
    }

    int MoveHorizontal(ref char[,] nextMap)
    {
        var moved = 0;

        for (var y = 0; y < map.GetLength(0); y++)
        {
            var length = map.GetLength(1);
            for (var x = 0; x < length; x++)
            {
                if (map[y, x] != '>') continue;
                var nextX = (x + 1) % length;
                if (map[y, nextX] != 'v' && map[y, nextX] != '>')
                {
                    nextMap[y, x] = '.';
                    nextMap[y, nextX] = '>';
                    moved++;
                }
                else
                {
                    nextMap[y, x] = '>';
                }
            }
        }

        return moved;
    }
    
    int MoveVertical(ref char[,] nextMap)
    {
        int moved = 0;
        for (var y = 0; y < map.GetLength(0); y++)
        {
            var length = map.GetLength(1);
            for (var x = 0; x < length; x++)
            {
                if (map[y, x] != 'v') continue;
                var nextY = (y + 1) % map.GetLength(0);
                if (map[nextY, x] != 'v' && map[nextY, x] != '>')
                {
                    nextMap[y, x] = '.';
                    nextMap[nextY, x] = 'v';
                    moved++;
                }
                else
                {
                    nextMap[y, x] = 'v';
                }
            }
        }
        return moved;
    }
    
    


    void ParseData(string[] data)
    {
        map = new char[data.Length,data[0].Length];
        for (int y = 0; y < data.Length; y++)
        {
            var line = data[y];
            for (int x = 0; x < line.Length; x++)
            {
                map[y, x] = line[x];
            }
        }
    }
    
}

