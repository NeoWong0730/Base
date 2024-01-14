using System.Collections.Generic;
using UnityEngine.UI;
using Logic.Core;
using System;
using UnityEngine;
using Table;

namespace Logic
{


    public class UI_Upgrade_Result: UIBase
    {
        private uint currentId;
        private Image skillImage;
        private Text leftLevelText;
        private Text rightLevelText;
        private Button closeBtn;
        protected override void OnLoaded()
        {
            skillImage = transform.Find("Animator/View/Image_Skillbg/Image_Icon").GetComponent<Image>();
            leftLevelText = transform.Find("Animator/View/Text_Level1").GetComponent<Text>();
            rightLevelText = transform.Find("Animator/View/Text_Level2").GetComponent<Text>();
            closeBtn= transform.Find("Image_BG").GetComponent<Button>();
            closeBtn.onClick.AddListener(CloseBtnClick);
        }

        protected override void OnOpen(object arg)
        {
            if (null != arg)
                currentId = (uint)arg;
        }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            CSVStone.Data currentData = CSVStone.Instance.GetConfData(currentId);
            if(null != currentData)
            {
                ImageHelper.SetIcon(skillImage, currentData.icon);
            }

            StoneSkillData severData = Sys_StoneSkill.Instance.GetServerDataById(currentId);
            if(null != severData)
            {
                TextHelper.SetText(leftLevelText, LanguageHelper.GetTextContent(2021003, (severData.powerStoneUnit.Level - 1).ToString()));
                TextHelper.SetText(rightLevelText, LanguageHelper.GetTextContent(2021003, severData.powerStoneUnit.Level.ToString()));
            }
        }

        protected override void OnHide()
        {

        }

        private void CloseBtnClick()
        {
            Sys_StoneSkill.Instance.eventEmitter.Trigger(Sys_StoneSkill.EEvents.UpgradeResultClose);
            UIManager.CloseUI(EUIID.UI_Upgrade_Result);
        }


    }
}

