using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WhampsChallenge.Shared.Communication;

namespace WhampsChallenge.Shared.Extensions
{
    /// <summary>
    /// Extends all the <see cref="ICommunicator"/> implementations
    /// </summary>
    public static class CommunicatorExtensions
    {
        static CommunicatorExtensions()
        {
            JsonConvert.DefaultSettings = () =>
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new StringEnumConverter());
                return settings;
            };
        }

        public static string SendAndReceive(this ICommunicator self, string message)
        {
            self.Send(message);
            return self.Receive();
        }

        public static async Task<string> SendAndReceiveAsync(this ICommunicator self, string message)
        {
            await self.SendAsync(message);
            return await self.ReceiveAsync();
        }

        /// <summary>
        /// Serializes and sends a message
        /// </summary>
        public static void Send(this ICommunicator self, object message)
        {
            self.Send(JsonConvert.SerializeObject(message));
        }

        /// <summary>
        /// Serializes and sends a message asynchronously
        /// </summary>
        public static async Task SendAsync(this ICommunicator self, object message)
        {
            var serialized = await Task.Factory.StartNew(() => JsonConvert.SerializeObject(message));
            await self.SendAsync(serialized);
        }

        /// <summary>
        /// Waits for a reply and deserializes it
        /// </summary>
        /// <typeparam name="TResponse">The reply type</typeparam>
        public static TResponse Receive<TResponse>(this ICommunicator self)
        {
            return JsonConvert.DeserializeObject<TResponse>(self.Receive());
        }

        /// <summary>
        /// Waits for a reply and deserializes it asynchronously
        /// </summary>
        /// <typeparam name="TResponse">The reply type</typeparam>
        public static async Task<TResponse> ReceiveAsync<TResponse>(this ICommunicator self)
        {
            var message = await self.ReceiveAsync();
            return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<TResponse>(message));
        }
    }
}