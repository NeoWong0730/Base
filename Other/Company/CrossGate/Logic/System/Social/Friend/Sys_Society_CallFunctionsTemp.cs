using Framework;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using System.Collections.Generic;
using Table;

namespace Logic
{
    public partial class Sys_Society : SystemModuleBase<Sys_Society>
    {
        /// <summary>
        /// 接受添加好友请求///
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="name"></param>
        public void AcceptAddFriendReq(ulong roleID, string name)
        {
            CmdSocialHandleFriendRequestReq req = new CmdSocialHandleFriendRequestReq();
            req.RoleId = roleID;
            req.Name = FrameworkTool.ConvertToGoogleByteString(name);
            req.Agree = true;

            NetClient.Instance.SendMessage((ushort)CmdSocial.HandleFriendRequestReq, req);
        }

        /// <summary>
        /// 拒绝添加好友请求///
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="name"></param>
        public void RefuseAddFriendReq(ulong roleID, string name)
        {
            CmdSocialHandleFriendRequestReq req = new CmdSocialHandleFriendRequestReq();
            req.RoleId = roleID;
            req.Name = FrameworkTool.ConvertToGoogleByteString(name);
            req.Agree = false;

            NetClient.Instance.SendMessage((ushort)CmdSocial.HandleFriendRequestReq, req);
        }

        /// <summary>
        /// 获取亲密度///
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public uint GetFriendValueByID(ulong roleID)
        {
            if (socialRolesInfo.rolesDic.ContainsKey(roleID))
                return socialRolesInfo.rolesDic[roleID].friendValue;

            return 0;
        }

        /// <summary>
        /// 获取当前亲密度等级信息///
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public CSVFriendIntimacy.Data GetCSVFriendIntimacyDataByID(ulong roleID)
        {
            if (socialRolesInfo.rolesDic.ContainsKey(roleID))
            {
                return CSVFriendIntimacy.Instance.GetDataByIntimacyValue(socialRolesInfo.rolesDic[roleID].friendValue);
            }

            return null;
        }

        public void InsertShareVideo(ulong senderRoleID, Google.Protobuf.Collections.RepeatedField<ulong> targerRoleIDs, string msg, uint sendTime, uint frameForm, uint textForm)
        {
            ///别人发的消息///
            if (senderRoleID != Sys_Role.Instance.RoleId)
            {
                if (IsInBlackList(senderRoleID))
                    return;

                if (!socialRecentlysInfo.recentlyIdsDic.ContainsKey(senderRoleID))
                {
                    socialRecentlysInfo.AddRecentlyInfo(senderRoleID);
                }

                SerializeRecentlysInfoToJsonFile();
                SerializeAllRolesInfoToJsonFile();

                if (!roleChatDataInfo.roleChatDatas.ContainsKey(senderRoleID))
                    DeserializeChatsInfoFromJsonFile(string.Format(chatsPath, Sys_Role.Instance.RoleId.ToString(), senderRoleID.ToString()));

                ChatData chatData = roleChatDataInfo.AddServerChatData(senderRoleID, msg, string.Empty, sendTime, frameForm, textForm);
                roleChatRefDataInfo.AddRoleChatRefDataInfo(senderRoleID);

                SerializeChatRefsToJsonFile();
                SerializeChatsInfoToJsonFile(roleChatDataInfo.roleChatDatas[senderRoleID], string.Format(chatsPath, Sys_Role.Instance.RoleId.ToString(), senderRoleID.ToString()));

                eventEmitter.Trigger<ChatData>(EEvents.OnGetSingleChat, chatData);
                object[] objs = new object[1];
                objs[0] = senderRoleID;
                RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnGetChat, objs);
            }
            else
            {
                for (int index = 0, len = targerRoleIDs.Count; index < len; index++)
                {
                    if (!roleChatDataInfo.roleChatDatas.ContainsKey(targerRoleIDs[index]))
                        DeserializeChatsInfoFromJsonFile(string.Format(chatsPath, Sys_Role.Instance.RoleId.ToString(), targerRoleIDs[index].ToString()));

                    ChatData chatData = roleChatDataInfo.AddClientChatData(targerRoleIDs[index], Sys_Role.Instance.RoleId, msg, null);
                    roleChatRefDataInfo.AddRoleChatRefDataInfo(targerRoleIDs[index]);

                    SerializeChatRefsToJsonFile();
                    SerializeChatsInfoToJsonFile(roleChatDataInfo.roleChatDatas[targerRoleIDs[index]], string.Format(chatsPath, Sys_Role.Instance.RoleId.ToString(), targerRoleIDs[index].ToString()));

                    eventEmitter.Trigger(EEvents.OnSendSingleChat);
                }
                UIManager.OpenUI(EUIID.UI_Society, false, targerRoleIDs[0]);
            }
        }
    }
}
