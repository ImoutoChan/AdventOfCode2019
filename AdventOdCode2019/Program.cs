using System;

namespace AdventOdCode2019
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            RunMe();
            //RunAlex();
        }

        private static void RunMe()
        {
            //new Day1Part1().Calculate("input/day1part1.txt").Dump(nameof(Day1Part1));
            //new Day1Part2().Calculate("input/day1part1.txt").Dump(nameof(Day1Part2));
            //new Day2().CalculatePart1("input/day2.txt").Dump(nameof(Day2));
            //new Day2().CalculatePart2("input/day2.txt").Dump(nameof(Day2));
            //new Day3().CalculatePart1("input/day3.txt").Dump(nameof(Day3));
            //new Day3().CalculatePart2("input/day3.txt").Dump(nameof(Day3));
            //new Day4().CalculatePart1(string.Empty).Dump(nameof(Day4));
            //new Day4().CalculatePart2(string.Empty).Dump(nameof(Day4));
            //new Day5().CalculatePart1("input/day5.txt").Dump(nameof(Day5));
            //new Day5().CalculatePart2("input/day5.txt").Dump(nameof(Day5));
            //new Day6().CalculatePart1("input/day6.txt").Dump(nameof(Day6));
            //new Day6().CalculatePart2("input/day6.txt").Dump(nameof(Day6));
            //new Day7().CalculatePart1("input/day7.txt").Dump(nameof(Day7));
            //new Day7().CalculatePart2("input/day7.txt").Dump(nameof(Day7));
            //new Day8().CalculatePart1("input/day8.txt").Dump(nameof(Day8));
            //new Day8().CalculatePart2("input/day8.txt").Dump(nameof(Day8));
            //new Day9().CalculatePart1("input/day9.txt").Dump(nameof(Day9));
            //new Day9().CalculatePart2("input/day9.txt").Dump(nameof(Day9));
            //new Day10().CalculatePart1("input/day10.txt").Dump(nameof(Day10));
            //new Day10().CalculatePart2("input/day10.txt").Dump(nameof(Day10));
            //new Day11().CalculatePart1("input/day11.txt").Dump(nameof(Day11));
            //new Day11().CalculatePart2("input/day11.txt").Dump(nameof(Day11));
            new Day12().CalculatePart1("input/day12.txt").Dump(nameof(Day12));
            new Day12().CalculatePart2("input/day12.txt").Dump(nameof(Day12));
        }

        private static void RunAlex()
        {
            new Day3().CalculatePart1("input/day3_alex.txt").Dump(nameof(Day3) + "Alex");
            new Day3().CalculatePart2("input/day3_alex.txt").Dump(nameof(Day3) + "Alex");
            new Day5().CalculatePart1("input/day5_alex.txt").Dump(nameof(Day5) + "Alex");
            new Day5().CalculatePart2("input/day5_alex.txt").Dump(nameof(Day5) + "Alex");
            new Day6().CalculatePart1("input/day6_alex.txt").Dump(nameof(Day6) + "Alex");
            new Day6().CalculatePart2("input/day6_alex.txt").Dump(nameof(Day6) + "Alex");
            new Day7().CalculatePart1("input/day7_alex.txt").Dump(nameof(Day7) + "Alex");
            new Day7().CalculatePart2("input/day7_alex.txt").Dump(nameof(Day7) + "Alex");
            new Day8().CalculatePart1("input/day8_alex.txt").Dump(nameof(Day8) + "Alex");
            new Day8().CalculatePart2("input/day8_alex.txt").Dump(nameof(Day8) + "Alex");
            new Day9().CalculatePart1("input/day9_alex.txt").Dump(nameof(Day9) + "Alex");
            new Day9().CalculatePart2("input/day9_alex.txt").Dump(nameof(Day9) + "Alex");
        }

        private static void Dump(this object obj, string day) => Console.WriteLine($"{day} {obj}");
    }
}