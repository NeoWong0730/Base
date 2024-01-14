using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Lib.Core;
using System;

namespace Logic
{
    public class UI_PetMagicCore_ReMake_ViewLeft : UIComponent
    {
        readonly int INFINITY_MIN_AMOUNT = 7;
        private ulong selectedUuid = 0;
        
        private int infinityCount;
        private List<UI_PetMagicCore_ReMake_CellView> itemlist = new List<UI_PetMagicCore_ReMake_CellView>();
        private List<ItemData> itemUuidList = new List<ItemData>();
        private IListener listener;
        private GameObject goTips;

        /// <summary> 无限滚动 </summary>
        private InfinityGrid infinityGrid;

        #region 系统函数
        protected override void Loaded()
        {
            Parse();
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
        #endregion

        #region func
        private void Parse()
        {
            goTips = transform.parent.Find("Text_Tips").gameObject;
            infinityGrid = transform.GetComponent<InfinityGrid>();
            infinityGrid.onCreateCell += OnCreateCell;
            infinityGrid.onCellChange += OnCellChange;
        }

        public void UpdateView()
        {
            itemUuidList = Sys_Pet.Instance.GetSmeltPetEquips();
            if (selectedUuid > 0)
            {
                bool hasSelect = false;
                for (int i = 0; i < itemUuidList.Count; i++)
                {
                    if (itemUuidList[i].Uuid == selectedUuid)
                    {
                        hasSelect = true;
                        listener?.OnItemSelect(itemUuidList[i]);
                        break;
                    }
                }
                if(!hasSelect)
                {
                    selectedUuid = 0;
                }
            }
            goTips.SetActive(itemUuidList.Count <= 0);
            infinityCount = itemUuidList.Count;
            infinityGrid.CellCount = infinityCount;
            infinityGrid.ForceRefreshActiveCell();
        }


        /// <summary>
        /// 滚动列表创建回调
        /// </summary>
        /// <param name="cell"></param>
        public void OnCreateCell(InfinityGridCell cell)
        {
            UI_PetMagicCore_ReMake_CellView entry = new UI_PetMagicCore_ReMake_CellView();
            GameObject go = cell.mRootTransform.gameObject;
            entry.Init(go.transform);
            entry.Register(OnItemSelect);
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
            UI_PetMagicCore_ReMake_CellView entry = cell.mUserData as UI_PetMagicCore_ReMake_CellView;
            entry.UpdateCellView(itemUuidList[index], selectedUuid);
        }

        public void Register(IListener _listener)
        {
            listener = _listener;
        }
        private void OnItemSelect(ulong itemUuid)
        {
            selectedUuid = itemUuid;
            for (int i = 0; i < itemlist.Count; i++)
            {
                if (itemlist[i].gameObject.activeInHierarchy)
                {
                    itemlist[i].SetSelectedState(selectedUuid);
                }
            }
            for (int i = 0; i < itemUuidList.Count; i++)
            {
                if(itemUuidList[i].Uuid == selectedUuid)
                {
                    listener?.OnItemSelect(itemUuidList[i]);
                    break;
                }
            }
            
            
        }
        #endregion

        #region event
        #endregion
        public interface IListener
        {
            void OnItemSelect(ItemData item);
        }
        public class UI_PetMagicCore_ReMake_CellView : UIComponent
        {
            private ItemData itemData;
            private Button btnItem;
            private Button equipBtn;
            private ulong uuid;
            private System.Action<ulong> _action;
            private Image imgIcon;
            private Image imgQuality;
            private Text txtName;
            private Text txtLv;
            private Text textType;
            private GameObject goSelected;
            private GameObject fitGo;
            protected override void Loaded()
            {
                btnItem = transform.GetComponent<Button>();
                btnItem.onClick.AddListener(OnBtnItemClick);
                equipBtn = transform.Find("EquipItem/Btn_Item").GetComponent<Button>();
                equipBtn.onClick.AddListener(OnEquipBtnCliced);
                imgIcon = transform.Find("EquipItem/Btn_Item/Image_Icon").GetComponent<Image>();
                imgQuality = transform.Find("EquipItem/Btn_Item/Image_Quality").GetComponent<Image>();
                txtName = transform.Find("Text_Name").GetComponent<Text>();
                txtLv = transform.Find("Text_Level/Text_Value").GetComponent<Text>();
                textType = transform.Find("Text_Type/Text_Value").GetComponent<Text>();
                goSelected = transform.Find("Image_Select").gameObject;
                fitGo = transform.Find("EquipItem/Image_Equiped").gameObject;

            }
            public void UpdateCellView(ItemData itemData,ulong selectedUuid)
            {
                uuid = itemData.Uuid;
                this.itemData = itemData;
                ImageHelper.SetIcon(imgIcon, itemData.cSVItemData.icon_id);
                ImageHelper.GetQualityColor_Frame(imgQuality, (int)itemData.Quality);;
                txtName.text = LanguageHelper.GetTextContent(itemData.cSVItemData.name_id);
                CSVPetEquip.Data petEquipData = CSVPetEquip.Instance.GetConfData(itemData.Id);
                textType.text = LanguageHelper.GetTextContent(680010000u + petEquipData.equipment_category);
                if (null!= petEquipData)
                {
                    txtLv.text = LanguageHelper.GetTextContent(4811, petEquipData.equipment_level.ToString());
                }
                fitGo.SetActive(Sys_Pet.Instance.GetPetEquipFitPetUid(itemData.Uuid) != 0);
                SetSelectedState(selectedUuid);
            }
            public void SetSelectedState(ulong _uuid)
            {
                goSelected.SetActive(uuid == _uuid);
            }
            public void Register(System.Action<ulong> action)
            {
                _action = action;
            }
            private void OnEquipBtnCliced()
            {
                if (null != itemData)
                {
                    PetEquipTipsData petEquipTipsData = new PetEquipTipsData();
                    petEquipTipsData.openUI = EUIID.UI_PetMagicCore;
                    petEquipTipsData.petEquip = itemData;
                    petEquipTipsData.isCompare = false;
                    petEquipTipsData.isShowOpBtn = false;
                    UIManager.OpenUI(EUIID.UI_Tips_PetMagicCore, false, petEquipTipsData);
                }
            }
            private void OnBtnItemClick()
            {
                _action?.Invoke(uuid);
            }
        }
    }
}
