using System;
using System.Collections.Generic;
using System.Text;

namespace ZefirCloudFileCommon.core.protocol
{
    // Protocol operations
    public enum MessageType
    {
        None,
        SendFile,
        ReadFile,
        ReadFileList,
        CreateDir,
        RemoveFile,
        RemoveDir
    }
}
