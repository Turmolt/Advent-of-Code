using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Solutions;
using static G8S;

public class Day20 : IChallenge
{
    private string Lookup;
    private Dictionary<(int, int), char> map;
    private int numSteps;

    private int xSize;
    private int ySize;
    private int padding = 1;

    private Bounds bounds;
    
    public void Solve(string[] data)
    {
        ParseData(data);
        bounds = new Bounds(-xSize / 2, xSize / 2, -ySize / 2, ySize / 2);
        
        //PrintImage();
        while (numSteps < 50)
        {
            Step();
            Log(numSteps,force:true);
            numSteps++;
        }
        
        PrintImage();
    }

    void Step()
    {
        var nextMap = new Dictionary<(int, int), char>();
        var nextBounds = new Bounds(0, 0, 0, 0);
        for (int y = bounds.MinY - padding; y <= bounds.MaxY + padding; y++)
        {
            for (int x = bounds.MinX - padding; x <= bounds.MaxX + padding; x++)
            {
                var index = LookupIndex(x, y);
                var nextChar = Lookup[index];
                
                nextMap[(x, y)] = nextChar;
                if (nextBounds.MinX > x) nextBounds.MinX = x;
                if (nextBounds.MaxX < x) nextBounds.MaxX = x;
                if (nextBounds.MinY > y) nextBounds.MinY = y;
                if (nextBounds.MaxY < y) nextBounds.MaxY = y;
            }
        }

        Log(bounds);
        bounds = nextBounds;
        map = nextMap;
    }

    void PrintImage()
    {
        int count = 0;
        for (int y = bounds.MinY; y <=bounds.MaxY; y++)
        {
            for (int x = bounds.MinX; x <=bounds.MaxX; x++)
            {
                var output = map[(x, y)];
                if (output == '#') count++;
                Log($"{output}",writeLine:false, true);
            }
            Log("", force:true);
        }
        
        Log($"There was a total of {count} white pixels", force:true);
    }
    
    

    int LookupIndex(int x, int y)
    {
        var result = "";
        for (int dy = -1; dy <= 1; dy++)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                var coord = (x + dx, y + dy);
                if (map.ContainsKey(coord))
                {
                    result += map[coord] == '#' ? '1':'0';
                }
                else
                {
                    result += numSteps % 2 == 0 ? '0' : '1';
                }
            }
        }

        return Convert.ToInt32(result, 2);
    }

    void ParseData(string[] data)
    {
        Lookup = data[0];

        map = new Dictionary<(int, int), char>();
        
        var d = data[2..];
        xSize = d[0].Length;
        ySize = d.Length;
        for (int y = 0; y < d.Length; y++)
        {
            for (int x = 0; x < d[y].Length; x++)
            {
                var coord = (x - (xSize / 2), y - (ySize / 2));
                map[coord] = d[y][x];
            }
        }

        for (int x = 0; x < xSize; x++)
        {
            for (int i = 1; i <= padding; i++)
            {
                var nyPad = (x - (xSize / 2), -ySize / 2 - i);
                var pyPad = (x - (xSize / 2), ySize / 2 + i);
                map[nyPad] = '.';
                map[pyPad] = '.';
            }
        }
        
        for (int y = 0; y < ySize; y++)
        {
            for (int i = 1; i <= padding; i++)
            {
                var nxPad = (-xSize / 2 - i, y - (ySize / 2));
                var pxPad = (xSize / 2 + i,y - (ySize / 2));
                map[nxPad] = '.';
                map[pxPad] = '.';
            }
        }
    }
}

struct Bounds
{
    public int MinX;
    public int MaxX;
    public int MinY;
    public int MaxY;

    public Bounds(int mx, int Mx, int my, int My)
    {
        MinX = mx;
        MaxX = Mx;
        MinY = my;
        MaxY = My;
    }

    public override string ToString()
    {
        return $"X: {MinX} -> {MaxX}, Y: {MinY} -> {MaxY}";
    }
}