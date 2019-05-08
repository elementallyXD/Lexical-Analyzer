using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPT
{

    class ParserErrorException : Exception
    {
        public ParserErrorException(string str) : base(str)
        {

        }

        public ParserErrorException() : base()
        {

        }
    }
    class Parser
    {
        private List<Token> tokens;
        private Tree syntaxTree;
        private List<string> parserErrors;
        private Stack<Position> position;
        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
            parserErrors = new List<string>();
            position = new Stack<Position>();
        }
        private class Position
        {
            private readonly int row;
            private readonly int column;
            public Position(int row, int column)
            {
                this.row = row;
                this.column = column;
            }
            public int GetRow(){ return row;}
            public int GetColumn() { return column; }
        }
        
        public void PrintParser() 
        {
            if (syntaxTree != null) syntaxTree.PrintTree();
        }
        
        public void PrintParserErrors()
        {
            foreach (var item in parserErrors)
            {
                Console.WriteLine("Syntax: " + item);
            }
        }
        
        public void Start()
        {
            try
            {
                SignalProgram();
            }
            catch (ParserErrorException)
            {
                return;
            }
        }
        
        private void SignalProgram()
        {
            if (tokens.Count == 0)
            {
                throw new ParserErrorException("Exception");
            }
            syntaxTree = new Tree("<Signal-Program>");
            Program(syntaxTree.root);
        }
        private void Program(TreeNode parent)
        {
            TreeNode childNode = new TreeNode("<Program>");
            parent.AddChild(childNode);
            bool program = false;
            if (tokens.Count != 0 && tokens[0].GetLine().ToUpper() == "PROGRAM")
            {
                childNode.AddChild(new TreeNode(tokens[0].GetLine() + " ( Code: " + tokens[0].GetCode() + ")"));
                position.Push(new Position(tokens[0].GetRow(), tokens[0].GetColumn()));
                tokens.RemoveAt(0);
                ProcedureIdentifier(childNode);
                program = true;
            }
            else if (tokens.Count != 0 && tokens[0].GetLine().ToUpper() == "PROCEDURE")
            {
                childNode.AddChild(new TreeNode(tokens[0].GetLine() + " ( Code: " + tokens[0].GetCode() + ")"));
                position.Push(new Position(tokens[0].GetRow(), tokens[0].GetColumn()));
                tokens.RemoveAt(0);
                ProcedureIdentifier(childNode);
                ParametersList(childNode);
            }
            else
            {
                parserErrors.Add($"Expected keyword PROGRAM || PROCEDURE");
                throw new ParserErrorException("Exception");
            }
           
            if (tokens.Count != 0 && tokens[0].GetLine() == ";")
            {
                childNode.AddChild(new TreeNode(tokens[0].GetLine() + " ( Code: " + tokens[0].GetCode() + ")"));
                position.Push(new Position(tokens[0].GetRow(), tokens[0].GetColumn()));
                tokens.RemoveAt(0);
            }
            else
            {
                parserErrors.Add($"Expected ; at  Row: " + position.Peek().GetRow().ToString() + " Column:" + position.Peek().GetColumn().ToString());
                throw new ParserErrorException("Exception");
            }

            Block(childNode);
            
            if (tokens.Count != 0 && tokens[0].GetLine() == "." && program)
            {
                childNode.AddChild(new TreeNode(tokens[0].GetLine() + " ( Code: " + tokens[0].GetCode() + ")"));
                position.Push(new Position(tokens[0].GetRow(), tokens[0].GetColumn()));
                tokens.RemoveAt(0);
            }
            else if (tokens.Count != 0 && tokens[0].GetLine() == ";" && !program){
                childNode.AddChild(new TreeNode(tokens[0].GetLine() + " ( Code: " + tokens[0].GetCode() + ")"));
                position.Push(new Position(tokens[0].GetRow(), tokens[0].GetColumn()));
                tokens.RemoveAt(0);
            }
            else if (program)
            {
                parserErrors.Add($"Expected . after Block at Row:" + position.Peek().GetRow().ToString() + " Column:" + position.Peek().GetColumn().ToString());
                throw new ParserErrorException("Exception");
            }
            else{
                parserErrors.Add($"Expected ; after Block at Row:" + position.Peek().GetRow().ToString() + " Column:" + position.Peek().GetColumn().ToString());
                throw new ParserErrorException("Exception");
            }
        }

        private void Declarations(TreeNode parent){
            TreeNode childNode = new TreeNode("<declarations>");
            parent.AddChild(childNode);
            LableDeclarations(childNode);
        }

        private void LableDeclarations(TreeNode parent){
            TreeNode childNode = new TreeNode("<lable-declarations>");
            parent.AddChild(childNode);
            if (tokens.Count != 0 && tokens[0].GetLine().ToUpper() == "LABEL")
            {
                childNode.AddChild(new TreeNode(tokens[0].GetLine() + " ( Code: " + tokens[0].GetCode() + ")"));
                position.Push(new Position(tokens[0].GetRow(), tokens[0].GetColumn()));
                tokens.RemoveAt(0);
                UnsignedInteger(childNode);
                LablesList(childNode);
            }
        }

        private void LablesList(TreeNode parent)
        {
            TreeNode childNode = new TreeNode("<lable-list>");
            parent.AddChild(childNode);
            if (tokens.Count != 0 && tokens[0].GetLine() == ",")
            {
                childNode.AddChild(new TreeNode(tokens[0].GetLine() + " ( Code: " + tokens[0].GetCode() + ")"));
                position.Push(new Position(tokens[0].GetRow(), tokens[0].GetColumn()));
                tokens.RemoveAt(0);
                UnsignedInteger(childNode);
                LablesList(childNode);
            }
            else
            {
                TreeNode empty = new TreeNode("<Empty>");
                childNode.AddChild(empty);
            }
            
        }

        private void Block(TreeNode parent)
        {
            TreeNode childNode = new TreeNode("<Block>");
            parent.AddChild(childNode);
            Declarations(childNode);
            if (tokens.Count != 0 && tokens[0].GetLine().ToUpper() == "BEGIN")
            {
                childNode.AddChild(new TreeNode(tokens[0].GetLine() + " ( Code: " + tokens[0].GetCode() + ")"));
                position.Push(new Position(tokens[0].GetRow(), tokens[0].GetColumn()));
                tokens.RemoveAt(0);
            }
            else
            {
                parserErrors.Add($"Expected BEGIN  at Row:" + position.Peek().GetRow().ToString() + " Column:" + position.Peek().GetColumn().ToString());
                throw new ParserErrorException("Exception");
            }
            StatementsList(childNode);
            if (tokens.Count != 0 && tokens[0].GetLine().ToUpper() == "END")
            {
                childNode.AddChild(new TreeNode(tokens[0].GetLine() + " ( Code: " + tokens[0].GetCode() + ")"));
                position.Push(new Position(tokens[0].GetRow(), tokens[0].GetColumn()));
                tokens.RemoveAt(0);
            }
            else
            {
                parserErrors.Add($"Expected END at Row:" + position.Peek().GetRow().ToString() + " Column" + position.Peek().GetColumn().ToString());
                throw new ParserErrorException("Exception");
            }
        }

        private void ParametersList(TreeNode parent)
        {
            TreeNode childNode = new TreeNode("<parameters-List>");
            parent.AddChild(childNode);
            if (tokens.Count != 0 && tokens[0].GetLine() == "("){
                childNode.AddChild(new TreeNode(tokens[0].GetLine() + " ( Code: " + tokens[0].GetCode() + ")"));
                position.Push(new Position(tokens[0].GetRow(), tokens[0].GetColumn()));
                tokens.RemoveAt(0);

                DeclarationsList(childNode);
                if (tokens.Count != 0 && tokens[0].GetLine() == ")")
                {
                    childNode.AddChild(new TreeNode(tokens[0].GetLine() + " ( Code: " + tokens[0].GetCode() + ")"));
                    position.Push(new Position(tokens[0].GetRow(), tokens[0].GetColumn()));
                    tokens.RemoveAt(0);
                }
                else
                {
                    parserErrors.Add($"Expected ) after declaration list at Row:" + position.Peek().GetRow().ToString() + " Column" + position.Peek().GetColumn().ToString());
                    throw new ParserErrorException("Exception");
                }
            }
            else{
                Empty(childNode);
            }
           
        }
        
        private void DeclarationsList(TreeNode parent)
        {
            TreeNode childNode = new TreeNode("<declarations-List>");
            parent.AddChild(childNode);
            Empty(childNode);
        }

        private void StatementsList(TreeNode parent)
        {
            TreeNode childNode = new TreeNode("<Statements List>");
            parent.AddChild(childNode);
            Empty(childNode);
        }

        private void Empty(TreeNode parent)
        {
            TreeNode empty = new TreeNode("<Empty>");
            parent.AddChild(empty);
        }

        private void UnsignedInteger(TreeNode parent)
        {
            TreeNode childNode = new TreeNode("<Unsigned Integer>");
            parent.AddChild(childNode);
            if (tokens.Count != 0 && tokens[0].GetCode() >= 400 && tokens[0].GetCode() < 500)
            {
                childNode.AddChild(new TreeNode(tokens[0].GetLine() + " ( Code: " + tokens[0].GetCode() + ")"));
                position.Push(new Position(tokens[0].GetRow(), tokens[0].GetColumn()));
                tokens.RemoveAt(0);
            }
            else
            {
                Empty(childNode);
            }
        }

        private void ProcedureIdentifier(TreeNode parent)
        {
            TreeNode childNode = new TreeNode("<Procedure Identifier>");
            parent.AddChild(childNode);
            Identifier(childNode);
        }
        private void Identifier(TreeNode parent)
        {
            TreeNode childNode = new TreeNode("<Identifier>");
            parent.AddChild(childNode);
            if (tokens.Count != 0 && tokens[0].GetCode() >= 500 && tokens[0].GetCode() < 600)
            {
                childNode.AddChild(new TreeNode(tokens[0].GetLine() + " ( Code: " + tokens[0].GetCode() + ")"));
                position.Push(new Position(tokens[0].GetRow(), tokens[0].GetColumn()));
                tokens.RemoveAt(0);
            }
            else
            {
                parserErrors.Add($"Expected Identifier at Row:" + position.Peek().GetRow().ToString() + " Column:" + position.Peek().GetColumn().ToString());
                throw new ParserErrorException("Exception");
            }
        }
    }
}
