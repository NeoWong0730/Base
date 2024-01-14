using System;
using System.Collections.Generic;
using Framework;
using Lib.Core;
using Logic;
using Logic.Core;
using Packet;
using Table;
using UnityEngine;

namespace Logic
{
    public class UI_BattlePass_Parma
    {
        /// <summary>
        /// 0 奖励 1 任务 2 商店
        /// </summary>
        public uint Type = 0;
    }
    public partial class UI_BattlePass : UIBase
    {
        UI_BattlePass_Layout m_Layout = new UI_BattlePass_Layout();

        private float m_TimeUpdate = 0;

        private CutSceneModelShowController cutSceneController = new CutSceneModelShowController();

        UI_CurrencyTitle m_CurrencyTitle = null;

        private List<uint> m_Money = new List<uint>() { 1,19};

        UI_BattlePass_Parma m_parma;
        protected override void OnLoaded()
        {
            m_Layout.OnLoaded(gameObject.transform);

            m_Layout.SetListener(this);

            cutSceneController.mInfoAssetDependencies = gameObject.GetComponent<AssetDependencies>();
           

            m_CurrencyTitle = new UI_CurrencyTitle(gameObject.transform.Find("Animator/Image_Bg/UI_Property").gameObject);

            m_CurrencyTitle.SetData(m_Money);


        }


        protected override void OnOpen(object arg)
        {
            Sys_BattlePass.Instance.SendGetInfo();

            var mallData = CSVMall.Instance.GetConfData(1001);
            Sys_Mall.Instance.OnItemRecordReq(mallData.shop_id[0]);

            LoadShopItems();


            m_parma = arg as UI_BattlePass_Parma;

        }

        protected override void OnShow()
        {
            m_CurrencyTitle.InitUi();

            LoadShowScene();
            m_Layout.SetModelController(cutSceneController);

            m_TimeUpdate = 0;
            switch(m_Layout.m_BattleState)
            {
                case UI_BattlePass_Layout.BattlePassState.BattlePass_Reward:
                    RefreshReward();
                    break;
                case UI_BattlePass_Layout.BattlePassState.BattlePass_Task:
                    RefreshTask(TaskState);
                    break;
                case UI_BattlePass_Layout.BattlePassState.BattleShop:
                    RefreshShop();
                    break;
            }

            RefreshTime();

            OnRefreshRedState();

            if (m_parma != null)
            {
                if (m_parma.Type == 2)
                {
                    m_Layout.SetFacusShop();
                    m_parma = null;
                }
            }
        }

        private void LoadShowScene()
        {
            cutSceneController.LoadShowScene();

            BattlePassModelShow modeshow = new BattlePassModelShow();

            modeshow.m_Data = Sys_BattlePass.Instance.GetRewardDisplayTableData(Sys_Role.Instance.Role.Career);

            modeshow.ModelIndex = Sys_BattlePass.Instance.GetRewardModelAssetIndex();

            cutSceneController.AddModelShow(modeshow);
        }
        protected override void OnUpdate()
        {
            if (Time.time - m_TimeUpdate > 10)
            {
                RefreshTime();
            }

        }

        private void RefreshTime()
        {
            if (Sys_BattlePass.Instance.Info != null && Sys_BattlePass.Instance.ActiveNtfInfo.EndTime > Sys_Time.Instance.GetServerTime())
            {
                var timeoffset = Sys_BattlePass.Instance.ActiveNtfInfo.EndTime - Sys_Time.Instance.GetServerTime();

                uint day = timeoffset / 86400;

                uint time0 = timeoffset % 86400;
                uint hours = time0 / (3600);

                uint mins = time0 % (3600) / 60;

                m_Layout.SetTime(day, hours, mins);
            }

            m_TimeUpdate = Time.time;
        }


        protected override void OnHide()
        {
            cutSceneController.UnLoadShowScene();
        }

        protected override void OnClose()
        {
            m_ScorllMoveTimer?.Cancel();
        }
    }

