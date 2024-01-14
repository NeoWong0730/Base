using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic.Core;
using Framework;
using Table;
using Lib.Core;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Logic
{
    public class UI_Title : UIComponent
    {
        private CP_ToggleRegistry CP_ToggleRegistryTab;
        private InfinityGridLayoutGroup infinity;
        private Transform infinityParent;
        private Dictionary<GameObject, TitleCeil> titleceils = new Dictionary<GameObject, TitleCeil>();
        private List<Title> titles = new List<Title>();
        private int curtitleSelectIndex = 0;

        private int curSelectTitleTab;
        private int CurSelectTitleTab
        {
            get { return curSelectTitleTab; }
            set
            {
                if (curSelectTitleTab != value)
                {
                    curSelectTitleTab = value;
                    curtitleSelectIndex = 0;
                    RefreshData();
                }
            }
        }

        //private Text curUseTitle;
        //private Text curShowTitle;

        private GameObject mTitleShowRoot;
        private Text mTitle_text1_show;
        private Text mTitle_text2_show;
        private Image mTitle_img2_show;
        private Image mTitle_img3_show;
        private Transform mTitle_Fx3parent_show;
        private AsyncOperationHandle<GameObject> requestRef_show;
        private GameObject titleEffect_show;
        private GameObject mTitleShowNone_show;

        private GameObject mTitleUseRoot;
        private Text mTitle_text1_use;
        private Text mTitle_text2_use;
        private Image mTitle_img2_use;
        private Image mTitle_img3_use;
        private Transform mTitle_Fx3parent_use;
        private AsyncOperationHandle<GameObject> requestRef_use;
        private GameObject titleEffect_use;
        private GameObject mTitleShowNone_use;

        private GameObject mCurSelectTile;
        private Text mTitle_text1_curSelect;
        private Text mTitle_text2_curSelect;
        private Image mTitle_img2_curSelect;
        private Image mTitle_img3_curSelect;
        private Transform mTitle_Fx3parent_curSelect;
        private AsyncOperationHandle<GameObject> requestRef_curSelect;
        private GameObject titleEffect_curSelect;
        private GameObject mTitleShowNone_curSelect;
        private Text condition;
        private Text time;
        private Text name;
        private Text point;
        private Button GoToButton;
        private Transform attrParentCurSelect;
        private Transform attrParentCurSelect_root;

        private GameObject m_NextConditionRoot;
        private Text m_NextCondition;

        private Transform curUseAttrParent;
        private Button useTitlebutton;
        private Button showTitlebutton;
        private Button seriesCollect;
        private Text useTitlebuttontext;
        private Text showTitlebuttontext;

        private Title curSelectTitle;
        private Transform seriesParent;

        private GameObject upArrow;
        private GameObject downArrow;
        //private GameObject viewAttr;
        private GameObject viewNone;

        private GameObject scoreCuruse;
        private GameObject scoreCurseries;
        private Text scoreCurusetext;
        private Text scoreCurseriestext;

        //private RawImage rawImage;
        //private Image eventImage;
        public AssetDependencies assetDependencies;
        private ShowSceneControl showSceneControl;
        private HeroLoader heroLoader;
        private WS_UIModelShowManagerEntity _uiModelShowManagerEntity;

        private Dictionary<uint, GameObject> m_RedPoints = new Dictionary<uint, GameObject>();

        protected override void Loaded()
        {
            mTitleShowRoot = transform.Find("View_Right/Title01/Title").gameObject;
            mTitle_text1_show = mTitleShowRoot.transform.Find("Text").GetComponent<Text>();
            mTitle_text2_show = mTitleShowRoot.transform.Find("Image1/Text").GetComponent<Text>();
            mTitle_img2_show = mTitleShowRoot.transform.Find("Image1").GetComponent<Image>();
            mTitle_img3_show = mTitleShowRoot.transform.Find("Image2").GetComponent<Image>();
            mTitle_Fx3parent_show = mTitleShowRoot.transform.Find("Image2/Fx");
            mTitleShowNone_show = transform.Find("View_Right/Title01/Image_None").gameObject;

            mTitleUseRoot = transform.Find("View_Right/Title02/Title").gameObject;
            mTitle_text1_use = mTitleUseRoot.transform.Find("Text").GetComponent<Text>();
            mTitle_text2_use = mTitleUseRoot.transform.Find("Image1/Text").GetComponent<Text>();
            mTitle_img2_use = mTitleUseRoot.transform.Find("Image1").GetComponent<Image>();
            mTitle_img3_use = mTitleUseRoot.transform.Find("Image2").GetComponent<Image>();
            mTitle_Fx3parent_use = mTitleUseRoot.transform.Find("Image2/Fx");
            mTitleShowNone_use = transform.Find("View_Right/Title02/Image_None").gameObject;

            mCurSelectTile = transform.Find("View_Right/Image_Message/Title").gameObject;
            mTitle_text1_curSelect = mCurSelectTile.transform.Find("Text").GetComponent<Text>();
            mTitle_text2_curSelect = mCurSelectTile.transform.Find("Image1/Text").GetComponent<Text>();
            mTitle_img2_curSelect = mCurSelectTile.transform.Find("Image1").GetComponent<Image>();
            mTitle_img3_curSelect = mCurSelectTile.transform.Find("Image2").GetComponent<Image>();
            mTitle_Fx3parent_curSelect = mCurSelectTile.transform.Find("Image2/Fx");
            condition = transform.Find("View_Right/Image_Message/Image_Title2/Text_Condition").GetComponent<Text>();

            m_NextConditionRoot = transform.Find("View_Right/Image_Message/Image_Title3").gameObject;
            m_NextCondition = transform.Find("View_Right/Image_Message/Image_Title3/Text_Condition").GetComponent<Text>();

            name = mCurSelectTile.transform.Find("Text_Name").GetComponent<Text>();
            point = transform.Find("View_Right/Image_Message/Image_Point/Text_Property/Text").GetComponent<Text>();
            GoToButton = transform.Find("View_Right/Image_Message/Btn_go").GetComponent<Button>();
            attrParentCurSelect = transform.Find("View_Right/Image_Message/Image_Title1/Grid01");
            attrParentCurSelect_root = transform.Find("View_Right/Image_Message/Image_Title1");
            time = transform.Find("View_Right/Image_Message/Image_Title2/Text_Condition/Text_Time").GetComponent<Text>();

            curUseAttrParent = transform.Find("View_Right/Vertial01/Grid01");
            scoreCuruse = transform.Find("View_Right/Vertial01/Image_Point").gameObject;
            scoreCurusetext = scoreCuruse.transform.Find("Text_Property/Text").GetComponent<Text>();

            seriesParent = transform.Find("View_Right/Vertial02/Grid01");
            scoreCurseries = transform.Find("View_Right/Vertial02/Image_Point").gameObject;
            scoreCurseriestext = scoreCurseries.transform.Find("Text_Property/Text").GetComponent<Text>();

            CP_ToggleRegistryTab = transform.Find("List_Menu/TabList").GetComponent<CP_ToggleRegistry>();
            Transform achTrans = transform.Find("List_Menu/TabList/ListItem");
            bool isShow = Sys_Achievement.Instance.CheckAchievementIsCanShow();
            achTrans.gameObject.SetActive(isShow);
            infinityParent = transform.Find("View_Content/Grid");
            infinity = infinityParent.gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            //curShowTitle = transform.Find("View_Attr/Grid/Title01/Text_Name").GetComponent<Text>();
            //curUseTitle = transform.Find("View_Attr/Grid/Title02/Text_Name").GetComponent<Text>();
            useTitlebutton = transform.Find("Btn_01").GetComponent<Button>();
            showTitlebutton = transform.Find("Btn_02").GetComponent<Button>();
            seriesCollect = transform.Find("Btn_Series").GetComponent<Button>();
            useTitlebuttontext = transform.Find("Btn_01/Text_01").GetComponent<Text>();
            showTitlebuttontext = transform.Find("Btn_02/Text_01").GetComponent<Text>();
            //viewAttr = transform.Find("View_Attr").gameObject;
            upArrow = transform.Find("View_Arrow/Image_UpArrow").gameObject;
            downArrow = transform.Find("View_Arrow/Image_DownArrow").gameObject;
            viewNone = transform.Find("View_None").gameObject;
            //rawImage = transform.Find("View_Middle/Charapter").GetComponent<RawImage>();
            //eventImage = transform.Find("View_Middle/EventImage").GetComponent<Image>();
            //Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(eventImage);
            //eventListener.AddEventListener(EventTriggerType.Drag, OnDrag);
            infinity.minAmount = 12;
            infinity.updateChildrenCallback = UpdateChildrenCallback1;
            for (int i = 0; i < infinityParent.childCount; i++)
            {
                GameObject go = infinityParent.GetChild(i).gameObject;
                TitleCeil titleCeil = new TitleCeil();
                titleCeil.BindGameObject(go);
                titleCeil.AddClickListener(OnCeilCollectSelected);
                titleceils.Add(go, titleCeil);
            }
            RegistEvent();
            ProcessEventsForAwake(true);

            m_RedPoints.Add(1, transform.Find("List_Menu/TabList/ListItem/Image_Dot").gameObject);
            m_RedPoints.Add(2, transform.Find("List_Menu/TabList/ListItem (1)/Image_Dot").gameObject);
            m_RedPoints.Add(3, transform.Find("List_Menu/TabList/ListItem (2)/Image_Dot").gameObject);
            m_RedPoints.Add(4, transform.Find("List_Menu/TabList/ListItem (3)/Image_Dot").gameObject);

            curSelectTitleTab = isShow ? 1 : 2;
        }

        protected override void ProcessEventsForAwake(bool toRegister)
        {
            Sys_Title.Instance.eventEmitter.Handle<uint>(Sys_Title.EEvents.OnRefreshTitleButtonShowState, OnRefreshTitleButtonShowState, toRegister);
            Sys_Title.Instance.eventEmitter.Handle<uint>(Sys_Title.EEvents.OnRefreshTitleButtonDressState, OnRefreshTitleButtonDressState, toRegister);
            Sys_Title.Instance.eventEmitter.Handle<uint>(Sys_Title.EEvents.OnRefreshTitleCeil, OnRefreshTitleCeil, toRegister);
            Sys_Title.Instance.eventEmitter.Handle(Sys_Title.EEvents.OnUpdateCurTitleDes, UpdateCurTitleDes, toRegister);
            Sys_Title.Instance.eventEmitter.Handle(Sys_Title.EEvents.OnUpdateTitleSeriesPos, UpdateTitleSeries, toRegister);
            Sys_Title.Instance.eventEmitter.Handle<uint>(Sys_Title.EEvents.OnTitleGet, OnTitleGet, toRegister);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            AddressablesUtil.ReleaseInstance(ref requestRef_use, OnAssetsLoaded_use);
            AddressablesUtil.ReleaseInstance(ref requestRef_show, OnAssetsLoaded_show);
            AddressablesUtil.ReleaseInstance(ref requestRef_curSelect, OnAssetsLoaded_CurSelect);

            Dictionary<GameObject, TitleCeil>.Enumerator enumerator = titleceils.GetEnumerator();
            while (enumerator.MoveNext())
            {
                TitleCeil TitleCeil = enumerator.Current.Value;
                TitleCeil?.OnDispose();
            }
        }

        uint onShowTime;
        public override void Show()
        {
            base.Show();
            gameObject.SetActive(true);
            CP_ToggleRegistryTab.SwitchTo(curSelectTitleTab);
            UpdateCurTitleDes();
            UpdateTitleSeries();
            RefreshData();
            //OnCreateModel();
            UIManager.HitPointShow(EUIID.UI_Attribute, ERoleViewType.ViewTitle.ToString());
            onShowTime = Sys_Time.Instance.GetServerTime();
            if (Sys_Family.Instance.familyData.isInFamily)
            {
                Sys_Family.Instance.SendGuildGetGuildInfoReq();
            }
        }

        public override void Hide()
        {
            UIManager.HitPointHide(EUIID.UI_Attribute, onShowTime, ERoleViewType.ViewTitle.ToString());
            //_UnloadShowContent();

            base.Hide();
        }

        protected override void Update()
        {
            foreach (var item in titleceils)
            {
                item.Value.Update();
            }
        }

        private void RegistEvent()
        {
            CP_ToggleRegistryTab.onToggleChange = OnTabChanged;
            useTitlebutton.onClick.AddListener(OnUseTitleClicked);
            showTitlebutton.onClick.AddListener(OnShowTitleClicked);
            seriesCollect.onClick.AddListener(() =>
            {
                UIManager.OpenUI(EUIID.UI_TitleSeries);
            });
            GoToButton.onClick.AddListener(OnGoToButtonClicked);
        }


        private void OnTitleGet(uint titleId)
        {
            UpdateRedState();
        }

        private void UpdateRedState()
        {
            if (Sys_Title.Instance.unReadTitles.Count == 0)
            {
                foreach (var item in m_RedPoints)
                {
                    item.Value.SetActive(false);
                }
                return;
            }
            for (int i = Sys_Title.Instance.unReadTitles.Count - 1; i >= 0; i--)
            {
                Title title = Sys_Title.Instance.unReadTitles[i];
                uint showClass = title.cSVTitleData.titleShowClass;
                if (showClass != curSelectTitleTab)
                {
                    m_RedPoints[showClass].SetActive(true);
                }
                else
                {
                    m_RedPoints[showClass].SetActive(false);
                    title.read = true;
                    Sys_Title.Instance.unReadTitles.RemoveAt(i);
                    Sys_Title.Instance.eventEmitter.Trigger(Sys_Title.EEvents.OnRefreshTitleRedState);
                }
            }
        }

        private void OnGoToButtonClicked()
        {
            CSVTitle.Data cSVTitleData = null;
            if (!curSelectTitle.isOnly)
            {
                cSVTitleData = curSelectTitle.cSVTitleData;
            }
            else
            {
                if (!curSelectTitle.active)
                {
                    cSVTitleData = curSelectTitle.cSVTitleData;
                }
                else if (!curSelectTitle.reachMaxLevel)
                {
                    uint nextTitle = curSelectTitle.cSVTitleData.id + 1;
                    cSVTitleData = CSVTitle.Instance.GetConfData(nextTitle);
                }
            }

            if (cSVTitleData.titleGo[0] == 1)
            {
                uint npcId = cSVTitleData.titleGo[1];
                ActionCtrl.Instance.MoveToTargetNPCAndInteractive(npcId);
                UIManager.CloseUI(EUIID.UI_Attribute);
            }
            else if (cSVTitleData.titleGo[0] == 2)//成就
            {
                if (!Sys_FunctionOpen.Instance.IsOpen(22090, true))
                    return;
                UIManager.OpenUI(EUIID.UI_Achievement);
                OpenAchievementMenuParam param = new OpenAchievementMenuParam()
                {
                    tid = cSVTitleData.titleGo[1],
                };
                UIManager.OpenUI(EUIID.UI_Achievement_Menu, false, param);
            }
            else if (cSVTitleData.titleGo[0] == 4 || cSVTitleData.titleGo[0] == 5)
            {
                uint itemId = cSVTitleData.titleGo[1];
                PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData
                              (_id: itemId,
                              _count: 0,
                              _bUseQuailty: true,
                              _bBind: false,
                              _bNew: false,
                              _bUnLock: false,
                              _bSelected: false,
                              _bShowCount: false,
                              _bShowBagCount: false,
                              _bUseClick: false,
                              _onClick: null,
                              _bshowBtnNo: false);
                UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_Attribute, showItem));
            }
            else if (cSVTitleData.titleGo[0] == 6)
            {

            }
            else if (cSVTitleData.titleGo[0] == 7)
            {
                if (!Sys_FunctionOpen.Instance.IsOpen(10701, true))
                {
                    return;
                }
                UIManager.OpenUI(EUIID.UI_LifeSkill_Message);
            }
            else if (cSVTitleData.titleGo[0] == 8)
            {
                if (!Sys_FunctionOpen.Instance.IsOpen(10701, true))
                {
                    return;
                }
                uint skillId = cSVTitleData.titleGo[1];
                LifeSkillOpenParm lifeSkillOpenParm = new LifeSkillOpenParm();
                lifeSkillOpenParm.skillId = skillId;
                lifeSkillOpenParm.itemId = 0;
                UIManager.OpenUI(EUIID.UI_LifeSkill_Message, false, lifeSkillOpenParm);
                //UIManager.OpenUI(EUIID.UI_LifeSkill_Message, false, skillId);
            }
            else if (cSVTitleData.titleGo[0] == 10)//成就等级
            {
                if (!Sys_FunctionOpen.Instance.IsOpen(22090, true))
                    return;
                UIManager.OpenUI(EUIID.UI_Achievement);
                UIManager.OpenUI(EUIID.UI_Achievement_Reward);
            }
        }

        /* 1)如果当前选择的称号已获得并且未穿戴，点击后，穿戴该称号并且按钮文字变成取消穿戴。
           2)如果当前选择的称号未获得，点击后，弹提示：未获得该称号。
           3)如果当前选择的称号已获得并且已穿戴，显示的是取消穿戴，点击后卸下该称号并且文字变成穿戴称号。
           4)如果当前没有外观，穿戴称号时，会同时使用该称号的外观。*/
        private void OnUseTitleClicked()
        {
            if (!curSelectTitle.active)
            {
                string content = CSVLanguage.Instance.GetConfData(2020741).words;
                Sys_Hint.Instance.PushContent_Normal(content);
                return;
            }
            if (curSelectTitle.Dress)
            {
                Sys_Title.Instance.DressUnloadReq();
            }
            else
            {
                Sys_Title.Instance.TitleDressReq(curSelectTitle.Id);
            }
        }

        /* 1）如果当前选择的称号已获得，点击后，显示该称号并且按钮文字变成取消显示。
           2）如果当前选择的称号未获得，点击后，弹提示：未获得该称号。
           3）如果当前选择的称号已获得并且已显示，显示的是取消显示，点击后卸除该称号外观并且文字变为显示称号。*/
        private void OnShowTitleClicked()
        {
            if (!curSelectTitle.active)
            {
                string content = CSVLanguage.Instance.GetConfData(2020741).words;
                Sys_Hint.Instance.PushContent_Normal(content);
                return;
            }
            if (!curSelectTitle.Show)
            {
                Sys_Title.Instance.TitleShowReq(curSelectTitle.Id);
            }
            else
            {
                Sys_Title.Instance.ShowUnloadReq();
            }
        }

        private void OnRefreshTitleButtonShowState(uint titleId)
        {
            if (curSelectTitle.Show)
            {
                TextHelper.SetText(showTitlebuttontext, 2020716);//取消显示
            }
            else
            {
                TextHelper.SetText(showTitlebuttontext, 2020715);//显示称号
            }
        }

        private void OnRefreshTitleButtonDressState(uint titleId)
        {
            if (curSelectTitle.Dress)
            {
                TextHelper.SetText(useTitlebuttontext, 2020714);//取消穿戴
            }
            else
            {
                TextHelper.SetText(useTitlebuttontext, 2020713);//穿戴称号
            }
        }


        private void OnTabChanged(int curToggle, int old)
        {
            CurSelectTitleTab = curToggle;
            UpdateRedState();
        }

        List<Title> tempList = new List<Title>();
        private void RefreshData()
        {
            if (curSelectTitleTab == 1)
            {
                tempList.Clear();
                int count = Sys_Title.Instance.AchievementTitles.Count;
                for (int i = 0; i < count; i++)
                {
                    if (Sys_Title.Instance.AchievementTitles[i].CheckAchievementTitleIsCanShow())
                        tempList.Add(Sys_Title.Instance.AchievementTitles[i]);
                }
                titles = tempList;
            }
            else if (curSelectTitleTab == 2)
            {
                titles = Sys_Title.Instance.Prestigetitles;
            }
            else if (curSelectTitleTab == 3)
            {
                titles = Sys_Title.Instance.CareerTitles;
            }
            else
            {
                titles = Sys_Title.Instance.Specialtitles;
            }

            infinity.SetAmount(titles.Count);

            if (titles.Count == 0)
            {
                viewNone.SetActive(true);
                upArrow.SetActive(false);
                downArrow.SetActive(false);
            }
            else
            {
                viewNone.SetActive(false);
                upArrow.SetActive(false);
                if (curtitleSelectIndex < 0)
                    curtitleSelectIndex = 0;
                if (curtitleSelectIndex >= titles.Count)
                    curtitleSelectIndex = 0;
                curSelectTitle = titles[curtitleSelectIndex];
                OnRefreshTitleButtonShowState(curSelectTitle.Id);
                OnRefreshTitleButtonDressState(curSelectTitle.Id);
                UpdateTitle_CurSelect();

                bool showGo = false;
                if (curSelectTitle.isOnly)
                {
                    showGo = curSelectTitle.cSVTitleData.titleGo[0] > 0 && !curSelectTitle.reachMaxLevel;
                }
                else
                {
                    showGo = curSelectTitle.cSVTitleData.titleGo[0] > 0;
                }
                GoToButton.gameObject.SetActive(showGo);
            }
        }

        private void OnRefreshTitleCeil(uint titleId)
        {
            foreach (var item in titleceils)
            {
                int dataIndex = item.Value.dataIndex;
                if (dataIndex > titles.Count - 1)
                    continue;
                item.Value.SetData(titles[dataIndex], dataIndex);
            }
        }

        private void UpdateChildrenCallback1(int index, Transform trans)
        {
            TitleCeil titleCeil = titleceils[trans.gameObject];
            titleCeil.SetData(titles[index], index);
            if (index != curtitleSelectIndex)
            {
                titleCeil.Release();
            }
            else
            {
                titleCeil.Select();
            }
            upArrow.SetActive(true);
            downArrow.SetActive(true);
            if (index == 0)
            {
                upArrow.SetActive(false);
            }
            if (index == titles.Count - 1)
            {
                downArrow.SetActive(false);
            }
        }

        #region UpdateCurTitle
        //更新当前穿戴当前使用的title
        private void UpdateCurTitleDes()
        {
            if (Sys_Title.Instance.curShowTitle != 0)
            {
                //TextHelper.SetText(curShowTitle, CSVTitle.Instance.GetConfData(Sys_Title.Instance.curShowTitle).titleLan);
                mTitleShowRoot.SetActive(true);
                mTitleShowNone_show.SetActive(false);
                UpdateTitle_Show();
            }
            else
            {
                mTitleShowRoot.SetActive(false);
                mTitleShowNone_show.SetActive(true);
                //TextHelper.SetText(curShowTitle, 2020718);
            }
            if (Sys_Title.Instance.curUseTitle != 0)
            {
                mTitleShowNone_use.SetActive(false);
                mTitleUseRoot.SetActive(true);
                UpdateTitle_Use();

                scoreCuruse.SetActive(true);
                curUseAttrParent.gameObject.SetActive(true);
                CSVTitle.Data cSVTitleData = CSVTitle.Instance.GetConfData(Sys_Title.Instance.curUseTitle);
                //TextHelper.SetText(curUseTitle, cSVTitleData.titleLan);
                scoreCurusetext.text = cSVTitleData.titlePoint.ToString();

                if (cSVTitleData.titleProperty == null)
                {
                    curUseAttrParent.gameObject.SetActive(false);
                }
                else
                {
                    curUseAttrParent.gameObject.SetActive(true);
                    int attrCount = cSVTitleData.titleProperty.Count;
                    FrameworkTool.CreateChildList(curUseAttrParent, attrCount);
                    for (int i = 0; i < attrCount; i++)
                    {
                        Transform child = curUseAttrParent.GetChild(i);
                        Text attrName = child.Find("Text_Property").GetComponent<Text>();
                        Text num = child.Find("Text_Property/Text").GetComponent<Text>();
                        uint attrid = cSVTitleData.titleProperty[i][0];
                        uint attrnum = cSVTitleData.titleProperty[i][1];
                        SetAttr(attrid, attrnum, attrName, num);
                    }
                }
            }
            else
            {
                mTitleShowNone_use.SetActive(true);
                mTitleUseRoot.SetActive(false);

                scoreCuruse.SetActive(false);
                //TextHelper.SetText(curUseTitle, 2020718);
                curUseAttrParent.gameObject.SetActive(false);
            }

            //FrameworkTool.ForceRebuildLayout(viewAttr);
        }

        public void UpdateTitle_Show()
        {
            CSVTitle.Data cSVTitleData = CSVTitle.Instance.GetConfData(Sys_Title.Instance.curShowTitle);
            if (cSVTitleData != null)
            {
                if (cSVTitleData.id == Sys_Title.Instance.familyTitle)
                {
                    if (cSVTitleData.titleShowIcon == 0)
                    {
                        SetTitleShowType_show(1);
                        TextHelper.SetText(mTitle_text1_show, Sys_Title.Instance.GetTitleFamiltyName());
                        TextHelper.SetTextGradient(mTitle_text1_show, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                        TextHelper.SetTextOutLine(mTitle_text1_show, cSVTitleData.titleShow[2]);
                    }
                    else
                    {
                        SetTitleShowType_show(2);
                        TextHelper.SetText(mTitle_text2_show, Sys_Title.Instance.GetTitleFamiltyName());
                        TextHelper.SetTextGradient(mTitle_text2_show, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                        TextHelper.SetTextOutLine(mTitle_text2_show, cSVTitleData.titleShow[2]);
                        ImageHelper.SetIcon(mTitle_img2_show, cSVTitleData.titleShowIcon, true);
                    }
                }
                else if (cSVTitleData.id == Sys_Title.Instance.bGroupTitle)
                {
                    if (cSVTitleData.titleShowIcon == 0)
                    {
                        SetTitleShowType_show(1);
                        TextHelper.SetText(mTitle_text1_show, Sys_Title.Instance.GetTitleWarriorGroupName());
                        TextHelper.SetTextGradient(mTitle_text1_show, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                        TextHelper.SetTextOutLine(mTitle_text1_show, cSVTitleData.titleShow[2]);
                    }
                    else
                    {
                        SetTitleShowType_show(2);
                        TextHelper.SetText(mTitle_text2_show, Sys_Title.Instance.GetTitleWarriorGroupName());
                        TextHelper.SetTextGradient(mTitle_text2_show, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                        TextHelper.SetTextOutLine(mTitle_text2_show, cSVTitleData.titleShow[2]);
                        ImageHelper.SetIcon(mTitle_img2_show, cSVTitleData.titleShowIcon, true);
                    }
                }
                else
                {
                    if (cSVTitleData.titleShowLan != 0)
                    {
                        if (cSVTitleData.titleShowIcon == 0)
                        {
                            SetTitleShowType_show(1);
                            TextHelper.SetText(mTitle_text1_show, cSVTitleData.titleShowLan);
                            //TextHelper.SetTextGradient(mTitle_text1_show, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                            //TextHelper.SetTextOutLine(mTitle_text1_show, cSVTitleData.titleShow[2]);
                        }
                        else
                        {
                            SetTitleShowType_show(2);
                            TextHelper.SetText(mTitle_text2_show, cSVTitleData.titleShowLan);
                            //TextHelper.SetTextGradient(mTitle_text2_show, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                            //TextHelper.SetTextOutLine(mTitle_text2_show, cSVTitleData.titleShow[2]);
                            ImageHelper.SetIcon(mTitle_img2_show, cSVTitleData.titleShowIcon);
                        }
                    }
                    else
                    {
                        SetTitleShowType_show(3);
                        ImageHelper.SetIcon(mTitle_img3_show, cSVTitleData.titleShowIcon);
                        uint FxId = cSVTitleData.titleShowEffect;
                        CSVSystemEffect.Data cSVSystemEffectData = CSVSystemEffect.Instance.GetConfData(FxId);
                        if (cSVSystemEffectData != null)
                        {
                            LoadTitleEffectAssetAsyn_show(cSVSystemEffectData.FxPath);
                        }
                    }
                }
            }
        }

        private void LoadTitleEffectAssetAsyn_show(string path)
        {
            AddressablesUtil.InstantiateAsync(ref requestRef_show, path, OnAssetsLoaded_show);
        }

        private void OnAssetsLoaded_show(AsyncOperationHandle<GameObject> handle)
        {
            titleEffect_show = handle.Result;
            if (null != titleEffect_show)
            {
                titleEffect_show.transform.SetParent(mTitle_Fx3parent_show);
                RectTransform rectTransform = titleEffect_show.transform as RectTransform;
                rectTransform.localPosition = Vector3.zero;
                rectTransform.localEulerAngles = Vector3.zero;
                rectTransform.localScale = Vector3.one;
            }
        }

        private void SetTitleShowType_show(int type)
        {
            if (type == 1)
            {
                mTitle_text1_show.gameObject.SetActive(true);
                mTitle_text2_show.gameObject.SetActive(false);
                mTitle_img2_show.gameObject.SetActive(false);
                mTitle_img3_show.gameObject.SetActive(false);
                mTitle_Fx3parent_show.gameObject.SetActive(false);
            }
            else if (type == 2)
            {
                mTitle_text1_show.gameObject.SetActive(false);
                mTitle_text2_show.gameObject.SetActive(true);
                mTitle_img2_show.gameObject.SetActive(true);
                mTitle_img3_show.gameObject.SetActive(false);
                mTitle_Fx3parent_show.gameObject.SetActive(false);
            }
            else
            {
                mTitle_text1_show.gameObject.SetActive(false);
                mTitle_text2_show.gameObject.SetActive(false);
                mTitle_img2_show.gameObject.SetActive(false);
                mTitle_img3_show.gameObject.SetActive(true);
                mTitle_Fx3parent_show.gameObject.SetActive(true);
            }
        }



        public void UpdateTitle_Use()
        {
            CSVTitle.Data cSVTitleData = CSVTitle.Instance.GetConfData(Sys_Title.Instance.curUseTitle);
            if (cSVTitleData != null)
            {
                if (cSVTitleData.id == Sys_Title.Instance.familyTitle)
                {
                    if (cSVTitleData.titleShowIcon == 0)
                    {
                        SetTitleShowType_CurSelect(1);
                        TextHelper.SetText(mTitle_text1_curSelect, Sys_Title.Instance.GetTitleFamiltyName());
                        TextHelper.SetTextGradient(mTitle_text1_curSelect, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                        TextHelper.SetTextOutLine(mTitle_text1_curSelect, cSVTitleData.titleShow[2]);
                    }
                    else
                    {
                        SetTitleShowType_CurSelect(2);
                        TextHelper.SetText(mTitle_text2_curSelect, Sys_Title.Instance.GetTitleFamiltyName());
                        TextHelper.SetTextGradient(mTitle_text2_curSelect, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                        TextHelper.SetTextOutLine(mTitle_text2_curSelect, cSVTitleData.titleShow[2]);
                        ImageHelper.SetIcon(mTitle_img2_curSelect, cSVTitleData.titleShowIcon, true);
                    }
                }
                else if (cSVTitleData.id == Sys_Title.Instance.bGroupTitle)
                {
                    if (cSVTitleData.titleShowIcon == 0)
                    {
                        SetTitleShowType_CurSelect(1);
                        TextHelper.SetText(mTitle_text1_curSelect, Sys_Title.Instance.GetTitleWarriorGroupName());
                        TextHelper.SetTextGradient(mTitle_text1_curSelect, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                        TextHelper.SetTextOutLine(mTitle_text1_curSelect, cSVTitleData.titleShow[2]);
                    }
                    else
                    {
                        SetTitleShowType_CurSelect(2);
                        TextHelper.SetText(mTitle_text2_curSelect, Sys_Title.Instance.GetTitleWarriorGroupName());
                        TextHelper.SetTextGradient(mTitle_text2_curSelect, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                        TextHelper.SetTextOutLine(mTitle_text2_curSelect, cSVTitleData.titleShow[2]);
                        ImageHelper.SetIcon(mTitle_img2_curSelect, cSVTitleData.titleShowIcon, true);
                    }
                }
                else
                {
                    if (cSVTitleData.titleShowLan != 0)
                    {
                        if (cSVTitleData.titleShowIcon == 0)
                        {
                            SetTitleShowType_Use(1);
                            TextHelper.SetText(mTitle_text1_use, cSVTitleData.titleShowLan);
                            //TextHelper.SetTextGradient(mTitle_text1_use, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                            //TextHelper.SetTextOutLine(mTitle_text1_use, cSVTitleData.titleShow[2]);
                        }
                        else
                        {
                            SetTitleShowType_Use(2);
                            TextHelper.SetText(mTitle_text2_use, cSVTitleData.titleShowLan);
                            //TextHelper.SetTextGradient(mTitle_text2_use, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                            //TextHelper.SetTextOutLine(mTitle_text2_use, cSVTitleData.titleShow[2]);
                            ImageHelper.SetIcon(mTitle_img2_use, cSVTitleData.titleShowIcon);
                        }
                    }
                    else
                    {
                        SetTitleShowType_Use(3);
                        ImageHelper.SetIcon(mTitle_img3_use, cSVTitleData.titleShowIcon);
                        uint FxId = cSVTitleData.titleShowEffect;
                        CSVSystemEffect.Data cSVSystemEffectData = CSVSystemEffect.Instance.GetConfData(FxId);
                        if (cSVSystemEffectData != null)
                        {
                            LoadTitleEffectAssetAsyn_Use(cSVSystemEffectData.FxPath);
                        }
                    }
                }
            }
        }

        private void LoadTitleEffectAssetAsyn_Use(string path)
        {
            AddressablesUtil.InstantiateAsync(ref requestRef_use, path, OnAssetsLoaded_use);
        }

        private void OnAssetsLoaded_use(AsyncOperationHandle<GameObject> handle)
        {
            titleEffect_use = handle.Result;
            if (null != titleEffect_use)
            {
                titleEffect_use.transform.SetParent(mTitle_Fx3parent_use);
                RectTransform rectTransform = titleEffect_use.transform as RectTransform;
                rectTransform.localPosition = Vector3.zero;
                rectTransform.localEulerAngles = Vector3.zero;
                rectTransform.localScale = Vector3.one;
            }
        }

        private void SetTitleShowType_Use(int type)
        {
            if (type == 1)
            {
                mTitle_text1_use.gameObject.SetActive(true);
                mTitle_text2_use.gameObject.SetActive(false);
                mTitle_img2_use.gameObject.SetActive(false);
                mTitle_img3_use.gameObject.SetActive(false);
                mTitle_Fx3parent_use.gameObject.SetActive(false);
            }
            else if (type == 2)
            {
                mTitle_text1_use.gameObject.SetActive(false);
                mTitle_text2_use.gameObject.SetActive(true);
                mTitle_img2_use.gameObject.SetActive(true);
                mTitle_img3_use.gameObject.SetActive(false);
                mTitle_Fx3parent_use.gameObject.SetActive(false);
            }
            else
            {
                mTitle_text1_use.gameObject.SetActive(false);
                mTitle_text2_use.gameObject.SetActive(false);
                mTitle_img2_use.gameObject.SetActive(false);
                mTitle_img3_use.gameObject.SetActive(true);
                mTitle_Fx3parent_use.gameObject.SetActive(true);
            }
        }

        #endregion

        //更新系列栏位
        private void UpdateTitleSeries()
        {
            List<uint> dressed = Sys_Title.Instance.GetDressedTitlePos();
            if (dressed.Count == 0)
            {
                scoreCurseries.SetActive(false);
                seriesParent.gameObject.SetActive(false);
                return;
            }
            scoreCurseries.SetActive(true);
            seriesParent.gameObject.SetActive(true);
            FrameworkTool.CreateChildList(seriesParent, dressed.Count);

            int point = 0;
            for (int i = 0; i < dressed.Count; i++)
            {
                Transform child = seriesParent.GetChild(i);
                uint seriesId = dressed[i];
                Text attrName = child.Find("Text_Property").GetComponent<Text>();
                Text attrNum = child.Find("Text_Property/Text").GetComponent<Text>();
                CSVTitleSeries.Data cSVTitleSeriesData = CSVTitleSeries.Instance.GetConfData(seriesId);
                uint attrId = cSVTitleSeriesData.seriesProperty[0][0];
                uint attrValue = cSVTitleSeriesData.seriesProperty[0][1];
                SetAttr(attrId, attrValue, attrName, attrNum);
                point += cSVTitleSeriesData.seriesPoint;

            }
            scoreCurseriestext.text = point.ToString();

            //FrameworkTool.ForceRebuildLayout(viewAttr);
        }

        private void SetAttr(uint attr1, uint value1, Text attrName1, Text attrValue1)
        {
            CSVAttr.Data cSVAttrData = CSVAttr.Instance.GetConfData(attr1);
            TextHelper.SetText(attrName1, cSVAttrData.name);
            if (cSVAttrData.show_type == 1)
            {
                attrValue1.text = string.Format("+{0}", value1.ToString());
            }
            else
            {
                attrValue1.text = string.Format("+{0}%", (value1 / 100f).ToString());
            }
        }

        private void OnCeilCollectSelected(TitleCeil titleCeil)
        {
            curSelectTitle = titleCeil.title;
            curtitleSelectIndex = titleCeil.dataIndex;
            OnSelectTitle();
            OnRefreshTitleButtonShowState(curSelectTitle.Id);
            OnRefreshTitleButtonDressState(curSelectTitle.Id);
            UpdateTitle_CurSelect();

            bool showGo = false;
            if (curSelectTitle.isOnly)
            {
                showGo = curSelectTitle.cSVTitleData.titleGo[0] > 0 && !curSelectTitle.reachMaxLevel;
            }
            else
            {
                showGo = curSelectTitle.cSVTitleData.titleGo[0] > 0;
            }
            GoToButton.gameObject.SetActive(showGo);
        }

        private void OnSelectTitle()
        {
            foreach (var item in titleceils)
            {
                if (item.Value.dataIndex != curtitleSelectIndex)
                {
                    item.Value.Release();
                }
                else
                {
                    item.Value.Select();
                }
            }
        }

        public void UpdateTitle_CurSelect()
        {
            CSVTitle.Data cSVTitleData = curSelectTitle.cSVTitleData;
            if (cSVTitleData != null)
            {
                if (cSVTitleData.id == Sys_Title.Instance.familyTitle)
                {
                    if (cSVTitleData.titleShowIcon == 0)
                    {
                        SetTitleShowType_CurSelect(1);
                        TextHelper.SetText(mTitle_text1_curSelect, Sys_Title.Instance.GetTitleFamiltyName());
                        TextHelper.SetTextGradient(mTitle_text1_curSelect, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                        TextHelper.SetTextOutLine(mTitle_text1_curSelect, cSVTitleData.titleShow[2]);
                    }
                    else
                    {
                        SetTitleShowType_CurSelect(2);
                        TextHelper.SetText(mTitle_text2_curSelect, Sys_Title.Instance.GetTitleFamiltyName());
                        TextHelper.SetTextGradient(mTitle_text2_curSelect, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                        TextHelper.SetTextOutLine(mTitle_text2_curSelect, cSVTitleData.titleShow[2]);
                        ImageHelper.SetIcon(mTitle_img2_curSelect, cSVTitleData.titleShowIcon, true);
                    }
                }
                else if (cSVTitleData.id == Sys_Title.Instance.bGroupTitle)
                {
                    if (cSVTitleData.titleShowIcon == 0)
                    {
                        SetTitleShowType_CurSelect(1);
                        TextHelper.SetText(mTitle_text1_curSelect, Sys_Title.Instance.GetTitleWarriorGroupName());
                        TextHelper.SetTextGradient(mTitle_text1_curSelect, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                        TextHelper.SetTextOutLine(mTitle_text1_curSelect, cSVTitleData.titleShow[2]);
                    }
                    else
                    {
                        SetTitleShowType_CurSelect(2);
                        TextHelper.SetText(mTitle_text2_curSelect, Sys_Title.Instance.GetTitleWarriorGroupName());
                        TextHelper.SetTextGradient(mTitle_text2_curSelect, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                        TextHelper.SetTextOutLine(mTitle_text2_curSelect, cSVTitleData.titleShow[2]);
                        ImageHelper.SetIcon(mTitle_img2_curSelect, cSVTitleData.titleShowIcon, true);
                    }
                }
                else
                {
                    if (cSVTitleData.titleShowLan != 0)
                    {
                        if (cSVTitleData.titleShowIcon == 0)
                        {
                            SetTitleShowType_CurSelect(1);
                            TextHelper.SetText(mTitle_text1_curSelect, cSVTitleData.titleShowLan);
                            //TextHelper.SetTextGradient(mTitle_text1_curSelect, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                            //TextHelper.SetTextOutLine(mTitle_text1_curSelect, cSVTitleData.titleShow[2]);
                        }
                        else
                        {
                            SetTitleShowType_CurSelect(2);
                            TextHelper.SetText(mTitle_text2_curSelect, cSVTitleData.titleShowLan);
                            //TextHelper.SetTextGradient(mTitle_text2_curSelect, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                            //TextHelper.SetTextOutLine(mTitle_text2_curSelect, cSVTitleData.titleShow[2]);
                            ImageHelper.SetIcon(mTitle_img2_curSelect, cSVTitleData.titleShowIcon);
                        }
                    }
                    else
                    {
                        SetTitleShowType_CurSelect(3);
                        ImageHelper.SetIcon(mTitle_img3_curSelect, cSVTitleData.titleShowIcon);
                        uint FxId = cSVTitleData.titleShowEffect;
                        CSVSystemEffect.Data cSVSystemEffectData = CSVSystemEffect.Instance.GetConfData(FxId);
                        if (cSVSystemEffectData != null)
                        {
                            LoadTitleEffectAssetAsyn_CurSelect(cSVSystemEffectData.FxPath);
                        }
                    }
                }
            }
            UpdateCurSelectTitleCondition();
            ContructAttr();
            time.gameObject.SetActive(curSelectTitle.active && curSelectTitle.ExpireTime != 0);
            if (curSelectTitle.active && curSelectTitle.ExpireTime > 0)
            {
                time.text = curSelectTitle.titleEndTimer.GetMarkendTimeFormat();
            }
        }

        private void UpdateCurSelectTitleCondition()
        {
            if (curSelectTitle.Id == Sys_Title.Instance.familyTitle)
            {
                TextHelper.SetText(name, Sys_Title.Instance.GetTitleFamiltyName());
            }
            else if (curSelectTitle.Id == Sys_Title.Instance.bGroupTitle)
            {
                TextHelper.SetText(name, Sys_Title.Instance.GetTitleWarriorGroupName());
            }
            else
            {
                if (curSelectTitle.active && curSelectTitle.ExpireTime != 0)
                {
                    TextHelper.SetText(name, string.Format(CSVLanguage.Instance.GetConfData(2020743).words, CSVLanguage.Instance.GetConfData(curSelectTitle.cSVTitleData.titleLan).words));
                }
                else
                {
                    TextHelper.SetText(name, curSelectTitle.cSVTitleData.titleLan);
                }
            }

            point.text = curSelectTitle.cSVTitleData.titlePoint.ToString();

            if (curSelectTitle.isOnly)
            {
                if (!curSelectTitle.active || curSelectTitle.reachMaxLevel)
                {
                    m_NextConditionRoot.SetActive(false);
                }
                else
                {
                    m_NextConditionRoot.SetActive(true);
                }
            }
            else
            {
                m_NextConditionRoot.SetActive(false);
            }

            string conditionGet = CSVLanguage.Instance.GetConfData(curSelectTitle.cSVTitleData.titleGetLan).words;

            if (curSelectTitle.cSVTitleData.titleGetType == 1)//npc功能组
            {
                string str0 = Sys_Reputation.Instance.GetRankTitleByReputationLevel(curSelectTitle.cSVTitleData.titleGet[0]);
                string str1 = (curSelectTitle.cSVTitleData.titleGet[0] % 100).ToString();
                string str2 = CSVNpcLanguage.Instance.GetConfData(CSVNpc.Instance.GetConfData(curSelectTitle.cSVTitleData.titleGet[1]).name).words;
                TextHelper.SetText(condition, curSelectTitle.active ? 2007211u : 2007212u, string.Format(conditionGet, str0, str1, str2));

                if (curSelectTitle.active && !curSelectTitle.reachMaxLevel)
                {
                    uint nextTitle = curSelectTitle.Id + 1;
                    CSVTitle.Data nextTitleData = CSVTitle.Instance.GetConfData(nextTitle);

                    string nextConditionGet = CSVLanguage.Instance.GetConfData(nextTitleData.titleGetLan).words;
                    string str0_next = Sys_Reputation.Instance.GetRankTitleByReputationLevel(nextTitleData.titleGet[0]);
                    string str1_next = (nextTitleData.titleGet[0] % 100).ToString();
                    string str2_next = CSVNpcLanguage.Instance.GetConfData(CSVNpc.Instance.GetConfData(nextTitleData.titleGet[1]).name).words;
                    TextHelper.SetText(m_NextCondition, 2007212u, string.Format(nextConditionGet, str0_next, str1_next, str2_next));
                }
            }
            else if (curSelectTitle.cSVTitleData.titleGetType == 2)//成就
            {
                AchievementDataCell data = Sys_Achievement.Instance.GetAchievementByTid(curSelectTitle.cSVTitleData.titleGet[0]);
                TextHelper.SetText(condition, curSelectTitle.active ? 2007211u : 2007212u, string.Format(conditionGet, LanguageHelper.GetAchievementContent(data.csvAchievementData.Achievement_Title)));
                if (curSelectTitle.active && !curSelectTitle.reachMaxLevel)
                {
                    uint nextTitle = curSelectTitle.Id + 1;
                    CSVTitle.Data nextTitleData = CSVTitle.Instance.GetConfData(nextTitle);
                    AchievementDataCell data_next = Sys_Achievement.Instance.GetAchievementByTid(nextTitleData.titleGet[0]);
                    string nextName = LanguageHelper.GetTextContent(nextTitleData.titleGetLan, LanguageHelper.GetTextContent(data_next.csvAchievementData.Achievement_Title));
                    TextHelper.SetText(m_NextCondition, 2007212u, nextName);
                }
            }
            else if (curSelectTitle.cSVTitleData.titleGetType == 3)//任务
            {
                string str = CSVTaskLanguage.Instance.GetConfData(CSVTask.Instance.GetConfData(curSelectTitle.cSVTitleData.titleGet[0]).taskName).words;
                TextHelper.SetText(condition, curSelectTitle.active ? 2007211u : 2007212u, string.Format(conditionGet, str));

                if (curSelectTitle.active && !curSelectTitle.reachMaxLevel)
                {
                    uint nextTitle = curSelectTitle.Id + 1;
                    CSVTitle.Data nextTitleData = CSVTitle.Instance.GetConfData(nextTitle);

                    string nextConditionGet = CSVLanguage.Instance.GetConfData(nextTitleData.titleGetLan).words;
                    string str_next = CSVTaskLanguage.Instance.GetConfData(CSVTask.Instance.GetConfData(nextTitleData.titleGet[0]).taskName).words;
                    TextHelper.SetText(m_NextCondition, 2007212u, string.Format(nextConditionGet, str_next));
                }
            }
            else if (curSelectTitle.cSVTitleData.titleGetType == 4 || curSelectTitle.cSVTitleData.titleGetType == 5)//道具
            {
                string str = CSVLanguage.Instance.GetConfData(CSVItem.Instance.GetConfData(curSelectTitle.cSVTitleData.titleGet[0]).name_id).words;
                TextHelper.SetText(condition, curSelectTitle.active ? 2007211u : 2007212u, string.Format(conditionGet, str));

                if (curSelectTitle.active && !curSelectTitle.reachMaxLevel)
                {
                    uint nextTitle = curSelectTitle.Id + 1;
                    CSVTitle.Data nextTitleData = CSVTitle.Instance.GetConfData(nextTitle);

                    string nextConditionGet = CSVLanguage.Instance.GetConfData(nextTitleData.titleGetLan).words;
                    string str_next = CSVLanguage.Instance.GetConfData(CSVItem.Instance.GetConfData(nextTitleData.titleGet[0]).name_id).words;
                    TextHelper.SetText(m_NextCondition, 2007212u, string.Format(nextConditionGet, str_next));
                }
            }
            else if (curSelectTitle.cSVTitleData.titleGetType == 6)  //职业进阶
            {
                uint titleGet = curSelectTitle.cSVTitleData.titleGet[0];
                uint careerid = titleGet / 100;
                uint level = titleGet % 100;
                string str1 = CSVLanguage.Instance.GetConfData(CSVCareer.Instance.GetConfData(careerid).name).words;
                string str2 = level.ToString();
                TextHelper.SetText(condition, curSelectTitle.active ? 2007211u : 2007212u, string.Format(conditionGet, str1, str2));

                if (curSelectTitle.active && !curSelectTitle.reachMaxLevel)
                {
                    uint nextTitle = curSelectTitle.Id + 1;
                    CSVTitle.Data nextTitleData = CSVTitle.Instance.GetConfData(nextTitle);

                    string nextConditionGet = CSVLanguage.Instance.GetConfData(nextTitleData.titleGetLan).words;
                    uint titleGet_next = nextTitleData.titleGet[0];
                    uint careerid_next = titleGet_next / 100;
                    uint level_next = titleGet_next % 100;
                    string str1_next = CSVLanguage.Instance.GetConfData(CSVCareer.Instance.GetConfData(careerid_next).name).words;
                    string str2_next = level_next.ToString();
                    TextHelper.SetText(m_NextCondition, 2007212u, string.Format(nextConditionGet, str1_next, str2_next));
                }
            }
            else if (curSelectTitle.cSVTitleData.titleGetType == 7)//生活技能段位
            {
                TextHelper.SetText(condition, curSelectTitle.active ? 2007211u : 2007212u, string.Format(conditionGet, curSelectTitle.cSVTitleData.titleGet[0]));

                if (curSelectTitle.active && !curSelectTitle.reachMaxLevel)
                {
                    uint nextTitle = curSelectTitle.Id + 1;
                    CSVTitle.Data nextTitleData = CSVTitle.Instance.GetConfData(nextTitle);

                    string nextConditionGet = CSVLanguage.Instance.GetConfData(nextTitleData.titleGetLan).words;
                    TextHelper.SetText(m_NextCondition, 2007212u, string.Format(nextConditionGet, nextTitleData.titleGet[0]));
                }
            }
            else if (curSelectTitle.cSVTitleData.titleGetType == 8)   //生活技能等级
            {
                string skillLifeName = CSVLanguage.Instance.GetConfData(CSVLifeSkill.Instance.GetConfData(curSelectTitle.cSVTitleData.titleGet[0]).name_id).words;
                uint skillLifeLevel = curSelectTitle.cSVTitleData.titleGet[1];
                TextHelper.SetText(condition, curSelectTitle.active ? 2007211u : 2007212u, string.Format(conditionGet, skillLifeName, skillLifeLevel));

                if (curSelectTitle.active && !curSelectTitle.reachMaxLevel)
                {
                    uint nextTitle = curSelectTitle.Id + 1;
                    CSVTitle.Data nextTitleData = CSVTitle.Instance.GetConfData(nextTitle);

                    string nextConditionGet = CSVLanguage.Instance.GetConfData(nextTitleData.titleGetLan).words;
                    string skillLifeName_next = CSVLanguage.Instance.GetConfData(CSVLifeSkill.Instance.GetConfData(nextTitleData.titleGet[0]).name_id).words;
                    uint skillLifeLevel_next = nextTitleData.titleGet[1];
                    TextHelper.SetText(m_NextCondition, 2007212u, string.Format(nextConditionGet, skillLifeName_next, skillLifeLevel_next));
                }
            }
            else if (curSelectTitle.cSVTitleData.titleGetType == 9) //家族称号
            {
                TextHelper.SetText(condition, curSelectTitle.active ? 2007211u : 2007212u, conditionGet);
            }
            else if (curSelectTitle.cSVTitleData.titleGetType == 10) //导师等级
            {
                CSVTutor.Data data = CSVTutor.Instance.GetConfData(curSelectTitle.cSVTitleData.titleGet[0] + 1);
                string tutorName = CSVLanguage.Instance.GetConfData(data.TutorLevelLan).words;
                TextHelper.SetText(condition, curSelectTitle.active ? 2007211u : 2007212u, string.Format(conditionGet, tutorName));

                if (curSelectTitle.active && !curSelectTitle.reachMaxLevel)
                {
                    uint nextTitle = curSelectTitle.Id + 1;
                    CSVTitle.Data nextTitleData = CSVTitle.Instance.GetConfData(nextTitle);

                    string nextConditionGet = CSVLanguage.Instance.GetConfData(nextTitleData.titleGetLan).words;
                    CSVTutor.Data data_next = CSVTutor.Instance.GetConfData(nextTitleData.titleGet[0] + 1);
                    string tutorName_next = CSVLanguage.Instance.GetConfData(data_next.TutorLevelLan).words;
                    TextHelper.SetText(m_NextCondition, 2007212u, string.Format(nextConditionGet, tutorName_next));
                }
            }
            else if (curSelectTitle.cSVTitleData.titleGetType == 11)
            {
                uint reachLv = curSelectTitle.cSVTitleData.titleGet[0];
                TextHelper.SetText(condition, curSelectTitle.active ? 2007211u : 2007212u, string.Format(conditionGet, reachLv.ToString()));

                if (curSelectTitle.active && !curSelectTitle.reachMaxLevel)
                {
                    uint nextTitle = curSelectTitle.Id + 1;
                    CSVTitle.Data nextTitleData = CSVTitle.Instance.GetConfData(nextTitle);

                    string nextConditionGet = CSVLanguage.Instance.GetConfData(nextTitleData.titleGetLan).words;
                    uint reachLv_next = nextTitleData.titleGet[0];
                    TextHelper.SetText(m_NextCondition, 2007212u, string.Format(nextConditionGet, reachLv_next.ToString()));
                }
            }
            else if (curSelectTitle.cSVTitleData.titleGetType == 12) 
            {
                TextHelper.SetText(condition, curSelectTitle.active ? 2007211u : 2007212u, conditionGet);
            }
            else if (curSelectTitle.cSVTitleData.titleGetType == 13)
            {
                TextHelper.SetText(condition, curSelectTitle.active ? 2007211u : 2007212u, conditionGet);
            }
        }

        private void ContructAttr()
        {
            if (curSelectTitle.cSVTitleData.titleProperty == null)
            {
                attrParentCurSelect_root.gameObject.SetActive(false);
                attrParentCurSelect.gameObject.SetActive(false);
                return;
            }
            attrParentCurSelect_root.gameObject.SetActive(true);
            attrParentCurSelect.gameObject.SetActive(true);
            int attrCount = curSelectTitle.cSVTitleData.titleProperty.Count;
            FrameworkTool.CreateChildList(attrParentCurSelect, attrCount);
            FrameworkTool.ForceRebuildLayout(attrParentCurSelect.gameObject);
            for (int i = 0; i < attrCount; i++)
            {
                Transform child = attrParentCurSelect.GetChild(i);
                Text attrName = child.GetComponent<Text>();
                Text num = child.Find("Text").GetComponent<Text>();
                uint attrid = curSelectTitle.cSVTitleData.titleProperty[i][0];
                uint attrnum = curSelectTitle.cSVTitleData.titleProperty[i][1];
                SetAttr(attrid, attrnum, attrName, num);
            }
        }

        private void LoadTitleEffectAssetAsyn_CurSelect(string path)
        {
            AddressablesUtil.InstantiateAsync(ref requestRef_show, path, OnAssetsLoaded_CurSelect);
        }

        private void OnAssetsLoaded_CurSelect(AsyncOperationHandle<GameObject> handle)
        {
            titleEffect_show = handle.Result;
            if (null != titleEffect_show)
            {
                titleEffect_show.transform.SetParent(mTitle_Fx3parent_show);
                RectTransform rectTransform = titleEffect_show.transform as RectTransform;
                rectTransform.localPosition = Vector3.zero;
                rectTransform.localEulerAngles = Vector3.zero;
                rectTransform.localScale = Vector3.one;
            }
        }

        private void SetTitleShowType_CurSelect(int type)
        {
            if (type == 1)
            {
                mTitle_text1_curSelect.gameObject.SetActive(true);
                mTitle_text2_curSelect.gameObject.SetActive(false);
                mTitle_img2_curSelect.gameObject.SetActive(false);
                mTitle_img3_curSelect.gameObject.SetActive(false);
                mTitle_Fx3parent_curSelect.gameObject.SetActive(false);
            }
            else if (type == 2)
            {
                mTitle_text1_curSelect.gameObject.SetActive(false);
                mTitle_text2_curSelect.gameObject.SetActive(true);
                mTitle_img2_curSelect.gameObject.SetActive(true);
                mTitle_img3_curSelect.gameObject.SetActive(false);
                mTitle_Fx3parent_curSelect.gameObject.SetActive(false);
            }
            else
            {
                mTitle_text1_curSelect.gameObject.SetActive(false);
                mTitle_text2_curSelect.gameObject.SetActive(false);
                mTitle_img2_curSelect.gameObject.SetActive(false);
                mTitle_img3_curSelect.gameObject.SetActive(true);
                mTitle_Fx3parent_curSelect.gameObject.SetActive(true);
            }
        }

        #region ModelShow
        private void OnCreateModel()
        {
            _LoadShowScene();
            _LoadShowModel((uint)GameCenter.mainHero.careerComponent.CurCarrerType);
            heroLoader.LoadWeaponPart(Sys_Fashion.Instance.GetCurDressedFashionWeapon(), Sys_Equip.Instance.GetCurWeapon());
        }

        private void _LoadShowScene()
        {
            if (showSceneControl == null)
            {
                showSceneControl = new ShowSceneControl();
            }

            GameObject sceneModel = GameObject.Instantiate<GameObject>(assetDependencies.mCustomDependencies[0] as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            showSceneControl.Parse(sceneModel);

            //rawImage.texture = showSceneControl.GetTemporary(0, 0, 6, RenderTextureFormat.ARGB32, 1f);
        }
        private void _LoadShowModel(uint careerid)
        {
            if (heroLoader == null)
            {
                heroLoader = HeroLoader.Create(true);
                heroLoader.heroDisplay.onLoaded += OnShowModelLoaded;
            }

            heroLoader.LoadHero(Sys_Role.Instance.Role.HeroId, GameCenter.mainHero.weaponComponent.CurWeaponID, ELayerMask.ModelShow, Sys_Fashion.Instance.GetDressData(), (go) =>
            {
                heroLoader.heroDisplay.GetPart(EHeroModelParts.Main).SetParent(showSceneControl.mModelPos, null);
            });

        }

        private void _UnloadShowContent()
        {
            _uiModelShowManagerEntity?.Dispose();
            _uiModelShowManagerEntity = null;
            //rawImage.texture = null;
            heroLoader?.Dispose();
            heroLoader = null;
            showSceneControl?.Dispose();
        }

        private void OnShowModelLoaded(int obj)
        {
            if (obj == 0)
            {
                _uiModelShowManagerEntity?.Dispose();
                _uiModelShowManagerEntity = null;
                CSVCareer.Data cSVCareerData = CSVCareer.Instance.GetConfData((uint)GameCenter.mainHero.careerComponent.CurCarrerType);
                uint highId = Hero.GetMainHeroHighModelAnimationID();
                heroLoader.heroDisplay.mAnimation.UpdateHoldingAnimations(highId, Sys_Equip.Instance.GetCurWeapon());

                GameObject mainGo = heroLoader.heroDisplay.GetPart(EHeroModelParts.Main).gameObject;
                mainGo.SetActive(false);
                _uiModelShowManagerEntity = WS_UIModelShowManagerEntity.Start(500000, null, heroLoader.heroDisplay.mAnimation, mainGo);
            }
        }

        public void OnDrag(BaseEventData eventData)
        {
            PointerEventData ped = eventData as PointerEventData;
            Vector3 angle = new Vector3(0f, ped.delta.x * -0.36f, 0f);
            AddEulerAngles(angle);
        }

        public void AddEulerAngles(Vector3 angle)
        {
            if (showSceneControl.mModelPos.transform != null)
            {
                Vector3 localAngle = showSceneControl.mModelPos.transform.localEulerAngles;
                localAngle.Set(localAngle.x + angle.x, localAngle.y + angle.y, localAngle.z + angle.z);
                showSceneControl.mModelPos.transform.localEulerAngles = localAngle;
            }
        }

        #endregion

        public class TitleCeil
        {
            public Title title;
            private Transform transform;
            private GameObject select;
            private Image eventBg;
            private GameObject curShowTitle;
            private GameObject curUseTitle;
            private GameObject unGet;
            //private Transform attrParent;
            //private Text name;
            private Text point;
            //private Text condition;
            //private Text time;
            public int dataIndex;
            private Action<TitleCeil> onClick;
            private Action<TitleCeil> onlongPressed;

            private Text mTitle_text1;

            private Text mTitle_text2;
            private Image mTitle_img2;

            private Image mTitle_img3;
            private Transform mTitle_Fx3parent;

            AsyncOperationHandle<GameObject> requestRef;
            private GameObject titleEffect;

            private string conditionGet;


            public void BindGameObject(GameObject go)
            {
                transform = go.transform;
                ParseComponent();
            }

            private void ParseComponent()
            {
                //name = transform.Find("Image_BG/Text_Name").GetComponent<Text>();
                point = transform.Find("Image_BG/Image_Point/Text_Point").GetComponent<Text>();
                //condition = transform.Find("Text_Condition").GetComponent<Text>();
                //time = transform.Find("Text_Condition/Text_Time").GetComponent<Text>();
                select = transform.Find("Image_Select").gameObject;
                curShowTitle = transform.Find("Image_BG/Text_Name/Grid/Image_Show").gameObject;
                curUseTitle = transform.Find("Image_BG/Text_Name/Grid/Image_Equip").gameObject;
                unGet = transform.Find("Image_None").gameObject;
                //attrParent = transform.Find("Grid");

                mTitle_text1 = transform.Find("Image_TitleBG/Title/Text").GetComponent<Text>();
                mTitle_text2 = transform.Find("Image_TitleBG/Title/Image1/Text").GetComponent<Text>();
                mTitle_img2 = transform.Find("Image_TitleBG/Title/Image1").GetComponent<Image>();
                mTitle_img3 = transform.Find("Image_TitleBG/Title/Image2").GetComponent<Image>();
                mTitle_Fx3parent = transform.Find("Image_TitleBG/Title/Image2/Fx");

                eventBg = transform.Find("eventBg").GetComponent<Image>();
                Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(eventBg);
                eventListener.AddEventListener(EventTriggerType.PointerClick, OnGridClicked);
            }

            public void AddClickListener(Action<TitleCeil> _onClick, Action<TitleCeil> onlongPressed = null)
            {
                onClick = _onClick;
                if (onlongPressed != null)
                {
                    this.onlongPressed = onlongPressed;
                    UI_LongPressButton uI_LongPressButton = eventBg.gameObject.AddComponent<UI_LongPressButton>();
                    uI_LongPressButton.onStartPress.AddListener(OnLongPressed);
                }
            }

            private void OnGridClicked(BaseEventData baseEventData)
            {
                onClick.Invoke(this);
            }

            private void OnLongPressed()
            {
                onlongPressed.Invoke(this);
            }

            public void SetData(Title _title, int _dataIndex)
            {
                this.title = _title;
                this.dataIndex = _dataIndex;
                Refresh();
            }

            public void Refresh()
            {
                //if (title.active && title.ExpireTime != 0)
                //{
                //    TextHelper.SetText(name, string.Format(CSVLanguage.Instance.GetConfData(2020743).words, CSVLanguage.Instance.GetConfData(title.cSVTitleData.titleLan).words));
                //}
                //else
                //{
                //    TextHelper.SetText(name, title.cSVTitleData.titleLan);
                //}
                curShowTitle.SetActive(title.Show);
                curUseTitle.SetActive(title.Dress);
                unGet.SetActive(!title.active);
                point.text = title.cSVTitleData.titlePoint.ToString();
                //conditionGet = CSVLanguage.Instance.GetConfData(title.cSVTitleData.titleGetLan).words;

                //if (title.cSVTitleData.titleGetType == 1)//npc功能组
                //{
                //    string str0 = Sys_Reputation.Instance.GetRankTitleByReputationLevel(title.cSVTitleData.titleGet[0]);
                //    string str1 = (title.cSVTitleData.titleGet[0] % 100).ToString();
                //    string str2 = CSVLanguage.Instance.GetConfData(CSVNpc.Instance.GetConfData(title.cSVTitleData.titleGet[1]).name).words;
                //    TextHelper.SetText(condition, title.active ? 2007211u : 2007212u, string.Format(conditionGet, str0, str1, str2));
                //}
                //else if (title.cSVTitleData.titleGetType == 2)//成就
                //{

                //}
                //else if (title.cSVTitleData.titleGetType == 3)//任务
                //{
                //    string str = CSVTaskLanguage.Instance.GetConfData(CSVTask.Instance.GetConfData(title.cSVTitleData.titleGet[0]).taskName).words;
                //    TextHelper.SetText(condition, title.active ? 2007211u : 2007212u, string.Format(conditionGet, str));
                //}
                //else if (title.cSVTitleData.titleGetType == 4 || title.cSVTitleData.titleGetType == 5)//道具
                //{
                //    string str = CSVLanguage.Instance.GetConfData(CSVItem.Instance.GetConfData(title.cSVTitleData.titleGet[0]).name_id).words;
                //    TextHelper.SetText(condition, title.active ? 2007211u : 2007212u, string.Format(conditionGet, str));
                //}
                //else if (title.cSVTitleData.titleGetType == 6)  //职业进阶
                //{
                //    uint titleGet = title.cSVTitleData.titleGet[0];
                //    uint careerid = titleGet / 100;
                //    uint level = titleGet % 100;
                //    string str1 = CSVLanguage.Instance.GetConfData(CSVCareer.Instance.GetConfData(careerid).name).words;
                //    string str2 = level.ToString();
                //    TextHelper.SetText(condition, title.active ? 2007211u : 2007212u, string.Format(conditionGet, str1, str2));
                //}
                //else if (title.cSVTitleData.titleGetType == 7)//生活技能段位
                //{
                //    TextHelper.SetText(condition, title.active ? 2007211u : 2007212u, string.Format(conditionGet, title.cSVTitleData.titleGet[0]));
                //}
                //else    //生活技能等级
                //{
                //    string skillLifeName = CSVLanguage.Instance.GetConfData(CSVLifeSkill.Instance.GetConfData(title.cSVTitleData.titleGet[0]).name_id).words;
                //    uint skillLifeLevel = title.cSVTitleData.titleGet[1];
                //    TextHelper.SetText(condition, title.active ? 2007211u : 2007212u, string.Format(conditionGet, skillLifeName, skillLifeLevel));
                //}

                //time.gameObject.SetActive(title.active && title.ExpireTime != 0);
                //ContructAttr();
                UpdateTitle();
            }

            private void ContructAttr()
            {
                //if (title.cSVTitleData.titleProperty == null)
                //{
                //    attrParent.gameObject.SetActive(false);
                //    return;
                //}
                //attrParent.gameObject.SetActive(true);
                //int attrCount = title.cSVTitleData.titleProperty.Count;
                //FrameworkTool.CreateChildList(attrParent, attrCount);
                //FrameworkTool.ForceRebuildLayout(attrParent.gameObject);
                //for (int i = 0; i < attrCount; i++)
                //{
                //    Transform child = attrParent.GetChild(i);
                //    Text attrName = child.GetComponent<Text>();
                //    Text num = child.Find("Text").GetComponent<Text>();
                //    uint attrid = title.cSVTitleData.titleProperty[i][0];
                //    uint attrnum = title.cSVTitleData.titleProperty[i][1];
                //    SetAttr(attrid, attrnum, attrName, num);
                //}
            }

            private void SetAttr(uint attr1, uint value1, Text attrName1, Text attrValue1)
            {
                CSVAttr.Data cSVAttrData = CSVAttr.Instance.GetConfData(attr1);
                TextHelper.SetText(attrName1, cSVAttrData.name);
                if (cSVAttrData.show_type == 1)
                {
                    attrValue1.text = string.Format("+{0}", value1);
                }
                else
                {
                    attrValue1.text = string.Format("+{0}%", value1 / 100f);
                }
            }

            public void Update()
            {
                if (title == null)
                    return;
                //if (title.active && title.ExpireTime > 0)
                //{
                //    time.text = title.titleEndTimer.GetMarkendTimeFormat();
                //}
            }

            public void Release()
            {
                select.SetActive(false);
            }

            public void Select()
            {
                select.SetActive(true);
            }

            public void UpdateTitle()
            {
                CSVTitle.Data cSVTitleData = title.cSVTitleData;
                if (cSVTitleData != null)
                {
                    if (cSVTitleData.id == Sys_Title.Instance.familyTitle)
                    {
                        if (cSVTitleData.titleShowIcon == 0)
                        {
                            SetTitleShowType(1);
                            TextHelper.SetText(mTitle_text1, Sys_Title.Instance.GetTitleFamiltyName());
                            TextHelper.SetTextGradient(mTitle_text1, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                            TextHelper.SetTextOutLine(mTitle_text1, cSVTitleData.titleShow[2]);
                        }
                        else
                        {
                            SetTitleShowType(2);
                            TextHelper.SetText(mTitle_text2, Sys_Title.Instance.GetTitleFamiltyName());
                            TextHelper.SetTextGradient(mTitle_text2, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                            TextHelper.SetTextOutLine(mTitle_text2, cSVTitleData.titleShow[2]);
                            ImageHelper.SetIcon(mTitle_img2, cSVTitleData.titleShowIcon, true);
                        }
                    }
                    else if (cSVTitleData.id == Sys_Title.Instance.bGroupTitle)
                    {
                        if (cSVTitleData.titleShowIcon == 0)
                        {
                            SetTitleShowType(1);
                            TextHelper.SetText(mTitle_text1, Sys_Title.Instance.GetTitleWarriorGroupName());
                            TextHelper.SetTextGradient(mTitle_text1, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                            TextHelper.SetTextOutLine(mTitle_text1, cSVTitleData.titleShow[2]);
                        }
                        else
                        {
                            SetTitleShowType(2);
                            TextHelper.SetText(mTitle_text2, Sys_Title.Instance.GetTitleWarriorGroupName());
                            TextHelper.SetTextGradient(mTitle_text2, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                            TextHelper.SetTextOutLine(mTitle_text2, cSVTitleData.titleShow[2]);
                            ImageHelper.SetIcon(mTitle_img2, cSVTitleData.titleShowIcon, true);
                        }
                    }
                    else
                    {
                        if (cSVTitleData.titleShowLan != 0)
                        {
                            if (cSVTitleData.titleShowIcon == 0)
                            {
                                SetTitleShowType(1);
                                TextHelper.SetText(mTitle_text1, cSVTitleData.titleShowLan);
                                //TextHelper.SetTextGradient(mTitle_text1, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                                //TextHelper.SetTextOutLine(mTitle_text1, cSVTitleData.titleShow[2]);
                            }
                            else
                            {
                                SetTitleShowType(2);
                                TextHelper.SetText(mTitle_text2, cSVTitleData.titleShowLan);
                                //TextHelper.SetTextGradient(mTitle_text2, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                                //TextHelper.SetTextOutLine(mTitle_text2, cSVTitleData.titleShow[2]);
                                ImageHelper.SetIcon(mTitle_img2, cSVTitleData.titleShowIcon);
                            }
                        }
                        else
                        {
                            SetTitleShowType(3);
                            ImageHelper.SetIcon(mTitle_img3, cSVTitleData.titleShowIcon);
                            uint FxId = cSVTitleData.titleShowEffect;
                            CSVSystemEffect.Data cSVSystemEffectData = CSVSystemEffect.Instance.GetConfData(FxId);
                            if (cSVSystemEffectData != null)
                            {
                                LoadTitleEffectAssetAsyn(cSVSystemEffectData.FxPath);
                            }
                        }
                    }

                }
            }

            private void LoadTitleEffectAssetAsyn(string path)
            {
                AddressablesUtil.InstantiateAsync(ref requestRef, path, OnAssetsLoaded);
            }

            private void OnAssetsLoaded(AsyncOperationHandle<GameObject> handle)
            {
                titleEffect = handle.Result;
                if (null != titleEffect)
                {
                    titleEffect.transform.SetParent(mTitle_Fx3parent);
                    RectTransform rectTransform = titleEffect.transform as RectTransform;
                    rectTransform.localPosition = Vector3.zero;
                    rectTransform.localEulerAngles = Vector3.zero;
                    rectTransform.localScale = Vector3.one;
                }
            }

            private void SetTitleShowType(int type)
            {
                if (type == 1)
                {
                    mTitle_text1.gameObject.SetActive(true);
                    mTitle_text2.gameObject.SetActive(false);
                    mTitle_img2.gameObject.SetActive(false);
                    mTitle_img3.gameObject.SetActive(false);
                    mTitle_Fx3parent.gameObject.SetActive(false);
                }
                else if (type == 2)
                {
                    mTitle_text1.gameObject.SetActive(false);
                    mTitle_text2.gameObject.SetActive(true);
                    mTitle_img2.gameObject.SetActive(true);
                    mTitle_img3.gameObject.SetActive(false);
                    mTitle_Fx3parent.gameObject.SetActive(false);
                }
                else
                {
                    mTitle_text1.gameObject.SetActive(false);
                    mTitle_text2.gameObject.SetActive(false);
                    mTitle_img2.gameObject.SetActive(false);
                    mTitle_img3.gameObject.SetActive(true);
                    mTitle_Fx3parent.gameObject.SetActive(true);
                }
            }

            public void OnDispose()
            {
                AddressablesUtil.ReleaseInstance(ref requestRef, OnAssetsLoaded);
            }
        }
    }
}


