using System;

namespace CmmInterpreter.Exceptions
{
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
