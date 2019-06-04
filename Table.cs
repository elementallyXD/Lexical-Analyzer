using System;
using System.Collections.Generic;

namespace OPT
{
    class Table
    {
        const int STARTING_SEP_NUMBER = 300;    // sep
        const int STARTING_CONST_NUMBER = 400;  // const
        const int STARTING_IDN_NUMBER = 500;    // idn 
        const int STARTING_KEY_NUMBER = 700;    // key word

        public Table () {
            InitTables();
            constants = new Dictionary<string, int>();
        }

        private int[] attributes = new int[256];
        private string[] keyTab = new string[100];
        private string[] constTab = new string[100];
        private Dictionary<string, int> constants;
        public Dictionary<string, int> getConstants() => constants;
        private string[] idnTab = new string[100];
        private string[] sepTab = new string[100];

        public List<string> Trace = new List<string>();

        public int GetIdn(string str, int row, int column)
        {
            for (int i = 0; i < 100; i++)
                if (idnTab[i] == str) return i + STARTING_IDN_NUMBER;
            
            return -1;
        }

        public int IdnTabForm(string str, int row, int column)
        {
            int i = 0;
            while (idnTab[i] != null) i++;
            Trace.Add("IDN#" + (i + STARTING_IDN_NUMBER) + " Str= " + str + "\trow " + row + "\tcolumn " + column);
            idnTab[i] = str;
            return i + STARTING_IDN_NUMBER;
        }

        //  search in table sep
        public int GetSeparators(string str, int row, int column)
        {
            int i;
            for (i = 0; i < 100; i++)
                if (sepTab[i] == str)
                {
                    Trace.Add("SEP#" + (i + STARTING_SEP_NUMBER) + " Str= " + str + "   \trow " + row + "\tcolumn " + column);
                    return i + STARTING_SEP_NUMBER;
                }
            return -1;
        }

        public int GetOldSep(string str){
            for (int i = 0; i < 100; i++)
                if (sepTab[i] == str)
                    return i + STARTING_SEP_NUMBER;
            return -1;
        }

        //  table sep
        public int SeparatorsForm(string str, int row, int column)
        {
            int i = 0;
            while (sepTab[i] != null) i++;
            Trace.Add("SEP#" + (i + STARTING_SEP_NUMBER) + " Str= " + str + "  \trow " + row + " \tcolumn " + column);
            sepTab[i] = str;
            return i + STARTING_SEP_NUMBER;
        }

        public int GetConst(string str, int row, int column)
        {
            int i;
            for (i = 0; i < 100; i++)
                if (constTab[i] == str) return i + STARTING_CONST_NUMBER;
            return -1;
        }

        public int ConstTabForm(string str, int row, int column)
        {
            int i = 0;
            bool flag = false;
            while (constTab[i] != null)
            {
                if (constTab[i] == str) { 
                    flag = true;
                    break;
                }
                i++;
            }
            if (!flag){ 
                Trace.Add("CNST#" + (i + STARTING_CONST_NUMBER) + " Str= " + str + "  \trow " + row + "\tcolumn " + column);
                constTab[i] = str;
                constants.Add(str, STARTING_CONST_NUMBER + i);
                return i + STARTING_CONST_NUMBER;
            }
            else {
                Trace.Add("CNST#" + (i + STARTING_CONST_NUMBER) + " Str= " + str + "  \trow " + row + "\tcolumn " + column);
                return i + STARTING_CONST_NUMBER;
            }
        }

        //  search in table key words
        public int KeyTabSearch(string str, int row, int column)
        {
            int i;
            for (i = 0; i < 100; i++)
                if (keyTab[i] == str) {
                    Trace.Add("Key#" + (i + STARTING_KEY_NUMBER) + " Str= "+ str + "\trow " + row + "\tcolumn " + column);
                    return i + STARTING_KEY_NUMBER;
                }
            return -1;
        }

        public int GetKeyPos(string str){
            for (int i = 0; i < 100; i++)
                if (keyTab[i] == str)
                {
                    return i + STARTING_KEY_NUMBER;
                }
            return -1;
        }

        public int GetAttribut(int iterator){
             return attributes[iterator];
        }

        public void PrintsTables()
        {
            //PrintAtributes();
            PrintTable(sepTab, STARTING_SEP_NUMBER, "Sep");
            PrintTable(constTab, STARTING_CONST_NUMBER, "Const");
            PrintTable(idnTab, STARTING_IDN_NUMBER, "Identification");
            PrintTable(keyTab, STARTING_KEY_NUMBER, "Key Words");
        }

        private void InitAttributes()
        {
            // Filling attribute table
            for (int i = 0; i <= 255; i++)
                attributes[i] = 5; // All symbols are errors by default

            attributes[9] = 0;  // Tab
            attributes[11] = 0;
            attributes[10] = 0; // Newline
            attributes[13] = 0; // Endline
            attributes[32] = 0; // Space

            attributes[40] = 3; // (
            attributes[41] = 3; // )
            attributes[42] = 3; // *


            attributes[46] = 4; // .
            attributes[44] = 4; // ,
            attributes[59] = 4; // ;
            attributes[91] = 4; //[
            //attributes[93] = 4; //]
            //attributes[43] = 4; //+

            for (int i = 48; i <= 57; i++)
                attributes[i] = 1; // constants

            for (int i = 65; i <= 90; i++)
                attributes[i] = 2; // identificators
        }
        private void InitKeyTab()
        {
            for (int i = 0; i < keyTab.Length; i++)
                keyTab[i] = null;

            // Filling keyword table
            keyTab[0] = "PROGRAM";
            keyTab[1] = "BEGIN";
            keyTab[2] = "END";
            keyTab[3] = "PROCEDURE";
        }
        private void InitIdnTab()
        {
            for (int i = 0; i < idnTab.Length; i++)
                idnTab[i] = null;
        }
        private void InitSepTab()
        {
            for (int i = 0; i < sepTab.Length; i++)
                sepTab[i] = null;
            sepTab[40] = "(";
            sepTab[41] = ")";
            sepTab[44] = ",";
            sepTab[46] = ".";
            sepTab[59] = ";";
        }
        private void InitConstTab()
        {
            for (int i = 0; i < constTab.Length; i++)
                constTab[i] = null;
        }
        private void InitTables()
        {
            InitKeyTab();
            InitAttributes();
            InitIdnTab();
            InitSepTab();
            InitConstTab();
        }

        public void PrintAtributes()
        {
            Console.WriteLine("________________________________________");
            Console.WriteLine("Attributes Table");
            for (int i = 0; i < attributes.Length; i++)
            {
                if (attributes[i] == 5) continue;
                else Console.WriteLine("{0} | {1}", i, attributes[i]);
            }
            Console.WriteLine("___|____________________________________");
        }
       
        public void PrintTable(string[] table, int number, string name)
        {
            Console.WriteLine("________________________________________");
            Console.WriteLine("{0} Table", name);
            for (int i = 0; i < table.Length; i++)
            {
                if (table[i] == null) continue;
                else Console.WriteLine("{0} | {1}", i + number, table[i]);
            }
            Console.WriteLine("____|___________________________________");
        }

        //public void PrintTable(Parser text, Table tables)
        //{
        //    string[] programCode = text.GetLines();
        //    for (int i = 0; i < programCode.Length; i++)
        //    {
        //        ushort j = 0;
        //        while (j < programCode[i].Length)
        //        {
        //            Console.Write("\t{0}{1}", GetSymbol( tables, programCode[i], j).value, GetSymbol( tables, programCode[i], j).attr);
        //            j++;
        //        }
        //        Console.WriteLine();
        //    }
        //}
    }
}
