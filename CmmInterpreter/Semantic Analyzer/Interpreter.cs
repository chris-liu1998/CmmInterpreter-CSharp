using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CmmInterpreter.Exceptions;

namespace CmmInterpreter.Semantic_Analyzer
{
    /// <summary>
    /// 语义分析器——执行中间代码
    /// </summary>
    public class Interpreter
    {
        private int Level;
        private int pc;
        private SymbolTable symbolTable;
        public LinkedList<Quadruple> Codes { get; set; }
        public string Error { get; set; }

        public Interpreter()
        {
            pc = 0;
            Level = 0;
        }

        public void RunCode()
        {
            var length = Codes.Count;
            symbolTable = new SymbolTable();
            try
            {
                while (pc < length)
                {
                    InterpretQuadruple(Codes.ElementAt(pc));
                }
            }
            catch (InterpretException e)
            {
                Error = e.Message;
            }
            SymbolTable.DeleteTables();
        }

        private void InterpretQuadruple(Quadruple code)
        {
            var instrType = code.Instruction;
            if (instrType.Equals(InstructionType.Jump)) //跳转指令
            {
                if (code.First == null || SymbolTable.GetSymbolValue(code.First).Type == SymbolType.False) //需要跳转
                {
                    pc = int.Parse(code.Third);
                    return;
                }
            }
            if (instrType.Equals(InstructionType.Scan))   //输入指令
            {
                var input = Console.ReadLine();
                var type = SymbolTable.FindSymbol(GetId(code.Third)).Type;
                switch (type)
                {
                    case SymbolType.IntArray:
                    case SymbolType.IntValue:
                        {
                            var value = ParseValue(input);
                            if (value.Type == SymbolType.IntValue)
                            {
                                SetValue(code.Third, value);
                            }
                            else
                            {
                                Console.WriteLine(@"Error : 类型不匹配.");
                            }
                            break;
                        }
                    case SymbolType.RealArray:
                    case SymbolType.RealValue:
                        {
                            var value = ParseValue(input);
                            SetValue(code.Third, value);
                            break;
                        }
                    case SymbolType.CharArray:
                    case SymbolType.CharValue:
                    {
                        var value = ParseValue(input);
                        if (value.Type == SymbolType.IntValue)
                        {
                            value.CharVal = (char) value.IntVal;
                            SetValue(code.Third, value);
                        }
                        if (value.Type == SymbolType.CharValue)
                        {
                            SetValue(code.Third, value);
                        }
                        else
                        {
                            Console.WriteLine(@"Error : 类型不匹配.");
                        }
                        break;
                    }
                }
            }
            if (instrType.Equals(InstructionType.Print))
            {
                var index = -1;
                if (IsArrayElement(code.Third))
                {
                    index = GetIndex(code.Third);
                }
                Console.WriteLine(SymbolTable.GetSymbolValue(code.Third, index - 1));
            }
            if (instrType.Equals(InstructionType.In))
            {
                Level++;
                var table = new SymbolTable();
            }
            if (instrType.Equals(InstructionType.Out))
            {
                SymbolTable.DeRegister();
                Level--;
            }
            if (instrType.Equals(InstructionType.Int))
            {
                if (code.Second != null && code.First != null)
                {
                    var symbol = new Symbol(code.Third, SymbolType.IntArray, Level);
                    symbol.Value.InitArray(GetLength(code.Second));
                    SymbolTable.Register(symbol);
                }
                else
                {
                    var intValue = 0;
                    if (code.First != null)
                    {
                        intValue = GetInt(code.First);
                    }
                    var symbol = new Symbol(code.Third, SymbolType.IntValue, Level, intValue);
                    SymbolTable.Register(symbol);
                }
            }
            if (instrType.Equals(InstructionType.Real))
            {
                if (code.Second != null)
                {
                    var symbol = new Symbol(code.Third, SymbolType.RealArray, Level);
                    symbol.Value.InitArray(GetLength(code.Second));
                    SymbolTable.Register(symbol);
                }
                else
                {
                    double doubleValue = 0;
                    if (code.First != null)
                    {
                        doubleValue = GetDouble(code.First);
                    }
                    Symbol symbol = new Symbol(code.Third, SymbolType.RealValue, Level, doubleValue);
                    SymbolTable.Register(symbol);
                }
            }

            if (instrType.Equals(InstructionType.Char))
            {
                if (code.Second != null)
                {
                    var symbol = new Symbol(code.Third, SymbolType.RealArray, Level);
                    symbol.Value.InitArray(GetLength(code.Second));
                    SymbolTable.Register(symbol);
                }
                else
                {
                    double doubleValue = 0;
                    if (code.First != null)
                    {
                        doubleValue = GetDouble(code.First);
                    }
                    var symbol = new Symbol(code.Third, SymbolType.RealValue, Level, doubleValue);
                    SymbolTable.Register(symbol);
                }
            }
            if (instrType.Equals(InstructionType.Assign))
            {
                var value = GetValue(code.First);
                SetValue(code.Third, value);
            }
            if (instrType.Equals(InstructionType.Plus))
            {
                SetValue(code.Third, GetValue(code.First).Plus(GetValue(code.Second)));
            }
            if (instrType.Equals(InstructionType.Minus))
            {
                if (code.Second != null)
                {
                    SetValue(code.Third, GetValue(code.First).Minus(GetValue(code.Second)));
                }
                else
                {
                    SetValue(code.Third, Value.Not(GetValue(code.First)));
                }
            }
            if (instrType.Equals(InstructionType.Mul))
            {
                SetValue(code.Third, GetValue(code.First).Mul(GetValue(code.Second)));
            }
            if (instrType.Equals(InstructionType.Div))
            {
                SetValue(code.Third, GetValue(code.First).Div(GetValue(code.Second)));
            }
            if (instrType.Equals(InstructionType.Mod))
            {
                SetValue(code.Third, GetValue(code.First).Mod(GetValue(code.Second)));
            }
            if (instrType.Equals(InstructionType.GreaterThan))
            {
                SetValue(code.Third, GetValue(code.First).GreaterThan(GetValue(code.Second)));
            }
            if (instrType.Equals(InstructionType.LessThan))
            {
                SetValue(code.Third, GetValue(code.First).LessThan(GetValue(code.Second)));
            }
            if (instrType.Equals(InstructionType.Eq))
            {
                SetValue(code.Third, GetValue(code.First).Equal(GetValue(code.Second)));
            }
            if (instrType.Equals(InstructionType.GreaterEqThan))
            {
                SetValue(code.Third, GetValue(code.First).GreaterEqual(GetValue(code.Second)));
            }
            if (instrType.Equals(InstructionType.LessEqThan))
            {
                SetValue(code.Third, GetValue(code.First).LessEqual(GetValue(code.Second)));
            }
            if (instrType.Equals(InstructionType.NotEq))
            {
                SetValue(code.Third, GetValue(code.First).NotEqual(GetValue(code.Second)));
            }
            if (instrType.Equals(InstructionType.Not))
            {
                SetValue(code.Third, Value.Not(GetValue(code.First)));
            }
            pc++;
        }

