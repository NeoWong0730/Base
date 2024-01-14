using Logic.Core;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Table;
using Packet;
using Lib.Core;

namespace Logic
{
    public class UI_Transfiguration_Plan_Item
    {
        private Transform transform;
        public CP_Toggle toggle;
        private Button addBtn;
        private Button reNameBtn;
        private Text nameLight;
        private Text nameDark;
        public int index;
        public Action<int> addAction;
        public Action<int> renameAction;

        public void Init(Transform _transform)
        {
            transform = _transform;
            toggle = transform.Find("TabItem").GetComponent<CP_Toggle>();
            toggle.onValueChanged.AddListener(onValueChanged);
            addBtn = transform.Find("Button_Add").GetComponent<Button>();
            addBtn.onClick.AddListener(OnAddBtnClicked);
            reNameBtn = transform.Find("TabItem/Button_Rename").GetComponent<Button>();
            reNameBtn.onClick.AddListener(OnReNameBtnClicked);
            nameLight = transform.Find("TabItem/Image_Menu_Light/Text_Menu_Dark").GetComponent<Text>();
            nameDark = transform.Find("TabItem/Btn_Menu_Dark/Text_Menu_Dark").GetComponent<Text>();
        }

        public void Refersh(int _index, string _name, bool _isAdd)
        {
            index = _index;
            if (_name == ""||_name == null)
            {
                nameLight.text = LanguageHelper.GetTextContent(10013503, (index + 1).ToString());
                nameDark.text = LanguageHelper.GetTextContent(10013503, (index + 1).ToString());
            }
            else
            {
                nameLight.text = _name;
                nameDark.text = _name;
            }
            addBtn.gameObject.SetActive(_isAdd);
            toggle.gameObject.SetActive(!_isAdd);
        }

        public void Destroy()
        {
            addAction = null;
            renameAction = null;
        }

        private void OnAddBtnClicked()
        {
            addAction?.Invoke(index);
        }

        private void OnReNameBtnClicked()
        {
            renameAction?.Invoke(index);
        }

        private void onValueChanged(bool isOn)
        {
            if (isOn)
            {
                Sys_Transfiguration.Instance.eventEmitter.Trigger<int>(Sys_Transfiguration.EEvents.OnSelectPlan, index);
            }
        }
    }

    public class UI_Transfiguration_Plan_Menu
    {
        private Transform transform;
        private Transform item;
        private List<UI_Transfiguration_Plan_Item> cells = new List<UI_Transfiguration_Plan_Item>();

        public void Init(Transform _transform)
        {
            transform = _transform;
            item = transform.Find("TabList/Item");
        }

        public void PocessEvent(bool isRegister)
        {
            Sys_Transfiguration.Instance.eventEmitter.Handle(Sys_Transfiguration.EEvents.OnUpdatePlan, OnUpdatePlan, isRegister);
            Sys_Transfiguration.Instance.eventEmitter.Handle<uint>(Sys_Transfiguration.EEvents.OnRenamePlan, OnRenamePlan, isRegister);
        }

        private void OnUpdatePlan()
        {
            Default();
            AddItems();
        }

        private void OnRenamePlan(uint index)
        {
            for (int i = 0; i < cells.Count; ++i)
            {
                if (cells[i].index == index)
                {
                    cells[i].Refersh((int)index, Sys_Transfiguration.Instance.clientShapeShiftPlans[i].name, i == (cells.Count - 1));
                }
            }
        }

        public void Refresh()
        {
            Default();
            AddItems();
        }

        private void AddItems()
        {
            cells.Clear();
            FrameworkTool.CreateChildList(item.parent.transform, Sys_Transfiguration.Instance.clientShapeShiftPlans.Count + 1);
            for (int i = 0; i <= Sys_Transfiguration.Instance.clientShapeShiftPlans.Count; ++i)
            {
                if (i != Sys_Transfiguration.Instance.clientShapeShiftPlans.Count)
                {
                    item.parent.transform.GetChild(i).name = (i + 1).ToString();
                }
                else
                {
                    item.parent.transform.GetChild(i).name = "Add";
                }
                UI_Transfiguration_Plan_Item cell = new UI_Transfiguration_Plan_Item();
                cell.Init(item.parent.transform.GetChild(i));
                if (i == Sys_Transfiguration.Instance.clientShapeShiftPlans.Count)
                {
                    cell.Refersh(i, string.Empty, true);
                }
                else
                {
                    cell.Refersh(i, Sys_Transfiguration.Instance.clientShapeShiftPlans[i].name, false);
                }
                cell.addAction = OnAddAction;
                cell.renameAction = OnRenamection;
                cells.Add(cell);
                cell.toggle.SetSelected(i == Sys_Transfiguration.Instance.curIndex, true);
            }
        }

