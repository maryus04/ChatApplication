using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Shapes;

namespace ChatApplication {
    class MessageReader {

        private static string _message, _method;

        public static void ReadMessages() {
            while (true) {
                string entireMessage = Player.getInstance().ReadLine();
                if (entireMessage == null || entireMessage.Length == 0) {
                    break;
                }
                SetMethodMessage(entireMessage);

                switch (_method) {
                    case "ConnectionAccepted:":
                        Player.getInstance().Name = _message;
                        Player.getInstance().Connected = true;
                        MainWindow.getInstance().SetAvaibility();
                        break;
                    case "NickNameInUse:":
                        MainWindow.getInstance().SetError("Nickname already in use");
                        break;
                    case "MainWindowMessage:":
                        string name = MessageParser.GetNick(_message);
                        _message = MessageParser.RemoveNickFrom(_message);
                        MainWindow.getInstance().AppendText(name + _message + "\r\n");
                        break;
                    case "MainWindowServerMessage:":
                        if (MainWindow.getInstance() != null) {
                            MainWindow.getInstance().AppendText(_message + "\r\n");
                        }
                        break;
                    case "Players:":
                        MainWindow.getInstance().SetPlayerList(_message.Split(','));
                        break;
                    case "PlayerReady:":
                        MainWindow.getInstance().UpdatePlayerStatus(MessageParser.GetName(_message), MessageParser.GetValue(_message));
                        break;
                }
            }
        }

        private static void SetMethodMessage(string message) {
            if (message.Contains(":")) {
                _method = message.Substring(0, message.IndexOf(":") + 1);
                _message = message.Replace(_method, "");
            }
        }
    }
}