using System;
using System.Collections.Generic;

namespace OPT
{
    class LexicalAnalyzer
    {
        enum TokenType
        {
            DIGIT,
            LETTER,
            WHITESPACE,
            DELIMETER,
            COM_PARENTH,
            COM_ASTERISK,
        }
        private List<Token> tokens;

        private struct TSymbol
        {
            public char value;
            public int attr;
        }

        private TSymbol GetSymbol(Table tables, string line, int iterator)
        {
            TSymbol result;
            
           if (iterator < line.Length) result.value = line[iterator];
           else result.value = ' ';
           
            result.attr = tables.GetAttribut(result.value);
            return result;
        }

        private void AddToken(int code, int row, int column, string line)
        {
            tokens.Add(new Token(code, row, column, line));
        }

        private string TokenizeIdn(string word, ref int row, ref int column, Table tables){
            AddToken(tables.IdnTabForm(word, row, column), row, column, word);
            //tables.IdnTabForm(word, row, column);
            return null;
        }

        private string TokenizeConst(string word, ref int row, ref int column, Table tables){
            AddToken(tables.ConstTabForm(word, row, column), row, column, word);
            return null;
        }

        private string TokenizeSep(string word, ref int row, ref int column, Table tables){
            AddToken(tables.SeparatorsForm(word, row, column), row, column, word);
            return null;
        }

        private string TokenizeOldSep(string word, ref int row, ref int column, Table tables)
        {
            //int pos = tables.GetOldSep(word);
            AddToken(tables.GetOldSep(word), row, column, word);
            return null;
        }

        private string TokenizeKeyWord(string word, ref int row, ref int column, Table tables)
        {
            int pos = tables.GetKeyPos(word);
            if (pos != -1)
                AddToken(pos, row, column, word);
            return null;
        }

        public LexicalAnalyzer(CodeParser text, Table tables){
            string[] programCode = text.GetLines();
            TSymbol symbol;

            tokens = new List<Token>();

            Console.WriteLine("\n\t\tLexical Analyzer:");
            int row = 1, column = 1;
    
            foreach (string line in programCode)
            {
                string buffer = "";
                //int iterDig = 0;

                //bool Suppres = true;

                //bool good = true;
                //bool goodWord = true;
                //bool goodDig = true;
                //bool notConst1 = true;
                //string bufferDig = "";


                bool notConst = false;
                bool comment = false;
                
                for (int iterator = 0; iterator < line.Length; iterator++)
                {
                    symbol = GetSymbol(tables, line, iterator);
                    switch (symbol.attr)
                    {
                        case 0:
                            buffer = null;
                            break;

                        case 1:
                            column = iterator;
                            while (iterator < line.Length ){
                                symbol = GetSymbol(tables, line, iterator);
                                if (symbol.attr == 1)
                                {
                                    buffer += symbol.value;
                                    iterator++;
                                }
                                else if (symbol.attr == 2)
                                {
                                    notConst = true;
                                    buffer += symbol.value;
                                    iterator++;
                                }
                                else
                                {
                                    iterator--;
                                    break;
                                }
                            }
                            if (tables.GetConst(buffer, row, column) == -1 && notConst == false) buffer = TokenizeConst(buffer, ref row, ref column, tables);
                            else if (notConst) 
                                if (tables.GetIdn(buffer, row, column) == -1) buffer = TokenizeIdn(buffer,ref row, ref column, tables);
                            
                            break;
                        case 2:
                            column = iterator;
                            while (iterator < line.Length)
                            {
                                symbol = GetSymbol(tables, line, iterator);
                                if (symbol.attr == 2)
                                {
                                    buffer += symbol.value;
                                    iterator++;
                                }
                                else
                                {
                                    iterator--;
                                    break;
                                }
                            }
                            if (tables.KeyTabSearch(buffer, row, column) == -1){
                                if (tables.GetIdn(buffer, row, column) == -1) buffer = TokenizeIdn(buffer, ref row, ref column, tables);
                                else
                                {
                                    buffer = null;
                                }
                            }
                            else {
                                buffer = TokenizeKeyWord(buffer, ref row, ref column, tables);
                            }
                            buffer = null;
                            break;
                        case 3:
                            switch (symbol.value)
                            {
                                case '(':
                                    column = iterator; 
                                    if (iterator + 1 < line.Length) {
                                        iterator++;
                                        if (GetSymbol(tables, line, iterator).value == '*')
                                        {
                                            if (iterator + 1 < line.Length)
                                            {
                                                while (iterator < line.Length)
                                                {
                                                    iterator++;
                                                    if (GetSymbol(tables, line, iterator).value == '*' && iterator > line.Length - 3)
                                                    {
                                                        if (iterator + 1 < line.Length)
                                                        {
                                                            iterator++;
                                                            if (GetSymbol(tables, line, iterator).value == ')')
                                                            {
                                                                comment = true;
                                                                break;
                                                            }
                                                            else tables.Trace.Add("Error. \")\" can't close com");
                                                        }
                                                        else tables.Trace.Add("Error. \"*\" can't close com");
                                                    }
                                                }
                                                if (comment == false) tables.Trace.Add("Error. \")\" can't close com");
                                            }
                                            else tables.Trace.Add("Error. \"*\" can't close com");
                                        }
                                        else if (GetSymbol(tables, line, iterator).value == ')')
                                        {
                                            buffer += symbol.value;
                                            if (tables.GetSeparators(buffer, row, column) == -1) buffer = TokenizeSep(buffer, ref row, ref column, tables);
                                            buffer = null;
                                            buffer += GetSymbol(tables, line, iterator).value;
                                            if (tables.GetSeparators(buffer, row, ++column) == -1) buffer = TokenizeSep(buffer, ref row, ref column, tables);
                                        }
                                        else tables.Trace.Add("Error. \"*\" can't open com");
                                    }
                                    else tables.Trace.Add("Error. \"(\" can't open com");
                                    break;
                                case '*':
                                    tables.Trace.Add("Error. \"*\"");
                                    break;
                            }
                            buffer = null;
                            break;
                        case 4:
                            column = iterator;
                            buffer += symbol.value;
                            if (tables.GetSeparators(buffer, row, column) == -1) buffer = TokenizeSep(buffer, ref row, ref column, tables);
                            else {
                                buffer = TokenizeOldSep(buffer, ref row, ref column, tables);
                            }
                            break;                     
                        default:
                            column = iterator;
                            tables.Trace.Add("Illegal symbol ---" + symbol.value + "\trow " + row + "\tcolumn " + column);
                            break;
                    }
                }
                row++;
            }
            tables.Trace.ForEach(Console.WriteLine);
            PrintTokens();
        }

        private void PrintTokens(){
            foreach (Token token in tokens)
            {
                token.GetInfo();
            }
        }

        public List<Token> GetTokens() => tokens;
    }
}
