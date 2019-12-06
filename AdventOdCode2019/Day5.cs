using System;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOdCode2019
{
    internal class Day5 : IAdventOfCodeDay
    {
        public string CalculatePart1(string inputFile)
        {
            var programString = File.ReadAllLines(inputFile).First();
            var program = programString.Split(',').Select(int.Parse).ToArray();
            
            var result = RunProgram(ref program, 1);

            return string.Join(
                ", ",
                result.Split(new[] { Environment.NewLine }, StringSplitOptions.None));
        }

        public string CalculatePart2(string inputFile)
        {
            var programString = File.ReadAllLines(inputFile).First();
            var program = programString.Split(',').Select(int.Parse).ToArray();

            var result = RunProgram(ref program, 5);

            return string.Join(
                ", ", 
                result.Split(new[] { Environment.NewLine }, StringSplitOptions.None));
        }

        private static string RunProgram(ref int[] program, int input)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < program.Length; )
            {
                var opCode = program[i] % 100;
                var modeMem1 = program[i] / 100 % 10 == 0;
                var modeMem2 = program[i] / 1000 % 10 == 0;
                var modeMem3 = program[i] / 10000 % 10 == 0;

                if (opCode == 99)
                    return sb.ToString();

                switch (opCode)
                {
                    case 1:
                        var op11 = modeMem1 ? program[program[i + 1]] : program[i + 1];
                        var op12 = modeMem2 ? program[program[i + 2]] : program[i + 2];
                        var target1 = program[i + 3];
                        program[target1] = op11 + op12;
                        i = i + 4;
                        break;
                    case 2:
                        var op21 = modeMem1 ? program[program[i + 1]] : program[i + 1];
                        var op22 = modeMem2 ? program[program[i + 2]] : program[i + 2];
                        var target2 = program[i + 3];
                        program[target2] = op21 * op22;
                        i = i + 4;
                        break;
                    case 3:
                        var target3 = program[i + 1];
                        program[target3] = input;
                        i = i + 2;
                        break;
                    case 4:
                        var target4 = program[i + 1];
                        var res = modeMem1 ? program[target4] : target4;
                        sb.AppendLine(res.ToString());
                        i = i + 2;
                        break;
                    case 5:
                        var op51 = modeMem1 ? program[program[i + 1]] : program[i + 1];
                        var op52 = modeMem2 ? program[program[i + 2]] : program[i + 2];
                        i = op51 != 0 ? op52 : i + 3;
                        break;
                    case 6:
                        var op61 = modeMem1 ? program[program[i + 1]] : program[i + 1];
                        var op62 = modeMem2 ? program[program[i + 2]] : program[i + 2];
                        i = op61 == 0 ? op62 : i + 3;
                        break;
                    case 7:
                        var op71 = modeMem1 ? program[program[i + 1]] : program[i + 1];
                        var op72 = modeMem2 ? program[program[i + 2]] : program[i + 2];
                        var target7 = program[i + 3];
                        program[target7] = op71 < op72 ? 1 : 0;
                        i = i + 4;
                        break;
                    case 8:
                        var op81 = modeMem1 ? program[program[i + 1]] : program[i + 1];
                        var op82 = modeMem2 ? program[program[i + 2]] : program[i + 2];
                        var target8 = program[i + 3];
                        program[target8] = op81 == op82 ? 1 : 0;
                        i = i + 4;
                        break;
                }
            }

            return "error!";
        }
    }
}