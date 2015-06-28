using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using SuperFriendBot.Slack;

namespace SuperFriendBot
{
    public class SuperFriendBot
    {
        private Thread _thread;
        private ManualResetEvent _shutdownEvent = new ManualResetEvent(false);
        private State _state;

        enum State
        {
            WaitingToConnect,
            Connect,
            Connected,
            Error
        }

        public SuperFriendBot()
        {
            _state = State.WaitingToConnect;
        }

        public void Start()
        {
            _thread = new Thread(MainThread);
            _thread.Name = "SuperFriendBot";
            _thread.IsBackground = true;
            _thread.Start();
        }

        public void Stop()
        {
            _shutdownEvent.Set();

            if (!_thread.Join(5000))
            {
                // give the thread 5 seconds to stop
                _thread.Abort();
            }
        }

        private async void MainThread()
        {
            rtm_start start = new rtm_start();
            string WebSocketUrl = "";
            ClientWebSocket SlackSocket = new ClientWebSocket();
            UTF8Encoding Encoder = new UTF8Encoding();

            while (!_shutdownEvent.WaitOne(0))
            {
                switch(_state)
                {
                    case State.WaitingToConnect:
                        bool result = await start.Call();
                        if (result == true)
                        {
                            WebSocketUrl = start.WebSocketUrl;
                            _state = State.Connect;
                        }
                        else
                        {
                            _state = State.Error;
                        }
                        break;
                    case State.Connect:
                        await SlackSocket.ConnectAsync(new Uri(WebSocketUrl), CancellationToken.None);
                        string test = "{ \"id\": 1, \"type\": \"message\", \"channel\": \"" + start.GeneralChat + "\", \"text\": \"Hello world\" }";
                        byte[] buffer = Encoder.GetBytes(test);
                        await SlackSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                        _state = State.Connected;
                        break;
                    case State.Connected:
                        break;
                    case State.Error:
                        break;
                }

                Thread.Sleep(1000);
            }
        }
    }
}
