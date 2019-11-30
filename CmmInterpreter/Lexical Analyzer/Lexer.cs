using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmmInterpreter.Lexical_Analyzer
{
    public class Lexer
    {
        private readonly string[] _keyWords = {"if", "else", "while", "int", "real", "NULL",
            "char", "for", "break", "continue", "print", "scan"};
        private StringBuilder ErrorInfoStrb { get; set; }
        public LinkedList<Token> Words { get; set; }
        public char[] Chars { get; set; }

        private static bool IsDigit (char dig)
        {    //判断是否为数字
            return (dig >= '0' && dig <= '9');
        }

        private static bool IsSpaceOrLine (char sp)
        {  //判断是否为空格、回车、换行等
            return (sp == ' ' || sp == '\t' || sp == '\n' || sp == '\r');
        }

        private static bool IsLetter (char let)
        {   //判断是否为字母
            return ((let >= 'a' && let <= 'z') || (let >= 'A' && let <= 'Z'));
        }

        private bool IsKey (string str)
        {
            //判断单词是否为关键字
            return _keyWords.Any(key => key.Equals(str));
        }
         /// <summary>
         /// 该方法用来将词法分析的结果以字符串的形式返回
         /// </summary>
         /// <returns>
         /// Token序列以及可能的错误
         /// </returns>
         public string PrintResult ()
        {
            var strb = new StringBuilder ();
            if (ErrorInfoStrb != null)
                strb.Append(ErrorInfoStrb);
            foreach (var token in Words)
                strb.Append(token);
            return strb.ToString ();
        }

         /// <summary>
         /// 该方法用来对输入的char型数组进行词法分析，调用前需要保证Chars属性不为空
         /// </summary>
        public void LexAnalyze ()
        {
            ErrorInfoStrb = new StringBuilder();
            Words = new LinkedList<Token> ();  //用来存储分析出来的token
            var lineNo = 1;
            //        bool is_pos = false;
            //        bool is_neg = false;
            var word = new StringBuilder ();
            for (var i = 0; i < Chars.Length; i++)
            {
                var startPos = i;
                word.Clear();
                var oneChar = Chars[i];
                if (IsSpaceOrLine (oneChar))
                {
                    if (oneChar == '\n')
                    {
                        lineNo++; //检测换行符，改变行号
                    }
                }
                else if (IsLetter (oneChar) || oneChar == '_')
                {  // _或字母开头
                    while (IsLetter (oneChar) || IsDigit (oneChar) || oneChar == '_')
                    {  //后面可以有数字
                        try
                        {
                            word.Append (oneChar);
                            oneChar = Chars[++i];
                        }
                        catch (IndexOutOfRangeException)
                        {  //如果读取的是最后一个字符会有越界异常
                            break;
                        }
                    }
                    i--; //多进了一位，需要回退
                    if (IsKey (word.ToString ()))
                    {
                        switch (word.ToString ())
                        {
                            case "int":
                                Words.AddLast (new Token (word.ToString (), TokenType.INT, lineNo));
                                Words.Last.Value.StartPos = startPos;
                                break;
                            case "if":
                                Words.AddLast (new Token (word.ToString (), TokenType.IF, lineNo));
                                Words.Last.Value.StartPos = startPos;
                                break;
                            case "else":
                                Words.AddLast (new Token (word.ToString (), TokenType.ELSE, lineNo));
                                Words.Last.Value.StartPos = startPos;
                                break;
                            case "real":
                                Words.AddLast (new Token (word.ToString (), TokenType.REAL, lineNo));
                                Words.Last.Value.StartPos = startPos;
                                break;
                            case "while":
                                Words.AddLast (new Token (word.ToString (), TokenType.WHILE, lineNo));
                                Words.Last.Value.StartPos = startPos;
                                break;
                            case "NULL":
                                Words.AddLast (new Token (word.ToString (), TokenType.NULL, lineNo));
                                Words.Last.Value.StartPos = startPos;
                                break;
                            case "char":
                                Words.AddLast (new Token (word.ToString (), TokenType.CHAR, lineNo));
                                Words.Last.Value.StartPos = startPos;
                                break;
                            case "for":
                                Words.AddLast (new Token (word.ToString (), TokenType.FOR, lineNo));
                                Words.Last.Value.StartPos = startPos;
                                break;
                            case "break":
                                Words.AddLast (new Token (word.ToString (), TokenType.BREAK, lineNo));
                                Words.Last.Value.StartPos = startPos;
                                break;
                            case "continue":
                                Words.AddLast (new Token (word.ToString (), TokenType.CONTINUE, lineNo));
                                Words.Last.Value.StartPos = startPos;
                                break;
                            case "print":
                                Words.AddLast (new Token (word.ToString (), TokenType.PRINT, lineNo));
                                Words.Last.Value.StartPos = startPos;
                                break;
                            case "scan":
                                Words.AddLast (new Token (word.ToString (), TokenType.SCAN, lineNo));
                                Words.Last.Value.StartPos = startPos;
                                break;
                        }
                    } //是关键字
                    else
                    {  //是普通标识符
                        if (word.Length <= 64)
                        {
                            Words.AddLast (new Token (word.ToString (), TokenType.ID, lineNo));// 自定义标识符不能超过64个字符
                            Words.Last.Value.StartPos = startPos;
                        }
                        else
                        {
                            Words.AddLast (new Token (word.ToString (), TokenType.ERROR, lineNo));
                            Words.Last.Value.StartPos = startPos;
                            ErrorInfoStrb.Append ("ERROR : Line: ").Append (lineNo).Append (" 标识符不得超过64个字符. ").Append ("(").Append (word).Append (")\n");
                        }
                    }
                }
                else if (IsDigit (oneChar) || (oneChar == '.'))
                {   //如果是数字
                    var realFlag = false; //判断是否为REAL类型
                    var isError = false;
                    //                if (is_pos) { //判断是否带有符号
                    //                    word.Append('+');
                    //                } else if (is_neg) {
                    //                    word.Append('-');
                    //                }
                    while (IsDigit (oneChar) || (oneChar == '.'))
                    {
                        if (oneChar == '.')
                        {
                            if (realFlag)
                            {
                                isError = true;
                            }
                            else
                            {
                                realFlag = true;
                            }
                        }
                        try
                        {
                            word.Append (oneChar);
                            oneChar = Chars[++i];
                        }
                        catch (IndexOutOfRangeException)
                        {
                            break;
                        }
                    }
                    i--;
                    //                is_pos = false;
                    //                is_neg = false;
                    if (isError)
                    {
                        Words.AddLast (new Token (word.ToString (), TokenType.ERROR, lineNo));
                        Words.Last.Value.StartPos = startPos;
                        ErrorInfoStrb.Append ("ERROR : Line: ").Append (lineNo).Append (" 非法数字. ").Append ("(").Append (word).Append (")\n");
                    }
                    else
                    {
                        if (realFlag)
                        {
                            Words.AddLast (new Token (word.ToString (), TokenType.REAL_VAL, lineNo));  //实型
                            Words.Last.Value.StartPos = startPos;
                        }
                        else
                        {
                            Words.AddLast (new Token (word.ToString (), TokenType.INT_VAL, lineNo));// 整型
                            Words.Last.Value.StartPos = startPos;
                        }
                    }

                }
                else
                {
                    switch (oneChar)
                    {  //各种符号
                        case '%':
                            try
                            {
                                oneChar = Chars[++i];
                                if (oneChar == '=')
                                {   //检测%=
                                    Words.AddLast (new Token ("%=", TokenType.MOD_ASSIGN, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                }
                                else
                                {
                                    Words.AddLast (new Token ("%", TokenType.MOD, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                    i--;
                                }
                            }
                            catch (IndexOutOfRangeException)
                            {
                                Words.AddLast (new Token ("%", TokenType.MOD, lineNo));
                                Words.Last.Value.StartPos = startPos;
                            }
                            break;
                        case '+':
                            try
                            {
                                oneChar = Chars[++i];
                                if (oneChar == '+')
                                {   //检测++
                                    Words.AddLast (new Token ("++", TokenType.PLUS_PLUS, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                }
                                else if (oneChar == '=')
                                {//检测+=
                                    Words.AddLast (new Token ("+=", TokenType.PLUS_ASSIGN, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                }
                                else
                                {
                                    Words.AddLast (new Token ("+", TokenType.PLUS, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                    //errorInfoStrb.Append("ERROR : Line: ").Append(lineNo).Append(" 非法字符'+'. ").Append("(").Append(word.ToString()).Append(")\n");
                                    i--;
                                }
                            }
                            catch (IndexOutOfRangeException)
                            {
                                Words.AddLast (new Token ("+", TokenType.PLUS, lineNo));
                                Words.Last.Value.StartPos = startPos;
                            }
                            break;
                        case '-':
                            try
                            {
                                oneChar = Chars[++i];
                                if (oneChar == '-')
                                {   //检测--
                                    Words.AddLast (new Token ("--", TokenType.MINUS_MINUS, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                }
                                else if (oneChar == '=')
                                { //检测-=
                                    Words.AddLast (new Token ("-=", TokenType.MINUS_ASSIGN, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                }
                                else
                                {
                                    Words.AddLast (new Token ("-", TokenType.MINUS, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                    //errorInfoStrb.Append("ERROR : Line: ").Append(lineNo).Append(" 非法字符'-'. ").Append("(").Append(word.ToString()).Append(")\n");
                                    i--;
                                }
                            }
                            catch (IndexOutOfRangeException)
                            {
                                Words.AddLast (new Token ("-", TokenType.MINUS, lineNo));
                                Words.Last.Value.StartPos = startPos;
                            }
                            break;
                        case '*':
                            try
                            {
                                oneChar = Chars[++i];
                                if (oneChar == '=')
                                {   //检测*=
                                    Words.AddLast (new Token ("*=", TokenType.MUL_ASSIGN, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                }
                                else
                                {
                                    Words.AddLast (new Token ("*", TokenType.MUL, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                    i--;
                                }
                            }
                            catch (IndexOutOfRangeException)
                            {
                                Words.AddLast (new Token ("*", TokenType.MUL, lineNo));
                                Words.Last.Value.StartPos = startPos;
                            }
                            break;
                        case '(':
                            Words.AddLast (new Token ("(", TokenType.LEFT_P, lineNo));
                            Words.Last.Value.StartPos = startPos;
                            break;
                        case ')':
                            Words.AddLast (new Token (")", TokenType.RIGHT_P, lineNo));
                            Words.Last.Value.StartPos = startPos;
                            break;
                        case '[':
                            Words.AddLast (new Token ("[", TokenType.LEFT_BRK, lineNo));
                            Words.Last.Value.StartPos = startPos;
                            break;
                        case ']':
                            Words.AddLast (new Token ("]", TokenType.RIGHT_BRK, lineNo));
                            Words.Last.Value.StartPos = startPos;
                            break;
                        case '{':
                            Words.AddLast (new Token ("{", TokenType.LEFT_BRA, lineNo));
                            Words.Last.Value.StartPos = startPos;
                            break;
                        case '!':
                            Words.AddLast (new Token ("!", TokenType.NOT, lineNo));
                            Words.Last.Value.StartPos = startPos;
                            break;
                        case '}':
                            Words.AddLast (new Token ("}", TokenType.RIGHT_BRA, lineNo));
                            Words.Last.Value.StartPos = startPos;
                            break;
                        case ';':
                            Words.AddLast (new Token (";", TokenType.END, lineNo));
                            Words.Last.Value.StartPos = startPos;
                            break;
                        case ',':
                            Words.AddLast (new Token (",", TokenType.COMMA, lineNo));
                            Words.Last.Value.StartPos = startPos;
                            break;
                        case '&':
                            try
                            {
                                oneChar = Chars[++i];
                                if (oneChar == '&')
                                {   //检测&&
                                    Words.AddLast (new Token ("&&", TokenType.AND, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                }
                                else
                                {
                                    Words.AddLast (new Token ("&", TokenType.ERROR, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                    ErrorInfoStrb.Append ("ERROR : Line: ").Append (lineNo).Append (" 非法字符'&'. ").Append ("(").Append (word).Append (")\n");
                                    i--;
                                }
                            }
                            catch (IndexOutOfRangeException)
                            {
                                Words.AddLast (new Token ("&", TokenType.ERROR, lineNo));
                                Words.Last.Value.StartPos = startPos;
                                ErrorInfoStrb.Append ("ERROR : Line: ").Append (lineNo).Append (" 非法字符'&'. ").Append ("(").Append (word).Append (")\n");
                            }
                            break;
                        case '|':
                            try
                            {
                                oneChar = Chars[++i];
                                if (oneChar == '|')
                                {  //检测||
                                    Words.AddLast (new Token ("||", TokenType.OR, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                }
                                else
                                {
                                    Words.AddLast (new Token ("|", TokenType.ERROR, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                    ErrorInfoStrb.Append ("ERROR : Line: ").Append (lineNo).Append (" 非法字符'|'. ").Append ("(").Append (word).Append (")\n");
                                    i--;
                                }
                            }
                            catch (IndexOutOfRangeException)
                            {
                                Words.AddLast (new Token ("|", TokenType.ERROR, lineNo));
                                Words.Last.Value.StartPos = startPos;
                            }
                            break;
                        case '<':
                            try
                            {
                                oneChar = Chars[++i];
                                if (oneChar == '>')   //不等于
                                {
                                    Words.AddLast (new Token ("<>", TokenType.NEQ, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                }
                                else if (oneChar == '=')
                                { //小于等于
                                    Words.AddLast (new Token ("<=", TokenType.LESS_EQ, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                }
                                else
                                {  //小于
                                    Words.AddLast (new Token ("<", TokenType.LESS, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                    i--;
                                }
                                break;
                            }
                            catch (IndexOutOfRangeException)
                            {
                                Words.AddLast (new Token ("<", TokenType.LESS, lineNo));
                                Words.Last.Value.StartPos = startPos;
                                break;
                            }

                        case '>':
                            try
                            {
                                oneChar = Chars[++i];
                                if (oneChar == '=')
                                { //大于等于
                                    Words.AddLast (new Token (">=", TokenType.GREATER_EQ, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                }
                                else
                                {   //大于
                                    Words.AddLast (new Token (">", TokenType.GREATER, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                    i--;
                                }
                                break;
                            }
                            catch (IndexOutOfRangeException)
                            {
                                Words.AddLast (new Token (">", TokenType.GREATER, lineNo));
                                Words.Last.Value.StartPos = startPos;
                                break;
                            }
                        case '=':
                            try
                            {
                                oneChar = Chars[++i];
                                if (oneChar == '=')
                                {  //等于
                                    Words.AddLast (new Token ("==", TokenType.EQ, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                }
                                else
                                {  //赋值
                                    Words.AddLast (new Token ("=", TokenType.ASSIGN, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                    i--;
                                }
                                break;
                            }
                            catch (IndexOutOfRangeException)
                            {
                                Words.AddLast (new Token ("=", TokenType.ASSIGN, lineNo));
                                Words.Last.Value.StartPos = startPos;
                                break;
                            }
                        case '/':
                            try
                            {
                                oneChar = Chars[++i];
                                if (oneChar == '/')
                                {  //行级注释
                                    while (oneChar != '\n')
                                    {
                                        try
                                        {
                                            oneChar = Chars[++i];
                                        }
                                        catch (IndexOutOfRangeException)
                                        {
                                            break;
                                        }
                                    }
                                    lineNo++;  //检测到换行符，行号加1

                                }
                                else if (oneChar == '*')
                                { //块级注释
                                    try
                                    {
                                        oneChar = Chars[++i];
                                        while (!((oneChar == '*') && (Chars[++i] == '/')))
                                        {
                                            if (oneChar == '\n')
                                            {  //检测换行符
                                                lineNo++;
                                            }
                                            oneChar = Chars[++i];
                                        }
                                    }
                                    catch (IndexOutOfRangeException)
                                    {
                                        ErrorInfoStrb.Append ("ERROR : Line: ").Append (lineNo).Append (" 块级注释未闭合.\n");
                                    }

                                }
                                else if (oneChar == '=')
                                {
                                    Words.AddLast (new Token ("/=", TokenType.DIV_ASSIGN, lineNo)); //检测/*
                                    Words.Last.Value.StartPos = startPos;
                                }
                                else
                                {  //除法
                                    Words.AddLast (new Token ("/", TokenType.DIV, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                    i--;
                                }
                                break;
                            }
                            catch (IndexOutOfRangeException)
                            {
                                Words.AddLast (new Token ("/", TokenType.DIV, lineNo));
                                Words.Last.Value.StartPos = startPos;
                                break;
                            }
                        default:
                            Words.AddLast (new Token (word.Append (oneChar).ToString (), TokenType.ERROR, lineNo));
                            Words.Last.Value.StartPos = startPos;
                            ErrorInfoStrb.Append ("ERROR : Line: ").Append (lineNo).Append (" 非法字符'").Append (word).Append ("'. ").Append ("(").Append (word).Append (")\n");
                            break;
                    }
                }
            }
        }
    }
}