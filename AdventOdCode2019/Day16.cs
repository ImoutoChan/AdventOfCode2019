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
                input = CalculatePhase(input).ToArray();
            }

            return string.Join('_', input.Take(8));
        }

        private IEnumerable<int> CalculatePhase(int[] input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                var multiplier = i + 1;
                using var pattern = GetCycledPattern(multiplier).GetEnumerator();
                var result = 0;
                foreach (var inputDigit in input)
                {
                    pattern.MoveNext();
                    result += pattern.Current * inputDigit;
                }

                yield return Math.Abs(result % 10);
            }
        }

        public string CalculatePart2(string inputFile)
        {
            IEnumerable<int> input = GetInput(inputFile);

            var inputReady = input.ToArray();

            var phasesCount = 100;
            var skip = inputReady.Take(7).Aggregate("", (acc, x) => acc + x);

            for (int i = 0; i < phasesCount; i++)
            {
                inputReady = CalculatePhase(inputReady).ToArray();
            }

            return string.Join('_', inputReady.Skip(int.Parse(skip)).Take(8));
        }

        private static int[] GetInput(string inputFile)
        {
            var programString = File.ReadAllLines(inputFile).First();
            //var programString = "80871224585914546619083218645595";
            var program = programString.Select(x => x - '0').ToArray();
            return program;
        }

        private static IEnumerable<int> GetCycledPattern(int multiplier) 
            => GetPattern(multiplier).Skip(1);

        private static IEnumerable<int> GetPattern(int multiplier)
        {
            var pattern = new[] {0, 1, 0, -1};

            while (true)
            {
                foreach (var entry in pattern)
                {
                    for (int j = 0; j < multiplier; j++)
                    {
                        yield return entry;
                    }
                }
            }
        }
    }
}