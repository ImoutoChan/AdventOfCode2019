using System;
using System.IO;
using System.Linq;

namespace AdventOdCode2019
{
    internal class Day2 : IAdventOfCodeDay
    {
        public string CalculatePart1(string inputFile)
        {
            var programString = File.ReadAllLines("input/day2.txt").First();
            var program = programString.Split(',').Select(int.Parse).ToArray();

            program[1] = 12;
            program[2] = 2;

            RunProgram(ref program);
            
            return program[0].ToString();
        }

        public string CalculatePart2(string inputFile)
        {
            var programString = File.ReadAllLines("input/day2.txt").First();
            var program = programString.Split(',').Select(int.Parse).ToArray();

            for (int i = 0; i < 99; i++)
            for (int j = 0; j < 99; j++)
            {
                var copy = program.ToArray();
                copy[1] = i;
                copy[2] = j;
                
                RunProgram(ref copy);

                if (copy[0] == 19690720)
                    return (i * 100 + j).ToString();
            }

            return "Nothing found!";
        }

        private static void RunProgram(ref int[] program)
        {
            for (int i = 0; i < program.Length; i = i + 4)
            {
                if (program[i] == 99)
                    return;

                var op1 = program[i + 1];
                var op2 = program[i + 2];
                var target = program[i + 3];

                switch (program[i])
                {
                    case 1:
                        program[target] = program[op1] + program[op2];
                        break;
                    case 2:
                        program[target] = program[op1] * program[op2];
                        break;
                }
            }
        }
    }
}