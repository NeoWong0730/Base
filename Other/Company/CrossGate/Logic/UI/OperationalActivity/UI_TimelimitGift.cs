using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using System;
using Table;
using Lib.Core;
using Packet;

namespace Logic
{
    public class UI_TimelimitGift : UIBase
    {
        private Button btnClose;
        private Text txtDesc;
        private Text txtPercent;
        private Button btnLeft;
        private Button btnRight;
        private Button btnBuy;
        private Image imgBuyIcon;
        private Text txtBuy;
        private Text txtDiscount;//折扣

        private GameObject itemCell;
        private List<ItemIdCount> dropItems = new List<ItemIdCount>();

        private Text txtRemaningTime;
        private Timer timer;        //界面当前礼包timer
        private float countDownTime = 0;
        private Timer hindTimer;    //最早过期礼包timer 用来刷新界面
        private float hindCountDownTime = 0;
        private Timer btnCDTimer;   //按钮cd 防止多次请求
        private bool isBtnCD;
        private List<LimitGiftInfo> listGifts = new List<LimitGiftInfo>();

        private uint selectGiftId = 0;//选中的礼包id
        private int selectGiftIndex = 0;//选中礼包在列表里的位置

        #region 系统函数

        protected override void OnOpen(object arg)
        {
            if (arg != null && arg.GetType() == typeof(uint))
            {
                selectGiftId = (uint)arg;
            }
        }
        protected override void OnLoaded()
        {
            Parse();
        }
        protected override void OnShow()
        {
            isBtnCD = false;
            UpdateView();
        }
        protected override void OnHide()
        {
            timer?.Cancel();
            hindTimer?.Cancel();
            btnCDTimer?.Cancel();
        }
        protected override void OnDestroy()
        {
            timer?.Cancel();
            hindTimer?.Cancel();
            btnCDTimer?.Cancel();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateTimelimitGiftData, OnUpdateTimelimitGiftData, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateOperatinalActivityShowOrHide, OnUpdateOperatinalActivityShowOrHide, toRegister);
            Sys_Time.Instance.eventEmitter.Handle<uint, uint>(Sys_Time.EEvents.OnTimeNtf, OnTimeNtf, toRegister);
        }
        #endregion

