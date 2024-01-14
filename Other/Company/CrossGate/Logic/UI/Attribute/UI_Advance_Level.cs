using UnityEngine;
using Logic.Core;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Advance_Level_Layout
    {
        public Transform transform;
        public Text curLimiteLv;
        public Text upLevel;
        public Text groupT;
        public Button btn_goAdvance;
        public Button btn_close;

        public void Init(Transform transform)
        {
            this.transform = transform;

            curLimiteLv = transform.Find("Animator/View01/Text_Tips01/Text").GetComponent<Text>();
            upLevel = transform.Find("Animator/View01/Text_Tips02/Text").GetComponent<Text>();
            groupT= transform.Find("Animator/View01/Text_Tips03/Text").GetComponent<Text>();
            btn_goAdvance = transform.Find("Animator/View01/Btn_01").GetComponent<Button>();

            btn_close = transform.Find("Animator/View_TipsBgNew04/Btn_Close").GetComponent<Button>();

        }

        public void RegisterEvents(IListener listener)
        {
            btn_goAdvance.onClick.AddListener(listener.OnClick_OpenAdvance);
            btn_close.onClick.AddListener(listener.OnClick_Close);

        }

        public interface IListener
        {
            void OnClick_Close();
            void OnClick_OpenAdvance();
        }
    }
    public class UI_Advance_Level : UIBase, UI_Advance_Level_Layout.IListener
    {
        private UI_Advance_Level_Layout layout = new UI_Advance_Level_Layout();
        #region 系统函数
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);        
        }

        protected override void OnShow()
        {
            InitData();
        }
        #endregion

        private void InitData()
        {
            layout.curLimiteLv.text = Sys_Advance.Instance.GetCurLimiteLevel().ToString();
            layout.upLevel.text = Sys_Attr.Instance.GetRealLv().ToString();
            layout.groupT.text = LanguageHelper.GetTextContent(2005060);
        }

        public void OnClick_OpenAdvance()
        {
            CloseSelf();
            Sys_Attr.Instance.eventEmitter.Trigger(Sys_Attr.EEvents.OnChangeTab, ERoleViewType.ViewAdvance);
        }

        public void OnClick_Close()
        {
            CloseSelf();
        }
    }
}