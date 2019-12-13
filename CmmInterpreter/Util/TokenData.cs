using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmmInterpreter.Util
{
    public class TokenData
    {
        public int LineNo { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public string TypeName { get; set; }

        public TokenData(int lineno, string name, int type, string typename )
        {
            LineNo = lineno;
            Name = name;
            Type = type;
            TypeName = typename;
        }
    }
}
