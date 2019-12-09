namespace CmmInterpreter.Semantic_Analyzer
{
    /// <summary>
    /// 四元式类，用来构造四元式
    /// </summary>
    public class Quadruple
    {
        public int Line { get; set; }
        public string Instruction { get; set; }
        public string First { get; set; }
        public string Second { get; set; }
        public string Third{ get; set; }
        

        public Quadruple(string instr, string first,string second, string third, int line)
        {
            Instruction = instr;
            First = first;
            Second = second;
            Third = third;
            Line = line + 1;
        }

        public override string ToString()
        {
            if (Instruction == null)
                Instruction = "null";
            if (First == null)
                First = "null";
            if (Second == null)
                Second = "null";
            if (Third == null)
                Third = "null";
            return $"{Line} : ({Instruction}, {First}, {Second}, {Third})\n";
        }
    }
}
