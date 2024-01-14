using Google.Protobuf;
using Logic.Core;
using Net;
using Packet;
using System;
using System.Collections.Generic;
using System.Json;
using Table;

namespace Logic
{
    public partial class Sys_Society : SystemModuleBase<Sys_Society>
    {
        /// <summary>
        /// 聊天数据类///
        /// </summary>
        public class ChatData
        {
            /// <summary>
            /// 消息发送者ID///
            /// </summary>
            public ulong roleID;

            /// <summary>
            /// 消息内容///
            /// </summary>
            public string content;

            /// <summary>
            /// 附加数据///
            /// </summary>
            public string extraContent;

            /// <summary>
            /// 发送时间///
            /// </summary>
            public uint sendTime;

            /// <summary>
            /// 消息是否已读(废弃)///
            /// </summary>
            public bool read;

            /// <summary>
            /// 是否显示发送时间///
            /// </summary>
            public bool needShowTimeText;

            /// <summary>
            /// 附加参数///
            /// </summary>
            public ulong paramID;

            /// <summary>
            /// 消息框格式ID///
            /// </summary>
            public uint frameID = DEFAULTCHATFRAMEID;

            /// <summary>
            /// 消息文本格式ID///
            /// </summary>
            public uint chatTextID = DEFAULTCHATTEXTID;

            /// <summary>
            /// 反序列化后的附加信息///
            /// </summary>
            [NonSerialized]
            ChatExtMsg mChatExtData;

            /// <summary>
            /// 获取附加信息///
            /// </summary>
            /// <returns></returns>
            public ChatExtMsg GetChatExtMsg()
            {
                return mChatExtData;
            }

            /// <summary>
            /// 反序列化///
            /// </summary>
            /// <param name="jo"></param>
            public void DeserializeObject(JsonObject jo)
            {
                JsonHeler.DeserializeObject(jo, this);
                FillChatExtData();
            }

            /// <summary>
            /// 反序列化附加信息///
            /// </summary>
            public void FillChatExtData()
            {
                if (extraContent != null)
                {
                    ChatExtMsg temp = null;
                    var datas = ByteString.FromBase64(extraContent);
                    NetMsgUtil.TryDeserialize<ChatExtMsg>(ChatExtMsg.Parser, datas, out temp);
                    mChatExtData = temp;
                }
            }
        }

        /// <summary>
        /// 一个玩家或一个群的聊天信息///
        /// </summary>
        public class ChatsInfo
        {
            /// <summary>
            /// 所属玩家或群的ID///
            /// </summary>
            public ulong ID;

            /// <summary>
            /// 最近消息产生时间///
            /// </summary>
            public uint lastChatTime;

            /// <summary>
            /// 是否看了所有消息(设定为点击了对应的玩家或群后就表示为看过所有消息，同微信)///
            /// </summary>
            public bool allRead = true;

            /// <summary>
            /// 所有聊天信息集合///
            /// </summary>
            public List<ChatData> chatsInfo = new List<ChatData>();
            const string chatsInfoFieldName = "chatsInfo";

            /// <summary>
            /// 反序列化本地持久化的聊天数据///
            /// </summary>
            /// <param name="jo"></param>
            public void DeserializeObject(JsonObject jo)
            {
                JsonHeler.DeserializeObject(jo, this);
                chatsInfo.Clear();
                if (jo.ContainsKey(chatsInfoFieldName))
                {
                    JsonArray ja = (JsonArray)jo[chatsInfoFieldName];
                    foreach (var item in ja)
                    {
                        ChatData chatData = new ChatData();
                        JsonObject jo2 = (JsonObject)item;
                        chatData.DeserializeObject(jo2);
                        chatsInfo.Add(chatData);
                    }
                }

                DeleteExtraChatInfo();
            }

            void DeleteExtraChatInfo()
            {
                if (chatsInfo.Count > int.Parse(CSVParam.Instance.GetConfData(1361).str_value))
                {
                    chatsInfo.RemoveRange(0, chatsInfo.Count - int.Parse(CSVParam.Instance.GetConfData(1361).str_value) + int.Parse(CSVParam.Instance.GetConfData(1362).str_value));
                }

                int flagIndex = 0;
                for (int index = 0, len = chatsInfo.Count; index < len; index++)
                {
                    uint currentTime = Sys_Time.Instance.GetServerTime();
                    int timeLimit = int.Parse(CSVParam.Instance.GetConfData(1360).str_value);
                    if (currentTime- chatsInfo[index].sendTime > timeLimit)
                    {
                        flagIndex = index;                     
                    }
                    else
                    {
                        break;
                    }
                }

                if (flagIndex > 0)
                    chatsInfo.RemoveRange(0, flagIndex);
            }

