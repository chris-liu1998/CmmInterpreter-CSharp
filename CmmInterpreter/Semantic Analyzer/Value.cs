using System;
using System.Globalization;
using System.Text;
using CmmInterpreter.Exceptions;

namespace CmmInterpreter.Semantic_Analyzer
{
    /// <summary>
    /// 值的类，用来在语义分析过程中存储变量或常量的值
    /// </summary>
    public class Value
    {
        public int Type { get; set; }  //存储变量值的类型
        public int IntVal { get; set; }
        public double RealVal { get; set; }
        public char CharVal { get; set; }
        public bool Bool { get; set; }
        public int[] IntArray { get; set; }
        public double[] RealArray { get; set; }
        public char[] CharArray { get; set; }
        public int [] DimArray { get; set; }

        public Value(int type)
        {
            Type = type;
            if (type == SymbolType.True)
                IntVal = 1;
            else if (type == SymbolType.False)
                IntVal = 0;
        }

        public Value(int type, object val)
        {
            Type = type;
            if (Type == SymbolType.IntValue)
            {
                IntVal = (int)val;
                if (IntVal != 0)
                    Bool = true;
            }
            else if (Type == SymbolType.RealValue)
            {
                RealVal = (double)val;
                if (Math.Abs(RealVal) > 0)
                    Bool = true;
            }
            else if (Type == SymbolType.CharValue)
            {
                CharVal = (char)val;
                if (CharVal != 0)
                    Bool = true;
            }
        }

        public Value(bool condition)
        {
            Type = condition ? SymbolType.True : SymbolType.False;
            IntVal = Type;
            if (IntVal == 0)
                Bool = false;
            Bool = true;
        }

