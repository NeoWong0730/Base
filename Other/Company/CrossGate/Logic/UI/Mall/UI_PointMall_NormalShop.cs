using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;

namespace Logic
{
    public class UI_PointMall_NormalShop : UIComponent, UI_PointMall.IListener
    {

        protected MallPrama mallPrama;
        protected EPointShopType shopType;

        #region 界面组件
        private UI_PointMall_Left viewLeft;
        private UI_PointMall_Middle viewMiddle;
        protected RectTransform tips2;
        protected RectTransform tips3;
        protected Button btnDailyInfo;
        protected Text txtDailyGetPoint;
        protected Text txtAllPoint;
        #endregion

        #region 系统函数
        protected override void Loaded()
        {
            viewLeft = AddComponent<UI_PointMall_Left>(transform.Find("View_Left"));
            viewMiddle = AddComponent<UI_PointMall_Middle>(transform.Find("View_Middle"));
            btnDailyInfo = transform.Find("View_Left/View_Tips/Tips2/Btn_Check").GetComponent<Button>();
            btnDailyInfo.onClick.AddListener(OnBtnDailyInfoClick);
            tips2 = transform.Find("View_Left/View_Tips/Tips2").GetComponent<RectTransform>();
            tips3 = transform.Find("View_Left/View_Tips/Tips3").GetComponent<RectTransform>();
            txtDailyGetPoint = transform.Find("View_Left/View_Tips/Tips2/Value").GetComponent<Text>();
            txtAllPoint = transform.Find("View_Left/View_Tips/Tips3/Value").GetComponent<Text>();
        }
        public override void OnDestroy()
        {
            viewLeft?.OnDestroy();
            viewMiddle?.OnDestroy();
            base.OnDestroy();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            base.ProcessEventsForEnable(toRegister);

            Sys_Mall.Instance.eventEmitter.Handle<uint>(Sys_Mall.EEvents.OnFillShopData, OnFillShopData, toRegister);
            Sys_Mall.Instance.eventEmitter.Handle(Sys_Mall.EEvents.OnSelectShopItem, OnSelectShopItem, toRegister);
            Sys_Mall.Instance.eventEmitter.Handle(Sys_Mall.EEvents.OnRefreshShopData, OnRefreshShopData, toRegister);
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnDailyPointUpdate, OnDailyPointUpdate, toRegister);
            Sys_Mall.Instance.eventEmitter.Handle(Sys_Mall.EEvents.OnBuyScuccess, OnBuyScuccess, toRegister);

        }
        public override void Show()
        {
            base.Show();
            viewLeft?.UpdateShop((uint)shopType);
            OnRefreshShopData();
            //OnFillShopData((uint)shopType);
            OnDailyPointUpdate();
        }
        #endregion

        #region function
        public EPointShopType getShopType()
        {
            return shopType;
        }
        public void SetMallPrama(MallPrama mp)
        {
            mallPrama = mp;
        }
        #endregion

        #region 响应事件
        protected void OnFillShopData(uint shopId)
        {
            CSVShop.Data shopData = CSVShop.Instance.GetConfData(shopId);
            if (mallPrama != null)
            {
                viewLeft?.UpdateShop(shopId, mallPrama.itemId);
                viewMiddle?.UpdateInfo(Sys_Mall.Instance.SelectShopItemId);
                mallPrama = null;
            }
            else
            {
                viewLeft?.UpdateShop(shopId);
                //Sys_Mall.Instance.SelectShopItemId = 0u;
                viewMiddle?.UpdateInfo(Sys_Mall.Instance.SelectShopItemId);

            }
        }
        protected virtual void OnDailyPointUpdate() { }
        private void OnSelectShopItem()
        {
            viewMiddle?.UpdateInfo(Sys_Mall.Instance.SelectShopItemId);
        }

        private void OnRefreshShopData()
        {
            viewMiddle?.UpdateInfo(Sys_Mall.Instance.SelectShopItemId);
        }

        private void OnBuyScuccess()
        {
            OnDailyPointUpdate();
            viewMiddle?.UpdateCostPirce();
        }
        private void OnBtnDailyInfoClick()
        {
            UIManager.OpenUI(EUIID.UI_PointGetTips, false, getShopType());
        }
        public virtual void OnBtnHelpClick() { }

        #endregion


    }
}
