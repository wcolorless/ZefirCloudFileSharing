using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using ZefirCloudFileCommon.core.file;
using Path = System.IO.Path;

namespace ZefirCloudFileClient.ui.windows
{
    /// <summary>
    /// Логика взаимодействия для UploadWindow.xaml
    /// </summary>
    public partial class UploadWindow : Window
    {
        private ZefirFile _zefirFile;
        public UploadWindow()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void SelectFileBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var opfd = new OpenFileDialog();
            if (opfd.ShowDialog() == true)
            {
                var fileName = opfd.FileName;
                PathText.Text = fileName;
                _zefirFile = new ZefirFile()
                {
                    FileName = fileName
                };
            }
        }

        private async void UploadBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (_zefirFile != null)
            {
                byte[] buffer;
                await using var fileStream = new FileStream(_zefirFile.FileName, FileMode.Open, FileAccess.Read);
                try
                {
                    var length = (int)fileStream.Length;  // get file length
                    buffer = new byte[length];            // create buffer
                    int count;                            // actual number of bytes read
                    int sum = 0;                          // total number of bytes read

                    // read until Read method returns 0 (end of the stream has been reached)
                    while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                        sum += count;  // sum is a buffer offset for next reading
                }
                finally
                {
                    fileStream.Close();
                }
                _zefirFile.FileData = buffer;
                _zefirFile.Size = buffer.Length;
                _zefirFile.CreateOrUpdateFile = DateTime.UtcNow;
                _zefirFile.Directory = "";
                _zefirFile.Extension = Path.GetExtension(_zefirFile.FileName);
                _zefirFile.FileName = Path.GetFileName(_zefirFile.FileName);
                DataContext = _zefirFile;
                DialogResult = true;
                Close();
            }
        }
    }
}
