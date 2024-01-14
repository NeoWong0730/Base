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
        public object CurMessage { get; private set; }

        #region 通用副本
        #region 发送消息
        /// <summary>
        /// 请求副本玩法数据
        /// </summary>
        /// <param name="playType"></param>
        public void InstanceDataReq(uint playType)
        {
            uint insType = 0;
            dict_CheckActivityToInstanceType.TryGetValue(playType, out insType);
            CmdInstanceDataReq req = new CmdInstanceDataReq();
            req.InsType = insType;
            req.PlayType = playType;
            NetClient.Instance.SendMessage((ushort)CmdInstance.DataReq, req);
        }
        /// <summary>
        /// 请求进入副本
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="stageId"></param>
        public void InstanceEnterReq(uint instanceId, uint stageId)
        {
            Sys_Task.currentTabType = Sys_Task.ETabType.Dungon;

            CmdInstanceEnterReq req = new CmdInstanceEnterReq();
            req.InstanceId = instanceId;
            req.StageId = stageId;
            NetClient.Instance.SendMessage((ushort)CmdInstance.EnterReq, req);
        }
        /// <summary>
        /// 请求退出副本
        /// </summary>
        public void InstanceExitReq()
        {
            if (IsInInstance == false)
                return;

            CmdInstanceExitReq req = new CmdInstanceExitReq();
            NetClient.Instance.SendMessage((ushort)CmdInstance.ExitReq, req);
        }
        #endregion
        #region 接收消息  
        /// <summary>
        /// 副本玩法数据
        /// </summary>
        private void OnInstanceDataRes(NetMsg msg)
        {
            CmdInstanceDataRes res = NetMsgUtil.Deserialize<CmdInstanceDataRes>(CmdInstanceDataRes.Parser, msg);
            ServerInstanceData serverInstanceData;
            if (dict_ServerInstanceData.TryGetValue(res.CommonData.PlayType, out serverInstanceData))
            {
                dict_ServerInstanceData[res.CommonData.PlayType].instanceCommonData = res.CommonData;
                dict_ServerInstanceData[res.CommonData.PlayType].instancePlayTypeData = res.PlayTypeData;
            }
            else
            {
                serverInstanceData = new ServerInstanceData();
                serverInstanceData.playType = res.CommonData.PlayType;
                serverInstanceData.insType = res.CommonData.InsType;
                serverInstanceData.instanceCommonData = res.CommonData;
                serverInstanceData.instancePlayTypeData = res.PlayTypeData;
                dict_ServerInstanceData.Add(res.CommonData.PlayType, serverInstanceData);
            }
            Sys_Instance.Instance.eventEmitter.Trigger<uint>(EEvents.InstanceData, res.InsType);
            
            Sys_Instance.Instance.eventEmitter.Trigger(EEvents.InstanceDataUpdate);
        }
        /// <summary>
        /// 副本数据
        /// </summary>
        /// <param name="msg"></param>
        private void OnInstanceDataNtf(NetMsg msg)
        {
            dict_ServerInstanceData.Clear();
            CmdInstanceDataNtf res = NetMsgUtil.Deserialize<CmdInstanceDataNtf>(CmdInstanceDataNtf.Parser, msg);
            foreach (var item in res.CommonDatas)
            {
                ServerInstanceData serverInstanceData = new ServerInstanceData();
                serverInstanceData.playType = item.PlayType;
                serverInstanceData.insType = item.InsType;
                serverInstanceData.instanceCommonData = item;
                dict_ServerInstanceData.Add(item.PlayType, serverInstanceData);
            }
            foreach (var item in res.PlayTypeData)
            {
                ServerInstanceData serverInstanceData = null;
                if (dict_ServerInstanceData.TryGetValue(item.PlayType, out serverInstanceData))
                {
                    serverInstanceData.instancePlayTypeData = item;
                }

                switch ((InsType)item.InsType)
                {
                    case InsType.Terror:
                        {
                            Sys_TerrorSeries.Instance.OnUpdateInsData(item.PlayType, item.TerrorInsData);
                        }
                        break;
                    case InsType.Tower:
                        {
                            Sys_HundredPeopleArea.Instance.towerInsData = item.TowerData;
                            Sys_HundredPeopleArea.Instance.lastDailyRewardTime = item.TowerData.LastDailyRewardTime; 
                            Sys_HundredPeopleArea.Instance.times = item.TowerData.PlayTimesLimit.UsedTimes;
                        }
                        break;

                }
            }

            Sys_Instance.Instance.eventEmitter.Trigger(EEvents.InstanceDataUpdate);
            Sys_Instance.Instance.eventEmitter.Trigger(EEvents.InstanceDataAll);
            
        }
        /// <summary>
        /// 副本数据更新
        /// </summary>
        /// <param name="msg"></param>
        private void OnInstanceDataUpdateNtf(NetMsg msg)
        {
            CmdInstanceDataUpdateNtf res = NetMsgUtil.Deserialize<CmdInstanceDataUpdateNtf>(CmdInstanceDataUpdateNtf.Parser, msg);

            foreach (var item in res.CommonData)
            {
                ServerInstanceData serverInstanceData = null;
                if (dict_ServerInstanceData.TryGetValue(item.PlayType, out serverInstanceData))
                {
                    serverInstanceData.instanceCommonData = item;
                }
                else
                {
                    serverInstanceData = new ServerInstanceData();
                    serverInstanceData.playType = item.PlayType;
                    serverInstanceData.insType = item.InsType;
                    serverInstanceData.instanceCommonData = item;
                    dict_ServerInstanceData.Add(serverInstanceData.playType, serverInstanceData);
                }
            }

            foreach (var item in res.PlayTypeData)
            {
                ServerInstanceData serverInstanceData = null;
                if (dict_ServerInstanceData.TryGetValue(item.PlayType, out serverInstanceData))
                {
                    serverInstanceData.instancePlayTypeData = item;
                }


                switch ((InsType)item.InsType)
                {
                    case InsType.Terror:
                        {
                            Sys_TerrorSeries.Instance.OnUpdateInsData(item.PlayType, item.TerrorInsData);
                        }
                        break;
                    case InsType.GoddessTrial:
                        {
                            Sys_Instance.Instance.eventEmitter.Trigger<uint>(EEvents.InstanceData, item.InsType);
                        }
                        break;
                    case InsType.Tower:
                        {
                            Sys_HundredPeopleArea.Instance.towerInsData = item.TowerData;
                            Sys_HundredPeopleArea.Instance.lastDailyRewardTime = item.TowerData.LastDailyRewardTime;
                            Sys_HundredPeopleArea.Instance.times = item.TowerData.PlayTimesLimit.UsedTimes;
                        }
                        break;
                }
            }
            Sys_Instance.Instance.eventEmitter.Trigger(EEvents.InstanceDataUpdate);
        }
        /// <summary>
        /// 副本次数限制更新
        /// </summary>
        /// <param name="msg"></param>
        private void OnInstanceResLimitNtf(NetMsg msg)
        {
            CmdInstanceResLimitNtf res = NetMsgUtil.Deserialize<CmdInstanceResLimitNtf>(CmdInstanceResLimitNtf.Parser, msg);
            ServerInstanceData serverInstanceData;
            if (dict_ServerInstanceData.TryGetValue(res.PlayType, out serverInstanceData))
            {
                serverInstanceData.instanceCommonData.ResLimit = res.ResLimit;
            }
        }
        /// <summary>
        /// 进入副本通知
        /// </summary>
        /// <param name="msg"></param>
        private void OnInstanceEnterNtf(NetMsg msg)
        {
            CmdInstanceEnterNtf res = NetMsgUtil.Deserialize<CmdInstanceEnterNtf>(CmdInstanceEnterNtf.Parser, msg);

            curInstance.InstanceId = res.InstanceId;
            curInstance.StageID = res.StageId;

            CurInstancID = res.InstanceId;

            Sys_Instance.Instance.eventEmitter.Trigger(EEvents.InstanceEnter);

            if (UIManager.IsOpen(EUIID.UI_Onedungeons))
                UIManager.CloseUI(EUIID.UI_Onedungeons);

            if (UIManager.IsOpen(EUIID.UI_Multi_Ready))
                UIManager.CloseUI(EUIID.UI_Multi_Ready);

            if (UIManager.IsOpen(EUIID.UI_Multi_PlayType))
                UIManager.CloseUI(EUIID.UI_Multi_PlayType);

            var data = CSVInstance.Instance.GetConfData(curInstance.InstanceId);

            /*if (data != null && data.PlayType == BioInstanceID)
            {
                Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.Event35);
            }*/
        }
        /// <summary>
        /// 副本结束通知
        /// </summary>
        /// <param name="msg"></param>
        private void OnInstanceEndNtf(NetMsg msg)
        {
            CmdInstanceEndNtf res = NetMsgUtil.Deserialize<CmdInstanceEndNtf>(CmdInstanceEndNtf.Parser, msg);

            if (isManyDungeons && res.Passed)
            {
                UIManager.OpenUI(EUIID.UI_Instance_Reuslt, false, 0);
            }

            var data = CSVInstance.Instance.GetConfData(curInstance.InstanceId);

            if (data.Type == 3) //恐怖旅团
            {
                UIManager.OpenUI(EUIID.UI_Instance_Reuslt, false, 0);

                //魔力宝典
                //Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.Event42);
            }

            //DebugUtil.LogError("instance_common get endntf netmsg");
            //if (data.Type == 6)
            //{
            //    int mode = res.Passed ? 1 : 2;

            //    UIManager.OpenUI(EUIID.UI_Bio_Result, false, new UI_Multi_ResultNew_Parma() { InstanceID = curInstance.InstanceId, Mode = mode, isFristCorss = false});
            //}
            eventEmitter.Trigger<bool,uint>(EEvents.InstanceEnd, res.Passed,curInstance.InstanceId);
        }
        /// <summary>
        /// 退出副本通知
        /// </summary>
        /// <param name="msg"></param>
        private void OnInstanceExitNtf(NetMsg msg)
        {
            CmdInstanceExitNtf res = NetMsgUtil.Deserialize<CmdInstanceExitNtf>(CmdInstanceExitNtf.Parser, msg);
            curInstance.Reset();
            Sys_Instance.Instance.eventEmitter.Trigger(EEvents.InstanceExit);

            if (UIManager.IsOpen(EUIID.UI_Instance_Reuslt))
            {
                UIManager.CloseUI(EUIID.UI_Instance_Reuslt);
            }

            Sys_Map.Instance.eventEmitter.Trigger(Sys_Map.EEvents.OnInterruptPathFind);
        }
        /// <summary>
        /// 通关关卡
        /// </summary>
        /// <param name="msg"></param>
        private void OnInstancePassStageNtf(NetMsg msg)
        {
            CmdInstancePassStageNtf res = NetMsgUtil.Deserialize<CmdInstancePassStageNtf>(CmdInstancePassStageNtf.Parser, msg);

            CurMessage = res;

            eventEmitter.Trigger<uint, uint, uint>(EEvents.PassStage, res.PlayType, res.InstanceId, res.StageId);

            switch (res.PlayType)
            {
                case 1://单人副本
                    //Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.Event26);
                    break;
            }

            ServerInstanceData serverInstanceData;

            if (!dict_ServerInstanceData.TryGetValue(res.PlayType, out serverInstanceData))
                return;

            InsEntry insEntry = serverInstanceData.GetInsEntry(res.InstanceId);

            if (null == insEntry)
                return;

            if (insEntry.PerMaxStageId <= res.StageId)
            {
                insEntry.PerMaxStageId = res.StageId;
            }

            if (insEntry.HistoryStageId < res.StageId)
                insEntry.HistoryStageId = res.StageId;

            CurMessage = null;
        }
        /// <summary>
        /// 切换到下一关
        /// </summary>
        /// <param name="msg"></param>
        private void OnInstanceSwitchStageNtf(NetMsg msg)
        {
            CmdInstanceSwitchStageNtf res = NetMsgUtil.Deserialize<CmdInstanceSwitchStageNtf>(CmdInstanceSwitchStageNtf.Parser, msg);

            if (curInstance.InstanceId == res.InstanceId)// 更新关卡，不做更新副本的操作
                curInstance.StageID = res.NextStageId;

            eventEmitter.Trigger<uint, uint, uint,uint>(EEvents.SwitchStage, res.PlayType, res.InstanceId, res.NowStageId,res.NextStageId);
        }
        /// <summary>
        /// 登录，重连通知是否在副本中
        /// </summary>
        /// <param name="msg"></param>
        private void OnInstanceStateNtf(NetMsg msg)
        {
            CmdInstanceStateNtf res = NetMsgUtil.Deserialize<CmdInstanceStateNtf>(CmdInstanceStateNtf.Parser, msg);
            if(res.InInsatnce)
            {
                curInstance.InstanceId = res.InstanceId;
                curInstance.StageID = res.StageId;

                eventEmitter.Trigger<uint, uint>(EEvents.StateNtf, res.InstanceId, res.StageId);
            }
            else
            {
                curInstance.Reset();
            }
        }
        #endregion
        #endregion


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

  
}
