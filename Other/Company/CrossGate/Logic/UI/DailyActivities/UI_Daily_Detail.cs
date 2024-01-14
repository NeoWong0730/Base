using System;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Table;
namespace Logic
{
    public class UI_Daily_Detail:UIComponent, UI_Daily_Detail_Layout.IListener
    {
        private UI_Daily_Detail_Layout m_Layout = new UI_Daily_Detail_Layout();

        private uint mConfigID;

      
        public uint ConfigID { get { return mConfigID; } set { mConfigID = value; } }

        public UnityEngine.Events.UnityEvent OnCloseEvent = new UnityEngine.Events.UnityEvent();

        public UnityEngine.Events.UnityEvent OnJoinEvent = new UnityEngine.Events.UnityEvent();

        private List<int> m_LevelCondition = new List<int>();
        private List<int> m_TaskCondition = new List<int>();

        protected override void Loaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);

        }


        public override void Show()
        {
            base.Show();

            Refresh();

            Sys_Daily.Instance.CloseNewTips(mConfigID);
        }

        public override void Hide()
        {
            base.Hide();

            OnCloseEvent.Invoke();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_ActivityTrialGate.Instance.eventEmitter.Handle(Sys_ActivityTrialGate.EEvents.OnRefreshRedPoint, OnRefreshTrialRedPoint, toRegister);
        }
        /// <summary>
        /// 刷新试炼之门红点
        /// </summary>
        private void OnRefreshTrialRedPoint()
        {
            bool trialRedPoint = Sys_ActivityTrialGate.Instance.CheckRedPoint();
            m_Layout.trialRedPoint.SetActive(trialRedPoint);
        }
        public void OnClickJoin()
        {
            if (Sys_Daily.Instance.JoinDaily(mConfigID) == false)
            {
                return;
            }
                

            base.Hide();

            OnJoinEvent.Invoke();
        }

        public void OnClickClose()
        {
            Hide();
        }
        public void OnClickCamp() {
            //if (Sys_Daily.Instance.JoinDaily(mConfigID) == false)
            //    return;

            UIManager.CloseUI(EUIID.UI_DailyActivites);
            UIManager.OpenUI(EUIID.UI_WorldBossCampPreview, false);
        }
        public void OnClickTrial()
        {
            UIManager.OpenUI(EUIID.UI_TrialSkillDeploy);
        }
        public void OnClickBossTowerRank()
        {
            Sys_ActivityBossTower.Instance.OpenCurStateRank();
        }
        public void OnClickCampShop()
        {
            MallPrama mallPrama = new MallPrama();
            mallPrama.mallId = 1600u;
            mallPrama.shopId = 16001u;

            UIManager.OpenUI(EUIID.UI_Mall, false, mallPrama);
        }
        public void SetJoinEnable(bool state)
        {
            m_Layout.SetJoinBtnActive(state);
        }

        protected override void Refresh()
        {
            var data = CSVDailyActivity.Instance.GetConfData(mConfigID);

            if (data == null)
                return;

           // bool isOpen = Sys_Daily.Instance.isTodayDaily(mConfigID);
           // m_Layout.SetDailyIsOpen(isOpen);
            // 控制
            bool isWorldBossActivity = Sys_WorldBoss.Instance.TryGetActivityIdByDailyId(mConfigID, out var actitityId);
            m_Layout.btnCamp.gameObject.SetActive(isWorldBossActivity);
            m_Layout.mBtnCampShop.gameObject.SetActive(mConfigID == 81u || mConfigID == 83u || mConfigID == 85u);

            m_Layout.mTransCampRed.gameObject.SetActive(Sys_WorldBoss.Instance.HasRewardUngot);

            bool trialRedPoint = Sys_ActivityTrialGate.Instance.CheckRedPoint();
            m_Layout.btnTrialSkill.gameObject.SetActive(mConfigID == 220);
            m_Layout.trialRedPoint.SetActive(trialRedPoint);

            m_Layout.SetName(data.ActiveName);
            m_Layout.SetDesc(data.OutputDisplay);

            //var funcData = CSVFunctionOpen.Instance.GetConfData(data.FunctionOpenid);
            // var conditionData = CSVCheckseq.Instance.GetConfData(funcData.Condition_id);
            // conditionData.

            m_LevelCondition.Clear();
            DailyDataHelper.GetCondition(data.FunctionOpenid, 10202, ref m_LevelCondition);

            m_TaskCondition.Clear();
            DailyDataHelper.GetCondition(data.FunctionOpenid, 10091, ref m_TaskCondition);

            m_Layout.SetLevelLimit(LanguageHelper.GetTextContent(data.OpeningImpose, GetLimiteString()));

            SetActivityDesInfo(mConfigID, data.ActiveDes);

            m_Layout.SetTaskType(data.WayDesc);

           var reward =  getDrop(data.Reward);

            m_Layout.SetReward(reward);

            uint curAc = Sys_Daily.Instance.getDailyAcitvity(mConfigID);

            uint maxAc = Sys_Daily.Instance.GetDailyMaxActivityNum(mConfigID);// Sys_Daily.Instance.IsRecommendDaily(mConfigID) ? (data.ActivityNumMax /** 2*/) : data.ActivityNumMax;

            m_Layout.SetActivity((int)curAc, (int)maxAc);

             uint curTimes = Sys_Daily.Instance.getDailyCurTimes(data.id);//当前次数;
            uint maxTimes = data.limite;

            m_Layout.SetTimes( (int)curTimes, (int)maxTimes);

            m_Layout.SetIcon(data.AvtiveIcon);

            uint type = Sys_Daily.Instance.getDailyType(mConfigID);

            int timeType = type == 2 ? 10698 : 10699;

            m_Layout.SetTime((uint)timeType);

            UI_Daily_Common.setDailyStateMask(type,data,(value0,value1) =>{ m_Layout.SetRightMark(value0,value1); });

            UI_Daily_Common.SetLimiteText(type, data, (value1,value2,value3) => { m_Layout.SetRightLimit(value1, value2,value3); });

            m_Layout.SetDetailCoutType(data.WayIcon);

           // var opState = UI_Daily_Common.GetDailyOpState(type, data);

           // opState = opState == UI_Daily_Common.DailyItem.EOpState.Go ? UI_Daily_Common.DailyItem.EOpState.None : opState;

            m_Layout.SetOpState(UI_Daily_Common.DailyItem.EOpState.None);

            m_Layout.SetAmount(Sys_Daily.Instance.GetDailySpecial(mConfigID));

            //资格赛 || boss挑战赛
            if (mConfigID == 240 || mConfigID == 250)
            {
                m_Layout.bossTowerTrans.gameObject.SetActive(true);
                m_Layout.qualifierTrans.gameObject.SetActive(true);
                if (mConfigID == 240)
                {
                    bool isHaveQualifier = Sys_ActivityBossTower.Instance.CheckIsHaveBossFightQualifier();
                    m_Layout.qualifierPass.gameObject.SetActive(isHaveQualifier);
                    m_Layout.qualifierFailed.gameObject.SetActive(!isHaveQualifier);
                    m_Layout.rnakTrans.gameObject.SetActive(true);
                    m_Layout.bossTowerRankNum.text = Sys_ActivityBossTower.Instance.GetCurRnakStateDescribe(2);
                }
                else if (mConfigID == 250)
                {
                    m_Layout.qualifierPass.gameObject.SetActive(Sys_ActivityBossTower.Instance.bBossUnlock);
                    m_Layout.qualifierFailed.gameObject.SetActive(!Sys_ActivityBossTower.Instance.bBossUnlock);
                    m_Layout.rnakTrans.gameObject.SetActive(false);
                }
            }
            else
                m_Layout.bossTowerTrans.gameObject.SetActive(false);
        }

        private void SetActivityDesInfo(uint activityID, uint langueID)
        {
            string value = string.Empty;

            if (activityID == 120)//魔物讨伐
            {
                var curcount = Sys_Daily.Instance.KilledMonsterCount;

                if (curcount > Sys_Daily.Instance.KilledMonsterMaxCount)
                    curcount = (uint)Sys_Daily.Instance.KilledMonsterMaxCount;

                value = LanguageHelper.GetTextContent(langueID, curcount.ToString(), Sys_Daily.Instance.KilledMonsterMaxCount.ToString());
            }            
            else if (activityID == 100)
            {
                int curStage = 0;
                int curLevel = 0;
                int totalStage = 0;
                int totalLevel = 0;

                Sys_HundredPeopleArea.Instance.GetPassedStage(out curStage, out curLevel);
                Sys_HundredPeopleArea.Instance.GetMaxStage(out totalStage, out totalLevel);

                value = LanguageHelper.GetTextContent(langueID, curStage.ToString(), curLevel.ToString(),totalStage.ToString(),totalLevel.ToString());
            }            
            else
                value = LanguageHelper.GetTextContent(langueID);

            m_Layout.SetDetailInfo(value);
        }
        private List<ItemIdCount> mDropCahce = new List<ItemIdCount>();
        private List<ItemIdCount> getDrop(List<uint> droplist)
        {
            mDropCahce.Clear();

            if (droplist == null)
                return mDropCahce;

            int count = droplist.Count;

            for (int i = 0; i < count; i++)
            {
                var reward = CSVDrop.Instance.GetDropItem(droplist[i]);

                mDropCahce.AddRange(reward);
            }

            return mDropCahce;
        }

        private string GetLimiteString()
        {
            var levelCount = m_LevelCondition.Count;
            var taskCount = m_TaskCondition.Count;

            if (levelCount == 0 && taskCount == 0)
                return string.Empty;

            if (levelCount > 0 && taskCount == 0)
                return LanguageHelper.GetTextContent(11831, (m_LevelCondition[0] ).ToString());

            var data = CSVTask.Instance.GetConfData((uint)m_TaskCondition[0]);
            if (levelCount == 0 && taskCount > 0)
            {      
               return LanguageHelper.GetTextContent(11832, CSVTaskLanguage.Instance.GetConfData(data.taskName).words);
            }
                
            return LanguageHelper.GetTextContent(11833, (m_LevelCondition[0]).ToString(), CSVTaskLanguage.Instance.GetConfData(data.taskName).words);
        }
    }
}
