using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Lib.Core;
using Logic.Core;
using Table;

namespace Logic
{
    public class UI_Knowledge_Gleanings_Items
    {
        public class CellItem
        {
            private Transform transform;

            private CP_Toggle _toggle;
            private Image _imgItem;
            private Text _textName;

            private Transform _transLock;
            private Transform _transRed;

            private System.Action<uint> _action;
            private uint _knowledgeId;

            public void Init(Transform trans)
            {
                transform = trans;

                _toggle = transform.Find("Btn_Item").GetComponent<CP_Toggle>();
                _toggle.onValueChanged.AddListener(OnClick);

                _imgItem = transform.Find("Btn_Item/Image_BG").GetComponent<Image>();
                _textName = transform.Find("Image_label/Label").GetComponent<Text>();
                _transLock = transform.Find("Image_lockitem");
                _transRed = transform.Find("Image_Red");
            }

            private void OnClick(bool isOn)
            {
                if (isOn)
                {
                    _action?.Invoke(_knowledgeId);

                    Sys_Knowledge.Instance.OnDelNewKnowledge(Sys_Knowledge.ETypes.Gleanings, _knowledgeId);
                    _transRed.gameObject.SetActive(false);
                }
            }

            public void Register(System.Action<uint> action)
            {
                _action = action;
            }

            public void UpdateInfo(uint knowledgeId)
            {
                _knowledgeId = knowledgeId;

                CSVGleanings.Data data = CSVGleanings.Instance.GetConfData(_knowledgeId);
                if (data != null)
                {
                    ImageHelper.SetIcon(_imgItem, data.icon_id, true);
                    _textName.text = LanguageHelper.GetTextContent(data.name_id);

                    bool isActive = Sys_Knowledge.Instance.IsKnowledgeActive(_knowledgeId);
                    _transLock.gameObject.SetActive(!isActive);
                }

                _toggle.SetSelected(_knowledgeId == Sys_Knowledge.Instance.SelectGleaningItemId, true);

                bool isRedPoint = Sys_Knowledge.Instance.IsRedPointByKnowledge(_knowledgeId);
                _transRed.gameObject.SetActive(isRedPoint);
            }
        }

        private Transform transform;

        private InfinityGrid _infinityGrid;

        private UI_Knowledge_Gleanings_Items_Info _rightInfo;
        private Transform _transEmpty;
        private Text _textSource;

        private List<uint> listItems = new List<uint>();

        public void Init(Transform trans)
        {
            transform = trans;

            _infinityGrid = transform.Find("Center/Scroll View").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnChangeCell;

            _rightInfo = new UI_Knowledge_Gleanings_Items_Info();
            _rightInfo.Init(transform.Find("Right"));

            _transEmpty = transform.Find("Right_Empty");
            _textSource = transform.Find("Right_Empty/Text_Detail").GetComponent<Text>();
        }

        public void OnShow()
        {
            transform.gameObject.SetActive(true);
        }

        public void OnHide()
        {
            transform.gameObject.SetActive(false);
        }

        public void OnDestroy()
        {
            //for (int i = 0; i < listEvents.Count; ++i)
            //    listEvents[i].OnDestroy();
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            CellItem item = new CellItem();
            item.Init(cell.mRootTransform);
            item.Register(OnSelectItem);
            cell.BindUserData(item);
        }

        private void OnChangeCell(InfinityGridCell cell , int index)
        {
            CellItem item = cell.mUserData as CellItem;
            item.UpdateInfo(listItems[index]);
        }

        private void OnSelectItem(uint knowledgeId)
        {
            Sys_Knowledge.Instance.SelectGleaningItemId = knowledgeId;

            bool isActive = Sys_Knowledge.Instance.IsKnowledgeActive(knowledgeId);
            if(isActive)
            {
                _transEmpty?.gameObject.SetActive(false);
                _rightInfo.OnShow();
                _rightInfo.UpdateInfo(knowledgeId);
            }
            else
            {
                _transEmpty?.gameObject.SetActive(true);
                _rightInfo.OnHide();

                CSVGleanings.Data data = CSVGleanings.Instance.GetConfData(knowledgeId);
                if (data != null)
                {
                    _textSource.text = LanguageHelper.GetTextContent(data.Source);
                }
            }
        }

        public void UpdateInfo(uint subTypeId)
        {
            listItems = Sys_Knowledge.Instance.GetGleanings(2u, subTypeId);

            if (listItems.Count > 0)
                Sys_Knowledge.Instance.SelectGleaningItemId = listItems[0];

            _infinityGrid.CellCount = listItems.Count;
            _infinityGrid.ForceRefreshActiveCell();
        }
    }
}


