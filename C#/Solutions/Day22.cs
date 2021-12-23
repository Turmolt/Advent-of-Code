using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Solutions;
using static G8S;
public class Day22 : IChallenge
{
    private List<Instruction> instructions;

    private Dictionary<Coordinate, bool> map;
    public void Solve(string[] data)
    {
        map = new Dictionary<Coordinate, bool>();
        ParseData(data);

        // Part 1
        int executed = 0;

        var boxes = new List<Box>();
        var a = new Box(new Range(-5, 5), new Range(-5, 5), new Range(-5, 5));
        var b = new Box(new Range(-5, 5), new Range(-5, 5), new Range(-1, -1));
        
        // SplitBox(ref boxes,a,b);
        // Log("Boxes:");
        // foreach (var bx in boxes)
        // {
        //     Log(bx);
        // }
        // Log(boxes.Count);
        // return;
        //Part 2
        executed = 0;

        
        foreach (var instruction in instructions)
        {
            ExecuteInstruction(ref boxes, instruction, true);
            executed++;
            Log($"{executed}/{instructions.Count} complete");
           // Console.ReadKey();
        }
        
        var sum = boxes.Aggregate(0UL,(last, current) => last + current.Size);
        Log($"Part 2 Answer: {sum}");
    }
    void SplitBox(ref List<Box> boxes, Box target, Box intersector)
    {
        if(!Intersects(target, intersector))
            return;
        var xSegments = GetIntersectionRanges(target.x, intersector.x);
        var ySegments = GetIntersectionRanges(target.y, intersector.y);
        var zSegments = GetIntersectionRanges(target.z, intersector.z);
        Log($"Targets: {target} and {intersector}");
        Log("XSegments:");
        foreach(var segment in xSegments)
            Log(segment);
        Log("YSegments:");
        foreach(var segment in ySegments)
            Log(segment);
        Log("ZSegments:");
        foreach(var segment in zSegments)
            Log(segment);

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
        bool lowerSame = a.min == b.min;
        bool upperSame = a.max == b.max;

        if (lowerSame && upperSame)
        {
            range = new Range(a.min, a.max);
            ranges.Add(range);
            return ranges;
        }

        if (lowerSame)
        {
            if (a.max > b.max)
            {
                range = new Range(b.min, b.max);
                ranges.Add(range);
                range = new Range(b.max + 1, a.max);
                ranges.Add(range);
            }
            else
            {
                range = new Range(b.min, a.max);
                ranges.Add(range);
                range = new Range(a.max + 1, b.max);
                ranges.Add(range);
            }

            return ranges;
        }

        if (a.min < b.min)
        {
            range = new Range(a.min, b.min - 1);
            ranges.Add(range);
        }
        else
        {
            range = new Range(b.min, a.min);
            ranges.Add(range);
        }

        
        if (upperSame)
        {
            range = new Range(range.max + 1, b.max);
            ranges.Add(range);
            return ranges;
        }
        
        if (a.max != b.max)
        {
            if (a.max > b.max)
            {
               // Log($"A max > b Max detected: {a.max} > {b.max}\nSplitting from {range.max+1} to {b.max}\n and {b.max+1} to {a.max}");
                range = new Range(range.max + 1, b.max);
                ranges.Add(range);
                range = new Range(b.max + 1, a.max);
                ranges.Add(range);
            }
            else
            {
                range = new Range(range.max + 1, a.max);
                ranges.Add(range);
                range = new Range(range.max + 1, b.max);
                ranges.Add(range);
            }
        }

        return ranges;
    }
    
    void ExecuteInstruction(ref List<Box> boxes, Instruction instruction, bool clamp = false)
    {
        var instructionBox = new Box(instruction.x, instruction.y, instruction.z);
        
        //clamp for p1
        if (clamp)
        {
            var initBox = new Box(new Range(-50, 50), new Range(-50, 50), new Range(-50, 50));
            if (!GetIntersection(instructionBox, initBox).Any) return;
        }

        var boxesAfterInstruction = new List<Box>();
        foreach (var box in boxes)
        {
            SplitBox(ref boxesAfterInstruction, box, instructionBox);
            
        }

        if (instruction.command)
            boxesAfterInstruction.Add(instructionBox);
        boxes = boxesAfterInstruction;
    }

