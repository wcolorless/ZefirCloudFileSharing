using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ZefirCloudFileClientLib.core.client.transport;
using ZefirCloudFileCommon.core;
using ZefirCloudFileCommon.core.file;
using ZefirCloudFileCommon.core.file.Info;
using ZefirCloudFileCommon.core.protocol;
using ZefirCloudFileCommon.core.server;

namespace ZefirCloudFileClientLib.core
{
    public class ZefirClient
    {
        private readonly UserCredentials _userCredentials;
        private readonly HttpTransport _httpTransport;

        private ZefirClient(UserCredentials credentials)
        {
            _userCredentials = credentials;
            _httpTransport = HttpTransport.Create();
        }
        public static ZefirClient Create(UserCredentials credentials)
        {
            return new ZefirClient(credentials);
        }

        public async Task<ZefirDirectoryInfo> GetFileAndDirList(string path = "root")
        {
            try
            {
                var message = MessageFactory.Create(_userCredentials.Login, _userCredentials.Password, MessageType.ReadFileList, path);
                var response = await _httpTransport.SendToServer(_userCredentials.Host, message, new byte[]{}, true);
                var result = JsonConvert.DeserializeObject<ZefirDirectoryInfo>(response.Json);
                return result;
            }
            catch (Exception e)
            {
                throw new Exception($"ZefirClient GetFileAndDirList Error: {e.Message}");
            }
        }

        public async Task<byte[]> ReadFile(string pathToFile)
        {
            try
            {
                var message = MessageFactory.Create(_userCredentials.Login, _userCredentials.Password, MessageType.ReadFile, pathToFile);
                var response = await _httpTransport.SendToServer(_userCredentials.Host, message, new byte[] { }, false);
                return response.Bytes;
            }
            catch (Exception e)
            {
                throw new Exception($"ZefirClient ReadFile Error: {e.Message}");
            }
        }

        public async Task<ZefirFileOperationResult> CreateDir(string path)
        {
            try
            {
                var message = MessageFactory.Create(_userCredentials.Login, _userCredentials.Password, MessageType.CreateDir, path);
                var response = await _httpTransport.SendToServer(_userCredentials.Host, message, new byte[]{}, true);
                return JsonConvert.DeserializeObject<ZefirFileOperationResult>(response.Json);
            }
            catch (Exception e)
            {
                throw new Exception($"ZefirClient CreateDir Error: {e.Message}");
            }
        }

        public async Task<ZefirFileOperationResult> SendFile(ZefirFile fileToSend)
        {
            try
            {
                var message = MessageFactory.Create(_userCredentials.Login, _userCredentials.Password, MessageType.SendFile, fileToSend.Directory);
                var response = await _httpTransport.SendToServer(_userCredentials.Host, message, fileToSend.FileData, true);
                return JsonConvert.DeserializeObject<ZefirFileOperationResult>(response.Json);
            }
            catch (Exception e)
            {
                throw new Exception($"ZefirClient SendFile Error: {e.Message}");
            }
        }

        public async Task<ZefirFileOperationResult> RemoveFile(string pathToFile)
        {
            try
            {
                var message = MessageFactory.Create(_userCredentials.Login, _userCredentials.Password, MessageType.RemoveFile, pathToFile);
                var response = await _httpTransport.SendToServer(_userCredentials.Host, message, new byte[]{}, true);
                return JsonConvert.DeserializeObject<ZefirFileOperationResult>(response.Json);
            }
            catch (Exception e)
            {
                throw new Exception($"ZefirClient RemoveFile Error: {e.Message}");
            }
        }

        public async Task<ZefirFileOperationResult> RemoveDir(string dirPath)
        {
            try
            {
                var message = MessageFactory.Create(_userCredentials.Login, _userCredentials.Password, MessageType.RemoveDir, dirPath);
                var response = await _httpTransport.SendToServer(_userCredentials.Host, message, new byte[] { }, true);
                return JsonConvert.DeserializeObject<ZefirFileOperationResult>(response.Json);
            }
            catch (Exception e)
            {
                throw new Exception($"ZefirClient RemoveDir Error: {e.Message}");
            }
        }
    }
}
