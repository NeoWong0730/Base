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
    public class UI_Knowledge_Fragment_Detail : UIBase
    {
        private class CellToggle
        {
            private Transform transform;

            private CP_Toggle _toggle;
            private Transform _transLock;
            private Transform _transRed;

            private uint _storyId;
            private System.Action<uint> _action;
            private bool _isActive;

            public void Init(Transform trans)
            {
                transform = trans;

                _toggle = transform.GetComponent<CP_Toggle>();
                _toggle.onValueChanged.AddListener(OnClickToggle);

                _transLock = transform.Find("Lock");
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

            private void OnClickToggle(bool isOn)
            {
                if (isOn)
                {
                    if (_isActive)
                    {

                        _action?.Invoke(_storyId);

                        Sys_Knowledge.Instance.OnDelNewKnowledge(Sys_Knowledge.ETypes.Fragment, _storyId);
                        _transRed.gameObject.SetActive(false);
                    }
                    else
                    {
                        CSVMemoryPieces.Data data = CSVMemoryPieces.Instance.GetConfData(_storyId);
                        if (data != null)
                        {
                            KnowledgeUnlockParam param = new KnowledgeUnlockParam();
                            param.knowledgeType = Sys_Knowledge.ETypes.Fragment;
                            param.sourceId = data.Source;

                            UIManager.OpenUI(EUIID.UI_Knowledge_Unlock, false, param);
                        }
                    }
                }
            }

            public void Register(System.Action<uint> action)
            {
                _action = action;
            }

            public void UpdateInfo(uint storyId, bool isOn)
            {
                _storyId = storyId;

                _isActive = Sys_Knowledge.Instance.IsKnowledgeActive(_storyId);

                _transLock?.gameObject.SetActive(!_isActive);
                //_toggle.enabled = _isActive;
                if (_isActive)
                    _toggle.SetSelected(isOn, true);

                bool isRedPoint = Sys_Knowledge.Instance.IsRedPointByKnowledge(_storyId);
                _transRed.gameObject.SetActive(isRedPoint);
            }
        }

        private Text _textFragTitle;
        private Image _imgTexture;

        private List<CellToggle> _listToggles = new List<CellToggle>(4);

        private Text _textStoryTitle;
        private Text _textStoryContent;

        private uint _fragId;
        private List<uint> _listStroys;

        protected override void OnLoaded()
        {            
            Button btnClose = transform.Find("Animator/ImageBG/Button_off").GetComponent<Button>();
            btnClose.onClick.AddListener(() => { this.CloseSelf(); });

            _textFragTitle = transform.Find("Animator/ImageBG/Image_Title/Text_Title").GetComponent<Text>();
            _imgTexture = transform.Find("Animator/ImageBG/Image_BG/Image_Mask/Image").GetComponent<Image>();

            Transform transToggle = transform.Find("Animator/ImageBG/Image_StoryBG/Toggles");
            int count = transToggle.childCount;
            for (int i = 0; i < count; ++i)
            {
                Transform temp = transToggle.GetChild(i);

                CellToggle toggle = new CellToggle();
                toggle.Init(temp);
                toggle.Register(OnSelect);

                _listToggles.Add(toggle);
            }

            _textStoryTitle = transform.Find("Animator/ImageBG/Image_StoryBG/Text_Title").GetComponent<Text>();
            _textStoryContent = transform.Find("Animator/ImageBG/Image_StoryBG/Text_Story").GetComponent<Text>();
        }

        protected override void OnOpen(object arg)
        {            
            _fragId = 0;
            if (arg != null)
                _fragId = (uint)arg;
        }
        
        protected override void OnShow()
        {        
            UpdateInfo();
        }

        protected override void OnHide()
        {
            Sys_Knowledge.Instance.eventEmitter.Trigger(Sys_Knowledge.EEvents.OnFragmentRedPointUpdate);
        }        

        private void OnSelect(uint stroyId)
        {
            CSVMemoryPieces.Data data = CSVMemoryPieces.Instance.GetConfData(stroyId);
            if (data != null)
            {
                _textStoryTitle.text = LanguageHelper.GetTextContent(data.memory_name);
                _textStoryContent.text = LanguageHelper.GetTextContent(data.memory_text);
            }
        }

        private void UpdateInfo()
        {
            _textFragTitle.text = LanguageHelper.GetTextContent(_fragId);

            _listStroys = Sys_Knowledge.Instance.GetFragStorys(_fragId);
            if (_listStroys.Count > 0)
            {
                CSVMemoryPieces.Data data = CSVMemoryPieces.Instance.GetConfData(_listStroys[0]);
                if (data != null)
                {
                    ImageHelper.SetIcon(_imgTexture, data.show_image, true);
                }
            }

            uint defaultId = 0u;
            for(int i = 0; i < _listStroys.Count; ++i)
            {
                bool isActive = Sys_Knowledge.Instance.IsKnowledgeActive(_listStroys[i]);
                if(isActive)
                {
                    defaultId = _listStroys[i];
                    break;
                }
            }

            for (int i = 0; i < _listToggles.Count; ++i)
            {
                if (i < _listStroys.Count)
                {
                    _listToggles[i].OnShow();
                    _listToggles[i].UpdateInfo(_listStroys[i], _listStroys[i] == defaultId);
                }
                else
                {
                    _listToggles[i].OnHide();
                }
            }
        }
    }
}


