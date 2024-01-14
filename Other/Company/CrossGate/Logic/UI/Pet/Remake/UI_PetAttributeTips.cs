using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
namespace Logic
{

    public class UI_PetAttributeTips : UIBase
    {
        private Text describe;
        private Button closeBtn;

        private string tip;

        protected override void OnLoaded()
        {
            describe = transform.Find("ImageBG/Text_Tips").GetComponent<Text>();
            closeBtn = transform.Find("Close").GetComponent<Button>();
            closeBtn.onClick.AddListener(OnCloseClicked);
        }


        protected override void OnOpen(object arg)
        {
            tip = Convert.ToString(arg);
        }

        protected override void OnShow()
        {
            describe.text = tip;
        }

        private void OnCloseClicked()
        {
            CloseSelf();
        }
    }
}