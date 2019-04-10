using System;
using System.IO;

namespace WhampsChallenge.Shared.Communication
{
    /// <summary>
    /// Contains helper methods for creation of <see cref="ICommunicator"/> instances
    /// </summary>
    public static class Communicator
    {
        /// <summary>
        /// Creates a new <see cref="ICommunicator"/> using the given streams to input and output messages line by line
        /// </summary>
        public static ICommunicator Create(Stream inStream, Stream outStream)
        {
            ValidateInputStream(inStream);
            ValidateOutputStream(outStream);

            return Create(new StreamReader(inStream), new StreamWriter(outStream));
        }

        /// <summary>
        /// Creates a new <see cref="ICommunicator"/> using the given <see cref="TextReader"/> and <see cref="TextWriter"/> for input and output respectively.
        /// </summary>
        public static ICommunicator Create(TextReader input, TextWriter output)
        {
            return new ReaderCommunicator(input, output);
        }

        /// <summary>
        /// Creates a new <see cref="ICommunicator"/> using the given <see cref="Stream"/> for input and the given <see cref="TextWriter"/> for output
        /// </summary>
        public static ICommunicator Create(Stream inStream, TextWriter output)
        {
            ValidateInputStream(inStream);
            return Create(new StreamReader(inStream), output);
        }

        /// <summary>
        /// Creates a new <see cref="ICommunicator"/> using the given <see cref="TextReader"/> for input and the given <see cref="Stream"/> for output.
        /// </summary>
        public static ICommunicator Create(TextReader input, Stream outStream)
        {
            ValidateOutputStream(outStream);
            return Create(input, new StreamWriter(outStream));
        }

        /// <summary>
        /// Creates a new <see cref="ICommunicator"/> that inputs and outputs using STDIN and STDOUT respectively.
        /// </summary>
        public static ICommunicator FromConsole()
        {
            return Create(System.Console.In, System.Console.Out);
        }

        public static ICommunicator FromStream(Stream communicationStream)
        {
            return Create(communicationStream, communicationStream);
        }

        private static void ValidateOutputStream(Stream outStream)
        {
            if (!outStream.CanWrite)
                throw new ArgumentException("The output stream must support writing", nameof(outStream));
        }

        private static void ValidateInputStream(Stream inStream)
        {
            if (!inStream.CanRead)
                throw new ArgumentException("The input stream must support reading", nameof(inStream));
        }
    }
}