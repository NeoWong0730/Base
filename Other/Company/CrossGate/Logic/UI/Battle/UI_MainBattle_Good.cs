using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using UnityEngine.EventSystems;
using Lib.Core;
using Packet;
using System;
using System.Linq;
using Component = Logic.Core.Component;

namespace Logic
{
    public class UI_MainBattle_Good : UIBase
    {
        private InfinityGridLayoutGroup infinity;
        private CP_ToggleRegistry CP_ToggleRegistry_Lable;
        private CP_Toggle CP_Toggle_II;
        private Button IIDarkBtn;
        private Dictionary<GameObject, CeilGrid> ceilGrids = new Dictionary<GameObject, CeilGrid>();
        private Transform parent;
        private List<ItemData> itemDatas = new List<ItemData>();
        private Image closeBtn;
        private Text _countText;
        private GameObject imageNo;
        private GameObject m_ScrollNode;
        private bool isHero;
        private bool IICanuse;
        private Button m_BuyButton;

        private InfinityGrid m_InfinityGrid_0;
        private InfinityGrid m_InfinityGrid_1;
        private InfinityGrid m_InfinityGrid_2;
        private InfinityGrid m_InfinityGrid_3;

        private Dictionary<int, List<ItemData>> mDatas = new Dictionary<int, List<ItemData>>(); //	1：回血 2：回蓝物品 3：双回物品 4：复活物品  5：药剂物品  6：变身物品

        private Text m_Type1Txt;
        private Text m_Type2Txt;
        private Text m_Type3Txt;
        private Text m_Type4Txt;
        
        private int m_MaxTypeCount = 6;


        private int curLable = 1;

        private int CurLable
        {
            get { return curLable; }
            set
            {
                if (curLable != value)
                {
                    curLable = value;
                }
            }
        }

        private bool b_CanUseLable1 = false;
        private bool b_CanUseLable2 = false;
        private bool b_CanUseLable3 = false;
        private uint countRemain1;
        private uint countRemain2;
        private uint countRemain3;


        protected override void OnLoaded()
        {
            ParseComponent();
            RegisterFashionEvent();
            ResetData();
        }

