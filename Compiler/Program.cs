﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Compiler.Parser;
using Compiler.ThreeAddrCode.Nodes;
using Compiler.ThreeAddrCode.Expressions;
using Compiler.ThreeAddrCode.CFG;
using Compiler.ThreeAddrCode;

namespace Compiler
{
    class Program
    {
        public static void Main()
        {
            TaCodeTest();
//            ASTTest();
        }

        private static void ASTTest()
        {
            Console.OutputEncoding = Encoding.UTF8;
            string fileName = @"..\..\sample.txt";
            try
            {
                string text = File.ReadAllText(fileName);

                var scanner = new Scanner();
                scanner.SetSource(text, 0);

                var parser = new Parser.Parser(scanner);

                var b = parser.Parse();
                Console.WriteLine(!b ? "Ошибка" : "Синтаксическое дерево построено");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Файл {fileName} не найден");
            }
            catch (LexException e)
            {
                Console.WriteLine($"Лексическая ошибка. {e.Message}");
            }
            catch (SyntaxException e)
            {
                Console.WriteLine($"Синтаксическая ошибка. {e.Message}");
            }

            Console.ReadLine();
        }

        private static void TaCodeTest()
        {
            var taCode = new TACode();

            var ass1 = new Assign
            {
                Left = new IntConst(3),
                Operation = OpCode.Minus,
                Right = new IntConst(5),
                Result = new Var()
            };

            var ass2 = new Assign
            {
                Left = new IntConst(10),
                Operation = OpCode.Plus,
                Right = new IntConst(2),
                Result = new Var()
            };

            var ass3 = new Assign
            {
                Operation = OpCode.Minus,
                Right = new IntConst(1),
                Result = new Var()
            };

            var ifgt1 = new IfGoto
            {
                Condition = new IntConst(1),
                TargetLabel = ass3.Label
            };
            ass3.IsLabeled = true; //На этот оперетор мы переходим по условию 

            var ass4 = new Assign
            {
                Left = ass3.Result,
                Operation = OpCode.Plus,
                Right = new IntConst(1999),
                Result = new Var()
            };

            var ifgt2 = new IfGoto
            {
                Condition = new IntConst(2),
                TargetLabel = ass2.Label
            };
            ass2.IsLabeled = true; //На этот оперетор мы переходим по условию 

            var ass5 = new Assign
            {
                Left = new IntConst(7),
                Operation = OpCode.Mul,
                Right = new IntConst(4),
                Result = new Var()
            };

            var ass6 = new Assign
            {
                Left = new IntConst(100),
                Operation = OpCode.Div,
                Right = new IntConst(25),
                Result = new Var()
            };

            taCode.AddNode(ass1);
            taCode.AddNode(ass2);
            taCode.AddNode(ass3);
            taCode.AddNode(ifgt1);
            taCode.AddNode(ass4);
            taCode.AddNode(ifgt2);
            taCode.AddNode(ass5);
            taCode.AddNode(ass6);

            Console.WriteLine($"TA Code\n: {taCode}");

            var bBlocks = taCode.CreateBasicBlockList();
            foreach (var bbl in bBlocks)
            {
                Console.WriteLine($"Block[{bbl.BlockId}]:");
                var bblCodeStr = bbl.CodeList.Aggregate("", (s, node) => s + node.ToString() + Environment.NewLine);
                Console.WriteLine($"{bblCodeStr}\n");
            }
        }
    }
}