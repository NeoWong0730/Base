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
    public class UI_Trade_Search_Ornament_Sort //: UI_Trade_Search_Ornament_Sort.LevelSort.IListner
    {
        public class TypeCell
        {
            private Transform transform;
    
            private CP_Toggle _toggle;
            private Text _text;
            private Text _lightText;
    
            private System.Action<uint> _action;
            private uint _Type;
    
            public void Init(Transform trans)
            {
                transform = trans;
    
                _toggle = transform.GetComponent<CP_Toggle>();
                _toggle.onValueChanged.AddListener(OnToggle);
    
                _text = transform.Find("Text").GetComponent<Text>();
                _lightText = transform.Find("Checkmark/LightText").GetComponent<Text>();
            }
    
            private void OnToggle(bool isOn)
            {
                if (isOn)
                {
                    Sys_Trade.Instance.SelectOraType = _Type;
                    _action?.Invoke(_Type);
                }
            }
    
            public void Register(System.Action<uint> action)
            {
                _action = action;
            }
    
            public void UpdateInfo(uint type)
            {
                _Type = type;
                if (_Type == 0u) //全部
                {
                    _text.text = _lightText.text = LanguageHelper.GetTextContent(2011190);
                }
                else
                {
                    //显示分类
                    _text.text = _lightText.text = LanguageHelper.GetTextContent(2011912 + _Type);
                }
    
                _toggle.SetSelected(_Type == Sys_Trade.Instance.SelectOraType, true);
            }
    
            public void OnSelect(bool isSelect)
            {
                _toggle.SetSelected(isSelect, true);
            }
        }

        public class OraCell
        {
            private Transform transform;

            private Button _Btn;
            private Text _textName;
            private Image _imgIcon;
            private uint _oraInfoId;

            public void Init(Transform trans)
            {
                transform = trans;

                _Btn = transform.Find("Image_BG").GetComponent<Button>();
                _Btn.onClick.AddListener(OnClickEquip);

                _textName = transform.Find("Text_name").GetComponent<Text>();
                _imgIcon = transform.Find("PropItem/Image_Icon").GetComponent<Image>();
            }

            public void Show()
            {
                transform.gameObject.SetActive(true);
            }

            public void Hide()
            {
                transform.gameObject.SetActive(false);
            }

            private void OnClickEquip()
            {
                Sys_Trade.Instance.eventEmitter.Trigger(Sys_Trade.EEvents.OnSelectOrnament, _oraInfoId);
            }

            public void UpdateInfo(uint oraId)
            {
                _oraInfoId = oraId;

                CSVItem.Data item = CSVItem.Instance.GetConfData(_oraInfoId);

                _textName.text = LanguageHelper.GetTextContent(item.name_id);
                ImageHelper.SetIcon(_imgIcon, item.icon_id);
            }
        }

        private Transform transform;
        private InfinityGrid _infinityTypes;
        private InfinityGrid _infinityOras;

        private List<uint> listTypes = new List<uint>(){0, 1, 2, 3};
        private List<uint> listOras = new List<uint>();
        
        public void Init(Transform trans)
        {
            transform = trans;
            
            Button btnClose = transform.Find("Interrcept_Image").GetComponent<Button>();
            btnClose.onClick.AddListener(() => { Hide(); });

            _infinityTypes = transform.Find("Toggles_Mask").GetComponent<InfinityGrid>();
            _infinityTypes.onCreateCell += OnCreateTypeCell;
            _infinityTypes.onCellChange += OnChangeTypeCell;

            _infinityOras = transform.Find("Scroll_View").GetComponent<InfinityGrid>();
            _infinityOras.onCreateCell += OnCreateOraCell;
            _infinityOras.onCellChange += OnChangeOraCell;
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);

            UpdateInfo();
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }

        private void OnCreateTypeCell(InfinityGridCell cell)
        {
            TypeCell typeCell = new TypeCell();
            typeCell.Init(cell.mRootTransform);
            typeCell.Register(OnSelectType);
            
            cell.BindUserData(typeCell);
        }

        private void OnChangeTypeCell(InfinityGridCell cell, int index)
        {
            TypeCell typeCell = cell.mUserData as TypeCell;
            typeCell.UpdateInfo(listTypes[index]);
        }
        
        private void OnCreateOraCell(InfinityGridCell cell)
        {
            OraCell oraCell = new OraCell();
            oraCell.Init(cell.mRootTransform);
            cell.BindUserData(oraCell);
        }

        private void OnChangeOraCell(InfinityGridCell cell, int index)
        {
            OraCell typeCell = cell.mUserData as OraCell;
            typeCell.UpdateInfo(listOras[index]);
        }

        private void UpdateInfo()
        {
            _infinityTypes.CellCount = listTypes.Count;
            _infinityTypes.ForceRefreshActiveCell();
        }

        private void OnSelectType(uint type)
        {
            listOras.Clear();
            if (type == 0)
            {
                int count = CSVOrnamentsUpgrade.Instance.Count;
                for (int i = 0; i < count; ++i)
                {
                    CSVOrnamentsUpgrade.Data data = CSVOrnamentsUpgrade.Instance.GetByIndex(i);
                    if (data.lv >= 6)
                        listOras.Add(data.id);
                }
            }
            else
            {
                int count = CSVOrnamentsUpgrade.Instance.Count;
                for (int i = 0; i < count; ++i)
                {
                    CSVOrnamentsUpgrade.Data data = CSVOrnamentsUpgrade.Instance.GetByIndex(i);
                    if (data.lv >= 6 && data.type == type)
                        listOras.Add(data.id);
                }
            }

            _infinityOras.CellCount = listOras.Count;
            _infinityOras.ForceRefreshActiveCell();
        }
    }
}


