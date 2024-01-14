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
    public class TradeMsgBoxParam
    {
        public bool bCross;
        //public bool isShowTip;
        public TradeItem tradeItem;
        public Sys_Trade.DetailSourceType srcType = Sys_Trade.DetailSourceType.None;
        public List<ulong> idlist;
    }

    public class UI_Trade_MessageBox_Com : UIBase
    {
        private UI_Trade_Message_ComItem _comItem;
        //private UI_Trade_Message_EquipItem _equipItem;
        //private UI_Trade_Message_PetItem _petItem;

        private Button _btnRelation;
        private Button _btnShare;

        private UI_Trade_MessageBox_Share _shareList;

        private TradeMsgBoxParam _param;

        //private TradeItem _tradeItem;
        private CSVCommodity.Data _commodityData;

        protected override void OnOpen(object arg)
        {
            _param = null;
            if (arg != null)
                _param = (TradeMsgBoxParam)arg;

            if (_param.tradeItem != null)
                _commodityData = CSVCommodity.Instance.GetConfData(_param.tradeItem.InfoId);
        }

        protected override void OnLoaded()
        {
            //Button btnPath = transform.Find("Animator/View_Message/Button").GetComponent<Button>();
            ////btnPath.onClick.AddListener(OnClickPath);
            //btnPath.gameObject.SetActive(false);

            _comItem = new UI_Trade_Message_ComItem();
            _comItem.Init(transform.Find("Animator/View_Message"));

            //_equipItem = new UI_Trade_Message_EquipItem();
            //_equipItem.Init(transform.Find("Animator/View_Equip"));

            //_petItem = new UI_Trade_Message_PetItem();
            //_petItem.Init(transform.Find("Animator/View_Pet"));

            _btnRelation = transform.Find("Animator/View_Button/Button_1").GetComponent<Button>();
            _btnRelation.onClick.AddListener(OnClickRelation);
            _btnShare = transform.Find("Animator/View_Button/Button_2").GetComponent<Button>();
            _btnShare.onClick.AddListener(OnClickShare);

            _shareList = new UI_Trade_MessageBox_Share();
            _shareList.Init(transform.Find("Animator/GroupList"));

            Button btnClose = transform.Find("ClickClose").GetComponent<Button>();
            btnClose.onClick.AddListener(OnClickClose);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Society.Instance.eventEmitter.Handle<Sys_Society.RoleInfo>(Sys_Society.EEvents.OnGetBriefInfoSuccess, OnGetBriefInfoSuccess, toRegister);
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
        
        //protected override void OnDestroy()
        //{
        //    //_equipItem?.OnDestroy();
        //}

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
            if (_param != null && _param.tradeItem.SellerId == Sys_Role.Instance.RoleId)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011166));
                return;
            }

            //Sys_Society.RoleInfo roleInfo = new Sys_Society.RoleInfo();
            //roleInfo.roleID = _param.tradeItem.SellerId;
            //roleInfo.roleName = _param.tradeItem.SellerName.ToStringUtf8();
            //roleInfo.heroID = _param.tradeItem.HeroId;
            //Sys_Society.Instance.OpenPrivateChat(roleInfo);

            Sys_Society.Instance.ReqGetBriefInfo(_param.tradeItem.SellerId.ToString());
        }

        private void OnClickShare()
        {
            _shareList.Show();
            _shareList.SetTradeData(_param.tradeItem);
        }

        private void UpdateInfo()
        {
            //_comItem.Hide();
            //_equipItem.Hide();
            //_petItem.Hide();
            _btnRelation.gameObject.SetActive(_commodityData.contact_seller && !Sys_Trade.Instance.IsCrossServer());
            _btnShare.gameObject.SetActive(_commodityData.share);

            _comItem.Show();
            _comItem.UpdateInfo(_param.tradeItem);
            _shareList.Hide();
        }
    }
}


