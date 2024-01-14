using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;
using System;
using Lib.Core;

namespace Logic
{
    /// <summary> 运营活动 </summary>
    public enum EOperationalActivity
    {
        /// <summary> 七日登入 </summary>
        SevenDaysSign = 0,
        /// <summary> 等级礼包 </summary>
        LevelGift = 1,
        /// <summary> 成长基金 </summary>
        GrowthFund = 2,
        /// <summary> 生涯累充 </summary>
        TotalCharge = 3,
        /// <summary> 特权卡/月卡 </summary>
        SpecialCard = 4,
        /// <summary> 每日签到 </summary>
        DailySign = 5,
        /// <summary> 火焰鼠礼券 </summary>
        LotteryActivity = 6,
        /// <summary> 每日礼包 </summary>
        DailyGift = 7,
        /// <summary> 手机绑定 </summary>
        PhoneBindGift = 8,
        /// <summary>/// 调查问卷/ </summary>
        Qa = 9,
        /// <summary> 排行榜 </summary>
        RankActivity = 10,
        /// <summary> 一键加群 </summary>
        AddQQGroup = 11,
        /// <summary> 经验找回 </summary>
        ExpRetrieve = 12,
        /// <summary> 充值返利 </summary>
        Rebate = 13,
        /// <summary> 支付宝 </summary>
        Alipay = 14,
        /// <summary> 一元夺宝 </summary>
        OneDollar = 15,
        /// <summary> 百元夺宝 </summary>
        HundredDollar = 16,
        /// <summary> 运营活动奖励 </summary>
        ActivityReward = 17,
        ///// <summary> 金宠抽奖 </summary>
        PedigreedDraw = 18,
        ///// <summary> 充值选礼 </summary>
        ChargeAB = 19,
        ///// <summary> 稀有金宠抽奖 </summary>
        KingPet = 20,
        ///// <summary> 单笔充值 </summary>
        SinglePay = 21,   
        ///// <summary> 回归援助 </summary>
        BackAssist = 22,
        ///// <summary> 夺宝奇兵 </summary>
        ActivitySubscribe = 23,
        ///// <summary> 累计充值 </summary>
        ActivityTotalCharge = 24,
        ///// <summary> 累计消费 </summary>
        ActivityTotalConsume = 25,
        ///// <summary> 单笔充值合服 </summary>
        SinglePayHeFu = 26,
    }
    public class OperationalActivityPrama
    {
        public uint pageType;
        public uint openValue;
    }

    public class UI_OperationalActivity : UIBase
    {
        public interface IListener
        {
            void SetOpenValue(uint openValue);
        }
        #region 数据定义
        /// <summary> 当前菜单类型 </summary>
        private uint curOperationalActivityType = 0;
        /// <summary> 当前菜单 </summary>
        private EOperationalActivity eOperationalActivity;
        /// <summary> 子页面的开启参数(使用过后及时清空) </summary>
        private uint openValue;
        /// <summary> 当前子界面 </summary>
        private UIComponent curChildView { set; get; }

        /// <summary> 活动开关信息 </summary>
        private Dictionary<EOperationalActivity, bool> dic_ActivityOpen = new Dictionary<EOperationalActivity, bool>();
        /// <summary> 活动红点信息 </summary>
        private Dictionary<EOperationalActivity, bool> dic_ActivityRedPoint = new Dictionary<EOperationalActivity, bool>();

        /// <summary> 菜单类型 </summary>
        private Dictionary<uint, UI_ActivityMenuType> dic_TypeView = new Dictionary<uint, UI_ActivityMenuType>();
        #endregion

        #region 组件
        GameObject go_TypeItem, go_Content, go_Scroll;
        #endregion

        #region 系统函数
        protected override void OnInit()
        {

        }
        protected override void OnLoaded()
        {
            OnParseComponent();
        }
        protected override void OnDestroy()
        {
            dic_ActivityOpen.Clear();
            foreach (var item in dic_TypeView.Values)
            {
                item?.Hide();
                item?.OnDestroy();
            }
        }
        protected override void OnOpen(object arg)
        {
            if (arg != null && arg.GetType() == typeof(uint))
            {
                eOperationalActivity = (EOperationalActivity)(uint)arg;
            }
            else if (arg != null && arg.GetType() == typeof(OperationalActivityPrama))
            {
                OperationalActivityPrama prama = arg as OperationalActivityPrama;
                eOperationalActivity = (EOperationalActivity)prama.pageType;
                openValue = prama.openValue;
            }
            else
            {
                eOperationalActivity = EOperationalActivity.SevenDaysSign;
            }
        }
        protected override void OnOpened()
        {

        }

