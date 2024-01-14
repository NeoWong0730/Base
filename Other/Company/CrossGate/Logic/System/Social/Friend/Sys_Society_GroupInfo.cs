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
        /// 群组信息///
        /// </summary>
        public class GroupInfo
        {
            /// <summary>
            /// 群组ID///
            /// </summary>
            public uint groupID;
            const string groupIDFieldName = "groupID";

            /// <summary>
            /// 群组名///
            /// </summary>
            public string name;
            const string nameFieldName = "name";

            /// <summary>
            /// 群组人数///
            /// </summary>
            public uint count;
            const string countFieldName = "count";

            /// <summary>
            /// 群公告///
            /// </summary>
            public string notice;
            const string noticeFieldName = "notice";

            /// <summary>
            /// 群主ID///
            /// </summary>
            public ulong leader;
            const string leaderFieldName = "leader";

            /// <summary>
            /// 消息不打扰///
            /// </summary>
            public bool remind;
            const string remindFieldName = "remind";

            /// <summary>
            /// 列表中显示的头像信息///
            /// </summary>
            public Dictionary<uint, InfoID> heroIDsDic = new Dictionary<uint, InfoID>();
            public List<InfoID> heroIDs = new List<InfoID>();
            const string heroIDsFieldName = "heroIDs";

            /// <summary>
            /// 列表中显示的头像信息///
            /// </summary>
            public Dictionary<ulong, InfoID> iconIDsDic = new Dictionary<ulong, InfoID>();
            public List<InfoID> iconIDs = new List<InfoID>();
            const string iconIDsFieldName = "iconIDs";

            /// <summary>
            /// 群组所有角色ID///
            /// </summary>
            public Dictionary<ulong, InfoID> roleIDsDic = new Dictionary<ulong, InfoID>();
            public List<InfoID> roleIDs = new List<InfoID>();
            const string roleIDsFieldName = "roleIDs";

            /// <summary>
            /// 反序列化群组数据///
            /// </summary>
            /// <param name="jo"></param>
            public void DeserializeObject(JsonObject jo)
            {
                groupID = (uint)((JsonPrimitive)jo[groupIDFieldName]).ToInt32(null);
                name = ((JsonPrimitive)(jo[nameFieldName])).ToString();
                count = (uint)((JsonPrimitive)jo[countFieldName]).ToInt32(null);
                notice = ((JsonPrimitive)(jo[noticeFieldName])).ToString();
                leader = (ulong)((JsonPrimitive)jo[leaderFieldName]).ToInt64(null);
                remind = (bool)((JsonPrimitive)jo[remindFieldName]).ToBoolean(null);

                heroIDsDic.Clear();
                heroIDs.Clear();
                iconIDsDic.Clear();
                iconIDs.Clear();
                roleIDsDic.Clear();
                roleIDs.Clear();
                if (jo.ContainsKey(heroIDsFieldName))
                {
                    JsonArray ja = (JsonArray)jo[heroIDsFieldName];
                    foreach (var item in ja)
                    {
                        InfoID heroID = new InfoID();
                        heroID.DeserializeObject((JsonObject)item);
                        heroIDsDic[(uint)heroID.infoID] = heroID;
                    }
                }

                if (jo.ContainsKey(iconIDsFieldName))
                {

                    JsonArray ja = (JsonArray)jo[iconIDsFieldName];
                    foreach (var item in ja)
                    {
                        InfoID iconID = new InfoID();
                        iconID.DeserializeObject((JsonObject)item);
                        iconIDsDic[iconID.infoID] = iconID;
                    }
                }

                if (jo.ContainsKey(roleIDsFieldName))
                {
                    JsonArray ja = (JsonArray)jo[roleIDsFieldName];
                    foreach (var item in ja)
                    {
                        InfoID roleID = new InfoID();
                        roleID.DeserializeObject((JsonObject)item);
                        roleIDsDic[roleID.infoID] = roleID;
                    }
                }
            }

            public void FillList()
            {
                heroIDs.Clear();
                var erator = heroIDsDic.GetEnumerator();
                while (erator.MoveNext())
                {
                    heroIDs.Add(erator.Current.Value);
                }

                iconIDs.Clear();
                var erator2 = iconIDsDic.GetEnumerator();
                while (erator2.MoveNext())
                {
                    iconIDs.Add(erator2.Current.Value);
                }

                roleIDs.Clear();
                var erator3 = roleIDsDic.GetEnumerator();
                while (erator3.MoveNext())
                {
                    roleIDs.Add(erator3.Current.Value);
                }
            }

            /// <summary>
            /// 更新公告///
            /// </summary>
            /// <param name="newNotice"></param>
            public void UpdateNotice(string newNotice)
            {
                notice = newNotice;
            }

            /// <summary>
            /// 更新群组名///
            /// </summary>
            /// <param name="newName"></param>
            public void UpdateName(string newName)
            {
                name = newName;
            }

            /// <summary>
            /// 更新成员ID (同步更新HeroID和IconID)///
            /// </summary>
            /// <param name="RolesInfo"></param>
            public void UpdateRoleIDsFromServer(RepeatedField<FriendInfo> RolesInfo)
            {
                for (int index = 0, len = RolesInfo.Count; index < len; index++)
                {
                    InfoID roleID = new InfoID();
                    roleID.infoID = RolesInfo[index].RoleId;
                    roleIDsDic[RolesInfo[index].RoleId] = roleID;

                    InfoID heroID = new InfoID();
                    heroID.infoID = RolesInfo[index].HeroId;
                    heroIDsDic[RolesInfo[index].HeroId] = heroID;

                    InfoID iconID = new InfoID();
                    iconID.infoID = RolesInfo[index].HeroId;
                    iconIDsDic[RolesInfo[index].HeroId] = iconID;
                }
            }

            /// <summary>
            /// 群组中添加成员///
            /// 此处服务器只返回了roleID， 但并没有给HeroID及ICONID，导致群组头像无法更新///
            /// </summary>
            /// <param name="roleIDs"></param>
            public void AddRoleIDsToGroup(RepeatedField<ulong> roleIDs)
            {
                for (int index = 0, len = roleIDs.Count; index < len; index++)
                {
                    InfoID roleID = new InfoID();
                    roleID.infoID = roleIDs[index];
                    roleIDsDic[roleIDs[index]] = roleID;

                    count++;
                }
            }

            /// <summary>
            /// 群组中删除成员///
            /// </summary>
            /// <param name="roleID"></param>
            public void DelRoleIDFromGroup(ulong roleID)
            {
                InfoID infoID;
                if (roleIDsDic.TryGetValue(roleID, out infoID))
                {
                    roleIDsDic.Remove(roleID);
                    count--;
                }
                else
                {
                    DebugUtil.LogError($"roleIDsDic not contain roleID {roleID}");
                }
            }

            /// <summary>
            /// 获取该群组所有的角色信息///
            /// </summary>
            /// <returns></returns>
            public Dictionary<ulong, RoleInfo> GetAllGroupRoleInfos()
            {
                Dictionary<ulong, RoleInfo> roleInfos = new Dictionary<ulong, RoleInfo>();

                var erator = roleIDsDic.GetEnumerator();
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
        }
    }
}
