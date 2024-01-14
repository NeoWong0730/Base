using Packet;
using Logic.Core;
using Lib.Core;
using Net;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Collections;
using Google.Protobuf;
using Table;
namespace Logic
{
    partial class Sys_Team : SystemModuleBase<Sys_Team>
    {
        public class CDController
        {
            public enum EType
            {
                ApplyQueryMatchList = 1,
                TalkTeam=2,
                TalkFamliy=3,
            }
            class CDData
            {          
                private uint m_CDTime;

                private bool m_IsReady = true;

                //public Timer timer = null;

                private uint m_StartTimePoint = 0;
                public void SetCDTime(float time)
                {
                    m_CDTime = (uint)time;
          
                }
                public uint GetCDTime()
                {
                    return m_CDTime;
                }

                public bool IsReady()
                {
                   var offset =  Sys_Time.Instance.GetServerTime() - m_StartTimePoint;
                   return offset >= m_CDTime;
                }
                public void Start()
                {
                    m_IsReady = false;

                    m_StartTimePoint = Sys_Time.Instance.GetServerTime();
                }

                private void OnFinish()
                {
   
                }

                public void OnDestory()
                {

                }

                public uint RemainingTime()
                {
                    var offset = Sys_Time.Instance.GetServerTime() - m_StartTimePoint;

                    var time =  m_CDTime - offset;

                    return time < 0 ? 0 :time;
                }
            }

            private Dictionary<uint, CDData> m_CDPool = new Dictionary<uint, CDData>();

            public void Add(EType type, float CDtime)
            {
                if (m_CDPool.ContainsKey((uint)type) == false)
                {
                    m_CDPool.Add((uint)type, new CDData());
                }

                var value = m_CDPool[(uint)type];

                value.SetCDTime(CDtime);
            }

            public bool get(EType type)
            {
                uint ntype = (uint)type;

                CDData data;

                if (m_CDPool.TryGetValue(ntype, out data) == false)
                    return false;

                return data.IsReady();
            }

            public uint GetRemainingTime(EType type)
            {
                uint ntype = (uint)type;

                CDData data;

                if (m_CDPool.TryGetValue(ntype, out data) == false)
                    return 0;

                return data.RemainingTime();
            }

            public void Start(EType type)
            {
                uint ntype = (uint)type;

                CDData data;

                if (m_CDPool.TryGetValue(ntype, out data) == false)
                    return;

                data.Start();
            }


            public void Clear()
            {
                foreach(var kvp in m_CDPool)
                {
                    kvp.Value.OnDestory();
                }

                m_CDPool.Clear();
            }

        }

        private CDController mCDCon = new CDController();
        public void AddCDTime()
        {

             mCDCon.Add(CDController.EType.ApplyQueryMatchList, getTimeFrom(527,0, 10000f));

            mCDCon.Add(CDController.EType.TalkTeam, getTimeFrom(1063, 0, 1f));
            mCDCon.Add(CDController.EType.TalkFamliy, getTimeFrom(1330, 0, 1f));

        }

        public bool isCDReady(CDController.EType type)
        {
            return mCDCon.get(type);
        }

        public uint RemainingCDTime(CDController.EType type)
        {
            return mCDCon.GetRemainingTime(type);
        }
        public void StartCD(CDController.EType type)
        {
             mCDCon.Start(type);
        }
        private float getTimeFrom(uint id, uint index, float param)
        {
            var data = CSVParam.Instance.GetConfData(id);

            if (data == null)
                return 0;

            string[] values = data.str_value.Split('|');

            if (index >= values.Length)
                return 0;

            float value = 0;

            float.TryParse(values[index], out value);

            value = value / param;

            return value;

        }