            /// <summary>
            /// 添加客户端产生的消息(玩家自己发送的消息)///
            /// </summary>
            /// <param name="ChatCreateRoleID"></param>
            /// <param name="msg"></param>
            /// <param name="extraMsg"></param>
            /// <param name="needShowTimeText"></param>
            /// <param name="frameID"></param>
            /// <param name="chatTextID"></param>
            /// <returns></returns>
            public ChatData AddClientChatData(ulong ChatCreateRoleID, string msg, string extraMsg, bool needShowTimeText, uint frameID, uint chatTextID)
            {
                ChatData chatData = new ChatData();
                chatData.roleID = ChatCreateRoleID;
                chatData.content = msg;
                chatData.extraContent = extraMsg;
                chatData.sendTime = Sys_Time.Instance.GetServerTime();
                chatData.needShowTimeText = needShowTimeText;            
                chatData.frameID = frameID;
                chatData.chatTextID = chatTextID;
                chatData.FillChatExtData();
                chatsInfo.Add(chatData);

                lastChatTime = Sys_Time.Instance.GetServerTime();

                return chatData;
            }

            /// <summary>
            /// 添加服务器发来的消息(其它玩家发送的消息)///
            /// </summary>
            /// <param name="ChatCreateRoleID"></param>
            /// <param name="msg"></param>
            /// <param name="extraMsg"></param>
            /// <param name="sendTime"></param>
            /// <param name="needShowTimeText"></param>
            /// <param name="frameID"></param>
            /// <param name="chatTextID"></param>
            /// <returns></returns>
            public ChatData AddServerChatData(ulong ChatCreateRoleID, string msg, string extraMsg, uint sendTime, bool needShowTimeText, uint frameID, uint chatTextID)
            {
                ChatData chatData = new ChatData();
                chatData.roleID = ChatCreateRoleID;
                chatData.content = msg;
                chatData.extraContent = extraMsg;
                chatData.sendTime = sendTime;
                chatData.needShowTimeText = needShowTimeText;              
                chatData.frameID = frameID;
                chatData.chatTextID = chatTextID;
                chatData.FillChatExtData();
                chatsInfo.Add(chatData);

                allRead = false;
                lastChatTime = Sys_Time.Instance.GetServerTime();

                return chatData;
            }
        }

        /// <summary>
        /// 私聊用户聊天索引///
        /// </summary>
        public class RoleChatRefDataInfo
        {
            /// <summary>
            /// 私聊用户ID索引集合///
            /// </summary>
            public Dictionary<ulong, InfoID> roleChatRefDatas = new Dictionary<ulong, InfoID>();
            public List<InfoID> roleChatRefDatasList = new List<InfoID>();
            const string roleChatRefDatasListfieldName = "roleChatRefDatasList";

            /// <summary>
            /// 反序列化///
            /// </summary>
            /// <param name="jo"></param>
            public void DeserializeObject(JsonObject jo)
            {
                roleChatRefDatas.Clear();
                roleChatRefDatasList.Clear();
                if (jo.ContainsKey(roleChatRefDatasListfieldName))
                {
                    JsonArray ja = (JsonArray)jo[roleChatRefDatasListfieldName];
                    foreach (var item in ja)
                    {
                        InfoID roleInfoID = new InfoID();
                        roleInfoID.DeserializeObject((JsonObject)item);
                        roleChatRefDatas[roleInfoID.infoID] = roleInfoID;
                    }
                }
            }

            /// <summary>
            /// 添加私聊用户ID索引///
            /// </summary>
            /// <param name="targetRoleID"></param>
            public void AddRoleChatRefDataInfo(ulong targetRoleID)
            {
                if (!roleChatRefDatas.ContainsKey(targetRoleID))
                {
                    InfoID infoID = new InfoID();
                    infoID.infoID = targetRoleID;
                    roleChatRefDatas[targetRoleID] = infoID;
                }
            }
        }

        /// <summary>
        ///  群聊天索引///
        /// </summary>
        public class GroupChatRefDataInfo
        {
            /// <summary>
            /// 群聊天ID索引集合///
            /// </summary>
            public Dictionary<ulong, InfoID> groupChatRefDatas = new Dictionary<ulong, InfoID>();
            public List<InfoID> groupChatRefDatasList = new List<InfoID>();
            const string groupChatRefDatasListFieldName = "groupChatRefDatasList";

            /// <summary>
            /// 反序列化///
            /// </summary>
            /// <param name="jo"></param>
            public void DeserializeObject(JsonObject jo)
            {
                groupChatRefDatas.Clear();
                groupChatRefDatasList.Clear();
                if (jo.ContainsKey(groupChatRefDatasListFieldName))
                {
                    JsonArray ja = (JsonArray)jo[groupChatRefDatasListFieldName];
                    foreach (var item in ja)
                    {
                        InfoID groupInfoID = new InfoID();
                        groupInfoID.DeserializeObject((JsonObject)item);
                        groupChatRefDatas[groupInfoID.infoID] = groupInfoID;
                    }
                }
            }

            /// <summary>
            /// 添加群组聊天ID索引///
            /// </summary>
            /// <param name="targetRoleID"></param>
            public void AddGroupChatRefDataInfo(ulong groupID)
            {
                if (!groupChatRefDatas.ContainsKey(groupID))
                {
                    InfoID infoID = new InfoID();
                    infoID.infoID = groupID;
                    groupChatRefDatas[groupID] = infoID;
                }
            }
        }
    }
}