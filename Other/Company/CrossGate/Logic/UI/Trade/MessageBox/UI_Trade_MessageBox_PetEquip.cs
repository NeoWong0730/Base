using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Logic.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_Trade_MessageBox_PetEquip : UIBase
    {
        //private UI_Trade_Message_ComItem _comItem;
        private UI_Trade_Message_PetEquipItem _selfOrnament;
        private UI_Trade_Message_PetEquipItem _rightOrnament;
        //private UI_Trade_Message_PetItem _petItem;

        private Button _btnRelation;
        private Button _btnShare;

        private UI_Trade_MessageBox_Share _shareList;

        private Transform leftArrow;
        private Button btnLeft;
        private Transform rightArrow;
        private Button btnRight;

        private TradeMsgBoxParam msgParam;
        private CSVCommodity.Data _commodityData;

        private int pageIndex;
        private int pageCount;

        protected override void OnOpen(object arg)
        {
            msgParam = null;
            if (arg != null)
                msgParam = (TradeMsgBoxParam)arg;

            if (msgParam.tradeItem != null)
                _commodityData = CSVCommodity.Instance.GetConfData(msgParam.tradeItem.InfoId);
        }

        protected override void OnLoaded()
        {
            //Button btnPath = transform.Find("Animator/View_Message/Button").GetComponent<Button>();
            ////btnPath.onClick.AddListener(OnClickPath);
            //btnPath.gameObject.SetActive(false);

            _selfOrnament = new UI_Trade_Message_PetEquipItem();
            _selfOrnament.Init(transform.Find("View_Tips/Tips02"));

            _rightOrnament = new UI_Trade_Message_PetEquipItem();
            _rightOrnament.Init(transform.Find("View_Tips/Tips01"));

            //_petItem = new UI_Trade_Message_PetItem();
            //_petItem.Init(transform.Find("Animator/View_Pet"));

            _btnRelation = transform.Find("View_Tips/View_Button/Button_1").GetComponent<Button>();
            _btnRelation.onClick.AddListener(OnClickRelation);
            _btnShare = transform.Find("View_Tips/View_Button/Button_2").GetComponent<Button>();
            _btnShare.onClick.AddListener(OnClickShare);

            _shareList = new UI_Trade_MessageBox_Share();
            _shareList.Init(transform.Find("View_Tips/GroupList"));

            leftArrow = transform.Find("View_Tips/View_PageTurn/Arrow_Left");
            btnLeft = transform.Find("View_Tips/View_PageTurn/Arrow_Left/Button_Left").GetComponent<Button>();
            btnLeft.onClick.AddListener(OnClickPageLeft);
            rightArrow = transform.Find("View_Tips/View_PageTurn/Arrow_Right");
            btnRight = transform.Find("View_Tips/View_PageTurn/Arrow_Right/Button_Right").GetComponent<Button>();
            btnRight.onClick.AddListener(OnClickPageRight);

            Button btnClose = transform.Find("Close").GetComponent<Button>();
            btnClose.onClick.AddListener(OnClickClose);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Society.Instance.eventEmitter.Handle<Sys_Society.RoleInfo>(Sys_Society.EEvents.OnGetBriefInfoSuccess, OnGetBriefInfoSuccess, toRegister);
            Sys_Trade.Instance.eventEmitter.Handle<TradeMsgBoxParam>(Sys_Trade.EEvents.OnTipsInfo, OnTipsInfo, toRegister);
        }

        private void OnGetBriefInfoSuccess(Sys_Society.RoleInfo roleInfo)
        {
            Sys_Society.Instance.OpenPrivateChat(roleInfo);

            this.CloseSelf();
        }

        protected override void OnShow()
        {
            UpdateInfo();
        }        

        protected override void OnDestroy()
        {
            _selfOrnament?.OnDestroy();
            _rightOrnament?.OnDestroy();
        }

        private void OnClickClose()
        {
            this.CloseSelf();
        }

        private void OnClickPath()
        {
            //_pathResource.Show();
            //_pathResource.SetItemInfoId(_tradeItem.InfoId);
        }

        private void OnClickRelation()
        {
            if (msgParam != null && msgParam.tradeItem.SellerId == Sys_Role.Instance.RoleId)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011166));
                return;
            }

            Sys_Society.Instance.ReqGetBriefInfo(msgParam.tradeItem.SellerId.ToString());
        }

        private void OnClickShare()
        {
            _shareList.Show();
            _shareList.SetTradeData(msgParam.tradeItem);
        }


        public void OnClickPageLeft()
        {
            if (pageIndex > 0)
            {
                pageIndex--;

                ulong desId = msgParam.idlist[pageIndex];
                Sys_Trade.Instance.OnDetailInfoReq(msgParam.bCross, desId, Sys_Trade.DetailSourceType.EquipOrPet);
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011236u));
            }
        }

        public void OnClickPageRight()
        {
            if (pageIndex < pageCount - 1)
            {
                pageIndex++;

                ulong desId = msgParam.idlist[pageIndex];
                Sys_Trade.Instance.OnDetailInfoReq(msgParam.bCross, desId, Sys_Trade.DetailSourceType.EquipOrPet);
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011237u));
            }
        }

        private void OnTipsInfo(TradeMsgBoxParam param)
        {
            msgParam = param;
            if (msgParam.tradeItem != null)
                _commodityData = CSVCommodity.Instance.GetConfData(msgParam.tradeItem.InfoId);

            UpdateInfo();
        }

        private void UpdateInfo()
        {
            //_comItem.Hide();
            //_equipItem.Hide();
            //_petItem.Hide();
            _btnRelation.gameObject.SetActive(_commodityData.contact_seller && !Sys_Trade.Instance.IsCrossServer());
            _btnShare.gameObject.SetActive(_commodityData.share);

            _rightOrnament.Show();
            _rightOrnament.UpdateInfo(msgParam.tradeItem);

            _shareList.Hide();

            bool isMulti = (msgParam.idlist != null) && (msgParam.idlist.Count > 1);
            leftArrow.gameObject.SetActive(isMulti);
            rightArrow.gameObject.SetActive(isMulti);

            pageCount = 0;
            pageIndex = 0;
            if (isMulti)
            {
                pageCount = msgParam.idlist.Count;
                pageIndex = msgParam.idlist.IndexOf(msgParam.tradeItem.GoodsUid);
            }

            _selfOrnament.Hide();

            // ItemData selfItem = null;
            // if (Sys_Pet.Instance.IsShowCompare(msgParam.tradeItem.InfoId, petUid, ref selfItem))
            // {
            //     //layout.leftCompare.gameObject.SetActive(isCompare && true);
            //     _selfOrnament.Show();
            //     _selfOrnament.UpdateInfo(selfItem);
            // }
        }
    }
}