        protected override void OnShow()
        {
            Sys_Sign.Instance.SignDailySignInfoReq();
            Sys_Sign.Instance.CheckUpdateOnOpenUI();
            InitShowOrHideAllMenu();
            RefreshRedPoint();
            InitMenuView();
        }
        protected override void OnHide()
        {

        }
        protected override void OnUpdate()
        {
            curChildView?.ExecUpdate();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateSignSevenDayData, OnUpdateSevenDaysSign, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle<uint>(Sys_OperationalActivity.EEvents.ReceiveSignReward, OnUpdateReceiveSignReward, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateLevelGiftData, OnUpdateLevelGiftData, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateGrowthFundData, OnUpdateGrowthFundData, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateTotalChargeData, OnUpdateTotalChargeData, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateSpecialCardData, OnUpdateSpecialCardData, toRegister);
            Sys_Sign.Instance.eventEmitter.Handle(Sys_Sign.EEvents.UpdateDailySignAwardCount, OnUpdateDailySignAwardCount, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateLotteryActivityData, OnUpdateLotteryActivityData, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateRankActivityData, OnUpdateRankActivityData, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateDailyGiftData, OnUpdateDailyGiftData, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdatePhoneBindStatus, OnUpdateBindPhoneGiftData, toRegister);
            Sys_Qa.Instance.eventEmitter.Handle(Sys_Qa.EEvents.OnRefreshQARedPoint, OnRefreshQaRed, toRegister);
            Sys_ActivitySubscribe.Instance.eventEmitter.Handle(Sys_ActivitySubscribe.EEvents.OnRefreshRedPoint, OnRefreshActivitySubscribeRed, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateExpRetrieveData, OnUpdateExpRetrieve, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateRebateData, OnUpdateRebateData, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateActivityRewardData, OnUpdateActivityRewardData, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle<uint>(Sys_OperationalActivity.EEvents.UpdateFightTreasureData, OnUpdateFightTreasureData, toRegister);
            Sys_PedigreedDraw.Instance.eventEmitter.Handle(Sys_PedigreedDraw.EEvents.OnUpdateTaskAward, OnUpdatePedigreedDrawData, toRegister);

            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateOperatinalActivityShowOrHide, UpdateActivityOpen, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateChargeABData, OnUpdateChargeABData, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.OnChargeABActivityEnd, UpdateActivityOpen, toRegister);
            Sys_SinglePay.Instance.eventEmitter.Handle(Sys_SinglePay.EEvents.OnSinglePayEnd, UpdateActivityOpen, toRegister);
            Sys_SinglePay.Instance.eventEmitter.Handle(Sys_SinglePay.EEvents.OnSinglePayHeFEnd, UpdateActivityOpen, toRegister);
            Sys_SinglePay.Instance.eventEmitter.Handle(Sys_SinglePay.EEvents.OnSinglePayDataUpdate, OnUpdateSinglePayData, toRegister);
            Sys_SinglePay.Instance.eventEmitter.Handle(Sys_SinglePay.EEvents.OnSinglePayHeFuDataUpdate, OnUpdateSinglePayData, toRegister);
            Sys_BackAssist.Instance.eventEmitter.Handle(Sys_BackAssist.EEvents.ActivityReturnRedPoint, OnUpdateBackAssistData, toRegister);

            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateActivityTotalCharge, OnUpdateActivityTotalCharge, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateActivityTotalConsume, OnUpdateActivityTotalConsume, toRegister);
        }
        #endregion

        #region 初始化
        private void OnParseComponent()
        {
            go_TypeItem = transform.Find("Animator/Menu_New/Scroll01/Toggle_Select01").gameObject;
            go_Content = transform.Find("Animator/Menu_New/Scroll01/Content").gameObject;
            go_Scroll= transform.Find("Animator/Menu_New/Scroll01").gameObject;
            CP_ToggleRegistry group = go_Scroll.GetComponent<CP_ToggleRegistry>();
            group.allowSwitchOff = true;
            transform.Find("Animator/View_Title07/Btn_Close").GetComponent<Button>().onClick.AddListener(OnClick_Close);

            curChildView = null;
            go_TypeItem.SetActive(false);
        }

