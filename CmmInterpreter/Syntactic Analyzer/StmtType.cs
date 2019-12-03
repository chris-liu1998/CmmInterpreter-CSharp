namespace CmmInterpreter.Syntactic_Analyzer
{
    /// <summary>
    /// 语句类型
    /// </summary>
    public class StmtType
    {
        public const int None = 0;
        public const int IfStmt = 1;
        public const int ElseStmt = 2;
        public const int WhileStmt = 3;
        public const int ScanStmt = 4;
        public const int PrintStmt = 5;
        public const int DecStmt = 6;  // 声明语句
        public const int AssignStmt = 7;// 赋值语句
        public const int Var = 8; //变量
        public const int Exp = 9;  //表达式
        public const int Factor = 10; //因子
        public const int StmtBlock = 11; // 语句块
        public const int Operator = 12; //操作符
        public const int Null = 13; //NULL
        public const int Value = 14;
        public const int MoreFactor = 15;
        public const int Break = 16;
        public const int Continue = 17;
        public const int MoreLogicExp = 18;
        public const int MoreAddExp = 19;
        public const int Init = 20;// 初始化语句
        public const int ValueList = 21;
        public const int MoreValue = 22;
        public const int MoreTerm = 23;
        public const int Term = 24;
        public const int JumpSt = 25;
        public const int Program = 26;
        public const int StmtSeq = 27;
        public const int IfBlock = 28;
        public const int VarList = 29;
        public const int MoreVar = 30;
        public const int ArrayDim = 31;
        public const int MoreCompExp = 32;
        public const int Id = 33;
    }
}
