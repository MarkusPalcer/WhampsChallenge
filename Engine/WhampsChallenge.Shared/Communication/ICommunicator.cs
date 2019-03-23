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
        /// Sends and receives a single message
        /// </summary>
        string SendAndReceive(string message);

        /// <summary>
        /// Sends and receives a single message
        /// </summary>
        Task<string> SendAndReceiveAsync(string request);
    }
}