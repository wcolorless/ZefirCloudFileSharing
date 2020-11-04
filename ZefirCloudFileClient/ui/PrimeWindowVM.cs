using System;
using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading;
using System.Windows.Controls;
using Microsoft.Win32;
using ZefirCloudFileClient.ui.windows;
using ZefirCloudFileClientLib.core;
using ZefirCloudFileCommon.core;
using ZefirCloudFileCommon.core.file;

namespace ZefirCloudFileClient.ui
{
    public class PrimeWindowVM : INotifyPropertyChanged
    {
        private Window _primeWindow;
        private Grid _picContainer;

        private UserCredentials _userCredentials;
        private ZefirClient _zefirClient;

        private string _CurrentDir = "root";

        public string CurrentDir
        {
            get => _CurrentDir;
            set
            {
                _CurrentDir = value;
                NotifyPropertyChanged("CurrentDir");
            }
        }

        private Command _ChangeCredentialsCommand;
        private Command _SendFileCommand;
        private Command _UpdateFileListCommand;
        private Command _UpdateCurrentFileListCommand;
        private Command _GoBackCommand;
        private Command _RemoveFileCommand;
        private Command _DownloadFileCommand;
        private Command _RemoveDirCommand;
        private Command _OpenDirCommand;
        private Command _CreateDirCommand;

        public Command ChangeCredentialsCommand
        {
            get
            {
                return _ChangeCredentialsCommand ??= new Command(async o =>
                {
                    try
                    {
                        var changeWindow = new ChangeCredentials() {Owner = _primeWindow};
                        changeWindow.ShowDialog();
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"PrimeWindowVM ChangeCredentialsCommand Error: {e.Message}");
                    }
                });
            }
        }

