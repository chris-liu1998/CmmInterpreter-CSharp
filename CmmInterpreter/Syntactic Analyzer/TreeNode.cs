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
                case StmtType.IF_ST:
                    return "IF_STATEMENT";
                case StmtType.ELSE_ST:
                    return "ELSE_STATEMENT";
                case StmtType.WHILE_ST:
                    return "WHILE_STATEMENT";
                case StmtType.PRINT_ST:
                    return "PRINT_STATEMENT";
                case StmtType.SCAN_ST:
                    return "SCAN_STATEMENT";
                case StmtType.INIT:
                    return "INITIAL_STATEMENT";
                case StmtType.DEC_ST:
                    return "DEC_STATEMENT";
                case StmtType.STBLOCK:
                    return "STATEMENT_BLOCK";
                case StmtType.ASSIGN_ST:
                    return "ASSIGN_STATEMENT";
                case StmtType.JUMP_ST:
                    return "JUMP_STATEMENT";
                case StmtType.VALUE_LIST:
                    return "VALUE_LIST";
                case StmtType.VAR_LIST:
                    return "VARIABLE_LIST";
                case StmtType.ID:
                    return "IDENTIFIER";
                case StmtType.MORE_VALUE:
                    return "MORE_VALUE";
                case StmtType.EXP:
                    return "EXP";
                case StmtType.BREAK:
                    return "BREAK";
                case StmtType.CONTINUE:
                    return "CONTINUE";
                case StmtType.OPR:
                    return "OPERATOR";
                case StmtType.VAR:
                    return "VAR";
                case StmtType.FACTOR:
                    return "FACTOR";
                case StmtType.MORE_FACTOR:
                    return "MORE_FACTOR";
                case StmtType.MORE_TERM:
                    return "MORE_TERM";
                case StmtType.TERM:
                    return "TERM";
                case StmtType.MORE_ADD_EXP:
                    return "MORE_ADDITIVE_EXP";
                case StmtType.MORE_LOGIC_EXP:
                    return "MORE_EXP";
                case StmtType.VALUE:
                    return "VALUE";
                case StmtType.PROGRAM:
                    return "PROGRAM";
                case StmtType.STMT_SEQ:
                    return "STATEMENT_SEQ";
                case StmtType.NONE:
                    return "NONE";
                case StmtType.NULL:
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