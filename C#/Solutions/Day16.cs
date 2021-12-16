using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Solutions
{
    public class Day16 : IChallenge
    {
        private Dictionary<string, string> conversion;
        private string code;

        private ulong totalVersionNumbers = 0;
        private bool endFlag = false;
        public void Solve(string[] data)
        {
            ParseData(data);
            var index = 0;
            
            while (index < code.Length)
            {
                var answer = ReadMessage(ref index);
                
                if(!endFlag)
                    Console.WriteLine($"Part 2 Answer: {answer}");
                
            }

            Console.WriteLine($"Part 1 Answer: {totalVersionNumbers}");
        }
        
        ulong ReadMessage(ref int index)
        {
            if (code.Length - index < 11)
            {
                index = code.Length;
                endFlag = true;
                return 0;
            }

            var version = code.Substring(index, 3);
            var versionNumber = Convert.ToUInt64(version, 2);
            
            totalVersionNumbers += versionNumber;

            index += 3;

            var packet = code.Substring(index, 3);
            var packetID = Convert.ToInt32(packet, 2);

            index += 3;

            return packetID switch
            {
                4 => LiteralValuePacket(ref index),
                _ => OperatorPacket(packetID, ref index)
            };
        }

        ulong OperatorPacket(int operation, ref int index)
        {
            var typeId = code[index++];
            switch (typeId)
            {
                case '0':
                {
                    var subpacketBits = code.Substring(index, 15);
                    index += 15;
                    var subpacketLength = Convert.ToInt32(subpacketBits, 2);
                    var startIndex = index;
                    List<ulong> values = new List<ulong>();
                    while (startIndex + subpacketLength > index)
                    {
                        values.Add(ReadMessage(ref index));
                    }
                    return ExecuteOperation(operation, values);
                }
                case '1':
                {
                    var subpacketBits = code.Substring(index, 11);
                    index += 11;
                    var numSubpackets = Convert.ToInt32(subpacketBits, 2);
                    List<ulong> values = new List<ulong>();
                    for (int i = 0; i < numSubpackets; i++)
                    {
                        values.Add(ReadMessage(ref index));
                    }

                    return ExecuteOperation(operation, values);
                }
            }

            return 0;
        }

        ulong ExecuteOperation(int operation, List<ulong> values)
        {
            return operation switch
            {
                0 => values.Aggregate<ulong, ulong>(0, (current, v) => current + v),
                1 => values.Aggregate<ulong, ulong>(1, (current, v) => current * v),
                2 => values.Aggregate<ulong, ulong>(ulong.MaxValue, (current, v) => v < current ? v : current),
                3 => values.Aggregate<ulong, ulong>(0, (current, v) => v > current ? v : current),
                5 => values[0] > values[1] ? (ulong) 1 : 0,
                6 => values[0] < values[1] ? (ulong) 1 : 0,
                7 => values[0] == values[1] ? (ulong) 1 : 0,
                _ => 0
            };
        }

        ulong LiteralValuePacket(ref int index)
        {
            var lastBit = code[index++];
            var binaryNumberString = "";
            while (true)
            {
                binaryNumberString += code.Substring(index, 4);
                index += 4;
                if (lastBit == '0')
                {
                    break;
                }
                lastBit = code[index++];
            }

            var literalValue = Convert.ToUInt64(binaryNumberString, 2);

            return literalValue;
        }

        void ParseData(string[] data)
        {
            conversion = new Dictionary<string, string>();
            var recipes = data[Range.EndAt(data.Length - 1)];
            foreach (var recipe in recipes)
            {
                var tokens = recipe.Split(" = ");
                conversion.Add(tokens[0],tokens[1]);
            }

            var target = data.Last();
            code = "";
            foreach (var c in target)
            {
                code += conversion[$"{c}"];
            }
        }
    }
}