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
    public class UI_Knowledge_Brave_Story : UIBase
    {
        private Image _imgHead;
        private Text _textContent;
        private uint _eventId;

        protected override void OnLoaded()
        {            
            Button btnClose = transform.Find("Black").GetComponent<Button>();
            btnClose.onClick.AddListener(()=> { this.CloseSelf(); });

            _imgHead = transform.Find("Animator/Image_BG/Image/Head").GetComponent<Image>();
            _textContent = transform.Find("Animator/Image_BG/Scroll View/Text_Detail").GetComponent<Text>();
        }

        protected override void OnOpen(object arg)
        {            
            _eventId = 0;
            if (arg != null)
                _eventId = (uint)arg;
        }
        
        protected override void OnShow()
        {
            UpdateInfo();
        }        

        private void UpdateInfo()
        {
            CSVBraveBiography.Data data = CSVBraveBiography.Instance.GetConfData(_eventId);
            if (data != null)
            {
                _textContent.text = LanguageHelper.GetTextContent(data.biography_text);
            }

            uint braveId = Sys_Knowledge.Instance.GetBraveId(_eventId);
            CSVBrave.Data dataBrave = CSVBrave.Instance.GetConfData(braveId);
            if (dataBrave != null)
            {
                ImageHelper.SetIcon(_imgHead, dataBrave.icon);
            }
        }
    }
}


