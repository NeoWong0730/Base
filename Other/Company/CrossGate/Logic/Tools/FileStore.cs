using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Json;
using Lib.Core;
using Google.Protobuf;
using System;

namespace Logic
{
    public class FileStore
    {
        static string path = Path.GetFullPath(Application.persistentDataPath) + "/{0}_{1}_{2}.json";
        static string _path02 = Path.GetFullPath(Application.persistentDataPath) + "/{0}.json";

        public static void WriteJson(string fileName, object o)
        {
            if (Sys_Role.Instance.Role == null)
            {
                return;
            }
            string filePath = string.Format(path, Sys_Login.Instance.Account, Sys_Role.Instance.RoleId.ToString(), fileName);
            string json = LitJson.JsonMapper.ToJson(o);

            if (!File.Exists(filePath))
            {
                using (File.Create(filePath)) {/* let using close filestream! */ }
            }
            using (StreamWriter sw = new StreamWriter(filePath, false)) { sw.Write(json); }
        }

        public static JsonObject ReadJson(string fileName)
        {
            string filePath = string.Format(path, Sys_Login.Instance.Account, Sys_Role.Instance.RoleId.ToString(), fileName);
            string jsonStr = JsonHeler.GetJsonStr(filePath);
            if (string.IsNullOrEmpty(jsonStr))
            {
                return null;
            }
            JsonObject jo = (JsonObject)JsonSerializer.Deserialize(jsonStr);

            return jo;
        }

        public static void WriteBytes(string fileName, object data)
        {
            if (Sys_Role.Instance.Role == null)
            {
                return;
            }
            string filePath = string.Format(path, Sys_Login.Instance.Account, Sys_Role.Instance.RoleId.ToString(), fileName);
            if (!File.Exists(filePath))
            {
                using (File.Create(filePath)) {/* let using close filestream! */ }
            }
            using (StreamWriter sw = new StreamWriter(filePath, false)) { sw.Write(data); }
        }

        public static byte[] ReadBytes(string fileName)
        {
            string filePath = string.Format(path, Sys_Login.Instance.Account, Sys_Role.Instance.RoleId.ToString(), fileName);
            string jsonStr = JsonHeler.GetJsonStr(filePath);
            if (string.IsNullOrEmpty(jsonStr))
            {
                return null;
            }
            return System.Text.Encoding.UTF8.GetBytes(jsonStr);
        }

        #region 读写单个proto的IMessage
        public static void WriteProto(string fileName, IMessage message)
        {
            if (string.IsNullOrWhiteSpace(fileName) || message == null)
                return;

            string filePath = string.Format(path, Sys_Login.Instance.Account, Sys_Role.Instance.RoleId.ToString(), fileName);

            byte[] bs = message.ToByteArray();

            File.WriteAllBytes(filePath, bs);
        }

        public static T ReadProto<T>(MessageParser msgParser, string fileName) where T : class, IMessage
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return null;

            string filePath = string.Format(path, Sys_Login.Instance.Account, Sys_Role.Instance.RoleId.ToString(), fileName);
            if (!File.Exists(filePath))
            {
                DebugUtil.Log(ELogType.eFileStore, $"<color=red>不存在文件：{filePath}</color>");
                return null;
            }

            Net.NetMsgUtil.TryDeserialize(msgParser, File.ReadAllBytes(filePath), out T t);
            return t;
        }
        #endregion

        #region 读写多个proto的IMessage
        private static int _maxAddCount;
        private static List<IMessage> _msgList = new List<IMessage>();
        private static List<ushort> _msgFlagList = new List<ushort>();
        /// <summary>
        /// 存储数据内容：List.Count的int值4位 + List(IMessage长度的int值4位 + IMessage的数据长度 + Flag标记【如opcode】数据2位)；
        /// WriteProtoList_Start，WriteProtoList_Add，WriteProtoList_End必须一起使用
        /// </summary>
        public static void WriteProtoList_Start(int maxAddCount = 10)
        {
            _maxAddCount = maxAddCount;
            _msgList.Clear();
            _msgFlagList.Clear();
        }
        public static void WriteProtoList_Add(ushort flag, IMessage msg)
        {
            if (_msgList.Count > _maxAddCount)
            {
                DebugUtil.LogError($"WriteProtoList_Add的数据数量{_msgList.Count.ToString()}超过定义的最大数据数量：{_maxAddCount.ToString()}，不处理Add数据");
                return;
            }
            _msgFlagList.Add(flag);
            _msgList.Add(msg);
        }
        public static bool WriteProtoList_End(string fileName, bool isNeedAccountFileFlag = true)
        {
            bool isWriteSuccess = WriteProtoList(fileName, _msgList, _msgFlagList, isNeedAccountFileFlag);
            _msgList.Clear();
            _msgFlagList.Clear();

            return isWriteSuccess;
        }