        private string GetId(string name)
        {
            return IsArrayElement(name) ? name.Substring(0, name.IndexOf("[", StringComparison.Ordinal)) : name;
        }

        private Value GetValue(string name)
        {
            var regex1 = new Regex("\\d*\\.\\d*");
            var regex2 = new Regex("\\d+");
            var regex3 = new Regex("");
            if (regex1.IsMatch(name))
            {
                var value = new Value(SymbolType.RealValue);
                value.RealVal = double.Parse(name);
                return value;
            }
            if (regex2.IsMatch(name))
            {
                var value = new Value(SymbolType.IntValue);
                value.IntVal = int.Parse(name);
                return value;
            }
            if (regex3.IsMatch(name))
            {
                var value = new Value(SymbolType.CharValue);
                value.CharVal = char.Parse(name);
                return value;
            }
            var index = -1;
            if (IsArrayElement(name))
            {
                index = GetIndex(name);
            }
            return SymbolTable.GetSymbolValue(GetId(name), index);
        }

        private double GetDouble(string name)
        {
            var pattern = "^(-?\\d+)(\\.\\d+)?$";
            var regex = new Regex(pattern);
            if (regex.IsMatch(name))
            {
                return double.Parse(name);
            }
            var valueInt = SymbolTable.GetSymbolValue(name);
            if (valueInt.Type != SymbolType.RealValue)
            {
                valueInt.RealVal = valueInt.IntVal;
                valueInt.IntVal = 0;
            }
           
            return valueInt.RealVal;
        }

