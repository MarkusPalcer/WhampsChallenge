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
    }
}
