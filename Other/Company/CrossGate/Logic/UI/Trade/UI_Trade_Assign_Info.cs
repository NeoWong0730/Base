using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_Trade_Assign_Info : UIBase
    {
        private Button _BtnClose;

        private Text _textLeftTime;
        private Text _textAssignName;
        private Text _textAssignPrice;

        private TradeBrief _tradeBrief;
        protected override void OnOpen(object arg)
        {
            _tradeBrief = null;
            if (arg != null)
                _tradeBrief = (TradeBrief)arg;
        }

        protected override void OnLoaded()
        {
            _BtnClose = transform.Find("Image_BG").GetComponent<Button>();
            _BtnClose.onClick.AddListener(() =>{ this.CloseSelf(); });

            _textLeftTime = transform.Find("View_Zhiding/Animator/Line0/Text_Right").GetComponent<Text>();
            _textAssignName = transform.Find("View_Zhiding/Animator/Line1/Text_Right").GetComponent<Text>();
            _textAssignPrice = transform.Find("View_Zhiding/Animator/Line2/Text_Right").GetComponent<Text>();
        }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            uint leftTime = 0u;
            if (_tradeBrief.OnsaleTime > Sys_Time.Instance.GetServerTime())
            {
                leftTime = _tradeBrief.TargetTime - _tradeBrief.OnsaleTime;
            }
            else
            {
                if (_tradeBrief.TargetTime > Sys_Time.Instance.GetServerTime())
                    leftTime = _tradeBrief.TargetTime - Sys_Time.Instance.GetServerTime();
            }

            _textLeftTime.text = LanguageHelper.TimeToString(leftTime, LanguageHelper.TimeFormat.Type_4);
            _textAssignName.text = _tradeBrief.TargetName.ToStringUtf8();
            _textAssignPrice.text = _tradeBrief.TargetPrice.ToString();
        }
    }
}


