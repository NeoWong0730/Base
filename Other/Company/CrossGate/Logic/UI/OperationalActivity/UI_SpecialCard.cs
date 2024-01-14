using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine.UI;
using UnityEngine;
using System;
using Lib.Core;
using Framework;
using Logic.Core;
using Packet;
using UnityEngine.Playables;

namespace Logic
{
    public enum ESpecialCardState
    {
        /// <summary> 立即获得魔币 </summary>
        GetCurrency = 0,
        /// <summary> 挂机(巡逻托管) </summary>
        AFK = 1,
        /// <summary> 捕捉成功率 </summary>
        CatchProbabily = 2,
        /// <summary> 随身银行 </summary>
        WithBank = 3,
        /// <summary> 额外交易栏位 </summary>
        TransactionTabs = 4,
        /// <summary> 签到转盘抽奖次数(原签到额外奖励) </summary>
        SignIn = 5,
        /// <summary> 世界聊天不消耗活力 </summary>
        Chat = 6,
        /// <summary> 每日额外特权礼包礼包 </summary>
        SpecialGift = 7,
    }

    public class UI_SpecialCard : UI_OperationalActivityBase
    {

        private UI_SpecialCardCell leftCell;
        private UI_SpecialCardCell rightCell;
        private Button btnGo;

        /// <summary> 月卡ID列表 </summary>
        private List<uint> listCardIds = new List<uint>();
        //private InfinityGrid infinityTabList;
        //private Dictionary<GameObject, UI_SpecialCardTab> dictTabs = new Dictionary<GameObject, UI_SpecialCardTab>();

        /// <summary> 已同步的月卡id列表(只在界面打开刷新时记录) </summary>
        private List<uint> refreshedCardIds = new List<uint>();


        #region 系统函数
        protected override void Loaded()
        {
        }
        protected override void InitBeforOnShow()
        {
            Parse();
        }
        public override void OnDestroy()
        {
            leftCell?.TimerCancel();
            rightCell?.TimerCancel();
            base.OnDestroy();
        }
        public override void Show()
        {
            base.Show();
            UpdateView();
        }
        public override void Hide()
        {
            leftCell?.TimerCancel();
            rightCell?.TimerCancel();
            refreshedCardIds.Clear();
            base.Hide();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateSpecialCardData, OnUpdateSpecialCardData, toRegister);
            Sys_Time.Instance.eventEmitter.Handle<uint, uint>(Sys_Time.EEvents.OnTimeNtf, OnTimeNtf, toRegister);
        }
        public override void SetOpenValue(uint openValue)
        {
            //selectCardId = openValue;
        }
        #endregion
        #region func
        private void Parse()
        {
            leftCell = new UI_SpecialCardCell();
            leftCell.Init(transform.Find("View_left"));
            rightCell = new UI_SpecialCardCell();
            rightCell.Init(transform.Find("View_Right"));
            btnGo = transform.Find("Button_Go").GetComponent<Button>();
            btnGo.onClick.AddListener(OnBtnGoClick);
        }
        private void UpdateView()
        {
            listCardIds = Sys_OperationalActivity.Instance.ListSpecialCardIds;

            var leftCardId = listCardIds[0];
            RefreshSpecilCardInfo(leftCardId);
            leftCell.UpdateCellView(leftCardId);

            var rightCardId = listCardIds[1];
            RefreshSpecilCardInfo(rightCardId);
            rightCell.UpdateCellView(rightCardId);
        }
        private void RefreshSpecilCardInfo(uint id)
        {
            if (!refreshedCardIds.Contains(id))
            {
                refreshedCardIds.Add(id);
                Sys_OperationalActivity.Instance.ReqRefreshSpecialCardInfo(id);
            }
        }

        #endregion
        #region event
        private void OnUpdateSpecialCardData()
        {
            UpdateView();
        }

        private void OnTimeNtf(uint oldTime, uint newTime)
        {
            UpdateView();
        }

        private void OnBtnGoClick()
        {
            //跳转点卡商城
            MallPrama param = new MallPrama();
            param.mallId = 101u;
            param.shopId = 1002u;
            UIManager.OpenUI(EUIID.UI_Mall, false, param);
        }
        #endregion


        public class UI_SpecialCardCell
        {
            private Transform transform;

