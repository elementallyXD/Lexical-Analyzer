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
                        case 4:
                            switch (symbol.value)
                            {
                                case '[':
                                    column = iterator;
                                    buffer += symbol.value;
                                    iterator++;
                                    if (GetSymbol(tables, line, iterator).value == '+')
                                    {
                                        buffer += GetSymbol(tables, line, iterator).value;
                                        //if (tables.GetSeparators(buffer, row, column) == -1) tables.SeparatorsForm(buffer, row, column);
                                        //buffer = "";
                                        int iterNum = iterator + 3;
                                        while ((iterator < iterNum))
                                        {
                                            iterator++;
                                            if (GetSymbol(tables, line, iterator).attr == 1)
                                            //{
                                                if (iterDig < 3)
                                                {
                                                    bufferDig += GetSymbol(tables, line, iterator).value;
                                                    iterDig++;
                                                }
                                                else good = false;
                                        }
                                            else
                                            {
                                            notConst1 = false;
                                            goodWord = false;
                                            iterator--;
                                            //if (tables.GetIdn(buffer, row, column) == -1)
                                            //{
                                            //    tables.IdnTabForm(buffer, row, column);
                                            //   // buffer = "";
                                        }
                                            //buffer = "";
                                            //tables.Trace.Add("Error. [+NUM+]");
                                            break;
                                        }
                                    }
                                        if (!goodWord && (iterDig != 3))
                                        {
                                            if (tables.GetSeparators(buffer, row, column) == -1) tables.SeparatorsForm(buffer, row, column);
                                            buffer = "";

                                            if (tables.GetConst(bufferDig, row, column) == -1 && notConst1 == false)
                                            {
                                                tables.ConstTabForm(bufferDig, row, column);
                                                buffer = "";
                                            }
                                            else if (notConst1)
                                            {
                                                if (tables.GetIdn(bufferDig, row, column) == -1)
                                                {
                                                    tables.IdnTabForm(bufferDig, row, column);
                                                    buffer = "";
                                                }
                                            }
                                        }
                                        else buffer += bufferDig;

                                        iterator++;
                                        symbol = GetSymbol(tables, line, iterator);
                                        if ((iterDig == 3) && symbol.value == '+' && good && iterator > line.Length - 3 && GetSymbol(tables, line, --iterator).attr == 1)
                                        {
                                            buffer += GetSymbol(tables, line, ++iterator).value;
                                            iterator++;
                                            if (GetSymbol(tables, line, iterator).value == ']')
                                            {
                                                buffer += GetSymbol(tables, line, iterator).value;
                                                goodWord = true;
                                            }
                                            else
                                            {
                                                goodWord = false;
                                                if (tables.GetSeparators(buffer, row, column) == -1) tables.SeparatorsForm(buffer, row, column);
                                                buffer = "";
                                                tables.Trace.Add("Error. Must be ]");
                                            }
                                        }
                                        else if (symbol.value != '+' && symbol.attr != 1)
                                        {
                                            goodWord = false;
                                            if (tables.GetSeparators(buffer, row, column) == -1) tables.SeparatorsForm(buffer, row, column);
                                            buffer = "";
                                            iterator = line.Length - 1;
                                            tables.Trace.Add("Error. Must be +");
                                        }
                                        else
                                        {
                                            goodWord = false;
                                            if (tables.GetSeparators(buffer, row, column) == -1) tables.SeparatorsForm(buffer, row, column);
                                            buffer = "";
                                            iterator = line.Length - 1;
                                            tables.Trace.Add("Error. [+NUM+]");
                                        }
                                    }
                                    else {
                                        if (tables.GetSeparators(buffer, row, column) == -1) tables.SeparatorsForm(buffer, row, column);
                                        tables.Trace.Add("Error. [. Need first '+'");
                                        goodWord = false;
                                        buffer = "";
                                        iterator += line.Length - 2;
                                    }
                                    if (goodWord){ 
                                        if (tables.GetSeparators(buffer, row, column) == -1) tables.SeparatorsForm(buffer, row, column);    
                                    }
                                    buffer = "";
                                    break;

                                default:
                                    column = iterator;
                                    buffer += symbol.value;
                                    if (tables.GetSeparators(buffer, row, column) == -1) tables.SeparatorsForm(buffer, row, column);
                                    buffer = "";
                                    break;
                            }
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
