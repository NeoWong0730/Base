using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using Table;
using Net;
using Packet;
using System;

namespace Logic
{
    /// <summary>
    /// 勇者团系统_会议///
    /// </summary>
    public partial class Sys_WarriorGroup : SystemModuleBase<Sys_WarriorGroup>
    {
        /// <summary>
        /// 会议信息基类///
        /// </summary>
        public abstract class MeetingInfoBase
        {
            /// <summary>
            /// 会议状态类型///
            /// </summary>
            public enum MeetingStatusType
            {
                Normal,
                Doing,
                History,
            }

            /// <summary>
            /// 会议类型///
            /// </summary>
            public enum MeetingType
            {
                None = 0,
                SuggestSelf = 1,
                Recruit = 2,
                Fire = 3,
                ChangeName = 4,
                ChangeDeclaration = 5,
            }

            /// <summary>
            /// 会议状态///
            /// </summary>
            public MeetingStatusType Status
            {
                get;
                set;
            }

            /// <summary>
            /// 会议ID///
            /// </summary>
            public uint MeetingID
            {
                get;
                protected set;
            }

            /// <summary>
            /// 会议表ID///
            /// </summary>
            public virtual uint InfoID
            {
                get;
            }

            /// <summary>
            /// 会议发起者ID///
            /// </summary>
            public ulong CreateRoleId
            {
                get;
                protected set;
            }

            public string CreateRoleName
            {
                get;
                protected set;
            }

            /// <summary>
            /// 会议结果///
            /// </summary>
            public bool Result
            {
                get;
                private set;
            } = false;

            public uint CreateTime
            {
                get;
                private set;
            }

            /// <summary>
            /// 完成时间///
            /// </summary>
            public uint FinishTime
            {
                get;
                private set;
            }          

            /// <summary>
            /// 剩余投票时间///
            /// </summary>
            public uint LeftTime
            {
                get;
                private set;
            }

            /// <summary>
            /// 参数列表///
            /// </summary>
            public BraveGroupMeetingParamList ParamList
            {
                get;
                private set;
            }

            /// <summary>
            /// 不同意的请举手，没有，没有，没有，通过！！！///
            /// </summary>
            public Dictionary<ulong, bool> VoteList = new Dictionary<ulong, bool>();

            public Dictionary<ulong, string> VoteNameList = new Dictionary<ulong, string>();

            /// <summary>
            /// 表信息///
            /// </summary>
            public CSVBraveTeamMeeting.Data CSVBraveTeamMeetingData
            {
                get;
                protected set;
            }

            /// <summary>
            /// 是否已投票///
            /// </summary>
            public bool Voted
            {
                get
                {
                    if (VoteList.ContainsKey(Sys_Role.Instance.RoleId))
                        return true;
                    return false;
                }
            }

            Timer doingTimer;

            /// <summary>
            /// 设置为未开始会议///
            /// </summary>
            /// <returns></returns>
            public MeetingInfoBase SetNormal()
            {
                MeetingID = 0;
                Status = MeetingStatusType.Normal;
                CreateRoleId = 0;
                CreateRoleName = string.Empty;
                Result = false;
                ParamList = null;
                VoteList.Clear();
                FinishTime = 0;
                CreateTime = 0;
                LeftTime = 0;
                VoteNameList.Clear();
                doingTimer?.Cancel();
                doingTimer = null;

                return this;
            }

            /// <summary>
            /// 设置为进行时会议///
            /// </summary>
            /// <param name="_meetingID"></param>
            /// <param name="_createRoldID"></param>
            public MeetingInfoBase SetDoing(uint _meetingID, ulong _createRoldID, string _createRoleName, uint createTime,
                BraveGroupMeetingParamList braveGroupMeetingParamList, List<ulong> agree,
                List<ulong> refuse)
            {
                MeetingID = _meetingID;
                CreateRoleId = _createRoldID;
                CreateRoleName = _createRoleName;
                Status = MeetingStatusType.Doing;
                CreateTime = createTime;
                ParamList = braveGroupMeetingParamList;
                LeftTime = Instance.doingMeetingMaxTime - (Sys_Time.Instance.GetServerTime() - CreateTime);
                VoteList.Clear();
                for (int index = 0, len = agree.Count; index < len; index++)
                {
                    VoteList[agree[index]] = true;
                }
                for (int index = 0, len = refuse.Count; index < len; index++)
                {
                    VoteList[refuse[index]] = false;
                }
                //VoteList[_createRoldID] = true;
                VoteNameList.Clear();
                foreach (var vote in VoteList)
                {
                    if (Instance.MyWarriorGroup.warriorInfos.ContainsKey(vote.Key))
                    {
                        VoteNameList[vote.Key] = Instance.MyWarriorGroup.warriorInfos[vote.Key].RoleName;
                    }
                    else
                    {
                        VoteNameList[vote.Key] = string.Empty;
                    }
                }
                doingTimer?.Cancel();
                doingTimer = Timer.Register(1f, () =>
                {
                    LeftTime--;
                }, null, true);
                return this;
            }

