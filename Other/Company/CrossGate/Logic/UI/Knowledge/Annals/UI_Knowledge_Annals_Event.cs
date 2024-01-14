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
    public class UI_Knowledge_Annals_Event
    {
        private Transform transform;

        private Image _imgIcon;
        private Button _btnIcon;

        private Text _textEvent;

        private GameObject _goLight;
        private GameObject _goMask;
        private Transform _transRed;

        private uint _eventId;
        private bool _isActive;

        public void Init(Transform trans)
        {
            transform = trans;

            _imgIcon = transform.Find("Image").GetComponent<Image>();
            _btnIcon = transform.Find("Image").GetComponent<Button>();
            _btnIcon.onClick.AddListener(OnClick);

            _textEvent = transform.Find("Image_Name/Text").GetComponent<Text>();

            _goLight = transform.Find("Image_Light").gameObject;
            _goMask = transform.Find("Mask").gameObject;

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

        public void OnDestroy()
        {

        }

        private void OnClick()
        {
            if (_isActive)
            {
                UIManager.OpenUI(EUIID.UI_Knowledge_Annals_Detail, false, _eventId);

                //Sys_Knowledge.Instance.OnDelNewKnowledge(Sys_Knowledge.ETypes.Annals, _eventId);
                _transRed.gameObject.SetActive(false);
            }
            else
            {
                CSVChronology.Data data = CSVChronology.Instance.GetConfData(_eventId);
                if (data != null)
                {
                    KnowledgeUnlockParam param = new KnowledgeUnlockParam();
                    param.knowledgeType = Sys_Knowledge.ETypes.Annals;
                    param.sourceId = data.Source;

                    UIManager.OpenUI(EUIID.UI_Knowledge_Unlock, false, param);
                }
            }
        }

        public void UpdateInfo(uint eventId)
        {
            _eventId = eventId;
            _isActive = Sys_Knowledge.Instance.IsKnowledgeActive(_eventId);

            CSVChronology.Data data = CSVChronology.Instance.GetConfData(_eventId);
            if (data != null)
            {
                _textEvent.text = LanguageHelper.GetTextContent(data.event_titel);
                ImageHelper.SetIcon(_imgIcon, data.icon);
            }

            _goLight.gameObject.SetActive(_isActive);
            ImageHelper.SetImageGray(_imgIcon, !_isActive, true);

            bool isRedPoint = Sys_Knowledge.Instance.IsRedPointByKnowledge(_eventId);
            _transRed.gameObject.SetActive(isRedPoint);
        }
    }
}


