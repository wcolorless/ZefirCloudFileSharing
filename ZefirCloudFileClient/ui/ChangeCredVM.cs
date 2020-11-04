using System;
using System.Windows;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Timers;
using System.Windows.Threading;
using ZefirCloudFileClient.ui.windows;
using ZefirCloudFileClientLib.core;
using ZefirCloudFileCommon.core;
using Timer = System.Timers.Timer;
using Newtonsoft.Json;
using ZefirCloudFileClient.settings;

namespace ZefirCloudFileClient.ui
{
    public class ChangeCredVM : INotifyPropertyChanged
    {
        public string Host
        {
            get => LoadSettings.UserCredentials.Host;
            set
            {
                LoadSettings.UserCredentials.Host = value;
                NotifyPropertyChanged("Host");
            }
        }
        public string Login 
        {
            get => LoadSettings.UserCredentials.Login;
            set
            {
                LoadSettings.UserCredentials.Login = value;
                NotifyPropertyChanged("Login");
            }
        }

        public string Password
        {
            get => LoadSettings.UserCredentials.Password;
            set
            {
                LoadSettings.UserCredentials.Password = value;
                NotifyPropertyChanged("Password");
            }
        }

        public ChangeCredVM()
        {

        }

        public void UpdateCredentials()
        {
            var newSettings = new Settings()
            {
                Credentials = new UserCredentials()
                {
                    Host = LoadSettings.UserCredentials.Host,
                    Login = LoadSettings.UserCredentials.Login,
                    Password = LoadSettings.UserCredentials.Password,
                }
            };
            var json = JsonConvert.SerializeObject(newSettings);
            File.WriteAllText(Directory.GetCurrentDirectory() + "/appsettings.json", json);
        }

        public static ChangeCredVM Create()
        {
            return new ChangeCredVM();
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
