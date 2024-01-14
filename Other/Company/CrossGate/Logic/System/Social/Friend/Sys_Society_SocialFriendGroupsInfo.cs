using Lib.Core;
using Logic.Core;
using Packet;
using System.Collections.Generic;
using System.Json;

namespace Logic
{
    public partial class Sys_Society : SystemModuleBase<Sys_Society>
    {
        /// <summary>
        /// 所有自定义好友分组信息///
        /// </summary>
        public class SocialFriendGroupsInfo
        {
            /// <summary>
            /// 自定义好友分组集合///
            /// </summary>
            public Dictionary<string, FriendGroupInfo> friendGroupInfosDic = new Dictionary<string, FriendGroupInfo>();
            public List<FriendGroupInfo> friendGroupInfosList = new List<FriendGroupInfo>();
            const string friendGroupInfosListFieldName = "friendGroupInfosList";

            /// <summary>
            /// 反序列化本地持久化的所有自定义好友分组数据///
            /// </summary>
            /// <param name="jo"></param>
            public void DeserializeObject(JsonObject jo)
            {
                friendGroupInfosDic.Clear();
                friendGroupInfosList.Clear();
                if (jo.ContainsKey(friendGroupInfosListFieldName))
                {
                    friendGroupInfosList.Clear();
                    friendGroupInfosDic.Clear();
                    JsonArray ja = (JsonArray)jo[friendGroupInfosListFieldName];
                    foreach (var item in ja)
                    {
                        FriendGroupInfo friendGroupInfo = new FriendGroupInfo();
                        friendGroupInfo.DeserializeObject((JsonObject)item);
                        friendGroupInfosDic[friendGroupInfo.name] = friendGroupInfo;
                    }
                }
            }

            /// <summary>
            /// 从服务器回调中初始化///
            /// </summary>
            /// <param name="ack"></param>
            public void InitFriendGroupsInfoByServerMessage(CmdSocialGetFriendInfoAck ack)
            {
                for (int index = 0, len = ack.Teams.Count; index < len; index++)
                {
                    List<ulong> roleIDs = new List<ulong>();

                    for (int index2 = 0, len2 = ack.Teams[index].RoleId.Count; index2 < len2; index2++)
                    {
                        roleIDs.Add(ack.Teams[index].RoleId[index2]);
                    }
                    AddFriendGroupInfo(ack.Teams[index].Name.ToStringUtf8(), roleIDs);
                }
            }

            /// <summary>
            /// 添加好友分组///
            /// </summary>
            /// <param name="name"></param>
            /// <param name="roleIDs"></param>
            public void AddFriendGroupInfo(string name, List<ulong> roleIDs)
            {
                FriendGroupInfo friendGroupInfo = new FriendGroupInfo();
                friendGroupInfo.name = name;
                for (int index = 0, len = roleIDs.Count; index < len; index++)
                {
                    friendGroupInfo.AddFriendIDInfo(roleIDs[index]);
                }
                friendGroupInfosDic[name] = friendGroupInfo;
            }


            /// <summary>
            /// 删除好友分组///
            /// </summary>
            /// <param name="name"></param>
            public void DelFriendGroupInfo(string name)
            {
                FriendGroupInfo friendGroupInfo;
                if (friendGroupInfosDic.TryGetValue(name, out friendGroupInfo))
                {
                    friendGroupInfosDic.Remove(name);
                }
                else
                {
                    DebugUtil.LogError($"not contain FriendGroupInfo name: {name}");
                }
            }

            /// <summary>
            /// 删除一个好友时，遍历好友分组也删除///
            /// </summary>
            /// <param name="roleID"></param>
            public void DelFriendIDEachGroup(ulong roleID)
            {
                var erator = friendGroupInfosDic.GetEnumerator();
                while (erator.MoveNext())
                {
                    erator.Current.Value.DelFriendIDInfo(roleID);
                }
            }
        }
    }
}
