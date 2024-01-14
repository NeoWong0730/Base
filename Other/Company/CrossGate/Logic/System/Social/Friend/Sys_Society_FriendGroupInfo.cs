using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using System.Json;

namespace Logic
{
    public partial class Sys_Society : SystemModuleBase<Sys_Society>
    {
        /// <summary>
        /// 单个自定义好友分组信息///
        /// </summary>
        public class FriendGroupInfo
        {
            /// <summary>
            /// 分组名///
            /// </summary>
            public string name;
            const string nameFieldName = "name";

            /// <summary>
            /// 分组类所有好友的索引集合///
            /// </summary>
            public Dictionary<ulong, InfoID> roleIdsDic = new Dictionary<ulong, InfoID>();
            public List<InfoID> roleIds = new List<InfoID>();
            const string roleIdsFieldName = "roleIds";

            /// <summary>
            /// 反序列化本地持久化的自定义好友分组数据///
            /// </summary>
            /// <param name="jo"></param>
            public void DeserializeObject(JsonObject jo)
            {
                roleIdsDic.Clear();
                roleIds.Clear();
                name = ((JsonPrimitive)(jo[nameFieldName])).ToString();
                if (jo.ContainsKey(roleIdsFieldName))
                {
                    JsonArray ja = (JsonArray)jo[roleIdsFieldName];
                    foreach (var item in ja)
                    {
                        InfoID roleInfoID = new InfoID();
                        roleInfoID.DeserializeObject((JsonObject)item);
                        roleIdsDic[roleInfoID.infoID] = roleInfoID;
                    }
                }
            }

            /// <summary>
            /// 填充List集合///
            /// </summary>
            public void FillList()
            {
                roleIds.Clear();

                var erator = roleIdsDic.GetEnumerator();
                while (erator.MoveNext())
                {
                    roleIds.Add(erator.Current.Value);
                }
            }

            /// <summary>
            /// 添加一个好友ID进自定义好友分组///
            /// </summary>
            /// <param name="roleID"></param>
            public void AddFriendIDInfo(ulong roleID)
            {
                InfoID infoID = new InfoID();
                infoID.infoID = roleID;
                roleIdsDic[roleID] = infoID;
            }

            /// <summary>
            /// 删除一个好友ID///
            /// </summary>
            /// <param name="roleID"></param>
            public void DelFriendIDInfo(ulong roleID)
            {
                InfoID infoID;
                if (roleIdsDic.TryGetValue(roleID, out infoID))
                {
                    roleIdsDic.Remove(roleID);
                }
                //else
                //{
                //    DebugUtil.LogError($"roleIdsDic not contain roleID {roleID}");
                //}
            }

            /// <summary>
            /// 获得自定义好友分组b包含的所有好友信息///
            /// </summary>
            /// <returns></returns>
            public Dictionary<ulong, RoleInfo> GetAllRoleInfos()
            {
                Dictionary<ulong, RoleInfo> roleInfos = new Dictionary<ulong, RoleInfo>();

                var erator = roleIdsDic.GetEnumerator();
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
            /// 获得排序过后的自定义好友分组包含的所有好友信息///
            /// </summary>
            /// <returns></returns>
            public List<RoleInfo> GetAllSortedRoleInfos()
            {
                List<RoleInfo> roleInfos = new List<RoleInfo>();

                var erator = roleIdsDic.GetEnumerator();
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
                    else
                    {
                        if (a.lastChatTime > b.lastChatTime)
                            return 1;
                        else
                            return -1;
                    }
                });

                return roleInfos;
            }

            /// <summary>
            /// 获取该自定义好友分组的在线人数///
            /// </summary>
            /// <returns></returns>
            public int GetOnLineCount()
            {
                int count = 0;
                Dictionary<ulong, RoleInfo> roleInfos = GetAllRoleInfos();
                var erator = roleInfos.GetEnumerator();
                while (erator.MoveNext())
                {
                    if (erator.Current.Value.isOnLine)
                        count++;
                }

                return count;
            }
        }
    }
}
