using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmmInterpreter.Semantic_Analyzer
{
    /// <summary>
    /// 符号类，用来构造符号表里的符号
    /// </summary>
    public class Symbol
    {
        public int Level { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public Value Value { get; set; }
        
        public Symbol NextSameSymbol { get; set; }

        public Symbol(string name, int type, int level)
        {
            Name = name;
            Type = type;
            Level = level;
            Value = new Value(type);
        }
    }
}
