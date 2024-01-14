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
    public class TradeSearchOraExtraAttrInfo
    {
        public uint infoId;
        public bool isSkill;
    }
    
    public class UI_Trade_Search_Ornament_ExtraAttr
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
                    _text.text = _lightText.text = LanguageHelper.GetTextContent(2011917 + _Type);
                }
    
                _toggle.SetSelected(_Type == 0u, true);
            }
    
            public void OnSelect(bool isSelect)
            {
                _toggle.SetSelected(isSelect, true);
            }
        }
        public class AttrCell 
        {
            public Transform transform;
            public GameObject gameObject;

            private Button _btn;
            private Text _textName;
            //private Text _textSelectName;
            //private Image _imgSelect;
            public TradeSearchOraExtraAttrInfo _info;
            
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
                Sys_Trade.Instance.eventEmitter.Trigger(Sys_Trade.EEvents.OnSelectOraExtraAttr, _info);
            }

            public void UpdateInfo(TradeSearchOraExtraAttrInfo info)
            {
                _info = info;

                if (_info.isSkill)
                {
                    uint skillId = _info.infoId * 1000 + 1;
                    CSVPassiveSkillInfo.Data data = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                    if (data != null)
                        _textName.text  = LanguageHelper.GetTextContent(data.name);
                }
                else
                {
                    CSVAttr.Data data = CSVAttr.Instance.GetConfData(_info.infoId);
                    if (data != null)
                        _textName.text  = LanguageHelper.GetTextContent(data.name);
                }
            }
        }

        private Transform transform;
        private GameObject gameObject;

        private InfinityGrid _infinityTypes;
        private InfinityGrid _infinityIds;

        private Button _btnClose;
        //private Button _btnConfirm;

        private List<uint> listTypes = new List<uint>(){0, 1, 2};
        private List<TradeSearchOraExtraAttrInfo> listIds = new List<TradeSearchOraExtraAttrInfo>();

        public void Init(Transform trans)
        {
            transform = trans;
            gameObject = transform.gameObject;

            _infinityTypes = transform.Find("Toggles_Mask").GetComponent<InfinityGrid>();
            _infinityTypes.onCreateCell += OnCreateTypeCell;
            _infinityTypes.onCellChange += OnChangeTypeCell;

            _infinityIds = transform.Find("Scroll_View").GetComponent<InfinityGrid>();
            _infinityIds.onCreateCell += OnCreateOraCell;
            _infinityIds.onCellChange += OnChangeOraCell;

            _btnClose = transform.Find("Interrcept_Image").GetComponent<Button>();
            _btnClose.onClick.AddListener(() => { Hide(); });
        }

        public void Show()
        {
            gameObject.SetActive(true);

            UpdateInfo();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
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
            AttrCell oraCell = new AttrCell();
            oraCell.Init(cell.mRootTransform);
            cell.BindUserData(oraCell);
        }

        private void OnChangeOraCell(InfinityGridCell cell, int index)
        {
            AttrCell typeCell = cell.mUserData as AttrCell;
            typeCell.UpdateInfo(listIds[index]);
        }

        private void UpdateInfo()
        {
            _infinityTypes.CellCount = listTypes.Count;
            _infinityTypes.ForceRefreshActiveCell();
        }

        private void OnSelectType(uint type)
        {
            listIds.Clear();
            if (type == 0) //全部
            {
                int count = CSVOrnamentsAttributes.Instance.Count;
                for (int i = 0; i < count; ++i)
                {
                    CSVOrnamentsAttributes.Data data = CSVOrnamentsAttributes.Instance.GetByIndex(i);
                    if (data.group_id == 1000)
                    {
                        TradeSearchOraExtraAttrInfo info = new TradeSearchOraExtraAttrInfo();
                        info.infoId = data.attr_id;
                        info.isSkill = false;
                        listIds.Add(info);
                    }
                }

                count = CSVOrnamentsSkill.Instance.Count;
                for (int i = 0; i < count; ++i)
                {
                    CSVOrnamentsSkill.Data data = CSVOrnamentsSkill.Instance.GetByIndex(i);
                    if (data.group_id == 1001)
                    {
                        TradeSearchOraExtraAttrInfo info = new TradeSearchOraExtraAttrInfo();
                        info.infoId = data.skill_id;
                        info.isSkill = true;
                        listIds.Add(info);
                    }
                }
            }
            else if (type == 1) //属性
            {
                int count = CSVOrnamentsAttributes.Instance.Count;
                for (int i = 0; i < count; ++i)
                {
                    CSVOrnamentsAttributes.Data data = CSVOrnamentsAttributes.Instance.GetByIndex(i);
                    if (data.group_id == 1000)
                    {
                        TradeSearchOraExtraAttrInfo info = new TradeSearchOraExtraAttrInfo();
                        info.infoId = data.attr_id;
                        info.isSkill = false;
                        listIds.Add(info);
                    }
                }
            }
            else if (type == 2) //技能
            {
                int count = CSVOrnamentsSkill.Instance.Count;
                for (int i = 0; i < count; ++i)
                {
                    CSVOrnamentsSkill.Data data = CSVOrnamentsSkill.Instance.GetByIndex(i);
                    if (data.group_id == 1001)
                    {
                        TradeSearchOraExtraAttrInfo info = new TradeSearchOraExtraAttrInfo();
                        info.infoId = data.skill_id;
                        info.isSkill = true;
                        listIds.Add(info);
                    }
                }
            }
            
            _infinityIds.CellCount = listIds.Count;
            _infinityIds.ForceRefreshActiveCell();
        }
    }
}


