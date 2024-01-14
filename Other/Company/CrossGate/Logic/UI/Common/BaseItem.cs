using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;

namespace Logic
{
    public class Item0_Layout
    {
        public Transform transform;
        public Button btnItem;
        public Image imgQuality;
        public Image imgIcon;

        public void BindGameObject(GameObject go)
        {
            transform = go.transform;
            btnItem = transform.GetComponent<Button>();
            imgQuality = transform.Find("Image_Quality").GetComponent<Image>();
            imgIcon = transform.Find("Image_Icon").GetComponent<Image>();
        }

        public void SetActive(bool active)
        {
            transform.gameObject.SetActive(active);
        }

        public void SetData(CSVItem.Data itemData, bool useQuality)
        {
            imgIcon.gameObject.SetActive(true);
            imgQuality.gameObject.SetActive(true);

            ImageHelper.SetIcon(imgIcon, itemData.icon_id);
            ImageHelper.GetQualityColor_Frame(imgQuality, (int)itemData.quality);
        }
    }
}


