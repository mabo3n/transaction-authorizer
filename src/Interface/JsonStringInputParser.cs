using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Authorizer.Application;

namespace Authorizer.Interface
{
    public class JsonStringInputParser : IInputParser<string>
    {
        private static IDictionary<string, Type> TypeMap
            = new Dictionary<string, Type>
            {
                ["create-account"] = typeof(CreateAccount),
                ["transaction"] = typeof(AuthorizeTransaction),
            };

        private static JsonSerializerOptions parseOptions
            = new JsonSerializerOptions
            {
                // PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = new DashCaseNamingPolicy()
            };

        public Operation Parse(string input)
        {
            using (var json = JsonDocument.Parse(input))
            {
                var rootNode = json.RootElement.EnumerateObject().First();
                var matchingType = TypeMap[rootNode.Name];

                var o = JsonSerializer
                    .Deserialize(rootNode.Value.GetRawText(), matchingType, parseOptions)
                    as Operation;

                // var newCreateAcc = new CreateAccount() { AvailableLimit = 300 };
                // var test = JsonSerializer.Serialize<CreateAccount>(newCreateAcc, parseOptions);

                Console.WriteLine(o);
                return o;
            }
        }
    }
}
