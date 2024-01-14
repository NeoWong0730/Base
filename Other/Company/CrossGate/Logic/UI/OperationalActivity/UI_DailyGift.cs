using Logic.Core;
using System.Collections.Generic;
using Table;
using UnityEngine.UI;
using UnityEngine;
using System;
using Lib.Core;
using Framework;
using Packet;
using System.Text;

namespace Logic
{
    public enum DailyGiftType
    {
        MagicCoin=1,//魔币
        RealCoin=2,//真实货币
    }
    public class UI_DailyGift_Ceil
    {
        private Text txt_GetName;
        private GameObject go_propItemOne;
        private GameObject go_propItemSec;
        private GameObject go_MultButtonOne;
        private GameObject go_MultButtonSec;
        private Button btn_singleReceive;
        private Text btn_singleReceiveText;//按钮text
        private Text txt_discountNum;
        private GameObject go_Received;
        private GameObject go_singleCostCoin;
        private Text txt_singleCostCoinNum;//魔币text
        private Text txt_RealCoinNum;//货币text

        private uint boxType;
        private int giftState;
        private uint giftType;

        public CSVDaliyPacks.Data pData;
        public bool isReceived;
        public bool isBuy;
        private uint firstId;
        private uint secondId;
        private double priceNum;
        private List<uint> pricePack;
        private Timer BtnCDTimer;

