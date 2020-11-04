using System;
using System.Collections.Generic;
using System.Text;
using ZefirCloudFileServerLib.core.server.dispatcher;
using ZefirCloudFileServerLib.core.server.transport;
using ZefirCloudFileServerLib.core.settings;
using ZefirCloudFileServerLib.core.users;

namespace ZefirCloudFileServerLib.core
{
    public class ZefirServer
    {
        private readonly ZefirServerSettings _zefirServerSettings;
        private readonly HttpTransport _httpTransport;
        private readonly UserStorage _userStorage;
        private readonly ProtocolDispatcher _protocolDispatcher;

        public ZefirServer(ZefirServerSettings zefirServerSettings)
        {
            _zefirServerSettings = zefirServerSettings;
            _userStorage = new UserStorage();
            _protocolDispatcher = new ProtocolDispatcher(_userStorage);
            _httpTransport = new HttpTransport(_zefirServerSettings.Host, _protocolDispatcher);
        }

        public void Start()
        {
            _httpTransport.StartServer();
        }

        public void Stop()
        {
            _httpTransport.StopServer();
        }
    }
}
