using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Table;
using Lib.Core;

namespace Logic
{
    public class FashionCeilGrid_Weapon
    {
        private Transform transform;
        private FashionWeapon mFashionWeapon;
        private GameObject _count;
        private GameObject use;
        private Text count;
        private Text name;
        private GameObject day;
        private Text _day;
        private Image bg;
        private Image icon;
        private GameObject select;
        private GameObject lockobj;
        private GameObject m_CustomGo;
        private GameObject m_ActionGo;
        private GameObject m_FreeDyeGo;
        private Button m_ClickButton;
        private Action<FashionCeilGrid_Weapon> m_SelectAction;
        public int dataIndex;

        public void BindGameObject(GameObject go)
        {
            transform = go.transform;
            ParseComponent();
        }

        public void SetData(FashionWeapon fashionWeapon, int _dataIndex)
        {
            mFashionWeapon = fashionWeapon;
            dataIndex = _dataIndex;
            Refresh();
        }

        public void AddEventListener(Action<FashionCeilGrid_Weapon> selectAction)
        {
            m_SelectAction = selectAction;
        }

        public void Refresh()
        {
            //if (index == 0)
            //{
            //    if (Sys_Equip.Instance.GetCurWeapon() != Constants.UMARMEDID)
            //    {
            //        uint equipId = Sys_Equip.Instance.GetCurWeapon();
            //        uint iconId = CSVEquipment.Instance.GetConfData(equipId).icon;
            //        CSVFashionIcon.Data cSVFashionIconData = CSVFashionIcon.Instance.GetConfData(iconId);
            //        if (cSVFashionIconData != null)
            //        {
            //            ImageHelper.SetIcon(icon, null, cSVFashionIconData.Icon_Path, true);
            //            icon.transform.localScale = new Vector3(cSVFashionIconData.Icon_scale, cSVFashionIconData.Icon_scale, 1);
            //            (icon.transform as RectTransform).anchoredPosition = new Vector2(cSVFashionIconData.Icon_pos[0], cSVFashionIconData.Icon_pos[1]);
            //        }
            //    }
            //}
            //else
            //{
            if (Sys_Equip.Instance.GetCurWeapon() != Constants.UMARMEDID)
            {
                uint equipId = Sys_Equip.Instance.GetCurWeapon();
                CSVEquipment.Data cSVEquipmentData = CSVEquipment.Instance.GetConfData(equipId);
                if (cSVEquipmentData != null)
                {
                    uint iconId = mFashionWeapon.cSVFashionWeaponData.id * 10 + cSVEquipmentData.equipment_type;
                    CSVFashionIcon.Data cSVFashionIconData = CSVFashionIcon.Instance.GetConfData(iconId);
                    if (cSVFashionIconData != null)
                    {
                        ImageHelper.SetIcon(icon, null, cSVFashionIconData.Icon_Path, true);
                        icon.transform.localScale = new Vector3(cSVFashionIconData.Icon_scale, cSVFashionIconData.Icon_scale, 1);
                        (icon.transform as RectTransform).anchoredPosition = new Vector2(cSVFashionIconData.Icon_pos[0], cSVFashionIconData.Icon_pos[1]);
                    }
                }

            }
            //}
            //uint iconId = mFashionWeapon.Id * 1000 + (uint)GameCenter.mainHero.careerComponent.CurCarrerType;
            //CSVFashionIcon.Data cSVFashionIconData = CSVFashionIcon.Instance.GetConfData(iconId);
            //if (cSVFashionIconData != null)
            //{
            //    ImageHelper.SetIcon(icon, null, cSVFashionIconData.Icon_Path, true);
            //    icon.transform.localScale = new Vector3(cSVFashionIconData.Icon_scale, cSVFashionIconData.Icon_scale, 1);
            //    (icon.transform as RectTransform).anchoredPosition = new Vector2(cSVFashionIconData.Icon_pos[0], cSVFashionIconData.Icon_pos[1]);
            //}
            TextHelper.SetText(name, mFashionWeapon.cSVFashionWeaponData.WeaponName);
            _count.SetActive(false);
            UpdateLockState();
            UpdateDressState();
            UpdateTime();

            m_CustomGo.SetActive(mFashionWeapon.cSVFashionWeaponData.Tag[0] == 1);
            m_FreeDyeGo.SetActive(mFashionWeapon.cSVFashionWeaponData.Tag[1] == 1);
            m_ActionGo.SetActive(mFashionWeapon.cSVFashionWeaponData.Tag[2] == 1);
        }

        private void ParseComponent()
        {
            icon = transform.Find("Image_Frame/Image_Icon").GetComponent<Image>();
            _count = transform.Find("Image_Count").gameObject;
            count = _count.transform.Find("Text").GetComponent<Text>();
            name = transform.Find("Image_Name/Text").GetComponent<Text>();
            use = transform.Find("Image_Used").gameObject;
            day = transform.Find("Image_Day").gameObject;
            _day = day.transform.Find("Text").GetComponent<Text>();
            bg = transform.Find("Btn_Card").GetComponent<Image>();
            select = transform.Find("Image_Select").gameObject;
            lockobj = transform.Find("Image_IconNone").gameObject;
            m_CustomGo = transform.Find("Feature/dingzhiranse").gameObject;
            m_ActionGo = transform.Find("Feature/zhuanshudongzuo").gameObject;
            m_FreeDyeGo = transform.Find("Feature/ziyouranse").gameObject;
            m_ClickButton = transform.Find("Btn_Card").GetComponent<Button>();
            m_ClickButton.onClick.AddListener(OnClicked);
        }

        private void OnClicked()
        {
            m_SelectAction?.Invoke(this);
        }

        public void UpdateTime()
        {
            if (mFashionWeapon.ownerType == OwnerType.None || mFashionWeapon.ownerType == OwnerType.Forever)
            {
                if (day.activeSelf)
                {
                    day.SetActive(false);
                }
            }
            else
            {
                if (!day.activeSelf)
                {
                    day.SetActive(true);
                }
                string days = string.Format(CSVLanguage.Instance.GetConfData(2009545).words, mFashionWeapon.fashionTimer.configRemainTime / (3600 * 24));
                TextHelper.SetText(_day, days);
            }
        }

        public void UpdateLockState()
        {
            //ImageHelper.SetImageGray(bg, !mFashionWeapon.UnLock);
            lockobj.SetActive(!mFashionWeapon.UnLock);
        }

        public void UpdateDressState()
        {
            use.SetActive(mFashionWeapon.Dress);
        }

        public void Select()
        {
            select.SetActive(true);
        }

        public void Release()
        {
            select.SetActive(false);
        }
    }

}