        protected override void OnOpen(object arg)
        {
            if (arg != null)
            {
                isHero = (bool) arg;
            }
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_Bag.Instance.eventEmitter.Handle<uint>(Sys_Bag.EEvents.OnUseItemSuccessInBattle, OnUseSuccess, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle(Sys_Bag.EEvents.OnResetMainBattleData, ResetData, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle(Sys_Bag.EEvents.OnSelectMob_UseItem, OnSelectMob_UseItem, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle(Sys_Bag.EEvents.OnBattleRoundStartNtf, OnBattleRoundStartNtf, toRegister);
            Net_Combat.Instance.eventEmitter.Handle<uint, uint>(Net_Combat.EEvents.OnCommandIsOk, OnCommandIsOk, toRegister);
        }

        protected override void OnShow()
        {
            m_InfinityGrid_0.MoveToIndex(0);
            m_InfinityGrid_1.MoveToIndex(0);
            m_InfinityGrid_2.MoveToIndex(0);
            m_InfinityGrid_3.MoveToIndex(0);
            
            RefreshItemData();
            CP_ToggleRegistry_Lable.SwitchTo(1);
            UpdateInfo();
            IICanuse = true;
        }

        private void ParseComponent()
        {
            parent = transform.Find("Animator/View_Main/Scroll_View/Viewport").transform;
            closeBtn = transform.Find("Button_Close").GetComponent<Image>();
            _countText = transform.Find("Animator/View_Main/View_Tips/Text").GetComponent<Text>();
            CP_ToggleRegistry_Lable = transform.Find("Animator/View_Main/List_Menu_Attr/TabList").GetComponent<CP_ToggleRegistry>();
            imageNo = transform.Find("Animator/View_Main/Image_No").gameObject;
            CP_Toggle_II = transform.Find("Animator/View_Main/List_Menu_Attr/TabList/ToggleItem01 (1)").GetComponent<CP_Toggle>();
            IIDarkBtn = transform.Find("Animator/View_Main/List_Menu_Attr/TabList/ToggleItem01 (1)/Btn_Menu_Dark").GetComponent<Button>();
            m_BuyButton = transform.Find("Animator/View_Main/Btn_Buy").GetComponent<Button>();
            
            m_Type1Txt = transform.Find("Animator/View_Main/Scroll_View/Viewport/Title1/Text").GetComponent<Text>();
            m_Type2Txt = transform.Find("Animator/View_Main/Scroll_View/Viewport/Title2/Text").GetComponent<Text>();
            m_Type3Txt = transform.Find("Animator/View_Main/Scroll_View/Viewport/Title3/Text").GetComponent<Text>();
            m_Type4Txt = transform.Find("Animator/View_Main/Scroll_View/Viewport/Title4/Text").GetComponent<Text>();
            
            m_BuyButton.onClick.AddListener(OnBuyButtonClicked);
            IIDarkBtn.onClick.AddListener(OnIIDarkBtnClicked);
            IIDarkBtn.enabled = false;
            // infinity = parent.gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            // infinity.minAmount = 20;
            // infinity.updateChildrenCallback = UpdateChildrenCallback;

            m_ScrollNode = transform.Find("Animator/View_Main/Scroll_View").gameObject;
            m_InfinityGrid_0 = transform.Find("Animator/View_Main/Scroll_View/Viewport/Scroll_View1").GetComponent<InfinityGrid>();
            m_InfinityGrid_1 = transform.Find("Animator/View_Main/Scroll_View/Viewport/Scroll_View2").GetComponent<InfinityGrid>();
            m_InfinityGrid_2 = transform.Find("Animator/View_Main/Scroll_View/Viewport/Scroll_View3").GetComponent<InfinityGrid>();
            m_InfinityGrid_3 = transform.Find("Animator/View_Main/Scroll_View/Viewport/Scroll_View4").GetComponent<InfinityGrid>();

            m_InfinityGrid_0.onCreateCell += OnCreateCell;
            m_InfinityGrid_1.onCreateCell += OnCreateCell;
            m_InfinityGrid_2.onCreateCell += OnCreateCell;
            m_InfinityGrid_3.onCreateCell += OnCreateCell;

            m_InfinityGrid_0.onCellChange += OnCellChange_0;
            m_InfinityGrid_1.onCellChange += OnCellChange_1;
            m_InfinityGrid_2.onCellChange += OnCellChange_2;
            m_InfinityGrid_3.onCellChange += OnCellChange_3;

            // for (int i = 0; i < parent.childCount; i++)
            // {
            //     GameObject go = parent.GetChild(i).gameObject;
            //     CeilGrid bagCeilGrid = new CeilGrid();
            //
            //     bagCeilGrid.BindGameObject(go);
            //     bagCeilGrid.AddClickListener(OnGridSelected, OnGridLongPressed);
            //
            //     ceilGrids.Add(go, bagCeilGrid);
            // }
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            MainBattleGoodGrid entry = new MainBattleGoodGrid();
            entry.BindGameObject(cell.mRootTransform.gameObject);
            cell.BindUserData(entry);
            entry.AddEventListener(OnGridSelected, OnGridLongPressed);
        }

        private void OnCellChange_0(InfinityGridCell cell, int index)
        {
            MainBattleGoodGrid entry = cell.mUserData as MainBattleGoodGrid;
            if (curLable == 1)
            {
                entry.SetData(mDatas[1][index]);
            }
            else
            {
                entry.SetData(mDatas[5][index]);
            }
        }

        private void OnCellChange_1(InfinityGridCell cell, int index)
        {
            MainBattleGoodGrid entry = cell.mUserData as MainBattleGoodGrid;
            if (curLable == 1)
            {
                entry.SetData(mDatas[2][index]);
            }
            else
            {
                entry.SetData(mDatas[6][index]);
            }
        }

        private void OnCellChange_2(InfinityGridCell cell, int index)
        {
            MainBattleGoodGrid entry = cell.mUserData as MainBattleGoodGrid;
            entry.SetData(mDatas[3][index]);
        }

        private void OnCellChange_3(InfinityGridCell cell, int index)
        {
            MainBattleGoodGrid entry = cell.mUserData as MainBattleGoodGrid;
            entry.SetData(mDatas[4][index]);
        }

        private void OnBuyButtonClicked()
        {
            UIManager.OpenUI(EUIID.UI_Trade);
            UIManager.CloseUI(EUIID.UI_MainBattle_Good, false, false);
        }
        
        private void OnIIDarkBtnClicked()
        {
            if (!IICanuse)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(359999994));
            }
        }

        private void RegisterFashionEvent()
        {
            CP_ToggleRegistry_Lable.onToggleChange = onParentToggleChanged;
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(closeBtn.GetComponent<Image>());
            eventListener.AddEventListener(EventTriggerType.PointerDown, OnCLoseBtnClicked);
        }

        private void OnBattleRoundStartNtf()
        {
            if (countRemain1 > 0)
            {
                b_CanUseLable1 = true;
            }

            if (countRemain2 > 0)
            {
                b_CanUseLable2 = true;
            }
        }

        private void ResetData()
        {
            countRemain1 = CombatManager.Instance.m_BattleTypeTb.normal_medic_num - Sys_Fight.Instance.cachItemUseCounts[0];
            countRemain2 = CombatManager.Instance.m_BattleTypeTb.special_medic_num - Sys_Fight.Instance.cachItemUseCounts[1];
            b_CanUseLable1 = true;
            b_CanUseLable2 = true;
            b_CanUseLable3 = true;
            SetCount();
        }

        private void RefreshItemData()
        {
            mDatas.Clear();
            itemDatas.Clear();
            if (Net_Combat.Instance.m_TeachingID == 0)
            {
                itemDatas = Sys_Bag.Instance.GetBattleUseItem(1);
                itemDatas.AddRange(Sys_Bag.Instance.GetBattleUseItem(2));
            }
            else
            {
                itemDatas = GameCenter.fightControl.GetTeachingItemData(1);
                itemDatas.AddRange(GameCenter.fightControl.GetTeachingItemData(1));
            }

            for (int i = 1; i <= m_MaxTypeCount; i++)
            {
                ProcessItemDatas(i);
            }
        }

        private void OnUseSuccess(uint itemId)
        {
            CSVItem.Data csvItem = CSVItem.Instance.GetConfData(itemId);

            if (csvItem != null)
            {
                uint itemType = csvItem.type_id;

                List<uint> itemTbs_Normal = CombatManager.Instance.m_BattleTypeTb.normal_medic;

                if (itemTbs_Normal.Contains(itemType))
                {
                    if (countRemain1 > 0)
                    {
                        countRemain1--;
                    }
                }

                List<uint> itemTbs_Special = CombatManager.Instance.m_BattleTypeTb.special_medic;

                if (itemTbs_Special.Contains(itemType))
                {
                    if (countRemain2 > 0)
                    {
                        countRemain2--;
                    }
                }
            }

            RefreshItemData();
            UpdateInfo();
        }

        private void OnCommandIsOk(uint uniteId, uint isOk)
        {
            if (uniteId == GameCenter.mainFightHero.battleUnit.UnitId)
            {
                if (isOk == 0) //撤销
                {
                    b_CanUseLable1 = true;
                    b_CanUseLable2 = true;
                }
            }
        }

        private void OnSelectMob_UseItem()
        {
            if (CurLable == 1)
            {
                if (countRemain1 == 1)
                {
                    b_CanUseLable1 = false;
                }
            }

            if (CurLable == 2)
            {
                if (countRemain2 == 1)
                {
                    b_CanUseLable2 = false;
                }
            }

            if (CurLable == 3)
            {
                if (countRemain3 == 1)
                {
                    b_CanUseLable3 = false;
                }
            }
        }

        private void SetCount(string format, string str1, object str2)
        {
            string count = string.Format(format, str1, str2);
            _countText.text = count;
        }

        private void onParentToggleChanged(int curToggle, int old)
        {
            CurLable = curToggle;
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            //infinity.SetAmount(itemDatas.Count);
            if (curLable == 1)
            {
                m_InfinityGrid_0.CellCount = mDatas[1].Count;
                m_InfinityGrid_0.ForceRefreshActiveCell();

                m_InfinityGrid_1.CellCount = mDatas[2].Count;
                m_InfinityGrid_1.ForceRefreshActiveCell();

                m_InfinityGrid_2.CellCount = mDatas[3].Count;
                m_InfinityGrid_2.ForceRefreshActiveCell();

                m_InfinityGrid_3.CellCount = mDatas[4].Count;
                m_InfinityGrid_3.ForceRefreshActiveCell();
                
                TextHelper.SetText(m_Type1Txt,2009715);
                TextHelper.SetText(m_Type2Txt,2009716);
                TextHelper.SetText(m_Type3Txt,2009717);
                TextHelper.SetText(m_Type4Txt,2009718);
            }
            else if (curLable == 2)
            {
                m_InfinityGrid_0.CellCount = mDatas[5].Count;
                m_InfinityGrid_0.ForceRefreshActiveCell();

                m_InfinityGrid_1.CellCount = mDatas[6].Count;
                m_InfinityGrid_1.ForceRefreshActiveCell();

                m_InfinityGrid_2.CellCount = 0;
                m_InfinityGrid_2.ForceRefreshActiveCell();

                m_InfinityGrid_3.CellCount = 0;
                m_InfinityGrid_3.ForceRefreshActiveCell();
                
                TextHelper.SetText(m_Type1Txt,2009719);
                TextHelper.SetText(m_Type2Txt,2009720);
                TextHelper.SetText(m_Type3Txt,String.Empty);
                TextHelper.SetText(m_Type4Txt,String.Empty);
            }

            SetCount();
            imageNo.SetActive(itemDatas.Count == 0);
            m_ScrollNode.SetActive(itemDatas.Count > 0);
        }

        private void ProcessItemDatas(int type)
        {
            List<ItemData> temp = new List<ItemData>();

            for (int i = 0; i < itemDatas.Count; i++)
            {
                var item = itemDatas[i];

                if (item.cSVItemData.battle_show_type == type)
                {
                    temp.Add(item);
                }
            }
            if(type == 6)
            {
                //变身卡排序处理
                temp =Sys_Bag.Instance.GetSortTransfigurationCard(temp);
            }
            mDatas[type] = temp;
        }

        private void SetCount()
        {
            if (CurLable == 1)
            {
                SetCount(CSVLanguage.Instance.GetConfData(2009701).words, CSVLanguage.Instance.GetConfData(2009703).words, countRemain1);
            }

            if (CurLable == 2)
            {
                SetCount(CSVLanguage.Instance.GetConfData(2009701).words, CSVLanguage.Instance.GetConfData(2009704).words, countRemain2);
            }

            if (CurLable == 3)
            {
                SetCount(CSVLanguage.Instance.GetConfData(2009701).words, CSVLanguage.Instance.GetConfData(2009705).words, countRemain3);
            }
        }

        // private void UpdateChildrenCallback(int index, Transform trans)
        // {
        //     CeilGrid ceilGrid = ceilGrids[trans.gameObject];
        //     ceilGrid.SetData(itemDatas[index], index, CeilGrid.EGridState.Normal, CeilGrid.ESource.e_BattleUse);
        // }

        private void OnGridSelected(MainBattleGoodGrid bagCeilGrid)
        {
            if (!CanUse())
            {
                if (CurLable == 1)
                    Sys_Hint.Instance.PushContent_Normal(CSVLanguage.Instance.GetConfData(2009707).words);
                if (CurLable == 2)
                    Sys_Hint.Instance.PushContent_Normal(CSVLanguage.Instance.GetConfData(2009708).words);
                if (CurLable == 3)
                    Sys_Hint.Instance.PushContent_Normal(CSVLanguage.Instance.GetConfData(2009709).words);
                return;
            }

            if (!b_CanUseLable1 && CurLable == 1)
            {
                string content = LanguageHelper.GetTextContent(2009714);
                Sys_Hint.Instance.PushContent_Normal(content);
                return;
            }

            if (!b_CanUseLable2 && CurLable == 2)
            {
                string content = LanguageHelper.GetTextContent(2009714);
                Sys_Hint.Instance.PushContent_Normal(content);
                return;
            }

            if (!b_CanUseLable3 && CurLable == 3)
            {
                string content = LanguageHelper.GetTextContent(2009714);
                Sys_Hint.Instance.PushContent_Normal(content);
                return;
            }
            //变身卡使用条件检测
            if (bagCeilGrid.ItemData.cSVItemData.type_id == 1704)
            {
                //功能未开启提示
                if (bagCeilGrid.ItemData.cSVItemData.FunctionOpenId != 0 && !Sys_FunctionOpen.Instance.IsOpen(bagCeilGrid.ItemData.cSVItemData.FunctionOpenId, true))
                {
                    return;
                }
                if (!Sys_Transfiguration.Instance.GetCurUseShapeShiftData().CanUseChangeCard(bagCeilGrid.ItemData.Id))
                {

                    string content = LanguageHelper.GetTextContent(2013033);
                    Sys_Hint.Instance.PushContent_Normal(content);
                    return;
                }
                //变身卡CD检测
                var m_CsvActiveSkillData = CSVActiveSkill.Instance.GetConfData(bagCeilGrid.ItemData.cSVItemData.active_skillid);
                if (Sys_Fight.Instance.useRound.ContainsKey(m_CsvActiveSkillData.main_skill_id))
                {
                    uint round = Sys_Fight.Instance.useRound[m_CsvActiveSkillData.main_skill_id];
                    bool allRoundOver = round + m_CsvActiveSkillData.type_cold_time < Net_Combat.Instance.m_CurRound;
                    if (!allRoundOver)
                    {
                        //CD没结束
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2013034));
                        return;
                    }
                }
                string content_Lan = LanguageHelper.GetTextContent(2013032, LanguageHelper.GetTextContent(bagCeilGrid.ItemData.cSVItemData.name_id));
                PromptBoxParameter.Instance.OpenPromptBox(content_Lan, 0, () =>
                 {
                     UIManager.CloseUI(EUIID.UI_MainBattle_Good, false, false);
                     GameCenter.fightControl.AttackById(bagCeilGrid.ItemData.cSVItemData.active_skillid, bagCeilGrid.ItemData.cSVItemData.id);
                 });
            }
            else
            {
                UIManager.CloseUI(EUIID.UI_MainBattle_Good, false, false);
                GameCenter.fightControl.AttackById(bagCeilGrid.ItemData.cSVItemData.active_skillid, bagCeilGrid.ItemData.cSVItemData.id);
            }
        }

