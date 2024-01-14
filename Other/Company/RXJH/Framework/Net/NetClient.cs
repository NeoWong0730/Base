using Google.Protobuf;
using Lib.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

//TODO : 稳定后清理多余逻辑代码
//TODO : BeginSend的不可靠性 可能需要自行管理发送的频率 以及 发送缓存 (当集中发送数据过大等)

namespace Net
{
    public class NetClient
    {
        /// <summary>
        /// 包头的长度 size(2)|0(2)|eventId(2)|cmd(2)
        /// </summary>
        public const short HEADER_LENGTH = 6;
        /*
         * 心跳逻辑：
         * 1. 登录/创角/选角 阶段是被动心跳阶段，也就是服务器发心跳，客户端回复即可。
         * 2. 进入游戏之后，客户端开始主导心跳阶段，也就是客户端主动发送心跳，等待server回复，长时间无回复会断线重连
         * 所以NetClient的心跳要分 主动 和 被动
         */
        public const ushort HeartBeatReq = 7169; // 1, 7
        public const ushort HeartBeatNtf = 8193; // 1, 8
        private const int gHeartBeatInterval = 10000; //毫秒
        private const int gNetThreadDeltaTime = 100;  //毫秒

        public enum EType {
            Login,
            Game,
        }

        public static readonly NetClient Instance = new NetClient(EType.Game);

        public EType name { get; private set; }
        public NetClient(EType name) {
            this.name = name;
        }

        public enum ENetState
        {
            Ready,
            Connecting,
            Connected,
            ConnectFail,
        };

        [Flags]
        public enum EErrorFlag
        {
            None = 0,
            SendError = 1,
            ReceiveError = 2,
        }

        private Socket mSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        private string serverIP = string.Empty;
        private int serverPort = 0;
        private EErrorFlag eErrorFlag = EErrorFlag.None;
        private ENetState eAimStatus = ENetState.Ready;
        private ENetState _status = ENetState.Ready;
        private IAsyncResult _connectAsyncResult = null;
        private Action<ENetState, ENetState> onStatusChnage;
        private Action<long> onPingChnage;

        #region Buffers
        private const int kMaxBufferSize = ushort.MaxValue;                                             //一条消息体最大ushort
        private byte[] mSendBuffer = new byte[kMaxBufferSize];                                          //消息发送缓冲区
        private byte[] mReceiveBuffer = new byte[kMaxBufferSize];                                       //消息接收缓冲区
        private MemoryStream mSendStream = new MemoryStream(kMaxBufferSize);                            //消息序列化缓冲区
        private NetMessageReceiveBuffer mMessageReceiveBuffer = new NetMessageReceiveBuffer();          //消息反序列化缓冲区

        Queue<NetMsg> ReceiveMessages0 = new Queue<NetMsg>();  //消息接收缓冲队列 主线程用
        Queue<NetMsg> ReceiveMessages1 = new Queue<NetMsg>();  //消息接收缓冲队列 网络线程用

        Queue<NetMsg> SendMessages0 = new Queue<NetMsg>();     //消息发送缓冲队列 主线程用
        Queue<NetMsg> SendMessages1 = new Queue<NetMsg>();     //消息发送缓冲队列 网络线程用

        private System.Threading.Thread _NetThread;
        private bool _bThreadRunning = false;
        private bool _hasReceivePackage = false;
        private bool _hasSendPackage = false;

        private uint _heartBeatReqCount = 0;
        private Lib.Core.Net.SecondLogin_Req_KeepHeart _reqHeartBeat = new Lib.Core.Net.SecondLogin_Req_KeepHeart();
        private long heartBeatInterval = gHeartBeatInterval;
        private System.Diagnostics.Stopwatch _stopWatch = new System.Diagnostics.Stopwatch();
        private long _currentPing = 0;
        private long _newPing = 0;
        #endregion

        public int LastExecuteTime = 0;
        public int LastDispatcherCount = 0;
        public int LastExecuteFrame = 0;

        public ENetState eNetStatus
        {
            get { return _status; }
            private set
            {
                if (_status != value)
                {
                    ENetState oldState = _status;
                    _status = value;
                    onStatusChnage?.Invoke(oldState, _status);
                }
            }
        }

