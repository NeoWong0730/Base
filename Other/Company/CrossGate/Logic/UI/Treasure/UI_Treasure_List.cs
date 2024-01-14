using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Packet;
using Table;
using Logic.Core;

namespace Logic
{
    public class UI_Treasure_List : UIParseCommon, UI_Treasure_List_Top.IListener
    {
        private UI_Treasure_List_Top listTop;
        private Text textOwn;

        private InfinityGridLayoutGroup gridGroup;
        private Dictionary<GameObject, UI_Treasure_List_Cell> dictCells = new Dictionary<GameObject, UI_Treasure_List_Cell>();
        private int visualGridCount;

        private List<CSVTreasures.Data>  mListTreasureIds = new List<CSVTreasures.Data>();

        private Sys_Treasure.ETreasureSortType mSortType;

        protected override void Parse()
        {
            listTop = new UI_Treasure_List_Top();
            listTop.Init(transform.Find("Image_Title_Unlock"));
            listTop.RegisterListener(this);

            textOwn = transform.Find("Image_Title_Unlock/Text_Own").GetComponent<Text>();

            gridGroup = transform.Find("Scroll_View/Viewport").GetComponent<InfinityGridLayoutGroup>();
            gridGroup.minAmount = 24;
            gridGroup.updateChildrenCallback += UpdateChildrenCallback;

            for (int i = 0; i < gridGroup.transform.childCount; ++i)
            {
                Transform trans = gridGroup.transform.GetChild(i);
                UI_Treasure_List_Cell cell = new UI_Treasure_List_Cell();
                cell.Init(trans);
                dictCells.Add(trans.gameObject, cell);
            }
        }

        public override void Show()
        {
            listTop.Show();
        }

        public override void Hide()
        {
            listTop.Hide();
        }

        private void UpdateChildrenCallback(int index, Transform trans)
        {
            if (index < 0 || index >= visualGridCount)
                return;

            if (dictCells.ContainsKey(trans.gameObject))
            {
                UI_Treasure_List_Cell cell = dictCells[trans.gameObject];
                cell.UpdateInfo(mListTreasureIds[index].id);
            }
        }

        public void OnSelectType(Sys_Treasure.ETreasureSortType sortType)
        {
            textOwn.text = Sys_Treasure.Instance.GetOwnShow();

            mSortType = sortType;
            mListTreasureIds = Sys_Treasure.Instance.GetListBySortType(mSortType);
            visualGridCount = mListTreasureIds.Count;
            gridGroup.SetAmount(visualGridCount);
        }

        public void Refresh()
        {
            OnSelectType(mSortType);
        }
    }
}
