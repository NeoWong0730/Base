using System;
using System.Collections.Generic;
using Google.Protobuf.Collections;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;

using Table;

namespace Logic
{
    public partial class Sys_GoddnessTrial : SystemModuleBase<Sys_GoddnessTrial>
    {
        ulong VoteID { get; set; } = 0;

        /// <summary>
        /// 所有主题通关的第一个队伍信息
        /// </summary>
        public CmdGoddessTrialGetTopicFirstTeamRes FristCrossInfo { get; private set; }

        /// <summary>
        /// 队员信息，包括本周通关的最高纪录
        /// </summary>
        public GoddessTrialTeamMemData TeamMemsRecord { get; private set; }

        /// <summary>
        /// 副本通用的玩法信息
        /// </summary>
        InstancePlayTypeData PlayData
        {
            get
            {
                var value = Sys_Instance.Instance.GetServerInstanceData(PLAYTYPE);
                if (value == null)
                    return null;
                return value.instancePlayTypeData;
            }
        }

        /// <summary>
        /// 女神炼狱的信息
        /// </summary>
        public GoddessTrialData GTData
        {
            get
            {
                var value = PlayData;
                if (value == null)
                    return null;
                return value.GoddessTrialData;
            }
        }
        List<RandomTopic> RandomTopicSortList = new List<RandomTopic>();

        /// <summary>
        /// 关卡投票结果
        /// </summary>
        public CmdGoddessTrialSelectStageVoteEndNtf StateVoteResult { get; private set; }


        /// <summary>
        /// 排行
        /// </summary>
        public TopicDiffiRank Rank { get; private set; }

        /// <summary>
        /// 本周副本特性，与服务器同步
        /// </summary>
        public uint TopicProperty { get; private set; } = 0;


        public CmdGoddessTrialInsStagePassAwardNtf InstancePassAward { get; private set; } = null;



        private bool m_IsWaitShowMainUI = false;
        private uint m_LastInstanceID = 0;
        private uint m_LastTopic = 0;

        private uint m_SelectVoteState = 0;
        public uint WaitResultInstanceID { get; private set; }
        public bool WaitResultIsPass { get; private set; }

        public bool IsWaitResult { get; private set; }

        public CmdGoddessTrialUpdateInsAwardNtf UpdateInsAwardNtf { get; private set; } = null;

        /// <summary>
        /// 获取队员在当前主题下的最高记录，副本ID
        /// </summary>
        /// <param name="roleid"></param>
        /// <returns></returns>
        public uint GetMemRecordIns(ulong roleid)
        {
            if (TeamMemsRecord == null)
                return 0;

            var data = TeamMemsRecord.TeamData.Find<GoddessTrialTeamMemData.Types.OneMemData>(o => o.RoleId == roleid);

            if (data == null)
                return 0;

            return data.InsId;
        }

        /// <summary>
        /// 获取当前主题类型的结局记录
        /// </summary>
        /// <returns></returns>
        public EndTopic GetEndRecord()
        {
            if (GTData == null)
                return null;

           var result = GTData.EndTopic.Find<EndTopic>(o => o.TopicId == TopicTypeID);

            return result;
        }

        /// <summary>
        /// 结局是否已开启
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool EndCollectIsEnd(uint id)
        {
            EndTopic endTopic = GetEndRecord();

            if (endTopic == null)
                return false;

           var result = endTopic.EndId.Find<uint>(o => o == id);

            return result != 0;
        }


        private bool isFristStageInstance(uint instance, uint levelid)
        {
            var stageList = Sys_Instance.Instance.getDailyByInstanceID(instance);

            var stageData = CSVInstanceDaily.Instance.GetConfData(levelid);

            if (stageData == null)
                return false;

            // int index = stageList.FindIndex(o => o.id == levelid);

            return stageData.Layerlevel == 1;
        }

        public List<CSVInstanceDaily.Data>  GetNextLevelID(uint instance, uint levelid)
        {
            var stageList = Sys_Instance.Instance.getDailyByInstanceID(instance);

            var stageData = CSVInstanceDaily.Instance.GetConfData(levelid);

            if (stageData == null)
                return null;

            var index = stageList.FindAll(o => o.Layerlevel == (stageData.Layerlevel + 1u));

            return index;
        }
        public bool IsLastLevel(uint instance, uint levelid)
        {
            var stageList = Sys_Instance.Instance.getDailyByInstanceID(instance);

            var stageData = CSVInstanceDaily.Instance.GetConfData(levelid);

            if (stageData == null)
                return false;

            int count = stageList.Count;

            if (count == 0)
                return false;

            return stageList[count - 1].Layerlevel == stageData.Layerlevel;
        }
    }

