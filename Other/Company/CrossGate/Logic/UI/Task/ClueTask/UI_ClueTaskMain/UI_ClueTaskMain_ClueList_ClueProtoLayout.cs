using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_ClueTaskMain_ClueList_ClueProtoLayout
    {
        public GameObject gameObject;
        public Transform transform;

        public Button btn;
        public RawImage icon;
        public RawImageLoader rawImageLoader;
        public GameObject pageNode;
        public CP_PageIndexer pageIndexer;
        public Text clueTaskName;
        public CP_StarLevel starLevel;

        public GameObject finishTimeNode;
        public Text finishTimeText;

        public GameObject tipGo;
        public Text tipText;

        public GameObject newClue;
        public GameObject tracing;
        public GameObject noClue;
        public GameObject first;

        public void Parse(GameObject root)
        {
            gameObject = root;
            transform = gameObject.transform;

            btn = transform.Find("Image_Frame").GetComponent<Button>();
            icon = transform.Find("Image_Frame/Image_Icon").GetComponent<RawImage>();
            rawImageLoader = icon.gameObject.GetComponent<RawImageLoader>();
            pageNode = transform.Find("Image_Frame02").gameObject;
            pageIndexer = pageNode.GetComponent<CP_PageIndexer>();
            clueTaskName = transform.Find("Image_Frame01/Text_Name").GetComponent<Text>();
            starLevel = transform.Find("Grid_Star").GetComponent<CP_StarLevel>();

            finishTimeNode = transform.Find("Image_Frame03").gameObject;
            finishTimeText = transform.Find("Image_Frame03/Text_Data").GetComponent<Text>();
            newClue = transform.Find("Image_MarkNew").gameObject;
            tracing = transform.Find("Image_MarkNow").gameObject;
            noClue = transform.Find("Image_MarkUnfind").gameObject;
            first = transform.Find("Image_MarkPerfect").gameObject;

            tipGo = transform.Find("Tips_bg").gameObject;
            tipText = transform.Find("Tips_bg/Text").GetComponent<Text>();
        }
    }
}
