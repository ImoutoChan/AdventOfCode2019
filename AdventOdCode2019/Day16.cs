﻿using System;
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
                        if (progress % 100 == 0)
                        {
                            var h = (progress - lastReported) / 100;
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
                    var init = false;

                    for (int inputIndex = 0 + patternMultiplier - 1; inputIndex < inputLength; inputIndex++)
                    {
                        var realIndex = ((inputIndex % patternMultiplier4) + 1) / patternMultiplier;

                        if (realIndex == 1)
                        {
                            sum += input[inputIndex];
                            init = true;
                        }
                        else if (realIndex == 3)
                        {
                            sum -= input[inputIndex];
                            init = true;
                        }
                        else if (init)
                        {
                            inputIndex += patternMultiplier - 1;
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

            for (int i = 0; i < phasesCount; i++)
            {
                input = CalculatePhaseFast(input, repeatInput).ToArray();
            }

            var skip = input.Take(7).Aggregate("", (acc, x) => acc + x);
            return string.Join('_', input.Skip(int.Parse(skip)).Take(8));
        }

        private static int[] GetInput(string inputFile)
        {
            var programString = File.ReadAllLines(inputFile).First();
            //var programString = "00123456789001234567890012345678900123456789";
            var program = programString.Select(x => (int)(x - '0')).ToArray();
            return program;
        }
    }
}