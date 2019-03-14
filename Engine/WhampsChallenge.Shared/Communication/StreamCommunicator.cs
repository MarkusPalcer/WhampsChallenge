using System.IO;
using System.Threading.Tasks;

namespace WhampsChallenge.Shared.Communication
{
    /// <summary>
    /// An implementation of <see cref="ICommunicator"/> that reads and writes messages one line per message
    /// </summary>
    public class StreamCommunicator : ICommunicator
    {
        private readonly ReaderCommunicator _internalCommunicator;

        /// <summary>
        /// Creates a new instance of a StreamCommunicator
        /// </summary>
        /// <param name="inStream">The stream to read from</param>
        /// <param name="outStream">The stream to write to</param>
        public StreamCommunicator(Stream inStream, Stream outStream)
        {
            _internalCommunicator = new ReaderCommunicator(new StreamReader(inStream), new StreamWriter(outStream));
        }

        public void Dispose()
        {
            _internalCommunicator?.Dispose();
        }

        public string SendAndReceive(string message)
        {
            return _internalCommunicator.SendAndReceive(message);
        }

        public async Task<string> SendAndReceiveAsync(string request)
        {
            return await _internalCommunicator.SendAndReceiveAsync(request);
        }
    }
}