    public partial class Sys_GoddnessTrial : SystemModuleBase<Sys_GoddnessTrial>
    {
        private void RefreshRandomTopicList()
        {
            RandomTopicSortList.Clear();

           RandomTopicSortList.AddRange(GTData.RandomTopic);

            RandomTopicSortList.Sort((value0, value1) => {

                if (value0.MinLevel > value1.MinLevel)
                    return -1;

                if (value0.MinLevel < value1.MinLevel)
                    return 1;

                return 0;
            });
        }
    }
    public partial class Sys_GoddnessTrial:SystemModuleBase<Sys_GoddnessTrial>
    {

        private void InitNet()
        {
            Sys_Instance.Instance.eventEmitter.Handle<uint>(Sys_Instance.EEvents.InstanceData, NotifyDataRefresh,true);

            Sys_Instance.Instance.eventEmitter.Handle<uint,uint,uint>(Sys_Instance.EEvents.PassStage, NotifyPassStage, true);
            Sys_Instance.Instance.eventEmitter.Handle<uint, uint, uint, uint>(Sys_Instance.EEvents.SwitchStage, NotifySwitchStage, true);
            Sys_Instance.Instance.eventEmitter.Handle<bool, uint>(Sys_Instance.EEvents.InstanceEnd, NotityInstanceEnd, true);
            Sys_Instance.Instance.eventEmitter.Handle<uint, uint>(Sys_Instance.EEvents.StateNtf, NotifyStateNtf, true);
            Sys_Instance.Instance.eventEmitter.Handle(Sys_Instance.EEvents.InstanceExit, NotifyInstanceExit, true);

            Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnLoadOK, NotifyMapLoadOk, true);
            Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnRoleSameMapTel, NotifyMapLoadOk, true);

            Sys_Vote.Instance.AddVoteLisitener((ushort)VoteType.GoddessTrial, OnNotify_Vote);

