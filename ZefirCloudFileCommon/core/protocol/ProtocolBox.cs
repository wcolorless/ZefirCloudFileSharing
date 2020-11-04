using System;
using System.Collections.Generic;
using System.Text;

namespace ZefirCloudFileCommon.core.protocol
{
    public class ProtocolBox
    {
        public MessageType Type { get; set; }
        public string FileOrDirName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public byte[] FileData { get; set; }
    }
}
