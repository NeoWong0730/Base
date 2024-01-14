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
        /// 所有群组信息///
        /// </summary>
        public class SocialGroupsInfo
        {
            /// <summary>
            /// 所有群组信息集合///
            /// </summary>
            public Dictionary<ulong, GroupInfo> groupsDic = new Dictionary<ulong, GroupInfo>();
            public List<GroupInfo> groupsList = new List<GroupInfo>();
            const string groupsListFieldName = "groupsList";

            /// <summary>
            /// 反序列化本地持久化的所有群组数据///
            /// </summary>
            /// <param name="jo"></param>
            public void DeserializeObject(JsonObject jo)
            {
                groupsDic.Clear();
                groupsList.Clear();
                if (jo.ContainsKey(groupsListFieldName))
                {
                    JsonArray ja = (JsonArray)jo[groupsListFieldName];
                    foreach (var item in ja)
                    {
                        GroupInfo groupInfo = new GroupInfo();
                        groupInfo.DeserializeObject((JsonObject)item);
                        groupsDic[groupInfo.groupID] = groupInfo;
                    }
                }
            }

            /// <summary>
            /// 更新为服务器传来的群组信息///
            /// </summary>
            /// <param name="ack"></param>
            public void InitGroupsInfoByServerMessage(CmdSocialGetGroupBriefInfoAck ack)
            {
                for (int index = 0, len = ack.Groups.Count; index < len; index++)
                {
                    List<uint> heroIDs = new List<uint>();

                    for (int index2 = 0, len2 = ack.Groups[index].HeroId.Count; index2 < len2; index2++)
                    {
                        heroIDs.Add(ack.Groups[index].HeroId[index2]);
                    }

                    AddGroup(ack.Groups[index].GroupId, ack.Groups[index].Count, ack.Groups[index].Name.ToStringUtf8(), ack.Groups[index].Notice.ToStringUtf8(), ack.Groups[index].Leader, heroIDs);
                }
            }

            /// <summary>
            /// 添加一个群组///
            /// </summary>
            /// <param name="groupID"></param>
            /// <param name="count"></param>
            /// <param name="name"></param>
            /// <param name="notice"></param>
            /// <param name="leader"></param>
            /// <param name="heroIDs"></param>
            public void AddGroup(uint groupID, uint count, string name, string notice, ulong leader, List<uint> heroIDs)
            {
                GroupInfo groupInfo = new GroupInfo();
                groupInfo.groupID = groupID;
                groupInfo.count = count;
                groupInfo.name = name;
                groupInfo.notice = notice;
                groupInfo.leader = leader;

                for (int index = 0, len = heroIDs.Count; index < len; index++)
                {
                    InfoID infoID = new InfoID();
                    infoID.infoID = heroIDs[index];
                    groupInfo.heroIDsDic[heroIDs[index]] = infoID;
                }

                groupsDic[groupID] = groupInfo;
            }

            /// <summary>
            /// 删除一个群组///
            /// </summary>
            /// <param name="groupID"></param>
            public void DelGroup(uint groupID)
            {
                GroupInfo groupInfo;
                if (groupsDic.TryGetValue(groupID, out groupInfo))
                {
                    groupsDic.Remove(groupID);
                }
                else
                {
                    DebugUtil.LogError($"groupsDic not contain group id: {groupID}");
                }
            }
        }
    }
}