            private Image imgIcon;
            private Button btnIcon;
            private GameObject goRedDot;
            private GameObject goIsGet;
            private Text txtTips;
            private Button btnBuy;
            private Text txtBtn;
            private Button btnPresent;//赠送按钮
            //private GameObject goIsGet;
            private GameObject goIsFirst;
            private Text txtTime;
            private List<List<uint>> ListSpecialDesc = new List<List<uint>>();
            private InfinityGrid infinityDesc;
            private Dictionary<GameObject, UI_SpecialCardDecsCell> dictDesc = new Dictionary<GameObject, UI_SpecialCardDecsCell>();
            private Animator ani;

            private uint cardId;
            CSVMonthCard.Data cardData;

            private Timer timer;
            private float countDownTime = 0;

            public void Init(Transform trans)
            {
                transform = trans;
                imgIcon = transform.Find("Present/Button_Icon").GetComponent<Image>();
                btnIcon = transform.Find("Present/Button_Icon").GetComponent<Button>();
                btnIcon.onClick.AddListener(OnBtnIconClick);
                goRedDot = transform.Find("Present/Image_Dot").gameObject;
                goIsGet = transform.Find("Present/Btn_Get").gameObject;

                txtTips = transform.Find("TextTip").GetComponent<Text>();
                btnBuy = transform.Find("Button").GetComponent<Button>();
                btnBuy.onClick.AddListener(OnBtnBuyClick);
                txtBtn = transform.Find("Button/Text").GetComponent<Text>();
                txtTime = transform.Find("Text").GetComponent<Text>();
                goIsFirst = transform.Find("Image_First").gameObject;
                btnPresent = transform.Find("Button_Gift").GetComponent<Button>();
                btnPresent.onClick.AddListener(OnBtnPresentClick);

                infinityDesc = transform.Find("ScrollView").gameObject.GetNeedComponent<InfinityGrid>();
                infinityDesc.onCreateCell += OnCreateCell_Desc;
                infinityDesc.onCellChange += OnCellChange_Desc;

                ani = transform.Find("Present").GetComponent<Animator>();
            }
            public void TimerCancel()
            {
                timer?.Cancel();
            }
            public void UpdateCellView(uint id)
            {
                cardId = id;
                cardData = CSVMonthCard.Instance.GetConfData(cardId);
                if (cardData != null)
                {
                    CSVItem.Data itemData = CSVItem.Instance.GetConfData(cardData.Extra_Giftbag[0]);
                    if (itemData != null)
                    {
                        ImageHelper.SetIcon(imgIcon, itemData.icon_id);
                    }
                    if (Sys_OperationalActivity.Instance.CheckSpecialCardIsActive(cardId))
                    {
                        var cardInfo = Sys_OperationalActivity.Instance.GetSpecialCardInfo(cardId);
                        txtTime.gameObject.SetActive(true);
                        StartTimer(cardInfo.Times);
                        bool canGet = Sys_OperationalActivity.Instance.CheckSpepcialGiftCanGet(cardId);
                        if (canGet)
                        {
                            goIsGet.SetActive(false);
                            goRedDot.SetActive(true);
                            ani.Play("Open");
                        }
                        else
                        {
                            goIsGet.SetActive(true);
                            goRedDot.SetActive(false);
                            ani.Play("Empty");
                        }
                    }
                    else
                    {
                        goIsGet.SetActive(false);
                        goRedDot.SetActive(false);
                        ani.Play("Empty");
                        txtTime.gameObject.SetActive(false);
                    }
                    //按钮价格
                    bool isFirstCharge = Sys_OperationalActivity.Instance.CheckSpecialCardFirstChargeShow(cardId);
                    goIsFirst.SetActive(isFirstCharge);
                    var chargeId = isFirstCharge ? cardData.First_Change_Id : cardData.Change_Id;
                    CSVChargeList.Data chargeData = CSVChargeList.Instance.GetConfData(chargeId);
                    uint priceId = isFirstCharge ? 12338u : 4723u;
                    txtBtn.text = LanguageHelper.GetTextContent(priceId, (chargeData.RechargeCurrency / 100).ToString());//按钮价格显示
                    
                    txtTips.text = LanguageHelper.GetTextContent(cardData.Return_Display);
                    //UpdatePrivilegeList();
                    ListSpecialDesc = isFirstCharge ? cardData.First_Pirviege_Des : cardData.Pirviege_Des;
                    infinityDesc.CellCount = ListSpecialDesc.Count;
                    infinityDesc.ForceRefreshActiveCell();
                    Sys_OperationalActivity.Instance.SetSpecialCardFirstRedPoint(cardData.id);
                }
            }

