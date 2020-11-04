using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using ZefirCloudFileCommon.core;
using ZefirCloudFileServerLib.core.settings;

namespace ZefirCloudFileServer.settings
{
    public class SettingsLoader
    {
        public static ZefirServerSettings Load(IConfigurationRoot _configuration)
        {
            var section = _configuration.GetSection("ZefirServerSettings");
            var result = new ZefirServerSettings
            {
                Host = section["Host"],
                DiskSize = long.Parse(section["DiskSize"])
            };
            return result;
        }
    }
}
