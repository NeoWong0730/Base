using Logic.Core;
using UnityEngine.UI;

namespace Logic
{
    public class UI_ActivityShortcut : UIBase
    {
        #region 系统函数
        protected override void OnLoaded()
        {
            OnParseComponent();
        }
        protected override void OnShow()
        {
            InitView();
        }
        protected override void OnClose()
        {
            dailyActivityData = null;
            Sys_ActivityShortcut.Instance.curDailyActivityData = null;
        }
        protected override void OnDestroy()
        {
            dailyActivityData = null;
            Sys_ActivityShortcut.Instance.curDailyActivityData = null;
        }
        #endregion
        #region 组件
        Button cancelBtn;
        Button confirmBtn;
        Text textTitle;
        Text cancelText;
        Text confirmText;
        #endregion
        #region 数据
        Sys_ActivityShortcut.DailyActivityData dailyActivityData;
        #endregion
        #region 查找组件、注册事件
        private void OnParseComponent()
        {
            textTitle = transform.Find("Animator/Text_Tip").GetComponent<Text>();
            confirmBtn = transform.Find("Animator/Buttons/Button_Sure").GetComponent<Button>();
            confirmText = transform.Find("Animator/Buttons/Button_Sure/Text").GetComponent<Text>();

            cancelBtn = transform.Find("Animator/Buttons/Button_Cancel").GetComponent<Button>();
            cancelText = transform.Find("Animator/Buttons/Button_Cancel/Text").GetComponent<Text>();

            cancelBtn.onClick.AddListener(() => { CloseSelf(); });
            confirmBtn.onClick.AddListener(ConfirmBtn);
        }
        #endregion
        #region 初始化
        private void InitView()
        {
            TextHelper.SetText(confirmText, 1000906);
            TextHelper.SetText(cancelText,  1000905);
            dailyActivityData = Sys_ActivityShortcut.Instance.curDailyActivityData;
            if (dailyActivityData != null)
            {
                textTitle.text = LanguageHelper.GetTextContent(12260, LanguageHelper.GetTextContent(dailyActivityData.activityData.ActiveName));
            }
        }
        #endregion
        private void ConfirmBtn()
        {
            if (dailyActivityData != null)
            {
                Sys_Daily.Instance.JoinDaily(dailyActivityData.tid);
                CloseSelf();
            }
        }
    }
}