        public enum DoTeamTargetType
        {
            Task,//任务 18
            GotoBoss,//经典头目 13
            HuangUp,//挂机 6
            SelectBoss,//挑战阵营 11
            FinishDF,//完成地域防范 10
            OverHuangUp,//终止挂机 7

        }
        public void DoTeamTarget(DoTeamTargetType dtype, uint id)
        {

            if (HaveTeam == false || isCaptain() == false)
                return;            

            uint targetid = 0;
            bool isCheckin = false;

            var teamTargetDatas = CSVTeamTarget.Instance.GetAll();

            for (int i = 0, len = teamTargetDatas.Count; i < len; i++)
            {
                var data = teamTargetDatas[i];

                if (dtype == DoTeamTargetType.Task && data.client_in == 18 && id == data.param1[0])
                {
                    targetid = data.id;
                    isCheckin = true;
                    break;
                }
                else if (dtype == DoTeamTargetType.GotoBoss && data.client_in == 13 && id == data.param1[0])
                {
                    targetid = data.id;
                    isCheckin = true;
                    break;
                }
                else if (dtype == DoTeamTargetType.HuangUp && data.client_in == 6 && id == data.param1[0])
                {
                    targetid = data.id;
                    isCheckin = true;
                    break;
                }
                else if (dtype == DoTeamTargetType.SelectBoss && data.client_in == 11 && id == data.param1[0])
                {
                    targetid = data.id;
                    isCheckin = true;
                    break;
                }
                else if (dtype == DoTeamTargetType.FinishDF && data.client_out == 10)
                {
                    targetid = data.id;
                    isCheckin = false;
                    break;
                }
                else if (dtype == DoTeamTargetType.OverHuangUp && data.client_out == 7 && id == data.param2[0])
                {
                    targetid = data.id;
                    isCheckin = false;
                    break;
                }
            }

            if (targetid > 0)
            {
                SendTargetChange(targetid, isCheckin);
            }
        }

        public void SendTargetChange(uint id, bool isCheckIn)
        {
            CmdTeamTargetChangeReq info = new CmdTeamTargetChangeReq() { InfoId = id, In = isCheckIn };

            NetClient.Instance.SendMessage((ushort)CmdTeam.TargetChangeReq, info);
        }

    }
    partial class Sys_Team : SystemModuleBase<Sys_Team>
    {

        #region toserver msg

        /// <summary>
        /// 申请加入
        /// </summary>
        /// <param name="msg"></param>
        protected void Send_Message_EnterReq(ulong teamID,uint applyType)
        {
            CmdTeamApplyReq info = new CmdTeamApplyReq() { TeamId = teamID ,ApplyType = applyType};
            NetClient.Instance.SendMessage((ushort)CmdTeam.ApplyReq, info);
        }

        /// <summary>
        /// 发送邀请
        /// </summary>
        /// <param name="msg"></param>
        protected void Send_Message_InviteReq(ulong roleID)
        {

            CmdTeamInviteReq info = new CmdTeamInviteReq() { RoleId = roleID };
            NetClient.Instance.SendMessage((ushort)CmdTeam.InviteReq, info);
        }

        /// <summary>
        /// 被邀请人回答
        /// </summary>
        /// <param name="msg"></param>
        public void Send_Message_InviteOpReq(uint reslut, CmdTeamInviteNtf ntf)
        {
            CmdTeamInviteOpReq info = new CmdTeamInviteOpReq()
            {
                Op = reslut,
                TeamId = ntf.TeamId,
                InvitorId = ntf.InvitorId
            };

            NetClient.Instance.SendMessage((ushort)CmdTeam.InviteOpReq, info);

        }

        public void Send_MessageBag_InviteOpReq(uint reslut,ulong teamId,ulong invitorId)
        {
            CmdTeamInviteOpReq info = new CmdTeamInviteOpReq()
            {
                Op = reslut,
                TeamId = teamId,
                InvitorId = invitorId
            };

            NetClient.Instance.SendMessage((ushort)CmdTeam.InviteOpReq, info);

        }

