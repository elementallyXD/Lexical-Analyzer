using System;
using System.Collections.Generic;

namespace OPT
{
    class SemanticErrorException : Exception
    {
        public SemanticErrorException() : base()
        {

        }
        public SemanticErrorException(string str) : base(str)
        {

        }
    }

    class CodeGenerator
    {
        private TreeNode node = null;
        private string code = null;
        public List<string> getErrorsList() => generatorErrors;
        private int labelCounter = 0;
        private Buffer buffer = null;
        private List<string> generatorErrors = null;
        private List<int> identifiers = null;
        private Dictionary<string, int> constants = null;
        private Dictionary<int, string> comparsionOperators = null;
        private int programName;
        private bool prog;
        
        public CodeGenerator(TreeNode node, Dictionary<string, int> constants, bool prog)
        {
            this.node = node;
            buffer = new Buffer();
            code = "";
            this.prog = prog;
            this.constants = constants;
            InitTables();
            identifiers = new List<int>();
            generatorErrors = new List<string>();
            try
            {
                Generate(this.node);
            }
            catch (SemanticErrorException)
            {
                return;
            }
            
        }

        public void PrintFirstStep(ref string code) {
            code += "Push ebp\n";
            code += "Mov esp, ebp\n";
        }

        public void PrintLastStep(ref string code){
            code += "Pop ebp\n";
            code += "Ret\n";
        }
        private void InitTables()
        {
            comparsionOperators = new Dictionary<int, string>
            {
                {703,"JGE " },
                {701,"JNE " },
                {702,"JLE " },
                {62,"JG " },
                {60,"JL " },
                {61,"JE " },

            };

        }
        public void printErrors()
        {
            foreach (var item in generatorErrors)
            {
                Console.WriteLine("Semantic: " + item);
            }
        }
        public string getStr()
        {
            return code;
        }
        public void Generate(TreeNode node)
        {
            switch (node.data)
            {
                case "<Signal-Program>":        //<signal-program> --> <program>
                    Generate(node.children[0]);
                    break;
                case "<Program>":               //<program> --> PROGRAM <procedure-identifier> ; < block >. || PROCEDURE  <procedure-identifier> <parameterst-list>; <block>;
                    if (prog)
                    {
                        Generate(node.children[1]);
                        var identifier = buffer.getBuffer();
                        var identifierCode = buffer.getCode();
                        programName = identifierCode;
                        code += identifier + "  SEGMENT\n";
                        PrintFirstStep(ref code);
                        Generate(node.children[3]);
                        PrintLastStep(ref code);
                        code += identifier + "  ENDS";
                    }
                    else {
                        Generate(node.children[1]);
                        var identifier = buffer.getBuffer();
                        var identifierCode = buffer.getCode();
                        programName = identifierCode;
                        code += identifier + "  proc\n";
                        PrintFirstStep(ref code);
                        Generate(node.children[2]);
                        Generate(node.children[4]);
                        PrintLastStep(ref code);
                        code += identifier + "  endp";
                    }
                    break;
                case "<Block>":                 //<block> --> <declarations> BEGIN < statements - list > END
                    Generate(node.children[0]);
                    Generate(node.children[2]);
                    break;
                case "<Statements List>":       // <Statements List> --> <empty>
                    if (node.children[0].data == "<Empty>")
                    {
                        code += "   NOP\n";
                    }
                    break;
                case "<declarations>":          //<declarations> --> <lebel-declarations>
                    Generate(node.children[0]);
                    break;
                case "<lable-declarations>":    // LABEL <uns-int><label-list> || <empty>
                    if (node.children[0].data == "<Empty>")
                    {
                        code += "   NOP\n";
                    }
                    else {
                        Generate(node.children[1]);
                        Generate(node.children[2]);
                    }
                    break;
                case "<lable-list>":            // , <uns-int><labels-list> || <empty>
                    if (node.children[0].data == "<Empty>")
                    {
                        code += "   NOP\n";
                    }
                    else {
                        var label = buffer.getBuffer();
                        Generate(node.children[1]);
                        var localBuffer = buffer.getBuffer();
                        var localBufferCode = buffer.getCode();
                        if (localBuffer == label) {
                            generatorErrors.Add("The same tag name");
                            throw new SemanticErrorException("Error");
                        }
                        Generate(node.children[2]);                    
                    }
                    break;
                case "<Procedure Identifier>":  //< Procedure Identifier > --><indefier>
                    Generate(node.children[0]);
                    break;
                case "<parameters-List>":       // ( <decl-list> ) || empty
                    if (node.children[0].data == "<Empty>")
                    {
                        code += "   NOP\n";
                    }
                    else
                    {
                        Generate(node.children[1]);
                    }
                    break;
                case "<declarations-List>":     // ->> <empty>
                    if (node.children[0].data == "<Empty>")
                    {
                        code += "   NOP\n";
                    }
                    else {
                        code += " ERROR. Must be <Empty>";
                    }
                    break;
                case "<Unsigned Integer>":      //<digit> <digString>
                    buffer.Set(node.children[0].data, node.children[0].code);
                    break;
                case "<Identifier>":            // <letter><string>
                    buffer.Set(node.children[0].data, node.children[0].code);
                    break;
            }
        }
    }


    class Buffer
    {
        private string buffer;
        private int code;
        public Buffer()
        {
            buffer = "";
        }
        public void Set(string value, int code = 0)
        {
            buffer = value;
            this.code = code;
        }
        public string getBuffer() => buffer;
        public int getCode() => code;
    }
}
