
using Lib.Core;
using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Table;
using System;
using Net;
using Packet;
using Google.Protobuf.Collections;

namespace Logic
{
    //UndergroundArena  地下竞技场
    public partial class Sys_Instance_UGA : SystemModuleBase<Sys_Instance_UGA>
    {
        public enum EEvents
        {
            StartVote,
            PlayerConfirmNtf,
            VoteUpdate,
            EndReward,
            VoteData,
            RankUpdate,
            InfoRefresh,
            InstanceRefresh,

        }
        
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        private List<uint> InstanceIDs = new List<uint>();
        public uint CurInstance { get {

                if (Data == null)
                    return 0;

                return Data.Formation.Instanceid;
            
            } }


        public uint CurInStageID { get; private set; }
        /// <summary>
        /// 界数
        /// </summary>
        public uint Num { get { return Data.Base.Round; }}

        public uint EndTime { get {

                if (serverInstanceData == null)
                    return 0;

                return serverInstanceData.instanceCommonData.ResLimit.ExpireTime;
            
            } }


        public readonly uint ActivityID = 210;
        public CmdUnderGroundDataNty Data { get; private set; }

        public CmdRankQueryRes RankInfo { get; private set; }

        public UnderGroundInsData InstanceData { get; private set; }

        public Sys_Instance.ServerInstanceData serverInstanceData { get; private set; }
        public override void Init()
        {
            Sys_Vote.Instance.AddVoteLisitener((ushort)VoteType.UnderGround, OnNotify_Vote);
            Sys_Instance.Instance.eventEmitter.Handle(Sys_Instance.EEvents.InstanceDataUpdate, OnInstanceInfoUpdate, true);
            Sys_Instance.Instance.eventEmitter.Handle(Sys_Instance.EEvents.InstanceExit, OnInstanceExit, true);
            //Sys_Instance.Instance.eventEmitter.Handle(Sys_Instance.EEvents.InstanceEnter, OnInstanceEnter, true);
            Sys_Instance.Instance.eventEmitter.Handle<uint, uint>(Sys_Instance.EEvents.StateNtf, OnInstanceEnter, true);

            Sys_Rank.Instance.eventEmitter.Handle<CmdRankQueryRes>(Sys_Rank.EEvents.RankQueryRes, OnRankUpdate, true); ;

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdUnderGround.DataNty, NotifyDataNtf, CmdUnderGroundDataNty.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdUnderGround.FormationNty, NotifyFormationNtf, CmdUnderGroundFormationNty.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdUnderGround.StageResultNty, OnInstanceStagePass, CmdUnderGroundStageResultNty.Parser);

            LoadConfig();
        }

        public override void OnLogout()
        {
            serverInstanceData = null;
            InstanceData = null;
            Data = null;
        }
        private void LoadConfig()
        {
            var data = CSVInstanceParam.Instance.GetConfData(1);
            var strids = data.str_value.Split('|');
            int length = strids.Length;
            for (int i = 0; i < length; i++)
            {
                if (uint.TryParse(strids[i], out uint value))
                {
                    InstanceIDs.Add(value);
                }

            }
        }

