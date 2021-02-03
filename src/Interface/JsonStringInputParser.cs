using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Authorizer.Application;

namespace Authorizer.Interface
{
    public class JsonStringInputParser : IInputParser<string>
    {
        private static IDictionary<string, Type> typeMap
            = new Dictionary<string, Type>
            {
                ["account"] = typeof(CreateAccount),
                ["transaction"] = typeof(AuthorizeTransaction),
            };

        private readonly JsonNamingPolicy namingPolicy;
        private readonly JsonSerializerOptions parsingOptions;

        public JsonStringInputParser()
        {
            namingPolicy = new DashCaseNamingPolicy();
            parsingOptions = new JsonSerializerOptions {
                PropertyNamingPolicy = namingPolicy,
                Converters = { new JsonStringEnumConverter(namingPolicy) }
            };
        }

        public Operation Parse(string input)
        {
            using (var json = JsonDocument.Parse(input))
            {
                var rootNode = json.RootElement.EnumerateObject().First();
                var matchingType = typeMap[rootNode.Name];

                var o = JsonSerializer
                    .Deserialize(rootNode.Value.GetRawText(), matchingType, parsingOptions)
                    as Operation;

                // var newCreateAcc = new CreateAccount() { AvailableLimit = 300 };
                // var test = JsonSerializer.Serialize<CreateAccount>(newCreateAcc, parseOptions);

                // Console.WriteLine(o);
                return o;
            }
        }
    }
}
