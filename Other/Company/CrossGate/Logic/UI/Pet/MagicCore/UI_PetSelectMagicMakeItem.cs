using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;

using Packet;
using System;


namespace Logic
{
    public class UI_PetSelectMagicMakeItem_Layout
    {
        public Transform transform;
        public Text titleText;
        public Button closeBtn;
        public Button getAwayBtn;
        public Button confirmBtn;
        /// <summary> 无限滚动 </summary>
        private InfinityGrid infinityGrid;
        public void Init(Transform transform)
        {
            this.transform = transform;
            closeBtn = transform.Find("Blank").GetComponent<Button>();
            titleText = transform.Find("Animator/Image_Title/Text").GetComponent<Text>();

            infinityGrid = transform.Find("Animator/Scroll View").GetComponent<InfinityGrid>();
            getAwayBtn = transform.Find("Animator/Add").GetComponent<Button>();
            confirmBtn = transform.Find("Animator/Btn_Confirm").GetComponent<Button>();
            
        }

        public void SetTitleText(uint langId)
        {
            TextHelper.SetText(titleText, langId);
        }
        public void SetInfinityGridCell(int count)
        {
            infinityGrid.CellCount = count;
            infinityGrid.ForceRefreshActiveCell();
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OncloseBtnClicked);
            getAwayBtn.onClick.AddListener(listener.OnGetAwayBtnClicked);
            confirmBtn.onClick.AddListener(listener.OnConfirmBtnClicked);
            infinityGrid.onCreateCell += listener.OnCreateCell;
            infinityGrid.onCellChange += listener.OnCellChange;
        }

