using System;
using System.Collections.Generic;
using System.Text;

namespace ZefirCloudFileServerLib.core.server.transport
{
    public class DispatcherResult
    {
        public string Json { get; set; }
        public byte[] Byte { get; set; }
        public bool IsJson { get; set; }
    }
}
