using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lib.Core;

namespace Logic
{
    public class UI_Partner_Config_Right : UIParseCommon
    {
        //private ToggleGroup toggleGroup;
        private InfinityGridLayoutGroup gridGroup;
        private Dictionary<GameObject, UI_Partner_Config_Right_Cell> dicCells = new Dictionary<GameObject, UI_Partner_Config_Right_Cell>();
        private int visualGridCount;

        protected override void Parse()
        {
            //toggleGroup = transform.Find("Scroll_Partner/Content").gameObject.GetComponent<ToggleGroup>();
            //toggleGroup.allowSwitchOff = false;
            gridGroup = transform.Find("Scroll_Partner/Content").gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            gridGroup.minAmount = 3;
            gridGroup.updateChildrenCallback = UpdateChildrenCallback;

            visualGridCount = gridGroup.transform.childCount;
            for (int i = 0; i < gridGroup.transform.childCount; ++i)
            {
                Transform trans = gridGroup.transform.GetChild(i);

                UI_Partner_Config_Right_Cell cell = new UI_Partner_Config_Right_Cell();
                cell.Init(trans);
                dicCells.Add(trans.gameObject, cell);
            }
        }

        public override void Show()
        {
            UpdateInfo();

            foreach (UI_Partner_Config_Right_Cell cell in dicCells.Values)
            {
                cell.Show();
            }
        }

        public override void Hide()
        {
            foreach (UI_Partner_Config_Right_Cell cell in dicCells.Values)
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
                UI_Partner_Config_Right_Cell cell = dicCells[trans.gameObject];
                cell.UpdateInfo(index);
            }
        }

        private void UpdateInfo()
        {
            gridGroup.SetAmount(visualGridCount);
        }
    }
}
