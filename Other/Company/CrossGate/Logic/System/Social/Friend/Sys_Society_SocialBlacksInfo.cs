using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using System.Json;

namespace Logic
{
    public partial class Sys_Society : SystemModuleBase<Sys_Society>
    {
        /// <summary>
        /// 黑名单索引数据///
        /// </summary>
        public class SocialBlacksInfo
        {
            /// <summary>
            /// 黑名单索引集合///
            /// </summary>
            public Dictionary<ulong, InfoID> blacksIdsDic = new Dictionary<ulong, InfoID>();
            public List<InfoID> blacksIds = new List<InfoID>();
            const string blacksIdsFieldName = "blacksIds";

            /// <summary>
            /// 反序列化本地持久化的黑名单索引数据///
            /// </summary>
            /// <param name="jo"></param>
            public void DeserializeObject(JsonObject jo)
            {
                blacksIdsDic.Clear();
                blacksIds.Clear();
                if (jo.ContainsKey(blacksIdsFieldName))
                {
                    JsonArray ja = (JsonArray)jo[blacksIdsFieldName];
                    foreach (var item in ja)
                    {
                        InfoID roleInfoID = new InfoID();
                        roleInfoID.DeserializeObject((JsonObject)item);
                        blacksIdsDic[roleInfoID.infoID] = roleInfoID;
                    }
                }
            }

            /// <summary>
            /// 添加一个黑名单///
            /// </summary>
            /// <param name="roleID"></param>
            public void AddBlackInfo(ulong roleID)
            {
                ///因为黑名单在发送添加请求时前端会判断是否已满故直接添加即可///
                InfoID infoID = new InfoID();
                infoID.infoID = roleID;
                blacksIdsDic[roleID] = infoID;
            }

            /// <summary>
            /// 删除一个黑名单///
            /// </summary>
            /// <param name="roleID"></param>
            public void DelBlackInfo(ulong roleID)
            {
                InfoID infoID;
                if (blacksIdsDic.TryGetValue(roleID, out infoID))
                {
                    blacksIdsDic.Remove(roleID);
                }
                else
                {
                    DebugUtil.LogError($"blacksIdsDic not contain roleID {roleID}");
                }
            }

            /// <summary>
            /// 获取所有黑名单玩家的详细信息///
            /// </summary>
            /// <returns></returns>
            public Dictionary<ulong, RoleInfo> GetAllFirendsInfos()
            {
                Dictionary<ulong, RoleInfo> roleInfos = new Dictionary<ulong, RoleInfo>();

                var erator = blacksIdsDic.GetEnumerator();
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
            /// 获取排好序的黑名单的详细信息///
            /// </summary>
            /// <returns></returns>
            public List<RoleInfo> GetAllSortedBlacksInfos()
            {
                List<RoleInfo> roleInfos = new List<RoleInfo>();

                var erator = blacksIdsDic.GetEnumerator();
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