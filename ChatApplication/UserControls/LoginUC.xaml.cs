using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

        public LoginUC() {
            instance = this;
            InitializeComponent();
            ipList.ItemsSource = serversIp;

        }

        public static LoginUC GetInstance() {
            return instance;
        }

        public void AddServerIp(string ip) {
            serversIp.Add(ip);
        }

        private void Login_Click(object sender, RoutedEventArgs e) {
            Player.GetInstance().InitConnection(serverIp.Text);
            Player.GetInstance().SendNickName(PlayerNameTB.Text);
        }

        public void StartChat() {
            this.Dispatcher.Invoke((Action)(() => { this.Content = new ChatUC(); }));
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            serversIp.Clear();
            ServerFinder.RefreshServerList();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            Player.GetInstance().CloseConnection();
        }

        private void Button_Click_1 (object sender, RoutedEventArgs e) {
            this.Content=new OptionsUC ();
        }

        //public void SetError(string error) {
        //    this.Dispatcher.Invoke((Action)(() => { ErrorLabel.Content = error; ; }));
        //}
    }
}
