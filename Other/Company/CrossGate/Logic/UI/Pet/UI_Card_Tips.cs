using Framework;
using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;
namespace Logic
{
    public class UI_Card_Tips : UIBase
    {
        private Button closeBtn;
        private Text text;
        private string tipStr;
        protected override void OnLoaded()
        {
            closeBtn = transform.Find("Close").GetComponent<Button>();
            closeBtn.onClick.AddListener(OnCloseBtnClicked);
            text = transform.Find("Text_Property").GetComponent<Text>();
        }

        protected override void OnOpen(object arg = null)
        {
            if (arg == null)
                return;
            tipStr = arg as string;
        }

        protected override void OnShow()
        {
            text.text = tipStr;
        }

        private void OnCloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Card_Tips);
        }
    }
}

