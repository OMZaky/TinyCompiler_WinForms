using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JASON_Compiler
{
    //TINY Language

    public enum Token_Class
    {
        // Data Types
        Int, Float, String,

        // Keywords
        Read, Write, Repeat, Until, If, ElseIf, Else, Then, Return, End, Endl, Main,

        // Operators
        AssignmentOp, EqualOp, LessThanOp, GreaterThanOp, NotEqualOp, LessThanOrEqualOp, GreaterThanOrEqualOp,
        PlusOp, MinusOp, MultiplyOp, DivideOp, AndOp, OrOp,

        // Delimiters
        Semicolon, Comma, LParanthesis, RParanthesis, LBrace, RBrace,

        // Identifiers and Values
        Identifier, Number, StringLiteral
    }

    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            // Reserved Words
            ReservedWords.Add("int", Token_Class.Int);
            ReservedWords.Add("float", Token_Class.Float);
            ReservedWords.Add("string", Token_Class.String);
            ReservedWords.Add("read", Token_Class.Read);
            ReservedWords.Add("write", Token_Class.Write);
            ReservedWords.Add("repeat", Token_Class.Repeat);
            ReservedWords.Add("until", Token_Class.Until);
            ReservedWords.Add("if", Token_Class.If);
            ReservedWords.Add("elseif", Token_Class.ElseIf);
            ReservedWords.Add("else", Token_Class.Else);
            ReservedWords.Add("then", Token_Class.Then);
            ReservedWords.Add("return", Token_Class.Return);
            ReservedWords.Add("end", Token_Class.End);
            ReservedWords.Add("endl", Token_Class.Endl);
            ReservedWords.Add("main", Token_Class.Main);

            // Operators & Delimiters
            Operators.Add(":=", Token_Class.AssignmentOp);
            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("<>", Token_Class.NotEqualOp);
            Operators.Add("<=", Token_Class.LessThanOrEqualOp);
            Operators.Add(">=", Token_Class.GreaterThanOrEqualOp);
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);
            Operators.Add("&&", Token_Class.AndOp);
            Operators.Add("||", Token_Class.OrOp);
            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LParanthesis);
            Operators.Add(")", Token_Class.RParanthesis);
            Operators.Add("{", Token_Class.LBrace);
            Operators.Add("}", Token_Class.RBrace);
        }

        public void StartScanning(string SourceCode)
        {
            int LineNumber = 1;

            for (int i = 0; i < SourceCode.Length; i++)
            {
                char CurrentChar = SourceCode[i];

                // skip Whitespace and count newlines
                if (char.IsWhiteSpace(CurrentChar))
                {
                    if (CurrentChar == '\n')
                    {
                        LineNumber++;
                    }
                    continue;
                }

                // handle Multi-line Comments (/* ... */)
                if (CurrentChar == '/' && i + 1 < SourceCode.Length && SourceCode[i + 1] == '*')
                {
                    i += 2;

                    // keep moving forward until we find '*/' or hit the end of the file
                    while (i < SourceCode.Length && !(SourceCode[i] == '*' && i + 1 < SourceCode.Length && SourceCode[i + 1] == '/'))
                    {
                        if (SourceCode[i] == '\n')
                        {
                            LineNumber++;
                        }
                        i++;
                    }
                    i++;
                    continue;
                }

                // handle Single-line Comments (//)
                else if (CurrentChar == '/' && i + 1 < SourceCode.Length && SourceCode[i + 1] == '/')
                {
                    i += 2;

                    while (i < SourceCode.Length && SourceCode[i] != '\n')
                    {
                        i++;
                    }

                    if (i < SourceCode.Length && SourceCode[i] == '\n')
                    {
                        LineNumber++;
                    }
                    continue;
                }

                // handle Identifiers and Reserved Words (Starts with a letter)
                if (char.IsLetter(CurrentChar))
                {
                    string CurrentLexeme = CurrentChar.ToString();

                    while (i + 1 < SourceCode.Length && char.IsLetterOrDigit(SourceCode[i + 1]))
                    {
                        CurrentLexeme += SourceCode[i + 1];
                        i++;
                    }
                    FindTokenClass(CurrentLexeme, LineNumber);
                }

                // handle Numbers/Constants (Starts with a digit)
                else if (char.IsDigit(CurrentChar))
                {
                    string CurrentLexeme = CurrentChar.ToString();

                    while (i + 1 < SourceCode.Length && (char.IsLetterOrDigit(SourceCode[i + 1]) || SourceCode[i + 1] == '.'))
                    {
                        CurrentLexeme += SourceCode[i + 1];
                        i++;
                    }
                    FindTokenClass(CurrentLexeme, LineNumber);
                }

                // handle String Literals (Starts with quotes)
                else if (CurrentChar == '"')
                {
                    string CurrentLexeme = CurrentChar.ToString();
                    i++;

                    while (i < SourceCode.Length && SourceCode[i] != '"')
                    {

                        // count newlines inside strings too
                        if (SourceCode[i] == '\n')
                        {
                            LineNumber++;
                        }

                        CurrentLexeme += SourceCode[i];
                        i++;
                    }

                    if (i < SourceCode.Length) // add the final quote if we found it
                    {
                        CurrentLexeme += SourceCode[i];
                    }
                    FindTokenClass(CurrentLexeme, LineNumber);
                }

                // handle Operators and Delimiters
                else
                {
                    string CurrentLexeme = CurrentChar.ToString();

                    if (i + 1 < SourceCode.Length)
                    {
                        string twoCharOp = CurrentLexeme + SourceCode[i + 1];

                        // check if the 2-char string
                        if (Operators.ContainsKey(twoCharOp))
                        {
                            CurrentLexeme = twoCharOp;
                            i++;
                        }
                    }

                    FindTokenClass(CurrentLexeme, LineNumber);
                }
            }

            JASON_Compiler.TokenStream = Tokens;
        }

        void FindTokenClass(string Lex, int LineNumber)
        {
            Token Tok = new Token();
            Tok.lex = Lex;

            // Is it a reserved word?
            if (ReservedWords.ContainsKey(Lex))
            {
                Tok.token_type = ReservedWords[Lex];
                Tokens.Add(Tok);
            }
            // Is it an operator or delimiter?
            else if (Operators.ContainsKey(Lex))
            {
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);
            }
            // Is it an identifier?
            else if (isIdentifier(Lex))
            {
                Tok.token_type = Token_Class.Identifier;
                Tokens.Add(Tok);
            }
            // Is it a Constant?
            else if (isConstant(Lex))
            {
                Tok.token_type = Token_Class.Number;
                Tokens.Add(Tok);
            }
            // Is it a String Literal?
            else if (Lex.Length >= 2 && Lex.StartsWith("\"") && Lex.EndsWith("\""))
            {
                Tok.token_type = Token_Class.StringLiteral;
                Tokens.Add(Tok);
            }
            // Is it undefined?
            else
            {
                Errors.Error_List.Add($"Line {LineNumber} | Lexical Error: Unrecognized token '{Lex}'");
            }
        }

        bool isIdentifier(string lex)
        {
            if (string.IsNullOrEmpty(lex) || !char.IsLetter(lex[0]))
                return false;

            foreach (char c in lex)
            {
                if (!char.IsLetterOrDigit(c))
                    return false;
            }
            return true;
        }

        bool isConstant(string lex)
        {
            if (string.IsNullOrEmpty(lex))
                return false;

            bool hasDecimal = false;
            foreach (char c in lex)
            {
                if (c == '.')
                {
                    if (hasDecimal) return false; // Two decimals makes it invalid
                    hasDecimal = true;
                }
                else if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }

       
    }
}