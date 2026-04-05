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
        Read, Write, Repeat, Until, If, ElseIf, Else, Then, Return, Endl, Main,

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
            for (int i = 0; i < SourceCode.Length; i++)
            {
                char CurrentChar = SourceCode[i];

                // 1. Skip Whitespace and Newlines
                if (char.IsWhiteSpace(CurrentChar))
                {
                    continue;
                }

                // 2. Handle Multi-line Comments (/* ... */)
                if (CurrentChar == '/' && i + 1 < SourceCode.Length && SourceCode[i + 1] == '*')
                {
                    i += 2; // Step past the '/*'

                    // Keep moving forward until we find '*/' or hit the end of the file
                    while (i < SourceCode.Length && !(SourceCode[i] == '*' && i + 1 < SourceCode.Length && SourceCode[i + 1] == '/'))
                    {
                        i++;
                    }
                    i++; // Step past the closing '/'
                    continue;
                }

                // 3. Handle Identifiers and Reserved Words (Starts with a letter)
                if (char.IsLetter(CurrentChar))
                {
                    string CurrentLexeme = CurrentChar.ToString();

                    // Keep reading as long as the next character is a letter or digit
                    while (i + 1 < SourceCode.Length && char.IsLetterOrDigit(SourceCode[i + 1]))
                    {
                        CurrentLexeme += SourceCode[i + 1];
                        i++;
                    }
                    FindTokenClass(CurrentLexeme);
                }

                // 4. Handle Numbers/Constants (Starts with a digit)
                else if (char.IsDigit(CurrentChar))
                {
                    string CurrentLexeme = CurrentChar.ToString();

                    // Keep reading digits or a single decimal point for floats
                    while (i + 1 < SourceCode.Length && (char.IsDigit(SourceCode[i + 1]) || SourceCode[i + 1] == '.'))
                    {
                        CurrentLexeme += SourceCode[i + 1];
                        i++;
                    }
                    FindTokenClass(CurrentLexeme);
                }

                // 5. Handle String Literals (Starts with quotes)
                else if (CurrentChar == '"')
                {
                    string CurrentLexeme = CurrentChar.ToString();
                    i++; // Move inside the quote

                    while (i < SourceCode.Length && SourceCode[i] != '"')
                    {
                        CurrentLexeme += SourceCode[i];
                        i++;
                    }

                    if (i < SourceCode.Length) // Append the closing quote if we didn't hit EOF
                    {
                        CurrentLexeme += SourceCode[i];
                    }
                    FindTokenClass(CurrentLexeme);
                }

                // 6. Handle Operators and Delimiters
                else
                {
                    string CurrentLexeme = CurrentChar.ToString();

                    // "Lookahead" logic: Check if the next character combines to make a 2-character operator
                    if (i + 1 < SourceCode.Length)
                    {
                        string twoCharOp = CurrentLexeme + SourceCode[i + 1];

                        // Check if the 2-char string (like ":=" or "<=") exists in our dictionary
                        if (Operators.ContainsKey(twoCharOp))
                        {
                            CurrentLexeme = twoCharOp;
                            i++; // Consume the second character so we don't read it twice
                        }
                    }

                    FindTokenClass(CurrentLexeme);
                }
            }

            // Link our local tokens list to the global JASON_Compiler state so the UI updates
            JASON_Compiler.TokenStream = Tokens;
        }

        void FindTokenClass(string Lex)
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
            else if (Lex.StartsWith("\"") && Lex.EndsWith("\""))
            {
                Tok.token_type = Token_Class.StringLiteral;
                Tokens.Add(Tok);
            }
            // Is it undefined?
            else
            {
                Errors.Error_List.Add($"Lexical Error: Unrecognized token '{Lex}'");
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