﻿using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        private TaskCompletionSource<string> hostMessage;
        private TaskCompletionSource<string> contestantMessage = new TaskCompletionSource<string>();
        private bool isDisposed;

        public void Dispose()
        {
            if (isDisposed) return;

            hostMessage?.TrySetCanceled();
            isDisposed = true;
        }

        public async Task<JObject> ReceiveFromContestantAsync()
        {
            var lastMessage = await contestantMessage.Task;
            Console.WriteLine("RECV: " + lastMessage);
            contestantMessage = new TaskCompletionSource<string>();
            return JsonConvert.DeserializeObject<JObject>(lastMessage);
        }

        public void SendToContestant(object message)
        {
            var serializedMessage = JsonConvert.SerializeObject(message);
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