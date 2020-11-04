using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace ZefirCloudFileCommon.core.protocol
{
    public class MessageFactory
    {
        /// <summary>
        /// Creates a protocol exchange model
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <param name="type"></param>
        /// <param name="fileOrDirName"></param>
        /// <returns></returns>
        public static string Create(string login, string password, MessageType type, string fileOrDirName)
        {
            if(string.IsNullOrEmpty(login)) throw new Exception("MessageFactory Create Error: Login is null or empty!");
            if (string.IsNullOrEmpty(password)) throw new Exception("MessageFactory Create Error: Password is null or empty!");
            var box = new ProtocolBox
            {
                Login = login,
                Password = password,
                Type = type,
                FileOrDirName = Convert.ToBase64String(Encoding.UTF8.GetBytes(fileOrDirName)),
                FileData = new byte[]{}
            };
            var json = JsonConvert.SerializeObject(box);
            return json;
        }
    }
}
