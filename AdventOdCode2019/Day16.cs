using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
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
                input = CalculatePhaseFast(input, 1);
            }

            return string.Join('_', input.Take(8));
        }

        private static int[] CalculatePhaseFast(int[] input, int repeater)
        {
            var pattern = new int[] {0, 1, 0, -1};
            var inputLength = input.Length;
            var result = new int[inputLength];
            var progress = 0;

            Task.Run(async () =>
                {
                    var sw = new Stopwatch();
                    sw.Start();
                    var lastReported = 0;
                    while (progress != inputLength)
                    {
                        if (progress % 10000 == 0)
                        {
                            var h = (progress - lastReported) / 10000;
                            if (h == 0)
                                h = 1;

                            Console.WriteLine(progress + " " + sw.ElapsedMilliseconds / h);
                            lastReported = progress;

                            sw.Reset();
                            sw.Start();
                            await Task.Delay(100);
                        }
                    }
                });

            Parallel.For(
                0,
                inputLength,
                //new ParallelOptions {MaxDegreeOfParallelism = 1},
                (resultDigitIndex) =>
                {
                    var patternMultiplier = resultDigitIndex + 1;
                    var patternMultiplier4 = patternMultiplier * 4;
                    var sum = 0;

                    var startPositive = patternMultiplier - 1;
                    var startNegative = patternMultiplier * 3 - 1;
                    var take = patternMultiplier;
                    var takeM = patternMultiplier - 1;

                    for (int i = startPositive; i < inputLength; i += patternMultiplier4)
                    {
                        if (i + takeM < inputLength)
                        {
                            for (int j = 0; j < take; j++)
                            {
                                sum += input[i + j];
                            }
                        }
                        else
                        {
                            for (int j = 0; j < take; j++)
                            {
                                var index = i + j;
                                if (index < inputLength)
                                    sum += input[index];
                            }
                        }
                    }

                    for (int i = startNegative; i < inputLength; i += patternMultiplier4)
                    {
                        if (i + takeM < inputLength)
                        {
                            for (int j = 0; j < take; j++)
                            {
                                sum -= input[i + j];
                            }
                        }
                        else
                        {
                            for (int j = 0; j < take; j++)
                            {
                                var index = i + j;
                                if (index < inputLength)
                                    sum -= input[index];
                            }
                        }
                    }

                    result[resultDigitIndex] = Math.Abs(sum % 10);
                    Interlocked.Increment(ref progress);
                });

            return result;
        }

        public string CalculatePart2(string inputFile)
        {
            var phasesCount = 100;
            var repeatInput = 10_000;

            var input = Enumerable.Repeat(GetInput(inputFile), 10000).SelectMany(x => x).ToArray();
            var skip = input.Take(7).Aggregate("", (acc, x) => acc + x);

            for (int i = 0; i < phasesCount; i++)
            {
                input = CalculatePhaseFast(input, repeatInput).ToArray();
                Console.WriteLine("Phase: " + i);
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