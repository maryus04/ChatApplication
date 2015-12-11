using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using NAudio.Wave;

namespace ChatApplication.UserControls {
    public partial class OptionsUC : UserControl {
        private ObservableCollection<string> waveInCollection=new ObservableCollection<string> ();

        public OptionsUC () {
            InitializeComponent ();
            waveInList.ItemsSource = waveInCollection;
            InitializeWaveIn ();
        }

        private void InitializeWaveIn () {
            for (int n=0; n<WaveIn.DeviceCount; n++) {
                var capabilities=WaveIn.GetCapabilities (n);
                waveInCollection.Add (capabilities.ProductName);
            }
        }

        private void waveInList_SelectionChanged (object sender, SelectionChangedEventArgs e) {
            Player.GetInstance().DeviceInNumber = waveInList.SelectedIndex;
        }

        private void Button_Click (object sender, RoutedEventArgs e) {
            this.Content= new LoginUC();
        }

        private void Button_Click_1 (object sender, RoutedEventArgs e) {
            this.Content=new LoginUC ();
        }
    }
}
