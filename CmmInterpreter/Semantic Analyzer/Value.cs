using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmmInterpreter.Exceptions;

namespace CmmInterpreter.Semantic_Analyzer
{
    public class Value
    {
        public int Type { get; set; }  //存储变量值的类型
        public int IntVal { get; set; }
        public double RealVal { get; set; }
        public char CharVal { get; set; }
        public int[] IntArray { get; set; }
        public double[] RealArray { get; set; }
        public char[] CharArray { get; set; }

        public Value(int type)
        {
            Type = type;
        }

        public Value(bool condition)
        {
            Type = condition ? SymbolType.True : SymbolType.False;
        }

        public void InitArray()
        {

        }

        public Value Plus(Value value)
        {
            switch (Type)
            {
                case SymbolType.IntValue:
                    {
                        var result = new Value(SymbolType.IntValue);
                        switch (value.Type)
                        {
                            case SymbolType.IntValue:
                                result.IntVal = IntVal + value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                result.IntVal = IntVal + value.CharVal;
                                return result;
                            case SymbolType.RealValue:
                                result.Type = SymbolType.RealValue;
                                result.RealVal = IntVal + value.RealVal;
                                return result;
                        }
                        break;
                    }
                case SymbolType.RealValue:
                    {
                        var result = new Value(SymbolType.RealValue);
                        switch (value.Type)
                        {
                            case SymbolType.IntValue:
                                result.RealVal = RealVal + value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                result.RealVal = RealVal + value.CharVal;
                                return result;
                            case SymbolType.RealValue:
                                result.RealVal = RealVal + value.RealVal;
                                return result;
                        }
                        break;
                    }
                case SymbolType.CharValue:
                    {
                        var result = new Value(SymbolType.IntValue);
                        switch (value.Type)
                        {
                            case SymbolType.IntValue:
                                result.IntVal = CharVal + value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                result.IntVal = CharVal + value.CharVal;
                                return result;
                            case SymbolType.RealValue:
                                result.Type = SymbolType.RealValue;
                                result.RealVal = CharVal + value.RealVal;
                                return result;
                        }
                        break;
                    }
            }
            throw new InterpretException("加法运算不合法.");
        }
        public Value Minus(Value value)
        {
            Value result;
            switch (Type)
            {
                case SymbolType.IntValue:
                    {
                        result = new Value(SymbolType.IntValue);
                        switch (value.Type)
                        {
                            case SymbolType.IntValue:
                                result.IntVal = IntVal - value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                result.IntVal = IntVal - value.CharVal;
                                return result;
                            case SymbolType.RealValue:
                                result.Type = SymbolType.RealValue;
                                result.RealVal = IntVal - value.RealVal;
                                return result;
                        }
                        break;
                    }
                case SymbolType.RealValue:
                    {
                        result = new Value(SymbolType.RealValue);
                        switch (value.Type)
                        {
                            case SymbolType.IntValue:
                                result.RealVal = RealVal - value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                result.RealVal = RealVal - value.CharVal;
                                return result;
                            case SymbolType.RealValue:
                                result.RealVal = RealVal - value.RealVal;
                                return result;
                        }
                        break;
                    }
                case SymbolType.CharValue:
                    {
                        result = new Value(SymbolType.IntValue);
                        switch (value.Type)
                        {
                            case SymbolType.IntValue:
                                result.IntVal = CharVal - value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                result.IntVal = CharVal - value.CharVal;
                                return result;
                            case SymbolType.RealValue:
                                result.Type = SymbolType.RealValue;
                                result.RealVal = CharVal - value.RealVal;
                                return result;
                        }
                        break;
                    }
            }
            throw new InterpretException("减法运算不合法.");
        }
        public Value Mul(Value value)
        {
            Value result;
            switch (Type)
            {
                case SymbolType.IntValue:
                    {
                        result = new Value(SymbolType.IntValue);
                        switch (value.Type)
                        {
                            case SymbolType.IntValue:
                                result.IntVal = IntVal * value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                result.IntVal = IntVal * value.CharVal;
                                return result;
                            case SymbolType.RealValue:
                                result.Type = SymbolType.RealValue;
                                result.RealVal = IntVal * value.RealVal;
                                return result;
                        }
                        break;
                    }
                case SymbolType.RealValue:
                    {
                        result = new Value(SymbolType.RealValue);
                        switch (value.Type)
                        {
                            case SymbolType.IntValue:
                                result.RealVal = RealVal * value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                result.RealVal = RealVal * value.CharVal;
                                return result;
                            case SymbolType.RealValue:
                                result.RealVal = RealVal * value.RealVal;
                                return result;
                        }
                        break;
                    }
                case SymbolType.CharValue:
                    {
                        result = new Value(SymbolType.IntValue);
                        switch (value.Type)
                        {
                            case SymbolType.IntValue:
                                result.IntVal = CharVal * value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                result.IntVal = CharVal * value.CharVal;
                                return result;
                            case SymbolType.RealValue:
                                result.Type = SymbolType.RealValue;
                                result.RealVal = CharVal * value.RealVal;
                                return result;
                        }
                        break;
                    }
            }
            throw new InterpretException("乘法运算不合法.");
        }

