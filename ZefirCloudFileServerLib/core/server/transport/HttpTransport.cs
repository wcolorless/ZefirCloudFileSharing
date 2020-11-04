using System;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using ZefirCloudFileCommon.core.protocol;
using ZefirCloudFileServerLib.core.server.dispatcher;

namespace ZefirCloudFileServerLib.core.server.transport
{
    public class HttpTransport
    {
        private HttpListener _httpListener;
        private ProtocolDispatcher _protocolDispatcher;
        private bool IsActive = false;

        public HttpTransport(string host, ProtocolDispatcher protocolDispatcher)
        {
            _protocolDispatcher = protocolDispatcher;
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(host);
        }

        public void StartServer()
        {
            try
            {
                IsActive = true;
                while (IsActive)
                {
                    _httpListener.Start();
                    var context = _httpListener.BeginGetContext(ServerCycle, _httpListener);
                    context.AsyncWaitHandle.WaitOne();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"HttpTransport StartServer Error: {e.Message}");
            }
        }

        public void StopServer()
        {
            IsActive = false;
        }

        public async void ServerCycle(IAsyncResult result)
        {
            var listener = (HttpListener)result.AsyncState;
            try
            {
                var context = listener.EndGetContext(result);
                var request = context.Request;
                var messageType = JsonConvert.DeserializeObject<ProtocolBox>(request.Headers.Get("MessageType"));
                messageType.FileOrDirName = Encoding.UTF8.GetString(Convert.FromBase64String(messageType.FileOrDirName));
                Console.WriteLine($"New event: time: {DateTime.UtcNow}; type: {messageType.Type}");
                var response = context.Response;
                var dispatcherResult = _protocolDispatcher.Handle(request, messageType).Result;
                if (dispatcherResult.IsJson)
                {
                    var buffer = Encoding.UTF8.GetBytes(dispatcherResult.Json);
                    response.ContentLength64 = buffer.Length;
                    var output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    output.Close();
                }
                else
                {
                    response.ContentLength64 = dispatcherResult.Byte.Length;
                    var output = response.OutputStream;
                    output.Write(dispatcherResult.Byte, 0, dispatcherResult.Byte.Length);
                    output.Close();
                }
            }
            catch (Exception e) 
            {
                throw new Exception($"HttpTransport ServerCycle Error: {e.Message}");
            }
        }
    }
}
