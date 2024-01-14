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
    public partial class Sys_Instance : SystemModuleBase<Sys_Instance>
    {
        #region 多人副本

        public const uint ManyInstanceID = 20u;
        public const uint BioInstanceID = 130u;
        public ulong VoteID { get; set; }

        /// <summary>
        /// 当前副本
        /// </summary>
        public uint CurInstancID { get; set; }

        public uint CurStageID { get; set; }

        /// <summary>
        /// 当前人物传记系列ID,用于记录当前选择
        /// </summary>
        public uint CurSeriesID { get; set; }
        /// <summary>
        /// 多人副本标记,进入副本后有效
        /// </summary>
        public bool isManyDungeons
        {
            get
            {

                var data = CSVInstance.Instance.GetConfData(curInstance.InstanceId);

                if (data == null)
                    return false;

                return data.Type == 2;
            }
        }

        private TeamInstanceProgress mTeamInstanceProgess;


        /// <summary>
        /// 发起多人副本准备投票通知
        /// </summary>
        /// <param name="msg"></param>
        void OnNotify_PlayerConfirmPush(CmdRoleStartVoteNtf info)
        {
            // CmdInstancePlayerConfirmPush info = NetMsgUtil.Deserialize<CmdInstancePlayerConfirmPush>(CmdInstancePlayerConfirmPush.Parser, msg);

            var lastVoteID = VoteID;

            VoteID = info.VoteId;

            //InstanceStartVoteNtf data = NetMsgUtil.Deserialize<InstanceStartVoteNtf>(InstanceStartVoteNtf.Parser, info.ClientData.ToByteArray());
            NetMsgUtil.TryDeserialize<InstanceStartVoteNtf>(InstanceStartVoteNtf.Parser, info.ClientData.ToByteArray(), out InstanceStartVoteNtf data);

            mTeamInstanceProgess = data.Progress;

            CurInstancID = data.Data.InstanceID;
            CurStageID = data.Data.StageID;

          
            eventEmitter.Trigger(EEvents.StartVote);


            UIManager.CloseUI(EUIID.UI_Multi_PlayType);

            UIManager.CloseUI(EUIID.UI_Multi_Info);





            UIManager.OpenUI(EUIID.UI_Multi_Ready, false, 2);


        }

        /// <summary>
        /// 队员投票通知
        /// </summary>
        /// <param name="msg"></param>
        void OnNotify_PlayerConfirmNtf(CmdRoleDoVoteNtf info)
        {

            if (info.VoteId != VoteID)
                return;

            eventEmitter.Trigger<ulong, int>(EEvents.PlayerConfirmNtf, info.RoleId, (int)info.Op);
        }

        /// <summary>
        /// 队员投票结果
        /// </summary>
        /// <param name="msg"></param>
        void OnNotify_PlayerConfirmEnd(CmdRoleVoteEndNtf info)
        {

            if (info.VoteId != VoteID)
                return;

            if (info.ResultType == 2)
                UIManager.CloseUI(EUIID.UI_Multi_Ready);

            VoteID = 0;
        }
        void OnNotify_Vote(Sys_Vote.EVote eVote, object message)
        {
            if (eVote == Sys_Vote.EVote.Start)
                OnNotify_PlayerConfirmPush(message as CmdRoleStartVoteNtf);

            if (eVote == Sys_Vote.EVote.DoVote)
                OnNotify_PlayerConfirmNtf(message as CmdRoleDoVoteNtf);

            if (eVote == Sys_Vote.EVote.End)
                OnNotify_PlayerConfirmEnd(message as CmdRoleVoteEndNtf);
        }
        ///// <summary>
        ///// 解散通知
        ///// </summary>
        ///// <param name="msg"></param>
        //void OnNotify_PlayerConfirmDismiss(NetMsg msg)
        //{
        //    CmdInstancePlayerConfirmDismiss info = NetMsgUtil.Deserialize<CmdInstancePlayerConfirmDismiss>(CmdInstancePlayerConfirmDismiss.Parser, msg);

        //    UIManager.CloseUI(EUIID.UI_Multi_Ready);
        //}


        /// <summary>
        /// 查询成员副本进度结果
        /// </summary>
        /// <param name="msg"></param>
        void OnNotify_TeamMemsInstancePorcess(NetMsg msg)
        {
            CmdInstanceQueryTeamInstanceProgressRes info = NetMsgUtil.Deserialize<CmdInstanceQueryTeamInstanceProgressRes>(CmdInstanceQueryTeamInstanceProgressRes.Parser, msg);


            mTeamInstanceProgess = info.TeamProgress;


            eventEmitter.Trigger(EEvents.TeamInstanceProgress);

        }

        /// <summary>
        /// 锁定奖励通知
        /// </summary>
        /// <param name="msg"></param>
        void OnNotify_PlayTypeLockRewordNtf(NetMsg msg)
        {
            CmdInstancePlayTypeLockRewordNtf info = NetMsgUtil.Deserialize<CmdInstancePlayTypeLockRewordNtf>(CmdInstancePlayTypeLockRewordNtf.Parser, msg);

            var data = GetServerInstanceData(info.PlayType);

            if (data == null)
                return;

            data.instanceCommonData.LockedSelectedInstanceID = info.Locked;

        }

        /// <summary>
        /// 选择奖励通知
        /// </summary>
        /// <param name="msg"></param>
        void OnNotify_SelectInstanceIDRes(NetMsg msg)
        {
            CmdInstanceSelectInstanceIDRes info = NetMsgUtil.Deserialize<CmdInstanceSelectInstanceIDRes>(CmdInstanceSelectInstanceIDRes.Parser, msg);

            var data = GetServerInstanceData(info.PlayType);

            if (data == null)
                return;

            data.instanceCommonData.SelectedInstanceId = info.InstanceID;

            eventEmitter.Trigger<uint>(EEvents.RewardRefresh, info.InstanceID);
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

            // CmdInstancePlayerConfirmRes info = new CmdInstancePlayerConfirmRes();

            //info.Confirm = b;
            //info.VoidID = VoteID;

            //NetClient.Instance.SendMessage((ushort)CmdInstance.PlayerConfirmRes, info);
        }


        /// <summary>
        /// 放弃进入
        /// </summary>

        public void OnSendExitConfirmres()
        {
            if (VoteID == 0)
                return;

            Sys_Vote.Instance.Send_CancleVoteReq(VoteID);
        }
        /// <summary>
        /// 查询队伍成员副本进度
        /// </summary>
        /// <param name="b"></param>
        public void OnSendTeamMemsInstance(uint id)
        {
            //if (VoteID == 0)
            //    return;

            CmdInstanceQueryTeamInstanceProgressReq info = new CmdInstanceQueryTeamInstanceProgressReq();

            info.InstanceID = id;

            NetClient.Instance.SendMessage((ushort)CmdInstance.QueryTeamInstanceProgressReq, info);
        }

        /// <summary>
        /// 选择奖励
        /// </summary>
        /// <param name="id"></param>
        public void OnSelectInstanceIDReq(uint id)
        {
            //if (VoteID == 0)
            //    return;

            CmdInstanceSelectInstanceIDReq info = new CmdInstanceSelectInstanceIDReq();

            info.InstanceID = id;

            NetClient.Instance.SendMessage((ushort)CmdInstance.SelectInstanceIdreq, info);
        }


        // 人物传记系列 -》副本-》 章节 -》 关卡

        /// <summary>
        /// 获取人物传记系列CVS数据
        /// </summary>
        /// <param name="id">系列ID</param>
        /// <returns></returns>
        public CSVBiographySeries.Data getBIO(uint id)
        {
            return CSVBiographySeries.Instance.GetConfData(id);
        }


        /// <summary>
        /// 获取CSV 系列包含的副本
        /// </summary>
        /// <param name="id">系列ID</param>
        /// <returns></returns>
        public List<CSVInstance.Data>  getBIOCopy(uint id)
        {
            var bio = getBIO(id);

            if (bio == null)
                return null;

            int count = bio.instance_id.Count;

            var list = new List<CSVInstance.Data>();

            for (int i = 0; i < count; i++)
            {
                var data = CSVInstance.Instance.GetConfData(bio.instance_id[i]);

                if (data != null)
                    list.Add(data);
            }

            return list;
        }

        /// <summary>
        /// 根据副本ID 获取人物传记系列 根据人物传记章节组织的数据
        /// </summary>
        /// <param name="instanceID"></param>
        /// <returns></returns>
        public CSVBiographySeries.Data getSeries(uint instanceID)
        {
            var data = CSVBiographySeries.Instance.GetAll();

            CSVBiographySeries.Data value = null;

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
        /// <summary>
        /// 获取CSV 章节 根据人物传记章节组织的数据
        /// </summary>
        /// <param name="id">副本ID</param>
        /// <returns></returns>
        public List<CSBiographyChapter.Data> getChapter(uint id)
        {
            var data = CSBiographyChapter.Instance.GetAll();

            var list = new List<CSBiographyChapter.Data>();

            foreach (var item in data)
            {
                if (item.InstanceID == id)
                {
                    list.Add(item);
                }
            }

            list.Sort((x, y) =>
            {

                if (x.Sort < y.Sort)
                    return -1;
                if (x.Sort > y.Sort)
                    return 1;
                return 0;
            });
            return list;
        }

        /// <summary>
        /// 获取CSV 关卡   根据人物传记章节组织的数据
        /// </summary>
        /// <param name="id">章节ID</param>
        /// <returns></returns>

        public List<CSVInstanceDaily.Data>  getDaily(uint id)
        {
            var data = CSBiographyChapter.Instance.GetConfData(id);

            int count = data.Lv.Count;

            var list = new List<CSVInstanceDaily.Data>();

            for (int i = 0; i < count; i++)
            {
                var item = CSVInstanceDaily.Instance.GetConfData(data.Lv[i]);

                if (item != null)
                    list.Add(item);
            }

            return list;
        }

        /// <summary>
        /// 副本包含的所有关卡
        /// </summary>
        /// <param name="instanceID">副本ID</param>
        /// <returns></returns>
        public List<CSVInstanceDaily.Data>  getDailyByInstanceID(uint instanceID)
        {
            var datas = CSVInstanceDaily.Instance.GetAll();

            var list = new List<CSVInstanceDaily.Data>();

            foreach (var item in datas)
            {
                if (item.InstanceId == instanceID)
                {
                    list.Add(item);
                }
            }

            list.Sort((x, y) =>
            {
                if (x.LayerStage > y.LayerStage)
                    return 1;

                if (x.LayerStage < y.LayerStage)
                    return -1;

                if (x.Layerlevel > y.Layerlevel)
                    return 1;
                if (x.Layerlevel < y.Layerlevel)
                    return -1;

                return 0;
            });

            return list;
        }

        /// <summary>
        /// 根据关卡ID 获得章节,根据人物传记章节组织的数据
        /// </summary>
        /// <param name="ID">关卡ID</param>
        /// <returns></returns>
        public CSBiographyChapter.Data getBiographyChapterLocal(uint ID)
        {
            var data = CSVInstanceDaily.Instance.GetConfData(ID);

            var chapterDatas = CSBiographyChapter.Instance.GetAll();

            CSBiographyChapter.Data value = null;

            foreach (var item in chapterDatas)
            {
                if (item.Lv.Contains(ID))
                {
                    value = item;
                    break;
                }
            }

            return value;

        }
        /// <summary>
        /// 获取队员当前副本中的记录
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public RoleInstanceProgress getTeamMemProcess(ulong roleID)
        {
            var processValue = mTeamInstanceProgess.RoleProgress;

            foreach (var item in processValue)
            {
                if (item.RoleID == roleID)
                    return item;
            }

            return null;
        }

        /// <summary>
        /// 判断是否可以进入准备状态
        /// </summary>
        public bool DecisionToReady()
        {
            var processValue = mTeamInstanceProgess.RoleProgress;
            int count = processValue.Count;

            if (count == 0)
                return false;

            bool result = true;

            foreach (var item in processValue)
            {
                if (item.Unlocked == false)
                {
                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceID">副本ID</param>
        /// <param name="stateID">关卡ID</param>
        public uint GetNextEnterStageID(uint instanceID)
        {
            int minLevelFault = 99999999;

            uint stateID = 0;
            int minlevel = minLevelFault;

            var processValue = mTeamInstanceProgess.RoleProgress;
            int count = processValue.Count;


            var list = getDailyByInstanceID(instanceID);


            foreach (var item in processValue)
            {
                int index = 0;

                if (item.StageID != 0)
                    index = getStateIndex(instanceID, item.StageID, list);

                // index -= 1;

                if (index >= 0 && index < minlevel)
                    minlevel = index;
            }

            if (minlevel == minLevelFault)
                minlevel = -1;

            //跳关


            var data = CSVInstance.Instance.GetConfData(instanceID);

            int offsetvalue = minlevel - (int)(data.DeductionLayers);

            minlevel = Mathf.Max(0, offsetvalue);

            // var minStage = list.Find(o => o.Layerlevel == minlevel);

            if (minlevel < list.Count)
                stateID = list[minlevel].id;

            return stateID;

        }

        /// <summary>
        /// 获取副本的最小关卡
        /// </summary>
        /// <param name="instanceID">副本ID</param>
        /// <returns></returns>
        public uint GetFristStageID(uint instanceID)
        {
            uint stateID = 0;

            //var processValue = mTeamInstanceProgess.RoleProgress;
            //int count = processValue.Count;


            var list = getDailyByInstanceID(instanceID);

            stateID = list[0].id;

            return stateID;
        }
        public int getStateIndex(uint instanceID, uint stageID, List<CSVInstanceDaily.Data>  list)
        {
            var item = list.FindIndex(o => o.id == stageID);

            //if (item == null)
            //    return -1;

            //int index = (int)item.Layerlevel;

            return item;
        }

        public int getStateIndex(uint instanceID, uint stageID)
        {
            var list = getDailyByInstanceID(instanceID);

            int index = list.FindIndex(o => o.id == stageID);

            list.Clear();

            return index;
        }

        /// <summary>
        /// 获取多人副本玩法 和 玩法通用数据
        /// </summary>
        /// <returns></returns>
        public ServerInstanceData getMultiInstance()
        {
            var data = GetServerInstanceData(20);

            return data;
        }


        public InstanceCommonData getMultiInstanceCommon()
        {
            var data = getMultiInstance();

            if (data == null)
                return null;

            return data.instanceCommonData;
        }

        /// <summary>
        /// 获取关卡 记录
        /// </summary>
        /// <param name="instanceID"></param>
        /// <returns></returns>
        public InsEntry getMultiInsEntry(uint instanceID)
        {
            var data = getMultiInstanceCommon();

            if (data == null)
                return null;

            InsEntry Ie = null;

            int count = data.Entries.Count;

            for (int i = 0; i < count; i++)
            {
                if (data.Entries[i].InstanceId == instanceID)
                {
                    Ie = data.Entries[i];
                    break;
                }
            }

            return Ie;
        }

        /// <summary>
        /// 获取当前的记录关卡，当通关副本后会被置0
        /// </summary>
        /// <param name="instanceID"></param>
        /// <returns></returns>
        public uint getMultiStateID(uint instanceID)
        {
            var data = getMultiInsEntry(instanceID);

            if (data == null)
                return 0;

            return data.CurPassedStageId;
        }

        /// <summary>
        /// 获取最高记录关卡,周期后会被重置为0
        /// </summary>
        /// <param name="instanceID"></param>
        /// <returns></returns>
        public uint getMultiBestStateID(uint instanceID)
        {
            var data = getMultiInsEntry(instanceID);

            if (data == null)
                return 0;

            return data.PerMaxStageId;
        }

        /// <summary>
        /// 获取历史最高记录关卡
        /// </summary>
        /// <param name="instanceID"></param>
        /// <returns></returns>
        public uint getMultiBestHistoryStateID(uint instanceID)
        {
            var data = getMultiInsEntry(instanceID);

            if (data == null)
                return 0;

            return data.HistoryStageId;
        }

        public void getRewardProcess(uint instanceID, out int cur, out int total)
        {
            var datadaily = getDailyByInstanceID(instanceID);

            uint stageID = getMultiBestStateID(instanceID);

            int index = datadaily.FindIndex(o => o.id == stageID);

            cur = index + 1;
            total = datadaily.Count;
        }

        /// <summary>
        /// 进入 副本前 判断是否可进入，主要判断 是否为队长，队伍人数是否满足条件
        /// </summary>
        /// <param name="instanceID">副本ID</param>
        /// <returns></returns>
        public bool CheckTeamCondition(uint instanceID)
        {
            var data = CSVInstance.Instance.GetConfData(instanceID);

            if (Sys_Team.Instance.TeamMemsCount == 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2009639, data.limite_number.ToString()));
                return false;
            }


            if (Sys_Team.Instance.TeamMemsCount < data.limite_number)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2009640, data.limite_number.ToString()));
                return false;
            }

            if (Sys_Team.Instance.isCaptain() == false)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2009650));
                return false;
            }

            List<TeamMem> indexLeavelist = new List<TeamMem>();
            List<TeamMem> indexOffLinelist = new List<TeamMem>();
            List<TeamMem> indexLevellist = new List<TeamMem>();

            bool resutl = true;

            for (int i = 0; i < Sys_Team.Instance.TeamMemsCount; i++)
            {
                var teamMem = Sys_Team.Instance.getTeamMem(i);

                if (teamMem.IsLeave() || teamMem.IsOffLine())
                    indexLeavelist.Add(teamMem);

                if (teamMem.IsOffLine())
                    indexOffLinelist.Add(teamMem);

                if (teamMem.Level < data.lv[0] || teamMem.Level > data.lv[1])
                {
                    indexLevellist.Add(teamMem);

                }
            }

            int useCount = Sys_Team.Instance.TeamMemsCount - indexLeavelist.Count;

            if (useCount < data.limite_number)
            {
                for (int i = 0; i < indexLeavelist.Count; i++)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2009653, indexLeavelist[i].Name.ToStringUtf8(), data.limite_number.ToString()));
                }
                resutl = false;
            }

            if (resutl && indexLevellist.Count > 0)
            {
                for (int i = 0; i < indexLevellist.Count; i++)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2009652, indexLevellist[i].Name.ToStringUtf8()));
                }

                resutl = false;
            }

            indexLeavelist.Clear();
            indexOffLinelist.Clear();
            indexLevellist.Clear();

            return resutl;
        }
        #endregion
    }

    public partial class Sys_Instance : SystemModuleBase<Sys_Instance>, IDailyTimes
    {
        public int GetPlayTimes(uint dailyID)
        {
            int times = -1;

            var data = GetServerInstanceData(dailyID);

            if (data != null)
            {
                if(data.instancePlayTypeData.DailyIns.PlayTimesLimit.ExpireTime < Sys_Time.Instance.GetServerTime())
                {
                    times = 0;
                }
                else
                {
                    times = (int)data.instancePlayTypeData.DailyIns.PlayTimesLimit.UsedTimes;
                }
            }
            return times;
        }

        public int GetDailyTimes(uint dailyID)
        {
            int times = -1;

            var data = GetServerInstanceData(dailyID);

            if (data != null)
            {
               times = (int)data.instanceCommonData.ResLimit.UsedTimes;
            }

            return times;
        }

        public int GetDailyMaxTimes(uint dailyID)
        {
            int times = -1;

            var data = GetServerInstanceData(dailyID);

            if (data != null)
            {
                times = (int)data.instanceCommonData.ResLimit.MaxTimes;
            }

            return times;
        }
    }

}
