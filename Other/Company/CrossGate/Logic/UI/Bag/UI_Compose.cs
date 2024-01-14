using Lib.Core;
using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Framework;
using Table;
using UnityEngine.EventSystems;
using Packet;
using System;

namespace Logic
{
    public class UI_Compose_TypeList_Sub
    {
        private Transform transform;
        private CP_Toggle parToggle;
        private Text textLight;
        private Text textDark;
        public CSVCompose.Data config;
        private Action<UI_Compose_TypeList_Sub> action;
        public void Bind(GameObject go)
        {
            transform = go.transform;
            parToggle = go.GetComponent<CP_Toggle>();
            parToggle.onValueChanged.AddListener(OnToggleClick);
            textLight = transform.Find("Image_Select/Text_Light").GetComponent<Text>();
            textDark = transform.Find("Text_Dark").GetComponent<Text>();

        }

        private void OnToggleClick(bool isOn)
        {
            if (isOn)
            {
                action?.Invoke(this);
            }
        }

        public void AddListener(Action<UI_Compose_TypeList_Sub> _action)
        {
            action = _action;
        }

        public void SetSub(CSVCompose.Data _config, uint subId)
        {
            config = _config;
            SetSelectActive(subId);
            textLight.text = LanguageHelper.GetTextContent(config.name_id);
            textDark.text = LanguageHelper.GetTextContent(config.name_id);
        }

        public void SetSelectActive(uint currentId, bool ismessage = false)
        {
            parToggle.SetSelected(config.id == currentId, ismessage);
        }

        public void HideToggle()
        {
            parToggle.SetSelected(false, false);
        }

    }

    public class UI_Compose_TypeList_ParentSub
    {
        private Transform transform;
        private Toggle parToggle;
        private GameObject subToggleGo;
        private Transform subTogglePar;
        private Text textLight;
        private Text textDark;
        public uint parId;
        private Action<UI_Compose_TypeList_ParentSub> action;
        private Action<UI_Compose_TypeList_Sub> actionSc;
        private List<UI_Compose_TypeList_Sub> subLists = new List<UI_Compose_TypeList_Sub>();
        private Image arrowToggleGo;
        public void Bind(GameObject go)
        {

            transform = go.transform;


            subToggleGo = transform.Find("Toggle_Select01").gameObject;
            subTogglePar = transform.Find("Content_Small");

            parToggle = go.GetComponent<Toggle>();
            parToggle.onValueChanged.AddListener(OnToggleClick);
            textLight = transform.Find("GameObject/Image_Select/Text_Light").GetComponent<Text>();
            textDark = transform.Find("GameObject/Text_Dark").GetComponent<Text>();
            arrowToggleGo = transform.Find("GameObject/Image_Frame").GetComponent<Image>();

        }

        private void OnToggleClick(bool isOn)
        {
            if (isOn)
            {
                action?.Invoke(this);
            }
        }

        public void AddListener(Action<UI_Compose_TypeList_ParentSub> _action, Action<UI_Compose_TypeList_Sub> _actionSc)
        {
            action = _action;
            actionSc = _actionSc;
        }
        public void SetSub(uint _parId, uint _subId)
        {
            parId = _parId;
            textLight.text = LanguageHelper.GetTextContent(_parId);
            textDark.text = LanguageHelper.GetTextContent(_parId);
            List<CSVCompose.Data> sencedMenu = Sys_Bag.Instance.SecondMenus(_parId);
            for (int i = 0; i < sencedMenu.Count; i++)
            {
                UI_Compose_TypeList_Sub uI_Pet_Seal_TypeList_Sub = new UI_Compose_TypeList_Sub();
                GameObject go = GameObject.Instantiate<GameObject>(subToggleGo, subTogglePar);
                uI_Pet_Seal_TypeList_Sub.Bind(go);
                uI_Pet_Seal_TypeList_Sub.SetSub(sencedMenu[i], _subId);
                uI_Pet_Seal_TypeList_Sub.AddListener(SubClicked);
                subLists.Add(uI_Pet_Seal_TypeList_Sub);
                go.SetActive(true);
            }
        }

        public void SubClicked(UI_Compose_TypeList_Sub uI_Pet_Seal_TypeList_Sub)
        {
            actionSc?.Invoke(uI_Pet_Seal_TypeList_Sub);
        }

