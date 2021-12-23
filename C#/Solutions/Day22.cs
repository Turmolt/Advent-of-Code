using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Solutions;
using static G8S;
public class Day22 : IChallenge
{
    private List<Box> instructions;

    private Dictionary<Coordinate, bool> map;
    public void Solve(string[] data)
    {
        ParseData(data);

        // Part 1
        int executed = 0;

        var boxes = new List<Box>();

        executed = 0;

        
        foreach (var box in instructions)
        {
            var overlapping = boxes.Select(intersector => GetOverlappingBox(box, intersector)).ToList();  
            var validOverlaps = overlapping.Where(ValidOverlap).ToList();
            boxes.AddRange(validOverlaps);

            if (box.state)
                boxes.Add(box);            
        
            executed++;
            Log($"{executed}/{instructions.Count} complete");
        }

        var sum = boxes.Sum(b => b.Size * (b.state ? 1L : -1));
        Log($"Part 2 Answer: {sum}");
    }

    bool ValidOverlap(Box a)
    {
        return a.x.lo <= a.x.hi && a.y.lo <= a.y.hi && a.z.lo <= a.z.hi;
    }

    record Coordinate(int x, int y, int z);

    record Range(int lo, int hi);
    
    record Box(bool state, Range x, Range y, Range z)
    {
        public long Size => ((x.hi - x.lo + 1L) * (y.hi - y.lo + 1) * (z.hi - z.lo + 1));
        public override string ToString()
        {
            return $"{x.lo},{x.hi} | {y.lo},{y.hi} | {z.lo},{z.hi}";
        }
    }

    static Box GetOverlappingBox(Box a, Box b)
    {
        return new Box(!b.state, new Range(Math.Max(a.x.lo, b.x.lo), Math.Min(a.x.hi, b.x.hi)), 
                                 new Range(Math.Max(a.y.lo, b.y.lo), Math.Min(a.y.hi, b.y.hi)),
                                 new Range(Math.Max(a.z.lo, b.z.lo), Math.Min(a.z.hi, b.z.hi)));
    }

    void ParseData(string[] data)
    {
        instructions = new List<Box>();
        foreach (var line in data)
        {
            var tokens = line.Split(' ');
            var command = tokens[0] == "on";
            var rangeTokens = tokens[1].Split(',');

            var xRangeTokens = rangeTokens[0][2..].Split("..");
            var yRangeTokens = rangeTokens[1][2..].Split("..");
            var zRangeTokens = rangeTokens[2][2..].Split("..");

            var xRange = new Range(int.Parse(xRangeTokens[0]), int.Parse(xRangeTokens[1]));
            var yRange = new Range(int.Parse(yRangeTokens[0]), int.Parse(yRangeTokens[1]));
            var zRange = new Range(int.Parse(zRangeTokens[0]), int.Parse(zRangeTokens[1]));

            instructions.Add(new Box(command, xRange, yRange, zRange));
        }
    }
}