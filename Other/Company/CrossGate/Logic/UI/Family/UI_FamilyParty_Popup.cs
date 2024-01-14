using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using System;
using Table;

namespace Logic
{
    /// <summary> 酒会弹窗标题背景颜色，对应全局表ID </summary>
    public enum EFamilyPartyPopupColor
    {
        PartyStart = 1021,         //酒会开始
        MonsterInvation = 1022,    //怪物入侵
        FoodBattle = 1023,         //美食大作战
        PartyEnd = 1024,           //酒会结束
    }
    public enum EFamilyPartyPopupType
    {
        PartyStart = 0,         //酒会开始
        MonsterInvation = 1,    //怪物入侵
        FoodBattle = 2,         //美食大作战
        PartyEnd = 3,           //酒会结束
        PartyUpgrade = 4,       //酒会价值提升
    }
    public class UI_FamilyParty_Popup : UIBase
    {
        private uint openType = 0;

        private Button btnClose;
        private GameObject goActiveTitleView;
        private Image imgActiveBg;
        private Text txtActiveTitle;
        private Text txtActiveDesc;
        private GameObject goPartyUpgradeView;
        private Text txtPartyValue;//酒会价值
        private Text txtPersonExp;//个人经验
        #region 系统函数

        protected override void OnOpen(object arg)
        {
            if (arg != null && arg.GetType() == typeof(uint))
            {
                openType = (uint)arg;
            }
        }
        protected override void OnLoaded()
        {
            Parse();
        }
        protected override void OnShow()
        {
            UpdateView();
        }
        protected override void OnDestroy()
        {
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
        }
        #endregion

        #region func
        private void Parse()
        {
            btnClose = transform.Find("Image").GetComponent<Button>();
            btnClose.onClick.AddListener(OnBtnCloseClick);

            goActiveTitleView = transform.Find("Animator/Image_BG").gameObject;
            imgActiveBg = goActiveTitleView.transform.GetComponent<Image>();
            txtActiveTitle = goActiveTitleView.transform.Find("Text_Title").GetComponent<Text>();
            txtActiveDesc = goActiveTitleView.transform.Find("Text").GetComponent<Text>();
            goPartyUpgradeView = transform.Find("Animator/Image_BG2").gameObject;
            txtPartyValue = goPartyUpgradeView.transform.Find("content/Value").GetComponent<Text>();
            txtPersonExp = goPartyUpgradeView.transform.Find("content/EXP").GetComponent<Text>();
        }
        private void UpdateView()
        {
            if(openType == (uint)EFamilyPartyPopupType.PartyUpgrade)
            {
                goActiveTitleView.SetActive(false);
                goPartyUpgradeView.SetActive(true);
                UpdatePartyUpgradeView();
            }
            else
            {
                goActiveTitleView.SetActive(true);
                goPartyUpgradeView.SetActive(false);
                UpdateActiveTitleView();
            }
        }
        private void UpdateActiveTitleView()
        {
            switch(openType)
            {
                case (uint)EFamilyPartyPopupType.PartyStart:
                    {
                        UpdateActiveTitleColor(EFamilyPartyPopupColor.PartyStart);
                        txtActiveTitle.text = LanguageHelper.GetTextContent(6225);//酒会开始
                        txtActiveDesc.text = LanguageHelper.GetTextContent(6234);
                    }
                    break;
                case (uint)EFamilyPartyPopupType.MonsterInvation:
                    {
                        UpdateActiveTitleColor(EFamilyPartyPopupColor.MonsterInvation);
                        txtActiveTitle.text = LanguageHelper.GetTextContent(6223);//大地鼠来袭
                        txtActiveDesc.text = LanguageHelper.GetTextContent(6236);
                    }
                    break;
                case (uint)EFamilyPartyPopupType.FoodBattle:
                    {
                        UpdateActiveTitleColor(EFamilyPartyPopupColor.FoodBattle);
                        txtActiveTitle.text = LanguageHelper.GetTextContent(6224);//美食大作战
                        txtActiveDesc.text = LanguageHelper.GetTextContent(6237);
                    }
                    break;
                case (uint)EFamilyPartyPopupType.PartyEnd:
                    {
                        UpdateActiveTitleColor(EFamilyPartyPopupColor.PartyEnd);
                        txtActiveTitle.text = LanguageHelper.GetTextContent(6226);//酒会结束
                        txtActiveDesc.text = LanguageHelper.GetTextContent(6235);
                    }
                    break;
            }
        }
        private void UpdatePartyUpgradeView()
        {
            var selectItem = CSVItem.Instance.GetConfData(Sys_Family.Instance.familyData.familyPartyInfo.lastSubmitFoodItemId);
            bool isSpecial = Sys_Family.Instance.CheckIsFashionFood(selectItem.id);
            uint curValue = selectItem.receptionValue;
            var rewardPaid = selectItem.rewardPaid;
            uint curExp = 0;
            for (int i = 0; i < rewardPaid.Count; i++)
            {
                if (rewardPaid[i][0] == 4)
                {
                    curExp = rewardPaid[i][1];
                }
            }
            if (isSpecial)
            {
                var extParam = float.Parse(CSVParam.Instance.GetConfData(1034).str_value);
                var extValue = curValue * (extParam - 100) / 100;
                txtPartyValue.text = LanguageHelper.GetTextContent(6228, (curValue + extValue).ToString(), extValue.ToString());//+{0}（额外+{1}）
                var extExp = curExp * (extParam - 100) / 100;
                txtPersonExp.text = LanguageHelper.GetTextContent(6228, (curExp + extExp).ToString(), extExp.ToString());
            }
            else
            {
                txtPartyValue.text = LanguageHelper.GetTextContent(6243, curValue.ToString(), "");
                txtPersonExp.text = LanguageHelper.GetTextContent(6243, curExp.ToString(), "");
            }
        }

        private void UpdateActiveTitleColor(EFamilyPartyPopupColor color)
        {
            CSVParam.Data param = CSVParam.Instance.GetConfData((uint)color);//绿底
            var colors = param.str_value.Split(',');
            var bgColor = colors[0].Split('|');
            var titleColor = colors[1].Split('|');
            imgActiveBg.color = new Color(float.Parse(bgColor[0]) / 255f, float.Parse(bgColor[1]) / 255f, float.Parse(bgColor[2]) / 255f);
            Shadow shadow = txtActiveTitle.GetComponent<Shadow>();
            shadow.effectColor = new Color(float.Parse(titleColor[0]) / 255f, float.Parse(titleColor[1]) / 255f, float.Parse(titleColor[2]) / 255f);
        }
        #endregion

        #region event
        private void OnBtnCloseClick()
        {
            this.CloseSelf();
        }
        #endregion
    }
}
