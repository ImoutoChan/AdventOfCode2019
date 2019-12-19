using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOdCode2019
{
    internal class Day16 : IAdventOfCodeDay
    {
        public string CalculatePart1(string inputFile)
        {
            var input = GetInput(inputFile);
            var phasesCount = 100;

            for (int i = 0; i < phasesCount; i++)
            {
                input = CalculatePhaseFast(input);
            }

            return string.Join('_', input.Take(8));
        }

        private static int[] CalculatePhaseFast(int[] input)
        {
            var inputLength = input.Length;
            var result = new int[inputLength];
            var cumSum = new int[inputLength];
            cumSum[0] = input[0];
            for (int i = 1; i < inputLength; i++)
            {
                cumSum[i] = cumSum[i - 1] + input[i];
            }

            Parallel.For(
                0,
                inputLength,
                //new ParallelOptions {  MaxDegreeOfParallelism = 48 },
                (resultDigitIndex) =>
                {
                    var patternMultiplier = resultDigitIndex + 1;
                    var patternMultiplier2 = patternMultiplier * 2;
                    var sum = 0;

                    var startPositive = resultDigitIndex;
                    var takeM = resultDigitIndex; // take - 1
                    var sign = 1;

                    var cond = inputLength - takeM;
                    var i = startPositive;

                    if (i == 0)
                    {
                        sum += sign * cumSum[i + takeM];
                        sign = -1 * sign;
                        i += patternMultiplier2;
                    }

                    for (; i < cond; i += patternMultiplier2)
                    {
                        sum += sign * (cumSum[i + takeM] - cumSum[i - 1]);
                        sign = -1 * sign;
                    }

                    if (i < inputLength)
                    {
                        sum += sign * (cumSum[inputLength - 1] - cumSum[i - 1]);
                    }

                    result[resultDigitIndex] = Math.Abs(sum % 10);
                });

            return result;
        }

        public string CalculatePart2(string inputFile)
        {
            const int phasesCount = 100;

            var input = Enumerable.Repeat(GetInput(inputFile), 10000).SelectMany(x => x).ToArray();
            var skip = input.Take(7).Aggregate("", (acc, x) => acc + x);

            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < phasesCount; i++)
            {
                input = CalculatePhaseFast(input);
                Console.WriteLine("Phase: " + i + "Time: " + sw.ElapsedMilliseconds);
            }

            return string.Join('_', input.Skip(int.Parse(skip)).Take(8));
        }

        private static int[] GetInput(string inputFile)
        {
            var programString = File.ReadAllLines(inputFile).First();
            //var programString = "03036732577212944063491565474664";
            var program = programString.Select(x => (int)(x - '0')).ToArray();
            return program;
        }
    }
}