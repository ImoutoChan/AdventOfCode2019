using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AdventOdCode2019
{
    internal class Day3 : IAdventOfCodeDay
    {
        public string CalculatePart1(string inputFile)
        {
            var firstWire  = File.ReadAllLines(inputFile).First();
            var secondWire  = File.ReadAllLines(inputFile).Last();

            var allPointsFirst = GetAllWirePoints(firstWire).ToHashSet();
            var allPointsSeconds = GetAllWirePoints(secondWire).ToHashSet();
            
            var result = allPointsSeconds
                .Where(x => allPointsFirst.Contains(x))
                .Select(x => Math.Abs(x.X) + Math.Abs(x.Y)).Min();
            
            return result.ToString();
        }

        public string CalculatePart2(string inputFile)
        {
            var firstWire  = File.ReadAllLines(inputFile).First();
            var secondWire  = File.ReadAllLines(inputFile).Last();

            var allPointsFirst = GetAllWirePoints(firstWire).OrderBy(x => x.Step).ToHashSet();
            var allPointsSeconds = GetAllWirePoints(secondWire).OrderBy(x => x.Step).ToHashSet();
            
            var result = GetResults(allPointsSeconds, allPointsFirst).OrderBy(x => x.Steps).First().Steps;

            return result.ToString();
        }

        private static IEnumerable<(Point Point, int Steps)> GetResults(
            IReadOnlyCollection<Point> allPointsSeconds, 
            ICollection<Point> allPointsFirst)
        {
            var crosses = allPointsSeconds.Where(allPointsFirst.Contains);

            foreach (var cross in crosses)
            {
                var first = allPointsFirst.First(x => x.Equals(cross));
                var second = allPointsSeconds.First(x => x.Equals(cross));

                yield return (cross, first.Step + second.Step);
            }
        }

        private static List<Point> GetAllWirePoints(string firstWire)
        {
            List<Point> allPointsFirst = new List<Point>();

            var commands = firstWire.Split(',');
            var startPoint = new Point(0, 0);
            var currentPoint = startPoint;
            foreach (var command in commands)
            {
                var count = int.Parse(command.Substring(1));

                for (int i = 0; i < count; i++)
                {

                    switch (command[0])
                    {
                        case 'R':
                            currentPoint = currentPoint.Right();
                            break;
                        case 'L':
                            currentPoint = currentPoint.Left();
                            break;
                        case 'U':
                            currentPoint = currentPoint.Top();
                            break;
                        case 'D':
                            currentPoint = currentPoint.Bottom();
                            break;
                    }

                    allPointsFirst.Add(currentPoint);
                }
            }

            return allPointsFirst;
        }
    }
        
    [DebuggerDisplay("X = {X} Y = {Y}")]
    internal struct Point : IEquatable<Point>
    {
        public int X { get; }
        public int Y { get; }
        public int Step { get; }

        public Point(int x, int y, int step = 0)
        {
            X = x;
            Y = y;
            Step = step;
        }

        public Point Left() => new Point(X - 1, Y, Step + 1);
        public Point Right() => new Point(X + 1, Y, Step + 1);
        public Point Top() => new Point(X, Y + 1, Step + 1);
        public Point Bottom() => new Point(X, Y - 1, Step + 1);

        public bool Equals(Point other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is Point other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }
    }
}