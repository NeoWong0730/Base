using Lib.Core;
using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Table;
using System;
using Net;
using Packet;
namespace Logic
{
    public partial class Sys_Instance_Bio : SystemModuleBase<Sys_Instance_Bio>
    {
        public enum EEvents
        {
            StartVote,
            PlayerConfirmNtf,
            VoteUpdate,
            FristPass,
            EndReward,
            VoteData,

        }
        public readonly uint ActivityID = 130u;

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public BioInsData insData { get; private set; }
        public Sys_Instance.ServerInstanceData serverInstanceData { get; private set; }

        public BioInsFirstPassInfo FristPassInfo { get; private set; } = null;
        public CmdBioInstanceEndRewardNtf EndReward { get; private set; }

        public override void Init()
        {
            Sys_Instance.Instance.eventEmitter.Handle(Sys_Instance.EEvents.InstanceDataUpdate, OnInstanceInfoUpdate, true);
            Sys_Instance.Instance.eventEmitter.Handle<uint, uint, uint>(Sys_Instance.EEvents.PassStage, OnInstanceStagePass, true);

            Sys_Vote.Instance.AddVoteLisitener((ushort)VoteType.Biography, OnNotify_Vote);

            EventDispatcher.Instance.AddEventListener(0,(ushort)CmdBioInstance.FirstPassRes, OnFirstPassRes, CmdBioInstanceFirstPassRes.Parser);

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBioInstance.EndRewardNtf, OnEndRewardNtf, CmdBioInstanceEndRewardNtf.Parser);

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBioInstance.QueryEnterInfoRes, OnQueryEnterInfoRes, CmdBioInstanceQueryEnterInfoRes.Parser);

        }


        private void OnInstanceInfoUpdate()
        {
            Sys_Instance.ServerInstanceData data = Sys_Instance.Instance.GetServerInstanceData(130);

            if (data == null)
                return;

            serverInstanceData = data;

            insData = data.instancePlayTypeData.BioData;
        }

        private CmdInstancePassStageNtf LastLevelReward = null;
        private void OnInstanceStagePass(uint playtype,uint instanceid,uint stageid)
        {
            if (playtype != ActivityID)
                return;


            bool islastStage = Sys_Instance.Instance.IsLastLevel(instanceid, stageid);

            LastLevelReward = null;

            CmdInstancePassStageNtf info = Sys_Instance.Instance.CurMessage as CmdInstancePassStageNtf;

            if (info != null && info.Rewards.Count > 0)
            {
                if (islastStage == false)
                {
                    UI_Multi_StageReward_Parmas parmas = new UI_Multi_StageReward_Parmas();
                    parmas.info = info;
                    UIManager.OpenUI(EUIID.UI_Bio_StageReward, false, parmas);
                }
                else
                {
                    LastLevelReward = info;
                }

            }

        }
        private void OnFirstPassRes(NetMsg msg)
        {
            CmdBioInstanceFirstPassRes info = NetMsgUtil.Deserialize<CmdBioInstanceFirstPassRes>(CmdBioInstanceFirstPassRes.Parser, msg);

            FristPassInfo = info.Info;

            eventEmitter.Trigger(EEvents.FristPass);
        }

        public void SendFristPass(uint instanceID)
        {
            CmdBioInstanceFirstPassReq info = new CmdBioInstanceFirstPassReq();
            info.InstanceId = instanceID;

            NetClient.Instance.SendMessage((ushort)CmdBioInstance.FirstPassReq, info);

        }

        private void OnEndRewardNtf(NetMsg msg)
        {
            CmdBioInstanceEndRewardNtf info = NetMsgUtil.Deserialize<CmdBioInstanceEndRewardNtf>(CmdBioInstanceEndRewardNtf.Parser, msg);

            EndReward = info;

            //DebugUtil.LogError("instance_bio get endrewardntf netmsg --- " + info.HistoryPass);
            UIManager.OpenUI(EUIID.UI_Bio_Result, false, new UI_Multi_ResultNew_Parma() { InstanceID = InInstanceID,Mode = 0,isFristCorss = info.HistoryPass });

            eventEmitter.Trigger(EEvents.EndReward);

            if (LastLevelReward != null)
            {
                UI_Multi_StageReward_Parmas parmas = new UI_Multi_StageReward_Parmas();
                parmas.info = LastLevelReward;
                UIManager.OpenUI(EUIID.UI_Bio_StageReward, false, parmas);

                LastLevelReward = null;
            }

        }

        private void OnQueryEnterInfoRes(NetMsg msg)
        {
            CmdBioInstanceQueryEnterInfoRes info = NetMsgUtil.Deserialize<CmdBioInstanceQueryEnterInfoRes>(CmdBioInstanceQueryEnterInfoRes.Parser, msg);

            VoteData = info.Info;

            eventEmitter.Trigger(EEvents.VoteData);
        }
        /// <summary>
        /// 根据副本ID 获取人物传记系列 根据人物传记章节组织的数据
        /// </summary>
        /// <param name="instanceID"></param>
        /// <returns></returns>
        public CSVNewBiographySeries.Data getSeries(uint instanceID)
        {
            var data = CSVNewBiographySeries.Instance.GetAll();

            CSVNewBiographySeries.Data value = null;

            foreach (var item in data)
            {
                if (item.instance_id.Contains(instanceID))
                {
                    value = item;
                    break;
                }
            }

            return value;
        }

        public uint GetDailyTimes()
        {
            if (insData == null)
                return 0;

            return insData.StageRewardLimit.UsedTimes;
        }

    }


    public partial class Sys_Instance_Bio : SystemModuleBase<Sys_Instance_Bio>
    {
        public ulong VoteID { get; private set; } = 0;
        public BioInsVoteData VoteData { get; private set; } = null;

        private uint VoteStartTime = 0;

        public uint InInstanceID { get; private set; } = 0;
        void OnNotify_Vote(Sys_Vote.EVote eVote, object message)
        {
            if (eVote == Sys_Vote.EVote.Start)
                OnNotify_PlayerConfirmPush(message as CmdRoleStartVoteNtf);

            if (eVote == Sys_Vote.EVote.DoVote)
                OnNotify_PlayerConfirmNtf(message as CmdRoleDoVoteNtf);

            if (eVote == Sys_Vote.EVote.End)
                OnNotify_PlayerConfirmEnd(message as CmdRoleVoteEndNtf);

            if (eVote == Sys_Vote.EVote.Update)
                OnNotify_VoteUpdateNtf(message as CmdRoleVoteUpdateNtf);
        }
      
        void OnNotify_PlayerConfirmPush(CmdRoleStartVoteNtf msg)
        {
            NetMsgUtil.TryDeserialize<BioInsVoteData>(BioInsVoteData.Parser, msg.ClientData.ToByteArray(), out BioInsVoteData data);

            VoteID = msg.VoteId;

            var CurInstancID = data.InstanceId;
            var CurStageID = data.StageId;

            InInstanceID = CurInstancID;

            VoteData = data;

            VoteStartTime = msg.StartTime;

            eventEmitter.Trigger(EEvents.StartVote);

            UIManager.CloseUI(EUIID.UI_Bio_PlayType);

            UIManager.CloseUI(EUIID.UI_Bio_Info);

            //if (UIManager.IsOpen(EUIID.UI_Bio_Vote) == false)
            {
                UIManager.OpenUI(EUIID.UI_Bio_Vote, false, new UI_Multi_ReadyNew_Parmas()
                { isCapter = Sys_Team.Instance.isCaptain(), InstanceID = CurInstancID, LevelID = CurStageID, State = 2 });
            }

        }

        void OnNotify_PlayerConfirmNtf(CmdRoleDoVoteNtf msg)
        {
            if (msg.VoteId != VoteID)
                return;

            eventEmitter.Trigger<ulong, int>(EEvents.PlayerConfirmNtf, msg.RoleId, (int)msg.Op);
        }

        void OnNotify_PlayerConfirmEnd(CmdRoleVoteEndNtf msg)
        {
            if (msg.VoteId != VoteID)
                return;

            if (msg.ResultType == 2)
            {
                InInstanceID = 0;
               

                if (msg.FailReason == (uint)VoteFailReason.ManualCancel)
                {
                   var teamMem = Sys_Team.Instance.getTeamMem(msg.CancelVoterId);

                    if (teamMem != null)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2022943, teamMem.Name.ToStringUtf8()));
                    }
                }
            }
               

            VoteID = 0;

            VoteData = null;

            UIManager.CloseUI(EUIID.UI_Bio_Vote);

            //if (UIManager.IsOpen(EUIID.UI_Bio_PlayType) && msg.ResultType == 2)
            //{
            //    UIManager.CloseUI(EUIID.UI_Bio_PlayType,true);
            //}

            if (msg.ResultType == 2u && Sys_Team.Instance.isCaptain())
            {
                UIManager.OpenUI(EUIID.UI_Bio_PlayType);
            }
        }

        void OnNotify_VoteUpdateNtf(CmdRoleVoteUpdateNtf msg)
        {
            NetMsgUtil.TryDeserialize<BioInsVoteData>(BioInsVoteData.Parser, msg.ClientData.ToByteArray(), out BioInsVoteData data);

            VoteID = msg.VoteId;

            VoteData = data;

            eventEmitter.Trigger(EEvents.VoteUpdate);
        }
        /// <summary>
        /// 成员投票请求
        /// </summary>
        /// <param name="b"></param>
        public void OnSendPlayerConfirmres(bool b)
        {
            if (VoteID == 0)
                return;

            Sys_Vote.Instance.Send_DoVoteReq(VoteID, (uint)(b ? 1 : 2));

        }

        public void OnSendExitConfirmres()
        {
            if (VoteID == 0)
                return;

            Sys_Vote.Instance.Send_CancleVoteReq(VoteID);
        }


        public void OnSendQueryEnterInfo(uint instenceID,uint stage)
        {
            CmdBioInstanceQueryEnterInfoReq info = new CmdBioInstanceQueryEnterInfoReq();
            info.InstanceId = instenceID;
            info.StageId = stage;

            NetClient.Instance.SendMessage((ushort)CmdBioInstance.QueryEnterInfoReq, info);
        }
        public float GetVoteTime()
        {
            if (VoteStartTime == 0)
                return 0;

            var data = CSVParam.Instance.GetConfData(350);

            var realtime = int.Parse(data.str_value) / 1000f;

            var servertime = Sys_Time.Instance.GetServerTime();

            float time = realtime;

            if (servertime > VoteStartTime)
            {
                uint distime = servertime - VoteStartTime;

                time = realtime - (float)distime;

                time = Mathf.Max(0, time);
            }

            return time;
        }

        public uint GetVoteMemberLeftTimes(ulong roleid)
        {
            if (VoteData == null)
            {
                if(insData == null)
                    return 0;

                return 0;
            }
               

            var data = VoteData.Players.Find(o => o.RoleId == roleid);

            if (data == null)
                return 0;

            return data.LeftTimes;
        }
    }
}
