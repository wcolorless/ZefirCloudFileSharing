using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ZefirCloudFileServerLib.core.server.dispatcher
{
    public class PathChecker
    {
        public static void Check(string loginName)
        {
            try
            {
                var filePath = $"{Directory.GetCurrentDirectory()}/Files";
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                var userStorageDir = $"{Directory.GetCurrentDirectory()}/Files/{loginName}";
                if (!Directory.Exists(userStorageDir))
                {
                    Directory.CreateDirectory(userStorageDir);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"PathChecker Check Error: {e.Message}");
            }
        }
    }
}
