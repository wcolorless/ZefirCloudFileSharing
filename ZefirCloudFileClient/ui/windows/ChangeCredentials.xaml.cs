using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ZefirCloudFileCommon.core;

namespace ZefirCloudFileClient.ui.windows
{
    /// <summary>
    /// Логика взаимодействия для ChangeCredentials.xaml
    /// </summary>
    public partial class ChangeCredentials : Window
    {
        private ChangeCredVM _changeCredVm;
        public ChangeCredentials()
        {
            InitializeComponent();
            DataContext = _changeCredVm = ChangeCredVM.Create();
        }

        private void ChangeCredBtn(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_changeCredVm.Host) && !string.IsNullOrEmpty(_changeCredVm.Login) && !string.IsNullOrEmpty(_changeCredVm.Password))
            {
                _changeCredVm.UpdateCredentials();
                Close();
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