        private void InitMenuView()
        {
            if (curChildView == null)
            {
                dic_TypeView.Clear();
                //var menuType = Sys_WelfareMenu.Instance.MenuType;

                var typeList = CSVWelfareType.Instance.GetAll();
                for (int i = 0; i < typeList.Count; i++)
                {
                    CSVWelfareType.Data typeData = typeList[i];
                    uint typeID = typeData.id;
                    if (CSVWelfareType.Instance.TryGetValue(typeID, out typeData))
                    {
                        GameObject obj = FrameworkTool.CreateGameObject(go_TypeItem, go_Content);
                        obj.name = typeID.ToString();
                        UI_ActivityMenuType ceil = AddComponent<UI_ActivityMenuType>(obj.transform);
                        ceil?.SetData(typeData, dic_ActivityOpen, dic_ActivityRedPoint);
                        ceil.AddListener(OnClick_MenuType, OnClick_MenuSub);
                        dic_TypeView.Add(typeID, ceil);
                    }
                }
                SetMenu(eOperationalActivity);
                ForceRebuildLayout(go_Scroll);
            }
        }

        private void SetMenu(EOperationalActivity menu)
        {
            CSVWelfareMenu.Data data = CSVWelfareMenu.Instance.GetConfData((uint)menu + 1);
            if (data == null)
                return;
            uint type = data.menuId;
            dic_TypeView[type].SetMenu(menu, true);
        }
        #endregion

        #region OnClick
        private void OnClick_MenuType(uint typeID)
        {
            if (typeID != curOperationalActivityType)
            {
                if (curOperationalActivityType != 0)
                {
                    dic_TypeView[typeID].SetMenu(eOperationalActivity);
                }
                curOperationalActivityType = typeID;
                ForceRebuildLayout(go_Scroll);
            }
        }

        private void OnClick_MenuSub(UI_ActivityMenuSub menuSub, bool isOn)
        {
            EOperationalActivity eMenu = (EOperationalActivity)menuSub.menuData.id - 1;
            OnClick_Menu(menuSub, eMenu, isOn);
        }

        private void OnClick_Menu(UI_ActivityMenuSub menuSub, EOperationalActivity menu, bool value)
        {
            if (value)
            {
                eOperationalActivity = menu;
                menuSub.InitUIComponent();
                if (openValue > 0)
                {
                    menuSub.uiComponent?.SetOpenValue(openValue);
                    openValue = 0;
                }
                menuSub.uiComponent?.Show();
                if (curChildView != menuSub.uiComponent)
                {
                    curChildView?.Hide();
                    curChildView = menuSub.uiComponent;
                }

                UIManager.HitPointShow(EUIID.UI_OperationalActivity, menu.ToString());
            }

            if (menu == EOperationalActivity.PhoneBindGift && value)
            {
                Sys_OperationalActivity.Instance.IsClickBindPhone = true;
                OnUpdateBindPhoneGiftData();
            }
            if (menu == EOperationalActivity.ExpRetrieve && value)
            {
                OnUpdateExpRetrieve();
            }
        }

        public void OnClick_Close()
        {
            CloseSelf();
        }

        private void ForceRebuildLayout(GameObject go)
        {
            ContentSizeFitter[] fitter = go.GetComponentsInChildren<ContentSizeFitter>();
            for (int i = fitter.Length - 1; i >= 0; --i)
            {
                RectTransform trans = fitter[i].gameObject.GetComponent<RectTransform>();
                if (trans != null)
                    LayoutRebuilder.ForceRebuildLayoutImmediate(trans);
            }
        }
        #endregion

