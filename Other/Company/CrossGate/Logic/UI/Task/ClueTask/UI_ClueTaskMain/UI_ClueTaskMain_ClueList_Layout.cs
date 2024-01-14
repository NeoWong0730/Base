using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_ClueTaskMain_ClueList_Layout
    {
        public GameObject gameObject;
        public Transform transform;

        public GameObject protoGo;
        public Transform parent;
        public CP_PopdownList popdownList;

        public void Parse(GameObject root)
        {
            gameObject = root;
            transform = gameObject.transform;

            protoGo = transform.Find("View_Middle/Scroll_Partner/Toggle_Clue01").gameObject;
            parent = transform.Find("View_Middle/Scroll_Partner/Content").transform;

            popdownList = gameObject.GetComponent<CP_PopdownList>();
        }
    }
}
