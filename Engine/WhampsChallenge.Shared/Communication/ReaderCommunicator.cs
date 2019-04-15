using System.IO;
using System.Threading.Tasks;

namespace WhampsChallenge.Shared.Communication
{
    /// <inheritdoc />
    /// <summary>
    /// An <see cref="T:WhampsChallenge.Shared.Communication.ICommunicator" /> that uses <see cref="T:System.IO.TextWriter" /> and <see cref="T:System.IO.TextReader" /> to read and write one message per line
    /// </summary>
    public class ReaderCommunicator : ICommunicator
    {
        private readonly TextWriter writer;
        private readonly TextReader reader;

        /// <summary>
        /// Creates a new instance of a ReaderCommunicator
        /// </summary>
        /// <param name="reader">The <see cref="TextReader"/> to read messages from</param>
        /// <param name="writer">The <see cref="TextWriter"/> to write messages to</param>
        public ReaderCommunicator(TextReader reader, TextWriter writer)
        {
            this.writer = writer;
            this.reader = reader;
        }

        public void Dispose()
        {
            writer?.Dispose();
            reader?.Dispose();
        }

        public void Send(string message)
        {
            writer.WriteLine(message);
            writer.Flush();
        }

        public async Task SendAsync(string message)
        {
            await writer.WriteLineAsync(message);
            await writer.FlushAsync();
        }

        public string Receive()
        {
            return reader.ReadLine();
        }

        public async Task<string> ReceiveAsync()
        {
            return await reader.ReadLineAsync();
        }
    }
}