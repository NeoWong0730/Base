using Logic.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Lib.Core;
using Framework;

namespace Logic
{
    public class UI_RedEnvelopeRain : UIBase
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
        protected override void OnShowEnd()
        {
            RefreshIndex();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            OnProcessEventsForEnable(toRegister);
        }
        protected override void OnLateUpdate(float dt, float usdt)
        {
            OnLateUpdate();
        }
        #endregion
        #region 组件
        Button btnClose;
        Text textTime;
        Button btnDetails;
        Text textTitle;
        InfinityGrid infinityGrid;
        Toggle toggle;
        Text text_Get;
        GameObject viewRecord;
        Button btnCloseRecord;
        InfinityIrregularGrid irregularGrid;
        Text txtCommon;
        Image backImage, foregImage;
        #endregion
        #region 数据
        bool isDirty;
        bool isOver;
        #endregion
        #region 组件查找、事件
        private void OnParseComponent()
        {
            backImage = transform.Find("Animator/View_Bg/Image_bg").GetComponent<Image>();
            foregImage = transform.Find("Animator/Image_bg1").GetComponent<Image>();
            btnClose = transform.Find("Animator/View_Bg/Btn_Close").GetComponent<Button>();
            textTime = transform.Find("Animator/View_Bg/Text_Time/Text_Value").GetComponent<Text>();
            btnDetails = transform.Find("Animator/View_Bg/Btn_Details").GetComponent<Button>();
            textTitle = transform.Find("Animator/View_Bg/Text_Title").GetComponent<Text>();
            infinityGrid = transform.Find("Animator/View_Bg/Scroll View").GetComponent<InfinityGrid>();
            toggle = transform.Find("Animator/View_Bg/Toggle").GetComponent<Toggle>();
            text_Get = transform.Find("Animator/View_Bg/Text_Get").GetComponent<Text>();

            viewRecord = transform.Find("Animator/View_Record").gameObject;
            btnCloseRecord = transform.Find("Animator/View_Record/Close").GetComponent<Button>();
            irregularGrid = transform.Find("Animator/View_Record/Scroll View").GetComponent<InfinityIrregularGrid>();
            txtCommon = transform.Find("Animator/View_Record/text_Common").GetComponent<Text>();

            btnClose.onClick.AddListener(OnClickClose);
            btnDetails.onClick.AddListener(RainRecordReq);
            btnCloseRecord.onClick.AddListener(() => { viewRecord.SetActive(false); });

            toggle.onValueChanged.AddListener(RefreshIsHide);

            infinityGrid.onCreateCell += OnCreateCell;
            infinityGrid.onCellChange += OnCellChange;

            irregularGrid.SetCapacity(99);
            irregularGrid.MinSize = 22;
            irregularGrid.onCreateCell += OnCreateCellRecord;
            irregularGrid.onCellChange += OnCellChangeRecord;
        }
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_RedEnvelopeRain.Instance.eventEmitter.Handle(Sys_RedEnvelopeRain.EEvents.OnRefreshActivityPreviewData, RefreshActivityPreviewData, toRegister);
            Sys_RedEnvelopeRain.Instance.eventEmitter.Handle(Sys_RedEnvelopeRain.EEvents.OnRefreshRainRecord, OnRefreshRainRecord, toRegister);      
        }
        private void RefreshActivityPreviewData()
        {
            RefreshRecords();
            RefreshIndex();
            RefreshTitle();
            RefreshGet();
        }
        private void OnRefreshRainRecord()
        {
            RefreshRecord();
        }
        #endregion
        #region 初始化
        private void InitView()
        {
            isDirty = false;
            isOver = true;
            RefreshRecords();
            SetTextData();
            RefreshTitle();
            RefreshGet();
            toggle.isOn = Sys_RedEnvelopeRain.Instance.isHide;
            SetThemeImage();
        }
        #endregion

        #region function
        /// <summary>
        /// 设置主题
        /// </summary>
        private void SetThemeImage()
        {
            if (Sys_RedEnvelopeRain.Instance.curCSVActivityRulerData != null)
            {
                ImageHelper.SetIcon(backImage, null, Sys_RedEnvelopeRain.Instance.curCSVActivityRulerData.Back_Image, false);
                ImageHelper.SetIcon(foregImage, null, Sys_RedEnvelopeRain.Instance.curCSVActivityRulerData.Foreg_Image, false);
            }
        }
        private void OnClickClose()
        {
            UIManager.HitButton(EUIID.UI_RedEnvelopeRain, "OnClickClose");
            CloseSelf();
        }
        private void RefreshIsHide(bool isHide)
        {
            UIManager.HitButton(EUIID.UI_RedEnvelopeRain, "RedEnvelopeRainIsShield：" + isHide);
            Sys_RedEnvelopeRain.Instance.OnSetHideReq(isHide);
        }
        private void RainRecordReq()
        {
            viewRecord.SetActive(true);
            Sys_RedEnvelopeRain.Instance.OnGetRecordReq();
        }
        private void RefreshRecord()
        {
            isDirty = true;
            irregularGrid.ForceRefreshActiveCell();
        }
        private void RefreshRecords()
        {
            infinityGrid.CellCount = Sys_RedEnvelopeRain.Instance.activityPreviewDataList.Count;
            infinityGrid.ForceRefreshActiveCell();
        }
        private void RefreshIndex()
        {
            int num = Sys_RedEnvelopeRain.Instance.curRainNum;
            int index = (num == -1 || num == -2) ? 0 : num - 1;
            infinityGrid.MoveToIndex(index);
        }
        private void OnCreateCell(InfinityGridCell cell)
        {
            RainPreviewCell entry = new RainPreviewCell();
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
        }
        private void OnCellChange(InfinityGridCell cell, int index)
        {
            RainPreviewCell entry = cell.mUserData as RainPreviewCell;
            entry.SetData(Sys_RedEnvelopeRain.Instance.activityPreviewDataList[index]);//索引数据
        }
        private void OnCreateCellRecord(InfinityGridCell cell)
        {
            RainRecordCell entry = new RainRecordCell();
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
        }
        private void OnCellChangeRecord(InfinityGridCell cell, int index)
        {
            RainRecordCell entry = cell.mUserData as RainRecordCell;
            entry.SetData(Sys_RedEnvelopeRain.Instance.redEnvelopeRecordList[index]);//索引数据
        }

        private void SetTextData()
        {
            textTime.text = string.Format("{0}-{1}", Sys_RedEnvelopeRain.Instance.activityStartTime, Sys_RedEnvelopeRain.Instance.activityEndTime);//活动时间
        }
        private void RefreshTitle()
        {
            string str;
            int num = Sys_RedEnvelopeRain.Instance.curRainNum;
            if (Sys_RedEnvelopeRain.Instance.CheckCurRainIsUnderway())
            {
                str = LanguageHelper.GetTextContent(1001903, num.ToString(), RefreshRainSustainTime());
            }
            else
            {
                bool isOver = Sys_RedEnvelopeRain.Instance.CheckCurDayActivityIsOver();
                if (isOver)
                {
                    str = LanguageHelper.GetTextContent(1001905);
                }
                else
                {
                    str = LanguageHelper.GetTextContent(1001904, RefreshResidueTime());
                }
            }
            textTitle.text = str;//当前红包雨轮数描述
        }
        private string RefreshResidueTime()
        {
            int index = Sys_RedEnvelopeRain.Instance.GetNextIndex();
            int second = Sys_RedEnvelopeRain.Instance.GetResidueSecond(true, index);
            if (second <= 0)
            {
                second = 0;
                isOver = false;
            }
            LanguageHelper.TimeFormat type = LanguageHelper.TimeFormat.Type_1;
            if (second > 86400)
                type = LanguageHelper.TimeFormat.Type_9;
            string strTime = LanguageHelper.TimeToString((uint)(second), type);
            return strTime;
        }
        private void UpdateResidueTime()
        {
            textTitle.text = LanguageHelper.GetTextContent(1001904, RefreshResidueTime());
        }
        private void UpdateRainSustainTime()
        {
            int num = Sys_RedEnvelopeRain.Instance.curRainNum;
            textTitle.text = LanguageHelper.GetTextContent(1001903, num.ToString(), RefreshRainSustainTime());
        }
        private string RefreshRainSustainTime()
        {
            int second = Sys_RedEnvelopeRain.Instance.GetResidueSecond(false);
            string strTime = LanguageHelper.TimeToString((uint)(second), LanguageHelper.TimeFormat.Type_1);
            return strTime;
        }
        private void RefreshGet()
        {
            //本轮红包雨领取情况
            if (Sys_RedEnvelopeRain.Instance.CheckCurRainIsUnderway())
            {
                text_Get.gameObject.SetActive(true);
                int index = Sys_RedEnvelopeRain.Instance.GetCurIndexByCurRainNum();
                uint awardCount = Sys_RedEnvelopeRain.Instance.GetCurAwardByIndex(index);
                uint max = Sys_RedEnvelopeRain.Instance.curCSVRainData.Limit_Max[index];
                text_Get.text = LanguageHelper.GetTextContent(1001911, awardCount.ToString(), max.ToString());
            }
            else
                text_Get.gameObject.SetActive(false);
        }
        private void OnLateUpdate()
        {
            if (isDirty)
            {
                int count = Sys_RedEnvelopeRain.Instance.redEnvelopeRecordList.Count;
                int i = 0;
                while (i < 99 && irregularGrid.CellCount < count)
                {
                    ++i;
                    irregularGrid.Add(CalculateSize_Normal(Sys_RedEnvelopeRain.Instance.redEnvelopeRecordList[irregularGrid.CellCount]));
                }
                if (irregularGrid.CellCount >= count)
                {
                    isDirty = false;
                }
            }
            if (Sys_RedEnvelopeRain.Instance.CheckRainActivityIsAlive() && !Sys_RedEnvelopeRain.Instance.CheckCurRainIsUnderway() && Sys_RedEnvelopeRain.Instance.curRainNum != -2 && isOver)
            {
                UpdateResidueTime();
            }
            if (Sys_RedEnvelopeRain.Instance.CheckCurRainIsUnderway())
            {
                UpdateRainSustainTime();
            }
        }
        private int gContentMinHeight = 22;
        private int gSpace = 10;
        private int CalculateSize_Normal(RedEnvelopeGetRecordData data)
        {
            int h = 0;
            if (!string.IsNullOrWhiteSpace(data.content))
            {
                h += gSpace;
                txtCommon.text = data.content;
                h += Mathf.Max((int)txtCommon.preferredHeight, gContentMinHeight);
            }
            return h;
        }
        #endregion
    }
    public class RainPreviewCell
    {
        Transform trans;
        Text textRound;
        Text textTime;
        Text textState;
        ActivityPreviewData data;
        public void Init(Transform trans)
        {
            this.trans = trans;
            textRound = trans.Find("Text_Round").GetComponent<Text>();
            textTime = trans.Find("Text_Time").GetComponent<Text>();
            textState = trans.Find("Text_State").GetComponent<Text>();
        }
        public void SetData(ActivityPreviewData data)
        {
            this.data = data;
            textRound.text = LanguageHelper.GetTextContent(1001906, data.index.ToString());
            DateTime serverTime = Sys_RedEnvelopeRain.Instance.GetServerDateTime();
            int curDay = Sys_RedEnvelopeRain.Instance.GetCurActivityDay();
            DateTime startTime = new DateTime(serverTime.Year, serverTime.Month, curDay, (int)data.startTime[0], (int)data.startTime[1], 0);
            DateTime endTime = startTime.Subtract(TimeSpan.FromSeconds(-data.durationTime));
            DateTime startLocalTime = startTime.Subtract(TimeSpan.FromSeconds(TimeManager.TimeZoneOffset)).ToLocalTime();
            DateTime endLocalTime = endTime.Subtract(TimeSpan.FromSeconds(TimeManager.TimeZoneOffset)).ToLocalTime();
            textTime.text = string.Format("{0}:{1}-{2}:{3}", startLocalTime.Hour.ToString("D2"), startLocalTime.Minute.ToString("D2"), endLocalTime.Hour.ToString("D2"), endLocalTime.Minute.ToString("D2"));
            uint lanId = data.state == EActivityState.Finished ? 1001909u : data.state == EActivityState.Underway ? 1001908u : 1001907u;
            textState.text = LanguageHelper.GetTextContent(lanId);
        }
    }
    public class RainRecordCell
    {
        Text textRecord;
        public void Init(Transform trans)
        {
            textRecord = trans.GetComponent<Text>();
        }
        public void SetData(RedEnvelopeGetRecordData data)
        {
            textRecord.text = data.content;
        }
    }

}