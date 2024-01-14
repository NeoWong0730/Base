
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Lib.Core;

namespace Logic
{
    public class TipPetEquipCompare : UIParseCommon
    {
        public TipPetEquipBackgroundFirst backgroundFirst;
        public CP_Toggle toggleLock;
        public TipPetEquipInfoView infoView;
        public TipPetEquipInfoTip tipInfo;

        private GameObject _content;
        private ScrollRect _scrollRect;
        private ContentSizeFitter _sizeFitter;
        private RectTransform _rectFitter;
        
        private ItemData _itemData;
        private uint _petUid;
        protected override void Parse()
        {
            backgroundFirst = new TipPetEquipBackgroundFirst();
            backgroundFirst.Init(transform.Find("Background_Root/IconRoot"));
            var toggleLockTransform = transform.Find("Background_Root/IconRoot/Toggle_Lock");
            if (toggleLockTransform != null)
            {
                toggleLock = transform.Find("Background_Root/IconRoot/Toggle_Lock").GetComponent<CP_Toggle>();
                toggleLock.onValueChanged.AddListener(OnToggleLock);
            }
            infoView = new TipPetEquipInfoView();
            infoView.Init(transform.Find("Background_Root/Scroll_View"));
            _content = transform.Find("Background_Root/Scroll_View/GameObject").gameObject;
            _scrollRect = infoView.gameObject.GetComponent<ScrollRect>();
            _sizeFitter = infoView.gameObject.GetComponent<ContentSizeFitter>();
            _rectFitter = _sizeFitter.GetComponent<RectTransform>();

            tipInfo = new TipPetEquipInfoTip();
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
                    Sys_Bag.Instance.OnItemLockReq(_itemData.Uuid, !isOn, _petUid);
            }
        }

        protected void UpdateEquipInfo(ItemData itemEquip,uint petUid, bool isShowLock = true)
        {
            _itemData = itemEquip;
            _petUid = petUid;
            
            backgroundFirst.UpdateInfo(itemEquip, petUid);

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
                bool show = _itemData.petEquip.Color >= (int) EItemQuality.Orange;
                toggleLock.gameObject.SetActive(show && isShowLock);
                if (show && isShowLock)
                {
                    ItemData data = Sys_Bag.Instance.GetItemDataByUuid(_itemData.Uuid);
                    if (data != null)
                        toggleLock.SetSelected(!data.IsLocked, true);
                }
            }
        }

        public void OnUpdatePetEquipLockState(uint petUid)
        {
            if (null != _itemData && toggleLock != null)
            {
                bool show = _itemData.petEquip.Color >= (int) EItemQuality.Orange;
                toggleLock.gameObject.SetActive(show);
                if (show)
                {
                    ClientPet pet = Sys_Pet.Instance.GetPetByUId(petUid);
                    Packet.Item data = null;
                    List<Packet.Item> items = pet.GetPetEquipItems();
                    if (items != null)
                    {
                        for (int i = 0; i < items.Count; ++i)
                        {
                            if (items[i].Uuid == _itemData.Uuid)
                            {
                                data = items[i];
                                _itemData.IsLocked = data.Islocked;
                                break;
                            }
                        }
                        
                        if (data != null)
                            toggleLock.SetSelected(!data.Islocked, true);
                    }
                }
            }
        }
    }

}