        private void Default()
        {
            for (int i = 0; i < cells.Count; ++i)
            {
                cells[i].Destroy();
            }
            if (item != null)
            {
                string name = item.parent.transform.GetChild(0).name;
                FrameworkTool.DestroyChildren(item.parent.gameObject, name);
            }
        }

        private void OnRenamection(int index)
        {
            void OnRename(int schIndex, int __, string newName)
            {
                Sys_Transfiguration.Instance.PlanRenameReq((uint)schIndex, newName);
            }

            UI_ChangeSchemeName.ChangeNameArgs arg = new UI_ChangeSchemeName.ChangeNameArgs()
            {
                arg1 = index,
                arg2 = 0,
                oldName = Sys_Transfiguration.Instance.clientShapeShiftPlans[index].name,
                onYes = OnRename
            };
            UIManager.OpenUI(EUIID.UI_ChangeSchemeName, false, arg);
        }

        private void OnAddAction(int index)
        {
            UIManager.OpenUI(EUIID.UI_Transfiguration_Tips);
        }
    }

    public class UI_Race_Item
    {
        private Transform transform;
        private Image raceIcon;
        private Text raceName;
        private GameObject skillItem;
        public uint raceId;
        private uint index;

        public void Init(Transform _transform)
        {
            transform = _transform;
            raceIcon = transform.Find("Image/Icon").GetComponent<Image>();
            raceName = transform.Find("Image/Text_Name").GetComponent<Text>();
            skillItem = transform.Find("Scroll View_Skill/Viewport/Content/Skill").gameObject;
        }

        public void RefreshDate(uint _raceId, uint _index)
        {
            raceId = _raceId;
            index = _index;
            CSVGenus.Data csvGenusData = CSVGenus.Instance.GetConfData(raceId);
            ImageHelper.SetIcon(raceIcon, csvGenusData.rale_icon);
            TextHelper.SetText(raceName, csvGenusData.rale_name);
            if (!Sys_Transfiguration.Instance.clientShapeShiftPlans[(int)index].shapeShiftRaceSubNodes.ContainsKey(raceId))
            {
                skillItem.SetActive(false);
            }
            else
            {
                DefaultSkillItem();
                skillItem.SetActive(true);
                ShapeShiftSubNode node = Sys_Transfiguration.Instance.clientShapeShiftPlans[(int)index].shapeShiftRaceSubNodes[raceId];
                FrameworkTool.CreateChildList(skillItem.transform.parent, node.Grids.Count);
                for (int i = 0; i < node.Grids.Count; ++i)
                {
                    Transform tran = skillItem.transform.parent.GetChild(i);
                    if (i != 0)
                    {
                        tran.name = node.Grids[i].Gridid.ToString();
                    }
                    Image icon = tran.Find("Image_Icon").GetComponent<Image>();
                    Image smallIcon = tran.Find("Image_Typebg/Image_Type").GetComponent<Image>();
                    Text lv = tran.Find("Image_Levelbg/Level").GetComponent<Text>();
                    GameObject tip = tran.Find("Image_Tip").gameObject;

                    ShapeShiftSkillGrid skillData = node.Grids[i];
                    CSVPassiveSkillInfo.Data info = CSVPassiveSkillInfo.Instance.GetConfData(skillData.Skillid);
                    CSVRaceSkillType.Data skillType = CSVRaceSkillType.Instance.GetConfData(skillData.Skillid);
                    if (info != null && skillType != null)
                    {
                        icon.gameObject.SetActive(true);
                        ImageHelper.SetIcon(icon, info.icon);
                        ImageHelper.SetIcon(smallIcon, skillType.skill_type);
                        TextHelper.SetText(lv, 12462, info.level.ToString());
                        tip.SetActive(skillType.type == 1);
                    }
                    Button btn = tran.Find("Image_bg").GetComponent<Button>();
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(delegate () { this.OnClicked(skillData.Skillid); });
                }
            }
        }

