using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Advent_of_Code.Solutions;
using static G8S;

public class Day19 : IChallenge
{
    List<Scanner> scanners;
    private Scanner map;

    public void Solve(string[] data)
    {
        ParseData(data);
        Solve();
    }

    async Task Solve()
    {
        var map = scanners[0];
        
        scanners.Remove(map);
        var scannerCoordinates = new List<Vector3>(){new(0,0,0)};
        
        while (scanners.Count > 0)
        {
            var next = await FindBest(map);
            Log($"Next: {next.ID}");
            var result = map.CompareBeacons(next, true);

            var common = new List<Beacon>();
            foreach ((_, var them, _) in result)
            {
                common.Add(them);
            }
            
            var scannerPosition = result[0].Item1.Position - result[0].Item2.Position;

            scannerCoordinates.Add(scannerPosition);
            
            Log($"Consuming {next.ID}");
            map.Consume(next, common, scannerPosition, result[0].Item3, scanners.Count > 0);
            Log($"Remaining: {scanners.Count}");
        }
        
        Log($"There are {map.Beacons.Count} beacons", force:true);

        int maxManhatten = 0;
        foreach (var a in scannerCoordinates)
        {
            foreach (var b in scannerCoordinates)
            {
                if (a == b) continue;
                maxManhatten = Math.Max(maxManhatten, ManhattenDistance(a, b));
            }
        }
        
        Log($"The max manhatten distance is {maxManhatten}", force:true);
    }

    int ManhattenDistance(Vector3 a, Vector3 b)
    {
        return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) + Math.Abs(a.z - b.z);
    }

    async Task<Scanner> FindBest(Scanner start)
    {
        Scanner best = null;
        var max = 0;
        Task<int>[] tasks = new Task<int>[scanners.Count];
        for (int i = 0; i < scanners.Count; i++)
        {
            
            var target = scanners[i];
            if(target == start) continue;

            tasks[i] = FindHits(start, scanners[i]);
        }

        await Task.WhenAll(tasks);
        
        for(int i=0;i<tasks.Length;i++)
        {
            var result = tasks[i];
            if (result.Result > max)
            {
                max = result.Result;
                best = scanners[i];
            }
        }
        scanners.Remove(best);
        return best;
    }

    async Task<int> FindHits(Scanner start, Scanner target)
    {
        if (start == target) return 0;
        var result = start.CompareBeacons(target);
        return result.Length;
    }

    void PrintScanners()
    {
        foreach (var scanner in scanners)
        {
            PrintScanner(scanner);
            Log("");
        }
    }

    void PrintScanner(Scanner scanner)
    {
        Log($"Scanner {scanner.ID} report:");
        for (int i = 0;i<scanner.Beacons.Count;i++)
        {
            Log($"Beacon {i} Fingerprint:");
            var beacon = scanner.Beacons[i];
            for (int j=0;j<beacon.Fingerprint.Count;j++)
            {
                var printVariation = beacon.Fingerprint[j];
                Log($"Variation {j}:");
                foreach (var print in printVariation)
                {
                    Log($"{print}");
                }
            }
        }
    }

    void ParseData(string[] data)
    {
        scanners = new List<Scanner>();
        Scanner targetScanner = null;
        foreach (var line in data)
        {
            if (string.IsNullOrEmpty(line))
            {
                if (targetScanner == null) continue;
                targetScanner.CreateFingerprint();
                scanners.Add(targetScanner);
                continue;
            }
            if (line.StartsWith("---"))
            {
                var id = line.Replace("--- scanner ", "").Replace(" ---","");
                targetScanner = new Scanner(int.Parse(id));
                continue;
            }

            var tokens = line.Split(",");
            var x = int.Parse(tokens[0]);
            var y = int.Parse(tokens[1]);
            var z = tokens.Length == 3 ? int.Parse(tokens[2]) : 0;
            
            targetScanner?.Beacons.Add(new Beacon(x,y,z));
        }

        if (targetScanner != null && !scanners.Contains(targetScanner))
        {
            targetScanner.CreateFingerprint();
            scanners.Add(targetScanner);
        }
    }
}

public class Scanner
{
    public readonly int ID;

    public List<Beacon> Beacons;

    public Scanner(int id)
    {
        ID = id;
        Beacons = new List<Beacon>();
    }

