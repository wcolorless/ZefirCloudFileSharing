using System;
using System.Collections.Generic;
using System.Text;

namespace ZefirCloudFileServerLib.core.users
{
    public class User
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public long DiskLimit { get; set; }
    }
}
