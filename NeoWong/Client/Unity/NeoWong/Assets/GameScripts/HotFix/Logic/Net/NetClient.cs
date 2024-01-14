using NWFramework;
using Google.Protobuf;
using System;
using UnityEngine;
using System.Collections;
using Proto.Basic;
using UnityWebSocket;

namespace HotFix
{
    public class WebSocketConnection : IConnection
    {
        private WebSocket _ws = null;

        public Action<IConnection> Disconnected;

        public void ConnectToServer(NetOption netOption)
        {
            if (string.IsNullOrEmpty(netOption.url))
            {
                Log.Error("Websocket Connect failed, net option URL cannot be null");
                return;
            }

            _ws = new WebSocket(netOption.url);

            _ws.OnOpen += (sender, e) =>
            {
                Log.Info("连接到服务端");
            };

            _ws.OnMessage += (sender, e) =>
            {
                ushort code = GetUShort(e.RawData, 0);
                var msg = ProtoHelper.ParseFrom(code, e.RawData, 2, e.RawData.Length - 2);

                if (MessageRouter.Instance.Running)
                {
                    MessageRouter.Instance.AddMessage(this, msg);
                }
            };

            _ws.OnClose += (sender, e) =>
            {
                Disconnected?.Invoke(this);
            };

            _ws.ConnectAsync();
        }

        public void Send(byte[] buffer)
        {
            if (IsConnected())
            {
                _ws.SendAsync(buffer);
            }
        }

        public bool IsConnected()
        {
            if (_ws == null) 
                return false;

            if (_ws.ReadyState == WebSocketState.Open) 
                return true;

            return false;
        }

        public void Close()
        {
            _ws.CloseAsync();
        }

        //前提是data必须是大端字节序
        private ushort GetUShort(byte[] data, int offset)
        {
            if (BitConverter.IsLittleEndian)
            {
                return (ushort)((data[offset] << 8) | data[offset + 1]);
            }
            else
            {
                return (ushort)((data[offset + 1] << 8) | data[offset]);
            }
        }
    }

    public class NetClient : Singleton<NetClient>
    {
        IConnection conn = null;

        public void Send(IMessage message)
        {
            if (conn != null)
            {
                using (var ds = DataStream.Allocate())
                {
                    int code = ProtoHelper.SeqCode(message.GetType());
                    ds.WriteInt(message.CalculateSize() + 2);
                    ds.WriteUShort((ushort)code);
                    message.WriteTo(ds);
                    conn.Send(ds.ToArray());
                }
            }
        }

        /// <summary>
        /// 连接到服务器
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        public void ConnectToServer(NetOption netOption)
        {
            ////服务器终端
            //IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(host), port);
            //Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //socket.Connect(ipe);
            //Log.Info("连接到服务端");

            conn = new Connection();
            conn.ConnectToServer(netOption);
            ((Connection)conn).Disconnected += OnDisconnected;

            //conn = new WebSocketConnection();
            //netOption.url = "ws://127.0.0.1:8888";
            //conn.ConnectToServer(netOption);
            //((WebSocketConnection)conn).Disconnected += OnDisconnected;

            //启动消息分发器
            MessageRouter.Instance.Start();

            MessageRouter.Instance.Subscribe<HeartBeatResponse>(OnHeartBeatResponse);
        }

        //连接断开
        private void OnDisconnected(IConnection sender)
        {
            Log.Info("与服务器断开");

            if (heartBeatCoroutine != null)
            {
                Utility.Unity.StopCoroutine(heartBeatCoroutine);
                heartBeatCoroutine = null;
            }
        }

        public void Close()
        {
            try
            {
                conn?.Close();
            }
            catch { }
        }

        #region HeartBeat

        Coroutine heartBeatCoroutine;
        HeartBeatRequest beatRequest = new HeartBeatRequest();
        DateTime lastBeatTime = DateTime.MinValue;

        public void StartSendHeartBeat()
        {
            heartBeatCoroutine = Utility.Unity.StartCoroutine(SendHeartMessage());
        }

        IEnumerator SendHeartMessage()
        {
            while (true)
            {
                yield return new WaitForSeconds(1.0f);
                Send(beatRequest);
                lastBeatTime = DateTime.Now;
            }
        }

        void OnHeartBeatResponse(IConnection conn, HeartBeatResponse res)
        {
            var t = DateTime.Now - lastBeatTime;
            int ms = Math.Max(1, (int)Math.Round(t.TotalMilliseconds));
            Log.Info($"网络延迟：{ms}ms");
        }

        #endregion
    }
}
