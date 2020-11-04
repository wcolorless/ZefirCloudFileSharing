using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace ZefirCloudFileServerLib.core.users
{
    public class UserStorage
    {
        private List<User> _users = new List<User>();

        public UserStorage()
        {
            Init();
        }

        public void Init()
        {
            try
            {
                var path = $"{Directory.GetCurrentDirectory()}/Users.json";
                if (File.Exists(path))
                {
                    var json= File.ReadAllText(path);
                    _users = JsonConvert.DeserializeObject<List<User>>(json);
                }
                else
                {
                    _users.Add(new User
                    {
                        Login = "test",
                        Password = "test",
                        DiskLimit = 31457280
                    });
                    var json = JsonConvert.SerializeObject(_users);
                    File.WriteAllText(path, json);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"UserStorage Init Error: {e.Message}");
            }
        }
        private void UpdateUsersFile()
        {
            try
            {
                var path = $"{Directory.GetCurrentDirectory()}/Users.json";
                var json = JsonConvert.SerializeObject(_users);
                File.WriteAllText(path, json);
            }
            catch (Exception e)
            {
                throw new Exception($"UserStorage UpdateUsersFile Error: {e.Message}");
            }
        }

        public void AddUser(string login, string password, long diskLimit)
        {
            var user = _users.First(x => x.Login == login);
            if (user != null) return;
            _users.Add(new User
            {
                Login = login,
                Password = password,
                DiskLimit = diskLimit
            });
            UpdateUsersFile();
        }

        public void ChangeUser(string login, string newPassword, long diskLimit)
        {
            var user = _users.First(x => x.Login == login);
            if (user == null) return;
            user.Password = newPassword;
            user.DiskLimit = diskLimit;
            UpdateUsersFile();
        }

        public void RemoveUser(string login)
        {
            var user = _users.First(x => x.Login == login);
            _users.Remove(user);
        }
        
        public bool CheckUser(string login, string password)
        {
            var user = _users.First(x => x.Login == login);
            if (user == null) return false;
            return user.Login == login && user.Password == password;
        }
    }
}
