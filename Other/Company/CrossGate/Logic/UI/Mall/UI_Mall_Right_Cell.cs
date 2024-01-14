using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic.Core;
using Packet;
using Table;

namespace Logic
{
    public class UI_Mall_Right_Cell : UIComponent
    {
        private CP_Toggle mToggle;
        private Text textName;
        private Text textSelectName;
        private Image imgIcon;
        private Image imgSelectIcon;
        private Transform transRed;

        private Action<uint> mSelectAction;

        public int gridIndex;
        public uint mShopId;
        private uint mShopNameId;

        protected override void Loaded()
        {
            mToggle = transform.GetComponent<CP_Toggle>();
            mToggle.onValueChanged.AddListener(OnToggleChange);
            textName = mToggle.transform.Find("Text").GetComponent<Text>();
            textSelectName = mToggle.transform.Find("Text_Select").GetComponent<Text>();

            imgIcon = transform.Find("Image").GetComponent<Image>();
            imgSelectIcon = transform.Find("Image_Select2").GetComponent<Image>();

            transRed = transform.Find("Image_Dot");
        }

        private void OnToggleChange(bool isOn)
        {
            if (isOn)
            {
                Sys_Mall.Instance.SelectShopId = mShopId;
                mSelectAction?.Invoke(mShopId);
            }
        }

        public void AddListener(Action<uint> func)
        {
            mSelectAction = func;
        }

        public void UpdateInfo(int index, uint shopId, uint shopName)
        {
            gridIndex = index;
            mShopId = shopId;
            mShopNameId = shopName;

            textName.text = textSelectName.text = LanguageHelper.GetTextContent(mShopNameId);

            CSVShop.Data shopData = CSVShop.Instance.GetConfData(mShopId);
            if (shopData != null)
            {
                ImageHelper.SetIcon(imgIcon, shopData.tab_icon);
                ImageHelper.SetIcon(imgSelectIcon, shopData.tab_icon);
            }

            OnSelect(shopId == Sys_Mall.Instance.SelectShopId);

            transRed.gameObject.SetActive(Sys_Mall.Instance.IsShopRed(shopId));
        }

        public void RefreshRedDot()
        {
            transRed.gameObject.SetActive(Sys_Mall.Instance.IsShopRed(mShopId));
        }

        public void OnSelect(bool isOn)
        {
            mToggle.SetSelected(isOn, true);
        }
    }
}


