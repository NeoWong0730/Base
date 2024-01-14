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
    public class UI_Knowledge_Brave_Detail_Left
    {
        private class CellBrave
        {
            private Transform transform;

            private CP_Toggle _toggle;
            private Image _imgHead;
            private Text _textName;
            private Text _textNameSelect;
            private Transform _transRed;

            private uint _braveId;
            private System.Action<uint> _action;

            public void Init(Transform trans)
            {
                transform = trans;

                _toggle = transform.GetComponent<CP_Toggle>();
                _toggle.onValueChanged.AddListener(OnClickToggle);

                _imgHead = transform.Find("Image_Head").GetComponent<Image>();
                _textName = transform.Find("Text").GetComponent<Text>();
                _textNameSelect = transform.Find("Text_Select").GetComponent<Text>();
                _transRed = transform.Find("Image_Red");

                ProcessEvents(true);
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
                ProcessEvents(false);
            }

            private void ProcessEvents(bool register)
            {
                Sys_Knowledge.Instance.eventEmitter.Handle(Sys_Knowledge.EEvents.OnDelNewKnowledgeNtf, OnDelNtf, register);
            }

            private void OnDelNtf()
            {
                bool isRed = Sys_Knowledge.Instance.IsBraveRedPoint(_braveId);
                if (_transRed != null)
                    _transRed.gameObject.SetActive(isRed);
            }

            private void OnClickToggle(bool isOn)
            {
                if (isOn)
                    _action?.Invoke(_braveId);
            }

            public void Register(System.Action<uint> action)
            {
                _action = action;
            }

            public void UpdateInfo(CSVBrave.Data data)//(uint braveId)
            {
                //_braveId = braveId;
                //CSVBrave.Data data = CSVBrave.Instance.GetConfData(_braveId);

                if (data != null)
                {
                    _braveId = data.id;
                    ImageHelper.SetIcon(_imgHead, data.icon);
                    _textName.text = _textNameSelect.text = LanguageHelper.GetTextContent(data.name_id);
                }

                _toggle.SetSelected(Sys_Knowledge.Instance.CurBraveId == _braveId, true);

                bool isRed = Sys_Knowledge.Instance.IsBraveRedPoint(_braveId);
                if (_transRed != null)
                    _transRed.gameObject.SetActive(isRed);
            }
        }

        private Transform transform;

        private List<CellBrave> _listBraves = new List<CellBrave>(6);

        private IListener _listener;

        public void Init(Transform trans)
        {
            transform = trans;

            int count = transform.childCount;
            for (int i = 0; i < count; ++i)
            {
                Transform temp = transform.GetChild(i);

                CellBrave hero = new CellBrave();
                hero.Init(temp);
                hero.Register(OnClickBrave);
                _listBraves.Add(hero);
            }
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
            for (int i = 0; i < _listBraves.Count; ++i)
                _listBraves[i].OnDestroy();
        }

        private void OnClickBrave(uint braveId)
        {
            _listener?.OnSelectBrave(braveId);
        }

        public void Register(IListener listener)
        {
            _listener = listener;
        }

        public void UpdateInfo()
        {
            //List<uint> listIds = new List<uint>(CSVBrave.Instance.GetDictData().Keys);
            var listDatas = CSVBrave.Instance.GetAll();
            for (int i = 0, len = _listBraves.Count; i < len; ++i)
            {
                if (i < listDatas.Count)
                    _listBraves[i].UpdateInfo(listDatas[i]);
            }
        }

        public interface IListener
        {
            void OnSelectBrave(uint braveId);
        }
    }
}


