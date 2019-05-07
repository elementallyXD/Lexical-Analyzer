using System;
using System.Collections.Generic;
using System.Text;

namespace OPT
{
    class Token
    {
        private readonly int tokenCode;
        private readonly int row;
        private readonly int column;
        private readonly string line;
        
        public Token(int tokenCode, int row, int column, string line)
        {
            this.tokenCode = tokenCode;
            this.row = row;
            this.column = column;
            this.line = line;
        }
        
        public void GetInfo()
        {
            Console.WriteLine("------------------------------");
            Console.WriteLine("Code - " + tokenCode);
            Console.WriteLine("Row - " + row);
            Console.WriteLine("Column - " + column);
            Console.WriteLine("Line - " + line);
            Console.WriteLine("------------------------------");
        }
        
        public int GetCode() {  return tokenCode; }
        public int GetRow() { return row; }
        public int GetColumn() { return column;}
        public string GetLine() { return line; }
    }
}
