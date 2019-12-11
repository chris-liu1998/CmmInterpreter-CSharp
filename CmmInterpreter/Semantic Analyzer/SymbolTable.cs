using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmmInterpreter.Exceptions;

namespace CmmInterpreter.Semantic_Analyzer
{

    public class SymbolList
    {
        public List<Symbol> Symbols { get; set; }

        public SymbolList()
        {
            Symbols = new List<Symbol>();
        }
    }
    /// <summary>
    /// 定义符号表，用来构造符号表
    /// </summary>
    public class SymbolTable
    {
        private const string TempPrefix = "$temp";  //临时变量前缀

        public static LinkedList<SymbolList> symbolTable = new LinkedList<SymbolList>();
        private static LinkedList<Symbol> _tempNames = new LinkedList<Symbol>();
        public static LinkedListNode<SymbolList> Current;
        public static int Level = -1;

        public SymbolTable()
        {
            Level++;
            var symbolList = new SymbolList();
            symbolTable.AddLast(symbolList);
            Current = symbolTable.Last;
        }

        public static void Register(Symbol symbol)
        {
            if(Current.Value.Symbols.FindAll(s => s.Name == symbol.Name).Count != 0)
                throw new InterpretException($"ERROR : 变量{symbol.Name}重复声明.\n");
            Current.Value.Symbols.Add(symbol);
        }

        public static void DeRegister()
        {
            symbolTable.RemoveLast();
            Current = symbolTable.Last;
            Level--;
        }

        /// <summary>
        /// 获取符号
        /// </summary>
        /// <param name="name"></param>
        /// <returns>符号symbol</returns>
        public static Symbol FindSymbol(string name)
        {
            while (true)
            {
                var symbol = Current.Value.Symbols.FirstOrDefault(s => s.Name == name);
                if (symbol != null)
                {
                    Current = symbolTable.Last;
                    return symbol;
                }

                if (Current.Previous != null)
                    Current = Current.Previous;
                else
                {
                    Current = symbolTable.Last;
                    break;
                }
            }

            if (name.StartsWith(TempPrefix))
            {
                var tempSymbol = _tempNames.FirstOrDefault(s => s.Name == name);
                if (tempSymbol == null)
                {
                    var symbol = new Symbol(name, SymbolType.Temp, -1);
                    _tempNames.AddLast(symbol);
                    tempSymbol = symbol;
                }
                return tempSymbol;
            }

            throw new InterpretException($"ERROR : 变量{name} 未定义或未初始化.\n");

        }

        /// <summary>
        /// 取单值
        /// </summary>
        /// <param name="name"></param>
        /// <returns>
        /// 符号的值
        /// </returns>
        public static Value GetSymbolValue(string name)
        {
            return FindSymbol(name).Value;
        }

        /// <summary>
        /// 取得数组的某个值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="index"></param>
        /// <returns>
        /// 数组内的值
        /// </returns>
        public static Value GetSymbolValue(string name, int index)
        {
            if (index == -1)
                return GetSymbolValue(name);
            var symbol = FindSymbol(name);
            if (symbol.Type == SymbolType.IntArray)
            {
                if (index >= symbol.Value.IntArray.Length)
                    throw new InterpretException($"ERROR : 数组 <{name}> 下标 {index} 越界.\n");
                var val = new Value(SymbolType.IntValue) {IntVal = symbol.Value.IntArray[index]};
                return val;
            }

            if (symbol.Type == SymbolType.RealArray)
            {
                if (index >= symbol.Value.RealArray.Length)
                    throw new InterpretException($"ERROR : 数组 <{name}> 下标 {index} 越界.\n");
                var val = new Value(SymbolType.RealValue) {RealVal = symbol.Value.RealArray[index]};
                return val;
            }

            if(symbol.Type == SymbolType.CharArray)
            {
                if (index >= symbol.Value.CharArray.Length)
                    throw new InterpretException($"ERROR : 数组 <{name}> 下标 {index} 越界.\n");
                var val = new Value(SymbolType.CharValue) {CharVal = symbol.Value.CharArray[index]};
                return val;
            }

            throw new InterpretException($"ERROR : {name}变量不是数组.\n");
        }

        /// <summary>
        /// 获取临时变量名
        /// </summary>
        /// <returns>
        /// 临时变量
        /// </returns>
        public static Symbol GetTempSymbol()
        {
            for (var i = 1; ; i++)
            {
                var temp = TempPrefix + i;
                var exist = false;

                foreach (var s in _tempNames)
                {
                    if (s.Name.Equals(temp))
                    {
                        exist = true;
                        break;
                    }
                }
                if (exist)
                {
                    continue;
                }
                var symbol = new Symbol(temp, SymbolType.Temp, -1);
                _tempNames.AddLast(symbol);
                return symbol;
            }
        }

        public static void ClearTempNames()
        {
            _tempNames.Clear();
        }

           /// <summary>
           /// 设置单值
           /// </summary>
           /// <param name="name"></param>
           /// <param name="value"></param>
        public static void SetSymbolValue(string name, Value value)
        {
            FindSymbol(name).Value = value;
        }

          /// <summary>
          /// 设置整型数组的值
          /// </summary>
          /// <param name="name"></param>
          /// <param name="value"></param>
          /// <param name="index"></param>
        public static void SetSymbolValue(string name, int value, int index)
        {
            var symbol = FindSymbol(name);
            if (symbol.Type == SymbolType.CharArray)
            {
                if (index < symbol.Value.CharArray.Length)
                    symbol.Value.CharArray[index] = (char)value;
                else
                    throw new InterpretException($"ERROR : 数组 <{name}> 下标 {index} 越界.\n");
            }
            else
            {
                if (index < symbol.Value.IntArray.Length)
                    symbol.Value.IntArray[index] = value;
                else
                    throw new InterpretException($"ERROR : 数组 <{name}> 下标 {index} 越界.\n");
            }
           
        }

          /// <summary>
          /// 设置实型数组的值
          /// </summary>
          /// <param name="name"></param>
          /// <param name="value"></param>
          /// <param name="index"></param>
        public static void SetSymbolValue(string name, double value, int index)
        {
            var symbol = FindSymbol(name);
            if (index < symbol.Value.RealArray.Length)
                symbol.Value.RealArray[index] = value;
            else
                throw new InterpretException($"ERROR : 数组 <{name}> 下标 {index} 越界.\n");
        }

          /// <summary>
          /// 设置字符数组的值
          /// </summary>
          /// <param name="name"></param>
          /// <param name="value"></param>
          /// <param name="index"></param>
        public static void SetSymbolValue(string name, char value, int index)
        {
            var symbol = FindSymbol(name);
            if (symbol.Type == SymbolType.IntArray)
            {
                if (index < symbol.Value.IntArray.Length)
                    symbol.Value.IntArray[index] = (char)value;
                else
                    throw new InterpretException($"ERROR : 数组 <{name}> 下标 {index} 越界.\n");
            }
            else
            {
                if (index < symbol.Value.CharArray.Length)
                    symbol.Value.CharArray[index] = value;
                else
                    throw new InterpretException($"ERROR : 数组 <{name}> 下标 {index} 越界.\n");
            }
        }

        public static void DeleteTables()
        {
            symbolTable.Clear();
            _tempNames.Clear();
        }
    }
}