        public bool IsConnected
        {
            get
            {
                return _status == ENetState.Connected;
            }
        }
        public bool IsConnecting
        {
            get
            {
                return _status == ENetState.Connecting;
            }
        }
        public void Update()
        {
            switch (_status)
            {
                case ENetState.Ready:
                case ENetState.ConnectFail:
                    {
                        if (eAimStatus == ENetState.Connected)
                        {
                            eNetStatus = ENetState.Connecting;
                            _DoConnect();
                        }
                    }
                    break;
                case ENetState.Connecting:
                    {
                        if (_connectAsyncResult.IsCompleted)
                        {
                            if (mSocket.Connected)
                            {
                                eNetStatus = ENetState.Connected;
                                eAimStatus = ENetState.Connected;
                                _heartBeatReqCount = 0;

                                //_NetThread.Change(0, gNetThreadDeltaTime);
                                _bThreadRunning = true;
                            }
                            else
                            {
                                eNetStatus = ENetState.ConnectFail;
                                eAimStatus = ENetState.ConnectFail;
                            }
                        }
                    }
                    break;
                case ENetState.Connected:
                    {
                        if (!_hasSendPackage)
                        {
                            Queue<NetMsg> tmp;
                            tmp = SendMessages0;
                            SendMessages0 = SendMessages1;
                            SendMessages1 = tmp;

                            _hasSendPackage = true;
                        }

                        if(_hasReceivePackage)
                        {
                            _DoDispatcher();
                            if (ReceiveMessages0.Count <= 0)
                            {
                                _hasReceivePackage = false;
                            }
                        }

                        if (eErrorFlag != EErrorFlag.None || _heartBeatReqCount >= 3 || !mSocket.Connected)
                        {
#if DEBUG_MODE
                            DebugUtil.LogErrorFormat("heartbeacount= {0}", _heartBeatReqCount);
                            DebugUtil.LogErrorFormat("非主动断开 {0}", name.ToString());
#endif
                            if (!_hasReceivePackage)
                                _DoDisconnect();
                        }
                        else
                        {
                            if (_currentPing != _newPing && onPingChnage != null)
                            {
                                _currentPing = _newPing;
                                onPingChnage(_currentPing);
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        public void Connect()
        {
            eAimStatus = ENetState.Connected;
            _hasSendPackage = false;
            _hasReceivePackage = false;
        }
        public bool Connect(string host, int port)
        {
            DebugUtil.LogFormat(ELogType.eNone, "{0} Connect ({1}:{2})",  name.ToString(), host.ToString(), port.ToString());
            if (_NetThread == null)
            {
                _NetThread = new System.Threading.Thread(new ThreadStart(NetThread));                
                _NetThread.Name = "NetThread";
                _NetThread.Priority = System.Threading.ThreadPriority.Normal;
                _NetThread.Start();
            }

            if (_status == ENetState.Ready || _status == ENetState.ConnectFail)
            {
                serverIP = host;
                serverPort = port;
                eAimStatus = ENetState.Connected;
                return true;
            }
            else
            {
                DebugUtil.LogError("Scoket 已经连接或者正在连接");
                return false;
            }
        }
        public void Disconnect()
        {
            _DoDisconnect();
        }
        public void Dispose()
        {
            onStatusChnage = null;
            onPingChnage = null;

            _DoDisconnect();
            _NetThread?.Abort();
            _NetThread = null;
        }

        public void SendMessage(ushort /*enClientFirst*/first, ushort /*enClientSecond*/second, IMessage message) {
            SendMessage(NetMsgUtil.CalMsgCode(first, second), message);
        }

        private void SendMessage(UInt16 cmd, IMessage message)
        {
            if (ENetState.Connected != _status)
            {
#if DEBUG_MODE
                DebugUtil.LogErrorFormat("try to send data while socket is not connected (eStatus = {0})", _status.ToString());
#endif
                return;
            }

#if DEBUG_MODE
            if (DebugUtil.IsOpenLogType(ELogType.eNetSendMSG) && EventDispatcher.Instance.FilterLog(cmd))
            {
                string json = string.Empty;
                try
                {
                    json = message == null ? string.Empty : LitJson.JsonMapper.ToJson(message);
                }
                catch
                {
                    json = "{...}";
                }

                DebugUtil.LogFormat(ELogType.eNetSendMSG, "{0} {1}", cmd.ToString(), json);
            }
#endif

            MemoryStream stream = null;
            if (message != null)
            {
                stream = new MemoryStream();
                message.WriteTo(stream);
            }

            NetMsg netMsg = new NetMsg() { evtId = cmd, data = null, stream = stream };

            SendMessages0.Enqueue(netMsg);
        }
        public void AddStateListener(Action<ENetState, ENetState> statusChnageCallback)
        {
            onStatusChnage += statusChnageCallback;
        }
        public void RemoveStateListener(Action<ENetState, ENetState> statusChnageCallback)
        {
            if (onStatusChnage != null)
            {
                onStatusChnage -= statusChnageCallback;
            }
        }
        public void AddPingListener(Action<long> action)
        {
            onPingChnage += action;
        }
        public void RemovePingListener(Action<long> action)
        {
            if (onPingChnage != null)
            {
                onPingChnage -= action;
            }
        }        
        private void _DoConnect()
        {
            eErrorFlag = EErrorFlag.None;

            string realIPAddress = TryGetIpv6ByAddress(serverIP, serverPort.ToString(), out AddressFamily realAddressFamily);

            mSocket.Close();
            mSocket = new Socket(realAddressFamily, SocketType.Stream, ProtocolType.Tcp);
            mSocket.NoDelay = true;
            mSocket.ReceiveTimeout = 0;
            mSocket.SendTimeout = 600;

            IPAddress address;
            if (!IPAddress.TryParse(realIPAddress, out address))
            {
                try
                {
                    var hostAll = Dns.GetHostAddresses(realIPAddress);
                    if (hostAll.Length > 0)
                    {
                        address = hostAll[0];
                    }
                    else
                    {
                        DebugUtil.LogError("NDS Get Host is null");
                    }
                }
                catch (Exception e)
                {
                    DebugUtil.LogException(e);
                    return;
                }
            }

            IPEndPoint remoteEP = new IPEndPoint(address, serverPort);
            DebugUtil.LogFormat(ELogType.eNone, "Socket BeginConnect ({0}) {1}", remoteEP.ToString(), remoteEP.AddressFamily.ToString());
            _connectAsyncResult = mSocket.BeginConnect(remoteEP, null, null);
        }
        private void _DoDisconnect()
        {
            DebugUtil.LogFormat(ELogType.eNone, "Socket _DoDisconnect {0}", name.ToString());
            if (mSocket.Connected)
            {
                // try
                // {
                //     mSocket.Shutdown(SocketShutdown.Both);
                // }
                // catch (Exception e)
                // {
                //     DebugUtil.LogException(e);
                // }

                try
                {
                    mSocket.Close();
                }
                catch (Exception e)
                {
                    DebugUtil.LogException(e);
                }
            }

            eNetStatus = ENetState.Ready;
            eAimStatus = ENetState.Ready;

            //_NetThread?.Change(Timeout.Infinite, gNetThreadDeltaTime);

            _hasSendPackage = false;
            _hasReceivePackage = false;
            _bThreadRunning = false;
            _ClearBuffers();
        }

#if DEBUG_MODE
        int frame = 0;
#endif
        private void NetThread()
        {
            while (_NetThread.ThreadState == ThreadState.Running)
            {
#if DEBUG_MODE
                ++frame;
                if (frame > 1)
                {
                    DebugUtil.LogErrorFormat("严重BUG: NetThread Function Enter Count {0}", frame.ToString());
                }
#endif
                if (_bThreadRunning)
                {
                    _DoHeartBeat();
                    if (_hasSendPackage)
                    {
                        _DoSend();
                        _hasSendPackage = false;
                    }
                    _DoReceive();

                    if (!_hasReceivePackage && ReceiveMessages1.Count > 0)
                    {
                        Queue<NetMsg> tmp;
                        tmp = ReceiveMessages0;
                        ReceiveMessages0 = ReceiveMessages1;
                        ReceiveMessages1 = tmp;

                        _hasReceivePackage = true;
                    }
                }
#if DEBUG_MODE
                --frame;
#endif
                Thread.Sleep(gNetThreadDeltaTime);
            }

            DebugUtil.LogFormat(ELogType.eNone, "NetThread Exit ThreadState = {0}", _NetThread.ThreadState.ToString());
        }
        private int _SendMessage(ushort cmd, IMessage message, Stream stream, bool useStream)
        {
            mSendStream.SetLength(HEADER_LENGTH);
            mSendStream.Position = HEADER_LENGTH;

            if (useStream)
            {
                if (stream != null)
                {
                    stream.Position = 0;
                    stream.CopyTo(mSendStream);
                    stream.Dispose();
                }
            }
            else
            {
                NetMsgUtil.Serialzie(message, mSendStream);
            }            

            int packetSize = (int)mSendStream.Length;
            if (packetSize > kMaxBufferSize)
            {
                Debug.LogErrorFormat("SendMessage {0} 包体过大 size = {1}", cmd.ToString(), packetSize.ToString());
                return 0;
            }

            mSendStream.Position = 0;
            mSendStream.Read(mSendBuffer, 0, packetSize);

            //加密数据部分
            int dataSize = packetSize - (HEADER_LENGTH - 2);

            // 组装
            // msg和eventId总长度, 小端
            mSendBuffer[0] = (byte)(dataSize);
            mSendBuffer[1] = (byte)(dataSize >> 8);
            // 暂时不用，后面做随机协议加密
            mSendBuffer[2] = 0;
            mSendBuffer[3] = 0;
            // eventId, 小端
            mSendBuffer[4] = (byte)(cmd);
            mSendBuffer[5] = (byte)(cmd >> 8);

            if (!mSocket.Connected)
            {
                return 1;
            }

            try
            {
                int count = 0;
                while (count < packetSize)
                {
                    count += mSocket.Send(mSendBuffer, count, packetSize - count, SocketFlags.None, out SocketError socketError);
                    if (socketError != SocketError.Success)
                    {
                        eErrorFlag |= EErrorFlag.SendError;
#if DEBUG_MODE                        
                        DebugUtil.LogErrorFormat("发送 {0} 时异常 SocketError = {1}", cmd.ToString(), socketError.ToString());
#endif
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                eErrorFlag |= EErrorFlag.SendError;

#if DEBUG_MODE                
                DebugUtil.LogErrorFormat("发送{0}时异常：{1}", cmd.ToString(), e.Message);
#endif
            }

            return 0;
        }
        private void _DoSend()
        {
            bool isSuccess = true;
            while (SendMessages1.Count > 0)
            {
                NetMsg msg = SendMessages1.Dequeue();
                if (isSuccess)
                {
                    if (_SendMessage(msg.evtId, msg.data, msg.stream, true) != 0)
                    {
                        isSuccess = false;
                    }
                }
                else
                {
                    msg.stream.Dispose();
                }
            }
        }

        // 登录/选角/创角阶段是被动心跳，进入scene之后是主动心跳
        // 客户端主动心跳，还是被动心跳
        // 默认客户端被动， 进入sceneFinish之后切换为主动，因为登录选角创角服务器资源比较紧缺，所以服务器主导，进入scene之后切换为客户端主导
        public bool positiveHB { get; private set; } = false;
        public void PositiveHeartBeat(bool potitiveHB) {
            this.positiveHB = potitiveHB;
        }

        private void _DoHeartBeat()
        {
            if (!positiveHB) {
                return;
            }

            heartBeatInterval += gNetThreadDeltaTime;
            if (heartBeatInterval >= gHeartBeatInterval)
            {
                heartBeatInterval = 0;
            
                _stopWatch.Restart();
                ++_heartBeatReqCount;
                _SendMessage(HeartBeatReq, _reqHeartBeat, null, false);
                DebugUtil.LogFormat(ELogType.eHeartBeat, "{0} 发送心跳包 {1}", name.ToString(), _heartBeatReqCount.ToString());
            }
        }

        private void _DoReceive()
        {            
            int size = 0;
            try
            {
                //if (mSocket.Available <= 0)
                //{
                //    return;
                //}

                if(!mSocket.Poll(10, SelectMode.SelectRead))
                {
                    return;
                }

                size = mSocket.Receive(mReceiveBuffer, 0, kMaxBufferSize, SocketFlags.None, out SocketError socketError);

                if (socketError != SocketError.Success)
                {
                    eErrorFlag |= EErrorFlag.ReceiveError;
#if DEBUG_MODE
                    DebugUtil.LogErrorFormat("接收消息时异常：SocketError = {0}", socketError.ToString());
#endif                    
                    return;
                }
            }
            catch (Exception e)
            {
                eErrorFlag |= EErrorFlag.ReceiveError;
#if DEBUG_MODE                
                DebugUtil.LogErrorFormat("接收消息时异常：{0}", e.Message);
#endif
                return;
            }

            if(size == 0)
            {
                eErrorFlag |= EErrorFlag.ReceiveError;
#if DEBUG_MODE
                DebugUtil.LogErrorFormat("服务器关闭");
#endif
                return;
            }

            if (!mSocket.Connected)
            {
                return;
            }

            int readCount = 0;
            while (readCount < size)
            {
                readCount += mMessageReceiveBuffer.WriteByte(mReceiveBuffer, readCount, size - readCount);
                if (!mMessageReceiveBuffer.CheckComplete())
                    continue;

                ushort evtId = mMessageReceiveBuffer.evtId;
                //收到心跳包直接处理
                if (evtId == HeartBeatNtf)
                {
                    DebugUtil.LogFormat(ELogType.eHeartBeat, " {0} 接收心跳包 {1} Ping {2}ms", name.ToString(), _heartBeatReqCount.ToString(), _newPing.ToString());
                    if(positiveHB) {
                        // 主动心跳阶段，开始老逻辑的主动心跳检测
                        _stopWatch.Stop();
                        _newPing = _stopWatch.ElapsedMilliseconds;
                        --_heartBeatReqCount;
                    }
                    else {
                        // 被动心跳阶段 网络线程中接收到心跳，直接回复
                        _SendMessage(HeartBeatReq, _reqHeartBeat, null, false);
                    }
                }
                else if (EventDispatcher.Instance.TryGetEventHandle(evtId, out NetEventHandle handler))
                {
                    NetMsg netMsg = new NetMsg();
                    netMsg.evtId = evtId;
                    netMsg.bodyLength = mMessageReceiveBuffer.bodyLength;

                    mMessageReceiveBuffer.data.Position = 0;
                    netMsg.stream = new MemoryStream();// (mMessageReceiveBuffer.bodyLength);
                    mMessageReceiveBuffer.data.CopyTo(netMsg.stream);

                    ReceiveMessages1.Enqueue(netMsg);
                }

                mMessageReceiveBuffer.Reset();
            }
        }
        private void _DoDispatcher()
        {
#if DEBUG_MODE
            float t = UnityEngine.Time.realtimeSinceStartup;
#endif
            float timeEnd = Time.realtimeSinceStartup + 0.01f;
            LastDispatcherCount = 0;
            while (ReceiveMessages0.Count > 0 && Time.realtimeSinceStartup < timeEnd)
            {
                ++LastDispatcherCount;
                NetMsg netMsg = ReceiveMessages0.Dequeue();

                if (EventDispatcher.Instance.TryGetEventHandle(netMsg.evtId, out NetEventHandle handler))
                {
                    if (handler.Parser != null)
                    {
                        netMsg.stream.Position = 0;
                        if (NetMsgUtil.TryDeserialize(handler.Parser, netMsg.stream, out IMessage message))
                        {
                            netMsg.stream.Dispose();
                            netMsg.data = message;
                        }
                        else
                        {                            
#if DEBUG_MODE
                            DebugUtil.LogErrorFormat("event = {0} Parser = {1} size = {2} buffer = {3} 解析错误", netMsg.evtId.ToString(), handler.Parser.GetType().ToString(), netMsg.bodyLength.ToString(), netMsg.stream.Length.ToString());
#endif
                            netMsg.stream.Dispose();
                            continue;
                        }                        
                    }

#if DEBUG_MODE
                    if (DebugUtil.IsOpenLogType(ELogType.eNetProcessMSG) && EventDispatcher.Instance.FilterLog(netMsg.evtId))
                    {
                        string json = string.Empty;
                        try
                        {
                            json = netMsg.data == null ? string.Empty : LitJson.JsonMapper.ToJson(netMsg.data);
                        }
                        catch
                        {
                            json = "{...}";
                        }

                        DebugUtil.LogFormat(ELogType.eNetProcessMSG, "{0} byte[{1}]={2}", netMsg.evtId.ToString(), netMsg.bodyLength.ToString(), json);
                    }
#endif

                    try
                    {
                        handler.Func(netMsg);
                    }
                    catch (Exception e)
                    {                        
                        DebugUtil.LogErrorFormat("协议 {0} 处理失败：{1}\n{2}", netMsg.evtId.ToString(), e.Message, e.StackTrace);
                    }
                }
            }

#if DEBUG_MODE
            LastExecuteTime = (int)((UnityEngine.Time.realtimeSinceStartup - t) * 1000000);
            LastExecuteFrame = Time.frameCount;
#endif
        }
        private void _ClearBuffers()
        {
            mMessageReceiveBuffer.Reset();                       

            while (SendMessages0.Count > 0)
            {
                NetMsg netMsg = SendMessages0.Dequeue();
                netMsg.stream.Dispose();
            }

            while (SendMessages1.Count > 0)
            {
                NetMsg netMsg = SendMessages1.Dequeue();
                netMsg.stream.Dispose();
            }

            while (ReceiveMessages0.Count > 0)
            {
                NetMsg netMsg = ReceiveMessages0.Dequeue();
                netMsg.stream.Dispose();
            }

            while (ReceiveMessages1.Count > 0)
            {
                NetMsg netMsg = ReceiveMessages1.Dequeue();
                netMsg.stream.Dispose();
            }
        }

#if UNITY_IPHONE && !UNITY_EDITOR
        [System.Runtime.InteropServices.DllImport("__Internal")]
        public static extern string getIPv6(string mHost, string mPort);
#endif
        /// <summary>
        /// 通过IP地址获取IPV6的地址
        /// </summary>
        /// <param name="serverIP"></param>
        /// <param name="serverPort"></param>
        /// <param name="addressFamily"></param>
        /// <returns></returns>
        private string TryGetIpv6ByAddress(string serverIP, string serverPort, out AddressFamily addressFamily)
        {
            string rltIP = serverIP;
            addressFamily = AddressFamily.InterNetwork;

#if UNITY_IPHONE && !UNITY_EDITOR
            try
            {
                string mIPv6 = getIPv6(serverIP, serverPort);
                if (!string.IsNullOrEmpty(mIPv6))
                {
                    string[] temp = System.Text.RegularExpressions.Regex.Split(mIPv6, "&&");
                    if (temp != null && temp.Length >= 2)
                    {
                        string IPType = temp[1];
                        if (IPType == "ipv6")
                        {
                            rltIP = temp[0];
                            addressFamily = AddressFamily.InterNetworkV6;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                DebugUtil.LogException(e);
            }
#endif
            return rltIP;
        }

        /// <summary>
        /// 通过域名获取IPV6的地址
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="addressFamily"></param>
        /// <returns></returns>
        private string TryGetIpv6ByHostName(string hostName, out AddressFamily addressFamily)
        {
            addressFamily = AddressFamily.InterNetwork;
            if (System.Net.Sockets.Socket.OSSupportsIPv6 || string.IsNullOrEmpty(hostName))
                return hostName;

            System.Net.IPAddress[] address = Dns.GetHostAddresses(hostName);
            if (address != null && address.Length > 0)
            {
                if (address[0].AddressFamily == AddressFamily.InterNetworkV6)
                {
                    addressFamily = AddressFamily.InterNetworkV6;
                    return address[0].ToString();
                }
                else
                {
                    addressFamily = AddressFamily.InterNetwork;
                    return address[0].ToString();
                }
            }
            return hostName;
        }
    }
}