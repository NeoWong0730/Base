using Lib.Core;
using Logic.Core;
using System.Json;

namespace Logic
{
    public partial class Sys_Society : SystemModuleBase<Sys_Society>
    {
        /// <summary>
        /// 反序列化本地持久化的所有玩家信息///
        /// </summary>
        void DeserializeAllRolesInfoFromJsonFile()
        {
            string jsonStr = JsonHeler.GetJsonStr(string.Format(allRolesPath, Sys_Role.Instance.RoleId.ToString()));
            JsonObject jo = (JsonObject)JsonSerializer.Deserialize(jsonStr);
            socialRolesInfo.DeserializeObject(jo);
        }

        /// <summary>
        /// 序列化所有玩家信息保存在本地///
        /// </summary>
        public void SerializeAllRolesInfoToJsonFile()
        {
            socialRolesInfo.rolesList.Clear();
            foreach (RoleInfo roleInfo in socialRolesInfo.rolesDic.Values)
            {
                socialRolesInfo.rolesList.Add(roleInfo);
            }
            JsonHeler.SerializeToJsonFile(socialRolesInfo, string.Format(allRolesPath, Sys_Role.Instance.RoleId.ToString()));
        }

        /// <summary>
        /// 反序列化本地持久化的所有最近联系人索引信息///
        /// </summary>
        void DeserializeRecentlysInfoFromJsonFile()
        {
            string jsonStr = JsonHeler.GetJsonStr(string.Format(recentlysPath, Sys_Role.Instance.RoleId.ToString()));
            JsonObject jo = (JsonObject)JsonSerializer.Deserialize(jsonStr);
            socialRecentlysInfo.DeserializeObject(jo);
        }

        /// <summary>
        /// 序列化所有最近联系人索引信息保存在本地///
        /// </summary>
        void SerializeRecentlysInfoToJsonFile()
        {
            socialRecentlysInfo.recentlyIds.Clear();
            foreach (InfoID infoID in socialRecentlysInfo.recentlyIdsDic.Values)
            {
                socialRecentlysInfo.recentlyIds.Add(infoID);
            }
            JsonHeler.SerializeToJsonFile(socialRecentlysInfo, string.Format(recentlysPath, Sys_Role.Instance.RoleId.ToString()));
        }

        /// <summary>
        /// 反序列化本地持久化的所有好友索引信息///
        /// </summary>
        void DeserializeFriendsInfoFromJsonFile()
        {
            string jsonStr = JsonHeler.GetJsonStr(string.Format(friendsPath, Sys_Role.Instance.RoleId.ToString()));
            JsonObject jo = (JsonObject)JsonSerializer.Deserialize(jsonStr);
            socialFriendsInfo.DeserializeObject(jo);
        }

        /// <summary>
        /// 序列化所有好友索引信息保存在本地///
        /// </summary>
        void SerializeFriendsInfoToJsonFile()
        {
            socialFriendsInfo.friendsIds.Clear();
            foreach (InfoID infoID in socialFriendsInfo.friendsIdsDic.Values)
            {
                socialFriendsInfo.friendsIds.Add(infoID);
            }
            JsonHeler.SerializeToJsonFile(socialFriendsInfo, string.Format(friendsPath, Sys_Role.Instance.RoleId.ToString()));
        }

        /// <summary>
        /// 反序列化本地持久化的队友索引信息///
        /// </summary>
        void DeserializeTeamMembersInfoFromJsonFile()
        {
            string jsonStr = JsonHeler.GetJsonStr(string.Format(teamMembersPath, Sys_Role.Instance.RoleId.ToString()));
            JsonObject jo = (JsonObject)JsonSerializer.Deserialize(jsonStr);
            socialTeamMembersInfo.DeserializeObject(jo);
        }

