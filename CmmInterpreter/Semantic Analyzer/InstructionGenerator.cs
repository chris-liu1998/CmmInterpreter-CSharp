using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmmInterpreter.Exceptions;
using CmmInterpreter.Lexical_Analyzer;
using CmmInterpreter.Syntactic_Analyzer;

namespace CmmInterpreter.Semantic_Analyzer
{
    /// <summary>
    /// 语义分析器——中间代码生成
    /// </summary>
    public class InstructionGenerator
    {
        private int Level { get; set; }
        private int Line { get; set; }
        public LinkedList<Quadruple> Codes { get; set; }
        private SymbolTable symbolTable;
        public string Error { get; set; }
        private bool isVar;
        private int inPos;
        private int outPos;
        private bool isRepeat;
        private Quadruple code;
        public TreeNode Tree { get; set; }

        public void GenerateInstructions()
        {
            Line = -1;
            Level = 0;
            Codes = new LinkedList<Quadruple>();
            symbolTable = new SymbolTable();
            try
            {
                InterpretStmtSeq(Tree.LeftNode);
            }
            catch (InterpretException e)
            {
                Error = e.Message;
            }

            SymbolTable.DeleteTables();
        }

        public void InterpretStmtSeq(TreeNode node)
        {
            while (true)
            {
                if (node.LeftNode != null) InterpretStmt(node.LeftNode);
                if (node.MiddleNode != null)
                {
                    node = node.MiddleNode;
                    continue;
                }

                break;
            }
        }

        public void InterpretStmt(TreeNode node)
        {
            switch (node.Type)
            {
                case StmtType.IfStmt:
                    InterpretIfStmt(node);
                    break;
                case StmtType.WhileStmt:
                    InterpretWhileStmt(node);
                    break;
                case StmtType.PrintStmt:
                    InterpretPrintStmt(node);
                    break;
                case StmtType.ScanStmt:
                    InterpretScanStmt(node);
                    break;
                case StmtType.AssignStmt:
                    InterpretAssignStmt(node);
                    break;
                case StmtType.DecStmt:
                    InterpretDecStmt(node);
                    break;
                case StmtType.JumpStmt:
                    InterpretJumpStmt(node, out Quadruple jumpCode);
                    if(jumpCode.Second.Equals("break"))
                        code = jumpCode;
                    break;
                case StmtType.StmtBlock:
                    InterpretStmtBlock(node);
                    break;
                case StmtType.Exp:
                case StmtType.Term:
                case StmtType.Factor:
                case StmtType.Value:
                case StmtType.Null:
                case StmtType.Var:
                    InterpretExp(node);
                    break;
            }
            SymbolTable.ClearTempNames();
        }

        private void InterpretStmtBlock(TreeNode node)
        {
            Codes.AddLast(new Quadruple(InstructionType.In, null, null, null, Line));
            Line++;
            Level++;
            var table = new SymbolTable();
            if(node.LeftNode != null)
                InterpretStmtSeq(node.LeftNode);
            SymbolTable.DeRegister();
            Level--;
            Codes.AddLast(new Quadruple(InstructionType.Out, null, null, null, Line));
            Line++;
        }

        private void InterpretJumpStmt(TreeNode node, out Quadruple jumpCode)
        {
            if (!isRepeat) throw new InterpretException("ERROR : 跳转语句需要在循环体内.\n");
            var jump = new Quadruple(InstructionType.Jump, null, null, null, Line);
            if (node.DataType == TokenType.Break)
            {
                jump.Second = "break";
                jump.Third = outPos.ToString();
            } 
            else if (node.DataType == TokenType.Continue)
            {
                jump.Second = "continue";
                jump.Third = inPos.ToString();
            }
            jumpCode = jump;
            Codes.AddLast(jump);
            Line++;
        }

        private void InterpretDecStmt(TreeNode node)
        {
            var table = SymbolTable.symbolTable.Last.Value;
            var type = node.DataType;
            var variable = node.LeftNode;
            InterpretDecVariable(variable, type, table);
            while (node.NextNode != null)
            {
                node = node.NextNode;
                InterpretDecVariable(node, type, table);
            }
        }