        public void SetParState(uint _parId, uint _subId, bool isSub)
        {
            if (parId == _parId && !subTogglePar.gameObject.activeSelf)
            {
                for (int i = 0; i < subLists.Count; i++)
                {
                    subLists[i].SetSelectActive(_subId);
                }
                subTogglePar.gameObject.SetActive(true);
            }
            else if (parId == _parId && subTogglePar.gameObject.activeSelf && !isSub)
            {
                subTogglePar.gameObject.SetActive(false);
            }
            else if (parId != _parId && subTogglePar.gameObject.activeSelf)
            {
                HideAllSub();
                subTogglePar.gameObject.SetActive(false);
            }
            SetArrow(_parId == parId);
        }

        public void HideAllSub()
        {
            for (int i = 0; i < subLists.Count; i++)
            {
                subLists[i].HideToggle();
            }
        }

        private void SetArrow(bool select)
        {
            float rotateZ = select ? 180f : 0f;
            arrowToggleGo.rectTransform.localRotation = Quaternion.Euler(0f, 0f, rotateZ);
        }

        public void InitToggle()
        {
            if (parToggle.isOn)
            {
                action?.Invoke(this);
                parToggle.isOn = true;
            }
            else
            {
                parToggle.isOn = true;
            }
            SetArrow(true);
        }
    }

    public class UI_Compose_TypeList
    {
        // Dictionary<uint, List<uint>> composeDic = new Dictionary<uint, List<uint>>();
        private Transform transform;

        private bool listInit = false;
        private uint currentParId = 0;
        private uint currentSubId = 0;
        private GameObject toggleSelectGo;
        private Transform parentGo;
        private ScrollRect scrollRect;
        private IListener listener;
        private List<UI_Compose_TypeList_ParentSub> sealTypeSubs = new List<UI_Compose_TypeList_ParentSub>();
        private VerticalLayoutGroup partnerLayout;
        private GridLayoutGroup subLayout;
        private RectTransform partnerRectTransform;
        public void Init(Transform trans)
        {
            transform = trans;
            toggleSelectGo = transform.Find("Scroll01/Toggle_Select01").gameObject;
            partnerRectTransform = toggleSelectGo.GetComponent<RectTransform>();
            partnerLayout = transform.Find("Scroll01/Content").GetComponent<VerticalLayoutGroup>();
            subLayout = transform.Find("Scroll01/Toggle_Select01/Content_Small").GetComponent<GridLayoutGroup>();
            parentGo = transform.Find("Scroll01/Content");
            scrollRect = transform.Find("Scroll01").GetComponent<ScrollRect>();
        }

        public void Show(uint _currentParId, uint _currentSubId)
        {
            currentSubId = _currentSubId;
            currentParId = _currentParId;
            if (!listInit)
            {
                sealTypeSubs.Clear();
                List<uint> sub = Sys_Bag.Instance.GetAllMenu();
                for (int i = 0; i < sub.Count; i++)
                {
                    UI_Compose_TypeList_ParentSub uI_Pet_Seal_TypeList_Sub = new UI_Compose_TypeList_ParentSub();
                    GameObject go = GameObject.Instantiate<GameObject>(toggleSelectGo, parentGo);
                    uI_Pet_Seal_TypeList_Sub.Bind(go);
                    uI_Pet_Seal_TypeList_Sub.SetSub(sub[i], currentSubId);
                    uI_Pet_Seal_TypeList_Sub.AddListener(OnClikcPar, OnSecondClick);
                    sealTypeSubs.Add(uI_Pet_Seal_TypeList_Sub);
                    go.SetActive(true);
                }
                listInit = true;
                ResetList(true);
            }
            else
            {
                ResetList(true);
            }
            FrameworkTool.ForceRebuildLayout(transform.gameObject);
        }

