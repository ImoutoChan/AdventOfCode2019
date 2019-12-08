using System.IO;
using System.Linq;

namespace AdventOdCode2019
{
    internal class Day7 : IAdventOfCodeDay
    {
        public string CalculatePart1(string inputFile)
        {
            var program = GetProgram(inputFile);

            var max = 0;
            var i = new int[5];
            var lastOutput = 0;
            for (i[0] = 0; i[0] < 5; i[0]++)
            for (i[1] = 0; i[1] < 5; i[1]++)
            for (i[2] = 0; i[2] < 5; i[2]++)
            for (i[3] = 0; i[3] < 5; i[3]++)
            for (i[4] = 0; i[4] < 5; i[4]++)
            {
                if (i.Distinct().Count() < 5)
                    continue;

                
                for (int ampIndex = 0; ampIndex < 5; ampIndex++)
                {
                    var a = new IntCodeRunner(program);
                    a.Run(i[ampIndex]);
                    lastOutput = (int)a.Run(lastOutput);
                }

                if (lastOutput > max)
                {
                    max = lastOutput;
                }
                lastOutput = 0;
            }

            return max.ToString();
        }

        public string CalculatePart2(string inputFile)
        {
            var program = GetProgram(inputFile);

            var max = 0;
            var i = new int[5];
            var lastOutput = 0;
            var start = 5;
            var end = 10;
            for (i[0] = start; i[0] < end; i[0]++)
            for (i[1] = start; i[1] < end; i[1]++)
            for (i[2] = start; i[2] < end; i[2]++)
            for (i[3] = start; i[3] < end; i[3]++)
            for (i[4] = start; i[4] < end; i[4]++)
            {
                if (i.Distinct().Count() < 5)
                    continue;
                
                var a = new IntCodeRunner(program);
                var b = new IntCodeRunner(program);
                var c = new IntCodeRunner(program);
                var d = new IntCodeRunner(program);
                var e = new IntCodeRunner(program);
                a.Run(i[0]);
                b.Run(i[1]);
                c.Run(i[2]);
                d.Run(i[3]);
                e.Run(i[4]);
                int? result = null;

                lastOutput = 0;
                do
                {
                    try
                    {
                        var resultA = a.Run(lastOutput);
                        var resultB = b.Run((int)resultA);
                        var resultC = c.Run((int)resultB);
                        var resultD = d.Run((int)resultC);
                        var resultE = e.Run((int)resultD);
                        result = resultE;

                        if (result.HasValue)
                        {
                            lastOutput = result.Value;
                        }
                    }
                    catch 
                    {
                        //Console.WriteLine(exception);
                        break;
                        //throw;
                    }

                } while (result.HasValue);

                if (lastOutput > max)
                {
                    max = lastOutput;
                }
            }

            return max.ToString();
        }

        private static int[] GetProgram(string inputFile)
        {
            var programString = File.ReadAllLines(inputFile).First();
            var program = programString.Split(',').Select(int.Parse).ToArray();
            return program;
        }
    }

    public class IntCodeRunner
    {
        private readonly int[] _program;
        private int i = 0;


        public IntCodeRunner(int[] program)
        {
            _program = program;
        }

        public int? Run(int input)
        {
            var program = _program;
            var inputUsed = false;

            while (i < _program.Length)
            {
                var opCode = program[i] % 100;
                var modeMem1 = program[i] / 100 % 10 == 0;
                var modeMem2 = program[i] / 1000 % 10 == 0;
                var modeMem3 = program[i] / 10000 % 10 == 0;

                if (opCode == 99)
                    return null;

                switch (opCode)
                {
                    case 1:
                        var target1 = program[i + 3];
                        program[target1] = GetOp(modeMem1, i + 1) + GetOp(modeMem2, i + 2);
                        i += 4;
                        break;
                    case 2:
                        var target2 = program[i + 3];
                        program[target2] = GetOp(modeMem1, i + 1) * GetOp(modeMem2, i + 2);
                        i += 4;
                        break;
                    case 3:
                        if (inputUsed)
                            return null;

                        var target3 = program[i + 1];
                        program[target3] = input;
                        inputUsed = true;
                        i += 2;
                        break;
                    case 4:
                        var target4 = program[i + 1];
                        var res = modeMem1 ? program[target4] : target4;
                        i += 2;
                        return res;
                    case 5:
                        i = GetOp(modeMem1, i + 1) != 0
                            ? GetOp(modeMem2, i + 2) : i + 3;
                        break;
                    case 6:
                        i = GetOp(modeMem1, i + 1) == 0
                            ? GetOp(modeMem2, i + 2) : i + 3;
                        break;
                    case 7:
                        var target7 = program[i + 3];
                        program[target7] = GetOp(modeMem1, i + 1) < GetOp(modeMem2, i + 2) ? 1 : 0;
                        i += 4;
                        break;
                    case 8:
                        var target8 = program[i + 3];
                        program[target8] = GetOp(modeMem1, i + 1) == GetOp(modeMem2, i + 2) ? 1 : 0;
                        i += 4;
                        break;
                }
            }

            return null;

            int GetOp(bool modeMem, int position)
                => modeMem ? program[program[position]] : program[position];
        }
    }
}