        public CSVInstance.Data GetRoleLevelInstanceData()
        {
            uint level = Sys_Role.Instance.Role.Level;

            int count = InstanceIDs.Count;

            for (int i = 0; i < count; i++)
            {
                var data = CSVInstance.Instance.GetConfData(InstanceIDs[i]);
                if (data.lv[0] <= level && level <= data.lv[0])
                {
                    return data;
                }
            }

            return null;
        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="LevelID"></param>
        /// <returns>小于0 未参与 ，等于0 失败 ，大于0 胜利</returns>
        public int GetPKRecord(uint LevelID)
        {
            return -1;
        }

        public UnderGroundFormation GetMonsterFormation()
        {
            return Data.Formation;
        }

        public uint GetInstanceID(int index)
        {
            if (index < 0 || index >= InstanceIDs.Count)
                return 0;

            return InstanceIDs[index];
        }

        public int GetInstanceIndex(uint id)
        {
            int result = -1;

            int count = InstanceIDs.Count;
            for (int i = 0; i < count; i++)
            {
                if (id == InstanceIDs[i])
                {
                    result = i;
                    break;
                }
            }

            return result;
        }

        public uint GetCurInstanceMaxStage()
        {
            if (serverInstanceData == null)
                return 0;

            var instancecommondata = serverInstanceData.instanceCommonData;

            var entriesdata = instancecommondata.Entries.Find(o => o.InstanceId == CurInstance);

            if (entriesdata == null)
                return 0;

            return entriesdata.PerMaxStageId;
        }
        public int GetCurInstanceMaxStageOrder()
        {
            var stageid = GetCurInstanceMaxStage();

            if (stageid == 0)
                return 0;

             var data = CSVInstanceDaily.Instance.GetConfData(stageid);

            if (data == null)
                return 0;

            return (int)data.Layerlevel;
        }
        public bool HadShowTips(uint instanceid)
        {
            var data = InstanceData.Instances.Find(o => o.Instanceid == instanceid);

            if (data == null)
                return true;

            return data.Hintdisplayed;
        }

        private void SetShowTips(uint instanceid)
        {
            var data = InstanceData.Instances.Find(o => o.Instanceid == instanceid);

            if (data == null)
                return;

            data.Hintdisplayed = true;
        }
        private void OnInstanceInfoUpdate()
        {
            Sys_Instance.ServerInstanceData data = Sys_Instance.Instance.GetServerInstanceData(ActivityID);

            if (data == null)
                return;


            serverInstanceData = data;

            InstanceData = data.instancePlayTypeData.UnderGroundData;

            eventEmitter.Trigger(Sys_Instance_UGA.EEvents.InstanceRefresh);
        }

        private void OnInstanceExit()
        {
            if (UIManager.IsOpen(EUIID.UI_UndergroundArena_Result))
            {
                UIManager.CloseUI(EUIID.UI_UndergroundArena_Result);
            }
        }

        private void OnInstanceEnter(uint instanceid, uint stageid)
        {
            InInstanceID = instanceid;
        }
        private CmdInstancePassStageNtf LastLevelReward = null;
        private void OnInstanceStagePass(NetMsg msg)
        {
            CmdUnderGroundStageResultNty info = NetMsgUtil.Deserialize<CmdUnderGroundStageResultNty>(CmdUnderGroundStageResultNty.Parser, msg);

            UI_Underground_Result_Parma parmas = new UI_Underground_Result_Parma();
            parmas.info = info;
            parmas.InstanceID = InInstanceID;
            parmas.StageID = info.Stageid;
            UIManager.OpenUI(EUIID.UI_UndergroundArena_Result, false, parmas);

        }

        private void NotifyDataNtf(NetMsg msg)
        {
            CmdUnderGroundDataNty info = NetMsgUtil.Deserialize<CmdUnderGroundDataNty>(CmdUnderGroundDataNty.Parser, msg);

            Data = info;

            eventEmitter.Trigger(EEvents.InfoRefresh);
        }

        private void NotifyFormationNtf(NetMsg msg)
        {
            CmdUnderGroundFormationNty info = NetMsgUtil.Deserialize<CmdUnderGroundFormationNty>(CmdUnderGroundFormationNty.Parser, msg);

            if (Data == null)
            {
                Data = new CmdUnderGroundDataNty();
            }
            Data.Base = info.Base;
            Data.Formation = info.Formation;

            eventEmitter.Trigger(EEvents.InfoRefresh);
        }

        Dictionary<uint, CmdRankQueryRes> m_RankDic = new Dictionary<uint, CmdRankQueryRes>();

        private void OnRankUpdate(CmdRankQueryRes res)
        {
            if (res.Type != (uint)RankType.UnderGround)
                return;

            RankInfo = res;

            if (m_RankDic.ContainsKey(res.SubType) == false)
                m_RankDic.Add(res.SubType, res);
            else
                m_RankDic[res.SubType] = res;

            eventEmitter.Trigger(EEvents.RankUpdate);
        }
        /// <summary>
        /// 排行榜
        /// </summary>
        /// <param name="id"></param>
        public void SendRankInfo(uint id)
        {
            uint subtype = (uint)(id % 1000);

            if (m_RankDic.TryGetValue(subtype, out CmdRankQueryRes data))
            {
                var nowtime = Sys_Time.Instance.GetServerTime();

                if (data.NextReqTime > nowtime)
                {
                    RankInfo = data;
                    eventEmitter.Trigger(EEvents.RankUpdate);
                    return;
                }
            }
            CmdRankQueryReq req = new CmdRankQueryReq();
            req.Type = (uint)RankType.UnderGround;
            req.SubType = subtype;
            req.GroupType = 0;
            req.Notmain = true;
            NetClient.Instance.SendMessage((ushort)CmdRank.QueryReq, req);

           // Sys_Rank.Instance.RankQueryReq((uint)RankType.UnderGround, id % 1000);
        }

        /// <summary>
        /// 标记提示已读
        /// </summary>
        /// <param name="id"></param>
        public void SendSetFristTips(uint id)
        {
            CmdUnderGroundClearHintReq info = new CmdUnderGroundClearHintReq();
            info.Instanceid = id;

            NetClient.Instance.SendMessage((ushort)CmdUnderGround.ClearHintReq, info);

            SetShowTips(id);
        }

        /// <summary>
        /// 副本内发起战斗
        /// </summary>
        public void SendStartFightInInstance()
        {
            CmdUnderGroundStartFightReq info = new CmdUnderGroundStartFightReq();
       
            NetClient.Instance.SendMessage((ushort)CmdUnderGround.StartFightReq, info);
        }

        /// <summary>
        /// 发起投票
        /// </summary>
        public void SendStartVote()
        {
            CmdUnderGroundStartVoteReq info = new CmdUnderGroundStartVoteReq();

            NetClient.Instance.SendMessage((ushort)CmdUnderGround.StartVoteReq, info);
        }
    }


