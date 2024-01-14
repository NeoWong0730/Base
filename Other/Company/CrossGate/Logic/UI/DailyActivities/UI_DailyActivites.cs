using Logic.Core;
using System.Collections.Generic;
using Table;
using UnityEngine;

namespace Logic
{
    public class UIDailyActivitesParmas
    {
        public uint  SkipToID{ get; set; }

        public bool IsSkipDetail { get; set; } = false;
    }
    public partial class UI_DailyActivites : UIBase, UI_UI_DailyActivites_Layout.IListener
    {

        private UI_UI_DailyActivites_Layout m_Layout = new UI_UI_DailyActivites_Layout();

        private UI_Daily_Detail m_Detail = new UI_Daily_Detail();

        private uint mFocusConfigID = 0;

        private uint mSkipToID = 0;

        private bool mIsSkipDetail = false;


        private float LastClickGetRewardTimePoint = 0;
        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Detail.Init(m_Layout.GetDetail());

            m_Layout.setListener(this);

            m_Detail.OnCloseEvent.AddListener(OnDetailClose);

            m_Detail.OnJoinEvent.AddListener(OnClickClose);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateLevel, OnRoleLevelUp, toRegister);

            Sys_Daily.Instance.eventEmitter.Handle<uint>(Sys_Daily.EEvents.RewardAck, OnDailyRewardAck, toRegister);

            Sys_Daily.Instance.eventEmitter.Handle(Sys_Daily.EEvents.ActiveValueChange, OnDailyValueChange, toRegister);

            Sys_Daily.Instance.eventEmitter.Handle<uint>(Sys_Daily.EEvents.NewTipsChange, OnNewTipsChange, toRegister);

