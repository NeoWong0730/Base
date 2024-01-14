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
    /// <summary>
    /// 附加属性，即绿字属性
    /// </summary>
    public class UI_Trade_Search_Equipment_AdditionAttr : UI_Trade_Search_Equipment_AdditionAttr.AttrCell.IListener
    {
        private class SelectedShow
        {
            public Transform transform;
            public GameObject gameObject;

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

            public void UpdateSelectInfo()
            {
                List<uint> ids = Sys_Trade.Instance.EquipAddtionData.attrIds;

                for (int i = 0; i < _textArray.Length; ++i)
                    _textArray[i].transform.parent.gameObject.SetActive(false);

                for (int i = 0; i < ids.Count; ++i)
                {
                    if (i < _textArray.Length)
                    {
                        _textArray[i].transform.parent.gameObject.SetActive(true);

                        CSVAttr.Data data = CSVAttr.Instance.GetConfData(ids[i]);
                        if (data != null)
                        {
                            _textArray[i].text = LanguageHelper.GetTextContent(data.name);
                        }
                    }
                }
            }
        }

        public class AttrCell
        {
            private Transform transform;
            private GameObject gameObject;

            private Button _btn;
            private Text _textName;
            private Text _textSelectName;
            private Image _imgSelect;

            public uint AttrId { set; get; } = 0u;
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

            public void UpdateInfo(uint attrId)
            {
                AttrId = attrId;

                CSVAttr.Data data = CSVAttr.Instance.GetConfData(AttrId);
                if (data != null)
                {
                    _textName.text = _textSelectName.text = LanguageHelper.GetTextContent(data.name);
                }

                OnSelect(Sys_Trade.Instance.EquipAddtionData.attrIds.Contains(AttrId));
            }

            public void OnSelect(bool isSelect)
            {
                IsSelect = isSelect;
                _imgSelect.gameObject.SetActive(isSelect);
            }

            public interface IListener
            {
                void OnSelectSpecial(AttrCell cell);
            }
        }

        private Transform transform;
        private GameObject gameObject;

        private SelectedShow _selectedShow;

        private InfinityGridLayoutGroup gridGroup;
        private int visualGridCount;
        private Dictionary<GameObject, AttrCell> dicCells = new Dictionary<GameObject, AttrCell>();

        private Button _btnClose;
        private Button _btnConfirm;

        private List<uint> _attrIds = new List<uint>(32);

        public void Init(Transform trans)
        {
            transform = trans;
            gameObject = transform.gameObject;

            _selectedShow = new SelectedShow();
            _selectedShow.Init(transform.Find("Image_Top"));

            gridGroup = transform.Find("Tips_Rect/Rectlist").gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            gridGroup.minAmount = 12;
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

            _btnConfirm = transform.Find("Bottom/Btn_04").GetComponent<Button>();
            _btnConfirm.onClick.AddListener(() => {
                Sys_Trade.Instance.eventEmitter.Trigger(Sys_Trade.EEvents.OnSelectEquipAdditionAttr);
            });
        }

        public void Show()
        {
            gameObject.SetActive(true);

            _selectedShow.UpdateSelectInfo();

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
                cell.Register(this);
                cell.UpdateInfo(_attrIds[index]);
            }
        }

        private void CalAttrIds()
        {
            _attrIds.Clear();

            CSVEquipment.Data equipInfo = CSVEquipment.Instance.GetConfData(Sys_Trade.Instance.SelectedEquipId);
            if (equipInfo != null)
            {
                foreach (var data in CSVGreen.Instance.GetAll())
                {
                    if (data.group_id == equipInfo.green_id)
                    {
                        _attrIds.Add(data.attr_id);
                    }
                }
            }

            visualGridCount = _attrIds.Count;
            gridGroup.SetAmount(visualGridCount);
        }

        public void OnSelectSpecial(AttrCell cell)
        {
            if (!cell.IsSelect)
            {
                if (Sys_Trade.Instance.EquipAddtionData.attrIds.Count < 2)
                {
                    Sys_Trade.Instance.EquipAddtionData.attrIds.Add(cell.AttrId);
                    cell.OnSelect(true);

                    _selectedShow.UpdateSelectInfo();
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011199));
                }
            }
            else
            {
                Sys_Trade.Instance.EquipAddtionData.attrIds.Remove(cell.AttrId);
                cell.OnSelect(false);
                _selectedShow.UpdateSelectInfo();
            }
        }
    }
}


