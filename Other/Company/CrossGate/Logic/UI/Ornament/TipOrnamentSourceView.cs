using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lib.Core;
using Table;
using Packet;
using Google.Protobuf.Collections;
using Framework;
using System;
using UnityEngine.EventSystems;
using Logic.Core;

namespace Logic
{
    public class TipOrnamentSourceView : UIComponent
    {
        private PropIconLoader.ShowItemData mItemData;
        private uint sourceUiID;

        private GameObject mSourceViewRoot;
        private Transform infinityParent;
        private InfinityGridLayoutGroup infinity;
        private Dictionary<GameObject, SourceItemCeil> ceil1s = new Dictionary<GameObject, SourceItemCeil>();
        private List<uint> filterSource = new List<uint>();

        protected override void Loaded()
        {
            infinityParent = transform.Find("Scroll_View/Grid");
            infinity = infinityParent.gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            infinity.minAmount = 5;
            infinity.updateChildrenCallback = UpdateChildrenCallback;
            for (int i = 0; i < infinityParent.childCount; i++)
            {
                GameObject go = infinityParent.GetChild(i).gameObject;
                SourceItemCeil sourceItemCeil = new SourceItemCeil();
                sourceItemCeil.BindGameObject(go);
                sourceItemCeil.AddClickListener(OnCeilSelected);
                ceil1s.Add(go, sourceItemCeil);
            }
        }
        public void UpdateInfo(ItemData item)
        {
            transform.gameObject.SetActive(true);
            mItemData = new PropIconLoader.ShowItemData(item.Id, item.Count, true, false, false, false, false, false, false, false);
            FilterSources();
            infinity.SetAmount(filterSource.Count);
        }

        public override void OnDestroy()
        {
        }

        private void UpdateChildrenCallback(int index, Transform trans)
        {
            SourceItemCeil sourceItemCeil = ceil1s[trans.gameObject];
            sourceItemCeil.SetData(filterSource[index]);
        }

