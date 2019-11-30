using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmmInterpreter.Exceptions;
using CmmInterpreter.Lexical_Analyzer;
using CmmInterpreter.Util;

namespace CmmInterpreter.Syntactic_Analyzer
{   
    //暂时忽略CurrentToken可能抛出空异常的警告，因为目前未出现报错情况
    public class Parser
    {
        public Token CurrentToken { get; set; }
        private PeekableEnumerator<Token> _enumerator;
        private int BlockLevel { get; set; }
        private StringBuilder ErrorInfo { get; set; }
        public LinkedList<Token> Tokens { get; set; }
        public TreeNode SyntaxTree { get; set; }
        public bool IsParseError { get; set; }
        public string Error { get; set; }
        //public  int dims;
       /// <summary>
       /// 此方法用来进行语法分析，调用之前须确保Parser对象里的Tokens链表不为空
       /// </summary>
        public void SyntaxAnalyze()
        {  //语法分析主程序
            if (Tokens.Count == 0) return;
            CurrentToken = Tokens.First();
            try
            {
                SyntaxTree = new TreeNode(StmtType.PROGRAM);
                ErrorInfo = new StringBuilder();
                _enumerator = Tokens.GetEnumerator().ToPeekable();
                BlockLevel = 0;
                SyntaxTree.LeftNode = ParseStmtSeq();

                if (ErrorInfo.Length != 0)
                    throw new ParseException(ErrorInfo.ToString());
            }
            catch (ParseException e)
            {
                IsParseError = true;
                Error = e.Message;
            }
            
        }

        private TreeNode ParseStmtSeq()
        {
            if (_enumerator.Peek == null) return null;
            if ((BlockLevel != 0) && (BlockLevel <= 0 || CheckNextTokenType(TokenType.RIGHT_BRA))) return null;
            var node = new TreeNode(StmtType.STMT_SEQ) {LeftNode = ParseStmt(), MiddleNode = ParseStmtSeq()};
            return node;
        }

        private TreeNode ParseStmt()
        { //处理语句或语句块
            switch (GetNextTokenType())
            {
                case TokenType.IF:
                    return ParseIfStmt();  //转向处理if语句
                case TokenType.END:
                    ConsumeNextToken(TokenType.END);
                    return null;  //转向处理空语句
                case TokenType.WHILE:
                    return ParseWhileStmt();  //转向处理while语句
                case TokenType.INT:
                case TokenType.REAL:
                case TokenType.CHAR:
                    return ParseDeclareStmt();  //转向处理声明语句
                case TokenType.PRINT:
                    return ParsePrintStmt();  //转向处理打印语句
                case TokenType.SCAN:
                    return ParseScanStmt();   //转向处理扫描语句
                case TokenType.LEFT_BRA:
                    return ParseStmtBlock();  //转向处理语句块
                case TokenType.BREAK:
                case TokenType.CONTINUE:
                    return ParseJumpStmt();  //转向处理跳转语句
                default:
                    return ParseAssignStmt();  //转向处理赋值语句
            }
        }

        private int GetNextTokenType()
        {
            if(_enumerator.Peek != null)
                return _enumerator.Peek.Type;
            return -1;
        }

        private TreeNode ParseStmtBlock()
        {
            var node = new TreeNode(StmtType.STBLOCK);
            ConsumeNextToken(TokenType.LEFT_BRA);
            BlockLevel++;
            node.LeftNode = ParseStmtSeq();
            ConsumeNextToken(TokenType.RIGHT_BRA);
            BlockLevel--;
            return node;
        }

        private TreeNode ParseIfStmt()
        {
            var node = new TreeNode(StmtType.IF_ST);
            ConsumeNextToken(TokenType.IF);
            ConsumeNextToken(TokenType.LEFT_P);
            node.LeftNode = ParseAssignExp();
            ConsumeNextToken(TokenType.RIGHT_P);
            node.MiddleNode = ParseIfBlock();
            return node;
        }

        private TreeNode ParseIfBlock()
        {
            if (CheckNextTokenType(TokenType.END))
            {
                ConsumeNextToken(TokenType.END);
                return null;
            }
            if (CheckNextTokenType(TokenType.ELSE))
            {
                return ParseElseStmt();
            }
            var node = new TreeNode(StmtType.IF_BLOCK) { LeftNode = ParseStmt(), MiddleNode = ParseElseStmt() };
            return node;
        }