        #region CheckFunOpen
        private void UpdateActivityOpen()
        {
            InitShowOrHideAllMenu();
            bool isCloseCur = false;
            if (dic_ActivityOpen.TryGetValue(eOperationalActivity, out bool isShow))
            {
                if (!isShow)
                {
                    isCloseCur = true;
                }
            }
            foreach (var item in dic_TypeView.Values)
            {
                if (isCloseCur)
                {
                    int eMenu = item.GetIsHaveOpenMenu();
                    if (eMenu != -1)
                    {
                        item.SetMenu((EOperationalActivity)eMenu, true);
                        isCloseCur = false;
                    }
                }
                item.UpdateTypeMenuOpen();
            }
            ForceRebuildLayout(go_Scroll);
        }
        private void InitShowOrHideAllMenu()
        {
            SetShowOrHideMenu(EOperationalActivity.SevenDaysSign, Sys_OperationalActivity.Instance.IsShowSignSevenDay());
            SetShowOrHideMenu(EOperationalActivity.PhoneBindGift, Sys_OperationalActivity.Instance.BindPhoneIsOpen());
            SetShowOrHideMenu(EOperationalActivity.AddQQGroup, Sys_OperationalActivity.Instance.IsShowQQGroup());
            SetShowOrHideMenu(EOperationalActivity.Qa, Sys_Qa.Instance.HasQa() && SDKManager.IsOpenGetExtJsonParam(SDKManager.EThirdSdkType.QuestionSurvey.ToString(), out string paramsValue));
            SetShowOrHideMenu(EOperationalActivity.ExpRetrieve, Sys_OperationalActivity.Instance.IsShowExpRetrieve());
            SetShowOrHideMenu(EOperationalActivity.GrowthFund, Sys_OperationalActivity.Instance.CheckGrowthFundIsOpen());
            SetShowOrHideMenu(EOperationalActivity.TotalCharge, Sys_OperationalActivity.Instance.CheckTotalChargeIsOpen());
            SetShowOrHideMenu(EOperationalActivity.SpecialCard, Sys_OperationalActivity.Instance.CheckSpecialCardIsOpen());
            SetShowOrHideMenu(EOperationalActivity.LotteryActivity, Sys_OperationalActivity.Instance.CheckLotteryActivityIsOpen());
            SetShowOrHideMenu(EOperationalActivity.LevelGift, Sys_OperationalActivity.Instance.CheckLevelGiftIsOpen());
            SetShowOrHideMenu(EOperationalActivity.DailySign, Sys_Sign.Instance.CheckDailySignIsOpen());
            SetShowOrHideMenu(EOperationalActivity.DailyGift, Sys_OperationalActivity.Instance.CheckDailyGiftIsOpen());
            SetShowOrHideMenu(EOperationalActivity.Rebate, Sys_OperationalActivity.Instance.CheckRebateisOpen());
            SetShowOrHideMenu(EOperationalActivity.Alipay, Sys_OperationalActivity.Instance.CheckAlipayActivityIsOpen());
            SetShowOrHideMenu(EOperationalActivity.OneDollar, Sys_OperationalActivity.Instance.CheckOneDollarIsOpen());
            SetShowOrHideMenu(EOperationalActivity.HundredDollar, Sys_OperationalActivity.Instance.CheckHundredDollarIsOpen());
            SetShowOrHideMenu(EOperationalActivity.PedigreedDraw, Sys_PedigreedDraw.Instance.CheckPedigreedIsOpen());
            SetShowOrHideMenu(EOperationalActivity.ActivityReward, Sys_OperationalActivity.Instance.CheckActivityRewardIsOpen());
            SetShowOrHideMenu(EOperationalActivity.ChargeAB, Sys_OperationalActivity.Instance.CheckChargeABIsOpen());
            SetShowOrHideMenu(EOperationalActivity.KingPet, Sys_OperationalActivity.Instance.CheckKingPetActivityIsOpen());
            SetShowOrHideMenu(EOperationalActivity.SinglePay, Sys_SinglePay.Instance.CheckSinglePayIsOpen());
            SetShowOrHideMenu(EOperationalActivity.ActivitySubscribe, Sys_ActivitySubscribe.Instance.CheckIsOpen());
            SetShowOrHideMenu(EOperationalActivity.BackAssist, Sys_BackAssist.Instance.CheckActivityReturnLovePointOpen());
            SetShowOrHideMenu(EOperationalActivity.ActivityTotalCharge, Sys_OperationalActivity.Instance.CheckActivityTotalChargeIsOpen());
            SetShowOrHideMenu(EOperationalActivity.ActivityTotalConsume, Sys_OperationalActivity.Instance.CheckActivityTotalConsumeIsOpen());
            SetShowOrHideMenu(EOperationalActivity.SinglePayHeFu, Sys_SinglePay.Instance.CheckSinglePayHefuIsOpen());

        }

        private void SetShowOrHideMenu(EOperationalActivity menu, bool isShow)
        {
            if (dic_ActivityOpen.ContainsKey(menu))
                dic_ActivityOpen[menu] = isShow;
            else
                dic_ActivityOpen.Add(menu, isShow);
        }
        #endregion

        #region RedPoint
        /// <summary> 刷新红点 </summary>
        private void RefreshRedPoint()
        {
            OnUpdateSevenDaysSign();
            OnUpdateLevelGiftData();
            OnUpdateGrowthFundData();
            OnUpdateTotalChargeData();
            OnUpdateSpecialCardData();
            OnUpdateDailySignAwardCount();
            OnUpdateLotteryActivityData();
            OnUpdateDailyGiftData();
            OnUpdateBindPhoneGiftData();
            OnRefreshQaRed();
            OnRefreshActivitySubscribeRed();
            OnUpdateRankActivityData();
            OnUpdateQQGroupData();
            OnUpdateExpRetrieve();
            OnUpdateRebateData();
            OnUpdateActivityRewardData();
            OnUpdateFightTreasureData(0);
            OnUpdatePedigreedDrawData();
            OnUpdateChargeABData();
            OnUpdateSinglePayData();
            OnUpdateBackAssistData();
            OnUpdateActivityTotalCharge();
            OnUpdateActivityTotalConsume();
        }
        private void RefreshMenuRedPoint(EOperationalActivity menu, bool isShow)
        {        
            if (dic_ActivityRedPoint.ContainsKey(menu))
                dic_ActivityRedPoint[menu] = isShow;
            else
                dic_ActivityRedPoint.Add(menu, isShow);

            CSVWelfareMenu.Data data = CSVWelfareMenu.Instance.GetConfData((uint)menu + 1);
            if (data != null && dic_TypeView.TryGetValue(data.menuId, out UI_ActivityMenuType typeView))
                typeView?.RefreshRedPoint(menu);
        }
        #endregion