        private bool CanUse()
        {
            bool canuse = false;
            if (CurLable == 1)
            {
                if (countRemain1 > 0)
                {
                    canuse = true;
                }
            }

            if (CurLable == 2)
            {
                if (countRemain2 > 0)
                {
                    canuse = true;
                }
            }

            if (CurLable == 3)
            {
                if (countRemain3 > 0)
                {
                    canuse = true;
                }
            }

            return canuse;
        }

        private bool CheckPetHaveLimit(uint skillId, PetUnit petUnit)
        {
            if (petUnit.BaseSkillInfo.Skills.Contains(skillId))
            {
                return true;
            }

            if (petUnit.BaseSkillInfo.UniqueSkills.Contains(skillId))
            {
                return true;
            }

            if (petUnit.BuildInfo.BuildSkills.Contains(skillId))
            {
                return true;
            }

            return false;
        }

        private void OnCLoseBtnClicked(BaseEventData baseEventData)
        {
            UIManager.CloseUI(EUIID.UI_MainBattle_Good, false, false);
        }


        private void OnGridLongPressed(MainBattleGoodGrid bagCeilGrid)
        {
            UIManager.OpenUI(EUIID.UI_Message_Box, false,
                new MessageBoxEvt(EUIID.UI_MainBattle_Good, new PropIconLoader.ShowItemData(bagCeilGrid.ItemData.Id,
                    bagCeilGrid.ItemData.Count, false, bagCeilGrid.ItemData.bBind, bagCeilGrid.ItemData.bNew, false,
                    false)));
        }