        private void InterpretDecVariable(TreeNode node, int type, SymbolList table)
        {
            var variable = node;
            if (variable.MiddleNode == null)
            {
                string value = null;
                if (variable.RightNode != null)
                {
                    value = InterpretExp(variable.RightNode.MiddleNode);
                }

                if (type == TokenType.Int)
                {
                    Codes.AddLast(new Quadruple(InstructionType.Int, value, null, variable.LeftNode.Value, Line));
                    Line++;
                    var symbol = new Symbol(variable.LeftNode.Value, SymbolType.IntValue, Level);
                    SymbolTable.Register(symbol);
                }
                else if (type == TokenType.Real)
                {
                    Codes.AddLast(new Quadruple(InstructionType.Real, value, null, variable.LeftNode.Value, Line));
                    Line++;
                    var symbol = new Symbol(variable.LeftNode.Value, SymbolType.RealValue, Level);
                    SymbolTable.Register(symbol);
                }
                else if (type == TokenType.Char)
                {
                    Codes.AddLast(new Quadruple(InstructionType.Char, value, null, variable.LeftNode.Value, Line));
                    Line++;
                    var symbol = new Symbol(variable.LeftNode.Value, SymbolType.CharValue, Level);
                    SymbolTable.Register(symbol);
                }
            }
            else if (variable.MiddleNode != null)       //数组暂时不能初始化
            {
                if (variable.RightNode != null)
                    throw new InterpretException("ERROR : 初始化非法.\n");
                var len = InterpretArrayDim(variable.MiddleNode, out var dim);
                if (type == TokenType.Int)
                {
                    
                    Codes.AddLast(new Quadruple(InstructionType.Int, dim.ToString(), len, variable.LeftNode.Value, Line));
                    Line++;
                    var symbol = new Symbol(variable.LeftNode.Value, SymbolType.IntArray, Level);
                    SymbolTable.Register(symbol);
                }
                else if(type == TokenType.Real)
                {
                    Codes.AddLast(new Quadruple(InstructionType.Real, dim.ToString(), len, variable.LeftNode.Value, Line));
                    Line++;
                    var symbol = new Symbol(variable.LeftNode.Value, SymbolType.RealArray, Level);
                    SymbolTable.Register(symbol);
                }
                else if (type == TokenType.Char)
                {
                    Codes.AddLast(new Quadruple(InstructionType.Char, dim.ToString(), len, variable.LeftNode.Value, Line));
                    Line++;
                    var symbol = new Symbol(variable.LeftNode.Value, SymbolType.CharArray, Level);
                    SymbolTable.Register(symbol);
                }

            }

        }

        private void InterpretAssignStmt(TreeNode node)
        {
            InterpretExp(node.LeftNode);
        }

        private void InterpretScanStmt(TreeNode node)
        {
            var targetNode = node.LeftNode.LeftNode;
            var type = SymbolTable.FindSymbol(targetNode.Value).Type;
            switch (type)
            {
                case SymbolType.IntValue:
                case SymbolType.RealValue:
                case SymbolType.CharValue:
                    Codes.AddLast(new Quadruple(InstructionType.Scan, null, null, targetNode.Value, Line));
                    Line++;
                    break;
                case SymbolType.IntArray:
                case SymbolType.RealArray:
                case SymbolType.CharArray:
                    Codes.AddLast(new Quadruple(InstructionType.Scan, null, null, $"{targetNode.Value}{InterpretArrayDim(node.LeftNode.MiddleNode, out var dim)}", Line));
                    Line++;
                    break;
                default:
                    throw new InterpretException("Error : 输入语句有误.\n");
            }
        }

        private string InterpretArrayDim(TreeNode node, out int dim)
        {
            dim = 1;

            var str = "[" + InterpretExp(node.LeftNode) + "]";
            while (node.NextNode != null)
            {
                node = node.NextNode;
                str += "[" + InterpretExp(node.LeftNode) + "]";
                dim++;
            }
            return str;
        }