        public UI_DailyGift_Ceil(uint _boxType,int _giftState,uint _giftType) : base()
        {
            boxType = _boxType;//礼包编号
            giftState = _giftState;//礼包状态
            giftType = _giftType;//礼包类型
        }
        #region Init
        public void Init(Transform transform)
        {
            txt_GetName = transform.Find("Name").gameObject.GetComponent<Text>();
            go_propItemOne = transform.Find("Reward0/PropItem").gameObject;
            go_MultButtonOne = transform.Find("Reward0/Button").gameObject;
            go_propItemSec = transform.Find("Reward1/PropItem").gameObject;
            go_MultButtonSec = transform.Find("Reward1/Button").gameObject;
            btn_singleReceive = transform.Find("Btn_01").gameObject.GetComponent<Button>();
            btn_singleReceiveText = transform.Find("Btn_01/Text_01").gameObject.GetComponent<Text>();
            txt_discountNum = transform.Find("Discount/Num").GetComponent<Text>();
            go_Received = transform.Find("Received").gameObject;
            go_singleCostCoin = transform.Find("Cost_Coin").gameObject;
            txt_singleCostCoinNum = transform.Find("Cost_Coin/Text_Cost").GetComponent<Text>();
            txt_RealCoinNum = transform.Find("Text_Cost2").GetComponent<Text>();
            go_MultButtonOne.GetComponent<Button>().onClick.RemoveAllListeners();
            go_MultButtonSec.GetComponent<Button>().onClick.RemoveAllListeners();
            btn_singleReceive.onClick.RemoveAllListeners();
            go_MultButtonOne.GetComponent<Button>().onClick.AddListener(OnFirstButtonClick);
            go_MultButtonSec.GetComponent<Button>().onClick.AddListener(OnSecondButtonClick);
            btn_singleReceive.onClick.AddListener(OnSingleButtonClick);
            pData = CSVDaliyPacks.Instance.GetConfData(giftType);
            btn_singleReceive.enabled = true;
        }
        public void SetData()
        {
            SetGiftData();
            InitButton();
            SetCoinImage();
        }
        private void SetGiftData()
        {
            switch (boxType)
            {
                case 1:
                    txt_GetName.text = LanguageHelper.GetTextContent(pData.pack1_name);
                    pricePack = pData.price_pack1;
                    firstId = pData.drop1_Id;
                    secondId = pData.drop2_Id;
                    break;
                case 2:
                    txt_GetName.text = LanguageHelper.GetTextContent(pData.pack2_name);
                    pricePack = pData.price_pack2;
                    firstId = pData.drop3_Id;
                    secondId = pData.drop4_Id;
                    break;
                case 3:
                    txt_GetName.text = LanguageHelper.GetTextContent(pData.pack3_name);
                    pricePack = pData.price_pack3;
                    firstId = pData.drop5_Id;
                    secondId = pData.drop6_Id;
                    break;
                default: break;

            }
            txt_discountNum.text = DiscountNumString(pricePack);
            priceNum = PriceNumString(pricePack);
            switch ((DailyGiftType)pricePack[0])
            {
                case DailyGiftType.MagicCoin://魔币
                    txt_singleCostCoinNum.text = priceNum.ToString();
                    break;
                case DailyGiftType.RealCoin://直购
                    txt_RealCoinNum.text = LanguageHelper.GetTextContent(pData.currecny_sign,priceNum.ToString("f2")); 
                    break;
                default:
                    break;
            }
            CheckGiftType(firstId, true);//后面bool值true第一个，false第二个
            CheckGiftType(secondId, false);
        }
        public void InitButton()
        {
            InitGiftState();
            go_Received.SetActive(isBuy && isReceived);
            btn_singleReceive.gameObject.SetActive(!isReceived);
            txt_RealCoinNum.gameObject.SetActive(!isBuy);
            if (isBuy)
            {
                btn_singleReceiveText.text = LanguageHelper.GetTextContent(599003004);
            }
            else
            {
                btn_singleReceiveText.text = " ";
            }
        }
        private void SetCoinImage()
        {
            ImageHelper.SetIcon(go_singleCostCoin.GetComponent<Image>(), 992501, true);
            go_singleCostCoin.SetActive(pricePack[0] == (uint)DailyGiftType.MagicCoin && !isBuy);
        }
        private void InitGiftState()
        {
            if (giftState == 0)
            {
                isBuy = false;
                isReceived = false;
            }
            else
            {
                isBuy = true;
                if (giftState == 1)
                {
                    isReceived = false;
                }
                else
                {
                    isReceived = true;
                }
            }
        }
        #endregion
        #region Function
        private double PriceNumString(List<uint> _packetData)
        {
            double _price =0;
            switch ((DailyGiftType)_packetData[0])
            {
                case DailyGiftType.MagicCoin://魔币
                    _price = _packetData[1];
                    break;
                case DailyGiftType.RealCoin://直购
                    var chargeData = CSVChargeList.Instance.GetConfData(_packetData[1]);
                    if (chargeData != null)
                    {
                        _price = chargeData.RechargeCurrency / 100.0;
                    }
                    break;
                default:
                    break;
            }
            return _price;
        }
        private string DiscountNumString(List<uint> _packetData)
        {
            string _str = string.Empty;
            switch ((DailyGiftType)_packetData[0])
            {
                case DailyGiftType.MagicCoin://魔币
                    _str=(_packetData[1] * 10.0 / _packetData[2]).ToString("f1");
                    break;
                case DailyGiftType.RealCoin://直购
                    _str = (_packetData[2] / 10.0).ToString("f1");
                    break;
                default:
                    break;
            }
            return _str;
        }
        private void CheckGiftType(uint id, bool isFirst)
        {
            CSVDrop.Data dropData = CSVDrop.Instance.GetDropItemData(id);

            if (dropData.reward_show.Count == 1)
            {
                SingleGift(dropData, GiftBoxCheck(true, isFirst));
            }
            else
            {
                GiftBoxCheck(false, isFirst);
            }
        }
        private GameObject GiftBoxCheck(bool isSingle, bool isFirst)
        {
            if (isFirst)
            {
                if (isSingle)
                {
                    go_MultButtonOne.SetActive(false);
                    return go_propItemOne;
                }
                else
                {
                    go_propItemOne.SetActive(false);
                    return go_MultButtonOne;
                }
            }
            else
            {
                if (isSingle)
                {
                    go_MultButtonSec.SetActive(false);
                    return go_propItemSec;
                }
                else
                {
                    go_propItemSec.SetActive(false);
                    return go_MultButtonSec;
                }
            }

        }
        private void SingleGift(CSVDrop.Data dData, GameObject go)
        {
            PropItem propItem = new PropItem();
            propItem.BindGameObject(go);
            propItem.SetData(new MessageBoxEvt(EUIID.UI_OperationalActivity, new PropIconLoader.ShowItemData(dData.reward_show[0][0], dData.reward_show[0][1], true, false, false, false, false,
                    _bShowCount: true, _bUseClick: true, _bShowBagCount: false)));

        }
        #endregion
        #region Refresh
        public void RefreshButton(int stateType)
        {
            giftState = stateType;
            InitButton();
        }
        public void ThisDestory()
        {
            BtnCDTimer?.Cancel();
        }
        #endregion
        #region Button
        private void OnSingleButtonClick()
        {
            if (isBuy)
            {
                if (!Sys_OperationalActivity.Instance.isDailyGiftOpen)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(599003018));
                    return;
                }
                Sys_OperationalActivity.Instance.OnDailyGiftOperatorReq(3, boxType);//领取
            }
            else
            {
                if (!Sys_OperationalActivity.Instance.isDailyGiftOpen)
                {//(功能开关)目前无法领取奖励
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(599003017));
                    return;
                }

