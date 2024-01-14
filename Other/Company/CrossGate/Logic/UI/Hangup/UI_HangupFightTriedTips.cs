using Logic.Core;
using Table;
using UnityEngine.UI;
using UnityEngine;
using Lib.Core;
using System;
using Packet;

namespace Logic
{
    public class UI_HangupFightTriedTips : UIBase
    {
        private Color[] colors = new Color[3];
        private Text notWorkTime;
        private Slider slider;
        private Text text;
        private Text text_Num;
        
        protected override void OnLoaded() {
            colors[0] = transform.Find("Animator/Tip/Text_Num/Text").GetComponent<Text>().color;
            colors[1] = transform.Find("Animator/Tip/Text1").GetComponent<Text>().color;
            colors[2] = transform.Find("Animator/Tip/Text2").GetComponent<Text>().color;
            notWorkTime = transform.Find("Animator/Tip/Text0/Text").GetComponent<Text>();
            
            slider = transform.Find("Animator/Tip/Image").GetComponent<Slider>();
            text = transform.Find("Animator/Tip/Image/Text").GetComponent<Text>();
            text_Num = transform.Find("Animator/Tip/Text_Num").GetComponent<Text>();
        }
        
        protected override void OnOpened() {
            uint level = Sys_Role.Instance.Role.Level;
            CSVCharacterAttribute.Data cSVCharacterAttributeData = CSVCharacterAttribute.Instance.GetConfData(level);
            if (null == cSVCharacterAttributeData) {
                return;
            }

            long maxExp = (long)cSVCharacterAttributeData.DailyHangupTotalExp;
            CmdHangUpDataNtf cmdHangUpDataNtf = Sys_Hangup.Instance.cmdHangUpDataNtf;
            bool isRestTime = !Sys_Time.IsServerSameDay5(Sys_Time.Instance.GetServerTime(), cmdHangUpDataNtf.RestExpTime);
            long curExp = isRestTime ? 0 : cmdHangUpDataNtf.RestExp;
            float value = maxExp == 0 ? 0 : (float)((double)curExp / (double)maxExp);
            uint uintValue = (uint)(value * 100);
            if (uintValue >= 100) {
                uintValue = 100;
            }
            /// <summary> 滑动条 </summary>
            slider.value = value;
            /// <summary> 疲劳度值 </summary>
            text.text = string.Format("{0}/{1}", curExp, maxExp);
            /// <summary> 疲劳度比例 </summary>
            text_Num.text = string.Format("{0}%", uintValue);
            if (value >= 1f)
            {
                text_Num.color = colors[2];
            }
            else if (value >= 0.5f)
            {
                text_Num.color = colors[1];
            }
            else
            {
                text_Num.color = colors[0];
            }
            
            var t = Mathf.RoundToInt(10000f / Sys_Attr.Instance.GetExpMultiple());
            TextHelper.SetText(notWorkTime, 2104023, t.ToString());
        }
    }
}