        private void InterpretWhileStmt(TreeNode node)
        {
            isRepeat = true;
            var jumpLine = Line + 1;
            var instruction = new Quadruple(InstructionType.Jump, InterpretExp(node.LeftNode), null, null, Line);
            Codes.AddLast(instruction);
            Line++;
            inPos = Line;
            if (node.MiddleNode != null) {
                if(node.MiddleNode.Type != StmtType.StmtBlock)
                {
                    Codes.AddLast(new Quadruple(InstructionType.In, null, null, null, Line));
                    Line++;
                    var table = new SymbolTable();
                    Level++;
                    InterpretStmt(node.MiddleNode);
                    Level--;
                    SymbolTable.DeRegister();
                    Codes.AddLast(new Quadruple(InstructionType.Out, null, null, null, Line));
                    Line++;
                    
                }
                else
                {
                    InterpretStmt(node.MiddleNode);
                }
               
            }
            Codes.AddLast(new Quadruple(InstructionType.Jump, null, null, jumpLine.ToString(), Line));
            Line++;
            outPos = Line + 1;
            if (code != null )
            {
                code.Third = outPos.ToString();
            }
            isRepeat = false;
            instruction.Third = (Line + 1).ToString();
            outPos = 0;
            inPos = 0;
        }

        private void InterpretIfStmt(TreeNode node)
        {
            var jump = new Quadruple(InstructionType.Jump, InterpretExp(node.LeftNode), null, null, Line);
            Codes.AddLast(jump);
            Line++;
            if (node.MiddleNode != null)
            {
                if (node.MiddleNode.Type == StmtType.ElseStmt)
                {
                    Codes.AddLast(new Quadruple(InstructionType.In, null, null, null, Line));
                    Line++;
                    Codes.AddLast(new Quadruple(InstructionType.Out, null, null, null, Line));
                    Line++;
                    InterpretELseStmt(node, jump);
                }
                else
                {
                    if (node.MiddleNode.LeftNode.Type != StmtType.StmtBlock)
                    {
                        Codes.AddLast(new Quadruple(InstructionType.In, null, null, null, Line));
                        Line++;
                        var table = new SymbolTable();
                        Level++;
                        InterpretStmt(node.MiddleNode.LeftNode);
                        Level--;
                        SymbolTable.DeRegister();
                        Codes.AddLast(new Quadruple(InstructionType.Out, null, null, null, Line));
                        Line++;
                    }
                    else
                    {
                        InterpretStmt(node.MiddleNode.LeftNode);
                    }
                    jump.Third = (Line + 1).ToString();
                    if (node.MiddleNode.MiddleNode != null)
                        InterpretELseStmt(node.MiddleNode.MiddleNode, jump);
                }
            } 
            else
                jump.Third = (Line + 1).ToString();
        }

        private void InterpretELseStmt(TreeNode node, Quadruple jump)
        {
            var elseJump = new Quadruple(InstructionType.Jump, null, null, null, Line);
            Codes.AddLast(elseJump);
            Line++;
            jump.Third = (Line + 1).ToString();
            if (node.LeftNode.Type != StmtType.StmtBlock)
            {
                Codes.AddLast(new Quadruple(InstructionType.In, null, null, null, Line));
                Line++;
                var table = new SymbolTable();
                Level++;
                InterpretStmt(node.LeftNode);
                Level--;
                SymbolTable.DeRegister();
                Codes.AddLast(new Quadruple(InstructionType.Out, null, null, null, Line));
                Line++;
            }
            else
            {
                InterpretStmt(node.LeftNode);
            }
            elseJump.Third = (Line + 1).ToString();
        }
        private void InterpretPrintStmt(TreeNode node)
        {
            var instruction = new Quadruple(InstructionType.Print, null, null, InterpretExp(node.LeftNode), Line);
            Codes.AddLast(instruction);
            Line++;
        }
        private string InterpretExp(TreeNode node)
        {
            isVar = false;
            if (node.Type == StmtType.Exp)
            {
                switch (node.DataType)
                {
                    case TokenType.AdditiveExp:
                        return InterpretAdditiveExp(node);
                    case TokenType.AssignExp:
                        return InterpretAssignExp(node);
                    case TokenType.LogicOrExp:
                        return InterpretLogicOrExp(node);
                    case TokenType.LogicAndExp:
                        return InterpretLogicAndExp(node);
                    case TokenType.CompExp:
                        return InterpretCompExp(node);
                    case TokenType.CompEqExp:
                        return InterpretCompEqExp(node);
                    default:
                        throw new InterpretException("ERROR : 复合表达式非法.\n");
                }
            }
            if (node.Type == StmtType.Term)
            {
                return InterpretTerm(node);
            }
            if (node.Type == StmtType.Factor)
            {
                return InterpretFactor(node);
            }
            if (node.Type == StmtType.Var)
            {
                isVar = true;
                return InterpretVariable(node);
            }

            if (node.Type == StmtType.Value || node.Type == StmtType.Null)
            {
                return node.Value;
            }
            throw new InterpretException("ERROR : 表达式非法.\n");
        }

