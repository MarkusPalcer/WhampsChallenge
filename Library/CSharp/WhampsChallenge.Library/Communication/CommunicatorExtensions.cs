using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WhampsChallenge.Library.Communication
{
    /// <summary>
    /// Extends all the <see cref="ICommunicator"/> implementations
    /// </summary>
    public static class CommunicatorExtensions
    {
        /// <summary>
        /// Executes an action on the contest host
        /// </summary>
        /// <typeparam name="TResponse">The response of the contest host to the action</typeparam>
        public static TResponse ExecuteAction<TResponse>(this ICommunicator self, object message)
        {
            var response = self.SendAndReceive(JsonConvert.SerializeObject(message));
            return JsonConvert.DeserializeObject<TResponse>(response);
        }

        /// <summary>
        /// Executes an action on the contest host
        /// </summary>
        /// <typeparam name="TResponse">The response of the contest host to the action</typeparam>
        public static async Task<TResponse> ExecuteActionAsync<TResponse>(this ICommunicator self, object message)
        {
            var request = await Task.Factory.StartNew(() => JsonConvert.SerializeObject(message));
            var response = await self.SendAndReceiveAsync(request);
            return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<TResponse>(response));
        }
    }
}