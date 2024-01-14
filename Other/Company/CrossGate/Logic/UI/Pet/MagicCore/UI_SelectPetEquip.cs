using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;

using Packet;
using System;


namespace Logic
{
    public class UI_SelectPetEquip_Layout
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

    public class UI_SelectPetEquip : UIBase, UI_SelectPetEquip_Layout.IListener
    {
        public class UI_PetEquipItem : UIComponent
        {
            private PropItem propItem;
            public Text itemname;
            public Text itemLv;
            public Button itemBtn;
            private GameObject selectGo;
            public Action<UI_PetEquipItem> action;
            public ItemData itemData;
            public int index;
            public uint petUid;
            protected override void Loaded()
            {
                propItem = new PropItem();
                propItem.BindGameObject(transform.Find("PropItem").gameObject);
                itemname = transform.Find("Grid/Text_Name").GetComponent<Text>();
                itemLv = transform.Find("Grid/Text_Lv/Text_Value").GetComponent<Text>();
                itemLv.transform.parent.gameObject.SetActive(true);
                selectGo = transform.Find("Image_Select").gameObject;
                itemBtn = transform.GetComponent<Button>();
                itemBtn.onClick.AddListener(OnitemBtnClicked);
            }

            public void AddAction(Action<UI_PetEquipItem> action)
            {
                this.action = action;
            }

            public void RefreshItem(ItemData itemdata, int index, bool isSelect, uint selectPetUid)
            {
                petUid = selectPetUid;
                this.index = index;
                itemData = itemdata;
                propItem.Layout.imgIcon.gameObject.SetActive(true);
                PropIconLoader.ShowItemData _itemData = new PropIconLoader.ShowItemData(itemData.cSVItemData.id, itemdata.Count, true, itemdata.bBind, false, false, false, false, false, true, PetEquipBeClicked, false, false);
                _itemData.EquipPara = itemdata.petEquip.Color;

                propItem.SetData(new MessageBoxEvt(EUIID.UI_SelectPetEquip, _itemData));
                propItem.txtNumber.gameObject.SetActive(true);
                itemname.text = LanguageHelper.GetTextContent(itemdata.cSVItemData.name_id);
                CSVPetEquip.Data petEquipData = CSVPetEquip.Instance.GetConfData(itemdata.Id);
                if(null != petEquipData)
                {
                    itemLv.text = petEquipData.equipment_level.ToString();
                }
                else
                {
                    itemLv.transform.parent.gameObject.SetActive(false);
                }
                
                SetSelectState(isSelect);
            }

            private void PetEquipBeClicked(PropItem item)
            {
                PetEquipTipsData petEquipTipsData = new PetEquipTipsData();
                petEquipTipsData.petUid = petUid;
                petEquipTipsData.petEquip = itemData;
                petEquipTipsData.isCompare = true;
                petEquipTipsData.isShowOpBtn = false;
                petEquipTipsData.isShowLock = false;
                UIManager.OpenUI(EUIID.UI_Tips_PetMagicCore, false, petEquipTipsData);
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

        private UI_SelectPetEquip_Layout layout = new UI_SelectPetEquip_Layout();
        private List<UI_PetEquipItem> itemlist = new List<UI_PetEquipItem>();
        private uint petUid; // 装备的宠物uid
        private uint itemType; // 装备的部位
        private int infinityCount;
        private uint titleId;
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
                petUid = tuple.Item1;
                itemType = tuple.Item2;
                titleId = tuple.Item3;
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
            petUid = 0;
        }
        private List<ItemData> items;
        private void SetItemView()
        {
            items = Sys_Bag.Instance.GetItemDatasByItemType((uint)EItemType.PetEquipment, new List<Func<ItemData, bool>>
                        {
                            (_item) => 
                            {
                                CSVPetEquip.Data data = CSVPetEquip.Instance.GetConfData(_item.Id);
                                if(null == data)
                                {
                                    return false;
                                }
                                else
                                {
                                    return data.equipment_category == itemType;
                                }
                            }
                        });
            if (items.Count > 1)
            {
                items.Sort(SortPetEquip);
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
            UI_PetEquipItem entry = new UI_PetEquipItem();
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
            UI_PetEquipItem entry = cell.mUserData as UI_PetEquipItem;
            entry.RefreshItem(items[index], index, selectIndex == index, petUid);
        }

        private int SortPetEquip(ItemData a, ItemData b)
        {
            int re = -a.cSVItemData.lv.CompareTo(b.cSVItemData.lv);
            if (re == 0)
            {
                re = -a.petEquip.Score.CompareTo(b.petEquip.Score);
            }
            return re;
        }

        public void OnItemClick(UI_PetEquipItem itemData)
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
            UIManager.CloseUI(EUIID.UI_SelectPetEquip);
        }