        private string InterpretVariable(TreeNode node)
        {
            var type = SymbolTable.FindSymbol(node.LeftNode.Value).Type;
            if (node.MiddleNode == null)
            {
               
                if (type == SymbolType.IntValue || type == SymbolType.RealValue || type == SymbolType.CharValue)
                {
                    isVar = true;
                    return node.LeftNode.Value;
                }
            }
            else
            {
                if (type == SymbolType.IntArray || type == SymbolType.CharArray || type == SymbolType.RealArray)
                {
                    var temp = SymbolTable.GetTempSymbol().Name;
                    var index = InterpretArrayDim(node.MiddleNode, out var dim);
                    Codes.AddLast(new Quadruple(InstructionType.Assign, $"{node.LeftNode.Value}{index}", dim.ToString(), temp, Line));
                    Line++;
                    isVar = true;
                    return $"{node.LeftNode.Value}{index}";
                }
            }
            throw new InterpretException("ERROR : 表达式非法.\n");
        }

        private string InterpretFactor(TreeNode node)
        {
            var temp = SymbolTable.GetTempSymbol().Name;
            switch (node.LeftNode.DataType)
            {
                case TokenType.PlusPlus:
                    Codes.AddLast(isVar
                        ? new Quadruple(InstructionType.Plus, InterpretExp(node.MiddleNode), "1",
                            InterpretExp(node.MiddleNode), Line)
                        : new Quadruple(InstructionType.Plus, InterpretExp(node.MiddleNode), "1", temp, Line));
                    break;
                case TokenType.MinusMinus:
                    Codes.AddLast(isVar
                        ? new Quadruple(InstructionType.Minus, InterpretExp(node.MiddleNode), "1",
                            InterpretExp(node.MiddleNode), Line)
                        : new Quadruple(InstructionType.Minus, InterpretExp(node.MiddleNode), "1", temp, Line));
                    break;
                case TokenType.Not:
                    Codes.AddLast(new Quadruple(InstructionType.Not, InterpretExp(node.MiddleNode), null, temp, Line));
                    break;
                case TokenType.Plus:
                    Codes.AddLast(new Quadruple(InstructionType.Plus, "0", InterpretExp(node.MiddleNode), temp, Line));
                    break;
                case TokenType.Minus:
                    Codes.AddLast(new Quadruple(InstructionType.Minus, "0", InterpretExp(node.MiddleNode), temp, Line));
                    break;
                default:
                    if (node.MiddleNode.DataType == TokenType.PlusPlus)
                    {
                        var targetNode = node.LeftNode;
                        Codes.AddLast(new Quadruple(InstructionType.Assign, InterpretExp(targetNode), null, temp, Line));
                        Line++;
                        if(isVar)
                            Codes.AddLast(new Quadruple(InstructionType.Plus, InterpretExp(targetNode), "1", InterpretExp(targetNode), Line));
                        break;
                    } 
                    else if (node.MiddleNode.DataType == TokenType.MinusMinus)
                    {
                        var targetNode = node.LeftNode;
                        Codes.AddLast(new Quadruple(InstructionType.Assign, InterpretExp(targetNode), null, temp, Line));
                        Line++;
                        if(isVar)
                            Codes.AddLast(new Quadruple(InstructionType.Minus, InterpretExp(targetNode), "1", InterpretExp(targetNode), Line));
                        break;
                    }
                    throw new InterpretException("ERROR : 因式表达式非法.\n");
            }
            Line++;
            return temp;
        }

