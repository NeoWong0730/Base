
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Lib.Core;

namespace Logic
{
    public class TipOrnamentCompare : UIParseCommon
    {
        public TipOrnamentBackGroundFirst backgroundFirst;
        public CP_Toggle toggleLock;
        public TipOrnamentInfoView infoView;
        public TipOrnamentInfoTip tipInfo;

        private GameObject _content;
        private ScrollRect _scrollRect;
        private ContentSizeFitter _sizeFitter;
        private RectTransform _rectFitter;
        
        private ItemData _itemData;
        protected override void Parse()
        {
            backgroundFirst = new TipOrnamentBackGroundFirst();
            backgroundFirst.Init(transform.Find("Background_Root/IconRoot"));
            var toggleLockTransform = transform.Find("Background_Root/IconRoot/Toggle_Lock");
            if (toggleLockTransform != null)
            {
                toggleLock = transform.Find("Background_Root/IconRoot/Toggle_Lock").GetComponent<CP_Toggle>();
                toggleLock.onValueChanged.AddListener(OnToggleLock);
            }

            infoView = new TipOrnamentInfoView();
            infoView.Init(transform.Find("Background_Root/Scroll_View"));
            _content = transform.Find("Background_Root/Scroll_View/GameObject").gameObject;
            _scrollRect = infoView.gameObject.GetComponent<ScrollRect>();
            _sizeFitter = infoView.gameObject.GetComponent<ContentSizeFitter>();
            _rectFitter = _sizeFitter.GetComponent<RectTransform>();

            tipInfo = new TipOrnamentInfoTip();
            tipInfo.Init(transform.Find("Background_Root/View_Tip"));
        }

        public override void OnDestroy()
        {
            infoView.OnDestroy();
        }
        
        private void OnToggleLock(bool isOn)
        {
            if (null != _itemData)
            {
                if (_itemData.IsLocked == isOn)
                    Sys_Bag.Instance.OnItemLockReq(_itemData.Uuid, !isOn);
            }
        }

        public void UpdateEquipInfo(ItemData itemEquip, bool isShowLock = true)
        {
            _itemData = itemEquip;
            
            backgroundFirst.UpdateInfo(itemEquip);

            infoView.UpdateInfo(itemEquip);

            tipInfo.UpdateInfo(itemEquip);

            _scrollRect.enabled = false;
            _sizeFitter.enabled = true;
            Lib.Core.FrameworkTool.ForceRebuildLayout(infoView.gameObject);

            if (_rectFitter.rect.height >= 460f)
            {
                _scrollRect.enabled = true;
                _sizeFitter.enabled = false;
                _rectFitter.sizeDelta = new Vector2(_rectFitter.sizeDelta.x, 460f);
            }
            
            OnUpdateLockState(isShowLock);
        }
        
        public void OnUpdateLockState(bool isShowLock = true)
        {
            if (null != _itemData && toggleLock != null)
            {
                bool show = _itemData.Quality >= (int) EItemQuality.Orange;
                toggleLock.gameObject.SetActive(show && isShowLock);
                if (show && isShowLock)
                {
                    ItemData data = Sys_Bag.Instance.GetItemDataByUuid(_itemData.Uuid);
                    toggleLock.SetSelected(!data.IsLocked, true);
                }
            }
        }
    }


    public class TipOrnamentRightCompare : TipOrnamentCompare
    {
        public ItemData itemInfo;

        protected override void Parse()
        {
            base.Parse();

            backgroundFirst.BtnSwitch.gameObject.SetActive(false);
            backgroundFirst.BtnMsg.gameObject.SetActive(false);
            backgroundFirst.BtnMsg.onClick.AddListener(OnBtnMessage);
        }

        public void UpdateItemInfo(ItemData itemEquip, bool isShowBtn = true, bool isShowLock = true)
        {
            itemInfo = itemEquip;

            UpdateEquipInfo(itemEquip, isShowLock);
        }

        private void OnBtnMessage()
        {
            Sys_Ornament.Instance.eventEmitter.Trigger<ItemData>(Sys_Ornament.EEvents.OnShowMsg, itemInfo);
        }
    }

    public class TipOrnamentLeftCompare : TipOrnamentCompare
    {
        private ItemData itemInfo;

        protected override void Parse()
        {
            base.Parse();
            backgroundFirst.BtnMsg.gameObject.SetActive(false);
            backgroundFirst.BtnSwitch.onClick.AddListener(OnBtnSwitch);
        }

        public void UpdateInfo(ItemData itemEquip, bool isShowLock)
        {
            itemInfo = itemEquip;
            UpdateEquipInfo(itemInfo, isShowLock);
            //backgroundFirst.BtnSwitch.gameObject.SetActive(Sys_Equip.Instance.compareList.Count > 1);
            Sys_Ornament.Instance.eventEmitter.Trigger(Sys_Ornament.EEvents.OnCompareBasicAttrValue, itemInfo);

        }

        private void OnBtnSwitch()
        {

        }
    }
}

