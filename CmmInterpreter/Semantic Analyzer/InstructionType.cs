namespace CmmInterpreter.Semantic_Analyzer
{
    /// <summary>
    /// 解释器所用的指令集
    /// </summary>
    public class InstructionType
    {
        public const string Jump = "jump";
        public const string Scan = "scan";
        public const string Print = "print";
        public const string In = "in";
        public const string Out = "out";
        public const string Int = "int";
        public const string Real = "real";
        public const string Assign = "assign";
        //public const string PlusAssign = "plus_assign";
        //public const string MinusAssign = "minus_assign";
        //public const string MulAssign = "mul_assign";
        //public const string DivAssign = "div_assign";
        //public const string ModAssign = "mod_assign";
        public const string Plus = "+";
        public const string Minus = "-";
        public const string Mul = "*";
        public const string Div = "/";
        //public const string PlusPlus = "++";
        //public const string MinusMinus = "--";
        public const string GreaterThan = ">";
        public const string LessThan = "<";
        public const string GreaterEqThan = ">=";
        public const string LessEqThan = "<=";
        public const string Eq = "==";
        public const string NotEq = "<>";
        public const string And = "&&";
        public const string Or = "||";
        public const string Not = "!";
        public const string Char = "char";
        public const string Mod = "%";
    }
}
