using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WhampsChallenge.Library.Communication
{
    /// <summary>
    /// An <see cref="ICommunicator"/> that uses <see cref="TextReader"/> and <see cref="TextWriter"/> to read and write one message per line
    /// </summary>
    public class ReaderCommunicator : ICommunicator
    {
        private readonly TextWriter _writer;
        private readonly TextReader _reader;

        /// <summary>
        /// Creates a new instance of a ReaderCommunicator
        /// </summary>
        /// <param name="reader">The <see cref="TextReader"/> to read messages from</param>
        /// <param name="writer">The <see cref="TextWriter"/> to write messages to</param>
        public ReaderCommunicator(TextReader reader, TextWriter writer)
        {
            _writer = writer;
            _reader = reader;
        }

        public void Dispose()
        {
            _writer?.Dispose();
            _reader?.Dispose();
        }

        public string SendAndReceive(string message)
        {
            _writer.WriteLine(message);
            _writer.Flush();
            return _reader.ReadLine();
        }

        public async Task<string> SendAndReceiveAsync(string request)
        {
            await _writer.WriteLineAsync(request);
            _writer.FlushAsync().FireAndForget(); // When flushing is not done yet, the reader will wait longer for the result.
            return await _reader.ReadLineAsync();
        }
    }
}