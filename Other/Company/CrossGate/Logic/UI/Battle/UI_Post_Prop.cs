using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Framework;

namespace Logic
{
    public class UI_Post_Prop_Layout
    {
        public Transform transform;
        public Button closeBtn;
        public Button postBtn;
        public Text number;
        public InputField inputField;

        public void Init(Transform transform)
        {
            this.transform = transform;
            closeBtn = transform.Find("Animator/Open/Btn_Open").GetComponent<Button>();
            postBtn = transform.Find("Animator/Btn_01").GetComponent<Button>();
            number = transform.Find("Animator/Text_Number").GetComponent<Text>();
            inputField = transform.Find("Animator/InputField").GetComponent<InputField>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OnClose_ButtonClicked);
            postBtn.onClick.AddListener(listener.OnPost_ButtonClicked);
        }

        public interface IListener
        {
            void OnClose_ButtonClicked();
            void OnPost_ButtonClicked();
        }
    }
    public class UI_Post_Prop : UIBase, UI_Post_Prop_Layout.IListener
    {
        private UI_Post_Prop_Layout layout = new UI_Post_Prop_Layout();

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }               

        public void OnClose_ButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_Post_Prop);
        }

        public void OnPost_ButtonClicked()
        {
            
        }
    }
}
