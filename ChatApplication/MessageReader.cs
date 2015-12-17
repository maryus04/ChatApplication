using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Shapes;
using ChatApplication.UserControls;

namespace ChatApplication {
    class MessageReader {

        private static string _message, _method;

        public static void ReadMessages() {
            while (true) {
                string entireMessage = Player.GetInstance().ReadLine();
                if (entireMessage == null || entireMessage.Length == 0) {
                    break;
                }
                SetMethodMessage(entireMessage);

                switch (_method) {
                    case "ConnectionAccepted:":
                        Player.GetInstance().Name = _message;
                        Player.GetInstance().Connected = true;
                        LoginUC.GetInstance().StartChat();
                        Player.GetInstance().SendChat();
                        break;
                    case "NickNameInUse:":
                        //MainWindow.getInstance().SetError("Nickname already in use"); TODO: TREAT ERROR IN LOGINUC
                        break;
                    case "MainWindowMessage:":
                        string name = MessageParser.GetNick(_message);
                        _message = MessageParser.RemoveNickFrom(_message);
                        ChatUC.GetInstance().AppendText(name + _message);
                        break;
                    case "MainWindowServerMessage:":
                        ChatUC.GetInstance().AppendText(_message);
                        break;
                    case "Players:":
                        ChatUC.GetInstance().SetNickNames(_message.Split(','));
                        break;
                    case "Sound:":
                        Player.GetInstance().PlayAudio(MessageParser.ToByteArray(_message));
                        break;
                    case "History:":
                        SetHistory(_message);
                        break;
                }
            }
        }

        private static void SetHistory(string _message) {
            ChatUC.GetInstance().AppendText(_message.Replace(@"\new_line\", "\r\n"));
        }

        private static void SetMethodMessage(string message) {
            if (message.Contains(":")) {
                _method = message.Substring(0, message.IndexOf(":") + 1);
                _message = message.Replace(_method, "");
            }
        }
    }
}