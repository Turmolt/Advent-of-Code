using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Solutions;
using static G8S;
public class Day22 : IChallenge
{
    record Coordinate(int x, int y, int z);

    record Range(int min, int max);

    record Instruction(bool command, Range x, Range y, Range z);

    record Box(Range x, Range y, Range z)
    {
        public ulong Size => (ulong)(Math.Abs(x.max - x.min + 1) * Math.Abs(y.max - y.min + 1) * Math.Abs(z.max - z.min + 1));
        public override string ToString()
        {
            return $"{x.min},{x.max} | {y.min},{y.max} | {z.min},{z.max}";
        }
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

    private List<Instruction> instructions;

    private Dictionary<Coordinate, bool> map;
    public void Solve(string[] data)
    {
        map = new Dictionary<Coordinate, bool>();
        ParseData(data);

        // Part 1
        int executed = 0;
        Console.ReadKey();
        var boxes = new List<Box>();
        
        // Part 2
        executed = 0;
        foreach (var instruction in instructions)
        {
            ExecuteInstruction(ref boxes, instruction);
            executed++;
            Log($"{executed}/{instructions.Count} complete");
           // Console.ReadKey();
        }
        
        var sum = boxes.Aggregate(0UL,(last, current) => last + current.Size);
        Log($"Part 2 Answer: {sum}");
    }