    public partial class UI_BattlePass : UIBase
    {
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_BattlePass.Instance.eventEmitter.Handle(Sys_BattlePass.EEvents.OneKeyLevelRewardRes, OnOneKeyGetAllLevelReward,toRegister);
            Sys_BattlePass.Instance.eventEmitter.Handle<uint>(Sys_BattlePass.EEvents.LevelRewardRes, OnLvelReward, toRegister);
            Sys_BattlePass.Instance.eventEmitter.Handle(Sys_BattlePass.EEvents.BattlePassTypeChange, OnBattlePassTypeChange, toRegister);
            Sys_BattlePass.Instance.eventEmitter.Handle(Sys_BattlePass.EEvents.UnLockNewLevelReward, OnUnLockNewLevelReward, toRegister);
            Sys_BattlePass.Instance.eventEmitter.Handle<uint>(Sys_BattlePass.EEvents.GetTaskReward, OnGetTaskReward, toRegister);
            Sys_BattlePass.Instance.eventEmitter.Handle(Sys_BattlePass.EEvents.GetAllTaskReward, OnGetAllTaskReward, toRegister);

            Sys_BattlePass.Instance.eventEmitter.Handle(Sys_BattlePass.EEvents.TaskProcessChange, OnGetAllTaskReward, toRegister);

            Sys_BattlePass.Instance.eventEmitter.Handle(Sys_BattlePass.EEvents.TaskReset, OnGetAllTaskReward, toRegister);
            Sys_BattlePass.Instance.eventEmitter.Handle(Sys_BattlePass.EEvents.LevelExpChange, RefreshLevel, toRegister);

            Sys_BattlePass.Instance.eventEmitter.Handle(Sys_BattlePass.EEvents.UpdateInfo, OnRefreshInfo, toRegister);

            Sys_Mall.Instance.eventEmitter.Handle<uint>(Sys_Mall.EEvents.OnFillShopData, OnShopGet, toRegister);
            Sys_Mall.Instance.eventEmitter.Handle(Sys_Mall.EEvents.OnBuyScuccess, OnBuySuccess, toRegister);


            Sys_BattlePass.Instance.eventEmitter.Handle(Sys_BattlePass.EEvents.RedState, OnRefreshRedState, toRegister);
     
        }

        private void OnRefreshInfo()
        {
            switch (m_Layout.m_BattleState)
            {
                case UI_BattlePass_Layout.BattlePassState.BattlePass_Reward:
                    RefreshReward();
                    break;
                case UI_BattlePass_Layout.BattlePassState.BattlePass_Task:
                    RefreshTask(TaskState);
                    break;
                case UI_BattlePass_Layout.BattlePassState.BattleShop:
                    RefreshShop();
                    break;
            }
            //RefreshReward();
        }
        private void OnOneKeyGetAllLevelReward()
        {
            

            UI_BattlePass_GotReward_Parma parma = new UI_BattlePass_GotReward_Parma();

            var count = Sys_BattlePass.Instance.Info.AwardList.Count;
           
            for (int i = 0; i < count; i++)
            {
                var data =  Sys_BattlePass.Instance.Info.AwardList[i];

                if(data.Common == 0)
                     UI_BattlePass_Common.GetBattlePassLevelReward(parma.GotReward, data.Id,false);

                if(data.Token == 0 && Sys_BattlePass.Instance.isVip)
                    UI_BattlePass_Common.GetBattlePassLevelVipReward(parma.GotReward, data.Id);
            }

            if (Sys_BattlePass.Instance.isVip == false)
            {
                for (uint i = 1; i <= Sys_BattlePass.Instance.Info.Level; i++)
                {
                    UI_BattlePass_Common.GetBattlePassLevelVipReward(parma.VipWillGetReward, i);
                }
            }

            UIManager.OpenUI(EUIID.UI_BattlePass_Collect, false, parma);

            //m_Layout.RefreshRewardItme();

            m_Layout.SetRewardRedDotActive(false);
            m_Layout.SetRewardOneKeyRedDot(false);

            RefreshReward();


        }