            /// <summary>
            /// 设置为历史会议///
            /// </summary>
            /// <param name="_meetingID"></param>
            /// <param name="_createRoldID"></param>
            /// <param name="result"></param>
            public MeetingInfoBase SetHistory(uint _meetingID, ulong _createRoldID, string _createRoleName,
                bool _result, uint finishTime, BraveGroupMeetingParamList braveGroupMeetingParamList,
                List<ulong> agree, List<ulong> refuse, Dictionary<ulong, string> voteName)
            {
                MeetingID = _meetingID;
                CreateRoleId = _createRoldID;
                CreateRoleName = _createRoleName;
                Result = _result;
                Status = MeetingStatusType.History;
                FinishTime = finishTime;
                LeftTime = 0;
                ParamList = braveGroupMeetingParamList;
                VoteList.Clear();
                for (int index = 0, len = agree.Count; index < len; index++)
                {
                    VoteList[agree[index]] = true;
                }
                for (int index = 0, len = refuse.Count; index < len; index++)
                {
                    VoteList[refuse[index]] = false;
                }
                VoteNameList.Clear();
                foreach (var vote in voteName)
                {
                    VoteNameList[vote.Key] = vote.Value;
                }
                doingTimer?.Cancel();
                doingTimer = null;
                return this;
            }

            /// <summary>
            /// 刷新投票///
            /// </summary>
            /// <returns></returns>
            public MeetingInfoBase RefreshVote(ulong roleID, bool voteResult)
            {
                VoteList[roleID] = voteResult;

                if (Instance.MyWarriorGroup.warriorInfos.ContainsKey(roleID))
                {
                    VoteNameList[roleID] = Instance.MyWarriorGroup.warriorInfos[roleID].RoleName;
                }
                else
                {
                    VoteNameList[roleID] = string.Empty;
                }

                return this;
            }

            public virtual string GetContent()
            {
                return "123";
            }