        private string InterpretLogicOrExp(TreeNode node)
        {
            var temp = SymbolTable.GetTempSymbol().Name;
            if (node.MiddleNode != null)
            {
                if (node.MiddleNode.LeftNode.DataType == TokenType.Or)
                    Codes.AddLast(new Quadruple(InstructionType.Or, InterpretExp(node.LeftNode),
                        InterpretExp(node.MiddleNode.MiddleNode), temp, Line));
                else
                    throw new InterpretException("ERROR : 逻辑运算非法.\n");

                Line++;
                node = node.MiddleNode;
            }

            while (node.RightNode != null)
            {
                var tempN = temp;
                temp = SymbolTable.GetTempSymbol().Name;
                if (node.RightNode.LeftNode.DataType == TokenType.Or)
                    Codes.AddLast(new Quadruple(InstructionType.Or, tempN, InterpretExp(node.RightNode.MiddleNode),
                        temp, Line));
                else
                    throw new InterpretException("ERROR : 逻辑运算非法.\n");

                Line++;
                node = node.RightNode;
            }
            return temp;
        }

        private string InterpretCompEqExp(TreeNode node)
        {
            var temp = SymbolTable.GetTempSymbol().Name;
            if (node.MiddleNode != null)
            {
                switch (node.MiddleNode.LeftNode.DataType)
                {
                    case TokenType.Eq:
                        Codes.AddLast(new Quadruple(InstructionType.Eq, InterpretExp(node.LeftNode), InterpretExp(node.MiddleNode.MiddleNode), temp, Line));
                        break;
                    case TokenType.Neq:
                        Codes.AddLast(new Quadruple(InstructionType.NotEq, InterpretExp(node.LeftNode), InterpretExp(node.MiddleNode.MiddleNode), temp, Line));
                        break;
                    default:
                        throw new InterpretException("ERROR : 关系运算非法.\n");
                }
                Line++;
                node = node.MiddleNode;
            }

            while (node.RightNode != null)
            {
                var tempN = temp;
                temp = SymbolTable.GetTempSymbol().Name;
                switch (node.RightNode.LeftNode.DataType)
                {
                    case TokenType.Eq:
                        Codes.AddLast(new Quadruple(InstructionType.Eq, tempN, InterpretExp(node.RightNode.MiddleNode), temp, Line));
                        break;
                    case TokenType.Neq:
                        Codes.AddLast(new Quadruple(InstructionType.NotEq, tempN, InterpretExp(node.RightNode.MiddleNode), temp, Line));
                        break;
                    default:
                        throw new InterpretException("ERROR : 关系运算非法.\n");
                }
                Line++;
                node = node.RightNode;
            }
            return temp;
        }

        private string InterpretCompExp(TreeNode node)
        {
            var temp = SymbolTable.GetTempSymbol().Name;
            if (node.MiddleNode != null)
            {
                switch (node.MiddleNode.LeftNode.DataType)
                {
                    case TokenType.Greater:
                        Codes.AddLast(new Quadruple(InstructionType.GreaterThan, InterpretExp(node.LeftNode), InterpretExp(node.MiddleNode.MiddleNode), temp, Line));
                        break;
                    case TokenType.GreaterEq:
                        Codes.AddLast(new Quadruple(InstructionType.GreaterEqThan, InterpretExp(node.LeftNode), InterpretExp(node.MiddleNode.MiddleNode), temp, Line));
                        break;
                    case TokenType.Less:
                        Codes.AddLast(new Quadruple(InstructionType.LessThan, InterpretExp(node.LeftNode), InterpretExp(node.MiddleNode.MiddleNode), temp, Line));
                        break;
                    case TokenType.LessEq:
                        Codes.AddLast(new Quadruple(InstructionType.LessEqThan, InterpretExp(node.LeftNode), InterpretExp(node.MiddleNode.MiddleNode), temp, Line));
                        break;
                    default:
                        throw new InterpretException("ERROR : 关系运算非法.\n");
                }
                Line++;
                node = node.MiddleNode;
            }

            while (node.RightNode != null)
            {
                var tempN = temp;
                temp = SymbolTable.GetTempSymbol().Name;
                switch (node.RightNode.LeftNode.DataType)
                {
                    case TokenType.Greater:
                        Codes.AddLast(new Quadruple(InstructionType.GreaterThan, tempN, InterpretExp(node.RightNode.MiddleNode), temp, Line));
                        break;
                    case TokenType.GreaterEq:
                        Codes.AddLast(new Quadruple(InstructionType.GreaterEqThan, tempN, InterpretExp(node.RightNode.MiddleNode), temp, Line));
                        break;
                    case TokenType.Less:
                        Codes.AddLast(new Quadruple(InstructionType.LessThan, tempN, InterpretExp(node.RightNode.MiddleNode), temp, Line));
                        break;
                    case TokenType.LessEq:
                        Codes.AddLast(new Quadruple(InstructionType.LessEqThan, tempN, InterpretExp(node.RightNode.MiddleNode), temp, Line));
                        break;
                    default:
                        throw new InterpretException("ERROR : 关系运算非法.\n");
                }
                Line++;
                node = node.RightNode;
            }
            return temp;
        }

