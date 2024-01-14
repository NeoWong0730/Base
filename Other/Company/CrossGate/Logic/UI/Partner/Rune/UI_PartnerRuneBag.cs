using Lib.Core;
using Logic.Core;
using Packet;
using System;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace Logic
{
    public class UI_RuneCellBase
    {
        public Transform transform;
        /// <summary>
        /// 底框选中
        /// </summary>
        public GameObject select01Go;
        /// <summary>
        /// 打勾选中
        /// </summary>
        public GameObject select02Go;

        private Image iconImage;
        private Image runeLevelImage;
        public Text nameText;
        private Text attrNameText;
        private Text attrValueText;
        private Text runeSkillNameText;
        public Text runeNumText;
        public UIRuneResultParam runeResultParam;
        public void BindGameObject(GameObject go)
        {
            transform = go.transform;
            select01Go = transform.Find("Image_Select").gameObject;
            select02Go = transform.Find("Image_Select1").gameObject;
            iconImage = transform.Find("PropItem/Btn_Item/Image_Icon")?.GetComponent<Image>();
            runeLevelImage = transform.Find("PropItem/Image_RuneRank")?.GetComponent<Image>();
            nameText = transform.Find("Text_name").GetComponent<Text>();
            attrNameText = transform.Find("Text_Property").GetComponent<Text>();
            attrValueText = transform.Find("Text_Property/Text").GetComponent<Text>();
            runeSkillNameText = transform.Find("Text_Skill").GetComponent<Text>();
            runeNumText = transform.Find("Text_Num").GetComponent<Text>();
        }

        public void SetRuneData(UIRuneResultParam runeResultParam)
        {
            this.runeResultParam = runeResultParam;
            Refresh();
        }

        public void Refresh()
        {
            CSVRuneInfo.Data runeInfo = CSVRuneInfo.Instance.GetConfData(this.runeResultParam.RuneId);
            if (null != runeInfo)
            {                
                ImageHelper.SetIcon(iconImage, runeInfo.icon);
                ImageHelper.SetIcon(runeLevelImage, Sys_Partner.Instance.GetRuneLevelImageId(runeInfo.rune_lvl));
                runeLevelImage?.gameObject.SetActive(true);
                iconImage.enabled = true;
                TextHelper.SetText(nameText, runeInfo.rune_name);
                if (runeInfo.rune_attribute.Count >= 2)
                {
                    uint attrId = runeInfo.rune_attribute[0];
                    uint attrValue = runeInfo.rune_attribute[1];
                    CSVAttr.Data attrInfo = CSVAttr.Instance.GetConfData(attrId);
                    if (null != attrInfo)
                    {
                        TextHelper.SetText(attrNameText, attrInfo.name);
                        TextHelper.SetText(attrValueText, LanguageHelper.GetTextContent(2006142u, attrInfo.show_type == 1 ? attrValue.ToString() : attrValue / 100.0f + "%"));
                    }
                }
                if (runeInfo.rune_passiveskillID != 0)
                {
                    CSVPassiveSkillInfo.Data passInfo = CSVPassiveSkillInfo.Instance.GetConfData(runeInfo.rune_passiveskillID);
                    if (null != passInfo)
                    {
                        TextHelper.SetText(runeSkillNameText, passInfo.name);
                        runeSkillNameText.gameObject.SetActive(true);
                    }
                }
                else
                {
                    runeSkillNameText.text = "";
                }
                TextHelper.SetText(runeNumText, this.runeResultParam.Count.ToString());
            }
        }
    }

    public class UI_Rune_DecomposeItem
    {
        public UI_RuneCellBase uI_RuneCellBase;
        public GameObject gameObject;
        private Button click;
        private GameObject viewNumGo;
        private InputField input;
        private Button btnSub;
        private Button btnAdd;

        private Action<UI_Rune_DecomposeItem> action;
        public int index;
        public bool deComposeState;
        public bool isSelect;
        public int DefaultComposeCount = 0;
        public int curComposeCount;        
        public void Init(Transform transform)
        {
            gameObject = transform.gameObject;
            uI_RuneCellBase = new UI_RuneCellBase();
            uI_RuneCellBase.BindGameObject(gameObject);

            click = transform.Find("Image_BG").GetComponent<Button>();
            click.onClick.AddListener(SelectStateView);

            viewNumGo = transform.Find("View_Num").gameObject;

            input = viewNumGo.transform.Find("InputField_Number").GetComponent<InputField>();
            input.contentType = InputField.ContentType.IntegerNumber;
            input.keyboardType = TouchScreenKeyboardType.NumberPad;
            input.onEndEdit.AddListener(OnInputEnd);

            btnSub = viewNumGo.transform.Find("Btn_Min").GetComponent<Button>();
            UI_LongPressButton LongPressSubButton = btnSub.gameObject.AddComponent<UI_LongPressButton>();
            LongPressSubButton.interval = 0.3f;
            LongPressSubButton.bPressAcc = true;
            LongPressSubButton.onRelease.AddListener(OnClickBtnSub);
            LongPressSubButton.OnPressAcc.AddListener(OnClickBtnSub);

            btnAdd = viewNumGo.transform.Find("Btn_Add").GetComponent<Button>();
            UI_LongPressButton LongPressAddButton = btnAdd.gameObject.AddComponent<UI_LongPressButton>();
            LongPressAddButton.interval = 0.3f;
            LongPressAddButton.bPressAcc = true;
            LongPressAddButton.onRelease.AddListener(OnClickBtnAdd);
            LongPressAddButton.OnPressAcc.AddListener(OnClickBtnAdd);
        }

        public void AddListtener(Action<UI_Rune_DecomposeItem> action)
        {
            this.action = action;
        }

        private void SelectStateView()
        {
            if (deComposeState)
            {
                uI_RuneCellBase.select01Go.SetActive(!isSelect);
                uI_RuneCellBase.select02Go.SetActive(!isSelect);
                isSelect = !isSelect;
                if (isSelect)
                {
                    curComposeCount = (int)uI_RuneCellBase.runeResultParam.Count;
                    input.text = curComposeCount.ToString();
                    action?.Invoke(this);
                }
                else
                {
                    curComposeCount = DefaultComposeCount;
                    action?.Invoke(this);
                }
                viewNumGo.SetActive(isSelect);
                uI_RuneCellBase.runeNumText.gameObject.SetActive(!isSelect);
            }
        }

        public void DescomposeState(bool isDecompose)
        {
            deComposeState = isDecompose;
            if (!deComposeState)
            {
                isSelect = false;
                uI_RuneCellBase.select01Go.SetActive(false);
                uI_RuneCellBase.select02Go.SetActive(false);
                viewNumGo.SetActive(false);
                uI_RuneCellBase.runeNumText.gameObject.SetActive(true);
            }
        }


        public void SetRuneData(UIRuneResultParam runeResultParam, int index, bool isSelect, int selectNum)
        {
            curComposeCount = selectNum;
            this.isSelect = isSelect;
            uI_RuneCellBase.SetRuneData(runeResultParam);
            this.index = index;
            this.Refresh();
        }

        public void Refresh()
        {

            uI_RuneCellBase.Refresh();
            uI_RuneCellBase.select01Go.SetActive(isSelect);
            uI_RuneCellBase.select02Go.SetActive(isSelect);
            if (isSelect)
            {
                input.text = curComposeCount.ToString();
            }
            viewNumGo.SetActive(isSelect);
            uI_RuneCellBase.runeNumText.gameObject.SetActive(!isSelect);

        }

        private void OnInputEnd(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                int result = int.Parse(s);
                if (result <= 0)
                {
                    curComposeCount = DefaultComposeCount;
                }
                else
                {
                    int maxCount = (int)uI_RuneCellBase.runeResultParam.Count;
                    curComposeCount = result > maxCount ? maxCount : result;
                }
            }
            else
            {
                curComposeCount = DefaultComposeCount;
            }

            input.text = curComposeCount.ToString();
            UpdateCostPirce();
        }

        private void UpdateCostPirce()
        {
            //发送消息
            action?.Invoke(this);
        }

        private void OnClickBtnSub()
        {
            if (curComposeCount > 0)
            {
                curComposeCount--;
            }

            input.text = curComposeCount.ToString();
            UpdateCostPirce();
        }

        private void OnClickBtnAdd()
        {
            int maxCount = (int)uI_RuneCellBase.runeResultParam.Count;
            if (curComposeCount < maxCount)
            {
                curComposeCount++;
            }

            input.text = curComposeCount.ToString();
            UpdateCostPirce();
        }

    }

    public class UIPartnerRunekPopdownItem : UISelectableElement
    {
        public uint runeLevel;
        public string runeLevel_Name;

        public Text text;
        public Button button;
        public GameObject highlight;

        protected override void Loaded()
        {
            button = transform.GetComponent<Button>();
            text = transform.Find("Text").GetComponent<Text>();
            highlight = transform.Find("Image_Select").gameObject;
            button.onClick.AddListener(OnBtnClicked);
        }

        public void SetHighlight(bool setHighLight = false)
        {
            highlight.SetActive(setHighLight);
        }

        public void Refresh(uint runeLevel, int index)
        {
            this.runeLevel = runeLevel;

            if (runeLevel != 0)
            {
                TextHelper.SetText(text, runeLevel);
                runeLevel_Name = text.text;
            }
            else
            {
                runeLevel_Name = LanguageHelper.GetTextContent(2009431);
                TextHelper.SetText(text, runeLevel_Name);
            }
        }
        private void OnBtnClicked()
        {
            onSelected?.Invoke((int)runeLevel, true);
        }
        public override void SetSelected(bool toSelected, bool force) { OnBtnClicked(); }
    }

    public class UI_PartnerRuneBag_Left_Cell
    {
        public int gridIndex;
        private Action<UI_PartnerRuneBag_Left_Cell> toggleAction;
        private CP_Toggle toggle;
        private Text textShow;
        private Text textLight;        
        public void Init(Transform transform)
        {
            toggle = transform.gameObject.GetComponent<CP_Toggle>();
            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener(OnToggleClick);

            textShow = toggle.transform.Find("Background/Label").GetComponent<Text>();
            textLight = toggle.transform.Find("Checkmark/Label").GetComponent<Text>();
        }

        private void OnToggleClick(bool _select)
        {
            if (_select)
            {
                toggleAction?.Invoke(this);
            }
        }

        public void AddListener(Action<UI_PartnerRuneBag_Left_Cell> _action)
        {
            toggleAction = _action;
        }

        public void OnSelect(bool _select)
        {
            toggle.SetSelected(_select, false);
        }

        public void ToggleOff()
        {
            toggle.SetSelected(false, false);
        }

        public void UpdateInfo(uint _lanId, int _index)
        {
            gridIndex = _index;

            if (_lanId == 0) //all
            {
                textShow.text = LanguageHelper.GetTextContent(2006003);
                textLight.text = LanguageHelper.GetTextContent(2006003);
            }
            else
            {
                textShow.text = LanguageHelper.GetTextContent(_lanId);
                textLight.text = LanguageHelper.GetTextContent(_lanId);
            }
        }
    }

    public class UI_PartnerRuneBag_Left
    {
        private CP_ToggleRegistry group;
        private InfinityGridLayoutGroup gridGroup;
        private Dictionary<GameObject, UI_PartnerRuneBag_Left_Cell> dicCells = new Dictionary<GameObject, UI_PartnerRuneBag_Left_Cell>();
        private int visualGridCount;
        private bool isInit;
        private IListener listener;
        private int curSelectIndex;

        private List<uint> listIds = new List<uint>();
        
        public void Init(Transform transform)
        {
            group = transform.Find("Toggles_search").GetComponent<CP_ToggleRegistry>();
            gridGroup = group.gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            gridGroup.minAmount = 9;
            gridGroup.updateChildrenCallback = UpdateChildrenCallback;

            for (int i = 0; i < gridGroup.transform.childCount; ++i)
            {
                Transform tran = gridGroup.transform.GetChild(i);
                UI_PartnerRuneBag_Left_Cell cell = new UI_PartnerRuneBag_Left_Cell();
                cell.Init(tran);
                cell.AddListener(OnSelectIndex);
                dicCells.Add(tran.gameObject, cell);
            }
            curSelectIndex = 0;
        }

        private void UpdateChildrenCallback(int index, Transform trans)
        {
            if (index < 0 || index >= visualGridCount)
                return;

            if (dicCells.ContainsKey(trans.gameObject))
            {
                UI_PartnerRuneBag_Left_Cell cell = dicCells[trans.gameObject];
                cell.UpdateInfo(listIds[index], index);
                cell.OnSelect(curSelectIndex == index);
            }
        }

        private void OnSelectIndex(UI_PartnerRuneBag_Left_Cell cell)
        {
            curSelectIndex = cell.gridIndex;
            cell.OnSelect(true);
            listener?.OnSelectListIndex(listIds[cell.gridIndex]);
        }

        public void OnSetMenu()
        {
            curSelectIndex = 0;
            if (!isInit)
            {
                listIds.Clear();
                listIds.Add(0); //id为0代表全部

                var dataList = CSVRuneTypeID.Instance.GetAll();
                for (int i = 0, len = dataList.Count; i < dataList.Count; i++)
                {
                    listIds.Add(dataList[i].typeID);
                }
            }

            visualGridCount = listIds.Count;
            gridGroup.SetAmount(visualGridCount);
            //group.EnsureValidState();
        }

        public void RegisterListener(IListener _listener)
        {
            listener = _listener;
        }

        public interface IListener
        {
            void OnSelectListIndex(uint _typeId);
        }
    }

    public class UI_PartnerRuneBagListParent
    {
        private InfinityGrid infinityGrid;
        private Dictionary<GameObject, UI_Rune_DecomposeItem> dicCells = new Dictionary<GameObject, UI_Rune_DecomposeItem>();
        private List<UI_Rune_DecomposeItem> scrpList = new List<UI_Rune_DecomposeItem>();
        private int visualGridCount;
        private IListener listener;

        private List<UIRuneResultParam> listIds = new List<UIRuneResultParam>();
        private List<UIRuneResultParam> selectRuneList = new List<UIRuneResultParam>();        
        public void Init(Transform transform)
        {
            infinityGrid = transform.Find("Scroll View").GetComponent<InfinityGrid>();
            infinityGrid.onCreateCell += OnCreateCell;
            infinityGrid.onCellChange += OnCellChange;
        }

        /// <summary>
        /// 滚动列表创建回调
        /// </summary>
        /// <param name="cell"></param>
        public void OnCreateCell(InfinityGridCell cell)
        {
            UI_Rune_DecomposeItem entry = new UI_Rune_DecomposeItem();
            GameObject go = cell.mRootTransform.gameObject;
            entry.Init(go.transform);
            entry.AddListtener(OnClick);
            scrpList.Add(entry);
            dicCells.Add(go, entry);
            cell.BindUserData(entry);
        }

        /// <summary>
        /// 滚动列表滚动回调
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        public void OnCellChange(InfinityGridCell cell, int index)
        {
            if (index < 0 || index >= visualGridCount)
                return;
            UI_Rune_DecomposeItem entry = cell.mUserData as UI_Rune_DecomposeItem;

            entry.SetRuneData(listIds[index], index, selectRuneList[index].Select, (int)selectRuneList[index].Count);
        }


        private void UpdateChildrenCallback(int index, Transform trans)
        {
            if (index < 0 || index >= visualGridCount)
                return;

            if (dicCells.ContainsKey(trans.gameObject))
            {
                UI_Rune_DecomposeItem cell = dicCells[trans.gameObject];
                cell.SetRuneData(listIds[index], index, selectRuneList[index].Select, (int)selectRuneList[index].Count);
            }
        }

        public void SetList(List<UIRuneResultParam> showList, List<UIRuneResultParam> selectRuneList)
        {
            listIds = showList;
            this.selectRuneList = selectRuneList;
            visualGridCount = showList.Count;
            infinityGrid.CellCount = visualGridCount;
            infinityGrid.ForceRefreshActiveCell();
        }

        public void DescomposeState(bool isDecompose)
        {
            for (int i = 0; i < scrpList.Count; i++)
            {
                scrpList[i].DescomposeState(isDecompose);
            }            
        }

        public void SetSelectList(List<UIRuneResultParam> selectRuneList)
        {
            this.selectRuneList = selectRuneList;
        }

        private void OnClick(UI_Rune_DecomposeItem uI_Rune_Item)
        {
            listener?.OnSelectListIndex(uI_Rune_Item);
        }

        public void RegisterListener(IListener _listener)
        {
            listener = _listener;
        }

        public interface IListener
        {
            void OnSelectListIndex(UI_Rune_DecomposeItem itemData);
        }
    }

    public class UI_PartnerRuneBag : UIParseCommon, UI_PartnerRuneBag_Left.IListener, UI_PartnerRuneBagListParent.IListener
    {
        public CP_PopdownList popdownList;
        public UIElementContainer<UIPartnerRunekPopdownItem, uint> popdownVds = new UIElementContainer<UIPartnerRunekPopdownItem, uint>();
        private UI_PartnerRuneBag_Left leftList;
        private UI_PartnerRuneBagListParent cellParentView;

       
        private Button decomposeBtn;
        private Text btnText;
        private Button cancelBtn;

        private bool isDeComposeState = false;

        private int selectRuneType = -1;
        private int selectRuneLevel = -1;
        private int selectRuneIndex = -1;

        public List<UIRuneResultParam> allRuneList = new List<UIRuneResultParam>();
        public List<UIRuneResultParam> selectRuneList = new List<UIRuneResultParam>();
        List<uint> level = new List<uint>();

        private IListener listener;

        protected override void Parse()
        {
            popdownList = transform.Find("GameObject/View_PopupList").GetComponent<CP_PopdownList>();
            leftList = new UI_PartnerRuneBag_Left();
            leftList.Init(transform.Find("Scroll_Sort"));
            leftList.RegisterListener(this);
            cellParentView = new UI_PartnerRuneBagListParent();
            cellParentView.Init(transform.Find("GameObject/View_sort"));
            cellParentView.RegisterListener(this);

            decomposeBtn = transform.Find("GameObject/Btn_01").GetComponent<Button>();
            decomposeBtn.onClick.AddListener(OnDeComposeSureOrSta);

            btnText = transform.Find("GameObject/Btn_01/Text_01").GetComponent<Text>();

            cancelBtn = transform.Find("GameObject/Btn_02").GetComponent<Button>();
            cancelBtn.onClick.AddListener(() =>
            {
                isDeComposeState = false;
                for (int i = 0; i < selectRuneList.Count; i++)
                {
                    selectRuneList[i].Count = 0;
                }
                cellParentView.DescomposeState(isDeComposeState);
                DecomposeBtnState();
                SetAddView();
            });
        }

        private void OnDeComposeSureOrSta()
        {
            if (!isDeComposeState)
            {
                isDeComposeState = true;
                cellParentView.DescomposeState(isDeComposeState);
                DecomposeBtnState();
            }
            else
            {
                PromptBoxParameter.Instance.OpenPromptBox(2006181u, 0, () =>
                {
                    //特殊处理 需要把分出去的堆再合并回去
                    Dictionary<uint, uint> sendDic = new Dictionary<uint, uint>();
                    for (int i = 0; i < selectRuneList.Count; i++)
                    {
                        if (sendDic.ContainsKey(selectRuneList[i].RuneId))
                        {
                            sendDic[selectRuneList[i].RuneId] += selectRuneList[i].Count;
                        }
                        else
                        {
                            sendDic.Add(selectRuneList[i].RuneId, selectRuneList[i].Count);
                        }
                    }
                    List<PartnerRune> sendList = new List<PartnerRune>();
                    List<uint> keyList = new List<uint>(sendDic.Keys);

                    for (int i = 0; i < keyList.Count; i++)
                    {
                        uint key = keyList[i];
                        var num = sendDic[key];
                        if(num > 0)
                        {
                            PartnerRune temp = new PartnerRune();
                            temp.Id = key;
                            temp.Num = sendDic[key];
                            sendList.Add(temp);
                        }
                    }
                    Sys_Partner.Instance.PartnerRuneDecomposeReq(sendList);
                });
            }
        }

        private void DecomposeBtnState()
        {
            if (!isDeComposeState)
            {
                TextHelper.SetText(btnText, 2006139); // 2006138
                ButtonHelper.Enable(decomposeBtn, true);
                ImageHelper.SetImageGray(decomposeBtn, false, true);
                cancelBtn.gameObject.SetActive(false);
            }
            else
            {
                TextHelper.SetText(btnText, 2006138); // 
                bool grayState = GetDeComposeListState();
                ButtonHelper.Enable(decomposeBtn, grayState);
                cancelBtn.gameObject.SetActive(true);
            }
        }

        private bool GetDeComposeListState()
        {
            for (int i = 0; i < selectRuneList.Count; i++)
            {
                if (selectRuneList[i].Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        private long GetDeComposeListNum()
        {
            long allCount = 0;
            for (int i = 0; i < selectRuneList.Count; i++)
            {
                if (selectRuneList[i].Count > 0)
                {
                    CSVRuneInfo.Data data = CSVRuneInfo.Instance.GetConfData(selectRuneList[i].RuneId);
                    allCount += data.disintegrate * selectRuneList[i].Count;
                }
            }
            return allCount;
        }

        public override void Show()
        {
            base.Show();
            ProcessEventsForEnable(false);
            ProcessEventsForEnable(true);

            leftList.OnSetMenu();
            PopdownListBuild();
            DecomposeBtnState();
        }

        public override void Hide()
        {
            base.Hide();
            ProcessEventsForEnable(false);

            selectRuneType = -1;
            selectRuneLevel = -1;
            selectRuneIndex = -1;
        }

        private void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Partner.Instance.eventEmitter.Handle(Sys_Partner.EEvents.OnRuneDecomposeCallBack, OnRuneDecomposeCallBack, toRegister);
        }

        private void OnRuneDecomposeCallBack()
        {
            isDeComposeState = false;
            for (int i = 0; i < selectRuneList.Count; i++)
            {
                selectRuneList[i].Count = 0;
            }
            InitBagView();
            cellParentView.DescomposeState(isDeComposeState);
            DecomposeBtnState();
            SetAddView();
        }

        public void Update()
        {
            popdownVds.Update();
        }

        public override void OnDestroy()
        {
            popdownVds.Clear();
            base.OnDestroy();
        }

        private void PopdownListBuild()
        {
            level.Clear();
            level.Add(0);
            var dataList = CSVRuneLvlID.Instance.GetAll();
            for (int i = 0, len = dataList.Count; i < len; i++)
            {
                level.Add(dataList[i].lvlID);
            }

            popdownVds.BuildOrRefresh(popdownList.optionProto, popdownList.optionParent, level, (vd, data, index) =>
            {
                vd.SetUniqueId((int)data);
                vd.SetSelectedAction((_selectRuneLevel, force) =>
                {
                    int vdIndex = index;
                    selectRuneIndex = vdIndex;

                    popdownVds.ForEach((e) =>
                    {
                        e.SetHighlight(false);
                    });
                    vd.SetHighlight(true);

                    popdownList.Expand(false);
                    popdownList.SetSelected(vd.runeLevel_Name);

                    this.selectRuneLevel = _selectRuneLevel;
                    OnSelectListIndex((uint)(selectRuneType == -1 ? 0 : selectRuneType));
                });
                vd.Refresh(data, index);
                vd.SetHighlight(false);
            });

            if (selectRuneIndex == -1)
            {
                selectRuneIndex = 0;
                if (popdownVds.Count > 0)
                {
                    popdownVds[selectRuneIndex].SetSelected(true, true);
                }
            }
            else
            {
                if (0 <= selectRuneIndex && selectRuneIndex < level.Count)
                {
                    popdownVds[selectRuneIndex].SetSelected(true, true);
                }
            }
        }

        public void OnSelectListIndex(uint _typeId)
        {
            isDeComposeState = false;
            selectRuneType = (int)_typeId;
            InitBagView();
            SetAddView();
        }

        private void InitBagView()
        {
            allRuneList = Sys_Partner.Instance.GetRuneBagList((uint)selectRuneType, (uint)selectRuneLevel);
            selectRuneList.Clear();
            for (int i = 0; i < allRuneList.Count; i++)
            {
                UIRuneResultParam temp = new UIRuneResultParam();
                temp.RuneId = allRuneList[i].RuneId;
                temp.Count = 0;
                selectRuneList.Add(temp);
            }
            cellParentView.DescomposeState(isDeComposeState);
            DecomposeBtnState();
            cellParentView.SetList(allRuneList, selectRuneList);
        }

        private void SetAddView()
        {
            listener?.RefreshExpState(GetDeComposeListState(), GetDeComposeListNum());
        }

        public void OnSelectListIndex(UI_Rune_DecomposeItem itemData)
        {
            if (itemData.index <= selectRuneList.Count - 1)
            {
                selectRuneList[itemData.index].Count = (uint)itemData.curComposeCount;
                selectRuneList[itemData.index].Select = itemData.isSelect;
            }

            cellParentView.SetSelectList(selectRuneList);
            SetAddView();
            DecomposeBtnState();
        }

        public void Register(IListener _listener)
        {
            listener = _listener;
        }

        public interface IListener
        {
            void RefreshExpState(bool show, long num);
        }
    }
}
