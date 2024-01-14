using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Logic.Core;
using Table;
using Lib.Core;
using Packet;

namespace Logic
{
    public class OrnamentBagBase
    {
        private Item0_Layout Layout = new Item0_Layout();
        private Image imgEmptyIcon;

        private ItemData curItem;
        public uint slotId = 0;

        public void BindGameObject(GameObject go)
        {
            Transform transform = go.transform;
            Layout.BindGameObject(transform.Find("Btn_Item").gameObject);
            imgEmptyIcon = transform.Find("Image_Equip").GetComponent<Image>();

            Layout.btnItem.onClick.AddListener(OnIconCliked);
        }

        public void SetData(ItemData _item)
        {
            curItem = _item;

            Layout.imgIcon.gameObject.SetActive(curItem != null);
            Layout.imgQuality.gameObject.SetActive(curItem != null);
            imgEmptyIcon.gameObject.SetActive(false);
            //imgEmptyIcon.gameObject.SetActive(curItem == null);

            if (curItem != null)
            {
                Layout.SetData(_item.cSVItemData, true);
                ImageHelper.GetQualityColor_Frame(Layout.imgQuality, (int)_item.Quality);
            }
        }

        private void OnIconCliked()
        {
            if (curItem != null)
            {
                OrnamentTipsData tipData = new OrnamentTipsData();
                tipData.equip = curItem;
                tipData.sourceUiId = EUIID.UI_Bag;

                UIManager.OpenUI(EUIID.UI_Tips_Ornament, false, tipData);
            }
            else
            {
                //TODO: tips
            }
        }
    }
    public class UI_OrnamentBag : UIComponent
    {
        private Dictionary<uint, string> dictParam = new Dictionary<uint, string>();
        private Dictionary<uint, OrnamentBagBase> dictOrnament = new Dictionary<uint, OrnamentBagBase>();

        protected override void Loaded()
        {
            dictParam.Clear();
            dictParam.Add((uint)OrnamentSlot.Necklace, "View_Equip/PropItemNecklace");
            dictParam.Add((uint)OrnamentSlot.Earring, "View_Equip/PropItemEarring");
            dictParam.Add((uint)OrnamentSlot.Ring, "View_Equip/PropItemRing");

            dictOrnament.Clear();
            foreach (var data in dictParam)
            {
                GameObject go = transform.Find(data.Value).gameObject;
                OrnamentBagBase equip = new OrnamentBagBase();
                equip.BindGameObject(go);
                equip.slotId = data.Key;
                dictOrnament.Add(data.Key, equip);
            }
        }

        public override void Show()
        {
            gameObject.SetActive(true);
            UpdateInfo();
        }

        public override void Hide()
        {
            gameObject.SetActive(false);
        }

        public void UpdateInfo()
        {
            foreach (var data in dictOrnament)
            {
                ItemData equip = Sys_Ornament.Instance.GetEquipedOrnament(data.Key);
                data.Value.SetData(equip);
            }
        }
    }
}