using System;
using System.IO;
using System.Linq;

namespace AdventOdCode2019
{
    internal class Day13 : IAdventOfCodeDay
    {
        public string CalculatePart1(string inputFile)
        {
            var program = GetProgram(inputFile);
            
            long? result = 0;
            var runner = new IntCodeRunner9(program);

            var counter = 0;
            var counter1 = 0;
            while (result != null)
            {
                result = runner.Run(0);
                if (counter == 2)
                {
                    counter = 0;
                    if (result.HasValue && result.Value == 2)
                        counter1++;
                }
                else
                {
                    counter++;
                }
            }

            return (counter1).ToString();
        }

        public string CalculatePart2(string inputFile)
        {
            var program = GetProgram(inputFile);
            program[0] = 2;

            long score = 0;
            long input = 0;
            long paddle = 0;
            long ball = 0;
            var runner = new IntCodeRunner9(program);

            while (true)
            {
                var resultX = runner.Run(input);
                var resultY = runner.Run(input);
                var resultT = runner.Run(input);

                if(resultX == null || resultY == null || resultT == null)
                    break;

                if (resultX.Value == -1 && resultY.Value == 0)
                {
                    score = resultT.Value;
                    Console.WriteLine(score);
                }
                
                if (resultT == 3)
                    paddle = resultX.Value;
                if (resultT == 4)
                    ball = resultX.Value;

                if (ball < paddle)
                    input = -1;
                else if (ball > paddle)
                    input = 1;
                else
                    input = 0;
            }

            return score.ToString();
        }

        private static long[] GetProgram(string inputFile)
        {
            var programString = File.ReadAllLines(inputFile).First();
            var program = programString.Split(',').Select(long.Parse).ToArray();
            return program;
        }
    }
}