        private string InterpretLogicAndExp(TreeNode node)
        {
            var temp = SymbolTable.GetTempSymbol().Name;
            if (node.MiddleNode != null)
            {
                if (node.MiddleNode.LeftNode.DataType == TokenType.And)
                    Codes.AddLast(new Quadruple(InstructionType.And, InterpretExp(node.LeftNode),
                        InterpretExp(node.MiddleNode.MiddleNode), temp, Line));
                else
                    throw new InterpretException("ERROR : 逻辑运算非法.\n");

                Line++;
                node = node.MiddleNode;
            }

            while (node.RightNode != null)
            {
                var tempN = temp;
                temp = SymbolTable.GetTempSymbol().Name;
                if (node.RightNode.LeftNode.DataType == TokenType.And)
                    Codes.AddLast(new Quadruple(InstructionType.And, tempN, InterpretExp(node.RightNode.MiddleNode),
                        temp, Line));
                else
                    throw new InterpretException("ERROR : 逻辑运算非法.\n");

                Line++;
                node = node.RightNode;
            }
            return temp;
        }

        private string InterpretAssignExp(TreeNode node)
        {
            var str = InterpretExp(node.LeftNode);
            if (!isVar)
                throw new InterpretException("ERROR : 赋值表达式左值应该为变量.\n");
            var temp = SymbolTable.GetTempSymbol().Name;
            if (node.MiddleNode != null)
            {
                switch (node.MiddleNode.LeftNode.DataType)
                {
                    case TokenType.Assign:
                        Codes.AddLast(new Quadruple(InstructionType.Assign, InterpretExp(node.MiddleNode.MiddleNode), null, str, Line)) ;
                        break;
                    case TokenType.PlusAssign:
                        Codes.AddLast(new Quadruple(InstructionType.Plus, InterpretExp(node.MiddleNode.MiddleNode), str, temp, Line));
                        Line++;
                        Codes.AddLast(new Quadruple(InstructionType.Assign, temp, null, str, Line));
                        break;
                    case TokenType.MinusAssign:
                        Codes.AddLast(new Quadruple(InstructionType.Minus, InterpretExp(node.MiddleNode.MiddleNode), str, temp, Line));
                        Line++;
                        Codes.AddLast(new Quadruple(InstructionType.Assign, temp, null, str, Line));
                        break;
                    case TokenType.MulAssign:
                        Codes.AddLast(new Quadruple(InstructionType.Mul, InterpretExp(node.MiddleNode.MiddleNode), str, temp, Line));
                        Line++;
                        Codes.AddLast(new Quadruple(InstructionType.Assign, temp, null, str, Line));
                        break;
                    case TokenType.DivAssign:
                        Codes.AddLast(new Quadruple(InstructionType.Div, InterpretExp(node.MiddleNode.MiddleNode), str, temp, Line));
                        Line++;
                        Codes.AddLast(new Quadruple(InstructionType.Assign, temp, null, str, Line));
                        break;
                    case TokenType.ModAssign:
                        Codes.AddLast(new Quadruple(InstructionType.Mod, InterpretExp(node.MiddleNode.MiddleNode), str, temp, Line));
                        Line++;
                        Codes.AddLast(new Quadruple(InstructionType.Assign, temp, null, str, Line));
                        break;
                    default:
                        throw new InterpretException("ERROR : 赋值表达式非法.\n");
                }
                Line++;
            }
            return str;
        }

