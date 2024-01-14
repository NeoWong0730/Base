using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using System.Linq;
using Lib.Core;
using Packet;
using System;

namespace Logic
{
    public class UI_Transfiguration_Result : UIBase
    {
        private Image icon;
        private Image typeIcon;
        private Text name;
        private Text Lv;
        private Text message;
        private GameObject tipGo;
        private Button closeBtn;

        private ShapeShiftSkillGrid shapeShiftSkillGrid;

        protected override void OnLoaded()
        {
            icon = transform.Find("Animator/View/Image_Skillbg/Image_Icon").GetComponent<Image>();
            typeIcon = transform.Find("Animator/View/Image_Skillbg/Image_Type").GetComponent<Image>();
            name = transform.Find("Animator/View/Image_Skillbg/Text_Name").GetComponent<Text>();
            message = transform.Find("Animator/View/Image_Skillbg/Text_Describe").GetComponent<Text>();
            Lv = transform.Find("Animator/View/Image_Skillbg/Level").GetComponent<Text>();
            tipGo = transform.Find("Animator/View/Image_Skillbg/Image_Tip").gameObject;
            closeBtn = transform.Find("Image_BG").GetComponent<Button>();
            closeBtn.onClick.AddListener(OnCloseBtn);
        }

        protected override void OnOpen(object arg)
        {
            if (arg != null)
            {
                shapeShiftSkillGrid = arg as ShapeShiftSkillGrid;
            }
        }

        protected override void OnShow()
        {
            CSVPassiveSkillInfo.Data info = CSVPassiveSkillInfo.Instance.GetConfData(shapeShiftSkillGrid.Skillid);
            CSVRaceSkillType.Data skillType = CSVRaceSkillType.Instance.GetConfData(shapeShiftSkillGrid.Skillid);
            if (info != null)
            {
                ImageHelper.SetIcon(icon, info.icon);
                ImageHelper.SetIcon(typeIcon, skillType.skill_type);
                TextHelper.SetText(name, info.name);
                TextHelper.SetText(message, info.desc);
                TextHelper.SetText(Lv, 12462, info.level.ToString());
                tipGo.SetActive(skillType.type == 1);
            }
        }

        protected override void OnHide()
        {
        }

        protected override void OnDestroy()
        {
            shapeShiftSkillGrid = null;
        }

        private void OnCloseBtn()
        {
            UIManager.CloseUI(EUIID.UI_Transfiguration_Result);
        }
    }
}
