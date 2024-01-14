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
    //Boss资格挑战排行榜
    public class UI_BossTower_QualifierRank : UIBase
    {
        #region 系统函数
        protected override void OnLoaded()
        {
            OnParseComponent();
        }
        protected override void OnShow()
        {
            Sys_ActivityBossTower.Instance.bossTowerRankUnitDic.Clear();
            InitView();
        }
        protected override void OnClose()
        {
            Sys_ActivityBossTower.Instance.bossTowerRankUnitDic.Clear();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            OnProcessEventsForEnable(toRegister);
        }
        #endregion
        #region 组件
        Button btnClose;
        GameObject objEmpty;
        GameObject objUnEmpty;
        Transform parent;
        InfinityGrid infinityGrid;
        ScrollRect scrollRect;
        Transform objSelfRank;
        Image selfIcon;
        Text selfNumber, selfName, selfStage, selfRound, selfTime;
        Text textRule;
        GameObject objHaveQua, objNotQua;
        #endregion
        #region 数据
        List<uint> levelGradeList = new List<uint>();
        List<RankParentTypeItem> rankParentTypeItemList = new List<RankParentTypeItem>();
        static uint curLevelGradeId;
        uint curRankType;
        uint curRankSubType;//1资格赛 2boss战
        int curPageNum;
        List<BossTowerRankUnit> bossTowerRankUnitList;
        bool isQuerying;
        #endregion
        #region 组件查找、事件注册
        private void OnParseComponent()
        {
            btnClose = transform.Find("Animator/View_TipsBgNew01/Btn_Close").GetComponent<Button>();
            parent = transform.Find("Animator/View_Left/Scroll01/Content");
            objEmpty = transform.Find("Animator/View_Right/Empty").gameObject;
            objUnEmpty = transform.Find("Animator/View_Right/UnEmpty").gameObject;
            infinityGrid = transform.Find("Animator/View_Right/UnEmpty/Scroll_List").GetComponent<InfinityGrid>();
            scrollRect = transform.Find("Animator/View_Right/UnEmpty/Scroll_List").GetComponent<ScrollRect>();
            objSelfRank = transform.Find("Animator/View_Right/UnEmpty/MyRank");
            selfIcon = objSelfRank.Find("BG").GetComponent<Image>();
            selfNumber = objSelfRank.Find("Text_Number").GetComponent<Text>();
            selfName = objSelfRank.Find("Text01").GetComponent<Text>();
            selfStage = objSelfRank.Find("Text02").GetComponent<Text>();
            selfRound = objSelfRank.Find("Text03").GetComponent<Text>();
            selfTime = objSelfRank.Find("Text04").GetComponent<Text>();
            objHaveQua = objSelfRank.Find("Image_Pass").gameObject;
            objNotQua = objSelfRank.Find("Text_Failed").gameObject;
            textRule = transform.Find("Animator/View_Right/UnEmpty/Text").GetComponent<Text>();

            objEmpty.SetActive(false);

            btnClose.onClick.AddListener(() => { CloseSelf(); });

            infinityGrid.onCreateCell += OnCreateCell;
            infinityGrid.onCellChange += OnCellChange;
            EventTrigger.Get(scrollRect.gameObject).onDrag = OnDrag;
        }
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_ActivityBossTower.Instance.eventEmitter.Handle(Sys_ActivityBossTower.EEvents.OnRefreshSelfRankData, RefreshSelfRnak, toRegister);
            Sys_ActivityBossTower.Instance.eventEmitter.Handle<uint>(Sys_ActivityBossTower.EEvents.OnRefreshRankList, OnRefreshRankList, toRegister);
        }
        private void OnRefreshRankList(uint pageNum)
        {
            curPageNum = (int)pageNum;
            isQuerying = false;
            bossTowerRankUnitList = Sys_ActivityBossTower.Instance.GetAllRankList(false, curLevelGradeId);
            if (bossTowerRankUnitList != null)
            {
                objEmpty.SetActive(false);
                objUnEmpty.SetActive(true);
                RefreshRightShow();
            }
            else
            {
                objEmpty.SetActive(true);
                objUnEmpty.SetActive(false);
            }
        }
        private void RefreshSelfRnak()
        {
            if (bossTowerRankUnitList != null)
                RefreshRightShow();
        }
        private void OnDrag(GameObject go, Vector2 delta)
        {
            if (isQuerying) return;
            if (bossTowerRankUnitList == null) return;
            int index = bossTowerRankUnitList.Count / Sys_ActivityBossTower.Instance.OnePageDatasNum;//页数也是index
            var vertical = scrollRect.verticalNormalizedPosition;//当前所在比例。越接近0越底下
            if (vertical > 1)
            {
                vertical = 0.99f;
            }
            else if (vertical < 0)
            {
                vertical = 0.01f;
            }

            int currentPage = (int)Math.Floor(index * (1 - vertical)); //所在页数     

            var up = delta.y > 0;

            if (up)
            {
                if (currentPage >= Sys_ActivityBossTower.Instance.MaxPageNum - 1)
                {
                    return;
                }
                if (vertical <= 0.2 * (index - currentPage))
                {
                    if (curPageNum == currentPage + 1)
                    {
                        return;
                    }
                    isQuerying = true;
                    curPageNum = currentPage + 1;
                    Sys_ActivityBossTower.Instance.OnRankListReq(false, curLevelGradeId, (uint)curPageNum);
                }
            }
            else
            {
                if (currentPage <= 0)
                {
                    return;
                }

                if (vertical >= 0.2 * (index - currentPage))
                {
                    if (curPageNum == currentPage - 1)
                    {
                        return;
                    }
                    isQuerying = true;
                    curPageNum = currentPage - 1;
                    Sys_ActivityBossTower.Instance.OnRankListReq(false, curLevelGradeId, (uint)curPageNum);
                }
            }
        }
        #endregion
        private void InitView()
        {
            curPageNum = 0;
            isQuerying = true;
            curRankType = (uint)RankType.BossTower;
            curLevelGradeId = Sys_ActivityBossTower.Instance.GetCurLevelGradeId();
            curRankSubType = 1;
            objSelfRank.gameObject.SetActive(false);
            RefreshTextRule();
            RefreshLeftRankType();
            Sys_ActivityBossTower.Instance.OnBossTowerSelfRankReq();
        }
        private void RefreshTextRule()
        {
            uint allRankNum = Sys_ActivityBossTower.Instance.GetCurAllRankNum();
            uint diffNum = uint.Parse(Sys_ActivityBossTower.Instance.GetBossTowerParameter(2));
            textRule.text = LanguageHelper.GetTextContent(1009035, allRankNum.ToString(), diffNum.ToString());
        }
        #region left
        private void RefreshLeftRankType()
        {
            Dictionary<uint, uint[]> levelGradeDic = Sys_ActivityBossTower.Instance.levelGradeRangeDic;
            foreach (var item in levelGradeDic.Keys)
            {
                levelGradeList.Add(item);
            }
            for (int i = 0; i < rankParentTypeItemList.Count; i++)
            {
                RankParentTypeItem cell = rankParentTypeItemList[i];
                PoolManager.Recycle(cell);
            }
            rankParentTypeItemList.Clear();
            FrameworkTool.CreateChildList(parent, levelGradeList.Count);
            for (int i = 0; i < levelGradeList.Count; i++)
            {
                Transform trans = parent.GetChild(i);
                RankParentTypeItem cell = PoolManager.Fetch<RankParentTypeItem>();
                cell.Init(trans);
                cell.SetData(levelGradeList[i], OnClickPar, OnClickSub);
                rankParentTypeItemList.Add(cell);
            }
            FrameworkTool.ForceRebuildLayout(transform.gameObject);
        }
        private void OnClickPar(uint id, bool isOn)
        {
            for (int i = 0; i < rankParentTypeItemList.Count; i++)
            {
                RankParentTypeItem item = rankParentTypeItemList[i];
                if (item.levelGradeId == id)
                    item.OnSelect(isOn);
            }
            FrameworkTool.ForceRebuildLayout(transform.gameObject);
        }
        private void OnClickSub(CSVBOSSTower.Data subTypeData)
        {
            if (subTypeData != null)
            {
                curLevelGradeId = subTypeData.levelGrade_id;
                Sys_ActivityBossTower.Instance.OnRankListReq(false, curLevelGradeId, (uint)curPageNum);
            }
        }
        private void Close()
        {
            for (int i = 0; i < rankParentTypeItemList.Count; i++)
            {
                rankParentTypeItemList[i].HideAllSub();
            }
        }
        #endregion
        #region right
        List<BossTowerRankUnit> rankUnitDataList = new List<BossTowerRankUnit>();
        private void RefreshRightShow()
        {
            rankUnitDataList.Clear();
            int count = bossTowerRankUnitList.Count;
            QualifierRankData qualifierRankData = Sys_ActivityBossTower.Instance.GetQualifierRankData(curLevelGradeId);
            for (int i = 0; i < count; i++)
            {
                rankUnitDataList.Add(bossTowerRankUnitList[i]);
            }
            if (qualifierRankData != null)
            {
                uint curQualifierNum = qualifierRankData.qualifierNum;
                if (count > 0 && curQualifierNum < count)
                {
                    count += 1;
                    rankUnitDataList.Insert((int)curQualifierNum, null);
                }
            }
            infinityGrid.CellCount = count;
            infinityGrid.ForceRefreshActiveCell();
            bool isShow = count > 0;
            objEmpty.SetActive(!isShow);
            SetSelfRankInfo(isShow && curLevelGradeId == Sys_ActivityBossTower.Instance.GetCurLevelGradeId());
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
            entry.SetData(rankUnitDataList[index], curRankType, curRankSubType);
        }
        private void SetSelfRankInfo(bool isShow)
        {
            objSelfRank.gameObject.SetActive(isShow);
            BossTowerRankUnit selfRankData = Sys_ActivityBossTower.Instance.selfRankUnit;
            if (selfRankData != null)
            {
                uint ranuNum = selfRankData.Rank;
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
                    bool bHaveBossFightQualifier = Sys_ActivityBossTower.Instance.CheckIsHaveBossFightQualifier(ranuNum);
                    objHaveQua.SetActive(bHaveBossFightQualifier);
                    objNotQua.SetActive(!bHaveBossFightQualifier);
                    selfName.text = selfRankData.Name.ToStringUtf8();
                    selfStage.text = selfRankData.Layer.ToString();
                    selfRound.text = selfRankData.Round.ToString();
                    selfTime.text = TimeManager.GetDateTime(selfRankData.Time).ToString("yyyy/MM/dd");
                }
                else
                {
                    DefaultDataShow();
                }
            }
            else
            {
                DefaultDataShow();
            }
        }
        private void DefaultDataShow()
        {
            selfIcon.gameObject.SetActive(false);
            selfNumber.gameObject.SetActive(true);
            BossTowerQualifierData qualifierData = Sys_ActivityBossTower.Instance.GetBossTowerQualifierData(Sys_ActivityBossTower.Instance.curQualifierMaxLvlTid);
            uint stageNum = qualifierData != null ? qualifierData.csvData.floor_number : 0;
            selfNumber.text = LanguageHelper.GetTextContent(10168);//未上榜
            selfName.text = Sys_Role.Instance.Role.Name.ToStringUtf8();
            selfStage.text = stageNum.ToString();
            selfRound.text = Sys_ActivityBossTower.Instance.minPreRound.ToString();
            selfTime.text = Sys_ActivityBossTower.Instance.lastPreTime != 0 ? TimeManager.GetDateTime(Sys_ActivityBossTower.Instance.lastPreTime).ToString("yyyy/MM/dd") : "----";
            bool bHaveBossFightQualifier = Sys_ActivityBossTower.Instance.CheckIsHaveBossFightQualifier();
            objHaveQua.SetActive(bHaveBossFightQualifier);
            objNotQua.SetActive(!bHaveBossFightQualifier);
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
            List<CSVBOSSTower.Data> subDataList = new List<CSVBOSSTower.Data>();
            public uint levelGradeId;
            Action<uint, bool> parAction;
            Action<CSVBOSSTower.Data> subAction;
            public void Init(Transform trans)
            {
                parToggle = trans.GetComponent<CP_Toggle>();
                textDark = trans.Find("GameObject/Text_Dark").GetComponent<Text>();
                arrowToggleGo = trans.Find("GameObject/Image_Frame").GetComponent<Image>();
                subParent = trans.Find("Content_Small");

                parToggle.onValueChanged.AddListener(OnToggleClick);
            }
            public void SetData(uint levelGradeId, Action<uint, bool> parAction, Action<CSVBOSSTower.Data> subAction)
            {
                this.levelGradeId = levelGradeId;
                this.parAction = parAction;
                this.subAction = subAction;
                textDark.text = LanguageHelper.GetTextContent(550000001u + levelGradeId);
                SetSubData();
                SetParState();
            }
            private void SetSubData()
            {
                subDataList = Sys_ActivityBossTower.Instance.GetCurBossTowerFeatureList(levelGradeId);
                if (subDataList != null && subDataList.Count > 0)
                {
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
            }
            private void SubOnClick(CSVBOSSTower.Data data)
            {
                if (null != data)
                {
                    subAction?.Invoke(data);
                }
            }
            private void OnToggleClick(bool isOn)
            {
                parAction?.Invoke(levelGradeId, isOn);
            }
            public void SetParState()
            {
                if (curLevelGradeId == levelGradeId)
                {
                    parToggle.SetSelected(true, false);
                    subParent.gameObject.SetActive(true);
                    rankSubTypeItemList[0].OpenTrigger();
                }
                else
                    subParent.gameObject.SetActive(false);
                SetArrow(curLevelGradeId == levelGradeId && subParent.gameObject.activeSelf);
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

            public CSVBOSSTower.Data typeSubData;
            Action<CSVBOSSTower.Data> action;
            public void Init(Transform trans)
            {
                subToggle = trans.GetComponent<CP_Toggle>();
                textDark = trans.Find("Text_Dark").GetComponent<Text>();
                textLight = trans.Find("Image_Select/Text_Light").GetComponent<Text>();
                objLabel = trans.Find("Image_Label").gameObject;

                subToggle?.onValueChanged.AddListener(OnToggleClick);
            }
            public void SetData(CSVBOSSTower.Data typeSubData, Action<CSVBOSSTower.Data> action)
            {
                this.typeSubData = typeSubData;
                this.action = action;
                textLight.text = LanguageHelper.GetTextContent(typeSubData.name);
                textDark.text = LanguageHelper.GetTextContent(typeSubData.name);
                objLabel.SetActive(Sys_ActivityBossTower.Instance.curFeatureId == typeSubData.id);//本周特性
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
            Transform objRankItem;
            GameObject objPass;
            Image icon;
            Text number, name, stage, round, time;
            Button btnDetail;
            GameObject objHaveQua, objNotQua;

            uint curRankType;
            uint curRankSubType;
            BossTowerRankUnit rankUnitData;
            Color color_1, color_2;
            public void Init(Transform trans)
            {
                objRankItem = trans.Find("RankItem");
                objPass = trans.Find("Image_PassBG").gameObject;
                icon = objRankItem.Find("Image_Rank").GetComponent<Image>();
                number = objRankItem.Find("Text_Number").GetComponent<Text>();
                name = objRankItem.Find("Text01").GetComponent<Text>();
                stage = objRankItem.Find("Text02").GetComponent<Text>();
                round = objRankItem.Find("Text03").GetComponent<Text>();
                time = objRankItem.Find("Text04").GetComponent<Text>();
                btnDetail = objRankItem.Find("Btn_Detail").GetComponent<Button>();
                objHaveQua = objRankItem.Find("Image_Pass").gameObject;
                objNotQua = objRankItem.Find("Text_Failed").gameObject;

                btnDetail.onClick.AddListener(DetailOnClick);
            }
            public void SetData(BossTowerRankUnit rankUnitData, uint curRankType, uint curRankSubType)
            {
                this.curRankType = curRankType;
                this.curRankSubType = curRankSubType;
                this.rankUnitData = rankUnitData;
                if (rankUnitData != null)
                {
                    objRankItem.gameObject.SetActive(true);
                    objPass.SetActive(false);
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
                    bool bHaveBossFightQualifier = Sys_ActivityBossTower.Instance.CheckIsHaveBossFightQualifier(ranuNum, rankUnitData.Layer);
                    if (bHaveBossFightQualifier)
                    {
                        ColorUtility.TryParseHtmlString("#8C5D34", out color_1);
                        ColorUtility.TryParseHtmlString("#8B5A6B", out color_2);
                        number.color = color_1;
                        name.color = color_2;
                        stage.color = color_2;
                        round.color = color_2;
                        time.color = color_2;
                    }
                    else
                    {
                        number.color = Color.gray;
                        name.color = Color.gray;
                        stage.color = Color.gray;
                        round.color = Color.gray;
                        time.color = Color.gray;
                        objNotQua.GetComponent<Text>().color = Color.gray;
                    }
                    objHaveQua.SetActive(bHaveBossFightQualifier);
                    objNotQua.SetActive(!bHaveBossFightQualifier);
                    btnDetail.gameObject.SetActive(rankUnitData.RoleId != Sys_Role.Instance.RoleId);
                    name.text = rankUnitData.Name.ToStringUtf8();
                    stage.text = rankUnitData.Layer.ToString();
                    round.text = rankUnitData.Round.ToString();
                    time.text = TimeManager.GetDateTime(rankUnitData.Time).ToString("yyyy/MM/dd");
                }
                else
                {
                    objRankItem.gameObject.SetActive(false);
                    objPass.SetActive(true);
                }
            }
            private void DetailOnClick()
            {
                if (null != rankUnitData)
                {
                    Sys_Role_Info.Instance.OpenRoleInfo(rankUnitData.RoleId,Sys_Role_Info.EType.Avatar);
                }
            }
        }
        #endregion 
    }
}