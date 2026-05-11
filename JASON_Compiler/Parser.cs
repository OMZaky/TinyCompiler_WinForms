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
        List<Token_Class> Datatype_tokens = new List<Token_Class> {Token_Class.Int, Token_Class.Float, Token_Class.String };
        List<Token_Class> Boolean_tokens = new List<Token_Class> { Token_Class.OrOp, Token_Class.AndOp };
        List<Token_Class> statementStarters = new List<Token_Class>
        {
            Token_Class.Int, Token_Class.Float, Token_Class.String,
            Token_Class.Read, Token_Class.Write,
            Token_Class.Repeat, Token_Class.If,
            Token_Class.Identifier
        };

        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = Program();

            return root;
        }


        // Rule 31: Program -> Function_Statement* Main_Function
        Node Program()
        {
            Node program = new Node("Program");
            List<Node> children = new List<Node>();
            children = function_block(children);
            foreach(Node c in children)
            {
                program.Children.Add(c);
            }
            program.Children.Add(main_function());
            MessageBox.Show("Success");
            return program;
        }

        List<Node> function_block(List<Node> nodes)
        {
            //To be implemented
            return null;
        }

        Node main_function()
        {
            Node Main = new Node("Main");
            if (InputPointer < TokenStream.Count && Datatype_tokens.Contains(TokenStream[InputPointer].token_type))
            {

                Main.Children.Add(Datatype());
                Main.Children.Add(match(Token_Class.Main));
                Main.Children.Add(match(Token_Class.LParanthesis));
                Main.Children.Add(match(Token_Class.RParanthesis));
                Main.Children.Add(function_body());
            }
            else if (InputPointer >= TokenStream.Count)
            {
                Errors.Error_List.Add("Parsing Error: ended before expected end of Main \n");
                InputPointer++;
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected a datatype to begin main function but found "
                + TokenStream[InputPointer].token_type.ToString() + "\r\n");
                InputPointer++;
            }
            return Main;
        }

        Node function_body()
        {
            return null;
        }

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

        
        Node function_call()
        {
            Node functionCall = new Node("function_call");
            if (InputPointer < TokenStream.Count)
            {
                functionCall.Children.Add(match(Token_Class.Identifier));
                functionCall.Children.Add(match(Token_Class.LParanthesis));
                functionCall.Children.Add(Parameter_values());
                functionCall.Children.Add(match(Token_Class.RParanthesis));
            }
            else if (InputPointer >= TokenStream.Count)
            {
                Errors.Error_List.Add("Parsing Error: ended before expected end of Function_call \n");
                InputPointer++;
            }
            return functionCall;
        }

        Node Parameter_values()
        {

            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Identifier)
            {
                Node parameters = new Node("Parameters");
                parameters.Children.Add(match(Token_Class.Identifier));
                List<Node> children = new List<Node>();
                children = repeat_parameter_values (children);
                foreach(Node c in children)
                {
                    parameters.Children.Add(c);
                }
                return parameters;
            }
            return null;
        }

        List<Node> repeat_parameter_values(List<Node> nodes)
        {
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                nodes.Add(match(Token_Class.Comma));
                nodes.Add(match(Token_Class.Identifier));
                nodes = repeat_parameter_values(nodes);
            }
            return nodes;

        }

        Node declaration_statment()
        {

            Node dec_stmt = new Node("declaration_statment");
            if (InputPointer < TokenStream.Count && Datatype_tokens.Contains(TokenStream[InputPointer].token_type))
            {
                dec_stmt.Children.Add(Datatype());

                dec_stmt.Children.Add(id_seq());

                dec_stmt.Children.Add(match(Token_Class.Semicolon));
            }
            else if (InputPointer >= TokenStream.Count)
            {
                Errors.Error_List.Add("Parsing Error: ended before expected end of Declaration stament \n");
                InputPointer++;
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected a datatype to begin main function but found "
                + TokenStream[InputPointer].token_type.ToString() + "\r\n");
                InputPointer++;
            }
            return dec_stmt;
        }

        

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


            Errors.Error_List.Add("Parsing Error: Expected a datatype but found "
            + (InputPointer < TokenStream.Count ? TokenStream[InputPointer].token_type.ToString() : "EOF") + "\r\n");
            return datatype;
        }

        Node id_seq()
        {
            Node Id_seq = new Node("ID_SEQ");
            Id_seq.Children.Add(match(Token_Class.Identifier));
            List<Node> chilrenA = new List<Node>();
            chilrenA = id_diff(chilrenA);
            foreach (Node node in chilrenA)
            {
                Id_seq.Children.Add(node);
            }
            List<Node> chilrenB = new List<Node>();
            chilrenB = dec_seq(chilrenB);
            foreach (Node node in chilrenB)
            {
                Id_seq.Children.Add(node);
            }
            return Id_seq;
        }

        // differentiates delarations from simple identifiers in int x , y := 45646
        List<Node> id_diff(List<Node> nodes)
        { 
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.AssignmentOp)
            {
                nodes.Add(match(Token_Class.AssignmentOp));
                nodes.Add(Expression());
            }
            return nodes;
        }

        List<Node> dec_seq(List<Node> nodes)
        {
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                nodes.Add(match(Token_Class.Comma));
                nodes.Add(match(Token_Class.Identifier));
                List<Node> chilren = new List<Node>();
                chilren = id_diff(chilren);
                foreach (Node node in chilren)
                {
                    nodes.Add(node);
                }
                nodes = dec_seq(nodes);
            }
            return nodes;
        }

        Node Expression()
        {
            return null;
        }

        Node Condition_statment() {
            Node cond = new Node("Condition_statment");
            cond.Children.Add(Condition());
            List<Node> chilren = new List<Node>();
            chilren = repeat_cond(chilren);
            foreach (Node node in chilren)
            {
                cond.Children.Add(node);
            }
            return cond;
        }

        List<Node> repeat_cond(List<Node> nodes)
        {
            if (InputPointer < TokenStream.Count && Boolean_tokens.Contains(TokenStream[InputPointer].token_type))
            {
                if (TokenStream[InputPointer].token_type == Token_Class.OrOp)
                {
                    nodes.Add(match(Token_Class.OrOp));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.AndOp)
                {
                    nodes.Add(match(Token_Class.AndOp));
                }
                nodes.Add(Condition());
                nodes = repeat_cond(nodes);
            }
            return nodes;
        }

        Node Condition()
        {
            return null;
        }

        Node Statements()
        {
            Node statementsNode = new Node("Statements");

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


        /// <summary>


        /// the things below are written in the style of JSON For refrence only please keep that in mind 
        /// </summary>
        /// <returns></returns>
        /*
        Node DeclSec()
        
        Node DeclSec()
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
            Node statementat = new Node("Statments");
            if (InputPointer < TokenStream.Count && statmentToken.Contains(TokenStream[InputPointer].token_type))
            {

                statementat.Children.Add(Statment());
                List<Node> children = new List<Node>();
                children = State(children);
                foreach(Node c in children)
                {
                    statementat.Children.Add(c);
                }
            }
            else if (InputPointer > TokenStream.Count)
            {   
               // error list
            }
            return statementat;

            return statementat;
            
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
        // Implement your logic here
        // Until here thing below are generally useful and are not to be altered for the sake of convenience 

        */

        // Implement your logic here
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
                        TokenStream[InputPointer].token_type.ToString() + " on Line X\r\n")
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