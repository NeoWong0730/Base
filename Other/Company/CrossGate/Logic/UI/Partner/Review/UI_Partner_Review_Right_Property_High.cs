
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Packet;
using Table;
using Lib.Core;

namespace Logic
{
    public class UI_Partner_Review_Right_Property_High
    {
        public class AttrCell
        {
            private Transform transform;

            private Text textName;
            private Text textNum;
            public void Init(Transform trans)
            {
                transform = trans;
                textName = transform.Find("Attr").GetComponent<Text>();
                textNum = transform.Find("Text_Number").GetComponent<Text>();
            }

            public void UpdateInfo(AttributeRow attr)
            {
                CSVAttr.Data arrData = CSVAttr.Instance.GetConfData(attr.Id);
                textName.text = LanguageHelper.GetTextContent(arrData.name);
                textNum.text = Sys_Attr.Instance.GetAttrValue(arrData, attr.Value);
            }
        }

        private Transform transform;

        private InfinityGrid _infinityGrid;
        private List<AttributeRow> listAttrs = new List<AttributeRow>();

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
            listAttrs = Sys_Partner.Instance.GetPartnerAttr(infoId, partnerLevel, EAttryType.High);

            _infinityGrid.CellCount = listAttrs.Count;
            _infinityGrid.ForceRefreshActiveCell();
        }
    }
}


