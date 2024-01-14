using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic.Core;
using Packet;
using Table;
using Framework;
using System;
using Lib.Core;

namespace Logic
{
    public class UI_Activity_Nien_Challenge  : UIBase
    {
        private Image imgCurrency;
        private Text txtCurrency;
        private Button btnCurrencyAdd;
        
        private Text txtCallNienDate;
        private Text txtChallengeCount;
        private Button btnChallenge;
        
        private PropItem srcItem;
        private PropItem desItem;
        private Text txtExchangeTime;
        private Button btnExchange;

        private Text txtDamage;
        private Text txtGetState;
        private Button btnGet;
        private Text txtGot;
        
        private Button btnRank;
        private Button btnRward;

        private uint secondStageStartTime;
        private uint secondStageEndTime;
        private uint srcItemId;
        private uint desItemId;
        private uint exchangeRate;
        
        protected override void OnLoaded()
        {
            Button btnClose = transform.Find("Animator/View_TipsBgNew01/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(OnClickClose);
            
            imgCurrency = transform.Find("Animator/Image_Property01/Image_Icon").GetComponent<Image>();
            txtCurrency = transform.Find("Animator/Image_Property01/Text_Number").GetComponent<Text>();
            btnCurrencyAdd = transform.Find("Animator/Image_Property01/Button_Add").GetComponent<Button>();
            btnCurrencyAdd.onClick.AddListener(OnClickAddCurrency);
            
            txtCallNienDate = transform.Find("Animator/Chanllenge/Text_Nien").GetComponent<Text>();
            txtChallengeCount = transform.Find("Animator/Chanllenge/Text_Time").GetComponent<Text>();
            btnChallenge = transform.Find("Animator/Chanllenge/Btn_01").GetComponent<Button>();
            btnChallenge.onClick.AddListener(OnClickChallenge);
            
            srcItem = new PropItem();
            srcItem.BindGameObject(transform.Find("Animator/Exchange/PropItem").gameObject);
            desItem = new PropItem();
            desItem.BindGameObject(transform.Find("Animator/Exchange/PropItem (1)").gameObject);
            txtExchangeTime = transform.Find("Animator/Exchange/Text_Exchange").GetComponent<Text>();
            btnExchange = transform.Find("Animator/Exchange/Btn_01").GetComponent<Button>();
            btnExchange.onClick.AddListener(OnClickExchange);
            
            txtDamage = transform.Find("Animator/Reward/Item01/Text_Num").GetComponent<Text>();
            txtGetState = transform.Find("Animator/Reward/Item01/Text_Get").GetComponent<Text>();
            btnGet = transform.Find("Animator/Reward/Btn_01").GetComponent<Button>();
            btnGet.onClick.AddListener(OnClickGetReward);
            txtGot = transform.Find("Animator/Reward/Text_yilingqu").GetComponent<Text>();
            
            btnRank = transform.Find("Animator/Grid/Btn_Rank").GetComponent<Button>();
            btnRank.onClick.AddListener(OnClickRank);
            
            btnRward = transform.Find("Animator/Grid/Btn_Reward").GetComponent<Button>();
            btnRward.onClick.AddListener(OnClickRward);
        }

        protected override void OnOpen(object arg)
        {

        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            Sys_Activity_Nien.Instance.eventEmitter.Handle(Sys_Activity_Nien.EEvents.OnExchangeNtf, OnExchangeNtf, toRegister);
            Sys_Activity_Nien.Instance.eventEmitter.Handle(Sys_Activity_Nien.EEvents.OnDailyResetNtf, OnDailyResetNtf, toRegister);
            // Sys_MallActivity.Instance.eventEmitter.Handle(Sys_MallActivity.EEvents.OnRefreshShopData, OnRefreshShopData, toRegister);
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

        private void OnClickClose()
        {
            this.CloseSelf();
        }

        private void OnClickAddCurrency()
        {
            Debug.LogError("currency");
        }

        private void OnClickChallenge()
        {
            
        }

        private void OnClickExchange()
        {
            if (Sys_Activity_Nien.Instance.LeftExchangeTimes > 0)
            {
                long srcItemCount = Sys_Bag.Instance.GetItemCount(srcItemId);
                if (srcItemCount > 0)
                {
                    Sys_Activity_Nien.Instance.OnExchangeReq();
                }
                else
                {
                    DebugUtil.LogError("剩余年糕不足");
                }
            }
            else
            {
                DebugUtil.LogError("剩余次数不足");
            }
        }

        private void OnClickGetReward()
        {
            uint curSvrTime = Sys_Time.Instance.GetServerTime();
            if (curSvrTime < secondStageStartTime) //第一阶段
            {
                uint leftTime = secondStageStartTime - curSvrTime;
                LanguageHelper.TimeToString(leftTime, LanguageHelper.TimeFormat.Type_4);
                Debug.LogError("xxxx后可挑战年兽");
            }
            else if (curSvrTime < secondStageEndTime) //挑战阶段
            {
                Sys_Activity_Nien.Instance.OnChallengeReq();
            }
            else
            {
                txtCallNienDate.text = LanguageHelper.GetTextContent(2016115);
            }
        }

        private void OnClickRank()
        {
            UIManager.OpenUI(EUIID.UI_Activity_Nien_Rank);
        }

        private void OnClickRward()
        {
            UIManager.OpenUI(EUIID.UI_Activity_Nien_ChallengeAward);
        }

        private void UpdateInfo()
        {
            uint temp = 0;
            uint.TryParse(CSVNienParameters.Instance.GetConfData(1).str_value, out temp);
            secondStageStartTime = TimeManager.ConvertFromZeroTimeZone(temp);
            temp = 0;
            uint.TryParse(CSVNienParameters.Instance.GetConfData(2).str_value, out temp);
            secondStageEndTime = TimeManager.ConvertFromZeroTimeZone(temp);
            
            //----兑换
            uint.TryParse(CSVNienParameters.Instance.GetConfData(12).str_value, out exchangeRate);
            
            //年糕
            srcItemId = 0;
            uint.TryParse(CSVNienParameters.Instance.GetConfData(13).str_value, out srcItemId);
            PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(srcItemId, 1, true, false, false, false, false, false, false, false);
            srcItem.SetData(itemData, EUIID.UI_Activity_Nien_Challenge);
            
            //爆竹
            desItemId = 0;
            uint.TryParse(CSVNienParameters.Instance.GetConfData(14).str_value, out desItemId);
            itemData = new PropIconLoader.ShowItemData(desItemId, exchangeRate, true, false, false, false, false, false, false, false);
            desItem.SetData(itemData, EUIID.UI_Activity_Nien_Challenge);
            
            //刷新兑换信息
            OnRefreshExchange();

            //刷新时间提示信息
            OnRefreshTimeTip();
            
            //刷新挑战进度
            OnRefreshChallegeStage();
        }

        private void OnRefreshExchange()
        {
            //刷新兑换信息
            txtExchangeTime.text = LanguageHelper.GetTextContent(2016103, Sys_Activity_Nien.Instance.LeftExchangeTimes.ToString());
        }

        private void OnRefreshTimeTip()
        {
            uint curSvrTime = Sys_Time.Instance.GetServerTime();
            if (curSvrTime < secondStageStartTime) //第一阶段
            {
                uint leftTime = secondStageStartTime - curSvrTime;
                txtCallNienDate.text = LanguageHelper.GetTextContent(2016105,
                    LanguageHelper.TimeToString(leftTime, LanguageHelper.TimeFormat.Type_4));
            }
            else if (curSvrTime < secondStageEndTime) //挑战阶段
            {
                uint leftTime = secondStageEndTime - curSvrTime;
                txtCallNienDate.text = LanguageHelper.GetTextContent(2016116,
                    LanguageHelper.TimeToString(leftTime, LanguageHelper.TimeFormat.Type_4));
            }
            else
            {
                txtCallNienDate.text = LanguageHelper.GetTextContent(2016115);
            }
        }

        private void OnRefreshChallegeStage()
        {
            txtChallengeCount.text =
                LanguageHelper.GetTextContent(2016106, Sys_Activity_Nien.Instance.FightStateTimes.ToString());
        }

        private void OnExchangeNtf()
        {
            OnRefreshExchange();
        }

        private void OnDailyResetNtf()
        {
            OnRefreshExchange();

            OnRefreshChallegeStage();
        }
    }
}


