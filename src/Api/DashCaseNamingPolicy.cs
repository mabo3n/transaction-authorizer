using System.Text.Json;
using System.Text.RegularExpressions;

namespace Authorizer.Api
{
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