        public Value Div(Value value)
        {
            Value result;
            switch (Type)
            {
                case SymbolType.IntValue:
                    {
                        result = new Value(SymbolType.IntValue);
                        switch (value.Type)
                        {
                            case SymbolType.IntValue:
                                if (value.IntVal == 0)
                                    throw new InterpretException("除以0");
                                result.IntVal = IntVal / value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                if (value.CharVal == 0)
                                    throw new InterpretException("除以0");
                                result.IntVal = IntVal / value.CharVal;
                                return result;
                            case SymbolType.RealValue:
                                if (value.RealVal == 0)
                                    throw new InterpretException("除以0");
                                result.Type = SymbolType.RealValue;
                                result.RealVal = IntVal / value.RealVal;
                                return result;
                        }
                        break;
                    }
                case SymbolType.RealValue:
                    {
                        result = new Value(SymbolType.RealValue);
                        switch (value.Type)
                        {
                            case SymbolType.IntValue:
                                if (value.IntVal == 0)
                                    throw new InterpretException("除以0");
                                result.RealVal = RealVal / value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                if (value.CharVal == 0)
                                    throw new InterpretException("除以0");
                                result.RealVal = RealVal / value.CharVal;
                                return result;
                            case SymbolType.RealValue:
                                if (value.RealVal == 0 )
                                    throw new InterpretException("除以0");
                                result.RealVal = RealVal / value.RealVal;
                                return result;
                        }
                        break;
                    }
                case SymbolType.CharValue:
                    {
                        result = new Value(SymbolType.IntValue);
                        switch (value.Type)
                        {
                            case SymbolType.IntValue:
                                if (value.IntVal == 0)
                                    throw new InterpretException("除以0");
                                result.IntVal = CharVal / value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                if (value.CharVal == 0)
                                    throw new InterpretException("除以0");
                                result.IntVal = CharVal / value.CharVal;
                                return result;
                            case SymbolType.RealValue:
                                if (value.RealVal == 0)
                                    throw new InterpretException("除以0");
                                result.Type = SymbolType.RealValue;
                                result.RealVal = CharVal / value.RealVal;
                                return result;
                        }
                        break;
                    }
            }
            throw new InterpretException("除法运算不合法.");
        }
        public Value Mod(Value value)
        {
            Value result;
            switch (Type)
            {
                case SymbolType.IntValue:
                    {
                        result = new Value(SymbolType.IntValue);
                        switch (value.Type)
                        {
                            case SymbolType.IntValue:
                                if (value.IntVal == 0)
                                    throw new InterpretException("除以0");
                                result.IntVal = IntVal % value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                if (value.CharVal == 0)
                                    throw new InterpretException("除以0");
                                result.IntVal = IntVal % value.CharVal;
                                return result;
                            case SymbolType.RealValue:
                                if (value.RealVal == 0)
                                    throw new InterpretException("除以0");
                                result.Type = SymbolType.RealValue;
                                result.RealVal = IntVal % value.RealVal;
                                return result;
                        }
                        break;
                    }
                case SymbolType.RealValue:
                    {
                        result = new Value(SymbolType.RealValue);
                        switch (value.Type)
                        {
                            case SymbolType.IntValue:
                                if (value.IntVal == 0)
                                    throw new InterpretException("除以0");
                                result.RealVal = RealVal % value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                if (value.CharVal == 0)
                                    throw new InterpretException("除以0");
                                result.RealVal = RealVal % value.CharVal;
                                return result;
                            case SymbolType.RealValue:
                                if (value.RealVal == 0)
                                    throw new InterpretException("除以0");
                                result.RealVal = RealVal % value.RealVal;
                                return result;
                        }
                        break;
                    }
                case SymbolType.CharValue:
                    {
                        result = new Value(SymbolType.IntValue);
                        switch (value.Type)
                        {
                            case SymbolType.IntValue:
                                if (value.IntVal == 0)
                                    throw new InterpretException("除以0");
                                result.IntVal = CharVal % value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                if (value.CharVal == 0)
                                    throw new InterpretException("除以0");
                                result.IntVal = CharVal % value.CharVal;
                                return result;
                            case SymbolType.RealValue:
                                if (value.RealVal == 0)
                                    throw new InterpretException("除以0");
                                result.Type = SymbolType.RealValue;
                                result.RealVal = CharVal % value.RealVal;
                                return result;
                        }
                        break;
                    }
            }
            throw new InterpretException("取余运算不合法.");
        }
    }
}