        public void SetViewPos()
        {
            //当前展开的父菜单所在位置
            List<uint> partnerConfig = Sys_Bag.Instance.GetAllMenu();
            int parnterCount = 1;

            for (int i = 0; i < partnerConfig.Count; i++)
            {
                if (partnerConfig[i] == currentParId)
                {
                    parnterCount = i + 1;
                }
            }

            List<CSVCompose.Data> sencedMenu = Sys_Bag.Instance.SecondMenus(currentParId);
            int subCount = 1;
            for (int i = 0; i < sencedMenu.Count; i++)
            {
                if (sencedMenu[i].id == currentSubId)
                {
                    subCount = i + 1;
                }
            }


            //float partnerTop = partnerLayout.padding.top;
            float partnerSpaY = partnerLayout.spacing;
            float partnerCellY = partnerRectTransform.sizeDelta.y;

            //float subTop = subLayout.padding.top;
            float subSpaY = subLayout.spacing.y;
            float subCelly = subLayout.cellSize.y;

            float content = partnerLayout.transform.parent.GetComponent<RectTransform>().sizeDelta.y / 2;
            float p_y = (parnterCount) * partnerCellY + (parnterCount - 1) * partnerSpaY
                + subCount * subCelly + (subCount - 1) * subSpaY - content;
            scrollRect.StopMovement();
            scrollRect.content.localPosition = new Vector2(0, p_y);
            //scrollRect.content.DOLocalMoveY(p_y, 0.3f);// new Vector2(0, , 0);
        }

        private void OnClikcPar(UI_Compose_TypeList_ParentSub uI_Compose_TypeList_TypeList_Sub)
        {
            currentParId = uI_Compose_TypeList_TypeList_Sub.parId;

            List<CSVCompose.Data> sencedMenu = Sys_Bag.Instance.SecondMenus(currentParId);
            bool isCurrentSub = false;
            for (int i = 0; i < sencedMenu.Count; i++)
            {
                if (sencedMenu[i].id == currentSubId)
                {
                    isCurrentSub = true;
                    break;
                }
            }
            if (!isCurrentSub && sencedMenu.Count >= 1)
            {
                currentSubId = sencedMenu[0].id;
                listener?.OnSelectSecondMenu(currentSubId);
            }
            ResetList(false);
        }

        private void OnSecondClick(UI_Compose_TypeList_Sub uI_Compose_TypeList_Sub)
        {
            currentSubId = uI_Compose_TypeList_Sub.config.id;
            ResetList(false, true);
            listener?.OnSelectSecondMenu(currentSubId);
        }
        private void ResetList(bool setInit, bool isSub = false)
        {
            for (int i = 0; i < sealTypeSubs.Count; i++)
            {
                UI_Compose_TypeList_ParentSub item = sealTypeSubs[i];
                if (setInit && item.parId == currentParId)
                    item.InitToggle();
                item.SetParState(currentParId, currentSubId, isSub);
            }
            FrameworkTool.ForceRebuildLayout(transform.gameObject);
        }


        public void RegisterListener(IListener _listener)
        {
            listener = _listener;
        }

        public interface IListener
        {
            void OnSelectMainMenu(uint _bingHun);
            void OnSelectSecondMenu(uint _smallHun);
        }
    }

    public class UI_Compose_Item : UIComponent
    {
        public PropItem propItem;
        public Text nameText;
        public GameObject lineGo;
        public EUIID eUIID;
        protected override void Loaded()
        {
            propItem = new PropItem();
            propItem.BindGameObject(transform.Find("Item").gameObject);
            nameText = transform.Find("Text_Name").GetComponent<Text>();
            lineGo = transform.Find("Image_LeftLine").gameObject;
        }

        public void SetItemResUIID(EUIID uiid)
        {
            eUIID = uiid;
        }

        public override void SetData(params object[] arg)
        {
            if (arg.Length >= 2)
            {
                uint id = Convert.ToUInt32(arg[0]);
                uint itemNum = Convert.ToUInt32(arg[1]);
                propItem.SetData(new MessageBoxEvt(eUIID, new PropIconLoader.ShowItemData(id,
                itemNum, true, false, false, false, false, true, true, true)));
                CSVItem.Data item = CSVItem.Instance.GetConfData(id);
                if (null != item)
                {
                    TextHelper.SetText(nameText, item.name_id);
                }
            }

        }

        public void SetLineState(bool isShow)
        {
            lineGo.SetActive(isShow);
        }
    }

