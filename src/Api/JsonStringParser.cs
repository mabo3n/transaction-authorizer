using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Authorizer.Api
{
    public class JsonStringParser
    {
        private readonly JsonNamingPolicy namingPolicy;
        private readonly JsonSerializerOptions parsingOptions;

        public JsonStringParser()
        {
            namingPolicy = new DashCaseNamingPolicy();
            parsingOptions = new JsonSerializerOptions {
                PropertyNamingPolicy = namingPolicy,
                Converters = { new JsonStringEnumConverter(namingPolicy) }
            };
        }

        public (string name, string value) GetRootAttribute(string jsonString)
        {
            using var json = JsonDocument.Parse(jsonString);
            var rootNode = json.RootElement.EnumerateObject().First();
            return (rootNode.Name, rootNode.Value.GetRawText());
        }

        public T Parse<T>(string jsonString)
            => JsonSerializer .Deserialize<T>(jsonString, parsingOptions);

        public string Stringify<T>(T @object)
            => JsonSerializer.Serialize<T>(@object, parsingOptions);
    }

    public class DashCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
            => Regex.Replace(
                name,
                pattern: "(?<!^)([A-Z])",
                replacement: "-$1"
            ).ToLower();
    }
}
