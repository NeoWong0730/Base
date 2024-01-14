using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Logic.Core;
using Lib.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_Trade_MessageBox_Path
    {
        public class SourceItemCeil
        {
            private Transform transform;
            private Image icon;
            private Text name;
            public uint itemSourceId;
            private Action<SourceItemCeil> onClick;
            private Image eventBg;

            public void BindGameObject(GameObject go)
            {
                transform = go.transform;
                ParseComponent();
            }

            private void ParseComponent()
            {
                icon = transform.Find("PropItem/Btn_Item/Image_Icon").GetComponent<Image>();
                name = transform.Find("Text_Name").GetComponent<Text>();
                eventBg = transform.Find("EventBG").GetComponent<Image>();
                Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(eventBg);
                eventListener.AddEventListener(EventTriggerType.PointerClick, OnGridClicked);
            }

            public void AddClickListener(Action<SourceItemCeil> _onClick)
            {
                onClick = _onClick;
            }

            private void OnGridClicked(BaseEventData baseEventData)
            {
                onClick.Invoke(this);
            }


            public void SetData(uint _id)
            {
                this.itemSourceId = _id;
                Refresh();
            }

            private void Refresh()
            {
                icon.enabled = true;
                ImageHelper.SetIcon(icon, CSVItemSource.Instance.GetConfData(itemSourceId).Icon_id);
                TextHelper.SetText(name, CSVItemSource.Instance.GetConfData(itemSourceId).Name_id);
            }
        }

        private Transform transform;

        private InfinityGridLayoutGroup infinity;
        private Dictionary<GameObject, SourceItemCeil> ceil1s = new Dictionary<GameObject, SourceItemCeil>();
        private List<uint> filterSource = new List<uint>();

        private CSVItem.Data _itemData; 

        public void Init(Transform trans)
        {
            transform = trans;

            Transform parent = transform.Find("Scroll_View/Grid"); ;
            infinity = parent.gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            infinity.minAmount = 5;
            infinity.updateChildrenCallback = UpdateChildrenCallback1;
            for (int i = 0; i < parent.childCount; i++)
            {
                GameObject go = parent.GetChild(i).gameObject;
                SourceItemCeil sourceItemCeil = new SourceItemCeil();
                sourceItemCeil.BindGameObject(go);
                sourceItemCeil.AddClickListener(OnCeilSelected);
                ceil1s.Add(go, sourceItemCeil);
            }
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);
            UpdateInfo();
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }

        public void SetItemInfoId(uint infoId)
        {
            _itemData = CSVItem.Instance.GetConfData(infoId);
        }

        private void UpdateInfo()
        {
            filterSources();
            infinity.SetAmount(filterSource.Count);
        }

        private void filterSources()
        {
            filterSource.Clear();
            if (_itemData == null || _itemData.Source == null)
                return;

            for (int i = 0; i < _itemData.Source.Count; ++i)
            {
                CSVItemSource.Data cSVItemSourceData = CSVItemSource.Instance.GetConfData(_itemData.Source[i][0]);
                if (cSVItemSourceData != null)
                {
                    if (Sys_FunctionOpen.Instance.IsOpen(cSVItemSourceData.Function_id) && Sys_Role.Instance.Role.Level >= cSVItemSourceData.Level[0]
                        && Sys_Role.Instance.Role.Level <= cSVItemSourceData.Level[1])
                    {
                        filterSource.Add(_itemData.Source[i][0]);
                    }
                }
            }
        }

        private void UpdateChildrenCallback1(int index, Transform trans)
        {
            SourceItemCeil sourceItemCeil = ceil1s[trans.gameObject];
            sourceItemCeil.SetData(filterSource[index]);
        }

        private void OnCeilSelected(SourceItemCeil sourceItemCeil)
        {
            UIManager.CloseUI(EUIID.UI_Message_Box);
            CSVItemSource.Data cSVItemSourceData = CSVItemSource.Instance.GetConfData(sourceItemCeil.itemSourceId);
            if (cSVItemSourceData != null)
            {
                if (cSVItemSourceData.Type == 1)            //商城
                {
                    MallPrama mallPrama = new MallPrama();
                    mallPrama.itemId = GetGoToItemId(_itemData.id, sourceItemCeil.itemSourceId);
                    mallPrama.mallId = cSVItemSourceData.Parameter[0];
                    mallPrama.shopId = cSVItemSourceData.Parameter[1];
                    EUIID uiId = (EUIID)cSVItemSourceData.UI_id;
                    UIManager.OpenUI(uiId, false, mallPrama);
                }
                else if (cSVItemSourceData.Type == 2)         //日常界面
                {
                    UIDailyActivitesParmas uIDailyActivitesParmas = new UIDailyActivitesParmas();
                    uIDailyActivitesParmas.SkipToID = cSVItemSourceData.Activity_id;
                    UIManager.OpenUI(EUIID.UI_DailyActivites, false, uIDailyActivitesParmas);
                }
                else if (cSVItemSourceData.Type == 3)         //寻路
                {
                    ActionCtrl.Instance.MoveToTargetNPCAndInteractive(cSVItemSourceData.NPC_id);
                }
                else if (cSVItemSourceData.Type == 4)         //晶石界面
                {
                    Sys_StoneSkill.Instance.eventEmitter.Trigger(Sys_StoneSkill.EEvents.EnergysparSpecilEvent, _itemData.id);
                }
                else if (cSVItemSourceData.Type == 5)           //道具合成 
                {
                    //CSVItem.Data item = CSVItem.Instance.GetConfData(_itemData.id);
                    //if (null != item)
                    //{
                    //    if (sourceUiID == cSVItemSourceData.UI_id)//在道具合成本界面
                    //    {
                    //        Sys_Bag.Instance.eventEmitter.Trigger(Sys_Bag.EEvents.ComposeSpecilEvent, item.compose);
                    //    }
                    //    //else                                      //打开道具合成界面
                    //    {
                    //        UIManager.OpenUI(EUIID.UI_Compose, false, item.compose);
                    //    }
                    //}
                }

                UIManager.CloseUI(EUIID.UI_Trade);
                //UIManager.CloseUI(EUIID.UI_Trade_MessageBox);
            }
        }

        private uint GetGoToItemId(uint itemId, uint sourceId)
        {
            var datas = CSVItem.Instance.GetConfData(itemId).Source;
            uint resId = itemId;
            if (datas != null)
            {
                for (int i = 0; i < datas.Count; ++i)
                {
                    if (datas[i][0] == sourceId)
                    {
                        resId = datas[i][1] == 0 ? itemId : datas[i][1];
                        break;
                    }
                }
            }

            return resId;
        }
    }
}


