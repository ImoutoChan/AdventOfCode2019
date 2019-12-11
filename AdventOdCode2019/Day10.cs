using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AdventOdCode2019
{
    internal class Day10 : IAdventOfCodeDay
    {
        public string CalculatePart1(string inputFile)
        {
            var asteroids = GetAsteroids(inputFile);

            var result = asteroids.Select(x => FindAllViewable(x, asteroids)).Max();

            return result.ToString();
        }

        public string CalculatePart2(string inputFile)
        {
            var asteroids = GetAsteroids(inputFile);

            var bestLocation = asteroids.Select(x => (x, FindAllViewableOrdered(x, asteroids))).OrderByDescending(x => x.Item2.Count()).First().x;
            
            var counter = 200;

            while (true)
            {
                var result = FindAllViewableOrdered(bestLocation, asteroids);
                if (counter - result.Count > 0)
                {
                    counter -= result.Count;
                    asteroids = asteroids.Except(result.Select(x => x.Value)).ToList();
                }
                else
                {
                    var res = result[counter - 1].Value;
                    return $"{res.X} {res.Y} {res.X * 100 + res.Y}";
                }
            }

            return "Error!";
        }

        private int FindAllViewable(Asteroid forAsteroid, List<Asteroid> asteroids)
        {
            var result = 0;
            var check = new Dictionary<double, Asteroid>();
            foreach (var asteroid in asteroids)
            {
                var angle = forAsteroid.GetAngle(asteroid);
                if (!check.Any(x => Math.Abs(x.Key - angle) < double.Epsilon) 
                    && !asteroid.Equals(forAsteroid))
                {
                    result++;
                    check.Add(angle, asteroid);
                }
            }

            return result;
        }

        private List<KeyValuePair<double, Asteroid>> FindAllViewableOrdered(Asteroid forAsteroid, List<Asteroid> asteroids)
        {
            var result = 0;
            var check = new Dictionary<double, (Asteroid, double Distance)>();
            foreach (var asteroid in asteroids.Where(x => !x.Equals(forAsteroid)))
            {
                var angle = forAsteroid.GetAngleDegree(asteroid);
                if (!check.Any(x => Math.Abs(x.Key - angle) < double.Epsilon))
                {
                    result++;
                    check.Add(angle, (asteroid, forAsteroid.GetLength(asteroid)));
                }
                else
                {
                    var distance = forAsteroid.GetLength(asteroid);
                    var first = check.First(x => Math.Abs(x.Key - angle) < double.Epsilon);

                    if (distance < first.Value.Item2)
                        check[first.Key] = (asteroid, distance);
                }
            }

            var res = check.OrderBy(x => x.Key).Select(x => new KeyValuePair<double, Asteroid>(x.Key, x.Value.Item1))
                .ToList();

            return res;
        }

        private static List<Asteroid> GetAsteroids(string inputFile)
        {
            var lines = File.ReadAllLines(inputFile);

            //            var lines = @".#..#
            //.....
            //#####
            //....#
            //...##".Split(new[] { Environment.NewLine }, StringSplitOptions.None);            

            //            var lines = @".#..##.###...#######
            //##.############..##.
            //.#.######.########.#
            //.###.#######.####.#.
            //#####.##.#.##.###.##
            //..#####..#.#########
            //####################
            //#.####....###.#.#.##
            //##.#################
            //#####.##.###..####..
            //..######..##.#######
            //####.##.####...##..#
            //.#####..#.######.###
            //##...#.##########...
            //#.##########.#######
            //.####.#.###.###.#.##
            //....##.##.###..#####
            //.#.#.###########.###
            //#.#.#.#####.####.###
            //###.##.####.##.#..##".Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            var result = new List<Asteroid>();
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                for (int j = 0; j < line.Length; j++)
                {
                    if (line[j] == '#')
                        result.Add(new Asteroid(j, i));
                }
            }

            return result;
        }
    }



    [DebuggerDisplay("X = {X} Y = {Y}")]
    internal struct Asteroid : IEquatable<Asteroid>
    {
        public int X { get; }
        public int Y { get; }

        public Asteroid(int x, int y)
        {
            X = x;
            Y = y;
        }

        public double GetAngle(Asteroid target)
        {
            var angle = Math.Atan2(target.Y - Y, target.X - X);

            if (angle < 0)
            {
                angle += Math.Ceiling(-angle / 360) * 360;
            }

            return angle;
        }

        public double GetAngleDegree(Asteroid target)
        {
            double xDiff = target.X - X;
            double yDiff = target.Y - Y;
            var res = Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI + 90;
            if (res < 0)

                res += 360;

            return res;
        }

        public double GetLength(Asteroid target)
        {
            double xDiff = target.X - X;
            double yDiff = target.Y - Y;
            return Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
        }

        public bool Equals(Asteroid other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is Asteroid other && Equals(other);
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