        #region Event

        private void OnUpdateReceiveSignReward(uint Id)
        {
            RefreshMenuRedPoint(EOperationalActivity.SevenDaysSign, Sys_OperationalActivity.Instance.CheckSevenDaysSignShowRedPoint());
        }

        private void OnUpdateSevenDaysSign()
        {
            RefreshMenuRedPoint(EOperationalActivity.SevenDaysSign, Sys_OperationalActivity.Instance.CheckSevenDaysSignShowRedPoint());
        }

        /// <summary>等级礼包数据刷新 </summary>
        private void OnUpdateLevelGiftData()
        {
            RefreshMenuRedPoint(EOperationalActivity.LevelGift, Sys_OperationalActivity.Instance.CheckLevelGiftShowRedPoint());
        }
        /// <summary>成长基金数据刷新</summary>
        private void OnUpdateGrowthFundData()
        {
            RefreshMenuRedPoint(EOperationalActivity.GrowthFund, Sys_OperationalActivity.Instance.CheckGrowthFundShowRedPoint());
        }
        /// <summary>生涯累充数据刷新</summary>
        private void OnUpdateTotalChargeData()
        {
            RefreshMenuRedPoint(EOperationalActivity.TotalCharge, Sys_OperationalActivity.Instance.CheckTotlaChargeGiftRedPoint());
        }
        /// <summary>月卡数据刷新</summary>
        private void OnUpdateSpecialCardData()
        {
            RefreshMenuRedPoint(EOperationalActivity.SpecialCard, Sys_OperationalActivity.Instance.CheckSpecialCardRedPoint());
        }

        private void OnUpdateDailySignAwardCount()
        {
            RefreshMenuRedPoint(EOperationalActivity.DailySign, Sys_Sign.Instance.CheckDailySignRedPoint());
        }
        /// <summary>大地鼠活动页签刷新刷新</summary>
        private void OnUpdateLotteryActivityData()
        {
            RefreshMenuRedPoint(EOperationalActivity.LotteryActivity, Sys_OperationalActivity.Instance.CheckLotteryActivityRedPoint());
        }
        /// <summary>排行榜活动页签刷新刷新</summary>
        private void OnUpdateRankActivityData()
        {
            RefreshMenuRedPoint(EOperationalActivity.RankActivity, Sys_OperationalActivity.Instance.CheckRankActivityRedPoint());
        }

        /// <summary>每日礼包刷新</summary>
        private void OnUpdateDailyGiftData()
        {
            RefreshMenuRedPoint(EOperationalActivity.DailyGift, Sys_OperationalActivity.Instance.CheckDailyGiftRedPoint());
        }

        /// <summary>手机绑定刷新</summary>
        private void OnUpdateBindPhoneGiftData()
        {
            RefreshMenuRedPoint(EOperationalActivity.PhoneBindGift, Sys_OperationalActivity.Instance.CheckBindPhoneRedPoint());
        }

        private void OnRefreshQaRed()
        {
            RefreshMenuRedPoint(EOperationalActivity.Qa, Sys_Qa.Instance.HasQa() && SDKManager.IsOpenGetExtJsonParam(SDKManager.EThirdSdkType.QuestionSurvey.ToString(), out string paramsValue) && !Sys_Qa.Instance.hasShowRedPoint);
        }
        private void OnRefreshActivitySubscribeRed()
        {
            RefreshMenuRedPoint(EOperationalActivity.ActivitySubscribe, Sys_ActivitySubscribe.Instance.redPoint);
        }

        /// <summary>一键加群</summary>
        private void OnUpdateQQGroupData()
        {
            RefreshMenuRedPoint(EOperationalActivity.AddQQGroup, Sys_OperationalActivity.Instance.CheckQQGroupRedPoint());
        }
        /// <summary>经验找回</summary>
        private void OnUpdateExpRetrieve()
        {
            RefreshMenuRedPoint(EOperationalActivity.ExpRetrieve, Sys_OperationalActivity.Instance.CheckExpRetrieveRedPoint());
        }

