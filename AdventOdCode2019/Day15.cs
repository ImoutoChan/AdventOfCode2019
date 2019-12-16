using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AdventOdCode2019
{
    public class Droid
    {
        private readonly IntCodeRunner9 _runner;
        private readonly DroidPoint _homePoint = new DroidPoint(0, 0, null);
        private readonly Stack<Movement> _fromHome = new Stack<Movement>();

        private DroidPoint _position = new DroidPoint(0, 0, null);

        public Droid(long[] program)
        {
            _runner = new IntCodeRunner9(program);
        }

        public Status Move(Movement movement)
        {
            var status = (Status)_runner.Run((int)movement);
            if (status != Status.Wall)
            {
                _fromHome.Push(movement);
                _position = _position.GetPoint(movement);
            }

            return status;
        }

        public Status Move(DroidPoint point)
        {
            Status status = Status.Empty;

            var currentPoint = point;
            foreach (var pathPoint in point.GetPathToHome().Reverse().Skip(1).Append(point))
            {
                var movement = _position.GetMovement(pathPoint);
                status = Move(movement);
            }

            return status;
        }

        public void ReturnHome()
        {
            while (_fromHome.TryPop(out var movement))
            {
                var moveTo = GetReverse(movement);
                var status = (Status)_runner.Run((int)moveTo);
                _position = _position.GetPoint(moveTo);

                if (status != Status.Empty)
                    throw new Exception("Invalid returning home");
            }

            if (!_position.Equals(_homePoint))
                throw new Exception("Invalid returning home");
        }

        private Movement GetReverse(Movement movement)
        {
            switch (movement)
            {
                case Movement.North:
                    return Movement.South;
                case Movement.South:
                    return Movement.North;
                case Movement.West:
                    return Movement.East;
                case Movement.East:
                    return Movement.West;
                default:
                    throw new ArgumentOutOfRangeException(nameof(movement), movement, null);
            }
        }
    }

    internal class Day15 : IAdventOfCodeDay
    {
        public string CalculatePart1(string inputFile)
        {
            var program = GetProgram(inputFile);
            var droid = new Droid(program);

            var startPoint = new DroidPoint(0, 0, null);
            var map = new Dictionary<DroidPoint, Status>();
            map.Add(startPoint, Status.Empty);

            var queue = new Queue<DroidPoint>();
            EnqueuePoints(startPoint, Movement.East);
            EnqueuePoints(startPoint, Movement.West);
            EnqueuePoints(startPoint, Movement.North);
            EnqueuePoints(startPoint, Movement.South);

            while (queue.Any())
            {
                var currentPoint = queue.Dequeue();
                if (map.ContainsKey(currentPoint))
                    continue;

                var status = CheckCurrentPoint(currentPoint, droid);
                map.Add(currentPoint, status);

                if (status == Status.Empty)
                {
                    EnqueuePoints(currentPoint, Movement.East);
                    EnqueuePoints(currentPoint, Movement.West);
                    EnqueuePoints(currentPoint, Movement.North);
                    EnqueuePoints(currentPoint, Movement.South);
                }

                else if (status == Status.Oxygen)
                {
                    return (currentPoint.HomePath.GetPathToHome().Count() + 1).ToString();
                }
            }

            return 0.ToString();

            void EnqueuePoints(DroidPoint currentPoint, Movement movement)
            {
                var newPoint = currentPoint.GetPoint(movement);
                if (!map.ContainsKey(newPoint))
                    queue.Enqueue(newPoint);
            }
        }
        
        private Status CheckCurrentPoint(DroidPoint currentPoint, Droid droid)
        {
            var statusResult = droid.Move(currentPoint);
            droid.ReturnHome();

            return statusResult;
        }
        
        public string CalculatePart2(string inputFile)
        {
            var program = GetProgram(inputFile);
            var droid = new Droid(program);
            
            var startPoint = new DroidPoint(0, 0, null);
            var map = new Dictionary<DroidPoint, Status>();
            map.Add(startPoint, Status.Empty);

            var queue = new Queue<DroidPoint>();
            EnqueuePoints(startPoint, Movement.East);
            EnqueuePoints(startPoint, Movement.West);
            EnqueuePoints(startPoint, Movement.North);
            EnqueuePoints(startPoint, Movement.South);

            while (queue.Any())
            {
                var currentPoint = queue.Dequeue();
                if (map.ContainsKey(currentPoint))
                    continue;

                var status = CheckCurrentPoint(currentPoint, droid);
                map.Add(currentPoint, status);

                if (status != Status.Wall)
                {
                    EnqueuePoints(currentPoint, Movement.East);
                    EnqueuePoints(currentPoint, Movement.West);
                    EnqueuePoints(currentPoint, Movement.North);
                    EnqueuePoints(currentPoint, Movement.South);
                }
            }

            return FindFullBreadth(map).ToString();

            void EnqueuePoints(DroidPoint currentPoint, Movement movement)
            {
                var newPoint = currentPoint.GetPoint(movement);
                if (!map.ContainsKey(newPoint))
                    queue.Enqueue(newPoint);
            }
        }

        private int FindFullBreadth(Dictionary<DroidPoint, Status> map)
        {
            var search = new BreadthFirstSearch<DroidPoint>(MoveFunc);
            int max = 0;
            search.GetShortestPathLength(
                map.Single(x => x.Value == Status.Oxygen).Key,
                (_, depth) =>
                {
                    max = Math.Max(max, depth);
                    return false;
                });

            return max;

            IEnumerable<DroidPoint> MoveFunc(DroidPoint point)
            {
                return point
                    .GetNearPoints()
                    .Where(x => map.TryGetValue(x, out var status) && status != Status.Wall);
            }
        }

        private static long[] GetProgram(string inputFile)
        {
            var programString = File.ReadAllLines(inputFile).First();
            var program = programString.Split(',').Select(long.Parse).ToArray();
            return program;
        }
    }

    [DebuggerDisplay("X = {X} Y = {Y}")]
    public class DroidPoint : IEquatable<DroidPoint>
    {
        public int X { get; }

        public int Y { get; }

        public DroidPoint HomePath { get; }

        public IEnumerable<DroidPoint> GetPathToHome()
        {
            var current = HomePath;
            while (current != null)
            {
                yield return current;
                current = current.HomePath;
            }
        }

        public DroidPoint(int x, int y, DroidPoint homePath)
        {
            X = x;
            Y = y;
            HomePath = homePath;
        }

        public DroidPoint GetPoint(Movement direction)
        {
            switch (direction)
            {
                case Movement.North:
                    return new DroidPoint(X, Y + 1, this);
                case Movement.East:
                    return new DroidPoint(X + 1, Y, this);
                case Movement.South:
                    return new DroidPoint(X, Y - 1, this);
                case Movement.West:
                    return new DroidPoint(X - 1, Y, this);
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }
        public IEnumerable<DroidPoint> GetNearPoints()
        {
            yield return GetPoint(Movement.East);
            yield return GetPoint(Movement.West);
            yield return GetPoint(Movement.North);
            yield return GetPoint(Movement.South);
        }

        public Movement GetMovement(DroidPoint pathPoint)
        {
            if (pathPoint.X < X)
                return Movement.West;
            if (pathPoint.X > X)
                return Movement.East;
            if (pathPoint.Y < Y)
                return Movement.South;
            if (pathPoint.Y > Y)
                return Movement.North;

            throw new Exception("IncorrectMovement");
        }

        public bool Equals(DroidPoint other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is DroidPoint other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }
    }

    public enum Movement
    {
        North = 1,
        South = 2,
        West = 3,
        East = 4
    }

    public enum Status
    {
        Wall = 0,
        Empty = 1,
        Oxygen = 2
    }
}