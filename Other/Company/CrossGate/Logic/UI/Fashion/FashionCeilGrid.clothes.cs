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

    public class FashionCeilGrid_Clothes
    {
        private Transform transform;
        private FashionClothes mFashionClothes;
        private Image bg;
        private GameObject _count;
        private GameObject use;
        private Text count;
        private Text name;
        private GameObject day;
        private Text _day;
        private Image icon;
        private GameObject select;
        private GameObject lockobj;
        private GameObject m_ActionGo;  //专属动作
        private GameObject m_FreeDyeGo; //自由染色
        private GameObject m_CustomGo;  //定制染色
        private Button m_ClickButton;
        private Action<FashionCeilGrid_Clothes> m_SelectAction;
        public int dataIndex;

        public void BindGameObject(GameObject go)
        {
            transform = go.transform;
            ParseComponent();
        }

        public void SetData(FashionClothes fashionClothes, int _dataIndex)
        {
            mFashionClothes = fashionClothes;
            dataIndex = _dataIndex;
            Refresh();
        }

        public void AddEventListener(Action<FashionCeilGrid_Clothes> selectAction)
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
            m_CustomGo = transform.Find("Feature/dingzhiranse").gameObject;
            m_ActionGo = transform.Find("Feature/zhuanshudongzuo").gameObject;
            m_FreeDyeGo = transform.Find("Feature/ziyouranse").gameObject;
            m_ClickButton = transform.Find("Btn_Card").GetComponent<Button>();
            m_ClickButton.onClick.AddListener(OnClicked);
        }

        public void Refresh()
        {
            uint iconId = mFashionClothes.Id * 10000 + Sys_Role.Instance.HeroId;
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
                DebugUtil.LogErrorFormat("{0}号图标找不到", cSVFashionIconData.id);
            }
            TextHelper.SetText(name, mFashionClothes.cSVFashionClothesData.FashionName);
            _count.SetActive(false);
            UpdateLockState();
            UpdateDressState();
            UpdateTime();

            m_CustomGo.SetActive(mFashionClothes.cSVFashionClothesData.Tag[0] == 1);
            m_FreeDyeGo.SetActive(mFashionClothes.cSVFashionClothesData.Tag[1] == 1);
            m_ActionGo.SetActive(mFashionClothes.cSVFashionClothesData.Tag[2] == 1);
        }

        private void OnClicked()
        {
            m_SelectAction?.Invoke(this);
        }

        public void UpdateTime()
        {
            if (mFashionClothes.ownerType == OwnerType.None || mFashionClothes.ownerType == OwnerType.Forever)
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
                string days = string.Format(CSVLanguage.Instance.GetConfData(2009545).words, mFashionClothes.fashionTimer.configRemainTime / (3600 * 24));
                DebugUtil.LogFormat(ELogType.eFashion, $"configRemainTime :{mFashionClothes.fashionTimer.configRemainTime}");
                DebugUtil.LogFormat(ELogType.eFashion, $"days :{mFashionClothes.fashionTimer.configRemainTime / (3600 * 24)}");
                TextHelper.SetText(_day, days);
            }
        }

        public void UpdateLockState()
        {
            //ImageHelper.SetImageGray(bg, !mFashionClothes.UnLock);
            lockobj.SetActive(!mFashionClothes.UnLock);
        }

        public void UpdateDressState()
        {
            use.SetActive(mFashionClothes.Dress);
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
