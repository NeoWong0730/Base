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
    public class KnowledgeUnlockParam
    {
        public Sys_Knowledge.ETypes knowledgeType;
        public uint sourceId;
    }

    public class UI_Knowledge_Unlock : UIBase
    {
        private Button _btnClose;
        private Text _textTitle;
        private Text _textTip;

        private KnowledgeUnlockParam _param;

        protected override void OnLoaded()
        {            
            _btnClose = transform.Find("Animator/View_TipsBgNew04/Btn_Close").GetComponent<Button>();
            _textTitle = transform.Find("Animator/View_TipsBgNew04/Text_Title").GetComponent<Text>();
            _textTip = transform.Find("Animator/Detail/Text").GetComponent<Text>();

            _btnClose.onClick.AddListener(OnClickClose);
        }

        protected override void OnOpen(object arg)
        {
            _param = null;
            if (arg != null)
                _param = (KnowledgeUnlockParam)arg;
        }

        protected override void OnShow()
        {            
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            uint lanId = 0;
            switch (_param.knowledgeType)
            {
                case Sys_Knowledge.ETypes.Gleanings:
                    lanId = 2030000;
                    break;
                case Sys_Knowledge.ETypes.Annals:
                    lanId = 2030400;
                    break;
                case Sys_Knowledge.ETypes.Fragment:
                    lanId = 2030500;
                    break;
                case Sys_Knowledge.ETypes.Brave:
                    lanId = 2030600;
                    break;
            }

            _textTitle.text = LanguageHelper.GetTextContent(lanId);
            _textTip.text = LanguageHelper.GetTextContent(_param.sourceId);
        }

        private void OnClickClose()
        {
            this.CloseSelf();
        }
    }
}


