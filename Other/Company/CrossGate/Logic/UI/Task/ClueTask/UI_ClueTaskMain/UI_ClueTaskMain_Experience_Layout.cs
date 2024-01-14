using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_ClueTaskMain_Experience_Layout
    {
        public GameObject gameObject;
        public Transform transform;

        public GameObject protoGo;
        public GameObject expend;

        public GameObject leftGo;
        public GameObject rightGo;

        public Transform parent;
        public Button returnBtn;
        public Transform rewardNode;

        public void Parse(GameObject root)
        {
            gameObject = root;
            transform = gameObject.transform;

            protoGo = transform.Find("Scroll_View/Condition_List/Proto").gameObject;
            expend = transform.Find("Expend").gameObject;

            leftGo = transform.Find("Expend/View_Left").gameObject;
            rightGo = transform.Find("Expend/View_Right").gameObject;

            parent = transform.Find("Scroll_View/Condition_List");

            returnBtn = transform.Find("Expend/Image_BG01/Btn_Details")?.GetComponent<Button>();
            rewardNode = transform.Find("Expend/View_Right/Rewards");
        }
    }
}