            private void OnCreateCell_Desc(InfinityGridCell cell)
            {
                UI_SpecialCardDecsCell mCell = new UI_SpecialCardDecsCell();
                mCell.Init(cell.mRootTransform.transform);
                cell.BindUserData(mCell);
                dictDesc.Add(cell.mRootTransform.gameObject, mCell);
            }
            private void OnCellChange_Desc(InfinityGridCell cell, int index)
            {
                UI_SpecialCardDecsCell mCell = cell.mUserData as UI_SpecialCardDecsCell;
                mCell.UpdateCellView(ListSpecialDesc[index], cardData);
            }

            private void OnBtnIconClick()
            {
                if (cardData != null)
                {
                    bool canGet = Sys_OperationalActivity.Instance.CheckSpepcialGiftCanGet(cardId); ;
                    if (canGet)
                    {
                        Sys_OperationalActivity.Instance.ReportSpecialCardClickEventHitPoint("GetReward:" + cardId);
                        Sys_OperationalActivity.Instance.GetSpecialCardReward(cardId);
                    }
                    else
                    {
                        Sys_OperationalActivity.Instance.ReportSpecialCardClickEventHitPoint("RewardIconClick:" + cardId);
                        PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(cardData.Extra_Giftbag[0], 0, false, false, false, false, false, false, true);
                        UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_OperationalActivity, itemData));
                    }
                }
            }

            private void OnBtnBuyClick()
            {
                CSVMonthCard.Data cardData = CSVMonthCard.Instance.GetConfData(cardId);
                if (cardData != null)
                {
                    bool isFirstCharge = Sys_OperationalActivity.Instance.CheckSpecialCardFirstChargeShow(cardId);
                    var chargeId = isFirstCharge ? cardData.First_Change_Id : cardData.Change_Id;
                    Sys_OperationalActivity.Instance.ReportSpecialCardClickEventHitPoint("GotoCharge:" + chargeId);
                    Sys_Charge.Instance.OnChargeReq(chargeId);
                }
            }
            private void OnBtnPresentClick()
            {
                if (Sys_OperationalActivity.Instance.CheckSpecialCardPresentIsOpen(true))
                {
                    //赠送窗口
                    UIManager.OpenUI(EUIID.UI_SpecialCardPresent, false, cardId);
                }
            }
            private void StartTimer(uint timestamp)
            {
                uint nowtime = Sys_Time.Instance.GetServerTime();
                var targetTime = timestamp;
                countDownTime = targetTime - nowtime;
                timer?.Cancel();
                timer = Timer.Register(countDownTime, OnTimerComplete, OnTimerUpdate, false, false);
            }
            private void OnTimerComplete()
            {
                timer?.Cancel();
                UpdateCellView(cardId);
                Sys_OperationalActivity.Instance.ReqRefreshSpecialCardInfo(cardId);
            }
            private void OnTimerUpdate(float time)
            {
                if (countDownTime >= time && txtTime != null)
                {
                    txtTime.text = LanguageHelper.GetTextContent(4741, LanguageHelper.TimeToString((uint)(countDownTime - time), LanguageHelper.TimeFormat.Type_4));
                }
            }
        }

        public class UI_SpecialCardDecsCell
        {
            private Transform transform;
            private Text txtDesc;
            private Button btnTips;
            private uint languageId = 0;

            private CSVMonthCard.Data cardData;
            public void Init(Transform trans)
            {
                transform = trans;
                txtDesc = transform.Find("Text_Name").GetComponent<Text>();
                btnTips = transform.Find("Image_Tips").GetComponent<Button>();
                btnTips.onClick.AddListener(OnBtnTipsClick);
            }

            public void UpdateCellView(List<uint> data, CSVMonthCard.Data _cardData)
            {
                cardData = _cardData;
                uint type = data[0];
                languageId = data[1];
                btnTips.gameObject.SetActive(languageId > 0);
                txtDesc.text = LanguageHelper.GetTextContent(type);
            }

            private void OnBtnTipsClick()
            {
                UIManager.OpenUI(EUIID.UI_SpecialCardRule, false, languageId);
            }
        }
    }
}
