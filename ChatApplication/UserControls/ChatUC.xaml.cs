using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ChatApplication.Utils;

namespace ChatApplication.UserControls {
    /// <summary>
    /// Interaction logic for ChatUC.xaml
    /// </summary>
    public partial class ChatUC : UserControl {
        private static ChatUC instance;
        int MAX_EMOTE_LENGTH = 3;
        char[] emoteStart = { ':', '>' };
        Hashtable emoticonMap = new Hashtable() {
            {":)", @"\Emoticons\smiley.jpg"},
            {":D", @"\Emoticons\d.jpg"},
            {":(", @"\Emoticons\sad.jpg"},
            {":|", @"\Emoticons\straightFace.jpg"},
            {":))", @"\Emoticons\veryHappy.jpg"},
            {":((", @"\Emoticons\verySad.jpg"}
        };
        string myColor;
        string myFont;

        private ObservableCollection<string> playerListCollection = new ObservableCollection<string>();

        public ChatUC() {
            instance = this;
            InitializeComponent();
            playerList.ItemsSource = playerListCollection;
        }

        public static ChatUC GetInstance() {
            return instance;
        }

        private void send_KeyDown(object sender, KeyEventArgs e) {
            if (send.Text == "" || e.Key != Key.Enter) return;
            Player.GetInstance().WriteLine("MainWindowMessage:" + "[col]" + myColor + "[/col]" + send.Text);
            send.Text = "";
        }

        public void SetNickNames(params string[] nickNames) {
            this.Dispatcher.Invoke((Action)(() => {
                playerListCollection.Clear();
                foreach (var nickName in nickNames) { playerListCollection.Add(nickName); }
            }));
        }

        public void AppendText(string message) {
            this.Dispatcher.Invoke((Action)(() => {
                var para = CreateParagraph(message);
                chatTextBox.Document.Blocks.Add(para);
                chatTextBox.ScrollToEnd();
            }));
        }

        private Paragraph CreateParagraph(string message) {
            Paragraph para = new Paragraph();
            para.BorderBrush = Brushes.Red;
            para.BorderThickness = new Thickness(1);
            var emoticons = emoticonMap.Keys.Cast<string>();
            var emoteExists = emoticons.Any(emote => message.Contains(emote));
            var color = Brushes.Red;

            if (message.Contains("[col]")) {
                var colorCode = message.Substring(message.IndexOf("[col]") + 5, message.IndexOf("[/col]") - (message.IndexOf("[col]") + 5));
                color = GetColor(colorCode);
                para.BorderBrush = color;

                var name = message.TakeWhile(elem => !(elem.Equals(':')));
                var parsedStringName = new string(name.ToArray());
                var regex = new Regex(parsedStringName);
                message = regex.Replace(message, "", 1);
                parsedStringName += message[0];
                parsedStringName += "\n\t";
                message = message.Substring(1);
                para.AddText(parsedStringName, Brushes.Black);
            }
            if (!emoteExists) {
                para.AddText(message, color);
                return para;
            } else {
                while (!message.Equals("")) {
                    var parsedMessage = message.TakeWhile(elem => !emoteStart.Contains(elem));
                    var parsedStringMessage = new string(parsedMessage.ToArray());
                    if (!parsedStringMessage.Equals("")) {
                        var regex = new Regex(parsedStringMessage);
                        message = regex.Replace(message, "", 1);
                        para.AddText(parsedStringMessage, color);
                    }
                    if (message.Count() == 0) break;
                    var emotes = emoticons.Where(emote => message.Substring(0, MAX_EMOTE_LENGTH).Contains(emote));
                    if (emotes.Count() > 0) {
                        para.Inlines.Add(GetImageBySymbol(emotes.First()));
                        message = message.Substring(emotes.First().Count());
                    } else {
                        para.AddText("" + message[0], color);
                        message = message.Substring(1);
                    }
                }
                return para;
            }
        }

        private SolidColorBrush GetColor(string colorCode) {
            switch (colorCode.ToLower()) {
                case "red":
                    return Brushes.Red;
                case "blue":
                    return Brushes.Blue;
                case "black":
                    return Brushes.Black;
                default:
                    return Brushes.Black;
            }
        }

        private Image GetImageBySymbol(string symbol) {
            if (!emoticonMap.Contains(symbol)) return null;
            return GetImage(emoticonMap[symbol].ToString());
        }

        private Image GetImage(string url) {
            Image image = new Image();
            image.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + url));
            image.Width = 20;
            image.Height = 20;
            return image;
        }

        public void SetMicButtonText(string text) {
            this.Dispatcher.Invoke((Action)(() => { toggleMicButton.Content = text; }));
        }

        private void toggleMicButton_Click(object sender, RoutedEventArgs e) {
            Player.GetInstance().ToggleMic();
        }

        private void toggleSpButton_Click(object sender, RoutedEventArgs e) {
            Player.GetInstance().ToggleSpeaker();
        }

        public void SetSpButtonText(string text) {
            this.Dispatcher.Invoke((Action)(() => { toggleSpButton.Content = text; }));
        }

        private void colorComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e) {
            ComboBoxItem colorItem = (ComboBoxItem)colorComboBox.SelectedItem;
            string color = colorItem.Content.ToString();
            colorComboBox.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString(color);
            send.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString(color);
            myColor = color;
        }

        private void fontComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e) {
            ComboBoxItem fontItem = (ComboBoxItem)fontComboBox.SelectedItem;
            string font = fontItem.Content.ToString();
            var fontFamilyConvertor = new FontFamilyConverter();
            send.FontFamily = fontFamilyConvertor.ConvertFromString(font) as FontFamily;
            myFont = font;
        }
    }
}
