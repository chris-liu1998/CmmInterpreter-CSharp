using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmmInterpreter.Lexical_Analyzer
{
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
                case TokenType.INT:
                    return "INT";
                case TokenType.INT_VAL:
                    return "INT_VALUE";
                case TokenType.REAL:
                    return "REAL";
                case TokenType.REAL_VAL:
                    return "REAL_VALUE";
                case TokenType.ID:
                    return "Identifier";
                case TokenType.END:
                    return ";";
                case TokenType.ASSIGN:
                    return "=";
                case TokenType.LEFT_P:
                    return "(";
                case TokenType.RIGHT_P:
                    return ")";
                case TokenType.RIGHT_BRA:
                    return "}";
                case TokenType.LEFT_BRA:
                    return "{";
                case TokenType.LEFT_BRK:
                    return "[";
                case TokenType.RIGHT_BRK:
                    return "]";
                case TokenType.WHILE:
                    return "WHILE";
                case TokenType.IF:
                    return "IF";
                case TokenType.ELSE:
                    return "ELSE";
                case TokenType.NEQ:
                    return "NOT_EQ";
                case TokenType.EQ:
                    return "EQ";
                case TokenType.MOD:
                    return "MOD";
                case TokenType.MOD_ASSIGN:
                    return "MOD_ASSIGN";
                case TokenType.MUL_ASSIGN:
                    return "MUL_ASSIGN";
                case TokenType.DIV_ASSIGN:
                    return "DIV_ASSIGN";
                case TokenType.PLUS_ASSIGN:
                    return "PLUS_ASSIGN";
                case TokenType.MINUS_ASSIGN:
                    return "MINUS_ASSIGN";
                case TokenType.GREATER_EQ:
                    return "GREATER_EQ";
                case TokenType.LESS_EQ:
                    return "LESS_EQ";
                case TokenType.LESS:
                    return "LESS";
                case TokenType.GREATER:
                    return "GREATER";
                case TokenType.BREAK:
                    return "BREAK";
                case TokenType.CONTINUE:
                    return "CONTINUE";
                case TokenType.PLUS:
                    return "PLUS";
                case TokenType.MINUS:
                    return "MINUS";
                case TokenType.MUL:
                    return "MULTIPLY";
                case TokenType.DIV:
                    return "DIVIDE";
                case TokenType.AND:
                    return "AND";
                case TokenType.OR:
                    return "OR";
                case TokenType.NOT:
                    return "NOT";
                case TokenType.PLUS_PLUS:
                    return "SELF_INC";
                case TokenType.MINUS_MINUS:
                    return "SELF_DEC";
                case TokenType.NULL:
                    return "NULL";
                case TokenType.ERROR:
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