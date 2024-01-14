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
    public class UI_Knowledge_Annals_Cell
    {
        private class CellLine
        {
            private Transform transform;

            private Transform _transFirst;
            private Text _textYear;

            public void Init(Transform trans)
            {
                transform = trans;

                _transFirst = transform.Find("Image_Line (2)");
                _textYear = transform.Find("Text_Year").GetComponent<Text>();
            }

            public void OnShow()
            {
                transform.gameObject.SetActive(true);
            }

            public void OnHide()
            {
                transform.gameObject.SetActive(false);
            }

            public void UpdateInfo(uint yearId)
            {
                _textYear.text = LanguageHelper.GetTextContent(yearId);
            }
        }

        private Transform transform;

        private GameObject _goMainLight;

        private List<UI_Knowledge_Annals_Event> listEvents = new List<UI_Knowledge_Annals_Event>(4);

        private CellLine _defaultLine;
        private CellLine _lightLine;

        private uint _yearId = 0u;
        private bool _isActive = false;

        public void Init(Transform trans)
        {
            transform = trans;

            _goMainLight = transform.Find("ImageGroup/Bottom_Line/Image_Light").gameObject;

            for (int i = 0; i < 4; ++i)
            {
                Transform temp = transform.Find(string.Format("ImageGroup/Item{0}", i));
                UI_Knowledge_Annals_Event evt = new UI_Knowledge_Annals_Event();
                evt.Init(temp);
                listEvents.Add(evt);
            }

            _defaultLine = new CellLine();
            _defaultLine.Init(transform.Find("Line"));

            _lightLine = new CellLine();
            _lightLine.Init(transform.Find("LightLine"));
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
            for (int i = 0; i < listEvents.Count; ++i)
                listEvents[i].OnDestroy();
        }

        public void UpdateInfo(uint yearId)
        {
            _yearId = yearId;

            _defaultLine.UpdateInfo(_yearId);
            _lightLine.UpdateInfo(_yearId);

            List<uint> events = Sys_Knowledge.Instance.GetAnnalEvents(yearId);

            //判断当前纪年是否开启
            _isActive = false;
            for (int i = 0; i < events.Count; ++i)
            {
                bool isActive = Sys_Knowledge.Instance.IsKnowledgeActive(events[i]);
                if (isActive)
                {
                    _isActive = true;
                    break;
                }
            }

            if (_isActive)
                _lightLine.OnShow();
            else
                _lightLine.OnHide();

            _goMainLight.gameObject.SetActive(_isActive);

            //魔力纪年事件
            for (int i = 0; i < listEvents.Count; ++i)
            {
                if (i < events.Count)
                {
                    listEvents[i].OnShow();
                    listEvents[i].UpdateInfo(events[i]);
                }
                else
                {
                    listEvents[i].OnHide();
                }
            }
        }
    }
}