    public class UI_Compose : UIBase, UI_Compose_TypeList.IListener
    {
        private InputField mInputField;
        private Image icon;
        private Text itemName;
        private Text itemNum;
        private Button itemBtn;

        private Button mCloseBtn;
        private Button mOkBtn;
        private Button mAdd;
        private Button mSub;
        private Slider mSlider;
        private GameObject composeSubGo;
        private Transform undoParent;

        private UI_Compose_TypeList sealTypeList;
        private uint MaxInputCount;

        private uint currentMainId;
        private uint currentSecondId;
        List<ItemIdCount> composeItemData;
        CSVCompose.Data composeData = null;
        List<List<uint>> undos = new List<List<uint>>();
        List<UI_Compose_Item> composeItems = new List<UI_Compose_Item>();
        public int Count
        {
            get
            {
                if (mInputField.text == "")
                {
                    return 1;
                }
                else
                {
                    return int.Parse(mInputField.text);
                }
            }
        }

        protected override void OnOpen(object arg)
        {
            if (null == arg)
            {
                composeData = Sys_Bag.Instance.GetComposeDataWhenCommon();
            }
            else
            {
                uint composeId = (uint)arg;
                composeData = CSVCompose.Instance.GetConfData(composeId);
                if (null == composeData)
                {
                    composeData = Sys_Bag.Instance.GetComposeDataWhenCommon();
                }
            }
        }

        protected override void OnLoaded()
        {
            icon = transform.Find("Animator/View_Right/PropItem/Btn_Item/Image_Icon").GetComponent<Image>();
            itemNum = transform.Find("Animator/View_Right/PropItem/Text_Number").GetComponent<Text>();
            itemName = transform.Find("Animator/View_Right/PropItem/Text_Name").GetComponent<Text>();
            itemBtn = transform.Find("Animator/View_Right/PropItem/Btn_Item").GetComponent<Button>();
            itemBtn.onClick.AddListener(() =>
            {
                if (null != composeData)
                {
                    uint id = composeItemData[0].id;
                    if (composeData.SeePet != 0)
                    {
                        ItemData mItemData = new ItemData(0, 0, id, 1, 0, false, false, null, null, 0);
                        PropMessageParam propParam = new PropMessageParam();
                        propParam.itemData = mItemData;
                        propParam.showBtnCheck = true;
                        propParam.targetEUIID = (uint)EUIID.UI_Pet_BookReview;
                        propParam.checkOpenParam = composeData.SeePet;
                        propParam.sourceUiId = EUIID.UI_Compose;
                        UIManager.OpenUI(EUIID.UI_Prop_Message, false, propParam);
                    }
                    else
                    {
                        PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(id, 0, false, false, false, false, false, false, false);
                        itemData.bShowBtnNo = false;
                        UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_Compose, itemData));

                    }
                }
            });

            mCloseBtn = transform.Find("Animator/View_TipsBg01_Big/Btn_Close").GetComponent<Button>();
            mOkBtn = transform.Find("Animator/View_Right/Btn_01").GetComponent<Button>();
            mSlider = transform.Find("Animator/View_Right/Image_Exchange_Number/Slider").GetComponent<Slider>();
            mAdd = transform.Find("Animator/View_Right/Image_Exchange_Number/Btn_Add").GetComponent<Button>();


            UI_LongPressButton LongPressAddButton = mAdd.gameObject.AddComponent<UI_LongPressButton>();
            LongPressAddButton.interval = 0.3f;
            LongPressAddButton.bPressAcc = true;
            LongPressAddButton.onRelease.AddListener(Add);
            LongPressAddButton.OnPressAcc.AddListener(Add);

            mSub = transform.Find("Animator/View_Right/Image_Exchange_Number/Btn_Min").GetComponent<Button>();
            UI_LongPressButton LongPressSubButton = mSub.gameObject.AddComponent<UI_LongPressButton>();
            LongPressSubButton.interval = 0.3f;
            LongPressSubButton.bPressAcc = true;
            LongPressSubButton.onRelease.AddListener(Sub);
            LongPressSubButton.OnPressAcc.AddListener(Sub);

            mInputField = transform.Find("Animator/View_Right/Image_Exchange_Number/InputField_Number").GetComponent<InputField>();
            undoParent = transform.Find("Animator/View_Right/View_BG/Viewport");
            composeSubGo = transform.Find("Animator/View_Right/View_BG/Viewport/View_Item01").gameObject;

