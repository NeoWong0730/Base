using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace Logic
{
    public class UI_LifeSkill_MedicineSelect : UIBase
    {
        private uint formulaId;
        private List<ItemData> itemDatas = new List<ItemData>();
        private Dictionary<GameObject, SimpleItemGrid> ceils = new Dictionary<GameObject, SimpleItemGrid>();
        private InfinityGridLayoutGroup infinity;
        private Transform infinityParent;
        private Transform selectParent;
        private Button closeBtn;
        private Button okBtn;
        private int curSelectIndex = -1;
        private int needCount = 0;
        private List<SelectContainer> selectContainers = new List<SelectContainer>();
        private List<uint> selectItems = new List<uint>();
        private CSVFormula.Data cSVFormulaData;

        protected override void OnOpen(object arg)
        {
            formulaId = (uint)arg;
        }

        protected override void OnLoaded()
        {
            infinityParent = transform.Find("Animator/Tips01/Scroll_View01/GameObject/Viewport");
            selectParent = transform.Find("Animator/Tips01/Scroll_View02/Viewport");
            closeBtn = transform.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>();
            okBtn = transform.Find("Animator/Tips01/Btn_01").GetComponent<Button>();
            infinity = infinityParent.gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            infinity.minAmount = 16;
            infinity.updateChildrenCallback = UpdateChildrenCallback;
            for (int i = 0; i < infinityParent.childCount; i++)
            {
                GameObject go = infinityParent.GetChild(i).gameObject;
                SimpleItemGrid ceil = new SimpleItemGrid();
                ceil.BindGameObject(go);
                ceil.AddClickListener(OnCeilSelected);
                ceils.Add(go, ceil);
            }

            closeBtn.onClick.AddListener(OnCloseButtonClicked);
            okBtn.onClick.AddListener(OnOKButtonClicked);
        }

        protected override void OnShow()
        {
            InitData();
        }

        private void InitData()
        {
            itemDatas.Clear();
            cSVFormulaData = CSVFormula.Instance.GetConfData(formulaId);
            foreach (var item in cSVFormulaData.item_type_id)
            {
                List<ItemData> datas = Sys_Bag.Instance.GetItemDatasByItemType(item);
                for (int i = 0; i < datas.Count; i++)
                {
                    if (cSVFormulaData.item_lv_min > datas[i].cSVItemData.lv)
                    {
                        continue;
                    }
                    itemDatas.Add(datas[i]);
                }
            }
            itemDatas.Sort((s1, s2) =>
            {
                return -s1.cSVItemData.lv.CompareTo(s2.cSVItemData.lv);
            });
            infinity.SetAmount(itemDatas.Count);

            CopySelectData();

            needCount = (int)CSVFormula.Instance.GetConfData(formulaId).forge_num;
            FrameworkTool.CreateChildList(selectParent, needCount);
            for (int i = 0; i < needCount; i++)
            {
                SelectContainer selectContainer = new SelectContainer();
                GameObject go = selectParent.GetChild(i).gameObject;
                selectContainer.BindGameObject(go);
                selectContainer.SetActive(false);
                selectContainer.BindEvent(OnClear);
                selectContainers.Add(selectContainer);
                if (selectItems[i] == 0)
                {
                    selectContainer.SetActive(false);
                }
                else
                {
                    selectContainer.SetActive(true);
                    selectContainer.SetData(selectItems[i]);
                }
            }
            foreach (var item in selectContainers)
            {
                Dictionary<GameObject, SimpleItemGrid>.Enumerator enumerator = ceils.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SimpleItemGrid simpleItemGrid = enumerator.Current.Value;
                    if (!simpleItemGrid.bValid)
                        continue;
                    if (simpleItemGrid.ItemId == item.itemId)
                    {
                        simpleItemGrid.SubCount();
                    }
                }
            }
        }

        private void CopySelectData()
        {
            selectItems.Clear();
            for (int i = 0, count = Sys_LivingSkill.Instance.UnfixedFomulaItems.Count; i < count; i++)
            {
                selectItems.Add(Sys_LivingSkill.Instance.UnfixedFomulaItems[i]);
            }
        }


        private void UpdateChildrenCallback(int index, Transform trans)
        {
            SimpleItemGrid simpleItemGrid = ceils[trans.gameObject];
            simpleItemGrid.SetData(itemDatas[index], index);
            if (index != curSelectIndex)
            {
                simpleItemGrid.Release();
            }
            else
            {
                simpleItemGrid.Select();
            }
        }

        private void OnCeilSelected(SimpleItemGrid ceilGrid)
        {
            curSelectIndex = ceilGrid.gridIndex;
            foreach (var item in ceils)
            {
                if (item.Value.gridIndex == curSelectIndex)
                {
                    item.Value.Select();
                }
                else
                {
                    item.Value.Release();
                }
            }
            if (ceilGrid.SubCount())
            {
                bool bFindEmptyGrid = false;
                for (int i = 0; i < selectContainers.Count; i++)
                {
                    if (selectContainers[i].Empty)
                    {
                        bFindEmptyGrid = true;
                        selectContainers[i].SetData(ceilGrid.ItemId);
                        selectContainers[i].SetActive(true);
                        selectItems[i] = selectContainers[i].itemId;
                        break;
                    }
                }
                if (!bFindEmptyGrid)
                {
                    for (int i = 0; i < selectItems.Count; i++)
                    {
                        if (selectItems[i] == 0)
                        {
                            DebugUtil.LogErrorFormat("没有找到空格子，但是数据为空");
                        }
                    }
                    uint tobeRemove = selectItems[0];
                    Dictionary<GameObject, SimpleItemGrid>.Enumerator enumerator = ceils.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        SimpleItemGrid simpleItemGrid = enumerator.Current.Value;
                        if (!simpleItemGrid.bValid)
                            continue;
                        if (simpleItemGrid.ItemId == tobeRemove)
                        {
                            simpleItemGrid.AddCount();
                        }
                    }
                    selectItems.RemoveAt(0);
                    selectItems.Add(ceilGrid.ItemId);
                    for (int i = 0; i < selectItems.Count; i++)
                    {
                        selectContainers[i].SetData(selectItems[i]);
                    }
                }
            }
        }

        private void OnCloseButtonClicked()
        {
            foreach (var item in selectContainers)
            {
                item.Dispose();
            }
            selectContainers.Clear();

            Dictionary<GameObject, SimpleItemGrid>.Enumerator enumerator = ceils.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.Value.Dispose();
            }

            selectItems.Clear();
            CloseSelf();
            curSelectIndex = -1;
        }

        private void OnOKButtonClicked()
        {
            int count = 0;
            for (int i = 0; i < selectItems.Count; i++)
            {
                if (selectItems[i] > 0)
                {
                    count++;
                }
            }
            if (count < cSVFormulaData.forge_num_min)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010142, cSVFormulaData.forge_num_min.ToString()));
                return;
            }
            foreach (var item in selectContainers)
            {
                item.Dispose();
            }
            selectContainers.Clear();

            Dictionary<GameObject, SimpleItemGrid>.Enumerator enumerator = ceils.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.Value.Dispose();
            }

            Sys_LivingSkill.Instance.UnfixedFomulaItems.Clear();
            for (int i = 0; i < selectItems.Count; i++)
            {
                Sys_LivingSkill.Instance.UnfixedFomulaItems.Add(selectItems[i]);
            }
            Sys_LivingSkill.Instance.eventEmitter.Trigger(Sys_LivingSkill.EEvents.OnRefreshUnfixFormulaSelectItems);
            UIManager.CloseUI(EUIID.UI_LifeSkill_MedicineSelect);
        }

        private void OnClear(SelectContainer selectContainer)
        {
            Dictionary<GameObject, SimpleItemGrid>.Enumerator enumerator = ceils.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SimpleItemGrid simpleItemGrid = enumerator.Current.Value;
                if (!simpleItemGrid.bValid)
                    continue;
                if (simpleItemGrid.ItemId == selectContainer.itemId)
                {
                    simpleItemGrid.AddCount();
                    selectContainer.SetActive(false);
                    int index = selectContainers.IndexOf(selectContainer);
                    selectItems[index] = 0;
                }
            }
        }

        public class SimpleItemGrid
        {
            private Transform transform;
            private Image mBg;
            private Image mIcon;
            private Text mCount;
            private Text mName;
            private Text mLevel;
            private GameObject mSelectObj;
            private Action<SimpleItemGrid> onClick;
            private Action<SimpleItemGrid> onLongPressed;
            public int gridIndex;
            private ItemData mItemData;
            private int count;
            public uint ItemId
            {
                get
                {
                    if (mItemData != null)
                    {
                        return mItemData.Id;
                    }
                    return 0;
                }
            }
            public bool bValid = false;

            public void BindGameObject(GameObject go)
            {
                transform = go.transform;
                mName = transform.Find("Text_Name").GetComponent<Text>();
                mLevel = transform.Find("Text_Level").GetComponent<Text>();
                mBg = transform.Find("Btn_Item/Image_BG").GetComponent<Image>();
                mIcon = transform.Find("Btn_Item/Image_Icon").GetComponent<Image>();
                mCount = transform.Find("Text_Number").GetComponent<Text>();
                mSelectObj = transform.Find("Image_Select").gameObject;

                Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(mBg.gameObject);
                eventListener.AddEventListener(EventTriggerType.PointerClick, OnClicked);
            }

            public void SetData(ItemData itemData, int gridIndex)
            {
                this.gridIndex = gridIndex;
                mItemData = itemData;
                bValid = true;
                RefreshIcon();
            }

            private void RefreshIcon()
            {
                mCount.gameObject.SetActive(true);
                mIcon.gameObject.SetActive(true);
                mIcon.GetComponent<Image>().enabled = true;
                TextHelper.SetText(mName, mItemData.cSVItemData.name_id);
                TextHelper.SetText(mLevel, LanguageHelper.GetTextContent(2011056, mItemData.cSVItemData.lv.ToString()));
                ImageHelper.SetIcon(mIcon, mItemData.cSVItemData.icon_id);
                SetCount((int)mItemData.Count);
            }

            private void SetCount(int _count)
            {
                count = _count;
                if (count < 0)
                {
                    DebugUtil.LogErrorFormat("item数量低于0");
                    count = 0;
                }
                else if (count > mItemData.Count)
                {
                    DebugUtil.LogErrorFormat("item数量低于道具总数量");
                    count = (int)mItemData.Count;
                }
                mCount.text = count.ToString();
            }

            public void AddClickListener(Action<SimpleItemGrid> onclicked = null, Action<SimpleItemGrid> onlongPressed = null)
            {
                onClick = onclicked;
                if (onlongPressed != null)
                {
                    onLongPressed = onlongPressed;
                    UI_LongPressButton uI_LongPressButton = mBg.gameObject.AddComponent<UI_LongPressButton>();
                    uI_LongPressButton.onStartPress.AddListener(OnLongPressed);
                }
            }

            private void OnClicked(BaseEventData baseEventData)
            {
                onClick?.Invoke(this);
            }
            private void OnLongPressed()
            {
                onLongPressed.Invoke(this);
            }

            public void Select()
            {
                mSelectObj.SetActive(true);
            }
            public void Release()
            {
                mSelectObj.SetActive(false);
            }

            public void AddCount()
            {
                int newCount = count + 1;
                SetCount(newCount);
            }

            public bool SubCount()
            {
                if (count == 0)
                    return false;
                int newCount = count - 1;
                SetCount(newCount);
                return true;
            }

            public void Dispose()
            {
                mItemData = null;
                bValid = false;
            }
        }


        public class SelectContainer
        {
            public uint itemId;
            private GameObject gameObject;
            public Image icon;
            private Image eventBg;
            private Text _name;
            private Action<SelectContainer> onclear;
            Lib.Core.EventTrigger eventListener;
            public bool Empty { get; private set; } = true;

            public void BindGameObject(GameObject go)
            {
                gameObject = go;
                ParseGo();
            }
            private void ParseGo()
            {
                icon = gameObject.transform.Find("Btn_Item/Image_Icon").GetComponent<Image>();
                _name = gameObject.transform.Find("Text_Name").GetComponent<Text>();
                icon.enabled = true;
                eventBg = gameObject.transform.Find("Btn_Item/Image_BG").GetComponent<Image>();
                eventListener = Lib.Core.EventTrigger.Get(eventBg);
                eventListener.AddEventListener(EventTriggerType.PointerClick, OnGridClicked);
            }

            public void SetData(uint id)
            {
                itemId = id;
                RefreshIcon();
            }

            private void RefreshIcon()
            {
                ImageHelper.SetIcon(icon, CSVItem.Instance.GetConfData(itemId).icon_id);
                TextHelper.SetText(_name, CSVItem.Instance.GetConfData(itemId).name_id);
            }

            public void BindEvent(Action<SelectContainer> _onclear)
            {
                onclear = _onclear;
            }

            private void OnGridClicked(BaseEventData baseEventData)
            {
                if (Empty)
                    return;
                onclear?.Invoke(this);
            }

            public void SetActive(bool active)
            {
                icon.gameObject.SetActive(active);
                Empty = !active;
                if (!active)
                {
                    _name.text = "";
                }
            }

            public void Dispose()
            {
                onclear = null;
                SetActive(false);
                gameObject = null;
                icon = null;
                eventBg = null;
                _name.text = "";
            }
        }

    }
}


