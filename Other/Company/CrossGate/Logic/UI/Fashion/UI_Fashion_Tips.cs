using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic.Core;
using Framework;
using Table;
using Lib.Core;
using System;
using System.Text;
using static Logic.Sys_Equip;

namespace Logic
{
    public class UI_Fashion_Tips : UIBase
    {
        public class FashionTipsData
        {
            public uint itemId;
            public uint fahsionId;
            public uint iconId;
        }

        private Text mFashionName;
        private Image mFashionIcon;
        private Text mFashionDes;
        private Button mItemSourceButton;
        private GameObject mSourceViewRoot;
        private FashionTipsData fashionTipsData;
        private CSVItem.Data mItemData;
        
        private bool bSourceActive;
        private ItemSource m_ItemSource;
        private uint sourceUiID;
        
        protected override void OnOpen(object arg)
        {
            fashionTipsData = arg as FashionTipsData;
            mItemData = CSVItem.Instance.GetConfData(fashionTipsData.itemId);
            sourceUiID = (uint)EUIID.UI_Fashion;
        }

        protected override void OnLoaded()
        {
            mFashionName = transform.Find("Animator/View_Message/Text_Name").GetComponent<Text>();
            mFashionIcon = transform.Find("Animator/View_Message/ListItem/Btn_Item/Image_Icon").GetComponent<Image>();
            mFashionDes = transform.Find("Animator/View_Message/Text_Ccontent").GetComponent<Text>();
            mItemSourceButton = transform.Find("Animator/View_Message/Button").GetComponent<Button>();
            mSourceViewRoot = transform.Find("Animator/View_Right").gameObject;
          
            m_ItemSource = new ItemSource();
            m_ItemSource.BindGameObject(mSourceViewRoot);
            mItemSourceButton.onClick.AddListener(ShowSourceItems);

            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(transform.Find("ClickClose").gameObject);
            eventListener.AddEventListener(EventTriggerType.PointerClick, (_) => { UIManager.CloseUI(EUIID.UI_Fashion_Tips); });
        }

        protected override void OnShow()
        {
            UpdateUi();
        }
     
        private void ShowSourceItems()
        {
            m_ItemSource.Show();
        }

        private void UpdateUi()
        {
            mSourceViewRoot.SetActive(false);
            if (Sys_Fashion.Instance.parts[fashionTipsData.fahsionId] == EHeroModelParts.Main)
            {
                TextHelper.SetText(mFashionName, CSVFashionClothes.Instance.GetConfData(fashionTipsData.fahsionId).FashionName);
                TextHelper.SetText(mFashionDes, CSVFashionClothes.Instance.GetConfData(fashionTipsData.fahsionId).FashionDescribe);
            }
            else if (Sys_Fashion.Instance.parts[fashionTipsData.fahsionId] == EHeroModelParts.Weapon)
            {
                TextHelper.SetText(mFashionName, CSVFashionWeapon.Instance.GetConfData(fashionTipsData.fahsionId).WeaponName);
                TextHelper.SetText(mFashionDes, CSVFashionWeapon.Instance.GetConfData(fashionTipsData.fahsionId).WeaponDescribe);
            }
            else
            {
                TextHelper.SetText(mFashionName, CSVFashionAccessory.Instance.GetConfData(fashionTipsData.fahsionId).AccName);
                TextHelper.SetText(mFashionDes, CSVFashionAccessory.Instance.GetConfData(fashionTipsData.fahsionId).AccDescribe);
            }
            ImageHelper.SetIcon(mFashionIcon, fashionTipsData.iconId);

            bSourceActive = m_ItemSource.SetData(mItemData.id, sourceUiID, EUIID.UI_Fashion_Tips);
          
            mItemSourceButton.gameObject.SetActive(bSourceActive);
        }
    }
}