            Sys_Daily.Instance.eventEmitter.Handle<uint>(Sys_Daily.EEvents.DailyReward,OnDailyReward ,toRegister);
        }

        private void OnRoleLevelUp()
        {
            RefreshLeftContext();

            FocusLeftContext(0);
        }
        protected override void OnOpen(object arg)
        {
          
            Sys_Daily.Instance.Apply_InfoReq();

            var parmas = arg as UIDailyActivitesParmas;

            if (parmas != null && parmas.SkipToID != 0)
            {
                mSkipToID = parmas.SkipToID;

                mIsSkipDetail = parmas.IsSkipDetail;

            }
        }
        protected override void OnShow()
        {
            Sys_Daily.Instance.InitDynamicType();

            RefreshLeftContext();

            if (mSkipToID == 0)
            {
                FocusLeftContext(0);
            }
            else
            {
                SkipTo();
            }

            m_Layout.SetActivityTex(Sys_Daily.Instance.TotalActivity.ToString());

            m_Layout.SetRightScrollGridActive(true);

            m_Layout.SetRewartActive(Sys_Daily.Instance.HaveReward());

            if (mIsSkipDetail)
            {
                if (mSkipToID == 112)
                {
                    UIManager.OpenUI(EUIID.UI_FamilyBoss_Info);
                }
                else
                {
                    m_Detail.ConfigID = mSkipToID;
                    m_Detail.Show();
                }
            }

            m_Layout.SetRewardActive(true);

            RefreshAward();
        }

        protected override void OnHide()
        {            
            m_Detail.Hide();

            mIsSkipDetail = false;
            mSkipToID = 0;

        }

        protected override void OnClose()
        {
            Sys_Daily.Instance.SaveNewTipsDB();
        }
        public void Close()
        {
            CloseSelf();
        }

        public void OnClickLeftItem(uint id)
        {     
            RefreshRightContext(id);
        }
        public void OnClickRightItem(uint id)
        {
            if(id == 112)
            {
                UIManager.OpenUI(EUIID.UI_FamilyBoss_Info);
            }
            else
            {
                m_Detail.ConfigID = id;

                // m_Layout.SetRightScrollGridActive(false);

                m_Layout.SetFocusRightItem(id);

                m_Detail.Show();
            }
        }

        public void OnClickClose()
        {
            CloseSelf();
        }

        public void OnCalendar()
        {
            UIManager.OpenUI(EUIID.UI_DailyActivites_Calendar);
        }

        public void OnBell()
        {
            UIManager.OpenUI(EUIID.UI_DailyActivites_Set);
        }

        public void OnClickActivity()
        {
            UIManager.OpenUI(EUIID.UI_DailyActivites_Activity);
        }

        public void OnFocusRightItem(bool b, uint configID)
        {
            if (b)
            {
                mFocusConfigID = configID;
            }
        }
        private void OnDetailClose()
        {
            m_Layout.SetRightScrollGridActive(true);
        }

        public void OnClickJoin()
        {
            if (mFocusConfigID == 0)
                return;

            if (Sys_Daily.Instance.GotoActivity(mFocusConfigID) == false)
                return;

            Close();


        }
        public void OnClickGotoJoin(uint id)
        {
            Sys_Daily.Instance.CloseNewTips(id);


            if (Sys_Daily.Instance.GotoActivity(id) == false)
                return;

            Close();

        
        }


        private void SkipTo()
        {
            var type = Sys_Daily.Instance.getDailyType(mSkipToID);

            var typeindex = mLeftContextCache.FindIndex(o => o.id == type);


            FocusLeftContext(typeindex < 0 ? 0 : typeindex);

            if (typeindex < 0)
                return;


            m_Layout.SetFocusRightItem(mSkipToID);

            
        }


        public void OnClickRaward(uint id)
        {
            if (Sys_Daily.Instance.HadGetReward(id))
                return;

            uint curDailyActivty = Sys_Daily.Instance.TotalActivity;

            var data = CSVDailyActivityReward.Instance.GetConfData(id);

            if (data == null)
            {
                Sys_Hint.Instance.PushContent_Normal("非法活跃度奖励编号");
                return;
            }


            if (data.Activity > curDailyActivty)
            {
                return;
            }

            if (Time.time - LastClickGetRewardTimePoint < 1f)
                return;

            Sys_Daily.Instance.Apply_Reward(id);

            LastClickGetRewardTimePoint = Time.time;
        }
    }

  
    /// <summary>
    /// UI类型处理
    /// </summary>
    /// 
    public partial class UI_DailyActivites : UIBase, UI_UI_DailyActivites_Layout.IListener
    {
        private List<CSVDailyActivityShow.Data>  mLeftContextCache = new List<CSVDailyActivityShow.Data>();


        private void RefreshLeftContextData()
        {
            mLeftContextCache.Clear();

            var dataList = CSVDailyActivityShow.Instance.GetAll();

            for(int i = 0, len = dataList.Count; i < len; i++)
            {
                var value = dataList[i];

                var ishave = Sys_Daily.Instance.HaveVaildType(value.id);

                if (ishave)
                    mLeftContextCache.Add(value);
            }
        }
        private void RefreshLeftContext()
        {
            RefreshLeftContextData();

            int count = mLeftContextCache.Count;

            m_Layout.SetLeftContentSize(count);

           
            for(int index = 0; index < count; index++)
            {
                var value = mLeftContextCache[index];
                m_Layout.SetLeftContentName(index, value.ActiveTypeName);
                m_Layout.SetLeftContentConfigID(index, value.id);

                m_Layout.SetLeftContentRed(index, Sys_Daily.Instance.isTypeNewTips(value.id));

                m_Layout.SetLeftContentRewardRed(index, Sys_Daily.Instance.IsTypeHaveRedPoint(value.id));

            }
        }

        /// <summary>
        /// 选中类型
        /// </summary>
        /// <param name="index"></param>
        private void FocusLeftContext(int index)
        {
            m_Layout.SetFoucsLeftContent(index);
        }


        private void RefreshLeftContextRedState()
        {
            int index = 0;

            foreach (var kvp in mLeftContextCache)
            {
                m_Layout.SetLeftContentRed(index, Sys_Daily.Instance.isTypeNewTips(kvp.id));

                index++;

            }
        }
    }

    public partial class UI_DailyActivites : UIBase, UI_UI_DailyActivites_Layout.IListener
    {
        List<CSVDailyActivity.Data>  itemList = new List<CSVDailyActivity.Data>();
        private void RefreshRightContext(uint type)
        {
            var itemList = Sys_Daily.Instance.GetVaildDailiesByType(type);
            #region 资格赛期间切换页签请求个人排行数据刷新
            if (type == 5)
            {
                var activityData = itemList.Find(o => o.id == 240);
                if (activityData != null)
                    Sys_ActivityBossTower.Instance.OnBossTowerSelfRankReq();
            }
            #endregion
            if (itemList == null)
            {
                m_Layout.setRightContentSize(0);
                return;
            }
               
            int count = itemList.Count;

            m_Layout.setRightContentSize(count);

            for (int i = 0; i < count; i++)
            {
                var dailyfunc = Sys_Daily.Instance.GetDailyFunc(itemList[i].id);

                m_Layout.SetRightContentName(i, itemList[i].ActiveName, itemList[i].OutputDisplay);

                m_Layout.SetRightContentConfigID(i, itemList[i].id);

                m_Layout.SetRightContentIndex(i);

                uint curAc = Sys_Daily.Instance.getDailyAcitvity(itemList[i].id);//当前活跃度

                uint maxAc = Sys_Daily.Instance.GetDailyMaxActivityNum(itemList[i].id);//Sys_Daily.Instance.IsRecommendDaily(itemList[i].id) ? (itemList[i].ActivityNumMax /** 2*/) : itemList[i].ActivityNumMax;

                m_Layout.SetRightContentActivity(i, curAc, maxAc);

                uint curTimes = Sys_Daily.Instance.getDailyCurTimes(itemList[i].id);//当前次数

                uint totalTimes = Sys_Daily.Instance.getDailyTotalTimes(itemList[i].id);

                m_Layout.SetRightContentTimes(i, curTimes, totalTimes);

                m_Layout.SetRightContentIcon(i, itemList[i].AvtiveIcon);

                m_Layout.SetRightContentNewTips(i, Sys_Daily.Instance.isNewTips(itemList[i].id));

                setDailyStateMask(type, i, itemList[i]);
             
                SetLimiteText(type, i, itemList[i]);

                m_Layout.SetRightPlyerCoutType(i, itemList[i].WayIcon);

                var opState = UI_Daily_Common.GetDailyOpState(type, itemList[i]);
                m_Layout.SetRightOpState(i, opState);

                m_Layout.SetRightContentRewardRed(i, Sys_Daily.Instance.HaveAward(itemList[i].id) || dailyfunc.HaveNewNotice());

                m_Layout.SetRightAmount(i, Sys_Daily.Instance.GetDailySpecial(itemList[i].id));

                bool maskactive = false;

                if (itemList[i].ActiveType != 2 && itemList[i].ActiveType != 3)
                {
                    maskactive = dailyfunc.HadUsedTimes();
                }
                else if (itemList[i].ActiveType == 2)
                {
                    maskactive = opState == UI_Daily_Common.DailyItem.EOpState.Finish;
                }

                if (itemList[i].id == 140 && maskactive)
                {
                   var jsdata =  Sys_JSBattle.Instance.GetRoleVictoryArenaDaily();

                    if (jsdata != null && (jsdata.LeftBuyTimes > 0 || jsdata.LeftChallengeTimes > 0))
                    {
                        maskactive = false;
                    }
                }
                m_Layout.SetRightContentMaskActive(i, maskactive);


                string strdifficult = dailyfunc.Data.IsEasy > 0 ? LanguageHelper.GetTextContent(dailyfunc.Data.IsEasy) : string.Empty;

                m_Layout.SetRightContentDifficultText(i,strdifficult);
            }

            if(itemList.Count > 0)
               m_Layout.SetFocusRightItem(itemList[0].id);
        }


        /// <summary>
        /// 左上角标记 限定
        /// </summary>
        /// <param name="type"></param>
        /// <param name="index"></param>
        /// <param name="data"></param>
        private void SetLimiteText(uint type, int index, CSVDailyActivity.Data data)
        {
            UI_Daily_Common.SetLimiteText(type, data, ( value1, value2,value3) =>
            {
                m_Layout.SetRightLimit(index,value1,value2,value3);

            });

           
        }


        /// <summary>
        /// 右上角标记 活动状态
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private void setDailyStateMask(uint type, int index,CSVDailyActivity.Data data)
        {
            UI_Daily_Common.setDailyStateMask(type, data, (value0,value1) => {

                m_Layout.SetRightMark(index, value0,value1);
            });

            
   
        }

        private void RefreshAward()
        {            
            uint curDailyActivty = Sys_Daily.Instance.TotalActivity;

            List<ItemIdCount> rewardList = new List<ItemIdCount>();

            var dataList = CSVDailyActivityReward.Instance.GetAll();
            for (int i = 0, len = dataList.Count; i < len; i++)
            {
                rewardList.Clear();

                rewardList = CSVDrop.Instance.GetDropItem(dataList[i].Reward);

                m_Layout.SetAward(i, dataList[i].id,rewardList[0].id,(uint)rewardList[0].count);

                uint state = curDailyActivty < dataList[i].Activity ? 0 : (Sys_Daily.Instance.HadGetReward(dataList[i].id) ? 2u : 1u);

                m_Layout.SetRewardState(i, state);
            }

            uint total = CSVDailyActivityReward.Instance.GetByIndex(CSVDailyActivityReward.Instance.Count - 1).Activity;

            float process = curDailyActivty * 1f / total;

            m_Layout.SetAwardProcess(process);
        }

        private void OnDailyRewardAck(uint id)
        {
            
            RefreshAward();
        }

        private void OnDailyValueChange()
        {
            m_Layout.SetRewartActive(Sys_Daily.Instance.HaveReward());
        }

        private void OnNewTipsChange(uint id)
        {
           var item = m_Layout.GetRightContentItemIndex(id);

            if (item < 0)
                return;

            var func = Sys_Daily.Instance.GetDailyFunc(id);

            m_Layout.SetRightContentNewTips(item, Sys_Daily.Instance.isNewTips(id));

            m_Layout.SetRightContentRewardRed(item, func.HaveNewNotice());


            RefreshLeftContextRedState();
        }


        private void OnDailyReward(uint id)
        {
            var item = m_Layout.GetRightContentItemIndex(id);

            if (item < 0)
                return;

            var data = CSVDailyActivity.Instance.GetConfData(id);

            m_Layout.SetRightContentRewardRed(item, Sys_Daily.Instance.HaveAward(id));

            int index = mLeftContextCache.FindIndex(o => o.id == data.ActiveType);

            m_Layout.SetLeftContentRewardRed(index, Sys_Daily.Instance.IsTypeHaveRedPoint(mLeftContextCache[index].id));
            

        }
    }

}