        /// <summary>
        /// 存储数据内容：List.Count的int值4位 + List(IMessage长度的int值4位 + IMessage的数据长度 + Flag标记【如opcode】数据2位)
        /// </summary>
        public static bool WriteProtoList(string fileName, List<IMessage> msgList, List<ushort> msgFlagList, 
            bool isNeedAccountFileFlag = true)
        {
            if (string.IsNullOrWhiteSpace(fileName) || msgList == null || msgFlagList == null)
                return false;

            int msgCount = msgList.Count;
            if (msgCount < 1)
                return false;

            int msgFlagCount = msgFlagList.Count;
            if (msgCount != msgFlagCount)
            {
                DebugUtil.LogError($"msgList.Count：{msgCount.ToString()}和msgFlagList.Count：{msgFlagCount.ToString()}数量不相等");
                return false;
            }

            List<byte[]> bsList = new List<byte[]>();
            int byteLen = 0;
            int bsCount = 0;
            for (int i = 0; i < msgCount; i++)
            {
                IMessage msg = msgList[i];
                if (msg == null)
                {
                    bsList.Add(null);
                    continue;
                }

                byte[] bs = msg.ToByteArray();
                if (bs == null)
                {
                    bsList.Add(null);
                    continue;
                }

                bsList.Add(bs);

                byteLen += bs.Length;

                ++bsCount;
            }

            if (byteLen == 0)
            {
                DebugUtil.LogError($"文件：{fileName}写入消息列表的总长度为0");
                return false;
            }

            string filePath;
            if (isNeedAccountFileFlag)
                filePath = string.Format(path, Sys_Login.Instance.Account, Sys_Role.Instance.RoleId.ToString(), fileName);
            else
                filePath = string.Format(_path02, fileName);

            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read, 4 + (4 + 2) * bsCount + byteLen))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(bsCount);
                    for (int i = 0; i < msgCount; i++)
                    {
                        byte[] bs = bsList[i];
                        if (bs == null)
                            continue;

                        bw.Write(bs.Length);
                        bw.Write(bs);
                        bw.Write(msgFlagList[i]);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 存储数据内容：List.Count的int值4位 + List(IMessage长度的int值4位 + IMessage的数据长度 + Flag标记【如opcode】数据2位)
        /// </summary>
        public static bool ReadProtoList(string fileName, Action<ushort, byte[]> action,
            bool isNeedAccountFileFlag = true)
        {
            if (string.IsNullOrWhiteSpace(fileName) || action == null)
                return false;

            string filePath;
            if (isNeedAccountFileFlag)
                filePath = string.Format(path, Sys_Login.Instance.Account, Sys_Role.Instance.RoleId.ToString(), fileName);
            else
                filePath = string.Format(_path02, fileName);

            if (!File.Exists(filePath))
            {
                DebugUtil.Log(ELogType.eFileStore, $"<color=red>不存在文件：{filePath}</color>");
                return false;
            }

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    int bsCount = br.ReadInt32();
                    for (int i = 0; i < bsCount; i++)
                    {
                        int bsLen = br.ReadInt32();
                        byte[] bs = br.ReadBytes(bsLen);

                        ushort flag = br.ReadUInt16();

                        action(flag, bs);
                    }
                }
            }

            return true;
        }
        /// <summary>
        /// 存储数据内容：List.Count的int值4位 + List(IMessage长度的int值4位 + IMessage的数据长度 + Flag标记【如opcode】数据2位)
        /// </summary>
        /// <param name="flagMsgParserDic">flag对应的MessageParser</param>
        public static bool ReadProtoList(string fileName, Dictionary<ushort, MessageParser> flagMsgParserDic, 
            Action<ushort, IMessage> action, bool isNeedAccountFileFlag = true)
        {
            if (flagMsgParserDic == null || action == null)
                return false;

            return ReadProtoList(fileName, (ushort flag, byte[] bs) =>
            {
                MessageParser messageParser;
                if (!flagMsgParserDic.TryGetValue(flag, out messageParser))
                {
                    DebugUtil.LogError($"ReadProtoList  解析的Flag：{flag}没有填入对应的MessageParser");
                    return;
                }

                Net.NetMsgUtil.TryDeserialize(messageParser, bs, out IMessage t);
                if (t == null)
                    return;

                action(flag, t);
            }, isNeedAccountFileFlag);
        }
        #endregion

        public static void DelProtoFile(string fileName, bool isNeedAccountFileFlag = true)
        {
            string filePath;
            if (isNeedAccountFileFlag)
                filePath = string.Format(path, Sys_Login.Instance.Account, Sys_Role.Instance.RoleId.ToString(), fileName);
            else
                filePath = string.Format(_path02, fileName);

            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}
