using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Solutions
{
    public class Day12 : IChallenge
    {
        private List<CavePath> calculatingPaths = new List<CavePath>();
        private List<string> finishedPaths = new List<string>();

        private Dictionary<string, Cave> CaveMap = new Dictionary<string, Cave>();
        private Cave start => CaveMap["start"];
        
        public void Solve(string[] data)
        {
            ParseCaves(data);
            calculatingPaths.Add(new CavePath(start));
            CalculateAllPaths();
            
        }

        void CalculateAllPaths()
        {
            while (calculatingPaths.Count > 0)
            {
                var currentPaths = calculatingPaths.ToArray();
                foreach (var path in currentPaths)
                {
                    path.Step(ref calculatingPaths, ref finishedPaths);
                }
            }
            
            Console.WriteLine($"\nFound {finishedPaths.Count} valid paths");
        }


        void ParseCaves(string[] data)
        {
            foreach (var line in data)
            {
                var tokens = line.Split('-');
                var startCave = GetCave(tokens[0]);
                var endCave = GetCave(tokens[1]);
                if(!startCave.Connections.Contains(endCave)) startCave.Connections.Add(endCave);
                if(!endCave.Connections.Contains(startCave)) endCave.Connections.Add(startCave);
            }
        }

        Cave GetCave(string name)
        {
            if (CaveMap.ContainsKey(name)) return CaveMap[name];

            var cave = new Cave(name);
            CaveMap.Add(name, cave);
            return cave;
        }
    }
    
    class Cave
        {
            public bool isBigCave;
            public string Name;
            public List<Cave> Connections;

            public Cave(string name)
            {
                Connections = new List<Cave>();
                Name = name;
                isBigCave = Char.IsUpper(name[0]);
            }
        }

        class CavePath
        {
            public List<string> VisitedCaves;
            public Cave CurrentCave;
            public bool CanRevisitSingleCave = true;

            public CavePath(Cave startingCave)
            {
                CurrentCave = startingCave;
                VisitedCaves = new List<string>(){CurrentCave.Name};
                CanRevisitSingleCave = true;
            }
            
            public CavePath(Cave nextCave, List<string> visited)
            {
                CurrentCave = nextCave;
                VisitedCaves = new List<string>(visited) { CurrentCave.Name };
            }

            public void Step(ref List<CavePath> calculatingPaths, ref List<string> finishedPaths)
            {

                if (CurrentCave.Name == "end")
                {
                    calculatingPaths.Remove(this);
                    if (!finishedPaths.Contains(this.ToString()))
                    {
                        finishedPaths.Add(this.ToString());
                    }

                    return;
                }

                var validConnections = CurrentCave.Connections
                    .Where(x => x.Name!="start" && 
                                    (x.isBigCave || 
                                    !VisitedCaves.Contains(x.Name) || 
                                    CanRevisitSingleCave)).ToArray();
                
                if (validConnections.Length == 0)
                {
                    //path failed
                    calculatingPaths.Remove(this);
                    return;
                }
                
                //branch other paths
                if (validConnections.Length > 1)
                {
                    for (int i = 1; i < validConnections.Length; i++)
                    {
                        var nextCave = validConnections[i];
                        var newPath = new CavePath(nextCave, VisitedCaves);

                        if (!nextCave.isBigCave && VisitedCaves.Contains(nextCave.Name))
                            newPath.CanRevisitSingleCave = false;
                        else
                            newPath.CanRevisitSingleCave = CanRevisitSingleCave;
                        
                        calculatingPaths.Add(newPath);
                    }
                }
                
                //move forward along my path
                if (!validConnections[0].isBigCave && VisitedCaves.Contains(validConnections[0].Name))
                    CanRevisitSingleCave = false;
                VisitedCaves.Add(validConnections[0].Name);
                CurrentCave = validConnections[0];
            }

            public string ToString()
            {
                var s = "";
                foreach (var c in VisitedCaves)
                {
                    s += $"{c},";
                }
                
                return s.TrimEnd(',');
            }
        }
    
}