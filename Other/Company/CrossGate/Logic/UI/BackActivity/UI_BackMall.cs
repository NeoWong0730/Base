using Logic.Core;
using Packet;
using UnityEngine.Playables;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Logic
{
    public class UI_BackMall : UI_BackActivityBase
    {

        public class MallItem
        {
            private Transform transform;
            
            private Button btnBg;
            private Image imgIcon;
            private Text txtName;
            private Image imgSellOut;

            private Transform transTurn;

            private Text txtSelfCount;
            private Transform transServerCount;
            private Text txtServerCount;
            private Transform transDiscount;
            private Text txtDiscount;

            private Transform transCostOriginal;
            private Image imgCostOriginal;
            private Text txtCostOriginal;
            private Transform transCostNow;
            private Image imgCostNow;
            private Text txtCostNow;

            private ShopGoods m_shopItem;
            private uint m_ShopItemId;
            private CSVActivityShopGoods.Data m_shopData;

            private bool isDiscount;
            private bool isSellOut;
            
            public void Init(Transform trans)
            {
                transform = trans;

                btnBg = transform.Find("Btn_Bg").GetComponent<Button>();
                btnBg.onClick.AddListener(OnClick);

                imgIcon = transform.Find("Btn_Item/Image_ICON").GetComponent<Image>();
                Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(imgIcon);
                eventListener.AddEventListener(EventTriggerType.PointerClick, OnClickItem);
                
                txtName = transform.Find("Text_Name").GetComponent<Text>();
                imgSellOut = transform.Find("Image_Sellout").GetComponent<Image>();

                transTurn = transform.Find("MarkGrid/Image_Mark03");
                
                txtSelfCount = transform.Find("MarkGrid/Text_Num").GetComponent<Text>();
                transServerCount = transform.Find("MarkGrid/Image_Mark02");
                txtServerCount = transform.Find("MarkGrid/Image_Mark02/Text_Num").GetComponent<Text>();
                transDiscount = transform.Find("MarkGrid/Image_Mark01");
                txtDiscount = transform.Find("MarkGrid/Image_Mark01/Text/Text_Num").GetComponent<Text>();

                transCostOriginal = transform.Find("Cost/Cost_Original");
                imgCostOriginal = transform.Find("Cost/Cost_Original/Text_Original/Image_Icon").GetComponent<Image>();
                txtCostOriginal = transform.Find("Cost/Cost_Original/Text_Original/Image_Icon/Text").GetComponent<Text>();

                transCostNow = transform.Find("Cost/Cost_Now");
                imgCostNow = transform.Find("Cost/Cost_Now/Image_Icon").GetComponent<Image>();
                txtCostNow = transform.Find("Cost/Cost_Now/Image_Icon/Text").GetComponent<Text>();
            }

            private void OnClick()
            {
                if (isSellOut)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2025808));
                    return;
                }
                
                int leftCount = Sys_MallActivity.Instance.CalCanBuyMaxCount(m_ShopItemId);
                if (leftCount > 0)
                {
                    UIManager.OpenUI(EUIID.UI_Activity_MallBuy, false, m_shopItem);
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2025809));
                }
            }

            private void OnClickItem(BaseEventData baseEventData)
            {
                PropMessageParam propParam = new PropMessageParam();
                propParam.itemData = new ItemData();
                propParam.itemData.SetData(0, 0, m_shopData.item_id, 1, 0, false, false, null, null, 0);
                propParam.needShowInfo = false;
                propParam.needShowMarket = false;
                propParam.sourceUiId = EUIID.UI_Activity_Mall;
                UIManager.OpenUI(EUIID.UI_Prop_Message, false, propParam);
            }

            public void UpdateInfo(ShopGoods item, bool discount = false)
            {
                m_ShopItemId = item.GoodsId;
                m_shopItem = Sys_MallActivity.Instance.GetNewData(m_ShopItemId);

                m_shopData = CSVActivityShopGoods.Instance.GetConfData(m_ShopItemId);
                if (m_shopData == null)
                {
                    Debug.LogErrorFormat("id {0} 商品未配置", m_ShopItemId);
                    return;
                }

                CSVItem.Data itemInfo = CSVItem.Instance.GetConfData(m_shopData.item_id);
                if (itemInfo == null)
                {
                    Debug.LogErrorFormat("id {0} 道具表未配置", m_shopData.item_id);
                    return;
                }

                ImageHelper.SetIcon(imgIcon, itemInfo.icon_id);
                txtName.text = LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(m_shopData.item_id).name_id);

                transTurn.gameObject.SetActive(m_shopData.limit_type == 3u);
                
                if (discount && m_shopData.price_now == m_shopData.price_before) //没打折不需要显示折扣
                    discount = false;
                
                transDiscount.gameObject.SetActive(discount);
                if (discount)
                {
                    float fDiscount = m_shopData.price_now * 1.0f / m_shopData.price_before;
                    fDiscount *= 10;
                    txtDiscount.text = fDiscount.ToString("0.0");
                }
                //costOriginal.SetActive(!priceChange && shopData.price_before != shopItem.Price);
                
                transCostOriginal.gameObject.SetActive(discount);
                CSVItem.Data priceTypeData = CSVItem.Instance.GetConfData(m_shopData.price_type);
                if (priceTypeData != null)
                {
                    //ImageHelper.SetIcon(iconOriginal, CSVItem.Instance.GetConfData(shopData.price_type).small_icon_id);
                    ImageHelper.SetIcon(imgCostNow, CSVItem.Instance.GetConfData(m_shopData.price_type).small_icon_id);
                    txtCostNow.text = m_shopData.price_now.ToString();
                    if (discount)
                    {
                        ImageHelper.SetIcon(imgCostOriginal, CSVItem.Instance.GetConfData(m_shopData.price_type).small_icon_id);
                        txtCostOriginal.text = m_shopData.price_before.ToString();
                    }
                }
                else
                {
                    Debug.LogErrorFormat("CSVItem 没有配置  id = {0}", m_shopData.price_type);
                }

                imgSellOut.gameObject.SetActive(false);
                isSellOut = Sys_MallActivity.Instance.IsActivitySellOut(m_ShopItemId); //售完 按全服限购的数量判断;
                imgSellOut.gameObject.SetActive(isSellOut);
                
                transServerCount.gameObject.SetActive(false);
                if (m_shopData.server_limit != 0)
                {
                    transServerCount.gameObject.SetActive(true);
                    int leftNum = Sys_MallActivity.Instance.CalActivityAllServerLeftNum(m_ShopItemId);
                    txtServerCount.text = leftNum.ToString();
                    // txtItemCount.text = LanguageHelper.GetTextContent(2009802, Sys_MallActivity.Instance.CalActivitySelfLeftNum(m_ShopItemId).ToString());
                }

                txtSelfCount.text = "";
                if (m_shopData.personal_limit != 0)
                {
                    int leftNum = Sys_MallActivity.Instance.CalActivitySelfLeftNum(m_ShopItemId);
                    txtSelfCount.text = LanguageHelper.GetTextContent(2025806, leftNum.ToString());
                }

                // //技能书等级特殊显示
                // Sys_Skill.Instance.ShowPetSkillBook(imgSkillBook, itemInfo);
            }
        }
        
        private UI_CurrencyTitle currency;
        private InfinityGrid _infinityGrid;
        private List<ShopGoods> listShopItems = new List<ShopGoods>();
        private CSVActivityShop.Data m_ShopInfo;
        private CSVOperationalActivityRuler.Data rulerData;
        private uint m_ActivityId = 1;
        #region 系统函数
        protected override void Loaded()
        {
            Parse();
        }
        public override void OnDestroy()
        {
            ProcessEventsForEnable(false);
            currency?.Dispose();
            base.OnDestroy();
        }
        public override void Show()
        {
            base.Show();
            ProcessEventsForEnable(false);
            ProcessEventsForEnable(true);
            UpdateView();
        }
        public override void Hide()
        {
            ProcessEventsForEnable(false);
            base.Hide();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_MallActivity.Instance.eventEmitter.Handle(Sys_MallActivity.EEvents.OnFillShopData, OnFillShopData, toRegister);
            Sys_MallActivity.Instance.eventEmitter.Handle(Sys_MallActivity.EEvents.OnRefreshShopData, OnRefreshShopData, toRegister);
        }
        public override bool CheckFunctionIsOpen()
        {
            return base.CheckFunctionIsOpen();
        }
        public override bool CheckTabRedPoint()
        {
            return base.CheckTabRedPoint();
        }
        #endregion
        #region func
        private void Parse()
        {
            currency = new UI_CurrencyTitle(transform.Find("UI_Property").gameObject);
            
            _infinityGrid = transform.Find("Scroll View").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;
            
        }
        
        private void OnCreateCell(InfinityGridCell cell)
        {
            MallItem entry = new MallItem();

            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);

            //dicCells.Add(cell.mRootTransform.gameObject, entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            MallItem entry = cell.mUserData as MallItem;
            entry.UpdateInfo(listShopItems[index], m_ShopInfo != null && m_ShopInfo.Show_Discount != 0);
        }
        
        private void UpdateView()
        {
            rulerData = CSVOperationalActivityRuler.Instance.GetConfData(m_ActivityId);
            m_ShopInfo = CSVActivityShop.Instance.GetConfData(m_ActivityId);
            currency.SetData(m_ShopInfo.Show_Currency);
            Sys_MallActivity.Instance.OnShopDataReq(m_ActivityId);

            // //活动周期
            // CSVOperationalActivityRuler.Data data = CSVOperationalActivityRuler.Instance.GetConfData(m_ActivityId);
            // m_TextTitle.text = LanguageHelper.GetTextContent(data.Activity_Name);
            // DateTime startT = TimeManager.GetDateTime(TimeManager.ConvertFromZeroTimeZone(data.Begining_Date));
            // DateTime endT = startT.AddDays(data.Duration_Day);
            // m_TextDate.text =  LanguageHelper.GetTextContent(591000705, startT.Year.ToString(), startT.Month.ToString(), startT.Day.ToString(), startT.Hour.ToString("D2") , startT.Minute.ToString("D2"),
            //     endT.Year.ToString(), endT.Month.ToString(), endT.Day.ToString(), endT.Hour.ToString("D2"), endT.Minute.ToString("D2"));//
            //
            // ImageHelper.SetIcon(m_AcBG, null, data.Back_Image, false);
            // ImageHelper.SetTexture(m_AcIcon, data.Foreg_Image, true);
        }
        
        private void CalTurnTime()
        {
            // m_TextTime.text = "";
            // bool isTurn = false;
            // for (int i = 0; i < listItems.Count; ++i)
            // {
            //     CSVActivityShopGoods.Data shopData = CSVActivityShopGoods.Instance.GetConfData(listItems[i].GoodsId);
            //     if (shopData != null)
            //     {
            //         if (shopData.limit_type == 2 || shopData.limit_type == 3)
            //         {
            //             isTurn = true;
            //             break;
            //         }
            //     }
            //     else
            //     {
            //         Debug.LogErrorFormat("id {0} 商品未配置", listItems[i].GoodsId);
            //     }
            // }
            //
            // if (!isTurn)
            //     return;
            //
            // //轮换时间
            // uint leftSeconds = 0;
            // try
            // {
            //     DateTime svrtime = TimeManager.GetDateTime(Sys_Time.Instance.GetServerTime());
            //     DateTime turnTime = TimeManager.GetDateTime(Sys_MallActivity.Instance.NextTurnTime);
            //     // Debug.LogError("svTime =" + svrtime.ToString());
            //     // Debug.LogError("turnTime =" + turnTime.ToString());
            //     TimeSpan spawn = turnTime.Subtract(svrtime).Duration();
            //     leftSeconds = (uint)spawn.TotalSeconds;
            // }
            // catch (Exception e)
            // {
            //     Console.WriteLine(e);
            //     throw;
            // }
            //
            // if (leftSeconds > 0)
            // {
            //     m_Timer?.Cancel();
            //     m_Timer = Timer.Register(1f, () =>
            //     {
            //         leftSeconds--;
            //         if (leftSeconds <= 0)
            //             m_Timer?.Cancel();
            //         
            //         m_TextTime.text = LanguageHelper.GetTextContent(2025803,
            //             LanguageHelper.TimeToString(leftSeconds, LanguageHelper.TimeFormat.Type_1));
            //     }, null, true);
            // }
            // //Debug.LogError("leftSeconds =" + leftSeconds.ToString());
            // m_TextTime.text = LanguageHelper.GetTextContent(2025803,
            //     LanguageHelper.TimeToString(leftSeconds, LanguageHelper.TimeFormat.Type_1));
        }

        
        #endregion
        #region event
        
        private void OnFillShopData()
        {
            listShopItems = Sys_MallActivity.Instance.GetActivityShopItems();
            _infinityGrid.CellCount = listShopItems.Count;
            _infinityGrid.ForceRefreshActiveCell();
        }
        
        private void OnRefreshShopData()
        {
            _infinityGrid.ForceRefreshActiveCell();
        }
        #endregion
    }
}
