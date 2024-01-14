using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_ClueTaskWallTips_Layout
    {
        public GameObject gameObject;
        public Transform transform;

        public Text name;
        public Text taskDesc;
        public Image icon;
        public Text clueDesc;

        public Button btnReturn;

        public void Parse(GameObject root)
        {
            gameObject = root;
            transform = gameObject.transform;

            btnReturn = transform.Find("Animator/Black").GetComponent<Button>();

            name = transform.Find("Animator/Tips/Text_Name").GetComponent<Text>();
            taskDesc = transform.Find("Animator/Tips/Text_Describe").GetComponent<Text>();

            icon = transform.Find("Animator/Tips/PropNode/Image").GetComponent<Image>();
            clueDesc = transform.Find("Animator/Tips/PropNode/Text").GetComponent<Text>();
        }
        public void RegisterEvents(IListener listener)
        {
            btnReturn.onClick.AddListener(listener.OnBtnReturnClicked);
        }

        public interface IListener
        {
            void OnBtnReturnClicked();
        }
    }
}