    public partial class Sys_Instance_UGA : SystemModuleBase<Sys_Instance_UGA>
    {
        public ulong VoteID { get; private set; } = 0;
        public UnderGroundInsVoteData VoteData { get; private set; } = null;

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
            NetMsgUtil.TryDeserialize<UnderGroundInsVoteData>(UnderGroundInsVoteData.Parser, msg.ClientData.ToByteArray(), out UnderGroundInsVoteData data);

            VoteID = msg.VoteId;

            var CurInstancID = data.Instanceid;
            var CurStageID = data.Stageid;

            InInstanceID = CurInstancID;

            VoteData = data;

            VoteStartTime = msg.StartTime;

            eventEmitter.Trigger(EEvents.StartVote);

            UIManager.CloseUI(EUIID.UI_UndergroundArena);

            bool iscapter = Sys_Team.Instance.isCaptain();
            //if (UIManager.IsOpen(EUIID.UI_Bio_Vote) == false)
            {
                UIManager.OpenUI(EUIID.UI_UndergroundArena_Vote, false, new UI_Underground_Vote_Parmas()
                { isCapter = iscapter, InstanceID = CurInstancID, LevelID = CurStageID, State = iscapter ? 3: 2 });
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

            UIManager.CloseUI(EUIID.UI_UndergroundArena_Vote);


            if (msg.ResultType == 2u && Sys_Team.Instance.isCaptain())
            {
                UIManager.OpenUI(EUIID.UI_UndergroundArena);
            }
        }

        void OnNotify_VoteUpdateNtf(CmdRoleVoteUpdateNtf msg)
        {
            NetMsgUtil.TryDeserialize<UnderGroundInsVoteData>(BioInsVoteData.Parser, msg.ClientData.ToByteArray(), out UnderGroundInsVoteData data);

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


        public void OnSendQueryEnterInfo(uint instenceID, uint stage)
        {
            CmdBioInstanceQueryEnterInfoReq info = new CmdBioInstanceQueryEnterInfoReq();
            info.InstanceId = instenceID;
            info.StageId = stage;

            NetClient.Instance.SendMessage((ushort)CmdBioInstance.QueryEnterInfoReq, info);
        }

        public uint GetVoteMemberRecord(ulong roleid)
        {
            if (VoteData == null)
            {
                if (Data == null)
                    return 0;

                return 0;
            }


            var data = VoteData.Players.Find(o => o.Roleid == roleid);

            if (data == null)
                return 0;

            return data.Stageid;
        }
    }
}
