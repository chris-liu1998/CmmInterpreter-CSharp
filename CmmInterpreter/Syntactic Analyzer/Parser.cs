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
    /// <summary>
    /// 语法分析器
    /// </summary>
    public class Parser
    {
        public Token CurrentToken { get; set; }
        private PeekableEnumerator<Token> _enumerator;
        public int BlockLevel { get; set; }
        private StringBuilder _errorInfo;
        public LinkedList<Token> Tokens { get; set; }
        public TreeNode SyntaxTree { get; set; }
        public bool IsParseError { get; set; }
        public string Error { get; set; }

        private bool _isAssign;
        //public  int dims;
       /// <summary>
       /// 此方法用来进行语法分析，调用之前须确保Parser对象里的Tokens链表不为空且不含错误token
       /// </summary>
        public void SyntaxAnalyze()
        {  //语法分析主程序
            if (Tokens.Count == 0) return;
            CurrentToken = Tokens.First();
            try
            {
                SyntaxTree = new TreeNode(StmtType.Program);
                _errorInfo = new StringBuilder();
                _enumerator = Tokens.GetEnumerator().ToPeekable();
                BlockLevel = 0;
                SyntaxTree.LeftNode = ParseStmtSeq();

                if (_errorInfo.Length != 0)
                    throw new ParseException(_errorInfo.ToString());
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
            if ((BlockLevel != 0) && (BlockLevel <= 0 || CheckNextTokenType(TokenType.RightBra))) return null;
            var node = new TreeNode(StmtType.StmtSeq) {LeftNode = ParseStmt(), MiddleNode = ParseStmtSeq()};
            return node;
        }

        private TreeNode ParseStmt()
        { //处理语句或语句块
            switch (GetNextTokenType())
            {
                case TokenType.If:
                    return ParseIfStmt();  //转向处理if语句
                case TokenType.End:
                    ConsumeNextToken(TokenType.End);
                    return null;  //转向处理空语句
                case TokenType.While:
                    return ParseWhileStmt();  //转向处理while语句
                case TokenType.Int:
                case TokenType.Real:
                case TokenType.Char:
                    return ParseDeclareStmt();  //转向处理声明语句
                case TokenType.Print:
                    return ParsePrintStmt();  //转向处理打印语句
                case TokenType.Scan:
                    return ParseScanStmt();   //转向处理扫描语句
                case TokenType.LeftBra:
                    return ParseStmtBlock();  //转向处理语句块
                case TokenType.Break:
                case TokenType.Continue:
                    return ParseJumpStmt();  //转向处理跳转语句
                case TokenType.Id:
                    return ParseAssignStmt();  //转向处理赋值语句
                case TokenType.IntVal:
                case TokenType.CharVal:
                case TokenType.RealVal:
                case TokenType.Plus:
                case TokenType.Minus:
                case TokenType.PlusPlus:
                case TokenType.MinusMinus:
                case TokenType.LeftP:
                case TokenType.Not:
                    var node = ParseAssignExp();
                    ConsumeNextToken(TokenType.End);
                    return node;
                case -1:
                    return null;
                default:
                    _enumerator.MoveNext();
                    CurrentToken = _enumerator.Current;
                    _errorInfo.Append("ERROR : " + "line: ").Append(CurrentToken.LineNo).Append(" ").Append("\"").Append(CurrentToken.Value).Append("\"").Append(" 此处缺少语句或语句块.\n");
                    return null;
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
            var node = new TreeNode(StmtType.StmtBlock);
            ConsumeNextToken(TokenType.LeftBra);
            BlockLevel++;
            node.LeftNode = ParseStmtSeq();
            ConsumeNextToken(TokenType.RightBra);
            BlockLevel--;
            return node;
        }

        private TreeNode ParseIfStmt()
        {
            var node = new TreeNode(StmtType.IfStmt);
            ConsumeNextToken(TokenType.If);
            ConsumeNextToken(TokenType.LeftP);
            node.LeftNode = ParseAssignExp();
            ConsumeNextToken(TokenType.RightP);
            node.MiddleNode = ParseIfBlock();
            return node;
        }

        private TreeNode ParseIfBlock()
        {
            if (CheckNextTokenType(TokenType.Else))
            {
                return ParseElseStmt();
            }
            var node = new TreeNode(StmtType.IfBlock) { LeftNode = ParseStmt(), MiddleNode = ParseElseStmt() };
            if (node.LeftNode == null)
            {
                return null;
            }
            return node;
        }

        private TreeNode ParseElseStmt()
        {
            if (!CheckNextTokenType(TokenType.Else)) return null;
            var node = new TreeNode(StmtType.ElseStmt);
            ConsumeNextToken(TokenType.Else);
            if (CheckNextTokenType(TokenType.End))
            {
                ConsumeNextToken(TokenType.End);
                return null;
            }
            node.LeftNode = ParseStmt();
            return node;
        }

        private TreeNode ParseWhileStmt()
        {
            var node = new TreeNode(StmtType.WhileStmt);
            ConsumeNextToken(TokenType.While);
            ConsumeNextToken(TokenType.LeftP);
            node.LeftNode = ParseAssignExp();
            ConsumeNextToken(TokenType.RightP);
            node.MiddleNode = ParseStmt();
            return node;
        }

        private TreeNode ParseAssignStmt()
        {
            _isAssign = false;
            var node = ParseAssignExp();
            ConsumeNextToken(TokenType.End);
            if (!_isAssign) return node;
            _isAssign = false;
            var headNode = new TreeNode(StmtType.AssignStmt) {LeftNode = node};
            return headNode;
        }

        private TreeNode ParseAssignExp()
        {
            var node = new TreeNode(StmtType.Exp) {LeftNode = ParseLogicOrExp(), MiddleNode = ParseMoreLogicOrExp(), DataType = TokenType.AssignExp};
            if (node.MiddleNode == null)
            {
                node = node.LeftNode;
            }
            return node;
        }

        private TreeNode ParseMoreLogicOrExp()
        {
            if (!CheckNextTokenType(TokenType.Assign, TokenType.DivAssign,
                TokenType.MulAssign, TokenType.PlusAssign,
                TokenType.MinusAssign, TokenType.ModAssign)) return null;
            var node = new TreeNode(StmtType.MoreLogicExp)
            {
                LeftNode = GetAssignOp(),
                MiddleNode = ParseAssignExp()
                //MiddleNode = ParseLogicOrExp(),
                //RightNode = ParseMoreLogicOrExp()
            };
            return node;
        }

        private TreeNode ParseJumpStmt()
        {   //处理跳转语句
            _enumerator.MoveNext();
            CurrentToken = _enumerator.Current;
            var type = CurrentToken.Type;
            var node = new TreeNode(StmtType.JumpStmt) { DataType = type, Value = CurrentToken.Value };
            ConsumeNextToken(TokenType.End);
            return node;
        }

        private TreeNode ParseScanStmt()
        { //处理scan语句
            var node = new TreeNode(StmtType.ScanStmt);
            ConsumeNextToken(TokenType.Scan);
            ConsumeNextToken(TokenType.LeftP);
            node.LeftNode = ParseVariable();
            ConsumeNextToken(TokenType.RightP);
            ConsumeNextToken(TokenType.End);
            return node;
        }

        private TreeNode ParsePrintStmt()
        { //处理print语句
            var node = new TreeNode(StmtType.PrintStmt);
            ConsumeNextToken(TokenType.Print);
            ConsumeNextToken(TokenType.LeftP);
            node.LeftNode = ParseAssignExp();
            ConsumeNextToken(TokenType.RightP);
            ConsumeNextToken(TokenType.End);
            return node;
        }

        private TreeNode ParseDeclareStmt()
        {
            var type = GetNextTokenType();
            ConsumeNextToken(type);
            var node = ParseVariableList();
            node.DataType = type;
            ConsumeNextToken(TokenType.End);
            return node;
        }

        private TreeNode ParseVariableList()
        {
            var node = new TreeNode(StmtType.DecStmt);
            node.LeftNode = ParseVariable();
            node.LeftNode.RightNode = ParseInitializer();
            node.NextNode = ParseMoreVars();
            return node;
        }

        private TreeNode ParseVariable()
        {
            var node = new TreeNode(StmtType.Var) { LeftNode = GetIdentifier(), MiddleNode = ParseArrayDim() };
            return node;
        }

        private TreeNode ParseArrayDim()
        {
            if (!CheckNextTokenType(TokenType.LeftBrk)) return null;
            var node = new TreeNode(StmtType.ArrayDim);
            ConsumeNextToken(TokenType.LeftBrk);
            node.LeftNode = ParseAssignExp();
            ConsumeNextToken(TokenType.RightBrk);
            node.NextNode = ParseArrayDim();
            return node;
        }

        private TreeNode ParseLogicOrExp()
        {
            var node = new TreeNode(StmtType.Exp) {LeftNode = ParseLogicAndExp(), MiddleNode = ParseMoreLogicAndExp(), DataType = TokenType.LogicOrExp };
            if (node.MiddleNode == null)
            {
                node = node.LeftNode;
            }
            return node;
        }

        private TreeNode ParseLogicAndExp()
        {
            var node = new TreeNode(StmtType.Exp) { LeftNode = ParseCompEqExp(), MiddleNode = ParseMoreCompEqExp(), DataType = TokenType.LogicAndExp };
            if (node.MiddleNode == null)
            {
                node = node.LeftNode;
            }
            return node;
        }

        private TreeNode ParseMoreCompEqExp()
        {
            if (!CheckNextTokenType(TokenType.And)) return null;
            var node = new TreeNode(StmtType.MoreCompExp)
            {
                LeftNode = GetLogicOp(),
                MiddleNode = ParseCompEqExp(),
                RightNode = ParseMoreCompEqExp()
            };
            return node;
        }

        private TreeNode ParseCompEqExp()
        {
            var node = new TreeNode(StmtType.Exp) { LeftNode = ParseCompExp(), MiddleNode = ParseMoreCompExp(), DataType = TokenType.CompEqExp };
            if (node.MiddleNode == null)
            {
                node = node.LeftNode;
            }
            return node;
        }

        private TreeNode ParseMoreCompExp()
        {
            if (!CheckNextTokenType(TokenType.Eq, TokenType.Neq)) return null;
            var node = new TreeNode(StmtType.MoreCompExp)
            {
                LeftNode = GetCompOp(),
                MiddleNode = ParseCompExp(),
                RightNode = ParseMoreCompExp()
            };
            return node;
        }

        private TreeNode ParseCompExp()
        {
            var node = new TreeNode(StmtType.Exp) { LeftNode = ParseAdditiveExp(), MiddleNode = ParseMoreAdditiveExp(), DataType = TokenType.CompExp };
            if (node.MiddleNode == null)
            {
                node = node.LeftNode;
            }
            return node;
        }

        private TreeNode ParseMoreLogicAndExp()
        {
            if (!CheckNextTokenType(TokenType.Or)) return null;
            var node = new TreeNode(StmtType.MoreLogicExp)
            {
                LeftNode = GetLogicOp(),
                MiddleNode = ParseLogicAndExp(),
                RightNode = ParseMoreLogicAndExp()
            };
            return node;
        }

        private TreeNode ParseMoreVars()
        {
            if (!CheckNextTokenType(TokenType.Comma)) return null;
            ConsumeNextToken(TokenType.Comma);
            var node = ParseVariable();
            node.RightNode = ParseInitializer();
            return node;
        }

        private TreeNode ParseInitializer()
        {
            if (!CheckNextTokenType(TokenType.Assign)) return null;
            var node = new TreeNode(StmtType.Init) { LeftNode = GetAssignOp(), MiddleNode = ParseValue() };
            return node;
        }

        private TreeNode ParseValue()
        {
            if (!CheckNextTokenType(TokenType.LeftBra)) return ParseLogicOrExp();
            ConsumeNextToken(TokenType.LeftBra);
            var node = ParseValueList();
            ConsumeNextToken(TokenType.RightBra);
            return node;
        }

        private TreeNode ParseValueList()
        {    //赋值列表
            var node = new TreeNode(StmtType.ValueList) { LeftNode = ParseValue(), NextNode = ParseMoreValue() };
            return node;
        }

        private TreeNode ParseMoreValue()
        {
            if (!CheckNextTokenType(TokenType.Comma)) return null;
            ConsumeNextToken(TokenType.Comma);
            var node = ParseValue();
            node.NextNode = ParseMoreValue();
            return node;
        }

        private TreeNode GetIdentifier()
        {
            if (CheckNextTokenType(TokenType.Id))
            {
                _enumerator.MoveNext();
                CurrentToken = _enumerator.Current;
                var node = new TreeNode(StmtType.Id) { Value = CurrentToken.Value };
                return node;
            }
            _errorInfo.Append("ERROR : " + "line: ").Append(CurrentToken.LineNo).Append(" ").Append("\"").Append(CurrentToken.Value).Append("\"").Append(" 此处缺少标识符.\n");
            return null;
        }

        private TreeNode GetLogicOp()
        {  //处理逻辑操作符

            if (CheckNextTokenType(TokenType.And, TokenType.Or, TokenType.Not))
            {
                _enumerator.MoveNext();
                CurrentToken = _enumerator.Current;
                var node = new TreeNode(StmtType.Operator) { DataType = CurrentToken.Type, Value = CurrentToken.Value };
                return node;
            }
            _errorInfo.Append("ERROR : " + "line: ").Append(CurrentToken.LineNo).Append(" ").Append("\"").Append(CurrentToken.Value).Append("\"").Append(" 此处缺少逻辑运算符.\n");
            return null;
        }

        private TreeNode ParseMoreAdditiveExp()
        {
            if (!CheckNextTokenType(TokenType.Greater,
                TokenType.GreaterEq, TokenType.Less,
                TokenType.LessEq)) return null;
            var node = new TreeNode(StmtType.MoreAddExp)
            {
                LeftNode = GetCompOp(),
                MiddleNode = ParseAdditiveExp(),
                RightNode = ParseMoreAdditiveExp()
            };
            return node;
        }

        private TreeNode GetCompOp()
        {  //逻辑运算符
            if (CheckNextTokenType(TokenType.LessEq, TokenType.Less,
                    TokenType.Eq, TokenType.Neq,
                    TokenType.GreaterEq, TokenType.Greater))
            {
                _enumerator.MoveNext();
                CurrentToken = _enumerator.Current;
                var node = new TreeNode(StmtType.Operator) { DataType = CurrentToken.Type, Value = CurrentToken.Value };
                return node;
            }
            _errorInfo.Append("ERROR : " + "line: ").Append(CurrentToken.LineNo).Append(" ").Append("\"").Append(CurrentToken.Value).Append("\"").Append(" 此处缺少关系运算符.\n");
            return null;
        }

        private TreeNode ParseAdditiveExp()
        {    //多项式
            var node = new TreeNode(StmtType.Exp) { LeftNode = ParseTerm(), MiddleNode = ParseMoreTerm(), DataType = TokenType.AdditiveExp };
            if (node.MiddleNode == null)
            {
                node = node.LeftNode;
            }
            return node;
        }

        private TreeNode ParseMoreTerm()
        {
            if (CheckNextTokenType(TokenType.Plus, TokenType.Minus))
            {
                var node = new TreeNode(StmtType.MoreTerm)
                {
                    LeftNode = GetAlgOp(),
                    MiddleNode = ParseTerm(),
                    RightNode = ParseMoreTerm()
                };
                return node;
            }
            return null;
        }

        private TreeNode GetAlgOp()
        {  //处理多项式算术运算符
            if (CheckNextTokenType(TokenType.Plus, TokenType.Minus, TokenType.Mul, TokenType.Div, TokenType.Mod))
            {
                _enumerator.MoveNext();
                CurrentToken = _enumerator.Current;
                var node = new TreeNode(StmtType.Operator) { DataType = CurrentToken.Type, Value = CurrentToken.Value };
                return node;
            }
            _errorInfo.Append("ERROR : " + "line: ").Append(CurrentToken.LineNo).Append(" ").Append("\"").Append(CurrentToken.Value).Append("\"").Append(" 此处缺少算术运算符.\n");
            return null;
        }

        private TreeNode GetAssignOp()
        {  //处理多项式算术运算符
            if (CheckNextTokenType(TokenType.Assign, TokenType.DivAssign,
                    TokenType.MulAssign, TokenType.PlusAssign,
                    TokenType.MinusAssign, TokenType.ModAssign))
            {
                _enumerator.MoveNext();
                CurrentToken = _enumerator.Current;
                var node = new TreeNode(StmtType.Operator) { DataType = CurrentToken.Type, Value = CurrentToken.Value };
                _isAssign = true;
                return node;
            }
            _errorInfo.Append("ERROR : " + "line: ").Append(CurrentToken.LineNo).Append(" ").Append("\"").Append(CurrentToken.Value).Append("\"").Append(" 此处缺少赋值运算符.\n");
            return null;
        }

        private TreeNode ParseTerm()
        {  //处理项
            var node = new TreeNode(StmtType.Term) { LeftNode = ParseFactor(), MiddleNode = ParseMoreFactor() };
            if (node.MiddleNode == null)
            {
                node = node.LeftNode;
            }
            return node;
        }

        private TreeNode ParseFactor()
        {
            var node = new TreeNode(StmtType.Factor);
            switch (GetNextTokenType())
            {
                case TokenType.Not:
                    node.LeftNode = GetLogicOp();
                    node.MiddleNode = ParseFactor();
                    return node;
                case TokenType.PlusPlus:
                case TokenType.MinusMinus:
                    node.LeftNode = GetIncDecOp();
                    node.MiddleNode = ParseFactor();
                    return node;
                case TokenType.Plus:
                case TokenType.Minus:
                    node.LeftNode = GetAlgOp();
                    node.MiddleNode = ParseFactor();
                    return node;
                default:
                    node.LeftNode = ParseSpecific();
                    node.MiddleNode = ParsePossibleIncDecOp();
                    if (node.MiddleNode == null)
                    {
                        node = node.LeftNode;
                    }
                    return node;
            }
        }

        private TreeNode ParsePossibleIncDecOp()
        {
            return CheckNextTokenType(TokenType.PlusPlus, TokenType.MinusMinus) ? GetIncDecOp() : null;
        }

        private TreeNode ParseSpecific()
        {
            TreeNode node;
            switch (GetNextTokenType())
            {
                case TokenType.LeftP:
                    ConsumeNextToken(TokenType.LeftP);
                    node = ParseAssignExp();
                    ConsumeNextToken(TokenType.RightP);
                    return node;
                case TokenType.IntVal:
                case TokenType.RealVal:
                case TokenType.CharVal:
                    node = GetSpecificValue();
                    return node;
                case TokenType.Null:
                    _enumerator.MoveNext();
                    CurrentToken = _enumerator.Current;
                    node = new TreeNode(StmtType.Null)
                    {
                        DataType = TokenType.Int,
                        Value = CurrentToken?.Value
                    };
                    return node;
                //case TokenType.Plus:
                //    ConsumeNextToken(TokenType.Plus);
                //    node = ParseTerm();
                //    node.DataType = TokenType.Plus;
                //    return node;
                //case TokenType.Minus:
                //    ConsumeNextToken(TokenType.Minus);
                //    node = ParseTerm();
                //    node.DataType = TokenType.Minus;
                //    return node;
                case TokenType.Scan:
                    ConsumeNextToken(TokenType.Scan);
                    ConsumeNextToken(TokenType.LeftP);
                    node = ParseVariable();
                    ConsumeNextToken(TokenType.RightP);
                    return node;
                case TokenType.Id:
                    node = ParseVariable();
                    return node;
                default:
                    _errorInfo.Append("ERROR : " + "line: ").Append(CurrentToken.LineNo).Append(" ").Append("\"").Append(CurrentToken.Value).Append("\"").Append(" 此处缺少标识符或数值或表达式.\n");
                    return null;
            }
        }

        private TreeNode GetIncDecOp()
        {     
            if (CheckNextTokenType(TokenType.PlusPlus, TokenType.MinusMinus))
            {
                _enumerator.MoveNext();
                CurrentToken = _enumerator.Current;
                var node = new TreeNode(StmtType.Operator) { DataType = CurrentToken.Type, Value = CurrentToken.Value };
                return node;
            }
            _errorInfo.Append("ERROR : " + "line: ").Append(CurrentToken.LineNo).Append(" ").Append("\"").Append(CurrentToken.Value).Append("\"").Append(" 此处缺少自增/自减运算符.\n");
            return null;
        }

        private TreeNode ParseMoreFactor()
        {
            if (!CheckNextTokenType(TokenType.Mul, TokenType.Div, TokenType.Mod)) return null;
            var node = new TreeNode(StmtType.MoreFactor) { LeftNode = GetAlgOp(), MiddleNode = ParseFactor(),RightNode = ParseMoreFactor() };
            return node;
        }

        private TreeNode GetSpecificValue()
        {    //获取具体的值
            if (CheckNextTokenType(TokenType.IntVal, TokenType.RealVal, TokenType.CharVal))
            {
                _enumerator.MoveNext();
                CurrentToken = _enumerator.Current;
                var node = new TreeNode(StmtType.Value) { DataType = CurrentToken.Type, Value = CurrentToken.Value };
                return node;
            }
            _errorInfo.Append("ERROR : " + "line: ").Append(CurrentToken.LineNo).Append(" ").Append("\"").Append(CurrentToken.Value).Append("\"").Append(" 此处缺少值类型.\n");
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
                    _errorInfo.Append("ERROR : " + "line: ").Append(CurrentToken.LineNo).Append(" ").Append("\"").Append(CurrentToken.Value).Append("\"").Append(" 此处缺少").Append(new Token(type).TypeToString()).Append("（类型）.\n");
                return;
            }
            _errorInfo.Append("ERROR : " + "line: ").Append(CurrentToken.LineNo).Append(" ").Append("\"").Append(CurrentToken.Value).Append("\"").Append(" 此处缺少").Append(new Token(type).TypeToString()).Append("（类型）.\n");
            throw new ParseException(_errorInfo.ToString());
        }
    }
}
