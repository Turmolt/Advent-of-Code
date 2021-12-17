using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Solutions
{
    public class Day17 : IChallenge
    {
        private int minX;
        private int maxX;
        private int minY;
        private int maxY;

        public void Solve(string[] data)
        {
            ParseData(data[0]);

            var velocityResults = new List<((int x, int y) velocity, int highest)>();

            const int maxVelocityToTest = 1000;

            for (var y = minY; y <= maxVelocityToTest; y++)
            {
                for (var x = 0; x <= maxVelocityToTest; x++)
                {
                    var velocity = (x, y);
                    (int highest, bool hit) = TryVelocity(velocity.x, velocity.y);

                    if (hit)
                    {
                        velocityResults.Add((velocity, highest));
                    }
                }
            }
            
            if (velocityResults.Count > 0)
            {
                Console.WriteLine($"There are {velocityResults.Count} total velocities in a {maxVelocityToTest} square range");
                var coolestVelocity = velocityResults.OrderByDescending(x => x.highest).First();

                Console.WriteLine($"The coolest velocity you found was {coolestVelocity.velocity} "+
                                  $"and it reached a height of {coolestVelocity.highest}");
            }
            else
            {
                Console.WriteLine("No good velocity found, try increasing bounds of search");
            }
        }

        (int, bool) TryVelocity(int x, int y)
        {
            (int x, int y) velocity = (x, y);
            (int x, int y) position = (0, 0);
            
            int highest = 0;

            while (true)
            {
                Step(ref position, ref velocity);
                
                highest = Math.Max(highest, position.y);

                if ((position.x >= minX && position.x <= maxX) && 
                    (position.y >= minY && position.y <= maxY))
                    return (highest, true);
                
                if (position.x < minX && velocity.x <= 0)
                    return (highest, false);
                
                if (position.x > maxX && velocity.x >= 0) 
                    return (highest, false);
                
                if (position.y < minY && velocity.y < 0 &&
                    position.x >=minX && position.x <= maxX) 
                    return (highest, false);
            }
        }

        void Step(ref (int x, int y) position, ref (int x, int y) velocity)
        {
            position.x += velocity.x;
            position.y += velocity.y;
            if(velocity.x != 0)
                velocity.x += (velocity.x < 0) ? 1 : -1;
            velocity.y -= 1;
        }

        void ParseData(string data)
        {
            var line = data.Substring(13);
            var tokens = line.Split(", ");
            var xTokens = tokens[0].Substring(2).Split("..");
            var yTokens = tokens[1].Substring(2).Split("..");
            minX = int.Parse(xTokens[0]);
            maxX = int.Parse(xTokens[1]);
            minY = int.Parse(yTokens[0]);
            maxY = int.Parse(yTokens[1]);
        }
    }
}