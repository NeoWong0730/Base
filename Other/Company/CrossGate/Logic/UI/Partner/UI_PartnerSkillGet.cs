using Logic.Core;
using Lib.Core;
using System;
using Table;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Logic
{
    public class UI_PartnerSkillGet : UIBase
    {
        private SkillItem02 skillItem;
        private Text txtName;
        private Text txtDes;
        private Button btnLeft;
        private Button btnRight;
        private Text txtNum;

        private List<uint> listSkills = new List<uint>();
        private int skillIndex;

        protected override void OnLoaded()
        {
            skillItem = new SkillItem02();
            skillItem.Bind(transform.Find("Animator/View_Right/SkillItem").gameObject);

            txtName = transform.Find("Animator/View_Right/Image_Light01 (1)/Text_Name").GetComponent<Text>();
            txtDes = transform.Find("Animator/View_Right/Text_dis").GetComponent<Text>();
            btnLeft = transform.Find("Animator/View_Right/Arrow_Left/Button_Left").GetComponent<Button>();
            btnLeft.onClick.AddListener(OnClikcArrowLeft);
            btnRight = transform.Find("Animator/View_Right/Arrow_Right/Button_Right").GetComponent<Button>();
            btnRight.onClick.AddListener(OnClikcArrowRight);
            txtNum = transform.Find("Animator/View_Right/Text_Num").GetComponent<Text>();

            Image closeBtn = transform.Find("Black").GetComponent<Image>();
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(closeBtn);
            eventListener.AddEventListener(EventTriggerType.PointerClick, (BaseEventData eventData) =>
            {
                CloseSelf();
            });
        }

        protected override void OnOpen(object arg)
        {
            listSkills.Clear();
            if (arg != null)
                listSkills = (List<uint>)arg;
        }

        protected override void OnShow()
        {
            skillIndex = 0;
            UpdateInfo(listSkills[skillIndex]);
        }

        private void OnClikcArrowLeft()
        {
            if (skillIndex == 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10657));
                return;
            }
            skillIndex--;
            UpdateInfo(listSkills[skillIndex]);
        }

        private void OnClikcArrowRight()
        {
            if (skillIndex == listSkills.Count - 1)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10658));
                return;
            }
            skillIndex++;
            UpdateInfo(listSkills[skillIndex]);
        }

        private void UpdateInfo(uint skillId)
        {
            skillItem.SetData(skillId);

            //Debug.LogError("udpateinfo =" + skillId.ToString());
            CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
            if (skillInfo != null)
            {
                txtName.text = LanguageHelper.GetTextContent(skillInfo.name);
                txtDes.text = LanguageHelper.GetTextContent(skillInfo.desc);
            }
            else
            {
                Debug.LogErrorFormat("CSVPassiveSkillInfo 未找到 skillId={0}", skillId);
            }

            txtNum.text = string.Format("{0}/{1}", skillIndex + 1, listSkills.Count);

            bool multi = listSkills.Count > 1;
            btnLeft.gameObject.SetActive(multi);
            btnRight.gameObject.SetActive(multi);
        }

    }
}


