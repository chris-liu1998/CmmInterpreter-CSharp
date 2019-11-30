using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmmInterpreter.Lexical_Analyzer;

namespace CmmInterpreter.Syntactic_Analyzer
{
    public class TreeNode
    {
        public int Type { get; set; }
        public string Value { get; set; }
        public int DataType { get; set; }
        public TreeNode LeftNode { get; set; }
        public TreeNode RightNode { get; set; }
        public TreeNode MiddleNode { get; set; }
        public TreeNode NextStmtNode { get; set; }


        public TreeNode (int type)
        {
            Type = type;
        }
        public string TypeToString ()
        {
            switch (Type)
            {
                case StmtType.IfStmt:
                    return "IF_STATEMENT";
                case StmtType.ElseStmt:
                    return "ELSE_STATEMENT";
                case StmtType.WhileStmt:
                    return "WHILE_STATEMENT";
                case StmtType.PrintStmt:
                    return "PRINT_STATEMENT";
                case StmtType.ScanStmt:
                    return "SCAN_STATEMENT";
                case StmtType.Init:
                    return "INITIAL_STATEMENT";
                case StmtType.DecStmt:
                    return "DEC_STATEMENT";
                case StmtType.StmtBlock:
                    return "STATEMENT_BLOCK";
                case StmtType.AssignStmt:
                    return "ASSIGN_STATEMENT";
                case StmtType.JumpSt:
                    return "JUMP_STATEMENT";
                case StmtType.ValueList:
                    return "VALUE_LIST";
                case StmtType.VarList:
                    return "VARIABLE_LIST";
                case StmtType.Id:
                    return "IDENTIFIER";
                case StmtType.MoreValue:
                    return "MORE_VALUE";
                case StmtType.Exp:
                    return "EXP";
                case StmtType.Break:
                    return "BREAK";
                case StmtType.Continue:
                    return "CONTINUE";
                case StmtType.Operator:
                    return "OPERATOR";
                case StmtType.Var:
                    return "VAR";
                case StmtType.Factor:
                    return "FACTOR";
                case StmtType.MoreFactor:
                    return "MORE_FACTOR";
                case StmtType.MoreTerm:
                    return "MORE_TERM";
                case StmtType.Term:
                    return "TERM";
                case StmtType.MoreAddExp:
                    return "MORE_ADDITIVE_EXP";
                case StmtType.MoreLogicExp:
                    return "MORE_EXP";
                case StmtType.Value:
                    return "VALUE";
                case StmtType.Program:
                    return "PROGRAM";
                case StmtType.StmtSeq:
                    return "STATEMENT_SEQ";
                case StmtType.None:
                    return "NONE";
                case StmtType.Null:
                    return "NULL";
                default:
                    return "UNKNOWN";
            }
        }

        /// <summary>
        /// 将节点以及其子节点以字符串的形式输出
        /// </summary>
        /// <param name="node"></param>
        /// <param name="space"></param>
        /// <returns>
        /// 树节点及其子结点
        /// </returns>
        public static string PrintNode (TreeNode node, string space)
        {
            var strb = new StringBuilder ();
            strb.Append (space).Append ("<").Append (node.TypeToString ()).Append (" : ")
                .Append (new Token (node.DataType).TypeToString ()).Append (">\n");
            if (node.Value != null)
            {
                strb.Append ("  ").Append (space).Append (node.Value).Append ("\n");
            }

            if (node.LeftNode != null)
                strb.Append (PrintNode (node.LeftNode, " " + space));
            if (node.MiddleNode != null)
                strb.Append (PrintNode (node.MiddleNode, " " + space));
            if (node.RightNode != null)
                strb.Append (PrintNode (node.RightNode, " " + space));
            if (node.NextStmtNode != null)
                strb.Append (PrintNode (node.NextStmtNode, " " + space));
            //strb.Append(space + "</" + node.typeToString() + ">\n");
            return strb.ToString ();
        }

        public override string ToString ()
        {
            return PrintNode (this, "");
        }
    }
}