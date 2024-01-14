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
    public class FashionCeilGrid_Suit
    {
        private Transform transform;
        private FashionSuit mFashionSuit;
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
        private GameObject m_ActionGo;
        private GameObject m_FreeDyeGo;
        private Button m_ClickButton;
        private Action<FashionCeilGrid_Suit> m_SelectAction;
        public int dataIndex;

        public void BindGameObject(GameObject go)
        {
            transform = go.transform;
            ParseComponent();
        }

        public void SetData(FashionSuit fashionSuit,int _dataIndex)
        {
            mFashionSuit = fashionSuit;
            dataIndex = _dataIndex;
            Refresh();
        }

        public void AddEventListener(Action<FashionCeilGrid_Suit> selectAction)
        {
            m_SelectAction = selectAction;
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
            m_ActionGo = transform.Find("Feature/zhuanshudongzuo").gameObject;
            m_FreeDyeGo = transform.Find("Feature/ziyouranse").gameObject;
            m_ClickButton = transform.Find("Btn_Card").GetComponent<Button>();
            m_ClickButton.onClick.AddListener(OnClicked);
        }

        private void OnClicked()
        {
            m_SelectAction?.Invoke(this);
        }

        public void Refresh()
        {
            uint iconId = mFashionSuit.Id * 10000 + Sys_Role.Instance.HeroId;
            CSVFashionIcon.Data cSVFashionIconData = CSVFashionIcon.Instance.GetConfData(iconId);
            if (cSVFashionIconData != null)
            {
                ImageHelper.SetIcon(icon, null, cSVFashionIconData.Icon_Path, true);
                icon.SetNativeSize();
                icon.transform.localScale = new Vector3(cSVFashionIconData.Icon_scale, cSVFashionIconData.Icon_scale, 1);
                (icon.transform as RectTransform).anchoredPosition = new Vector2(cSVFashionIconData.Icon_pos[0], cSVFashionIconData.Icon_pos[1]);
            }
            else
            {
                DebugUtil.LogErrorFormat("{0}号图标找不到", iconId);
            }
            TextHelper.SetText(name, mFashionSuit.cSVFashionSuitData.SuitName);
            UpdateAssoiated();
            UpdateLockState();
            UpdateDressState();
            day.SetActive(false);

            m_ActionGo.SetActive(false);
            m_FreeDyeGo.SetActive(false);
        }

        public void UpdateLockState()
        {
            //ImageHelper.SetImageGray(bg, !mFashionSuit.UnLock);
            lockobj.SetActive(!mFashionSuit.UnLock);
        }
        public void UpdateDressState()
        {
            use.SetActive(mFashionSuit.Dress);
        }

        public void UpdateAssoiated()
        {
            count.text = string.Format("{0}/{1}", mFashionSuit.unlockedAssoiated.Count, mFashionSuit.associated.Count);
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
