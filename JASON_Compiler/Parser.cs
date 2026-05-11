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
        
        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Program");
            root.Children.Add(Program());
            return root;
        }
        Node Program()
        {
            Node program = new Node("Program");
            program.Children.Add(Header());
            program.Children.Add(DeclSec());
            program.Children.Add(Block());
            program.Children.Add(match(Token_Class.Dot));
            MessageBox.Show("Success");
            return program;
        }
        
        Node Header()
        {
            Node header = new Node("Header");
            header.Children.Add(match(Token_Class.Program));
            header.Children.Add(match(Token_Class.Idenifier));
            header.Children.Add(match(Token_Class.Semicolon));
            return header;
        }
        
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
