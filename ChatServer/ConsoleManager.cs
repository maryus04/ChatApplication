using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatServer {
    public class ConsoleManager {
        public static bool debugMode = false;
        public static bool infoMode = false;

        private static readonly int PREFIX_LENGTH = 26;

        public static bool DebugMode {
            get { return debugMode; }
            set { debugMode = value; }
        }

        public static void Server(string message) {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(Prefix("(Info)(Server)") + message);
            Console.ResetColor();
        }

        public static void ServerWarn(string message) {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(Prefix("(Warn)(Server)") + message);
            Console.ResetColor();
        }

        public static void ServerError(string message) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(Prefix("(Error)(Server)") + message);
            Console.ResetColor();
        }

        public static void ServerDebug(string message) {
            if (!debugMode) return;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(Prefix("(Debug)(Server)") + message);
            Console.ResetColor();
        }

        public static void Communication(string message) {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(Prefix("(Info)(Comunication)") + message);
            Console.ResetColor();
        }

        public static void CommunicationWarn(string message) {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(Prefix("(Warn)(Comunication)") + message);
            Console.ResetColor();
        }

        public static void CommunicationError(string message) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(Prefix("(Error)(Comunication)") + message);
            Console.ResetColor();
        }

        public static void CommunicationDebug(string message) {
            if (!debugMode) return;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(Prefix("(Debug)(Comunication)") + message);
            Console.ResetColor();
        }

        private static string Prefix(string prefix) {
            String temp = prefix;
            for (int i = temp.Length; i < PREFIX_LENGTH; i++) {
                temp = temp + " ";
            }
            return temp;
        }
    }
}