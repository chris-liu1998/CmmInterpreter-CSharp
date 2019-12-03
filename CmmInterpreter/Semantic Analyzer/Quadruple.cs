namespace CmmInterpreter.Semantic_Analyzer
{
    /// <summary>
    /// 四元式类，用来构造四元式
    /// </summary>
    public class Quadruple
    {
        public string Instruction { get; set; }
        public string First { get; set; }
        public string Second { get; set; }
        public string Third{ get; set; }
        

        public Quadruple(string instr, string first,string second, string third)
        {
            Instruction = instr;
            First = first;
            Second = second;
            Third = third;
        }

        public override string ToString()
        {
            return $"({Instruction}, {First}, {Second}, {Third})";
        }
    }
}
