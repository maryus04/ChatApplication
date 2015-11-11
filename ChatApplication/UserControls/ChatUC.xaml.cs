using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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

namespace ChatApplication.UserControls {
    /// <summary>
    /// Interaction logic for ChatUC.xaml
    /// </summary>
    public partial class ChatUC : UserControl {
        private static ChatUC instance;

        private ObservableCollection<string> playerListCollection = new ObservableCollection<string>();

        public ChatUC() {
            instance = this;
            InitializeComponent();
            playerList.ItemsSource = playerListCollection;
        }

        public static ChatUC GetInstance() {
            return instance;
        }

        public void SetNickNames(params string[] nickNames) {
            this.Dispatcher.Invoke((Action)(() => {
                playerListCollection.Clear();
                foreach (var nickName in nickNames) { playerListCollection.Add(nickName); }
            }));
        }
    }
}
