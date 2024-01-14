using Lib.Core;
using Logic.Core;
using System.Text;
using System.Collections.Generic;

namespace Logic
{
    public partial class Sys_Society : SystemModuleBase<Sys_Society>
    {
        public readonly string MYFRIEND = "我的好友";
        public readonly string TEAMPLAYER = "最近队友";
        public readonly string BLACK = "黑名单";

        #region RoleSocicalData

        public uint Sex
        {
            get;
            private set;
        }

        public uint Age
        {
            get;
            private set;
        }

        public uint Area
        {
            get;
            private set;
        }

        public List<uint> Hobbys
        {
            get;
            private set;
        } = new List<uint>();

        public ulong CacheLastSearchRoleID
        {
            get;
            private set;
        }

        #endregion

        public bool sendOpenReqFlag = true;
        Timer sendOpenReqTimer;
        float sendOpenReqCd = 30.0f;

        /// <summary>
        /// 最近聊天最大数量///
        /// </summary>
        uint recentlyMaxCount;

        /// <summary>
        /// 好友最大数量///
        /// </summary>
        public uint friendsMaxCount;

        /// <summary>
        /// 最近队友最大数量///
        /// </summary>
        uint teammemberMaxCount;

        /// <summary>
        /// 黑名单最大数量///
        /// </summary>
        public uint blacksMaxCount;

        /// <summary>
        /// 群组最大数量///
        /// </summary>
        public uint GroupMaxCount;

        public InputCache inputCache = new InputCache();
        public InputCacheRecord mInputCacheRecord;

        public SocialRolesInfo socialRolesInfo = new SocialRolesInfo();      
        public SocialRecentlysInfo socialRecentlysInfo = new SocialRecentlysInfo();     
        public SocialFriendsInfo socialFriendsInfo = new SocialFriendsInfo();      
        public SocialTeamMembersInfo socialTeamMembersInfo = new SocialTeamMembersInfo();       
        public SocialBlacksInfo socialBlacksInfo = new SocialBlacksInfo();
        public SocialGroupsInfo socialGroupsInfo = new SocialGroupsInfo();
        public SocialFriendGroupsInfo socialFriendGroupsInfo = new SocialFriendGroupsInfo();
        public RoleChatRefDataInfo roleChatRefDataInfo = new RoleChatRefDataInfo();
        public RoleChatDataInfo roleChatDataInfo = new RoleChatDataInfo();
        public GroupChatRefDataInfo groupChatRefDataInfo = new GroupChatRefDataInfo();
        public GroupChatDataInfo groupChatDataInfo = new GroupChatDataInfo();
                   
        void OnInputChange()
        {
            eventEmitter.Trigger(EEvents.InputChange);
        }


        /// <summary>
        /// 清空个人聊天信息///
        /// </summary>
        /// <param name="roleID"></param>
        public void ClearSingleChatLog(ulong roleID)
        {
            roleChatDataInfo.ClearRoleChatData(roleID);
            if (roleChatDataInfo.roleChatDatas.ContainsKey(roleID))
            {
                SerializeChatsInfoToJsonFile(roleChatDataInfo.roleChatDatas[roleID], string.Format(chatsPath, Sys_Role.Instance.RoleId.ToString(), roleID.ToString()));
            }
            eventEmitter.Trigger<ulong>(EEvents.OnClearSingleChatLog, roleID);
        }


        /// <summary>
        /// 清空群聊信息///
        /// </summary>
        /// <param name="groupID"></param>
        public void ClearGroupChatLog(uint groupID)
        {
            groupChatDataInfo.ClearGroupChatData(groupID);
            if (groupChatDataInfo.groupChatDatas.ContainsKey(groupID))
            {
                SerializeGroupChatsInfoToJsonFile(groupChatDataInfo.groupChatDatas[groupID], string.Format(groupChatsPath, Sys_Role.Instance.RoleId.ToString(), groupID.ToString()));
            }
            eventEmitter.Trigger<uint>(EEvents.OnClearGroupChatLog, groupID);
        }

        /// <summary>
        /// 看了私聊消息(即点了对应的玩家)///
        /// </summary>
        /// <param name="roleID"></param>
        void OnReadRoleChat(ulong roleID)
        {
            if (roleChatDataInfo.roleChatDatas.ContainsKey(roleID))
            {
                roleChatDataInfo.roleChatDatas[roleID].allRead = true;

                SerializeChatsInfoToJsonFile(roleChatDataInfo.roleChatDatas[roleID], string.Format(chatsPath, Sys_Role.Instance.RoleId.ToString(), roleID.ToString()));
                object[] objs = new object[1];
                objs[0] = roleID;
                RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnReadRoleChat, objs);
            }
        }

