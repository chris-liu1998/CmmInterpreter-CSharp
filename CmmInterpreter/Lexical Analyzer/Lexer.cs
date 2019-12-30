using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CmmInterpreter.Lexical_Analyzer
{
    /// <summary>
    /// 词法分析器
    /// </summary>

    public class Lexer
    {
        private readonly string[] _keyWords = {"if", "else", "while", "int", "real", "NULL",
            "char", "for", "break", "continue", "print", "scan"};
        public StringBuilder ErrorInfoStrb { get; set; }
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
            if (ErrorInfoStrb.ToString().Length != 0)
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
                                Words.AddLast (new Token (word.ToString (), TokenType.Int, lineNo));
                                Words.Last.Value.StartPos = startPos;
                                break;
                            case "if":
                                Words.AddLast (new Token (word.ToString (), TokenType.If, lineNo));
                                Words.Last.Value.StartPos = startPos;
                                break;
                            case "else":
                                Words.AddLast (new Token (word.ToString (), TokenType.Else, lineNo));
                                Words.Last.Value.StartPos = startPos;
                                break;
                            case "real":
                                Words.AddLast (new Token (word.ToString (), TokenType.Real, lineNo));
                                Words.Last.Value.StartPos = startPos;
                                break;
                            case "while":
                                Words.AddLast (new Token (word.ToString (), TokenType.While, lineNo));
                                Words.Last.Value.StartPos = startPos;
                                break;
                            case "NULL":
                                Words.AddLast (new Token (word.ToString (), TokenType.Null, lineNo));
                                Words.Last.Value.StartPos = startPos;
                                break;
                            case "char":
                                Words.AddLast (new Token (word.ToString (), TokenType.Char, lineNo));
                                Words.Last.Value.StartPos = startPos;
                                break;
                            case "for":
                                Words.AddLast (new Token (word.ToString (), TokenType.For, lineNo));
                                Words.Last.Value.StartPos = startPos;
                                break;
                            case "break":
                                Words.AddLast (new Token (word.ToString (), TokenType.Break, lineNo));
                                Words.Last.Value.StartPos = startPos;
                                break;
                            case "continue":
                                Words.AddLast (new Token (word.ToString (), TokenType.Continue, lineNo));
                                Words.Last.Value.StartPos = startPos;
                                break;
                            case "print":
                                Words.AddLast (new Token (word.ToString (), TokenType.Print, lineNo));
                                Words.Last.Value.StartPos = startPos;
                                break;
                            case "scan":
                                Words.AddLast (new Token (word.ToString (), TokenType.Scan, lineNo));
                                Words.Last.Value.StartPos = startPos;
                                break;
                        }
                    } //是关键字
                    else
                    {  //是普通标识符
                        if (word.Length <= 64)
                        {
                            Words.AddLast (new Token (word.ToString (), TokenType.Id, lineNo));// 自定义标识符不能超过64个字符
                            Words.Last.Value.StartPos = startPos;
                        }
                        else
                        {
                            Words.AddLast (new Token (word.ToString (), TokenType.Error, lineNo));
                            Words.Last.Value.StartPos = startPos;
                            ErrorInfoStrb.Append ("ERROR : Line: ").Append (lineNo).Append (" 标识符不得超过64个字符. ").Append ("(").Append (word).Append (")\n");
                        }
                    }
                }
                else if (IsDigit (oneChar) || (oneChar == '.'))
                {   //如果是数字
                    var realFlag = false; //判断是否为REAL类型
                    var isError = false;
                  
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
                    if (isError)
                    {
                        Words.AddLast (new Token (word.ToString (), TokenType.Error, lineNo));
                        Words.Last.Value.StartPos = startPos;
                        ErrorInfoStrb.Append ("ERROR : Line: ").Append (lineNo).Append (" 非法数字. ").Append ("(").Append (word).Append (")\n");
                    }
                    else
                    {
                        if (realFlag)
                        {
                            Words.AddLast (new Token (word.ToString (), TokenType.RealVal, lineNo));  //实型
                            Words.Last.Value.StartPos = startPos;
                        }
                        else
                        {
                            Words.AddLast (new Token (word.ToString (), TokenType.IntVal, lineNo));// 整型
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
                                    Words.AddLast (new Token ("%=", TokenType.ModAssign, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                }
                                else
                                {
                                    Words.AddLast (new Token ("%", TokenType.Mod, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                    i--;
                                }
                            }
                            catch (IndexOutOfRangeException)
                            {
                                Words.AddLast (new Token ("%", TokenType.Mod, lineNo));
                                Words.Last.Value.StartPos = startPos;
                            }
                            break;
                        case '+':
                            try
                            {
                                oneChar = Chars[++i];
                                if (oneChar == '+')
                                {   //检测++
                                    Words.AddLast (new Token ("++", TokenType.PlusPlus, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                }
                                else if (oneChar == '=')
                                {//检测+=
                                    Words.AddLast (new Token ("+=", TokenType.PlusAssign, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                }
                                else
                                {
                                    Words.AddLast (new Token ("+", TokenType.Plus, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                    //errorInfoStrb.Append("ERROR : Line: ").Append(lineNo).Append(" 非法字符'+'. ").Append("(").Append(word.ToString()).Append(")\n");
                                    i--;
                                }
                            }
                            catch (IndexOutOfRangeException)
                            {
                                Words.AddLast (new Token ("+", TokenType.Plus, lineNo));
                                Words.Last.Value.StartPos = startPos;
                            }
                            break;
                        case '-':
                            try
                            {
                                oneChar = Chars[++i];
                                if (oneChar == '-')
                                {   //检测--
                                    Words.AddLast (new Token ("--", TokenType.MinusMinus, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                }
                                else if (oneChar == '=')
                                { //检测-=
                                    Words.AddLast (new Token ("-=", TokenType.MinusAssign, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                }
                                else
                                {
                                    Words.AddLast (new Token ("-", TokenType.Minus, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                    //errorInfoStrb.Append("ERROR : Line: ").Append(lineNo).Append(" 非法字符'-'. ").Append("(").Append(word.ToString()).Append(")\n");
                                    i--;
                                }
                            }
                            catch (IndexOutOfRangeException)
                            {
                                Words.AddLast (new Token ("-", TokenType.Minus, lineNo));
                                Words.Last.Value.StartPos = startPos;
                            }
                            break;
                        case '*':
                            try
                            {
                                oneChar = Chars[++i];
                                if (oneChar == '=')
                                {   //检测*=
                                    Words.AddLast (new Token ("*=", TokenType.MulAssign, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                }
                                else
                                {
                                    Words.AddLast (new Token ("*", TokenType.Mul, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                    i--;
                                }
                            }
                            catch (IndexOutOfRangeException)
                            {
                                Words.AddLast (new Token ("*", TokenType.Mul, lineNo));
                                Words.Last.Value.StartPos = startPos;
                            }
                            break;
                        case '(':
                            Words.AddLast (new Token ("(", TokenType.LeftP, lineNo));
                            Words.Last.Value.StartPos = startPos;
                            break;
                        case ')':
                            Words.AddLast (new Token (")", TokenType.RightP, lineNo));
                            Words.Last.Value.StartPos = startPos;
                            break;
                        case '[':
                            Words.AddLast (new Token ("[", TokenType.LeftBrk, lineNo));
                            Words.Last.Value.StartPos = startPos;
                            break;
                        case ']':
                            Words.AddLast (new Token ("]", TokenType.RightBrk, lineNo));
                            Words.Last.Value.StartPos = startPos;
                            break;
                        case '{':
                            Words.AddLast (new Token ("{", TokenType.LeftBra, lineNo));
                            Words.Last.Value.StartPos = startPos;
                            break;
                        case '!':
                            Words.AddLast (new Token ("!", TokenType.Not, lineNo));
                            Words.Last.Value.StartPos = startPos;
                            break;
                        case '}':
                            Words.AddLast (new Token ("}", TokenType.RightBra, lineNo));
                            Words.Last.Value.StartPos = startPos;
                            break;
                        case ';':
                            Words.AddLast (new Token (";", TokenType.End, lineNo));
                            Words.Last.Value.StartPos = startPos;
                            break;
                        case ',':
                            Words.AddLast (new Token (",", TokenType.Comma, lineNo));
                            Words.Last.Value.StartPos = startPos;
                            break;
                        case '&':
                            try
                            {
                                oneChar = Chars[++i];
                                if (oneChar == '&')
                                {   //检测&&
                                    Words.AddLast (new Token ("&&", TokenType.And, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                }
                                else
                                {
                                    Words.AddLast (new Token ("&", TokenType.Error, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                    ErrorInfoStrb.Append ("ERROR : Line: ").Append (lineNo).Append (" 非法字符'&'. ").Append ("(").Append (word).Append (")\n");
                                    i--;
                                }
                            }
                            catch (IndexOutOfRangeException)
                            {
                                Words.AddLast (new Token ("&", TokenType.Error, lineNo));
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
                                    Words.AddLast (new Token ("||", TokenType.Or, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                }
                                else
                                {
                                    Words.AddLast (new Token ("|", TokenType.Error, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                    ErrorInfoStrb.Append ("ERROR : Line: ").Append (lineNo).Append (" 非法字符'|'. ").Append ("(").Append (word).Append (")\n");
                                    i--;
                                }
                            }
                            catch (IndexOutOfRangeException)
                            {
                                Words.AddLast (new Token ("|", TokenType.Error, lineNo));
                                Words.Last.Value.StartPos = startPos;
                            }
                            break;
                        case '<':
                            try
                            {
                                oneChar = Chars[++i];
                                if (oneChar == '>')   //不等于
                                {
                                    Words.AddLast (new Token ("<>", TokenType.Neq, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                }
                                else if (oneChar == '=')
                                { //小于等于
                                    Words.AddLast (new Token ("<=", TokenType.LessEq, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                }
                                else
                                {  //小于
                                    Words.AddLast (new Token ("<", TokenType.Less, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                    i--;
                                }
                                break;
                            }
                            catch (IndexOutOfRangeException)
                            {
                                Words.AddLast (new Token ("<", TokenType.Less, lineNo));
                                Words.Last.Value.StartPos = startPos;
                                break;
                            }

                        case '>':
                            try
                            {
                                oneChar = Chars[++i];
                                if (oneChar == '=')
                                { //大于等于
                                    Words.AddLast (new Token (">=", TokenType.GreaterEq, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                }
                                else
                                {   //大于
                                    Words.AddLast (new Token (">", TokenType.Greater, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                    i--;
                                }
                                break;
                            }
                            catch (IndexOutOfRangeException)
                            {
                                Words.AddLast (new Token (">", TokenType.Greater, lineNo));
                                Words.Last.Value.StartPos = startPos;
                                break;
                            }
                        case '=':
                            try
                            {
                                oneChar = Chars[++i];
                                if (oneChar == '=')
                                {  //等于
                                    Words.AddLast (new Token ("==", TokenType.Eq, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                }
                                else
                                {  //赋值
                                    Words.AddLast (new Token ("=", TokenType.Assign, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                    i--;
                                }
                                break;
                            }
                            catch (IndexOutOfRangeException)
                            {
                                Words.AddLast (new Token ("=", TokenType.Assign, lineNo));
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
                                    Words.AddLast (new Token ("/=", TokenType.DivAssign, lineNo)); //检测/*
                                    Words.Last.Value.StartPos = startPos;
                                }
                                else
                                {  //除法
                                    Words.AddLast (new Token ("/", TokenType.Div, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                    i--;
                                }
                                break;
                            }
                            catch (IndexOutOfRangeException)
                            {
                                Words.AddLast (new Token ("/", TokenType.Div, lineNo));
                                Words.Last.Value.StartPos = startPos;
                                break;
                            }
                        case '\'':
                            word.Append(oneChar);
                            try
                            {
                                oneChar = Chars[++i];
                                word.Append(oneChar);
                                if (oneChar == '\'')
                                {
                                    Words.AddLast(new Token("''", TokenType.CharVal, lineNo));
                                    Words.Last.Value.StartPos = startPos;
                                }
                                else if (oneChar == '\\')
                                {
                                    oneChar = Chars[++i];
                                    word.Append(oneChar);
                                    if (oneChar != 'r' && oneChar != 't' 
                                                       && oneChar != 'n' && oneChar != '\\' 
                                                       && oneChar != '\'' && oneChar != '0')
                                    {
                                        throw new IndexOutOfRangeException();
                                    }
                                    oneChar = Chars[++i];
                                    if (oneChar == '\'')
                                    {
                                        word.Append(oneChar);
                                        Words.AddLast(new Token(word.ToString(), TokenType.CharVal, lineNo));
                                        Words.Last.Value.StartPos = startPos;
                                    }
                                    else
                                    {
                                        i--;
                                        throw new IndexOutOfRangeException();
                                    }
                                }
                                else
                                {
                                    oneChar = Chars[++i];
                                    if(oneChar == '\'')
                                    {
                                        word.Append(oneChar);
                                        Words.AddLast(new Token(word.ToString(), TokenType.CharVal, lineNo));
                                    }
                                    else
                                    {
                                        i--;
                                        throw new IndexOutOfRangeException();
                                    }
                                }
                            }
                            catch (IndexOutOfRangeException)
                            {
                                Words.AddLast(new Token(word.ToString(), TokenType.Error, lineNo));
                                Words.Last.Value.StartPos = startPos;
                                ErrorInfoStrb.Append("ERROR : Line: ").Append(lineNo).Append(" 非法字符'").Append(word).Append("'. ").Append("(").Append(word).Append(")\n");
                            }
                            break;
                        default:
                            Words.AddLast (new Token (word.Append (oneChar).ToString (), TokenType.Error, lineNo));
                            Words.Last.Value.StartPos = startPos;
                            ErrorInfoStrb.Append ("ERROR : Line: ").Append (lineNo).Append (" 非法字符'").Append (word).Append ("'. ").Append ("(").Append (word).Append (")\n");
                            break;
                    }
                }
            }
        }
    }
}