        public void OnGetAwayBtnClicked()
        {
            PropIconLoader.ShowItemData iItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, false, false);

            iItemData.id = 100497;

            var boxEvt = new MessageBoxEvt(EUIID.UI_PetMagicCore, iItemData);

            boxEvt.b_ForceShowScource = true;
            boxEvt.b_ShowItemInfo = false;
            UIManager.OpenUI(EUIID.UI_Message_Box, false, boxEvt);
        }

        public void OnConfirmBtnClicked()
        {
            if(selectIndex >= 0 && selectIndex < items.Count)
            {
                ClientPet pet = Sys_Pet.Instance.GetPetByUId(petUid);
                if(null != pet)
                {
                    var item = items[selectIndex];
                    CSVPetEquip.Data petEquipData = CSVPetEquip.Instance.GetConfData(item.Id);
                    if (null != petEquipData && pet.petUnit.SimpleInfo.Level < petEquipData.equipment_level)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000902, petEquipData.equipment_level.ToString()));//等级不匹配
                        return;
                    }
                    uint suitSkillId = item.petEquip.SuitSkill;
                    if(suitSkillId> 0)
                    {
                        CSVPetEquipSuitSkill.Data petEquipSuitSkillData = CSVPetEquipSuitSkill.Instance.GetConfData(suitSkillId);
                        if (null != petEquipSuitSkillData)
                        {
                            var petSkills = pet.GetPetSkillList();
                            if(petSkills.Contains(petEquipSuitSkillData.base_skill) || petSkills.Contains(petEquipSuitSkillData.upgrade_skill))
                            {
                                Sys_Pet.Instance.ItemFitPetEquipReq(itemType, item.Uuid, petUid);
                                UIManager.CloseUI(EUIID.UI_SelectPetEquip);
                            }
                            else
                            {
                                string strName = string.Empty;
                                bool isActiveSkill = Sys_Skill.Instance.IsActiveSkill(petEquipSuitSkillData.base_skill);
                                if (isActiveSkill) //主动技能
                                {
                                    CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(petEquipSuitSkillData.base_skill);
                                    if (skillInfo != null)
                                    {
                                        strName = LanguageHelper.GetTextContent(680000911, LanguageHelper.GetTextContent(skillInfo.name));
                                    }
                                    else
                                    {
                                        Debug.LogErrorFormat("not found skillId={0}", petEquipSuitSkillData.base_skill);
                                    }
                                }
                                else
                                {
                                    CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(petEquipSuitSkillData.base_skill);
                                    if (skillInfo != null)
                                    {
                                        strName = LanguageHelper.GetTextContent(680000911, LanguageHelper.GetTextContent(skillInfo.name));

                                    }
                                    else
                                    {
                                        Debug.LogErrorFormat("not found skillId={0}", petEquipSuitSkillData.base_skill);
                                    }
                                }
                                var str = LanguageHelper.GetTextContent(680000903, strName);
                                PromptBoxParameter.Instance.OpenPromptBox(str, 0,
                                    () => 
                                    {
                                        Sys_Pet.Instance.ItemFitPetEquipReq(itemType, item.Uuid, petUid);
                                        UIManager.CloseUI(EUIID.UI_SelectPetEquip);
                                    });
                            }
                        }
                    }
                    else
                    {
                        Sys_Pet.Instance.ItemFitPetEquipReq(itemType, item.Uuid, petUid);
                        UIManager.CloseUI(EUIID.UI_SelectPetEquip);
                    }
                }
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000901));//请选择一个
            }
        }
    }
}
