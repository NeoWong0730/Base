using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_ClueTaskMain_Experience_Proto_Layout
    {
        public GameObject gameObject;
        public Transform transform;

        public Text detectiveOrAdventureTitle;
        public Image icon;
        public Text title;

        public Slider slider;
        public CP_PageIndexer pageIndexer;
        public Button button;

        public void Parse(GameObject root)
        {
            gameObject = root;
            transform = gameObject.transform;

            detectiveOrAdventureTitle = transform.Find("Image_Tips/Text_Tips01")?.GetComponent<Text>();
            icon = transform.Find("Image_Title")?.GetComponent<Image>();
            title = transform.Find("Text")?.GetComponent<Text>();

            slider = transform.Find("Slider_Exp")?.GetComponent<Slider>();
            pageIndexer = transform.Find("Slider_Exp/Percent")?.GetComponent<CP_PageIndexer>();
            button = transform.Find("Image_BG01/Btn_Details")?.GetComponent<Button>();
        }
    }
}