    void SplitRecursively(ref List<Box> boxes, Box target, Box intersector, Intersection intersection, List<Axis> previousSplits)
    {
        Log(target);
        Log(intersector);
        Log(intersection);
        Log(previousSplits.Aggregate("",(p,c)=>p+" "+c));
//        Console.ReadKey();
        if (intersection.nx && !previousSplits.Contains(Axis.PX))
        {
            Log($"PX:\n{target}\n{intersector}");
            var (positive, negative) = target.SplitAlongAxis(Axis.PX, intersector.x.max);
            CheckSquare(ref boxes, positive, intersector, new List<Axis>(previousSplits) { Axis.PX });
            CheckSquare(ref boxes, negative, intersector, new List<Axis>(previousSplits) { Axis.PX });
            return;
        }

        if (intersection.px && !previousSplits.Contains(Axis.NX))
        {
            Log($"NX\n{target}\n{intersector}");
            var (positive, negative) = target.SplitAlongAxis(Axis.NX, intersector.x.min);
            CheckSquare(ref boxes, positive, intersector, new List<Axis>(previousSplits) { Axis.NX });
            CheckSquare(ref boxes, negative, intersector, new List<Axis>(previousSplits) { Axis.NX });
            return;
        }
        
        if (intersection.ny && !previousSplits.Contains(Axis.PY))
        {
            
            Log($"PY\n{target}\n{intersector}");
            var (positive, negative) = target.SplitAlongAxis(Axis.PY, intersector.y.max);
            CheckSquare(ref boxes, positive, intersector, new List<Axis>(previousSplits) { Axis.PY });
            CheckSquare(ref boxes, negative, intersector, new List<Axis>(previousSplits) { Axis.PY });
            return;
        }

        if (intersection.py && !previousSplits.Contains(Axis.NY))
        {
            Log($"NY\n{target}\n{intersector}");
            var (positive, negative) = target.SplitAlongAxis(Axis.NY, intersector.y.min);
            CheckSquare(ref boxes, positive, intersector, new List<Axis>(previousSplits) { Axis.NY });
            CheckSquare(ref boxes, negative, intersector, new List<Axis>(previousSplits) { Axis.NY });
            return;
        }
        
        if (intersection.nz && !previousSplits.Contains(Axis.PZ))
        {
            Log($"PZ\n{target}\n{intersector}");
            var (positive, negative) = target.SplitAlongAxis(Axis.PZ, intersector.z.max);
            CheckSquare(ref boxes, positive, intersector, new List<Axis>(previousSplits) { Axis.PZ });
            CheckSquare(ref boxes, negative, intersector, new List<Axis>(previousSplits) { Axis.PZ });
            return;
        }

        if (intersection.pz && !previousSplits.Contains(Axis.NZ))
        {
            Log($"NZ\n{target}\n{intersector}\n");
            var (positive, negative) = target.SplitAlongAxis(Axis.NZ, intersector.z.min);
            Log(positive);
            Log(negative);
            CheckSquare(ref boxes, positive, intersector, new List<Axis>(previousSplits) { Axis.NZ });
            CheckSquare(ref boxes, negative, intersector, new List<Axis>(previousSplits) { Axis.NZ });
            return;
        }
        
    }

    void CheckSquare(ref List<Box> boxes, Box target, Box intersector, List<Axis> previousSplits)
    {
        if (target == null)
        {
            Log($"null");
            return;
        }
        Log($"Checking: {target}");
        var intersection = GetIntersection(target, intersector);
        
        if (!target.IsInside(intersector))
        {
            if (intersection.Any)
            {
                SplitRecursively(ref boxes, target, intersector, intersection, previousSplits);
            }
            else
            {
                Log($"Adding");
                boxes.Add(target);
            }
        }
        else
        {
            Log(target);
            Log("Inside");
        }
    }
    
    Range ExtendPositive(Range target, Range intersecting) => new Range(intersecting.min, target.max);
    Range ExtendNegative(Range target, Range intersecting) => new Range(target.min, intersecting.max);
    Range NonCollidingPositive(Range target, Range intersecting) => new Range(intersecting.max + 1, target.max);
    Range NonCollidingNegative(Range target, Range intersecting) => new Range(target.min, intersecting.min - 1);

    bool Intersects(Box a, Box b)
    {
        if (CheckRange(a.x, b.x)) return true;
        if (CheckRange(b.x, a.x)) return true;
        if (CheckRange(a.y, b.y)) return true;
        if (CheckRange(b.y, a.y)) return true;
        if (CheckRange(a.z, b.z)) return true;
        if (CheckRange(b.z, a.z)) return true;
        return false;
    }

    bool CheckRange(Range a, Range b)
    {
        if (a.min >= b.min && a.min <= b.max) return true;
        if (a.max >= b.min && a.max <= b.max) return true;
        return false;
    }
    
    Intersection GetIntersection(Box boxToSplit, Box incomingBox)
    {
        Intersection intersection = new Intersection();

        if (boxToSplit.z.min > incomingBox.z.max || boxToSplit.z.max < incomingBox.z.min)
        {
            return intersection;
        };
        if (boxToSplit.y.min > incomingBox.y.max || boxToSplit.y.max < incomingBox.y.min)
        {
            return intersection;
        }

        if (boxToSplit.x.min > incomingBox.x.max || boxToSplit.x.max < incomingBox.x.min)
        {
            return intersection;
        }

        if (boxToSplit.x.min >= incomingBox.x.min || boxToSplit.x.max >= incomingBox.x.max)
        {
            intersection.nx = true;
        }
        
        if (boxToSplit.x.max <= incomingBox.x.max || boxToSplit.x.min <= incomingBox.x.min)
        {
            intersection.px = true;
        }

        if (boxToSplit.y.min >= incomingBox.y.min || boxToSplit.y.max >= incomingBox.y.max)
        {
            intersection.ny = true;
        }
        
        if (boxToSplit.y.max <= incomingBox.y.max || boxToSplit.y.min <= incomingBox.y.min)
        {
            intersection.py = true;
        }
        
        if (boxToSplit.z.min >= incomingBox.z.min || boxToSplit.z.max >= incomingBox.z.max)
        {
            intersection.nz = true;
        }

        if (boxToSplit.z.min <= incomingBox.z.min || boxToSplit.z.max <= incomingBox.z.max)
        {
            intersection.pz = true;
        }
        
        return intersection;
    }

