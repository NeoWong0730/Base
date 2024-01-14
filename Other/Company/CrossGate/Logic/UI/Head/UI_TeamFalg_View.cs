using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Framework;
using Lib.Core;

namespace Logic
{
    public class UI_TeamFalg_View : UIComponent
    {
        public AssetDependencies assetDependencies;
        private ShowSceneControl showSceneControl;
        private HeroLoader heroLoader;

        private GameObject itemGo;
        public Text collectNum;

        private List<uint> logos = new List<uint>();
        private Dictionary<GameObject, UI_Head_Item> itemCeils = new Dictionary<GameObject, UI_Head_Item>();
        private List<UI_Head_Item> items = new List<UI_Head_Item>();
        private InfinityGrid infinityGrid;
        private int infinityCount;
        private ClientHeadData clientHeadData;

        protected override void Loaded()
        {
            itemGo = transform.Find("Viewport/Content/Item").gameObject;
            logos = Sys_Head.Instance.GetDatasId(EHeadViewType.TeamFalgView);

            infinityGrid = transform.GetComponent<InfinityGrid>();
            infinityGrid.onCreateCell += OnCreateCell;
            infinityGrid.onCellChange += OnCellChange;
        }

        public override void Show()
        {
            base.Show();

            infinityCount = logos.Count;
            infinityGrid.CellCount = infinityCount;
            infinityGrid.ForceRefreshActiveCell();
            collectNum.text = LanguageHelper.GetTextContent(10884, Sys_Head.Instance.GetTypesActiveCount(EHeadViewType.TeamFalgView).ToString(), logos.Count.ToString());
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
            uint id = logos[index];
            UI_Head_Item item = cell.mUserData as UI_Head_Item;
            item.SetData(id, EHeadViewType.TeamFalgView);
            items.Add(item);
            if (index == 0)
            {
                item.select.SetActive(true);
                Sys_Head.Instance.eventEmitter.Trigger<uint, EHeadViewType>(Sys_Head.EEvents.OnSelectItem, item.id, item.type);
            }
            if (clientHeadData.teamLogeId == id)
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
                items[i].useTag.SetActive(items[i].id == clientHeadData.teamLogeId);
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
