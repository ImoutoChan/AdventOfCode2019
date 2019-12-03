using System;

namespace AdventOdCode2019
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            new Day1Part1().Calculate("input/day1part1.txt").Dump();
            new Day1Part2().Calculate("input/day1part1.txt").Dump();
            new Day2().CalculatePart1("input/day2.txt").Dump();
            new Day2().CalculatePart2("input/day2.txt").Dump();
            new Day3().CalculatePart1("input/day3.txt").Dump();
            new Day3().CalculatePart2("input/day3.txt").Dump();
        }

        private static void Dump(this object obj) => Console.WriteLine(obj.ToString());
    }
}