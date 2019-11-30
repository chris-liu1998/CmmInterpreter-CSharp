using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmmInterpreter.Exceptions
{
    public class InterpretException:Exception
    {
        public InterpretException() : base()
        {
        }

        public InterpretException(string message) : base(message)
        {
        }
    }
}
