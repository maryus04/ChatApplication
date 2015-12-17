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
using System.Windows.Navigation;
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
            chatTextBox.IsDocumentEnabled = true;
            playerList.ItemsSource = playerListCollection;
        }

        public static ChatUC GetInstance() {
            return instance;
        }

        private void send_KeyDown(object sender, KeyEventArgs e) {
            if (send.Text == "" || e.Key != Key.Enter) return;
            Player.GetInstance().WriteLine("MainWindowMessage:" + "[col]" + myColor + "[/col]" + "[fon]" + myFont + "[/fon]" + send.Text);
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

        public string GetText() {
            return new TextRange(chatTextBox.Document.ContentStart, chatTextBox.Document.ContentEnd).Text;
        }

        private Paragraph CreateParagraph(string message) {
            Paragraph para = new Paragraph();
            para.BorderBrush = Brushes.Red;
            var emoticons = emoticonMap.Keys.Cast<string>();
            var color = Brushes.Red;
            para.Padding = new Thickness(0);
            var fontFamilyConvertor = new FontFamilyConverter();
            var font = fontFamilyConvertor.ConvertFromString("Segoe UI") as FontFamily;

            if (message.Contains("[col]")) {
                var colorCode = message.Substring(message.IndexOf("[col]") + 5, message.IndexOf("[/col]") - (message.IndexOf("[col]") + 5));
                message = message.Replace("[col]" + colorCode + "[/col]", "");
                color = GetColor(colorCode);

                var fontCode = message.Substring(message.IndexOf("[fon]") + 5, message.IndexOf("[/fon]") - (message.IndexOf("[fon]") + 5));
                message = message.Replace("[fon]" + fontCode + "[/fon]", "");
                font = fontFamilyConvertor.ConvertFromString(fontCode) as FontFamily;

                para.BorderBrush = color;
                para.FontFamily = font;

                var name = message.TakeWhile(elem => !(elem.Equals(':')));
                var parsedStringName = new string(name.ToArray());
                var regex = new Regex(parsedStringName);
                message = regex.Replace(message, "", 1);
                parsedStringName += message[0];
                message = message.Substring(1);
                para.AddText(parsedStringName, Brushes.Black);
            }
            var emoteExists = emoticons.Any(emote => message.Contains(emote));
            while (emoteExists || message.ToLower().Contains("https") || message.ToLower().Contains("http") || message.ToLower().Contains("www")) {
                message = message.Trim();
                if ("".Equals(message)) break;
                if (message.ToLower().StartsWith("https") || message.ToLower().StartsWith("http") || message.ToLower().StartsWith("www")) {
                    Hyperlink link = new Hyperlink();
                    link.IsEnabled = true;
                    link.Inlines.Add(" (Click) ");
                    var startIndex = message.IndexOf("https:");
                    if (startIndex == -1) startIndex = message.IndexOf("http:");
                    if (startIndex == -1) startIndex = message.IndexOf("www.");
                    if (startIndex != -1) {
                        var endIndex = message.IndexOf(" ", startIndex);
                        if (endIndex == -1) endIndex = message.Count();
                        var prefix = message.Substring(startIndex, endIndex - startIndex).StartsWith("www.") ? "http://" : "";
                        link.NavigateUri = new Uri(prefix + message.Substring(startIndex, endIndex - startIndex));
                        para.Inlines.Add(link);
                        link.RequestNavigate += new RequestNavigateEventHandler(link_RequestNavigate);
                        message = message.Replace(message.Substring(startIndex, endIndex - startIndex), "");
                    }
                } else if (emoticons.Any(emote => message.Substring(0, message.IndexOf(" ") == -1 ? message.Count() : message.IndexOf(" ")).Contains(emote))) {
                    var parsedMessage = message.TakeWhile(elem => !emoteStart.Contains(elem));
                    var parsedStringMessage = new string(parsedMessage.ToArray());
                    if (!parsedStringMessage.Equals("")) {
                        var regex = new Regex(parsedStringMessage);
                        message = regex.Replace(message, "", 1);
                        para.AddText(parsedStringMessage, color);
                    }
                    if (message.Count() == 0) break;
                    var emotes = emoticons.Where(emote => message.Substring(0, message.Count() < MAX_EMOTE_LENGTH ? message.Count() : MAX_EMOTE_LENGTH).Contains(emote));
                    if (emotes.Count() > 0) {
                        para.Inlines.Add(GetImageBySymbol(emotes.Last()));
                        message = message.Substring(emotes.Last().Count());
                    } else {
                        para.AddText("" + message[0], color);
                        message = message.Substring(1);
                    }
                } else {
                    para.AddText(message.Substring(0, message.IndexOf(" ") == -1 ? message.Count() : message.IndexOf(" ")) + " ", color);
                    var regex = new Regex(message.Substring(0, message.IndexOf(" ") == -1 ? message.Count() : message.IndexOf(" ")));
                    message = regex.Replace(message, "", 1);
                }
            }
            if (!"".Equals(message)) {
                para.AddText(message, color);
            }
            return para;
        }

        private void link_RequestNavigate(object sender, RequestNavigateEventArgs e) {
            System.Diagnostics.Process.Start(e.Uri.ToString());
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
