using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmmInterpreter.Semantic_Analyzer
{
    /// <summary>
    /// 符号类型
    /// </summary>
    public class SymbolType
    {
        public const int Temp = -1;
        public const int False = 0;
        public const int True = 1;
        public const int IntValue = 2;
        public const int RealValue = 3;
        public const int CharValue = 4;
        public const int IntArray = 5;
        public const int RealArray = 6;
        public const int CharArray = 7;
    }
}
