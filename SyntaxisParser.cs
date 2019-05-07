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
        
        public void PrintParser() //1
        {
            if (syntaxTree != null) syntaxTree.PrintTree();
        }
        //public void ExitProgram()
        //{
        //    Environment.Exit(0);
        //}
        
        public void PrintParserErrors() //1
        {
            foreach (var item in parserErrors)
            {
                Console.WriteLine("Syntax: " + item);
            }
        }
        
        public void Start() //1
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
            if (tokens.Count != 0 && tokens[0].GetLine().ToUpper() == "PROGRAM")
            {
                childNode.AddChild(new TreeNode(tokens[0].GetLine() + " ( Code: " + tokens[0].GetCode() + ")"));
                position.Push(new Position(tokens[0].GetRow(), tokens[0].GetColumn()));
                tokens.RemoveAt(0);
            }
            else
            {
                parserErrors.Add($"Expected keyword PROGRAM ");
                throw new ParserErrorException("Exception");
            }
            ProcedureIdentifier(childNode);
            if (tokens.Count != 0 && tokens[0].GetLine() == ";")
            {
                childNode.AddChild(new TreeNode(tokens[0].GetLine() + " ( Code: " + tokens[0].GetCode() + ")"));
                position.Push(new Position(tokens[0].GetRow(), tokens[0].GetColumn()));
                tokens.RemoveAt(0);
            }
            else
            {
                parserErrors.Add($"Expected ; after Identifier at  Row: " + position.Peek().GetRow().ToString() + " Column:" + position.Peek().GetColumn().ToString());
                throw new ParserErrorException("Exception");
            }

            Block(childNode);
            if (tokens.Count != 0 && tokens[0].GetLine() == ".")
            {
                childNode.AddChild(new TreeNode(tokens[0].GetLine() + " ( Code: " + tokens[0].GetCode() + ")"));
                position.Push(new Position(tokens[0].GetRow(), tokens[0].GetColumn()));
                tokens.RemoveAt(0);
            }
            else
            {
                parserErrors.Add($"Expected . after Block at Row:" + position.Peek().GetRow().ToString() + " Column:" + position.Peek().GetColumn().ToString());
                throw new ParserErrorException("Exception");
            }
        }

        private void Declarations(TreeNode parent){ //my
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
            //VariableDeclarations(childNode);
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
            //StatementsList(childNode);
            if (tokens.Count != 0 && tokens[0].GetLine().ToUpper() == "END")
            {
                childNode.AddChild(new TreeNode(tokens[0].GetLine() + " ( Code: " + tokens[0].GetCode() + ")"));
                position.Push(new Position(tokens[0].GetRow(), tokens[0].GetColumn()));
                tokens.RemoveAt(0);
            }
            else
            {
                parserErrors.Add($"Expected END after  Statement list at Row:" + position.Peek().GetRow().ToString() + " Column" + position.Peek().GetColumn().ToString());
                throw new ParserErrorException("Exception");
            }
        }
        private void VariableDeclarations(TreeNode parent)
        {
            TreeNode childNode = new TreeNode("<Variable Declarations>");
            parent.AddChild(childNode);
            if (tokens.Count != 0 && tokens[0].GetLine().ToUpper() == "VAR")
            {

                childNode.AddChild(new TreeNode(tokens[0].GetLine() + " ( Code: " + tokens[0].GetCode() + ")"));
                position.Push(new Position(tokens[0].GetRow(), tokens[0].GetColumn()));
                tokens.RemoveAt(0);
                DeclarationsList(childNode);
            }
            else
            {
                TreeNode empty = new TreeNode("<Empty>");
                childNode.AddChild(empty);

            }
        }
        
        private void DeclarationsList(TreeNode parent)
        {
            bool flag = false;

            while (tokens.Count != 0 && tokens[0].GetCode() > 500 && tokens[0].GetCode() < 600)
            {
                flag = true;
                TreeNode childNode = new TreeNode("<Declaration List>");
                parent.AddChild(childNode);
                Declaration(childNode);
            }
            if (!flag)
            {
                TreeNode childNode = new TreeNode("<Declaration List>");
                parent.AddChild(childNode);
                TreeNode empty = new TreeNode("<Empty>");
                childNode.AddChild(empty);
            }
        }
        private void Declaration(TreeNode parent)
        {
            TreeNode childNode = new TreeNode("<Declaration>");
            parent.AddChild(childNode);
            VariableIdentifier(childNode);
            if (tokens.Count != 0 && tokens[0].GetCode() == ':')
            {
                childNode.AddChild(new TreeNode(tokens[0].GetLine() + " ( Code: " + tokens[0].GetCode() + ")"));
                position.Push(new Position(tokens[0].GetRow(), tokens[0].GetColumn()));
                tokens.RemoveAt(0);
            }
            else
            {
                parserErrors.Add($"Expected : after Identifier at Row:" + position.Peek().GetRow().ToString() + " Column" + position.Peek().GetColumn().ToString());
                throw new ParserErrorException("Exception");
            }
            Attribute(childNode);
            if (tokens.Count != 0 && tokens[0].GetCode() == ';')
            {
                childNode.AddChild(new TreeNode(tokens[0].GetLine() + " ( Code: " + tokens[0].GetCode() + ")"));
                position.Push(new Position(tokens[0].GetRow(), tokens[0].GetColumn()));
                tokens.RemoveAt(0);
            }
            else
            {
                parserErrors.Add($"Expected ; after Attribute at Row:" + position.Peek().GetRow().ToString() + " Column:" + position.Peek().GetColumn().ToString());
                throw new ParserErrorException("Exception");
            }

            DeclarationsList(parent);

        }

        private void Attribute(TreeNode parent)
        {
            //bool flag = false;
            TreeNode childNode = new TreeNode("<Attribute>");
            parent.AddChild(childNode);
            if (tokens.Count != 0 && tokens[0].GetLine().ToUpper() == "INTEGER" || tokens[0].GetLine().ToUpper() == "FLOAT")
            {
               //flag = true;
                childNode.AddChild(new TreeNode(tokens[0].GetLine() + " ( Code: " + tokens[0].GetCode() + ")"));
                position.Push(new Position(tokens[0].GetRow(), tokens[0].GetColumn()));
                tokens.RemoveAt(0);
            }

            else
            {
                parserErrors.Add($"Expected Attribute at Row:" + position.Peek().GetRow().ToString() + " Column" + position.Peek().GetColumn().ToString());
                throw new ParserErrorException("Exception");
            }
        }
        private void StatementsList(TreeNode parent)
        {
            bool flag = false;

            while (tokens.Count != 0 && tokens[0].GetLine().ToUpper() == "WHILE")
            {
                flag = true;
                TreeNode childNode = new TreeNode("<Statements List>");
                parent.AddChild(childNode);
                Statement(childNode);
            }

            if (!flag)
            {
                TreeNode childNode = new TreeNode("<Statements List>");
                parent.AddChild(childNode);
                TreeNode empty = new TreeNode("<Empty>");
                childNode.AddChild(empty);
            }
        }
        private void Statement(TreeNode parent)
        {
            TreeNode childNode = new TreeNode("<Statement>");
            parent.AddChild(childNode);
            if (tokens.Count != 0 && tokens[0].GetLine().ToUpper() == "WHILE")
            {
                childNode.AddChild(new TreeNode(tokens[0].GetLine() + " ( Code: " + tokens[0].GetCode() + ")"));
                position.Push(new Position(tokens[0].GetRow(), tokens[0].GetColumn()));
                tokens.RemoveAt(0);

            }
            ConditionalExpression(childNode);
            if (tokens.Count != 0 && tokens[0].GetLine().ToUpper() == "DO")
            {
                childNode.AddChild(new TreeNode(tokens[0].GetLine() + " ( Code: " + tokens[0].GetCode() + ")"));
                position.Push(new Position(tokens[0].GetRow(), tokens[0].GetColumn()));
                tokens.RemoveAt(0);
            }
            else
            {
                parserErrors.Add($"Expected DO after Conditional expression at Row:" + position.Peek().GetRow().ToString() + " Column" + position.Peek().GetColumn().ToString());
                throw new ParserErrorException("Exception");
            }
            StatementsList(childNode);
            if (tokens.Count != 0 && tokens[0].GetLine().ToUpper() == "ENDWHILE")
            {
                childNode.AddChild(new TreeNode(tokens[0].GetLine() + " ( Code: " + tokens[0].GetCode() + ")"));
                position.Push(new Position(tokens[0].GetRow(), tokens[0].GetColumn()));
                tokens.RemoveAt(0);
            }
            else
            {
                parserErrors.Add($"Expected ENDWHILE after Statements list at Row:" + position.Peek().GetRow().ToString() + " Column:" + position.Peek().GetColumn().ToString());
                throw new ParserErrorException("Exception");
            }
            if (tokens.Count != 0 && tokens[0].GetCode() == ';')
            {
                childNode.AddChild(new TreeNode(tokens[0].GetLine() + " ( Code: " + tokens[0].GetCode() + ")"));
                position.Push(new Position(tokens[0].GetRow(), tokens[0].GetColumn()));
                tokens.RemoveAt(0);
            }
            else
            {
                parserErrors.Add($"Expected ; after keyword Endwhile at Row:" + position.Peek().GetRow().ToString() + " Column:" + position.Peek().GetColumn().ToString());
                throw new ParserErrorException("Exception");
            }
            StatementsList(parent);

        }
        private void ConditionalExpression(TreeNode parent)
        {
            TreeNode childNode = new TreeNode("<Conditional Expression>");
            parent.AddChild(childNode);
            Expression(childNode);
            ComparisonOperator(childNode);
            Expression(childNode);
        }
        private void ComparisonOperator(TreeNode parent)
        {
            TreeNode childNode = new TreeNode("<Comparison Operator>");
            parent.AddChild(childNode);
            if (tokens.Count != 0 && tokens[0].GetLine() == "<" ||
                tokens[0].GetLine() == ">" ||
                tokens[0].GetLine() == "<>" ||
                tokens[0].GetLine() == "<=" ||
                tokens[0].GetLine() == ">=" ||
                tokens[0].GetLine() == "=")
            {
                childNode.AddChild(new TreeNode(tokens[0].GetLine() + " ( Code: " + tokens[0].GetCode() + ")"));
                position.Push(new Position(tokens[0].GetRow(), tokens[0].GetColumn()));
                tokens.RemoveAt(0);
            }
            else
            {
                parserErrors.Add($"Expected Comparison Operator at Row:" + position.Peek().GetRow().ToString() + " Column:" + position.Peek().GetColumn().ToString());
                throw new ParserErrorException("Exception");
            }
        }
        private void Expression(TreeNode parent)
        {
            TreeNode childNode = new TreeNode("<Expression>");
            parent.AddChild(childNode);
            if (tokens.Count != 0 && tokens[0].GetCode() > 500 && tokens[0].GetCode() < 600)
            {
                VariableIdentifier(childNode);
            }
            else if (tokens.Count != 0 && tokens[0].GetCode() > 600 && tokens[0].GetCode() < 700)
            {
                UnsignedInteger(childNode);
            }
            else
            {
                parserErrors.Add($"Expected Expression  at Row:" + position.Peek().GetRow().ToString() + " Column:" + position.Peek().GetColumn().ToString());
                throw new ParserErrorException("Exception");
            }
        }
        private void VariableIdentifier(TreeNode parent)
        {
            TreeNode childNode = new TreeNode("<Variable Identifier>");
            parent.AddChild(childNode);
            Identifier(childNode);

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
                parserErrors.Add($"Expected Unsigned Integer at Row:" + position.Peek().GetRow().ToString() + " Column:" + position.Peek().GetColumn().ToString());
                throw new ParserErrorException("Exception");
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
