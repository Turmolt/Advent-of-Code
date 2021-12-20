using System;

using static Advent_of_Code.G8S;

namespace Advent_of_Code.Solutions;

public class Day18 : IChallenge
{
    private SnailNumber[] homeworkInput;
    public void Solve(string[] data)
    {
        ParseData(data);

        long maxMagnitude = 0;
            
        for (int s = 0; s < homeworkInput.Length; s++)
        {
            var target = homeworkInput[s];
            for (int i = 0; i < homeworkInput.Length; i++)
            {
                if (i == s) continue;
                var sum = target + homeworkInput[i];
                while (sum.AnyNeedsReduce)
                {
                    ReduceNumber(ref sum);
                }

                var magnitude = sum.Magnitude();
                if (magnitude > maxMagnitude)
                {
                    maxMagnitude = magnitude;
                }
                    
                sum = homeworkInput[i] + target;
                while (sum.AnyNeedsReduce)
                {
                    ReduceNumber(ref sum);
                }

                magnitude = sum.Magnitude();
                if (magnitude > maxMagnitude)
                {
                    maxMagnitude = magnitude;
                }
            }
        }
            
        Console.WriteLine($"Max magnitude: {maxMagnitude}");
    }

    void ReduceNumber(ref SnailNumber number)
    {
        if (!number.AnyNeedsReduce) return;
        if (number.AnyNeedsExplode)
        {
            var target = number.FirstNeedsExplode;
            target.Explode();
        }
        else
        {
            var target = number.FirstNeedsReducing;
            target.Split();
        }
    }

    void ParseData(string[] data)
    {
        homeworkInput = new SnailNumber[data.Length];

        for (int i = 0; i < data.Length; i++)
        {
            homeworkInput[i] = new SnailNumber(data[i]);
        }
    }
}

class SnailNumber
{
    public enum Type
    {
        Value,
        Pair
    }

    public long Value;
    public SnailNumber Left;
    public SnailNumber Right;
    public SnailNumber Parent;
    public Type type;
        
    public int Depth => Parent == null ? 1 : Parent.Depth + 1;
    public bool NeedsExplode => Depth == 5 && type == Type.Pair;
    public bool AnyNeedsExplode
    {
        get
        {
            if (NeedsExplode) return true;
            if (Left is { AnyNeedsExplode: true }) return true;
            if (Right is { AnyNeedsExplode: true }) return true;
            return false;
        }
    }
    public SnailNumber FirstNeedsExplode
    {
        get
        {
            if (NeedsExplode) return this;
            if (Left.AnyNeedsExplode) return Left.FirstNeedsExplode;
            if (Right.AnyNeedsExplode) return Right.FirstNeedsExplode;
                
            return null;
        }
    }
    public bool NeedsSplit => type == Type.Value && Value > 9;
    public bool NeedsReducing => NeedsExplode || NeedsSplit;
    public bool AnyNeedsReduce
    {
        get
        {
            if (NeedsReducing) return true;
            if (Left is { AnyNeedsReduce: true }) return true;
            if (Right is { AnyNeedsReduce: true }) return true;
            return false;
        }
    }
    public SnailNumber FirstNeedsReducing
    {
        get
        {
            if (NeedsReducing) return this;
            if (Left.AnyNeedsReduce) return Left.FirstNeedsReducing;
            if (Right.AnyNeedsReduce) return Right.FirstNeedsReducing;
            return null;
        }
    }

    public SnailNumber(string input, SnailNumber parent = null)
    {
        Parent = parent;
            
        if (input[0] != '[')
        {
            var tokens = input.Split(',', 2);
            Value = Int64.Parse(tokens[0]);
            type = Type.Value;
            return;
        }

        type = Type.Pair;

        (int closingIndex, int splitIndex) = FindDetails(input);

        var leftTemplate = input.Substring(1, splitIndex - 1);
        var rightTemplate = input.Substring(splitIndex + 1)[..^1];
        Left = new SnailNumber(leftTemplate, this);
        Right = new SnailNumber(rightTemplate, this);
    }

    public SnailNumber(SnailNumber left, SnailNumber right, SnailNumber parent)
    {
        type = Type.Pair;
        Left = new SnailNumber(left, this);
        Right = new SnailNumber(right, this);
        Parent = parent;
    }

    public SnailNumber(long value, SnailNumber parent = null)
    {
        type = Type.Value;
        Value = value;
        Parent = parent;
    }

    public SnailNumber(SnailNumber template, SnailNumber parent)
    {
        type = template.type;
        Left = template.Left != null ? new SnailNumber(template.Left, this) : null;
        Right = template.Right!=null ? new SnailNumber(template.Right, this) : null;
        Value = template.Value;
        Parent = parent;
    }

    SnailNumber FindFirstToRight()
    {
        if (Parent == null) return null;
        if (Parent.Right == this)
        {
            return Parent.FindFirstToRight();
        }

        if (Parent.Right.type == Type.Value) return Parent.Right;
        if (Parent.Right.type == Type.Pair) return Parent.Right.FirstLeftValue();
        return null;
    }
        
    SnailNumber FindFirstToLeft()
    {
        if (Parent == null) return null;
        if (Parent.Left == this)
        {
            return Parent.FindFirstToLeft();
        }

        if (Parent.Left.type == Type.Value) return Parent.Left;
        if (Parent.Left.type == Type.Pair) return Parent.Left.FirstRightValue();
        return null;
    }

    SnailNumber FirstLeftValue()
    {
        if (type == Type.Value) return this;
        return Left.FirstLeftValue();
    }
        
    SnailNumber FirstRightValue()
    {
        if (type == Type.Value) return this;
        return Right.FirstRightValue();
    }
        

    public void Explode()
    {
        type = Type.Value;
        var firstRight = FindFirstToRight();
        if (firstRight != null)
        {
            firstRight.Value += Right.Value;
        }

        var firstLeft = FindFirstToLeft();
        if (firstLeft != null)
        {
            firstLeft.Value += Left.Value;
        }

        Value = 0;
        Right = null;
        Left = null;
    }

    public void Split()
    {
        type = Type.Pair;
        var oddVal = (Value % 2) == 0 ? 0 : 1;
        var leftNumber = Value / 2;
        var rightNumber = Value / 2 + oddVal;
        Left = new SnailNumber(leftNumber, this);
        Right = new SnailNumber(rightNumber, this);
    }

    public static SnailNumber operator +(SnailNumber a, SnailNumber b) => new SnailNumber(a, b, null);

    public long Magnitude()
    {
        if (type == Type.Value) return Value;
        return (3 * Left.Magnitude()) + (2 * Right.Magnitude());
    }

    (int,int) FindDetails(string snailNumberString)
    {
        int closingIndex = 1;
        int openedSnailNumbers = 1;
        int splitIndex = 0;
        while (openedSnailNumbers > 0)
        {
            if (snailNumberString[closingIndex] == ']') openedSnailNumbers--;
            if (snailNumberString[closingIndex] == '[') openedSnailNumbers++;
            if (snailNumberString[closingIndex] == ',' && openedSnailNumbers == 1) splitIndex = closingIndex;
            if (openedSnailNumbers > 0) closingIndex++;
        }
        return (closingIndex, splitIndex);
    }

    public override string ToString()
    {
        if (type == Type.Value)
        {
            return $"{Value}";
        }

        return $"[{Left},{Right}]";
    }
}