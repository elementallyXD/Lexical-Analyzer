using System;

namespace OPT
{
    class LexicalAnalyzer
    {
        private struct TSymbol
        {
            public char value;
            public int attr;
        }

        private TSymbol GetSymbol(Table tables, string line, int iterator)
        {
            TSymbol result;
            
           if (iterator < line.Length) {
                result.value = line[iterator];
           }
           else{
               result.value = ' ';
           }
            result.attr = tables.GetAttribut(result.value);
        

            return result;
        }

        public void PrintTable(Parser text, Table tables)
        {
            string[] programCode = text.GetLines();
            for (int i = 0; i < programCode.Length; i++)
            {
                ushort j = 0;
                while (j < programCode[i].Length)
                {
                    Console.Write("\t{0}{1}", GetSymbol( tables, programCode[i], j).value, GetSymbol( tables, programCode[i], j).attr);
                    j++;
                }
                Console.WriteLine();
            }
        }

        public LexicalAnalyzer(Parser text, Table tables){
            string[] programCode = text.GetLines();
            TSymbol symbol;
            Console.WriteLine("\n\t\tLexical Analyzer:");
            int row = 1, column = 1;
        
            
    
            foreach (string line in programCode)
            {
                string buffer = "";
                int iterDig = 0;

                bool Suppres = true;

                bool good = true;
                bool goodWord = true;
                bool goodDig = true;
                bool notConst1 = true;
                string bufferDig = "";


                bool notConst = false;
                bool comment = false;
                
                for (int iterator = 0; iterator < line.Length; iterator++)
                {
                   
                    symbol = GetSymbol(tables, line, iterator);
                    switch (symbol.attr)
                    {
                        case 0:
                            buffer = "";
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
                            if (tables.GetConst(buffer, row, column) == -1 && notConst == false)
                            {
                                tables.ConstTabForm(buffer, row, column);
                                buffer = "";
                            }
                            else if (notConst)
                            {
                                if (tables.GetIdn(buffer, row, column) == -1)
                                {
                                    tables.IdnTabForm(buffer, row, column);
                                    buffer = "";
                                }
                            }
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
                                if (tables.GetIdn(buffer, row, column) == -1)
                                {
                                    tables.IdnTabForm(buffer, row, column);
                                    buffer = "";
                                }
                                buffer = "";
                            }
                            buffer = "";
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
                                            if (tables.GetSeparators(buffer, row, column) == -1) tables.SeparatorsForm(buffer, row, column);
                                            buffer = "";
                                            buffer += GetSymbol(tables, line, iterator).value;
                                            if (tables.GetSeparators(buffer, row, ++column) == -1) tables.SeparatorsForm(buffer, row, ++column);
                                            buffer = "";
                                        }
                                        else tables.Trace.Add("Error. \"*\" can't open com");
                                    }
                                    else tables.Trace.Add("Error. \"(\" can't open com");
                                    break;
                                case '*':
                                    tables.Trace.Add("Error. \"*\"");
                                    break;
                            }
                            break;
                        //need to fix
                        case 4:
                            column = iterator;
                            buffer += symbol.value;
                            if (tables.GetSeparators(buffer, row, column) == -1) tables.SeparatorsForm(buffer, row, column);
                            buffer = "";
                            break;
                    
                        
                        case 5:
                            column = iterator;
                            tables.Trace.Add("Illegal symbol ---" + symbol.value + "\trow " + row + "\tcolumn " + column);
                            break;
                    }
                }
                row++;
            }
            tables.Trace.ForEach(Console.WriteLine);
        }
    }
}
