using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WhampsChallenge.Core.Common;
using WhampsChallenge.Core.Common.Discovery;
using WhampsChallenge.Core.Common.Events;
using WhampsChallenge.Shared.Communication;

namespace WhampsChallenge.Runner.Shared.Direct
{
    /// <summary>
    /// Provides a way for contestant and contest host to communicate directly (without IPC)
    ///
    /// The Interface <see cref="ICommunicator" /> implements the view from the contestant, the public methods implement the view from the host.
    /// </summary>
    public class DirectCommunicator : ICommunicator
    {
        private readonly KnownTypeJsonConverter<IAction> actionKnownTypeJsonConverter;
        private readonly KnownTypeJsonConverter<IEvent> eventKnownTypeJsonConverter;

        private TaskCompletionSource<string> hostMessage;
        private TaskCompletionSource<string> contestantMessage = new();
        private bool isDisposed;

        public DirectCommunicator(IDiscoverer discoverer, int level)
        {
            actionKnownTypeJsonConverter = new KnownTypeJsonConverter<IAction>("Action", discoverer[level].Actions);
            eventKnownTypeJsonConverter = new KnownTypeJsonConverter<IEvent>("Event", discoverer[level].Events);
        }

        public void Dispose()
        {
            if (isDisposed) return;

            hostMessage?.TrySetCanceled();
            isDisposed = true;
        }

        public async Task<IAction> ReceiveFromContestantAsync()
        {
            var lastMessage = await contestantMessage.Task;
            Console.WriteLine("RECV: " + lastMessage);
            contestantMessage = new TaskCompletionSource<string>();
            return JsonConvert.DeserializeObject<IAction>(lastMessage, actionKnownTypeJsonConverter);
        }

        public void SendToContestant(object message)
        {
            var serializedMessage = JsonConvert.SerializeObject(message, eventKnownTypeJsonConverter);
            Console.WriteLine("SEND: " + serializedMessage);
            hostMessage.SetResult(serializedMessage);
        }

        public void Send(string message)
        {
            hostMessage = new TaskCompletionSource<string>();
            if (isDisposed) hostMessage.SetCanceled();
            contestantMessage.SetResult(message);
        }

        public Task SendAsync(string message)
        {
            Send(message);
            return Task.CompletedTask;
        }

        public string Receive()
        {
            return ReceiveAsync().Result;
        }

        public async Task<string> ReceiveAsync()
        {
            return await hostMessage.Task;
        }
    }
}
