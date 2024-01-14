using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;

namespace Logic
{
    public class EquipItem
    {
        public Transform transform;

        public Item0_Layout Layout;
        public Image img_Equip;
        public Image img_Select;
        public Image img_Equiped;

        public void Bind(GameObject go)
        {
            transform = go.transform;

            Layout = new Item0_Layout();
            Layout.BindGameObject(transform.Find("Btn_Item").gameObject);

            img_Equip = transform.Find("Image_Equip").GetComponent<Image>();
            img_Select = transform.Find("Image_Select").GetComponent<Image>();
            img_Equiped = transform.Find("Image_Equiped").GetComponent<Image>();
        }

        public void SetActive(bool active)
        {
            transform.gameObject.SetActive(active);
        }

        public void SetData(ItemData item)
        {
            var csv = CSVItem.Instance.GetConfData(item.Id);
            if (csv != null)
            {
                Layout.imgIcon.gameObject.SetActive(true);
                Layout.imgQuality.enabled = true;
                Layout.SetData(item.cSVItemData, true);
                ImageHelper.GetQualityColor_Frame(Layout.imgQuality, (int)item.Quality);
            }
            else
            {
                Debug.LogError("not found Item id =" + item.Id.ToString());
            }
        }
    }
}

