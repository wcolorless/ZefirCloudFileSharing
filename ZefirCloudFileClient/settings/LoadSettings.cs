using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using ZefirCloudFileCommon.core;

namespace ZefirCloudFileClient.settings
{
    public class LoadSettings
    {
        public static UserCredentials UserCredentials { get; set; }

        public static UserCredentials LoadCredentials(IConfigurationRoot _configuration)
        {
            var section = _configuration.GetSection("Credentials");
            var result = new UserCredentials
            {
                Host = section["Host"],
                Login = section["Login"],
                Password = section["Password"]
            };
            UserCredentials = result;
            return result;
        }
    }
}