            sealTypeList = new UI_Compose_TypeList();
            sealTypeList.Init(transform.Find("Animator/View_TypeList"));
            sealTypeList.RegisterListener(this);
            mSlider.onValueChanged.AddListener(OnSliderValueChanged);
            mOkBtn.onClick.AddListener(Undo);
            mCloseBtn.onClick.AddListener(() =>
            {
                UIManager.CloseUI(EUIID.UI_Compose);
            });
            mInputField.onEndEdit.AddListener(str =>
            {
                uint count;
                try
                {
                    if (str == "")
                        count = 1;
                    else
                        count = uint.Parse(str);

                    if (count > MaxInputCount)
                    {
                        count = MaxInputCount;
                    }
                    count = (uint)Mathf.Clamp(count, 1, composeData.maxBatchCompse);
                }
                catch (System.Exception)
                {
                    DebugUtil.Log(ELogType.eBag, "格式不正确");
                    count = 1;
                }
                mInputField.text = count.ToString();
                UpdateSlider((int)count);
                UpdateUndoItems();
                UpdateButtonState();
            });
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Bag.Instance.eventEmitter.Handle<uint>(Sys_Bag.EEvents.ComposeSpecilEvent, ComposeSpecilEvent, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle(Sys_Bag.EEvents.ComposeSuccess, OnComposeSuccessRefresh, toRegister);
        }

        private void ComposeSpecilEvent(uint composeId)
        {
            composeData = CSVCompose.Instance.GetConfData(composeId);
            if (null == composeData)
            {
                composeData = Sys_Bag.Instance.GetComposeDataWhenCommon();
            }
            undos = GetComposeSubItem();
            Refresh();
        }

        protected override void OnShow()
        {
            OnShowRefresh();
        }

        private void Refresh()
        {
            currentMainId = composeData.SynthesisTab;
            currentSecondId = composeData.id;
            composeItemData = Sys_Bag.Instance.GetComposeItemId(composeData.id);
            undos = GetComposeSubItem();
            sealTypeList.Show(currentMainId, currentSecondId);
            RefreshUI();
        }

        private void OnShowRefresh()
        {
            Refresh();
            sealTypeList.SetViewPos();
        }

        private void OnComposeSuccessRefresh()
        {
            undos = GetComposeSubItem();
            UpdateUndoItems();
            UpdateSlider(1);
            UpdateButtonState();
        }

        protected override void OnDestroy()
        {
            composeItems.Clear();
        }

        private void RefreshUI()
        {
            mInputField.text = "1";
            UpdateSlider(1);
            icon.enabled = true;
            itemNum.gameObject.SetActive(true);
            itemName.gameObject.SetActive(true);
            if (composeItemData.Count > 0)
            {
                ImageHelper.SetIcon(icon, composeItemData[0].CSV.icon_id);
                TextHelper.SetText(itemName, composeItemData[0].CSV.name_id);
            }
            UpdateUndoItems();
            UpdateButtonState();
        }

        private void ResetComposeItems()
        {
            for (int i = 0; i < undos.Count; i++)
            {
                if (composeItems.Count <= i)
                {
                    GameObject go = GameObject.Instantiate(composeSubGo, undoParent);
                    UI_Compose_Item item = AddComponent<UI_Compose_Item>(go.transform);
                    item.SetItemResUIID(EUIID.UI_Compose);
                    item.SetData(undos[i][0], undos[i][1] * Count);
                    item.SetLineState(!(i == undos.Count - 1));
                    composeItems.Add(item);
                    go.SetActive(true);
                }
                else
                {
                    composeItems[i].SetData(undos[i][0], undos[i][1] * Count);
                    composeItems[i].SetLineState(!(i == undos.Count - 1));
                    composeItems[i].gameObject.SetActive(true);
                }
            }

            for (int i = undos.Count; i < composeItems.Count; i++)
            {
                composeItems[i].gameObject.SetActive(false);
            }
        }

        private void UpdateUndoItems()
        {
            if (composeItemData.Count > 0)
            {
                itemNum.text = (composeItemData[0].count * Count).ToString();
            }
            ResetComposeItems();
        }

