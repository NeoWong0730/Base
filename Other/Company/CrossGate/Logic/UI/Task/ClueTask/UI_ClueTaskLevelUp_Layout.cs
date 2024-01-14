using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_ClueTaskLevelUp_Layout
    {
        public GameObject gameObject;
        public Transform transform;

        public Button btnReturn;

        public Text title;

        public Text oldLevelDesc;
        public Text newLevelDesc;

        public Image oldIcon;
        public Image newIcon;

        public Transform node;

        public void Parse(GameObject root)
        {
            gameObject = root;
            transform = gameObject.transform;

            title = transform.Find("Animator/View_TipsBg02_Small/Text_Title").GetComponent<Text>();

            oldLevelDesc = transform.Find("Animator/View_Right/Text").GetComponent<Text>();
            newLevelDesc = transform.Find("Animator/View_Right/Text").GetComponent<Text>();

            oldIcon = transform.Find("Animator/View_Right/Image_Title").GetComponent<Image>();
            newIcon = transform.Find("Animator/View_Right/Image_Title").GetComponent<Image>();

            btnReturn = transform.Find("Animator/View_TipsBg02_Small/Btn_Close").GetComponent<Button>();
            node = transform.Find("Animator/View_Right/Grid_Item");
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