        #region func
        private void Parse()
        {
            btnClose = transform.Find("Animator/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(OnBtnCloseClick);
            txtDesc = transform.Find("Animator/Content/Object_Text/Text").GetComponent<Text>();
            txtPercent = transform.Find("Animator/Content/Object_Text/Text1").GetComponent<Text>();
            txtRemaningTime = transform.Find("Animator/Content/Object_Text/Text_Time").GetComponent<Text>();
            btnLeft = transform.Find("Animator/Content/RawImage/Button_Left").GetComponent<Button>();
            btnLeft.onClick.AddListener(OnBtnLeftArrowClick);
            btnRight = transform.Find("Animator/Content/RawImage/Button_Right").GetComponent<Button>();
            btnRight.onClick.AddListener(OnBtnRightArrowClick);
            btnBuy = transform.Find("Animator/Content/RawImage/Button").GetComponent<Button>();
            btnBuy.onClick.AddListener(OnBtnBuyClick);
            imgBuyIcon = transform.Find("Animator/Content/RawImage/Button/Image").GetComponent<Image>();
            txtBuy = transform.Find("Animator/Content/RawImage/Button/Text1").GetComponent<Text>();
            txtDiscount = transform.Find("Animator/Content/Image/Text").GetComponent<Text>();

            itemCell = transform.Find("Animator/Content/RawImage/Scroll_View/Viewport/Item").gameObject;
        }
        private void UpdateView()
        {
            listGifts = Sys_OperationalActivity.Instance.GetTimelimitSortGifts();
            if (listGifts.Count > 0 && !Sys_OperationalActivity.Instance.CheckTimelimitGiftIsValid(selectGiftId))
            {
                selectGiftId = listGifts[0].Id;
            }
            for (int i = 0; i < listGifts.Count; i++)
            {
                if(listGifts[i].Id == selectGiftId)
                {
                    selectGiftIndex = i;
                    break;
                }
            }
            CSVConditionalGift.Data giftData = CSVConditionalGift.Instance.GetConfData(selectGiftId);
            if (giftData != null)
            {
                UpdateTimer();
                txtDesc.text = LanguageHelper.GetTextContent(giftData.Titleid);
                txtPercent.text = LanguageHelper.GetTextContent(giftData.Text);
                txtDiscount.text = LanguageHelper.GetTextContent(591000315, giftData.Discount);
                btnLeft.gameObject.SetActive(selectGiftIndex > 0);
                btnRight.gameObject.SetActive(selectGiftIndex < listGifts.Count - 1);
                uint dropId = giftData.Reward;
                dropItems = CSVDrop.Instance.GetDropItem(dropId);
                FrameworkTool.DestroyChildren(itemCell.transform.parent.gameObject, itemCell.name);
                int len = dropItems.Count;
                for (int i = 0; i < len; i++)
                {
                    GameObject go = GameObject.Instantiate<GameObject>(itemCell, itemCell.transform.parent);
                    go.SetActive(true);
                    PropItem mCell = new PropItem();
                    mCell.BindGameObject(go);
                    var dropItem = dropItems[i];
                    PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(dropItem.id, dropItem.count, true, false, false, false, false, true);
                    mCell.SetData(itemData, EUIID.UI_TimelimitGift);
                }
                if (giftData.PriceType == 0)
                {
                    imgBuyIcon.gameObject.SetActive(false);
                    CSVChargeList.Data chargeData = CSVChargeList.Instance.GetConfData(giftData.Price);
                    if (chargeData != null)
                    {
                        txtBuy.text = LanguageHelper.GetTextContent(11647u, (chargeData.RechargeCurrency / 100f).ToString());
                    }
                }
                else
                {
                    imgBuyIcon.gameObject.SetActive(true);
                    var itemData = CSVItem.Instance.GetConfData(giftData.PriceType);
                    if (itemData != null)
                    {
                        ImageHelper.SetIcon(imgBuyIcon, itemData.icon_id);
                    }
                    txtBuy.text = giftData.Price.ToString();
                }
            }
        }

        private void UpdateTimer()
        {
            timer?.Cancel();
            hindTimer?.Cancel();
            countDownTime = Sys_OperationalActivity.Instance.GetTimelimitGiftCountdown(selectGiftId);
            timer = Timer.Register(countDownTime, OnTimerComplete, OnTimerUpdate, false, false);
            if (selectGiftIndex > 0)
            {
                var minGift = Sys_OperationalActivity.Instance.GetTimelimitMinCDValidGift();
                if (minGift != null)
                {
                    hindCountDownTime = minGift.Time;
                    hindTimer = Timer.Register(hindCountDownTime, OnHindTimerComplete, null, false, false);
                }
            }
        }

        #endregion

        #region event
        private void OnBtnBuyClick()
        {
            if (isBtnCD)
                return;
            isBtnCD = true;
            btnCDTimer?.Cancel();
            btnCDTimer = null;
            btnCDTimer = Timer.Register(1f, () =>
            {
                isBtnCD = false;
            });

            CSVConditionalGift.Data giftData = CSVConditionalGift.Instance.GetConfData(selectGiftId);
            UIManager.HitButton(EUIID.UI_TimelimitGift, "OnBtnBuyClick_" + selectGiftId);
            if (giftData != null)
            {
                if(giftData.PriceType == 0)
                {
                    //走充值逻辑
                    uint chargeId = giftData.Price;
                    Sys_Charge.Instance.OnChargeReq(chargeId, selectGiftId);
                }
                else
                {
                    long costPrice = giftData.Price;
                    long hadPrice = Sys_Bag.Instance.GetItemCount(giftData.PriceType);
                    if (costPrice > hadPrice)
                    {
                        Sys_Bag.Instance.TryOpenExchangeCoinUI((ECurrencyType)giftData.PriceType, costPrice);
                    }
                    else
                    {
                        //弹窗提示
                        PromptBoxParameter.Instance.Clear();
                        PromptBoxParameter.Instance.SetConfirm(true, () =>
                        {
                            Sys_OperationalActivity.Instance.LimitGiftBuyReq(selectGiftId);
                        }, 591000313);
                        PromptBoxParameter.Instance.SetCancel(true, null);
                        CSVItem.Data item = CSVItem.Instance.GetConfData(giftData.PriceType);
                        string content = LanguageHelper.GetTextContent(591000312, giftData.Price.ToString(), LanguageHelper.GetTextContent(item.name_id), LanguageHelper.GetTextContent(giftData.Text));//确认使用{0}{1}购买{3}
                        PromptBoxParameter.Instance.content = content;
                        UIManager.OpenUI(EUIID.UI_PromptBox, true, PromptBoxParameter.Instance);
                    }
                }
            }
        }
        private void OnBtnCloseClick()
        {
            UIManager.HitButton(EUIID.UI_TimelimitGift, "CloseClick");
            this.CloseSelf();
        }
        private void OnTimerComplete()
        {
            timer?.Cancel();
            //关掉二次弹框
            UIManager.CloseUI(EUIID.UI_PromptBox);
            if (Sys_OperationalActivity.Instance.CheckTimelimitMainBtnIsShow())
            {
                UpdateView();
            }
            else
            {
                this.CloseSelf();
            }
        }
        private void OnHindTimerComplete()
        {
            UpdateView();
        }
        private void OnTimerUpdate(float time)
        {
            if (countDownTime >= time && txtRemaningTime != null)
            {
                txtRemaningTime.text = LanguageHelper.TimeToString((uint)(countDownTime - time), LanguageHelper.TimeFormat.Type_1);
            }
        }
        private void OnUpdateTimelimitGiftData()
        {
            UpdateView();
        }
        private void OnTimeNtf(uint oldTime, uint newTime)
        {
            UpdateView();
        }
        private void OnBtnLeftArrowClick()
        {
            var index = selectGiftIndex <= 0 ? 0 : selectGiftIndex - 1;
            selectGiftId = listGifts[index].Id;
            UpdateView();
        }
        private void OnBtnRightArrowClick()
        {
            var max = listGifts.Count - 1;
            var index = selectGiftIndex >= max ? max : selectGiftIndex + 1;
            selectGiftId = listGifts[index].Id;
            UpdateView();
        }
        private void OnUpdateOperatinalActivityShowOrHide()
        {
            if (!Sys_OperationalActivity.Instance.CheckTimelimitFunctionIsOpen())
            {
                this.CloseSelf();
            }
        }
        #endregion
    }
}
