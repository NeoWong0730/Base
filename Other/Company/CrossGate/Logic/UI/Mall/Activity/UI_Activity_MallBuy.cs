using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic.Core;
using Packet;
using Table;
using Lib.Core;
using UnityEngine.PlayerLoop;

namespace Logic
{
    public class UI_Activity_MallBuy  : UIBase
    {
        private PropItem propItem;
        private Text txtName;

        private Transform transLimit;
        private Text txtLimitNum;

        private UI_Common_Num m_ComNum;
        private Button btnSub;
        private Button btnAdd;
        private Button btnMax;

        private Image imgCostIcon;
        private Text txtCost;

        private Button btnBuy;

        private ShopGoods m_ShopItem;
        private CSVActivityShopGoods.Data shopData;
        private int curBuyNum;
        private int maxBuyNum;

        protected override void OnLoaded()
        {
            Button btnClose = transform.Find("Animator/View_TipsBgNew06/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(()=>{ this.CloseSelf(); });

            propItem = new PropItem();
            propItem.BindGameObject(transform.Find("Animator/View_BuyDetail/Image_ItemBG/PropItem").gameObject);

            transLimit = transform.Find("Animator/View_BuyDetail/Limit");
            txtLimitNum = transform.Find("Animator/View_BuyDetail/Limit/Num").GetComponent<Text>();

            m_ComNum = new UI_Common_Num();
            m_ComNum.Init(transform.Find("Animator/View_BuyDetail/Num/Image_input1"));
            m_ComNum.RegEnd(OnInputEnd);
            btnSub = transform.Find("Animator/View_BuyDetail/Num/Button_Sub").GetComponent<Button>();
            btnSub.onClick.AddListener(OnClickSub);
            btnAdd = transform.Find("Animator/View_BuyDetail/Num/Button_Add").GetComponent<Button>();
            btnAdd.onClick.AddListener(OnClickAdd);
            btnMax = transform.Find("Animator/View_BuyDetail/Num/Button_Max").GetComponent<Button>();
            btnMax.onClick.AddListener(OnClickMax);
            
            imgCostIcon = transform.Find("Animator/View_BuyDetail/Price/Cost_Coin").GetComponent<Image>();
            txtCost = transform.Find("Animator/View_BuyDetail/Price/Cost_Coin/Text_Cost").GetComponent<Text>();
            
            btnBuy = transform.Find("Animator/View_BuyDetail/BtnBuy").GetComponent<Button>();
            btnBuy.onClick.AddListener(OnClickBuy);
        }

        protected override void OnOpen(object arg)
        {
            m_ShopItem = null;
            if (arg != null)
                m_ShopItem = (ShopGoods) arg;
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {            

        }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        protected override void OnHide()
        {
     
        }

        protected override void OnDestroy()
        {

        }

        private void OnInputEnd(uint num)
        {
            if (num <= 1)
            {
                num = 1;
            }
            else if (num > maxBuyNum)
            {
                num = (uint)maxBuyNum;
            }

            curBuyNum = (int)num;
            CalCostPrice();
        }

        private void OnClickSub()
        {
            if (curBuyNum <= 1)
            {
                return;
            }
            
            curBuyNum--;
            CalCostPrice();
        }

        private void OnClickAdd()
        {
            if (curBuyNum >= maxBuyNum)
            {
                return;
            }

            curBuyNum++;
            CalCostPrice();
        }

        private void OnClickMax()
        {
            curBuyNum = maxBuyNum;
            CalCostPrice();
        }

        private void OnClickBuy()
        {
            if (m_ShopItem != null)
            {
                uint price = shopData.price_now * (uint)curBuyNum;
                long curPrice = Sys_Bag.Instance.GetItemCount(shopData.price_type);
                if (curPrice >= price)
                {
                    Sys_MallActivity.Instance.OnBuyReq(m_ShopItem.GoodsId, (uint)curBuyNum);
                    UIManager.CloseUI(EUIID.UI_Activity_MallBuy);
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3940000010));
                }
            }
        }

        private void CalCostPrice()
        {
            if (m_ShopItem != null)
            {
                m_ComNum.SetData((uint)curBuyNum);
                uint price = shopData.price_now * (uint)curBuyNum;
                long curPrice = Sys_Bag.Instance.GetItemCount(shopData.price_type);
                uint lanId = curPrice >= price ? (uint)2007203 : (uint)2007204;
                TextHelper.SetText(txtCost, lanId, price.ToString());
            }
        }

        private void UpdateInfo()
        {
            //Sys_Mall.Instance.OnItemRecordReq(shopId);
            if (m_ShopItem != null)
            {
                shopData = CSVActivityShopGoods.Instance.GetConfData(m_ShopItem.GoodsId);
                if (shopData == null)
                {
                    Debug.LogErrorFormat("id {0} 商品未配置", m_ShopItem.GoodsId);
                    return;
                }

                CSVItem.Data itemData = CSVItem.Instance.GetConfData(shopData.item_id);
                if (itemData == null)
                {
                    Debug.LogErrorFormat("id {0} 道具表未配置", shopData.item_id);
                    return;
                }
                
                CSVItem.Data priceTypeData = CSVItem.Instance.GetConfData(shopData.price_type);
                if (priceTypeData != null)
                {
                    //ImageHelper.SetIcon(iconOriginal, CSVItem.Instance.GetConfData(shopData.price_type).small_icon_id);
                    ImageHelper.SetIcon(imgCostIcon, CSVItem.Instance.GetConfData(shopData.price_type).small_icon_id);
                }

                PropIconLoader.ShowItemData showitem = new PropIconLoader.ShowItemData(shopData.item_id, 1, true, false, false, false, false, false, false, true);
                propItem.SetData(showitem, EUIID.UI_Activity_MallBuy);

                propItem.txtName.text = LanguageHelper.GetTextContent(itemData.name_id);
                if (shopData.Item_Number != 0)
                {
                    propItem.txtNumber.gameObject.SetActive(true); 
                    propItem.txtNumber.text = string.Format("x{0}", shopData.Item_Number.ToString());
                }

                transLimit.gameObject.SetActive(false);
                curBuyNum = 1;
                maxBuyNum = Sys_MallActivity.Instance.CalCanBuyMaxCount(m_ShopItem.GoodsId, true);
                maxBuyNum = maxBuyNum == 0 ? 1 : maxBuyNum;
                if (maxBuyNum != (int)shopData.perPurchase_limit_count)
                {
                    transLimit.gameObject.SetActive(true);
                    txtLimitNum.text = maxBuyNum.ToString();
                }

                CalCostPrice();
            }
        }
    }
}


