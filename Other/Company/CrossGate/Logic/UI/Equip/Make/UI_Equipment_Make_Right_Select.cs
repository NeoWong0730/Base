using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using Logic.Core;
using Table;
using Packet;
using Lib.Core;

namespace Logic
{
    public class UI_Equipment_Make_Right_Select : UIParseCommon
    {
        private class SelectCell : UIParseCommon
        {
            private Button btn;
            private PropItem prop;
            private Text textName;
            private Text textLevel;

            private ItemData itemPaper;
            private Action<ItemData> action;

            protected override void Parse()
            {
                btn = transform.GetComponent<Button>();
                btn.onClick.AddListener(OnClick);

                prop = new PropItem();
                prop.BindGameObject(transform.Find("PropItem").gameObject);
                prop.btnNone.image.raycastTarget = false;

                textName = transform.Find("Text_Name").GetComponent<Text>();
                textLevel = transform.Find("Text").GetComponent<Text>();
            }

            private void OnClick()
            {
                action?.Invoke(itemPaper);
            }

            public void AddListener(Action<ItemData> _action)
            {
                action = _action;
            }

            public override void UpdateInfo(ItemData item)
            {
                itemPaper = item;

                PropIconLoader.ShowItemData costData = new PropIconLoader.ShowItemData(item.cSVItemData.id, 1, true, false, false, false, false, false, true, true);
                prop.SetData(new MessageBoxEvt( EUIID.UI_Equipment, costData));

                textName.text = LanguageHelper.GetTextContent(item.cSVItemData.name_id);
                textLevel.text = "";

                btn.enabled = itemPaper.Uuid != 0;
            }

            //public void OnDisable()
            //{
            //    //if (toggle != null)
            //    //    toggle.isOn = false;
            //}
        }

        //private Dictionary<GameObject, SelectCell> dicCells = new Dictionary<GameObject, SelectCell>();
        private InfinityGrid _infinityGrid;
        private Transform transTip;

        private List<ItemData> listPapers = new List<ItemData>();

        private ItemData curOpEquip;
        private IListener listener;

        protected override void Parse()
        {
            Lib.Core.EventTrigger.AddEventListener(transform.Find("Image_Black").gameObject, EventTriggerType.PointerClick, (eventData) =>
            {
                Hide();
            });

            _infinityGrid = transform.Find("View_SelectSkill/Scroll_View").gameObject.GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCellCreate;
            _infinityGrid.onCellChange += OnCellChange;

            transTip = transform.Find("View_SelectSkill/Text_Tips");
        }

        public override void Show()
        {
            base.Show();

            FillList();
        }

        public override void Hide()
        {
            base.Hide();

            //foreach (var data in dicCells)
            //{
            //    data.Value.OnDisable();
            //}
        }

        private void OnCellCreate(InfinityGridCell cell)
        {
            SelectCell entry = new SelectCell();

            entry.AddListener(OnSelectPaper);
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
        }

        private void OnCellChange(InfinityGridCell cell , int index)
        {
            SelectCell entry = cell.mUserData as SelectCell;
            entry.UpdateInfo(listPapers[index]);
        }

        private void OnSelectPaper(ItemData paperItem)
        {
            listener?.OnSelectPaper(paperItem);
        }

        private void FillList()
        {
            listPapers.Clear();

            ////如果没有图纸，构建默认图纸
            //ItemData None = new ItemData(0, 0, 201600u, 0, 0, false, false, null, null, 0);
            //listPapers.Add(item);

            if (curOpEquip != null)
            {
                CSVEquipment.Data equipInfo = CSVEquipment.Instance.GetConfData(curOpEquip.cSVItemData.id);

                List<uint> paperIds = new List<uint>();
                if (equipInfo.suit_item_special != null)
                {
                    for (int i = 0; i < equipInfo.suit_item_special.Count; ++i)
                        paperIds.Add(equipInfo.suit_item_special[i][0]);
                }

                List<ItemData> itemList;
                if (Sys_Bag.Instance.BagItems.TryGetValue((int)BoxIDEnum.BoxIdNormal, out itemList))
                {
                    foreach (ItemData item in itemList)
                    {
                        if (paperIds.IndexOf(item.Id) >= 0)
                        {
                            listPapers.Add(item);
                        }
                    }
                }
            }
            
            transTip.gameObject.SetActive(listPapers.Count == 0);

            _infinityGrid.CellCount = listPapers.Count;
            _infinityGrid.ForceRefreshActiveCell();
        }

        public void Register(IListener _listener)
        {
            listener = _listener;
        }

        public  override void UpdateInfo(ItemData  item)
        {
            curOpEquip = item;
        }

        public interface IListener
        {
            void OnSelectPaper(ItemData paperItem);
        }
    }
}


