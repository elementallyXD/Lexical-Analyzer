using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace OPT
{
    class Parser
    {
        private static string[] lines;

        public Parser(string filename){
            var textList = new List<string>();
            string text;
            try
            {
                using (StreamReader streamReader = new StreamReader(filename, Encoding.UTF8))
                {
                    /* Read and display lines from the file until the end of the file is reached.*/
                    while ((text = streamReader.ReadLine()) != null)
                    {
                        textList.Add(text);
                    }
                }
            }
            catch (Exception e){
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            lines = textList.ToArray();
        }

        public string[] GetLines(){
            return lines;
        }
        public void LineReader(){
            Console.WriteLine("\t\tProgram Text: ");
            foreach(string line in lines)
                Console.WriteLine(line);
        }

        ~Parser(){}
    }
}
