using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
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
using NAudio.Wave;

namespace ChatApplication.UserControls {
    /// <summary>
    /// Interaction logic for LoginUC.xaml
    /// </summary>
    public partial class LoginUC : UserControl {
        private static LoginUC instance;

        private ObservableCollection<string> serversIp = new ObservableCollection<string>();
        private ObservableCollection<string> waveInCollection = new ObservableCollection<string>();

        public LoginUC() {
            instance = this;
            InitializeComponent();
            ipList.ItemsSource = serversIp;
            waveInList.ItemsSource = waveInCollection;
            InitializeWaveIn();
        }

        public static LoginUC GetInstance() {
            return instance;
        }

        private void InitializeWaveIn() {
            for (int n = 0; n < WaveIn.DeviceCount; n++) {
                var capabilities = WaveIn.GetCapabilities(n);
                waveInCollection.Add(capabilities.ProductName);
            }
        }

        public void AddServerIp(string ip) {
            serversIp.Add(ip);
        }

        private void Login_Click(object sender, RoutedEventArgs e) {
            Player.GetInstance().InitConnection(serverIp.Text);
            Player.GetInstance().SendNickName(PlayerNameTB.Text);
            this.Content = new ChatUC();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            serversIp.Clear();
            ServerFinder.RefreshServerList();
        }

        private void waveInList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            Player.GetInstance().DeviceInNumber = waveInList.SelectedIndex;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            Player.GetInstance().CloseConnection();
        }

        //public void SetError(string error) {
        //    this.Dispatcher.Invoke((Action)(() => { ErrorLabel.Content = error; ; }));
        //}
    }
}
