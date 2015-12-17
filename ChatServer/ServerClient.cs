using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Drawing;

namespace ChatServer {
    public class ServerClient {
        private string nickName = "NotYetSet";

        public TcpClient TcpClient { get; set; }

        public StreamReader Reader { get; set; }
        public StreamWriter Writer { get; set; }

        public string NickName {
            get { return nickName; }
            set { nickName = value; }
        }

        public NetworkStream GetStream() {
            return TcpClient.GetStream();
        }

        public void WriteLine(string message) {
            try {
                Writer.WriteLine(message);
                Writer.Flush();
            } catch (Exception) {
                ConsoleManager.Communication(NickName + " lost connection.");
                Dispose();
            }
        }

        public bool IsTcpConnected() {
            return TcpClient.Connected;
        }

        public String ReadLine() {
            try {
                return Reader.ReadLine();
            } catch (Exception) {
                Server._nickName.Remove(NickName);
                ConsoleManager.Communication(NickName + " lost connection.");
                Dispose();
            }
            return "";
        }

        public void Flush() {
            Writer.Flush();
        }

        public void Dispose() {
            Reader.Close();
            Writer.Close();
            TcpClient.Close();
        }

        public int GetPort() {
            return ((IPEndPoint)TcpClient.Client.RemoteEndPoint).Port;
        }

        public string GetIp() {
            return ((IPEndPoint)TcpClient.Client.RemoteEndPoint).Address.ToString();
        }
    }
}