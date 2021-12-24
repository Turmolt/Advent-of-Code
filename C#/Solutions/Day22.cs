using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Solutions;
using static G8S;
public class Day22 : IChallenge
{
    private List<Instruction> instructions;
    
    public void Solve(string[] data)
    {
        ParseData(data);

        int executed = 0;

        var boxes = new List<Box>();

        foreach (var instruction in instructions)
        {
            ExecuteInstruction(ref boxes, instruction, false);
            executed++;
            Log($"{executed}/{instructions.Count} complete");
        }

        var sum = boxes.Sum(b => b.Size);
        Log($"Part 2 Answer: {sum}",force:true);
    }
    
    void ExecuteInstruction(ref List<Box> boxes, Instruction instruction, bool clamp = false)
    {
        //clamp for p1
        if (clamp)
        {
            var initBox = new Box(new Range(-50, 50), new Range(-50, 50), new Range(-50, 50));
            if (!Intersects(instruction.box, initBox)) return;
        }

        var boxesAfterInstruction = new List<Box>();
        foreach (var box in boxes)
        {
            if (!Intersects(box, instruction.box))
            {
                boxesAfterInstruction.Add(box);
                continue;
            }
            
            SplitBox(ref boxesAfterInstruction, box, instruction.box);
        }

        if (instruction.command)
            boxesAfterInstruction.Add(instruction.box);
        boxes = boxesAfterInstruction;
    }
    
    void SplitBox(ref List<Box> boxes, Box target, Box intersector)
    {
        var xSegments = GetIntersectionRanges(target.x, intersector.x);
        var ySegments = GetIntersectionRanges(target.y, intersector.y);
        var zSegments = GetIntersectionRanges(target.z, intersector.z);

        foreach (var x in xSegments)
        {
            foreach (var y in ySegments)
            {
                foreach (var z in zSegments)
                {
                    var box = new Box(x, y, z);
                    if(!box.IsInside(intersector) && box.IsInside(target))
                        boxes.Add(box);
                }
            }
        }
    }
    
    List<Range> GetIntersectionRanges(Range a, Range b)
    {
        var ranges = new List<Range>();
        Range range = null;
        
        if (a.min < b.min)
        {
            range = new Range(a.min, b.min - 1);
            ranges.Add(range);
        }
        else
        {
            range = new Range(b.min, a.min-1);
        }
        
        if (a.max > b.max)
        {
            range = new Range(range.max + 1, b.max);
            ranges.Add(range);
            range = new Range(b.max + 1, a.max);
            ranges.Add(range);
        }
        else
        {
            range = new Range(range.max + 1, a.max);
            ranges.Add(range);
        }

        return ranges;
    }

    bool Intersects(Box a, Box b)
    {
        return (CheckRange(a.x, b.x) || CheckRange(b.x, a.x)) &&
               (CheckRange(a.y, b.y) || CheckRange(b.y, a.y)) &&
               (CheckRange(a.z, b.z) || CheckRange(b.z, a.z));
    }

    bool CheckRange(Range a, Range b)
    {
        return a.min >= b.min && a.min <= b.max;
    }

    record Range(long min, long max);

    record Instruction(bool command, Box box);

    record Box(Range x, Range y, Range z)
    {
        public long Size => (Math.Abs(x.max - x.min + 1L) * Math.Abs(y.max - y.min + 1) * Math.Abs(z.max - z.min + 1));

        public bool IsInside(Box target)
        {
            return x.min >= target.x.min && x.max <= target.x.max &&
                   y.min >= target.y.min && y.max <= target.y.max &&
                   z.min >= target.z.min && z.max <= target.z.max;
        }
        
        public override string ToString()
        {
            return $"{x.min},{x.max} | {y.min},{y.max} | {z.min},{z.max}";
        }
    }

    void ParseData(string[] data)
    {
        instructions = new List<Instruction>();
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

            instructions.Add(new Instruction(command, new Box(xRange, yRange, zRange)));
        }
    }
}