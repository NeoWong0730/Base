using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;

namespace Logic
{
    public class PartnerItem01
    {
        public Transform transform;

        public Button btnNormal;
        public Image imgSelect;
        public Image imgIcon;

        public Button btnBlank;
        public Image imgBlank;
        public Image imgArrowUp;
        public Image imgArrowDown;
        public Image imgLeftRight;
        public Image imgAdd;

        public void Bind(GameObject go)
        {
            transform = go.transform;

            btnNormal = transform.Find("Button_Normal").GetComponent<Button>();
            imgSelect = transform.Find("Image_Select").GetComponent<Image>();
            imgIcon = transform.Find("Image_Icon").GetComponent<Image>();

            btnBlank = transform.Find("Image_Blank").GetComponent<Button>();
            imgBlank = transform.Find("Image_Blank").GetComponent<Image>();
            imgArrowUp = transform.Find("Image_Blank/Image_ArrowUp").GetComponent<Image>();
            imgArrowDown = transform.Find("Image_Blank/Image_ArrowDown").GetComponent<Image>();
            imgLeftRight = transform.Find("Image_Blank/Image_Arrow").GetComponent<Image>();
            imgAdd = transform.Find("Image_Add").GetComponent<Image>();
        }

        public void SetActive(bool active)
        {
            transform.gameObject.SetActive(active);
        }

        public void HideBlank()
        {
            imgSelect.gameObject.SetActive(false);
            imgBlank.gameObject.SetActive(false);
            imgArrowUp.gameObject.SetActive(false);
            imgArrowDown.gameObject.SetActive(false);
            imgLeftRight.gameObject.SetActive(false);
        }
    }
}