        /// <summary>
        /// 队长收到队员邀请他人入队回答
        /// </summary>
        /// <param name="msg"></param>
        protected void Send_Message_MemInviteOpReq(uint result, CmdTeamMemInviteNtf ntf)
        {
            CmdTeamMemInviteOpReq info = new CmdTeamMemInviteOpReq()
            {
                Op = result,
                TargetId = ntf.TargetId,
                InvitorId = ntf.MemId
            };

            NetClient.Instance.SendMessage((ushort)CmdTeam.MemInviteOpReq, info);
        }

        /// <summary>
        /// 申请入队回答
        /// </summary>
        /// <param name="result"></param>
        protected void Send_Message_ApplyListOpReq(uint result, CmdTeamApplyNtf ntf)
        {
            CmdTeamApplyListOpReq info = new CmdTeamApplyListOpReq() { Op = result, RoleId = ntf.Applyrole.Info.MemId };
            NetClient.Instance.SendMessage((ushort)CmdTeam.ApplyListOpReq, info);
        }

        protected void Send_Message_ApplyListOpReq(uint result, ulong roleID)
        {
            CmdTeamApplyListOpReq info = new CmdTeamApplyListOpReq() { Op = result, RoleId = roleID };
            NetClient.Instance.SendMessage((ushort)CmdTeam.ApplyListOpReq, info);
        }
        // 创建队伍
        protected void Send_Message_CreateTeam(ulong roleid,uint targetId)
        {
            CmdTeamCreateTeamReq info = new CmdTeamCreateTeamReq() { TargetId = targetId };
            NetClient.Instance.SendMessage((ushort)CmdTeam.CreateTeamReq, info);
        }

        //退出队伍
        protected void Send_Message_ExitTeam(uint teamID, ulong roleid)
        {
            CmdTeamMemLeaveReq info = new CmdTeamMemLeaveReq();
            NetClient.Instance.SendMessage((ushort)CmdTeam.MemLeaveReq, info);
        }

        //暂离队伍
        protected void Send_Message_LeaveTeam(uint teamID, ulong roleid)
        {
            CmdTeamMemTmpLeaveReq info = new CmdTeamMemTmpLeaveReq();
            NetClient.Instance.SendMessage((ushort)CmdTeam.MemTmpLeaveReq, info);
        }

        // 请离，踢人
        protected void Send_Message_KickMemReq(ulong roleid)
        {
            CmdTeamKickMemReq info = new CmdTeamKickMemReq() { RoleId = roleid };
            NetClient.Instance.SendMessage((ushort)CmdTeam.KickMemReq, info);
        }

        protected void Send_Message_ApplyCaptain(ulong roleid)
        {
            CmdTeamTransLeaderReq info = new CmdTeamTransLeaderReq() { RoleId = roleid };

            NetClient.Instance.SendMessage((ushort)CmdTeam.TransLeaderReq, info);
        }

        //请求回归
        protected void Send_Message_MemApplyBack(ulong roleid)
        {
            CmdTeamMemBackReq info = new CmdTeamMemBackReq() { RoleId = 0 };

            NetClient.Instance.SendMessage((ushort)CmdTeam.MemBackReq, info);
        }

        //召回
        protected void Send_Message_ApplyCallBack(ulong roleid)
        {
            CmdTeamCallBackReq info = new CmdTeamCallBackReq() { MemId = roleid };

            NetClient.Instance.SendMessage((ushort)CmdTeam.CallBackReq, info);
        }

        /// <summary>
        /// 回答召回通知
        /// </summary>
        /// <param name="msg"></param>
        protected void Send_Message_CallBackOpReq(uint result, CmdTeamCallBackNtf ntf)
        {
            CmdTeamCallBackOpReq info = new CmdTeamCallBackOpReq()
            {
                Op = result,
            };

            NetClient.Instance.SendMessage((ushort)CmdTeam.CallBackOpReq, info);

        }
        //申请带队
        protected void Send_Message_ApplyLaeding(ulong roleid)
        {
            CmdTeamApplyLeadingReq info = new CmdTeamApplyLeadingReq();

            NetClient.Instance.SendMessage((ushort)CmdTeam.ApplyLeadingReq, info);
        }

