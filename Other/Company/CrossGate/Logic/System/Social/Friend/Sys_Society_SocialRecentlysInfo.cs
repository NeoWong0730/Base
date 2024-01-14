using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using System.Json;
namespace Logic
{
    public partial class Sys_Society : SystemModuleBase<Sys_Society>
    {
        /// <summary>
        /// 最近联系人索引数据///
        /// </summary>
        public class SocialRecentlysInfo
        {
            /// <summary>
            /// 最近联系人索引的集合///
            /// </summary>
            public Dictionary<ulong, InfoID> recentlyIdsDic = new Dictionary<ulong, InfoID>();
            public List<InfoID> recentlyIds = new List<InfoID>();
            const string recentlyIdsFieldName = "recentlyIds";

            /// <summary>
            /// 反序列化本地持久化的最近联系人索引数据///
            /// </summary>
            /// <param name="jo"></param>
            public void DeserializeObject(JsonObject jo)
            {
                recentlyIdsDic.Clear();
                recentlyIds.Clear();
                if (jo.ContainsKey(recentlyIdsFieldName))
                {
                    JsonArray ja = (JsonArray)jo[recentlyIdsFieldName];
                    foreach (var item in ja)
                    {
                        InfoID roleInfoID = new InfoID();
                        roleInfoID.DeserializeObject((JsonObject)item);
                        recentlyIdsDic[roleInfoID.infoID] = roleInfoID;
                    }
                }
            }

            /// <summary>
            /// 添加一个最近联系人索引///
            /// </summary>
            /// <param name="roleID"></param>
            public void AddRecentlyInfo(ulong roleID)
            {
                ///当最近联系人数量达到最大值时，删除排序后最后的那个数据///
                if (recentlyIdsDic.Count >= Instance.recentlyMaxCount)
                {
                    List<RoleInfo> temps = GetAllSortedRecentlysInfos();
                    DelRecentlyInfo(temps[temps.Count - 1].roleID);
                }

                InfoID infoID = new InfoID();
                infoID.infoID = roleID;
                recentlyIdsDic[roleID] = infoID;
            }

            /// <summary>
            /// 删除一个最近联系人///
            /// </summary>
            /// <param name="roleID"></param>
            public void DelRecentlyInfo(ulong roleID)
            {
                InfoID infoID;
                if (recentlyIdsDic.TryGetValue(roleID, out infoID))
                {
                    recentlyIdsDic.Remove(roleID);
                }
                else
                {
                    DebugUtil.LogError($"recentlyIdsDic not contain roleID {roleID}");
                }
            }

            /// <summary>
            /// 获取所有最近的人的详细信息///
            /// </summary>
            /// <returns></returns>
            public Dictionary<ulong, RoleInfo> GetAllRecentlysInfos()
            {
                Dictionary<ulong, RoleInfo> roleInfos = new Dictionary<ulong, RoleInfo>();

                var erator = recentlyIdsDic.GetEnumerator();
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
            /// 获得排序过后的最近联系人索引///
            /// </summary>
            /// <returns></returns>
            public List<RoleInfo> GetAllSortedRecentlysInfos()
            {
                List<RoleInfo> roleInfos = new List<RoleInfo>();

                var erator = recentlyIdsDic.GetEnumerator();
                while (erator.MoveNext())
                {
                    RoleInfo roleInfo;
                    if (Instance.socialRolesInfo.rolesDic.TryGetValue(erator.Current.Key, out roleInfo))
                    {
                        roleInfos.Add(roleInfo);
                    }
                }

                ///排序规则:
                ///最近发生聊天的优先
                roleInfos.Sort((a, b) =>
                {
                    if (a.lastChatTime > b.lastChatTime)
                        return -1;
                    else
                        return 1;
                });

                return roleInfos;
            }
        }
    }
}
