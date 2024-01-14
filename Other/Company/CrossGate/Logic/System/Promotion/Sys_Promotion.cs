using Lib.Core;
using Logic.Core;
using Packet;
using System;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;

namespace Logic
{
    public class Sys_Promotion : SystemModuleBase<Sys_Promotion>
    {
        #region 系统函数
        public override void Init()
        {
            Sys_Fight.Instance.OnEnterFight += OnEnterBattle;
            Net_Combat.Instance.eventEmitter.Handle<CmdBattleEndNtf>(Net_Combat.EEvents.OnEndBattle, OnEndBattle, true);
            //ProcedureManager.eventEmitter.Handle(ProcedureManager.EEvents.OnAfterExitFightEffect, OnExitEffect, true);
        }
        public override void OnLogin()
        {
            InitData();
        }
        public override void OnLogout()
        {
            _Dispose();
        }
        public override void Dispose()
        {
            _Dispose();
        }
        #endregion
        #region 定义数据
        public enum ViewType
        {
            Equip = 1,//装备
            Skill = 2,//技能
            FamilyEmpowerment = 3,//家族历练
            Ornament = 4,//饰品
            Treasure = 5,//宝藏库
            Awaken = 6,//觉醒
            Adventure = 7,//冒险手册
            Reputation = 8,//声望
            PetMessage = 9,//宠物
            Partner = 10,//伙伴
            Jewel = 11,//宝石
            Team = 12,//组队界面
            Trade = 13,//交易
            AwakenImprint = 14,//觉醒印记
            FamilyEntrust = 15,//家族委托
            FamilyMall = 16,//家族商店（未加入家族时打开家族面板)
            AssistShop = 17,//援助值商店
            ArenaShop = 18,//竞技场商店
        }
        //达标区间
        public enum PromotionLve
        {
            Special = 0,//特殊
            LvUp = 1,//建议提升
            Great = 2,//达标值
            Normal = 3,//普通
            Perfect = 4,//完美值
        }
        public List<CSVAssessType.Data>  assessTypeDataList = new List<CSVAssessType.Data>();
        private Dictionary<uint, List<CSVAssessMain.Data>> assessMainDic = new Dictionary<uint, List<CSVAssessMain.Data>>();
        public Dictionary<PromotionLve, List<CheckAssessData>> _mineMainDataDic = new Dictionary<PromotionLve, List<CheckAssessData>>();

        private IReadOnlyList<CSVLossTipsBanList.Data>  shieldBattleList { get { return CSVLossTipsBanList.Instance.GetAll(); } }// = new List<CSVLossTipsBanList.Data>();
        private List<CheckAssessData> checkDataList = new List<CheckAssessData>();
        public List<SubAssessMainData> subAssessMainDatas = new List<SubAssessMainData>();
        private List<CheckTypeData> checkTypeDataList = new List<CheckTypeData>();
        private bool isCanShowPromotionDefeat { get; set; }
        //当前所选页签类型
        public uint curSelectedRankType { get; set; }
        //当前战斗失败后需要提示组队的数据
        CSVLossTipsBanList.Data curLossTipsBanListData;
        #endregion

