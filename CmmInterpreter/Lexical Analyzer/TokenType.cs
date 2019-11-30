using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmmInterpreter.Lexical_Analyzer
{
    public class TokenType
    {
        public const int ERROR = -1;
        public const int NULL = 0;
        public const int INT = 1;
        public const int REAL = 2;
        public const int IF = 3;
        public const int ELSE = 4;
        public const int WHILE = 5;
        public const int PLUS = 6; //+
        public const int MINUS = 7; //-
        public const int DIV = 8; // /
        public const int MUL = 9; //*
        public const int ASSIGN = 10; //=
        public const int EQ = 11; //==
        public const int LEFT_P = 12; //(
        public const int RIGHT_P = 13; //)
        public const int LESS = 14; //<
        public const int GREATER = 15; //>
        public const int NEQ = 16; //<>
        public const int END = 17; //;
        public const int L_COMMENT = 18; // //
        public const int B_COMMENT = 19; // /* */
        public const int LEFT_BRK = 20; // [
        public const int RIGHT_BRK = 21; // ]
        public const int MOD = 22; // %
        public const int LEFT_BRA = 23; // {
        public const int RIGHT_BRA = 24; // }
        public const int ID = 25; // 标识符
        public const int INT_VAL = 26; // 整型值
        public const int REAL_VAL = 27; // 浮点型值
        public const int LESS_EQ = 28; // <=
        public const int GREATER_EQ = 29; // >=
        public const int AND = 30; // &&
        public const int OR = 31; // ||
        public const int CHAR = 32; // char
        public const int FOR = 33; // for
        public const int BREAK = 34; // break
        public const int CONTINUE = 35; // continue
        public const int PRINT = 36; // print
        public const int SCAN = 37; // scan
        public const int COMMA = 38; // ,
        public const int NOT = 39; //!
        public const int MINUS_MINUS = 40; //--
        public const int PLUS_PLUS = 41; //++
        public const int PLUS_ASSIGN = 42; // +=
        public const int MINUS_ASSIGN = 43; // -=
        public const int MUL_ASSIGN = 44; // *=
        public const int DIV_ASSIGN = 45; // /=
        public const int MOD_ASSIGN = 46; // %=
    }
}