            EventDispatcher.Instance.AddEventListener(0,(ushort)CmdGoddessTrial.GetTopicFirstTeamRes, NotifyTopicFristRes, CmdGoddessTrialGetTopicFirstTeamRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGoddessTrial.GetTeamInsInfoOnVoteInInsRes, NotifyTeamRecordRes, CmdGoddessTrialGetTeamInsInfoOnVoteInInsRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGoddessTrial.SelectStageStartVoteRes, NotifySelectStageStart, CmdGoddessTrialSelectStageStartVoteRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGoddessTrial.SelectStageVoteRes, NotifySelectStageVote, CmdGoddessTrialSelectStageVoteRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGoddessTrial.VoteFollowLeaderRes, NotifyFollowLeader, CmdGoddessTrialVoteFollowLeaderRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGoddessTrial.SelectStageStartVoteNtf, NotifyStageStartVoteNtf, CmdGoddessTrialSelectStageStartVoteNtf.Parser);

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGoddessTrial.SelectStageVoteProcessNtf, NotifySelestStageProcessNtf, CmdGoddessTrialSelectStageVoteProcessNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGoddessTrial.SelectStageVoteEndNtf, NotifySelectStageVoteEndNtf, CmdGoddessTrialSelectStageVoteEndNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGoddessTrial.SelectStageCancelVoteNtf, NotifySelectStageVoteCancleNtf, CmdGoddessTrialSelectStageCancelVoteNtf.Parser);

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGoddessTrial.SelectTopicDifficultyRes, NotifySetDifficulty, CmdGoddessTrialSelectTopicDifficultyRes.Parser);

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGoddessTrial.TopicDiffiFirstRankRes, NotifyFristRank, CmdGoddessTrialTopicDiffiFirstRankRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGoddessTrial.GetTopicPropertyRes, NotifyTopicProperty, CmdGoddessTrialGetTopicPropertyRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGoddessTrial.InsSwitchStageRes, NotifyInsSwitchState, CmdGoddessTrialInsSwitchStageRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGoddessTrial.TopicUnlockNtf, NotifyTopicUnlockNtf, CmdGoddessTrialTopicUnlockNtf.Parser);

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGoddessTrial.GetTopicEndAwardRes, NotifyTopicEndAward, CmdGoddessTrialGetTopicEndAwardRes.Parser);

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGoddessTrial.InsStagePassAwardNtf, NotifyStagePassAward, CmdGoddessTrialInsStagePassAwardNtf.Parser);

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGoddessTrial.GetFirstTeamAwardRes, NotifyGetFristAward, CmdGoddessTrialGetFirstTeamAwardRes.Parser);



            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGoddessTrial.GetFirstRankRoleDetailInfoRes, NotifyGetRankRoleInfo, CmdGoddessTrialGetFirstRankRoleDetailInfoRes.Parser);


            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGoddessTrial.ResetInsAwardNtf, NotifyResetAwardNtf, CmdGoddessTrialResetInsAwardNtf.Parser);

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGoddessTrial.UpdateInsAwardNtf, NotifyUpdateInsAwardNtf, CmdGoddessTrialUpdateInsAwardNtf.Parser);


        }

        private void DestoryNet()
        {
            Sys_Instance.Instance.eventEmitter.Handle<uint>(Sys_Instance.EEvents.InstanceData, NotifyDataRefresh, false);
            Sys_Instance.Instance.eventEmitter.Handle<uint, uint, uint>(Sys_Instance.EEvents.PassStage, NotifyPassStage, false);
        }

        private bool isInInstance = false;
        public bool IsInstance()
        {
            return isInInstance;
        }

        /// <summary>
        /// 请求进入副本
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="stageId"></param>
        public void SendInstanceEnterReq(uint instanceId, uint stageId)
        {
            //DebugUtil.LogError("SendInstanceEnterReq " + instanceId + "  " + stageId);

            CmdInstanceEnterReq req = new CmdInstanceEnterReq();
            req.InstanceId = instanceId;
            req.StageId = stageId;
            NetClient.Instance.SendMessage((ushort)CmdInstance.EnterReq, req);
        }

        /// <summary>
        /// 请求获得章节第一个通关的队伍
        /// </summary>
        /// <param name="ID"></param>
        public void SendGetTopicFirstTeamReq(List<uint> ids)
        {
            CmdGoddessTrialGetTopicFirstTeamReq info = new CmdGoddessTrialGetTopicFirstTeamReq();

            info.Id.AddRange(ids);

            NetClient.Instance.SendMessage((ushort)CmdGoddessTrial.GetTopicFirstTeamReq, info);
        }

        /// <summary>
        /// 请求女神副本的基础数据
        /// </summary>
        public void SendInstanceReq()
        {
            Sys_Instance.Instance.InstanceDataReq(PLAYTYPE);
        }

        /// <summary>
        /// 获取队员章节的本周最高记录
        /// </summary>
        /// <param name="difficly"></param>
        /// <param name="id"></param>
        public void SendGetTeamMemsRecord(uint id)
        {
            CmdGoddessTrialGetTeamInsInfoOnVoteInInsReq info = new CmdGoddessTrialGetTeamInsInfoOnVoteInInsReq();

            info.Id = id;

            NetClient.Instance.SendMessage((ushort)CmdGoddessTrial.GetTeamInsInfoOnVoteInInsReq, info);
        }

        /// <summary>
        /// 队长发起关卡投票
        /// </summary>
        public void SendSelectStageStartVote()
        {
            if (Sys_Team.Instance.HaveTeam == false || Sys_Team.Instance.isCaptain() == false)
            {
                return;
            }
            CmdGoddessTrialSelectStageStartVoteReq info = new CmdGoddessTrialSelectStageStartVoteReq();

            NetClient.Instance.SendMessage((ushort)CmdGoddessTrial.SelectStageStartVoteReq, info);

           
        }

        /// <summary>
        /// 队伍成员选择关卡投票
        /// </summary>
        /// <param name="id"></param>
        public void SendSelectStageVote(uint id,uint index)
        {
            CmdGoddessTrialSelectStageVoteReq info = new CmdGoddessTrialSelectStageVoteReq();

            info.StageId = id;
            info.StageIndex = index;
            NetClient.Instance.SendMessage((ushort)CmdGoddessTrial.SelectStageVoteReq, info);
        }

        /// <summary>
        /// 跟随队长选择
        
        /// </summary>
        /// <param name="op">1 跟随，0 取消跟随</param>
        public void SendSelectStageFollowLeader(uint op)
        {
            CmdGoddessTrialVoteFollowLeaderReq info = new CmdGoddessTrialVoteFollowLeaderReq();

            info.Op = op;

            NetClient.Instance.SendMessage((ushort)CmdGoddessTrial.VoteFollowLeaderReq, info);
        }

        /// <summary>
        /// 设置困难度
        /// </summary>
        /// <param name="id">主题ID</param>
        public void SendSetDifficulty(uint id)
        {
            CmdGoddessTrialSelectTopicDifficultyReq info = new CmdGoddessTrialSelectTopicDifficultyReq();

            info.Id = id;

            NetClient.Instance.SendMessage((ushort)CmdGoddessTrial.SelectTopicDifficultyReq, info);
        }


        /// <summary>
        /// 排行榜，
        /// </summary>
        /// <param name="topicID">主题ID</param>
        public void SendFristRank(uint topicID)
        {
           // DebugUtil.LogError("send frist rank : " + topicID.ToString());
            CmdGoddessTrialTopicDiffiFirstRankReq info = new CmdGoddessTrialTopicDiffiFirstRankReq();

            info.Id = topicID;

            NetClient.Instance.SendMessage((ushort)CmdGoddessTrial.TopicDiffiFirstRankReq, info);
        }

        /// <summary>
        /// 请求当前主题类型的特性
        /// </summary>
        public void SendTopicProperty()
        {
            if (TopicTypeID == 0)
            {
                //DebugUtil.LogError(" send topic property fail because topic type id is 0");
                return;
            }
                
            CmdGoddessTrialGetTopicPropertyReq info = new CmdGoddessTrialGetTopicPropertyReq();

            info.TopicId = TopicTypeID;

            NetClient.Instance.SendMessage((ushort)CmdGoddessTrial.GetTopicPropertyReq, info);
        }

        /// <summary>
        /// 请求切关卡
        /// </summary>
        public void SendSwitchStage()
        {
            CmdGoddessTrialInsSwitchStageReq info = new CmdGoddessTrialInsSwitchStageReq();

            NetClient.Instance.SendMessage((ushort)CmdGoddessTrial.InsSwitchStageReq, info);

            //DebugUtil.LogError("send switch stage ++++");
        }

        /// <summary>
        /// 请求获取副本收集结局奖励
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="instanceCount"></param>
        public void SendGetEndCollectAll(uint topic, uint instanceCount)
        {
            CmdGoddessTrialGetTopicEndAwardReq info = new CmdGoddessTrialGetTopicEndAwardReq();

            info.TopicId = topic;

            info.EndNum = instanceCount;

            NetClient.Instance.SendMessage((ushort)CmdGoddessTrial.GetTopicEndAwardReq, info);
        }

        public void SendGetFristCrossReward(uint id)
        {
            CmdGoddessTrialGetFirstTeamAwardReq info = new CmdGoddessTrialGetFirstTeamAwardReq();
            info.Id = id;
            NetClient.Instance.SendMessage((ushort)CmdGoddessTrial.GetFirstTeamAwardReq, info);
        }

        public void SendGetRankRoleInfo(uint id,ulong roleid)
        {
            CmdGoddessTrialGetFirstRankRoleDetailInfoReq info = new CmdGoddessTrialGetFirstRankRoleDetailInfoReq();
            info.Id = id;
            info.RoleId = roleid;
            NetClient.Instance.SendMessage((ushort)CmdGoddessTrial.GetFirstRankRoleDetailInfoReq, info);
        }
    }

    public partial class Sys_GoddnessTrial : SystemModuleBase<Sys_GoddnessTrial>
    {

        /// <summary>
        /// 女神炼狱副本的基本数据
        /// </summary>
        /// <param name="insType"></param>
        private void NotifyDataRefresh(uint insType)
        {
            if (insType != (uint)InsType.GoddessTrial)
                return;
           // DebugUtil.LogError("Goddness instance data refresh");
            //  DebugUtil.LogError("doddness trial data refresh +++++");

            RefreshRandomTopicList();

            RefreshCurTopicLevel(Sys_Role.Instance.Role.Level);

            RefreshDefaultConfig();

            eventEmitter.Trigger(EEvents.RefrshData);

        }
        /// <summary>
        /// 获取主题第一个通关的队伍
        /// </summary>
        /// <param name="msg"></param>
        private void NotifyTopicFristRes(NetMsg msg)
        {
            CmdGoddessTrialGetTopicFirstTeamRes res = NetMsgUtil.Deserialize<CmdGoddessTrialGetTopicFirstTeamRes>(CmdGoddessTrialGetTopicFirstTeamRes.Parser, msg);

            FristCrossInfo = res;

            eventEmitter.Trigger(EEvents.FristCrossInfo);
        }
        /// <summary>
        /// 获取队伍成员章节的本周记录
        /// </summary>
        /// <param name="msg"></param>
        private void NotifyTeamRecordRes(NetMsg msg)
        {
            CmdGoddessTrialGetTeamInsInfoOnVoteInInsRes res = NetMsgUtil.Deserialize<CmdGoddessTrialGetTeamInsInfoOnVoteInInsRes>(CmdGoddessTrialGetTeamInsInfoOnVoteInInsRes.Parser, msg);

            TeamMemsRecord = res.TeamMemInsData;

            eventEmitter.Trigger(EEvents.TeamMemsRecord);

        }

     

        /// <summary>
        /// 首次通关主题排行
        /// </summary>
        /// <param name="msg"></param>
        private void NotifyFristRank(NetMsg msg)
        {
            CmdGoddessTrialTopicDiffiFirstRankRes info = NetMsgUtil.Deserialize<CmdGoddessTrialTopicDiffiFirstRankRes>(CmdGoddessTrialTopicDiffiFirstRankRes.Parser, msg);

            Rank = info.TopicDiffiRank;

            eventEmitter.Trigger(EEvents.TopicRank);
            
        }
        /// <summary>
        /// 关卡通过通知
        /// </summary>
        /// <param name="playtype"></param>
        /// <param name="instance"></param>
        /// <param name="stageid"></param>
        private void NotifyPassStage(uint playtype, uint instance, uint stageid)
        {
            if (playtype != PLAYTYPE)
                return;

            //DebugUtil.LogError("Notify pass stage +++++++ instance " + instance.ToString() + "------stageid " + stageid.ToString());

            var statedata = CSVGoddessSelect.Instance.GetConfData(stageid);

            int count = statedata.StatgeCount();

            //切关卡前，需要判端是否需要投票
            if (count > 1)
            {
                if (Sys_Team.Instance.isCaptain())
                    SendSelectStageStartVote();

                m_SelectVoteState = 1;
            }
            else
            {
                m_SelectVoteState = 0;
            }

        }

        private uint mWaitSwitchStage = 0;
        /// <summary>
        /// 关卡切换通知
        /// </summary>
        /// <param name="playtype"></param>
        /// <param name="instance"></param>
        /// <param name="nowstage"></param>
        /// <param name="nextstage"></param>
        private void NotifySwitchStage(uint playtype, uint instance, uint nowstage, uint nextstage)
        {
            if (playtype != PLAYTYPE)
                return;
   
            mWaitSwitchStage = 1;

            SelectStageID = nextstage;

            //关卡开始前，显示关卡介绍

            UIManager.OpenUI(EUIID.UI_Goddess_Select, false, new UI_Goddness_Select_Parma() { State = 0, InstanceID = instance, LevelID = nextstage });
        }

        /// <summary>
        /// 地图加载完成
        /// </summary>
        private void NotifyMapLoadOk()
        {
            ///通关章节，自动选择下一关
            if (m_IsWaitShowMainUI)
            {
                var topicdata =  CSVGoddessTopic.Instance.GetConfData(m_LastTopic);

                int index = topicdata.InstanceId.FindIndex(o => o == m_LastInstanceID);

                if (index >= 0 && index < topicdata.InstanceId.Count-1)
                {
                    UI_GoddnessTrial_Parma uiparma = new UI_GoddnessTrial_Parma();

                    uiparma.instanceID = topicdata.InstanceId[index + 1];
                    uiparma.instanceIndex = index + 1;
                    uiparma.TopicID = m_LastTopic;

                    uiparma.isShowDetail = true;

                    UIManager.OpenUI(EUIID.UI_GoddessTrial, false, uiparma);
                }

            }

            m_IsWaitShowMainUI = false;

           // DebugUtil.LogError("notify map load ok ++++");

            if (mWaitSwitchStage == 0)
                return;

            mWaitSwitchStage = 0;

           var dailydata = CSVInstanceDaily.Instance.GetConfData(SelectStageID);

            if (dailydata == null)
                return;

            isInInstance = true;

            var isFristLevel = isFristStageInstance(SelectInstance, SelectStageID);
            if(isFristLevel)
              UIManager.OpenUI(EUIID.UI_Goddess_Select, false, new UI_Goddness_Select_Parma() { State = 0, InstanceID = SelectInstance,LevelID = SelectStageID});
        }

        /// <summary>
        /// 副本结束
        /// </summary>
        /// <param name="isPass"></param>
        /// <param name="instanceID"></param>
        private void NotityInstanceEnd(bool isPass, uint instanceID)
        {
           // DebugUtil.LogError("instance end ++++ instance id : " + instanceID.ToString() + " is pass : " + isPass.ToString());

            if (instanceID != SelectInstance)
                return;

            mWaitSwitchStage = 0;

            if (!isPass)
                return;

            //当副本通关时，会有两种情况，一种是普通的副本结束，显示通关结算奖励界面，第二种是 故事的结局 需要显示故事结局的界面
            var data =  CSVInstance.Instance.GetConfData(instanceID);

            if (data == null)
                return;

            var topicdata = CSVGoddessTopic.Instance.GetConfData(SelectID);

            //if (topicdata == null)
            //{
            //    DebugUtil.LogError(" goddness trial  instance end , not find topic config data ");
            //}

            if (topicdata != null)
            {
                int index = topicdata.InstanceId.FindIndex(o => o == instanceID);

                if (index >= 0 && index == (topicdata.InstanceId.Count - 1))
                {
                    
                    UI_Goddness_Ending_Parma parmas = new UI_Goddness_Ending_Parma();

                    parmas.InstanceID = instanceID;
                    parmas.LeveleID = SelectStageID;
                    parmas.IsPass = isPass;
                    parmas.Topic = SelectID;

                    var enddata = GetEndData(SelectStageID);

                    parmas.EndId = enddata == null ? 0 : enddata.id;

                    UIManager.OpenUI(EUIID.UI_Goddess_Ending,false, parmas);

                    return;
                }
            }

            if (m_SelectVoteState < 1)
            {
                OpenResultUI(instanceID, isPass);
            }         
            else
            {
                WaitResultInstanceID = instanceID;
                WaitResultIsPass = isPass;

                IsWaitResult = true;
            }

            if (Sys_Team.Instance.isCaptain() && isPass)
            {
                m_IsWaitShowMainUI = true;
                m_LastInstanceID = instanceID;
                m_LastTopic = SelectID;
            }
        }

        public void OpenResultUI(uint instanceID,bool isPass)
        {
          
            IsWaitResult = false;
            UI_Goddness_Result_Parma parma = new UI_Goddness_Result_Parma();

            parma.InstanceID = instanceID;
            parma.LeveleID = SelectStageID;
            parma.IsPass = isPass;

            UIManager.OpenUI(EUIID.UI_Goddess_Result, false, parma);

        }
        private void NotifyStateNtf(uint instance, uint stage)
        {
            CSVGoddessTopic.Data data = null;

            if (GetTopicDataByInstanceID(instance, out data) < 0)
                return;

            SelectID = data.id;
            SelectInstance = instance;
            SelectStageID = stage;

            isInInstance = true;
        }


        private void NotifyInstanceExit()
        {
            isInInstance = false;


            if ( UIManager.IsOpen(EUIID.UI_Goddess_Select))
            {
                UIManager.CloseUI(EUIID.UI_Goddess_Select);
            }

            if (InstancePassAward != null)
            {
                InstancePassAward = null;
            }
        }
        private void NotifyTopicProperty(NetMsg msg)
        {
            CmdGoddessTrialGetTopicPropertyRes info = NetMsgUtil.Deserialize<CmdGoddessTrialGetTopicPropertyRes>(CmdGoddessTrialGetTopicPropertyRes.Parser, msg);

            TopicProperty = info.InsAndAISeq;

            eventEmitter.Trigger(EEvents.TopicProperty);
        }

        private void NotifyInsSwitchState(NetMsg msg)
        {
            CmdGoddessTrialInsSwitchStageRes info = NetMsgUtil.Deserialize<CmdGoddessTrialInsSwitchStageRes>(CmdGoddessTrialInsSwitchStageRes.Parser, msg);
        }

        private void NotifyTopicUnlockNtf(NetMsg msg)
        {
            if (GTData == null)
                return;

            CmdGoddessTrialTopicUnlockNtf info = NetMsgUtil.Deserialize<CmdGoddessTrialTopicUnlockNtf>(CmdGoddessTrialInsSwitchStageRes.Parser, msg);

            GTData.Id.Add(info.Id);

            MaxDifficlyID = GetMaxDifficult();

            eventEmitter.Trigger(EEvents.TopicUnlock);
        }

        private void NotifyTopicEndAward(NetMsg msg)
        {
            CmdGoddessTrialGetTopicEndAwardRes info = NetMsgUtil.Deserialize<CmdGoddessTrialGetTopicEndAwardRes>(CmdGoddessTrialGetTopicEndAwardRes.Parser, msg);

            var data = GTData.EndTopic.Find(o => o.TopicId == info.TopicId);

            if (data != null)
            {
                data.GivedMaxRec = info.EndNum;
            }

            eventEmitter.Trigger(EEvents.GetTopicEndAward);
        }


        private void NotifyStagePassAward(NetMsg msg)
        {
            CmdGoddessTrialInsStagePassAwardNtf info = NetMsgUtil.Deserialize<CmdGoddessTrialInsStagePassAwardNtf>(CmdGoddessTrialInsStagePassAwardNtf.Parser, msg);

            InstancePassAward = info;

        }

        private void NotifyGetFristAward(NetMsg msg)
        {
            CmdGoddessTrialGetFirstTeamAwardRes info = NetMsgUtil.Deserialize<CmdGoddessTrialGetFirstTeamAwardRes>(CmdGoddessTrialGetFirstTeamAwardRes.Parser, msg);

            var record = FristCrossInfo.Team.Find(o => o.Id == info.Id);

            if (record != null)
            {
                record.RolesGetedAward.Add(Sys_Role.Instance.RoleId);

                eventEmitter.Trigger(EEvents.FristCrossInfo);
            }

            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2022386u));
        }

        private void NotifyGetRankRoleInfo(NetMsg msg)
        {
            CmdGoddessTrialGetFirstRankRoleDetailInfoRes info = NetMsgUtil.Deserialize<CmdGoddessTrialGetFirstRankRoleDetailInfoRes>(CmdGoddessTrialGetFirstRankRoleDetailInfoRes.Parser, msg);


            var result = Rank.RankList.Find(o => o.RoleId == info.RoleId);

            if (result == null)
                return;

            UI_Rank_DetailParam rankDetail = new UI_Rank_DetailParam();
            rankDetail.showType = ERankDetailType.Player;

            RankDescRole rankDescRole = new RankDescRole();
            rankDescRole.Name = result.RoleName;
            rankDescRole.RoleId = info.RoleId;
            rankDescRole.RoleScore = result.RoleScore;
            rankDescRole.TotalScore = result.Score;
            rankDescRole.Fashions = info.DressFashionList;
            rankDescRole.HeroId = info.HeroId;
            rankDetail.rankDescRole = rankDescRole;

            UIManager.OpenUI(EUIID.UI_Rank_Detail01, false, rankDetail);
        }

        private void NotifyResetAwardNtf(NetMsg msg)
        {
            CmdGoddessTrialResetInsAwardNtf info = NetMsgUtil.Deserialize<CmdGoddessTrialResetInsAwardNtf>(CmdGoddessTrialResetInsAwardNtf.Parser, msg);

            if (GTData != null)
            {
                GTData.Awards.Clear();
            }

            if (UpdateInsAwardNtf != null)
            {
                UpdateInsAwardNtf = null;
            }

          //  DebugUtil.LogError("msg CmdGoddessTrialResetInsAwardNtf ， Awards :" + GTData.Awards.Count);
        }

        private void NotifyUpdateInsAwardNtf(NetMsg msg)
        {
            CmdGoddessTrialUpdateInsAwardNtf info = NetMsgUtil.Deserialize<CmdGoddessTrialUpdateInsAwardNtf>(CmdGoddessTrialUpdateInsAwardNtf.Parser, msg);

            UpdateInsAwardNtf = info;
        }
    }
}