        public Command SendFileCommand
        {
            get
            {
                return _SendFileCommand ??= new Command(async o =>
                {
                    try
                    {
                        var uploadFileWindow = new UploadWindow() { Owner = _primeWindow };
                        if (uploadFileWindow.ShowDialog() == true)
                        {
                            var newFile = uploadFileWindow.DataContext as ZefirFile;
                            if (newFile != null)
                            {
                                newFile.Directory = $"{Path.GetRelativePath("root/", CurrentDir)}/{newFile.FileName}";
                                var result = await _zefirClient.SendFile(newFile);
                                UpdateCurrentDir();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"PrimeWindowVM SendFileCommand Error: {e.Message}");
                    }
                });
            }
        }

        public Command UpdateFileListCommand
        {
            get
            {
                return _UpdateFileListCommand ??= new Command(async o =>
                {
                    try
                    {
                        CurrentDir = "root";
                        var result = await _zefirClient.GetFileAndDirList();
                        if (result != null)
                        {
                            PicDrawer.Draw(this, result, _picContainer);
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"PrimeWindowVM UpdateFileListCommand Error: {e.Message}");
                    }
                });
            }
        }

        public Command UpdateCurrentFileListCommand
        {
            get
            {
                return _UpdateCurrentFileListCommand ??= new Command(async o =>
                {
                    UpdateCurrentDir();
                });
            }
        }

        private async void UpdateCurrentDir()
        {
            try
            {
                var path = Path.GetRelativePath("root/", CurrentDir);
                var result = await _zefirClient.GetFileAndDirList(path);
                if (result != null)
                {
                    PicDrawer.Draw(this, result, _picContainer);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"PrimeWindowVM UpdateCurrentDir Error: {e.Message}");
            }
        }

        public Command GoBackCommand
        {
            get
            {
                return _GoBackCommand ??= new Command(async o =>
                {
                    try
                    {
                        var delIndex = _CurrentDir.LastIndexOf("/");
                        if (delIndex != -1)
                        {
                            CurrentDir = _CurrentDir.Substring(0, delIndex);
                            var path = Path.GetRelativePath("root/", _CurrentDir);
                            var result = await _zefirClient.GetFileAndDirList(path);
                            if (result != null)
                            {
                                PicDrawer.Draw(this, result, _picContainer);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"PrimeWindowVM UpdateCurrentDir Error: {e.Message}");
                    }
                });
            }
        }

        public Command OpenDirCommand
        {
            get
            {
                return _OpenDirCommand ??= new Command(async obj =>
                {
                    try
                    {
                        var info = obj as PicInfo;
                        var dirInfo = info.DirInfo;
                        var path = $"{Path.GetRelativePath(_userCredentials.Login, dirInfo.Name)}".Replace("\\", "/");
                        var result = await _zefirClient.GetFileAndDirList(path);
                        if (result != null)
                        {
                            PicDrawer.Draw(this, result, _picContainer);
                            CurrentDir = $"root/{path}";
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"PrimeWindowVM OpenDirCommand Error: {e.Message}");
                    }
                });
            }
        }

        public Command CreateDirCommand
        {
            get
            {
                return _CreateDirCommand ??= new Command(async obj =>
                {
                    try
                    {
                        var newDirWindow = new CreateDirWindow() { Owner = _primeWindow};
                        if (newDirWindow.ShowDialog() == true)
                        {
                            var newDirName = newDirWindow.DataContext as string;
                            var path = ($"{CurrentDir}/{newDirName}").Replace("root/", "");
                            await _zefirClient.CreateDir(path);
                            var result = await _zefirClient.GetFileAndDirList(Path.GetRelativePath("root/", CurrentDir));
                            if (result != null)
                            {
                                PicDrawer.Draw(this, result, _picContainer);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"PrimeWindowVM CreateDirCommand Error: {e.Message}");
                    }
                });
            }
        }

        public Command RemoveFileCommand
        {
            get
            {
                return _RemoveFileCommand ??= new Command(async obj =>
                {
                    try
                    {
                        var info = obj as PicInfo;
                        var fileInfo = info.FileInfo;
                        var path = $"{fileInfo.Directory}";
                        var result = await _zefirClient.RemoveFile(path);
                        if (result.IsComplete && result.Message == "File Removed")
                        {
                            info.Container.Children.Remove(info.Rectangle);
                            info.Container.Children.Remove(info.Title);
                            info.Source.Files.RemoveAll(x => x.FileName == info.FileInfo.FileName);
                            PicDrawer.Draw(this, info.Source, _picContainer);
                        }
                    }
                    catch (Exception exception)
                    {
                        throw new Exception($"PrimeWindowVM RemoveFileCommand Error: {exception.Message}");
                    }
                });
            }
        }

        public Command RemoveDirCommand
        {
            get
            {
                return _RemoveDirCommand ??= new Command(async obj =>
                {
                    try
                    {
                        var info = obj as PicInfo;
                        var dirInfo = info.DirInfo;
                        var path = $"{dirInfo.Name}";
                        if (MessageBox.Show("Are you sure?", "You delete the folder", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            var result = await _zefirClient.RemoveDir(path);
                            if (result.IsComplete && result.Message == "Dir Removed")
                            {
                                info.Container.Children.Remove(info.Rectangle);
                                info.Container.Children.Remove(info.Title);
                                info.Source.Directories.RemoveAll(x => x.Name == info.DirInfo.Name);
                                PicDrawer.Draw(this, info.Source, _picContainer);
                            }
                        }
                        
                    }
                    catch (Exception exception)
                    {
                        throw new Exception($"PrimeWindowVM RemoveDirCommand Error: {exception.Message}");
                    }
                });
            }
        }

        public Command DownloadFileCommand
        {
            get
            {
                return _DownloadFileCommand ??= new Command(async obj =>
                {
                    try
                    {
                        var info = obj as PicInfo;
                        var fileInfo = info.FileInfo;
                        var sfd = new SaveFileDialog() { FileName = fileInfo.FileName, Filter = $"{fileInfo.Extension.Replace(".", "*.")}|{fileInfo.Extension.Replace(".", "*.")}" };
                        if (sfd.ShowDialog() == true)
                        {
                            var path = $"{fileInfo.Directory}";
                            var result = await _zefirClient.ReadFile(path);
                            using (var fs = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write))
                            {
                                fs.Write(result, 0, result.Length);
                                fs.Close();
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        throw new Exception($"PrimeWindowVM DownloadFileCommand Error: {exception.Message}");
                    }
                });
            }
        }

        public PrimeWindowVM(Window primeWindow, UserCredentials userCredentials, Grid picContainer)
        {
            _primeWindow = primeWindow;
            _userCredentials = userCredentials;
            _picContainer = picContainer;
            _zefirClient = ZefirClient.Create(userCredentials);
        }

        public static PrimeWindowVM Create(Window primeWindow, UserCredentials userCredentials, Grid picContainer)
        {
            return new PrimeWindowVM(primeWindow, userCredentials, picContainer);
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