        public class MainBattleGoodGrid
        {
            private GameObject m_GameObject;

            private Image m_ImageCD;

            private Text m_TextCount;

            public ItemData ItemData;

            private CSVActiveSkill.Data m_CsvActiveSkillData;
            
            private Button m_ClickButton;

            private Action<MainBattleGoodGrid> m_OnClicked;

            private Action<MainBattleGoodGrid> m_LongPressed;
            
            private Image mBg;
            private Image mIcon;
            private Text mCount;

            private GameObject goGray;

            public void BindGameObject(GameObject go)
            {
                m_GameObject = go;

                m_ImageCD = m_GameObject.transform.Find("Image_cd").GetComponent<Image>();

                m_TextCount = m_GameObject.transform.Find("Image_cd/Text_cd").GetComponent<Text>();

                m_ClickButton=m_GameObject.transform.Find("Btn_Item").GetComponent<Button>();
                m_ClickButton.onClick.AddListener(OnClick);
                
                mBg = m_GameObject.transform.Find("Btn_Item/Image_BG").GetComponent<Image>();
                mIcon = m_GameObject.transform.Find("Btn_Item/Image_Icon").GetComponent<Image>();
                mCount = m_GameObject.transform.Find("Text_Number").GetComponent<Text>();
                goGray = m_GameObject.transform.Find("Image_Limit").gameObject;
            }

