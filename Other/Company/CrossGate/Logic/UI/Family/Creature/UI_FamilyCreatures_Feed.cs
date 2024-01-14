using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
using Framework;

namespace Logic
{
    public class UI_FamilyCreatures_Feed_Layout
    {
        private Button closeBtn;
        private Button priviewBtn;
        private Button noticeBtn;
        private Button ruleBtn;
        private GameObject noticeRedPointGo;
        private Transform leftTopTran;
        private Transform infoViewTran;
        private Transform giveViewTran;
        public UI_FamilyCreatureSlider allSlider;
        public UI_FamilyCreatureSlider todaySlider;

        public Transform LeftTopTran { get => leftTopTran; }
        public Transform InfoViewTran { get => infoViewTran;}
        public Transform GiveViewTran { get => giveViewTran; }

        public void Init(Transform transform)
        {
            closeBtn = transform.Find("Animator/View_FullTitle01_New/Btn_Close").GetComponent<Button>();
            priviewBtn = transform.Find("Animator/Btn_Form").GetComponent<Button>();
            noticeBtn = transform.Find("Animator/Btn_Notice").GetComponent<Button>();
            ruleBtn = transform.Find("Animator/View_FullTitle01_New/GrowthValue_Today/Button_Help").GetComponent<Button>();
            leftTopTran = transform.Find("Animator/Image_Title");
            infoViewTran = transform.Find("Animator/View_Info");
            giveViewTran = transform.Find("Animator/View_Give");
            allSlider = new UI_FamilyCreatureSlider();
            allSlider.InitTransform(transform.Find("Animator/View_FullTitle01_New/GrowthValue_Total"));
            todaySlider = new UI_FamilyCreatureSlider();
            todaySlider.InitTransform(transform.Find("Animator/View_FullTitle01_New/GrowthValue_Today"));
            noticeRedPointGo = transform.Find("Animator/Btn_Notice/Image_Prompt").gameObject;
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.CloseBtnClicked);
            priviewBtn.onClick.AddListener(listener.PriviewBtnClicked);
            noticeBtn.onClick.AddListener(listener.NotoceBtnClicked);
            ruleBtn.onClick.AddListener(listener.RuleBtnClicked);
        }

        public void SetNoticeRedPoint()
        {
            noticeRedPointGo.SetActive(Sys_Family.Instance.IsShowNoticeRedPoint());
        }

