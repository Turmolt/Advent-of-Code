using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Solutions
{
    public class Day13 : IChallenge
    {
        private Dictionary<(int x, int y), char> charMap = new Dictionary<(int, int), char>();
        private ((int x, int y) lower, (int x, int y) upper) bounds = ((int.MaxValue, int.MaxValue), (int.MinValue, int.MinValue));
        private List<(int x, int y)> foldInstructions = new List<(int, int)>();
        
        public void Solve(string[] data)
        {
            ParseData(data);
            
             foreach (var fold in foldInstructions)
             {
                 Fold(fold.x, fold.y);
             }
             
            Console.WriteLine(charMap.Count);
            
            Console.WriteLine($"{bounds.lower.x},{bounds.upper.y} to {bounds.upper.x},{bounds.upper.y}");
            PrintBoard();

        }

        void Fold(int x, int y)
        {
            var foldingY = y != 0;
            if (foldingY)
            {
                for (int dy = 0; dy <= (bounds.upper.y - y); dy++)
                {
                    for (int tx = bounds.lower.x; tx <= bounds.upper.x; tx++)
                    {
                        if(charMap.ContainsKey((tx, y + dy)))
                        {
                            if(!charMap.ContainsKey((tx, y - dy)))
                                charMap.Add((tx, y - dy),'#');
                            
                            charMap.Remove((tx, y + dy));
                        }
                    }
                }
                
                bounds.upper.y = y-1;
            }
            else
            {
                for (int ty = bounds.lower.y; ty <= bounds.upper.y; ty++)
                {
                    for (int dx = 0; dx <= (bounds.upper.x - x); dx++)
                    {
                        if(charMap.ContainsKey((x + dx, ty)))
                        {
                            if(!charMap.ContainsKey((x - dx, ty)))
                                charMap.Add((x - dx, ty),'#');
                            
                            charMap.Remove((x + dx, ty));
                        }
                    }
                }
                
                bounds.upper.x = x-1;
            }
        }
        
        void PrintBoard()
        {
            for (int y = bounds.lower.y; y <= bounds.upper.y; y++)
            {
                for (int x = bounds.lower.x; x <= bounds.upper.x; x++)
                {
                    Console.Write(charMap.ContainsKey((x, y)) ? "#" : ".");
                }
                Console.WriteLine();
            }
        }

        void ParseData(string[] data)
        {
            bool hasFinishedParsingPoints = false;
            foreach (var line in data)
            {
                if (line.Trim() == string.Empty && !hasFinishedParsingPoints)
                {
                    hasFinishedParsingPoints = true;
                    continue;
                }

                if (!hasFinishedParsingPoints)
                {
                    var tokens = line.Split(',');
                    var x = int.Parse(tokens[0]);
                    var y = int.Parse(tokens[1]);
                    charMap[(x, y)] = '#';
                    CheckBounds(x, y);
                }
                else
                {
                    var trimmedLine = line.Replace("fold along ", "");
                    var tokens = trimmedLine.Split('=');
                    if(tokens[0]=="x") foldInstructions.Add((int.Parse(tokens[1]), 0));
                    else foldInstructions.Add((0, int.Parse(tokens[1])));
                }
            }
        }

        void CheckBounds(int x, int y)
        {
            if (x < bounds.lower.x) bounds.lower.x = x;
            if (x > bounds.upper.x) bounds.upper.x = x;
            if (y < bounds.lower.y) bounds.lower.y = y;
            if (y > bounds.upper.y) bounds.upper.y = y;
        }
    }
}