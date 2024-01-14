using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Table;
using Logic.Core;
using Packet;

namespace Logic
{
    public class UI_Partner_Fetter_List
    {
        public class BondCell
        {
            private Transform transform;

            private Image imgIcon;
            private Text txtName;
            private Text txtActive;
            private Button btnDetail;

            private uint bondId;

            public void Init(Transform trans)
            {
                transform = trans;

                imgIcon = transform.Find("Image_Icon").GetComponent<Image>();
                txtName = transform.Find("Image_Name/Text_Name").GetComponent<Text>();
                txtActive = transform.Find("Text_Activate").GetComponent<Text>();
                btnDetail = transform.Find("Btn_Details").GetComponent<Button>();
                btnDetail.onClick.AddListener(OnClick);
            }

            private void OnClick()
            {
                UIManager.OpenUI(EUIID.UI_Partner_Fetter_Detail, false, this.bondId);
            }

            public void UpdateInfo(uint bondId)
            {
                this.bondId = bondId;
                CSVBond.Data data = CSVBond.Instance.GetConfData(this.bondId);
                ImageHelper.SetIcon(imgIcon, data.icon);
                txtName.text = LanguageHelper.GetTextContent(data.name);

                uint count = 0;
                PartnerBond bond = Sys_Partner.Instance.GetBondData(this.bondId);
                if (bond != null)
                    count = bond.Index;
                txtActive.text = LanguageHelper.GetTextContent(2006072, count.ToString(), data.group_effect.Count.ToString());
            }
        }

        private Transform transform;

        private InfinityGrid _infinityGrid;

        private List<uint> listIds = new List<uint>();

        public void Init(Transform trans)
        {
            transform = trans;

            _infinityGrid = transform.GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            BondCell entry = new BondCell();
            entry.Init(cell.mRootTransform);
            //entry.AddListener(OnSelectIndex);

            cell.BindUserData(entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            BondCell entry = cell.mUserData as BondCell;
            entry.UpdateInfo(listIds[index]);
        }

        public void UpdateInfo()
        {
            listIds.Clear();
            int count = CSVBond.Instance.Count;
            for (int i = 0; i < count; ++i)
                listIds.Add(CSVBond.Instance.GetByIndex(i).id);

            _infinityGrid.CellCount = listIds.Count;
            _infinityGrid.ForceRefreshActiveCell();
            _infinityGrid.MoveToIndex(0);
        }
    }
}