        private void FilterSources()
        {
            filterSource.Clear();
            if (mItemData.CSV.Source == null)
                return;
            foreach (var item in mItemData.CSV.Source)
            {
                CSVItemSource.Data cSVItemSourceData = CSVItemSource.Instance.GetConfData(item[0]);
                if (cSVItemSourceData != null)
                {
                    if (Sys_FunctionOpen.Instance.IsOpen(cSVItemSourceData.Function_id) && Sys_Role.Instance.Role.Level >= cSVItemSourceData.Level[0]
                        && Sys_Role.Instance.Role.Level <= cSVItemSourceData.Level[1])
                    {
                        filterSource.Add(item[0]);
                    }
                }
            }
        }
        private void OnCeilSelected(SourceItemCeil sourceItemCeil)
        {
            UIManager.CloseUI(EUIID.UI_Message_Box);
            CSVItemSource.Data cSVItemSourceData = CSVItemSource.Instance.GetConfData(sourceItemCeil.itemSourceId);
            if (cSVItemSourceData != null)
            {
                if (cSVItemSourceData.Type == 1)            //商城
                {
                    //UIManager.CloseUI((EUIID)sourceUiID);
                    MallPrama mallPrama = new MallPrama();
                    mallPrama.itemId = GetGoToItemId(mItemData.id, sourceItemCeil.itemSourceId);
                    mallPrama.mallId = cSVItemSourceData.Parameter[0];
                    mallPrama.shopId = cSVItemSourceData.Parameter[1];
                    EUIID uiId = (EUIID)cSVItemSourceData.UI_id;
                    UIManager.OpenUI(uiId, false, mallPrama);
                }
                else if (cSVItemSourceData.Type == 2)         //日常界面
                {
                    //UIManager.CloseUI((EUIID)sourceUiID);
                    if (cSVItemSourceData.Activity_id == 0)
                    {
                        UIManager.OpenUI(EUIID.UI_DailyActivites);
                    }
                    else
                    {
                        UIDailyActivitesParmas uIDailyActivitesParmas = new UIDailyActivitesParmas();
                        uIDailyActivitesParmas.SkipToID = cSVItemSourceData.Activity_id;
                        UIManager.OpenUI(EUIID.UI_DailyActivites, false, uIDailyActivitesParmas);
                    }
                }
                else if (cSVItemSourceData.Type == 3)         //寻路
                {
                    UIManager.CloseUI(EUIID.UI_Tips_Ornament);
                    ActionCtrl.Instance.MoveToTargetNPCAndInteractive(cSVItemSourceData.NPC_id);
                }
                else if (cSVItemSourceData.Type == 4)         //晶石界面
                {
                    Sys_StoneSkill.Instance.eventEmitter.Trigger(Sys_StoneSkill.EEvents.EnergysparSpecilEvent, mItemData.id);
                }
                else if (cSVItemSourceData.Type == 5)           //道具合成 
                {
                    CSVItem.Data item = CSVItem.Instance.GetConfData(mItemData.id);
                    if (null != item)
                    {
                        UIManager.OpenUI(EUIID.UI_Compose, false, item.compose);
                    }
                }
                else if (cSVItemSourceData.Type == 6)
                {
                    uint itemId = GetGoToItemId(mItemData.id, sourceItemCeil.itemSourceId);
                    Sys_Trade.Instance.TradeFind(itemId);
                }
                else if (cSVItemSourceData.Type == 7)
                {
                    uint lifeskillId = cSVItemSourceData.id - cSVItemSourceData.Type * 1000;
                    LifeSkillOpenParm lifeSkillOpenParm = new LifeSkillOpenParm();
                    lifeSkillOpenParm.skillId = lifeskillId;
                    lifeSkillOpenParm.itemId = mItemData.id;
                    UIManager.OpenUI(EUIID.UI_LifeSkill_Message, false, lifeSkillOpenParm);
                }
                else if (cSVItemSourceData.Type == 8)
                {
                    UIManager.OpenUI(EUIID.UI_ClueTaskMain);
                }
                else if (cSVItemSourceData.Type == 9)
                {
                    UIManager.CloseUI(EUIID.UI_Tips_Ornament, true);
                    UIManager.OpenUI((EUIID)cSVItemSourceData.UI_id);
                }
                else if (cSVItemSourceData.Type == 10)
                {
                    MallPrama mallPrama = new MallPrama();
                    mallPrama.itemId = GetGoToItemId(mItemData.id, sourceItemCeil.itemSourceId);
                    mallPrama.mallId = cSVItemSourceData.Parameter[0];
                    mallPrama.shopId = cSVItemSourceData.Parameter[1];
                    UIManager.OpenUI(EUIID.UI_PointMall, false, mallPrama);
                }
                else if (cSVItemSourceData.Type == 11)
                {
                    UIManager.OpenUI(EUIID.UI_Adventure, false, new AdventurePrama { page = cSVItemSourceData.Parameter[0] });
                }
                else if (cSVItemSourceData.Type == 12)
                {
                    Sys_Equip.UIEquipPrama data = new Sys_Equip.UIEquipPrama();
                    data.opType = (Sys_Equip.EquipmentOperations)cSVItemSourceData.Parameter[0];
                    UIManager.OpenUI(EUIID.UI_Equipment, false, data);
                }
                else if (cSVItemSourceData.Type == 13)
                {
                    UIManager.CloseUI(EUIID.UI_Tips_Ornament);
                    Sys_Mall.Instance.skip2MallFromItemSource = new MallPrama();
                    Sys_Mall.Instance.skip2MallFromItemSource.mallId = cSVItemSourceData.Parameter[0];
                    Sys_Mall.Instance.skip2MallFromItemSource.shopId = cSVItemSourceData.Parameter[1];
                    Sys_Mall.Instance.skip2MallFromItemSource.itemId = mItemData.id;
                    ActionCtrl.Instance.MoveToTargetNPCAndInteractive(cSVItemSourceData.NPC_id);
                }
            }
        }
        private uint GetGoToItemId(uint itemId, uint sourceId)
        {
            var datas = CSVItem.Instance.GetConfData(itemId).Source;
            uint resId = itemId;
            for (int i = 0; i < datas.Count; i++)
            {
                if (datas[i][0] == sourceId)
                {
                    resId = datas[i][1] == 0 ? itemId : datas[i][1];
                    break;
                }
            }
            return resId;
        }
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
    }
}
