using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Advent_of_Code.Solutions
{
    class Day5
    {
        List<((int x, int y) start, (int x, int y) end)> parsedCoordinates;
        private Dictionary<(int x, int y), int> ventField;

        public void Solve(string[] data)
        {
            ParseData(data);
            ventField = new Dictionary<(int x, int y), int>();
            MapField();

            var intersecting = ventField.Values.Where(x => x > 1).ToArray();
            Console.WriteLine(intersecting.Length);
        }

        void MapField()
        {
            foreach (var coordPair in parsedCoordinates)
            {
                AddVent(coordPair.start, coordPair.end);
            }
        }

        void AddVent((int x, int y) c1, (int x, int y) c2)
        {
            var dx = c2.x - c1.x;
            var dy =  c2.y - c1.y;
            var sx = Math.Sign(dx);
            var sy = Math.Sign(dy);

            var length = Math.Max(Math.Abs(dx), Math.Abs(dy));

            for (int d = 0; d <= length; d++)
            {
                var x = c1.x + (d * sx);
                var y = c1.y + (d * sy);
                AddCoordinateToField((x, y));
            }
        }
        
        void AddCoordinateToField((int x, int y) coord)
        {
            if (ventField.ContainsKey(coord))
            {
                ventField[coord]++;
            }
            else
            {
                ventField[coord] = 1;
            }
        }
        
        void ParseData(string[] data)
        {
            parsedCoordinates = new List<((int x, int y) start, (int x, int y) end)>();
            foreach(var line in data)
            {
                var tokens = line.Split(" -> ");
                var start = ParseCoordinate(tokens[0]);
                var end = ParseCoordinate(tokens[1]);
                parsedCoordinates.Add((start, end));
            }
        }
        
        (int x, int y) ParseCoordinate(string coord)
        {
            var tokens = coord.Split(',');
            var x = int.Parse(tokens[0]);
            var y = int.Parse(tokens[1]);
            return (x, y);
        }
    }
}
