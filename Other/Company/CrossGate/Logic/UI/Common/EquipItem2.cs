using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;

namespace Logic
{
    public class EquipItem2
    {
        public Transform transform;

        public Button btn;
        public Image imgIcon;

        public void Bind(GameObject go)
        {
            transform = go.transform;

            btn = transform.Find("Btn_Item").GetComponent<Button>();
            imgIcon = transform.Find("Btn_Item/Image_Icon").GetComponent<Image>();
        }

        public void SetActive(bool active)
        {
            transform.gameObject.SetActive(active);
        }

        public void SetData(ItemData item)
        {
            var csv = CSVItem.Instance.GetConfData(item.Id);
            imgIcon.enabled = csv != null;
            if (csv != null)
            {
                //Layout.imgIcon.gameObject.SetActive(true);
                //Layout.imgQuality.enabled = true;
                //Layout.SetData(item.cSVItemData, true);
                ImageHelper.SetIcon(imgIcon, item.cSVItemData.icon_id);
                //ImageHelper.GetQualityColor_Frame(Layout.imgQuality, (int)Sys_Equip.Instance.CalEquipQuality(item));
            }
            else
            {
                Debug.LogError("not found Item id =" + item.Id.ToString());
            }
        }
    }
}

