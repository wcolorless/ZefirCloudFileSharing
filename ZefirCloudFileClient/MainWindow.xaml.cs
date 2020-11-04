using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Extensions.Configuration;
using ZefirCloudFileClient.settings;
using ZefirCloudFileClient.ui;
using ZefirCloudFileCommon.core;

namespace ZefirCloudFileClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static IConfigurationRoot _configurationRoot;
        private static UserCredentials _userCredentials;
        private PrimeWindowVM _primeWindowVm;
        public MainWindow()
        {
            InitializeComponent();
            Init();
            DataContext = _primeWindowVm = PrimeWindowVM.Create(this, _userCredentials, PicContainer);
        }

        private void CloseApp(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Init()
        {
            //configuration
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "dev";

            _configurationRoot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();


            _userCredentials = LoadSettings.LoadCredentials(_configurationRoot);
        }
    }
}