        /// <summary>
        /// 是否存在未读的私聊信息///
        /// </summary>
        /// <returns></returns>
        public bool HasUnReadRoleChat()
        {
            foreach (var chatInfos in roleChatDataInfo.roleChatDatas.Values)
            {
                if (!chatInfos.allRead)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 是否存在未领取的礼物///
        /// </summary>
        /// <returns></returns>
        public bool HasUnGetGift()
        {
            foreach (var roleInfo in socialRolesInfo.rolesDic.Values)
            {
                if (socialFriendsInfo.friendsIdsDic.ContainsKey(roleInfo.roleID) && roleInfo.hasGift)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 是否存在未读的最近私聊消息///
        /// </summary>
        /// <returns></returns>
        public bool HasUnReadRecentRoleChat()
        {
            foreach (var chatInfos in roleChatDataInfo.roleChatDatas.Values)
            {
                if (!socialRecentlysInfo.recentlyIdsDic.ContainsKey(chatInfos.ID))
                {
                    continue;
                }
                if (!chatInfos.allRead)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 是否存在未领取的最近礼物///
        /// </summary>
        /// <returns></returns>
        public bool HasUnGetGiftRecent()
        {
            foreach (var roleInfo in socialRolesInfo.rolesDic.Values)
            {
                if (socialFriendsInfo.friendsIdsDic.ContainsKey(roleInfo.roleID) && socialRecentlysInfo.recentlyIdsDic.ContainsKey(roleInfo.roleID) && roleInfo.hasGift)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 是否存在未读的好友私聊消息///
        /// </summary>
        /// <returns></returns>
        public bool HasUnReadFriendRoleChat()
        {
            foreach (var chatInfos in roleChatDataInfo.roleChatDatas.Values)
            {
                if (!socialFriendsInfo.friendsIdsDic.ContainsKey(chatInfos.ID))
                {
                    continue;
                }
                if (!chatInfos.allRead)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 是否存在未领取的好友礼物///
        /// </summary>
        /// <returns></returns>
        public bool HasUnGetGiftFriend()
        {
            foreach (var roleInfo in socialRolesInfo.rolesDic.Values)
            {
                if (socialFriendsInfo.friendsIdsDic.ContainsKey(roleInfo.roleID) && roleInfo.hasGift)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 是否存在未读的群聊信息///
        /// </summary>
        /// <returns></returns>
        public bool HasUnReadGroupChat()
        {
            foreach (var chatInfos in groupChatDataInfo.groupChatDatas.Values)
            {
                if (!chatInfos.allRead)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 该角色是否无未读消息///
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public bool IsRoleChatAllRead(ulong roleID)
        {
            if (roleChatDataInfo.roleChatDatas.ContainsKey(roleID))
            {
                if (!roleChatDataInfo.roleChatDatas[roleID].allRead)
                    return false;
            }
            return true;
        }

        public bool IsRoleGiftAllGet(ulong roleID)
        {
            if (socialRolesInfo.rolesDic.ContainsKey(roleID))
            {
                return !socialRolesInfo.rolesDic[roleID].hasGift;
            }
            return true;
        }

        public bool IsRoleRedPointShow(ulong roleID)
        {
            if (!IsRoleChatAllRead(roleID) || !IsRoleGiftAllGet(roleID))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 该群是否无未读消息///
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool IsGroupChatAllRead(uint groupID)
        {
            if (groupChatDataInfo.groupChatDatas.ContainsKey(groupID))
            {
                if (!groupChatDataInfo.groupChatDatas[groupID].allRead)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 获取好友的总数///
        /// </summary>
        /// <returns></returns>
        public int GetFriendsCount()
        {
            return socialFriendsInfo.friendsIdsDic.Count;
        }

        /// <summary>
        /// 获取队友的总数///
        /// </summary>
        /// <returns></returns>
        public int GetTeamMembersCount()
        {
            return socialTeamMembersInfo.teamMemberIdsDic.Count;
        }

        /// <summary>
        /// 获取在线的好友数量///
        /// </summary>
        /// <returns></returns>
        public int GetOnLineFriendsCount()
        {
            int count = 0;
            foreach (var friend in socialFriendsInfo.GetAllFirendsInfos().Values)
            {
                if (friend.isOnLine)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// 获取在线的队友数量///
        /// </summary>
        /// <returns></returns>
        public int GetOnLineTeamMembersCount()
        {
            int count = 0;
            foreach (var teamMember in socialTeamMembersInfo.GetAllTeamMembersInfos().Values)
            {
                if (teamMember.isOnLine)
                {
                    count++;
                }
            }
            return count;
        }

        public void AddSeverSystemTip(ulong roleID, string messageContent)
        {
            if (!roleChatDataInfo.roleChatDatas.ContainsKey(roleID))
                DeserializeChatsInfoFromJsonFile(string.Format(chatsPath, Sys_Role.Instance.RoleId.ToString(), roleID.ToString()));

            ChatData chatData = roleChatDataInfo.AddClientChatData(roleID, socialSystemTipID, messageContent, string.Empty);
            roleChatDataInfo.roleChatDatas[roleID].allRead = false;
            roleChatRefDataInfo.AddRoleChatRefDataInfo(roleID);

            SerializeChatRefsToJsonFile();
            SerializeChatsInfoToJsonFile(roleChatDataInfo.roleChatDatas[roleID], string.Format(chatsPath, Sys_Role.Instance.RoleId.ToString(), roleID.ToString()));
            object[] objs = new object[1];
            objs[0] = roleID;
            RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnGetChat, objs);
        }
    }
}