            public virtual bool IsValid()
            {
                if (CSVBraveTeamMeetingData.Condition != null && CSVBraveTeamMeetingData.Condition.Count > 0)
                {
                    for (int index = 0, len = CSVBraveTeamMeetingData.Condition.Count; index < len; index++)
                    {
                        if (CSVBraveTeamMeetingData.Condition[index] == (uint)MeetingConditionBase.MeetingConditionType.LeaderOfflineTime)
                        {
                            MeetingCondition_LeaderOfflineTime meetingCondition_LeaderOfflineTime = new MeetingCondition_LeaderOfflineTime(Instance.MyWarriorGroup.warriorInfos[Instance.MyWarriorGroup.LeaderRoleID].LastOffline);
                            if (!meetingCondition_LeaderOfflineTime.IsValid())
                                return false;
                        }
                        else if (CSVBraveTeamMeetingData.Condition[index] == (uint)MeetingConditionBase.MeetingConditionType.MembersCount)
                        {
                            MeetingCondition_MembersCount meetingCondition_MembersCount = new MeetingCondition_MembersCount((uint)Instance.MyWarriorGroup.MemberCount);
                            if (!meetingCondition_MembersCount.IsValid())
                                return false;
                        }
                        else if (CSVBraveTeamMeetingData.Condition[index] == (uint)MeetingConditionBase.MeetingConditionType.AvailableCount)
                        {
                            MeetingCondition_AvailableCount meetingCondition_AvailableCount = new MeetingCondition_AvailableCount(Instance.memberMaxCount - (uint)Instance.MyWarriorGroup.MemberCount);
                            if (!meetingCondition_AvailableCount.IsValid())
                                return false;
                        } 
                        else if (CSVBraveTeamMeetingData.Condition[index] == (uint)MeetingConditionBase.MeetingConditionType.Recruit)
                        {
                            MeetingCondition_Recruit meetingCondition_Recruit = new MeetingCondition_Recruit(30);
                            if (!meetingCondition_Recruit.IsValid())
                                return false;
                        }
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// 会议_自荐///
        /// </summary>
        public class Meeting_SuggestSelf : MeetingInfoBase
        {
            public override uint InfoID
            {
                get
                {
                    return (uint)MeetingType.SuggestSelf;
                }
            }

            public override bool IsValid()
            {
                return base.IsValid() && (Instance.MyWarriorGroup.LeaderRoleID != Sys_Role.Instance.RoleId);
            }

            public override string GetContent()
            {
                string createName = string.Empty;
                string targetName = string.Empty;
                if (Status == MeetingStatusType.Doing)
                {
                    if (Instance.MyWarriorGroup.warriorInfos.ContainsKey(CreateRoleId))
                        createName = Instance.MyWarriorGroup.warriorInfos[CreateRoleId].RoleName;
                }
                else if (Status == MeetingStatusType.History)
                {
                    if (Instance.MyWarriorGroup.warriorInfos.ContainsKey(CreateRoleId))
                        createName = Instance.MyWarriorGroup.warriorInfos[CreateRoleId].RoleName;
                    else
                        createName = CreateRoleName;
                }

                if (ParamList.Params != null && ParamList.Params.Count > 0)
                {
                    if (ParamList.Params[0].IntParam != 0 && Instance.MyWarriorGroup.warriorInfos.ContainsKey(ParamList.Params[0].IntParam))
                    {
                        targetName = Instance.MyWarriorGroup.warriorInfos[ParamList.Params[0].IntParam].RoleName;
                    }
                    else
                    {
                        targetName = ParamList.Params[0].StrParam.ToStringUtf8();
                    }
                }

                return LanguageHelper.GetTextContent(CSVBraveTeamMeetingData.ContentLan, createName, targetName);
            }

            public Meeting_SuggestSelf()
            {
                CSVBraveTeamMeetingData = CSVBraveTeamMeeting.Instance.GetConfData(InfoID);
            }
        }

        /// <summary>
        /// 会议_招募///
        /// </summary>
        public class Meeting_Recruit : MeetingInfoBase
        {
            public override uint InfoID
            {
                get
                {
                    return (uint)MeetingType.Recruit;
                }
            }

            public override string GetContent()
            {
                string createName = string.Empty;
                string targetName = string.Empty;
                if (Status == MeetingStatusType.Doing)
                {
                    if (Instance.MyWarriorGroup.warriorInfos.ContainsKey(CreateRoleId))
                        createName = Instance.MyWarriorGroup.warriorInfos[CreateRoleId].RoleName;
                }
                else if (Status == MeetingStatusType.History)
                {
                    if (Instance.MyWarriorGroup.warriorInfos.ContainsKey(CreateRoleId))
                        createName = Instance.MyWarriorGroup.warriorInfos[CreateRoleId].RoleName;
                    else
                        createName = CreateRoleName;
                }

                if (ParamList.Params != null && ParamList.Params.Count > 0)
                {
                    if (ParamList.Params[0].IntParam != 0 && Instance.MyWarriorGroup.warriorInfos.ContainsKey(ParamList.Params[0].IntParam))
                    {
                        targetName = Instance.MyWarriorGroup.warriorInfos[ParamList.Params[0].IntParam].RoleName;
                    }
                    else
                    {
                        targetName = ParamList.Params[0].StrParam.ToStringUtf8();
                    }
                }

                return LanguageHelper.GetTextContent(CSVBraveTeamMeetingData.ContentLan, createName, targetName);
            }

            public Meeting_Recruit()
            {
                CSVBraveTeamMeetingData = CSVBraveTeamMeeting.Instance.GetConfData(InfoID);
            }
        }

        /// <summary>
        /// 会议_请离///
        /// </summary>
        public class Meeting_Fire : MeetingInfoBase
        {
            public override uint InfoID
            {
                get
                {
                    return (uint)MeetingType.Fire;
                }
            }

            public override string GetContent()
            {
                string createName = string.Empty;
                string targetName = string.Empty;
                if (Status == MeetingStatusType.Doing)
                {
                    if (Instance.MyWarriorGroup.warriorInfos.ContainsKey(CreateRoleId))
                        createName = Instance.MyWarriorGroup.warriorInfos[CreateRoleId].RoleName;                  
                }
                else if (Status == MeetingStatusType.History)
                {
                    if (Instance.MyWarriorGroup.warriorInfos.ContainsKey(CreateRoleId))
                        createName = Instance.MyWarriorGroup.warriorInfos[CreateRoleId].RoleName;
                    else
                        createName = CreateRoleName;
                }

                if (ParamList.Params != null && ParamList.Params.Count > 0)
                {
                    if (ParamList.Params[0].IntParam != 0 && Instance.MyWarriorGroup.warriorInfos.ContainsKey(ParamList.Params[0].IntParam))
                    {
                        targetName = Instance.MyWarriorGroup.warriorInfos[ParamList.Params[0].IntParam].RoleName;
                    }
                    else
                    {
                        targetName = ParamList.Params[0].StrParam.ToStringUtf8();
                    }
                }

                return LanguageHelper.GetTextContent(CSVBraveTeamMeetingData.ContentLan, createName, targetName);
            }

            public Meeting_Fire()
            {
                CSVBraveTeamMeetingData = CSVBraveTeamMeeting.Instance.GetConfData(InfoID);
            }
        }

        /// <summary>
        /// 会议_改名///
        /// </summary>
        public class Meeting_ChangeName : MeetingInfoBase
        {
            public override uint InfoID
            {
                get
                {
                    return (uint)MeetingType.ChangeName;
                }
            }

            public override bool IsValid()
            {
                return true;
            }

            public override string GetContent()
            {
                string createName = string.Empty;
                if (Status == MeetingStatusType.Doing)
                {
                    if (Instance.MyWarriorGroup.warriorInfos.ContainsKey(CreateRoleId))
                        createName = Instance.MyWarriorGroup.warriorInfos[CreateRoleId].RoleName;
                }
                else if (Status == MeetingStatusType.History)
                {
                    if (Instance.MyWarriorGroup.warriorInfos.ContainsKey(CreateRoleId))
                        createName = Instance.MyWarriorGroup.warriorInfos[CreateRoleId].RoleName;
                    else
                        createName = CreateRoleName;
                }

                return LanguageHelper.GetTextContent(CSVBraveTeamMeetingData.ContentLan, createName, ParamList.Params[0].StrParam.ToStringUtf8());
            }

            public Meeting_ChangeName()
            {
                CSVBraveTeamMeetingData = CSVBraveTeamMeeting.Instance.GetConfData(InfoID);
            }
        }

        /// <summary>
        /// 会议_改宣言///
        /// </summary>
        public class Meeting_ChangeDeclaration : MeetingInfoBase
        {
            public override uint InfoID
            {
                get
                {
                    return (uint)MeetingType.ChangeDeclaration;
                }
            }

            public override bool IsValid()
            {
                return true;
            }

            public override string GetContent()
            {
                string createName = string.Empty;
                if (Status == MeetingStatusType.Doing)
                {
                    if (Instance.MyWarriorGroup.warriorInfos.ContainsKey(CreateRoleId))
                        createName = Instance.MyWarriorGroup.warriorInfos[CreateRoleId].RoleName;
                }
                else if (Status == MeetingStatusType.History)
                {
                    if (Instance.MyWarriorGroup.warriorInfos.ContainsKey(CreateRoleId))
                        createName = Instance.MyWarriorGroup.warriorInfos[CreateRoleId].RoleName;
                    else
                        createName = CreateRoleName;
                }

                return LanguageHelper.GetTextContent(CSVBraveTeamMeetingData.ContentLan, createName, ParamList.Params[0].StrParam.ToStringUtf8());
            }

            public Meeting_ChangeDeclaration()
            {
                CSVBraveTeamMeetingData = CSVBraveTeamMeeting.Instance.GetConfData(InfoID);
            }
        }
    }
}
