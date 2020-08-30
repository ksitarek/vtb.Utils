using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace vtb.Utils.Extensions
{
    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> PostAsJsonAsync(this HttpClient client, string requestUri,
            object payload)
        {
            var stringContent =
                new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            return client.PostAsync(requestUri, stringContent);
        }

        public static async Task<T> ReadContentAsObjectAsync<T>(this HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}