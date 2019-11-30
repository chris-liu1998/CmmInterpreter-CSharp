using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmmInterpreter.Syntactic_Analyzer
{
    public class StmtType
    {
        public const int NONE = 0;
        public const int IF_ST = 1;
        public const int ELSE_ST = 2;
        public const int WHILE_ST = 3;
        public const int SCAN_ST = 4;
        public const int PRINT_ST = 5;
        public const int DEC_ST = 6;  // 声明语句
        public const int ASSIGN_ST = 7;// 赋值语句
        public const int VAR = 8; //变量
        public const int EXP = 9;  //表达式
        public const int FACTOR = 10; //因子
        public const int STBLOCK = 11; // 语句块
        public const int OPR = 12; //操作符
        public const int NULL = 13; //NULL
        public const int VALUE = 14;
        public const int MORE_FACTOR = 15;
        public const int BREAK = 16;
        public const int CONTINUE = 17;
        public const int MORE_LOGIC_EXP = 18;
        public const int MORE_ADD_EXP = 19;
        public const int INIT = 20;// 初始化语句
        public const int VALUE_LIST = 21;
        public const int MORE_VALUE = 22;
        public const int MORE_TERM = 23;
        public const int TERM = 24;
        public const int JUMP_ST = 25;
        public const int PROGRAM = 26;
        public const int STMT_SEQ = 27;
        public const int IF_BLOCK = 28;
        public const int VAR_LIST = 29;
        public const int MORE_VAR = 30;
        public const int ARRAY_DIM = 31;
        public const int MORE_COMP_EXP = 32;
        public const int ID = 33;
    }
}
