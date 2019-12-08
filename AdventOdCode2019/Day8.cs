using System;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOdCode2019
{
    internal class Day8 : IAdventOfCodeDay
    {
        public string CalculatePart1(string inputFile)
        {
            var digits = GetDigits(inputFile);
            var width = 25;
            var height = 6;
            var pixelsPerLayer = width * height;

            var zeros = int.MaxValue;
            var result = 0;
            for (int i = 0; i < digits.Length; i += pixelsPerLayer)
            {
                var layer = digits.Skip(i).Take(pixelsPerLayer).ToArray();
                var currentZeros = layer.Count(x => x == 0);
                if (currentZeros >= zeros) 
                    continue;
                
                result = layer.Count(x => x == 1) * layer.Count(x => x == 2);
                zeros = currentZeros;
            }

            return result.ToString();
        }

        public string CalculatePart2(string inputFile)
        {
            var digits = GetDigits(inputFile);
            var width = 25;
            var height = 6;
            var pixelsPerLayer = width * height;

            var result = Enumerable.Range(0, pixelsPerLayer).Select(_ => 2).ToArray();

            for (int i = 0; i < digits.Length; i += pixelsPerLayer)
            {
                var layer = digits.Skip(i).Take(pixelsPerLayer).ToArray();

                for (int j = 0; j < layer.Length; j++)
                {
                    if (result[j] == 2 && layer[j] != 2)
                        result[j] = layer[j];
                }
            }

            var sb = new StringBuilder();
            sb.AppendLine();
            for (int i = 0; i < result.Length; i += width)
            {
                var line = result
                    .Skip(i)
                    .Take(width)
                    .Select(x => x == 0 ? " " : "▮")
                    .ToList();

                sb.AppendLine(string.Join("", line));
            }

            return sb.ToString();
        }

        private static int[] GetDigits(string inputFile)
        {
            var inputLine = File.ReadAllLines(inputFile).First();
            var digits = inputLine
                .ToCharArray()
                .Select(x => int.Parse(x.ToString()))
                .ToArray();

            return digits;
        }
    }
}