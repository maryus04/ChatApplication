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
        public MainWindow() {
            InitializeComponent();
            this.Content = new LoginUC();
        }
        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            Player.GetInstance().CloseConnection();
        }
    }
}