                //购买逻辑
                switch ((DailyGiftType)pricePack[0])
                {
                    case DailyGiftType.MagicCoin://魔币
                        if (priceNum <= Sys_Bag.Instance.GetItemCount(1))
                        {
                            Sys_OperationalActivity.Instance.OnDailyGiftOperatorReq(1, boxType);//单独购买
                        }
                        else
                        {
                            Sys_OperationalActivity.Instance.OpenChargeBox();
                        }
                        break;
                    case DailyGiftType.RealCoin://直购
                        Sys_Charge.Instance.OnChargeReq(pricePack[1]);
                        btn_singleReceive.enabled = false;
                        BtnCDTimer?.Cancel();
                        BtnCDTimer=Timer.Register(1f, () =>
                        {
                            btn_singleReceive.enabled = true;
                        });
                        break;
                    default:
                        break;
                }

            }
        }
        
        private void OnFirstButtonClick()
        {
            RewardPanelParam _param = new RewardPanelParam();
            Vector3 _vec = go_MultButtonOne.gameObject.GetComponent<RectTransform>().position;
            Vector3 _screenVec = CameraManager.mUICamera.WorldToScreenPoint(_vec);
            _param.propList = CSVDrop.Instance.GetDropItem(firstId);
            _param.Pos = _screenVec;
            UIManager.OpenUI(EUIID.UI_RewardPanel, false, _param);
        }
        private void OnSecondButtonClick()
        {
            RewardPanelParam _param = new RewardPanelParam();
            Vector3 _vec = go_MultButtonSec.gameObject.GetComponent<RectTransform>().position;
            Vector3 _screenVec = CameraManager.mUICamera.WorldToScreenPoint(_vec);
            _param.propList = CSVDrop.Instance.GetDropItem(secondId);
            _param.Pos = _screenVec;
            UIManager.OpenUI(EUIID.UI_RewardPanel, false, _param);
        }
        #endregion

    }
    public class UI_DailyGift : UI_OperationalActivityBase
    {
        #region 界面显示
        private GameObject go_dailyGiftOne;
        private GameObject go_dailyGiftSec;
        private GameObject go_dailyGiftThird;
        private GameObject go_viewRight;
        private Text go_viewRight_Grade;
        private Button btn_dailyBenefit;
        private GameObject btn_dailyBenefit_redpoint;
        private GameObject go_sevenDayCostCoin;//七日购买货币
        private Text txt_BottonUp;
        private Button btn_Botton;
        private Text txt_BottonText;
        private Text txt_BottonRealCoin;
        private Text txt_tip;
        private GameObject go_sevenDayDiscount;//折扣GO
        private Button btn_tip;//等级提示按钮
        private Text txt_sevenDayDiscountNum;//折扣数字
        private Text txt_ViewRightText;
        #endregion

        private List<UI_DailyGift_Ceil> dList = new List<UI_DailyGift_Ceil>();
        private UI_DailyGift_RedPoint giftRedPoint;
        private Dictionary<int,uint> giftTypeDic = new Dictionary<int, uint>();
        private CSVDaliyPacks.Data packetData;
        private Timer BottonCDTimer;
        #region 系统函数
        protected override void Loaded()
        {
            dList.Clear();
        }
        protected override void InitBeforOnShow()
        {
            go_dailyGiftOne = transform.Find("Gift/0").gameObject;
            go_dailyGiftSec = transform.Find("Gift/1").gameObject;
            go_dailyGiftThird = transform.Find("Gift/2").gameObject;
            go_viewRight = transform.Find("View_Right/Dialogue").gameObject;
            btn_tip = transform.Find("View_Right/Dialogue/Button").GetComponent<Button>();
            btn_dailyBenefit = transform.Find("Bottom/Button").GetComponent<Button>();
            btn_dailyBenefit_redpoint = transform.Find("Bottom/Button/Red").gameObject;
            go_sevenDayCostCoin = transform.Find("Bottom/Cost_Coin").gameObject;
            txt_BottonText = transform.Find("Bottom/Cost_Coin/Text_Cost").GetComponent<Text>();
            txt_BottonRealCoin = transform.Find("Bottom/Text_Cost2").GetComponent<Text>();
            btn_Botton = transform.Find("Bottom/Btn_01").GetComponent<Button>();
            txt_BottonUp = transform.Find("Bottom/Btn_01/Text_01").GetComponent<Text>();
            txt_tip = transform.Find("Bottom/Text_Tip").GetComponent<Text>();
            go_sevenDayDiscount = transform.Find("Bottom/Btn_01/Discount").gameObject;
            txt_sevenDayDiscountNum = transform.Find("Bottom/Btn_01/Discount/Num").GetComponent<Text>();
            txt_ViewRightText = transform.Find("View_Right/Dialogue/Text").GetComponent<Text>();

            btn_Botton.onClick.AddListener(OnBottonButtonClicked);
            btn_dailyBenefit.onClick.AddListener(OnBenefitButtonClicked);
            btn_tip.onClick.AddListener(OnTipButtonClicked);
            Sys_OperationalActivity.Instance.DailygiftBagTypeInit();
            giftRedPoint = gameObject.AddComponent<UI_DailyGift_RedPoint>();
            btn_Botton.enabled = true;
            giftRedPoint?.Init(this);
        }
        public override void Show()
        {
            base.Show();
            InitData();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateDailyGiftData, OnUpdateDailyGiftData, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateOperatinalActivityShowOrHide, OnUpdateDailyGiftOpen, toRegister);
        }
        public override void OnDestroy()
        {
            CheckPanelClose();
            for (int i=0;i<dList.Count;i++)
            {
                dList[i].ThisDestory();
            }
            dList.Clear();
            BottonCDTimer?.Cancel();
        }
        #endregion
        #region Function 
        private void InitData()
        {
            packetData = CSVDaliyPacks.Instance.GetConfData(Sys_OperationalActivity.Instance.giftBagType);
            CheckPanelClose();
            btn_dailyBenefit.onClick.RemoveAllListeners();
            btn_dailyBenefit.onClick.AddListener(OnBenefitButtonClicked);
            InitgiftTypeDic();
            InitGiftPanelTotal();
            BottonShow();
            CheckViewRight();
            giftRedPoint.RefreshAllRedPoints();
            Sys_OperationalActivity.Instance.SetDailyGiftFirstRedPoint();
        }

        /// <summary>
        /// 若玩家升级后进入下一个档位，已领取的礼包显示领取时的状态
        /// </summary>
        private void InitgiftTypeDic()
        {
            giftTypeDic.Clear();
            for (int i=1;i<=3;i++)
            {
                giftTypeDic.Add(i,Sys_OperationalActivity.Instance.giftBagType);
                if (Sys_OperationalActivity.Instance.giftDic[i]==2)
                {
                    if (Sys_OperationalActivity.Instance.giftGetLevelDic[i] < packetData.need_level)
                    {
                        giftTypeDic[i] = Sys_OperationalActivity.Instance.CheckDailyGiftBagType(Sys_OperationalActivity.Instance.giftGetLevelDic[i]);
                    }

                }

            }
        }
        private void InitGiftPanelTotal()
        {
            InitGiftPanel(go_dailyGiftOne, 1, Sys_OperationalActivity.Instance.giftDic[1], giftTypeDic[1]);
            InitGiftPanel(go_dailyGiftSec, 2, Sys_OperationalActivity.Instance.giftDic[2], giftTypeDic[2]);
            InitGiftPanel(go_dailyGiftThird, 3, Sys_OperationalActivity.Instance.giftDic[3], giftTypeDic[3]);
        }
        private void InitGiftPanel(GameObject go,uint boxType,int giftState,uint giftType)
        {
            UI_DailyGift_Ceil giftItem = new UI_DailyGift_Ceil(boxType, giftState, giftType);
            giftItem.Init(go.transform);
            giftItem.SetData();
            dList.Add(giftItem);
        }

        private void CheckPanelClose()
        {
            if (UIManager.IsOpen(EUIID.UI_Rule))
            {
                UIManager.CloseUI(EUIID.UI_Rule);
            }
            if (UIManager.IsOpen(EUIID.UI_RewardPanel))
            {
                UIManager.CloseUI(EUIID.UI_RewardPanel);
            }

        }
        /// <summary>
        /// 购买等级提示按钮是否显示
        /// </summary>
        private void CheckViewRight()
        {
            go_viewRight.SetActive(!Sys_OperationalActivity.Instance.isMaxTpye);
            if (!Sys_OperationalActivity.Instance.isMaxTpye)
            {
                txt_ViewRightText.text = LanguageHelper.GetTextContent(4747, CSVDaliyPacks.Instance.GetConfData(Sys_OperationalActivity.Instance.giftBagType+1).need_level.ToString());
            }
            
        }
        /// <summary>
        /// 一键按钮购买显示及免费礼包显示
        /// </summary>
        private void BottonShow()
        {
            //免费礼包
            ImageHelper.SetImageGray(btn_dailyBenefit.GetComponent<Image>(), Sys_OperationalActivity.Instance.dayGift);//是否领取
            btn_dailyBenefit_redpoint.SetActive(!Sys_OperationalActivity.Instance.dayGift);//红点
            //一键购买
            go_sevenDayDiscount.SetActive(!Sys_OperationalActivity.Instance.isSevenDayBuy);
            btn_Botton.enabled = true;
            //设置魔币图片
            ImageHelper.SetIcon(go_sevenDayCostCoin.GetComponent<Image>(), 992501, true);
            ImageHelper.SetImageGray(btn_Botton.GetComponent<Image>(), false);

            
            if (!Sys_OperationalActivity.Instance.isSevenDayBuy)
            {
                switch ((DailyGiftType)packetData.price_7day[0])
                {
                    case DailyGiftType.MagicCoin://魔币
                        txt_BottonText.text = packetData.price_7day[1].ToString();
                        txt_sevenDayDiscountNum.text = (packetData.price_7day[1] * 10.0 / packetData.price_7day[2]).ToString("f1");
                        go_sevenDayCostCoin.GetComponent<Image>().enabled = true;
                        break;
                    case DailyGiftType.RealCoin://直购
                        var chargeData = CSVChargeList.Instance.GetConfData(packetData.price_7day[1]);
                        if (chargeData!=null)
                        {
                            txt_BottonRealCoin.text=LanguageHelper.GetTextContent(packetData.currecny_sign,(chargeData.RechargeCurrency/100.0).ToString("f2")) ;
                        }
                        txt_sevenDayDiscountNum.text = (packetData.price_7day[2] / 10.0).ToString("f1");
                        txt_BottonText.text = "";
                        go_sevenDayCostCoin.GetComponent<Image>().enabled = false;
                        break;
                    default:
                        break;
                }
                
                txt_BottonRealCoin.gameObject.SetActive(packetData.price_7day[0] == (uint)DailyGiftType.RealCoin);
                //提示语
                txt_tip.text = LanguageHelper.GetTextContent(599003009);//购买提示
                txt_BottonUp.text = LanguageHelper.GetTextContent(599003013);//一键购买7天
                //如果单独购买，置灰
                ImageHelper.SetImageGray(btn_Botton.GetComponent<Image>(), IsSingleBuy());
                ImageHelper.SetImageGray(go_sevenDayCostCoin.GetComponent<Image>(), IsSingleBuy());
                ImageHelper.SetImageGray(go_sevenDayDiscount.GetComponent<Image>(), IsSingleBuy());


            }
            else
            {
                go_sevenDayCostCoin.GetComponent<Image>().enabled = false;
                txt_tip.text = LanguageHelper.GetTextContent(599003010);//领取提示
                txt_BottonUp.text = LanguageHelper.GetTextContent(599003011, Sys_OperationalActivity.Instance.giftEndDay.ToString());//{0}天后可购买
                txt_BottonRealCoin.text="";
                if (Sys_OperationalActivity.Instance.isSevenDayAllReceived)
                {
                    ImageHelper.SetImageGray(btn_Botton.GetComponent<Image>(), true);
                    txt_BottonText.text = LanguageHelper.GetTextContent(599003006);//已领取
                }
                else
                {
                    txt_BottonText.text = LanguageHelper.GetTextContent(599003005);//一键领取
                }
            }
        }

        private bool IsSingleBuy()
        {//如果单独购买了则返回true
            for (int i=1;i<=3;i++)
            {
                if (Sys_OperationalActivity.Instance.giftDic[i]!=0)
                {
                    return true;
                }
            }

            return false;
        }
        private string GetName(uint id)
        {
            return LanguageHelper.GetTextContent(id);
        }
        private uint GetNameId(uint id)
        {
            return CSVItem.Instance.GetConfData(id).name_id;
        }
        private string GiftText(uint id)
        {
            StringBuilder str = new StringBuilder();
            List<List<uint>> dList = CSVDrop.Instance.GetDropItemData(id).reward_show;
            for (int i = 0; i < dList.Count; i++)
            {
                str.Append(GetName(GetNameId(dList[i][0]))).Append("×" + dList[i][1]);
                if (i!= dList.Count-1)
                {
                    str.Append(",");
                }
            }
            return str.ToString();
        }
        #endregion
        #region CallBack
        public void OnUpdateDailyGiftData()
        {
            InitData();
        }
        public void OnUpdateDailyGiftOpen()
        {
            Sys_OperationalActivity.Instance.isDailyGiftOpen = Sys_OperationalActivity.Instance.CheckDailyGiftIsOpen();
        }
        //public void OnUpdateSingleDailyGiftData(uint OnType,uint boxIndex,int nowState)
        //{
        //    if (OnType==4)
        //    {
        //        btn_dailyBenefit_redpoint.SetActive(false);
        //        return;
        //    }
        //    dList[(int)boxIndex-1].RefreshButton(nowState);
        //    BottonShow();
        //    giftRedPoint.RefreshAllRedPoints();
        //}
        #endregion
        #region Button
        private void OnBottonButtonClicked()
        {
            

            if (Sys_OperationalActivity.Instance.isSevenDayBuy)
            {
                
                if (Sys_OperationalActivity.Instance.isSevenDayAllReceived)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(599003015));//已领取
                }
                else
                {
                    if (!Sys_OperationalActivity.Instance.isDailyGiftOpen)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(599003018));//目前无法购买礼包
                        return;
                    }
                    Sys_OperationalActivity.Instance.OnDailyGiftOperatorReq(3, 0);//一键领取
                }
                
            }else
            {
                if (!Sys_OperationalActivity.Instance.isDailyGiftOpen)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(599003017));//(功能开关)目前无法领取奖励
                    return;
                }
                if (IsSingleBuy())
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(599003014));//已单独购买
                    return;
                }

                //购买逻辑
                switch ((DailyGiftType)packetData.price_7day[0])
                {
                    case DailyGiftType.MagicCoin://魔币
                        if (packetData.price_7day[0] <= Sys_Bag.Instance.GetItemCount(1))
                        {
                            Sys_OperationalActivity.Instance.OnDailyGiftOperatorReq(2, 0);//购买七天
                        }
                        else
                        {
                            Sys_OperationalActivity.Instance.OpenChargeBox();
                        }
                        break;
                    case DailyGiftType.RealCoin://直购
                        Sys_Charge.Instance.OnChargeReq(packetData.price_7day[1]);
                        btn_Botton.enabled = false;
                        BottonCDTimer?.Cancel();
                        BottonCDTimer = Timer.Register(1f, () =>
                        {
                            btn_Botton.enabled = true;
                        });
                        break;
                    default:
                        break;
                }

            }
        }
        public void OnTipButtonClicked()
        {
            CSVDaliyPacks.Data pdata = CSVDaliyPacks.Instance.GetConfData(Sys_OperationalActivity.Instance.giftBagType + 1);
            UIRuleParam rParam = new UIRuleParam();
            rParam.StrContent = LanguageHelper.GetTextContent(599003012, pdata.need_level.ToString(), GetName(pdata.pack1_name), GiftText(pdata.drop1_Id), GiftText(pdata.drop2_Id), GetName(pdata.pack2_name), GiftText(pdata.drop3_Id), GiftText(pdata.drop4_Id), GetName(pdata.pack3_name), GiftText(pdata.drop5_Id), GiftText(pdata.drop6_Id));
            rParam.Pos =CameraManager.mUICamera.WorldToScreenPoint(btn_tip.GetComponent<RectTransform>().position-new Vector3(2.7f,1.5f,0));
            UIManager.OpenUI(EUIID.UI_Rule, false, rParam);
        }
        private void OnBenefitButtonClicked()
        {
            if (Sys_OperationalActivity.Instance.dayGift)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(599003007));//今日已领取过
            }
            else
            {
                if (!Sys_OperationalActivity.Instance.isDailyGiftOpen)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(599003018));
                    return;
                }
                Sys_OperationalActivity.Instance.OnDailyGiftOperatorReq(4, 0);
            }

        }
        #endregion
    }
}
