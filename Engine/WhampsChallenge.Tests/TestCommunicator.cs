using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WhampsChallenge.Shared.Communication;

namespace WhampsChallenge.Tests
{
    public class TestCommunicator : ICommunicator
    {
        private TaskCompletionSource<string> response;
        private TaskCompletionSource<string> request = new TaskCompletionSource<string>();
        
        public void Dispose()
        {
        }

        public string SendAndReceive(string message)
        {
            throw new NotImplementedException();
        }

        public Task<string> SendAndReceiveAsync(string request)
        {
            response = new TaskCompletionSource<string>();
            this.request.SetResult(request);
            return response.Task;
        }

        public async Task<JObject> GetLastMessage()
        {
            var lastMessage = await request.Task;
            request = new TaskCompletionSource<string>();
            return JsonConvert.DeserializeObject<JObject>(lastMessage);
        }

        public void SetResponse(object response)
        {
            this.response.SetResult(JsonConvert.SerializeObject(response));
        }
    }
}