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
    public class UI_Trade_Search_Equipment_Special : UI_Trade_Search_Equipment_Special.SpecialCell.IListener
    {
        private class SelectedShow
        {
            private Transform transform;
            private GameObject gameObject;

            private Text[] _textArray = new Text[2];

            public void Init(Transform trans)
            {
                transform = trans;
                gameObject = transform.gameObject;

                for (int i = 0; i < 2; ++i)
                {
                    _textArray[i] = transform.Find(string.Format("Mask/Items/Item{0}/Text", i)).GetComponent<Text>();
                }
            }

            public void UpdateSelectInfo(List<uint> ids)
            {
                for (int i = 0; i < _textArray.Length; ++i)
                    _textArray[i].transform.parent.gameObject.SetActive(false);

                for (int i = 0; i < ids.Count; ++i)
                {
                    if (i < _textArray.Length)
                    {
                        _textArray[i].transform.parent.gameObject.SetActive(true);

                        CSVEquipmentEffect.Data data = CSVEquipmentEffect.Instance.GetConfData(ids[i]);
                        if (data != null)
                        {
                            _textArray[i].text = LanguageHelper.GetTextContent(data.name);
                        }
                        //else
                        //{
                        //    _textArray[i].text = LanguageHelper.GetTextContent(2011134);
                        //}
                    }
                }
            }
        }

        public class SpecialCell 
        {
            private Transform transform;
            private GameObject gameObject;

            private Button _btn;
            private Text _textName;
            private Text _textSelectName;
            private Image _imgSelect;

            public uint EffectId { set; get; } = 0u;
            public bool IsSelect { set; get; } = false;

            private IListener _listener;

            public void Init(Transform trans)
            {
                transform = trans;
                gameObject = transform.gameObject;

                _btn = transform.Find("Image_BG").GetComponent<Button>();
                _btn.onClick.AddListener(OnClick);

                _imgSelect = transform.Find("Select").GetComponent<Image>();
                _imgSelect.gameObject.SetActive(false);

                _textName = transform.Find("Text_name").GetComponent<Text>();
                _textSelectName = transform.Find("Select/Text_name").GetComponent<Text>();
            }

            private void OnClick()
            {
                _listener?.OnSelectSpecial(this);
            }

            public void Register(IListener listener)
            {
                _listener = listener;
            }

            public void UpdateInfo(uint effectId)
            {
                EffectId = effectId;

                CSVEquipmentEffect.Data data = CSVEquipmentEffect.Instance.GetConfData(effectId);
                if (data != null)
                {
                    _textName.text = _textSelectName.text = LanguageHelper.GetTextContent(data.name);
                }
                //else
                //{
                //    _textName.text = _textSelectName.text = LanguageHelper.GetTextContent(2011134);
                //}
                OnSelect(Sys_Trade.Instance.EquipSpeicalAttrIds.Contains(EffectId));
            }

            public void OnSelect(bool isSelect)
            {
                IsSelect = isSelect;
                _imgSelect.gameObject.SetActive(isSelect);
            }

            public interface IListener
            {
                void OnSelectSpecial(SpecialCell cell);
            }
        }


        private Transform transform;
        private GameObject gameObject;

        private SelectedShow _selectedShow;

        private InfinityGrid grid;
        private Dictionary<GameObject, SpecialCell> dicCells = new Dictionary<GameObject, SpecialCell>();

        private Button _btnClose;
        private Button _btnConfirm;

        private List<uint> _effectIds = new List<uint>(32);
        private List<uint> m_SelectIds = new List<uint>(4);

        public void Init(Transform trans)
        {
            transform = trans;
            gameObject = transform.gameObject;

            _selectedShow = new SelectedShow();
            _selectedShow.Init(transform.Find("Image_Top"));

            grid = transform.Find("Scroll View").gameObject.GetNeedComponent<InfinityGrid>();
            grid.onCreateCell += OnCreateCell;
            grid.onCellChange += OnCellChange;

            _btnClose = transform.Find("Image_Back").GetComponent<Button>();
            _btnClose.onClick.AddListener(() => { Hide(); });

            _btnConfirm = transform.Find("Bottom/Btn_04").GetComponent<Button>();
            _btnConfirm.onClick.AddListener(() => {

                Sys_Trade.Instance.EquipSpeicalAttrIds.Clear();
                Sys_Trade.Instance.EquipSpeicalAttrIds.AddRange(m_SelectIds);

                Sys_Trade.Instance.eventEmitter.Trigger(Sys_Trade.EEvents.OnSelectEquipSpecialAttr);
            });
        }

        public void Show()
        {
            gameObject.SetActive(true);

            m_SelectIds.Clear();
            m_SelectIds.AddRange(Sys_Trade.Instance.EquipSpeicalAttrIds);

            _selectedShow.UpdateSelectInfo(m_SelectIds);

            CalEffectIds();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            SpecialCell tempCell = new SpecialCell();
            tempCell.Init(cell.mRootTransform);
            tempCell.Register(this);
            cell.BindUserData(tempCell);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            SpecialCell tempCell = cell.mUserData as SpecialCell;
            tempCell.UpdateInfo(_effectIds[index]);
        }

        private void CalEffectIds()
        {
            _effectIds.Clear();

            var dataList = CSVEquipmentEffect.Instance.GetAll();
            for (int i = 0, len = dataList.Count; i < len; ++i)
            {
                var data = dataList[i];
                if (data.group_id == 10u)
                    _effectIds.Add(data.id);
            }

            //foreach(var data in CSVEquipmentEffect.Instance.GetDictData())
            //{
            //    if (data.Value.group_id == 1u)
            //        _effectIds.Add(data.Value.id);
            //}
            grid.CellCount = _effectIds.Count;
            grid.ForceRefreshActiveCell();
        }

        public void OnSelectSpecial(SpecialCell cell)
        {
            if (!cell.IsSelect)
            {
                if (m_SelectIds.Count < 2)
                {
                    m_SelectIds.Add(cell.EffectId);
                    cell.OnSelect(true);

                    _selectedShow.UpdateSelectInfo(m_SelectIds);
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011174));
                }
            }
            else
            {
                m_SelectIds.Remove(cell.EffectId);
                cell.OnSelect(false);
                _selectedShow.UpdateSelectInfo(m_SelectIds);
            }
        }
    }
}


