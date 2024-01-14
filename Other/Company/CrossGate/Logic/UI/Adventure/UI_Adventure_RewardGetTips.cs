using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;

namespace Logic
{
    public class UI_Adventure_RewardGetTips : UIBase
    {
        private uint rewardId;
        #region 界面组件
        private Image imgIcon;
        private Text txtName;
        private Text txtDesc;
        private Button btnInfo;
        #endregion

        #region 系统函数
        protected override void OnOpen(object arg)
        {            
            rewardId = (uint)arg;
        }
        protected override void OnLoaded()
        {         
            Parse();
        }
        protected override void OnShow()
        {            
            UpdateView();
        }
        #endregion

        #region function
        private void Parse()
        {
            imgIcon = transform.Find("Animator/BG/bg_Icon/Icon").GetComponent<Image>();
            txtName = transform.Find("Animator/BG/Text_Name").GetComponent<Text>();
            txtDesc = transform.Find("Animator/BG/Text_Describe").GetComponent<Text>();
            btnInfo = transform.Find("Animator/BG/Btn_01").GetComponent<Button>();
            btnInfo.onClick.AddListener(OnBtnInfoClick);
        }
        private void UpdateView()
        {
            CSVAdventureCriminal.Data data = CSVAdventureCriminal.Instance.GetConfData(rewardId);
            if(data != null)
            {
                ImageHelper.SetIcon(imgIcon, data.image);
                txtName.text = LanguageHelper.GetTextContent(data.name);
                txtDesc.text = LanguageHelper.GetTextContent(data.simpleDes);
            }
        }
        #endregion

        #region 响应事件
        private void OnBtnInfoClick()
        {
            UIManager.OpenUI(EUIID.UI_Adventure_RewardInfo, false, rewardId);
            OnbtnCloseClick();
        }
        private void OnbtnCloseClick()
        {
            UIManager.CloseUI(EUIID.UI_Adventure_RewardGetTips);
        }
        #endregion
    }
}