        private TreeNode ParseElseStmt()
        {
            if (!CheckNextTokenType(TokenType.ELSE)) return null;
            var node = new TreeNode(StmtType.ELSE_ST);
            ConsumeNextToken(TokenType.ELSE);
            node.LeftNode = ParseStmt();
            return node;
        }

        private TreeNode ParseWhileStmt()
        {
            var node = new TreeNode(StmtType.WHILE_ST);
            ConsumeNextToken(TokenType.WHILE);
            ConsumeNextToken(TokenType.LEFT_P);
            node.LeftNode = ParseAssignExp();
            ConsumeNextToken(TokenType.RIGHT_P);
            node.MiddleNode = ParseStmt();
            return node;
        }

        private TreeNode ParseAssignStmt()
        {
            var node = ParseAssignExp();
            ConsumeNextToken(TokenType.END);
            return node;
        }

        private TreeNode ParseAssignExp()
        {
            var node = new TreeNode(StmtType.EXP) { LeftNode = ParseLogicOrExp(), MiddleNode = ParseMoreLogicOrExp() };
            return node;
        }

        private TreeNode ParseMoreLogicOrExp()
        {
            if (!CheckNextTokenType(TokenType.ASSIGN)) return null;
            var node = new TreeNode(StmtType.MORE_LOGIC_EXP)
            {
                LeftNode = GetAssignOp(),
                MiddleNode = ParseLogicOrExp(),
                RightNode = ParseMoreLogicOrExp()
            };
            return node;
        }

        private TreeNode ParseJumpStmt()
        {   //处理跳转语句
            _enumerator.MoveNext();
            CurrentToken = _enumerator.Current;
            var type = CurrentToken.Type;
            var node = new TreeNode(StmtType.JUMP_ST) { DataType = type, Value = CurrentToken.Value };
            ConsumeNextToken(TokenType.END);
            return node;
        }

        private TreeNode ParseScanStmt()
        { //处理scan语句
            var node = new TreeNode(StmtType.SCAN_ST);
            ConsumeNextToken(TokenType.SCAN);
            ConsumeNextToken(TokenType.LEFT_P);
            node.LeftNode = ParseVariable();
            ConsumeNextToken(TokenType.RIGHT_P);
            ConsumeNextToken(TokenType.END);
            return node;
        }

        private TreeNode ParsePrintStmt()
        { //处理print语句
            var node = new TreeNode(StmtType.PRINT_ST);
            ConsumeNextToken(TokenType.PRINT);
            ConsumeNextToken(TokenType.LEFT_P);
            node.LeftNode = ParseAssignExp();
            ConsumeNextToken(TokenType.RIGHT_P);
            ConsumeNextToken(TokenType.END);
            return node;
        }

        private TreeNode ParseDeclareStmt()
        {
            var node = new TreeNode(StmtType.DEC_ST);
            if (CheckNextTokenType(TokenType.INT, TokenType.REAL, TokenType.CHAR))
            {
                var type = GetNextTokenType();
                node.DataType = type;
                ConsumeNextToken(type);
            }
            node.LeftNode = ParseVariableList();
            ConsumeNextToken(TokenType.END);
            return node;
        }

        private TreeNode ParseVariableList()
        {
            var node = new TreeNode(StmtType.VAR_LIST)
            {
                LeftNode = ParseVariable(),
                MiddleNode = ParseInitializer(),
                RightNode = ParseMoreVars()
            };
            return node;
        }

        private TreeNode ParseVariable()
        {
            var node = new TreeNode(StmtType.VAR) { LeftNode = GetIdentifier(), MiddleNode = ParseArrayDim() };
            return node;
        }

        private TreeNode ParseArrayDim()
        {
            if (!CheckNextTokenType(TokenType.LEFT_BRK)) return null;
            var node = new TreeNode(StmtType.ARRAY_DIM);
            ConsumeNextToken(TokenType.LEFT_BRK);
            node.LeftNode = ParseLogicOrExp();
            ConsumeNextToken(TokenType.RIGHT_BRK);
            node.MiddleNode = ParseArrayDim();
            return node;
        }

        private TreeNode ParseLogicOrExp()
        {
            var node = new TreeNode(StmtType.EXP) { LeftNode = ParseLogicAndExp(), MiddleNode = ParseMoreLogicAndExp() };
            return node;
        }

