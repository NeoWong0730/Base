using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Table;

namespace Logic
{
    public class UI_Partner_Fetter_Result
    {
        public class AtrrCell
        {
            private Transform transform;

            private Text txtName;
            private Text txtValue;

            public void Init(Transform trans)
            {
                transform = trans;
                txtName = transform.Find("Text_Name").GetComponent<Text>();
                txtValue = transform.Find("Text_Value").GetComponent<Text>();
            }

            public void UpdateInfo(uint arrId, uint arrValue)
            {
                CSVAttr.Data arrData = CSVAttr.Instance.GetConfData(arrId);
                if (arrData != null)
                {
                    txtName.text = LanguageHelper.GetTextContent(arrData.name);
                    txtValue.text = Sys_Attr.Instance.GetAttrValue(arrData, arrValue);
                }
            }
        }

        private Transform transform;

        private InfinityGrid _infinityGrid;
        private Dictionary<uint, uint> dict;
        private List<uint> listKeys = new List<uint>();

        public void Init(Transform trans)
        {
            transform = trans;

            _infinityGrid = transform.Find("View_Pandect/Scroll View").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            AtrrCell entry = new AtrrCell();

            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            AtrrCell entry = cell.mUserData as AtrrCell;
            entry.UpdateInfo(listKeys[index], dict[listKeys[index]]);
        }

        public void UpdateInfo()
        {
            listKeys.Clear();
            dict = Sys_Partner.Instance.GetAllAttrs();
            listKeys.AddRange(dict.Keys);

            _infinityGrid.CellCount = listKeys.Count;
            _infinityGrid.ForceRefreshActiveCell();
        }
    }
}