        /// <summary>
        /// 带队申请回答
        /// </summary>
        /// <param name="msg"></param>
        protected void Send_Message_ApplyLeadingOpReq(uint result, CmdTeamApplyLeadingNtf ntf)
        {
            CmdTeamApplyLeadingOpReq info = new CmdTeamApplyLeadingOpReq()
            {
                Op = result,
                RoleId = ntf.RoleId
            };

            NetClient.Instance.SendMessage((ushort)CmdTeam.ApplyLeadingOpReq, info);
        }


        /// <summary>
        /// 委托指挥
        /// </summary>
        protected void Send_Message_EntrustCommandReq(ulong roleid)
        {

            CmdTeamEntrustCommandReq info = new CmdTeamEntrustCommandReq();

            info.EntrustMemId = roleid;

            NetClient.Instance.SendMessage((ushort)CmdTeam.EntrustCommandReq, info);
        }


        /// <summary>
        /// 更改队伍目标请求
        /// </summary>
        /// <param name="msg"></param>
        protected void Send_Message_EditTargetReq(uint id, uint lowLv, uint highLv, bool autoApply, bool allowRobot, string desc)
        {

            CmdTeamEditTargetReq info = new CmdTeamEditTargetReq()
            {
                Target = new TeamTarget()
                {
                    TargetId = id,
                    LowLv = lowLv,
                    HighLv = highLv,
                    AutoApply = autoApply,
                    Desc = ByteString.CopyFromUtf8(desc),
                    AllowRobot = allowRobot
                }
            };

            NetClient.Instance.SendMessage((ushort)CmdTeam.EditTargetReq, info);

        }

        /// <summary>
        /// 修改自定义目标
        /// </summary>
        /// <param name="msg"></param>
        protected void Send_Message_EditCustomInfoReq(uint id, string customInfo)
        {

            CmdTeamEditCustomInfoReq info = new CmdTeamEditCustomInfoReq()
            {
                TargetId = id,
                CustomInfo = ByteString.CopyFromUtf8(customInfo)
            };

            NetClient.Instance.SendMessage((ushort)CmdTeam.EditCustomInfoReq, info);
        }

        /// <summary>
        /// 修改队伍描述
        /// </summary>
        /// <param name="msg"></param>
        protected void Send_Message_EditDescReq(string des)
        {
            CmdTeamEditDescReq info = new CmdTeamEditDescReq()
            {
                Desc = ByteString.CopyFromUtf8(des),
            };

            NetClient.Instance.SendMessage((ushort)CmdTeam.EditDescReq, info);
        }

        /// <summary>
        /// 请求匹配操作
        /// </summary>
        /// <param name="result"> 0 开始匹配，1 取消</param>
        protected void Send_Message_MatchingOpReq(uint result, uint targetID)
        {
            CmdTeamMatchingOpReq info = new CmdTeamMatchingOpReq()
            {
                Op = result,
                TargetId = targetID
            };

            NetClient.Instance.SendMessage((ushort)CmdTeam.MatchingOpReq, info);
        }

        /// <summary>
        /// 请求队伍列表
        /// </summary>
        /// <param name="msg"></param>
        protected void Send_Message_QueryMatchListReq(uint targetID,bool isType)
        {

            CmdTeamQueryMatchListReq info = new CmdTeamQueryMatchListReq();

            if (isType)
                info.PlayType = new UInt32Value() { Value = targetID };
            else
                info.TargetId = targetID;

            NetClient.Instance.SendMessage((ushort)CmdTeam.QueryMatchListReq, info);
        }



        #endregion