        private TreeNode ParseLogicAndExp()
        {
            var node = new TreeNode(StmtType.EXP) { LeftNode = ParseCompEqExp(), MiddleNode = ParseMoreCompEqExp() };
            return node;
        }

        private TreeNode ParseMoreCompEqExp()
        {
            if (!CheckNextTokenType(TokenType.AND)) return null;
            var node = new TreeNode(StmtType.MORE_COMP_EXP)
            {
                LeftNode = GetLogicOp(),
                MiddleNode = ParseCompEqExp(),
                RightNode = ParseMoreCompEqExp()
            };
            return node;
        }

        private TreeNode ParseCompEqExp()
        {
            var node = new TreeNode(StmtType.EXP) { LeftNode = ParseCompExp(), MiddleNode = ParseMoreCompExp() };
            return node;
        }

        private TreeNode ParseMoreCompExp()
        {
            if (!CheckNextTokenType(TokenType.EQ, TokenType.NEQ)) return null;
            var node = new TreeNode(StmtType.MORE_COMP_EXP)
            {
                LeftNode = GetCompOp(),
                MiddleNode = ParseCompExp(),
                RightNode = ParseMoreCompExp()
            };
            return node;
        }

        private TreeNode ParseCompExp()
        {
            var node = new TreeNode(StmtType.EXP) { LeftNode = ParseAdditiveExp(), RightNode = ParseMoreAdditiveExp() };
            return node;
        }

        private TreeNode ParseMoreLogicAndExp()
        {
            if (!CheckNextTokenType(TokenType.OR)) return null;
            var node = new TreeNode(StmtType.MORE_LOGIC_EXP)
            {
                LeftNode = GetLogicOp(),
                MiddleNode = ParseLogicAndExp(),
                RightNode = ParseMoreLogicAndExp()
            };
            return node;
        }

        private TreeNode ParseMoreVars()
        {
            if (!CheckNextTokenType(TokenType.COMMA)) return null;
            var node = new TreeNode(StmtType.MORE_VAR);
            ConsumeNextToken(TokenType.COMMA);
            node.LeftNode = ParseVariable();
            node.MiddleNode = ParseInitializer();
            node.RightNode = ParseMoreVars();
            return node;
        }

        private TreeNode ParseInitializer()
        {
            if (!CheckNextTokenType(TokenType.ASSIGN)) return null;
            var node = new TreeNode(StmtType.INIT) { LeftNode = GetAssignOp(), MiddleNode = ParseValue() };
            return node;
        }

        private TreeNode ParseValue()
        {
            if (!CheckNextTokenType(TokenType.LEFT_BRA)) return ParseLogicOrExp();
            ConsumeNextToken(TokenType.LEFT_BRA);
            var node = ParseValueList();
            ConsumeNextToken(TokenType.RIGHT_BRA);
            return node;
        }

        private TreeNode ParseValueList()
        {    //赋值列表
            var node = new TreeNode(StmtType.VALUE_LIST) { LeftNode = ParseValue(), MiddleNode = ParseMoreValue() };
            return node;
        }

        private TreeNode ParseMoreValue()
        {
            if (!CheckNextTokenType(TokenType.COMMA)) return null;
            var node = new TreeNode(StmtType.MORE_VALUE);
            ConsumeNextToken(TokenType.COMMA);
            node.LeftNode = ParseValue();
            node.MiddleNode = ParseMoreValue();
            return node;
        }

        private TreeNode GetIdentifier()
        {
            if (CheckNextTokenType(TokenType.ID))
            {
                _enumerator.MoveNext();
                CurrentToken = _enumerator.Current;
                var node = new TreeNode(StmtType.ID) { Value = CurrentToken.Value };
                return node;
            }
            ErrorInfo.Append("ERROR : " + "line: ").Append(CurrentToken.LineNo).Append(" 此处缺少标识符.\n");
            return null;
        }

        private TreeNode GetLogicOp()
        {  //处理逻辑操作符

            if (CheckNextTokenType(TokenType.AND, TokenType.OR, TokenType.NOT))
            {
                _enumerator.MoveNext();
                CurrentToken = _enumerator.Current;
                var node = new TreeNode(StmtType.OPR) { DataType = CurrentToken.Type, Value = CurrentToken.Value };
                return node;
            }
            ErrorInfo.Append("ERROR : " + "line: ").Append(CurrentToken.LineNo).Append(" 此处缺少逻辑运算符.\n");
            return null;
        }

