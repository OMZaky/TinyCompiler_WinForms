using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = Program();
            return root;
        }
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
                if (TokenStream[InputPointer].token_type == Token_Class.Int)
                {
                    Main.Children.Add(match(Token_Class.Int));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Float)
                {
                    Main.Children.Add(match(Token_Class.Float));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.String)
                {
                    Main.Children.Add(match(Token_Class.String));
                }

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

        Node function_call()
        {
            Node functionCall = new Node("function_call");
            if (InputPointer < TokenStream.Count)
            {
                functionCall.Children.Add(match(Token_Class.Identifier));
                functionCall.Children.Add(match(Token_Class.LParanthesis));
                functionCall.Children.Add(Parameters());
                functionCall.Children.Add(match(Token_Class.RParanthesis));
            }
            else if (InputPointer >= TokenStream.Count)
            {
                Errors.Error_List.Add("Parsing Error: ended before expected end of Function_call \n");
                InputPointer++;
            }
            return functionCall;
        }

        Node Parameters()
        {

            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Identifier)
            {
                Node parameters = new Node("Parameters");
                parameters.Children.Add(match(Token_Class.Identifier));
                List<Node> children = new List<Node>();
                children = repeat_parameters(children);
                foreach(Node c in children)
                {
                    parameters.Children.Add(c);
                }
                return parameters;
            }
            return null;
        }

        List<Node> repeat_parameters(List<Node> nodes)
        {
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                nodes.Add(match(Token_Class.Comma));
                nodes.Add(match(Token_Class.Identifier));
                nodes = repeat_parameters(nodes);
            }
            return nodes;

        }

        Node declaration_statment()
        {

            Node dec_stmt = new Node("declaration_statment");
            if (InputPointer < TokenStream.Count && Datatype_tokens.Contains(TokenStream[InputPointer].token_type))
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Int)
                {
                    dec_stmt.Children.Add(match(Token_Class.Int));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Float)
                {
                    dec_stmt.Children.Add(match(Token_Class.Float));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.String)
                {
                    dec_stmt.Children.Add(match(Token_Class.String));
                }

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
                nodes.Add(match(Token_Class.Comma));
                nodes.Add(Condition());
                nodes = repeat_cond(nodes);
            }
            return nodes;
        }

        Node Condition()
        {
            return null;
        }


        /// <summary>


        /// the things below are written in the style of JSON For refrence only please keep that in mind 
        /// </summary>
        /// <returns></returns>
        /*
        Node DeclSec()
        {
            Node declsec = new Node("DeclSec");
            // write your code here to check atleast the declare sturcure 
            // without adding procedures
            return declsec;
        }
        Node Block()
        {
            Node block = new Node("block");
            block.Children.Add(match(Token_Class.Begin));
            block.Children.Add(statements());
            block.Children.Add(match(Token_Class.End));
            return block;
        }

        List<Token_Class> statmentToken = new List<Token_Class> { Token_Class.Read , Token_Class.Write , Token_Class.While , Token_Class.Set , Token_Class.If , Token_Class.Call}
        Node statements()
        {
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

        }
        Node Statment()
        {
            Node Statement = new Node();
            if(InputPointer < TokenStream.Count && TokenStream[InputPointer] == Token_Class.Read)
            {
                Statement.Children.Add(match(Token_Class.Read));
                Statement.Children.Add(match(Token_Class.Idenifier));
            }
            return Statement;
        }
        Node State(List<Node> nodes)
        {
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Semicolon)
            {
                nodes.Add(match(Token_Class.Semicolon));
                nodes.Add(Statment());
                nodes = State(nodes);
            }
            return nodes;
        }

        // Implement your logic here
        // Until here thing below are generally useful and are not to be altered for the sake of convenience 

        */
        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;

                }

                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString()  + "\r\n");
                InputPointer++;
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
