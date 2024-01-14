using Logic.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Element_Layout
    {
        public Transform transform;
        public Button Button_Close;
        public Button Button_Tips;

        public void Init(Transform transform)
        {
            this.transform = transform;
            Button_Close = transform.Find("Animator/View_Bg/Btn_Close").GetComponent<Button>();
            Button_Tips = transform.Find("Animator/View_Circle/Title_Tips04/Button").GetComponent<Button>();
        }
        public void RegisterEvents(IListener listener)
        {
            Button_Close.onClick.AddListener(listener.OnButton_CloseTargetClicked);
            Button_Tips.onClick.AddListener(listener.OnButton_TipsClicked);
        }

        public interface IListener
        {
            void OnButton_CloseTargetClicked();
            void OnButton_TipsClicked();
        }
    }
    public class UI_Element : UIBase, UI_Element_Layout.IListener
    {
        private UI_Element_Layout layout = new UI_Element_Layout();
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        public void OnButton_CloseTargetClicked()
        {
            UIManager.CloseUI(EUIID.UI_Element);
        }

        public void OnButton_TipsClicked()
        {
            UIManager.OpenUI(EUIID.UI_ElementalCrystal);
        }
    }
}