        #region 功能方法
        /// <summary>
        /// 进入战斗
        /// </summary>
        private void OnEnterBattle(CSVBattleType.Data cSVBattleTypeTb)
        {
            if (UIManager.IsVisibleAndOpen(EUIID.UI_PromotionDefeat))
            {
                UIManager.CloseUI(EUIID.UI_PromotionDefeat);
            }
            if (cSVBattleTypeTb != null)
            {
                isCanShowPromotionDefeat = CheckBattleEndShow(cSVBattleTypeTb.id);
                if (isCanShowPromotionDefeat)
                    RefreshMineMainData();
            }
        }
        private void OnEndBattle(CmdBattleEndNtf obj)
        {
            if (obj.BattleFlag == (uint)BattleFlag.GuildBoss)
            {
                //DebugUtil.LogError("BattleFlag.GuildBoss");
                //UIManager.OpenUI(EUIID.UI_FamilyBoss_Result);
                //Sys_FamilyBoss.Instance.IsFamilyBossBattle = false;
                return;
            }
            if (Sys_MerchantFleet.Instance.CheckMerchantBattle(obj.BattleTypeId))
            {
                return;
            }
            //祈福走祈福自有结算
            if (Sys_Blessing.Instance.GetBlessDataIDByBattleID(obj.BattleTypeId) > 0)
                return;

            //boss资格挑战赛(特殊判断)
            bool isCanShow = true;
            if (obj != null && obj.BossTower != null)
            {
                BattleEndBossTower battleEndBossTower = obj.BossTower;
                if (battleEndBossTower.IsBoss)
                {
                    //未通过第一阶段 || 达到最后一阶段 即为战败
                    if (!Sys_ActivityBossTower.Instance.CheckBossTowerShowState(battleEndBossTower.StageId))
                        isCanShow = false;
                }
            }

            //战斗失败
            bool isFailed = Net_Combat.Instance.GetBattleOverResult() == 2;
            //战败 && 符合战败界面显示规则 && 不是在观战中
            if (isFailed && isCanShowPromotionDefeat && !Net_Combat.Instance.m_IsWatchBattle && isCanShow)
            {
                UIManager.OpenUI(EUIID.UI_PromotionDefeat);
            }
        }
        public void _Dispose()
        {
            assessTypeDataList.Clear();
            assessMainDic.Clear();
            _mineMainDataDic.Clear();
            //shieldBattleList.Clear();            
            checkDataList.Clear();
            subAssessMainDatas.Clear();
            checkTypeDataList.Clear();
            curSelectedRankType = 0;
            isCanShowPromotionDefeat = true;
            curLossTipsBanListData = null;
        }
        public void InitData()
        {
            _Dispose();

            var assessTypeDatas = CSVAssessType.Instance.GetAll();
            for (int i = 0, len = assessTypeDatas.Count; i < len; i++)
            {
                assessTypeDataList.Add(assessTypeDatas[i]);
            }
            assessTypeDataList.Sort((a, b) =>
            {
                return (int)(a.Sort - b.Sort);
            });

            var assessMainDatas = CSVAssessMain.Instance.GetAll();
            for (int i = 0, len = assessMainDatas.Count; i < len; i++)
            {
                CSVAssessMain.Data data = assessMainDatas[i];
                if (!assessMainDic.ContainsKey(data.RankType))
                {
                    assessMainDic[data.RankType] = new List<CSVAssessMain.Data>();
                    assessMainDic[data.RankType].Add(data);
                }
                else
                {
                    assessMainDic[data.RankType].Add(data);
                }
            }
            foreach (var item in assessMainDic)
            {
                item.Value.Sort((a, b) =>
                {
                    if (a.Subtype > b.Subtype)
                        return 1;
                    else
                        return -1;
                });
            }
            //for (int i = 0; i < CSVLossTipsBanList.Instance.Count; i++)
            //{
            //    shieldBattleList.Add(CSVLossTipsBanList.Instance[i]);                
            //}
        }
        public bool CheckBattleEndShow(uint typeId)
        {
            curLossTipsBanListData = null;
            bool isShow = true;
            for (int i = 0; i < shieldBattleList.Count; i++)
            {
                if (shieldBattleList[i].type == 0)
                {
                    if (shieldBattleList[i].battle_type == typeId)
                    {
                        isShow = false;
                        break;
                    }
                    else if (shieldBattleList[i].battle_id == Sys_Fight.curMonsterGroupId)
                    {
                        isShow = false;
                        break;
                    }
                }
                else
                {
                    if (shieldBattleList[i].battle_type == typeId)
                    {
                        isShow = true;
                        curLossTipsBanListData = shieldBattleList[i];
                        break;
                    }
                    else if (shieldBattleList[i].battle_id == Sys_Fight.curMonsterGroupId)
                    {
                        isShow = true;
                        curLossTipsBanListData = shieldBattleList[i];
                        break;
                    }
                }
            }
            return isShow;
        }
        /// <summary>
        /// 刷新战败要提升的三类型(建议提升 达标 常规(不可量化))
        /// </summary>
        public void RefreshMineMainData()
        {
            _mineMainDataDic.Clear();

            var assessMainDatas = CSVAssessMain.Instance.GetAll();
            for (int j = 0, len = assessMainDatas.Count; j < len; j++)
            {
                CSVAssessMain.Data mainData = assessMainDatas[j];
                if (mainData.UnlockCondition.Count > 0)
                {
                    bool isOpen = false;
                    for (int i = 0; i < mainData.UnlockCondition.Count; i++)
                    {
                        bool _isOpen = Sys_FunctionOpen.Instance.IsOpen(mainData.UnlockCondition[i]);
                        if (_isOpen)
                        {
                            isOpen = true;
                            break;
                        }
                    }
                    if (isOpen)
                    {
                        if (mainData.id < 10000)
                        {
                            if (mainData.Bar)
                            {
                                CSVAssessScore.Data scoreData = GetAssessScoreData(mainData.id);
                                if (scoreData != null)
                                {
                                    long curPower = GetCurTypePower(mainData.Jump[0]);
                                    PromotionLve prolv = CheckRoleEvaluation(scoreData, curPower);
                                    long targetPower = curPower < scoreData.Great ? scoreData.Great : scoreData.Perfect;
                                    CheckAssessData checkData = new CheckAssessData();
                                    checkData.type = prolv;
                                    checkData.diff = Math.Round((double)curPower / (double)targetPower, 2);
                                    checkData.mainData = mainData;
                                    AddPromotionLveData(prolv, checkData);
                                }
                            }
                            else
                            {
                                CheckAssessData checkData = new CheckAssessData();
                                checkData.type = PromotionLve.Normal;
                                checkData.diff = -1;
                                checkData.mainData = mainData;
                                AddPromotionLveData(PromotionLve.Normal, checkData);
                            }
                        }
                        else//特殊类型
                        {
                            CheckAssessData checkData = new CheckAssessData();
                            checkData.type = PromotionLve.Special;
                            checkData.diff = -1;
                            checkData.mainData = mainData;
                            AddPromotionLveData(PromotionLve.Special, checkData);
                        }
                    }
                }
            }
        }

