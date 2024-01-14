using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using Table;
using Net;
using Packet;
using System.Json;
using System.IO;
using UnityEngine;

namespace Logic
{
    /// <summary>
    /// 勇者团系统///
    /// </summary>
    public partial class Sys_WarriorGroup : SystemModuleBase<Sys_WarriorGroup>
    {
        /// <summary>
        /// 勇者团最大成员数量///
        /// </summary>
        public uint memberMaxCount;

        /// <summary>
        /// 历史会议最大数量///
        /// </summary>
        uint historyMeetingMaxCount;

        /// <summary>
        /// 历史会议最大存留时间///
        /// </summary>
        uint historyMeetingMaxTime;

        /// <summary>
        /// 投票最长时间///
        /// </summary>
        uint doingMeetingMaxTime;

        /// <summary>
        /// 我的勇者团///
        /// </summary>
        public WarriorGroup MyWarriorGroup = new WarriorGroup();

        /// <summary>
        /// 我的勇者团被邀请列表///
        /// </summary>
        public Dictionary<ulong, InvitedInfo> invitedInfoDict = new Dictionary<ulong, InvitedInfo>();

        /// <summary>
        /// 退团后的冷却时间///
        /// </summary>
        public uint QuitAfterColdTime
        {
            get;
            set;
        }

        /// <summary>
        /// 勇者团///
        /// </summary>
        public class WarriorGroup
        {
            /// <summary>
            /// 勇者团UID///
            /// </summary>
            public ulong GroupUID
            {
                get;
                set;
            }

            /// <summary>
            /// 勇者团名字///
            /// </summary>
            public string GroupName
            {
                get;
                set;
            }

            /// <summary>
            /// 勇者团宣言///
            /// </summary>
            public string GroupDeclaration
            {
                get;
                set;
            }

            /// <summary>
            /// 团长角色ID///
            /// </summary>
            public ulong LeaderRoleID
            {
                get;
                set;
            }

            /// <summary>
            /// 入团时间///
            /// </summary>
            public uint JoinBraveGroupTime
            {
                get;
                set;
            }

            uint _quitBeforeThinkingTime;

            /// <summary>
            /// 退出前的冷静时间///
            /// </summary>
            public uint QuitBeforeThinkingTime
            {
                get
                {
                    return _quitBeforeThinkingTime;
                }
                set
                {               
                    _quitBeforeThinkingTime = value;
                    if (value == 0)
                    {
                        Instance.eventEmitter.Trigger(EEvents.RefreshQuitTime);
                    }
                }
            }

            uint _nextCreateMeetingTime;

            /// <summary>
            /// 下次创建会议的到期时间///
            /// </summary>
            public uint NextCreateMeetingTime
            {
                get
                {
                    return _nextCreateMeetingTime;
                }
                set
                {                
                    _nextCreateMeetingTime = value;
                    if (value == 0)
                    {
                        Instance.eventEmitter.Trigger(EEvents.RefreshCreateMeetTime);
                    }
                }
            }

            uint _fastInviteTime;

            /// <summary>
            /// 邀请组队时间///
            /// </summary>
            public uint FastInviteTime
            {
                get
                {
                    return _fastInviteTime;
                }
                set
                {                   
                    _fastInviteTime = value;
                    if (value == 0)
                    {
                        Instance.eventEmitter.Trigger(EEvents.RefreshFastInviteTime);
                    }
                }
            }

            /// <summary>
            /// 勇者团成员数///
            /// </summary>
            public int MemberCount
            {
                get
                {
                    if (warriorInfos == null || warriorInfos.Count == 0)
                        return 0;
                    else
                        return warriorInfos.Count;
                }
            }

            /// <summary>
            /// 勇士团所有勇士信息集合///
            /// </summary>
            public Dictionary<ulong, WarriorInfo> warriorInfos = new Dictionary<ulong, WarriorInfo>();

            /// <summary>
            /// 勇士团动态集合///
            /// </summary>
            public List<ActionInfo> actionInfos = new List<ActionInfo>();

            /// <summary>
            /// 勇者团当前会议集合///
            /// </summary>
            public Dictionary<uint, MeetingInfoBase> currentMeetingInfos = new Dictionary<uint, MeetingInfoBase>();

            /// <summary>
            /// 勇者团历史会议集合///
            /// </summary>
            public Dictionary<uint, MeetingInfoBase> historyMeetingInfos = new Dictionary<uint, MeetingInfoBase>();

            public Timer quitColdTimer;
            public Timer createMeetTimer;
            public Timer fastInviteTimer;
            public void Dispose()
            {
                GroupUID = 0;
                GroupName = string.Empty;
                GroupDeclaration = string.Empty;
                LeaderRoleID = 0;
                warriorInfos.Clear();
                actionInfos.Clear();
                currentMeetingInfos.Clear();
                currentMeetingInfos[(uint)MeetingInfoBase.MeetingType.SuggestSelf] = new Meeting_SuggestSelf();
                currentMeetingInfos[(uint)MeetingInfoBase.MeetingType.Recruit] = new Meeting_Recruit();
                currentMeetingInfos[(uint)MeetingInfoBase.MeetingType.Fire] = new Meeting_Fire();
                currentMeetingInfos[(uint)MeetingInfoBase.MeetingType.ChangeName] = new Meeting_ChangeName();
                currentMeetingInfos[(uint)MeetingInfoBase.MeetingType.ChangeDeclaration] = new Meeting_ChangeDeclaration();
                historyMeetingInfos.Clear();
                quitColdTimer?.Cancel();
                quitColdTimer = null;
                createMeetTimer?.Cancel();
                createMeetTimer = null;
                fastInviteTimer?.Cancel();
                fastInviteTimer = null;
                QuitBeforeThinkingTime = 0;
                NextCreateMeetingTime = 0;
                FastInviteTime = 0;
            }

