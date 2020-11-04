using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using ZefirCloudFileServer.settings;
using ZefirCloudFileServerLib.core;
using ZefirCloudFileServerLib.core.settings;

namespace ZefirCloudFileServer
{
    class Program
    {
        private static ZefirServerSettings _zefirServerSettings;
        private static ZefirServer _zefirServer;
        private static IConfigurationRoot _configurationRoot;
        static void Main(string[] args)
        {
            Console.WriteLine("Starting server...");
            Init();
            _zefirServer = new ZefirServer(_zefirServerSettings);
            _zefirServer.Start();
        }

        private static void Init()
        {
            //configuration
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "dev";

            _configurationRoot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            // Load settings
            _zefirServerSettings = SettingsLoader.Load(_configurationRoot);
        }

    }
}
