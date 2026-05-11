using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace JASON_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();
        public string Name;

        public Node(string N)
        {
            this.Name = N;
        }
    }

    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public Node root;

        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Program");

            root.Children.Add(Program());

            return root;
        }


        // Rule 31: Program -> Function_Statement* Main_Function
        Node Program()
        {
            Node program = new Node("Program");

            // Keep looping as long as the token AFTER the datatype is NOT 'main'
            while (InputPointer + 1 < TokenStream.Count &&
                   TokenStream[InputPointer + 1].token_type != Token_Class.Main)
            {
                program.Children.Add(Function_Statement());
            }

            // only thing left must be the Main Function
            program.Children.Add(Main_Function());

            MessageBox.Show("Parsing Completed!");
            return program;
        }

        // Rule 30: Main_Function -> Datatype main () Function_Body
        Node Main_Function()
        {
            Node mainFunc = new Node("Main_Function");
            mainFunc.Children.Add(Datatype());
            mainFunc.Children.Add(match(Token_Class.Main));
            mainFunc.Children.Add(match(Token_Class.LParanthesis));
            mainFunc.Children.Add(match(Token_Class.RParanthesis));
            mainFunc.Children.Add(Function_Body());
            return mainFunc;
        }

        // Rule 29 & 27: Function_Statement -> Datatype FunctionName ( Parameters ) Function_Body
        Node Function_Statement()
        {
            Node funcStmt = new Node("Function_Statement");
            funcStmt.Children.Add(Datatype());
            funcStmt.Children.Add(match(Token_Class.Identifier)); // FunctionName
            funcStmt.Children.Add(match(Token_Class.LParanthesis));
            funcStmt.Children.Add(Parameters());
            funcStmt.Children.Add(match(Token_Class.RParanthesis));
            funcStmt.Children.Add(Function_Body());
            return funcStmt;
        }

        // Rule 12: Datatype -> int | float | string
        Node Datatype()
        {
            Node datatype = new Node("Datatype");

            if (InputPointer < TokenStream.Count)
            {
                Token_Class current = TokenStream[InputPointer].token_type;
                if (current == Token_Class.Int || current == Token_Class.Float || current == Token_Class.String)
                {
                    datatype.Children.Add(match(current));
                    return datatype;
                }
            }

            // If it's not one of the 3 datatypes, force a match to trigger an error
            return match(Token_Class.Int);
        }

        List<Token_Class> statementStarters = new List<Token_Class>
        {
            Token_Class.Int, Token_Class.Float, Token_Class.String,
            Token_Class.Read, Token_Class.Write,
            Token_Class.Repeat, Token_Class.If,
            Token_Class.Return,
            Token_Class.Identifier
        };

        Node Statements()
        {
            Node statementsNode = new Node("Statements");

            // Keep looping and adding statements as long as we see a valid starting keyword
            while (InputPointer < TokenStream.Count && statementStarters.Contains(TokenStream[InputPointer].token_type))
            {
                statementsNode.Children.Add(Statement());
            }

            return statementsNode;
        }

        Node Statement()
        {
            Node stmt = new Node("Statement");

            if (InputPointer >= TokenStream.Count) return stmt;

            Token_Class current = TokenStream[InputPointer].token_type;

            // route to the correct statement rule based on the keyword
            if (current == Token_Class.Read)
                stmt.Children.Add(Read_Statement());
            else if (current == Token_Class.Write)
                stmt.Children.Add(Write_Statement());
            else if (current == Token_Class.If)
                stmt.Children.Add(If_Statement());
            else if (current == Token_Class.Repeat)
                stmt.Children.Add(Repeat_Statement());
            else if (current == Token_Class.Return)
                stmt.Children.Add(Return_Statement());
            else if (current == Token_Class.Int || current == Token_Class.Float || current == Token_Class.String)
                stmt.Children.Add(Declaration_Statement());
            else if (current == Token_Class.Identifier)
            {
                // to differentiate Assignment (:=) vs Function Call ( "(" )
                if (InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1].token_type == Token_Class.AssignmentOp)
                {
                    stmt.Children.Add(Assignment_Statement());
                }
                else
                {
                    stmt.Children.Add(Function_Call());
                    stmt.Children.Add(match(Token_Class.Semicolon)); // function calls alone need semicolons
                }
            }

            return stmt;
        }

        

       //TO BE IMPLEMENTED




        // Rule 28: Function_Body -> { Statements Return_Statement }
        Node Function_Body()
        {
            Node n = new Node("Function_Body");
            // TODO: Match '{', call Statements(), call Return_Statement(), match '}'
            return n;
        }

        // Rule 26/27 handling: Zero or more Parameters separated by comma
        Node Parameters()
        {
            Node n = new Node("Parameters");
            // TODO: Implement recursive parameter parsing
            return n;
        }

        // Rule 26: Parameter -> Datatype Identifier
        Node Parameter()
        {
            Node n = new Node("Parameter");
            // TODO: Match Datatype, Match Identifier
            return n;
        }

        // Rule 13: Declaration_Statement
        Node Declaration_Statement()
        {
            Node n = new Node("Declaration_Statement");
            // TODO: Match Datatype, Match Identifiers, Match Semicolon
            return n;
        }

        // Rule 11: Assignment_Statement
        Node Assignment_Statement()
        {
            Node n = new Node("Assignment_Statement");
            // TODO: Match Identifier, Match :=, Call Expression(), Match Semicolon
            return n;
        }

        // Rule 14: Write_Statement
        Node Write_Statement()
        {
            Node n = new Node("Write_Statement");
            // TODO: Match write, Call Expression() or Match endl, Match Semicolon
            return n;
        }

        // Rule 15: Read_Statement
        Node Read_Statement()
        {
            Node n = new Node("Read_Statement");
            // TODO: Match read, Match Identifier, Match Semicolon
            return n;
        }

        // Rule 16: Return_Statement
        Node Return_Statement()
        {
            Node n = new Node("Return_Statement");
            // TODO: Match return, Call Expression(), Match Semicolon
            return n;
        }

        // Rule 21: If_Statement
        Node If_Statement()
        {
            Node n = new Node("If_Statement");
            // TODO: Match if, Call Condition_Statement, Match then, Call Statements, Call ElseIf/Else/End
            return n;
        }

        // Rule 22: Else_If_Statement
        Node Else_If_Statement()
        {
            Node n = new Node("Else_If_Statement");
            // TODO: Match elseif, Call Condition_Statement, Match then, Call Statements
            return n;
        }

        // Rule 23: Else_Statement
        Node Else_Statement()
        {
            Node n = new Node("Else_Statement");
            // TODO: Match else, Call Statements, Match end
            return n;
        }

        // Rule 24: Repeat_Statement
        Node Repeat_Statement()
        {
            Node n = new Node("Repeat_Statement");
            // TODO: Match repeat, Call Statements, Match until, Call Condition_Statement
            return n;
        }

        // Rule 20: Condition_Statement
        Node Condition_Statement()
        {
            Node n = new Node("Condition_Statement");
            // TODO: Call Condition(), handle recursive Boolean Operators
            return n;
        }

        // Rule 18: Condition -> Identifier Condition_Operator Term
        Node Condition()
        {
            Node n = new Node("Condition");
            // TODO: Match Identifier, Match Operator, Call Term()
            return n;
        }

        // Rule 10: Expression -> String | Term | Equation
        Node Expression()
        {
            Node n = new Node("Expression");
            // TODO: Handle logic to route to StringLiteral, Term, or Equation
            return n;
        }

        // Rule 9: Equation
        Node Equation()
        {
            Node n = new Node("Equation");
            // TODO: Handle math operations and brackets
            return n;
        }

        // Rule 7: Term -> Number | Identifier | Function_Call
        Node Term()
        {
            Node n = new Node("Term");
            // TODO: Route to correct term type
            return n;
        }

        // Rule 6: Function_Call
        Node Function_Call()
        {
            Node n = new Node("Function_Call");
            // TODO: Match Identifier, Match (, loop parameters, Match )
            return n;
        }




        //Utility Functions




        public Node match(Token_Class ExpectedToken)
        {
            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    Node newNode = new Node(TokenStream[InputPointer].lex);
                    InputPointer++;
                    return newNode;
                }
                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " but found " +
                        TokenStream[InputPointer].token_type.ToString() + " on Line X\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected " + ExpectedToken.ToString() + " but found EOF\r\n");
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }

        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;

            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;

            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}