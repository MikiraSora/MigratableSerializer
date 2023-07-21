using System;
using System.Collections.Generic;
using System.Text;

namespace MigratableSerializer.Base.Exceptions
{
    public class MigrationException : Exception
    {
        public MigrationException(string message) : base(message)
        {
        }
    }
}