        public interface IListener
        {
            void CloseBtnClicked();
            void PriviewBtnClicked();
            void NotoceBtnClicked();
            void RuleBtnClicked();
        }
    }

    public class UI_FamilyCreaturesFeedInfo : UIComponent
    {
        private UI_FamilyCreatureMoodSlider moodSlider;
        private UI_FamilyCreatureHealthSlider healthSlider;
        protected override void Loaded()
        {

            moodSlider = new UI_FamilyCreatureMoodSlider();
            moodSlider.InitTransform(transform.Find("Mood"));

            healthSlider = new UI_FamilyCreatureHealthSlider();
            healthSlider.InitTransform(transform.Find("Health"));
        }

        public void SetData(FamilyCreatureEntry creatureEntry)
        {
            if(null != creatureEntry)
            {
                moodSlider.SetSliderValue(creatureEntry, 0);
                healthSlider.SetSliderValue(creatureEntry, 0);
            }
        }
    }

    public class UI_FamilyCreaturesFeedGive : UIComponent
    { 
        /// <summary> 无限滚动 </summary>
        private InfinityGrid infinityGrid;
        private Transform hobbyTransform;
        //家族兽名字
        private Text nameText;
        //可喂食时间
        private Text timeText;
        //睡眠中提示
        private GameObject sleepGo;
        //喂食次数
        private Text residueDegreeText;
        //喂食暴击次数
        private Text criticalDegreeText;
        //成长值
        private Text growthText;
        //心情值 or 健康值 互斥显示
        private Text moodOrHealthValueText;
        private Text moodOrHealthText;
        //经验值
        private Text expText;
        private Button detailsBtn;
        private Button details2Btn;
        private Button feedBtn;
        private GameObject tabProto;
        private GameObject feedRatioGo;
        public UIElementCollector<FamilyCreaturesTab> tabVds = new UIElementCollector<FamilyCreaturesTab>();
        public class FamilyCreaturesTab : UISelectableElement
        {
            public CP_Toggle toggle;
            public Text tabNameLight;
            public Text tabNameDark;
            public GameObject likeObj;
            public int tabId = 0;

            protected override void Loaded()
            {
                this.toggle = this.transform.GetComponent<CP_Toggle>();
                this.toggle.onValueChanged.AddListener(this.Switch);

                this.tabNameLight = this.transform.Find("Image_Menu_Light/Text_Menu_Light").GetComponent<Text>();
                this.tabNameDark = this.transform.Find("Btn_Menu_Dark/Text_Menu_Dark").GetComponent<Text>();
                this.likeObj = this.transform.Find("Image").gameObject;
            }

            public void Refresh(int typeid, int currentType)
            {
                this.tabId = typeid;
                uint langId = 2010605;
                switch (typeid)
                {
                    case 1:
                        langId = 2023512;
                        break;
                    case 2:
                        langId = 2023513;
                        break;
                    case 3:
                        langId = 2023511;
                        break;
                    case 4:
                        langId = 2023514;
                        break;
                }
                TextHelper.SetText(this.tabNameLight, langId);
                TextHelper.SetText(this.tabNameDark, langId);
                SetLike(currentType);
            }

            public void SetLike(int type)
            {
                this.likeObj.SetActive(type == tabId);
            }

            public void Switch(bool arg)
            {
                if (arg)
                {
                    this.onSelected?.Invoke(this.tabId, true);
                }
            }

            public override void SetSelected(bool toSelected, bool force)
            {
                this.toggle.SetSelected(toSelected, true);
            }
        }

        public List<PropIconLoader.ShowItemDataExt> cellDatas = new List<PropIconLoader.ShowItemDataExt>();
        private int currentTabId;
        private int selectedGiftPropItemIndex = -1;
        private FamilyCreatureEntry familyCreatureEntry;
        public MessageBoxEvt boxEvent = new MessageBoxEvt();
        private Timer timeShow;
        protected override void Loaded()
        {
            tabProto = transform.Find("Menu/Water").gameObject;

            detailsBtn = transform.Find("Btn_Details_01").GetComponent<Button>();
            detailsBtn.onClick.AddListener(DetailsBtnClicked);

            details2Btn = transform.Find("Btn_Details_02").GetComponent<Button>();
            details2Btn.onClick.AddListener(Details2BtnClicked);

            feedBtn = transform.Find("Btn_Feed").GetComponent<Button>();
            feedBtn.onClick.AddListener(FeedBtnClicked);
            hobbyTransform = transform.Find("Object_Type/Scroll_View/Viewport");
            nameText = transform.Find("View_Info/Text_Name").GetComponent<Text>();
            timeText = transform.Find("View_Info/Text_RemainingTime/Text_Value").GetComponent<Text>();
            sleepGo = transform.Find("View_Info/Text_Sleep").gameObject;

            residueDegreeText = transform.Find("Text_ResidueDegree").GetComponent<Text>();
            criticalDegreeText = transform.Find("Text_CriticalDegree").GetComponent<Text>();
            growthText =transform.Find("GrowthValue/Value").GetComponent<Text>();
            moodOrHealthText = transform.Find("MoodValue/Title").GetComponent<Text>();
            moodOrHealthValueText = transform.Find("MoodValue/Value").GetComponent<Text>();
            expText = transform.Find("ExpValue/Value/Value").GetComponent<Text>();
            feedRatioGo = transform.Find("ExpValue/Value/Critical").gameObject;
            transform.Find("ExpValue/Value/Critical/Text").GetComponent<Text>().text = "x" + Sys_Family.Instance.FeedExpRatio.ToString();
            infinityGrid = transform.Find("Scroll View").GetComponent<InfinityGrid>();
            infinityGrid.onCreateCell += OnCreateCell;
            infinityGrid.onCellChange += OnCellChange;
        }

        public void SetData(FamilyCreatureEntry creatureEntry)
        {
            //selectedGiftPropItemIndex = -1;
            familyCreatureEntry = creatureEntry;
            if (null != familyCreatureEntry)
            {
                RefreshPreView();
                RefreshAll();
                RefreshLikes();
                timeShow?.Cancel();
                nameText.text = familyCreatureEntry.Name;
                timeShow = Timer.Register(1, null,
                    (t) =>
                    {
                        if(Sys_Family.Instance.FamilyCreatureIsSleep())
                        {
                            sleepGo.SetActive(true);
                            timeText.transform.parent.gameObject.SetActive(false);
                            timeText.text = "";
                        }
                        else
                        {
                            sleepGo.SetActive(false);
                            timeText.transform.parent.gameObject.SetActive(true);
                            timeText.text = Sys_Family.Instance.GetFeedTimerStr();
                        }
                    }, true);
            }
            RefreshCount();
        }

        public override void Hide()
        {
            timeShow?.Cancel();
        }

        public void RefreshCount()
        {
            residueDegreeText.text = LanguageHelper.GetTextContent(2023611, Sys_Family.Instance.feedCount.ToString());
            criticalDegreeText.text = LanguageHelper.GetTextContent(2023612, Sys_Family.Instance.hungerCount.ToString());
        }

        private void DetailsBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_FamilyCreatures_Feed, "DetailsBtnClicked");
            UIManager.OpenUI(EUIID.UI_Rule, false, new UIRuleParam { StrContent = LanguageHelper.GetTextContent(2023651) });
        }

        private void Details2BtnClicked()
        {
            UIManager.HitButton(EUIID.UI_FamilyCreatures_Feed, "Details2BtnClicked");
            Sys_Ini.Instance.Get<IniElement_Int>(1230, out IniElement_Int addFeedCount);
            Sys_Ini.Instance.Get<IniElement_Int>(1231, out IniElement_Int addHungerCount);
            UIManager.OpenUI(EUIID.UI_Rule, false, new UIRuleParam { StrContent = LanguageHelper.GetTextContent(2023652, addFeedCount.value.ToString(), addHungerCount.value.ToString(), (7 * addFeedCount.value).ToString(), (7 * addHungerCount.value).ToString()) });
        }

        private void FeedBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_FamilyCreatures_Feed, "FeedBtnClicked");
            // 喂食
            if (selectedGiftPropItemIndex == -1)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010609));
                return;
            }
            else if (Sys_Family.Instance.feedCount <= 0) // 剩余次数不足
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010610));
                return;
            }
            else if(Sys_Family.Instance.FamilyCreatureIsSleep())
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2023630));
                return;
            }

            var sendItem = cellDatas[selectedGiftPropItemIndex];
            CSVFamilyPetFood.Data cSVFamilyPetFoodData = CSVFamilyPetFood.Instance.GetConfData(sendItem.id);
            if(null != cSVFamilyPetFoodData)
            {
                if (cSVFamilyPetFoodData.num > sendItem.count)//验证道具数量
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010613));
                }
                else
                {
                    var cellData = this.cellDatas[this.selectedGiftPropItemIndex];
                    PropIconLoader.ShowItemDataExt ext = cellData as PropIconLoader.ShowItemDataExt;
                    if (ext != null)
                    {
                        Sys_Family.Instance.GuildPetFeedReq(familyCreatureEntry.FmilyCreatureId, ext.id);
                    }
                }
            }
        }

        /// <summary>
        /// 滚动列表创建回调
        /// </summary>
        /// <param name="cell"></param>
        public void OnCreateCell(InfinityGridCell cell)
        {
            PropItem entry = new PropItem();
            var go = cell.mRootTransform.gameObject;
            entry.BindGameObject(go);
            cell.BindUserData(entry);
        }

        /// <summary>
        /// 滚动列表滚动回调
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        public void OnCellChange(InfinityGridCell cell, int index)
        {
            var entry = cell.mUserData as PropItem;
            this.UpdateChildrenCallback(index, entry, cell.mRootTransform);
        }

        public void UpdateChildrenCallback(int index, PropItem entry, Transform trans)
        {
            if (0 <= index && index < this.cellDatas.Count)
            {
                var cellData = this.cellDatas[index];
                if(cellData.id == 0)
                {
                    entry.SetData(cellData, EUIID.UI_FamilyCreatures_Feed);
                    entry.SetEmpty();
                    
                    cellData.bUseClick = true;
                    entry.btnNone.gameObject.SetActive(true);
                }
                else
                {
                    cellData.bUseClick = true;
                    cellData.bUseTips = false;
                    cellData.bUseQuailty = true;
                    entry.SetData(cellData, EUIID.UI_FamilyCreatures_Feed);
                    CSVFamilyPetFood.Data cSVFamilyPetFoodData = CSVFamilyPetFood.Instance.GetConfData(cellData.id);
                    entry.imgHealth.SetActive(null != cSVFamilyPetFoodData && cSVFamilyPetFoodData.isDrugs == 1);
                    entry.imgLike.SetActive(this.familyCreatureEntry.creature.LoveFoods.Contains(cellData.id));
                    entry.SetSelected(index == this.selectedGiftPropItemIndex);
                    // 刷新个数
                    entry.RefreshLRCount(cellData.count, cellData.neededCount);
                    if(index == selectedGiftPropItemIndex)
                    {
                        selectedGiftPropItem = entry;
                    }
                    
                }
                

                cellData.onclick = (propItem) => {
                    int idx = index;
                    this.OnCellClicked(propItem, idx);
                };
            }
            else
            {
                entry.SetEmpty();
            }
        }

        private PropItem selectedGiftPropItem = null;
        private void OnCellClicked(PropItem propItem, int index)
        {
            if(this.cellDatas[index].id == 0)
            {
                MallPrama mall = new MallPrama();
                mall.mallId = 1101;
                mall.shopId = 11011;
                UIManager.OpenUI(EUIID.UI_Mall, false, mall);
            }
            else
            {
                this.selectedGiftPropItem?.SetSelected(false);
                this.selectedGiftPropItemIndex = index;

                this.boxEvent.Reset(EUIID.UI_FamilyCreatures_Feed, propItem.ItemData);

                PropMessageParam propParam = new PropMessageParam();
                propParam.itemData = this.cellDatas[index].bagData;
                propParam.needShowInfo = false;
                propParam.needShowMarket = false;
                propParam.sourceUiId = EUIID.UI_FamilyCreatures_Feed;
                UIManager.OpenUI(EUIID.UI_Prop_Message, false, propParam);

                propItem.SetSelected(true);
                this.selectedGiftPropItem = propItem;

                this.RefreshPreView();
            }
        }

        private void RefreshPreView()
        {
            if(selectedGiftPropItemIndex == -1)
            {
                ClearItemEffect();
            }
            else
            {
                if (0 <= selectedGiftPropItemIndex && selectedGiftPropItemIndex < this.cellDatas.Count - 1)
                {
                    CSVFamilyPetFood.Data cSVFamilyPetFoodData = CSVFamilyPetFood.Instance.GetConfData(this.cellDatas[selectedGiftPropItemIndex].id);
                    if(null != cSVFamilyPetFoodData)
                    {
                        bool isDrugs = cSVFamilyPetFoodData.isDrugs == 1;
                        bool isSameType = cSVFamilyPetFoodData.food_Type == familyCreatureEntry.cSV.food_Type;
                        bool isLove = familyCreatureEntry.creature.LoveFoods.Contains(this.cellDatas[selectedGiftPropItemIndex].id);
                        moodOrHealthText.text = LanguageHelper.GetTextContent(isDrugs? 2023616u : 2023609u);
                        //增加成长值 = 食物固定成长值 * 心情喂食成长值系数 * 健康喂食成长值系数 * 家族兽成长值匹配属性加成系数
                        var growthValue = Math.Round((cSVFamilyPetFoodData.addGrowthValue * familyCreatureEntry.CurrentMoodRatio * familyCreatureEntry.CurrentHealthRatio
                             * (isSameType ? Sys_Family.Instance.GrothTypeRatio : 1f)));
                        Sys_Family.Instance.SetTextData(growthText, (float)growthValue, cSVFamilyPetFoodData.addGrowthValue);
                        //药品只增加成长值和健康值
                        if (isDrugs)
                        {
                            //增加健康值 = 食物固定健康值 * 家族兽健康值匹配属性加成系数
                            var healthValue = Math.Round((cSVFamilyPetFoodData.addHealthValue * (isSameType ? Sys_Family.Instance.HealthTypeRatio : 1f)));
                            Sys_Family.Instance.SetTextData(moodOrHealthValueText, (float)healthValue, cSVFamilyPetFoodData.addHealthValue);
                        }
                        else
                        {
                            //除了药品都增加心情值
                            //增加心情值=食物固定心情值*家族兽心情值匹配属性加成系数*家族兽心情值当前喜好品加成系数
                            var moodValue = Math.Round((cSVFamilyPetFoodData.addMoodValue * (isSameType ? Sys_Family.Instance.MoodTypeRatio : 1f) * (isLove ? Sys_Family.Instance.MoodLoveRatio : 1)));
                            Sys_Family.Instance.SetTextData(moodOrHealthValueText, (float)moodValue, cSVFamilyPetFoodData.addMoodValue);
                        }

                        bool isHunger = Sys_Family.Instance.hungerCount > 0;
                        feedRatioGo.gameObject.SetActive(isHunger);
                        var roleLevel = Sys_Role.Instance.Role.Level;
                        uint exp = 0;
                        for (int i = 0; i < cSVFamilyPetFoodData.feedRewardEXP.Count; i++)
                        {
                            if(null != cSVFamilyPetFoodData.feedRewardEXP[i] && cSVFamilyPetFoodData.feedRewardEXP[i].Count >= 2 && roleLevel <= cSVFamilyPetFoodData.feedRewardEXP[i][0])
                            {
                                exp = cSVFamilyPetFoodData.feedRewardEXP[i][1];
                                break;
                            }
                        }
                        var expValue = isHunger ? exp * Sys_Family.Instance.FeedExpRatio: exp;
                        Sys_Family.Instance.SetTextData(expText, (float)expValue, exp);
                    }
                    else
                    {
                        DebugUtil.LogError($"Not Find is {this.cellDatas[selectedGiftPropItemIndex].id} in CSVFamilyPetFood");
                    }
                }
                else if(selectedGiftPropItemIndex >= this.cellDatas.Count - 1 && this.cellDatas.Count > 1)
                {
                    selectedGiftPropItemIndex = this.cellDatas.Count - 2;
                    RefreshPreView();
                }
                else
                {
                    selectedGiftPropItemIndex = -1;
                    ClearItemEffect();
                }
            }
        }

        private void ClearItemEffect()
        {
            moodOrHealthText.text = LanguageHelper.GetTextContent(2023609u);
            growthText.text = "0";
            moodOrHealthValueText.text = "0";
            expText.text = "0";
            feedRatioGo.gameObject.SetActive(false);
        }

        public void SetInfinityGridCell(int count)
        {
            infinityGrid.CellCount = count;
            infinityGrid.ForceRefreshActiveCell();
        }

        List<int> tabIds = new List<int>(4) {1, 2, 3, 4};

        public void RefreshAll()
        {
            var ids = this.tabIds;
            this.tabVds.BuildOrRefresh<int>(this.tabProto, this.tabProto.transform.parent, ids, (vd, id, indexOfVdList) => {
                vd.SetUniqueId(id);
                vd.SetSelectedAction((innerId, force) => {
                    if (this.currentTabId != innerId)
                    {
                        this.selectedGiftPropItemIndex = -1;
                    }

                    this.currentTabId = innerId;
                    this.GetCellDatas();

                    bool isValid = (0 <= this.selectedGiftPropItemIndex && this.selectedGiftPropItemIndex < this.cellDatas.Count);
                    if (!isValid)
                    {
                        this.selectedGiftPropItemIndex = -1;
                    }

                    this.RefreshPreView();
                    this.RefreshGrid();

                    if (this.selectedGiftPropItemIndex == -1)
                    {
                        this.infinityGrid.MoveToIndex(0);
                    }
                });
                vd.Refresh(id, (int)familyCreatureEntry.cSV.food_Type);
            });
            // 默认选中Tab
            if (ids.Count > 0)
            {
                if (this.currentTabId <= 0 || !ids.Contains(this.currentTabId))
                {
                    if (null != familyCreatureEntry)
                    {
                        currentTabId = (int)familyCreatureEntry.cSV.food_Type;
                    }
                }
                this.tabVds[this.currentTabId - 1].SetSelected(true, true);
            }
        }

        private void GetCellDatas()
        {
            this.cellDatas = familyCreatureEntry.FilterByGiftType((uint)currentTabId);
        }

        private void RefreshGrid()
        {
            SetInfinityGridCell(cellDatas.Count);
        }

        private void RefreshLikes()
        {
            if(null != familyCreatureEntry)
            {
                FrameworkTool.CreateChildList(hobbyTransform, familyCreatureEntry.creature.LoveFoods.Count);
                for (int i = 0; i < hobbyTransform.childCount; i++)
                {
                    PropItem item = new PropItem();
                    item.BindGameObject(hobbyTransform.GetChild(i).gameObject);
                    PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(familyCreatureEntry.creature.LoveFoods[i], 0, false, false, false, false, false, false, true);
                    item.txtName.text = LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(familyCreatureEntry.creature.LoveFoods[i]).name_id);
                    item.SetData(itemData, EUIID.UI_FamilyCreatures);
                }
            }
        }
    }

    public class UI_FamilyCreatures_Feed : UIBase, UI_FamilyCreatures_Feed_Layout.IListener
    {
        private UI_FamilyCreatures_Feed_Layout layout = new UI_FamilyCreatures_Feed_Layout();
        private UI_FamilyCreature_LeftView leftTopView;
        private UI_FamilyCreaturesFeedInfo uI_FamilyCreaturesFeedInfo;
        private UI_FamilyCreaturesFeedGive uI_FamilyCreaturesFeedGive;
        private UI_FamilyCreatureModelLoad modelLoad = new UI_FamilyCreatureModelLoad();
        private uint currentId;
        private FamilyCreatureEntry creatureEntry;
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
            leftTopView = AddComponent<UI_FamilyCreature_LeftView>(layout.LeftTopTran);
            uI_FamilyCreaturesFeedInfo = AddComponent<UI_FamilyCreaturesFeedInfo>(layout.InfoViewTran);
            uI_FamilyCreaturesFeedGive = AddComponent< UI_FamilyCreaturesFeedGive >(layout.GiveViewTran);
            modelLoad.SetEventImage(transform.Find("Animator/Texture").GetComponent<Image>());
            modelLoad.assetDependencies = transform.GetComponent<AssetDependencies>();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.OnGetFamilyPetInfo, RefreshInfo, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.OnGetFamilyPetFeedInfo, RefreshFeedCout, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.OnFamilyPetFeedEnd, RefreshInfo, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.OnFamilyPetNoticeVerChange, RefreshRedPoint, toRegister);
        }

        protected override void OnOpen(object arg = null)
        {
            if(arg is Tuple<uint,object>)
            {
                Tuple<uint, object> tuple = arg as Tuple<uint, object>;
                currentId = Convert.ToUInt32(tuple.Item2);
            }
            else if(arg is uint)
            {
                currentId = Convert.ToUInt32(arg);
            }
        }

        protected override void OnShowEnd()
        {
            
        }

        protected override void OnShow()
        {
            Sys_Family.Instance.GuildPetGetFeedInfoReq();
            Sys_Family.Instance.GuildPetUpdatePetInfoReq(currentId);
            RefreshInfo();
        }

        private void RefreshRedPoint()
        {
            layout.SetNoticeRedPoint();
        }

        private void RefreshInfo()
        {
            creatureEntry = Sys_Family.Instance.GetFamilyCreatureByType(currentId);
            modelLoad.SetValue(creatureEntry.cSV);
            uI_FamilyCreaturesFeedInfo.SetData(creatureEntry);
            uI_FamilyCreaturesFeedGive.SetData(creatureEntry);
            if (null != creatureEntry)
            {
                leftTopView.SetFamilyCreatureInfo(creatureEntry);
                layout.allSlider.SetSliderValue(creatureEntry.creature.Growth, creatureEntry.cSV.growthValueMax);
                layout.todaySlider.SetSliderValue(creatureEntry.creature.DailyGrowth, creatureEntry.cSV.dailyGrowthValueMax);
            }
            RefreshRedPoint();
        }

        private void RefreshFeedCout()
        {
            uI_FamilyCreaturesFeedGive.RefreshCount();
        }

        protected override void OnHide()
        {
            modelLoad.Hide();
            uI_FamilyCreaturesFeedGive.Hide();
        }

        protected override void OnDestroy()
        {
        }

        public void CloseBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_FamilyCreatures_Feed, "CloseBtnClicked");
            UIManager.CloseUI(EUIID.UI_FamilyCreatures_Feed);
        }

        public void PriviewBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_FamilyCreatures_Feed, "PriviewBtnClicked");
            if (null != creatureEntry)
            {
                UIManager.OpenUI(EUIID.UI_FamilyCreatures_Get, false, new Tuple<uint, object>(1, creatureEntry.cSV.food_Type));
            }
        }

        public void NotoceBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_FamilyCreatures_Feed, "NotoceBtnClicked");
            UIManager.OpenUI(EUIID.UI_FamilyCreatures_Notice);
        }

        public void RuleBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_FamilyCreatures_Feed, "RuleBtnClicked");
            UIManager.OpenUI(EUIID.UI_Rule, false, new UIRuleParam { StrContent = LanguageHelper.GetTextContent(2023650) });
        }
    }
}