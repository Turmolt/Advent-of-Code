using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code
{
    public class Day10 : IChallenge
    {    
        Dictionary<char, int> errorScore = new Dictionary<char, int>
        {
            {')',3},
            {']',57},
            {'}',1197},
            {'>',25137}
        };
        
        Dictionary<char, char> complimentaryBracket = new Dictionary<char, char>
        {
            {'(',')'},
            {'[',']'},
            {'{','}'},
            {'<','>'}
        };
        
        Dictionary<char, int> autocompleteScore = new Dictionary<char, int>
        {
            {')', 1},
            {']', 2},
            {'}', 3},
            {'>', 4}
        };
        
        char[] openingBracket = {'(', '[', '{', '<'};

        public void Solve(string[] data)
        {
            int syntaxErrorSum = 0;
            foreach (var line in data)
            {
                syntaxErrorSum += AddLineErrors(line);
            }
            
            Console.WriteLine($"The syntax error sum is: {syntaxErrorSum}");

            var incompleteLines = data.Where(x => AddLineErrors(x) <= 0).ToArray();

            var scores = new List<ulong>();
            foreach (var line in incompleteLines)
            {
                scores.Add(CalculateAutocompleteScore(line));
            }

            scores.Sort();

            int mid = scores.Count / 2;

            Console.WriteLine($"The middle score is {scores[mid]}");
        }

        ulong CalculateAutocompleteScore(string line)
        {
            var syntaxStack = new Stack<char>();
            foreach (var c in line)
            {
                
                if (openingBracket.Contains(c))
                {
                    syntaxStack.Push(c);
                }
                else
                {
                    var next = syntaxStack.Peek();
                    if (c == complimentaryBracket[next])
                        syntaxStack.Pop();
                }
            }

            ulong score = 0;
            
            while (syntaxStack.Count > 0)
            {
                score *= 5;
                var nextBracket = complimentaryBracket[syntaxStack.Pop()];
                score += (ulong)autocompleteScore[nextBracket];
            }

            return score;
        }

        int AddLineErrors(string line)
        {
            var syntaxStack = new Stack<char>();
            foreach (var c in line)
            {
                if (openingBracket.Contains(c))
                {
                    syntaxStack.Push(c);
                }
                else
                {
                    var match = syntaxStack.Pop();
                    if (c != complimentaryBracket[match])
                    {
                        var score = errorScore[c];
                        return score;
                    }
                }
            }

            return 0;
        }
    }
}