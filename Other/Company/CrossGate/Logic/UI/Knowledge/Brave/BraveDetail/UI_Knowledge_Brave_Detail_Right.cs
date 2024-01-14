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
    public class UI_Knowledge_Brave_Detail_Right
    {
        private class BraveEvent
        {
            private Transform transform;

            private Button _btn;
            private Text _textName;
            private Transform _transRed;

            private Button _btnLock;

            private uint _eventId;
            private bool _isActive;

            public void Init(Transform trans)
            {
                transform = trans;

                _btn = transform.Find("Button").GetComponent<Button>();
                _btn.onClick.AddListener(OnClick);

                _textName = transform.Find("Button/Text").GetComponent<Text>();
                _transRed = transform.Find("Button/Image_Red");

                _btnLock = transform.Find("Image_Lock").GetComponent<Button>();
                _btnLock.onClick.AddListener(OnLock);
            }

            public void OnShow()
            {
                transform.gameObject.SetActive(true);
            }

            public void OnHide()
            {
                transform.gameObject.SetActive(false);
            }

            private void OnClick()
            {
                if (_isActive)
                {
                    UIManager.OpenUI(EUIID.UI_Knowledge_Brave_Stroy, false, _eventId);

                    Sys_Knowledge.Instance.OnDelNewKnowledge(Sys_Knowledge.ETypes.Brave, _eventId);
                    _transRed.gameObject.SetActive(false);
                }
            }

            private void OnLock()
            {
                if (!_isActive)
                {
                    CSVBraveBiography.Data data = CSVBraveBiography.Instance.GetConfData(_eventId);
                    if (data != null)
                    {
                        KnowledgeUnlockParam param = new KnowledgeUnlockParam();
                        param.knowledgeType = Sys_Knowledge.ETypes.Brave;
                        param.sourceId = data.Source;

                        UIManager.OpenUI(EUIID.UI_Knowledge_Unlock, false, param);
                    }
                }
            }

            public void UpdateInfo(uint eventId)
            {
                _eventId = eventId;

                _isActive = Sys_Knowledge.Instance.IsKnowledgeActive(_eventId);

                CSVBraveBiography.Data data = CSVBraveBiography.Instance.GetConfData(eventId);
                if (data != null)
                {
                    _textName.text = LanguageHelper.GetTextContent(data.biography_name);
                }

                _btnLock.gameObject.SetActive(!_isActive);
                _btn.gameObject.SetActive(_isActive);

                bool isRedPoint = Sys_Knowledge.Instance.IsRedPointByKnowledge(_eventId);
                _transRed.gameObject.SetActive(isRedPoint);

                FrameworkTool.ForceRebuildLayout(transform.gameObject);
            }
        }


        private Transform transform;

        private Text _textOccupation; //职业
        private Text _textHeight; //身高
        private Text _textHobby; //爱好
        private Text _textIdea; //理想

        private Text _textChar; //性格
        private Text _textExpert; //擅长
       

        private List<BraveEvent> listEvents = new List<BraveEvent>();

        public void Init(Transform trans)
        {
            transform = trans;

            _textOccupation = transform.Find("Intro/Line/Text_Title/Text").GetComponent<Text>();
            _textHeight = transform.Find("Intro/Line/Text_Title (1)/Text").GetComponent<Text>();

            _textHobby = transform.Find("Intro/Line (1)/Text_Title/Text").GetComponent<Text>();
            _textIdea = transform.Find("Intro/Line (1)/Text_Title (1)/Text").GetComponent<Text>();

            _textChar = transform.Find("Intro/Line (2)/Text_Title/Text").GetComponent<Text>();
            _textExpert = transform.Find("Intro/Line (3)/Text_Title/Text").GetComponent<Text>();
            
            //_textHobby = transform.Find("Intro/Line (2)/Text_Title (1)/Text").GetComponent<Text>();

            Transform transParent = transform.Find("Scroll View/Viewport/Content");
            int count = transParent.childCount;
            for (int i = 0; i < count; ++i)
            {
                Transform transTemp = transParent.GetChild(i);

                BraveEvent evt = new BraveEvent();
                evt.Init(transTemp);

                listEvents.Add(evt);
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

        public void UpdateInfo(uint braveId)
        {
            CSVBrave.Data data = CSVBrave.Instance.GetConfData(braveId);
            if (data != null)
            {
                //intro
                _textOccupation.text = LanguageHelper.GetTextContent(data.Occupation);
                _textHeight.text = LanguageHelper.GetTextContent(data.Height);

                _textHobby.text = LanguageHelper.GetTextContent(data.Hobby);
                _textIdea.text = LanguageHelper.GetTextContent(data.Ideal);

                _textChar.text = LanguageHelper.GetTextContent(data.Character);
                _textExpert.text = LanguageHelper.GetTextContent(data.Weight);

                for (int i = 0; i < listEvents.Count; ++i)
                {
                    listEvents[i].OnHide();

                    if (data.story_id != null  && i < data.story_id.Count)
                    {
                        listEvents[i].OnShow();
                        listEvents[i].UpdateInfo(data.story_id[i]);
                    }
                }
            } 
        }
    }
}


