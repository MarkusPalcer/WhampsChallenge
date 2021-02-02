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
    }
}
