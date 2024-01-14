using Lib.Core;
using Logic.Core;
using System;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Pet_RemakeTips : UIBase
    {
        public Text content;
        public Text leftBtnText;
        public Text rightBtnText;

        public GameObject firstTip;
        public Text fitstTipContent;
        //1 重新领悟技能 2重塑档位
        public uint type = 1;
        protected override void OnLoaded()
        {
            this.content = this.transform.Find("Animator/Text_Tip").GetComponent<Text>();

            Button btn = this.transform.Find("Animator/Buttons/Button_Cancel").GetComponent<Button>();
            btn.onClick.AddListener(this.OnLeftBtnClicked);
            btn = this.transform.Find("Animator/Buttons/Button_Sure").GetComponent<Button>();
            btn.onClick.AddListener(this.OnRightBtnClicked);
            this.leftBtnText = this.transform.Find("Animator/Buttons/Button_Cancel/Text").GetComponent<Text>();
            this.rightBtnText = this.transform.Find("Animator/Buttons/Button_Sure/Text").GetComponent<Text>();

            this.transform.Find("Animator/View_Toggle").gameObject.SetActive(true);

            Toggle tg = this.transform.Find("Animator/View_Toggle/Toggle_Read").GetComponent<Toggle>();
            tg.onValueChanged.AddListener(this.OnValueChanged);
            this.fitstTipContent = this.transform.Find("Animator/View_Toggle/Label").GetComponent<Text>();

        }

        protected override void OnOpen(object arg)
        {
            if (null != arg)
            {
                type = Convert.ToUInt32(arg);
            }
        }

        protected override void OnOpened()
        {
            if (type == 1)
            {
                TextHelper.SetText(this.content, 12031u);
            }
            else if (type == 2)
            {
                TextHelper.SetText(this.content, 12046U);
            }
            else if (type == 3)
            {
                TextHelper.SetText(this.content, 12056u);
            }
            else if (type == 4)
            {
                TextHelper.SetText(this.content, 680000908u);
            }
            else if (type == 5)
            {
                TextHelper.SetText(this.content, 680000909u);
            }
            else if (type == 6) // 魔魂刷新
            {
                TextHelper.SetText(this.content, 680002020u);
            }
            TextHelper.SetText(this.fitstTipContent, 12032);
            TextHelper.SetText(this.leftBtnText, 11829);
            TextHelper.SetText(this.rightBtnText, 2104050);
        }

        private void OnLeftBtnClicked()
        {
            this.CloseSelf();
        }

        private void OnRightBtnClicked()
        {
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnPetRemakeRecastTipsEntry, type);
            this.CloseSelf();
        }
        private void OnValueChanged(bool flag)
        {
            if (type == 1)
            {
                Sys_Pet.Instance.isShowRemakeSkillTips = flag;
            }
            else if(type == 2)
            {
                Sys_Pet.Instance.isShowRemakeTips = flag;
            }
            else if(type == 3)
            {
                Sys_Pet.Instance.isShowRemakePerfectTips = flag;
            }
            else if (type == 4)
            {
                Sys_Pet.Instance.isSmeltFashionTips = flag;
            }
            else if (type == 5)
            {
                Sys_Pet.Instance.isSmeltSkillTips = flag;
            }
            else if (type == 6)
            {
                Sys_Pet.Instance.isDemonSpiritSkillRefresh = flag;
            }
            
        }
    }
}