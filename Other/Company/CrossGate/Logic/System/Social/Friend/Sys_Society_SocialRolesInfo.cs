using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using System.Json;

namespace Logic
{
    public partial class Sys_Society : SystemModuleBase<Sys_Society>
    {
        /// <summary>
        /// 所有玩家的信息///
        /// </summary>
        public class SocialRolesInfo
        {
            /// <summary>
            /// 玩家信息集合///
            /// </summary>
            public Dictionary<ulong, RoleInfo> rolesDic = new Dictionary<ulong, RoleInfo>();
            public List<RoleInfo> rolesList = new List<RoleInfo>();
            const string rolesListFieldName = "rolesList";

            /// <summary>
            /// 反序列化本地持久化的玩家数据///
            /// </summary>
            /// <param name="jo"></param>
            public void DeserializeObject(JsonObject jo)
            {
                rolesDic.Clear();
                rolesList.Clear();
                if (jo.ContainsKey(rolesListFieldName))
                {
                    JsonArray ja = (JsonArray)jo[rolesListFieldName];
                    foreach (var item in ja)
                    {
                        RoleInfo roleInfo = new RoleInfo();
                        roleInfo.DeserializeObject((JsonObject)item);
                        rolesDic[roleInfo.roleID] = roleInfo;
                    }
                }
            }

            /// <summary>
            /// 改名///
            /// </summary>
            /// <param name="roleID"></param>
            /// <param name="newName"></param>
            public void ReName(ulong roleID, string newName)
            {
                RoleInfo roleInfo;
                if (rolesDic.TryGetValue(roleID, out roleInfo))
                {
                    roleInfo.roleName = newName;
                }
            }

            /// <summary>
            /// 更新人物信息///
            /// </summary>
            /// <param name="roleID"></param>
            /// <param name="name"></param>
            /// <param name="level"></param>
            /// <param name="isOnLine"></param>
            /// <param name="heroID"></param>
            /// <param name="friendValue"></param>
            /// <param name="occ"></param>
            /// <param name="iconId"></param>
            /// <param name="iconFrameId"></param>
            public void UpdateRoleInfo(ulong roleID, string name, uint level, bool isOnLine, uint heroID, uint friendValue, uint occ, uint iconId, uint iconFrameId, string guildName, uint commonNum, uint unCommonNum, bool hasGift)
            {
                RoleInfo roleInfo;
                if (!rolesDic.TryGetValue(roleID, out roleInfo))
                {
                    roleInfo = new RoleInfo();
                }

                roleInfo.roleID = roleID;
                roleInfo.roleName = name;
                roleInfo.level = level;
                roleInfo.isOnLine = isOnLine;
                roleInfo.heroID = heroID;
                roleInfo.friendValue = friendValue;
                roleInfo.occ = occ;
                roleInfo.iconId = iconId;
                roleInfo.iconFrameId = iconFrameId;
                roleInfo.guildName = guildName;
                roleInfo.commonSendNum = commonNum;
                roleInfo.unCommonSendNum = unCommonNum;
                roleInfo.hasGift = hasGift;

                ///设置默认的Icon外显///
                if (roleInfo.iconId == 0)
                {
                    roleInfo.iconId = DEFAULTROLEICONID;
                }

                ///设置默认的Icon框外显///
                if (roleInfo.iconFrameId == 0)
                {
                    roleInfo.iconFrameId = DEFAULTROLEICONFRAMEID;
                }

                rolesDic[roleID] = roleInfo;
            }

            /// <summary>
            /// 删除人物信息///
            /// </summary>
            /// <param name="roleID"></param>
            public void DelRoleInfo(ulong roleID)
            {
                if (rolesDic.ContainsKey(roleID))
                {
                    rolesDic.Remove(roleID);
                }
            }

            /// <summary>
            /// 更新人物在线状态///
            /// </summary>
            /// <param name="roleID"></param>
            /// <param name="isOnline"></param>
            public void UpdateRoleInfoOnLineStatus(ulong roleID, bool isOnline)
            {
                RoleInfo roleInfo;
                if (rolesDic.TryGetValue(roleID, out roleInfo))
                {
                    roleInfo.isOnLine = isOnline;
                }
                else
                {
                    DebugUtil.LogError($"Can't find roleInfo in socialRolesInfo roleID:{roleID}");
                }
            }


            /// <summary>
            /// 更新人物聊天的时间///
            /// </summary>
            /// <param name="roleID"></param>
            public void UpdateRoleInfoChatTime(ulong roleID)
            {
                RoleInfo roleInfo;
                if (rolesDic.TryGetValue(roleID, out roleInfo))
                {
                    roleInfo.lastChatTime = Sys_Time.Instance.GetServerTime();
                }
                else
                {
                    if (roleID != 0)
                        DebugUtil.LogError($"Can't find roleInfo in socialRolesInfo roleID:{roleID}");
                }
            }
        }
    }
}