        #region toserver api
        //创建队伍
        public void ApplyCreateTeam(ulong roleId,uint targetid = 0)
        {
            //Debug.LogError("申请升为队长");
            Send_Message_CreateTeam(roleId, targetid);
        }

        // 申请加入队伍
        public void ApplyJoinTeam(ulong teamID, ulong roldId,uint applyType = 0)
        {
            if (roldId == Sys_Role.Instance.RoleId && HaveTeam)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(101801));
                return;
            }

            if (Sys_FamilyResBattle.Instance.InFamilyBattle) {
                if (!Sys_FamilyResBattle.Instance.IsInSafeArea(Sys_Role.Instance.RoleId)) {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3251000007));
                    return;
                }
                //else if (!Sys_FamilyResBattle.Instance.IsInSafeArea(roldId)) {
                //    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3251000003));
                //    return;
                //}
            }

            Send_Message_EnterReq(teamID, applyType);
        }
        // 请离队伍
        public void KickMemTeam(ulong roleId)
        {
            if (Sys_FamilyResBattle.Instance.InFamilyBattle) {
                if (Sys_FamilyResBattle.Instance.HasResource(out uint _)) {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3251000005));
                    return;
                }
            }

            var mem = getTeamMem(roleId);

            if (mem == null)
                return;

            OpenTips(0, LanguageHelper.GetTextContent(15129, mem.Name.ToStringUtf8()), () =>
            {

                Send_Message_KickMemReq(roleId);
            });

           
        }

        //退出 离开队伍
        public void ApplyExitTeam()
        {
            if (Sys_FamilyResBattle.Instance.InFamilyBattle) {
                if (Sys_FamilyResBattle.Instance.HasResource(CaptainRoleId, out uint _)) {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3251000004));
                    return;
                }
            }

            OpenTips(0, LanguageHelper.GetTextContent(15165), () =>
            {

                Send_Message_ExitTeam(0, 0);
            });

           
        }

        //暂离
        public void ApplyLeave(ulong roleID)
        {
            //Debug.LogError("暂离");

            if (roleID == CaptainRoleId)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2002247));
                return;
            }
                

            TeamMem teamMem = getTeamMem(roleID);

            if (teamMem == null)
                return;

            if (teamMem.IsLeave())
                return;

            Send_Message_LeaveTeam(0, roleID);
        }
        //申请回归
        public void ApplyComeBack(ulong roleID)
        {
            TeamMem teamMem = getTeamMem(Sys_Role.Instance.RoleId);

            if (teamMem == null)
                return;

            if (teamMem.IsLeave() == false)
                return;

            if (teamMem.IsOffLine())
                return;

            Send_Message_MemApplyBack(roleID);
        }

        //申请召回
        public void ApplyCallBack(ulong roleID)
        {
            Send_Message_ApplyCallBack(roleID);
        }
        //申请带队
        public void ApplyLeader(ulong roleID)
        {
            // Debug.LogError("申请带队");

            if (Sys_FamilyResBattle.Instance.InFamilyBattle) {
                if (Sys_FamilyResBattle.Instance.HasResource(CaptainRoleId, out uint _)) {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3251000006));
                    return;
                }
            }

            Send_Message_ApplyLaeding(roleID);
        }

        //申请升为队长
        public void ApplyToCaptaion(ulong roleID)
        {
            TeamMem teamMem = getTeamMem(roleID);

            if (teamMem == null)
                return;

            if (teamMem.IsLeave())
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2002242));
                return;
            }

            if (teamMem.IsOffLine())
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2002241));
                return;
            }

            if (Sys_FamilyResBattle.Instance.InFamilyBattle) {
                if (Sys_FamilyResBattle.Instance.HasResource(CaptainRoleId, out uint _)) {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3251000006));
                    return;
                }
            }

            Send_Message_ApplyCaptain(roleID);
        }

        /// 邀请他人
        public void InvitedOther(ulong roldID)
        {
            if (TeamMemsCount >= 5)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2002201));
                return;
            }

            if (Sys_FamilyResBattle.Instance.InFamilyBattle) {
                if (!Sys_FamilyResBattle.Instance.IsInSafeArea(Sys_Role.Instance.RoleId)) {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3251000001));
                    return;
                }
                //else if (!Sys_FamilyResBattle.Instance.IsInSafeArea(roldID)) {
                //    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3251000002));
                //    return;
                //}
            }

            Send_Message_InviteReq(roldID);
        }


        public void ClearApplyList()
        {
            Send_Message_ApplyListOpReq(2, 0);
        }

        /// <summary>
        /// 委托指挥
        /// </summary>
        /// <param name="roleID"></param>
        public void GiveToCommand(ulong roleID)
        {
            Send_Message_EntrustCommandReq(roleID);
        }

        /// <summary>
        /// 取消委托指挥
        /// </summary>
        /// <param name="roleID"></param>
        public void CancleToCommand()
        {
            Send_Message_EntrustCommandReq(0);
        }
        #endregion

        #region

        //更改队伍目标请求
        public void ApplyEditTarget(uint id, uint LowLv, uint HighLv, bool autoApply, bool allowRobot,string desc = "")
        {
            Send_Message_EditTargetReq(id, LowLv, HighLv, autoApply, allowRobot,desc);
        }

        //修改自定义目标
        public void ApplyEditCustomInfo(uint id, string strinfo)
        {
            Send_Message_EditCustomInfoReq(id, strinfo);
        }

        //修改队伍描述
        public void ApplyEditDesc(string desc)
        {
            Send_Message_EditDescReq(desc);
        }

        //请求匹配操作
        public void ApplyMatching(uint result, uint targetID)
        {
            Send_Message_MatchingOpReq(result, targetID);

           // MatchingTarget = targetID;
        }

        //请求队伍列表
        public void ApplyQueryMatchList(uint targetID,bool isType)
        {
            //if (mCDCon.get(CDController.EType.ApplyQueryMatchList) == false)
            //{
            //    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2002248));
            //    return;
            //}
            Send_Message_QueryMatchListReq(targetID,isType);


           // mCDCon.Start(CDController.EType.ApplyQueryMatchList);
        }


        public void ApplySearchNearRole()
        {
            CmdTeamSearchNearRoleReq info = new CmdTeamSearchNearRoleReq();


            NetClient.Instance.SendMessage((ushort)CmdTeam.SearchNearRoleReq, info);
        }
        #endregion

        /// <summary>
        /// 一键喊话 0 组队  1，家族
        /// </summary>
        public void SendTalk(uint type,string value)
        {
            if (type == 1 && Sys_Family.Instance.familyData.isInFamily == false)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12290));
                return;
            }
            var cdtype = type == 0 ? CDController.EType.TalkTeam : CDController.EType.TalkFamliy;

            if (isCDReady(cdtype) == false)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11882, RemainingCDTime(cdtype).ToString()));
                return;
            }

            var data = CSVTeam.Instance.GetConfData(teamTargetID);

            string strname;

            if (getCustomTargetName(teamTargetID, out strname) == false)
            {
                strname = LanguageHelper.GetTextContent(data.subclass_name);
            }

            string languageid =  data == null ? string.Empty : LanguageHelper.GetTextContent(10705, strname,
                TeamMemsCount.ToString(),teamTarget.LowLv.ToString(), teamTarget.HighLv.ToString());

            if (!string.IsNullOrEmpty(value))
                languageid += value;

            languageid += string.Format("[${0}]", teamID.ToString());

            ChatType ctype = type == 1 ? ChatType.Guild : ChatType.LookForTeam;

            //int rlt = Sys_Chat.Instance.SendContent(ChatType.Local, languageid);

            CmdTeamLookForTeamReq info = new CmdTeamLookForTeamReq();

            info.ChatType = (uint)ctype;
            info.ChatMsg = ByteString.CopyFromUtf8(languageid);


            NetClient.Instance.SendMessage((ushort)CmdTeam.LookForTeamReq, info);


           StartCD(cdtype);
        }

        /// <summary>
        /// 社交邀请
        /// </summary>
        /// <param name="type">1 好友 2 家族 </param>
        /// <param name="roleids"></param>
        private void SendInviteSocialReq()
        {
            CmdTeamInviteSocialReq info = new CmdTeamInviteSocialReq();

            NetClient.Instance.SendMessage((ushort)CmdTeam.InviteSocialReq, info);
        }

        /// <summary>
        /// 社交邀请
        /// </summary>
        /// <param name="type">1 好友 2 家族 </param>
        /// <param name="roleids"></param>
        private void SendInviteSocialConfirmReq(RepeatedField<ulong>roleids,uint type,bool isSend)
        {
            CmdTeamInviteSocialConfirmReq info = new CmdTeamInviteSocialConfirmReq();

            info.Send = isSend;
            info.Type = type;
            info.MemIds.AddRange(roleids);
            NetClient.Instance.SendMessage((ushort)CmdTeam.InviteSocialConfirmReq, info);
        }

        public void SendChangeMemberOrder(ulong roleid0,ulong roleid1)
        {
            if (isCaptain() == false)
                return;

            CmdTeamChangeOrderReq info = new CmdTeamChangeOrderReq();
            info.RoleA = roleid0;
            info.RoleB = roleid1;

            NetClient.Instance.SendMessage((ushort)CmdTeam.ChangeOrderReq, info);
        }

        public void SendTalkToggleChange(uint key,uint op)
        {

            if (HaveTeam && (isCaptain() == false && isPlayerLeave() == false))
                return;

            Sys_Task.Instance.OnChageSkipReq(key, (op == 1 ? true : false));
        }
        #region 交互



        /// <summary>
        /// 被邀请 回答
        /// </summary>
        /// <param name="result">结果</param>
        protected void InvitedAnswer(int result, IMessage message)
        {
            if (message is CmdTeamInviteNtf == false)
                return;

            Send_Message_InviteOpReq((uint)result, message as CmdTeamInviteNtf);
        }


        /// <summary>
        /// 队长收到入队申请的反馈 同意或者拒绝
        /// </summary>
        /// <param name="result">同意或者拒绝</param>
        protected void ApplyCaptainAnswer(int result, IMessage message)
        {
            Send_Message_ApplyListOpReq((uint)result, message as CmdTeamApplyNtf);
        }

        /// <summary>
        /// 队长处理申请列表
        /// </summary>
        /// <param name="result"></param>
        /// <param name="message"></param>
        public void ApplyCaptainAnswer(int result, ulong roleId)
        {
            Send_Message_ApplyListOpReq((uint)result, roleId);
        }

        /// <summary>
        /// 队长收到邀请申请 同意或者拒绝
        /// </summary>
        /// <param name="result">同意或者拒绝</param>
        protected void MemberCaptainAnswer(int result, IMessage message)
        {
            if (message is CmdTeamMemInviteNtf == false)
                return;

            Send_Message_MemInviteOpReq((uint)result, message as CmdTeamMemInviteNtf);
        }


        protected void ApplyLeadingAnswer(int result, IMessage message)
        {
            if (message is CmdTeamApplyLeadingNtf == false)
                return;

            Send_Message_ApplyLeadingOpReq((uint)result, message as CmdTeamApplyLeadingNtf);
        }


        // 召回回复
        protected void CallBackAnswer(int result, IMessage message)
        {
            if (message is CmdTeamCallBackNtf == false)
                return;

            Send_Message_CallBackOpReq((uint)result, message as CmdTeamCallBackNtf);
        }
        #endregion




    }
}