        /// <summary> 充值返利 /// </summary>
        private void OnUpdateRebateData()
        {
            RefreshMenuRedPoint(EOperationalActivity.Rebate, Sys_OperationalActivity.Instance.CheckRebateRedPoint());
        }
        /// <summary> 运营活动奖励 /// </summary>
        private void OnUpdateActivityRewardData()
        {
            RefreshMenuRedPoint(EOperationalActivity.ActivityReward, Sys_OperationalActivity.Instance.CheckActivityRewardRedPoint());
        }
        /// <summary> 夺宝活动 /// </summary>
        private void OnUpdateFightTreasureData(uint _id)
        {
            RefreshMenuRedPoint(EOperationalActivity.OneDollar, Sys_OperationalActivity.Instance.CheckOneDollarRedPoint());
            RefreshMenuRedPoint(EOperationalActivity.HundredDollar, Sys_OperationalActivity.Instance.CheckHundredDollarRedPoint());
        }

        private void OnUpdatePedigreedDrawData()
        {
            RefreshMenuRedPoint(EOperationalActivity.PedigreedDraw, Sys_PedigreedDraw.Instance.HasRewardUnGet());
        }
        /// <summary>充值选礼刷新</summary>
        private void OnUpdateChargeABData()
        {
            RefreshMenuRedPoint(EOperationalActivity.ChargeAB, Sys_OperationalActivity.Instance.CheckChargeABRedPoint());
        }
        /// <summary>充值选礼刷新</summary>
        private void OnUpdateSinglePayData()
        {
            RefreshMenuRedPoint(EOperationalActivity.SinglePay, Sys_SinglePay.Instance.CheckSinglePlayRedPoint());
            RefreshMenuRedPoint(EOperationalActivity.SinglePayHeFu, Sys_SinglePay.Instance.CheckSinglePayHeFuRedPoint());
        }
        /// <summary>回归援助</summary>
        private void OnUpdateBackAssistData()
        {
            RefreshMenuRedPoint(EOperationalActivity.BackAssist, Sys_BackAssist.Instance.ActivityReturnLovePointRedPoint());
        }
        private void OnUpdateActivityTotalCharge()
        {
            RefreshMenuRedPoint(EOperationalActivity.ActivityTotalCharge, Sys_OperationalActivity.Instance.CheckActivityTotalChargeRedPoint());
        }
        private void OnUpdateActivityTotalConsume()
        {
            RefreshMenuRedPoint(EOperationalActivity.ActivityTotalConsume, Sys_OperationalActivity.Instance.CheckActivityTotalConsumeRedPoint());
        }
        #endregion
    }

    /// <summary>
    /// 福利菜单类型-一级菜单
    /// </summary>
    public class UI_ActivityMenuType : UIComponent
    {
        CSVWelfareType.Data data;
        Dictionary<EOperationalActivity, bool> dic_ActivityOpen;
        Dictionary<EOperationalActivity, bool> dic_ActivityRedPoint;
        Dictionary<EOperationalActivity, UI_ActivityMenuSub> dic_MenuSubView = new Dictionary<EOperationalActivity, UI_ActivityMenuSub>();

        private CP_Toggle toggle;
        private GameObject image_Select, go_MenuItem, go_ContentSmall,go_RedPoint;
        private Text text_Dark, text_Light,text_RedPoint;

        private Action<uint> menuTypeAction;
        private Action<UI_ActivityMenuSub, bool> menuSubAction;

        bool isInitSubList;
        bool isShow = false;
        bool isUpdateMenu = false;

        #region 系统函数
        protected override void Loaded()
        {
            base.Loaded();
            text_Dark = transform.Find("GameObject/Text_Dark").GetComponent<Text>();
            text_Light = transform.Find("GameObject/Image_Select/Text_Light").GetComponent<Text>();
            image_Select = transform.Find("GameObject/Image_Select").gameObject;
            go_MenuItem = transform.Find("Toggle_Select01").gameObject;
            go_ContentSmall = transform.Find("Content_Small").gameObject;
            go_RedPoint = transform.Find("Image_Sign").gameObject;
            text_RedPoint = transform.Find("Image_Sign/Text").GetComponent<Text>();
            toggle = transform.GetComponent<CP_Toggle>();
            toggle.onValueChanged.AddListener(OnClick_Toggle);

            go_RedPoint.SetActive(false);
            image_Select.SetActive(false);
            go_MenuItem.SetActive(false);
        }
        public override void SetData(params object[] arg)
        {
            base.SetData();
            if (arg != null && arg.Length > 0)
            {
                data = (CSVWelfareType.Data)arg[0];
                dic_ActivityOpen = (Dictionary<EOperationalActivity, bool>)arg[1];
                dic_ActivityRedPoint= (Dictionary<EOperationalActivity, bool>)arg[2];
                ShowTypeItem();
            }
        }
        public override void Show()
        {
            base.Show();
        }
        public override void Hide()
        {
            base.Hide();
            foreach (var item in dic_MenuSubView.Values)
            {
                item.uiComponent?.Hide();
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            foreach (var item in dic_MenuSubView.Values)
            {
                item.uiComponent?.OnDestroy();
            }
        }
        #endregion

        private void ShowTypeItem()
        {
            if (data == null)
                return;
            text_Dark.text = LanguageHelper.GetTextContent(data.lanId);
            text_Light.text= LanguageHelper.GetTextContent(data.lanId);

            UpdateTypeRedPoint();
            ShowHide(isShow);
        }

        #region UpdateRedPoint
        public void RefreshRedPoint(EOperationalActivity menu)
        {
            if (data == null)
                return;
            CSVWelfareMenu.Data csvmenudata = CSVWelfareMenu.Instance.GetConfData((uint)menu + 1);
            if (csvmenudata != null && csvmenudata.menuId == data.id)
            {
                dic_ActivityRedPoint.TryGetValue(menu, out bool isRedPoint);
                if (dic_MenuSubView.TryGetValue(menu, out UI_ActivityMenuSub view))
                    view?.RefreshRedPoint(isRedPoint);
                UpdateTypeRedPoint();
            }
        }
        private void UpdateTypeRedPoint()
        {
            var list_menu = Sys_WelfareMenu.Instance.MenuType[data.id];
            int redNum = 0;
            isShow = false;
            for (int i = 0; i < list_menu.Count; i++)
            {
                CSVWelfareMenu.Data menuData = list_menu[i];
                EOperationalActivity eActive = (EOperationalActivity)(menuData.id - 1);
                bool isOpen = CheckFunctionIsOpen(eActive);
                dic_ActivityRedPoint.TryGetValue(eActive, out bool isRedPoint);
                if (isRedPoint && isOpen)
                    redNum++;
                isShow |= isOpen;
            }
            go_RedPoint.SetActive(redNum > 0);
            text_RedPoint.text = redNum.ToString();
        }
        #endregion

        private void SetSubList()
        {
            if (isInitSubList)
                return;
            var menuType = Sys_WelfareMenu.Instance.MenuType;
            dic_MenuSubView.Clear();
            if (menuType.TryGetValue(data.id,out List<CSVWelfareMenu.Data> list))
            {
                list.Sort((x, y) => x.OrderId.CompareTo(y.OrderId));
                for (int i = 0; i < list.Count; i++)
                {
                    CSVWelfareMenu.Data menuData = list[i];
                    EOperationalActivity eMenu = (EOperationalActivity)(menuData.id - 1);
                    bool isShow = CheckFunctionIsOpen(eMenu);
                    dic_ActivityRedPoint.TryGetValue(eMenu, out bool isRedPoint);

                    GameObject obj = FrameworkTool.CreateGameObject(go_MenuItem, go_ContentSmall);
                    obj.name = menuData.id.ToString();
                    UI_ActivityMenuSub ceil = AddComponent<UI_ActivityMenuSub>(obj.transform);
                    ceil.AddListener(OnClick_SubToggle);
                    dic_MenuSubView.Add(eMenu, ceil);
                    if (isShow)
                        ceil.SetData(menuData, isRedPoint);
                    ceil.ShowHide(isShow);
                }
            }
            isInitSubList = true;
        }

        private void OnClick_Toggle(bool arg0)
        {
            if (arg0)
            {
                SetSubList();
                menuTypeAction?.Invoke(data.id);
                ResetMenu();
            }
            image_Select.SetActive(arg0);
            go_ContentSmall.SetActive(arg0);
            ForceRebuildLayout(transform.gameObject);
        }

        private void OnClick_SubToggle(UI_ActivityMenuSub ui_MenuSub,bool isOn)
        {
            if (ui_MenuSub != null)
            {
                menuSubAction?.Invoke(ui_MenuSub, isOn);
            }
        }

        public void AddListener(Action<uint> onClick_MenuType, Action<UI_ActivityMenuSub, bool> onClick_MenuSub)
        {
            menuTypeAction = onClick_MenuType;
            menuSubAction = onClick_MenuSub;
        }

        private void ForceRebuildLayout(GameObject go)
        {
            ContentSizeFitter[] fitter = go.GetComponentsInChildren<ContentSizeFitter>();
            for (int i = fitter.Length - 1; i >= 0; --i)
            {
                RectTransform trans = fitter[i].gameObject.GetComponent<RectTransform>();
                if (trans != null)
                    LayoutRebuilder.ForceRebuildLayoutImmediate(trans);
            }
        }

        public void SetMenu(EOperationalActivity eMenu = EOperationalActivity.SevenDaysSign, bool isOnClick = false)
        {
            if(isOnClick)
                toggle.SetSelected(true, true);
            bool isShow = CheckFunctionIsOpen(eMenu);
            if (isShow && dic_MenuSubView.TryGetValue(eMenu, out UI_ActivityMenuSub menuView))
                menuView?.toggle.SetSelected(true, true);
            else
            {
                foreach (var item in dic_MenuSubView.Keys)
                {
                    bool isOpen = CheckFunctionIsOpen(item);
                    if (isOpen)
                    {
                        dic_MenuSubView[item].toggle.SetSelected(true, true);
                        break;
                    }
                }
            }
        }

        private bool CheckFunctionIsOpen(EOperationalActivity menu)
        {
            if (dic_ActivityOpen.TryGetValue(menu, out bool isOpen))
                return isOpen;
            else
            {
                CSVWelfareMenu.Data data = CSVWelfareMenu.Instance.GetConfData((uint)menu + 1);
                if (data != null && data.functionId != 0)
                    return Sys_FunctionOpen.Instance.IsOpen(data.functionId);
            }
            return false;
        }

        #region UpdateOpen
        /// <summary>
        /// 更新紧急开关
        /// </summary>
        public void UpdateTypeMenuOpen()
        {
            isUpdateMenu = true;
            ResetMenu();
            UpdateTypeRedPoint();
            ShowHide(isShow);
        }
        private void ResetMenu()
        {
            if (isUpdateMenu && toggle.IsOn)
            {
                foreach (var item in dic_MenuSubView)
                {
                    EOperationalActivity eMenu = item.Key;
                    UI_ActivityMenuSub menuView = item.Value;
                    bool isOpen = CheckFunctionIsOpen(eMenu);
                    dic_ActivityRedPoint.TryGetValue(eMenu, out bool isRedPoint);
                    CSVWelfareMenu.Data data = CSVWelfareMenu.Instance.GetConfData((uint)eMenu + 1);
                    if (!menuView.isInit)
                        menuView.SetData(data, isRedPoint);
                    menuView.ShowHide(isOpen);
                }
                isUpdateMenu = false;
                ForceRebuildLayout(transform.gameObject);
            }
        }

        public int GetIsHaveOpenMenu()
        {
            foreach (var item in dic_MenuSubView.Keys)
            {
                if (CheckFunctionIsOpen(item))
                    return (int)item;
            }
            return -1;
        }
        #endregion
    }

    /// <summary>
    /// 福利功能菜单-二级菜单
    /// </summary>
    public class UI_ActivityMenuSub : UIComponent
    {
        public CP_ToggleEx toggle;
        private GameObject image_Select, go_RedPoint;
        private Text text_Dark, text_Light;

        Action<UI_ActivityMenuSub, bool> action;
        EOperationalActivity eActivity;

        public UI_OperationalActivityBase uiComponent;
        public CSVWelfareMenu.Data menuData;
        public bool isInit = false;

        protected override void Loaded()
        {
            base.Loaded();
            text_Dark = transform.Find("Text_Dark").GetComponent<Text>();
            text_Light = transform.Find("Image_Select/Text_Light").GetComponent<Text>();
            image_Select = transform.Find("Image_Select").gameObject;
            go_RedPoint = transform.Find("Image_Dot").gameObject;
            toggle = transform.GetComponent<CP_ToggleEx>();
            toggle.onValueChanged.AddListener(OnClick_Toggle);
            image_Select.SetActive(false);
        }

        public void SetData(CSVWelfareMenu.Data data, bool redPoint = false)
        {
            menuData = data;
            text_Dark.text = LanguageHelper.GetTextContent(data.lanId);
            text_Light.text = LanguageHelper.GetTextContent(data.lanId);
            RefreshRedPoint(redPoint);
            eActivity = (EOperationalActivity)data.id - 1;
            isInit = true;
        }

        public void InitUIComponent()
        {
            if(uiComponent == null)
            {
                uiComponent = Sys_WelfareMenu.Instance.CreateOperationUIComponent(eActivity);
                Transform uiTran = Sys_WelfareMenu.Instance.CreateOperationCellGameobject(menuData.prefabNode).transform;
                uiComponent.Init(uiTran.transform);
                uiComponent.Hide();
            }
        }

        private void OnClick_Toggle(bool isOn)
        {
            action?.Invoke(this, isOn);
        }

        public void AddListener(Action<UI_ActivityMenuSub, bool> subAction)
        {
            action = subAction;
        }

        public void RefreshRedPoint(bool isRedPoint)
        {
            go_RedPoint.SetActive(isRedPoint);
        }
    }
}