        private int GetInt(string name)
        {
            var pattern = "^(-?\\d+)$";
            var regex = new Regex(pattern);
            if (regex.IsMatch(name))
            {
                return int.Parse(name);
            }
            var valueInt = SymbolTable.GetSymbolValue(name);
            if (valueInt.Type == SymbolType.IntValue)
            {
                return valueInt.IntVal;
            }
            throw new InterpretException("Error : 类型应为int.\n");
        }
        private int GetChar(string name)
        {
            var pattern = "^(\\'\\\\[ntrf\\\\'\"]\\')|(\\'(.?)\\')|(\\'\\s\\')$";
            var regex = new Regex(pattern);
            if (regex.IsMatch(name))
            {
                name = name.Substring(1, name.Length - 2);
                return char.Parse(name);
            }
            var valueInt = SymbolTable.GetSymbolValue(name);
            if (valueInt.Type == SymbolType.IntValue)
            {
                return valueInt.IntVal;
            }
            throw new InterpretException("Error : 类型应为int.\n");
        }

        private int GetLength(string name)
        {
            var dimStr = name.Substring(name.IndexOf("[") + 1, name.Length - 2);
            string[] str;

            dimStr = dimStr.Replace("][", ",");

            str = dimStr.Split(',');
            var length = 1;
            foreach (var s in str)
            {
                length *= GetInt(s);
            }

            return length;
        }
        private int GetIndex(string name, params int[] dims)      //多维数组转为一维数组的索引
        {
            var indexStr = name.Substring(name.IndexOf("[") + 1, name.Length - 2);
            string[] str;

            indexStr = indexStr.Replace("][", ",");

            str = indexStr.Split(',');
            if (dims.Length != str.Length - 1)
                throw new InterpretException("ERROR : 数组索引错误.\n");
            var index = 0;
            var indexes = new List<int>();
            foreach (var s in str)
            {
                indexes.Add(GetInt(s));
            }

            var count = 0;
            foreach (var d in dims)
            {
                index += indexes[count] * d;
                count++;
            }
            return index;
        }

        private bool IsArrayElement(string name)
        {
            return name.Contains("[");
        }

        private void SetValue(string name, Value value)
        {
            var index = -1;
            if (IsArrayElement(name))
            {
                index = GetIndex(name);
            }
            var type = SymbolTable.FindSymbol(GetId(name)).Type;
            switch (type)
            {
                case SymbolType.IntValue:
                case SymbolType.RealValue:
                {
                    if (type == SymbolType.RealValue)
                    {
                        SymbolTable.SetSymbolValue(GetId(name), value.ToReal());
                    }
                    else
                    {
                        if (value.Type == SymbolType.RealValue)
                        {
                            Console.WriteLine($"Error : 表达式{name}与变量类型不匹配");
                        }
                        else
                        {
                            SymbolTable.SetSymbolValue(GetId(name), value);
                        }
                    }
                    break;
                }
                case SymbolType.IntArray:
                case SymbolType.RealArray:
                {
                    if (SymbolTable.GetSymbolValue(GetId(name), index).Type == SymbolType.RealValue)
                    {
                        SymbolTable.SetSymbolValue(GetId(name), value.ToReal().RealVal, index);
                    }
                    else
                    {
                        if (value.Type == SymbolType.RealValue)
                        {
                            Console.WriteLine($"Error : 表达式{name}与变量类型不匹配");
                        }
                        else
                        {
                            SymbolTable.SetSymbolValue(GetId(name), value.IntVal, index);
                        }
                    }
                    break;
                }
                case SymbolType.Temp:
                    SymbolTable.SetSymbolValue(GetId(name), value);
                    break;
            }
        }

        private Value ParseValue(string input)
        { ;
            var regex1 = new Regex("^(-?\\d+)(\\.\\d?)$");
            var regex2 = new Regex("^(-?\\d+)$");
            var regex3 = new Regex("^(\\s)|(.?)|(\\[ntrf\'\"\\])$");
            if (regex1.IsMatch(input))
            {
                var value = new Value(SymbolType.RealValue) {RealVal = double.Parse(input)};
                return value;
            }
            if (regex2.IsMatch(input))
            {
                var value = new Value(SymbolType.IntValue) {IntVal = int.Parse(input)};
                return value;
            }
            if (regex3.IsMatch(input))
            {
                var value = new Value(SymbolType.CharValue) {CharVal = char.Parse(input)};
                return value;
            }
            throw new InterpretException("ERROR : 输入非法\n");
        }
    }
}
