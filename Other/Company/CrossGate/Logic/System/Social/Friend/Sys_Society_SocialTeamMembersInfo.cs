using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using System.Json;

namespace Logic
{
    public partial class Sys_Society : SystemModuleBase<Sys_Society>
    {
        /// <summary>
        /// 最近队友索引数据///
        /// </summary>
        public class SocialTeamMembersInfo
        {
            /// <summary>
            /// 最近队友索引集合///
            /// </summary>
            public Dictionary<ulong, InfoID> teamMemberIdsDic = new Dictionary<ulong, InfoID>();
            public List<InfoID> teamMemberIds = new List<InfoID>();
            const string teamMemberIdsFieldName = "teamMemberIds";

            /// <summary>
            /// 反序列化本地持久化的最近队友索引数据///
            /// </summary>
            /// <param name="jo"></param>
            public void DeserializeObject(JsonObject jo)
            {
                teamMemberIdsDic.Clear();
                teamMemberIds.Clear();
                if (jo.ContainsKey(teamMemberIdsFieldName))
                {
                    JsonArray ja = (JsonArray)jo[teamMemberIdsFieldName];
                    foreach (var item in ja)
                    {
                        InfoID roleInfoID = new InfoID();
                        roleInfoID.DeserializeObject((JsonObject)item);
                        teamMemberIdsDic[roleInfoID.infoID] = roleInfoID;
                    }
                }
            }

            /// <summary>
            /// 添加一个队友///
            /// </summary>
            /// <param name="roleID"></param>
            public void AddTeamMemberInfo(ulong roleID)
            {
                ///当队友/数量达到最大值时，删除排序后最后的那个数据///
                if (teamMemberIdsDic.Count >= Instance.teammemberMaxCount)
                {
                    List<RoleInfo> temps = GetAllSortedTeamMemberInfos();
                    DelTeamMemberInfo(temps[temps.Count - 1].roleID);
                }

                InfoID infoID = new InfoID();
                infoID.infoID = roleID;
                teamMemberIdsDic[roleID] = infoID;

                RoleInfo roleInfo;
                if (Instance.socialRolesInfo.rolesDic.TryGetValue(roleID, out roleInfo))
                {
                    roleInfo.beTeamMemberTime = Sys_Time.Instance.GetServerTime();
                }
            }

            /// <summary>
            /// 删除一个队友///
            /// </summary>
            /// <param name="roleID"></param>
            public void DelTeamMemberInfo(ulong roleID)
            {
                InfoID infoID;
                if (teamMemberIdsDic.TryGetValue(roleID, out infoID))
                {
                    teamMemberIdsDic.Remove(roleID);
                }
                else
                {
                    DebugUtil.Log(ELogType.eNone, $"teamMemberIdsDic not contain roleID {roleID}");
                }
            }

            /// <summary>
            /// 获取所有最近队友的详细信息///
            /// </summary>
            /// <returns></returns>
            public Dictionary<ulong, RoleInfo> GetAllTeamMembersInfos()
            {
                Dictionary<ulong, RoleInfo> roleInfos = new Dictionary<ulong, RoleInfo>();

                var erator = teamMemberIdsDic.GetEnumerator();
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
            /// 获取排好序的最近队友的详细信息///
            /// </summary>
            /// <returns></returns>
            public List<RoleInfo> GetAllSortedTeamMemberInfos()
            {
                List<RoleInfo> roleInfos = new List<RoleInfo>();

                var erator = teamMemberIdsDic.GetEnumerator();
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
                ///2.最后入队的优先
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
                        if (a.beTeamMemberTime < b.beTeamMemberTime)
                            return 1;
                        else
                            return -1;
                    }
                });

                return roleInfos;
            }
        }
    }
}
