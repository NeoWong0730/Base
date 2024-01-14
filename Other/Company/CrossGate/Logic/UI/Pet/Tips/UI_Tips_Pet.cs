using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Packet;
using Table;
using Lib.Core;
using Logic.Core;
using Framework;
using static Packet.PetPkAttr.Types;
using System;

namespace Logic
{
    public class UI_Tips_Pet : UIBase, UI_Tips_Pet_Layout.IListener
    {
        private UI_Tips_Pet_Layout layout = new UI_Tips_Pet_Layout();
        private ClientPet curClientPet;
        private PetUnit curPetUnit;
        private TradeMsgBoxParam msgParam;
        private TradeItem tradeItem;

        private int petCounts;
        private int petIndex;
        public UI_Pet_Detail_View view;

        protected override void OnOpen(object arg)
        {
            msgParam = (TradeMsgBoxParam)arg;
            tradeItem = msgParam.tradeItem;
            curPetUnit = tradeItem.Pet;
            curClientPet = new ClientPet(curPetUnit);
        }

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
            view = new UI_Pet_Detail_View(curClientPet);
            view.Init(gameObject.transform);

            //设置价格icon
            CSVItem.Data item = CSVItem.Instance.GetConfData(2u);
            ImageHelper.SetIcon(layout.imgPrice, item.small_icon_id);
        }

        protected override void OnShow()
        {
            view.Show();
            SetValue();
            UpdatePageState();
        }

        protected override void OnHide()
        {
            view.Hide();
        }

        protected override void OnDestroy()
        {
            view.OnDestroy();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Society.Instance.eventEmitter.Handle<Sys_Society.RoleInfo>(Sys_Society.EEvents.OnGetBriefInfoSuccess, OnGetBriefInfoSuccess, toRegister);
            Sys_Trade.Instance.eventEmitter.Handle<TradeMsgBoxParam>(Sys_Trade.EEvents.OnTipsInfo, OnTipsInfo, toRegister);
        }

        #region Function

        private void UpdateInfo()
        {
            SetValue();
            layout.shareList.Hide();
            UpdatePageState();
            //view.OnDestroy();
            view.UpdateClientData(curClientPet);
            //view = new UI_Pet_Detail_View(curClientPet);
            //view.Init(gameObject.transform);
        }

        private void OnTipsInfo(TradeMsgBoxParam param)
        {
            msgParam = param;
            tradeItem = msgParam.tradeItem;
            curPetUnit = tradeItem.Pet;
            curClientPet = new ClientPet(curPetUnit);
            UpdateInfo();
        }

        private void OnGetBriefInfoSuccess(Sys_Society.RoleInfo roleInfo)
        {
            Sys_Society.Instance.OpenPrivateChat(roleInfo);

            this.CloseSelf();
        }

        private void SetValue()
        {
            UpdatePriceAndTime();
        }

        private void UpdatePriceAndTime()
        {
            bool isPublicity = false;

            CSVCommodity.Data goodData = CSVCommodity.Instance.GetConfData(tradeItem.InfoId);
            if (goodData != null)
            {
                if (goodData.publicity)
                {
                    isPublicity = tradeItem.OnsaleTime > Sys_Time.Instance.GetServerTime();
                }
            }

            layout.textPublicityTime.text = "";
            if (isPublicity)
            {
                uint leftTime = tradeItem.OnsaleTime - Sys_Time.Instance.GetServerTime();
                layout.textPublicityTime.text = string.Format("{0}\n{1}", 
                    LanguageHelper.GetTextContent(2011004u),
                    LanguageHelper.TimeToString(leftTime, LanguageHelper.TimeFormat.Type_1));
            }

            bool isBargin = tradeItem.Price == 0u;
            if (isBargin)
                layout.textPrice.text = LanguageHelper.GetTextContent(2011090u);
            else
                layout.textPrice.text = tradeItem.Price.ToString();
        }


        private void UpdatePageState()
        {
            bool isMulti = (msgParam.idlist != null) && (msgParam.idlist.Count > 1);
            layout.leftArrow.gameObject.SetActive(isMulti);
            layout.rightArrow.gameObject.SetActive(isMulti);

            petCounts = 0;
            petIndex = 0;
            if (isMulti)
            {
                petCounts = msgParam.idlist.Count;
                petIndex = msgParam.idlist.IndexOf(msgParam.tradeItem.GoodsUid);
            }
        }

        #endregion


        #region ButtonAndToggle
        public void OnClickRelation()
        {
            if (tradeItem != null  && tradeItem.SellerId == Sys_Role.Instance.RoleId)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011166));
                return;
            }

            Sys_Society.Instance.ReqGetBriefInfo(tradeItem.SellerId.ToString());

            //Sys_Society.RoleInfo roleInfo = new Sys_Society.RoleInfo();
            //roleInfo.roleID = tradeItem.SellerId;
            //roleInfo.roleName = tradeItem.SellerName.ToStringUtf8();
            //roleInfo.heroID = tradeItem.HeroId;
            //Sys_Society.Instance.OpenPrivateChat(roleInfo);
            //this.CloseSelf();
        }

        public void OnClickShare()
        {
            layout.shareList.Show();
            layout.shareList.SetTradeData(tradeItem);
        }

        public void OnClickPageLeft()
        {
            if (petIndex > 0)
            {
                petIndex--;

                ulong desId = msgParam.idlist[petIndex];
                Sys_Trade.Instance.OnDetailInfoReq(msgParam.bCross, desId, Sys_Trade.DetailSourceType.EquipOrPet);
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011236u));
            }
        }

        public void OnClickPageRight()
        {
            if (petIndex < petCounts - 1)
            {
                petIndex++;

                ulong desId = msgParam.idlist[petIndex];
                Sys_Trade.Instance.OnDetailInfoReq(msgParam.bCross, desId, Sys_Trade.DetailSourceType.EquipOrPet);
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011237u));
            }
        }
        #endregion
    }
}