            public void SetData(ItemData itemData)
            {
                ItemData = itemData;

                CSVItem.Data csvItem = CSVItem.Instance.GetConfData(ItemData.Id);

                if (csvItem != null)
                {
                    m_CsvActiveSkillData = CSVActiveSkill.Instance.GetConfData(csvItem.active_skillid);
                    
                    if (m_CsvActiveSkillData == null)
                    {
                        DebugUtil.LogErrorFormat($"activeSkillId not found: {csvItem.active_skillid}   itemId:{csvItem.id}");
                    }
                }

                Refresh();
            }

            private void Refresh()
            {
                mIcon.gameObject.SetActive(true);
                ImageHelper.SetIcon(mIcon, ItemData.cSVItemData.icon_id);
                ImageHelper.GetQualityColor_Frame(mBg, (int)ItemData.Quality);
                mCount.gameObject.SetActive(true);
                if (ItemData.Count > 1)
                {
                    mCount.text = ItemData.Count.ToString();
                }
                else
                {
                    mCount.text = string.Empty;
                }

                
                if (!Sys_Fight.Instance.useRound.ContainsKey(m_CsvActiveSkillData.main_skill_id))
                {
                    m_ImageCD.gameObject.SetActive(false);
                }
                else
                {
                    uint round = Sys_Fight.Instance.useRound[m_CsvActiveSkillData.main_skill_id];

                    bool allRoundOver = round + m_CsvActiveSkillData.type_cold_time < Net_Combat.Instance.m_CurRound;

                    m_ImageCD.gameObject.SetActive(!allRoundOver);

                    uint remainRound = round + m_CsvActiveSkillData.type_cold_time - Net_Combat.Instance.m_CurRound + 1;
                    
                    if (!allRoundOver)
                    {
                        m_ImageCD.fillAmount = (float) remainRound / (float) m_CsvActiveSkillData.type_cold_time;
                        
                        TextHelper.SetText(m_TextCount, remainRound.ToString());
                    }
                }
                //变身卡不可用置灰逻辑
                bool isGray = ItemData.cSVItemData.type_id == 1704 && !Sys_Transfiguration.Instance.GetCurUseShapeShiftData().CanUseChangeCard(ItemData.Id);
                goGray.SetActive(isGray);
            }

            public void AddEventListener(Action<MainBattleGoodGrid> onClicked, Action<MainBattleGoodGrid> onLongPressed)
            {
                m_OnClicked = onClicked;
                if (onLongPressed != null) 
                {
                    UI_LongPressButton uI_LongPressButton = m_ClickButton.gameObject.GetNeedComponent<UI_LongPressButton>();
                    uI_LongPressButton.onStartPress.AddListener(OnLongPressed);
                    m_LongPressed = onLongPressed;
                }
            }

            private void OnClick()
            {
                m_OnClicked?.Invoke(this);
            }
            
            private void OnLongPressed()
            {
                UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_MainBattle_Good,
                    new PropIconLoader.ShowItemData(ItemData.Id, 0, true, false, false, false, false)));
            }
        }
    }
}