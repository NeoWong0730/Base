using Logic.Core;
using System.Collections.Generic;
using System.Json;

namespace Logic
{
    public partial class Sys_Society : SystemModuleBase<Sys_Society>
    {
        /// <summary>
        /// 所有群组聊天信息///
        /// </summary>
        public class GroupChatDataInfo
        {
            /// <summary>
            /// 群聊天信息集合///
            /// </summary>
            public Dictionary<uint, ChatsInfo> groupChatDatas = new Dictionary<uint, ChatsInfo>();
            public List<ChatsInfo> groupChatDatasList = new List<ChatsInfo>();

            /// <summary>
            /// 添加客户端产生的消息///
            /// </summary>
            /// <param name="groupID"></param>
            /// <param name="sendRoleID"></param>
            /// <param name="msg"></param>
            /// <param name="extraMsg"></param>
            public void AddClientChatData(uint groupID, ulong sendRoleID, string msg, string extraMsg)
            {
                ChatsInfo chatsInfo;
                if (groupChatDatas.TryGetValue(groupID, out chatsInfo))
                {
                    chatsInfo.AddClientChatData(sendRoleID, msg, extraMsg, false, Sys_Head.Instance.clientHead.chatFrameId, Sys_Head.Instance.clientHead.chatTextId);
                }
                else
                {
                    chatsInfo = new ChatsInfo();
                    chatsInfo.ID = groupID;
                    chatsInfo.AddClientChatData(sendRoleID, msg, extraMsg, false, Sys_Head.Instance.clientHead.chatFrameId, Sys_Head.Instance.clientHead.chatTextId);
                    groupChatDatas[groupID] = chatsInfo;
                }
            }

            /// <summary>
            /// 添加服务器发来的消息///
            /// </summary>
            /// <param name="groupID"></param>
            /// <param name="sendRoleID"></param>
            /// <param name="msg"></param>
            /// <param name="extraMsg"></param>
            /// <param name="frameID"></param>
            /// <param name="chatTextID"></param>
            /// <returns></returns>
            public ChatData AddServerChatData(uint groupID, ulong sendRoleID, string msg, string extraMsg, uint frameID, uint chatTextID)
            {
                ChatData chatData;
                ChatsInfo chatsInfo;
                uint sendTime = Sys_Time.Instance.GetServerTime();
                if (groupChatDatas.TryGetValue(groupID, out chatsInfo))
                {
                    chatData = chatsInfo.AddServerChatData(sendRoleID, msg, extraMsg, sendTime, sendTime - chatsInfo.lastChatTime > 300, frameID, chatTextID);
                    chatsInfo.lastChatTime = Sys_Time.Instance.GetServerTime();
                }
                else
                {
                    chatsInfo = new ChatsInfo();
                    chatData = chatsInfo.AddServerChatData(sendRoleID, msg, extraMsg, Sys_Time.Instance.GetServerTime(), sendTime - chatsInfo.lastChatTime > 300, frameID, chatTextID);
                    groupChatDatas[groupID] = chatsInfo;
                }
                return chatData;
            }

            /// <summary>
            /// 清空某个群聊消息///
            /// </summary>
            /// <param name="groupID"></param>
            public void ClearGroupChatData(uint groupID)
            {
                ChatsInfo chatsInfo;
                if (groupChatDatas.TryGetValue(groupID, out chatsInfo))
                {
                    chatsInfo.chatsInfo.Clear();
                }
            }
        }
    }
}