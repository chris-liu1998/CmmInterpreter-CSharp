using System;
using System.Globalization;
using CmmInterpreter.Exceptions;

namespace CmmInterpreter.Semantic_Analyzer
{
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
            throw new InterpretException("加法运算不合法.");
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
            throw new InterpretException("减法运算不合法.");
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
            throw new InterpretException("乘法运算不合法.");
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
                                    throw new InterpretException("除以0");
                                result.IntVal = IntVal / value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                if (value.CharVal == 0)
                                    throw new InterpretException("除以0");
                                result.IntVal = IntVal / value.CharVal;
                                return result;
                            case SymbolType.True:
                                result.IntVal = IntVal / 1;
                                return result;
                            case SymbolType.False:
                                throw new InterpretException("除以0");
                            case SymbolType.RealValue:
                                if (Math.Abs(value.RealVal) <= 0)
                                    throw new InterpretException("除以0");
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
                                    throw new InterpretException("除以0");
                                result.RealVal = RealVal / value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                if (value.CharVal == 0)
                                    throw new InterpretException("除以0");
                                result.RealVal = RealVal / value.CharVal;
                                return result;
                            case SymbolType.True:
                                result.RealVal = RealVal / 1;
                                return result;
                            case SymbolType.False:
                                throw new InterpretException("除以0");
                            case SymbolType.RealValue:
                                if (Math.Abs(value.RealVal) <= 0)
                                    throw new InterpretException("除以0");
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
                                    throw new InterpretException("除以0");
                                result.IntVal = CharVal / value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                if (value.CharVal == 0)
                                    throw new InterpretException("除以0");
                                result.IntVal = CharVal / value.CharVal;
                                return result;
                            case SymbolType.True:
                                result.IntVal = IntVal / 1;
                                return result;
                            case SymbolType.False:
                                throw new InterpretException("除以0");
                            case SymbolType.RealValue:
                                if (Math.Abs(value.RealVal) <= 0)
                                    throw new InterpretException("除以0");
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
                                    throw new InterpretException("除以0");
                                result.IntVal = IntVal / value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                if (value.CharVal == 0)
                                    throw new InterpretException("除以0");
                                result.IntVal = IntVal / value.CharVal;
                                return result;
                            case SymbolType.True:
                                result.IntVal = IntVal / 1;
                                return result;
                            case SymbolType.False:
                                throw new InterpretException("除以0");
                            case SymbolType.RealValue:
                                if (Math.Abs(value.RealVal) <= 0)
                                    throw new InterpretException("除以0");
                                result.Type = SymbolType.RealValue;
                                result.RealVal = IntVal / value.RealVal;
                                return result;
                        }
                        break;
                    }
            }
            throw new InterpretException("除法运算不合法.");
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
                                    throw new InterpretException("除以0");
                                result.IntVal = IntVal % value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                if (value.CharVal == 0)
                                    throw new InterpretException("除以0");
                                result.IntVal = IntVal % value.CharVal;
                                return result;
                            case SymbolType.True:
                                result.IntVal = IntVal % 1;
                                return result;
                            case SymbolType.False:
                                throw new InterpretException("除以0");
                            case SymbolType.RealValue:
                                if (Math.Abs(value.RealVal) <= 0)
                                    throw new InterpretException("除以0");
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
                                    throw new InterpretException("除以0");
                                result.RealVal = RealVal % value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                if (value.CharVal == 0)
                                    throw new InterpretException("除以0");
                                result.RealVal = RealVal % value.CharVal;
                                return result;
                            case SymbolType.True:
                                result.RealVal = RealVal % 1;
                                return result;
                            case SymbolType.False:
                                throw new InterpretException("除以0");
                            case SymbolType.RealValue:
                                if (Math.Abs(value.RealVal) <= 0)
                                    throw new InterpretException("除以0");
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
                                    throw new InterpretException("除以0");
                                result.IntVal = CharVal % value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                if (value.CharVal == 0)
                                    throw new InterpretException("除以0");
                                result.IntVal = CharVal % value.CharVal;
                                return result;
                            case SymbolType.True:
                                result.IntVal = IntVal % 1;
                                return result;
                            case SymbolType.False:
                                throw new InterpretException("除以0");
                            case SymbolType.RealValue:
                                if (Math.Abs(value.RealVal) <= 0)
                                    throw new InterpretException("除以0");
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
                                    throw new InterpretException("除以0");
                                result.IntVal = IntVal % value.IntVal;
                                return result;
                            case SymbolType.CharValue:
                                if (value.CharVal == 0)
                                    throw new InterpretException("除以0");
                                result.IntVal = IntVal % value.CharVal;
                                return result;
                            case SymbolType.True:
                                result.IntVal = IntVal % 1;
                                return result;
                            case SymbolType.False:
                                throw new InterpretException("除以0");
                            case SymbolType.RealValue:
                                if (Math.Abs(value.RealVal) <= 0)
                                    throw new InterpretException("除以0");
                                result.Type = SymbolType.RealValue;
                                result.RealVal = IntVal % value.RealVal;
                                return result;
                        }
                        break;
                    }
            }
            throw new InterpretException("取余运算不合法.");
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
            throw new InterpretException("关系运算不合法.");
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
            throw new InterpretException("关系运算不合法.");
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
            throw new InterpretException("关系运算不合法.");
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

        public Value LeftIncrease()
        {
            Value result;
            switch (Type)
            {
                case SymbolType.IntValue:
                    result = new Value(SymbolType.IntValue);
                    result.IntVal = IntVal + 1;
                    return result;
                case SymbolType.CharValue:
                    result = new Value(SymbolType.CharValue);
                    result.CharVal = (char)(CharVal + 1);
                    return result;
                case SymbolType.True:
                case SymbolType.False:
                    result = new Value(SymbolType.IntValue);
                    result.IntVal = IntVal + 1;
                    return result;
                case SymbolType.RealValue:
                    result = new Value(SymbolType.RealValue);
                    result.RealVal = RealVal + 1;
                    return result;
            }

            throw new InterpretException("自增运算错误不合法.");
        }

        public Value LeftDecrease()
        {
            Value result;
            switch (Type)
            {
                case SymbolType.IntValue:
                    result = new Value(SymbolType.IntValue);
                    result.IntVal = IntVal - 1;
                    return result;
                case SymbolType.CharValue:
                    result = new Value(SymbolType.CharValue);
                    result.CharVal = (char)(CharVal - 1);
                    return result;
                case SymbolType.True:
                case SymbolType.False:
                    result = new Value(SymbolType.IntValue);
                    result.IntVal = IntVal - 1;
                    return result;
                case SymbolType.RealValue:
                    result = new Value(SymbolType.RealValue);
                    result.RealVal = RealVal - 1;
                    return result;
            }

            throw new InterpretException("自减运算错误不合法.");
        }

        public override string ToString()
        {
            switch (Type)
            {
                case SymbolType.IntValue:
                    return IntVal.ToString();
                case SymbolType.CharValue:
                    return CharVal.ToString();
                case SymbolType.RealValue:
                    return RealVal.ToString(CultureInfo.InvariantCulture);
                case SymbolType.True:
                    return $"true ({IntVal})";
                case SymbolType.False:
                    return $"False ({IntVal})";
                default:
                    return "Array";
            }
        }

    }
}