        public void InitArray(int dim)
        {
            if (Type == SymbolType.IntArray)
            {
                IntArray = new int[dim];
            } 
            else if (Type == SymbolType.RealArray)
            {
                RealArray = new double[dim];
            }
            else
            {
                CharArray = new char[dim];
            }
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
                            case SymbolType.True:
                                result.IntVal = IntVal + 1;
                                return result;
                            case SymbolType.False:
                                result.IntVal = IntVal + 0;
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
                            case SymbolType.True:
                                result.RealVal = RealVal + 1;
                                return result;
                            case SymbolType.False:
                                result.RealVal = RealVal + 0;
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
                            case SymbolType.True:
                                result.IntVal = CharVal + 1;
                                return result;
                            case SymbolType.False:
                                result.IntVal = CharVal + 0;
                                return result;
                            case SymbolType.RealValue:
                                result.Type = SymbolType.RealValue;
                                result.RealVal = CharVal + value.RealVal;
                                return result;
                        }
                        break;
                    }
                case SymbolType.True:
                case SymbolType.False:
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
                            case SymbolType.True:
                                result.IntVal = IntVal + 1;
                                return result;
                            case SymbolType.False:
                                result.IntVal = IntVal + 0;
                                return result;
                            case SymbolType.RealValue:
                                result.Type = SymbolType.RealValue;
                                result.RealVal = IntVal + value.RealVal;
                                return result;
                        }
                        break;
                    }

            }
            throw new InterpretException("ERROR : 加法运算不合法.\n");
        }

        public Value Minus(Value value)
        {
            switch (Type)
            {
                case SymbolType.IntValue:
                    {
                        var result = new Value(SymbolType.IntValue);
                        switch (value.Type)
                        {
                            case SymbolType.IntValue:
                                result.IntVal = IntVal - value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                result.IntVal = IntVal - value.CharVal;
                                return result;
                            case SymbolType.True:
                                result.IntVal = IntVal - 1;
                                return result;
                            case SymbolType.False:
                                result.IntVal = IntVal - 0;
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
                        var result = new Value(SymbolType.RealValue);
                        switch (value.Type)
                        {
                            case SymbolType.IntValue:
                                result.RealVal = RealVal - value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                result.RealVal = RealVal - value.CharVal;
                                return result;
                            case SymbolType.True:
                                result.RealVal = RealVal - 1;
                                return result;
                            case SymbolType.False:
                                result.RealVal = RealVal - 0;
                                return result;
                            case SymbolType.RealValue:
                                result.RealVal = RealVal - value.RealVal;
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
                                result.IntVal = CharVal - value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                result.IntVal = CharVal - value.CharVal;
                                return result;
                            case SymbolType.True:
                                result.IntVal = CharVal - 1;
                                return result;
                            case SymbolType.False:
                                result.IntVal = CharVal - 0;
                                return result;
                            case SymbolType.RealValue:
                                result.Type = SymbolType.RealValue;
                                result.RealVal = CharVal - value.RealVal;
                                return result;
                        }
                        break;
                    }
                case SymbolType.True:
                case SymbolType.False:
                    {
                        var result = new Value(SymbolType.IntValue);
                        switch (value.Type)
                        {
                            case SymbolType.IntValue:
                                result.IntVal = IntVal - value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                result.IntVal = IntVal - value.CharVal;
                                return result;
                            case SymbolType.True:
                                result.IntVal = IntVal - 1;
                                return result;
                            case SymbolType.False:
                                result.IntVal = IntVal - 0;
                                return result;
                            case SymbolType.RealValue:
                                result.Type = SymbolType.RealValue;
                                result.RealVal = IntVal - value.RealVal;
                                return result;
                        }
                        break;
                    }

            }
            throw new InterpretException("ERROR : 减法运算不合法.\n");
        }

        public Value Mul(Value value)
        {
            switch (Type)
            {
                case SymbolType.IntValue:
                    {
                        var result = new Value(SymbolType.IntValue);
                        switch (value.Type)
                        {
                            case SymbolType.IntValue:
                                result.IntVal = IntVal * value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                result.IntVal = IntVal * value.CharVal;
                                return result;
                            case SymbolType.True:
                                result.IntVal = IntVal * 1;
                                return result;
                            case SymbolType.False:
                                result.IntVal = IntVal * 0;
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
                        var result = new Value(SymbolType.RealValue);
                        switch (value.Type)
                        {
                            case SymbolType.IntValue:
                                result.RealVal = RealVal * value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                result.RealVal = RealVal * value.CharVal;
                                return result;
                            case SymbolType.True:
                                result.RealVal = RealVal * 1;
                                return result;
                            case SymbolType.False:
                                result.RealVal = RealVal * 0;
                                return result;
                            case SymbolType.RealValue:
                                result.RealVal = RealVal * value.RealVal;
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
                                result.IntVal = CharVal * value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                result.IntVal = CharVal * value.CharVal;
                                return result;
                            case SymbolType.True:
                                result.IntVal = CharVal * 1;
                                return result;
                            case SymbolType.False:
                                result.IntVal = CharVal * 0;
                                return result;
                            case SymbolType.RealValue:
                                result.Type = SymbolType.RealValue;
                                result.RealVal = CharVal * value.RealVal;
                                return result;
                        }
                        break;
                    }
                case SymbolType.True:
                case SymbolType.False:
                    {
                        var result = new Value(SymbolType.IntValue);
                        switch (value.Type)
                        {
                            case SymbolType.IntValue:
                                result.IntVal = IntVal * value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                result.IntVal = IntVal * value.CharVal;
                                return result;
                            case SymbolType.True:
                                result.IntVal = IntVal * 1;
                                return result;
                            case SymbolType.False:
                                result.IntVal = IntVal * 0;
                                return result;
                            case SymbolType.RealValue:
                                result.Type = SymbolType.RealValue;
                                result.RealVal = IntVal * value.RealVal;
                                return result;
                        }
                        break;
                    }

            }
            throw new InterpretException("ERROR : 乘法运算不合法.\n");
        }

        public Value Div(Value value)
        {
            switch (Type)
            {
                case SymbolType.IntValue:
                    {
                        var result = new Value(SymbolType.IntValue);
                        switch (value.Type)
                        {
                            case SymbolType.IntValue:
                                if (value.IntVal == 0)
                                    throw new InterpretException("ERROR : 除以0.\n");
                                result.IntVal = IntVal / value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                if (value.CharVal == 0)
                                    throw new InterpretException("ERROR : 除以0.\n");
                                result.IntVal = IntVal / value.CharVal;
                                return result;
                            case SymbolType.True:
                                result.IntVal = IntVal / 1;
                                return result;
                            case SymbolType.False:
                                throw new InterpretException("ERROR : 除以0.\n");
                            case SymbolType.RealValue:
                                if (Math.Abs(value.RealVal) <= 0)
                                    throw new InterpretException("ERROR : 除以0.\n");
                                result.Type = SymbolType.RealValue;
                                result.RealVal = IntVal / value.RealVal;
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
                                if (value.IntVal == 0)
                                    throw new InterpretException("ERROR : 除以0.\n");
                                result.RealVal = RealVal / value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                if (value.CharVal == 0)
                                    throw new InterpretException("ERROR : 除以0.\n");
                                result.RealVal = RealVal / value.CharVal;
                                return result;
                            case SymbolType.True:
                                result.RealVal = RealVal / 1;
                                return result;
                            case SymbolType.False:
                                throw new InterpretException("ERROR : 除以0.\n");
                            case SymbolType.RealValue:
                                if (Math.Abs(value.RealVal) <= 0)
                                    throw new InterpretException("ERROR : 除以0.\n");
                                result.RealVal = RealVal / value.RealVal;
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
                                if (value.IntVal == 0)
                                    throw new InterpretException("ERROR : 除以0.\n");
                                result.IntVal = CharVal / value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                if (value.CharVal == 0)
                                    throw new InterpretException("ERROR : 除以0.\n");
                                result.IntVal = CharVal / value.CharVal;
                                return result;
                            case SymbolType.True:
                                result.IntVal = IntVal / 1;
                                return result;
                            case SymbolType.False:
                                throw new InterpretException("ERROR : 除以0.\n");
                            case SymbolType.RealValue:
                                if (Math.Abs(value.RealVal) <= 0)
                                    throw new InterpretException("ERROR : 除以0.\n");
                                result.Type = SymbolType.RealValue;
                                result.RealVal = CharVal / value.RealVal;
                                return result;
                        }
                        break;
                    }
                case SymbolType.True:
                case SymbolType.False:
                    {
                        var result = new Value(SymbolType.IntValue);
                        switch (value.Type)
                        {
                            case SymbolType.IntValue:
                                if (value.IntVal == 0)
                                    throw new InterpretException("ERROR : 除以0.\n");
                                result.IntVal = IntVal / value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                if (value.CharVal == 0)
                                    throw new InterpretException("ERROR : 除以0.\n");
                                result.IntVal = IntVal / value.CharVal;
                                return result;
                            case SymbolType.True:
                                result.IntVal = IntVal / 1;
                                return result;
                            case SymbolType.False:
                                throw new InterpretException("ERROR : 除以0.\n");
                            case SymbolType.RealValue:
                                if (Math.Abs(value.RealVal) <= 0)
                                    throw new InterpretException("ERROR : 除以0.\n");
                                result.Type = SymbolType.RealValue;
                                result.RealVal = IntVal / value.RealVal;
                                return result;
                        }
                        break;
                    }
            }
            throw new InterpretException("ERROR : 除法运算不合法.\n");
        }

        public Value Mod(Value value)
        {
            switch (Type)
            {
                case SymbolType.IntValue:
                    {
                        var result = new Value(SymbolType.IntValue);
                        switch (value.Type)
                        {
                            case SymbolType.IntValue:
                                if (value.IntVal == 0)
                                    throw new InterpretException("ERROR : 除以0.\n");
                                result.IntVal = IntVal % value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                if (value.CharVal == 0)
                                    throw new InterpretException("ERROR : 除以0.\n");
                                result.IntVal = IntVal % value.CharVal;
                                return result;
                            case SymbolType.True:
                                result.IntVal = IntVal % 1;
                                return result;
                            case SymbolType.False:
                                throw new InterpretException("ERROR : 除以0.\n");
                            case SymbolType.RealValue:
                                if (Math.Abs(value.RealVal) <= 0)
                                    throw new InterpretException("ERROR : 除以0.\n");
                                result.Type = SymbolType.RealValue;
                                result.RealVal = IntVal % value.RealVal;
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
                                if (value.IntVal == 0)
                                    throw new InterpretException("ERROR : 除以0.\n");
                                result.RealVal = RealVal % value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                if (value.CharVal == 0)
                                    throw new InterpretException("ERROR : 除以0.\n");
                                result.RealVal = RealVal % value.CharVal;
                                return result;
                            case SymbolType.True:
                                result.RealVal = RealVal % 1;
                                return result;
                            case SymbolType.False:
                                throw new InterpretException("ERROR : 除以0.\n");
                            case SymbolType.RealValue:
                                if (Math.Abs(value.RealVal) <= 0)
                                    throw new InterpretException("ERROR : 除以0.\n");
                                result.RealVal = RealVal % value.RealVal;
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
                                if (value.IntVal == 0)
                                    throw new InterpretException("ERROR : 除以0.\n");
                                result.IntVal = CharVal % value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                if (value.CharVal == 0)
                                    throw new InterpretException("ERROR : 除以0.\n");
                                result.IntVal = CharVal % value.CharVal;
                                return result;
                            case SymbolType.True:
                                result.IntVal = IntVal % 1;
                                return result;
                            case SymbolType.False:
                                throw new InterpretException("ERROR : 除以0.\n");
                            case SymbolType.RealValue:
                                if (Math.Abs(value.RealVal) <= 0)
                                    throw new InterpretException("ERROR : 除以0.\n");
                                result.Type = SymbolType.RealValue;
                                result.RealVal = CharVal % value.RealVal;
                                return result;
                        }
                        break;
                    }
                case SymbolType.True:
                case SymbolType.False:
                    {
                        var result = new Value(SymbolType.IntValue);
                        switch (value.Type)
                        {
                            case SymbolType.IntValue:
                                if (value.IntVal == 0)
                                    throw new InterpretException("ERROR : 除以0.\n");
                                result.IntVal = IntVal % value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                if (value.CharVal == 0)
                                    throw new InterpretException("ERROR : 除以0.\n");
                                result.IntVal = IntVal % value.CharVal;
                                return result;
                            case SymbolType.True:
                                result.IntVal = IntVal % 1;
                                return result;
                            case SymbolType.False:
                                throw new InterpretException("ERROR : 除以0.\n");
                            case SymbolType.RealValue:
                                if (Math.Abs(value.RealVal) <= 0)
                                    throw new InterpretException("ERROR : 除以0.\n");
                                result.Type = SymbolType.RealValue;
                                result.RealVal = IntVal % value.RealVal;
                                return result;
                        }
                        break;
                    }
            }
            throw new InterpretException("ERROR : 取余运算不合法.\n");
        }

        public Value GreaterThan(Value value)
        {
            switch (Type)
            {
                case SymbolType.IntValue:
                    switch (value.Type)
                    {
                        case SymbolType.IntValue:
                            return new Value(IntVal > value.IntVal);
                        case SymbolType.CharValue:
                            return new Value(IntVal > value.CharVal);
                        case SymbolType.True:
                            return new Value(IntVal > 1);
                        case SymbolType.False:
                            return new Value(IntVal > 0);
                        case SymbolType.RealValue:
                            return new Value(IntVal > value.RealVal);
                    }

                    break;
                case SymbolType.CharValue:
                    switch (value.Type)
                    {
                        case SymbolType.IntValue:
                            return new Value(CharVal > value.IntVal);
                        case SymbolType.CharValue:
                            return new Value(CharVal > value.CharVal);
                        case SymbolType.True:
                            return new Value(CharVal > 1);
                        case SymbolType.False:
                            return new Value(CharVal > 0);
                        case SymbolType.RealValue:
                            return new Value(CharVal > value.RealVal);
                    }

                    break;
                case SymbolType.RealValue:
                    switch (value.Type)
                    {
                        case SymbolType.IntValue:
                            return new Value(RealVal > value.IntVal);
                        case SymbolType.CharValue:
                            return new Value(RealVal > value.CharVal);
                        case SymbolType.True:
                            return new Value(RealVal > 1);
                        case SymbolType.False:
                            return new Value(RealVal > 0);
                        case SymbolType.RealValue:
                            return new Value(RealVal > value.RealVal);
                    }

                    break;
                case SymbolType.True:
                case SymbolType.False:
                    switch (value.Type)
                    {
                        case SymbolType.IntValue:
                            return new Value(IntVal > value.IntVal);
                        case SymbolType.CharValue:
                            return new Value(IntVal > value.CharVal);
                        case SymbolType.True:
                            return new Value(IntVal > 1);
                        case SymbolType.False:
                            return new Value(IntVal > 0);
                        case SymbolType.RealValue:
                            return new Value(IntVal > value.RealVal);
                    }

                    break;
            }
            throw new InterpretException("ERROR : 关系运算不合法.\n");
        }

        public Value LessThan(Value value)
        {
            switch (Type)
            {
                case SymbolType.IntValue:
                    switch (value.Type)
                    {
                        case SymbolType.IntValue:
                            return new Value(IntVal < value.IntVal);
                        case SymbolType.CharValue:
                            return new Value(IntVal < value.CharVal);
                        case SymbolType.True:
                            return new Value(IntVal < 1);
                        case SymbolType.False:
                            return new Value(IntVal < 0);
                        case SymbolType.RealValue:
                            return new Value(IntVal < value.RealVal);
                    }

                    break;
                case SymbolType.CharValue:
                    switch (value.Type)
                    {
                        case SymbolType.IntValue:
                            return new Value(CharVal < value.IntVal);
                        case SymbolType.CharValue:
                            return new Value(CharVal < value.CharVal);
                        case SymbolType.True:
                            return new Value(CharVal < 1);
                        case SymbolType.False:
                            return new Value(CharVal < 0);
                        case SymbolType.RealValue:
                            return new Value(CharVal < value.RealVal);
                    }

                    break;
                case SymbolType.RealValue:
                    switch (value.Type)
                    {
                        case SymbolType.IntValue:
                            return new Value(RealVal < value.IntVal);
                        case SymbolType.CharValue:
                            return new Value(RealVal < value.CharVal);
                        case SymbolType.True:
                            return new Value(RealVal < 1);
                        case SymbolType.False:
                            return new Value(RealVal < 0);
                        case SymbolType.RealValue:
                            return new Value(RealVal < value.RealVal);
                    }

                    break;
                case SymbolType.True:
                case SymbolType.False:
                    switch (value.Type)
                    {
                        case SymbolType.IntValue:
                            return new Value(IntVal < value.IntVal);
                        case SymbolType.CharValue:
                            return new Value(IntVal < value.CharVal);
                        case SymbolType.True:
                            return new Value(IntVal < 1);
                        case SymbolType.False:
                            return new Value(IntVal < 0);
                        case SymbolType.RealValue:
                            return new Value(IntVal < value.RealVal);
                    }

                    break;
            }
            throw new InterpretException("ERROR : 关系运算不合法.\n");
        }

        public Value Equal(Value value)
        {
            switch (Type)
            {
                case SymbolType.IntValue:
                    switch (value.Type)
                    {
                        case SymbolType.IntValue:
                            return new Value(IntVal == value.IntVal);
                        case SymbolType.CharValue:
                            return new Value(IntVal == value.CharVal);
                        case SymbolType.True:
                            return new Value(IntVal == 1);
                        case SymbolType.False:
                            return new Value(IntVal == 0);
                        case SymbolType.RealValue:
                            return new Value(Math.Abs(IntVal - value.RealVal) <= 0);
                    }

                    break;
                case SymbolType.CharValue:
                    switch (value.Type)
                    {
                        case SymbolType.IntValue:
                            return new Value(CharVal == value.IntVal);
                        case SymbolType.CharValue:
                            return new Value(CharVal == value.CharVal);
                        case SymbolType.True:
                            return new Value(CharVal == 1);
                        case SymbolType.False:
                            return new Value(CharVal == 0);
                        case SymbolType.RealValue:
                            return new Value(Math.Abs(IntVal - value.RealVal) <= 0);
                    }

                    break;
                case SymbolType.RealValue:
                    switch (value.Type)
                    {
                        case SymbolType.IntValue:
                            return new Value(Math.Abs(RealVal - value.IntVal) <= 0);
                        case SymbolType.CharValue:
                            return new Value(Math.Abs(RealVal - value.CharVal) <= 0);
                        case SymbolType.True:
                            return new Value(Math.Abs(RealVal - 1) <= 0);
                        case SymbolType.False:
                            return new Value(Math.Abs(RealVal) <= 0);
                        case SymbolType.RealValue:
                            return new Value(Math.Abs(RealVal - value.RealVal) <= 0);
                    }

                    break;
                case SymbolType.True:
                case SymbolType.False:
                    switch (value.Type)
                    {
                        case SymbolType.IntValue:
                            return new Value(IntVal == value.IntVal);
                        case SymbolType.CharValue:
                            return new Value(IntVal == value.CharVal);
                        case SymbolType.True:
                            return new Value(IntVal == 1);
                        case SymbolType.False:
                            return new Value(IntVal == 0);
                        case SymbolType.RealValue:
                            return new Value(Math.Abs(IntVal - value.RealVal) <= 0);
                    }

                    break;
            }
            throw new InterpretException("ERROR : 关系运算不合法.\n");
        }

        public bool Boolean()
        {
            return Math.Abs(RealVal) > 0 || IntVal != 0 || CharVal != 0;
        }

        public Value And(Value value)
        {
            if ((Boolean() && value.Boolean()))
            {
                return new Value(SymbolType.True);
            }
            return new Value(SymbolType.False);
        }

        public Value Or(Value value)
        {
            if (!Boolean() && !value.Boolean())
            {
                return new Value(SymbolType.False);
            }
            return new Value(SymbolType.True);
        }

        public static Value Not(Value value)
        {
            if (!value.Boolean())
            {
                return new Value(SymbolType.True);
            }
            return new Value(SymbolType.False);
        }

        public Value NotEqual(Value value)
        {
            return Not(Equal(value));
        }

        public Value LessEqual(Value value)
        {
            return Not(GreaterThan(value));
        }

        public Value GreaterEqual(Value value)
        {
            return Not(LessThan(value));
        }


        public Value ToReal()
        {
            if (Type == SymbolType.RealValue)
            {
                return this;
            }

            if (Type == SymbolType.IntValue)
            {
                Type = SymbolType.RealValue;
                RealVal = IntVal;
                IntVal = 0;
                return this;
            }
            Type = SymbolType.RealValue;
            RealVal = CharVal; 
            CharVal = '\0'; 
            return this;

        }

        public override string ToString()
        {
            switch (Type)
            {
                case SymbolType.IntValue:
                    return IntVal.ToString();
                case SymbolType.CharValue:
                    var str = new StringBuilder();
                    str.Append(CharVal);
                    return str.ToString();
                case SymbolType.RealValue:
                    return RealVal.ToString(CultureInfo.InvariantCulture);
                case SymbolType.True:
                    return $"true ({IntVal})";
                case SymbolType.False:
                    return $"false ({IntVal})";
                default:
                    return "Array";
            }
        }

    }
}