    void DebugPrint()
    {
        var boxes = new List<Box>();
        var a = new Box(new Range(-5, 5), new Range(-5, 5), new Range(-5, 5));
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dz = -1; dz <= 1; dz++)
                {
                    boxes = new List<Box>();
                    var bsize = 3;
                    var b = new Box(new Range(-bsize + dx * 2, bsize + dx * 2), new Range(-bsize + dy * 2, bsize + dy * 2),
                        new Range(-bsize + dz * 2, bsize + dz * 2));
                    Log($"{a}\n{b}");
                    var intersection = GetIntersection(a, b);
                    Log(intersection);
                    Log("");
                    SplitBox(ref boxes, a, b, intersection);
                    foreach (var box in boxes)
                    {
                        Log(box);
                        Log("\n");
                    }

                    Console.ReadKey();
                }
            }
        }
    }

    void ExecuteInstruction(ref List<Box> boxes, Instruction instruction, bool clamp = false)
    {
        var instructionBox = new Box(instruction.x, instruction.y, instruction.z);
        
        //clamp for p1
        if (clamp)
        {
            var initBox = new Box(new Range(-50, 50), new Range(-50, 50), new Range(-50, 50));
//            Log($"{initBox} * {instructionBox} =\n{GetIntersection(instructionBox, initBox)}");
            if (!GetIntersection(instructionBox, initBox).Any) return;
        }

        var boxesAfterInstruction = new List<Box>();
        foreach (var box in boxes)
        {
            var intersection = GetIntersection(box, instructionBox);
            
            SplitBox(ref boxesAfterInstruction, box, instructionBox, intersection);
            
            // Log(intersection);
            //
            // Log($"{box}\n{instructionBox}\n");
            // Log($"RESULT:");
            // foreach (var newBox in boxesAfterInstruction)
            // {
            //     Log(newBox);
            // }
            //
            // Log("");
            // Console.ReadKey();
        }

        if (instruction.command)
            boxesAfterInstruction.Add(instructionBox);
        // Log($"{instruction}");
        // foreach (var box in boxesAfterInstruction)
        // {
        //     Log(box);
        // }
        boxes = boxesAfterInstruction;
    }

    void SplitBox(ref List<Box> boxes, Box target, Box intersecting, Intersection intersection)
    {
        switch (intersection.Count())
        {
            case 5:
                FiveIntersections(ref boxes, target, intersecting, intersection);
                break;
            case 4:
                FourIntersections(ref boxes, target, intersecting, intersection);
                break;
            case 3:
                ThreeIntersections(ref boxes, target, intersecting, intersection);
                break;
            case 2:
                TwoIntersections(ref boxes, target, intersecting, intersection);
                break;
            case 1:
                OneIntersection(ref boxes, target, intersecting, intersection);
                break;
            case 0:
                boxes.Add(target);
                break;
            
        }
    }

    void OneIntersection(ref List<Box> boxes, Box target, Box intersecting, Intersection intersection)
    {
        if (intersection.nz)
        {
            var extraBoxXRange = intersecting.x; 
            var extraBoxYRange = intersecting.y;
            var extraBoxZRange = NonCollidingPositive(target.z, intersecting.z);
            var extraBox = new Box(extraBoxXRange, extraBoxYRange, extraBoxZRange);
            boxes.Add(extraBox);
            
            //use two intersection with extended box to grab the rest
            var extended = new Box(intersecting.x, intersecting.y, new Range(intersecting.z.min, target.z.max + 1));
            var extendedIntersection = GetIntersection(target, extended);
            TwoIntersections(ref boxes, target, extended, extendedIntersection);
            return;
        }
        
        if (intersection.pz)
        {
            var extraBoxXRange = intersecting.x; 
            var extraBoxYRange = intersecting.y;
            var extraBoxZRange = NonCollidingNegative(target.z, intersecting.z);
            var extraBox = new Box(extraBoxXRange, extraBoxYRange, extraBoxZRange);
            boxes.Add(extraBox);
            
            //use two intersection with extended box to grab the rest
            var extended = new Box(intersecting.x, intersecting.y, new Range(target.z.min-1, intersecting.z.max));
            var extendedIntersection = GetIntersection(target, extended);
            TwoIntersections(ref boxes, target, extended, extendedIntersection);
            return;
        }
        
        if (intersection.nx)
        {
            var extraBoxXRange = NonCollidingPositive(target.x, intersecting.x);; 
            var extraBoxYRange = intersecting.y;
            var extraBoxZRange = intersecting.z;
            var extraBox = new Box(extraBoxXRange, extraBoxYRange, extraBoxZRange);
            boxes.Add(extraBox);
            
            //use two intersection with extended box to grab the rest
            var extended = new Box(new Range(intersecting.x.min, target.x.max + 1), intersecting.y, intersecting.z);
            var extendedIntersection = GetIntersection(target, extended);
            TwoIntersections(ref boxes, target, extended, extendedIntersection);
            return;
        }
        
        if (intersection.px)
        {
            var extraBoxXRange = NonCollidingNegative(target.x, intersecting.x);; 
            var extraBoxYRange = intersecting.y;
            var extraBoxZRange = intersecting.z;
            var extraBox = new Box(extraBoxXRange, extraBoxYRange, extraBoxZRange);
            boxes.Add(extraBox);
            
            //use two intersection with extended box to grab the rest
            var extended = new Box(new Range(target.x.min-1, intersecting.x.max), intersecting.y, intersecting.z);
            var extendedIntersection = GetIntersection(target, extended);
            Log(extendedIntersection);
            TwoIntersections(ref boxes, target, extended, extendedIntersection);
            return;
        }
        
        if (intersection.py)
        {
            var extraBoxXRange = intersecting.x;
            var extraBoxYRange = NonCollidingNegative(target.y, intersecting.y);
            var extraBoxZRange = intersecting.z;
            var extraBox = new Box(extraBoxXRange, extraBoxYRange, extraBoxZRange);
            boxes.Add(extraBox);
            
            //use two intersection with extended box to grab the rest
            var extended = new Box(intersecting.x,new Range(target.y.min-1, intersecting.y.max), intersecting.z);
            var extendedIntersection = GetIntersection(target, extended);
            TwoIntersections(ref boxes, target, extended, extendedIntersection);
            return;
        }
        
        if (intersection.ny)
        {
            var extraBoxXRange = intersecting.x;
            var extraBoxYRange = NonCollidingPositive(target.y, intersecting.y);
            var extraBoxZRange = intersecting.z;
            var extraBox = new Box(extraBoxXRange, extraBoxYRange, extraBoxZRange);
            boxes.Add(extraBox);
            
            //use two intersection with extended box to grab the rest
            var extended = new Box(intersecting.x,new Range(intersecting.y.min, target.y.max+1), intersecting.z);
            var extendedIntersection = GetIntersection(target, extended);
            TwoIntersections(ref boxes, target, extended, extendedIntersection);
            return;
        }
    }
    
    void TwoIntersections(ref List<Box> boxes, Box target, Box intersecting, Intersection intersection)
    {
        if (intersection.px && intersection.nx)
        {
            var yRangeB1 = new Range(intersecting.y.max + 1, target.y.max);
            var yRangeB2 = new Range(target.y.min, intersecting.y.min - 1);

            var zRangeB12 = intersecting.z;
            var zRangeB3 = new Range(target.z.min, intersecting.z.min - 1);
            var zRangeB4 = new Range(intersecting.z.max + 1, target.z.max);

            var box1 = new Box(target.x, yRangeB1, zRangeB12);
            var box2 = new Box(target.x, yRangeB2, zRangeB12);
            var box3 = new Box(target.x, target.y, zRangeB3);
            var box4 = new Box(target.x, target.y, zRangeB4);
            
            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);
            boxes.Add(box4);
            return;
        }
        
        if (intersection.pz && intersection.nz)
        {
            var yRangeB1 = new Range(intersecting.y.max + 1, target.y.max);
            var yRangeB2 = new Range(target.y.min, intersecting.y.min - 1);

            var xRangeB12 = intersecting.x;
            var xRangeB3 = new Range(target.x.min, intersecting.x.min - 1);
            var xRangeB4 = new Range(intersecting.x.max + 1, target.x.max);

            var box1 = new Box(xRangeB12,yRangeB1, target.z);
            var box2 = new Box(xRangeB12,yRangeB2, target.z);
            var box3 = new Box(xRangeB3, target.y, target.z);
            var box4 = new Box(xRangeB4, target.y, target.z);
            
            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);
            boxes.Add(box4);
            return;
        }
        
        if (intersection.py && intersection.ny)
        {
            var zRangeB1 = new Range(intersecting.z.max + 1, target.z.max);
            var zRangeB2 = new Range(target.z.min, intersecting.z.min - 1);

            var xRangeB12 = intersecting.x;
            var xRangeB3 = new Range(target.x.min, intersecting.x.min - 1);
            var xRangeB4 = new Range(intersecting.x.max + 1, target.x.max);

            var box1 = new Box(xRangeB12,target.y, zRangeB1);
            var box2 = new Box(xRangeB12,target.y, zRangeB2);
            var box3 = new Box(xRangeB3, target.y, target.z);
            var box4 = new Box(xRangeB4, target.y, target.z);
            
            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);
            boxes.Add(box4);
            return;
        }

        //PY
        if ( intersection.py && intersection.pz)
        {
            var xRangeB12 = intersecting.x;
            var xRangeB3 = NonCollidingNegative(target.x, intersecting.x);//new Range(target.x.min, intersecting.x.min - 1));
            var xRangeB4 = NonCollidingPositive(target.x, intersecting.x);//new Range(intersecting.x.max + 1, target.x.max));
            
            var yRangeB1 = NonCollidingNegative(target.y, intersecting.y);
            var yRangeB234 = target.y;

            var zRangeB1 = ExtendPositive(target.z, intersecting.z);
            var zRangeB2 = NonCollidingNegative(target.z, intersecting.z);
            var zRangeB34 = target.z;

            var box1 = new Box(xRangeB12, yRangeB1, zRangeB1);
            var box2 = new Box(xRangeB12, yRangeB234, zRangeB2);
            var box3 = new Box(xRangeB3, yRangeB234, zRangeB34);
            var box4 = new Box(xRangeB4, yRangeB234, zRangeB34);
            
            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);
            boxes.Add(box4);
            return;
        }
        
        if ( intersection.py && intersection.px)
        {
            var zRangeB12 = intersecting.z;
            var zRangeB3 = NonCollidingNegative(target.z, intersecting.z);
            var zRangeB4 = NonCollidingPositive(target.z, intersecting.z);
            
            var yRangeB1 = NonCollidingNegative(target.y, intersecting.y);
            var yRangeB234 = target.y;

            var xRangeB1 = ExtendPositive(target.x, intersecting.x);
            var xRangeB2 = NonCollidingNegative(target.x, intersecting.x);
            var xRangeB34 = target.x;

            var box1 = new Box(xRangeB1, yRangeB1, zRangeB12);
            var box2 = new Box(xRangeB2, yRangeB234, zRangeB12);
            var box3 = new Box(xRangeB34, yRangeB234, zRangeB3);
            var box4 = new Box(xRangeB34, yRangeB234, zRangeB4);
            
            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);
            boxes.Add(box4);
            return;
        }
        
        if ( intersection.py && intersection.nx)
        {
            var zRangeB12 = intersecting.z;
            var zRangeB3 = NonCollidingNegative(target.z, intersecting.z);
            var zRangeB4 = NonCollidingPositive(target.z, intersecting.z);
            
            var yRangeB1 = NonCollidingNegative(target.y, intersecting.y);
            var yRangeB234 = target.y;

            var xRangeB1 = ExtendNegative(target.x, intersecting.x);
            var xRangeB2 = NonCollidingPositive(target.x, intersecting.x);
            var xRangeB34 = target.x;

            var box1 = new Box(xRangeB1, yRangeB1, zRangeB12);
            var box2 = new Box(xRangeB2, yRangeB234, zRangeB12);
            var box3 = new Box(xRangeB34, yRangeB234, zRangeB3);
            var box4 = new Box(xRangeB34, yRangeB234, zRangeB4);
            
            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);
            boxes.Add(box4);
            return;
        }
        
        if (intersection.py && intersection.nz)
        {
            var xRangeB12 = intersecting.x;
            var xRangeB3 = NonCollidingNegative(target.x, intersecting.x);//new Range(target.x.min, intersecting.x.min - 1));
            var xRangeB4 = NonCollidingPositive(target.x, intersecting.x);//new Range(intersecting.x.max + 1, target.x.max));
            
            var yRangeB1 = NonCollidingNegative(target.y, intersecting.y);
            var yRangeB234 = target.y;

            var zRangeB1 = ExtendNegative(target.z, intersecting.z);
            var zRangeB2 = NonCollidingPositive(target.z, intersecting.z);
            var zRangeB34 = target.z;

            var box1 = new Box(xRangeB12, yRangeB1, zRangeB1);
            var box2 = new Box(xRangeB12, yRangeB234, zRangeB2);
            var box3 = new Box(xRangeB3, yRangeB234, zRangeB34);
            var box4 = new Box(xRangeB4, yRangeB234, zRangeB34);
            
            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);
            boxes.Add(box4);
            return;
        }
        
        //NY
        if ( intersection.ny && intersection.pz)
        {
            var xRangeB12 = intersecting.x;
            var xRangeB3 = NonCollidingNegative(target.x, intersecting.x);//new Range(target.x.min, intersecting.x.min - 1));
            var xRangeB4 = NonCollidingPositive(target.x, intersecting.x);//new Range(intersecting.x.max + 1, target.x.max));
            
            var yRangeB1 = NonCollidingPositive(target.y, intersecting.y);
            var yRangeB234 = target.y;

            var zRangeB1 = ExtendPositive(target.z, intersecting.z);
            var zRangeB2 = NonCollidingNegative(target.z, intersecting.z);
            var zRangeB34 = target.z;

            var box1 = new Box(xRangeB12, yRangeB1, zRangeB1);
            var box2 = new Box(xRangeB12, yRangeB234, zRangeB2);
            var box3 = new Box(xRangeB3, yRangeB234, zRangeB34);
            var box4 = new Box(xRangeB4, yRangeB234, zRangeB34);
            
            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);
            boxes.Add(box4);
            return;
        }
        
        if ( intersection.ny && intersection.px)
        {
            var zRangeB12 = intersecting.z;
            var zRangeB3 = NonCollidingNegative(target.z, intersecting.z);
            var zRangeB4 = NonCollidingPositive(target.z, intersecting.z);
            
            var yRangeB1 = NonCollidingPositive(target.y, intersecting.y);
            var yRangeB234 = target.y;

            var xRangeB1 = ExtendPositive(target.x, intersecting.x);
            var xRangeB2 = NonCollidingNegative(target.x, intersecting.x);
            var xRangeB34 = target.x;

            var box1 = new Box(xRangeB1, yRangeB1, zRangeB12);
            var box2 = new Box(xRangeB2, yRangeB234, zRangeB12);
            var box3 = new Box(xRangeB34, yRangeB234, zRangeB3);
            var box4 = new Box(xRangeB34, yRangeB234, zRangeB4);
            
            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);
            boxes.Add(box4);
            return;
        }
        
        if ( intersection.ny && intersection.nx)
        {
            var zRangeB12 = intersecting.z;
            var zRangeB3 = NonCollidingNegative(target.z, intersecting.z);
            var zRangeB4 = NonCollidingPositive(target.z, intersecting.z);
            
            var yRangeB1 = NonCollidingPositive(target.y, intersecting.y);
            var yRangeB234 = target.y;

            var xRangeB1 = ExtendNegative(target.x, intersecting.x);
            var xRangeB2 = NonCollidingPositive(target.x, intersecting.x);
            var xRangeB34 = target.x;

            var box1 = new Box(xRangeB1, yRangeB1, zRangeB12);
            var box2 = new Box(xRangeB2, yRangeB234, zRangeB12);
            var box3 = new Box(xRangeB34, yRangeB234, zRangeB3);
            var box4 = new Box(xRangeB34, yRangeB234, zRangeB4);
            
            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);
            boxes.Add(box4);
            return;
        }
        
        if (intersection.ny && intersection.nz)
        {
            var xRangeB12 = intersecting.x;
            var xRangeB3 = NonCollidingNegative(target.x, intersecting.x);//new Range(target.x.min, intersecting.x.min - 1));
            var xRangeB4 = NonCollidingPositive(target.x, intersecting.x);//new Range(intersecting.x.max + 1, target.x.max));
            
            var yRangeB1 = NonCollidingPositive(target.y, intersecting.y);
            var yRangeB234 = target.y;

            var zRangeB1 = ExtendNegative(target.z, intersecting.z);
            var zRangeB2 = NonCollidingPositive(target.z, intersecting.z);
            var zRangeB34 = target.z;

            var box1 = new Box(xRangeB12, yRangeB1, zRangeB1);
            var box2 = new Box(xRangeB12, yRangeB234, zRangeB2);
            var box3 = new Box(xRangeB3, yRangeB234, zRangeB34);
            var box4 = new Box(xRangeB4, yRangeB234, zRangeB34);
            
            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);
            boxes.Add(box4);
            return;
        }
        
        //NX
        if (intersection.nx && intersection.nz)
        {
            var yRangeB12 = intersecting.y;
            var yRangeB3 = NonCollidingNegative(target.y, intersecting.y);
            var yRangeB4 = NonCollidingPositive(target.y, intersecting.y);
            
            var zRangeB1 = NonCollidingPositive(target.z, intersecting.z);
            var zRangeB234 = target.z;

            var xRangeB1 = ExtendNegative(target.x, intersecting.x);
            var xRangeB2 = NonCollidingPositive(target.x, intersecting.x);
            var xRangeB34 = target.x;

            var box1 = new Box(xRangeB1, yRangeB12, zRangeB1);
            var box2 = new Box(xRangeB2, yRangeB12, zRangeB234);
            var box3 = new Box(xRangeB34, yRangeB3, zRangeB234);
            var box4 = new Box(xRangeB34, yRangeB4, zRangeB234);
            
            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);
            boxes.Add(box4);
            return;
        }
        
        if (intersection.nx && intersection.pz)
        {
            var yRangeB12 = intersecting.y;
            var yRangeB3 = NonCollidingNegative(target.y, intersecting.y);
            var yRangeB4 = NonCollidingPositive(target.y, intersecting.y);
            
            var zRangeB1 = NonCollidingNegative(target.z, intersecting.z);
            var zRangeB234 = target.z;

            var xRangeB1 = ExtendNegative(target.x, intersecting.x);
            var xRangeB2 = NonCollidingPositive(target.x, intersecting.x);
            var xRangeB34 = target.x;

            var box1 = new Box(xRangeB1, yRangeB12, zRangeB1);
            var box2 = new Box(xRangeB2, yRangeB12, zRangeB234);
            var box3 = new Box(xRangeB34, yRangeB3, zRangeB234);
            var box4 = new Box(xRangeB34, yRangeB4, zRangeB234);
            
            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);
            boxes.Add(box4);
            return;
        }
        
        //PX
        if (intersection.px && intersection.nz)
        {
            var yRangeB12 = intersecting.y;
            var yRangeB3 = NonCollidingNegative(target.y, intersecting.y);
            var yRangeB4 = NonCollidingPositive(target.y, intersecting.y);
            
            var zRangeB1 = NonCollidingPositive(target.z, intersecting.z);
            var zRangeB234 = target.z;

            var xRangeB1 = ExtendPositive(target.x, intersecting.x);
            var xRangeB2 = NonCollidingNegative(target.x, intersecting.x);
            var xRangeB34 = target.x;

            var box1 = new Box(xRangeB1, yRangeB12, zRangeB1);
            var box2 = new Box(xRangeB2, yRangeB12, zRangeB234);
            var box3 = new Box(xRangeB34, yRangeB3, zRangeB234);
            var box4 = new Box(xRangeB34, yRangeB4, zRangeB234);
            
            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);
            boxes.Add(box4);
            return;
        }
        
        if (intersection.px && intersection.pz)
        {
            var yRangeB12 = intersecting.y;
            var yRangeB3 = NonCollidingNegative(target.y, intersecting.y);
            var yRangeB4 = NonCollidingPositive(target.y, intersecting.y);
            
            var zRangeB1 = NonCollidingNegative(target.z, intersecting.z);
            var zRangeB234 = target.z;

            var xRangeB1 = ExtendPositive(target.x, intersecting.x);
            var xRangeB2 = NonCollidingNegative(target.x, intersecting.x);
            var xRangeB34 = target.x;

            var box1 = new Box(xRangeB1, yRangeB12, zRangeB1);
            var box2 = new Box(xRangeB2, yRangeB12, zRangeB234);
            var box3 = new Box(xRangeB34, yRangeB3, zRangeB234);
            var box4 = new Box(xRangeB34, yRangeB4, zRangeB234);
            
            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);
            boxes.Add(box4);
            return;
        }
    }
    
    Range ExtendPositive(Range target, Range intersecting) => new Range(intersecting.min, target.max);
    Range ExtendNegative(Range target, Range intersecting) => new Range(target.min, intersecting.max);
    Range NonCollidingPositive(Range target, Range intersecting) => new Range(intersecting.max + 1, target.max);
    Range NonCollidingNegative(Range target, Range intersecting) => new Range(target.min, intersecting.min - 1);
    
    void ThreeIntersections(ref List<Box> boxes, Box target, Box intersecting, Intersection intersection)
    {
        if (intersection.px && intersection.py && intersection.pz)
        {
            var xRangeB12 = new Range(intersecting.x.min, target.x.max);
            var xRangeB3 = new Range(target.x.min, intersecting.x.min - 1);

            var yRangeB1 = new Range(target.y.min, intersecting.y.min - 1);
            var yRangeB23 = target.y;

            var zRangeB1 = new Range(intersecting.z.min, target.z.max);
            var zRangeB2 = new Range(target.z.min, intersecting.z.min-1);
            var zRangeB3 = target.z;

            var box1 = new Box(xRangeB12, yRangeB1, zRangeB1);
            var box2 = new Box(xRangeB12, yRangeB23, zRangeB2);
            var box3 = new Box(xRangeB3, yRangeB23, zRangeB3);
            
            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);
            return;
        }
        
        if (intersection.px && intersection.py && intersection.nz)
        {
            var xRangeB12 = new Range(intersecting.x.min, target.x.max);
            var xRangeB3 = new Range(target.x.min, intersecting.x.min - 1);

            var yRangeB1 = new Range(target.y.min, intersecting.y.min - 1);
            var yRangeB23 = target.y;

            var zRangeB1 = new Range(target.z.min, intersecting.z.max);
            var zRangeB2 = new Range(intersecting.z.max + 1, target.z.max);
            var zRangeB3 = target.z;

            var box1 = new Box(xRangeB12, yRangeB1, zRangeB1);
            var box2 = new Box(xRangeB12, yRangeB23, zRangeB2);
            var box3 = new Box(xRangeB3, yRangeB23, zRangeB3);
            
            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);
            return;
        }
        
        if (intersection.px && intersection.ny && intersection.pz)
        {
            var xRangeB12 = new Range(intersecting.x.min, target.x.max);
            var xRangeB3 = new Range(target.x.min, intersecting.x.min - 1);

            var yRangeB1 = new Range(intersecting.y.max+1, target.y.max);
            var yRangeB23 = target.y;

            var zRangeB1 = new Range(intersecting.z.min, target.z.max);
            var zRangeB2 = new Range(target.z.min, intersecting.z.min-1);
            var zRangeB3 = target.z;

            var box1 = new Box(xRangeB12, yRangeB1, zRangeB1);
            var box2 = new Box(xRangeB12, yRangeB23, zRangeB2);
            var box3 = new Box(xRangeB3, yRangeB23, zRangeB3);
            
            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);
            return;
        }
        
        if (intersection.nx && intersection.py && intersection.pz)
        {
            var xRangeB12 = new Range(target.x.min, intersecting.x.max);
            var xRangeB3 = new Range(intersecting.x.max+1, target.x.max);

            var yRangeB1 = new Range(target.y.min, intersecting.y.min - 1);
            var yRangeB23 = target.y;

            var zRangeB1 = new Range(intersecting.z.min, target.z.max);
            var zRangeB2 = new Range(target.z.min, intersecting.z.min-1);
            var zRangeB3 = target.z;

            var box1 = new Box(xRangeB12, yRangeB1, zRangeB1);
            var box2 = new Box(xRangeB12, yRangeB23, zRangeB2);
            var box3 = new Box(xRangeB3, yRangeB23, zRangeB3);
            
            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);
            return;
        }

        if (intersection.px && intersection.ny && intersection.nz)
        {
            var xRangeB12 = new Range(intersecting.x.min, target.x.max);
            var xRangeB3 = new Range(target.x.min, intersecting.x.min - 1);

            var yRangeB1 = new Range(intersecting.y.max + 1, target.y.max);
            var yRangeB23 = target.y;

            var zRangeB1 = new Range(target.z.min, intersecting.z.max);
            var zRangeB2 = new Range(intersecting.z.max + 1, target.z.max);
            var zRangeB3 = target.z;

            var box1 = new Box(xRangeB12, yRangeB1, zRangeB1);
            var box2 = new Box(xRangeB12, yRangeB23, zRangeB2);
            var box3 = new Box(xRangeB3, yRangeB23, zRangeB3);
            
            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);
            return;
        }
        
        if (intersection.nx && intersection.py && intersection.nz)
        {
            var xRangeB12 = new Range(target.x.min, intersecting.x.max);
            var xRangeB3 = new Range(intersecting.x.max+1, target.x.max);

            var yRangeB1 = new Range(target.y.min, intersecting.y.min - 1);
            var yRangeB23 = target.y;

            var zRangeB1 = new Range(target.z.min, intersecting.z.max);
            var zRangeB2 = new Range(intersecting.z.max + 1, target.z.max);
            var zRangeB3 = target.z;

            var box1 = new Box(xRangeB12, yRangeB1, zRangeB1);
            var box2 = new Box(xRangeB12, yRangeB23, zRangeB2);
            var box3 = new Box(xRangeB3, yRangeB23, zRangeB3);
            
            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);
            return;
        }
        
        if (intersection.nx && intersection.ny && intersection.pz)
        {
            var xRangeB12 = new Range(target.x.min, intersecting.x.max);
            var xRangeB3 = new Range(intersecting.x.max+1, target.x.max);

            var yRangeB1 = new Range(intersecting.y.max+1, target.y.max);
            var yRangeB23 = target.y;

            var zRangeB1 = new Range(intersecting.z.min, target.z.max);
            var zRangeB2 = new Range(target.z.min, intersecting.z.min-1);
            var zRangeB3 = target.z;

            var box1 = new Box(xRangeB12, yRangeB1, zRangeB1);
            var box2 = new Box(xRangeB12, yRangeB23, zRangeB2);
            var box3 = new Box(xRangeB3, yRangeB23, zRangeB3);
            
            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);
            return;
        }
        
        if (intersection.nx && intersection.ny && intersection.nz)
        {
            var xRangeB12 = new Range(target.x.min, intersecting.x.max);
            var xRangeB3 = new Range(intersecting.x.max+1, target.x.max);

            var yRangeB1 = new Range(intersecting.y.max+1, target.y.max);
            var yRangeB23 = target.y;

            var zRangeB1 = new Range(target.z.min, intersecting.z.max);
            var zRangeB2 = new Range(intersecting.z.max + 1, target.z.max);
            var zRangeB3 = target.z;

            var box1 = new Box(xRangeB12, yRangeB1, zRangeB1);
            var box2 = new Box(xRangeB12, yRangeB23, zRangeB2);
            var box3 = new Box(xRangeB3, yRangeB23, zRangeB3);
            
            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);
            return;
        }
        
        //X Through

        if (intersection.px && intersection.nx && intersection.nz)
        {
            var yRangeB1 = intersecting.y;
            var yRangeB2 = NonCollidingPositive(target.y, intersecting.y);
            var yRangeB3 = NonCollidingNegative(target.y, intersecting.y);

            var zRangeB1 = NonCollidingPositive(target.z, intersecting.z);
            var zRangeB23 = target.z;

            var box1 = new Box(target.x, yRangeB1, zRangeB1);
            var box2 = new Box(target.x, yRangeB2, zRangeB23);
            var box3 = new Box(target.x, yRangeB3, zRangeB23);
            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);
            return;
        }
        
        if (intersection.px && intersection.nx && intersection.pz)
        {
            var yRangeB1 = intersecting.y;
            var yRangeB2 = NonCollidingPositive(target.y, intersecting.y);
            var yRangeB3 = NonCollidingNegative(target.y, intersecting.y);

            var zRangeB1 = NonCollidingNegative(target.z, intersecting.z);
            var zRangeB23 = target.z;

            var box1 = new Box(target.x, yRangeB1, zRangeB1);
            var box2 = new Box(target.x, yRangeB2, zRangeB23);
            var box3 = new Box(target.x, yRangeB3, zRangeB23);
            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);
            return;
        }

        if (intersection.px && intersection.nx && intersection.ny)
        {
            var zRangeB1 = intersecting.z;
            var zRangeB2 = NonCollidingPositive(target.z, intersecting.z);
            var zRangeB3 = NonCollidingNegative(target.z, intersecting.z);

            var yRangeB1 = NonCollidingPositive(target.y, intersecting.y);
            var yRangeB23 = target.y;

            var box1 = new Box(target.x, yRangeB1, zRangeB1);
            var box2 = new Box(target.x, yRangeB23, zRangeB2);
            var box3 = new Box(target.x, yRangeB23, zRangeB3);
            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);
            return;
        }
        
        if (intersection.px && intersection.nx && intersection.py)
        {
            var zRangeB1 = intersecting.z;
            var zRangeB2 = NonCollidingPositive(target.z, intersecting.z);
            var zRangeB3 = NonCollidingNegative(target.z, intersecting.z);

            var yRangeB1 = NonCollidingNegative(target.y, intersecting.y);
            var yRangeB23 = target.y;

            var box1 = new Box(target.x, yRangeB1, zRangeB1);
            var box2 = new Box(target.x, yRangeB23, zRangeB2);
            var box3 = new Box(target.x, yRangeB23, zRangeB3);
            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);
            return;
        }
        
        //Y Through
        if (intersection.py && intersection.ny && intersection.nz)
        {
            var xRangeB1 = intersecting.x;
            var xRangeB2 = NonCollidingPositive(target.x, intersecting.x);
            var xRangeB3 = NonCollidingNegative(target.x, intersecting.x);

            var zRangeB1 = NonCollidingPositive(target.z, intersecting.z);
            var zRangeB23 = target.z;

            var box1 = new Box(xRangeB1, target.y, zRangeB1);
            var box2 = new Box(xRangeB2, target.y, zRangeB23);
            var box3 = new Box(xRangeB3, target.y, zRangeB23);
            
            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);
            return;
        }
        
        if (intersection.py && intersection.ny && intersection.pz)
        {
            var xRangeB1 = intersecting.x;
            var xRangeB2 = NonCollidingPositive(target.x, intersecting.x);
            var xRangeB3 = NonCollidingNegative(target.x, intersecting.x);

            var zRangeB1 = NonCollidingNegative(target.z, intersecting.z);
            var zRangeB23 = target.z;

            var box1 = new Box(xRangeB1, target.y, zRangeB1);
            var box2 = new Box(xRangeB2, target.y, zRangeB23);
            var box3 = new Box(xRangeB3, target.y, zRangeB23);
            
            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);
            return;
        }
        
        if (intersection.py && intersection.ny && intersection.px)
        {
            var zRangeB1 = intersecting.z;
            var zRangeB2 = NonCollidingPositive(target.z, intersecting.z);
            var zRangeB3 = NonCollidingNegative(target.z, intersecting.z);

            var xRangeB1 = NonCollidingNegative(target.z, intersecting.z);
            var xRangeB23 = target.x;

            var box1 = new Box(xRangeB1, target.y, zRangeB1);
            var box2 = new Box(xRangeB23, target.y, zRangeB2);
            var box3 = new Box(xRangeB23, target.y, zRangeB3);
            
            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);
            return;
        }
        
        if (intersection.py && intersection.ny && intersection.nx)
        {
            var zRangeB1 = intersecting.z;
            var zRangeB2 = NonCollidingPositive(target.z, intersecting.z);
            var zRangeB3 = NonCollidingNegative(target.z, intersecting.z);

            var xRangeB1 = NonCollidingPositive(target.z, intersecting.z);
            var xRangeB23 = target.x;

            var box1 = new Box(xRangeB1, target.y, zRangeB1);
            var box2 = new Box(xRangeB23, target.y, zRangeB2);
            var box3 = new Box(xRangeB23, target.y, zRangeB3);
            
            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);
            return;
        }
        
        //Z Through
        if (intersection.pz && intersection.nz && intersection.px)
        {
            var yRangeB1 = intersecting.y;
            var yRangeB2 = NonCollidingPositive(target.y, intersecting.y);
            var yRangeB3 = NonCollidingNegative(target.y, intersecting.y);

            var xRangeB1 = NonCollidingNegative(target.z, intersecting.z);
            var xRangeB23 = target.x;

            var box1 = new Box(xRangeB1, yRangeB1, target.z);
            var box2 = new Box(xRangeB23, yRangeB2, target.z);
            var box3 = new Box(xRangeB23, yRangeB3, target.z);
            
            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);
            return;
        }        
        
        if (intersection.pz && intersection.nz && intersection.nx)
        {
            var yRangeB1 = intersecting.y;
            var yRangeB2 = NonCollidingPositive(target.y, intersecting.y);
            var yRangeB3 = NonCollidingNegative(target.y, intersecting.y);

            var xRangeB1 = NonCollidingPositive(target.z, intersecting.z);
            var xRangeB23 = target.x;

            var box1 = new Box(xRangeB1, yRangeB1, target.z);
            var box2 = new Box(xRangeB23, yRangeB2, target.z);
            var box3 = new Box(xRangeB23, yRangeB3, target.z);
            
            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);
            return;
        }
        
        if (intersection.pz && intersection.nz && intersection.ny)
        {
            var xRangeB1 = intersecting.x;
            var xRangeB2 = NonCollidingPositive(target.x, intersecting.x);
            var xRangeB3 = NonCollidingNegative(target.x, intersecting.x);

            var yRangeB1 = NonCollidingPositive(target.z, intersecting.z);
            var yRangeB23 = target.x;

            var box1 = new Box(xRangeB1, yRangeB1, target.z);
            var box2 = new Box(xRangeB2, yRangeB23, target.z);
            var box3 = new Box(xRangeB3, yRangeB23, target.z);
            
            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);
            return;
        }
        
        if (intersection.pz && intersection.nz && intersection.py)
        {
            var xRangeB1 = intersecting.x;
            var xRangeB2 = NonCollidingPositive(target.x, intersecting.x);
            var xRangeB3 = NonCollidingNegative(target.x, intersecting.x);

            var yRangeB1 = NonCollidingNegative(target.z, intersecting.z);
            var yRangeB23 = target.x;

            var box1 = new Box(xRangeB1, yRangeB1, target.z);
            var box2 = new Box(xRangeB2, yRangeB23, target.z);
            var box3 = new Box(xRangeB3, yRangeB23, target.z);
            
            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);
            return;
        }
    }

    void FourIntersections(ref List<Box> boxes, Box target, Box intersecting, Intersection intersection)
    {
        //cut through
        if (!intersection.nx && !intersection.px)
        {
            var left = new Range(target.x.min, intersecting.x.min - 1);
            var right = new Range( intersecting.x.max + 1, target.x.max);
            var leftBox = new Box(left, target.y, target.z);
            var rightBox = new Box(right, target.y, target.z);
            boxes.Add(leftBox);
            boxes.Add(rightBox);
        }
        if (!intersection.nz && !intersection.pz)
        {
            var bwd = new Range(target.z.min, intersecting.z.min - 1);
            var fwd = new Range( intersecting.z.max + 1, target.z.max);
            var bwdBox = new Box(target.x, target.y, bwd);
            var fwdBox = new Box(target.x, target.y, fwd);
            boxes.Add(bwdBox);
            boxes.Add(fwdBox);
        }
        if (!intersection.ny && !intersection.py)
        {
            var down = new Range(target.y.min, intersecting.y.min - 1);
            var up = new Range( intersecting.y.max + 1, target.y.max);
            var downBox = new Box(target.x, down, target.z);
            var upBox = new Box(target.x, up, target.z);
            boxes.Add(downBox);
            boxes.Add(upBox);
        }
        
        //edge cases
        //NY
        if (!intersection.px && !intersection.py)
        {
            var xRangeB1 = new Range(intersecting.x.max+1, target.x.max);
            var xRangeB2 = target.x;

            var yRangeB1 = new Range(target.y.min, intersecting.y.max);
            var yRangeB2 = new Range(intersecting.y.max + 1, target.y.max);

            var box1 = new Box(xRangeB1, yRangeB1, target.z);
            var box2 = new Box(xRangeB2, yRangeB2, target.z);
            boxes.Add(box1);
            boxes.Add(box2);
        }
        if (!intersection.pz && !intersection.py)
        {
            var zRangeB1 = new Range(intersecting.z.max+1, target.z.max);
            var zRangeB2 = target.z;

            var yRangeB1 = new Range(target.y.min, intersecting.y.max);
            var yRangeB2 = new Range(intersecting.y.max + 1, target.y.max);

            var box1 = new Box(target.x, yRangeB1, zRangeB1);
            var box2 = new Box(target.x, yRangeB2, zRangeB2);
            boxes.Add(box1);
            boxes.Add(box2);
        }
        if (!intersection.nx && !intersection.py)
        {
            var xRangeB1 = new Range(target.x.min, intersecting.x.min-1);
            var xRangeB2 = target.x;

            var yRangeB1 = new Range(target.y.min, intersecting.y.max);
            var yRangeB2 = new Range(intersecting.y.max + 1, target.y.max);

            var box1 = new Box(xRangeB1, yRangeB1, target.z);
            var box2 = new Box(xRangeB2, yRangeB2, target.z);
            boxes.Add(box1);
            boxes.Add(box2);
            return;
        }
        if (!intersection.nz && !intersection.py)
        {
            var zRangeB1 = new Range(target.z.min, intersecting.z.min-1);
            var zRangeB2 = target.z;

            var yRangeB1 = new Range(target.y.min, intersecting.y.max);
            var yRangeB2 = new Range(intersecting.y.max + 1, target.y.max);

            var box1 = new Box(target.x, yRangeB1, zRangeB1);
            var box2 = new Box(target.x, yRangeB2, zRangeB2);
            boxes.Add(box1);
            boxes.Add(box2);
            return;
        }
        
        //PY
        if (!intersection.nz && !intersection.ny)
        {
            var zRangeB1 = new Range(target.z.min, intersecting.z.min-1);
            var zRangeB2 = target.z;

            var yRangeB1 = new Range(intersecting.y.min, target.y.max);
            var yRangeB2 = new Range(target.y.min, intersecting.y.min-1);

            var box1 = new Box(target.x, yRangeB1, zRangeB1);
            var box2 = new Box(target.x, yRangeB2, zRangeB2);
            boxes.Add(box1);
            boxes.Add(box2);
            return;
        }
        if (!intersection.nx && !intersection.ny)
        {
            var xRangeB1 = new Range(target.x.min, intersecting.x.min-1);
            var xRangeB2 = target.x;

            var yRangeB1 = new Range(intersecting.y.min, target.y.max);
            var yRangeB2 = new Range(target.y.min, intersecting.y.min - 1);

            var box1 = new Box(xRangeB1, yRangeB1, target.z);
            var box2 = new Box(xRangeB2, yRangeB2, target.z);
            boxes.Add(box1);
            boxes.Add(box2);
        }
        if (!intersection.px && !intersection.ny)
        {
            var xRangeB1 = new Range(intersecting.x.max+1, target.x.max);
            var xRangeB2 = target.x;

            var yRangeB1 = new Range(intersecting.y.min, target.y.max);
            var yRangeB2 = new Range(target.y.min, intersecting.y.min-1);

            var box1 = new Box(xRangeB1, yRangeB1, target.z);
            var box2 = new Box(xRangeB2, yRangeB2, target.z);
            boxes.Add(box1);
            boxes.Add(box2);
            return;
        }
        if (!intersection.pz && !intersection.ny)
        {
            var zRangeB1 = new Range(intersecting.z.max+1, target.z.max);
            var zRangeB2 = target.z;

            var yRangeB1 = new Range(intersecting.y.min, target.y.max);
            var yRangeB2 = new Range(target.y.min, intersecting.y.min-1);

            var box1 = new Box(target.x, yRangeB1, zRangeB1);
            var box2 = new Box(target.x, yRangeB2, zRangeB2);
            boxes.Add(box1);
            boxes.Add(box2);
            return;
        }
        //NX
        if (!intersection.nz && !intersection.nx)
        {
            var xRangeB1 = new Range(target.x.min, intersecting.x.min - 1);
            var xRangeB2 = target.x;

            var zRangeB1 = new Range(intersecting.z.min, target.z.max);
            var zRangeB2 = new Range(target.z.min, intersecting.z.min - 1);

            var box1 = new Box(xRangeB1, target.y, zRangeB1);
            var box2 = new Box(xRangeB2, target.y, zRangeB2);

            boxes.Add(box1);
            boxes.Add(box2);
            return;
        }
        if (!intersection.pz && !intersection.nx)
        {
            var xRangeB1 = new Range(target.x.min, intersecting.x.min - 1);
            var xRangeB2 = target.x;

            var zRangeB1 = new Range(target.z.min, intersecting.z.max);
            var zRangeB2 = new Range(intersecting.z.max + 1, target.z.max);

            var box1 = new Box(xRangeB1, target.y, zRangeB1);
            var box2 = new Box(xRangeB2, target.y, zRangeB2);

            boxes.Add(box1);
            boxes.Add(box2);
            return;
        }
        //PX
        if (!intersection.nz && !intersection.px)
        {
            var xRangeB1 = new Range(intersecting.x.max+1, target.x.max);
            var xRangeB2 = target.x;

            var zRangeB1 = new Range(intersecting.z.min, target.z.max);
            var zRangeB2 = new Range(target.z.min, intersecting.z.min - 1);

            var box1 = new Box(xRangeB1, target.y, zRangeB1);
            var box2 = new Box(xRangeB2, target.y, zRangeB2);

            boxes.Add(box1);
            boxes.Add(box2);
            return;
        }
        if (!intersection.pz && !intersection.px)
        {
            var xRangeB1 = new Range(intersecting.x.max + 1, target.x.max);
            var xRangeB2 = target.x;

            var zRangeB1 = new Range(target.z.min, intersecting.z.max);
            var zRangeB2 = new Range(intersecting.z.max + 1, target.z.max);

            var box1 = new Box(xRangeB1, target.y, zRangeB1);
            var box2 = new Box(xRangeB2, target.y, zRangeB2);

            boxes.Add(box1);
            boxes.Add(box2);
            return;
        }
    }

    void FiveIntersections(ref List<Box> boxes, Box target, Box intersecting, Intersection intersection)
    {
        if (!intersection.nx && intersection.Count() == 5)
        {
            var xRange = new Range(target.x.min, intersecting.x.min - 1);
            boxes.Add(new Box(xRange, target.y, target.z));
        }
        if (!intersection.px && intersection.Count() == 5)
        {
            var xRange = new Range(intersecting.x.max + 1, target.x.max);
            boxes.Add(new Box(xRange, target.y, target.z));
        }
        if (!intersection.ny && intersection.Count() == 5)
        {
            var yRange = new Range(target.y.min, intersecting.y.min - 1);
            boxes.Add(new Box(target.x, yRange, target.z));
        }
        if (!intersection.py && intersection.Count() == 5)
        {
            var yRange = new Range(intersecting.y.max + 1, target.y.max);
            boxes.Add(new Box(target.x, yRange, target.z));
        }
        if (!intersection.nz && intersection.Count() == 5)
        {
            var zRange = new Range(target.z.min, intersecting.z.min - 1);
            boxes.Add(new Box(target.x, target.y, zRange));
        }
        if (!intersection.pz && intersection.Count() == 5)
        {
            var zRange = new Range(intersecting.z.max + 1, target.z.max);
            boxes.Add(new Box(target.x, target.y, zRange));
        }
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

        if (boxToSplit.x.min >= incomingBox.x.min)
        {
            intersection.nx = true;
        }
        
        if (boxToSplit.x.max <= incomingBox.x.max)
        {
            intersection.px = true;
        }

        if (boxToSplit.y.min >= incomingBox.y.min)
        {
            intersection.ny = true;
        }
        
        if (boxToSplit.y.max <= incomingBox.y.max)
        {
            intersection.py = true;
        }
        
        if (boxToSplit.z.min >= incomingBox.z.min)
        {
            intersection.nz = true;
        }
        
        if (boxToSplit.z.max <= incomingBox.z.max)
        {
            intersection.pz = true;
        }
        
        return intersection;
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