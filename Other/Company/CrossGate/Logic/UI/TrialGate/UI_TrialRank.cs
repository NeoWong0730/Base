using System;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Table;
using UnityEngine.UI;
using Lib.Core;
using Packet;
using Framework;

namespace Logic
{
    //试炼排行界面
    public class UI_TrialRank : UIBase
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
        protected override void OnClose()
        {
            Close();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            OnProcessEventsForEnable(toRegister);
        }
        #endregion
        #region 组件
        Button btnClose;
        Transform parent;
        GameObject objEmpty;
        GameObject objUnEmpty;
        InfinityGrid infinityGrid;
        Transform objSelfRank;
        Image selfIcon;
        Text selfNumber, selfName, selfStage, selfRound, selfTime;
        #endregion
        #region 数据
        List<RankParentTypeItem> rankParentTypeItemList = new List<RankParentTypeItem>();
        static uint curLevelGradeId;
        uint rankGroupType;
        uint curSelectCharacteristicId;
        uint curRankType;
        uint curRankSubType;
        RankDataBase rankData;
        #endregion
        #region 组件查找、事件注册
        private void OnParseComponent()
        {
            btnClose = transform.Find("Animator/View_TipsBgNew01/Btn_Close").GetComponent<Button>();
            parent = transform.Find("Animator/View_Left/Scroll01/Content");
            objEmpty = transform.Find("Animator/View_Right/Empty").gameObject;
            objUnEmpty = transform.Find("Animator/View_Right/UnEmpty").gameObject;
            infinityGrid = transform.Find("Animator/View_Right/UnEmpty/Scroll_List").GetComponent<InfinityGrid>();
            objSelfRank = transform.Find("Animator/View_Right/UnEmpty/MyRank");
            selfIcon = objSelfRank.Find("BG").GetComponent<Image>();
            selfNumber = objSelfRank.Find("Text_Number").GetComponent<Text>();
            selfName = objSelfRank.Find("Text01").GetComponent<Text>();
            selfStage = objSelfRank.Find("Text02").GetComponent<Text>();
            selfRound = objSelfRank.Find("Text03").GetComponent<Text>();
            selfTime = objSelfRank.Find("Text04").GetComponent<Text>();

            btnClose.onClick.AddListener(() => { CloseSelf(); });

            infinityGrid.onCreateCell += OnCreateCell;
            infinityGrid.onCellChange += OnCellChange;
        }
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_Rank.Instance.eventEmitter.Handle<uint>(Sys_Rank.EEvents.GetRankRes, OnRefreshRank, toRegister);
        }
        private void OnRefreshRank(uint key)
        {
            rankData = Sys_Rank.Instance.GetRankDataBaseByKey(key);
            if (null != rankData)
            {
                objEmpty.SetActive(false);
                objUnEmpty.SetActive(true);
                RefreshRightShow();
            }
            else
            {
                objEmpty.SetActive(true);
                objUnEmpty.SetActive(false);
                DebugUtil.Log(ELogType.eNone, $"RankDataBase Dictionary not find key = {key} ");
            }
        }
        #endregion
        private void InitView()
        {
            curRankType = (uint)RankType.TrialGate;
            curLevelGradeId = Sys_ActivityTrialGate.Instance.GetCurLevelGradeId();
            rankGroupType = 0;
            RefreshLeftRankType();
        }
        #region left
        private void RefreshLeftRankType()
        {
            var trialLevelGradeDataList = CSVTrialLevelGrade.Instance.GetAll();
            for (int i = 0; i < rankParentTypeItemList.Count; i++)
            {
                RankParentTypeItem cell = rankParentTypeItemList[i];
                PoolManager.Recycle(cell);
            }
            rankParentTypeItemList.Clear();
            FrameworkTool.CreateChildList(parent, trialLevelGradeDataList.Count);
            for (int i = 0; i < trialLevelGradeDataList.Count; i++)
            {
                Transform trans = parent.GetChild(i);
                RankParentTypeItem cell = PoolManager.Fetch<RankParentTypeItem>();
                cell.Init(trans);
                cell.SetData(trialLevelGradeDataList[i], OnClickPar, OnClickSub);
                rankParentTypeItemList.Add(cell);
            }
            FrameworkTool.ForceRebuildLayout(transform.gameObject);
        }
        private void OnClickPar(uint id,bool isOn)
        {
            curLevelGradeId = id;
            for (int i = 0; i < rankParentTypeItemList.Count; i++)
            {
                RankParentTypeItem item = rankParentTypeItemList[i];
                if (item.levelGradeData.id == curLevelGradeId)
                    item.OnSelect(isOn);
            }
            FrameworkTool.ForceRebuildLayout(transform.gameObject);
        }
        private void OnClickSub(CSVTrialCharacteristic.Data subTypeData)
        {
            if (subTypeData != null)
            {
                curSelectCharacteristicId = subTypeData.id;
                curRankSubType = subTypeData.rank_sub;
                Sys_Rank.Instance.RankQueryReq(curRankType, curRankSubType, 0);
            }
        }
        private void Close ()
        {
            for (int i = 0; i < rankParentTypeItemList.Count; i++)
            {
                rankParentTypeItemList[i].HideAllSub();
            }
        }
        #endregion
        #region right
        private void RefreshRightShow()
        {
            int count= rankData.GetOtherDataCount();
            infinityGrid.CellCount = count;
            infinityGrid.ForceRefreshActiveCell();
            if (count >= 0)
                infinityGrid.MoveToIndex(0);
            bool isShow = count > 0;
            objEmpty.SetActive(!isShow);
            SetSelfRankInfo(isShow);
        }
        private void OnCreateCell(InfinityGridCell cell)
        {
            RankRightShowItem entry = new RankRightShowItem();
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
        }
        private void OnCellChange(InfinityGridCell cell, int index)
        {
            RankRightShowItem entry = cell.mUserData as RankRightShowItem;
            entry.SetData(rankData.GetRankDataByIndex(index), curRankType, curRankSubType);//索引数据
        }
        private void SetSelfRankInfo(bool isShow)
        {
            objSelfRank.gameObject.SetActive(isShow);
            RankUnitData selfData = rankData.GetSelfData();
            if (selfData != null)
            {
                uint ranuNum = selfData.Rank;
                if (ranuNum != 0)
                {
                    uint iconId = Sys_Rank.Instance.GetRankIcon((int)ranuNum);
                    if (iconId != 0)
                    {
                        selfIcon.gameObject.SetActive(true);
                        selfNumber.gameObject.SetActive(false);
                        ImageHelper.SetIcon(selfIcon, iconId);
                    }
                    else
                    {
                        selfIcon.gameObject.SetActive(false);
                        selfNumber.gameObject.SetActive(true);
                        selfNumber.text = ranuNum.ToString();
                    }
                    if (curRankType == (uint)RankType.TrialGate)
                    {
                        RankDataTrialGate rankDataTrialGate = selfData.TrialGateData;
                        if (rankDataTrialGate != null)
                        {
                            selfName.text = rankDataTrialGate.Name.ToStringUtf8();
                            selfStage.text = rankDataTrialGate.Stage.ToString();
                            selfRound.text = rankDataTrialGate.Round.ToString();
                            selfTime.text = TimeManager.GetDateTime(rankDataTrialGate.Time).ToString("yyyy/MM/dd HH:mm");
                        }
                    }
                }
                else
                {
                    selfIcon.gameObject.SetActive(false);
                    selfNumber.gameObject.SetActive(true);
                    selfNumber.text = LanguageHelper.GetTextContent(10168);//未上榜
                    selfName.text = Sys_Role.Instance.Role.Name.ToStringUtf8();
                    RoleTrialFeature roleTrialFeature = Sys_ActivityTrialGate.Instance.GetRoleTrialFeature(curSelectCharacteristicId);
                    if (roleTrialFeature != null)
                    {
                        selfStage.text = roleTrialFeature.maxStage.ToString();
                        selfRound.text = roleTrialFeature.minRound.ToString();
                        selfTime.text = TimeManager.GetDateTime(roleTrialFeature.timestamp).ToString("yyyy/MM/dd HH:mm");
                    }
                    else
                    {
                        selfStage.text = "0";
                        selfRound.text = "0";
                        selfTime.text = "----";
                    }
                }
            }
            else
            {
                selfIcon.gameObject.SetActive(false);
                selfNumber.gameObject.SetActive(true);
                selfNumber.text = LanguageHelper.GetTextContent(10168);//未上榜
                selfName.text = Sys_Role.Instance.Role.Name.ToStringUtf8();
                RoleTrialFeature roleTrialFeature = Sys_ActivityTrialGate.Instance.GetRoleTrialFeature(curSelectCharacteristicId);
                if (roleTrialFeature != null)
                {
                    selfStage.text = roleTrialFeature.maxStage.ToString();
                    selfRound.text = roleTrialFeature.minRound.ToString();
                    selfTime.text = TimeManager.GetDateTime(roleTrialFeature.timestamp).ToString("yyyy/MM/dd HH:mm");
                }
                else
                {
                    selfStage.text = "0";
                    selfRound.text = "0";
                    selfTime.text = "----";
                }
            }
        }
        #endregion
        #region Item
        public class RankParentTypeItem
        {
            Text textDark;
            Image arrowToggleGo;
            CP_Toggle parToggle;
            Transform subParent;

            List<RankSubTypeItem> rankSubTypeItemList = new List<RankSubTypeItem>();
            List<CSVTrialCharacteristic.Data> subDataList = new List<CSVTrialCharacteristic.Data>();
            public CSVTrialLevelGrade.Data levelGradeData;
            Action<uint,bool> parAction;
            Action<CSVTrialCharacteristic.Data> subAction;
            public void Init(Transform trans)
            {
                parToggle = trans.GetComponent<CP_Toggle>();
                textDark = trans.Find("GameObject/Text_Dark").GetComponent<Text>();
                arrowToggleGo = trans.Find("GameObject/Image_Frame").GetComponent<Image>();
                subParent = trans.Find("Content_Small");

                parToggle.onValueChanged.AddListener(OnToggleClick);
            }
            public void SetData(CSVTrialLevelGrade.Data levelGradeData,Action<uint,bool> parAction, Action<CSVTrialCharacteristic.Data> subAction)
            {
                this.levelGradeData = levelGradeData;
                this.parAction = parAction;
                this.subAction = subAction;
                textDark.text = LanguageHelper.GetTextContent(550000001u + levelGradeData.id);
                SetSubData();
                SetParState();
            }
            private void SetSubData()
            {
                subDataList= Sys_ActivityTrialGate.Instance.GetTrialCharacteristicList(levelGradeData.id);
                for (int i = 0; i < rankSubTypeItemList.Count; i++)
                {
                    RankSubTypeItem cell = rankSubTypeItemList[i];
                    PoolManager.Recycle(cell);
                }
                rankSubTypeItemList.Clear();
                FrameworkTool.CreateChildList(subParent, subDataList.Count);
                for (int i = 0; i < subDataList.Count; i++)
                {
                    Transform trans = subParent.GetChild(i);
                    RankSubTypeItem cell = PoolManager.Fetch<RankSubTypeItem>();
                    cell.Init(trans);
                    cell.SetData(subDataList[i], SubOnClick);
                    rankSubTypeItemList.Add(cell);
                }
            }
            private void SubOnClick(CSVTrialCharacteristic.Data data)
            {
                if (null != data)
                {
                    subAction?.Invoke(data);
                }
            }
            private void OnToggleClick(bool isOn)
            {
                if (levelGradeData != null)
                {
                    parAction?.Invoke(levelGradeData.id, isOn);
                }
            }
            public void SetParState()
            {
                if (curLevelGradeId == levelGradeData.id)
                {
                    parToggle.SetSelected(true, false);
                    subParent.gameObject.SetActive(true);
                    rankSubTypeItemList[0].OpenTrigger();
                }
                else
                    subParent.gameObject.SetActive(false);
                SetArrow(curLevelGradeId == levelGradeData.id && subParent.gameObject.activeSelf); 
            }
            public void OnSelect(bool isOn)
            {
                if (isOn)
                {
                    subParent.gameObject.SetActive(true);
                    rankSubTypeItemList[0].OpenTrigger();
                }
                else
                {
                    subParent.gameObject.SetActive(false);
                }
                SetArrow(subParent.gameObject.activeSelf);
            }
            private void SetArrow(bool select)
            {
                float rotateZ = select ? 0f : 90f;
                arrowToggleGo.rectTransform.localRotation = Quaternion.Euler(0f, 0f, rotateZ);
            }
            public void HideAllSub()
            {
                for (int i = 0; i < rankSubTypeItemList.Count; i++)
                {
                    rankSubTypeItemList[i].OnHide();
                }
            }
        }
        public class RankSubTypeItem
        {
            CP_Toggle subToggle;
            Text textDark;
            Text textLight;
            GameObject objLabel;

            public CSVTrialCharacteristic.Data typeSubData;
            Action<CSVTrialCharacteristic.Data> action;
            public void Init(Transform trans)
            {
                subToggle = trans.GetComponent<CP_Toggle>();
                textDark = trans.Find("Text_Dark").GetComponent<Text>();
                textLight = trans.Find("Image_Select/Text_Light").GetComponent<Text>();
                objLabel = trans.Find("Image_Label").gameObject;

                subToggle?.onValueChanged.AddListener(OnToggleClick);
            }
            public void SetData(CSVTrialCharacteristic.Data typeSubData ,Action<CSVTrialCharacteristic.Data> action)
            {
                this.typeSubData = typeSubData;
                this.action = action;
                textLight.text = LanguageHelper.GetTextContent(typeSubData.characteristic1_name);
                textDark.text = LanguageHelper.GetTextContent(typeSubData.characteristic1_name);
                objLabel.SetActive(Sys_ActivityTrialGate.Instance.curTrialFeatureTid == typeSubData.id);//本周特性
                if (null != subToggle && typeSubData.id == Sys_Rank.Instance.InitTypeSub)
                {
                    subToggle.SetSelected(true, true);
                }
            }
            private void OnToggleClick(bool isOn)
            {
                if (isOn)
                {
                    OpenTrigger();
                }
            }
            public void OpenTrigger()
            {
                subToggle.SetSelected(true, false);
                action?.Invoke(typeSubData);
            }
            public void OnHide()
            {
                subToggle.SetSelected(false, false);
            }
        }
        public class RankRightShowItem
        {
            Image icon;
            Text number, name, stage, round, time;
            Button btnDetail;

            RankUnitData rankUnitData;
            RankDataTrialGate rankDataTrialGate;
            uint curRankType;
            uint curRankSubType;
            public void Init(Transform trans)
            {
                icon = trans.Find("Image_Rank").GetComponent<Image>();
                number = trans.Find("Text_Number").GetComponent<Text>();
                name = trans.Find("Text01").GetComponent<Text>();
                stage = trans.Find("Text02").GetComponent<Text>();
                round = trans.Find("Text03").GetComponent<Text>();
                time = trans.Find("Text04").GetComponent<Text>();
                btnDetail = trans.Find("Btn_Detail").GetComponent<Button>();

                btnDetail.onClick.AddListener(DetailOnClick);
            }
            public void SetData(RankUnitData rankUnitData, uint curRankType, uint curRankSubType)
            {
                this.curRankType = curRankType;
                this.curRankSubType = curRankSubType;
                if (rankUnitData!=null)
                {
                    this.rankUnitData = rankUnitData;
                    uint ranuNum = rankUnitData.Rank;
                    if (ranuNum != 0)
                    {
                        uint iconId = Sys_Rank.Instance.GetRankIcon((int)ranuNum);
                        if (iconId != 0)
                        {
                            icon.gameObject.SetActive(true);
                            number.gameObject.SetActive(false);
                            ImageHelper.SetIcon(icon, iconId);
                        }
                        else
                        {
                            icon.gameObject.SetActive(false);
                            number.gameObject.SetActive(true);
                            number.text = ranuNum.ToString();
                        }
                    }
                }
                if (curRankType == (uint)RankType.TrialGate)
                {
                    rankDataTrialGate = rankUnitData.TrialGateData;
                    btnDetail.gameObject.SetActive(rankDataTrialGate.RoleId != Sys_Role.Instance.RoleId);
                    if (rankDataTrialGate != null)
                    {
                        name.text = rankDataTrialGate.Name.ToStringUtf8();
                        stage.text = rankDataTrialGate.Stage.ToString();
                        round.text = rankDataTrialGate.Round.ToString();
                        time.text = TimeManager.GetDateTime(rankDataTrialGate.Time).ToString("yyyy/MM/dd HH:mm");
                    }
                }
            }
            private void DetailOnClick()
            {
                if (curRankType == (uint)RankType.TrialGate)
                {
                    if (null != rankDataTrialGate)
                    {
                        Sys_Rank.Instance.RankUnitDescReq(curRankType, curRankSubType, rankDataTrialGate.RoleId);
                    }
                }
            }
        }
        #endregion 
    }
}