        public List<CheckTypeData> GetCurDataList(uint type)
        {
            checkTypeDataList.Clear();
            GetDailyActivityDataList(type);
            if (assessMainDic.ContainsKey(type))
            {
                foreach (var item in assessMainDic[type])
                {
                    if (item.UnlockCondition.Count > 0)
                    {
                        bool isOpen = false;
                        for (int i = 0; i < item.UnlockCondition.Count; i++)
                        {
                            bool _isOpen = Sys_FunctionOpen.Instance.IsOpen(item.UnlockCondition[i]);
                            if (_isOpen)
                            {
                                isOpen = true;
                                break;
                            }
                        }
                        if (isOpen)
                        {
                            bool isCan = true;
                            if (item.Bar)
                            {
                                if (GetAssessScoreData(item.id) == null)
                                    isCan = false;
                            }
                            if (isCan)
                            {
                                CheckTypeData data = new CheckTypeData();
                                data.activitydata = null;
                                data.mainData = item;
                                data.diff = 0;
                                checkTypeDataList.Add(data);
                            }
                        }
                    }
                }
            }
            return checkTypeDataList;
        }
        List<CSVDailyActivity.Data>  activityDataList = new List<CSVDailyActivity.Data>();
        private void GetDailyActivityDataList(uint type)
        {
            activityDataList.Clear();

            var dailyActivityShowDatas = CSVDailyActivityShow.Instance.GetAll();
            for (int i = 0, len = dailyActivityShowDatas.Count; i < len; i++)
            {
                //排除即将开始的
                if (dailyActivityShowDatas[i].id != 3)
                {
                    List<CSVDailyActivity.Data>  list = Sys_Daily.Instance.getTodayUsefulDailies(dailyActivityShowDatas[i].id);
                    if (list != null && list.Count > 0)
                        activityDataList.AddRange(list);
                }
            }
            CSVAssessType.Data typeData = CSVAssessType.Instance.GetConfData(type);
            if (typeData != null && typeData.Reward_int != 0)
            {
                for (int j = 0; j < activityDataList.Count; j++)
                {
                    CSVDailyActivity.Data data = activityDataList[j];
                    if (data.Reward_int != null && data.Reward_int.Count > 0)
                    {
                        for (int i = 0; i < data.Reward_int.Count; i++)
                        {
                            if (data.Reward_int[i] == typeData.Reward_int)
                            {
                                uint curTimes = Sys_Daily.Instance.getDailyCurTimes(data.id);//当前次数;
                                uint maxTimes = data.limite;
                                double diff = maxTimes != 0 ? Math.Round((double)(curTimes / maxTimes), 2) : 2;
                                CheckTypeData checkData = new CheckTypeData();
                                checkData.activitydata = data;
                                checkData.mainData = null;
                                checkData.diff = diff;
                                checkTypeDataList.Add(checkData);
                            }
                        }
                    }
                }
            }
            checkTypeDataList.Sort((a, b) =>
            {
                if (a.diff > b.diff)
                    return 1;
                else
                    return -1;
            });
        }
        public CSVAssessScore.Data GetAssessScoreData(uint id)
        {
            uint level = Sys_Role.Instance.Role.Level;
            CSVAssessScore.Data data = CSVAssessScore.Instance.GetConfData(id * 1000 + level);
            if (data == null)
            {
                data = CSVAssessScore.Instance.GetConfData(id * 1000);
                if (data == null)
                {
                    if (level > 100)
                        data = CSVAssessScore.Instance.GetConfData(id * 1000 + 100);
                    else
                        data = CSVAssessScore.Instance.GetConfData(id * 1000 + 1);
                }
            }
            else
            {
                if (data.Great == 0 && data.Perfect == 0)
                {
                    data = null;
                }
            }
            return data;
        }
        public long GetCurTypePower(uint jump,uint itemid=0)
        {
            long curValue = 0;
            switch ((ViewType)jump)
            {
                case ViewType.FamilyEntrust:
                case ViewType.Trade:
                case ViewType.Equip://已穿戴装备总评分
                    List<ItemData> listItems;
                    Sys_Bag.Instance.BagItems.TryGetValue((int)BoxIDEnum.BoxIdEquipment, out listItems);
                    if (listItems != null && listItems.Count > 0)
                    {
                        foreach (var item in listItems)
                        {
                            curValue += item.Equip.Score;
                        }
                    }
                    break;
                case ViewType.Skill://已学习技能总评分
                    foreach (var item in Sys_Skill.Instance.bestSkillInfos)
                    {
                        if (item.Value.Level > 0)
                        {
                            if (item.Value.ESkillType == Sys_Skill.ESkillType.Active)
                            {
                                curValue += CSVActiveSkillInfo.Instance.GetConfData(item.Value.CurInfoID).score;
                            }
                            else if (item.Value.ESkillType == Sys_Skill.ESkillType.Passive)
                            {
                                curValue += CSVPassiveSkillInfo.Instance.GetConfData(item.Value.CurInfoID).score;
                            }
                        }
                    }
                    foreach (var item in Sys_Skill.Instance.commonSkillInfos)
                    {
                        if (item.Value.Level > 0)
                        {
                            if (item.Value.ESkillType == Sys_Skill.ESkillType.Active)
                            {
                                curValue += CSVActiveSkillInfo.Instance.GetConfData(item.Value.CurInfoID).score;

                            }
                            else if (item.Value.ESkillType == Sys_Skill.ESkillType.Passive)
                            {
                                curValue += CSVPassiveSkillInfo.Instance.GetConfData(item.Value.CurInfoID).score;
                            }
                        }
                    }
                    break;
                case ViewType.FamilyEmpowerment://家族历练等级
                    curValue = Sys_Experience.Instance.exPerienceLevel;
                    break;
                case ViewType.Ornament://已穿戴饰品总评分
                    List<ItemData> BoxIdOrnamentList;
                    Sys_Bag.Instance.BagItems.TryGetValue((int)BoxIDEnum.BoxIdOrnament, out BoxIdOrnamentList);
                    if (BoxIdOrnamentList != null && BoxIdOrnamentList.Count > 0)
                    {
                        foreach (var item in BoxIdOrnamentList)
                        {
                            curValue += item.ornament.Score;
                        }
                    }
                    break;
                case ViewType.Treasure://已携带宝藏总评分
                    List<uint> listIds = Sys_Treasure.Instance.GetSlotList();
                    for (int i = 0; i < listIds.Count; i++)
                    {
                        bool isUnlock = Sys_Treasure.Instance.IsSlotUnlock(listIds[i]);
                        if (isUnlock)
                        {
                            uint mTreasureId = Sys_Treasure.Instance.GetTreasureAtSlot(listIds[i]);
                            if (mTreasureId != 0)
                            {
                                CSVTreasures.Data treaData = CSVTreasures.Instance.GetConfData(mTreasureId);
                                curValue += treaData.score;
                            }
                        }
                    }
                    break;
                case ViewType.Awaken://当前觉醒阶段
                    CSVTravellerAwakening.Data csvAwakeData;
                    CSVTravellerAwakening.Instance.TryGetValue(Sys_TravellerAwakening.Instance.awakeLevel, out csvAwakeData);
                    if (csvAwakeData != null)
                    {
                        curValue = csvAwakeData.id;
                    }
                    break;
                case ViewType.Adventure://当前冒险手册等级
                    curValue = Sys_Adventure.Instance.Level;
                    break;
                case ViewType.Reputation://当前声望
                    curValue = Sys_Reputation.Instance.reputationLevel;
                    break;
                case ViewType.PetMessage://拥有宠物数量
                    foreach (var item in Sys_Pet.Instance.petsList)
                    {
                        if (item.petUnit.SimpleInfo.Level >= Sys_Role.Instance.Role.Level)
                        {
                            curValue++;
                        }
                    }
                    break;
                case ViewType.Partner://当前出战伙伴数量
                    curValue = FormPartnerCount();
                    break;
                case ViewType.Jewel://装备已镶嵌宝石总评分
                    List<ItemData> jewelList;
                    Sys_Bag.Instance.BagItems.TryGetValue((int)BoxIDEnum.BoxIdEquipment, out jewelList);
                    if (jewelList != null && jewelList.Count > 0)
                    {
                        foreach (var item in jewelList)
                        {
                            curValue += (long)Sys_Equip.Instance.CalJewelScore(item.Id, item.Equip);
                        }
                    }
                    break;
                case ViewType.AwakenImprint://觉醒印记总评分
                    Dictionary<uint, uint> dic = new Dictionary<uint, uint>();
                    foreach (var item in Sys_TravellerAwakening.Instance.severDic)
                    {
                        CSVImprintUpgrade.Data data = CSVImprintUpgrade.Instance.GetConfData(item.Key);
                        if (data != null)
                        {
                            if (dic.ContainsKey(data.Node_ID))
                            {
                                dic[data.Node_ID] = System.Math.Max(dic[data.Node_ID], data.score);
                            }
                            else
                            {
                                dic[data.Node_ID] = data.score;
                            }
                        }
                    }
                    foreach (var item in dic.Values)
                    {
                        curValue += item;
                    }
                    break;
                //道具使用当日使用数量
                case ViewType.FamilyMall:
                case ViewType.AssistShop:
                case ViewType.ArenaShop:
                    if (itemid != 0)
                        curValue = Sys_Bag.Instance.GetDayLimitItemUsedCount(itemid);
                    else
                        curValue = 0;
                    break;
                default:
                    break;
            }
            return curValue;
        }
        public int FormPartnerCount()
        {
            int count = 0;
            List<Partner> partnerList = Sys_Partner.Instance.GetUnlockPartners();
            for (int i = 0; i < partnerList.Count; i++)
            {
                bool isForm = Sys_Partner.Instance.IsInForm(partnerList[i].InfoId);
                if (isForm)
                {
                    count++;
                }
            }
            return count;
        }
        public PromotionLve CheckRoleEvaluation(CSVAssessScore.Data scoreData, long curPower)
        {
            PromotionLve prolv = PromotionLve.Normal;
            uint great = scoreData.Great;
            uint perfect = scoreData.Perfect;
            if (curPower < great)
            {
                prolv = PromotionLve.LvUp;
            }
            else if (curPower >= great && curPower < perfect)
            {
                prolv = PromotionLve.Great;
            }
            else if (curPower >= perfect)
            {
                prolv = PromotionLve.Perfect;
            }
            return prolv;
        }
        public void AddPromotionLveData(PromotionLve prolv, CheckAssessData checkData)
        {
            if (prolv == PromotionLve.Perfect)
                return;

            if (_mineMainDataDic.ContainsKey(prolv))
            {
                List<CheckAssessData> list = _mineMainDataDic[prolv];
                bool ishave = false;
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].mainData.id == checkData.mainData.id)
                    {
                        ishave = true;
                        break;
                    }
                }
                if (ishave) return;
                if (list.Count < 3)
                {
                    if (CheckProlv(checkData))
                        list.Add(checkData);
                }
                else
                {
                    if (prolv == PromotionLve.Normal || prolv == PromotionLve.Special)
                        return;
                    int index = -1;
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].diff > checkData.diff)
                        {
                            index = i;
                            break;
                        }
                    }
                    if (index != -1)
                    {
                        list.RemoveAt(index);
                        list.Add(checkData);
                    }
                }
                list.Sort((a, b) =>
                {
                    if (a.diff > b.diff)
                    {
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                });
            }
            else
            {
                if (CheckProlv(checkData))
                    _mineMainDataDic[prolv] = new List<CheckAssessData> { checkData };
            }
        }
        private bool CheckProlv(CheckAssessData checkData)
        {
            bool isCanAdd = true;
            if (Sys_Team.Instance.HaveTeam)
            {
                if (Sys_Team.Instance.TeamMemsCount >= 5)
                {
                    if (checkData.mainData.Tips == 4)
                        isCanAdd=false;
                }
                if (checkData.mainData.Tips == 2)
                    isCanAdd = false;
            }
            else
            {
                if (checkData.mainData.Tips == 3)
                    isCanAdd = false;
            }
            if (checkData.mainData.Tips == 4)
            {
                if (curLossTipsBanListData == null)
                    isCanAdd = false;
            }
            if (checkData.mainData.Tips == 0)
                isCanAdd = false;
            return isCanAdd;
        }
        public List<CheckAssessData> GetSortAssessMainData()
        {
            checkDataList.Clear();
            foreach (PromotionLve prolv in Enum.GetValues(typeof(PromotionLve)))
            {
                if (checkDataList.Count == 3)
                {
                    break;
                }
                if (_mineMainDataDic.TryGetValue(prolv, out List<CheckAssessData> list))
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        checkDataList.Add(list[i]);
                        if (checkDataList.Count == 3)
                        {
                            break;
                        }
                    }
                }
            }
            return checkDataList;
        }
        /// <summary>
        /// 跳转到指定界面
        /// </summary>
        public void MoveToView(uint Jump, bool bar)
        {
            if (UIManager.IsVisibleAndOpen(EUIID.UI_PromotionDefeat))
            {
                UIManager.CloseUI(EUIID.UI_PromotionDefeat);
            }
            switch ((ViewType)Jump)
            {
                case ViewType.Equip:
                    UIManager.OpenUI(EUIID.UI_LifeSkill_Message);
                    break;
                case ViewType.Skill:
                    UIManager.OpenUI(EUIID.UI_SkillUpgrade);
                    break;
                case ViewType.FamilyEmpowerment:
                    UIManager.OpenUI(EUIID.UI_Family_Empowerment);
                    break;
                case ViewType.Ornament:
                    UIManager.OpenUI(EUIID.UI_Ornament);
                    break;
                case ViewType.Treasure:
                    UIManager.OpenUI(EUIID.UI_Adventure, false, new AdventurePrama { page = (uint)EAdventurePageType.Treasure });
                    break;
                case ViewType.Awaken:
                    UIManager.OpenUI(EUIID.UI_Awaken);
                    break;
                case ViewType.Adventure:
                    UIManager.OpenUI(EUIID.UI_Adventure);
                    break;
                case ViewType.Reputation:
                    UIManager.OpenUI(EUIID.UI_Reputation);
                    break;
                case ViewType.PetMessage:
                    if (bar)
                    {
                        UIManager.OpenUI(EUIID.UI_Pet_Message);
                    }
                    else
                    {
                        UIManager.OpenUI(EUIID.UI_Pet_Message, false, new PetPrama { page = EPetMessageViewState.Skill });
                    }
                    break;
                case ViewType.Partner:
                    UIManager.OpenUI(EUIID.UI_Partner);
                    break;
                case ViewType.Jewel:
                    UIManager.OpenUI(EUIID.UI_Equipment);
                    break;
                case ViewType.Team:
                    if (curLossTipsBanListData != null)
                    {
                        if (curLossTipsBanListData.Parameter != 0)
                            Sys_Team.Instance.ApplyCreateTeam(Sys_Role.Instance.RoleId, curLossTipsBanListData.Parameter);
                        UIManager.OpenUI(EUIID.UI_Team_Member,false, UI_Team_Member.EType.Team);
                    }
                    break;
                case ViewType.Trade:
                    UIManager.OpenUI(EUIID.UI_Trade);
                    break;
                case ViewType.AwakenImprint:
                    UIManager.OpenUI(EUIID.UI_Awaken, false, 1);
                    break;
                case ViewType.FamilyEntrust:
                    if(Sys_Family.Instance.familyData.isInFamily)
                        UIManager.OpenUI(EUIID.UI_FamilyWorkshop);
                    else
                        UIManager.OpenUI(EUIID.UI_ApplyFamily);
                    break;
                case ViewType.FamilyMall:
                    if (Sys_Family.Instance.familyData.isInFamily)
                    {
                        MallPrama mall = new MallPrama();
                        mall.mallId = 901;
                        mall.shopId = 9001;
                        UIManager.OpenUI(EUIID.UI_Mall, false, mall);
                    }
                    else
                        UIManager.OpenUI(EUIID.UI_ApplyFamily);
                    break;
                case ViewType.AssistShop:
                    UIManager.OpenUI(EUIID.UI_PointMall, false, new MallPrama() { mallId = 501, shopId = 5011 });
                    break;
                case ViewType.ArenaShop:
                    UIManager.OpenUI(EUIID.UI_PointMall, false, new MallPrama() { mallId = 501, shopId=1006 });
                    break;
                default:
                    break;
            }
        }

        public List<SubAssessMainData> GetSubAssessMainData(CSVAssessMain.Data mainData)
        {
            subAssessMainDatas.Clear();
            for (int i = 0; i < mainData.Jump.Count; i++)
            {
                bool isOpen = Sys_FunctionOpen.Instance.IsOpen(mainData.UnlockCondition[i]);
                if (isOpen)
                {
                    SubAssessMainData data = new SubAssessMainData();
                    data.unlockCondition = mainData.UnlockCondition[i];
                    data.jump = mainData.Jump[i];
                    data.subName = mainData.LIst[i];
                    data.bar = mainData.Bar;
                    subAssessMainDatas.Add(data);
                }
            }
            return subAssessMainDatas;
        }
        #endregion
    }
    public class CheckAssessData
    {
        public Sys_Promotion.PromotionLve type;
        public double diff;
        public CSVAssessMain.Data mainData;
    }
    public class SubAssessMainData
    {
        public uint unlockCondition;
        public uint jump;
        public uint subName;
        public bool bar;
    }
    public class ClickPromotionListData
    {
        public EUIID eUIID;
        public RectTransform clickTarget;
        public RectTransform parent;
    }
    public class CheckTypeData
    {
        public CSVDailyActivity.Data activitydata;
        public CSVAssessMain.Data mainData;
        public double diff;
    }
}