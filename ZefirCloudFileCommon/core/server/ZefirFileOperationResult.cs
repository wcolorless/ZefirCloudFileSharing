using System;
using System.Collections.Generic;
using System.Text;
using ZefirCloudFileCommon.core.file;

namespace ZefirCloudFileCommon.core.server
{
    public class ZefirFileOperationResult
    {
        public bool IsComplete { get; set; }
        public string Message { get; set; }
        public ZefirFile File { get; set; }
    }
}
