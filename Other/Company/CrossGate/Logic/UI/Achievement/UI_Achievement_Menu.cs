using DG.Tweening;
using Framework;
using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Achievement_Menu : UIBase
    {
        #region 组件
        Button closeBtn;
        Text mainClassTitle;
        Transform View_Left;
        InputField inputField;
        InfinityGrid infinityGridLeft;
        Button searchBtn;

        Transform View_Right;
        Transform viewNormal;
        Transform viewSearch;
        Slider achSlider;
        Text sliderNum;
        Text achTitle;
        Text achSubTitle;
        Text searchTitle;
        Dropdown dropdown;
        ScrollRect scrollRect;
        RectTransform contentParent;
        RectTransform itemParent;
        RectTransform bigItemRect;
        Transform subItem1;
        Transform subItem2;
        Transform subItem3;
        GameObject starObj;
        VerticalLayoutGroup layoutGroup;

        static RectTransform targetParent;
        #endregion
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
        protected override void OnOpen(object arg)
        {
            if (arg != null)
                param = arg as OpenAchievementMenuParam;
        }
        protected override void OnDestroy()
        {
            param = null;
            curAchievementMainTypeData = null;
            curAchievementSubTypeData.Clear();
            curSelectAchievementDataList.Clear();
            achievementDetailsCellList.Clear();
            requestedSubClassList.Clear();
            curAllAchievementReqList.Clear();
            showTimer?.Cancel();
        }
        #endregion
        #region 组件查找、事件注册
        private void OnParseComponent()
        {
            targetParent = transform.Find("Animator").GetComponent<RectTransform>();
            closeBtn = transform.Find("Animator/View_TipsBgNew01/Btn_Close").GetComponent<Button>();
            mainClassTitle = transform.Find("Animator/View_TipsBgNew01/Text_Title").GetComponent<Text>();
            View_Left = transform.Find("Animator/View_Left");
            inputField = View_Left.Find("InputField").GetComponent<InputField>();
            infinityGridLeft = View_Left.Find("Scroll View").GetComponent<InfinityGrid>();
            searchBtn = View_Left.Find("Button").GetComponent<Button>();

            View_Right = transform.Find("Animator/View_Right");
            viewNormal = View_Right.Find("View_Top/View_Normal");
            viewSearch = View_Right.Find("View_Top/View_Search");
            achSlider = viewNormal.Find("Slider").GetComponent<Slider>();
            sliderNum = viewNormal.Find("Slider_Num").GetComponent<Text>();
            starObj = viewNormal.Find("Star").gameObject;
            achTitle = viewNormal.Find("Text_Title").GetComponent<Text>();
            achSubTitle = viewNormal.Find("Text_Title/Text_Title2").GetComponent<Text>();
            searchTitle = viewSearch.Find("Text_Title").GetComponent<Text>();
            dropdown = View_Right.Find("View_Top/Sort/Dropdown").GetComponent<Dropdown>();
            scrollRect = View_Right.Find("Scroll View").GetComponent<ScrollRect>();
            contentParent = View_Right.Find("Scroll View/Viewport/Content").GetComponent<RectTransform>();
            itemParent = View_Right.Find("Scroll View/Viewport/Content/Item").GetComponent<RectTransform>();
            layoutGroup = itemParent.GetComponent<VerticalLayoutGroup>();
            bigItemRect = View_Right.Find("Scroll View/Viewport/Content/Item/List").GetComponent<RectTransform>();
            subItem1 = View_Right.Find("Scroll View/Viewport/Content/Spread_Out").GetComponent<RectTransform>();
            subItem2 = View_Right.Find("Scroll View/Viewport/Content/Spread_Out2").GetComponent<RectTransform>();
            subItem3 = View_Right.Find("Scroll View/Viewport/Content/Spread_Out3").GetComponent<RectTransform>();
            subItem1.gameObject.SetActive(false);
            subItem2.gameObject.SetActive(false);
            subItem3.gameObject.SetActive(false);
            itemParent.gameObject.SetActive(true);
            bigItemRect.gameObject.SetActive(false);

            inputField.onValueChanged.AddListener(InputFieldChanged);
            closeBtn.onClick.AddListener(() => { CloseSelf(); });
            searchBtn.onClick.AddListener(SearchOnClick);

            infinityGridLeft.onCreateCell += OnCreateCellLeft;
            infinityGridLeft.onCellChange += OnCellChangeLeft;

            PopdownListBuild();
            dropdown.onValueChanged.AddListener(OnDropdownChanged);
        }
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_Achievement.Instance.eventEmitter.Handle<bool>(Sys_Achievement.EEvents.OnRefreshAchievementData, OnRefreshAchievementData, toRegister);
        }
        private void OnRefreshAchievementData(bool isFinished)
        {
            //if (isFinished)
            //{
            //    Sys_Achievement.Instance.InsertionSort(curAllAchievementReqList);
            //}
            RefreshAchievementDetailData();
        }
        #endregion
        #region 数据
        string curInputContent;
        static uint curMainCalss;
        uint curSelectSubClass;
        uint curSelectedAchievementDataId;
        EAchievementDegreeType curAchievementDegreeType;
        OpenAchievementMenuParam param;
        CSVAchievementType.Data curAchievementMainTypeData;
        List<SubAchievementTypeData> curAchievementSubTypeData;
        List<AchievementDataCell> curSelectAchievementDataList;

        List<AchievementDetailsCell> achievementDetailsCellList = new List<AchievementDetailsCell>();
        List<AchievementDataCell> requestedSubClassList = new List<AchievementDataCell>();
        List<AchievementDataCell> curAllAchievementReqList = new List<AchievementDataCell>();

        List<AchievementDetailsSubCell_1> subCellList_1 = new List<AchievementDetailsSubCell_1>();
        List<AchievementDetailsSubCell_2> subCellList_2 = new List<AchievementDetailsSubCell_2>();
        List<AchievementDetailsSubCell_3> subCellList_3 = new List<AchievementDetailsSubCell_3>();
        #endregion
        #region 初始化
        private void InitView()
        {
            if (param != null)
            {
                if (param.tid != 0)
                {
                    AchievementDataCell achData = Sys_Achievement.Instance.GetAchievementByTid(param.tid);
                    curMainCalss = achData.csvAchievementData.MainClass;
                    curSelectSubClass = achData.csvAchievementData.SubClass;
                }
                else
                {
                    curMainCalss = param.mainCalss;
                    curSelectSubClass = param.subCalss;
                }

                curAchievementMainTypeData = CSVAchievementType.Instance.GetConfData(curMainCalss);
                curAchievementSubTypeData = Sys_Achievement.Instance.GetSubAchievementTypeDataList(curMainCalss);

                curAchievementDegreeType = EAchievementDegreeType.All;
                dropdown.value = (int)curAchievementDegreeType;

                SetAchievementTitle();
                RefreshSearchResult();
                SetAchievementReqList();
            }
        }
        #endregion
        #region 界面显示
        private void SetAchievementReqList()
        {
            if (isSearch && !string.IsNullOrEmpty(curInputContent))
                curAllAchievementReqList = GetAchievementBySubClass(curSelectSubClass);
            else
                curAllAchievementReqList = Sys_Achievement.Instance.GetAchievementData(curMainCalss, curSelectSubClass, curAchievementDegreeType, 3);
            //服务器成就免请求，在成就主界面已请求过一次 (光辉事迹成就同步服务器成就，同理免请求)
            if (curMainCalss != Sys_Achievement.Instance.GetServerAchievementMainClassData().tid || curMainCalss != Sys_Achievement.Instance.GetSheenAchievementMainClassData().tid)
            {
                for (int i = 0; i < curAllAchievementReqList.Count; i++)
                {
                    if (CheckRequestedAchData(curAllAchievementReqList[i].tid))
                    {
                        Sys_Achievement.Instance.curSelectedReqAchIdList.Add(curAllAchievementReqList[i].tid);
                        requestedSubClassList.Add(curAllAchievementReqList[i]);
                    }
                }
            }
            curSelectAchievementDataList = curAllAchievementReqList;
            if (Sys_Achievement.Instance.curSelectedReqAchIdList.Count > 0)
                Sys_Achievement.Instance.OnSystemDataReq();
            else
                RefreshAchievementDetailData();
        }
        private bool CheckRequestedAchData(uint tid)
        {
            bool isCanAdd = true;
            if (requestedSubClassList.Count > 0)
            {
                for (int i = 0; i < requestedSubClassList.Count; i++)
                {
                    if (requestedSubClassList[i].tid == tid)
                    {
                        isCanAdd = false;
                        break;
                    }
                }
            }
            return isCanAdd;
        }
        private int GetIndexByCurSelectSubClass()
        {
            int index = 0;
            for (int i = 0; i < curAchievementSubTypeData.Count; i++)
            {
                if (curSelectSubClass == curAchievementSubTypeData[i].subClass)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }
        private int GetIndexByCurAchievementId()
        {
            int index = 0;
            for (int i = 0; i < curSelectAchievementDataList.Count; i++)
            {
                if (param.tid == curSelectAchievementDataList[i].tid)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }
        private void SetAchievementTitle()
        {
            mainClassTitle.text= LanguageHelper.GetAchievementContent(curAchievementMainTypeData.MainClassTest);
            achTitle.text = LanguageHelper.GetAchievementContent(curAchievementMainTypeData.MainClassTest);
            SetViewNormal();
        }
        private void SetViewNormal()
        {
            viewNormal.gameObject.SetActive(true);
            viewSearch.gameObject.SetActive(false);
            achSubTitle.text = LanguageHelper.GetAchievementContent(Sys_Achievement.Instance.GetSubAchievementTypeData(curMainCalss, curSelectSubClass).subTitle);
            bool isServerAch = curMainCalss == Sys_Achievement.Instance.GetServerAchievementMainClassData().tid || curMainCalss == Sys_Achievement.Instance.GetSheenAchievementMainClassData().tid;
            if (isServerAch)
            {
                sliderNum.gameObject.SetActive(false);
                achSlider.gameObject.SetActive(false);
                starObj.SetActive(false);
            }
            else
            {
                sliderNum.gameObject.SetActive(true);
                achSlider.gameObject.SetActive(true);
                starObj.SetActive(true);
                uint finishedStar = Sys_Achievement.Instance.GetAchievementStar(curMainCalss, curSelectSubClass, EAchievementDegreeType.Finished, false, 1);
                uint allStar = Sys_Achievement.Instance.GetAchievementStar(curMainCalss, curSelectSubClass, EAchievementDegreeType.All, false, 1);
                achSlider.minValue = 0;
                achSlider.maxValue = allStar;
                achSlider.value = finishedStar;
                sliderNum.text = string.Format("{0}/{1}", finishedStar, allStar);
            }
        }
        private void SetViewSearch(int count)
        {
            viewNormal.gameObject.SetActive(false);
            viewSearch.gameObject.SetActive(true);
            searchTitle.text = LanguageHelper.GetTextContent(5877, curInputContent, count.ToString());
        }
        private void PopdownListBuild()
        {
            dropdown.ClearOptions();
            dropdown.options.Clear();
            Dropdown.OptionData op_1 = new Dropdown.OptionData
            {
                text = LanguageHelper.GetTextContent(5873),
            };
            dropdown.options.Add(op_1);
            Dropdown.OptionData op_2 = new Dropdown.OptionData
            {
                text = LanguageHelper.GetTextContent(5874),
            };
            dropdown.options.Add(op_2);
            Dropdown.OptionData op_3 = new Dropdown.OptionData
            {
                text = LanguageHelper.GetTextContent(5875),
            };
            dropdown.options.Add(op_3);
            Dropdown.OptionData op_4 = new Dropdown.OptionData
            {
                text = LanguageHelper.GetTextContent(5876),
            };
            dropdown.options.Add(op_4);
        }
        private void OnDropdownChanged(int value)
        {
            curAchievementDegreeType = (EAchievementDegreeType)value;
            SetAchievementReqList();
        }
        private void InputFieldChanged(string arg0)
        {
            curInputContent = arg0;
            if (isSearch)
            {
                if (!string.IsNullOrEmpty(curInputContent))
                    SearchOnClick();
                else
                {
                    isSearch = false;
                    curAchievementSubTypeData = Sys_Achievement.Instance.GetSubAchievementTypeDataList(curMainCalss);
                    curSelectSubClass = curAchievementSubTypeData[0].subClass;
                    RefreshSearchResult();
                    SetAchievementReqList();
                }
            }
        }
        bool isSearch;
        Dictionary<uint, Dictionary<EAchievementDegreeType, List<AchievementDataCell>>> curSearchResultDataDic = new Dictionary<uint, Dictionary<EAchievementDegreeType, List<AchievementDataCell>>>();
        private void SearchOnClick()
        {
            if (string.IsNullOrEmpty(curInputContent))
                return;
            isSearch = true;
            curSearchResultDataDic.Clear();
            //符合搜索内容及成就完成度的所有成就
            curSearchResultDataDic = Sys_Achievement.Instance.GetAchievementBySearch(curInputContent, curMainCalss, EAchievementDegreeType.All);
            int searchCount = Sys_Achievement.Instance.GetAchievementBySearchCount();
            SetViewSearch(searchCount);
            #region 刷新左边子类型页签
            //清理成就子类型页签，根据数据重新分子类页签
            curAchievementSubTypeData.Clear();
            //添加总页签
            curAchievementSubTypeData.Add(Sys_Achievement.Instance.GetSubAchievementTypeData(curMainCalss));
            foreach (var item in curSearchResultDataDic)
            {
                curAchievementSubTypeData.Add(Sys_Achievement.Instance.GetSubAchievementTypeData(curMainCalss, item.Key));
            }
            curAchievementSubTypeData.Sort((a, b) =>
            {
                return (int)(a.subClass - b.subClass);
            });
            curSelectSubClass = curAchievementSubTypeData[0].subClass;
            RefreshSearchResult();
            #endregion
            #region 刷新右边具体成就数据
            SetAchievementReqList();
            #endregion
        }
        private List<AchievementDataCell> GetAchievementBySubClass(uint subClass)
        {
            List<AchievementDataCell> list = new List<AchievementDataCell>();
            if (subClass == 0)
            {
                foreach (var item in curSearchResultDataDic.Values)
                {
                    if (item.ContainsKey(curAchievementDegreeType))
                    {
                        List<AchievementDataCell> dataList = item[curAchievementDegreeType];
                        list.AddRange(dataList);
                    }
                }
            }
            else
            {
                if (curSearchResultDataDic.ContainsKey(subClass))
                {
                    if (curSearchResultDataDic[subClass].ContainsKey(curAchievementDegreeType))
                    {
                        list.AddRange(curSearchResultDataDic[subClass][curAchievementDegreeType]);
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// 刷新搜索结果子类页签
        /// </summary>
        private void RefreshSearchResult()
        {
            infinityGridLeft.CellCount = curAchievementSubTypeData.Count;
            infinityGridLeft.ForceRefreshActiveCell();
            //infinityGridLeft.MoveToIndex(0);
        }
        private void OnCreateCellLeft(InfinityGridCell cell)
        {
            SearchCell entry = new SearchCell();
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
        }
        private void OnCellChangeLeft(InfinityGridCell cell, int index)
        {
            SearchCell entry = cell.mUserData as SearchCell;
            entry.SetData(curSelectSubClass,curAchievementSubTypeData[index], RefreshAchievementDataBySubType);
        }
        /// <summary>
        /// 刷新成就数据通过所选子类型
        /// </summary>
        private void RefreshAchievementDataBySubType(uint subCalss)
        {
            curSelectSubClass = subCalss;
            SetAchievementReqList();
        }

        /// <summary>
        /// 刷新成就详情数据
        /// </summary>
        private void RefreshAchievementDetailData()
        {
            if (!isSearch)
                SetViewNormal();
            DefaultSubCell();
            for (int i = 0; i < achievementDetailsCellList.Count; i++)
            {
                AchievementDetailsCell cell = achievementDetailsCellList[i];
                cell.ClearAllEvent();
                PoolManager.Recycle(cell);
            }
            achievementDetailsCellList.Clear();
            FrameworkTool.CreateChildList(itemParent, curSelectAchievementDataList.Count);
            for (int i = 0; i < curSelectAchievementDataList.Count; i++)
            {
                Transform trans = itemParent.GetChild(i);
                AchievementDetailsCell cell = PoolManager.Fetch<AchievementDetailsCell>();
                cell.Init(trans);
                cell.SetData(curSelectAchievementDataList[i], ShowAchievementDetail, i);
                achievementDetailsCellList.Add(cell);
            }
            FrameworkTool.ForceRebuildLayout(itemParent.gameObject);
            scrollRect.content.anchoredPosition = new Vector2(scrollRect.content.anchoredPosition.x, 0);

            if (param != null && param.tid != 0)
            {
                infinityGridLeft.MoveToIndex(GetIndexByCurSelectSubClass());
                SetPosView(GetIndexByCurAchievementId());
                param = null;
            }
        }

        private void ShowAchievementDetail(uint type, AchievementDataCell data,int index)
        {
            if (curSelectedAchievementDataId != data.tid)
            {
                curSelectedAchievementDataId = data.tid;
                if (type == 1)
                    RefreshSubCell_1(data,index);
                else if(type==2)
                    RefreshSubCell_2(data, index);
                else
                    RefreshSubCell_3(data, index);
            }
            else
            {
                oldDiffY = 0;
                oldIndex = 0;
                curSelectedAchievementDataId = 0;
                if (type == 1)
                {
                    subItem1.SetParent(contentParent);
                    RecycleSubCell_1();
                    subItem1.gameObject.SetActive(false);
                }
                else if (type == 2)
                {
                    subItem2.SetParent(contentParent);
                    RecycleSubCell_2();
                    subItem2.gameObject.SetActive(false);
                }
                else
                {
                    subItem3.SetParent(contentParent);
                    RecycleSubCell_3();
                    subItem3.gameObject.SetActive(false);
                }
                FrameworkTool.ForceRebuildLayout(itemParent.gameObject);
            }
        }
        private void DefaultSubCell()
        {
            oldDiffY = 0;
            oldIndex = 0;
            if (subCellList_1.Count > 0)
            {
                subItem1.SetParent(contentParent);
                RecycleSubCell_1();
                subItem1.gameObject.SetActive(false);
            }
            if (subCellList_2.Count > 0)
            {
                subItem2.SetParent(contentParent);
                RecycleSubCell_2();
                subItem2.gameObject.SetActive(false);
            }
            if (subCellList_3.Count > 0)
            {
                subItem3.SetParent(contentParent);
                RecycleSubCell_3();
                subItem3.gameObject.SetActive(false);
            }
        }
        private void RecycleSubCell_1()
        {
            for (int i = 0; i < subCellList_1.Count; i++)
            {
                AchievementDetailsSubCell_1 cell = subCellList_1[i];
                PoolManager.Recycle(cell);
            }
            subCellList_1.Clear();
        }
        private void RefreshSubCell_1(AchievementDataCell data,int index)
        {
            RecycleSubCell_1();
            int count = data.gatherItemList.Count;
            FrameworkTool.CreateChildList(subItem1, count);
            for (int i = 0; i < count; i++)
            {
                Transform trans = subItem1.GetChild(i);
                AchievementDetailsSubCell_1 cell = PoolManager.Fetch<AchievementDetailsSubCell_1>();
                cell.Init(trans);
                cell.SetData(data.gatherItemList[i]);
                subCellList_1.Add(cell);
            }
            subItem1.gameObject.SetActive(true);
            subItem1.SetParent(itemParent);
            subItem1.transform.SetSiblingIndex(index+1);
            FrameworkTool.ForceRebuildLayout(itemParent.gameObject);
            SetPosView(index,1);
        }
        private void RecycleSubCell_2()
        {
            for (int i = 0; i < subCellList_2.Count; i++)
            {
                AchievementDetailsSubCell_2 cell = subCellList_2[i];
                PoolManager.Recycle(cell);
            }
            subCellList_2.Clear();
        }
        private void RefreshSubCell_2(AchievementDataCell data,int index)
        {
            RecycleSubCell_2();
            int count = data.multiShared.Count;
            FrameworkTool.CreateChildList(subItem2, count);
            for (int i = 0; i < count; i++)
            {
                Transform trans = subItem2.GetChild(i);
                AchievementDetailsSubCell_2 cell = PoolManager.Fetch<AchievementDetailsSubCell_2>();
                cell.Init(trans);
                cell.SetData(data.multiShared[i]);
                subCellList_2.Add(cell);
            }
            subItem2.gameObject.SetActive(true);
            subItem2.SetParent(itemParent);
            subItem2.transform.SetSiblingIndex(index + 1);
            FrameworkTool.ForceRebuildLayout(itemParent.gameObject);
            SetPosView(index,2);
        }
        private void RecycleSubCell_3()
        {
            for (int i = 0; i < subCellList_3.Count; i++)
            {
                AchievementDetailsSubCell_3 cell = subCellList_3[i];
                PoolManager.Recycle(cell);
            }
            subCellList_3.Clear();
        }
        private void RefreshSubCell_3(AchievementDataCell data, int index)
        {
            RecycleSubCell_3();
            int count = data.achHistoryList.Count;
            FrameworkTool.CreateChildList(subItem3, count);
            for (int i = 0; i < count; i++)
            {
                Transform trans = subItem3.GetChild(i);
                AchievementDetailsSubCell_3 cell = PoolManager.Fetch<AchievementDetailsSubCell_3>();
                cell.Init(trans);
                cell.SetData(data.achHistoryList[i]);
                subCellList_3.Add(cell);
            }
            subItem3.gameObject.SetActive(true);
            subItem3.SetParent(itemParent);
            subItem3.transform.SetSiblingIndex(index + 1);
            FrameworkTool.ForceRebuildLayout(itemParent.gameObject);
            SetPosView(index, 2);
        }
        Timer showTimer;
        float oldDiffY = 0;
        int oldIndex = 0;
        private void SetPosView(int index, int type = 0)
        {
            float spaY = layoutGroup.spacing;
            float cellY = bigItemRect.sizeDelta.y;
            float curContentY = index * cellY;
            float maxContentY = curSelectAchievementDataList.Count <= 5 ? 0 : (curSelectAchievementDataList.Count - 5) * cellY;
            float posY;
            if (type == 0)
            {
                posY = curContentY < maxContentY ? curContentY : maxContentY;
                if (posY > 0)
                {
                    showTimer?.Cancel();
                    showTimer = Timer.Register(0.2f, () =>
                    {
                        showTimer?.Cancel();
                        scrollRect.StopMovement();
                        scrollRect.content.DOLocalMoveY(Mathf.Min(contentParent.sizeDelta.y < 0 ? 0 : contentParent.sizeDelta.y, posY), 0.3f);
                    }, null, false, true);
                }
            }
            else
            {
                curContentY = (index + 1) * cellY;
                float diffY = type == 1 ? subItem1.GetComponent<RectTransform>().sizeDelta.y : subItem2.GetComponent<RectTransform>().sizeDelta.y;
                float value_1 = 5 * cellY;
                float value_2 = curContentY + diffY;
                posY = value_1 < value_2 ? value_2 - value_1 : 0;
                if (oldDiffY != 0 && posY < scrollRect.content.anchoredPosition.y)
                {
                    if (oldIndex < index || maxContentY < scrollRect.content.anchoredPosition.y)
                    {
                        float curY = scrollRect.content.anchoredPosition.y > oldDiffY ? (scrollRect.content.anchoredPosition.y - oldDiffY) : (index < 4 ? 0 : scrollRect.content.anchoredPosition.y);
                        scrollRect.content.anchoredPosition = new Vector2(scrollRect.content.anchoredPosition.x, curY);
                    }
                }
                posY = posY <= scrollRect.content.anchoredPosition.y ? scrollRect.content.anchoredPosition.y : posY;
                scrollRect.StopMovement();
                scrollRect.content.DOLocalMoveY(Mathf.Min(contentParent.sizeDelta.y < 0 ? 0 : contentParent.sizeDelta.y, (posY + 0.1f)), 0.3f);
                oldDiffY = diffY;
                oldIndex = index;
            }
        }
        #endregion
        public class SearchCell
        {
            Text textDrak;
            Text textLight;
            CP_Toggle toggle;
            SubAchievementTypeData data;
            Action<uint> aciton;
            public void Init(Transform trans)
            {
                toggle = trans.GetComponent<CP_Toggle>();
                textDrak = trans.Find("Btn_Menu_Dark/Text_Menu_Dark").GetComponent<Text>();
                textLight = trans.Find("Image_Menu_Light/Text_Menu_Light").GetComponent<Text>();

                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener(ToggleChanged);
            }
            public void SetData(uint curselectSubClass,SubAchievementTypeData data,Action<uint> action)
            {
                this.data = data;
                this.aciton = action;
                textDrak.text = LanguageHelper.GetAchievementContent(data.subTitle);
                textLight.text = LanguageHelper.GetAchievementContent(data.subTitle);
                OnSelect(curselectSubClass== data.subClass);
            }
            private void OnSelect(bool isSelect)
            {
                toggle.SetSelected(isSelect, false);
            }
            private void ToggleChanged(bool isOn)
            {
                if(isOn)
                    aciton?.Invoke(data.subClass);
            }
        }
        public class AchievementDetailsCell
        {
            Text name;
            Button giftBtn;
            Transform starList;
            Button shareBtn;
            Slider slider;
            Text sliderNum;
            Text content;
            Button checkBtn;
            Transform textRate;
            Text textRateNum;
            Transform textPlayer;
            Text textPlayerName;
            Transform textPlayers;
            Button textPlayersBtn;
            Text textTime;
            Text textServer;
            Text textServers;
            Button btnServer;
            Transform complete;
            Transform[] starArray=new Transform[4];
            AchievementIconCell iconCell;

            AchievementDataCell data;
            Action<uint,AchievementDataCell,int> action;
            int index;
            public void Init(Transform tran)
            {
                iconCell = new AchievementIconCell();
                iconCell.Init(tran.Find("Achievement_Item"));
                name = tran.Find("Title/Text_Name").GetComponent<Text>();
                giftBtn = tran.Find("Title/Button_Gift").GetComponent<Button>();
                starList = tran.Find("Title/Starlist");
                shareBtn = tran.Find("Button_Share").GetComponent<Button>();
                slider = tran.Find("Slider").GetComponent<Slider>();
                sliderNum = tran.Find("Text_Slider_Num").GetComponent<Text>();
                content = tran.Find("Text_Content").GetComponent<Text>();
                checkBtn = tran.Find("Text_Content/Button").GetComponent<Button>();
                textRate = tran.Find("Text_Group/Text_Rate");
                textRateNum = tran.Find("Text_Group/Text_Rate/Num").GetComponent<Text>();
                textPlayer = tran.Find("Text_Group/Text_Player");
                textPlayerName = tran.Find("Text_Group/Text_Player/Name").GetComponent<Text>();
                textPlayers = tran.Find("Text_Group/Text_Players");
                textPlayersBtn = tran.Find("Text_Group/Text_Players/Button").GetComponent<Button>();
                textServer = tran.Find("Text_Group/Text_Sever").GetComponent<Text>();
                textServers = tran.Find("Text_Group/Text_Server2").GetComponent<Text>();
                btnServer = textServers.transform.Find("Button").GetComponent<Button>();

                textTime = tran.Find("Text_Time").GetComponent<Text>();
                complete = tran.Find("Image_Complete");

                for (int i = 0; i < 4; i++)
                {
                    starArray[i] = starList.GetChild(i);
                }

                giftBtn.onClick.AddListener(GiftOnClick);
                shareBtn.onClick.AddListener(ShareOnClick);
                checkBtn.onClick.AddListener(CheckOnClick);
            }
            public void SetData(AchievementDataCell data,Action<uint,AchievementDataCell,int> aciton,int index)
            {
                this.data = data;
                this.action = aciton;
                this.index = index;
                iconCell.SetData(data);
                giftBtn.gameObject.SetActive(data.dropItems.Count > 0);
                name.text = LanguageHelper.GetAchievementContent(data.csvAchievementData.Achievement_Title);
                content.text = LanguageHelper.GetAchievementContent(data.csvAchievementData.Task_Test, 1);

                SetStar();
                SetReachState();
                CheckIsFinished();
            }
            private void SetStar()
            {
                if (data.CheckIsMerge() && curMainCalss == Sys_Achievement.Instance.GetSheenAchievementMainClassData().tid)
                {
                    for (int i = 0; i < starArray.Length; i++)
                    {
                        starArray[i].gameObject.SetActive(false);
                    }
                    for (int i = 0; i < data.csvAchievementData.Rare; i++)
                    {
                        starArray[i].gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (data.CheckIsSelf())
                    {
                        for (int i = 0; i < starArray.Length; i++)
                        {
                            starArray[i].gameObject.SetActive(false);
                        }
                        for (int i = 0; i < data.csvAchievementData.Rare; i++)
                        {
                            starArray[i].gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < starArray.Length; i++)
                        {
                            starArray[i].gameObject.SetActive(false);
                        }
                    }
                }
            }
            private void SetReachState()
            {
                if (data.csvAchievementData.MainClass != Sys_Achievement.Instance.GetServerAchievementMainClassData().tid)
                {
                    uint ratioNum = data.ratio;
                    if (data.timestamp != 0)
                        ratioNum = data.ratio <= 1 ? 1 : data.ratio;
                    string ratio  = (ratioNum / 10000f).ToString("P2");
                    SetTextGroup(1, ratio);
                }
                //和服
                else if (data.CheckIsMerge() && curMainCalss == Sys_Achievement.Instance.GetSheenAchievementMainClassData().tid)
                {
                    SetTextGroup(0);
                    SetTextGroup(5);
                }
                //服务器成就
                else
                {
                    if (data.timestamp == 0)
                    {
                        SetTextGroup(0);
                    }
                    else
                    {
                        if (data.familyShared != null || data.roleShared != null)
                        {
                            string str = data.familyShared != null ? data.familyShared.familyName : data.roleShared.roleName;
                            SetTextGroup(data.roleShared != null ? 2 : 3, str);
                        }
                        else if (data.multiShared != null && data.multiShared.Count > 0)
                        {
                            SetTextGroup(4);
                        }
                    }
                }
                SetTrigger_Type();
            }
            private void SetTextGroup(int index,string str=null)
            {
                if (index == 0)
                {
                    textRate.gameObject.SetActive(false);
                    textPlayer.gameObject.SetActive(false);
                    textPlayers.gameObject.SetActive(false);
                }
                else if (index == 1)
                {
                    textRate.gameObject.SetActive(true);
                    textPlayer.gameObject.SetActive(false);
                    textPlayers.gameObject.SetActive(false);
                    textRateNum.text = str;
                }
                else if (index == 2 || index == 3)
                {
                    textRate.gameObject.SetActive(false);
                    textPlayer.gameObject.SetActive(true);
                    textPlayer.GetComponent<Text>().text = LanguageHelper.GetTextContent(index == 2 ? 5860u : 5883u);
                    textPlayers.gameObject.SetActive(false);
                    textPlayerName.text = str;
                }
                else if (index == 4)
                {
                    textRate.gameObject.SetActive(false);
                    textPlayer.gameObject.SetActive(false);
                    textPlayers.gameObject.SetActive(true);

                    textPlayersBtn.onClick.AddListener(CheckPlaysOnClick);
                }
                else if (index == 5)
                {
                    if (data.achHistoryList.Count > 1)
                    {
                        textServer.gameObject.SetActive(false);
                        textServers.gameObject.SetActive(true);
                        btnServer.onClick.AddListener(CheckHistoryOnClick);
                    }
                    else
                    {
                        textServers.gameObject.SetActive(false);
                        textServer.gameObject.SetActive(true);
                        str = string.Format("{0} {1}",data.achHistoryList[0].serverName, TimeManager.GetDateTime(data.achHistoryList[0].timestamp).ToString("yyyy-MM-dd HH:mm:ss"));
                        textServer.text = LanguageHelper.GetTextContent(5884, str);
                    }
                }
            }
            private void CheckIsFinished()
            {
                //在其他服合并前达成过
                if (data.CheckIsMerge() && curMainCalss == Sys_Achievement.Instance.GetSheenAchievementMainClassData().tid)
                {
                    complete.gameObject.SetActive(true);
                    shareBtn.gameObject.SetActive(true);
                    textTime.gameObject.SetActive(false);
                }
                else
                {
                    if (data.timestamp != 0)
                    {
                        textTime.gameObject.SetActive(true);
                        complete.gameObject.SetActive(true);
                        shareBtn.gameObject.SetActive(data.CheckIsSelf());
                        textTime.text = TimeManager.GetDateTime(data.timestamp).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    else
                    {
                        textTime.gameObject.SetActive(false);
                        complete.gameObject.SetActive(false);
                        shareBtn.gameObject.SetActive(false);
                    }
                }
            }
            private void SetTrigger_Type()
            {
                //触发类成就
                if (data.csvAchievementData.Trigger_Type == 1)
                {
                    slider.gameObject.SetActive(false);
                    sliderNum.gameObject.SetActive(false);
                    checkBtn.gameObject.SetActive(false);
                }
                //累计类成就
                else if (data.csvAchievementData.Trigger_Type == 2)
                {
                    slider.gameObject.SetActive(true);
                    sliderNum.gameObject.SetActive(data.timestamp == 0);
                    checkBtn.gameObject.SetActive(false);
                    slider.minValue = 0;
                    uint maxValue= data.csvAchievementData.ReachTypeAchievement[data.csvAchievementData.ReachTypeAchievement.Count - 1];
                    slider.maxValue = maxValue;
                    slider.value = data.achievementValue;
                    sliderNum.text = string.Format("{0}/{1}", data.achievementValue, maxValue);
                }
                //收集类成就
                else if (data.csvAchievementData.Trigger_Type == 3)
                {
                    slider.gameObject.SetActive(true);
                    sliderNum.gameObject.SetActive(data.timestamp == 0);
                    checkBtn.gameObject.SetActive(true);
                    slider.minValue = 0;
                    slider.maxValue = data.gatherItemList.Count;
                    slider.value = data.gatherItemCount;
                    sliderNum.text = string.Format("{0}/{1}", data.gatherItemCount, data.gatherItemList.Count);
                }
            }
            /// <summary>
            /// 奖励预览
            /// </summary>
            private void GiftOnClick()
            {
                if (data != null)
                {
                    ClickShowPositionData clickData = new ClickShowPositionData();
                    clickData.data = data;
                    clickData.clickTarget = giftBtn.GetComponent<RectTransform>();
                    clickData.parent = targetParent;
                    UIManager.OpenUI(EUIID.UI_Achievement_RewardList, false, clickData);
                }
            }
            /// <summary>
            /// 分享
            /// </summary>
            private void ShareOnClick()
            {
                if (data != null)
                {
                    ClickShowPositionData clickData = new ClickShowPositionData();
                    clickData.data = data;
                    clickData.clickTarget = shareBtn.GetComponent<RectTransform>();
                    clickData.parent = targetParent;
                    UIManager.OpenUI(EUIID.UI_Achievement_ShareList, false, clickData);
                }
            }
            /// <summary>
            /// 查看成就详情
            /// </summary>
            private void CheckOnClick()
            {
                action?.Invoke(1, data, index);
            }
            /// <summary>
            /// 查看达成玩家
            /// </summary>
            private void CheckPlaysOnClick()
            {
                action?.Invoke(2, data,index);
            }
            private void CheckHistoryOnClick()
            {
                action?.Invoke(3, data, index);
            }
            public void ClearAllEvent()
            {
                giftBtn.onClick.RemoveAllListeners();
                shareBtn.onClick.RemoveAllListeners();
                checkBtn.onClick.RemoveAllListeners();
                textPlayersBtn.onClick.RemoveAllListeners();
            }
        }
        public class AchievementDetailsSubCell_1
        {
            Transform getTrtans;
            Text textName;
            public void Init(Transform tran)
            {
                getTrtans = tran.Find("Background/Checkmark/Checkmark (1)");
                textName = tran.Find("Text").GetComponent<Text>();
            }
            public void SetData(AchievementDataCell.GatherItem data)
            {
                getTrtans.gameObject.SetActive(data.isGet);
                textName.text = Sys_Achievement.Instance.GetItemNameByTypeAchievement(data);
            }
        }
        public class AchievementDetailsSubCell_2
        {
            Text textName;
            public void Init(Transform tran)
            {
                textName = tran.GetComponent<Text>();
            }
            public void SetData(AchievementDataCell.RoleShared data)
            {
                textName.text = data.roleName;
            }
        }
        public class AchievementDetailsSubCell_3
        {
            Text textServer;
            public void Init(Transform tran)
            {
                textServer = tran.GetComponent<Text>();
            }
            public void SetData(AchievementDataCell.RoleAchievementHistory data)
            {
                textServer.text = string.Format("{0} {1}", data.serverName, TimeManager.GetDateTime(data.timestamp).ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }
    }
}