        private void DefaultSkillItem()
        {
            FrameworkTool.DestroyChildren(skillItem.transform.parent.gameObject, skillItem.transform.name.ToString());
        }

        private void OnClicked(uint skillId)
        {
            if (skillId != 0)
            {
                UIManager.OpenUI(EUIID.UI_Skill_Tips, false, new Tuple<uint, uint>(skillId, 0));
            }
        }
    }

    public class UI_Transfigutation_Plan : UIComponent
    {
        private Button useBtn;
        private Text useBtnText;
        private Toggle raceSkillToggle;
        private Toggle allRaceAddToggle;
        private GameObject raceItem;
        private GameObject allRaceSkillItem;
        private GameObject raceSkillView;
        private GameObject allRaceSkillView;
        private GameObject planMenuGo;
        private GameObject noSkillGo;

        private int curIndex;
        private InfinityGrid infinityGridRaceSkill;
        private List<UI_Race_Item> raceItemList = new List<UI_Race_Item>();
        private int infinityCountRaceSkill;
        private InfinityGrid infinityGridAddRace;
        private List<TransformAddCeil> addCeilList = new List<TransformAddCeil>();
        private int infinityCountAddRace;

        private UI_Transfiguration_Plan_Menu UI_Transfiguration_Plan_Menu = new UI_Transfiguration_Plan_Menu();

        protected override void Loaded()
        {
            raceItem = transform.Find("Page1/Item").gameObject;
            allRaceSkillItem = transform.Find("Page2/Scroll View/Viewport/Image_Bottom").gameObject;
            raceSkillView = transform.Find("Page1").gameObject;
            allRaceSkillView = transform.Find("Page2").gameObject;
            planMenuGo = transform.Find("Label_Scroll01").gameObject;
            noSkillGo = transform.Find("Page2/View_None").gameObject;
            useBtn = transform.Find("Button_use").GetComponent<Button>();
            useBtnText = transform.Find("Button_use/Text_01").GetComponent<Text>();
            raceSkillToggle = transform.Find("Menu/ListItem").GetComponent<Toggle>();
            allRaceAddToggle = transform.Find("Menu/ListItem (1)").GetComponent<Toggle>();
            useBtn.onClick.AddListener(OnUseBtnClicked);
            raceSkillToggle.onValueChanged.AddListener(OnRaceSkillToggleValueChanged);
            allRaceAddToggle.onValueChanged.AddListener(OnAllRaceAddToggleValueChanged);
            infinityGridRaceSkill = transform.Find("Page1/Scroll View").GetComponent<InfinityGrid>();
            infinityGridRaceSkill.onCreateCell += OnCreateCellRaceSkill;
            infinityGridRaceSkill.onCellChange += OnCellChangeRaceSkill;
            infinityGridAddRace = transform.Find("Page2/Scroll View").GetComponent<InfinityGrid>();
            infinityGridAddRace.onCreateCell += OnCreateCellAddRace;
            infinityGridAddRace.onCellChange += OnCellChangeAddRace;
            UI_Transfiguration_Plan_Menu.Init(planMenuGo.transform);
        }

        public override void Show()
        {
            curIndex = (int)Sys_Transfiguration.Instance.curIndex;
            raceItemList.Clear();
            infinityCountRaceSkill = Sys_Transfiguration.Instance.GetRacesIds().Count;
            infinityGridRaceSkill.CellCount = infinityCountRaceSkill;
            infinityGridRaceSkill.ForceRefreshActiveCell();
            raceSkillToggle.SetIsOnWithoutNotify(true);
            allRaceAddToggle.SetIsOnWithoutNotify(false);
            raceSkillView.SetActive(true);
            allRaceSkillView.SetActive(false);
            UI_Transfiguration_Plan_Menu.Refresh();
            ProcessEventsForEnable(true);
            SetUseBtnState();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Transfiguration.Instance.eventEmitter.Handle<int>(Sys_Transfiguration.EEvents.OnSelectPlan, OnSelectPlan, toRegister);
            Sys_Transfiguration.Instance.eventEmitter.Handle(Sys_Transfiguration.EEvents.OnUpdatePlan, OnUpdatePlan, toRegister);
            UI_Transfiguration_Plan_Menu.PocessEvent(toRegister);
        }

        public override void Hide()
        {
            ProcessEventsForEnable(false);
        }

