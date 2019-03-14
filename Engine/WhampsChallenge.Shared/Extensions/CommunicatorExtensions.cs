using System.Threading.Tasks;
using Newtonsoft.Json;
using WhampsChallenge.Shared.Communication;

namespace WhampsChallenge.Shared.Extensions
{
    /// <summary>
    /// Extends all the <see cref="ICommunicator"/> implementations
    /// </summary>
    public static class CommunicatorExtensions
    {
        /// <summary>
        /// Sends a message and expects a specific reply
        /// </summary>
        /// <typeparam name="TResponse">The reply type</typeparam>
        public static TResponse SendAndReceive<TResponse>(this ICommunicator self, object message)
        {
            var response = self.SendAndReceive(JsonConvert.SerializeObject(message));
            return JsonConvert.DeserializeObject<TResponse>(response);
        }

        /// <summary>
        /// Sends a message and expects a specific reply
        /// </summary>
        /// <typeparam name="TResponse">The reply type</typeparam>
        public static async Task<TResponse> SendAndReceiveAsync<TResponse>(this ICommunicator self, object message)
        {
            var request = await Task.Factory.StartNew(() => JsonConvert.SerializeObject(message));
            var response = await self.SendAndReceiveAsync(request);
            return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<TResponse>(response));
        }
    }
}