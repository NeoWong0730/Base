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
    public class UI_Knowledge_Annals_Detail_Right
    {
        private class CellEvent
        {
            private Transform transform;

            private CP_Toggle _toggle;

            private Text _textTitle;
            private Transform _transRed;

            private int _index;
            private System.Action<int> _action;
            private bool _isActive;

            public void Init(Transform trans, int index = 0)
            {
                transform = trans;

                _index = index;

                _toggle = transform.GetComponent<CP_Toggle>();
                _toggle.onValueChanged.AddListener(OnClick);

                _textTitle = transform.Find("Text").GetComponent<Text>();
                _transRed = transform.Find("Image_Red");
            }

            public void OnShow()
            {
                transform.gameObject.SetActive(true);
            }

            public void OnHide()
            {
                transform.gameObject.SetActive(false);
            }

            private void OnClick(bool isOn)
            {
                if (isOn)
                    _action?.Invoke(_index);
            }

            public void Register(System.Action<int> action)
            {
                _action = action;
            }

            public void SetSiblingIndex(int index)
            {
                transform.SetSiblingIndex(index);
            }

            public void OnSelect(bool isSelect)
            {
                _toggle.SetSelected(isSelect, true);
            }

            public void UpdateInfo(uint eventId)
            {
                CSVChronology.Data data = CSVChronology.Instance.GetConfData(eventId);
                if (data != null)
                {
                    _textTitle.text =  LanguageHelper.GetTextContent(data.event_titel);
                }

                _isActive = Sys_Knowledge.Instance.IsKnowledgeActive(eventId);
                ImageHelper.SetImageGray(transform, !_isActive, true);
                _toggle.enabled = _isActive;

                _transRed.gameObject.SetActive(Sys_Knowledge.Instance.IsRedPointByKnowledge(eventId));
            }
        }

        private Transform transform;

        private Transform _parent;
        private List<CellEvent> listEvents = new List<CellEvent>(4);

        private UI_Knowledge_Annals_Detail_Right_Info _detailInfo;

        private List<uint> _listEventIds = new List<uint>();

        public void Init(Transform trans)
        {
            transform = trans;

            _parent = transform.Find("Content");
            
            for (int i = 0; i < 4; ++i)
            {
                string str = string.Format("Image_Title{0}", i);

                CellEvent evt = new CellEvent();
                evt.Init(_parent.Find(str), i);
                evt.Register(OnClickEvent);
                listEvents.Add(evt);
            }

            _detailInfo = new UI_Knowledge_Annals_Detail_Right_Info();
            _detailInfo.Init(transform.Find("Content/Image_Detail"));
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

        }

        private void OnClickEvent(int cellIndex)
        {
            int index = 0;
            for (int i = 0; i <= cellIndex; ++i)
            {
                listEvents[i].SetSiblingIndex(index++);
            }

            //_detailInfo.OnShow();
            _detailInfo.SetSliblingIndex(index++);

            for (int i = cellIndex + 1 ; i < listEvents.Count; ++i)
            {
                listEvents[i].SetSiblingIndex(index++);
            }

            if (cellIndex < _listEventIds.Count)
            {
                _detailInfo.UpdateInfo(_listEventIds[cellIndex]);
                Sys_Knowledge.Instance.OnDelNewKnowledge(Sys_Knowledge.ETypes.Annals, _listEventIds[cellIndex]);
                listEvents[cellIndex].UpdateInfo(_listEventIds[cellIndex]);
            }

            Sys_Knowledge.Instance.eventEmitter.Trigger(Sys_Knowledge.EEvents.OnAnnalsRedPointUpdate);
        }

        public void UpdateInfo(uint yearId, uint eventId = 0)
        {
            _listEventIds = Sys_Knowledge.Instance.GetAnnalEvents(yearId);

            uint selectId = eventId;
            if (selectId == 0u && _listEventIds.Count > 0)
            {
                for (int i = 0; i < _listEventIds.Count; ++i)
                {
                    if (Sys_Knowledge.Instance.IsKnowledgeActive(_listEventIds[i]))
                    {
                        selectId = _listEventIds[i];
                        break;
                    }
                }
            }
                //selectId = _listEventIds[0];

            for (int i = 0; i < listEvents.Count; ++i)
            {
                if (i < _listEventIds.Count)
                {
                    listEvents[i].OnShow();
                    listEvents[i].UpdateInfo(_listEventIds[i]);
                }
                else
                {
                    listEvents[i].OnHide();
                }
            }

            int index = _listEventIds.IndexOf(selectId);
            listEvents[index].OnSelect(true);
        }
    }
}


