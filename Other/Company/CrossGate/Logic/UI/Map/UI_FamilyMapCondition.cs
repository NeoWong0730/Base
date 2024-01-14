using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_FamilyMapCondition : UIBase
    {
        #region 界面组件
        private Text txtTitle;
        private Text txtInfo;
        private Button btnClose;
        private Button btnOK;
        private List<TeleInfo> showTeleInfos;
        #endregion

        #region 系统函数        
        protected override void OnLoaded()
        {
            OnParseComponent();
        }        
        protected override void OnOpen(object arg)
        {
            showTeleInfos = arg as List<TeleInfo>;
        }
        protected override void OnShow()
        {
            UpdateView();
        }        
        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnEnterMap, OnEnterMap, toRegister);
        }
        #endregion

        #region 初始化
        private void OnParseComponent()
        {
            txtTitle = transform.Find("Animator/Title").GetComponent<Text>();
            txtInfo = transform.Find("Animator/Group/Text").GetComponent<Text>();

            btnClose = transform.Find("Animator/View_TipsBg02_Small/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(Close);
            btnOK = transform.Find("Animator/Btn_01").GetComponent<Button>();
            btnOK.onClick.AddListener(Close);

        }
        #endregion

        #region 界面显示

        private void UpdateView()
        {
            txtTitle.text = GetTitleTextString();
            txtInfo.text = GetTextInfoString();
        }
        private string GetTitleTextString()
        {
            return string.Format(CSVLanguage.Instance.GetConfData(5705).words, Sys_Map.Instance.TeleErrFamilyName);
        }

        private string GetTextInfoString()
        {
            StringBuilder names = new StringBuilder();
            int len = showTeleInfos.Count;
            for (int i = 0; i < len; i++)
            {
                names.Append(showTeleInfos[i].roleName + " ");
            }
            return string.Format(CSVLanguage.Instance.GetConfData(5706).words, names);
        }
        #endregion

        private void Close()
        {
            UIManager.CloseUI(EUIID.UI_FamilyMapCondition_Tips);
        }

        private void OnEnterMap()
        {
            Close();
        }
    }
}
