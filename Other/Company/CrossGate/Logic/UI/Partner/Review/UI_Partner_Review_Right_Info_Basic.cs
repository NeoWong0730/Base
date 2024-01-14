using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Packet;
using Table;

namespace Logic
{
    public class UI_Partner_Review_Right_Info_Basic
    {
        public class AttrData
        {
            public uint nameId;
            public int numValue;
        }

        public class AttrCell
        {
            private Transform transform;

            private Text textName;
            private Text textNum;
            public void Init(Transform trans)
            {
                transform = trans;
                textName = transform.GetComponent<Text>();
                textNum = transform.Find("Text_Number").GetComponent<Text>();
            }

            public void UpdateInfo(AttrData attr)
            {
                textName.text = LanguageHelper.GetTextContent(attr.nameId);
                textNum.text = attr.numValue.ToString();
            }
        }

        private Transform transform;

        private InfinityGrid _infinityGrid;
        private List<AttrData> listAttrs = new List<AttrData>();

        public void Init(Transform trans)
        {
            transform = trans;

            _infinityGrid = transform.Find("Scroll View").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnChangeCell;
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            AttrCell attrCell = new AttrCell();
            attrCell.Init(cell.mRootTransform);
            cell.BindUserData(attrCell);
        }

        private void OnChangeCell(InfinityGridCell cell, int index)
        {
            AttrCell attrCell = cell.mUserData as AttrCell;
            attrCell.UpdateInfo(listAttrs[index]);
        }

        public void UpdateInfo(uint infoId)
        {
            uint partnerLevel = 1;
            Partner partner = Sys_Partner.Instance.GetPartnerInfo(infoId);
            if (partner != null)
            {
                partnerLevel = partner.Level;
            }

            listAttrs.Clear();
            List<AttributeRow> listTemps = Sys_Partner.Instance.GetPartnerAttr(infoId, partnerLevel, EAttryType.Basic, true);
            Dictionary<uint, uint> addAttrs = Sys_Partner.Instance.GetAllAttrs();
            for (int i = 0; i < listTemps.Count; ++i)
            {
                AttributeRow attrRow = listTemps[i];
                CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(attrRow.Id);

                uint addValue = 0;
                addAttrs.TryGetValue(attrRow.Id, out addValue);
                
                AttrData data = new AttrData();
                data.nameId = attrData.name;
                data.numValue = attrRow.Value + Sys_Partner.Instance.GetChangeValue(infoId, attrRow.Id, attrRow.Value) + (int)addValue;

                listAttrs.Add(data);
            }

            _infinityGrid.CellCount = listAttrs.Count;
            _infinityGrid.ForceRefreshActiveCell();
        }
    }
}


