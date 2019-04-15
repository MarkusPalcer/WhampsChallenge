using System;
using System.Threading.Tasks;
using PantherDI.Attributes;

namespace WhampsChallenge.Shared.Communication
{
    /// <summary>
    /// A means to communicate with the contest host
    /// </summary>
    [Contract]
    public interface ICommunicator : IDisposable
    {
        /// <summary>
        /// Sends the given message
        /// </summary>
        void Send(string message);

        /// <summary>
        /// Sends the given message asynchronously
        /// </summary>
        Task SendAsync(string message);

        /// <summary>
        /// Receives a single message
        /// </summary>
        string Receive();

        /// <summary>
        /// Receives a single message asynchronously
        /// </summary>
        /// <returns></returns>
        Task<string> ReceiveAsync();
    }
}
