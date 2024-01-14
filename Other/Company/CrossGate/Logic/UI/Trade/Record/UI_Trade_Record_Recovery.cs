using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_Trade_Record_Recovery : UIBase
    {
        private Button m_BtnClose;

        private Text _textTip1;
        private Text _textTip2;
        private Text _textTotalPrice;

        private Text _textPrice;
        private Text _textTimeTip;
        private Button _btn;

        private TradeSaleRecord _saleRecord;

        protected override void OnOpen(object arg)
        {
            _saleRecord = null;
            if (arg != null)
                _saleRecord = (TradeSaleRecord)arg;
        }

        protected override void OnLoaded()
        {
            m_BtnClose = transform.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>();
            m_BtnClose.onClick.AddListener(() =>{ this.CloseSelf(); });

            _textTip1 = transform.Find("Animator/Image_Left/Text_Dex").GetComponent<Text>();
            _textTip2 = transform.Find("Animator/Image_Left/Text_Dex2").GetComponent<Text>();
            _textTotalPrice = transform.Find("Animator/Image_Left/Image_Title2/Text_Num").GetComponent<Text>();

            _textPrice = transform.Find("Animator/Image_Right/Image_Title2/Text_Num").GetComponent<Text>();
            _textTimeTip = transform.Find("Animator/Image_Right/Text/Text_Time").GetComponent<Text>();
            _btn = transform.Find("Animator/Image_Right/Btn_01").GetComponent<Button>();
            _btn.onClick.AddListener(OnClickRecovery);
        }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        //protected override void ProcessEvents(bool toRegister)
        //{
        //    //Sys_Trade.Instance.eventEmitter.Handle<uint>(Sys_Trade.EEvents.OnRecordType, OnRecordType, toRegister);
        //}

        private void OnClickRecovery()
        {
            Sys_Trade.Instance.OnGetSaleCoinReq(_saleRecord.DealId);
            this.CloseSelf();
        }

        private void UpdateInfo()
        {
            _textTip1.text = LanguageHelper.GetTextContent(2011125);

            //total price
            _textTotalPrice.text = _saleRecord.RealCoin.ToString();

            //第一阶段可取回的金额
            uint firstStageCoin = ((uint)(_saleRecord.RealCoin * _saleRecord.FirstPer / 100f));

            //tip2
            string firstTimeTip = LanguageHelper.TimeToString(_saleRecord.FirstDay * 86400, LanguageHelper.TimeFormat.Type_4);
            string canGetCoinTip = firstStageCoin.ToString();
            string totalTimeTip = LanguageHelper.TimeToString(_saleRecord.TotalDay * 86400, LanguageHelper.TimeFormat.Type_4);
            _textTip2.text = LanguageHelper.GetTextContent(2011126, firstTimeTip, canGetCoinTip, totalTimeTip);

            //右边可领取信息------
            _textTimeTip.transform.parent.gameObject.SetActive(false);
            _btn.gameObject.SetActive(false);

            //第一阶段剩余时间
            uint leftFirstTime = 0u;
            uint firstUnlockTime = _saleRecord.CheckTime + _saleRecord.FirstDay * 86400;
            if (firstUnlockTime > Sys_Time.Instance.GetServerTime())
                leftFirstTime = firstUnlockTime - Sys_Time.Instance.GetServerTime();

            //第二阶段剩余时间
            uint leftTotalTime = 0u;
            uint totalUnlockTime = _saleRecord.CheckTime + _saleRecord.TotalDay * 86400;
            if (totalUnlockTime > Sys_Time.Instance.GetServerTime())
                leftTotalTime = totalUnlockTime - Sys_Time.Instance.GetServerTime();

            if (leftFirstTime > 0u) //第一时间段内
            {
                //第一时间段内，不可领取，只显示可领取剩余时间
                _textTimeTip.transform.parent.gameObject.SetActive(true);
                _textTimeTip.text = LanguageHelper.TimeToString(leftFirstTime, LanguageHelper.TimeFormat.Type_4);

                //显示第一阶段可取回的金额
                _textPrice.text = firstStageCoin.ToString();
            }
            else if (leftTotalTime > 0u) //第二时间段内
            {
                if (_saleRecord.ReceiveStep == 0u) //未领取
                {
                    _btn.gameObject.SetActive(true);
                    //显示第一阶段可取回的金额
                    _textPrice.text = firstStageCoin.ToString();
                }
                else if (_saleRecord.ReceiveStep == 1u) //第一阶段已领取
                {
                    _textTimeTip.transform.parent.gameObject.SetActive(true);
                    _textTimeTip.text = LanguageHelper.TimeToString(leftTotalTime, LanguageHelper.TimeFormat.Type_4);
                    //显示第二阶段可取回的金额
                    _textPrice.text = (_saleRecord.RealCoin - firstStageCoin).ToString();
                }
            }
            else //第三阶段
            {
                _btn.gameObject.SetActive(true);

                if (_saleRecord.ReceiveStep == 0u) //未领取
                {
                    //显示全部可领取的金额
                    _textPrice.text = _saleRecord.RealCoin.ToString();
                }
                else if (_saleRecord.ReceiveStep == 1u) //第一阶段已领取
                {
                    //显示第二阶段可取回的金额
                    _textPrice.text = (_saleRecord.RealCoin - firstStageCoin).ToString();
                }
            }
        }
    }
}


