using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
                input = CalculatePhaseFast(input, 1).ToArray();
            }

            return string.Join('_', input.Take(8));
        }

        private static IEnumerable<int> CalculatePhaseFast(int[] input, int repeater)
        {
            var pattern = new int[] {0, 1, 0, -1};

            var sw = new Stopwatch();
            sw.Start();
            for (int resultDigitIndex = 0; resultDigitIndex < input.Length; resultDigitIndex++)
            {
                var patternMultiplier = resultDigitIndex + 1;
                var patternMultiplier4 = patternMultiplier * 4;
                var sum = 0;
                var init = false;

                for (int inputIndex = 0; inputIndex < input.Length; inputIndex++)
                {
                    //var realIndex = GetPatternRealIndex(patternMultiplier, inputIndex + 1);
                    var realIndex = ((inputIndex % patternMultiplier4) + 1) / patternMultiplier;
                    //(index % (multiplier * 4)) / multiplier

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

                if (resultDigitIndex % 100 == 0)
                {
                    Console.WriteLine(resultDigitIndex + " " + sw.ElapsedMilliseconds);
                    sw.Reset();
                    sw.Start();
                }

                yield return Math.Abs(sum % 10);
            }
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