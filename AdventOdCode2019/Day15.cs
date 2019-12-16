using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AdventOdCode2019
{
    internal class Day15 : IAdventOfCodeDay
    {
        public string CalculatePart1(string inputFile)
        {
            var program = GetProgram(inputFile);

            var runner = new IntCodeRunner9(program);

            var startPoint = new DroidPoint(0, 0, Enumerable.Empty<DroidPoint>());
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

                var status = CheckCurrentPoint(currentPoint);
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
                    return currentPoint.HomePath.Count().ToString();
                }
            }

            return 0.ToString();

            void EnqueuePoints(DroidPoint currentPoint, Movement movement)
            {
                var newPoint = currentPoint.GetPoint(movement);
                if (!map.ContainsKey(newPoint))
                    queue.Enqueue(newPoint);
            }

            Status CheckCurrentPoint(DroidPoint currentPoint)
            {
                if (currentPoint.Equals(startPoint))
                    return Status.Empty;

                var imPoint = startPoint;
                foreach (var pathPoint in currentPoint.HomePath.Skip(1))
                {
                    var movement = imPoint.GetMovement(pathPoint);
                    var res = runner.Run((int)movement);
                    imPoint = imPoint.GetPoint(movement);
                    if (res != 1)
                        throw new Exception();
                }

                var currentPointMovement = imPoint.GetMovement(currentPoint);
                var statusResult = (Status)runner.Run((int)currentPointMovement);
                
                if (statusResult == Status.Empty)
                {
                    imPoint = imPoint.GetPoint(currentPointMovement);
                }
                else if(statusResult == Status.Oxygen)
                {
                    return statusResult;
                }

                if (!imPoint.Equals(startPoint))
                {
                    foreach (var pathPoint in currentPoint.HomePath.Reverse())
                    {
                        if (imPoint.Equals(pathPoint))
                            continue;

                        var movement = imPoint.GetMovement(pathPoint);
                        var res = runner.Run((int) movement);
                        imPoint = imPoint.GetPoint(movement);
                        if (res != 1)
                            throw new Exception();
                    }
                }

                if (!imPoint.Equals(startPoint))
                    throw new Exception();

                return statusResult;
            }
        }


        public string CalculatePart2(string inputFile)
        {
            throw new NotImplementedException();
        }

        private static long[] GetProgram(string inputFile)
        {
            var programString = File.ReadAllLines(inputFile).First();
            var program = programString.Split(',').Select(long.Parse).ToArray();
            return program;
        }
    }

    [DebuggerDisplay("X = {X} Y = {Y}")]
    internal struct DroidPoint : IEquatable<DroidPoint>
    {
        public int X { get; }

        public int Y { get; }

        public IEnumerable<DroidPoint> HomePath { get; }

        public DroidPoint(int x, int y, IEnumerable<DroidPoint> homePath)
        {
            X = x;
            Y = y;
            HomePath = homePath.ToList();
        }

        public DroidPoint GetPoint(Movement direction)
        {
            switch (direction)
            {
                case Movement.North:
                    return new DroidPoint(X, Y + 1, HomePath.Append(this));
                case Movement.East:
                    return new DroidPoint(X + 1, Y, HomePath.Append(this));
                case Movement.South:
                    return new DroidPoint(X, Y - 1, HomePath.Append(this));
                case Movement.West:
                    return new DroidPoint(X - 1, Y, HomePath.Append(this));
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
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