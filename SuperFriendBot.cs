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

using Newtonsoft.Json.Linq;
using SuperFriendBot.Slack;

namespace SuperFriendBot
{
    public class SuperFriendBot
    {
        private Thread _thread;
        private ManualResetEvent _shutdownEvent = new ManualResetEvent(false);
        private State _state;
        private rtm_start _start;
        private ClientWebSocket _slackSocket = new ClientWebSocket();
        private byte[] _recBuffer;
        private ArraySegment<byte> _recSegment;
        private UTF8Encoding _encoder = new UTF8Encoding();
        private int _msgId = 0;

        enum State
        {
            WaitingToConnect,
            Connect,
            Connected,
            Error,
            Finished
        }

        public SuperFriendBot()
        {
            _state = State.WaitingToConnect;
            _recBuffer = new byte[1024]; // 1KB buffer
            _recSegment = new ArraySegment<byte>(_recBuffer);
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
            Logger.Init();
            _start = new rtm_start();

            while (!_shutdownEvent.WaitOne(0))
            {
                switch(_state)
                {
                    case State.WaitingToConnect:
                        _state = (await _start.Call()) ? State.Connect : State.Error;
                        break;
                    case State.Connect:
                        await _slackSocket.ConnectAsync(_start.WebSocketUrl, CancellationToken.None);
                        _state = State.Connected;
                        break;
                    case State.Connected:
                        await ReceiveMessages();
                        break;
                    case State.Finished:
                        return;
                    case State.Error:
                        break;
                }

                Thread.Sleep(33); // 30 tick rate
            }

            Logger.Shutdown();
        }

        private async Task ReceiveMessages()
        {
            var result = await _slackSocket.ReceiveAsync(_recSegment, CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                _state = State.Finished;
            }
            else if (result.MessageType == WebSocketMessageType.Binary)
            {
                _state = State.Finished;
            }

            int count = result.Count;
            while (!result.EndOfMessage)
            {
                if (count >= _recBuffer.Length)
                {
                    await _slackSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "That's too long", CancellationToken.None);
                    return;
                }

                _recSegment = new ArraySegment<byte>(_recBuffer, count, _recBuffer.Length - count);
                result = await _slackSocket.ReceiveAsync(_recSegment, CancellationToken.None);
                count += result.Count;
            }

            var message = Encoding.UTF8.GetString(_recBuffer, 0, result.Count);
            HandleMessage(message);
            Logger.Log(message);
        }

        private void HandleMessage(string message)
        {
            JObject result = JObject.Parse(message);
            switch((string)result["type"])
            {
                case "presence_change":
                    HandlePresenceChange(result);
                    break;
                case "user_typing":
                    HandleUserTypeing(result);
                    break;
                case "message":
                    HandleMsg(result);
                    break;
            }
        }

        private void HandlePresenceChange(JObject msg)
        {

        }

        private void HandleUserTypeing(JObject msg)
        {

        }

        private void HandleMsg(JObject msg)
        {
            string fromChannel = (string)msg["channel"];
            string text = (string)msg["text"];

            if (text == "!aryaspam")
            {
                string response = ":praiseit: OYSTERS :praiseit: CLAMS :praiseit: AND COCKLES :praiseit:";

                JObject jResp = JObject.FromObject(new
                    {
                        id = _msgId,
                        type = "message",
                        channel = fromChannel,
                        text = response
                    });

                SendMessage(jResp);
            }
        }

        private async void SendMessage(JObject msg)
        {
            _msgId++;

            Logger.Log("Sent: " + msg);

            byte[] buf = _encoder.GetBytes(msg.ToString());
            ArraySegment<byte> send = new ArraySegment<byte>(buf);
           await _slackSocket.SendAsync(send, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
