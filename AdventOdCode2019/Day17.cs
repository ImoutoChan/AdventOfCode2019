using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AdventOdCode2019
{
    internal class Day17 : IAdventOfCodeDay
    {
        public string CalculatePart1(string inputFile)
        {
            var program = GetProgram(inputFile);

            var computer = new IntCodeRunner9(program);
            var sb = new StringBuilder();
            sb.AppendLine();
            var map = GetMap(computer, sb);

            long intersectionSum = 0;
            foreach (var scaffold in map
                .Where(x => x.Value == PointType.Scaffold)
                .Select(x => x.Key))
            {
                var values = Enum.GetValues(typeof(Direction)).Cast<Direction>();

                var isIntersection = values
                    .Select(x => scaffold.GetPoint(x))
                    .All(x => map.ContainsKey(x) && map[x] == PointType.Scaffold);

                if (isIntersection)
                    intersectionSum += scaffold.X * scaffold.Y;
            }

            sb.AppendLine(intersectionSum.ToString());

            var path = GetPath(map);
            sb.AppendLine(path);
            return sb.ToString();
        }

        private static Dictionary<ScaffoldPoint, PointType> GetMap(IntCodeRunner9 computer, StringBuilder sb)
        {
            var map = new Dictionary<ScaffoldPoint, PointType>();

            var x = 0;
            var y = 0;

            var result = computer.Run(null);
            while (result.HasValue)
            {
                if (result == 10)
                {
                    x = 0;
                    y++;
                    sb.Append((char) result);
                    result = computer.Run(null);
                    continue;
                }

                var type = GetPointType((char) result);
                map.Add(new ScaffoldPoint(x, y), type);

                sb.Append((char) result);

                x++;
                result = computer.Run(null);
            }

            return map;
        }

        private string GetPath(Dictionary<ScaffoldPoint, PointType> map)
        {
            var robo = map.First(x => (int) x.Value > 1);
            var currentPoint = robo.Key;
            Direction currentDirection;
            switch (robo.Value)
            {
                case PointType.RoboUp:
                    currentDirection = Direction.Up;
                    break;
                case PointType.RoboDown:
                    currentDirection = Direction.Down;
                    break;
                case PointType.RoboLeft:
                    currentDirection = Direction.Left;
                    break;
                case PointType.RoboRight:
                    currentDirection = Direction.Right;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var result = new List<(Turn Turn, int Count)>();
            var count = 0;

            do
            {
                var turn = GetTurn(map, currentPoint, currentDirection);
                if (turn == Turn.None)
                    break;

                currentDirection = GetDirection(currentDirection, turn);
                (count, currentPoint) = GetCount(map, currentPoint, currentDirection);
                
                result.Add((turn, count));
            } while (true);

            return String.Join(
                ",", 
                result.Select(x => $"{x.Turn.ToString().Substring(0, 1)},{x.Count}"));
        }

        private (int counter, ScaffoldPoint) GetCount(
            Dictionary<ScaffoldPoint, PointType> map, 
            ScaffoldPoint currentPoint,
            Direction newDirection)
        {
            var counter = 0;
            var point = currentPoint;
            point = point.GetPoint(newDirection);

            while (map.TryGetValue(point, out var type) && type == PointType.Scaffold)
            {
                counter++;
                point = point.GetPoint(newDirection);
            }

            var reverse = GetDirection(newDirection, Turn.Left);
            reverse = GetDirection(reverse, Turn.Left);

            return (counter, point.GetPoint(reverse));
        }

        private Turn GetTurn(
            Dictionary<ScaffoldPoint, PointType> map, 
            ScaffoldPoint currentPoint,
            Direction currentDirection)
        {
            var turns = new[] {Turn.Left, Turn.Right};

            foreach (var turn in turns)
            {
                var point = currentPoint.GetPoint(GetDirection(currentDirection, turn));

                if (map.TryGetValue(point, out var type) && type == PointType.Scaffold)
                    return turn;
            }


            return Turn.None;
        }

        private Direction GetDirection(Direction direction, Turn turn)
        {
            switch (turn)
            {
                case Turn.None:
                    return direction;
                case Turn.Left:
                    switch (direction)
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
                            throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
                    }
                case Turn.Right:
                    switch (direction)
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
                            throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(turn), turn, null);
            }
        }

        private static PointType GetPointType(char result)
        {
            var type = PointType.Empty;
            switch (result)
            {
                case '.':
                    type = PointType.Empty;
                    break;
                case '#':
                    type = PointType.Scaffold;
                    break;
                case '^':
                    type = PointType.RoboUp;
                    break;
                case 'v':
                    type = PointType.RoboDown;
                    break;
                case '<':
                    type = PointType.RoboLeft;
                    break;
                case '>':
                    type = PointType.RoboRight;
                    break;
            }

            return type;
        }

        public string CalculatePart2(string inputFile)
        {
            var program = GetProgram(inputFile);
            program[0] = 2;
            var solution = "A,A,B,C,B,C,B,C,C,A\nR,8,L,4,R,4,R,10,R,8\nL,12,L,12,R,8,R,8\nR,10,R,4,R,4\nn\n";

            var computer = new IntCodeRunner9(program);
            long? result = 0;
            long numResult = 0;
            var sb = new StringBuilder();
            GetMap(computer, sb);

            foreach (var c in solution.ToCharArray())
            {
                var input = (long) c;
                result = computer.Run(input);
                if (c == '\n')
                {
                    sb.Append((char) result);
                    numResult = result.Value;
                    do
                    {
                        result = computer.Run(null);
                        if (result.HasValue)
                        {
                            sb.Append((char) result);
                            numResult = result.Value;
                        }
                    } while (result != null);

                    sb.AppendLine();
                }
            }

            sb.AppendLine(numResult.ToString());

            return sb.ToString();
        }

        private static long[] GetProgram(string inputFile)
        {
            var programString = File.ReadAllLines(inputFile).First();
            var program = programString.Split(',').Select(long.Parse).ToArray();
            return program;
        }
    }

    [DebuggerDisplay("{X} {Y}")]
    struct ScaffoldPoint : IEquatable<ScaffoldPoint>
    {
        public ScaffoldPoint(long x, long y)
        {
            X = x;
            Y = y;
        }

        public long X { get; }

        public long Y { get; }

        public ScaffoldPoint GetPoint(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return new ScaffoldPoint(X, Y - 1);
                case Direction.Right:
                    return new ScaffoldPoint(X + 1, Y);
                case Direction.Down:
                    return new ScaffoldPoint(X, Y + 1);
                case Direction.Left:
                    return new ScaffoldPoint(X - 1, Y);
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public bool Equals(ScaffoldPoint other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is ScaffoldPoint other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }
    }

    internal enum PointType
    {
        Empty = 0,
        Scaffold = 1,
        RoboUp = 2,
        RoboDown = 3,
        RoboLeft = 4,
        RoboRight = 5
    }

    internal enum Turn
    {
        None,
        Left,
        Right
    }
}