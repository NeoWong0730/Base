using Framework;
using UnityEngine;
using Lib;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Logic
{
    public static class GameMain
    {
        public enum EState
        {
            None,
            Running,
            PreloadShader,
            PreloadTable,
            PreloadAsset,
            RegisterSystem,
            InitSystem,
        }

        private static EState eState = EState.None;
        private static System.Threading.Tasks.Task _loadCsvTask;


        public static void Start()
        {
            //var host = IPAddress.Parse("127.0.0.1");
            //int port = 32510;
            //IPEndPoint iPEndPoint = new IPEndPoint(host, port);
            //Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //socket.Connect(iPEndPoint);

            //string text = "123";
            //byte[] body = Encoding.UTF8.GetBytes(text);
            //SendMessage(socket, body);

#if DEBUG_MODE
            DebugUtil.OpenLogType(ELogType.eNone);
            DebugUtil.OpenLogType(ELogType.eScene);

            float timePoint = Time.realtimeSinceStartup;
#endif
        }



        public static void Exit()
        {
            
        }

        public static void Update()
        {

        }

        public static void LateUpdate()
        {

        }

        public static void OnFixedUpdate()
        {

        }

        public static void OnLowMemory()
        {

        }

        public static void OnGUI()
        {
            
        }

        public static void OnApplicationPause(bool pause)
        {

        }



        //static void SendMessage(Socket socket, byte[] body)
        //{
        //    byte[] lenBytes = BitConverter.GetBytes(body.Length);
        //    socket.Send(lenBytes);
        //    socket.Send(body);
        //}
    }

}