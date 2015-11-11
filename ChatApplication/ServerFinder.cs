using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChatApplication.UserControls;

namespace ChatApplication {
    class ServerFinder {
        private static int port = 4296;

        public static void RefreshServerList() {
            IPAddress ipAddress = Array.FindAll(Dns.GetHostAddresses(Dns.GetHostName()), a => a.AddressFamily == AddressFamily.InterNetwork).First();
            string[] baseIP = ipAddress.ToString().Split('.');

            for (int j = Int32.Parse(baseIP[2]); j < Int32.Parse(baseIP[2]) + 5; j++) {
                for (int i = 0; i < 255; i++) {
                    try {
                        IPAddress ip = IPAddress.Parse(baseIP[0] + "." + baseIP[1] + "." + j + "." + i);

                        Ping ping = new Ping();
                        ping.PingCompleted += new PingCompletedEventHandler(PingCompleted);
                        ping.SendAsync(ip, 100, ip);
                    } catch (Exception) { }
                }
            }
        }

        private static void PingCompleted(object sender, PingCompletedEventArgs e) {
            if (e.Reply != null && e.Reply.Status == IPStatus.Success) {
                Thread SearchServer = new Thread(() => CheckServer(e));
                SearchServer.SetApartmentState(ApartmentState.STA);
                SearchServer.Name = "SearchServer";

                SearchServer.Start();
            }
        }

        private static void CheckServer(PingCompletedEventArgs e) {
            TcpClient connection = new TcpClient();
            try {
                connection.Connect("" + e.UserState, port);
                if (connection.Connected) {
                    LoginUC.GetInstance().AddServerIp("" + e.UserState);
                }
            } catch (Exception) { }
        }
    }
}
