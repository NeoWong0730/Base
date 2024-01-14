using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;
using System;
using Lib.Core;
using Framework;

namespace Logic
{
    /// <summary> 鼠王存钱活动 </summary>
    public class UI_ActivitySavingBank : UIBase
    {
        #region 系统函数
        protected override void OnLoaded()
        {
            OnParseComponent();
        }
        protected override void OnShow()
        {
            InitView();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            OnProcessEventsForEnable(toRegister);
        }
        protected override void OnDestroy()
        {

        }
        #endregion
        #region 数据
        DateTime saveEndTime;
        List<Sys_ActivitySavingBank.SavingsBankData> savingBankDataList;
        bool isSaveTerm;
        #endregion
        #region 组件、事件注册
        Button closeBtn,getBtn,saveBtn;
        GameObject bubbleObj_1, bubbleObj_2;
        GameObject getObj;
        Text bubbleTips, timeTips;
        GameObject getRedPoint, saveRedPoint;
        GameObject fx_ui_quan_mandang;
        List<Transform> viewTops = new List<Transform>();
        List<Transform> saveTokens = new List<Transform>();
        List<Transform> saveTokenProgress = new List<Transform>();
        Transform viewFull;
        private void OnParseComponent()
        {
            closeBtn = transform.Find("Animator/View_TipsBgNew01/Btn_Close").GetComponent<Button>();
            getBtn = transform.Find("Animator/Image_PiggyBank/Btn_PiggyBank").GetComponent<Button>();
            getRedPoint = getBtn.transform.Find("Dot").gameObject;
            getObj = transform.Find("Animator/Image_PiggyBank/Image_Get").gameObject;
            fx_ui_quan_mandang = getBtn.transform.Find("Fx_ui_quan_mandang").gameObject;
            saveBtn = transform.Find("Animator/Btn_01").GetComponent<Button>();
            saveRedPoint = saveBtn.transform.Find("Dot").gameObject;
            timeTips = transform.Find("Animator/Image_Time/Text").GetComponent<Text>();
            bubbleObj_1 = transform.Find("Animator/Image_Get1").gameObject;
            bubbleObj_2 = transform.Find("Animator/Image_Get2").gameObject;
            bubbleTips = bubbleObj_1.transform.Find("Text2").GetComponent<Text>();
            viewFull = transform.Find("Animator/View_Full");

            Transform viewTop1 = transform.Find("Animator/View_Top/Text1");
            Transform viewTop2 = transform.Find("Animator/View_Top/Text2");
            Transform viewTop3 = transform.Find("Animator/View_Top/Text3");
            viewTops.Add(viewTop1);
            viewTops.Add(viewTop2);
            viewTops.Add(viewTop3);

            Transform viewPop1 = transform.Find("Animator/View_Pop/Image1");
            Transform viewPop2 = transform.Find("Animator/View_Pop/Image2");
            Transform viewPop3 = transform.Find("Animator/View_Pop/Image3");
            saveTokens.Add(viewPop1);
            saveTokens.Add(viewPop2);
            saveTokens.Add(viewPop3);

            Transform savingPot01 = transform.Find("Animator/View_Bottom/SavingPot01");
            Transform savingPot02 = transform.Find("Animator/View_Bottom/SavingPot02");
            Transform savingPot03 = transform.Find("Animator/View_Bottom/SavingPot03");
            saveTokenProgress.Add(savingPot01);
            saveTokenProgress.Add(savingPot02);
            saveTokenProgress.Add(savingPot03);

            closeBtn.onClick.AddListener(()=> { CloseSelf(); });
            getBtn.onClick.AddListener(()=> { Sys_ActivitySavingBank.Instance.OnActivityPiggyBankRewardGetRes(); });
            saveBtn.onClick.AddListener(()=> {
                if (!isSaveTerm)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2026010));
                    return ;
                }
                Sys_ActivitySavingBank.Instance.OnActivityPiggyBankSaveReq(); 
            });
        }
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_ActivitySavingBank.Instance.eventEmitter.Handle(Sys_ActivitySavingBank.EEvents.OnRefreshSavingsBankState, OnRefreshSavingsBankState, toRegister);
            Sys_ActivitySavingBank.Instance.eventEmitter.Handle(Sys_ActivitySavingBank.EEvents.OnRefreshSavingsBankData, OnRefreshSavingsBankData, toRegister);
        }
        private void OnRefreshSavingsBankState()
        {
            isSaveTerm = Sys_ActivitySavingBank.Instance.CheckIsSaveTerm();
            SetBankState();
            RefreshRedPoint();
        }
        private void OnRefreshSavingsBankData()
        {
            isSaveTerm = Sys_ActivitySavingBank.Instance.CheckIsSaveTerm();
            SetData();
            RefreshRedPoint();
        }
        #endregion

        private void InitView()
        {
            Sys_ActivitySavingBank.Instance.OnActivityPiggyBankDataReq();
            isSaveTerm = Sys_ActivitySavingBank.Instance.CheckIsSaveTerm();
            SetDefaultData();
            SetData();
            SetBankState();
            RefreshRedPoint();
        }
        private void SetDefaultData()
        {
            DateTime startTime = TimeManager.GetDateTime(Sys_ActivitySavingBank.Instance.startTime);
            saveEndTime = TimeManager.GetDateTime(Sys_ActivitySavingBank.Instance.saveEndTime);
            DateTime saveEndTime_1 = TimeManager.GetDateTime(Sys_ActivitySavingBank.Instance.saveEndTime - 60);
            DateTime allEndTime = TimeManager.GetDateTime(Sys_ActivitySavingBank.Instance.allEndTime - 60);
            bubbleTips.text = LanguageHelper.GetTextContent(2026003, saveEndTime.ToString("MM"), saveEndTime.ToString("dd"));
            //timeTips.text = LanguageHelper.GetTextContent(2026004, startTime.ToString("yyyy/MM/dd"), saveEndTime_1.ToString("yyyy/MM/dd HH:mm"), saveEndTime.ToString("yyyy/MM/dd"), allEndTime.ToString("yyyy/MM/dd HH:mm"));
            timeTips.text = LanguageHelper.GetTextContent(2026004, saveEndTime_1.ToString("yyyy/MM/dd HH:mm"), saveEndTime.ToString("yyyy/MM/dd"));
        }
        private void SetData()
        {
            RefreshGetState();
            savingBankDataList = Sys_ActivitySavingBank.Instance.curSavingsBankDataList;
            for (int i = 0; i < savingBankDataList.Count; i++)
            {
                viewTops[i].GetComponent<Text>().text= LanguageHelper.GetTextContent(2026002, savingBankDataList[i].expenseRatio.ToString());
                ImageHelper.SetIcon(viewTops[i].Find("Icon1").GetComponent<Image>(), CSVItem.Instance.GetConfData(savingBankDataList[i].itemId).small_icon_id) ;
                viewTops[i].Find("Text").GetComponent<Text>().text= LanguageHelper.GetTextContent(2026019, "1");

                saveTokens[i].Find("Num").GetComponent<Text>().text = savingBankDataList[i].saveCount.ToString();
                ImageHelper.SetIcon(saveTokens[i].Find("Cost_Coin").GetComponent<Image>(), CSVItem.Instance.GetConfData(savingBankDataList[i].itemId).small_icon_id);
                saveTokens[i].Find("Cost_Coin/Text_Cost").GetComponent<Text>().text= savingBankDataList[i].GetRebateCount().ToString();

                Slider slider= saveTokenProgress[i].Find("Slider_Exp").GetComponent<Slider>();
                slider.minValue = 0;
                slider.maxValue = savingBankDataList[i].upperLimit;
                slider.value = savingBankDataList[i].saveCount;
                saveTokenProgress[i].Find("Text_Num").GetComponent<Text>().text= string.Format("{0}/{1}", savingBankDataList[i].saveCount, savingBankDataList[i].upperLimit);
                saveTokenProgress[i].Find("Text_Num1").GetComponent<Text>().text = savingBankDataList[i].haveCount.ToString();
            }
            SetSaveExtent();
        }
        /// <summary>
        /// 设置活动状态
        /// </summary>
        private void SetBankState()
        {
            RefreshGetState();
            saveBtn.GetComponent<Image>().color = isSaveTerm ? Color.white : new Color(0.5f,0.5f,0.5f,0.5f);
            bubbleObj_1.SetActive(isSaveTerm);
            bubbleObj_2.SetActive(!isSaveTerm);
        }
        private void RefreshGetState()
        {
            if (!isSaveTerm)
            {
                bool isGet = Sys_ActivitySavingBank.Instance.isGetRebate;
                fx_ui_quan_mandang.SetActive(Sys_ActivitySavingBank.Instance.GetTokenCount(1) > 0);
                getBtn.gameObject.SetActive(!isGet);
                getObj.SetActive(isGet);
            }
            else
                getBtn.gameObject.SetActive(false);
        }
        private void SetSaveExtent()
        {
            int index= Sys_ActivitySavingBank.Instance.GetTokenSaveExtent();
            int count = viewFull.childCount;
            for (int i = 0; i < count; i++)
            {
                if (i == index)
                    viewFull.GetChild(i).gameObject.SetActive(true);
                else
                    viewFull.GetChild(i).gameObject.SetActive(false);
            }
        }
        private void RefreshRedPoint()
        {
            bool isHave= Sys_ActivitySavingBank.Instance.CheckRedPoint();
            if (isSaveTerm)
            {
                saveRedPoint.SetActive(isHave);
                getRedPoint.SetActive(false);
            }
            else
            {
                saveRedPoint.SetActive(false);
                getRedPoint.SetActive(isHave);
            }
        }
    }
}