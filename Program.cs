using System;

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


            Console.WriteLine("End. Press any key to close...");
            Console.ReadLine();
        }
    }
}
