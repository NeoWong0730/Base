using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_BravePromotion : UIBase
    {
        #region 系统函数
        protected override void OnLoaded()
        {
            OnParseComponent();
        }
        protected override void OnOpen(object arg)
        {
            if (arg != null)
                Sys_Promotion.Instance.curSelectedRankType = (uint)arg;
        }
        protected override void OnShow()
        {
            OnProwShow();
        }
        protected override void OnHide()
        {
            Sys_Promotion.Instance.curSelectedRankType = 0;
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Reputation.Instance.eventEmitter.Handle(Sys_Reputation.EEvents.OnReputationUpdate, OnReputationUpdate, toRegister);
            Sys_Experience.Instance.eventEmitter.Handle(Sys_Experience.EEvents.OnExperienceUpgrade, OnReputationUpdate, toRegister);
        }        
        #endregion
        #region 组件
        Transform leftParent;
        UI_BravePromotionRightView viewRight;
        #endregion
        #region 定义数据
        private List<UI_BravePromotionLeftItemCeil> leftCeilList = new List<UI_BravePromotionLeftItemCeil>();
        private List<CSVAssessType.Data>  assessTypeDataList;
        private int visualGridCount;
        private uint curSelectIndex;
        public static RectTransform targetParent;
        #endregion
        #region 初始化
        private void OnParseComponent()
        {
            targetParent = transform.Find("Animator").GetComponent<RectTransform>();
            leftParent = transform.Find("Animator/Scroll View/Viewport/Content");
            viewRight = new UI_BravePromotionRightView();
            viewRight.OnInit(transform.Find("Animator/View_Right"));
            transform.Find("Animator/View_TipsBgNew02/Btn_Close").GetComponent<Button>().onClick.AddListener(()=> { CloseSelf(); });
        }
        #endregion
        #region 界面显示
        private void OnProwShow()
        {
            assessTypeDataList = Sys_Promotion.Instance.assessTypeDataList;
            visualGridCount = assessTypeDataList.Count;
            if (Sys_Promotion.Instance.curSelectedRankType != 0)
            {
                curSelectIndex = Sys_Promotion.Instance.curSelectedRankType;
            }
            else
            {
                curSelectIndex = assessTypeDataList[0].id;
                Sys_Promotion.Instance.curSelectedRankType = curSelectIndex;
            }
            for (int i = 0; i < leftCeilList.Count; i++)
            {
                UI_BravePromotionLeftItemCeil ceil = leftCeilList[i];
                ceil.RemoveListener();
                PoolManager.Recycle(leftCeilList[i]);
            }
            leftCeilList.Clear();
            FrameworkTool.CreateChildList(leftParent, visualGridCount);
            for (int i = 0; i < visualGridCount; i++)
            {
                Transform tran = leftParent.transform.GetChild(i);
                UI_BravePromotionLeftItemCeil ceil = PoolManager.Fetch<UI_BravePromotionLeftItemCeil>();
                ceil.Init(tran);
                ceil.AddListener(OnSelectIndex);
                ceil.RefreshData(assessTypeDataList[i]);
                ceil.OnSelect(curSelectIndex == assessTypeDataList[i].id);
                leftCeilList.Add(ceil);
            }
            viewRight.RefreshRightView(curSelectIndex);
        }
        private void OnSelectIndex(uint index)
        {
            curSelectIndex = index;
            Sys_Promotion.Instance.curSelectedRankType = curSelectIndex;
            viewRight.RefreshRightView(index);
        }
        #endregion
        #region 功能函数
        private void OnReputationUpdate()
        {
            viewRight.RefreshRightView(curSelectIndex);
        }
        #endregion
    }
    public class UI_BravePromotionLeftItemCeil
    {
        private Transform trans;
        private Text btnMenuDark_text;
        private Text btnMenuLight_text;
        private Action<uint> action;
        private CP_Toggle toggle;
        private CSVAssessType.Data data;
        public void Init(Transform trans)
        {
            this.trans = trans;
            btnMenuDark_text = trans.Find("Btn_Menu_Dark/Text_Menu_Dark").GetComponent<Text>();
            btnMenuLight_text = trans.Find("Image_Menu_Light/Text_Menu_Light").GetComponent<Text>();
            toggle = trans.GetComponent<CP_Toggle>();
            toggle.onValueChanged.AddListener((isOn) =>
            {
                if (null != data)
                {
                    OnToggleClick(isOn, data.id);
                }
            });
        }
        public void RefreshData(CSVAssessType.Data data)
        {
            this.data = data;
            TextHelper.SetText(btnMenuDark_text, LanguageHelper.GetTextContent(data.langid));
            TextHelper.SetText(btnMenuLight_text, LanguageHelper.GetTextContent(data.langid));
        }
        public void AddListener(Action<uint> action)
        {
            this.action = action;
        }
        private void OnToggleClick(bool isOn, uint _index)
        {
            if (isOn)
            {
                //StringBuilder btnEventStr = StringBuilderPool.GetTemporary();
                action?.Invoke(_index);
                //btnEventStr.Append("UnLock_BravePromotion_Id:"+ _index);
                //UIManager.HitButton(EUIID.UI_BravePromotion, StringBuilderPool.ReleaseTemporaryAndToString(btnEventStr));
            }
        }
        public void RemoveListener()
        {
            if (this.action != null)
                this.action = null;
            if (toggle != null)
                toggle.onValueChanged.RemoveAllListeners();
        }
        public void OnSelect(bool isSelect)
        {
            toggle.SetSelected(isSelect, false);
        }
        public void ToggleOff()
        {
            toggle.SetSelected(false, false);
        }
    }
    public class UI_BravePromotionRightView
    {
        #region 组件
        Transform trans;
        InfinityGrid infinityGrid;
        #endregion
        #region 数据定义
        List<CheckTypeData> curCheckTypeDataList;
        int visualGridCount;
        List<UI_BravePromotionRightItemCeil> promoRightItemList = new List<UI_BravePromotionRightItemCeil>();
        CSVAssessType.Data typeData;
        #endregion
        #region 初始化
        public void OnInit(Transform transform)
        {
            trans = transform;
            infinityGrid = trans.Find("Scroll View").GetComponent<InfinityGrid>();
            infinityGrid.onCreateCell += OnCreateCell;
            infinityGrid.onCellChange += OnCellChange;
        }
        private void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_BravePromotionRightItemCeil entry = cell.mUserData as UI_BravePromotionRightItemCeil;
            entry.RefreshData(curCheckTypeDataList[index]);
        }
        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_BravePromotionRightItemCeil entry = new UI_BravePromotionRightItemCeil();
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
        }
        #endregion
        #region 功能函数
        public void RefreshRightView(uint type)
        {
            typeData = CSVAssessType.Instance.GetConfData(type);
            if (typeData == null)
                return;

            curCheckTypeDataList= Sys_Promotion.Instance.GetCurDataList(type);
            visualGridCount = curCheckTypeDataList.Count;

            infinityGrid.CellCount = visualGridCount;
            infinityGrid.ForceRefreshActiveCell();
            infinityGrid.MoveToIndex(0);
        }
        #endregion

        public class UI_BravePromotionRightItemCeil
        {
            Transform trans;
            Image icon;
            Text title;
            Text des;
            Button btn1;
            Button btn2;
            Slider slider;
            Text perText;
            Text quaText;
            Image quaImage;
            RectTransform imgRectTransform;
            CSVAssessMain.Data mainData;
            CSVAssessScore.Data scoreData;
            CSVDailyActivity.Data dailyData;
            Color _color;
            public void Init(Transform trans)
            {
                this.trans = trans;
                icon = trans.Find("Mask/Image_Icon").GetComponent<Image>();
                imgRectTransform = icon.GetComponent<RectTransform>();
                title = trans.Find("Title").GetComponent<Text>();
                des = trans.Find("Text").GetComponent<Text>();
                btn1 = trans.Find("Btn01").GetComponent<Button>();
                btn2 = trans.Find("Btn02").GetComponent<Button>();
                slider = trans.Find("Slider").GetComponent<Slider>();
                perText = trans.Find("Slider/Text_Percent").GetComponent<Text>();
                quaImage = trans.Find("Qua_BG").GetComponent<Image>();
                quaText = trans.Find("Qua_BG/Text").GetComponent<Text>();

                btn1.onClick.AddListener(MoveToView);
                btn2.onClick.AddListener(MoveToView);
                slider.minValue = 0;
            }
            public void RefreshData(CheckTypeData data)
            {
                if (data.activitydata != null)
                    RefreshData(data.activitydata);
                if (data.mainData != null)
                    RefreshData(data.mainData);
            }

            public void RefreshData(CSVAssessMain.Data data)
            {
                this.mainData = data;
                dailyData = null;
                if (this.mainData != null)
                {
                    imgRectTransform.sizeDelta = new Vector2(56, 56);
                    imgRectTransform.localScale = new Vector3(1, 1, 1);
                    ImageHelper.SetIcon(icon, this.mainData.Icon);
                    TextHelper.SetText(title, LanguageHelper.GetTextContent(this.mainData.Name));
                    string strDes = string.Empty;
                    string strPercent = string.Empty;
                    CheckState(this.mainData.Bar);

                    if (this.mainData.Bar)//可量化的
                    {
                        scoreData = Sys_Promotion.Instance.GetAssessScoreData(this.mainData.id);
                        if (scoreData != null)
                        {
                            long curPower = Sys_Promotion.Instance.GetCurTypePower(this.mainData.Jump[0]);
                            long targetPower = curPower < scoreData.Great ? scoreData.Great : scoreData.Perfect;
                            Sys_Promotion.PromotionLve prolv = Sys_Promotion.Instance.CheckRoleEvaluation(scoreData, curPower);
                            strPercent = string.Format("{0}/{1}", curPower, targetPower);
                            switch ((Sys_Promotion.ViewType)this.mainData.Jump[0])
                            {
                                case Sys_Promotion.ViewType.Equip:
                                case Sys_Promotion.ViewType.Skill:
                                case Sys_Promotion.ViewType.FamilyEmpowerment:
                                case Sys_Promotion.ViewType.Ornament:
                                case Sys_Promotion.ViewType.Treasure:
                                case Sys_Promotion.ViewType.Partner:
                                case Sys_Promotion.ViewType.Adventure:
                                case Sys_Promotion.ViewType.Jewel:
                                case Sys_Promotion.ViewType.AwakenImprint:
                                case Sys_Promotion.ViewType.FamilyEntrust:
                                    strDes = LanguageHelper.GetTextContent(this.mainData.Describe, targetPower.ToString());
                                    break;
                                case Sys_Promotion.ViewType.Reputation://声望阶段+等级
                                    uint danLevel = ((uint)targetPower + 100) / 100;
                                    uint specificLevel = (uint)targetPower % 100;
                                    CSVFameRank.Data curFameRank = CSVFameRank.Instance.GetConfData(Sys_Reputation.Instance.danLevel);
                                    CSVFameRank.Data targetFameRank = CSVFameRank.Instance.GetConfData(danLevel);
                                    string curTitle = string.Format("{0}{1}", LanguageHelper.GetTextContent(curFameRank.name), LanguageHelper.GetTextContent(1000002, Sys_Reputation.Instance.specificLevel.ToString()));
                                    string targetTitle = string.Format("{0}{1}", LanguageHelper.GetTextContent(targetFameRank.name), LanguageHelper.GetTextContent(1000002, specificLevel.ToString()));
                                    strDes = LanguageHelper.GetTextContent(this.mainData.Describe, targetTitle);
                                    strPercent = string.Format("{0}/{1}", curTitle, targetTitle);
                                    break;
                                case Sys_Promotion.ViewType.Awaken://觉醒描述内容需要觉醒名称+觉醒阶数
                                    CSVTravellerAwakening.Data curTraveData = CSVTravellerAwakening.Instance.GetConfData((uint)curPower);
                                    CSVTravellerAwakening.Data targetTraveData = CSVTravellerAwakening.Instance.GetConfData((uint)targetPower);
                                    string curDes = string.Format("{0}{1}", LanguageHelper.GetTextContent(curTraveData.NameId), LanguageHelper.GetTextContent(curTraveData.StepsId));
                                    string targetDes = string.Format("{0}{1}", LanguageHelper.GetTextContent(targetTraveData.NameId), LanguageHelper.GetTextContent(targetTraveData.StepsId));
                                    strDes = LanguageHelper.GetTextContent(this.mainData.Describe, targetDes);
                                    strPercent = string.Format("{0}/{1}", curDes, targetDes);
                                    break;
                                case Sys_Promotion.ViewType.PetMessage://宠物描述需要宠物数量+人物等级
                                    strDes = LanguageHelper.GetTextContent(this.mainData.Describe, targetPower.ToString(), Sys_Role.Instance.Role.Level.ToString());
                                    break;
                                default:
                                    break;
                            }
                            TextHelper.SetText(des, strDes);
                            slider.maxValue = targetPower;
                            slider.value = curPower > targetPower ? targetPower : curPower;
                            TextHelper.SetText(perText, strPercent);

                            string quatext = null;
                            switch (prolv)
                            {
                                case Sys_Promotion.PromotionLve.LvUp:
                                    quatext = LanguageHelper.GetTextContent(1002904);
                                    ColorUtility.TryParseHtmlString("#cb5656", out _color);
                                    break;
                                case Sys_Promotion.PromotionLve.Great:
                                    quatext = LanguageHelper.GetTextContent(1002905);
                                    ColorUtility.TryParseHtmlString("#63a8c1", out _color);
                                    break;
                                case Sys_Promotion.PromotionLve.Perfect:
                                    quatext = LanguageHelper.GetTextContent(1002906);
                                    ColorUtility.TryParseHtmlString("#e2b256", out _color);
                                    break;
                                default:
                                    break;
                            }
                            TextHelper.SetText(quaText, quatext);
                            quaImage.color = _color;
                            quaImage.rectTransform.sizeDelta = new Vector2(quaText.preferredWidth, quaImage.rectTransform.sizeDelta.y);
                        }
                    }
                    else
                    {
                        if (this.mainData.RankType == 4 || this.mainData.RankType == 5)
                        {
                            CheckState_Reward(true);
                            long curPower = Sys_Promotion.Instance.GetCurTypePower(this.mainData.Jump[0], this.mainData.id);
                            uint targetPower = CSVItem.Instance.GetConfData(this.mainData.id).Daily_limit;
                            strPercent = string.Format("{0}/{1}", curPower, targetPower);
                            switch ((Sys_Promotion.ViewType)this.mainData.Jump[0])
                            {
                                case Sys_Promotion.ViewType.FamilyMall:
                                case Sys_Promotion.ViewType.AssistShop:
                                case Sys_Promotion.ViewType.ArenaShop:
                                    strDes = LanguageHelper.GetTextContent(this.mainData.Describe, curPower.ToString());
                                    break;
                                default:
                                    break;
                            }
                            TextHelper.SetText(des, strDes);
                            slider.maxValue = targetPower;
                            slider.value = curPower > targetPower ? targetPower : curPower;
                            TextHelper.SetText(perText, strPercent);
                        }
                        else
                        {
                            strDes = LanguageHelper.GetTextContent(this.mainData.Describe);
                            TextHelper.SetText(des, strDes);
                        }
                    }
                }
            }

            public void RefreshData(CSVDailyActivity.Data data)
            {
                this.dailyData = data;
                mainData = null;
                if (dailyData != null)
                {
                    imgRectTransform.sizeDelta = new Vector2(116, 148);
                    imgRectTransform.localScale = new Vector3(0.6f, 0.6f, 1f);
                    uint curTimes = Sys_Daily.Instance.getDailyCurTimes(dailyData.id);//当前次数;
                    uint maxTimes = dailyData.limite;
                    CheckState_Reward(maxTimes != 0);
                    ImageHelper.SetIcon(icon, dailyData.AvtiveIcon);
                    string titleStr = LanguageHelper.GetTextContent(dailyData.ActiveName).Replace("\n", "");
                    Regex reg = new Regex("\\<color.*?\\>.*?\\</color\\>");
                    MatchCollection matches = reg.Matches(titleStr);
                    int _index = 0;
                    int[] colorIndexs = new int[matches.Count];
                    foreach (Match item in matches)
                    {
                        colorIndexs[_index++] = item.Index;
                    }
                    for (int i = 0; i < colorIndexs.Length; i++)
                    {
                        titleStr = titleStr.Remove(colorIndexs[i], 15);
                        titleStr = titleStr.Insert(colorIndexs[i], "<color=#8b5a6b>");
                    }
                    TextHelper.SetText(title, titleStr);
                    StringBuilder strDes = new StringBuilder();
                    for (int i = 0; i < dailyData.Reward_int.Count; i++)
                    {
                        strDes.Append(LanguageHelper.GetTextContent(590000000 + dailyData.Reward_int[i]));
                        if (i < dailyData.Reward_int.Count - 1)
                        {
                            strDes.Append("、");
                        }
                    }
                    TextHelper.SetText(des, LanguageHelper.GetTextContent(590000200, strDes.ToString()));
                    TextHelper.SetText(perText, string.Format("{0}/{1}", curTimes, maxTimes));
                    slider.maxValue = maxTimes;
                    slider.value = curTimes;
                }
            }
            public void CheckState(bool _bool)
            {
                if (_bool)
                {
                    btn1.gameObject.SetActive(true);
                    btn1.transform.Find("Text_01").GetComponent<Text>().text = LanguageHelper.GetTextContent(1002903);
                    btn2.gameObject.SetActive(false);
                    slider.gameObject.SetActive(true);
                    quaImage.gameObject.SetActive(true);
                }
                else
                {
                    btn1.gameObject.SetActive(false);
                    btn2.gameObject.SetActive(true);
                    btn2.transform.Find("Text_01").GetComponent<Text>().text = LanguageHelper.GetTextContent(1002902);
                    slider.gameObject.SetActive(false);
                    quaImage.gameObject.SetActive(false);
                }
            }
            public void CheckState_Reward(bool _bool)
            {
                quaImage.gameObject.SetActive(false);
                btn1.gameObject.SetActive(true);
                btn1.transform.Find("Text_01").GetComponent<Text>().text = LanguageHelper.GetTextContent(1002908);
                btn2.gameObject.SetActive(false);
                if (_bool)
                {
                    slider.gameObject.SetActive(true);
                }
                else
                {
                    slider.gameObject.SetActive(false);
                }
            }
            private void MoveToView()
            {
                if (this.mainData != null)
                {
                    if (this.mainData.Jump.Count > 1)
                    {
                        List<SubAssessMainData> subList = Sys_Promotion.Instance.GetSubAssessMainData(this.mainData);
                        if (subList.Count > 1)
                        {
                            ClickPromotionListData clickData = new ClickPromotionListData();
                            clickData.eUIID = EUIID.UI_BravePromotion;
                            clickData.clickTarget = btn1.GetComponent<RectTransform>();
                            clickData.parent = UI_BravePromotion.targetParent;
                            UIManager.OpenUI(EUIID.UI_PromotionList, false, clickData);
                        }
                        else
                            Sys_Promotion.Instance.MoveToView(subList[0].jump, subList[0].bar);
                    }
                    else
                    {
                        Sys_Promotion.Instance.MoveToView(this.mainData.Jump[0], this.mainData.Bar);
                    }
                }

                if (dailyData != null)
                {
                    if (Sys_Daily.Instance.JoinDaily(dailyData.id) == false)
                        return;
                    UIManager.CloseUI(EUIID.UI_BravePromotion);
                }
            }
            public void RemoveListener()
            {
                if (btn1 != null)
                    btn1.onClick.RemoveAllListeners();
                if (btn2 != null)
                    btn2.onClick.RemoveAllListeners();
            }
        }
    }
}