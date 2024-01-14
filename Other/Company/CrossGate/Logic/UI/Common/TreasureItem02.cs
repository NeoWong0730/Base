using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;

namespace Logic
{
    public class TreasureItem02
    {
        public Transform transform;
        public Image icon;
        public Image imgLable;
        public Image imgTip;
        public Text textTip;
        public Text textLevel;
        public Button btn;
        public Image mask;

        public void Bind(GameObject go)
        {
            transform = go.transform;

            icon = transform.Find("Image").GetComponent<Image>();
            imgLable = transform.Find("Image_Lable").GetComponent<Image>();
            imgTip = transform.Find("Image_ListBG").GetComponent<Image>();
            textTip = transform.Find("Image_ListBG/Text").GetComponent<Text>();
            textLevel = transform.Find("Image_Frames/Text_Amount").GetComponent<Text>();
            mask = transform.Find("Image_Mask").GetComponent<Image>();
            btn = transform.Find("Button_ListAdd").GetComponent<Button>();
            btn.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {

        }

        public void SetData(uint treasureId)
        {
            
        }
    }
}

