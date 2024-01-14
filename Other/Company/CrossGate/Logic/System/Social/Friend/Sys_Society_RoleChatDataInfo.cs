using Logic.Core;
using System.Collections.Generic;

namespace Logic
{
    public partial class Sys_Society : SystemModuleBase<Sys_Society>
    {
        /// <summary>
        /// 所有单人聊天信息///
        /// </summary>
        public class RoleChatDataInfo
        {
            /// <summary>
            /// 私聊天信息集合///
            /// </summary>
            public Dictionary<ulong, ChatsInfo> roleChatDatas = new Dictionary<ulong, ChatsInfo>();
            public List<ChatsInfo> roleChatDatasList = new List<ChatsInfo>();

            /// <summary>
            /// 添加客户端产生的消息///
            /// </summary>
            /// <param name="targetRoleID"></param>
            /// <param name="msg"></param>
            /// <param name="extraMsg"></param>
            /// <returns></returns>
            public ChatData AddClientChatData(ulong targetRoleID, ulong sendRoleID, string msg, string extraMsg)
            {
                ChatsInfo chatsInfo;
                ChatData chatData;
                if (roleChatDatas.TryGetValue(targetRoleID, out chatsInfo))
                {
                    chatData = chatsInfo.AddClientChatData(sendRoleID, msg, extraMsg, (Sys_Time.Instance.GetServerTime() - chatsInfo.lastChatTime > 300), Sys_Head.Instance.clientHead.chatFrameId, Sys_Head.Instance.clientHead.chatTextId);                   
                }
                else
                {
                    chatsInfo = new ChatsInfo();
                    chatData = chatsInfo.AddClientChatData(sendRoleID, msg, extraMsg, true, Sys_Head.Instance.clientHead.chatFrameId, Sys_Head.Instance.clientHead.chatTextId);
                    roleChatDatas[targetRoleID] = chatsInfo;
                    chatsInfo.ID = targetRoleID;
                }               

                RoleInfo roleInfo;
                if (Instance.socialRolesInfo.rolesDic.TryGetValue(targetRoleID, out roleInfo))
                {
                    roleInfo.lastChatTime = chatsInfo.lastChatTime;
                }
                return chatData;
            }

            /// <summary>
            /// 添加服务器发来的消息///
            /// </summary>
            /// <param name="sendRoleID"></param>
            /// <param name="msg"></param>
            /// <param name="extraMsg"></param>
            /// <param name="sendTime"></param>
            /// <param name="frameID"></param>
            /// <param name="chatTextID"></param>
            /// <returns></returns>
            public ChatData AddServerChatData(ulong sendRoleID, string msg, string extraMsg, uint sendTime, uint frameID, uint chatTextID)
            {
                ChatsInfo chatsInfo;
                ChatData chatData;
                if (roleChatDatas.TryGetValue(sendRoleID, out chatsInfo))
                {
                    chatData = chatsInfo.AddServerChatData(sendRoleID, msg, extraMsg, sendTime, (Sys_Time.Instance.GetServerTime() - chatsInfo.lastChatTime) > 300, frameID, chatTextID);                   
                }
                else
                {
                    chatsInfo = new ChatsInfo();
                    chatData = chatsInfo.AddServerChatData(sendRoleID, msg, extraMsg, sendTime, true, frameID, chatTextID);
                    roleChatDatas[sendRoleID] = chatsInfo;
                    chatsInfo.ID = sendRoleID;
                }
             
                RoleInfo roleInfo;
                if (Instance.socialRolesInfo.rolesDic.TryGetValue(sendRoleID, out roleInfo))
                {
                    roleInfo.lastChatTime = chatsInfo.lastChatTime;
                }
                return chatData;
            }

            /// <summary>
            /// 清空某个私聊消息///
            /// </summary>
            /// <param name="roleID"></param>
            public void ClearRoleChatData(ulong roleID)
            {
                ChatsInfo chatsInfo;
                if (roleChatDatas.TryGetValue(roleID, out chatsInfo))
                {
                    chatsInfo.chatsInfo.Clear();
                    chatsInfo.allRead = true;
                }
            }
        }
    }
}