        /// <summary>
        /// 序列化队友索引信息保存在本地///
        /// </summary>
        void SerializeTeamMembersInfoToJsonFile()
        {
            socialTeamMembersInfo.teamMemberIds.Clear();
            foreach (InfoID infoID in socialTeamMembersInfo.teamMemberIdsDic.Values)
            {
                socialTeamMembersInfo.teamMemberIds.Add(infoID);
            }
            JsonHeler.SerializeToJsonFile(socialTeamMembersInfo, string.Format(teamMembersPath, Sys_Role.Instance.RoleId.ToString()));
        }

        /// <summary>
        /// 反序列化本地持久化的黑名单索引信息///
        /// </summary>
        void DeserializeBlacksInfoFromJsonFile()
        {
            string jsonStr = JsonHeler.GetJsonStr(string.Format(blacksPath, Sys_Role.Instance.RoleId.ToString()));
            JsonObject jo = (JsonObject)JsonSerializer.Deserialize(jsonStr);
            socialBlacksInfo.DeserializeObject(jo);
        }

        /// <summary>
        /// 序列化黑名单索引信息保存在本地///
        /// </summary>
        void SerializeBlacksInfoToJsonFile()
        {
            socialBlacksInfo.blacksIds.Clear();
            foreach (InfoID infoID in socialBlacksInfo.blacksIdsDic.Values)
            {
                socialBlacksInfo.blacksIds.Add(infoID);
            }

            JsonHeler.SerializeToJsonFile(socialBlacksInfo, string.Format(blacksPath, Sys_Role.Instance.RoleId.ToString()));
        }

        /// <summary>
        /// 反序列化本地持久化的自定义好友分组信息///
        /// </summary>
        void DeserializeFriendGroupInfoFromJsonFile()
        {
            string jsonStr = JsonHeler.GetJsonStr(string.Format(friendGroupsPath, Sys_Role.Instance.RoleId.ToString()));
            JsonObject jo = (JsonObject)JsonSerializer.Deserialize(jsonStr);
            socialFriendGroupsInfo.DeserializeObject(jo);
        }

        /// <summary>
        /// 序列化自定义好友分组信息保存在本地///
        /// </summary>
        void SerializeFriendGroupsInfoToJsonFile()
        {
            socialFriendGroupsInfo.friendGroupInfosList.Clear();
            foreach (FriendGroupInfo friendGroupInfo in socialFriendGroupsInfo.friendGroupInfosDic.Values)
            {
                friendGroupInfo.FillList();
                socialFriendGroupsInfo.friendGroupInfosList.Add(friendGroupInfo);
            }
            JsonHeler.SerializeToJsonFile(socialFriendGroupsInfo, string.Format(friendGroupsPath, Sys_Role.Instance.RoleId.ToString()));
        }

        /// <summary>
        /// 反序列化本地持久化的群组信息///
        /// </summary>
        void DeserializeGroupsInfoFromJsonFile()
        {
            string jsonStr = JsonHeler.GetJsonStr(string.Format(groupsPath, Sys_Role.Instance.RoleId.ToString()));
            JsonObject jo = (JsonObject)JsonSerializer.Deserialize(jsonStr);
            socialGroupsInfo.DeserializeObject(jo);
        }

        /// <summary>
        /// 序列化群组信息保存在本地///
        /// </summary>
        public void SerializeGroupsInfoToJsonFile()
        {
            socialGroupsInfo.groupsList.Clear();
            foreach (var groupInfo in socialGroupsInfo.groupsDic.Values)
            {
                groupInfo.FillList();
                socialGroupsInfo.groupsList.Add(groupInfo);
            }
            JsonHeler.SerializeToJsonFile(socialGroupsInfo, string.Format(groupsPath, Sys_Role.Instance.RoleId.ToString()));
        }

        /// <summary>
        /// 反序列化本地持久化的私聊索引信息///
        /// </summary>
        void DeserializeChatRefsFromJsonFile()
        {
            string jsonStr = JsonHeler.GetJsonStr(string.Format(chatRefsPath, Sys_Role.Instance.RoleId));
            JsonObject jo = (JsonObject)JsonSerializer.Deserialize(jsonStr);
            roleChatRefDataInfo.DeserializeObject(jo);
        }