        private TreeNode ParseMoreAdditiveExp()
        {
            if (!CheckNextTokenType(TokenType.GREATER,
                TokenType.GREATER_EQ, TokenType.LESS,
                TokenType.LESS_EQ)) return null;
            var node = new TreeNode(StmtType.MORE_ADD_EXP)
            {
                LeftNode = GetCompOp(),
                MiddleNode = ParseAdditiveExp(),
                RightNode = ParseMoreAdditiveExp()
            };
            return node;
        }

        private TreeNode GetCompOp()
        {  //逻辑运算符
            if (CheckNextTokenType(TokenType.LESS_EQ, TokenType.LESS,
                    TokenType.EQ, TokenType.NEQ,
                    TokenType.GREATER_EQ, TokenType.GREATER))
            {
                _enumerator.MoveNext();
                CurrentToken = _enumerator.Current;
                var node = new TreeNode(StmtType.OPR) { DataType = CurrentToken.Type, Value = CurrentToken.Value };
                return node;
            }
            ErrorInfo.Append("ERROR : " + "line: ").Append(CurrentToken.LineNo).Append(" 此处缺少关系运算符.\n");
            return null;
        }

        private TreeNode ParseAdditiveExp()
        {    //多项式
            var node = new TreeNode(StmtType.EXP) { LeftNode = ParseTerm(), MiddleNode = ParseMoreTerm() };
            return node;
        }

        private TreeNode ParseMoreTerm()
        {
            if (CheckNextTokenType(TokenType.PLUS, TokenType.MINUS))
            {
                var node = new TreeNode(StmtType.MORE_TERM)
                {
                    LeftNode = GetAlgOp(),
                    MiddleNode = ParseAdditiveExp(),
                    RightNode = ParseMoreTerm()
                };
                return node;
            }
            return null;
        }

        private TreeNode GetAlgOp()
        {  //处理多项式算术运算符
            if (CheckNextTokenType(TokenType.PLUS, TokenType.MINUS, TokenType.MUL, TokenType.DIV))
            {
                _enumerator.MoveNext();
                CurrentToken = _enumerator.Current;
                var node = new TreeNode(StmtType.OPR) { DataType = CurrentToken.Type, Value = CurrentToken.Value };
                return node;
            }
            ErrorInfo.Append("ERROR : " + "line: ").Append(CurrentToken.LineNo).Append(" 此处缺少算术运算符.\n");
            return null;
        }

        private TreeNode GetAssignOp()
        {  //处理多项式算术运算符
            if (CheckNextTokenType(TokenType.ASSIGN, TokenType.DIV_ASSIGN,
                    TokenType.MUL_ASSIGN, TokenType.PLUS_ASSIGN,
                    TokenType.MINUS_ASSIGN, TokenType.MOD_ASSIGN))
            {
                _enumerator.MoveNext();
                CurrentToken = _enumerator.Current;
                var node = new TreeNode(StmtType.OPR) { DataType = CurrentToken.Type, Value = CurrentToken.Value };
                return node;
            }
            ErrorInfo.Append("ERROR : " + "line: ").Append(CurrentToken.LineNo).Append(" 此处缺少赋值运算符.\n");
            return null;
        }

        private TreeNode ParseTerm()
        {  //处理项
            var node = new TreeNode(StmtType.TERM) { LeftNode = ParseFactor(), MiddleNode = ParseMoreFactor() };
            return node;
        }

        private TreeNode ParseFactor()
        {
            var node = new TreeNode(StmtType.FACTOR);
            switch (GetNextTokenType())
            {
                case TokenType.NOT:
                    node.LeftNode = GetLogicOp();
                    node.MiddleNode = ParseFactor();
                    return node;
                case TokenType.PLUS_PLUS:
                    node.LeftNode = GetIncDecOp();
                    node.MiddleNode = ParseFactor();
                    return node;
                case TokenType.PLUS:
                case TokenType.MINUS:
                    node.LeftNode = GetAlgOp();
                    node.MiddleNode = ParseFactor();
                    return node;
                default:
                    node.LeftNode = ParseSpecific();
                    node.MiddleNode = ParsePossibleIncDecOp();
                    return node;
            }
        }

