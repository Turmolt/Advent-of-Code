using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Solutions;
using static G8S;
public class Day21 : IChallenge
{
    public record GameState
    {
        public int Player1Score;
        public int Player1Position;
        public int Player2Score;
        public int Player2Position;
        public Turn Current;

        public enum Turn : byte
        {
            Player1,
            Player2
        }

        public GameState(GameState template)
        {
            Player1Score = template.Player1Score;
            Player1Position = template.Player1Position;
            Player2Score = template.Player2Score;
            Player2Position = template.Player2Position;
            Current = template.Current == Turn.Player1 ? Turn.Player2 : Turn.Player1;
        }

        public GameState(int p1Score, int p1Position, int p2Score, int p2Position, Turn current, int lastResult)
        {
            Player1Score = p1Score;
            Player1Position = p1Position;
            Player2Score = p2Score;
            Player2Position = p2Position;
            Current = current;
        }

        public GameState MovePlayer(int number)
        {
            var next = new GameState(this)
            {
            };
            switch (Current)
            {
                case Turn.Player1:
                {
                    next.Player1Position = Player1Position + number;
                    if (next.Player1Position > 10) next.Player1Position %= 10;
                    if (next.Player1Position == 0) next.Player1Position = 10;
                    next.Player1Score = Player1Score + next.Player1Position;
                    next.Current = Turn.Player2;
                    return next;
                }
                case Turn.Player2:
                {
                    next.Player2Position = Player2Position + number;
                    if (next.Player2Position > 10) next.Player2Position %= 10;
                    if (next.Player2Position == 0) next.Player2Position = 10;
                    next.Player2Score = Player2Score + next.Player2Position;
                    next.Current = Turn.Player1;
                    return next;
                }
                default:
                    return null;
            }
        }

        public override string ToString()
        {
            return $"Turn: {Current}\n"+
                   "P1: (S: {Player1Score}, P: {Player1Position})\n"+ 
                   "P2: (S: {Player2Score}, P: {Player2Position})";
        }
    }

    private Dictionary<GameState, (ulong player1Wins, ulong player2Wins)> cache;
    
    public void Solve(string[] data)
    {
        var p1Start = int.Parse($"{data[0][^1]}");
        var p2Start = int.Parse($"{data[1][^1]}");
        cache = new Dictionary<GameState, (ulong,ulong)>();
        SolvePartOne(p1Start, p2Start);
        StartRecursiveGame(p1Start, p2Start);
    }

    void StartRecursiveGame(int p1Start, int p2Start)
    {
        Log($"Starting recursive game:\n"+
                $"P1Start: {p1Start}\n"+
                $"P2Start: {p2Start}");
        var state = new GameState(0, p1Start, 0, p2Start, GameState.Turn.Player1,0);
        var results = CheckAllPossible(state);
        Log($"Player 1 Wins: {results.p1Wins} times\n"+
                $"Player 2 Wins: {results.p2Wins} times\n"+
                $"Part 2 Answer: {Math.Max(results.p1Wins, results.p2Wins)}");
    }

    (ulong, ulong) RecursiveStep(GameState last, int diceResult)
    {
        var current = last.MovePlayer(diceResult);
        if (cache.ContainsKey(current)) return cache[current];
        if (current.Player1Score >= 21)
        {
            return (1UL, 0UL);
        }

        if (current.Player2Score >= 21)
        {
            return (0UL, 1UL);
        }
        
        //check all the possible next steps and then store the sum of them in the dictionary for this step
        var result = CheckAllPossible(current);
        cache[current] = result;
        return result;
    }

    (ulong p1Wins, ulong p2Wins) CheckAllPossible(GameState state)
    {
        (ulong p1Wins, ulong p2Wins) result = (0UL, 0UL);
        
        for (int dice = 3; dice <= 9; dice++)
        {
            var (p1Wins, p2Wins) = RecursiveStep(state, dice);
            result = (result.p1Wins + p1Wins * GetPermutations(dice),
                      result.p2Wins + p2Wins * GetPermutations(dice));
        }

        return result;
    }
    
    ulong GetPermutations(int number)
    {
        return number switch
        {
            3 => 1,
            4 => 3,
            5 => 6,
            6 => 7,
            7 => 6,
            8 => 3,
            9 => 1,
            _ => 0
        };
    }

    void SolvePartOne(int p1Start, int p2Start)
    {
        int dice = 1;
        int count = 0;

        int p1Score = 0;
        int p2Score = 0;
        
        while (p1Score < 1000 && p2Score < 1000)
        {
            p1Score += RollDice(ref p1Start, ref dice, ref count);
            if (p1Score >= 1000) break;
            p2Score += RollDice(ref p2Start, ref dice, ref count);
        }
        Log($"Rolls: {count}, P1: {p1Score}, P2: {p2Score}");
        Log($"Part 1 Answer: {Math.Min(p1Score,p2Score) * count}");
    }

    int RollDice(ref int position, ref int dice, ref int count)
    {
        var ret = dice;
        IncreaseDice(ref dice);
        count++;
        ret += dice;
        IncreaseDice(ref dice);
        count++;
        ret += dice;
        IncreaseDice(ref dice);
        count++;
        position += ret;
        if (position > 10) position %= 10;
        if (position == 0) position = 10;
//        Log($"{ret} and scores {position}");
        return position;
    }
    
    void IncreaseDice(ref int dice)
    {
        dice++;
        if (dice > 100) dice %= 100;
    }
}