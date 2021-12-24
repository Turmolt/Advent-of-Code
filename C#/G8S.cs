using System;
using System.Text;

namespace Advent_of_Code;

public static class G8S
{
    private const bool LOG = false;

    public static void Log(object n, bool writeLine = true, bool force = false) => Log($"{n}",writeLine,force);
    
    public static void Log(string msg, bool writeLine = true, bool force = false)
    {
        if (!LOG && !force) return;
            
        if(writeLine)
            Console.WriteLine(msg);
        else
            Console.Write(msg);
            
    }
}