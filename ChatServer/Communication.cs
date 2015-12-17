using System.IO;
using System.Net;
using System;
using System.Threading;
using Chat = System.Net;
using System.Collections;
using System.Net.Sockets;
using System.Security.Permissions;
using System.Drawing;

namespace ChatServer {
    class Communication {
        ServerClient _client = new ServerClient();

        private static string _message, _method;

        public Communication(TcpClient tcpClient) {
            _client.TcpClient = tcpClient;

            BeginConnection();
        }

        private void BeginConnection() {
            _client.Reader = new StreamReader(_client.GetStream());
            _client.Writer = new StreamWriter(_client.GetStream());

            ReadMessages();
        }

        private void ReadMessages() {
            while (true) {
                string message = _client.ReadLine();
                if (message == null || message.Length == 0) {
                    break;
                }

                //Console.WriteLine(_client.NickName + " sent:" + message);

                SetMethodMessage(message);

                switch (_method) {
                    case "MyName:":
                        ValidateNickName(_message);
                        break;
                    case "CloseConnection:":
                        string nickName = _client.NickName;
                        CloseConnection();
                        Server.SendServerToAll("MainWindowServerMessage:** " + nickName + " left the room.");
                        Server.SendPlayerNames();
                        break;
                    case "MainWindowMessage:":
                        Server.SendPlayerToAll(_client, "MainWindowMessage:" + _message);
                        break;
                    case "Sound:":
                        Server.SendServerMessageExcept(_client, "Sound:" + _message);
                        break;
                    case "History:":
                        SetHistory(_message);
                        break;
                }
            }
        }

        private static void SetHistory(string message) {
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\history-took.hst", message);
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\history.hst")) File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\history.hst", "");
            var file = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + @"\history.hst");
            var newFile = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + @"\history-took.hst");
            if (File.ReadAllText(file.FullName).Length <= File.ReadAllText(newFile.FullName).Length) {
                File.WriteAllText(file.FullName, File.ReadAllText(newFile.FullName));
            }
            Server.SendServerToAll("History:" + File.ReadAllText(file.FullName));
        }

        private static void SetMethodMessage(string message) {
            _method = message.Substring(0, message.IndexOf(":") + 1);
            _message = message.Replace(_method, "");
        }

        private void ValidateNickName(string name) {
            if (!Server._nickName.Contains(name) && !"NotYetSet".Equals(name)) {
                _client.NickName = name;
                AcceptConnection();
                Server.SendServerToAll("MainWindowServerMessage:** " + _client.NickName + " joined the room.");
                Server.SendPlayerNames();
            } else {
                ConsoleManager.Communication("Name \"" + name + "\" already in use. Connection refused.");
                _client.WriteLine("NickNameInUse:");
            }
        }

        private void AcceptConnection() {
            Server._nickName.Add(_client.NickName, _client);
            _client.WriteLine("ConnectionAccepted:" + _client.NickName);
            ConsoleManager.Communication(_client.NickName + " is now connected to server." + " Address " + _client.GetIp() + " Port:" + _client.GetPort());
        }

        private void CloseConnection() {
            ConsoleManager.Communication(_client.NickName + " is now disconnected from the server.");
            Server._nickName.Remove(_client.NickName);
            _client.Dispose();
        }
    }
}