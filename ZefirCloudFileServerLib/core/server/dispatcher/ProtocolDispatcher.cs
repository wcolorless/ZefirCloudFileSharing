using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ZefirCloudFileCommon.core.protocol;
using ZefirCloudFileCommon.core.server;
using ZefirCloudFileServerLib.core.file;
using ZefirCloudFileServerLib.core.server.transport;
using ZefirCloudFileServerLib.core.users;

namespace ZefirCloudFileServerLib.core.server.dispatcher
{
    public class ProtocolDispatcher
    {
        private readonly UserStorage _userStorage;
        private readonly LocalDiskSaver _localDiskSaver;

        public ProtocolDispatcher(UserStorage userStorage)
        {
            _userStorage = userStorage;
            _localDiskSaver = new LocalDiskSaver($"{Directory.GetCurrentDirectory()}/Files");
        }

        public async Task<DispatcherResult> Handle(HttpListenerRequest request, ProtocolBox messageType)
        {
            var result = new DispatcherResult();
            if (_userStorage.CheckUser(messageType.Login, messageType.Password))
            {
                try
                {
                    PathChecker.Check(messageType.Login);
                    switch (messageType.Type)
                    {
                        case MessageType.None:
                            break;
                        case MessageType.SendFile:
                            {
                                byte[] buffer;
                                var stream = request.InputStream;
                                try
                                {
                                    var length = (int)request.ContentLength64;
                                    buffer = new byte[length];
                                    int count;
                                    int sum = 0;
                                    while ((count = stream.Read(buffer, sum, length - sum)) > 0)
                                    {
                                        sum += count;
                                    }
                                }
                                finally
                                {
                                    stream.Close();
                                }

                                _localDiskSaver.SaveFile(buffer, messageType.Login, messageType.FileOrDirName);
                                result.Json = string.Empty;
                                result.IsJson = true;
                            }
                            break;
                        case MessageType.ReadFile:
                            {
                                var readFilePath = $"{Directory.GetCurrentDirectory()}/Files/{messageType.FileOrDirName}";
                                byte[] readFileBuffer;
                                using (var stream = new FileStream(readFilePath, FileMode.Open, FileAccess.Read))
                                {
                                    try
                                    {
                                        var length = (int)stream.Length;
                                        readFileBuffer = new byte[length];
                                        int count;
                                        int sum = 0;
                                        while ((count = stream.Read(readFileBuffer, sum, length - sum)) > 0)
                                        {
                                            sum += count;
                                        }
                                    }
                                    finally
                                    {
                                        stream.Close();
                                    }

                                    result.IsJson = false;
                                    result.Byte = readFileBuffer;
                                }
                            }
                            break;
                        case MessageType.ReadFileList:
                            {
                                var path = messageType.FileOrDirName == "root"
                                    ? $"{Directory.GetCurrentDirectory()}/Files/{messageType.Login}"
                                    : $"{Directory.GetCurrentDirectory()}/Files/{messageType.Login}/{messageType.FileOrDirName}/";
                                var scanResult = PathScanner.Scan(path);
                                result.Json = JsonConvert.SerializeObject(scanResult);
                                result.IsJson = true;
                            }
                            break;
                        case MessageType.CreateDir:
                            {
                                var path = messageType.FileOrDirName == "root"
                                    ? $"{Directory.GetCurrentDirectory()}/Files/{messageType.Login}"
                                    : $"{Directory.GetCurrentDirectory()}/Files/{messageType.Login}/{messageType.FileOrDirName}/";
                                result.IsJson = true;
                                var operationResultresult = new ZefirFileOperationResult() { IsComplete = false, Message = "Dir not created" };
                                if (Directory.Exists(path))
                                {
                                    operationResultresult.IsComplete = false;
                                    operationResultresult.Message = "Dir already exist";
                                }
                                else
                                {
                                    try
                                    {
                                        Directory.CreateDirectory(path);
                                        if (!Directory.Exists(path))
                                        {
                                            operationResultresult.IsComplete = true;
                                            operationResultresult.Message = "Dir was created";
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        operationResultresult.IsComplete = false;
                                        operationResultresult.Message = e.Message;
                                    }
                                }
                                result.Json = JsonConvert.SerializeObject(operationResultresult);
                            }
                            break;
                        case MessageType.RemoveFile:
                            {
                                result.IsJson = true;
                                var operationResultresult = new ZefirFileOperationResult() { IsComplete = false, Message = "File not found" };
                                var removeFilePath = $"{Directory.GetCurrentDirectory()}/Files/{messageType.FileOrDirName}";
                                if (File.Exists(removeFilePath))
                                {
                                    try
                                    {
                                        File.Delete(removeFilePath);
                                        if (!File.Exists(removeFilePath))
                                        {
                                            operationResultresult.IsComplete = true;
                                            operationResultresult.Message = "File Removed";
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        operationResultresult.IsComplete = false;
                                        operationResultresult.Message = e.Message;
                                    }
                                }
                                result.Json = JsonConvert.SerializeObject(operationResultresult);
                            }
                            break;
                        case MessageType.RemoveDir:
                            {
                                result.IsJson = true;
                                var operationResultresult = new ZefirFileOperationResult() { IsComplete = false, Message = "Dir not found" };
                                var removeDirPath = $"{Directory.GetCurrentDirectory()}/Files/{messageType.FileOrDirName}";
                                if (Directory.Exists(removeDirPath))
                                {
                                    try
                                    {
                                        Directory.Delete(removeDirPath, true);
                                        if (!Directory.Exists(removeDirPath))
                                        {
                                            operationResultresult.IsComplete = true;
                                            operationResultresult.Message = "Dir Removed";
                                        }
                                        else
                                        {
                                            operationResultresult.IsComplete = false;
                                            operationResultresult.Message = "Dir not removed";
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        operationResultresult.IsComplete = false;
                                        operationResultresult.Message = e.Message;
                                    }
                                }

                                result.Json = JsonConvert.SerializeObject(operationResultresult);
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                catch (Exception e)
                {
                    throw new Exception($"ProtocolDispatcher Handle Error: {e.Message}");
                }
            }

            return result;
        }
    }
}
