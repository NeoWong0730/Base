using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic.Core;
using Packet;
using Table;
using Lib.Core;

namespace Logic
{
    public class MallPrama
    {
        public uint mallId;
        public uint shopId;
        public uint itemId;
        public bool isCharge = false;
    }

    public class UI_Mall : UIBase, UI_Mall_Right.IListener
    {
        private UI_CurrencyTitle currency;
        private UI_Mall_Right viewRight;
        private UI_Mall_Left viewLeft;
        private UI_Mall_Middle viewMiddle;
        private UI_Mall_Charge viewCharge;

        private Text textMallName;

        private MallPrama mallPrama;

        private ButtonList btnHelp;

        private Button m_BtnPointMall;

        private Timer m_Timer;

        protected override void OnLoaded()
        {            
            currency = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);

            Button btnClose = transform.Find("Animator/View_Title08/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(()=>{ this.CloseSelf(); });

            viewRight = AddComponent<UI_Mall_Right>(transform.Find("Animator/View_Right"));
            viewRight.RegisterListener(this);

            viewLeft = AddComponent<UI_Mall_Left>(transform.Find("Animator/View_Left"));
            viewMiddle = AddComponent<UI_Mall_Middle>(transform.Find("Animator/View_Middle"));
            viewCharge = AddComponent<UI_Mall_Charge>(transform.Find("Animator/View_Charge"));

            textMallName = transform.Find("Animator/View_Title08/Text_Title").GetComponent<Text>();

            btnHelp = transform.GetComponent<ButtonList>();

            m_BtnPointMall = transform.Find("Animator/Btn_PointMall").GetComponent<Button>();
            m_BtnPointMall.onClick.AddListener(OnClickPointMall);
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
            if (Sys_Mall.Instance.skip2MallFromItemSource != null)
            {
                mallPrama = Sys_Mall.Instance.skip2MallFromItemSource;
            }
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            Sys_Mall.Instance.eventEmitter.Handle<uint>(Sys_Mall.EEvents.OnFillShopData, OnFillShopData, toRegister);
            Sys_Mall.Instance.eventEmitter.Handle(Sys_Mall.EEvents.OnSelectShopItem, OnSelectShopItem, toRegister);
            Sys_Mall.Instance.eventEmitter.Handle(Sys_Mall.EEvents.OnRefreshShopData, OnRefreshShopData, toRegister);
            Sys_Mall.Instance.eventEmitter.Handle(Sys_Mall.EEvents.OnTelCharge, OnTelCharge, toRegister);
            Sys_Mall.Instance.eventEmitter.Handle(Sys_Mall.EEvents.OnRefreshRedDot, OnRefreshRedDot, toRegister);
        }

        protected override void OnShow()
        {            
            UpdateMall();

            m_Timer?.Cancel();
            m_Timer = null;
            m_Timer = Timer.Register(1f, this.CheckNetTip, null, true);
        }

        protected override void OnHide()
        {
            m_Timer?.Cancel();
            m_Timer = null;
            Sys_Mall.Instance.ClearData();            
        }

        protected override void OnDestroy()
        {
            currency?.Dispose();
            viewLeft?.OnDestroy();
            viewMiddle?.OnDestroy();
            viewCharge?.OnDestroy();
        }

        private void CheckNetTip()
        {
            if (Net.NetClient.Instance.eNetStatus != Net.NetClient.ENetState.Connected)
            {
                m_Timer?.Cancel();
                m_Timer = null;
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(1610000002);//此处需要策划配置LanguageHelper.GetTextContent(11720)
                PromptBoxParameter.Instance.SetConfirm(true, ()=> {
                    UIManager.CloseUI(EUIID.UI_Mall);
                });
                PromptBoxParameter.Instance.SetCancel(false, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, true, PromptBoxParameter.Instance);
            }
        }

        private void OnClickPointMall()
        {
            this.CloseSelf();
            UIManager.OpenUI(EUIID.UI_PointMall, false, new MallPrama { mallId = 501});
        }

        private void UpdateMall()
        {
            if (mallPrama == null)
            {
                Debug.LogError("mall Param is Error");
                return;
            }

            m_BtnPointMall.gameObject.SetActive(mallPrama.mallId == 101u); //策划说写死id

            CSVMall.Data mallData = CSVMall.Instance.GetConfData(mallPrama.mallId);
            if (mallData != null)
            {
                viewRight?.UpdateInfo(mallPrama.mallId, mallPrama.shopId, mallPrama.isCharge);
                textMallName.text = LanguageHelper.GetTextContent(mallData.mall_name);

                if (mallPrama.isCharge)
                    currency.SetData(new List<uint>() { 1, 2, 3});

                if (mallData.args_num == -1)
                {
                    btnHelp.ShowBtn(false);
                }
                else
                {
                    btnHelp.ShowBtn(true);
                    btnHelp.SetRuleId(mallData.args_num);
                }
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

            viewMiddle?.Hide();
            Sys_Mall.Instance.OnItemRecordReq(shopId);

            //商店埋点
            UIManager.HitPointShow(EUIID.UI_Mall, shopId.ToString());
        }

        private void OnFillShopData(uint shopId)
        {
            //Debug.LogError(shopId);
            viewMiddle?.Show();
            if (mallPrama != null)
            {
                viewLeft?.UpdateShop(shopId, mallPrama.itemId);
                viewMiddle?.UpdateInfo(Sys_Mall.Instance.SelectShopItemId);
                mallPrama = null;
            }
            else
            {
                viewLeft?.UpdateShop(shopId);
                viewMiddle?.UpdateInfo(Sys_Mall.Instance.SelectShopItemId);
            }
        }

        private void OnSelectShopItem()
        {
            viewMiddle?.UpdateInfo(Sys_Mall.Instance.SelectShopItemId);
        }

        private void OnRefreshShopData()
        {
            viewMiddle?.UpdateInfo(Sys_Mall.Instance.SelectShopItemId);
        }
        
        private void OnTelCharge()
        {
            mallPrama = new MallPrama();
            mallPrama.mallId = 101u;
            mallPrama.isCharge = true;

            UpdateMall();
        }

        private void OnRefreshRedDot()
        {
            viewRight?.RefreshRedDot();
        }

        private uint chargeShowTime = 0;
        public void OnToggleCharge(bool toggle)
        {
            if(toggle)
            {
                viewCharge.Show();
                viewCharge.UpdateCharge();

                chargeShowTime = Sys_Time.Instance.GetServerTime();
                UIManager.HitPointShow(EUIID.UI_Mall, "Charge");
            }
            else
            {
                viewCharge.Hide();

                uint hideTime = Sys_Time.Instance.GetServerTime();
                if (chargeShowTime != 0 && hideTime >= chargeShowTime)
                {
                    UIManager.HitPointHide(EUIID.UI_Mall, hideTime - chargeShowTime, "Charge");
                }
                
            }
        }
    }
}


