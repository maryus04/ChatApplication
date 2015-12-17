using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Net.Sockets;
using System.IO;
using NAudio.Wave;
using System.Threading;
using ChatApplication.UserControls;
using NAudioDemo.NetworkChatDemo;
using NSpeex;

namespace ChatApplication {
    public class Player {
        private static Player instance;

        public string Name { get; set; }

        public TcpClient tcpClient { get; set; }
        public bool Connected { get; set; }

        public StreamWriter Writer { get; set; }
        public StreamReader Reader { get; set; }

        private WaveIn waveIn;
        private bool micOn = true;
        public int DeviceInNumber = 0;
        private WaveOut waveOut;
        private bool spOn = true;
        private BufferedWaveProvider waveProvider;
        private SpeexChatCodec audioCodec;

        public Player() { instance = this; }

        public void WriteLine(string message) {
            try {
                Writer.WriteLine(message);
                Writer.Flush();
            } catch (Exception) {
                CloseConnection();
            }
        }

        public string ReadLine() {
            try {
                return Reader.ReadLine();
            } catch (Exception) {
                CloseConnection();
            }
            return "";
        }

        public void CloseConnection() {
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\history.hst", ChatUC.GetInstance().GetText());
            if (tcpClient != null && tcpClient.Connected) {
                Connected = false;
                WriteLine("CloseConnection:");
                Reader.Close();
                Writer.Close();
                tcpClient.Close();
                waveIn.DataAvailable -= waveIn_DataAvailable;
                waveIn.StopRecording();
                waveIn.Dispose();
                waveOut.Stop();
                waveOut.Dispose();
            }
        }

        public void InitializeStream() {
            Writer = new StreamWriter(tcpClient.GetStream());
            Reader = new StreamReader(tcpClient.GetStream());
            InitializeAudioStream();
        }

        private void InitializeAudioStream() {
            audioCodec = new SpeexChatCodec(BandMode.Wide, 16000, "Speex Wide Band (16kHz)");
            InitializeWaveIn();
            InitializeWaveOut();
        }

        private void InitializeWaveOut() {
            waveOut = new WaveOut();
            waveProvider = new BufferedWaveProvider(audioCodec.RecordFormat);
            waveOut.Init(waveProvider);
            waveOut.Play();
        }

        private void InitializeWaveIn() {
            waveIn = new WaveIn();
            waveIn.BufferMilliseconds = 50;
            waveIn.DeviceNumber = DeviceInNumber;
            waveIn.WaveFormat = audioCodec.RecordFormat;
            waveIn.DataAvailable += waveIn_DataAvailable;
            waveIn.StartRecording();
        }

        public void PlayAudio(byte[] receivedAudioBytes) {
            if (!spOn) return;
            byte[] decoded = audioCodec.Decode(receivedAudioBytes, 0, receivedAudioBytes.Length);
            waveProvider.AddSamples(decoded, 0, decoded.Length);
        }

        private void waveIn_DataAvailable(object sender, WaveInEventArgs e) {
            byte[] encoded = audioCodec.Encode(e.Buffer, 0, e.BytesRecorded);
            WriteLine("Sound:" + string.Join(",", encoded));
        }
        
        public static Player GetInstance() {
            if (instance == null) {
                instance = new Player();
            }
            return instance;
        }

        public void SendChat() {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\history.hst")) return;
            WriteLine("History:" + File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\history.hst").Replace("\r\n", @"\new_line\"));
        }

        public void SendNickName(string myName) { WriteLine("MyName:" + myName); }

        public void InitConnection(string serverIp) {
            if (tcpClient != null) return;
            if (!InitializeConnection(serverIp)) return;

            InitializeStream();

            ReadMessages();
        }

        private bool InitializeConnection(string serverIp) {
            try {
                tcpClient = new TcpClient();
                tcpClient.Connect(serverIp, 4296);
            } catch {
                return false;
            }
            return true;
        }

        public void ToggleMic() {
            if (micOn) {
                waveIn.DataAvailable -= waveIn_DataAvailable;
                waveIn.StopRecording();
                ChatUC.GetInstance().SetMicButtonText("UnMute");
                micOn = false;
            } else {
                waveIn.DataAvailable += waveIn_DataAvailable;
                waveIn.StartRecording();
                ChatUC.GetInstance().SetMicButtonText("Mute");
                micOn = true;
            }
        }

        public void ToggleSpeaker() {
            if (spOn) {
                waveOut.Stop();
                ChatUC.GetInstance().SetSpButtonText("SP Off");
                spOn = false;
            } else {
                waveOut.Play();
                ChatUC.GetInstance().SetSpButtonText("SP On");
                spOn = true;
            }
        }

        private void ReadMessages() {
            Thread ReadIncomming = new Thread(() => MessageReader.ReadMessages());
            ReadIncomming.SetApartmentState(ApartmentState.STA);
            ReadIncomming.Name = "ReadMessages";
            ReadIncomming.Start();
        }
    }
}