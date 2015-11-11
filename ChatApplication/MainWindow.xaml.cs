using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using ChatApplication.UserControls;

namespace ChatApplication {
    public partial class MainWindow : Window {

        private static MainWindow instance;
        public MainWindow() {
            instance = this;
            InitializeComponent();
            var userControl=new LoginUC();
            this.Content=userControl;
        }
        
        private void Exit(object sender, RoutedEventArgs e) {
            this.Close();
        }

        public static MainWindow getInstance() {
            return instance;
        }
        
        private void SendMessage(object sender, RoutedEventArgs e) {
            if (textName.Text != "") {
                Player.GetInstance().WriteLine("MainWindowMessage:" + textName.Text);
            }
            textName.Text = "";
        }
        
        public void AppendText(string message) {
            this.Dispatcher.Invoke((Action)(() => { textBox.Text += message; textBox.ScrollToEnd(); }));
        }

        private void textName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                SendMessage(null, null);
            }
        }

        public void SetPlayerList(string[] message) {
            this.Dispatcher.Invoke((Action)(() => {
                playersList.Items.Clear();
                foreach (string name in message) {
                    ListBoxItem item = new ListBoxItem();
                    item.Content = name;
                    playersList.Items.Add(item);
                }
            }));
        }

        public void UpdatePlayerStatus(string playerName, string status) {
            this.Dispatcher.Invoke((Action)(() => {
                ListBoxItem item = new ListBoxItem();
                item.Content = playerName + status;
                int index = -1;
                foreach (ListBoxItem curItem in playersList.Items) {
                    string itemContent = curItem.Content.ToString();
                    string[] name = itemContent.Split('-');
                    if (name[0].Equals(playerName)) {
                        index = playersList.Items.IndexOf(curItem);
                    }
                }
                if (index != -1) {
                    playersList.Items.RemoveAt(index);
                    playersList.Items.Insert(index, item);
                }
            }));
        }
    }
}