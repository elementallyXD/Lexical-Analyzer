using System;
using System.Collections.Generic;

namespace OPT
{
    class Program
    {
        static void Main(string[] args)
        {
            CodeParser pr = new CodeParser("Program.txt");
            Table tb = new Table();
            tb.PrintsTables();
            pr.LineReader();
            LexicalAnalyzer lexicalAnalizer = new LexicalAnalyzer(pr,tb);
            tb.PrintsTables();
           
            Parser synt = new Parser(lexicalAnalizer.GetTokens());
            synt.Start();
            synt.PrintParser();
            synt.PrintParserErrors();

            Console.WriteLine("\n----------Code generator----------\n");
            CodeGenerator CG = new CodeGenerator(synt.getTree().root, tb.getConstants(), synt.GetProgram);
            Console.WriteLine(CG.getStr());
            if (CG.getErrorsList().Count != 0)
            {
                Console.WriteLine("\n----------Error tables----------\n");
                CG.printErrors();

            }
            //else CG.PrintLastStep();

            Console.WriteLine("\nEnd. Press any key to close...");
            Console.ReadLine();
        }
    }
}
