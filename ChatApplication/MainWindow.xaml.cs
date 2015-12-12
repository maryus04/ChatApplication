using System.Windows;
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