        private List<List<uint>> GetComposeSubItem()
        {
            List<List<uint>> temp = new List<List<uint>>();
            if (null != composeData)
            {
                MaxInputCount = composeData.maxBatchCompse;
                if (null != composeData.RecyclingItem1)
                {
                    MaxInputCount = (uint)Math.Min((Math.Floor((double)Sys_Bag.Instance.GetItemCount(composeData.RecyclingItem1[0])) / (composeData.RecyclingItem1[1])), MaxInputCount);
                    temp.Add(composeData.RecyclingItem1);
                    if (null != composeData.RecyclingItem2)
                    {
                        MaxInputCount = (uint)Math.Min((Math.Floor((double)Sys_Bag.Instance.GetItemCount(composeData.RecyclingItem2[0])) / (composeData.RecyclingItem2[1])), MaxInputCount);
                        temp.Add(composeData.RecyclingItem2);
                        if (null != composeData.RecyclingItem3)
                        {
                            MaxInputCount = (uint)Math.Min((Math.Floor((double)Sys_Bag.Instance.GetItemCount(composeData.RecyclingItem3[0])) / (composeData.RecyclingItem3[1])), MaxInputCount);
                            temp.Add(composeData.RecyclingItem3);
                            if (null != composeData.RecyclingItem4)
                            {
                                MaxInputCount = (uint)Math.Min((Math.Floor((double)Sys_Bag.Instance.GetItemCount(composeData.RecyclingItem4[0])) / (composeData.RecyclingItem4[1])), MaxInputCount);
                                temp.Add(composeData.RecyclingItem4);
                                if (null != composeData.RecyclingItem5)
                                {
                                    MaxInputCount = (uint)Math.Min((Math.Floor((double)Sys_Bag.Instance.GetItemCount(composeData.RecyclingItem5[0])) / (composeData.RecyclingItem5[1])), MaxInputCount);
                                    temp.Add(composeData.RecyclingItem5);
                                }
                            }
                        }
                    }
                }
            }
            mSlider.maxValue = MaxInputCount == 0 ? 1 : MaxInputCount;
            return temp;
        }

        private void UpdateButtonState()
        {
            ButtonHelper.Enable(mOkBtn, MaxInputCount > 0);
        }

        private void UpdateSlider(int curCount)
        {
            float val = 0;
            if (!mSlider.interactable)
                mSlider.interactable = true;
            if (MaxInputCount == 0)
            {
                val = 1;
            }
            else if (MaxInputCount == 1)
            {
                mSlider.interactable = false;
                mSlider.minValue = 1;
                mSlider.maxValue = 2;
                val = 2;
            }
            else
            {
                val = curCount;
            }
            mSlider.value = val;
        }

        private void OnSliderValueChanged(float val)
        {
            if (MaxInputCount == 1 && val > MaxInputCount)
                val = 1;
            mInputField.text = val.ToString();
            UpdateUndoItems();
            UpdateButtonState();
        }

        private void Undo()
        {
            if (null != composeData)
            {
                if (composeData.SeePet != 0)
                {
                    if (Sys_Pet.Instance.IsUniquePet(composeData.SeePet) && Sys_Pet.Instance.HasUniquePet(composeData.SeePet))
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12470));
                        return;
                    }
                }
                PromptBoxParameter.Instance.OpenPromptBox(2021112, 0, () => {
                    Sys_Bag.Instance.OnComposeItemReq(composeData.id, (uint)Count);
                });
            }

        }

        private void Add()
        {
            int count = Count;
            count++;
            count = (int)Mathf.Clamp(count, 1, MaxInputCount == 0 ? 1 : MaxInputCount);
            UpdateSlider((int)count);
            mInputField.text = count.ToString();
        }

        private void Sub()
        {
            int count = Count;
            count--;
            count = (int)Mathf.Clamp(count, 1, MaxInputCount == 0 ? 1 : MaxInputCount);
            UpdateSlider((int)count);
            mInputField.text = count.ToString();
        }

        public void OnSelectMainMenu(uint _bingHun)
        {
        }

        public void OnSelectSecondMenu(uint second)
        {
            composeData = CSVCompose.Instance.GetConfData(second);
            Refresh();
        }
    }
}
