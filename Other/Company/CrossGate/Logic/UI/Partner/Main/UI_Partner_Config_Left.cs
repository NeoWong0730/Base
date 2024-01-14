using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Packet;
using Lib.Core;

namespace Logic
{
    public class UI_Partner_Config_Left : UIParseCommon
    {
        private ToggleGroup toggleGroup;
        private InfinityGridLayoutGroup gridGroup;
        private Dictionary<GameObject, UI_Partner_Config_Left_Cell> dicCells = new Dictionary<GameObject, UI_Partner_Config_Left_Cell>();
        private int visualGridCount;

        private List<Partner> listPartners = new List<Partner>();

        protected override void Parse()
        {
            toggleGroup = transform.Find("Scroll01/Content").gameObject.GetComponent<ToggleGroup>();
            gridGroup = transform.Find("Scroll01/Content").gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            gridGroup.minAmount = 7;
            gridGroup.updateChildrenCallback = UpdateChildrenCallback;

            for (int i = 0; i < gridGroup.transform.childCount; ++i)
            {
                Transform trans = gridGroup.transform.GetChild(i);

                UI_Partner_Config_Left_Cell cell = new UI_Partner_Config_Left_Cell();
                cell.Init(trans);
                //cell.AddListener(OnSelectIndex);
                dicCells.Add(trans.gameObject, cell);
            }
        }

        public override void Show()
        {
            foreach (UI_Partner_Config_Left_Cell cell in dicCells.Values)
            {
                cell.Show();
            }

            UpdateInfo();
        }

        public override void Hide()
        {
            foreach (UI_Partner_Config_Left_Cell cell in dicCells.Values)
            {
                cell.Hide();
            }
        }

        private void UpdateChildrenCallback(int index, Transform trans)
        {
            if (index < 0 || index >= visualGridCount)
                return;

            if (dicCells.ContainsKey(trans.gameObject))
            {
                UI_Partner_Config_Left_Cell cell = dicCells[trans.gameObject];
                cell.UpdateInfo(listPartners[index], index);
            }
        }

        private void UpdateInfo()
        {
            listPartners.Clear();
            listPartners = Sys_Partner.Instance.GetUnlockPartners();
            visualGridCount = listPartners.Count;
            gridGroup.SetAmount(visualGridCount);
        }
    }
}
