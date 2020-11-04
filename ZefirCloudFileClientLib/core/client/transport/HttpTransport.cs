using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZefirCloudFileClientLib.core.client.transport
{
    public class HttpTransport
    {
        private readonly HttpClient _httpClient;

        private HttpTransport()
        {
            _httpClient = new HttpClient();
        }
        public static HttpTransport Create()
        {
            return new HttpTransport();
        }

        public async Task<ServerResponse> SendToServer(string host, string message, byte[] data, bool isJsonAnswer)
        {
            var result = new ServerResponse();
            try
            {
                var content = new ByteArrayContent(data);
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                content.Headers.Add("MessageType", message);
                var response = await _httpClient.PostAsync(new Uri($"{host}"), content);
                if (response.IsSuccessStatusCode)
                {
                    if (isJsonAnswer)
                    {
                        result.Json = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        result.Bytes = await response.Content.ReadAsByteArrayAsync();
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                throw new Exception($"HttpTransport SendToServer Error: {e.Message}");
            }
        }

        public class ServerResponse
        {
            public string Json { get; set; }
            public byte[] Bytes { get; set; }
        }
    }
}
