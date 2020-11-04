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
    public partial class CreateDirWindow : Window
    {
        public CreateDirWindow()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(NewDirNameBox.Text))
            {
                DialogResult = true;
                DataContext = NewDirNameBox.Text;
                Close();
            }
        }
    }
}
