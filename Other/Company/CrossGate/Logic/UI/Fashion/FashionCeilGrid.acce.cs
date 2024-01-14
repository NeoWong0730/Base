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
    public class FashionCeilGrid_Accessory
    {
        private Transform transform;
        private GameObject select;
        private GameObject use;
        private Image icon;
        private Image quality;
        private GameObject time;
        private Text _time;
        private GameObject lockobj;
        public FashionAccessory mFashionAccessory { get; private set; }

        private Action<FashionCeilGrid_Accessory> onClick;
        public int dataIndex;
        public bool dress=false;

        public void BindGameObject(GameObject go)
        {
            transform = go.transform;
            ParseGo();
        }

        private void ParseGo()
        {
            quality = transform.Find("Image_Quaility").GetComponent<Image>();
            select = transform.Find("Image_Select").gameObject;
            use = transform.Find("Image_Used").gameObject;
            icon = transform.Find("Image_Icon").GetComponent<Image>();
            time = transform.Find("Image_Day").gameObject;
            _time = time.transform.Find("Text").GetComponent<Text>();
            lockobj = transform.Find("Image_IconNone").gameObject;
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(transform.Find("Image_Quaility"));
            eventListener.AddEventListener(EventTriggerType.PointerClick, OnGridClicked);
        }

        public void SetData(FashionAccessory  _fashionAccessory,int _dataIndex)
        {
            mFashionAccessory = _fashionAccessory;
            dataIndex = _dataIndex;
            Refresh();
        }
        
        public void Refresh()
        {
            ImageHelper.SetIcon(icon, mFashionAccessory.cSVFashionAccessoryData.AccIcon);
            time.gameObject.SetActive(mFashionAccessory.UnLock);
            //time.gameObject.SetActive(true);
            UpdateLockState();
            UpdateDressState();
            UpdateTime();
        }

        public void AddClickListener(Action<FashionCeilGrid_Accessory> _onClick)
        {
            onClick = _onClick;
        }

        public void Release()
        {
            select.SetActive(false);
            dress = false;
            //time.text = dress.ToString();
        }

        public void Select()
        {
            select.SetActive(true);
            //dress = true;
            //time.text = dress.ToString();
        }

        private void OnGridClicked(BaseEventData baseEventData)
        {
            onClick.Invoke(this);
            dress = !dress;
            if (!dress)
            {
                Release();
                Sys_Fashion.Instance.eventEmitter.Trigger<uint, EHeroModelParts>(Sys_Fashion.EEvents.OnUnLoadModelParts, mFashionAccessory.Id, Sys_Fashion.Instance.parts[mFashionAccessory.Id]);
            }
            //time.text = dress.ToString();
        }
        public void UpdateLockState()
        {
            //ImageHelper.SetImageGray(quality, !mFashionAccessory.UnLock);
            lockobj.SetActive(!mFashionAccessory.UnLock);
        }

        public void UpdateDressState()
        {
            use.SetActive(mFashionAccessory.Dress);
        }

        public void UpdateTime()
        {
            if (mFashionAccessory == null)
                return;
            if (mFashionAccessory.ownerType == OwnerType.None || mFashionAccessory.ownerType == OwnerType.Forever)
            {
                if (time.gameObject.activeSelf)
                {
                    time.gameObject.SetActive(false);
                }
            }
            else
            {
                if (!time.gameObject.activeSelf)
                {
                    time.gameObject.SetActive(true);
                }
                string days = string.Format(CSVLanguage.Instance.GetConfData(2009545).words, mFashionAccessory.fashionTimer.configRemainTime/(3600*24));
                TextHelper.SetText(_time, days);
            }
        }
    }
}