        private void OnLvelReward(uint level)
        {
            m_Layout.RefreshRewardItme();
            OnRefreshRewardRedState();
        }

        private void OnBattlePassTypeChange()
        {
            m_Layout.SetRewardLockActive(Sys_BattlePass.Instance.isVip == false);

            m_Layout.SetVipBtnActive(Sys_BattlePass.Instance.isVip == false);


            m_Layout.RefreshRewardItme();

            OnRefreshRewardRedState();
        }

        private void OnUnLockNewLevelReward()
        {
            m_Layout.RefreshRewardItme();

            RefreshLevel();

            OnRefreshRewardRedState();
        }

        private void OnGetTaskReward(uint taskId)
        {
            m_Layout.RefreshTaskItmes();
            OnRefreshRedState();
        }

        private void OnGetAllTaskReward()
        {
            m_Layout.RefreshTaskItmes();

            OnRefreshRedState();
        }

        private void OnRefreshRedState()
        {

            OnRefreshTaskRedState();

            OnRefreshRewardRedState();
        }


        private void OnRefreshTaskRedState()
        {
            var ishaveReward = HaveCanGetRewardTask();

            m_Layout.SetTaskRedDotActive(ishaveReward);

            m_Layout.SetTaskOneKeyRedDotActive(ishaveReward);
        }

