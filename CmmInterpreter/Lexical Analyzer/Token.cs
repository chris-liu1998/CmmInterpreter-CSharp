namespace CmmInterpreter.Lexical_Analyzer
{
    /// <summary>
    /// token类，用来创建token
    /// </summary>
    public class Token
    {
        public string Value { get; set; }
        public int Type { get; set; }
        public int LineNo { get; set; }
        public int StartPos { get; set; }

        public Token(int type)
        {
            Type = type;
        }

        public Token(string value, int type, int lineNo)
        {
            Value = value;
            Type = type;
            LineNo = lineNo;
        }

        public string TypeToString()
        {
            switch (Type)
            {
                case TokenType.Int:
                    return "INT";
                case TokenType.IntVal:
                    return "INT_VALUE";
                case TokenType.Real:
                    return "REAL";
                case TokenType.RealVal:
                    return "REAL_VALUE";
                case TokenType.Char:
                    return "CHAR";
                case TokenType.CharVal:
                    return "CHAR_VALUE";
                case TokenType.Id:
                    return "IDENTIFIER";
                case TokenType.End:
                    return ";";
                case TokenType.Assign:
                    return "=";
                case TokenType.LeftP:
                    return "(";
                case TokenType.RightP:
                    return ")";
                case TokenType.RightBra:
                    return "}";
                case TokenType.LeftBra:
                    return "{";
                case TokenType.LeftBrk:
                    return "[";
                case TokenType.RightBrk:
                    return "]";
                case TokenType.While:
                    return "WHILE";
                case TokenType.If:
                    return "IF";
                case TokenType.Else:
                    return "ELSE";
                case TokenType.Neq:
                    return "NOT_EQ";
                case TokenType.Eq:
                    return "EQ";
                case TokenType.Mod:
                    return "MOD";
                case TokenType.ModAssign:
                    return "MOD_ASSIGN";
                case TokenType.MulAssign:
                    return "MUL_ASSIGN";
                case TokenType.DivAssign:
                    return "DIV_ASSIGN";
                case TokenType.PlusAssign:
                    return "PLUS_ASSIGN";
                case TokenType.MinusAssign:
                    return "MINUS_ASSIGN";
                case TokenType.GreaterEq:
                    return "GREATER_EQ";
                case TokenType.LessEq:
                    return "LESS_EQ";
                case TokenType.Less:
                    return "LESS";
                case TokenType.Greater:
                    return "GREATER";
                case TokenType.Break:
                    return "BREAK";
                case TokenType.Continue:
                    return "CONTINUE";
                case TokenType.Plus:
                    return "PLUS";
                case TokenType.Minus:
                    return "MINUS";
                case TokenType.Mul:
                    return "MULTIPLY";
                case TokenType.Div:
                    return "DIVIDE";
                case TokenType.And:
                    return "AND";
                case TokenType.Or:
                    return "OR";
                case TokenType.Not:
                    return "NOT";
                case TokenType.PlusPlus:
                    return "SELF_INC";
                case TokenType.MinusMinus:
                    return "SELF_DEC";
                case TokenType.Null:
                    return "NULL";
                case TokenType.CompEqExp:
                    return "COMP_EQ_EXP";
                case TokenType.CompExp:
                    return "COMP_EXP";
                case TokenType.LogicAndExp:
                    return "LOGIC_AND_EXP";
                case TokenType.LogicOrExp:
                    return "LOGIC_OR_EXP";
                case TokenType.AdditiveExp:
                    return "ADDITIVE_EXP";
                case TokenType.AssignExp:
                    return "ASSIGN_EXP";
                case TokenType.Error:
                    return "ERROR";
                default:
                    return "NONE";
            }
        }


        public override string ToString()
        {
            return ("起始位置：" + StartPos + "\t<value: '" + Value + "', " + "TokenType: " +
                    TypeToString() + " (" + Type + ")" + ", " + "lineNo: " + LineNo + ">\n");
        }
    }
}