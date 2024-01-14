using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Lib.Core;
using Packet;

namespace Logic
{
    public class UI_HeadFrame_View : UIComponent
    {
        private GameObject itemGo;
        public Text collectNum;

        private List<uint> headFrames = new List<uint>();
        private Dictionary<GameObject, UI_Head_Item> itemCeils = new Dictionary<GameObject, UI_Head_Item>();
        private List<UI_Head_Item> items = new List<UI_Head_Item>();
        private InfinityGrid infinityGrid;
        private int infinityCount;
        private ClientHeadData clientHeadData;

        protected override void Loaded()
        {
            itemGo = transform.Find("Viewport/Content/Item").gameObject;
            headFrames = Sys_Head.Instance.GetDatasId(EHeadViewType.HeadFrameView);

            infinityGrid = transform.GetComponent<InfinityGrid>();
            infinityGrid.onCreateCell += OnCreateCell;
            infinityGrid.onCellChange += OnCellChange;
        }

        public override void Show()
        {
            base.Show();

            infinityCount = headFrames.Count;
            infinityGrid.CellCount = infinityCount;
            infinityGrid.ForceRefreshActiveCell();
            collectNum.text = LanguageHelper.GetTextContent(10884, Sys_Head.Instance.GetTypesActiveCount(EHeadViewType.HeadFrameView).ToString(), headFrames.Count.ToString());
            clientHeadData = Sys_Head.Instance.clientHead;
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            base.ProcessEventsForEnable(toRegister);

            Sys_Head.Instance.eventEmitter.Handle<uint, EHeadViewType>(Sys_Head.EEvents.OnSelectItem, OnSelectItem, toRegister);
            Sys_Head.Instance.eventEmitter.Handle(Sys_Head.EEvents.OnUsingUpdate, OnUsingUpdate, toRegister);
            Sys_Head.Instance.eventEmitter.Handle(Sys_Head.EEvents.OnExpritedUpdate, OnExpritedUpdate, toRegister);
            Sys_Head.Instance.eventEmitter.Handle(Sys_Head.EEvents.OnAddUpdate, OnAddUpdate, toRegister);
        }

        public override void Hide()
        {
            base.Hide();
            items.Clear();
            infinityGrid.Clear();
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            itemCeils.Clear();
            UI_Head_Item entry = new UI_Head_Item();
            GameObject go = cell.mRootTransform.gameObject;
            entry.BingGameObject(go);
            cell.BindUserData(entry);
            itemCeils.Add(go, entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            uint id = headFrames[index];
            UI_Head_Item item = cell.mUserData as UI_Head_Item;
            item.SetData(id, EHeadViewType.HeadFrameView);
            items.Add(item);
            if (index == 0)
            {
                item.select.SetActive(true);
                Sys_Head.Instance.eventEmitter.Trigger<uint, EHeadViewType>(Sys_Head.EEvents.OnSelectItem, item.id, item.type);
            }
            if (clientHeadData.headFrameId == id)
            {
                item.select.SetActive(true);
                Sys_Head.Instance.eventEmitter.Trigger<uint, EHeadViewType>(Sys_Head.EEvents.OnSelectItem, id, item.type);
                return;
            }
        }


        private void OnSelectItem(uint selectId, EHeadViewType type)
        {
            for (int i = 0; i < items.Count; ++i)
            {
                items[i].select.SetActive(items[i].id == selectId);
            }
        }

        private void OnUsingUpdate()
        {
            for (int i = 0; i < items.Count; ++i)
            {
                items[i].useTag.SetActive(items[i].id == clientHeadData.headFrameId);
            }
        }

        private void OnExpritedUpdate()
        {
            for (int i = 0; i < items.Count; ++i)
            {
                items[i].lockTag.SetActive(!Sys_Head.Instance.IsActiveData(items[i].id));
                items[i].redPoint.SetActive(false);
            }
        }

        private void OnAddUpdate()
        {
            for (int i = 0; i < items.Count; ++i)
            {
                items[i].AddRefresh();
            }
        }
    }
}
