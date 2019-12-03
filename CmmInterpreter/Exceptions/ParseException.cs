using System;

namespace CmmInterpreter.Exceptions
{
    /// <summary>
    /// 语法分析器异常
    /// </summary>
    public class ParseException : Exception
    {
        public ParseException() : base()
        {
        }

        public ParseException(string message) : base(message)
        {
        }
    }
}
