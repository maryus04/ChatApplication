using System.IO;
using System.Net;
using System;
using System.Threading;
using Chat = System.Net;
using System.Collections;
using System.Net.Sockets;

namespace ChatServer {
    public class Server {
        TcpListener _chatServer;
        public static Hashtable _nickName;

        public Server() {
            _nickName = new Hashtable(100);
            _chatServer = new TcpListener(IPAddress.Any, 4296);
            _chatServer.Start();
            ConsoleManager.Server("Server started on " + Environment.MachineName);

            while (true) {
                if (_chatServer.Pending()) {
                    TcpClient connection = _chatServer.AcceptTcpClient();
                    ConsoleManager.Server("New client is pending...");

                    Thread startCommunication = new Thread(() => newCommunication(connection));
                    startCommunication.Name = "StartConnetion";
                    startCommunication.Start();
                }
            }
        }

        public static void SendServerMessageExcept(ServerClient sendingClient, string message) {
            foreach (ServerClient client in _nickName.Values) {
                if (client == sendingClient) {
                    continue;
                }
                client.WriteLine(message);
            }
        }

        public static void SendServerToAll(string message) {
            foreach (ServerClient client in _nickName.Values) {
                client.WriteLine(message);
            }
        }

        public static void SendPlayerToAll(ServerClient sendingClient, string message) {
            foreach (ServerClient client in _nickName.Values) {
                client.WriteLine(message + "NICK:" + sendingClient.NickName + "ENDNICK");
            }
        }

        public static void SendPlayerNames() {
            string names = "";
            foreach (ServerClient client in _nickName.Values) {
                names += client.NickName + ",";
            }
            if (names.Equals("")) {
                return;
            }
            names = names.Remove(names.Length - 1);
            foreach (ServerClient client in _nickName.Values) {
                client.WriteLine("Players:" + names);
            }
        }

        private void newCommunication(TcpClient connection) {
            new Communication(connection);
        }

        internal static void Win() {
            ConsoleManager.Server("Game finished");
            foreach (ServerClient client in _nickName.Values) {
                client.WriteLine("GameFinished:");
            }
        }
    }
}