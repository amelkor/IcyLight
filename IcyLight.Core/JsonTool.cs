using System.Text.Encodings.Web;
using System.Text.Json;

namespace IcyLight.Core
{
    public static class JsonTool
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };
        
        public static string Serialize(object o)
        {
            return JsonSerializer.Serialize(o, _options);
        }
    }
}