            /// <summary>
            /// 删除超时的历史会议///
            /// </summary>
            public void DelOutTimeHistoryMeeting()
            {
                List<MeetingInfoBase> temps = GetAllSortedHistoryMeetings();

                for (int index = temps.Count -1; index > 0; index--)
                {
                    if (Sys_Time.Instance.GetServerTime() -  temps[index].FinishTime > Instance.historyMeetingMaxTime * 24 * 3600)
                    {
                        if (historyMeetingInfos.ContainsKey(temps[index].MeetingID))
                        {
                            historyMeetingInfos.Remove(temps[index].MeetingID);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                            
                if (historyMeetingInfos.Count >= Instance.historyMeetingMaxCount)
                {
                    temps.Clear();
                    temps = GetAllSortedHistoryMeetings();

                    if (historyMeetingInfos.ContainsKey(temps[temps.Count - 1].MeetingID))
                    {
                        historyMeetingInfos.Remove(temps[temps.Count - 1].MeetingID);
                    }
                }
            }

            public List<MeetingInfoBase> GetAllSortedHistoryMeetings()
            {
                List<MeetingInfoBase> meetings = new List<MeetingInfoBase>();

                foreach (var meeting in historyMeetingInfos.Values)
                {
                    meetings.Add(meeting);
                }

                meetings.Sort((a, b) =>
                {
                    if (a.FinishTime > b.FinishTime)
                        return -1;
                    else
                        return 1;
                });

                return meetings;
            }
        }           

        /// <summary>
        /// 历史会议红点信息///
        /// </summary>
        public class HistoryMeetingRedPointInfo
        {
            public Dictionary<uint, HistoryMeetingRead> meetingDic = new Dictionary<uint, HistoryMeetingRead>();
            public List<HistoryMeetingRead> meetingList = new List<HistoryMeetingRead>();
            const string meetingListFieldName = "meetingList";

            public void DeserializeObject(JsonObject jo)
            {
                meetingDic.Clear();
                meetingList.Clear();
                if (jo.ContainsKey(meetingListFieldName))
                {
                    JsonArray ja = (JsonArray)jo[meetingListFieldName];
                    foreach (var item in ja)
                    {
                        HistoryMeetingRead meetingInfo = new HistoryMeetingRead();
                        meetingInfo.DeserializeObject((JsonObject)item);
                        meetingDic[meetingInfo.meetingID] = meetingInfo;
                    }
                }
            }
        }

        public class HistoryMeetingRead
        {
            public uint meetingID;

            public bool read;

            public void DeserializeObject(JsonObject jo)
            {
                JsonHeler.DeserializeObject(jo, this);
            }
        }

        public HistoryMeetingRedPointInfo historyMeetingRedPointInfo = new HistoryMeetingRedPointInfo();

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        /// <summary>
        /// 事件///
        /// </summary>
        public enum EEvents
        {
            CreatedSuccess,  //创建成功
            BeenDismissed, //被解散
            BeenInvited,   //被邀请
            JoinedSuccess,    //加入成功
            AddedNewMember,     //加入了新人
            RevomedMember,      //删除了成员
            RefrehedDeclaration,    //刷新团宣言
            RefrehedName,           //刷新团名字
            RefrehedLeader,         //刷新团长
            AddedNewActions,        //添加了新动态
            CreatedMeetingSuccess,  //创建会议成功
            AddNewInvite,   //添加一个新邀请
            RemoveInvite,   //删除一个邀请
            RefreshQuitTime,    //更新离开时间
            RefreshCreateMeetTime,  //更新创建会议时间
            RefreshFastInviteTime,  //更新快捷组队时间
            QuitSuccessed,  //退团成功
            AddNewDoingMeeting, //添加一个新的进行中会议
            DelDoingMeeting,    //删除一个进行中的会议
            AddNewHistoryMeeting,   //添加一个历史会议
            DelHistoryMeeting,  //删除一个历史会议
            RefreshVote,    //更新投票
            ReadHistoryMeetingInfo,
        }

        static readonly string historyMeetingRedPointInfoPath = Path.GetFullPath(Framework.Consts.persistentDataPath) + "/{0}/WarriorGroup_HistoryMeeting_RedPointInfo.log";

        /// <summary>
        /// 初始化///
        /// </summary>
        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBraveGroup.BraveGroupMineInfoNtf, OnWarroirInfosInit, CmdBraveGroupBraveGroupMineInfoNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBraveGroup.JoinBraveGroupNtf, OnJoinWarriorGroupSuccess, CmdBraveGroupJoinBraveGroupNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBraveGroup.QuitBraveGroupNtf, OnQuitWarriorGroupSuccess, CmdBraveGroupQuitBraveGroupNtf.Parser);

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBraveGroup.AddMemberNtf, OnAddMemberNtf, CmdBraveGroupAddMemberNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBraveGroup.RemoveMemberNtf, OnRemoveMemberNtf, CmdBraveGroupRemoveMemberNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBraveGroup.FreshMemberBaseInfoNtf, OnFreshMemberBaseInfoNtf, CmdBraveGroupFreshMemberBaseInfoNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBraveGroup.FreshBriefNtf, OnFreshDeclarationNtf, CmdBraveGroupFreshBriefNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBraveGroup.FreshNameNtf, OnFreshNameNtf, CmdBraveGroupFreshNameNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBraveGroup.FreshLeaderNtf, OnFreshLeaderNtf, CmdBraveGroupFreshLeaderNtf.Parser);

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBraveGroup.AddDynamicNtf, OnAddActionNtf, CmdBraveGroupAddDynamicNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBraveGroup.AddInvitedNtf, OnAddInvitedNtf, CmdBraveGroupAddInvitedNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBraveGroup.RemoveInvitedNtf, OnRemoveInvitedNtf, CmdBraveGroupRemoveInvitedNtf.Parser);

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBraveGroup.FreshQuitTimeNtf, OnFreshQuitTimeNtf, CmdBraveGroupFreshQuitTimeNtf.Parser);

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBraveGroup.AddDoingMeetingNtf, OnAddDoingMeeting, CmdBraveGroupAddDoingMeetingNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBraveGroup.RemoveDoingMeetingNtf, OnRemoveDoingMeetingNtf, CmdBraveGroupRemoveDoingMeetingNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBraveGroup.AddFinishMeetingNtf, OnAddFinishMeetingNtf, CmdBraveGroupAddFinishMeetingNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBraveGroup.RemoveFinishMeetingNtf, OnRemoveFinishMeetingNtf, CmdBraveGroupRemoveFinishMeetingNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBraveGroup.FreshMemberVoteResultNtf, OnFreshMemberVoteResult, CmdBraveGroupFreshMemberVoteResultNtf.Parser);

            Sys_Time.Instance.eventEmitter.Handle<uint, uint>(Sys_Time.EEvents.OnTimeNtf, OnTimeNtf, true);
            Sys_Title.Instance.eventEmitter.Handle<uint>(Sys_Title.EEvents.OnShowTitle, OnShowTitle, true);
            uint.TryParse(CSVParam.Instance.GetConfData(1385).str_value, out historyMeetingMaxCount);
            uint.TryParse(CSVParam.Instance.GetConfData(1386).str_value, out historyMeetingMaxTime);
            uint.TryParse(CSVParam.Instance.GetConfData(1388).str_value, out doingMeetingMaxTime);
            uint.TryParse(CSVParam.Instance.GetConfData(1376).str_value, out memberMaxCount);

            MyWarriorGroup.currentMeetingInfos[(uint)MeetingInfoBase.MeetingType.SuggestSelf] = new Meeting_SuggestSelf();
            MyWarriorGroup.currentMeetingInfos[(uint)MeetingInfoBase.MeetingType.Recruit] = new Meeting_Recruit();
            MyWarriorGroup.currentMeetingInfos[(uint)MeetingInfoBase.MeetingType.Fire] = new Meeting_Fire();
            MyWarriorGroup.currentMeetingInfos[(uint)MeetingInfoBase.MeetingType.ChangeName] = new Meeting_ChangeName();
            MyWarriorGroup.currentMeetingInfos[(uint)MeetingInfoBase.MeetingType.ChangeDeclaration] = new Meeting_ChangeDeclaration();
        }

        public override void OnSyncFinished()
        {
            DeserializeHistoryMeetingRedPointInfo();
        }

        /// <summary>
        /// 登出清理///
        /// </summary>
        public override void OnLogout()
        {
            historyMeetingRedPointInfo.meetingDic.Clear();
            historyMeetingRedPointInfo.meetingList.Clear();

            MyWarriorGroup.Dispose();

            MyWarriorGroup.currentMeetingInfos[(uint)MeetingInfoBase.MeetingType.SuggestSelf] = new Meeting_SuggestSelf();
            MyWarriorGroup.currentMeetingInfos[(uint)MeetingInfoBase.MeetingType.Recruit] = new Meeting_Recruit();
            MyWarriorGroup.currentMeetingInfos[(uint)MeetingInfoBase.MeetingType.Fire] = new Meeting_Fire();
            MyWarriorGroup.currentMeetingInfos[(uint)MeetingInfoBase.MeetingType.ChangeName] = new Meeting_ChangeName();
            MyWarriorGroup.currentMeetingInfos[(uint)MeetingInfoBase.MeetingType.ChangeDeclaration] = new Meeting_ChangeDeclaration();

            invitedInfoDict.Clear();
            QuitAfterColdTime = 0;

            base.OnLogout();
        }

        void DeserializeHistoryMeetingRedPointInfo()
        {
            string jsonStr = JsonHeler.GetJsonStr(string.Format(historyMeetingRedPointInfoPath, Sys_Role.Instance.RoleId));
            JsonObject jo = (JsonObject)JsonSerializer.Deserialize(jsonStr);
            historyMeetingRedPointInfo.DeserializeObject(jo);
        }

        public void SerializeHistoryMeetingRedPointInfo()
        {
            historyMeetingRedPointInfo.meetingList.Clear();
            foreach (var meetingInfo in historyMeetingRedPointInfo.meetingDic.Values)
            {
                historyMeetingRedPointInfo.meetingList.Add(meetingInfo);
            }
            JsonHeler.SerializeToJsonFile(historyMeetingRedPointInfo, string.Format(historyMeetingRedPointInfoPath, Sys_Role.Instance.RoleId.ToString()));
        }

        #region Call

        /// <summary>
        /// 请求创建勇士团///
        /// </summary>
        /// <param name="groupName">勇士团名字</param>
        public void ReqCreateWarriorGroup(string groupName)
        {
            uint itemID = uint.Parse(CSVParam.Instance.GetConfData(1371).str_value);
            if (Sys_Bag.Instance.GetItemCount(itemID) <= 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13543));
                return;
            }

            CmdBraveGroupCreateBraveGroupReq req = new CmdBraveGroupCreateBraveGroupReq();
            req.BraveGroupName = FrameworkTool.ConvertToGoogleByteString(groupName);
            req.ItemId = Sys_Bag.Instance.GetUuidsByItemId(uint.Parse(CSVParam.Instance.GetConfData(1371).str_value))[0];

            NetClient.Instance.SendMessage((ushort)CmdBraveGroup.CreateBraveGroupReq, req);
        }

        /// <summary>
        /// 请求转移团长///
        /// </summary>
        /// <param name="oldLeaderRoleID"></param>
        /// <param name="newLeaderRoleID"></param>
        public void ReqResetLeader(ulong newLeaderRoleID)
        {
            CmdBraveGroupResetLeaderReq req = new CmdBraveGroupResetLeaderReq();
            req.NewLeaderId = newLeaderRoleID;

            NetClient.Instance.SendMessage((ushort)CmdBraveGroup.ResetLeaderReq, req);
        }

        /// <summary>
        /// 请求邀请好友加入勇者团///
        /// </summary>
        /// <param name="friendRoleID"></param>
        public void ReqInviteFriendIntoGroup(ulong friendRoleID)
        {
            CmdBraveGroupLeaderInviteFriendReq req = new CmdBraveGroupLeaderInviteFriendReq();
            req.FriendIds.Add(friendRoleID);

            NetClient.Instance.SendMessage((ushort)CmdBraveGroup.LeaderInviteFriendReq, req);
        }

        /// <summary>
        /// 同意邀请///
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="groupID"></param>
        public void ReqAgreeInvite(ulong roleID, ulong groupID)
        {
            CmdBraveGroupAgreeInviteReq req = new CmdBraveGroupAgreeInviteReq();
            req.RoleId = roleID;
            req.BraveGroupId = groupID;
            NetClient.Instance.SendMessage((ushort)CmdBraveGroup.AgreeInviteReq, req);

            RemoveInvite(roleID);
        }

        /// <summary>
        /// 拒绝邀请///
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="groupID"></param>
        public void ReqRefuseInvite(ulong roleID, ulong groupID)
        {
            CmdBraveGroupRefuseInviteReq req = new CmdBraveGroupRefuseInviteReq();
            req.RoleId = roleID;
            req.BraveGroupId = groupID;

            NetClient.Instance.SendMessage((ushort)CmdBraveGroup.RefuseInviteReq, req);

            RemoveInvite(roleID);
        }

        /// <summary>
        /// 请求离团///
        /// </summary>
        public void ReqQuit()
        {
            CmdBraveGroupFirstQuitReq req = new CmdBraveGroupFirstQuitReq();
            NetClient.Instance.SendMessage((ushort)CmdBraveGroup.FirstQuitReq, req);
        }

        /// <summary>
        /// 请求取消离团///
        /// </summary>
        public void ReqCancelQuit()
        {
            CmdBraveGroupCancelFirstQuitReq req = new CmdBraveGroupCancelFirstQuitReq();
            NetClient.Instance.SendMessage((ushort)CmdBraveGroup.CancelFirstQuitReq, req);
        }

        /// <summary>
        /// 请求创建会议_自荐///
        /// </summary>
        /// <param name="infoID"></param>
        /// <param name="str"></param>
        public void ReqCreateMeeting_SuggestSelf(uint infoID, string str)
        {
            CmdBraveGroupCreateMeetingReq req = new CmdBraveGroupCreateMeetingReq();
            req.TableId = infoID;
            req.ParamList = new BraveGroupMeetingParamList();
            req.ParamList.Params.Add(new BraveGroupParam()
            {
                StrParam = FrameworkTool.ConvertToGoogleByteString(str)
            });
            NetClient.Instance.SendMessage((ushort)CmdBraveGroup.CreateMeetingReq, req);
        }

        /// <summary>
        /// 请求创建会议_招募///
        /// </summary>
        /// <param name="infoID"></param>
        /// <param name="roleID"></param>
        public void ReqCreateMeeting_Recruit(uint infoID, ulong roleID)
        {
            CmdBraveGroupCreateMeetingReq req = new CmdBraveGroupCreateMeetingReq();
            req.TableId = infoID;
            req.ParamList = new BraveGroupMeetingParamList();
            req.ParamList.Params.Add(new BraveGroupParam()
            {
                 IntParam = roleID
            });
            NetClient.Instance.SendMessage((ushort)CmdBraveGroup.CreateMeetingReq, req);
        }

        /// <summary>
        /// 请求创建会议_请离///
        /// </summary>
        /// <param name="infoID"></param>
        /// <param name="roleID"></param>
        public void ReqCreateMeeting_Fire(uint infoID, ulong roleID)
        {
            CmdBraveGroupCreateMeetingReq req = new CmdBraveGroupCreateMeetingReq();
            req.TableId = infoID;
            req.ParamList = new BraveGroupMeetingParamList();
            req.ParamList.Params.Add(new BraveGroupParam()
            {
                IntParam = roleID
            });
            NetClient.Instance.SendMessage((ushort)CmdBraveGroup.CreateMeetingReq, req);
        }

        /// <summary>
        /// 请求创建会议_改名///
        /// </summary>
        /// <param name="infoID"></param>
        /// <param name="str"></param>
        public void ReqCreateMeeting_ChangeName(uint infoID, string str)
        {
            CmdBraveGroupCreateMeetingReq req = new CmdBraveGroupCreateMeetingReq();
            req.TableId = infoID;
            req.ParamList = new BraveGroupMeetingParamList();
            req.ParamList.Params.Add(new BraveGroupParam()
            {
                StrParam = FrameworkTool.ConvertToGoogleByteString(str)
            });
            NetClient.Instance.SendMessage((ushort)CmdBraveGroup.CreateMeetingReq, req);
        }

        /// <summary>
        /// 请求创建会议_改宣言///
        /// </summary>
        /// <param name="infoID"></param>
        /// <param name="str"></param>
        public void ReqCreateMeeting_ChangeDeclaration(uint infoID, string str)
        {
            CmdBraveGroupCreateMeetingReq req = new CmdBraveGroupCreateMeetingReq();
            req.TableId = infoID;
            req.ParamList = new BraveGroupMeetingParamList();
            req.ParamList.Params.Add(new BraveGroupParam()
            {
                StrParam = FrameworkTool.ConvertToGoogleByteString(str)
            });
            NetClient.Instance.SendMessage((ushort)CmdBraveGroup.CreateMeetingReq, req);
        }

        /// <summary>
        /// 请求对会议投赞成票///
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="meetingID"></param>
        public void ReqMeetingVoteAgree(uint meetingID)
        {
            CmdBraveGroupMeetingVoteAgreeReq req = new CmdBraveGroupMeetingVoteAgreeReq();
            req.MeetingId = meetingID;

            NetClient.Instance.SendMessage((ushort)CmdBraveGroup.MeetingVoteAgreeReq, req);
        }

        /// <summary>
        /// 请求对会议投反对票///
        /// </summary>
        /// <param name="meetingID"></param>
        public void ReqMeetingVoteRefuse(uint meetingID)
        {
            CmdBraveGroupMeetingVoteRefuseReq req = new CmdBraveGroupMeetingVoteRefuseReq();
            req.MeetingId = meetingID;

            NetClient.Instance.SendMessage((ushort)CmdBraveGroup.MeetingVoteRefuseReq, req);
        }

        /// <summary>
        /// 请求快速组队///
        /// </summary>
        public void ReqFastTeam()
        {
            if (Sys_Team.Instance.HaveTeam && !Sys_Team.Instance.isCaptain(Sys_Role.Instance.RoleId))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2002152));
            }
            else
            {
                CmdBraveGroupFastInviteTeamReq req = new CmdBraveGroupFastInviteTeamReq();
                NetClient.Instance.SendMessage((ushort)CmdBraveGroup.FastInviteTeamReq, req);
            }
        }

        #endregion

        #region CallBack

        /// <summary>
        /// 勇士团信息初始化///
        /// </summary>
        void OnWarroirInfosInit(NetMsg msg)
        {
            CmdBraveGroupBraveGroupMineInfoNtf ntf = NetMsgUtil.Deserialize<CmdBraveGroupBraveGroupMineInfoNtf>(CmdBraveGroupBraveGroupMineInfoNtf.Parser, msg);
            if (ntf != null)
            {
                if (ntf.Bgroup != null)
                {
                    if (Sys_Title.Instance.infoReceived)
                    {
                        Sys_Title.Instance.UpdateBGroupTitle();    
                    }
                    MyWarriorGroup.GroupUID = ntf.Bgroup.Base.BraveGroupId;
                    MyWarriorGroup.GroupName = ntf.Bgroup.Base.BraveGroupName.ToStringUtf8();
                    if (ntf.Bgroup.Base.BraveGroupBrief.Length == 0)
                    {
                        MyWarriorGroup.GroupDeclaration = LanguageHelper.GetTextContent(13542);
                    }
                    else
                    {
                        MyWarriorGroup.GroupDeclaration = ntf.Bgroup.Base.BraveGroupBrief.ToStringUtf8();
                    }
                    MyWarriorGroup.LeaderRoleID = ntf.Bgroup.Base.LeaderId;
                    MyWarriorGroup.JoinBraveGroupTime = ntf.Info.MemPrivate.JoinBraveGroupTime;

                    uint currentTime = Sys_Time.Instance.GetServerTime();

                    if (ntf.Info.MemPrivate.NextCreateMeetingTime >= currentTime)
                        MyWarriorGroup.NextCreateMeetingTime = ntf.Info.MemPrivate.NextCreateMeetingTime - currentTime;
                    else
                        MyWarriorGroup.NextCreateMeetingTime = 0;

                    if (ntf.Info.MemPrivate.QuitBeforeThinkingTime >= currentTime)
                        MyWarriorGroup.QuitBeforeThinkingTime = ntf.Info.MemPrivate.QuitBeforeThinkingTime - currentTime;
                    else
                        MyWarriorGroup.QuitBeforeThinkingTime = 0;

                    if (ntf.Info.MemPrivate.FastInviteTeamTime >= currentTime)
                        MyWarriorGroup.FastInviteTime = ntf.Info.MemPrivate.FastInviteTeamTime - currentTime;
                    else
                        MyWarriorGroup.FastInviteTime = 0;

                    if (MyWarriorGroup.QuitBeforeThinkingTime > 0)
                    {
                        MyWarriorGroup.quitColdTimer?.Cancel();
                        MyWarriorGroup.quitColdTimer = Timer.Register(1f, () =>
                        {
                            if (MyWarriorGroup.QuitBeforeThinkingTime >= 1)
                                MyWarriorGroup.QuitBeforeThinkingTime--;
                        }, null, true, true);
                    }
                    else
                    {
                        MyWarriorGroup.QuitBeforeThinkingTime = 0;
                    }

                    if (MyWarriorGroup.NextCreateMeetingTime > 0)
                    {
                        MyWarriorGroup.createMeetTimer?.Cancel();
                        MyWarriorGroup.createMeetTimer = Timer.Register(1f, () =>
                        {
                            if (MyWarriorGroup.NextCreateMeetingTime >= 1)
                                MyWarriorGroup.NextCreateMeetingTime--;
                        }, null, true, true);
                    }
                    else
                    {
                        MyWarriorGroup.NextCreateMeetingTime = 0;
                    }

                    if (MyWarriorGroup.FastInviteTime > 0)
                    {
                        MyWarriorGroup.fastInviteTimer?.Cancel();
                        MyWarriorGroup.fastInviteTimer = Timer.Register(1f, () =>
                        {
                            if (MyWarriorGroup.FastInviteTime >= 1)
                                MyWarriorGroup.FastInviteTime--;
                        }, null, true, true);
                    }
                    else
                    {
                        MyWarriorGroup.FastInviteTime = 0;
                    }

                    MyWarriorGroup.warriorInfos.Clear();
                    if (ntf.Bgroup.MemberList != null && ntf.Bgroup.MemberList.Members != null && ntf.Bgroup.MemberList.Members.Count > 0)
                    {
                        for (int index = 0, len = ntf.Bgroup.MemberList.Members.Count; index < len; index++)
                        {
                            WarriorInfo warriorInfo = new WarriorInfo()
                            {
                                RoleID = ntf.Bgroup.MemberList.Members[index].Base.RoleId,
                                RoleName = ntf.Bgroup.MemberList.Members[index].Base.RoleName.ToStringUtf8(),
                                Level = ntf.Bgroup.MemberList.Members[index].Base.Level,
                                HeroID = ntf.Bgroup.MemberList.Members[index].Base.HeroId,
                                IconID = ntf.Bgroup.MemberList.Members[index].Base.HeadPhoto,
                                FrameID = ntf.Bgroup.MemberList.Members[index].Base.Frame,
                                WeaponID = ntf.Bgroup.MemberList.Members[index].Base.Weapon,
                                Occ = ntf.Bgroup.MemberList.Members[index].Base.Career,
                                DressID = ntf.Bgroup.MemberList.Members[index].Base.Fashion.FashionInfos[0].FashionId,
                                DressData = Sys_Fashion.Instance.GetDressData(ntf.Bgroup.MemberList.Members[index].Base.Fashion.FashionInfos, ntf.Bgroup.MemberList.Members[index].Base.HeroId),
                                TitleID = ntf.Bgroup.MemberList.Members[index].Base.TitleId,
                                LastOffline = ntf.Bgroup.MemberList.Members[index].Base.LastOffline,
                                GroupName = ntf.Bgroup.Base.BraveGroupName.ToStringUtf8(),
                            };
                            warriorInfo.IsLeader = warriorInfo.RoleID == ntf.Bgroup.Base.LeaderId ? true : false;
                            MyWarriorGroup.warriorInfos[warriorInfo.RoleID] = warriorInfo;
                        }
                    }

                    MyWarriorGroup.actionInfos.Clear();
                    if (ntf.Bgroup.DynamicList != null && ntf.Bgroup.DynamicList.Dynamics != null && ntf.Bgroup.DynamicList.Dynamics.Count > 0)
                    {
                        for (int index = 0, len = ntf.Bgroup.DynamicList.Dynamics.Count; index < len; index++)
                        {
                            ActionInfo actionInfo = new ActionInfo(ntf.Bgroup.DynamicList.Dynamics[index].TableId,
                                ntf.Bgroup.DynamicList.Dynamics[index].CreateTime);
                            actionInfo.FillParamList(ntf.Bgroup.DynamicList.Dynamics[index].ParamList);
                            MyWarriorGroup.actionInfos.Add(actionInfo);
                        }
                    }

                    MyWarriorGroup.historyMeetingInfos.Clear();
                    if (ntf.Bgroup.Finish != null && ntf.Bgroup.Finish.Meetings != null && ntf.Bgroup.Finish.Meetings.Count > 0)
                    {
                        for (int index = 0, len = ntf.Bgroup.Finish.Meetings.Count; index < len; index++)
                        {
                            MeetingInfoBase meetingInfo = null;
                            if (ntf.Bgroup.Finish.Meetings[index].Base.TableId == (uint)MeetingInfoBase.MeetingType.SuggestSelf)
                                meetingInfo = new Meeting_SuggestSelf();
                            else if (ntf.Bgroup.Finish.Meetings[index].Base.TableId == (uint)MeetingInfoBase.MeetingType.Recruit)
                                meetingInfo = new Meeting_Recruit();
                            else if (ntf.Bgroup.Finish.Meetings[index].Base.TableId == (uint)MeetingInfoBase.MeetingType.Fire)
                                meetingInfo = new Meeting_Fire();
                            else if (ntf.Bgroup.Finish.Meetings[index].Base.TableId == (uint)MeetingInfoBase.MeetingType.ChangeName)
                                meetingInfo = new Meeting_ChangeName();
                            else if (ntf.Bgroup.Finish.Meetings[index].Base.TableId == (uint)MeetingInfoBase.MeetingType.ChangeDeclaration)
                                meetingInfo = new Meeting_ChangeDeclaration();

                            if (meetingInfo != null)
                            {
                                Dictionary<ulong, string> voteName = new Dictionary<ulong, string>();
                                List<ulong> agreeList = new List<ulong>();
                                if (ntf.Bgroup.Finish.Meetings[index].AgreeList != null && ntf.Bgroup.Finish.Meetings[index].AgreeList.Members != null && ntf.Bgroup.Finish.Meetings[index].AgreeList.Members.Count > 0)
                                {
                                    for (int index2 = 0, len2 = ntf.Bgroup.Finish.Meetings[index].AgreeList.Members.Count; index2 < len2; index2++)
                                    {
                                        agreeList.Add(ntf.Bgroup.Finish.Meetings[index].AgreeList.Members[index2].RoleId);
                                        voteName[ntf.Bgroup.Finish.Meetings[index].AgreeList.Members[index2].RoleId] = ntf.Bgroup.Finish.Meetings[index].AgreeList.Members[index2].RoleName.ToStringUtf8();
                                    }
                                }
                                List<ulong> refuseList = new List<ulong>();
                                if (ntf.Bgroup.Finish.Meetings[index].RefuseList != null && ntf.Bgroup.Finish.Meetings[index].RefuseList.Members != null && ntf.Bgroup.Finish.Meetings[index].RefuseList.Members.Count > 0)
                                {
                                    for (int index2 = 0, len2 = ntf.Bgroup.Finish.Meetings[index].RefuseList.Members.Count; index2 < len2; index2++)
                                    {
                                        refuseList.Add(ntf.Bgroup.Finish.Meetings[index].RefuseList.Members[index2].RoleId);
                                        voteName[ntf.Bgroup.Finish.Meetings[index].RefuseList.Members[index2].RoleId] = ntf.Bgroup.Finish.Meetings[index].RefuseList.Members[index2].RoleName.ToStringUtf8();
                                    }
                                }

                                meetingInfo.SetHistory(ntf.Bgroup.Finish.Meetings[index].Base.MeetingId,
                                    ntf.Bgroup.Finish.Meetings[index].Base.CreateRoleId,
                                    ntf.Bgroup.Finish.Meetings[index].Base.CreateRoleName.ToStringUtf8(),
                                    ntf.Bgroup.Finish.Meetings[index].Result, ntf.Bgroup.Finish.Meetings[index].FinishTime,
                                    ntf.Bgroup.Finish.Meetings[index].ParamList, agreeList, refuseList, voteName);
                                MyWarriorGroup.historyMeetingInfos[ntf.Bgroup.Finish.Meetings[index].Base.MeetingId] = meetingInfo;

                                historyMeetingRedPointInfo.meetingDic[meetingInfo.MeetingID] = new HistoryMeetingRead()
                                {
                                    meetingID = meetingInfo.MeetingID,
                                    read = false,
                                };
                            }
                        }
                    }
                    MyWarriorGroup.DelOutTimeHistoryMeeting();

                    if (ntf.Bgroup.Doing != null && ntf.Bgroup.Doing.Meetings != null && ntf.Bgroup.Doing.Meetings.Count > 0)
                    {
                        for (int index = 0, len = ntf.Bgroup.Doing.Meetings.Count; index < len; index++)
                        {
                            MeetingInfoBase meetingInfo = null;
                            if (ntf.Bgroup.Doing.Meetings[index].Base.TableId == (uint)MeetingInfoBase.MeetingType.SuggestSelf)
                                meetingInfo = new Meeting_SuggestSelf();
                            else if (ntf.Bgroup.Doing.Meetings[index].Base.TableId == (uint)MeetingInfoBase.MeetingType.Recruit)
                                meetingInfo = new Meeting_Recruit();
                            else if (ntf.Bgroup.Doing.Meetings[index].Base.TableId == (uint)MeetingInfoBase.MeetingType.Fire)
                                meetingInfo = new Meeting_Fire();
                            else if (ntf.Bgroup.Doing.Meetings[index].Base.TableId == (uint)MeetingInfoBase.MeetingType.ChangeName)
                                meetingInfo = new Meeting_ChangeName();
                            else if (ntf.Bgroup.Doing.Meetings[index].Base.TableId == (uint)MeetingInfoBase.MeetingType.ChangeDeclaration)
                                meetingInfo = new Meeting_ChangeDeclaration();

                            if (meetingInfo != null)
                            {
                                List<ulong> agreeList = new List<ulong>();
                                if (ntf.Bgroup.Doing.Meetings[index].AgreeList != null && ntf.Bgroup.Doing.Meetings[index].AgreeList.Count > 0)
                                {
                                    for (int index2 = 0, len2 = ntf.Bgroup.Doing.Meetings[index].AgreeList.Count; index2 < len2; index2++)
                                    {
                                        agreeList.Add(ntf.Bgroup.Doing.Meetings[index].AgreeList[index2]);
                                    }
                                }
                                List<ulong> refuseList = new List<ulong>();
                                if (ntf.Bgroup.Doing.Meetings[index].RefuseList != null && ntf.Bgroup.Doing.Meetings[index].RefuseList.Count > 0)
                                {
                                    for (int index2 = 0, len2 = ntf.Bgroup.Doing.Meetings[index].RefuseList.Count; index2 < len2; index2++)
                                    {
                                        refuseList.Add(ntf.Bgroup.Doing.Meetings[index].RefuseList[index2]);
                                    }
                                }

                                meetingInfo.SetDoing(ntf.Bgroup.Doing.Meetings[index].Base.MeetingId,
                                    ntf.Bgroup.Doing.Meetings[index].Base.CreateRoleId,
                                    ntf.Bgroup.Doing.Meetings[index].Base.CreateRoleName.ToStringUtf8(),
                                    ntf.Bgroup.Doing.Meetings[index].Base.CreateTime,
                                    ntf.Bgroup.Doing.Meetings[index].ParamList,
                                    agreeList, refuseList);
                                MyWarriorGroup.currentMeetingInfos[meetingInfo.MeetingID] = meetingInfo;
                            }
                        }
                    }
                }
                else
                {
                    if (ntf.Info.NotMemPrivate != null)
                        QuitAfterColdTime = ntf.Info.NotMemPrivate.QuitAfterColdTime;

                    invitedInfoDict.Clear();
                    if (ntf.Info.NotMemInvite != null && ntf.Info.NotMemInvite.Inviteds != null && ntf.Info.NotMemInvite.Inviteds.Count > 0)
                    {
                        for (int index = 0, len = ntf.Info.NotMemInvite.Inviteds.Count; index < len; index++)
                        {
                            InvitedInfo invitedInfo = new InvitedInfo()
                            {
                                RoleID = ntf.Info.NotMemInvite.Inviteds[index].RoleId,
                                Time = ntf.Info.NotMemInvite.Inviteds[index].InviteTime,
                            };
                            invitedInfo.WarriorGroup = new WarriorGroup();
                            invitedInfo.WarriorGroup.GroupUID = ntf.Info.NotMemInvite.Inviteds[index].Base.BraveGroupId;
                            invitedInfo.WarriorGroup.GroupName = ntf.Info.NotMemInvite.Inviteds[index].Base.BraveGroupName.ToStringUtf8();
                            invitedInfo.WarriorGroup.warriorInfos.Clear();

                            if (ntf.Info.NotMemInvite.Inviteds[index].Members != null && ntf.Info.NotMemInvite.Inviteds[index].Members.Count > 0)
                            {
                                for (int index2 = 0, len2 = ntf.Info.NotMemInvite.Inviteds[index].Members.Count; index2 < len2; index2++)
                                {
                                    WarriorInfo warriorInfo = new WarriorInfo()
                                    {
                                        RoleID = ntf.Info.NotMemInvite.Inviteds[index].Members[index2].RoleId,
                                        RoleName = ntf.Info.NotMemInvite.Inviteds[index].Members[index2].RoleName.ToStringUtf8(),
                                        Level = ntf.Info.NotMemInvite.Inviteds[index].Members[index2].Level,
                                        HeroID = ntf.Info.NotMemInvite.Inviteds[index].Members[index2].HeroId,
                                        IconID = ntf.Info.NotMemInvite.Inviteds[index].Members[index2].HeadPhoto,
                                        FrameID = ntf.Info.NotMemInvite.Inviteds[index].Members[index2].Frame,
                                        Occ = ntf.Info.NotMemInvite.Inviteds[index].Members[index2].Career,
                                    };
                                    invitedInfo.WarriorGroup.warriorInfos[warriorInfo.RoleID] = warriorInfo;
                                }
                            }

                            invitedInfoDict[ntf.Info.NotMemInvite.Inviteds[index].RoleId] = invitedInfo;
                        }

                        if (invitedInfoDict != null && invitedInfoDict.Count > 0)
                        {
                            foreach (var invite in invitedInfoDict.Values)
                            {
                                Sys_MessageBag.MessageContent messageContent = new Sys_MessageBag.MessageContent();
                                messageContent.mType = EMessageBagType.BraveTeam;
                                messageContent.invitorId = invite.RoleID;
                                messageContent.invitiorName = invite.RoleName;
                                messageContent.cMess = new Sys_MessageBag.GuildMessage()
                                {
                                    guildId = invite.WarriorGroup.GroupUID,
                                    guildName = invite.WarriorGroup.GroupName,
                                };
                                Sys_MessageBag.Instance.SendMessageInfo(EMessageBagType.BraveTeam, messageContent);
                            }
                        }
                    }                 
                }
            }
        }

        /// <summary>
        /// 加入勇者团成功通知///
        /// </summary>
        /// <param name="msg"></param>
        void OnJoinWarriorGroupSuccess(NetMsg msg)
        {
            CmdBraveGroupJoinBraveGroupNtf ntf = NetMsgUtil.Deserialize<CmdBraveGroupJoinBraveGroupNtf>(CmdBraveGroupJoinBraveGroupNtf.Parser, msg);
            if (ntf != null)
            {
                if (Sys_Title.Instance.infoReceived)
                {
                    Sys_Title.Instance.UpdateBGroupTitle();    
                }
                MyWarriorGroup.GroupUID = ntf.Info.Base.BraveGroupId;
                MyWarriorGroup.GroupName = ntf.Info.Base.BraveGroupName.ToStringUtf8();
                if (ntf.Info.Base.BraveGroupBrief.Length == 0)
                {
                    MyWarriorGroup.GroupDeclaration = LanguageHelper.GetTextContent(13542);
                }
                else
                {
                    MyWarriorGroup.GroupDeclaration = ntf.Info.Base.BraveGroupBrief.ToStringUtf8();
                }
                MyWarriorGroup.LeaderRoleID = ntf.Info.Base.LeaderId;
                MyWarriorGroup.JoinBraveGroupTime = ntf.Mine.MemPrivate.JoinBraveGroupTime;
                uint currentTime = Sys_Time.Instance.GetServerTime();
                if (ntf.Mine.MemPrivate.NextCreateMeetingTime >= currentTime)
                    MyWarriorGroup.NextCreateMeetingTime = ntf.Mine.MemPrivate.NextCreateMeetingTime - currentTime;
                else
                    MyWarriorGroup.NextCreateMeetingTime = 0;

                if (ntf.Mine.MemPrivate.FastInviteTeamTime >= currentTime)
                    MyWarriorGroup.FastInviteTime = ntf.Mine.MemPrivate.FastInviteTeamTime - currentTime;
                else
                    MyWarriorGroup.FastInviteTime = 0;

                if (ntf.Mine.MemPrivate.QuitBeforeThinkingTime >= currentTime)
                    MyWarriorGroup.QuitBeforeThinkingTime = ntf.Mine.MemPrivate.QuitBeforeThinkingTime - currentTime;
                else
                    MyWarriorGroup.QuitBeforeThinkingTime = 0;

                if (MyWarriorGroup.QuitBeforeThinkingTime > 0)
                {
                    MyWarriorGroup.quitColdTimer?.Cancel();
                    MyWarriorGroup.quitColdTimer = Timer.Register(1f, () =>
                    {
                        if (MyWarriorGroup.QuitBeforeThinkingTime >= 1)
                            MyWarriorGroup.QuitBeforeThinkingTime--;
                    }, null, true, true);
                }
                else
                {
                    MyWarriorGroup.QuitBeforeThinkingTime = 0;
                }

                if (MyWarriorGroup.NextCreateMeetingTime > 0)
                {
                    MyWarriorGroup.createMeetTimer?.Cancel();
                    MyWarriorGroup.createMeetTimer = Timer.Register(1f, () =>
                    {
                        if (MyWarriorGroup.NextCreateMeetingTime >= 1)
                            MyWarriorGroup.NextCreateMeetingTime--;
                    }, null, true, true);
                }
                else
                {
                    MyWarriorGroup.NextCreateMeetingTime = 0;
                }

                if (MyWarriorGroup.FastInviteTime > 0)
                {
                    MyWarriorGroup.fastInviteTimer?.Cancel();
                    MyWarriorGroup.fastInviteTimer = Timer.Register(1f, () =>
                    {
                        if (MyWarriorGroup.FastInviteTime >= 1)
                            MyWarriorGroup.FastInviteTime--;
                    }, null, true, true);
                }
                else
                {
                    MyWarriorGroup.FastInviteTime = 0;
                }

                MyWarriorGroup.warriorInfos.Clear();
                if (ntf.Info.MemberList != null && ntf.Info.MemberList.Members != null && ntf.Info.MemberList.Members.Count > 0)
                {
                    for (int index = 0, len = ntf.Info.MemberList.Members.Count; index < len; index++)
                    {
                        WarriorInfo warriorInfo = new WarriorInfo()
                        {
                            RoleID = ntf.Info.MemberList.Members[index].Base.RoleId,
                            RoleName = ntf.Info.MemberList.Members[index].Base.RoleName.ToStringUtf8(),
                            Level = ntf.Info.MemberList.Members[index].Base.Level,
                            HeroID = ntf.Info.MemberList.Members[index].Base.HeroId,
                            IconID = ntf.Info.MemberList.Members[index].Base.HeadPhoto,
                            FrameID = ntf.Info.MemberList.Members[index].Base.Frame,
                            WeaponID = ntf.Info.MemberList.Members[index].Base.Weapon,
                            Occ = ntf.Info.MemberList.Members[index].Base.Career,
                            DressID = ntf.Info.MemberList.Members[index].Base.Fashion.FashionInfos[0].FashionId,
                            DressData = Sys_Fashion.Instance.GetDressData(ntf.Info.MemberList.Members[index].Base.Fashion.FashionInfos, ntf.Info.MemberList.Members[index].Base.HeroId),
                            TitleID = ntf.Info.MemberList.Members[index].Base.TitleId,
                            LastOffline = ntf.Info.MemberList.Members[index].Base.LastOffline,
                            GroupName = ntf.Info.Base.BraveGroupName.ToStringUtf8(),
                        };
                        warriorInfo.IsLeader = warriorInfo.RoleID == ntf.Info.Base.LeaderId ? true : false;
                        MyWarriorGroup.warriorInfos[warriorInfo.RoleID] = warriorInfo;
                    }
                }

                MyWarriorGroup.actionInfos.Clear();
                if (ntf.Info.DynamicList != null && ntf.Info.DynamicList.Dynamics != null && ntf.Info.DynamicList.Dynamics.Count > 0)
                {
                    for (int index = 0, len = ntf.Info.DynamicList.Dynamics.Count; index < len; index++)
                    {
                        ActionInfo actionInfo = new ActionInfo(ntf.Info.DynamicList.Dynamics[index].TableId,
                            ntf.Info.DynamicList.Dynamics[index].CreateTime);
                        actionInfo.FillParamList(ntf.Info.DynamicList.Dynamics[index].ParamList);
                        MyWarriorGroup.actionInfos.Add(actionInfo);
                    }
                }

                MyWarriorGroup.historyMeetingInfos.Clear();
                if (ntf.Info.Finish != null && ntf.Info.Finish.Meetings != null && ntf.Info.Finish.Meetings.Count > 0)
                {
                    for (int index = 0, len = ntf.Info.Finish.Meetings.Count; index < len; index++)
                    {
                        MeetingInfoBase meetingInfo = null;
                        if (ntf.Info.Finish.Meetings[index].Base.TableId == (uint)MeetingInfoBase.MeetingType.SuggestSelf)
                            meetingInfo = new Meeting_SuggestSelf();
                        else if (ntf.Info.Finish.Meetings[index].Base.TableId == (uint)MeetingInfoBase.MeetingType.Recruit)
                            meetingInfo = new Meeting_Recruit();
                        else if (ntf.Info.Finish.Meetings[index].Base.TableId == (uint)MeetingInfoBase.MeetingType.Fire)
                            meetingInfo = new Meeting_Fire();
                        else if (ntf.Info.Finish.Meetings[index].Base.TableId == (uint)MeetingInfoBase.MeetingType.ChangeName)
                            meetingInfo = new Meeting_ChangeName();
                        else if (ntf.Info.Finish.Meetings[index].Base.TableId == (uint)MeetingInfoBase.MeetingType.ChangeDeclaration)
                            meetingInfo = new Meeting_ChangeDeclaration();

                        if (meetingInfo != null)
                        {
                            Dictionary<ulong, string> voteName = new Dictionary<ulong, string>();
                            List<ulong> agreeList = new List<ulong>();
                            if (ntf.Info.Finish.Meetings[index].AgreeList != null && ntf.Info.Finish.Meetings[index].AgreeList.Members != null && ntf.Info.Finish.Meetings[index].AgreeList.Members.Count > 0)
                            {
                                for (int index2 = 0, len2 = ntf.Info.Finish.Meetings[index].AgreeList.Members.Count; index2 < len2; index2++)
                                {
                                    agreeList.Add(ntf.Info.Finish.Meetings[index].AgreeList.Members[index2].RoleId);
                                    voteName[ntf.Info.Finish.Meetings[index].AgreeList.Members[index2].RoleId] = ntf.Info.Finish.Meetings[index].AgreeList.Members[index2].RoleName.ToStringUtf8();
                                }
                            }

                            List<ulong> refuseList = new List<ulong>();
                            if (ntf.Info.Finish.Meetings[index].RefuseList != null && ntf.Info.Finish.Meetings[index].RefuseList.Members != null && ntf.Info.Finish.Meetings[index].RefuseList.Members.Count > 0)
                            {
                                for (int index2 = 0, len2 = ntf.Info.Finish.Meetings[index].RefuseList.Members.Count; index2 < len2; index2++)
                                {
                                    refuseList.Add(ntf.Info.Finish.Meetings[index].RefuseList.Members[index2].RoleId);
                                    voteName[ntf.Info.Finish.Meetings[index].RefuseList.Members[index2].RoleId] = ntf.Info.Finish.Meetings[index].RefuseList.Members[index2].RoleName.ToStringUtf8();
                                }
                            }

                            meetingInfo.SetHistory(ntf.Info.Finish.Meetings[index].Base.MeetingId,
                                ntf.Info.Finish.Meetings[index].Base.CreateRoleId,
                                ntf.Info.Finish.Meetings[index].Base.CreateRoleName.ToStringUtf8(),
                                ntf.Info.Finish.Meetings[index].Result, ntf.Info.Finish.Meetings[index].FinishTime,
                                ntf.Info.Finish.Meetings[index].ParamList, agreeList, refuseList, voteName);
                            MyWarriorGroup.historyMeetingInfos[ntf.Info.Finish.Meetings[index].Base.MeetingId] = meetingInfo;

                            historyMeetingRedPointInfo.meetingDic[meetingInfo.MeetingID] = new HistoryMeetingRead()
                            {
                                 meetingID = meetingInfo.MeetingID,
                                 read = false,
                            };
                        }
                    }
                    SerializeHistoryMeetingRedPointInfo();
                }

                if (ntf.Info.Doing != null && ntf.Info.Doing.Meetings != null && ntf.Info.Doing.Meetings.Count > 0)
                {
                    for (int index = 0, len = ntf.Info.Doing.Meetings.Count; index < len; index++)
                    {
                        MeetingInfoBase meetingInfo = null;
                        if (ntf.Info.Doing.Meetings[index].Base.TableId == (uint)MeetingInfoBase.MeetingType.SuggestSelf)
                            meetingInfo = new Meeting_SuggestSelf();
                        else if (ntf.Info.Doing.Meetings[index].Base.TableId == (uint)MeetingInfoBase.MeetingType.Recruit)
                            meetingInfo = new Meeting_Recruit();
                        else if (ntf.Info.Doing.Meetings[index].Base.TableId == (uint)MeetingInfoBase.MeetingType.Fire)
                            meetingInfo = new Meeting_Fire();
                        else if (ntf.Info.Doing.Meetings[index].Base.TableId == (uint)MeetingInfoBase.MeetingType.ChangeName)
                            meetingInfo = new Meeting_ChangeName();
                        else if (ntf.Info.Doing.Meetings[index].Base.TableId == (uint)MeetingInfoBase.MeetingType.ChangeDeclaration)
                            meetingInfo = new Meeting_ChangeDeclaration();

                        if (meetingInfo != null)
                        {
                            List<ulong> agreeList = new List<ulong>();
                            if (ntf.Info.Doing.Meetings[index].AgreeList != null && ntf.Info.Doing.Meetings[index].AgreeList.Count > 0)
                            {
                                for (int index2 = 0, len2 = ntf.Info.Doing.Meetings[index].AgreeList.Count; index2 < len2; index2++)
                                {
                                    agreeList.Add(ntf.Info.Doing.Meetings[index].AgreeList[index2]);
                                }
                            }
                            List<ulong> refuseList = new List<ulong>();
                            if (ntf.Info.Doing.Meetings[index].RefuseList != null && ntf.Info.Doing.Meetings[index].RefuseList.Count > 0)
                            {
                                for (int index2 = 0, len2 = ntf.Info.Doing.Meetings[index].RefuseList.Count; index2 < len2; index2++)
                                {
                                    refuseList.Add(ntf.Info.Doing.Meetings[index].RefuseList[index2]);
                                }
                            }

                            meetingInfo.SetDoing(ntf.Info.Doing.Meetings[index].Base.MeetingId,
                                ntf.Info.Doing.Meetings[index].Base.CreateRoleId,
                                ntf.Info.Doing.Meetings[index].Base.CreateRoleName.ToStringUtf8(),
                                ntf.Info.Doing.Meetings[index].Base.CreateTime,
                                ntf.Info.Doing.Meetings[index].ParamList, agreeList, refuseList);
                            MyWarriorGroup.currentMeetingInfos[meetingInfo.MeetingID] = meetingInfo;
                        }
                    }
                }
            }

            ActionInfo memberJoined = new ActionInfo((uint)EActionType.MemberJoined, Sys_Time.Instance.GetServerTime());
            memberJoined.paramList.Add(Sys_Role.Instance.Role.Name.ToStringUtf8());
            MyWarriorGroup.actionInfos.Add(memberJoined);
            eventEmitter.Trigger(EEvents.AddedNewActions);

            if (ntf.Info.Base.LeaderId == Sys_Role.Instance.RoleId)
            {
                eventEmitter.Trigger(EEvents.CreatedSuccess);
            }
            else
            {
                invitedInfoDict.Clear();
                eventEmitter.Trigger(EEvents.JoinedSuccess);
            }
        }

        /// <summary>
        /// 退出勇者团成功回调///
        /// </summary>
        /// <param name="msg"></param>
        void OnQuitWarriorGroupSuccess(NetMsg msg)
        {
            CmdBraveGroupQuitBraveGroupNtf ntf = NetMsgUtil.Deserialize<CmdBraveGroupQuitBraveGroupNtf>(CmdBraveGroupQuitBraveGroupNtf.Parser, msg);
            if (ntf != null)
            {
                QuitAfterColdTime = ntf.QuitAfterColdTime;

                MyWarriorGroup.Dispose();
                historyMeetingRedPointInfo.meetingDic.Clear();
                SerializeHistoryMeetingRedPointInfo();

                eventEmitter.Trigger(EEvents.QuitSuccessed);
            }
        }

        /// <summary>
        /// 添加团成员通知///
        /// </summary>
        /// <param name="msg"></param>
        void OnAddMemberNtf(NetMsg msg)
        {
            CmdBraveGroupAddMemberNtf ntf = NetMsgUtil.Deserialize<CmdBraveGroupAddMemberNtf>(CmdBraveGroupAddMemberNtf.Parser, msg);
            if (ntf != null)
            {
                WarriorInfo warriorInfo = new WarriorInfo()
                {
                    RoleID = ntf.Member.Base.RoleId,
                     RoleName = ntf.Member.Base.RoleName.ToStringUtf8(),
                      Level = ntf.Member.Base.Level,
                       HeroID = ntf.Member.Base.HeroId,
                        IconID = ntf.Member.Base.HeadPhoto,
                         FrameID = ntf.Member.Base.Frame,
                          WeaponID = ntf.Member.Base.Weapon,
                           Occ = ntf.Member.Base.Career,
                           DressID = ntf.Member.Base.Fashion.FashionInfos[0].FashionId,
                            DressData = Sys_Fashion.Instance.GetDressData(ntf.Member.Base.Fashion.FashionInfos, ntf.Member.Base.HeroId),
                             TitleID = ntf.Member.Base.TitleId,
                              LastOffline = ntf.Member.Base.LastOffline,
                               GroupName = MyWarriorGroup.GroupName,
                };
                warriorInfo.IsLeader = warriorInfo.RoleID == MyWarriorGroup.LeaderRoleID ? true : false;
                MyWarriorGroup.warriorInfos[warriorInfo.RoleID] = warriorInfo;
                eventEmitter.Trigger(EEvents.AddedNewMember);

                ActionInfo actionInfo = new ActionInfo((uint)EActionType.MemberJoined, Sys_Time.Instance.GetServerTime());
                actionInfo.paramList.Add(warriorInfo.RoleName);
                MyWarriorGroup.actionInfos.Add(actionInfo);
                eventEmitter.Trigger(EEvents.AddedNewActions);

                Sys_Chat.Instance.PushMessage(ChatType.System, null, LanguageHelper.GetTextContent(13525, warriorInfo.RoleName));
            }
        }

        /// <summary>
        /// 删除团成员通知///
        /// </summary>
        /// <param name="msg"></param>
        void OnRemoveMemberNtf(NetMsg msg)
        {
            CmdBraveGroupRemoveMemberNtf ntf = NetMsgUtil.Deserialize<CmdBraveGroupRemoveMemberNtf>(CmdBraveGroupRemoveMemberNtf.Parser, msg);
            if (ntf != null)
            {
                if (MyWarriorGroup.warriorInfos.ContainsKey(ntf.RoleId))
                {
                    string tempRoleName = MyWarriorGroup.warriorInfos[ntf.RoleId].RoleName;
                    MyWarriorGroup.warriorInfos.Remove(ntf.RoleId);
                    eventEmitter.Trigger(EEvents.RevomedMember);

                    ActionInfo actionInfo = new ActionInfo((uint)EActionType.MemberLeft, Sys_Time.Instance.GetServerTime());
                    actionInfo.paramList.Add(tempRoleName);
                    MyWarriorGroup.actionInfos.Add(actionInfo);
                    eventEmitter.Trigger(EEvents.AddedNewActions);
                }
                else
                {
                    DebugUtil.LogError($"Sys_WarriorGroup.Instance.warriorInfos doestn't contain a key: {ntf.RoleId} !");
                }
            }
        }

        /// <summary>
        /// 刷新团成员基础信息通知///
        /// </summary>
        /// <param name="msg"></param>
        void OnFreshMemberBaseInfoNtf(NetMsg msg)
        {
            CmdBraveGroupFreshMemberBaseInfoNtf ntf = NetMsgUtil.Deserialize<CmdBraveGroupFreshMemberBaseInfoNtf>(CmdBraveGroupFreshMemberBaseInfoNtf.Parser, msg);
            if (ntf != null)
            {
                WarriorInfo warriorInfo;
                if (MyWarriorGroup.warriorInfos.TryGetValue(ntf.ElementList.RoleId, out warriorInfo))
                {
                    if (ntf.ElementList != null && ntf.ElementList.Elements != null && ntf.ElementList.Elements.Count > 0)
                    {
                        for (int index = 0, len = ntf.ElementList.Elements.Count; index < len; index++)
                        {
                            if (ntf.ElementList.Elements[index].Type == enBraveGroupMemberBase.BraveGroupMemberBaseName)
                                warriorInfo.RoleName = ntf.ElementList.Elements[index].Param.StrParam.ToStringUtf8();
                            else if (ntf.ElementList.Elements[index].Type == enBraveGroupMemberBase.BraveGroupMemberBaseLevel)
                                warriorInfo.Level = (uint)ntf.ElementList.Elements[index].Param.IntParam;
                            else if (ntf.ElementList.Elements[index].Type == enBraveGroupMemberBase.BraveGroupMemberBaseHeroId)
                                warriorInfo.HeroID = (uint)ntf.ElementList.Elements[index].Param.IntParam;
                            else if (ntf.ElementList.Elements[index].Type == enBraveGroupMemberBase.BraveGroupMemberBaseFrame)
                                warriorInfo.FrameID = (uint)ntf.ElementList.Elements[index].Param.IntParam;
                            else if (ntf.ElementList.Elements[index].Type == enBraveGroupMemberBase.BraveGroupMemberBaseWeapon)
                                warriorInfo.WeaponID = (uint)ntf.ElementList.Elements[index].Param.IntParam;
                            else if (ntf.ElementList.Elements[index].Type == enBraveGroupMemberBase.BraveGroupMemberBaseHeadPhoto)
                                warriorInfo.IconID = (uint)ntf.ElementList.Elements[index].Param.IntParam;
                            else if (ntf.ElementList.Elements[index].Type == enBraveGroupMemberBase.BraveGroupMemberBaseCareer)
                                warriorInfo.Occ = (uint)ntf.ElementList.Elements[index].Param.IntParam;
                            else if (ntf.ElementList.Elements[index].Type == enBraveGroupMemberBase.BraveGroupMemberBaseCareerRank)
                                warriorInfo.OccRank = (uint)ntf.ElementList.Elements[index].Param.IntParam;
                            else if (ntf.ElementList.Elements[index].Type == enBraveGroupMemberBase.BraveGroupMemberBaseLastOffline)
                                warriorInfo.LastOffline = (uint)ntf.ElementList.Elements[index].Param.IntParam;
                            else if (ntf.ElementList.Elements[index].Type == enBraveGroupMemberBase.BraveGroupMemberBaseTitle)
                                warriorInfo.TitleID = (uint)ntf.ElementList.Elements[index].Param.IntParam;
                            else if (ntf.ElementList.Elements[index].Type == enBraveGroupMemberBase.BraveGroupMemberBaseFashion)
                            {

                            }
                        }
                    }
                }
                else
                {
                    DebugUtil.LogError($"Sys_WarriorGroup.Instance.warriorInfos doestn't contain a key: {ntf.ElementList.RoleId} !");
                }
            }
        }

        /// <summary>
        /// 刷新团宣言通知///
        /// </summary>
        /// <param name="msg"></param>
        void OnFreshDeclarationNtf(NetMsg msg)
        {
            CmdBraveGroupFreshBriefNtf ntf = NetMsgUtil.Deserialize<CmdBraveGroupFreshBriefNtf>(CmdBraveGroupFreshBriefNtf.Parser, msg);
            if (ntf != null)
            {
                MyWarriorGroup.GroupDeclaration = ntf.BraveGroupBrief.ToStringUtf8();

                eventEmitter.Trigger(EEvents.RefrehedDeclaration);
            }
        }

        /// <summary>
        /// 刷新团名字通知///
        /// </summary>
        /// <param name="msg"></param>
        void OnFreshNameNtf(NetMsg msg)
        {
            CmdBraveGroupFreshNameNtf ntf = NetMsgUtil.Deserialize<CmdBraveGroupFreshNameNtf>(CmdBraveGroupFreshNameNtf.Parser, msg);
            if (ntf != null)
            {
                MyWarriorGroup.GroupName = ntf.BraveGroupName.ToStringUtf8();
                foreach (var warriorInfo in MyWarriorGroup.warriorInfos.Values)
                {
                    warriorInfo.GroupName = ntf.BraveGroupName.ToStringUtf8();
                }

                eventEmitter.Trigger(EEvents.RefrehedName);
                
                if (GameCenter.mainHero!=null)
                {
                    if (GameCenter.mainHero.heroBaseComponent.TitleId == Sys_Title.Instance.bGroupTitle) 
                    {
                        UpdateBGroupTitleEvt evt = new UpdateBGroupTitleEvt();
                        evt.actorId = Sys_Role.Instance.RoleId;
                        evt.titleId = Sys_Title.Instance.bGroupTitle;
                        evt.name = MyWarriorGroup.GroupName;
                        evt.pos = MyWarriorGroup.LeaderRoleID == Sys_Role.Instance.RoleId ? 1u : 0u;
                        Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnUpdateBGroupTitleName, evt);
                    }
                }
            }
        }

        /// <summary>
        /// 刷新团长通知///
        /// </summary>
        /// <param name="msg"></param>
        void OnFreshLeaderNtf(NetMsg msg)
        {
            CmdBraveGroupFreshLeaderNtf ntf = NetMsgUtil.Deserialize<CmdBraveGroupFreshLeaderNtf>(CmdBraveGroupFreshLeaderNtf.Parser, msg);
            if (ntf != null)
            {
                ulong tempLeaderID = MyWarriorGroup.LeaderRoleID;
                MyWarriorGroup.LeaderRoleID = ntf.LeaderId;

                foreach (var warriorInfo in MyWarriorGroup.warriorInfos.Values)
                {
                    if (warriorInfo.RoleID == tempLeaderID)
                        warriorInfo.IsLeader = false;
                    if (warriorInfo.RoleID == ntf.LeaderId)
                        warriorInfo.IsLeader = true;
                }

                eventEmitter.Trigger(EEvents.RefrehedLeader);

                ActionInfo actionInfo = new ActionInfo((uint)EActionType.NewLeader, Sys_Time.Instance.GetServerTime());
                actionInfo.paramList.Add(Instance.MyWarriorGroup.warriorInfos[ntf.LeaderId].RoleName);
                actionInfo.paramList.Add(Instance.MyWarriorGroup.warriorInfos[tempLeaderID].RoleName);
                MyWarriorGroup.actionInfos.Add(actionInfo);
                eventEmitter.Trigger(EEvents.AddedNewActions);
                
                if (GameCenter.mainHero!=null)
                {
                    if (GameCenter.mainHero.heroBaseComponent.TitleId == Sys_Title.Instance.bGroupTitle) 
                    {
                        UpdateBGroupTitleEvt evt = new UpdateBGroupTitleEvt();
                        evt.actorId = Sys_Role.Instance.RoleId;
                        evt.titleId = Sys_Title.Instance.bGroupTitle;
                        evt.name = MyWarriorGroup.GroupName;
                        evt.pos = MyWarriorGroup.LeaderRoleID == Sys_Role.Instance.RoleId ? 1u : 0u;
                        Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnUpdateBGroupTitleName, evt);
                    }
                }
            }
        }

        /// <summary>
        /// 添加动态通知///
        /// </summary>
        /// <param name="msg"></param>
        void OnAddActionNtf(NetMsg msg)
        {
            CmdBraveGroupAddDynamicNtf ntf = NetMsgUtil.Deserialize<CmdBraveGroupAddDynamicNtf>(CmdBraveGroupAddDynamicNtf.Parser, msg);
            if (ntf != null)
            {
                if (ntf.DynamicList != null && ntf.DynamicList.Dynamics != null && ntf.DynamicList.Dynamics.Count > 0)
                {
                    for (int index = 0, len = ntf.DynamicList.Dynamics.Count; index < len; index++)
                    {
                        ActionInfo actionInfo = new ActionInfo(ntf.DynamicList.Dynamics[index].TableId,
                            ntf.DynamicList.Dynamics[index].CreateTime);
                        actionInfo.FillParamList(ntf.DynamicList.Dynamics[index].ParamList);
                        MyWarriorGroup.actionInfos.Add(actionInfo);
                    }
                }

                eventEmitter.Trigger(EEvents.AddedNewActions);
            }
        }

        /// <summary>
        /// 添加一个被邀请///
        /// </summary>
        /// <param name="msg"></param>
        void OnAddInvitedNtf(NetMsg msg)
        {
            CmdBraveGroupAddInvitedNtf ntf = NetMsgUtil.Deserialize<CmdBraveGroupAddInvitedNtf>(CmdBraveGroupAddInvitedNtf.Parser, msg);
            if (ntf != null)
            {
                InvitedInfo invitedInfo = new InvitedInfo()
                {
                       RoleID = ntf.Invited.RoleId,
                         Time = ntf.Invited.InviteTime,
                };

                invitedInfo.WarriorGroup = new WarriorGroup();
                invitedInfo.WarriorGroup.GroupUID = ntf.Invited.Base.BraveGroupId;
                invitedInfo.WarriorGroup.GroupName = ntf.Invited.Base.BraveGroupName.ToStringUtf8();
                invitedInfo.WarriorGroup.warriorInfos.Clear();

                if (ntf.Invited != null && ntf.Invited.Members != null && ntf.Invited.Members.Count > 0)
                {
                    for (int index = 0, len = ntf.Invited.Members.Count; index < len; index++)
                    {
                        WarriorInfo warriorInfo = new WarriorInfo()
                        {
                            RoleID = ntf.Invited.Members[index].RoleId,
                            RoleName = ntf.Invited.Members[index].RoleName.ToStringUtf8(),
                            Level = ntf.Invited.Members[index].Level,
                            HeroID = ntf.Invited.Members[index].HeroId,
                            IconID = ntf.Invited.Members[index].HeadPhoto,
                            FrameID = ntf.Invited.Members[index].Frame,
                            Occ = ntf.Invited.Members[index].Career,
                        };
                        invitedInfo.WarriorGroup.warriorInfos[warriorInfo.RoleID] = warriorInfo;
                    }
                }

                invitedInfoDict[ntf.Invited.RoleId] = invitedInfo;
                eventEmitter.Trigger(EEvents.AddNewInvite);

                Sys_MessageBag.MessageContent messageContent = new Sys_MessageBag.MessageContent();
                messageContent.mType = EMessageBagType.BraveTeam;
                messageContent.invitorId = invitedInfo.RoleID;
                messageContent.invitiorName = invitedInfo.RoleName;
                messageContent.cMess = new Sys_MessageBag.GuildMessage()
                {
                    guildId = invitedInfo.WarriorGroup.GroupUID,
                    guildName = invitedInfo.WarriorGroup.GroupName,
                };
                Sys_MessageBag.Instance.SendMessageInfo(EMessageBagType.BraveTeam, messageContent);
            }
        }

        /// <summary>
        /// 删除一个被邀请///
        /// </summary>
        /// <param name="msg"></param>
        void OnRemoveInvitedNtf(NetMsg msg)
        {
            CmdBraveGroupRemoveInvitedNtf ntf = NetMsgUtil.Deserialize<CmdBraveGroupRemoveInvitedNtf>(CmdBraveGroupRemoveInvitedNtf.Parser, msg);
            if (ntf != null)
            {
                RemoveInvite(ntf.RoleId);
            }
        }

        void OnShowTitle(uint titleID)
        {
            foreach(var info in MyWarriorGroup.warriorInfos.Values)
            {
                if (info.RoleID == Sys_Role.Instance.RoleId)
                {
                    MyWarriorGroup.warriorInfos[info.RoleID].TitleID = titleID;
                }
            }
        }

        void OnTimeNtf(uint oldTime, uint newTime)
        {
            uint duringTime = newTime - oldTime;
            if (MyWarriorGroup.QuitBeforeThinkingTime > duringTime)
            {
                MyWarriorGroup.QuitBeforeThinkingTime -= duringTime;
                MyWarriorGroup.quitColdTimer?.Cancel();
                MyWarriorGroup.quitColdTimer = Timer.Register(1f, () =>
                {
                    if (MyWarriorGroup.QuitBeforeThinkingTime >= 1)
                        MyWarriorGroup.QuitBeforeThinkingTime--;
                }, null, true, true);
            }
            else
            {
                MyWarriorGroup.QuitBeforeThinkingTime = 0;
            }          
            eventEmitter.Trigger(EEvents.RefreshQuitTime);

            if (MyWarriorGroup.NextCreateMeetingTime > duringTime)
            {
                MyWarriorGroup.NextCreateMeetingTime -= duringTime;
                MyWarriorGroup.quitColdTimer?.Cancel();
                MyWarriorGroup.quitColdTimer = Timer.Register(1f, () =>
                {
                    if (MyWarriorGroup.NextCreateMeetingTime >= 1)
                        MyWarriorGroup.NextCreateMeetingTime--;
                }, null, true, true);
            }
            else
            {
                MyWarriorGroup.NextCreateMeetingTime = 0;
            }
            eventEmitter.Trigger(EEvents.RefreshCreateMeetTime);

            if (MyWarriorGroup.FastInviteTime > duringTime)
            {
                MyWarriorGroup.FastInviteTime -= duringTime;
                MyWarriorGroup.quitColdTimer?.Cancel();
                MyWarriorGroup.quitColdTimer = Timer.Register(1f, () =>
                {
                    if (MyWarriorGroup.FastInviteTime >= 1)
                        MyWarriorGroup.FastInviteTime--;
                }, null, true, true);
            }
            else
            {
                MyWarriorGroup.FastInviteTime = 0;
            }
            eventEmitter.Trigger(EEvents.RefreshFastInviteTime);
        }

        /// <summary>
        /// 刷新离开时间///
        /// </summary>
        /// <param name="msg"></param>
        void OnFreshQuitTimeNtf(NetMsg msg)
        {
            CmdBraveGroupFreshQuitTimeNtf ntf = NetMsgUtil.Deserialize<CmdBraveGroupFreshQuitTimeNtf>(CmdBraveGroupFreshQuitTimeNtf.Parser, msg);
            if (ntf != null)
            {
                if (ntf.RoleId != Sys_Role.Instance.RoleId)
                    return;

                if (ntf.Elements != null && ntf.Elements.Count > 0)
                {
                    for (int index = 0, len = ntf.Elements.Count; index < len; index++)
                    {
                        if (ntf.Elements[index].Type == enBraveGroupMemberTime.BraveGroupMemberTimeQuitBeforeThikingTime)
                        {
                            uint currentTime = Sys_Time.Instance.GetServerTime();
                            if (ntf.Elements[index].Value >= currentTime)
                                MyWarriorGroup.QuitBeforeThinkingTime = ntf.Elements[index].Value - currentTime;
                            else
                                MyWarriorGroup.QuitBeforeThinkingTime = 0;

                            if (MyWarriorGroup.QuitBeforeThinkingTime > 0)
                            {
                                MyWarriorGroup.quitColdTimer?.Cancel();
                                MyWarriorGroup.quitColdTimer = Timer.Register(1f, () =>
                                {
                                    if (MyWarriorGroup.QuitBeforeThinkingTime >= 1)
                                        MyWarriorGroup.QuitBeforeThinkingTime--;
                                }, null, true, true);
                            }
                            else
                            {
                                MyWarriorGroup.QuitBeforeThinkingTime = 0;
                            }

                            eventEmitter.Trigger(EEvents.RefreshQuitTime);
                        }
                        else if (ntf.Elements[index].Type == enBraveGroupMemberTime.BraveGroupMemberTimeNextCreateMeetingTime)
                        {
                            uint currentTime = Sys_Time.Instance.GetServerTime();
                            if (ntf.Elements[index].Value >= currentTime)
                                MyWarriorGroup.NextCreateMeetingTime = ntf.Elements[index].Value - currentTime;
                            else
                                MyWarriorGroup.NextCreateMeetingTime = 0;

                            if (MyWarriorGroup.NextCreateMeetingTime > 0)
                            {
                                MyWarriorGroup.createMeetTimer?.Cancel();
                                MyWarriorGroup.createMeetTimer = Timer.Register(1f, () =>
                                {
                                    if (MyWarriorGroup.NextCreateMeetingTime >= 1)
                                        MyWarriorGroup.NextCreateMeetingTime--;
                                }, null, true, true);
                            }
                            else
                            {
                                MyWarriorGroup.NextCreateMeetingTime = 0;
                            }
                            eventEmitter.Trigger(EEvents.RefreshCreateMeetTime);
                        }
                        else if (ntf.Elements[index].Type == enBraveGroupMemberTime.BraveGroupMemberTimeFastInviteTeamTime)
                        {
                            uint currentTime = Sys_Time.Instance.GetServerTime();
                            if (ntf.Elements[index].Value >= currentTime)
                                MyWarriorGroup.FastInviteTime = ntf.Elements[index].Value - currentTime;
                            else
                                MyWarriorGroup.FastInviteTime = 0;

                            if (MyWarriorGroup.FastInviteTime > 0)
                            {
                                MyWarriorGroup.fastInviteTimer?.Cancel();
                                MyWarriorGroup.fastInviteTimer = Timer.Register(1f, () =>
                                {
                                    if (MyWarriorGroup.FastInviteTime >= 1)
                                        MyWarriorGroup.FastInviteTime--;
                                }, null, true, true);
                            }
                            else
                            {
                                MyWarriorGroup.FastInviteTime = 0;
                            }
                            eventEmitter.Trigger(EEvents.RefreshFastInviteTime);
                        }           
                    }
                }
            }
        }

        /// <summary>
        /// 添加一个进行中的会议通知///
        /// </summary>
        /// <param name="msg"></param>
        void OnAddDoingMeeting(NetMsg msg)
        {
            CmdBraveGroupAddDoingMeetingNtf ntf = NetMsgUtil.Deserialize<CmdBraveGroupAddDoingMeetingNtf>(CmdBraveGroupAddDoingMeetingNtf.Parser, msg);
            if (ntf != null)
            {
                List<ulong> agreeList = new List<ulong>();
                if (ntf.Meeting != null && ntf.Meeting.AgreeList != null && ntf.Meeting.AgreeList.Count > 0)
                {
                    for (int index = 0, len = ntf.Meeting.AgreeList.Count; index < len; index++)
                    {
                        agreeList.Add(ntf.Meeting.AgreeList[index]);
                    }
                }
                List<ulong> refuseList = new List<ulong>();
                if (ntf.Meeting != null && ntf.Meeting.RefuseList != null && ntf.Meeting.RefuseList.Count > 0)
                {
                    for (int index = 0, len = ntf.Meeting.RefuseList.Count; index < len; index++)
                    {
                        refuseList.Add(ntf.Meeting.RefuseList[index]);
                    }
                }
                MyWarriorGroup.currentMeetingInfos[ntf.Meeting.Base.TableId].SetDoing(ntf.Meeting.Base.MeetingId, 
                    ntf.Meeting.Base.CreateRoleId,
                    ntf.Meeting.Base.CreateRoleName.ToStringUtf8(),
                    ntf.Meeting.Base.CreateTime,
                    ntf.Meeting.ParamList, agreeList, refuseList);

                eventEmitter.Trigger<MeetingInfoBase>(EEvents.AddNewDoingMeeting, MyWarriorGroup.currentMeetingInfos[ntf.Meeting.Base.TableId]);

                ActionInfo actionInfo = null;
                if (ntf.Meeting.Base.TableId == (uint)MeetingInfoBase.MeetingType.SuggestSelf)
                {
                    actionInfo = new ActionInfo((uint)EActionType.SuggestSelfMeetingStarted, Sys_Time.Instance.GetServerTime());
                    actionInfo.paramList.Add(Instance.MyWarriorGroup.warriorInfos[ntf.Meeting.Base.CreateRoleId].RoleName);
                }
                else if (ntf.Meeting.Base.TableId == (uint)MeetingInfoBase.MeetingType.Recruit)
                {
                    actionInfo = new ActionInfo((uint)EActionType.RecruitMeetingStarted, Sys_Time.Instance.GetServerTime());
                    actionInfo.paramList.Add(Instance.MyWarriorGroup.warriorInfos[ntf.Meeting.Base.CreateRoleId].RoleName);
                }
                else if (ntf.Meeting.Base.TableId == (uint)MeetingInfoBase.MeetingType.Fire)
                {
                    actionInfo = new ActionInfo((uint)EActionType.FireMeetingStarted, Sys_Time.Instance.GetServerTime());
                    actionInfo.paramList.Add(Instance.MyWarriorGroup.warriorInfos[ntf.Meeting.Base.CreateRoleId].RoleName);
                }
                else if (ntf.Meeting.Base.TableId == (uint)MeetingInfoBase.MeetingType.ChangeName)
                {
                    actionInfo = new ActionInfo((uint)EActionType.ChangeNameMeetingStarted, Sys_Time.Instance.GetServerTime());
                    actionInfo.paramList.Add(Instance.MyWarriorGroup.warriorInfos[ntf.Meeting.Base.CreateRoleId].RoleName);
                }
                else if (ntf.Meeting.Base.TableId == (uint)MeetingInfoBase.MeetingType.ChangeDeclaration)
                {
                    actionInfo = new ActionInfo((uint)EActionType.ChangeDeclarationMeetingStarted, Sys_Time.Instance.GetServerTime());
                    actionInfo.paramList.Add(Instance.MyWarriorGroup.warriorInfos[ntf.Meeting.Base.CreateRoleId].RoleName);
                }         
                MyWarriorGroup.actionInfos.Add(actionInfo);

                eventEmitter.Trigger(EEvents.AddedNewActions);
            }
        }

        /// <summary>
        /// 删除一个进行中的会议///
        /// </summary>
        /// <param name="msg"></param>
        void OnRemoveDoingMeetingNtf(NetMsg msg)
        {
            CmdBraveGroupRemoveDoingMeetingNtf ntf = NetMsgUtil.Deserialize<CmdBraveGroupRemoveDoingMeetingNtf>(CmdBraveGroupRemoveDoingMeetingNtf.Parser, msg);
            if (ntf != null)
            {
                foreach (var meeting in MyWarriorGroup.currentMeetingInfos.Values)
                {
                    if (meeting.MeetingID == ntf.MeetingId)
                    {
                        meeting.SetNormal();
                    }
                }

                eventEmitter.Trigger(EEvents.DelDoingMeeting);
            }
        }

        /// <summary>
        /// 添加一个完成的会议///
        /// </summary>
        /// <param name="msg"></param>
        void OnAddFinishMeetingNtf(NetMsg msg)
        {
            CmdBraveGroupAddFinishMeetingNtf ntf = NetMsgUtil.Deserialize<CmdBraveGroupAddFinishMeetingNtf>(CmdBraveGroupAddFinishMeetingNtf.Parser, msg);
            if (ntf != null)
            {
                MyWarriorGroup.DelOutTimeHistoryMeeting();

                MeetingInfoBase meetingInfo = null;
                if (ntf.Meeting.Base.TableId == (uint)MeetingInfoBase.MeetingType.SuggestSelf)
                    meetingInfo = new Meeting_SuggestSelf();
                else if (ntf.Meeting.Base.TableId == (uint)MeetingInfoBase.MeetingType.Recruit)
                    meetingInfo = new Meeting_Recruit();
                else if (ntf.Meeting.Base.TableId == (uint)MeetingInfoBase.MeetingType.Fire)
                    meetingInfo = new Meeting_Fire();
                else if (ntf.Meeting.Base.TableId == (uint)MeetingInfoBase.MeetingType.ChangeName)
                    meetingInfo = new Meeting_ChangeName();
                else if (ntf.Meeting.Base.TableId == (uint)MeetingInfoBase.MeetingType.ChangeDeclaration)
                    meetingInfo = new Meeting_ChangeDeclaration();

                Dictionary<ulong, string> voteName = new Dictionary<ulong, string>();
                List<ulong> agreeList = new List<ulong>();
                if (ntf.Meeting.AgreeList != null && ntf.Meeting.AgreeList.Members != null && ntf.Meeting.AgreeList.Members.Count > 0)
                {
                    for (int index = 0, len = ntf.Meeting.AgreeList.Members.Count; index < len; index++)
                    {
                        agreeList.Add(ntf.Meeting.AgreeList.Members[index].RoleId);
                        voteName[ntf.Meeting.AgreeList.Members[index].RoleId] = ntf.Meeting.AgreeList.Members[index].RoleName.ToStringUtf8();
                    }
                }
                List<ulong> refuseList = new List<ulong>();
                if (ntf.Meeting.RefuseList != null && ntf.Meeting.RefuseList.Members != null && ntf.Meeting.RefuseList.Members.Count > 0)
                {
                    for (int index = 0, len = ntf.Meeting.RefuseList.Members.Count; index < len; index++)
                    {
                        refuseList.Add(ntf.Meeting.RefuseList.Members[index].RoleId);
                        voteName[ntf.Meeting.RefuseList.Members[index].RoleId] = ntf.Meeting.RefuseList.Members[index].RoleName.ToStringUtf8();
                    }
                }

                meetingInfo.SetHistory(ntf.Meeting.Base.MeetingId, ntf.Meeting.Base.CreateRoleId, ntf.Meeting.Base.CreateRoleName.ToStringUtf8(),
                    ntf.Meeting.Result, ntf.Meeting.FinishTime, ntf.Meeting.ParamList,
                    agreeList, refuseList, voteName);
                MyWarriorGroup.historyMeetingInfos[ntf.Meeting.Base.MeetingId] = meetingInfo;

                historyMeetingRedPointInfo.meetingDic[ntf.Meeting.Base.MeetingId] = new HistoryMeetingRead()
                {
                    meetingID = ntf.Meeting.Base.MeetingId,
                    read = false,
                };
                SerializeHistoryMeetingRedPointInfo();

                eventEmitter.Trigger(EEvents.AddNewHistoryMeeting);
            }
        }

        /// <summary>
        /// 删除一个完成的会议///
        /// </summary>
        /// <param name="msg"></param>
        void OnRemoveFinishMeetingNtf(NetMsg msg)
        {
            CmdBraveGroupRemoveFinishMeetingNtf ntf = NetMsgUtil.Deserialize<CmdBraveGroupRemoveFinishMeetingNtf>(CmdBraveGroupRemoveFinishMeetingNtf.Parser, msg);
            if (ntf != null)
            {
                if (MyWarriorGroup.historyMeetingInfos.ContainsKey(ntf.MeetingId))
                {
                    MyWarriorGroup.historyMeetingInfos.Remove(ntf.MeetingId);

                    if (historyMeetingRedPointInfo.meetingDic.ContainsKey(ntf.MeetingId))
                    {
                        historyMeetingRedPointInfo.meetingDic.Remove(ntf.MeetingId);
                    }
                    SerializeHistoryMeetingRedPointInfo();

                    eventEmitter.Trigger(EEvents.DelHistoryMeeting);
                }
            }
        }

        /// <summary>
        /// 刷新成员的投票结果通知///
        /// </summary>
        /// <param name="msg"></param>
        void OnFreshMemberVoteResult(NetMsg msg)
        {
            CmdBraveGroupFreshMemberVoteResultNtf ntf = NetMsgUtil.Deserialize<CmdBraveGroupFreshMemberVoteResultNtf>(CmdBraveGroupFreshMemberVoteResultNtf.Parser, msg);
            if (ntf != null)
            {
                foreach (var meeting in MyWarriorGroup.currentMeetingInfos.Values)
                {
                    if (meeting.MeetingID == ntf.MeetingId)
                    {
                        meeting.RefreshVote(ntf.RoleId, ntf.Result);

                        eventEmitter.Trigger<uint>(EEvents.RefreshVote, meeting.MeetingID);
                        return;
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// 获取所有的动态集合///
        /// </summary>
        /// <returns></returns>
        public List<ActionInfo> GetAllActionInfos()
        {
            return MyWarriorGroup.actionInfos;
        }

        /// <summary>
        /// 获取当前的会议集合///
        /// </summary>
        /// <returns></returns>
        public Dictionary<uint, MeetingInfoBase> GetCurrentMeetingInfos()
        {
            return MyWarriorGroup.currentMeetingInfos;
        }

        /// <summary>
        /// 获取历史会议集合///
        /// </summary>
        /// <returns></returns>
        public Dictionary<uint, MeetingInfoBase> GetHistoryMeetingInfos()
        {
            return MyWarriorGroup.historyMeetingInfos;
        }

        /// <summary>
        /// 删除邀请///
        /// </summary>
        /// <param name="roleID">邀请者ID</param>
        public void RemoveInvite(ulong roleID)
        {
            if (invitedInfoDict.ContainsKey(roleID))
            {
                invitedInfoDict.Remove(roleID);
                eventEmitter.Trigger(EEvents.RemoveInvite);
            }
        }

        /// <summary>
        /// 是否在勇者团中///
        /// </summary>
        /// <returns></returns>
        public bool IsJoinedWarriorGroup()
        {
            return Instance.MyWarriorGroup.GroupUID != 0;
        }

        /// <summary>
        /// 获取勇者团名///
        /// </summary>
        /// <returns></returns>
        public string GetWarriorGroup()
        {
            return Instance.MyWarriorGroup.GroupName;
        }

        /// <summary>
        /// 是否存在未投票的进行时会议///
        /// </summary>
        /// <returns></returns>
        public bool HavingUnVoteDoingMeeting()
        {
            foreach(var meeting in MyWarriorGroup.currentMeetingInfos.Values)
            {
                if (meeting.Status == MeetingInfoBase.MeetingStatusType.Doing && !meeting.Voted)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 是否存在未浏览过的历史会议///
        /// </summary>
        /// <returns></returns>
        public bool HavingUnReadHistoryMeeting()
        {
            if (historyMeetingRedPointInfo.meetingDic != null && historyMeetingRedPointInfo.meetingDic.Count > 0)
            {
                foreach (var meetingInfo in historyMeetingRedPointInfo.meetingDic.Values)
                {
                    if (MyWarriorGroup.historyMeetingInfos != null && MyWarriorGroup.historyMeetingInfos.ContainsKey(meetingInfo.meetingID) && meetingInfo.read == false)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 会议Toggle红点显示///
        /// </summary>
        /// <returns></returns>
        public bool HavingUnVoteDoingMeetingOrHavingUnReadHistoryMeeting()
        {
            if (HavingUnVoteDoingMeeting())
                return true;

            if (HavingUnReadHistoryMeeting())
                return true;

            return false;
        }

        /// <summary>
        /// 主界面按钮红点显示///
        /// </summary>
        /// <returns></returns>
        public bool HavingWarriorGroupRedPoint()
        {
            if (HavingUnVoteDoingMeetingOrHavingUnReadHistoryMeeting())
                return true;

            return false;
        }

        /// <summary>
        /// 获取勇者团职位///
        /// 1:团长///
        /// 2:团员///
        /// </summary>
        /// <returns></returns>
        public uint GetPosInWarriorGroup()
        {
            if (Instance.MyWarriorGroup.LeaderRoleID == Sys_Role.Instance.RoleId)
                return 1;
            else
                return 0;
        }

        public string GetTitle()
        {
            if (MyWarriorGroup.GroupUID == 0)
                return string.Empty;

            string titleStr = MyWarriorGroup.GroupName;
            titleStr += "-";

            if (Sys_Role.Instance.RoleId == MyWarriorGroup.LeaderRoleID)
            {
                titleStr += LanguageHelper.GetTextContent(1002820);
            }
            else
            {
                titleStr += LanguageHelper.GetTextContent(1002819);
            }

            return titleStr;
        }
    }
}
