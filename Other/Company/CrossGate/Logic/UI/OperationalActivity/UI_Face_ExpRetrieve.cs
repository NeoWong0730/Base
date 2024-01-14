using Lib.Core;
using Logic;
using Logic.Core;
using Packet;
using System.Text;
using Table;
using UnityEngine;
using UnityEngine.UI;
using System;


namespace Logic
{
    public class UI_Face_ExpRetrieve : UIBase
    {
        private Button btn_Close;
        private Text txt_Content;
        private Button btn_Jump;
        CSVexpRetrieve.Data eData;
        CSVDailyActivity.Data aData;
        public void Init(Transform transform)
        {
            btn_Close = transform.Find("Animator/View_TipsBg01_Small/Btn_Close").GetComponent<Button>();
            txt_Content = transform.Find("Animator/Text_Tips").GetComponent<Text>();
            btn_Jump = transform.Find("Animator/Btn_Check").GetComponent<Button>();
            btn_Close.onClick.AddListener(OnCloseButtonClicked);
            btn_Jump.onClick.AddListener(OnJumpButtonClicked);

        }
        protected override void OnLoaded()
        {
            Init(transform);
        }
        protected override void OnShow()
        {
            eData = CSVexpRetrieve.Instance.GetConfData(Sys_OperationalActivity.Instance.temporaryId);
            aData = CSVDailyActivity.Instance.GetConfData(eData.Activity_id);
            FaceTipsShow();
        }
        private void FaceTipsShow()
        {
            string activityName = LanguageHelper.GetTextContent(aData.ActiveName);
            txt_Content.text = LanguageHelper.GetTextContent(5408, activityName, (Sys_OperationalActivity.Instance.temporaryValue/10000.0f).ToString());
        }
        private void OnJumpButtonClicked()
        {
            UIManager.OpenUI(EUIID.UI_OperationalActivity, false, (uint)EOperationalActivity.ExpRetrieve);
            UIManager.CloseUI(EUIID.UI_Face_ExpRetrieve);
        }

        private void OnCloseButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_Face_ExpRetrieve);
        }

    }

}


