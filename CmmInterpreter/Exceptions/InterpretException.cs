using System;

namespace CmmInterpreter.Exceptions
{
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
