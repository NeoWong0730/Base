using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Lib.Core;
using Packet;
using System;
using static Logic.Sys_Equip;

namespace Logic
{
    public class UI_SelectMountSkillParam
    {
        public uint tittle_langId;
        public uint useType;
        public List<ItemData> itemDatas;
        public uint petUid;
        public List<ulong> fiters;
        public uint itemQuailty;
        public int selectIndex;
    }

    public class UI_Pet_MountSkillSelectItem_Layout
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
            titleText = transform.Find("Animator/Image_Title/Text").GetComponent<Text>();
            closeBtn = transform.Find("Blank").GetComponent<Button>();
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

    public class UI_Pet_MountSkillSelectItem : UIBase, UI_Pet_MountSkillSelectItem_Layout.IListener
    {
        public class UI_Pet_UseMountSkillItem : UIComponent
        {
            private PropItem propItem;
            public Text itemname;
            public Button itemBtn;
            private GameObject selectGo;
            public Action<UI_Pet_UseMountSkillItem> action;
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

            public void AddAction(Action<UI_Pet_UseMountSkillItem> action)
            {
                this.action = action;
            }

            public void RefreshItem(ItemData itemdata, int index, bool isSelect)
            {
                this.index = index;
                itemData = itemdata;
                propItem.Layout.imgIcon.gameObject.SetActive(true);
                propItem.SetData(new MessageBoxEvt(EUIID.UI_Pet_MountSkillSelectItem, new PropIconLoader.ShowItemData(itemData.cSVItemData.id, itemdata.Count, true, false, false, false, false, false, false, true)));
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

        private UI_Pet_MountSkillSelectItem_Layout layout = new UI_Pet_MountSkillSelectItem_Layout();
        private List<UI_Pet_UseMountSkillItem> itemlist = new List<UI_Pet_UseMountSkillItem>();
        private UI_SelectMountSkillParam param;
        private int infinityCount;

        private int selectIndex = -1;
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void OnOpen(object arg)
        {
            param = arg as UI_SelectMountSkillParam;
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
            if(null != param)
            {
                SetItemView();
            }
        }

        protected override void OnClose()
        {
            param = null;
        }

        private void SetItemView()
        {
            List<ItemData> _itemDatas1 = new List<ItemData>(); 
            uint typeid = param.useType;
            if(typeid == 0)
            {
                _itemDatas1 = Sys_Bag.Instance.GetItemDatasByItemType((uint)EItemType.PetMountSkillBook);
                if (_itemDatas1.Count > 1)
                {
                    _itemDatas1.Sort(SortSkillLevel);
                }
            }
            else
            {
                _itemDatas1 = Sys_Bag.Instance.GetItemDatasByItemType((uint)EItemType.PetMountSkillBook, new List<Func<ItemData, bool>>
                        {
                            (_item) =>
                            {
                                return !param.fiters.Contains(_item.Uuid) && _item.cSVItemData.quality == param.itemQuailty;
                            }
                        });
            }

            layout.SetTitleText(param.tittle_langId);
            param.itemDatas = _itemDatas1;
            infinityCount = param.itemDatas.Count;
            layout.SetInfinityGridCell(infinityCount);
            selectIndex = -1;
        }

        /// <summary>
        /// 滚动列表创建回调
        /// </summary>
        /// <param name="cell"></param>
        public void OnCreateCell(InfinityGridCell cell)
        {
            UI_Pet_UseMountSkillItem entry = new UI_Pet_UseMountSkillItem();
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
            UI_Pet_UseMountSkillItem entry = cell.mUserData as UI_Pet_UseMountSkillItem;
            entry.RefreshItem(param.itemDatas[index], index, selectIndex == index);
        }

        private int SortSkillLevel(ItemData a, ItemData b)
        {
            return (int)a.cSVItemData.lv - (int)b.cSVItemData.lv;
        }

        public void OnItemClick(UI_Pet_UseMountSkillItem itemData)
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
            UIManager.CloseUI(EUIID.UI_Pet_MountSkillSelectItem);
        }

        public void OnGetAwayBtnClicked()
        {
            if(null != param)
            {
                PropIconLoader.ShowItemData iItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, false, false);
                iItemData.id = 100498;
                var boxEvt = new MessageBoxEvt(EUIID.UI_Pet_Message, iItemData);
                boxEvt.b_ForceShowScource = true;
                boxEvt.b_ShowItemInfo = false;
                UIManager.OpenUI(EUIID.UI_Message_Box, false, boxEvt);
            }
        }

        public void OnConfirmBtnClicked()
        {
            
            if(selectIndex >= 0 && selectIndex < param.itemDatas.Count)
            {
                ItemData itemData = param.itemDatas[selectIndex];
                if (param.useType == 0)
                {
                    ClientPet clientPet = Sys_Pet.Instance.GetPetByUId(param.petUid);
                    
                    if (null != clientPet && null != itemData.cSVItemData.fun_value && itemData.cSVItemData.fun_value.Count >= 2)
                    {
                        var passiveSkill = itemData.cSVItemData.fun_value[1];
                        if (clientPet.IsHasSameMountSkill(passiveSkill))
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000210));//有相同的技能
                            return;
                        }
                        CSVPassiveSkillInfo.Data passiveSkillInfo = CSVPassiveSkillInfo.Instance.GetConfData(passiveSkill);
                        if (null != passiveSkillInfo)
                        {
                            if (passiveSkillInfo.mutex_id != 0 && clientPet.IsMutexId(passiveSkillInfo.mutex_id))
                            {
                                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000230));//有相同的技能
                                return;
                            }
                        }
                    }
                    Sys_Pet.Instance.OnPetRidingSkillLearnReq(param.petUid, itemData.Id);
                }
                else
                {
                    Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnMountSkillItemSelect, param.selectIndex ,itemData.Uuid);
                }
               
                UIManager.CloseUI(EUIID.UI_Pet_MountSkillSelectItem);
            }
            else
            {
                uint langId = 680000227;
                if (param.useType != 0)
                {
                    langId = 1018610;
                }
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(langId));
            }
        }
    }
}
