using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Net.Sockets;
using System.IO;

namespace ChatApplication {
    public class Player {
        private static Player instance;

        public string Name { get; set; }

        public bool FirstClick { get; set; }

        public TcpClient TcpClient { get; set; }
        public bool Connected { get; set; }

        public StreamWriter Writer { get; set; }
        public StreamReader Reader { get; set; }

        public Player() {
            instance = this;
        }

        public void WriteLine(string message) {
            try {
                Writer.WriteLine(message);
                Writer.Flush();
            } catch (Exception) {
                if (TcpClient.Connected) {
                    Reader.Close();
                    Writer.Close();
                    TcpClient.Close();
                }
            }
        }

        public string ReadLine() {
            try {
                return Reader.ReadLine();
            } catch (Exception) {
                if (TcpClient.Connected) {
                    Reader.Close();
                    Writer.Close();
                    TcpClient.Close();
                }
            }
            return "";
        }

        public void CloseConnection() {
            if (TcpClient != null && TcpClient.Connected) {
                Connected = false;
                WriteLine("CloseConnection:");
                Reader.Close();
                Writer.Close();
                TcpClient.Close();
            }
        }

        public void InitializeStream() {
            Writer = new StreamWriter(TcpClient.GetStream());
            Reader = new StreamReader(TcpClient.GetStream());
        }

        public static Player getInstance() {
            if (instance == null) {
                instance = new Player();
            }
            return instance;
        }

    }
}