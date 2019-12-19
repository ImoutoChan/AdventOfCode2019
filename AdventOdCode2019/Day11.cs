using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOdCode2019
{
    internal class Day11 : IAdventOfCodeDay
    {
        public string CalculatePart1(string inputFile)
        {
            var program = GetProgram(inputFile);

            var runner = new IntCodeRunner9(program);
            var painted =
                new IntCodeRunner9.DefaultableDictionary<PanelPoint, long>(new Dictionary<PanelPoint, long>(), 0);


            var currentPoint = new PanelPoint(0, 0);
            var currentDirection = Direction.Up;
            while (true)
            {
                var color = painted[currentPoint];

                var paint = runner.Run(color);
                var turn = runner.Run(color);

                if (!paint.HasValue || !turn.HasValue)
                    break;

                painted[currentPoint] = paint.Value;
                if (turn == 0)
                    currentDirection = TurnLeft(currentDirection);
                else if (turn == 1)
                    currentDirection = TurnRight(currentDirection);

                currentPoint = currentPoint.GetPoint(currentDirection);
            }

            return painted.Count.ToString();
        }

        public string CalculatePart2(string inputFile)
        {
            var program = GetProgram(inputFile);

            var runner = new IntCodeRunner9(program);
            var painted =
                new IntCodeRunner9.DefaultableDictionary<PanelPoint, long>(new Dictionary<PanelPoint, long>(), 0);


            var currentPoint = new PanelPoint(0, 0);
            painted[currentPoint] = 1;
            var currentDirection = Direction.Up;
            while (true)
            {
                var color = painted[currentPoint];

                var paint = runner.Run(color);
                var turn = runner.Run(color);

                if (!paint.HasValue || !turn.HasValue)
                    break;

                painted[currentPoint] = paint.Value;
                if (turn == 0)
                    currentDirection = TurnLeft(currentDirection);
                else if (turn == 1)
                    currentDirection = TurnRight(currentDirection);

                currentPoint = currentPoint.GetPoint(currentDirection);
            }

            var whites = painted.Where(x => x.Value == 1).OrderByDescending(x => x.Key.Y).GroupBy(x => x.Key.Y);

            var minX = painted.Min(x => x.Key.X);
            var maxX = painted.Max(x => x.Key.X);

            var sb = new StringBuilder();
            sb.AppendLine();
            foreach (var whiteLine in whites)
            {
                for (int i = minX; i < maxX + 1; i++)
                {
                    sb.Append(whiteLine.Any(x => x.Key.X == i) ? "*" : " ");
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static long[] GetProgram(string inputFile)
        {
            var programString = File.ReadAllLines(inputFile).First();
            //var programString = "109,1,204,-1,1001,100,1,100,1008,100,16,101,1006,101,0,99";
            var program = programString.Split(',').Select(long.Parse).ToArray();
            return program;
        }

        private Direction TurnLeft(Direction current)
        {
            switch (current)
            {
                case Direction.Up:
                    return Direction.Left;
                case Direction.Right:
                    return Direction.Up;
                case Direction.Down:
                    return Direction.Right;
                case Direction.Left:
                    return Direction.Down;
                default:
                    throw new ArgumentOutOfRangeException(nameof(current), current, null);
            }
        }

        private Direction TurnRight(Direction current)
        {
            switch (current)
            {
                case Direction.Up:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Down;
                case Direction.Down:
                    return Direction.Left;
                case Direction.Left:
                    return Direction.Up;
                default:
                    throw new ArgumentOutOfRangeException(nameof(current), current, null);
            }
        }
    }

    enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }

    [DebuggerDisplay("X = {X} Y = {Y}")]
    internal struct PanelPoint : IEquatable<PanelPoint>
    {
        public int X { get; }

        public int Y { get; }

        public PanelPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public PanelPoint GetPoint(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return new PanelPoint(X, Y + 1);
                case Direction.Right:
                    return new PanelPoint(X + 1, Y);
                case Direction.Down:
                    return new PanelPoint(X, Y - 1);
                case Direction.Left:
                    return new PanelPoint(X - 1, Y);
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public bool Equals(PanelPoint other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is PanelPoint other && Equals(other);
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