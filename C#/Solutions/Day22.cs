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
            ExecuteInstruction(ref boxes, instruction);
            executed++;
            //Log($"{executed}/{instructions.Count} complete");
        }

        var sum = boxes.Sum(b => b.Size);
        Log($"Part 2 Answer: {sum}");
    }

    void ExecuteInstruction(ref List<Box> boxes, Instruction instruction, bool clamp = false)
    {
        var boxesAfterInstruction = new List<Box>();

        if (clamp && !Intersects(instruction.box,
                new Box(new Range(-50, 50), new Range(-50, 50), new Range(-50, 50)))) return;
        
        foreach (var box in boxes)
        {
            SplitRecursively(ref boxesAfterInstruction, box, instruction.box, new List<Split>());
        }

        if (instruction.command)
            boxesAfterInstruction.Add(instruction.box);
        
        boxes = boxesAfterInstruction;
    }

    void SplitRecursively(ref List<Box> boxes, Box target, Box intersector, List<Split> previousSplits)
    {
        if (!Intersects(target, intersector))
        {
            boxes.Add(target);
            return;
        }
        
        if (!previousSplits.Contains(Split.PreX))
        {
            var (positive, negative) = target.SplitAlongAxis(Split.PreX, intersector.x.min);
            CheckSquare(ref boxes, positive, intersector, new List<Split>(previousSplits) { Split.PreX });
            CheckSquare(ref boxes, negative, intersector, new List<Split>(previousSplits) { Split.PreX });
            return;
        }

        if (!previousSplits.Contains(Split.PostX))
        {
            var (positive, negative) = target.SplitAlongAxis(Split.PostX, intersector.x.max);
            CheckSquare(ref boxes, positive, intersector, new List<Split>(previousSplits) { Split.PostX });
            CheckSquare(ref boxes, negative, intersector, new List<Split>(previousSplits) { Split.PostX });
            return;
        }
        
        if (!previousSplits.Contains(Split.PreY))
        {
            var (positive, negative) = target.SplitAlongAxis(Split.PreY, intersector.y.min);
            CheckSquare(ref boxes, positive, intersector, new List<Split>(previousSplits) { Split.PreY });
            CheckSquare(ref boxes, negative, intersector, new List<Split>(previousSplits) { Split.PreY });
            return;
        }

        if (!previousSplits.Contains(Split.PostY))
        {
            var (positive, negative) = target.SplitAlongAxis(Split.PostY, intersector.y.max);
            CheckSquare(ref boxes, positive, intersector, new List<Split>(previousSplits) { Split.PostY });
            CheckSquare(ref boxes, negative, intersector, new List<Split>(previousSplits) { Split.PostY });
            return;
        }
        
        if (!previousSplits.Contains(Split.PreZ))
        {
            var (positive, negative) = target.SplitAlongAxis(Split.PreZ, intersector.z.min);
            CheckSquare(ref boxes, positive, intersector, new List<Split>(previousSplits) { Split.PreZ });
            CheckSquare(ref boxes, negative, intersector, new List<Split>(previousSplits) { Split.PreZ });
            return;
        }

        if (!previousSplits.Contains(Split.PostZ))
        {   
            var (positive, negative) = target.SplitAlongAxis(Split.PostZ, intersector.z.max);
            CheckSquare(ref boxes, positive, intersector, new List<Split>(previousSplits) { Split.PostZ });
            CheckSquare(ref boxes, negative, intersector, new List<Split>(previousSplits) { Split.PostZ });
            return;
        }
        
    }

    void CheckSquare(ref List<Box> boxes, Box target, Box intersector, List<Split> previousSplits)
    {
        if (target == null)
        {
            return;
        }
        
        if (!target.IsInside(intersector))
        {
            if (Intersects(target, intersector))
            {
                SplitRecursively(ref boxes, target, intersector, previousSplits);
            }
            else
            {
                boxes.Add(target);
            }
        }
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

    enum Split
    {
        PreX,
        PostX,
        PreY,
        PostY,
        PreZ,
        PostZ
    }

    record Box
    {
        public Range x;
        public Range y;
        public Range z;
        
        public Box(Range x, Range y, Range z)
        {
            this.x = new Range(Math.Min(x.min, x.max), Math.Max(x.min, x.max));
            this.y = new Range(Math.Min(y.min, y.max), Math.Max(y.min, y.max));
            this.z = new Range(Math.Min(z.min, z.max), Math.Max(z.min, z.max));
        }
        
        public long Size => (Math.Abs(x.max - x.min + 1L) * Math.Abs(y.max - y.min + 1) * Math.Abs(z.max - z.min + 1));

        public bool IsInside(Box target)
        {
            return x.min >= target.x.min && x.max <= target.x.max &&
                   y.min >= target.y.min && y.max <= target.y.max &&
                   z.min >= target.z.min && z.max <= target.z.max;
        }

        public (Box positive, Box negative) SplitAlongAxis(Split split, long position)
        {
            Box negative, positive;
            Range n, p;
            
            switch (split)
            {
                case Split.PreX:
                    if (position <= x.min || position > x.max)
                        return (null, this);
                    (n, p) = BreakNegative(x, position);
                    negative = new Box(n, y, z);
                    positive = new Box(p, y, z);
                    return (negative, positive);
                case Split.PostX:
                    if (position < x.min || position >= x.max)
                        return (this, null);

                    (n, p) = BreakPositive(x, position);
                    negative = new Box(n, y, z);
                    positive = new Box(p, y, z);
                    return (negative, positive);
                case Split.PreY:
                    if (position <= y.min || position > y.max)
                        return (null, this);

                    (n, p) = BreakNegative(y, position);
                    negative = new Box(x, n, z);
                    positive = new Box(x, p, z);
                    return (negative, positive);
                case Split.PostY:
                    if (position < y.min || position >= y.max)
                        return (this, null);

                    (n, p) = BreakPositive(y, position);
                    negative = new Box(x, n, z);
                    positive = new Box(x, p, z);
                    return (negative, positive);
                case Split.PreZ:
                    if (position <= z.min || position > z.max)
                        return (null, this);

                    (n, p) = BreakNegative(z, position);
                    negative = new Box(x, y, n);
                    positive = new Box(x, y, p);
                    return (negative, positive);
                case Split.PostZ:
                    if (position < z.min || position >= z.max)
                        return (this, null);

                    (n, p) = BreakPositive(z, position);
                    negative = new Box(x, y, n);
                    positive = new Box(x, y, p);
                    return (negative, positive);
            }

            return (null, null);
        }
        
        (Range negative, Range positive) BreakNegative(Range target, long position)
        {
            return (new Range(target.min, position - 1), new Range(position, target.max));
        }
        
        (Range negative, Range positive) BreakPositive(Range target, long position)
        {
            return (new Range(target.min, position), new Range(position + 1, target.max));
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