        private string InterpretAdditiveExp(TreeNode node)
        {
            var temp = SymbolTable.GetTempSymbol().Name;
            if (node.MiddleNode != null)
            {
                switch (node.MiddleNode.LeftNode.DataType)
                {
                case TokenType.Plus:
                    Codes.AddLast(new Quadruple(InstructionType.Plus, InterpretExp(node.LeftNode), InterpretExp(node.MiddleNode.MiddleNode), temp, Line));
                    break;
                case TokenType.Minus:
                    Codes.AddLast(new Quadruple(InstructionType.Minus, InterpretExp(node.LeftNode), InterpretExp(node.MiddleNode.MiddleNode), temp, Line));
                    break;
                default:
                    throw new InterpretException("ERROR : 算术运算非法.\n");
                }
                Line++;
                node = node.MiddleNode;
            }
           
            while (node.RightNode != null)
            {
                var tempN = temp;
                temp = SymbolTable.GetTempSymbol().Name;
                switch (node.RightNode.LeftNode.DataType)
                {
                    case TokenType.Plus:
                        Codes.AddLast(new Quadruple(InstructionType.Plus, tempN, InterpretExp(node.RightNode.MiddleNode), temp, Line));
                        break;
                    case TokenType.Minus:
                        Codes.AddLast(new Quadruple(InstructionType.Minus, tempN, InterpretExp(node.RightNode.MiddleNode), temp, Line));
                        break;
                    default:
                        throw new InterpretException("ERROR : 算术运算非法.\n");
                }
                Line++;
                node = node.RightNode;
            }
            return temp;
        }

        private string InterpretTerm(TreeNode node)
        {
            var temp = SymbolTable.GetTempSymbol().Name;
            if (node.MiddleNode != null)
            {
                switch (node.MiddleNode.LeftNode.DataType)
                {
                    case TokenType.Mul:
                        Codes.AddLast(new Quadruple(InstructionType.Mul, InterpretExp(node.LeftNode), InterpretExp(node.MiddleNode.MiddleNode), temp, Line));
                        break;
                    case TokenType.Div:
                        Codes.AddLast(new Quadruple(InstructionType.Div, InterpretExp(node.LeftNode), InterpretExp(node.MiddleNode.MiddleNode), temp, Line));
                        break;
                    case TokenType.Mod:
                        Codes.AddLast(new Quadruple(InstructionType.Mod, InterpretExp(node.LeftNode), InterpretExp(node.MiddleNode.MiddleNode), temp, Line));
                        break;
                    default:
                        throw new InterpretException("ERROR : 算术运算非法.\n");
                }
                Line++;
                node = node.MiddleNode;
            }

            while (node.RightNode != null)
            {
                var tempN = temp;
                temp = SymbolTable.GetTempSymbol().Name;
                switch (node.RightNode.LeftNode.DataType)
                {
                    case TokenType.Mul:
                        Codes.AddLast(new Quadruple(InstructionType.Mul, tempN, InterpretExp(node.RightNode.MiddleNode), temp, Line));
                        break;
                    case TokenType.Div:
                        Codes.AddLast(new Quadruple(InstructionType.Div, tempN, InterpretExp(node.RightNode.MiddleNode), temp, Line));
                        break;
                    case TokenType.Mod:
                        Codes.AddLast(new Quadruple(InstructionType.Mod, tempN, InterpretExp(node.RightNode.MiddleNode), temp, Line));
                        break;
                    default:
                        throw new InterpretException("ERROR : 算术运算非法.\n");
                }
                Line++;
                node = node.RightNode;
            }
            return temp;
        }
    }
}
