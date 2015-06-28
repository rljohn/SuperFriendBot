using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperFriendBot
{
    public class Logger
    {
        private static readonly object _lock = new object();
        private static StreamWriter _sw;

        public static void Init()
        {
            _sw = File.CreateText("log.txt");
        }

        public static void Shutdown()
        {
            _sw.Close();
        }

        public static void Log(string message)
        {
            lock (_lock)
            {
                String timeStamp = DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt");
                _sw.WriteLine(String.Format("[{0}] {1}", timeStamp, message));
                _sw.Flush();
            }
        }

        public static void Error(string message)
        {
            lock (_lock)
            {
                String timeStamp = DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt");
                _sw.WriteLine(String.Format("[{0}] [ERROR] {1}", timeStamp, message));
                _sw.Flush();
            }
        }
    }
}
