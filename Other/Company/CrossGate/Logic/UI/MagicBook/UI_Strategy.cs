using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Lib.Core;
using Packet;
using System;
using DG.Tweening;

namespace Logic
{
    public class UI_Strategy_Item
    {
        private Transform transform;
        private Image icon;
        private Transform arrowIcon;
        private Text name;
        private Text describe;
        private Text message;
        private Button moreBtn;
        private GameObject redPoint;
        public GameObject messageGo;
        private Action<UI_Strategy_Item> onClick;

        public uint id;
        private CSVTeaching.Data cSVTeachingData;
        private bool isFaq;

        public void Init(GameObject gameObject)
        {
            transform = gameObject.transform;
            icon = transform.Find("List_Big/Image/Image_Icon").GetComponent<Image>();
            arrowIcon = transform.Find("List_Big/Image_Line/Image_Arrow").transform;
            name = transform.Find("List_Big/Text_Name").GetComponent<Text>();
            describe = transform.Find("List_Big/Text_Describe").GetComponent<Text>();
            message = transform.Find("SmallItem/List_Small/Text_Des").GetComponent<Text>();
            moreBtn = transform.Find("List_Big/Image_bg").GetComponent<Button>();
            redPoint = transform.Find("List_Big/Image_Dot").gameObject;
            messageGo = transform.Find("SmallItem").gameObject;
            moreBtn.onClick.AddListener(OnMoreBtnClicked);

            messageGo.SetActive(false);
        }

        public void SetData(uint _id,bool _isFaq)
        {
            id = _id;
            isFaq = _isFaq;
            CSVTeaching.Instance.TryGetValue(id, out cSVTeachingData);
            ImageHelper.SetIcon(icon, cSVTeachingData.icon);
            name.text = LanguageHelper.GetTextContent(cSVTeachingData.name);
            describe.text = LanguageHelper.GetTextContent(cSVTeachingData.desc);
            message.text = LanguageHelper.GetTextContent(cSVTeachingData.detail);
            arrowIcon.localRotation = Quaternion.Euler(0f, 0f, 0f);
            RefressData();
        }

        public void SetMessageShow(bool isShow)
        {
            messageGo.SetActive(isShow);
            arrowIcon.localRotation = isShow? Quaternion.Euler(0f, 0f, -90f): Quaternion.Euler(0f, 0f, 0f);
        }

        public void RefressData()
        {
            if (isFaq)
            {
                redPoint.SetActive(false);
                return;
            }
            bool hasProcess = false;
            for(int i = 0; i < Sys_MagicBook.Instance.teachList.Count; ++i)
            {
                if (Sys_MagicBook.Instance.teachList[i].Id == id&& Sys_MagicBook.Instance.teachList[i].Process == 1)
                {
                    hasProcess = true;
                    break;
                }
            }
            redPoint.SetActive(!hasProcess);
        }

        public void RemoveEvents()
        {
            if (null != moreBtn)
            {
                moreBtn.onClick.RemoveListener(OnMoreBtnClicked);
            }
        }

        public void AddClickListener(Action<UI_Strategy_Item> onclicked = null)
        {
            onClick = onclicked;
        }

        private void OnMoreBtnClicked()
        {
            onClick?.Invoke(this);
        }      
    }
}