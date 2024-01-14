using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Logic.Core;
using Table;
using Packet;
using Lib.Core;

namespace Logic
{
    public class UI_Trade_Search_MagicCore_Attr
    {
        public class AttrCell
        {
            private Transform transform;
            private GameObject gameObject;

            private Button _btn;
            private Text _textName;

            public uint _attrId = 0u;

            public void Init(Transform trans)
            {
                transform = trans;
                gameObject = transform.gameObject;

                _btn = transform.Find("Image_BG").GetComponent<Button>();
                _btn.onClick.AddListener(OnClick);

                _textName = transform.Find("Text_name").GetComponent<Text>();
                //_textSelectName = transform.Find("Select/Text_name").GetComponent<Text>();
            }

            private void OnClick()
            {
                Sys_Trade.Instance.eventEmitter.Trigger(Sys_Trade.EEvents.OnSelectCoreAttr, _attrId);
            }

            public void UpdateInfo(uint attrId)
            {
                _attrId = attrId;
                CSVAttr.Data data = CSVAttr.Instance.GetConfData(_attrId);
                _textName.text = LanguageHelper.GetTextContent(data.name);
            }
        }

        private Transform transform;
        private GameObject gameObject;

        private InfinityGrid _infinityGrid;
        private Button _btnClose;
        //private Button _btnConfirm;

        private List<uint> listAttrs = new List<uint>(6);

        public void Init(Transform trans)
        {
            transform = trans;
            gameObject = transform.gameObject;

            _infinityGrid = transform.Find("Scroll View").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;

            _btnClose = transform.Find("Image_Back").GetComponent<Button>();
            _btnClose.onClick.AddListener(() => { Hide(); });
        }

        public void Show()
        {
            gameObject.SetActive(true);

            CalAttrIds();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            AttrCell core = new AttrCell();
            core.Init(cell.mRootTransform);
            cell.BindUserData(core);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            AttrCell core = cell.mUserData as AttrCell;
            core.UpdateInfo(listAttrs[index]);
        }

        private void CalAttrIds()
        {
            listAttrs.Clear();
            CSVPetEquip.Data data = CSVPetEquip.Instance.GetConfData(Sys_Trade.Instance.SelectedCoreId);
            if (data != null)
            {
                listAttrs = Sys_Trade.Instance.LeftCoreAttrIds(data.attr_id);
            }
            
            _infinityGrid.CellCount = listAttrs.Count;
            _infinityGrid.ForceRefreshActiveCell();
        }
    }
}


