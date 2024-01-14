using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace Logic
{
    public class UI_ChooseItem : UIBase
    {
        private List<ulong> items = new List<ulong>();
        private InfinityGridLayoutGroup infinity;
        private Transform infinityParent;
        private Dictionary<GameObject, GridItem> ceils = new Dictionary<GameObject, GridItem>();

        protected override void OnOpen(object arg)
        {
            items = arg as List<ulong>;
        }

        protected override void OnLoaded()
        {
            infinityParent = transform.Find("Animator/Scroll_View/Grid");
            infinity = infinityParent.gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            infinity.minAmount = 6;
            infinity.updateChildrenCallback = UpdateChildrenCallback;
            for (int i = 0; i < infinityParent.childCount; i++)
            {
                GameObject go = infinityParent.GetChild(i).gameObject;
                GridItem ceil = new GridItem();
                ceil.BindGameObject(go);
                ceils.Add(go, ceil);
            }
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(transform.Find("Animator/ClickClose").gameObject);
            eventListener.AddEventListener(EventTriggerType.PointerClick, (_) => { CloseSelf(); });
        }


        protected override void OnShow()
        {
            infinity.SetAmount(items.Count);
        }

        private void UpdateChildrenCallback(int index, Transform trans)
        {
            GridItem gridItem = ceils[trans.gameObject];
            gridItem.SetData(items[index]);
            if (index == 0)
            {
                gridItem.Select();
            }
            else
            {
                gridItem.Release();
            }
        }

        public class GridItem
        {
            private Transform transform;
            private Image mEventBg;
            private Image mIcon;
            private Text mitemName;
            private GameObject mSelectObj;
            private UI_LongPressButton uI_LongPressButton;
            private ulong itemUuid;
            //private ItemData mItemData;
            private CSVItem.Data mItemData;
            private GameObject m_PropGo;

            public void SetData(ulong uuid)
            {
                this.itemUuid = uuid;
                if (Sys_LivingSkill.Instance.bHasItem)
                {
                    ItemData itemData = Sys_Bag.Instance.GetItemDataByUuid(uuid);
                    this.mItemData = itemData.cSVItemData;
                }
                else
                {
                    this.mItemData = CSVItem.Instance.GetConfData((uint)itemUuid);
                }
                RefreshIcon();
            }


            public void BindGameObject(GameObject go)
            {
                transform = go.transform;
                m_PropGo = go.transform.Find("PropItem").gameObject;
                mEventBg = transform.Find("EventBG").GetComponent<Image>();
                mIcon = transform.Find("PropItem/Btn_Item/Image_Icon").GetComponent<Image>();
                mitemName = transform.Find("Text_Name").GetComponent<Text>();
                mSelectObj = transform.Find("Image_Select").gameObject;
                //uI_LongPressButton = mEventBg.gameObject.AddComponent<UI_LongPressButton>();
                //uI_LongPressButton.onStartPress.AddListener(OnPressed);
                //uI_LongPressButton.interval = 0.5f;

                Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(mEventBg);
                eventListener.AddEventListener(EventTriggerType.PointerClick, OnPointClick);
            }

            private void RefreshIcon()
            {
                TextHelper.SetText(mitemName, mItemData.name_id);
                PropItem propItem = new PropItem();
                propItem.BindGameObject(m_PropGo);
                if (Sys_LivingSkill.Instance.bHasItem)
                {
                    PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData
                                 (_id: mItemData.id,
                                 _count: 0,
                                 _bUseQuailty: true,
                                 _bBind: false,
                                 _bNew: false,
                                 _bUnLock: false,
                                 _bSelected: false,
                                 _bShowCount: false,
                                 _bShowBagCount: true,
                                 _bUseClick: true,
                                 _onClick: null,
                                 _bshowBtnNo: false);
                    showItem.SetQuality(mItemData.quality);
                    propItem.SetData(new MessageBoxEvt(EUIID.UI_ChooseItem, showItem));
                }
                else
                {
                    PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData
                                (_id: mItemData.id,
                                _count: 1,
                                _bUseQuailty: true,
                                _bBind: false,
                                _bNew: false,
                                _bUnLock: false,
                                _bSelected: false,
                                _bShowCount: false,
                                _bShowBagCount: true,
                                _bUseClick: true,
                                _onClick: null,
                                _bshowBtnNo: true);
                    showItem.SetQuality(mItemData.quality);
                    propItem.SetData(new MessageBoxEvt(EUIID.UI_ChooseItem, showItem));
                }
            }
            
            private void OnPointClick(BaseEventData baseEventData)
            {
                UIManager.CloseUI(EUIID.UI_ChooseItem);
                if (Sys_LivingSkill.Instance.bHasItem)
                {
                    Sys_LivingSkill.Instance.uuid = itemUuid;
                    Sys_LivingSkill.Instance.eventEmitter.Trigger(Sys_LivingSkill.EEvents.OnSetHardenItem);
                }
            }

            private void OnPressed()
            {
                UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_ChooseItem, new PropIconLoader.ShowItemData(mItemData.id, 0, false, false, false, false, false)));
            }

            public void Select()
            {
                if (Sys_LivingSkill.Instance.uuid != 0)
                {
                    mSelectObj.SetActive(true);
                }
                else
                {
                    Release();
                }
            }

            public void Release()
            {
                mSelectObj.SetActive(false);
            }
        }
    }
}

