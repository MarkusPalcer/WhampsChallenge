using System.IO;
using Newtonsoft.Json;

namespace ContractGeneration
{
    public static class Program
    {
        private const string FileName = "Library\\common\\contract.json";

        public static void Main()
        {
            File.WriteAllText(FileName, JsonConvert.SerializeObject(new Contract(), Formatting.Indented));
        }
    }
}