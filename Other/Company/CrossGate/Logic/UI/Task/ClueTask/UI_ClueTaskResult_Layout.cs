using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_ClueTaskResult_Layout
    {
        public GameObject gameObject;
        public Transform transform;

        public Text taskName;
        public Text taskDesc;
        public Transform node;

        public Button btnReturn;

        public void Parse(GameObject root)
        {
            gameObject = root;
            transform = gameObject.transform;

            taskName = transform.Find("Animator/Text_Name").GetComponent<Text>();
            taskDesc = transform.Find("Animator/Text_Desc").GetComponent<Text>();
            btnReturn = transform.Find("Animator/Black").GetComponent<Button>();
            node = transform.Find("Animator/Scroll01/Content");
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