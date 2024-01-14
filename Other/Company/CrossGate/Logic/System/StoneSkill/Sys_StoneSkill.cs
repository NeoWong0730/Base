using Logic.Core;
using Packet;
using Lib.Core;
using Net;
using Table;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Logic
{
    public class StoneSkillData
    {
        public PowerStoneUnit powerStoneUnit;
        public int dataVerNum;
    }
    public class Sys_StoneSkill : SystemModuleBase<Sys_StoneSkill>
    {
        //服务器最新数据
        public Dictionary<uint, StoneSkillData> serverDataDic = new Dictionary<uint, StoneSkillData>();

        public List<uint> nextLevelLock = new List<uint>();
        
        public uint stoneUpgradeTimes;
        private bool showItem;
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        private NextRefreshTimer nextRefreshTimer;
        public uint allStage;
        public ResLimit levelUpCount;
        public enum EEvents
        {
            OnSkillActive,//激活后
            OnNonaSkillUpGa,//升级后
            UpgradeResultClose,//特定界面关闭后
            AdvancedEnd,//进阶后
            AdvancedEndUpWait,//进阶后等待播放动画刷新
            AdvancedResultClose,//特定界面关闭后
            ResolveStone,//晶石分解后
            ResolveStoneCerrer,//职业晶石降级-晶石分解后
            ReverseEnd,//逆转后
            FlyStarEnd,//表现结束
            ChaoSkillChange,//混沌技能改变
            EnergysparSpecilEvent,
            RefineTimeRefresh,
        }

        public override void Init()
        {
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateLevel, OnUpdateLevel, true);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdPowerStone.DataNtf, OnPoweStoneNtf, CmdPowerStoneDataNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPowerStone.ActivateReq, (ushort)CmdPowerStone.ActivateRes, OnActivateRes, CmdPowerStoneActivateRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPowerStone.LevelUpReq, (ushort)CmdPowerStone.LevelUpRes, OnStoneLevelUpRes, CmdPowerStoneLevelUpRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPowerStone.StageUpReq, (ushort)CmdPowerStone.StageUpRes, OnStoneStageUpRes, CmdPowerStoneStageUpRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPowerStone.ReverseReq, (ushort)CmdPowerStone.ReverseRes, OnStoneReverseRes, CmdPowerStoneReverseRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPowerStone.DecomposeReq, (ushort)CmdPowerStone.DecomposeRes, OnStoneDecomposeRes, CmdPowerStoneDecomposeRes.Parser);
        }
        public override void OnLogin()
        {

        }

        public override void OnLogout()
        {
            serverDataDic.Clear();
            nextLevelLock.Clear();
            nextRefreshTimer?.Cancel();
            base.OnLogout();
        }

        private void OnUpdateLevel()
        {
            if (!Sys_FunctionOpen.Instance.IsOpen(10801, false) || Sys_Role.Instance.Role.Career == 0)
                return;

            List<CSVStone.Data>  data = GetCareerStone(Sys_Role.Instance.Role.Career);
            uint currentLevel = Sys_Role.Instance.Role.Level;
            for (int i = 0; i < data.Count; i++)
            {
                if(currentLevel == data[i].level_limit)
                {
                    if (!nextLevelLock.Contains(currentLevel))
                    {
                        nextLevelLock.Add(data[i].id);
                    }
                }
            }
        }

        public void RemoveLevelUnLcokList(uint id)
        {
            if (nextLevelLock.Contains(id))
            {
                nextLevelLock.Remove(id);
            }           
        }

        private void OnPoweStoneNtf(NetMsg msg)
        {
            CmdPowerStoneDataNtf ntf = NetMsgUtil.Deserialize<CmdPowerStoneDataNtf>(CmdPowerStoneDataNtf.Parser, msg);
            allStage = ntf.AllStage;
            levelUpCount = ntf.LevelUpCount;
            if(null != nextRefreshTimer)
            {
                nextRefreshTimer.Reset(levelUpCount.ExpireTime, 3600f, ReturnSeverTime, OnTimeRefresh);
            }
            else
            {
                nextRefreshTimer = new NextRefreshTimer(levelUpCount.ExpireTime, 3600f, ReturnSeverTime, OnTimeRefresh);
            }
            serverDataDic.Clear();
            for (int i = 0; i < ntf.PowerStoneInfo.Count; i++)
            {
                PowerStoneUnit item = ntf.PowerStoneInfo[i];
                StoneSkillData tempSeverSkillData = new StoneSkillData();
                tempSeverSkillData.powerStoneUnit = item;
                tempSeverSkillData.dataVerNum = 0;
                serverDataDic.Add(item.Id, tempSeverSkillData);
            }            
        }

        private void OnTimeRefresh()
        {
            if(null != levelUpCount)
            {
                if(levelUpCount.ExpireTime <= Sys_Time.Instance.GetServerTime())
                {
                    levelUpCount.UsedTimes = 0;
                    eventEmitter.Trigger(EEvents.RefineTimeRefresh);                    
                }                
            }
        }

        private float ReturnSeverTime()
        {
            return Sys_Time.Instance.GetServerTime();
        }

        #region 激活
        public void OnPowerStoneActivateReq(uint stroneId)
        {
            CmdPowerStoneActivateReq activateReq = new CmdPowerStoneActivateReq();
            activateReq.Id = stroneId;
            NetClient.Instance.SendMessage((ushort)CmdPowerStone.ActivateReq, activateReq);
        }

        private void OnActivateRes(NetMsg msg)
        {
            CmdPowerStoneActivateRes data = NetMsgUtil.Deserialize<CmdPowerStoneActivateRes>(CmdPowerStoneActivateRes.Parser, msg);

            for (int i = 0; i < data.NewPowerStone.Count; i++)
            {
                PowerStoneUnit item = data.NewPowerStone[i];
                if (serverDataDic.ContainsKey(item.Id))
                {

                }
                else
                {
                    StoneSkillData tempSeverSkillData = new StoneSkillData();
                    tempSeverSkillData.powerStoneUnit = item;
                    tempSeverSkillData.dataVerNum = 0;
                    StoneSkillData tempClientSkillData = new StoneSkillData();
                    tempClientSkillData.powerStoneUnit = item;
                    tempClientSkillData.dataVerNum = 0;
                    serverDataDic.Add(item.Id, tempSeverSkillData);
                    eventEmitter.Trigger(EEvents.OnSkillActive, item.Id);
                    CSVStone.Data stoneData = CSVStone.Instance.GetConfData(item.Id);
                    if (null != stoneData)
                    {
                        /*if(stoneData.initial_level == 0)
                        {
                            Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.ActiveEnergysparSkill);
                        }*/
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2021073, LanguageHelper.GetTextContent(stoneData.stone_name)));
                    }
                }
            }            
        }
        #endregion

        #region 升级
        public void OnPowerStoneLevelUpReq(uint stroneId, uint itemId, uint itemNum)
        {
            showItem = false;
            if (itemId != 0)
            {
                showItem = true;
            }
            CmdPowerStoneLevelUpReq levelUpReq = new CmdPowerStoneLevelUpReq();
            levelUpReq.Id = stroneId;
            levelUpReq.ItemId = itemId;
            levelUpReq.ItemNumber = itemNum;
            NetClient.Instance.SendMessage((ushort)CmdPowerStone.LevelUpReq, levelUpReq);
        }

        private void OnStoneLevelUpRes(NetMsg msg)
        {
            CmdPowerStoneLevelUpRes data = NetMsgUtil.Deserialize<CmdPowerStoneLevelUpRes>(CmdPowerStoneLevelUpRes.Parser, msg);
            if (serverDataDic.ContainsKey(data.Id))
            {
                bool isUp = serverDataDic[data.Id].powerStoneUnit.Level != data.Level;
                serverDataDic[data.Id].powerStoneUnit.Level = data.Level;
                serverDataDic[data.Id].powerStoneUnit.Exp = data.Exp;
                serverDataDic[data.Id].dataVerNum += 1;

                if (isUp)
                {
                    UIManager.OpenUI(EUIID.UI_Upgrade_Result, false, data.Id);
                    if(showItem)
                    {
                        CSVItem.Data itemData = CSVItem.Instance.GetConfData(itemId);
                        if(null != itemData)
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2021069, stoneUpgradeTimes.ToString(), LanguageHelper.GetTextContent(itemData.name_id)));
                        }                     
                    }
                    else
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2021070, stoneUpgradeTimes.ToString()));
                    }
                }
            }
            //Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.EnergysparSkillUp);
            levelUpCount = data.LevelUpCount;
            eventEmitter.Trigger(EEvents.OnNonaSkillUpGa, data.Id);
        }
        #endregion

        #region 进阶
        public void OnPowerStoneStageUpReq(uint stroneId)
        {
            CmdPowerStoneStageUpReq stageUpReq = new CmdPowerStoneStageUpReq();
            stageUpReq.Id = stroneId;
            NetClient.Instance.SendMessage((ushort)CmdPowerStone.StageUpReq, stageUpReq);
        }

        private void OnStoneStageUpRes(NetMsg msg)
        {
            CmdPowerStoneStageUpRes data = NetMsgUtil.Deserialize<CmdPowerStoneStageUpRes>(CmdPowerStoneStageUpRes.Parser, msg);
            if (serverDataDic.ContainsKey(data.PowerStone.Id))
            {
                serverDataDic[data.PowerStone.Id].powerStoneUnit = data.PowerStone;

                serverDataDic[data.PowerStone.Id].dataVerNum += 1;

                UIManager.OpenUI(EUIID.UI_Advancedl_Result, false, data.PowerStone.Id);
            }            
            allStage = data.AllStage;

            for (int i = 0; i < data.StoneLevelUp.Count; i++)
            {
                StoneLevelUnit item = data.StoneLevelUp[i];
                if (serverDataDic.ContainsKey(item.Id))
                {
                    serverDataDic[item.Id].powerStoneUnit.Level = item.Level;
                    serverDataDic[item.Id].dataVerNum += 1;
                }
            }

            eventEmitter.Trigger(EEvents.AdvancedEndUpWait);
            eventEmitter.Trigger(EEvents.AdvancedEnd, data.PowerStone.Id);
        }
        #endregion

        #region 逆转
        public void OnPowerStoneReverseReq(uint stroneId, uint stage)
        {
            CmdPowerStoneReverseReq reverseReq = new CmdPowerStoneReverseReq();
            reverseReq.Id = stroneId;
            reverseReq.Stage = stage;
            NetClient.Instance.SendMessage((ushort)CmdPowerStone.ReverseReq, reverseReq);
        }

        private void OnStoneReverseRes(NetMsg msg)
        {
            CmdPowerStoneReverseRes data = NetMsgUtil.Deserialize<CmdPowerStoneReverseRes>(CmdPowerStoneReverseRes.Parser, msg);
            if (serverDataDic.ContainsKey(data.Id))
            {
                if (null != serverDataDic[data.Id].powerStoneUnit.StageSkill[(int)data.Stage - 1])
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2021076, LanguageHelper.GetTextContent(data.NewStageSkill.SkillType == 0 ? 2021048u : 2021049u)));
                    serverDataDic[data.Id].powerStoneUnit.StageSkill[(int)data.Stage - 1] = data.NewStageSkill;                   
                }

                if (serverDataDic[data.Id].powerStoneUnit.ChaosSkill != data.ChaosSkill)
                {
                    serverDataDic[data.Id].powerStoneUnit.ChaosSkill = data.ChaosSkill;
                    eventEmitter.Trigger(EEvents.ChaoSkillChange, data.Id);
                }
                serverDataDic[data.Id].dataVerNum += 1;                
                
                eventEmitter.Trigger(EEvents.ReverseEnd, data.Id, data.Stage);
            }                        
        }
        #endregion

        #region 分解
        public void OnPowerStoneDecomposeReq(uint stroneId)
        {
            CmdPowerStoneDecomposeReq decomposeReq = new CmdPowerStoneDecomposeReq();
            decomposeReq.Id = stroneId;
            NetClient.Instance.SendMessage((ushort)CmdPowerStone.DecomposeReq, decomposeReq);
        }

        private void OnStoneDecomposeRes(NetMsg msg)
        {
            CmdPowerStoneDecomposeRes data = NetMsgUtil.Deserialize<CmdPowerStoneDecomposeRes>(CmdPowerStoneDecomposeRes.Parser, msg);
            if (serverDataDic.ContainsKey(data.Id))
            {
                serverDataDic.Remove(data.Id);
                eventEmitter.Trigger(EEvents.ResolveStone, data.Id);
            }
            allStage = data.AllStage;

            for (int i = 0; i < data.StoneLevelUp.Count; i++)
            {
                StoneLevelUnit item = data.StoneLevelUp[i];
                if (serverDataDic.ContainsKey(item.Id))
                {
                    serverDataDic[item.Id].powerStoneUnit.Level = item.Level;
                    serverDataDic[item.Id].dataVerNum += 1;
                    eventEmitter.Trigger(EEvents.ResolveStoneCerrer, item.Id);
                }
            }
        }
        #endregion
        /// <summary>
        /// 获取晶石的主动技能，返回0时，晶石技能或为被动
        /// </summary>
        /// <param name="id"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public uint GetPowerStoneActiveSkill(uint id, uint level)
        {
            uint skillid = id * 1000 + level;
            CSVStoneLevel.Data stoneLevelData = CSVStoneLevel.Instance.GetConfData(skillid);
            if(null != stoneLevelData )
            {
                return stoneLevelData.activeskill;
            }
            return 0;
        }

        /// <summary>
        /// 获取晶石的主动技能，返回0时，为配置错误
        /// </summary>
        /// <param name="id"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public uint GetPowerStoneSkill(uint id, uint level)
        {
            uint skillid = id * 1000 + level;
            CSVStoneLevel.Data stoneLevelData = CSVStoneLevel.Instance.GetConfData(skillid);
            if (null != stoneLevelData)
            {
                return stoneLevelData.activeskill != 0 ? stoneLevelData.activeskill : stoneLevelData.passiveskill;
            }
            return 0;
        }
       
        public bool ChenckData(uint powerStoneId)
        {

            if(serverDataDic.ContainsKey(powerStoneId))
            {
                CSVStoneLevel.Data stoneLevelData = CSVStoneLevel.Instance.GetConfData(powerStoneId * 1000 + serverDataDic[powerStoneId].powerStoneUnit.Level);
                if(null != stoneLevelData)
                {
                    if(stoneLevelData.sum_stage == allStage)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public List<CSVStone.Data>  GetCareerStone(uint careerId)
        {
            List<CSVStone.Data>  playerCareerStoneList = new List<CSVStone.Data>();
            //Dictionary<uint, CSVStone.Data> allData = CSVStone.Instance.GetDictData();
            //List<CSVStone.Data>  dataList = new List<CSVStone.Data>(allData.Values);

            var dataList = CSVStone.Instance.GetAll();
            for (int i = 0; i < dataList.Count; i++)
            {
                CSVStone.Data stoneData = dataList[i];
                if (stoneData.career_limit == null)
                {
                    playerCareerStoneList.Add(stoneData);
                }
                else
                {
                    for (int j = 0; j < stoneData.career_limit.Count; j++)
                    {
                        if (stoneData.career_limit[j] == careerId)
                        {
                            playerCareerStoneList.Add(stoneData);
                        }
                    }
                }
            }

            if(playerCareerStoneList.Count > 1)
            {
                playerCareerStoneList.Sort(CareerStoneComp);
            }
            
            return playerCareerStoneList;
        }

        private int CareerStoneComp(CSVStone.Data x, CSVStone.Data y)
        {
            if (x.id > y.id)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public StoneSkillData GetServerDataById(uint powerStoneId)
        {
            if (serverDataDic == null)
                return null;

            StoneSkillData data;
            serverDataDic.TryGetValue(powerStoneId, out data);
            return data;
        }


        public string GetSkillDesc(uint stoneId, uint add = 0)
        {
            string desc = "";
            StoneSkillData currentData = Sys_StoneSkill.Instance.GetServerDataById(stoneId);            
            CSVStoneLevel.Data showLeveldata = null;
            if (null != currentData)
            {
                showLeveldata = CSVStoneLevel.Instance.GetConfData(stoneId * 1000 + currentData.powerStoneUnit.Level + add);
            }
            else
            {
                showLeveldata = CSVStoneLevel.Instance.GetConfData(stoneId * 1000 + 1);
            }

            if (null != showLeveldata)
            {
                uint skillId = showLeveldata.activeskill == 0 ? showLeveldata.passiveskill : showLeveldata.activeskill;
                if (Sys_Skill.Instance.IsActiveSkill(skillId))
                {
                    CSVActiveSkillInfo.Data cureentSkillData = CSVActiveSkillInfo.Instance.GetConfData(skillId);
                    desc = (null != cureentSkillData) ? LanguageHelper.GetTextContent(cureentSkillData.desc) : "";
                }
                else
                {
                    CSVPassiveSkillInfo.Data cureentSkillData = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                    desc = (null != cureentSkillData) ? LanguageHelper.GetTextContent(cureentSkillData.desc) : "";
                }
            }
            return desc;
        }

        public bool CanAdvance(uint stoneId)
        {
            StoneSkillData currentData = GetServerDataById(stoneId);
            if (null != currentData)
            {
                CSVStoneStage.Data NextStagedata = CSVStoneStage.Instance.GetConfData(stoneId * 1000 + currentData.powerStoneUnit.Stage + 1);
                if(null == NextStagedata)
                {
                    return false;
                }

                if(NextStagedata.stone_level == currentData.powerStoneUnit.Level)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }

        public StageSkillUnit GetStageSkillData(uint powerStoneId, uint index)
        {
            if (serverDataDic == null)
                return null;
            StoneSkillData data;
            serverDataDic.TryGetValue(powerStoneId, out data);
            if (null != data)
            {
                if (data.powerStoneUnit.StageSkill.Count > index)
                {
                    return data.powerStoneUnit.StageSkill[(int)index];
                }
                else
                {
                    return null;
                }
            }
            return null;
        }

        public List<CSVStoneStage.Data>  GetAllStoneStageDataById(uint powerStoneId)
        {
            List<CSVStoneStage.Data>  StoneStageList = new List<CSVStoneStage.Data>();

            //Dictionary<uint, CSVStoneStage.Data> allData = CSVStoneStage.Instance.GetDictData();
            //List<CSVStoneStage.Data>  dataList = new List<CSVStoneStage.Data>(allData.Values);

            var dataList = CSVStoneStage.Instance.GetAll();
            for (int i = 0; i < dataList.Count; i++)
            {
                CSVStoneStage.Data stoneStageData = dataList[i];
                if (stoneStageData.stone_id == powerStoneId)
                {
                    StoneStageList.Add(stoneStageData);
                }
            }

            if (StoneStageList.Count > 1)
            {
                StoneStageList.Sort(StoneStageComp);
            }
            return StoneStageList;
        }

        private int StoneStageComp(CSVStoneStage.Data x, CSVStoneStage.Data y)
        {
            if (x.stone_level > y.stone_level)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public uint _itemId = 0;
        public uint itemId
        {
            get
            {
                if(_itemId == 0)
                {
                    CSVParam.Data data1 = CSVParam.Instance.GetConfData(723);
                    if (null != data1)
                    {
                        string[] _strs4 = data1.str_value.Split('|');
                        _expNum = Convert.ToUInt32(_strs4[0]);
                        _itemId = Convert.ToUInt32(_strs4[1]);
                    }
                }

                return _itemId;
            }
        }

        public uint _expNum = 0;
        public uint expNum
        {
            get
            {
                if (_expNum == 0)
                {
                    CSVParam.Data data1 = CSVParam.Instance.GetConfData(723);
                    if (null != data1)
                    {
                        string[] _strs4 = data1.str_value.Split('|');
                        _expNum = Convert.ToUInt32(_strs4[0]);
                        _itemId = Convert.ToUInt32(_strs4[1]);
                    }
                }
                return _expNum;
            }
        }
    }
}

