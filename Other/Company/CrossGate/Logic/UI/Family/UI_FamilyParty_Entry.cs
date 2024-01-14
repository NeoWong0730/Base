using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine.UI;
using UnityEngine;
using System;
using Lib.Core;
using Framework;
using Logic.Core;
using Packet;
using UnityEngine.Playables;

namespace Logic
{
    public class UI_FamilyParty_Entry : UIComponent
    {
        private Button btnClose;
        private Text txtType;
        private Text txtLv;
        private Text txtTime;
        private Text txtDesc;
        private PropItem propItem;
        private Button btnJoin;
        private Text txtBtnJoin;
        private Text txtCDTitle;
        private Text txtCountDown;
        private Timer timer;
        private float countDownTime = 0;
        private Image imgActivity;//活动图
        #region 系统函数
        protected override void Loaded()
        {
            Parse();
        }
        public override void OnDestroy()
        {
            timer?.Cancel();
            base.OnDestroy();
        }
        public override void Show()
        {
            base.Show();
            Sys_Family.Instance.GetCuisineInfoReq();
            UpdateView();
        }
        public override void Hide()
        {
            timer?.Cancel();
            base.Hide();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.OnPartyDataUpdate, UpdateView, toRegister);
        }
        #endregion
        #region func
        private void Parse()
        {
            btnClose = transform.Find("Animator/Image_Bg/Black").GetComponent<Button>();
            btnClose.onClick.AddListener(OnBtnCloseClick);
            txtType = transform.Find("Animator/View_Content/Text_Type/Text").GetComponent<Text>();
            txtLv = transform.Find("Animator/View_Content/Text_LV/Text").GetComponent<Text>();
            txtTime = transform.Find("Animator/View_Content/Text_Time/Text").GetComponent<Text>();
            txtDesc = transform.Find("Animator/View_Content/Text_Content").GetComponent<Text>();
            propItem = new PropItem();
            propItem.BindGameObject(transform.Find("Animator/View_Content/PropItem").gameObject);
            btnJoin = transform.Find("Animator/View_Content/Btn_01").GetComponent<Button>();
            btnJoin.onClick.AddListener(OnBtnJoinClick);
            txtBtnJoin = transform.Find("Animator/View_Content/Btn_01/Text_01").GetComponent<Text>();
            txtCDTitle = transform.Find("Animator/View_Content/Text_CountDown").GetComponent<Text>();
            txtCountDown = transform.Find("Animator/View_Content/Text_CountDown/Text").GetComponent<Text>();
            imgActivity = transform.Find("Animator/ShopItem/Image2").GetComponent<Image>();
        }
        private void UpdateView()
        {
            //活动形式
            txtType.text = LanguageHelper.GetTextContent(6247);
            //等级限制
            txtLv.text = LanguageHelper.GetTextContent(6248);
            //活动时间
            txtTime.text = LanguageHelper.GetTextContent(6249);
            //活动描述
            txtDesc.text = LanguageHelper.GetTextContent(6250);

            var id = Sys_Family.Instance.GetFashionFoodId();
            var itemData = new PropIconLoader.ShowItemData(id, 0, true, false, false, false, false);
            propItem.SetData(itemData, EUIID.UI_Family);
            CSVFamilyActive.Data activeData = CSVFamilyActive.Instance.GetConfData(10);
            ImageHelper.SetIcon(imgActivity, activeData.familyActiveIcon);
            UpdateCountDown();
        }
        private void UpdateCountDown()
        {
            uint nowtime = Sys_Time.Instance.GetServerTime();
            var partyStartTime = Sys_Family.Instance.GetPartyStartTimestamp();
            var partyEndTime = Sys_Family.Instance.GetPartyEndTimestamp();
            var zeroTime = Sys_Time.Instance.GetDayZeroTimestamp();
            if (nowtime >= zeroTime + 5 * 3600 && nowtime < partyStartTime)
            {
                btnJoin.gameObject.SetActive(true);
                txtBtnJoin.text = LanguageHelper.GetTextContent(6270);//筹备
                countDownTime = partyStartTime - nowtime;
                timer?.Cancel();
                timer = Timer.Register(countDownTime, OnTimerComplete, OnTimerUpdate, false, false);
            }
            else
            {
                btnJoin.gameObject.SetActive(true);
                txtBtnJoin.text = LanguageHelper.GetTextContent(2010205);//参加
                txtCDTitle.text = "";
                txtCountDown.text = LanguageHelper.GetTextContent(6254); //"进行中";
            }
            //else if (nowtime > partyEndTime)
            //{
            //    btnJoin.gameObject.SetActive(false);
            //    txtCDTitle.text = "";
            //    txtCountDown.text = LanguageHelper.GetTextContent(6255); //"已结束";
            //}
        }
        #endregion
        #region event
        private void OnBtnCloseClick()
        {
            this.Hide();
        }
        private void OnBtnJoinClick()
        {
            if (Sys_Family.Instance.GoToFamilyParty())
            {
                UIManager.CloseUI(EUIID.UI_Family);
            }
        }
        private void OnTimerComplete()
        {
            timer?.Cancel();
            UpdateView();
        }
        private void OnTimerUpdate(float time)
        {
            if (txtCountDown != null && countDownTime >= time)
                txtCountDown.text = LanguageHelper.TimeToString((uint)(countDownTime - time), LanguageHelper.TimeFormat.Type_1);
        }
        #endregion

    }
}