    record Coordinate(int x, int y, int z);

    record Range(int min, int max);

    record Instruction(bool command, Range x, Range y, Range z);

    record Box(Range x, Range y, Range z)
    {
        public ulong Size => (ulong)(Math.Abs(x.max - x.min + 1) * Math.Abs(y.max - y.min + 1) * Math.Abs(z.max - z.min + 1));

        public bool IsInside(Box target)
        {
            return x.min >= target.x.min && x.max <= target.x.max &&
                   y.min >= target.y.min && y.max <= target.y.max &&
                   z.min >= target.z.min && z.max <= target.z.max;
        }

        public (Box positive, Box negative) SplitAlongAxis(Axis axis, int position)
        {
            Box negative = null;
            Box positive = null;
            
            switch (axis)
            {
                case Axis.PX:
                    Log($"Splitting PX at {position}\n{this}");
                    negative = new Box(new Range(x.min, position), y, z);
                    positive = new Box(new Range(position + 1, x.max), y, z);
                    Log($"Result:\n{positive}\n{negative}");
                    if (x.max < position + 1) positive = null;
                    if (x.min > position) negative = null;
                    break;
                case Axis.NX:
                    Log($"Splitting NX at {position}\n{this}");
                    negative = new Box(new Range(x.min, position - 1), y, z);
                    positive = new Box(new Range(position, x.max), y, z);
                    Log($"Result:\n{positive}\n{negative}");
                    if (x.max < position) positive = null;
                    if (x.min > position - 1) negative = null;
                    break;
                case Axis.PY:
                    Log($"Splitting PY at {position}\n{this}");
                    negative = new Box(x, new Range(y.min, position), z);
                    positive = new Box(x, new Range(position + 1, y.max), z);
                    Log($"Result:\n{positive}\n{negative}");
                    if (y.max < position + 1) positive = null;
                    if (y.min > position) negative = null;
                    break;
                case Axis.NY:
                    Log($"Splitting NY at {position}\n{this}");
                    negative = new Box(x, new Range(y.min, position - 1), z);
                    positive = new Box(x,  new Range(position, y.max), z);
                    Log($"Result:\n{positive}\n{negative}");
                    if (y.max < position) positive = null;
                    if (y.min > position - 1) negative = null;
                    break;
                case Axis.PZ:
                    Log($"Splitting PZ at {position}\n{this}");
                    negative = new Box(x, y, new Range(z.min, position));
                    positive = new Box(x, y, new Range(position + 1, z.max));
                    Log($"Result:\n{positive}\n{negative}");
                    if (z.max < position + 1) positive = null;
                    if (z.min > position) negative = null;
                    break;
                case Axis.NZ:
                    Log($"Splitting NZ at {position}\n{this}");
                    negative = new Box(x, y, new Range(z.min, position - 1));
                    positive = new Box(x, y, new Range(position, z.max));
                    Log($"Result:\n{positive}\n{negative}");

                    if (z.max < position) positive = null;
                    if (z.min > position - 1) negative = null;
                    break;
            }

            return (positive, negative);
        }
        
        public override string ToString()
        {
            return $"{x.min},{x.max} | {y.min},{y.max} | {z.min},{z.max}";
        }
    }

    enum Axis
    {
        PX,
        PY,
        PZ,
        NX,
        NY,
        NZ
    }
    
    struct Intersection
    {
        public bool px;
        public bool nx;
        public bool py;
        public bool ny;
        public bool pz;
        public bool nz;
        public bool Any => px || nx || py || ny || pz || nz;

        public Intersection(bool px, bool nx, bool py, bool ny, bool pz, bool nz)
        {
            this.px = px;
            this.nx = nx;
            this.py = py;
            this.ny = ny;
            this.pz = pz;
            this.nz = nz;
        }

        public int Count()
        {
            int count = 0;
            if (px) count++;
            if (nx) count++;
            if (py) count++;
            if (ny) count++;
            if (pz) count++;
            if (nz) count++;
            return count;
        }

        public override string ToString()
        {
            return $"X: {nx}, {px}\n" +
                   $"Y: {ny}, {py}\n" +
                   $"Z: {nz}, {pz}";
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

            instructions.Add(new Instruction(command, xRange, yRange, zRange));
        }
    }
}