using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JASON_Compiler
{
    public static class JASON_Compiler
    {
        public static Scanner Jason_Scanner = new Scanner();
       
        public static List<string> Lexemes= new List<string>();
        public static List<Token> TokenStream = new List<Token>();


        public static void Start_Compiling(string SourceCode)
        {
            // 1. Clear memory from any previous compilations
            Jason_Scanner.Tokens.Clear();
            TokenStream.Clear();
            Errors.Error_List.Clear();

            // 2. Run the Scanner
            Jason_Scanner.StartScanning(SourceCode);

            // Parser (Coming Soon)
            // Semantic Analysis (Coming Soon)
        }


        static void SplitLexemes(string SourceCode)
        {
            string[] Lexemes_arr=SourceCode.Split(' ');
            for(int i=0; i< Lexemes_arr.Length; i++)
            {
                if(Lexemes_arr[i].Contains("\r\n"))
                {
                    Lexemes_arr[i] = Lexemes_arr[i].Replace("\r\n", string.Empty);
                }
                Lexemes.Add(Lexemes_arr[i]);
            }

        }

        
    }
}
