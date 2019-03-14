using System;
using System.Reflection;

namespace WhampsChallenge.Exceptions
{
    /// <summary>
    /// This exception models that something happened internally to the engine.
    /// When this exception occurs it is not the fault of the contestant, so it doesn't disqualify the contestant.
    /// </summary>
    public class InternalException : Exception
    {
        public InternalException(string message) : base(message)
        {
        }

        public InternalException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}