        public override void OnDestroy()
        {

        }

        private void OnSelectPlan(int index)
        {
            if (index == curIndex)
            {
                return;
            }
            curIndex = index;
            for (int i = 0; i < raceItemList.Count; ++i)
            {
                raceItemList[i].RefreshDate(raceItemList[i].raceId, (uint)curIndex);
            }
            SetUseBtnState();
        }

        private void OnUpdatePlan()
        {
            SetUseBtnState();
        }

        private void SetUseBtnState()
        {
            if (curIndex == Sys_Transfiguration.Instance.curIndex)
            {
                ImageHelper.SetImageGray(useBtn.GetComponent<Image>(), true);
                TextHelper.SetText(useBtnText, 8311);
                useBtn.enabled = false;
            }
            else
            {
                ImageHelper.SetImageGray(useBtn.GetComponent<Image>(), false);
                TextHelper.SetText(useBtnText, 5127);
                useBtn.enabled = true;
            }
        }
         
        private void OnCreateCellRaceSkill(InfinityGridCell cell)
        {
            UI_Race_Item entry = new UI_Race_Item();
            GameObject go = cell.mRootTransform.gameObject;
            entry.Init(go.transform);
            cell.BindUserData(entry);
        }

        private void OnCellChangeRaceSkill(InfinityGridCell cell, int index)
        {
            uint raceId = Sys_Transfiguration.Instance.GetRacesIds()[index];
            UI_Race_Item item = cell.mUserData as UI_Race_Item;
            item.RefreshDate(raceId, (uint)curIndex);
            raceItemList.Add(item);
        }

        private void OnCreateCellAddRace(InfinityGridCell cell)
        {
            TransformAddCeil entry = new TransformAddCeil();
            GameObject go = cell.mRootTransform.gameObject;
            entry.Init(go.transform);
            entry.AddAction(OnSelectItem);
            cell.BindUserData(entry);
        }

        private void OnSelectItem(uint skillGridId)
        {
           
            uint skillId = Sys_Transfiguration.Instance.clientShapeShiftPlans[curIndex].allRaceSkillGrids[skillGridId].Skillid;
            if (skillId != 0)
            {
                UIManager.OpenUI(EUIID.UI_Skill_Tips, false, new Tuple<uint, uint>(skillId, 0));
            }
        }

        private void OnCellChangeAddRace(InfinityGridCell cell, int index)
        {
            uint id = Sys_Transfiguration.Instance.GetAllRaceSkillGridData()[index].id;
            TransformAddCeil item = cell.mUserData as TransformAddCeil;
            if (Sys_Transfiguration.Instance.clientShapeShiftPlans[curIndex].allRaceSkillGrids.ContainsKey(id))
            {
                item.SetData(id, Sys_Transfiguration.Instance.clientShapeShiftPlans[curIndex].allRaceSkillGrids[id]);
            }
            else
            {
                item.SetData(id, null);
            }
            addCeilList.Add(item);
        }

        private void OnUseBtnClicked()
        {
            Sys_Transfiguration.Instance.ChangePlanReq((uint)curIndex);
        }

        private void OnRaceSkillToggleValueChanged(bool isOn)
        {
            if (isOn)
            {
                raceItemList.Clear();
                infinityCountRaceSkill = Sys_Transfiguration.Instance.GetRacesIds().Count;
                infinityGridRaceSkill.CellCount = infinityCountRaceSkill;
                infinityGridRaceSkill.ForceRefreshActiveCell();
            }
            raceSkillView.SetActive(isOn);
            allRaceSkillView.SetActive(!isOn);
        }

        private void OnAllRaceAddToggleValueChanged(bool isOn)
        {
            if (isOn)
            {
                infinityCountAddRace = Sys_Transfiguration.Instance.clientShapeShiftPlans[curIndex].allRaceSkillGrids.Count;
                if (infinityCountAddRace == 0)
                {
                    allRaceSkillView.SetActive(false);
                    noSkillGo.SetActive(true);
                }
                else
                {
                    allRaceSkillView.SetActive(true);
                    noSkillGo.SetActive(false);
                    infinityGridAddRace.CellCount = infinityCountAddRace;
                    infinityGridAddRace.ForceRefreshActiveCell();
                }
            } 
            raceSkillView.SetActive(!isOn);
        }

    }
}