        public interface IListener
        {
            void OncloseBtnClicked();
            void OnGetAwayBtnClicked();
            void OnConfirmBtnClicked();
            void OnCreateCell(InfinityGridCell cell);
            void OnCellChange(InfinityGridCell cell, int index);
        }
    }

    public class UI_PetSelectMagicMakeItem : UIBase, UI_PetSelectMagicMakeItem_Layout.IListener
    {
        public class UI_Pet_MagicMakeItem : UIComponent
        {
            private PropItem propItem;
            public Text itemname;
            public Button itemBtn;
            private GameObject selectGo;
            public Action<UI_Pet_MagicMakeItem> action;
            public ItemData itemData;
            public int index;
            protected override void Loaded()
            {
                propItem = new PropItem();
                propItem.BindGameObject(transform.Find("PropItem").gameObject);
                itemname = transform.Find("Grid/Text_Name").GetComponent<Text>();
                selectGo = transform.Find("Image_Select").gameObject;
                itemBtn = transform.GetComponent<Button>();
                itemBtn.onClick.AddListener(OnitemBtnClicked);
            }

            public void AddAction(Action<UI_Pet_MagicMakeItem> action)
            {
                this.action = action;
            }

            public void RefreshItem(ItemData itemdata, int index, bool isSelect)
            {
                this.index = index;
                itemData = itemdata;
                propItem.Layout.imgIcon.gameObject.SetActive(true);
                propItem.SetData(new MessageBoxEvt(EUIID.UI_PetSelectMagicMakeItem, new PropIconLoader.ShowItemData(itemData.cSVItemData.id, itemdata.Count, true, false, false, false, false, false, false, true)));
                propItem.txtNumber.gameObject.SetActive(true);
                itemname.text = LanguageHelper.GetTextContent(itemdata.cSVItemData.name_id);
                SetSelectState(isSelect);
            }

            public void SetSelectState(bool isSelect)
            {
                selectGo.SetActive(isSelect);
            }

            private void OnitemBtnClicked()
            {
                action?.Invoke(this);
            }
        }

        private UI_PetSelectMagicMakeItem_Layout layout = new UI_PetSelectMagicMakeItem_Layout();
        private List<UI_Pet_MagicMakeItem> itemlist = new List<UI_Pet_MagicMakeItem>();
        private uint makePetEquipId;
        private int infinityCount;
        private uint titleId;
        private uint itemType;
        private int selectIndex = -1;
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void OnOpen(object arg)
        {
            if(null != arg)
            {
                Tuple<uint, uint,uint> tuple = arg as Tuple<uint, uint, uint>;
                makePetEquipId = tuple.Item1;
                titleId = tuple.Item2;
                itemType = tuple.Item3;
            }
        }

        protected override void OnShow()
        {
            SetItemView();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnChangeItemCount, OnChangeItemCount, toRegister);
        }


        private void OnChangeItemCount()
        {
            SetItemView();
        }

        protected override void OnClose()
        {
            makePetEquipId = 0;
        }
        private List<ItemData> items;
        private void SetItemView()
        {
            if(itemType == (uint)EItemType.PetRemakeItem)
            {
                if (0 != makePetEquipId)
                {
                    List<uint> itemIds = new List<uint>(4);
                    CSVPetEquip.Data petEquip = CSVPetEquip.Instance.GetConfData(makePetEquipId);
                    if (null != petEquip && null != petEquip.forge_special)
                    {
                        for (int i = 0; i < petEquip.forge_special.Count; i++)
                        {
                            if (petEquip.forge_special[i].Count >= 1)
                            {
                                itemIds.Add(petEquip.forge_special[i][0]);
                            }
                        }
                    }

                    items = Sys_Bag.Instance.GetItemDatasByItemType(itemType, new List<Func<ItemData, bool>>
                        {
                            (_item) => {return itemIds.Contains(_item.Id); }
                        });
                    if (items.Count > 1)
                    {
                        items.Sort(SortSkillLevel);
                    }
                }
            }
            else if(itemType == (uint)EItemType.PetEquipSmeltItem)
            {
                var _items = Sys_Bag.Instance.GetItemDatasByItemType(itemType);
                items = new List<ItemData>(_items.Count);
                for (int i = 0; i < _items.Count; i++)
                {
                    var itemData = _items[i];
                    bool hasData = false;
                    for (int j = 0, len = items.Count; j < len; j++)
                    {
                        var showItem = items[j];
                        if (showItem.Id == itemData.Id)
                        {
                            hasData = true;
                            showItem.SetCount(showItem.Count + itemData.Count);
                            break;
                        }
                    }

                    if(!hasData)
                    {
                        ItemData temp = new ItemData();
                        temp.SetData(itemData);
                        items.Add(temp);
                    }
                }
            }
            layout.SetTitleText(titleId);
            infinityCount = items.Count;
            layout.SetInfinityGridCell(infinityCount);
            selectIndex = -1;
        }

        /// <summary>
        /// 滚动列表创建回调
        /// </summary>
        /// <param name="cell"></param>
        public void OnCreateCell(InfinityGridCell cell)
        {
            UI_Pet_MagicMakeItem entry = new UI_Pet_MagicMakeItem();
            GameObject go = cell.mRootTransform.gameObject;
            entry.Init(go.transform);
            entry.AddAction(OnItemClick);
            cell.BindUserData(entry);
            itemlist.Add(entry);
        }
        
        /// <summary>
        /// 滚动列表滚动回调
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        public void OnCellChange(InfinityGridCell cell, int index)
        {
            if (index < 0 || index >= infinityCount)
                return;
            UI_Pet_MagicMakeItem entry = cell.mUserData as UI_Pet_MagicMakeItem;
            entry.RefreshItem(items[index], index, selectIndex == index);
        }

        private int SortSkillLevel(ItemData a, ItemData b)
        {
            return (int)a.cSVItemData.lv - (int)b.cSVItemData.lv;
        }

        public void OnItemClick(UI_Pet_MagicMakeItem itemData)
        {
            if(selectIndex != itemData.index)
            {
                selectIndex = itemData.index;
                for (int i = 0; i < itemlist.Count; i++)
                {
                    itemlist[i].SetSelectState(selectIndex == itemlist[i].index);
                }
            }
        }
        
        public void OncloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_PetSelectMagicMakeItem);
        }

        public void OnGetAwayBtnClicked()
        {
            PropIconLoader.ShowItemData iItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, false, false);
            if (itemType == (uint)EItemType.PetRemakeItem)
            {
                iItemData.id = 100499;
            }
            else if (itemType == (uint)EItemType.PetEquipSmeltItem)
            {
                iItemData.id = 139910;
            }
            var boxEvt = new MessageBoxEvt(EUIID.UI_PetMagicCore, iItemData);

            boxEvt.b_ForceShowScource = true;
            boxEvt.b_ShowItemInfo = false;
            UIManager.OpenUI(EUIID.UI_Message_Box, false, boxEvt);
        }

        public void OnConfirmBtnClicked()
        {
            if(selectIndex >= 0 && selectIndex < items.Count)
            {
                Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnSelectItem, items[selectIndex].cSVItemData.id);
                UIManager.CloseUI(EUIID.UI_PetSelectMagicMakeItem);
            }
            else
            {
                uint langId = 0;
                if (itemType == (uint)EItemType.PetRemakeItem)
                {
                    langId = 680000921;
                }
                else if (itemType == (uint)EItemType.PetEquipSmeltItem)
                {
                    langId = 680000922;
                }
                
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(langId));//清选择
            }
        }
    }
}