    public void Consume(Scanner target, List<Beacon> commonBeacons, Vector3 scannerPosition, int rotationId, bool createFingerprint = true)
    {
        var newBeacons = target.Beacons.Where(x => !commonBeacons.Contains(x)).ToArray();
        
        foreach (var beacon in newBeacons)
        {
            beacon.Position = beacon.PossiblePositions[rotationId] + scannerPosition;
            Beacons.Add(beacon);
        }
        if (!createFingerprint) return;
        
        Log($"Creating fingerprint...");
        CreateFingerprint(newBeacons.ToList());
    }

    public (Beacon, Beacon,int)[] CompareBeacons(Scanner target, bool modify =false)
    {
        var matches = new List<(Beacon,Beacon,int)>();
        foreach (var beacon in Beacons)
        {
            foreach (var targetBeacon in target.Beacons)
            {
                (var match, var rotationId) = beacon.CompareFingerprint(targetBeacon);
                
                if (!match) continue;
                
                if (modify)
                {
                    targetBeacon.Position = targetBeacon.PossiblePositions[rotationId];
                }
                matches.Add((beacon, targetBeacon, rotationId));
            }
        }

        return matches.ToArray();
    }

    public void CreateFingerprint() => CreateFingerprint(Beacons);

    private void CreateFingerprint(List<Beacon> targets)
    {
        foreach (var beacon in targets)
        {
            beacon.CreateFingerprint(Beacons);
        }
    }
}

public class Beacon
{
    public Vector3 Position;
    public List<Vector3> PossiblePositions;
    public List<List<(Vector3 position,int rotId)>> Fingerprint;

    private const int SIMILAR_NEEDED = 2;
    
    public Beacon(int x, int y, int z)
    {
        Position = new Vector3(x, y, z);
        Fingerprint = new List<List<(Vector3,int)>>();
    }

    public (bool, int) CompareFingerprint(Beacon target)
    {
        int similarPrints = 0;

        for (int i = 0; i < target.Fingerprint.Count; i++)
        {
            var printList = target.Fingerprint[i];
            similarPrints = 0;
            
            foreach (var print in printList)
            {
                foreach (var targetPrint in Fingerprint[0])
                {
                    if (print.position == targetPrint.position && !print.position.isZero)
                    {
                        similarPrints++;
                    }
                }
            }

            if (similarPrints >= SIMILAR_NEEDED) return (true, i);
        }

        return (false, -1);
    }

    public void CreateFingerprint(List<Beacon> beacons)
    {
        Fingerprint = new List<List<(Vector3 position, int rotId)>>();
        
        for (int p = 0; p < 24; p++)
        {
            Fingerprint.Add(new List<(Vector3,int)>());
        }

        PossiblePositions = Vector3.GetAllPossible(Position.x, Position.y, Position.z);
        
        foreach (var target in beacons)
        {
            var distance = Position - target.Position;
            
            var allPositions = Vector3.GetAllPossible(distance.x, distance.y, distance.z);
            
            for (var i = 0; i < allPositions.Count; i++)
            {
                Fingerprint[i].Add((allPositions[i],i));
            }
        }

    }
}

public struct Vector3
{
    public Vector3(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public int x;
    public int y;
    public int z;

    public static Vector3 operator -(Vector3 a, Vector3 b)
        => new(a.x - b.x, a.y - b.y, a.z - b.z);
    
    public static Vector3 operator +(Vector3 a, Vector3 b)
        => new(a.x + b.x, a.y + b.y, a.z + b.z);
    
    public static bool operator ==(Vector3 a, Vector3 b)
        => a.x == b.x && a.y == b.y && a.z == b.z;
    
    public static bool operator !=(Vector3 a, Vector3 b)
        => !(a == b);

    public bool isZero => x == 0 && y == 0 && z == 0;
    

    public static List<Vector3> GetAllPossible(int x, int y, int z)
    {
        var p = new[]
        {
            new Vector3(x, y, z),
            new Vector3(x, -y, -z),
            
            new Vector3(-y, x, z),
            new Vector3(-y, -x, -z),
            
            new Vector3(x, z, -y),
            new Vector3(-x, -z, -y),
        };

        var allPossibleCoordinates = new List<Vector3>();

        for (int i = 0; i < p.Length; i++)
        {
            var allRotated = GetAllRotated(p[i]);
            foreach(var possible in allRotated)
                allPossibleCoordinates.Add(possible);
        }
        return allPossibleCoordinates;
    }

    public static Vector3[] GetAllRotated(Vector3 start)
    {
        return new[]
        {
            start,
            new Vector3(start.z, start.y, -start.x),
            new Vector3(-start.x, start.y, -start.z),
            new Vector3(-start.z, start.y, start.x),
        };
    }

    public override string ToString()
    {
        return $"{x},{y},{z}";
    }
}