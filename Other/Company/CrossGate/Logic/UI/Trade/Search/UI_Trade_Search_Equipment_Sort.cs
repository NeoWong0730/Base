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
    public class UI_Trade_Search_Equipment_Sort : UI_Trade_Search_Equipment_Sort.LevelSort.IListner
    {
        public class LevelSort
        {
            public class LevelCell
            {
                private Transform transform;

                private CP_Toggle _toggle;
                private Text _text;
                private Text _lightText;

                private System.Action<uint> _action;
                private uint _level;

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
                        Sys_Trade.Instance.EquipLevel = _level;
                        _action?.Invoke(_level);
                    }
                }

                public void Register(System.Action<uint> action)
                {
                    _action = action;
                }

                public void UpdateInfo(uint level)
                {
                    _level = level;
                    if (_level == 0u) //全部
                    {
                        _text.text = _lightText.text = LanguageHelper.GetTextContent(2011055);
                    }
                    else
                    {
                        //显示穿戴等级
                        _text.text = _lightText.text = LanguageHelper.GetTextContent(2011056, (_level / 10 + 1).ToString());
                    }

                    _toggle.SetSelected(_level == Sys_Trade.Instance.EquipLevel, true);
                }

                public void OnSelect(bool isSelect)
                {
                    _toggle.SetSelected(isSelect, true);
                }
            }

            private Transform transform;

            private GameObject goTemplate;

            private List<uint> listLevel = new List<uint>(16);
            private IListner _listner;

            public void Init(Transform trans)
            {
                transform = trans;

                goTemplate = transform.Find("Toggles/Toggle0").gameObject;
                goTemplate.SetActive(false);
            }

            private void OnSelectLevel(uint level)
            {
                _listner?.OnSelectLevel(level);
            }

            public void Register(IListner listner)
            {
                _listner = listner;
            }

            public void UpdateInfo(uint[] levelArray)
            {
                listLevel.Clear();
                listLevel.Add(0); //多一个全部
                listLevel.AddRange(levelArray);

                Sys_Trade.Instance.EquipLevel = 0;//默认选择全部

                FrameworkTool.DestroyChildren(goTemplate.transform.parent.gameObject, goTemplate.name);

                for (int i = 0; i < listLevel.Count; ++i)
                {
                    GameObject go = GameObject.Instantiate<GameObject>(goTemplate);
                    go.transform.SetParent(goTemplate.transform.parent);
                    go.SetActive(true);
                    //rGo.name = rTemplateGo.name;
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = goTemplate.transform.localScale;

                    LevelCell cell = new LevelCell();
                    cell.Init(go.transform);
                    cell.Register(OnSelectLevel);
                    cell.UpdateInfo(listLevel[i]);
                }
            }

            public interface IListner
            {
                void OnSelectLevel(uint level);
            }
        }

        private class EquipSort
        {
            private class EquipLevelCell
            {
                private class ItemCell
                {
                    private class EquipCell
                    {
                        private Transform transform;

                        private Button _Btn;
                        private Text _textName;
                        private Image _imgIcon;
                        private uint _equipInfoId;

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
                            Sys_Trade.Instance.eventEmitter.Trigger(Sys_Trade.EEvents.OnSelectEquip, _equipInfoId);
                        }

                        public void UpdateEquip(uint equipId)
                        {
                            _equipInfoId = equipId;

                            CSVItem.Data item = CSVItem.Instance.GetConfData(_equipInfoId);

                            _textName.text = LanguageHelper.GetTextContent(item.name_id);
                            ImageHelper.SetIcon(_imgIcon, item.icon_id);
                        }
                    }

                    private Transform transform;

                    List<EquipCell> cells = new List<EquipCell>(2);

                    public void Init(Transform trans)
                    {
                        transform = trans;

                        for (int i = 0; i < 2; ++i)
                        {
                            EquipCell cell = new EquipCell();
                            cell.Init(transform.Find(string.Format("Item{0}", i)));
                            cells.Add(cell);
                        }
                    }

                    public void Show()
                    {
                        transform.gameObject.SetActive(true);
                    }

                    public void Hide()
                    {
                        transform.gameObject.SetActive(false);
                    }

                    public void UpdateInfo(List<uint> ids)
                    {
                        for (int i = 0; i < cells.Count; ++i)
                        {
                            if (i < ids.Count)
                            {
                                cells[i].Show();
                                cells[i].UpdateEquip(ids[i]);
                            }
                            else
                            {
                                cells[i].Hide();
                            }
                        }
                    }
                }

                private Transform transform;

                private Transform _transTextParent;
                private Text _textLevel;
                //private Transform _cellParent;
                private GameObject _cellTemplate;

                public void Init(Transform trans)
                {
                    transform = trans;
  
                    _transTextParent = transform.Find("Image_LvBG");
                    _textLevel = transform.Find("Image_LvBG/Text").GetComponent<Text>();

                    //_cellParent = transform.Find("Items");
                    _cellTemplate = transform.Find("Items").gameObject;
                    _cellTemplate.SetActive(false);
                }

                public void Show()
                {
                    transform.gameObject.SetActive(true);
                }

                public void Hide()
                {
                    transform.gameObject.SetActive(false);
                }

                public void UpdateLevel(uint level)
                {
                    _textLevel.text = LanguageHelper.GetTextContent(2011056, (level / 10 + 1).ToString());

                    FrameworkTool.DestroyChildren(transform.gameObject, _cellTemplate.name, _transTextParent.name);

                    List<uint> listIds = Sys_Trade.Instance.GetEquipByLevel(level);

                    int cellCount = listIds.Count / 2;
                    cellCount = listIds.Count % 2 == 0 ? cellCount : cellCount + 1;

                    for (int i = 0; i < cellCount; ++i)
                    {
                        List<uint> ids = new List<uint>(2);
                        int index = i * 2;
                        for (int k = index; k <= index + 1; ++k)
                        {
                            if (k < listIds.Count)
                                ids.Add(listIds[k]);
                        }

                        if (ids.Count > 0)
                        {
                            GameObject go = GameObject.Instantiate(_cellTemplate, transform);
                            ItemCell cell = new ItemCell();
                            cell.Init(go.transform);
                            cell.UpdateInfo(ids);
                            cell.Show();
                        }
                    }
                }
            }

            private Transform transform;

            private Transform _transParent;
            private GameObject _goTemplate;

            public void Init(Transform trans)
            {
                transform = trans;

                _transParent = transform.Find("Rectlist");
                _goTemplate = transform.Find("Rectlist/Group").gameObject;
                _goTemplate.SetActive(false);
            }

            public void UpdateInfo(uint[] levels)
            {
                FrameworkTool.DestroyChildren(_transParent.gameObject, new string[] { _goTemplate.name });
                for (int i = 0; i < levels.Length; ++i)
                {
                    GameObject go = GameObject.Instantiate(_goTemplate, _transParent);
                    EquipLevelCell cell = new EquipLevelCell();
                    cell.Init(go.transform);
                    cell.UpdateLevel(levels[i]);
                    cell.Show();
                }

                //设置大小
                FrameworkTool.ForceRebuildLayout(transform.gameObject);
            }
        }

        private Transform transform;

        private LevelSort _levelSort;
        private EquipSort _equipSort;

        private uint[] _arrLevel = new uint[] {40, 50, 60, 70, 80, 90 };

        public void Init(Transform trans)
        {
            transform = trans;

            Button btnClose = transform.Find("Interrcept_Image").GetComponent<Button>();
            btnClose.onClick.AddListener(() => { Hide(); });

            _levelSort = new LevelSort();
            _levelSort.Init(transform.Find("Toggles_Mask"));
            _levelSort.Register(this);

            _equipSort = new EquipSort();
            _equipSort.Init(transform.Find("Rect"));
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);
            _levelSort.UpdateInfo(_arrLevel);
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }

        public void OnSelectLevel(uint level)
        {
            if (level == 0u)
            {
                _equipSort.UpdateInfo(_arrLevel);
            }
            else
            {
                _equipSort.UpdateInfo(new uint[] { level });
            }
        }
    }
}


