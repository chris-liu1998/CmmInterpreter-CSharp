using System;

namespace CmmInterpreter.Exceptions
{      
     /// <summary>
     /// 语义分析器异常
     /// </summary>
    public class InterpretException : Exception
    {
        public InterpretException() : base()
        {
        }

        public InterpretException(string message) : base(message)
        {
        }
    }
}
