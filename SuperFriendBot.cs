using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
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
            Connected
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
            while (!_shutdownEvent.WaitOne(0))
            {
                switch(_state)
                {
                    case State.WaitingToConnect:
                        rtm_start connect = new rtm_start();
                        await connect.Call();
                        _state = State.Connected;
                        break;
                    case State.Connected:
                        break;
                }

                Thread.Sleep(1000);
            }
        }
    }
}
