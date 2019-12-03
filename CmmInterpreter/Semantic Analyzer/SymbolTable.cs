using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmmInterpreter.Exceptions;

namespace CmmInterpreter.Semantic_Analyzer
{
    /// <summary>
    /// 定义符号表，用来构造符号表
    /// </summary>
    public class SymbolTable
    {
        private const string TempPrefix = "$temp";  //临时变量前缀
        public List<Symbol> Symbols { get; set; }
                                                   
        public static LinkedList<SymbolTable> SymbolTables = new LinkedList<SymbolTable>();
        private static LinkedList<Symbol> _tempNames = new LinkedList<Symbol>();
        public static LinkedListNode<SymbolTable> Current = SymbolTables.Last;
        public static int Level = -1;

        public SymbolTable()
        {
            Level++;
            Symbols = new List<Symbol>();
        }

        public void Register(Symbol symbol)
        {
            if(Symbols.FindAll(s => s.Name == symbol.Name).Count != 0)
                throw new InterpretException($"变量{symbol.Name}重复声明.");
            Symbols.Add(symbol);
        }
        //public void Register(Symbol symbol)
        //{
        //    if (Symbols.FindAll(s => s.Name == symbol.Name).Count != 0)
        //    {
        //        if (Symbols.FindAll(s => s.Level >= symbol.Level).Count != 0)
        //            throw new InterpretException($"变量{symbol.Name}重复声明.");
        //        var sym = Symbols.FirstOrDefault(s => s.Level < symbol.Level);
        //        var index = Symbols.IndexOf(sym);
        //        Symbols[index] = symbol;
        //        symbol.NextSameSymbol = sym;
        //    }

        //    Symbols.Add(symbol);
        //}

        public static void DeRegister()
        {
            SymbolTables.RemoveLast();
            Level--;
        }

        /// <summary>
        /// 获取符号
        /// </summary>
        /// <param name="name"></param>
        /// <returns>符号symbol</returns>
        public static Symbol FindSymbol(string name)
        {
            while (Current.Previous != null)
            {
                var symbol = Current.Value.Symbols.FirstOrDefault(s => s.Name == name);
                if (symbol != null)
                {
                    Current = SymbolTables.Last;
                    return symbol;
                }
                Current = Current.Previous;
            }

            if (!name.StartsWith(TempPrefix)) return null;
            {
                var tempSymbol = _tempNames.FirstOrDefault(s => s.Name == name);
                if (tempSymbol != null) return tempSymbol;
                var ts = new Symbol(name, SymbolType.Temp, -1);
                _tempNames.AddLast(ts);
                return ts;
            }

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
            var symbol = FindSymbol(name);
            if (symbol.Type == SymbolType.IntArray)
            {
                if (index >= symbol.Value.IntArray.Length)
                    throw new InterpretException($"数组 <{name}> 下标 {index} 越界.");
                var val = new Value(SymbolType.IntValue) {IntVal = symbol.Value.IntArray[index]};
                return val;
            }

            if (symbol.Type == SymbolType.RealArray)
            {
                if (index >= symbol.Value.RealArray.Length)
                    throw new InterpretException($"数组 <{name}> 下标 {index} 越界.");
                var val = new Value(SymbolType.RealValue) {RealVal = symbol.Value.RealArray[index]};
                return val;
            }

            if(symbol.Type == SymbolType.CharArray)
            {
                if (index >= symbol.Value.CharArray.Length)
                    throw new InterpretException($"数组 <{name}> 下标 {index} 越界.");
                var val = new Value(SymbolType.CharValue) {CharVal = symbol.Value.CharArray[index]};
                return val;
            }

            throw new InterpretException($"{name}变量不是数组.");
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
                foreach(var s in _tempNames)
                {
                    if (s.Name.Equals(temp))
                    {
                        exist = true;
                        break;
                    }
                }

                if (FindSymbol(temp) != null)
                {
                    exist = true;
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
            if (index < symbol.Value.IntArray.Length)
                symbol.Value.IntArray[index] = value;
            else
                throw new InterpretException($"数组 <{name}> 下标 {index} 越界.");
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
                throw new InterpretException($"数组 <{name}> 下标 {index} 越界.");
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
            if (index < symbol.Value.CharArray.Length)
                symbol.Value.CharArray[index] = value;
            else
                throw new InterpretException($"数组 <{name}> 下标 {index} 越界.");
        }

        public static void DeleteTables()
        {
            SymbolTables.Clear();
            _tempNames.Clear();
        }
    }
}