        private TreeNode ParsePossibleIncDecOp()
        {
            return CheckNextTokenType(TokenType.PLUS_PLUS, TokenType.MINUS_MINUS) ? GetIncDecOp() : null;
        }

        private TreeNode ParseSpecific()
        {
            TreeNode node;
            switch (GetNextTokenType())
            {
                case TokenType.LEFT_P:
                    ConsumeNextToken(TokenType.LEFT_P);
                    node = ParseAssignExp();
                    ConsumeNextToken(TokenType.RIGHT_P);
                    return node;
                case TokenType.INT_VAL:
                case TokenType.REAL_VAL:
                    node = GetNumValue();
                    return node;
                case TokenType.NULL:
                    _enumerator.MoveNext();
                    CurrentToken = _enumerator.Current;
                    node = new TreeNode(StmtType.NULL)
                    {
                        DataType = TokenType.NULL,
                        Value = CurrentToken?.Value
                    };
                    return node;
                case TokenType.PLUS:
                    ConsumeNextToken(TokenType.PLUS);
                    node = ParseTerm();
                    return node;
                case TokenType.MINUS:
                    ConsumeNextToken(TokenType.MINUS);
                    node = ParseTerm();
                    node.DataType = TokenType.MINUS;
                    return node;
                case TokenType.SCAN:
                    ConsumeNextToken(TokenType.SCAN);
                    ConsumeNextToken(TokenType.LEFT_P);
                    node = ParseVariable();
                    ConsumeNextToken(TokenType.RIGHT_P);
                    return node;
                case TokenType.ID:
                    node = ParseVariable();
                    return node;
                default:
                    ErrorInfo.Append("ERROR : " + "line: ").Append(CurrentToken.LineNo).Append(" 此处缺少标识符或数值或表达式.\n");
                    return null;
            }
        }

        private TreeNode GetIncDecOp()
        {     
            if (CheckNextTokenType(TokenType.PLUS_PLUS, TokenType.MINUS_MINUS))
            {
                _enumerator.MoveNext();
                CurrentToken = _enumerator.Current;
                var node = new TreeNode(StmtType.OPR) { DataType = CurrentToken.Type, Value = CurrentToken.Value };
                return node;
            }
            ErrorInfo.Append("ERROR : " + "line: ").Append(CurrentToken.LineNo).Append(" 此处缺少自增/自减运算符.\n");
            return null;
        }

        private TreeNode ParseMoreFactor()
        {
            if (!CheckNextTokenType(TokenType.MUL, TokenType.DIV)) return null;
            var node = new TreeNode(StmtType.MORE_FACTOR) { LeftNode = GetAlgOp(), MiddleNode = ParseTerm() };
            return node;
        }

        private TreeNode GetNumValue()
        {    //获取具体的值
            if (CheckNextTokenType(TokenType.INT_VAL, TokenType.REAL_VAL))
            {
                _enumerator.MoveNext();
                CurrentToken = _enumerator.Current;
                var node = new TreeNode(StmtType.VALUE) { DataType = CurrentToken.Type, Value = CurrentToken.Value };
                return node;
            }
            ErrorInfo.Append("ERROR : " + "line: ").Append(CurrentToken.LineNo).Append(" 此处缺少值类型.\n");
            return null;
        }

        private bool CheckNextTokenType(params int[] types)
        {
            //检查当前token的类型                
            if (_enumerator.Peek == null) return false;
            var type = _enumerator.Peek.Type;
            return types.Any(t => t == type);

        }

        private void ConsumeNextToken(int type)
        {   //消耗掉无用的token
            if (_enumerator.Peek != null)
            {
                if (CheckNextTokenType(type))
                {
                    _enumerator.MoveNext();
                    CurrentToken = _enumerator.Current;
                }else
                    ErrorInfo.Append("ERROR : " + "line: ").Append(CurrentToken.LineNo).Append(" 此处缺少").Append(new Token(type).TypeToString()).Append("（类型）.\n");
                return;
            }
            ErrorInfo.Append("ERROR : " + "line: ").Append(CurrentToken.LineNo).Append(" 此处缺少").Append(new Token(type).TypeToString()).Append("（类型）.\n");
            throw new ParseException(ErrorInfo.ToString());
        }
    }
}
