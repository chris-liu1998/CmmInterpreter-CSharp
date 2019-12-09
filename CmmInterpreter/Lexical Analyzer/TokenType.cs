namespace CmmInterpreter.Lexical_Analyzer
{
    /// <summary>
    /// 种别码
    /// </summary>
    public class TokenType
    {
        public const int Error = -1;
        public const int Null = 0;
        public const int Int = 1;
        public const int Real = 2;
        public const int If = 3;
        public const int Else = 4;
        public const int While = 5;
        public const int Plus = 6; //+
        public const int Minus = 7; //-
        public const int Div = 8; // /
        public const int Mul = 9; //*
        public const int Assign = 10; //=
        public const int Eq = 11; //==
        public const int LeftP = 12; //(
        public const int RightP = 13; //)
        public const int Less = 14; //<
        public const int Greater = 15; //>
        public const int Neq = 16; //<>
        public const int End = 17; //;
        public const int LineComment = 18; // //
        public const int BlockComment = 19; // /* */
        public const int LeftBrk = 20; // [
        public const int RightBrk = 21; // ]
        public const int Mod = 22; // %
        public const int LeftBra = 23; // {
        public const int RightBra = 24; // }
        public const int Id = 25; // 标识符
        public const int IntVal = 26; // 整型值
        public const int RealVal = 27; // 浮点型值
        public const int LessEq = 28; // <=
        public const int GreaterEq = 29; // >=
        public const int And = 30; // &&
        public const int Or = 31; // ||
        public const int Char = 32; // char
        public const int For = 33; // for
        public const int Break = 34; // break
        public const int Continue = 35; // continue
        public const int Print = 36; // print
        public const int Scan = 37; // scan
        public const int Comma = 38; // ,
        public const int Not = 39; //!
        public const int MinusMinus = 40; //--
        public const int PlusPlus = 41; //++
        public const int PlusAssign = 42; // +=
        public const int MinusAssign = 43; // -=
        public const int MulAssign = 44; // *=
        public const int DivAssign = 45; // /=
        public const int ModAssign = 46; // %=
        public const int CharVal = 47; //char型值

        public const int LogicOrExp = 48;
        public const int LogicAndExp = 49;
        public const int CompEqExp = 50;
        public const int CompExp = 51;
        public const int AdditiveExp = 52;
        public const int AssignExp = 53;
    }
}