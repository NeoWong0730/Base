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
    public class UI_Trade_Search_Equipment_BasicAttr
    {
        public class AttrCell 
        {
            public Transform transform;
            public GameObject gameObject;

            private Button _btn;
            private Text _textName;
            //private Text _textSelectName;
            //private Image _imgSelect;

            public uint _attrId = 0u;

            public void Init(Transform trans)
            {
                transform = trans;
                gameObject = transform.gameObject;

                _btn = transform.Find("Image_BG").GetComponent<Button>();
                _btn.onClick.AddListener(OnClick);

                //_imgSelect = transform.Find("Select").GetComponent<Image>();
                //_imgSelect.gameObject.SetActive(false);

                _textName = transform.Find("Text_name").GetComponent<Text>();
            }

            private void OnClick()
            {
                Sys_Trade.Instance.eventEmitter.Trigger(Sys_Trade.EEvents.OnSelectEquipBasicAttr, _attrId);
            }

            public void UpdateInfo(uint attrId)
            {
                _attrId = attrId;

                CSVAttr.Data data = CSVAttr.Instance.GetConfData(_attrId);
                if (data != null)
                {
                    _textName.text  = LanguageHelper.GetTextContent(data.name);
                }
            }
        }

        private Transform transform;
        private GameObject gameObject;

        private InfinityGridLayoutGroup gridGroup;
        private int visualGridCount;
        private Dictionary<GameObject, AttrCell> dicCells = new Dictionary<GameObject, AttrCell>();

        private Button _btnClose;
        //private Button _btnConfirm;

        private List<uint> _attrIds = new List<uint>(4);

        public void Init(Transform trans)
        {
            transform = trans;
            gameObject = transform.gameObject;

            gridGroup = transform.Find("Tips_Rect/Rectlist").gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            gridGroup.minAmount = 5;
            gridGroup.updateChildrenCallback = UpdateChildrenCallback;

            for (int i = 0; i < gridGroup.transform.childCount; ++i)
            {
                Transform tran = gridGroup.transform.GetChild(i);
                AttrCell cell = new AttrCell();
                cell.Init(tran);
                dicCells.Add(tran.gameObject, cell);
            }

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

        private void UpdateChildrenCallback(int index, Transform trans)
        {
            if (index < 0 || index >= visualGridCount)
                return;

            if (dicCells.ContainsKey(trans.gameObject))
            {
                AttrCell cell = dicCells[trans.gameObject];
                cell.UpdateInfo(_attrIds[index]);
            }
        }

        private void CalAttrIds()
        {
            _attrIds.Clear();

            CSVEquipment.Data data = CSVEquipment.Instance.GetConfData(Sys_Trade.Instance.SelectedEquipId);
            if (data != null)
            {
                for (int i = 0; i < data.attr.Count; ++i)
                {
                    _attrIds.Add(data.attr[i][0]);
                }
            }

            for (int i = 0; i < Sys_Trade.Instance.EquipBasicAttrArray.Count; ++i)
            {
                Sys_Trade.EquipBasicAttr basicAttr = Sys_Trade.Instance.EquipBasicAttrArray[i];
                if (basicAttr.attrId != 0u)
                    _attrIds.Remove(basicAttr.attrId);
            }

            visualGridCount = _attrIds.Count;
            gridGroup.SetAmount(visualGridCount);
        }
    }
}