        /// <summary>
        /// 序列化私聊索引信息保存在本地///
        /// </summary>
        void SerializeChatRefsToJsonFile()
        {
            roleChatRefDataInfo.roleChatRefDatasList.Clear();
            foreach (InfoID roleInfoID in roleChatRefDataInfo.roleChatRefDatas.Values)
            {
                roleChatRefDataInfo.roleChatRefDatasList.Add(roleInfoID);
            }
            JsonHeler.SerializeToJsonFile(roleChatRefDataInfo, string.Format(chatRefsPath, Sys_Role.Instance.RoleId.ToString()));
        }

        /// <summary>
        /// 反序列化本地持久化的私聊信息///
        /// </summary>
        /// <param name="path"></param>
        public void DeserializeChatsInfoFromJsonFile(string path)
        {
            string jsonStr = JsonHeler.GetJsonStr(path);
            JsonObject jo = (JsonObject)JsonSerializer.Deserialize(jsonStr);
            ChatsInfo chatsInfo = new ChatsInfo();
            chatsInfo.DeserializeObject(jo);
            roleChatDataInfo.roleChatDatas[chatsInfo.ID] = chatsInfo;
        }

        /// <summary>
        /// 序列化私聊信息保存在本地///
        /// </summary>
        /// <param name="roleChats"></param>
        /// <param name="path"></param>
        public void SerializeChatsInfoToJsonFile(ChatsInfo roleChats, string path)
        {
            roleChatDataInfo.roleChatDatasList.Clear();
            foreach (ChatsInfo roleChatData in roleChatDataInfo.roleChatDatas.Values)
            {
                roleChatDataInfo.roleChatDatasList.Add(roleChatData);
            }
            JsonHeler.SerializeToJsonFile(roleChats, path);
        }

        /// <summary>
        /// 反序列化本地持久化的群聊索引信息///
        /// </summary>
        void DeserializeGroupChatRefsFromJsonFile()
        {
            string jsonStr = JsonHeler.GetJsonStr(string.Format(groupRefsPath, Sys_Role.Instance.RoleId.ToString()));
            JsonObject jo = (JsonObject)JsonSerializer.Deserialize(jsonStr);
            groupChatRefDataInfo.DeserializeObject(jo);
        }

        /// <summary>
        /// 序列化群聊索引信息保存在本地///
        /// </summary>
        void SerializeGroupChatRefsToJsonFile()
        {
            groupChatRefDataInfo.groupChatRefDatasList.Clear();
            foreach (InfoID groupInfoID in groupChatRefDataInfo.groupChatRefDatas.Values)
            {
                groupChatRefDataInfo.groupChatRefDatasList.Add(groupInfoID);
            }
            JsonHeler.SerializeToJsonFile(groupChatRefDataInfo, string.Format(groupRefsPath, Sys_Role.Instance.RoleId.ToString()));
        }

        /// <summary>
        /// 反序列化本地持久化的群聊信息///
        /// </summary>
        /// <param name="path"></param>
        void DeserializeGroupChatsInfoFromJsonFile(string path)
        {
            string jsonStr = JsonHeler.GetJsonStr(path);
            JsonObject jo = (JsonObject)JsonSerializer.Deserialize(jsonStr);
            ChatsInfo chatsInfo = new ChatsInfo();
            chatsInfo.DeserializeObject(jo);
            groupChatDataInfo.groupChatDatas[(uint)(chatsInfo.ID)] = chatsInfo;
        }

        /// <summary>
        /// 序列化群聊信息保存在本地///
        /// </summary>
        /// <param name="groupChats"></param>
        /// <param name="path"></param>
        public void SerializeGroupChatsInfoToJsonFile(ChatsInfo groupChats, string path)
        {
            groupChatDataInfo.groupChatDatasList.Clear();
            foreach (ChatsInfo groupChatData in groupChatDataInfo.groupChatDatas.Values)
            {
                groupChatDataInfo.groupChatDatasList.Add(groupChatData);
            }
            JsonHeler.SerializeToJsonFile(groupChats, path);
        }
    }
}