        private void OnRefreshRewardRedState()
        {
            var isHadAward = Sys_BattlePass.Instance.HaveAward();
            m_Layout.SetRewardRedDotActive(isHadAward);

            m_Layout.SetRewardOneKeyRedDot(isHadAward);
        }

    }
    /// <summary>
    /// 战令奖励
    /// </summary>
    public partial class UI_BattlePass : UIBase
    {
        private uint maxCurRewardLevel = 0;

        private Timer m_ScorllMoveTimer = null;

        private void RefreshLevel()
        {
            var data = Sys_BattlePass.Instance.GetUpgradeTableData(Sys_BattlePass.Instance.Info.Level + 1);
            uint maxExp = data != null ? data.Level : 0;
            m_Layout.SetRewardLevel(Sys_BattlePass.Instance.Info.Level, Sys_BattlePass.Instance.Info.Exp, maxExp);
        }


        void RefreshReward()
        {
            var data = Sys_BattlePass.Instance.GetUpgradeTableDataMaxLevel();

            int count = (int)data.id% 1000;

            m_Layout.SetRewardCardCount(count);

            RefreshLevel();

            m_Layout.SetVipBtnActive(Sys_BattlePass.Instance.isVip == false);

            m_Layout.SetRewardLockActive(Sys_BattlePass.Instance.isVip == false);
      
            m_Layout.SetRewardFocusIndex(0);

            m_Layout.SetRewardRedDotActive(Sys_BattlePass.Instance.HaveAward());

            m_Layout.SetModelImageActive(true);

            var displaydata =Sys_BattlePass.Instance.GetRewardDisplayTableData(Sys_Role.Instance.Role.Career);

            m_Layout.SetAwardName(displaydata.Reward_Name);

            MoveToFoucs();
        }

        private void MoveToFoucs()
        {

            m_ScorllMoveTimer?.Cancel();
            m_ScorllMoveTimer = Timer.Register(0.2f, ()=> {

                var focusIndex = getLastFocueLevelIndex();
                m_Layout.SetRewardFocusIndex(focusIndex);

            });


        }
        private int getLastFocueLevelIndex()
        {
            int rewardcount = Sys_BattlePass.Instance.Info.AwardList.Count;

            var isvip = Sys_BattlePass.Instance.isVip;

            uint focusid = 0;
            for (int i = 0; i < rewardcount; i++)
            {
                var value = Sys_BattlePass.Instance.Info.AwardList[i];

                if ((isvip == false && value.Common == 0) || (isvip && value.Token == 0) )
                {
                    focusid = value.Id;
                    break;
                }
            }


            if (focusid <= 0)
            {
                focusid = Sys_BattlePass.Instance.Info.Level;
            }


            return focusid == 0 ? 0 : ((int)(focusid - 1));
        }


        private UI_BattlePass_Layout.UIRewardCardConfig GetUIRewardCardConfig(uint level)
        {
            UI_BattlePass_Layout.UIRewardCardConfig config;

            config.level = level;

            var csvData = Sys_BattlePass.Instance.GetUpgradeTableData(config.level);

            config.name = 4080u;

            var normalDrop = CSVDrop.Instance.GetDropItem(csvData.Base_Reward);

            config.normalItem = normalDrop != null && normalDrop.Count > 0 ? normalDrop[0].id : 0;
            config.normalItemCount = normalDrop != null && normalDrop.Count > 0 ? (uint)normalDrop[0].count : 0;


            var vipDrop = CSVDrop.Instance.GetDropItem(csvData.Advanced_reward);

            config.vipItem0 = vipDrop != null && vipDrop.Count > 0 ? vipDrop[0].id : 0;
            config.vipItem0Count = vipDrop != null && vipDrop.Count > 0 ? (uint)vipDrop[0].count : 0;

            config.vipItem1 = vipDrop != null && vipDrop.Count > 1 ? vipDrop[1].id : 0;
            config.vipItem1Count = vipDrop != null && vipDrop.Count > 1 ? (uint)vipDrop[1].count : 0;

            var normalState = Sys_BattlePass.Instance.GetLevelNromalRewardGetState(level);
            var vipState = Sys_BattlePass.Instance.GetLevelVipRewardGetState(level);

            config.VipState = vipState;
            config.NormalState = normalState;

            config.State = normalState == 0 && vipState == 0 ? 0 : (normalState == 1 || vipState == 1 ? 1u : 2u);

            config.IsSpceReward = csvData.Reward_Type == 2;
            return config;
        }
    }

    /// <summary>
    /// 战令任务
    /// </summary>
    public partial class UI_BattlePass : UIBase
    {
        int TaskState = 1;

        readonly uint TaskTypeDay = 1;
        readonly uint TaskTypeWeek = 2;
        readonly uint TaskTypeSeason = 4;
        private bool HaveCanGetRewardTask()
        {
           return Sys_BattlePass.Instance.HaveCanGetRewardTask();


        }
        void RefreshTask(int type) // 1 day 2 week 3 季度
        {
            TaskState = type;

            if (type == TaskTypeDay)
                RefreshDayTask();
            else if (type == TaskTypeWeek)
                RefreshWeekTask();
            else if (type == TaskTypeSeason)
                RefreshSeasonTask();

            var ishaveReward = HaveCanGetRewardTask();

            m_Layout.SetTaskRedDotActive(ishaveReward);

            m_Layout.SetModelImageActive(true);

            var displaydata = Sys_BattlePass.Instance.GetRewardDisplayTableData(Sys_Role.Instance.Role.Career);

            m_Layout.SetAwardName(displaydata.Reward_Name);

            m_Layout.SetTaskOneKeyRedDotActive(ishaveReward);
        }

        void RefreshDayTask()
        {
            Sys_BattlePass.Instance.SortTask(Sys_BattlePass.Instance.Info.DailyTasks);

           int count =  Sys_BattlePass.Instance.Info.DailyTasks.Count;

            m_Layout.SetTaskCount(count);
        }

        void RefreshWeekTask()
        {
            Sys_BattlePass.Instance.SortTask(Sys_BattlePass.Instance.Info.WeeklyTasks);

            int count = Sys_BattlePass.Instance.Info.WeeklyTasks.Count;

            m_Layout.SetTaskCount(count);
        }

        void RefreshSeasonTask()
        {
            Sys_BattlePass.Instance.SortTask(Sys_BattlePass.Instance.Info.SeasonTasks);

            int count = Sys_BattlePass.Instance.Info.SeasonTasks.Count;

            m_Layout.SetTaskCount(count);
        }
        UI_BattlePass_Layout.TaskItemConfig GetTaskConfig(int type,int index,uint id,BattlePassTask battlePassTask)
        {
            UI_BattlePass_Layout.TaskItemConfig config;

            var taskData = CSVBattlePassTaskGroup.Instance.GetConfData(id);

            config.ID = taskData == null ? 0 : taskData.id;
            config.Name = taskData == null ? string.Empty : LanguageHelper.GetTextContent(taskData.Task_Des);
           //  + config.ID.ToString() + "---" + battlePassTask.Status.ToString();
            
            config.MaxStep = taskData.ReachTypeAchievement.Count == 0 ? 0 : taskData.ReachTypeAchievement[taskData.ReachTypeAchievement.Count - 1];

            config.CurStep = (uint)Mathf.Min( battlePassTask.Process, config.MaxStep);

            if (taskData == null || taskData.Change_UI == null || taskData.Change_UI.Count == 0 || taskData.Change_UI[0] == 0)
                config.HideGoto = true;
            else
                config.HideGoto = false;

            config.State = battlePassTask.Status;
            config.Index = index;

            config.ItemID = 0;
            config.ItemCount = (uint)(taskData.Exp*(Sys_BattlePass.Instance.ActiveNtfInfo.Rate/1000f));

            return config;
        }
        
    }

    /// <summary>
    /// 战令商店
    /// </summary>
    public partial class UI_BattlePass : UIBase
    {
        private uint m_SelectShopId = 0;

        private List<uint> m_ShopItemList = null;

        private int m_SelectCount = 0;

        private List<ShopItem> m_ShopItems;

        private uint ShopID = 0;
        private void LoadShopItems()
        {
            var mallData = CSVMall.Instance.GetConfData(1001);
            var shopData = CSVShop.Instance.GetConfData(mallData.shop_id[0]);

            if (m_ShopItemList == null)
            {
                m_ShopItemList = CSVShopItem.Instance.GetShopItemList(shopData.id);
            }

            ShopID = shopData.id;
        }
        private void OnShopGet(uint shopid)
        {
            var mallData = CSVMall.Instance.GetConfData(1001);
            

            if (mallData == null || mallData.shop_id.Count == 0 || mallData.shop_id[0] != shopid)
                return;


            m_ShopItems = Sys_Mall.Instance.GetShopItems(mallData.shop_id[0]);
        }

        private void OnBuySuccess()
        {
            SetSelectCount(m_SelectCount);
        }

        void RefreshShop()
        {

            m_Layout.SetModelImageActive(false);

            if (m_ShopItems == null)
                m_ShopItems = Sys_Mall.Instance.GetShopItems(ShopID);

            m_Layout.SetShopItemCount(m_ShopItemList.Count);

            if (m_ShopItemList.Count > 0)
            {
                m_SelectShopId = 0;
                SetSeleteFocus(m_ShopItemList[0]);
            }
                

        }


        private void SetSeleteFocus(uint shopitemId)
        {
            if (shopitemId == m_SelectShopId)
                return;

            var data = CSVShopItem.Instance.GetConfData(shopitemId);

            if (data == null)
                return;

            m_SelectShopId = shopitemId;

            m_Layout.SetSelectShopItem(data, m_ShopItems.Find(o=>o.ShopItemId == shopitemId));

            SetSelectCount(1);
        }

        private void SetSelectCount(int count)
        {
            var data = CSVShopItem.Instance.GetConfData(m_SelectShopId);

            if (data == null)
                return;
            m_SelectCount = count;

            m_Layout.SetSelectShopItemCount(count, (int)data.price_now);

           
        }
    }
    public partial class UI_BattlePass : UIBase, UI_BattlePass_Layout.IListener
    {
        public void OnClickClose()
        {
            CloseSelf();
        }
        public void OnToggleReward(bool state)
        {
            if (state)
            {
                m_Layout.m_BattleState = UI_BattlePass_Layout.BattlePassState.BattlePass_Reward;
                RefreshReward();
            }
        }
        public void OnToggleTask(bool state)
        {
            if (state)
            {
                Sys_BattlePass.Instance.SendGetTaskInfo();
                m_Layout.m_BattleState = UI_BattlePass_Layout.BattlePassState.BattlePass_Task;
                m_Layout.SetTaskDailyToggleFocus();
                RefreshTask((int)TaskTypeDay);
            }
                
        }
        public void OnToggleShop(bool state)
        {
            if (state)
            {
                m_Layout.m_BattleState = UI_BattlePass_Layout.BattlePassState.BattleShop;
                RefreshShop();
            }
        }
        public void OnClickShopItemAdd()
        {
            int max = Sys_Mall.Instance.CalCanBuyMaxCount(m_SelectShopId);
            m_SelectCount += 1;
            if (m_SelectCount > max)
                m_SelectCount = max;
            SetSelectCount(m_SelectCount);
        }

        public void OnClickShopItemMax()
        {
            int max = Sys_Mall.Instance.CalCanBuyMaxCount(m_SelectShopId);
            SetSelectCount(max);
        }

        public void OnClickShopItemSub()
        {
            m_SelectCount -= 1;
            if (m_SelectCount <= 0)
                m_SelectCount = 1;
            SetSelectCount(m_SelectCount);
        }

        public void OnInputValueChanged(uint input)
        {
            int result = (int)input;

            int max = Sys_Mall.Instance.CalCanBuyMaxCount(m_SelectShopId);
            if (result <= 0)
                result = 1;
            m_SelectCount = result > max ? max : result;
            SetSelectCount(m_SelectCount);

        }

        public void OnClickShopItemSure()
        {
           var data =  CSVShopItem.Instance.GetConfData(m_SelectShopId);


            if (data.need_senior_BP && Sys_BattlePass.Instance.isVip == false)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3910010305));
                return;
            }

            if (data.need_senior_BP && Sys_BattlePass.Instance.isVip&& Sys_BattlePass.Instance.Info.Level < data.need_BP_lv)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3910010306, data.need_BP_lv.ToString()));
                return;
            }

            var hadcount =  Sys_Bag.Instance.GetItemCount(data.price_type);

            var needcount = data.price_now * m_SelectCount;

            if (hadcount < needcount)
            {
                //ExchangeCoinParm exchangeCoinParm = new ExchangeCoinParm();
                //exchangeCoinParm.ExchangeType = (uint)data.price_type;
                //exchangeCoinParm.needCount = needcount - hadcount;
                //UIManager.OpenUI(EUIID.UI_Exchange_Coin, false, exchangeCoinParm);
                //return;
            }


            Sys_BattlePass.Instance.SendBuyShop(m_SelectShopId, m_SelectCount); 
        }

        public void OnClickTaskGet(uint id, int index)
        {
            
            Sys_BattlePass.Instance.SendGetBPTAward(id,(uint)TaskState);
        }

        public void OnClickTaskGo(uint id, int index)
        {
           var data = CSVBattlePassTaskGroup.Instance.GetConfData(id);

            if (data == null || data.Change_UI == null || data.Change_UI.Count == 0 || data.Change_UI[0] == 0)
            {         
                return;
            }




            if (data.Change_UI[0] == 1)
            {
                if (data.Change_UI.Count == 3 && Sys_FunctionOpen.Instance.IsOpen(data.Change_UI[2]) == false)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3910010310));
                    return;
                }
                CloseSelf();

                UIManager.OpenUI((EUIID)(data.Change_UI[1]));
            }
            else if (data.Change_UI[0] == 3)
            {
                SkipDaily(data.Change_UI[1]);

            }
            else if (data.Change_UI[0] == 4)
            {
                int skipidcount = data.Change_UI.Count;

                var rolelevel = Sys_Role.Instance.Role.Level;

                for (int i = 1; i < skipidcount; i += 2)
                {
                    bool haveNextRange = i + 2 <= (skipidcount - 2);

                    if (haveNextRange && rolelevel >= data.Change_UI[i] && rolelevel < data.Change_UI[i + 2])
                    {
                        SkipDaily(data.Change_UI[i+1]);
                    }
                    else if(!haveNextRange)
                    {
                        SkipDaily(data.Change_UI[i+1]);
                    }
                }
            }

        }

        private void SkipDaily(uint id)
        {
            if (Sys_FunctionOpen.Instance.IsOpen(20201))
            {
                CloseSelf();
                UIManager.OpenUI(EUIID.UI_DailyActivites, false, new UIDailyActivitesParmas() { SkipToID = id, IsSkipDetail = true });
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3910010310));
            }
        }
        public void OnClickTaskOneKeyGet()
        {
            if (HaveCanGetRewardTask())
                Sys_BattlePass.Instance.SendGetBPTAwardAll();
            else
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3910010315));
        }

        public void OnTogTaskDay(bool state)
        {
            if (state)
            {
                RefreshTask((int)TaskTypeDay);
            }
        }
        public void OnTogTaskWeek(bool state)
        {
            if (state)
            {
                RefreshTask((int)TaskTypeWeek);
            }
        }


        public void OnTogTaskSeason(bool state)
        {
            if (state)
            {
                RefreshTask((int)TaskTypeSeason);
            }
        }
        public void OnRewardInfinityChange(InfinityGridCell cell, int index)
        {
            UI_BattlePass_Layout.UIRewardCardConfig config = GetUIRewardCardConfig((uint)index + 1);

            m_Layout.SetRewardCard(cell, config);

            uint slevel = Sys_BattlePass.Instance.GetSpalceLevel(config.level);

            if (slevel != maxCurRewardLevel && slevel != 0)
            {
                maxCurRewardLevel = slevel;


                UI_BattlePass_Layout.UIRewardCardConfig configPreview = GetUIRewardCardConfig(slevel);


                m_Layout.SetRewardPreView(configPreview);
            }
                
        }

        public void OnRewardLevelUp()
        {
            //var maxdata = CSVBattlePassUpgrade.Instance[CSVBattlePassUpgrade.Instance.Count - 1];

            //if (Sys_BattlePass.Instance.Info.Level >= maxdata.id)
            //{
            //    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3910010304));
            //    return;
            //}
            UIManager.OpenUI(EUIID.UI_BattlePass_Pay);
        }

        public void OnRewardOnekeyGet()
        {


            Sys_BattlePass.Instance.SendGetBPLAwardAll();
        }

        public void OnClickRewardBuyLevel()
        {
            var maxdata = Sys_BattlePass.Instance.GetUpgradeTableDataMaxLevel();
            uint maxlevel = maxdata.id % 1000;

            if (Sys_BattlePass.Instance.Info.Level >= maxlevel)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3910010304));
                return;
            }

            UIManager.OpenUI(EUIID.UI_BattlePass_LevelBuy);
        }

        public void OnClickRewardGetExp()
        {
            m_Layout.SetFacusTask();
        }

        public void OnShopInfinityChange(InfinityGridCell cell, int index)
        {
           var data = CSVShopItem.Instance.GetConfData(m_ShopItemList[index]);

            if (data == null)
                return;

            m_Layout.SetShopItem(cell,data, data.id == m_SelectShopId);
        }

        public void OnTaskInfinityChange(InfinityGridCell cell, int index)
        {
            BattlePassTask data = null;

            switch (TaskState)
            {
                case 1:
                    data = Sys_BattlePass.Instance.Info.DailyTasks[index];
                    break;
                case 2:
                    data = Sys_BattlePass.Instance.Info.WeeklyTasks[index];
                    break;
                case 4:
                    data = Sys_BattlePass.Instance.Info.SeasonTasks[index];
                    break;
            }
         

            var config = GetTaskConfig(TaskState, index, data.TaskId, data);

            m_Layout.SetTaskItem(cell, config);
        }

        public void OnClickShopItemToggle(uint id)
        {
            SetSeleteFocus(id);
        }

        public void OnClickGetReward(uint level)
        {
            Sys_BattlePass.Instance.SendGetBPLAward(level);
        }
    }
}
