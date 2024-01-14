using Google.Protobuf.Collections;
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
        /// 好友索引数据///
        /// </summary>
        public class SocialFriendsInfo
        {
            /// <summary>
            /// 好友索引集合///
            /// </summary>
            public Dictionary<ulong, InfoID> friendsIdsDic = new Dictionary<ulong, InfoID>();
            public List<InfoID> friendsIds = new List<InfoID>();
            const string friendsIdsFieldName = "friendsIds";

            /// <summary>
            /// 反序列化本地持久化的好友索引数据///
            /// </summary>
            /// <param name="jo"></param>
            public void DeserializeObject(JsonObject jo)
            {
                friendsIdsDic.Clear();
                friendsIds.Clear();
                if (jo.ContainsKey(friendsIdsFieldName))
                {
                    JsonArray ja = (JsonArray)jo[friendsIdsFieldName];
                    foreach (var item in ja)
                    {
                        InfoID roleInfoID = new InfoID();
                        roleInfoID.DeserializeObject((JsonObject)item);
                        friendsIdsDic[roleInfoID.infoID] = roleInfoID;
                    }
                }
            }


            /// <summary>
            /// 从服务器的回调数据中初始化好友索引///
            /// </summary>
            /// <param name="friendInfos"></param>
            public void InitFriendsInfoByServerMessage(RepeatedField<FriendInfo> friendInfos)
            {
                for (int index = 0, len = friendInfos.Count; index < len; index++)
                {
                    AddFriendInfo(friendInfos[index].RoleId);
                }
            }

            /// <summary>
            /// 添加一个好友///
            /// </summary>
            /// <param name="roleID"></param>
            public void AddFriendInfo(ulong roleID)
            {
                ///因为好友在发送添加请求时前端会判断是否已满故直接添加即可///
                InfoID infoID = new InfoID();
                infoID.infoID = roleID;
                friendsIdsDic[roleID] = infoID;
            }

            /// <summary>
            /// 删除一个好友///
            /// </summary>
            /// <param name="roleID"></param>
            public void DelFriendInfo(ulong roleID)
            {
                InfoID infoID;
                if (friendsIdsDic.TryGetValue(roleID, out infoID))
                {
                    friendsIdsDic.Remove(roleID);
                }
                else
                {
                    DebugUtil.Log(ELogType.eNone, $"friendsIdsDic not contain roleID {roleID}");
                }
            }

            /// <summary>
            /// 获取所有好友的详细信息///
            /// </summary>
            /// <returns></returns>
            public Dictionary<ulong, RoleInfo> GetAllFirendsInfos()
            {
                Dictionary<ulong, RoleInfo> roleInfos = new Dictionary<ulong, RoleInfo>();

                var erator = friendsIdsDic.GetEnumerator();
                while (erator.MoveNext())
                {
                    RoleInfo roleInfo;
                    if (Instance.socialRolesInfo.rolesDic.TryGetValue(erator.Current.Key, out roleInfo))
                    {
                        roleInfos[erator.Current.Key] = roleInfo;
                    }
                }

                return roleInfos;
            }

            /// <summary>
            /// 获取排好序的好友的详细信息///
            /// </summary>
            /// <returns></returns>
            public List<RoleInfo> GetAllSortedFriendsInfos()
            {
                List<RoleInfo> roleInfos = new List<RoleInfo>();

                var erator = friendsIdsDic.GetEnumerator();
                while (erator.MoveNext())
                {
                    RoleInfo roleInfo;
                    if (Instance.socialRolesInfo.rolesDic.TryGetValue(erator.Current.Key, out roleInfo))
                    {
                        roleInfos.Add(roleInfo);
                    }
                }

                ///排序规则:
                ///1.在线的优先
                ///2.最近发生聊天的优先
                roleInfos.Sort((a, b) =>
                {
                    if (!a.isOnLine && b.isOnLine)
                    {
                        return 1;
                    }
                    else if (a.isOnLine && !b.isOnLine)
                    {
                        return -1;
                    }
                    else
                    {
                        if (a.lastChatTime > b.lastChatTime)
                            return -1;
                        else
                            return 1;
                    }
                });

                return roleInfos;
            }

            public List<RoleInfo> GetAllSortedFriendsInfosByFitler(uint plusFriendValue = 0)
            {
                List<RoleInfo> roleInfos = new List<RoleInfo>();
                List<RoleInfo> tempRoleInfos = GetAllSortedFriendsInfos();

                for (int index = 0, len = tempRoleInfos.Count; index < len; index++)
                {
                    if (tempRoleInfos[index].friendValue >= plusFriendValue)
                    {
                        roleInfos.Add(tempRoleInfos[index]);
                    }
                }

                return roleInfos;
            }

            /// <summary>
            /// 用于勇者团///
            /// </summary>
            /// <param name="plusFriendValue"></param>
            /// <returns></returns>
            public List<RoleInfo> GetAllSortedFriendsInfosByFitlerForWarriorGroup(uint plusFriendValue = 0, uint level = 30)
            {
                List<RoleInfo> roleInfos = new List<RoleInfo>();
                List<RoleInfo> tempRoleInfos = GetAllSortedFriendsInfos();

                for (int index = 0, len = tempRoleInfos.Count; index < len; index++)
                {
                    if (tempRoleInfos[index].friendValue >= plusFriendValue && tempRoleInfos[index].level >= level)
                    {
                        if (!Sys_WarriorGroup.Instance.MyWarriorGroup.warriorInfos.ContainsKey(tempRoleInfos[index].roleID))
                            roleInfos.Add(tempRoleInfos[index]);
                    }
                }

                return roleInfos;
            }
        }
    }

}