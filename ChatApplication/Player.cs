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

namespace ChatApplication {
    public class Player {
        private static Player instance;

        public string Name { get; set; }

        public TcpClient TcpClient { get; set; }
        public bool Connected { get; set; }

        public StreamWriter Writer { get; set; }
        public StreamReader Reader { get; set; }

        private WaveIn waveIn;
        private WaveOut waveOut;
        private BufferedWaveProvider waveProvider;
        private UncompressedPcmChatCodec audioCodec;

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
            if (TcpClient != null && TcpClient.Connected) {
                Connected = false;
                WriteLine("CloseConnection:");
                Reader.Close();
                Writer.Close();
                TcpClient.Close();
                waveIn.DataAvailable -= waveIn_DataAvailable;
                waveIn.StopRecording();
                waveIn.Dispose();
            }
        }

        public void InitializeStream() {
            Writer = new StreamWriter(TcpClient.GetStream());
            Reader = new StreamReader(TcpClient.GetStream());
            InitializeAudioStream();
        }

        private void InitializeAudioStream() {
            audioCodec = new UncompressedPcmChatCodec();
            InitializeWaveIn();
            InitializeWaveOut();
        }

        private void InitializeWaveOut() {
            waveOut = new WaveOut();
            waveProvider = new BufferedWaveProvider(audioCodec.RecordFormat);
            waveOut.Init(waveProvider);
            waveOut.Play(); // TODO: MUTE UNMUTE
        }

        private void InitializeWaveIn() {
            waveIn = new WaveIn();
            waveIn.BufferMilliseconds = 200;
            waveIn.DeviceNumber = 0; // TODO: HARD CODED
            waveIn.WaveFormat = audioCodec.RecordFormat;
            waveIn.DataAvailable += waveIn_DataAvailable;
            waveIn.StartRecording(); // TODO: MUTE UNMUTE
        }

        public void PlayAudio(byte[] receivedAudioBytes) {
            byte[] decoded = audioCodec.Decode(receivedAudioBytes, 0, receivedAudioBytes.Length);
            waveProvider.AddSamples(decoded, 0, decoded.Length);
        }

        private void waveIn_DataAvailable(object sender, WaveInEventArgs e) {
            byte[] encoded = audioCodec.Encode(e.Buffer, 0, e.BytesRecorded);
            WriteLine("Sound:" + string.Join(",", encoded));
        }

        public static Player getInstance() {
            if (instance == null) {
                instance = new Player();
            }
            return instance;
        }

    }
}