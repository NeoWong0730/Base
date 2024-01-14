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
    public class UI_Trade_Search_Pet_Sort : UI_Trade_Search_Pet_Sort.LevelSort.IListner
    {
        public class LevelSort
        {
            private class LevelCell
            {
                private Transform transform;
                private GameObject gameObject;

                private CP_Toggle _toggle;
                private Text _text;
                private Text _lightText;

                private System.Action<uint> _action;
                private uint _level;

                public void Init(Transform trans)
                {
                    transform = trans;
                    gameObject = transform.gameObject;

                    _toggle = transform.GetComponent<CP_Toggle>();
                    _toggle.onValueChanged.AddListener(OnToggle);

                    _text = transform.Find("Text").GetComponent<Text>();
                    _lightText = transform.Find("Checkmark/LightText").GetComponent<Text>();
                }

                private void OnToggle(bool isOn)
                {
                    if (isOn)
                    {
                        Sys_Trade.Instance.PetLevel = _level;
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
                        _text.text = _lightText.text = LanguageHelper.GetTextContent(2011056, _level.ToString());
                    }

                    _toggle.SetSelected(_level == Sys_Trade.Instance.PetLevel, true);
                }

                public void OnSelect(bool isSelect)
                {
                    _toggle.SetSelected(isSelect, true);
                }
            }

            private Transform transform;
            private GameObject gameObject;
            
            private InfinityGrid _infinityGrid;

            private List<uint> listLevel = new List<uint>(16);
            private IListner _listner;

            public void Init(Transform trans)
            {
                transform = trans;
                gameObject = transform.gameObject;

                _infinityGrid = transform.GetComponent<InfinityGrid>();
                _infinityGrid.onCreateCell += OnCreateCell;
                _infinityGrid.onCellChange += OnCellChange;
            }

            private void OnCreateCell(InfinityGridCell cell)
            {
                LevelCell lvCell = new LevelCell();
                lvCell.Init(cell.mRootTransform);
                lvCell.Register(OnSelectLevel);
                cell.BindUserData(lvCell);
            }

            private void OnCellChange(InfinityGridCell cell, int index)
            {
                LevelCell lvCell = cell.mUserData as LevelCell;
                lvCell.UpdateInfo(listLevel[index]);
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

                Sys_Trade.Instance.PetLevel = 0;//默认选择全部
                _infinityGrid.CellCount = listLevel.Count;
                _infinityGrid.ForceRefreshActiveCell();
            }

            public interface IListner
            {
                void OnSelectLevel(uint level);
            }
        }

        private class PetSort
        {
            private class PetLevelCell
            {
                private class ItemCell
                {
                    private class PetCell
                    {
                        private Transform transform;
                        private GameObject gameObject;

                        private Button _Btn;
                        private Text _textName;
                        private Image _imgIcon;
                        private uint _petId;

                        public void Init(Transform trans)
                        {
                            transform = trans;
                            gameObject = transform.gameObject;

                            _Btn = transform.Find("Image_BG").GetComponent<Button>();
                            _Btn.onClick.AddListener(OnClickPet);

                            _textName = transform.Find("Text_name").GetComponent<Text>();
                            _imgIcon = transform.Find("PetItem/Image_Icon").GetComponent<Image>();
                        }

                        public void Show()
                        {
                            gameObject.SetActive(true);
                        }

                        public void Hide()
                        {
                            gameObject.SetActive(false);
                        }

                        private void OnClickPet()
                        {
                            Sys_Trade.Instance.eventEmitter.Trigger(Sys_Trade.EEvents.OnSelectPet, _petId);
                        }

                        public void UpdateInfo(uint petId)
                        {
                            _petId = petId;

                            CSVPetNew.Data data = CSVPetNew.Instance.GetConfData(_petId);
                            _textName.text = LanguageHelper.GetTextContent(data.name);
                            ImageHelper.SetIcon(_imgIcon, data.icon_id);
                        }
                    }

                    private Transform transform;
                    private GameObject gameObject;

                    List<PetCell> cells = new List<PetCell>(2);

                    public void Init(Transform trans)
                    {
                        transform = trans;
                        gameObject = transform.gameObject;

                        for (int i = 0; i < 2; ++i)
                        {
                            string name = string.Format("Item{0}", i);
                            PetCell cell = new PetCell();
                            cell.Init(transform.Find(name));
                            cells.Add(cell);
                        }
                    }

                    public void Show()
                    {
                        gameObject.SetActive(true);
                    }

                    public void Hide()
                    {
                        gameObject.SetActive(false);
                    }

                    public void UpdateInfo(List<uint> ids)
                    {
                        for (int i = 0; i < cells.Count; ++i)
                        {
                            if (i < ids.Count)
                            {
                                cells[i].Show();
                                cells[i].UpdateInfo(ids[i]);
                            }
                            else
                            {
                                cells[i].Hide();
                            }
                        }
                    }
                }

                private Transform transform;
                private GameObject gameObject;

                private Transform _transTextParent;
                private Text _textLevel;
                //private Transform _cellParent;
                private GameObject _cellTemplate;

                public void Init(Transform trans)
                {
                    transform = trans;
                    gameObject = transform.gameObject;

                    _transTextParent = transform.Find("Image_LvBG");
                    _textLevel = transform.Find("Image_LvBG/Text").GetComponent<Text>();

                    //_cellParent = transform.Find("Items");
                    _cellTemplate = transform.Find("Items").gameObject;
                    _cellTemplate.SetActive(false);
                }

                public void Show()
                {
                    gameObject.SetActive(true);
                }

                public void Hide()
                {
                    gameObject.SetActive(false);
                }

                public void UpdateLevel(uint level)
                {
                    _textLevel.text = LanguageHelper.GetTextContent(2011056, level.ToString());

                    FrameworkTool.DestroyChildren(transform.gameObject, _cellTemplate.name, _transTextParent.name);

                    List<uint> listIds = Sys_Trade.Instance.GetPetByLevel(level);

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
            private GameObject gameObject;

            private Transform _transParent;
            private GameObject _goTemplate;

            public void Init(Transform trans)
            {
                transform = trans;
                gameObject = transform.gameObject;

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
                    PetLevelCell cell = new PetLevelCell();
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
        private PetSort _petSort;

        private uint[] _arrLevel = new uint[] {1, 2, 3, 4, 5, 6, 7};

        public void Init(Transform trans)
        {
            transform = trans;

            Button btnClose = transform.Find("Interrcept_Image").GetComponent<Button>();
            btnClose.onClick.AddListener(() => { Hide(); });

            _levelSort = new LevelSort();
            _levelSort.Init(transform.Find("Toggles_Mask"));
            _levelSort.Register(this);

            _petSort = new PetSort();
            _petSort.Init(transform.Find("Rect"));
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
                _petSort.UpdateInfo(_arrLevel);
            }
            else
            {
                _petSort.UpdateInfo(new uint[] { level });
            }
        }
    }
}


