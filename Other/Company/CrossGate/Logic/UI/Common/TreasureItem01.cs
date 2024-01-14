using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;

namespace Logic
{
    public class TreasureItem01
    {
        public Transform transform;
        public Image icon;
        public Text textLevel;
        public Button btn;

        public void Bind(GameObject go)
        {
            transform = go.transform;

            icon = transform.Find("RawImage").GetComponent<Image>();
            textLevel = transform.Find("Image_Frames/Text_Amount").GetComponent<Text>();
            btn = transform.Find("Button").GetComponent<Button>();
        }

        public void SetData()
        {

        }
    }
}

