using System;

namespace OPT
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser pr = new Parser("Program.txt");
            Table tb = new Table();
            tb.PrintsTables();
            pr.LineReader();
            LexicalAnalyzer lexicalAnalizer = new LexicalAnalyzer(pr,tb);
            tb.PrintsTables();


            Console.WriteLine("End. Press any key to close...");
            Console.ReadLine();
        }
    }
}
