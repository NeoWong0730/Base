using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic.Core;
using Packet;
using Table;

namespace Logic
{
    public enum EPointShopType
    {
        //Common
        LovePoint=1,
        //商店表ID
        Aid = 5011, //援助值
        Captain = 5012, //队长积分
        Arena = 1006, //竞技场积分
        TravelersLog = 5013, //旅人志积分
        AncientCoin = 5014,       //古币
        TransfigurationCard= 5015,   //变身积分
    }

    public enum EPointType
    {
        //道具表ID
        Aid = 13, //援助值
        Captain = 14, //队长积分
        Arena= 12,   // 竞技场积分
        TravelersLog = 15,   // 旅人币
        AncientCoin = 18, //古币
        TransfigurationCard = 24,  //变身积分
    }

    public class UI_PointMall : UIBase, UI_Mall_Right.IListener
    {
        public interface IListener
        {
            EPointShopType getShopType();
            void SetMallPrama(MallPrama mp);
        }

        private UI_CurrencyTitle currency;
        private UI_Mall_Right viewRight;
        private Text textMallName;

        private UI_PointMall_AidShop viewAid;
        private UI_PointMall_CaptainShop viewCap;
        private UI_PointMall_ArenaShop viewArena;
        private UI_PointMall_TravelShop viewTravel;
        private UI_PointMall_AncientCoinShop viewAncientCoin;
        private UI_PointMall_Transfiguration viewTransfigutationCard;

        private Button m_BtnMall;

        private MallPrama mallPrama;
        private List<EPointShopType> shopList = new List<EPointShopType>();
        private Dictionary<EPointShopType,UIComponent> shopDict = new Dictionary<EPointShopType,UIComponent>();
        private Dictionary<EPointShopType, IListener> normalShopDict = new Dictionary<EPointShopType, IListener>();

        protected override void OnLoaded()
        {            
            currency = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);

            Button btnClose = transform.Find("Animator/View_Title08/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(Close);

            viewRight = AddComponent<UI_Mall_Right>(transform.Find("Animator/View_Right"));
            viewRight.RegisterListener(this);

            viewAid = AddComponent<UI_PointMall_AidShop>(transform.Find("Animator/View_AidShop"));
            shopList.Add(viewAid.getShopType());
            shopDict.Add(viewAid.getShopType(), viewAid);
            normalShopDict.Add(viewAid.getShopType(), viewAid);
            viewAid.Hide();

            viewCap = AddComponent<UI_PointMall_CaptainShop>(transform.Find("Animator/View_CaptainShop"));
            shopList.Add(viewCap.getShopType());
            shopDict.Add(viewCap.getShopType(), viewCap);
            normalShopDict.Add(viewCap.getShopType(), viewCap);
            viewCap.Hide();

            viewArena = AddComponent<UI_PointMall_ArenaShop>(transform.Find("Animator/View_ArenaShop"));
            shopList.Add(viewArena.getShopType());
            shopDict.Add(viewArena.getShopType(), viewArena);
            normalShopDict.Add(viewArena.getShopType(), viewArena);
            viewArena.Hide();

            viewTravel = AddComponent<UI_PointMall_TravelShop>(transform.Find("Animator/View_TravelShop"));
            shopList.Add(viewTravel.getShopType());
            shopDict.Add(viewTravel.getShopType(), viewTravel);
            normalShopDict.Add(viewTravel.getShopType(), viewTravel);
            viewTravel.Hide();

            viewAncientCoin = AddComponent<UI_PointMall_AncientCoinShop>(transform.Find("Animator/View_gubiShop"));
            shopList.Add(viewAncientCoin.getShopType());
            shopDict.Add(viewAncientCoin.getShopType(), viewAncientCoin);
            normalShopDict.Add(viewAncientCoin.getShopType(), viewAncientCoin);
            viewAncientCoin.Hide();

            viewTransfigutationCard = AddComponent<UI_PointMall_Transfiguration>(transform.Find("Animator/View_Transform"));
            shopList.Add(viewTransfigutationCard.getShopType());
            shopDict.Add(viewTransfigutationCard.getShopType(), viewTransfigutationCard);
            normalShopDict.Add(viewTransfigutationCard.getShopType(), viewTransfigutationCard);
            viewTransfigutationCard.Hide();

            textMallName = transform.Find("Animator/View_Title08/Text_Title").GetComponent<Text>();

            m_BtnMall = transform.Find("Animator/Btn_Mall").GetComponent<Button>();
            m_BtnMall.onClick.AddListener(OnClickMall);
        }

        protected override void OnOpen(object arg)
        {            
            if (arg != null)
            {
                mallPrama = (MallPrama)arg;
            }
            else
            {
                mallPrama = null;
            }
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            Sys_Mall.Instance.eventEmitter.Handle<uint>(Sys_Mall.EEvents.OnFillShopData, OnFillShopData, toRegister);
        }

        protected override void OnShow()
        {            
            UpdateMall();
        }

        protected override void OnHide()
        {
            Sys_Mall.Instance.ClearData();            
        }

        protected override void OnDestroy()
        {
            shopList.Clear();
            shopDict.Clear();
            normalShopDict.Clear();
            currency?.Dispose();
            viewAid?.OnDestroy();
            viewCap?.OnDestroy();
            viewArena?.OnDestroy();
            viewTravel?.OnDestroy();
            viewAncientCoin?.OnDestroy();      
            viewTransfigutationCard?.OnDestroy();
        }

        private void OnClickMall()
        {
            this.CloseSelf();
            UIManager.OpenUI(EUIID.UI_Mall, false, new MallPrama() { mallId = 101 });
        }

        private void UpdateMall()
        {
            if (mallPrama == null)
            {
                Debug.LogError("mall Param is Error");
                return;
            }

            CSVMall.Data mallData = CSVMall.Instance.GetConfData(mallPrama.mallId);
            if (mallData != null)
            {
                viewRight?.UpdateInfo(mallPrama.mallId, mallPrama.shopId);
                textMallName.text = LanguageHelper.GetTextContent(mallData.mall_name);
            }
            else
            {
                Debug.LogErrorFormat("mall is not found id={0}", mallPrama.mallId);
            }
        }

        public void OnSelectShop(uint shopId)
        {
            CSVShop.Data shopData = CSVShop.Instance.GetConfData(shopId);
            if (shopData != null)
                currency.SetData(shopData.show_currency);
            Sys_Mall.Instance.OnItemRecordReq(shopId);

            //积分商店埋点
            UIManager.HitPointShow(EUIID.UI_PointMall, shopId.ToString());
        }

        private void OnFillShopData(uint shopId)
        {
            for (int i = 0; i < shopList.Count; i++)
            {
                EPointShopType key = shopList[i];
                if ((uint)key == shopId)
                {
                    if(normalShopDict.TryGetValue(key, out IListener curShop))
                    {
                        curShop.SetMallPrama(mallPrama);
                        mallPrama = null;
                    }
                    shopDict[key].Show();
                }
                else
                {
                    shopDict[key].Hide();
                }
            }
        }

        private void Close()
        {
            UIManager.CloseUI(EUIID.UI_PointMall);
        }

        public void OnToggleCharge(bool toggle)
        {
            throw new System.NotImplementedException();
        }
    }
}


