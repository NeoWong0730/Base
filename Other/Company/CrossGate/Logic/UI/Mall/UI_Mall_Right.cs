using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic.Core;
using Packet;
using Table;
using Lib.Core;

namespace Logic
{
    public class UI_Mall_Right : UIComponent
    {
        private InfinityGrid _infinityGrid;
        private CoroutineHandler handler;
        private Dictionary<GameObject, UI_Mall_Right_Cell> dicCells = new Dictionary<GameObject, UI_Mall_Right_Cell>();
        //private int visualGridCount;

        private CP_Toggle _toggleCharge;

        private List<uint> shopIds = new List<uint>();
        private List<uint> shopNames = new List<uint>();

        private IListener mlistener;
        protected override void Loaded()
        {
            _infinityGrid = transform.Find("Scroll_View").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;

            Transform transCharge = transform.Find("Toggle_Charge");
            if (transCharge != null)
            {
                _toggleCharge = transCharge.GetComponent<CP_Toggle>();
                _toggleCharge.onValueChanged.AddListener(OnToggleCharge);
            }
            //_toggleCharge = transform.Find("Toggle_Charge").GetComponent<CP_Toggle>();
            //_toggleCharge.onValueChanged.AddListener(OnToggleCharge);
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_Mall_Right_Cell entry = new UI_Mall_Right_Cell();
            entry.AddListener(OnSelectShop);
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);

            dicCells.Add(cell.mRootTransform.gameObject, entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            //cell.mRootTransform.name = index.ToString();

            UI_Mall_Right_Cell entry = cell.mUserData as UI_Mall_Right_Cell;
            entry.UpdateInfo(index, shopIds[index], shopNames[index]);
        }

        private void OnSelectShop(uint shopId)
        {
            mlistener?.OnSelectShop(shopId);
            _toggleCharge?.SetSelected(false, true);
        }

        private void OnToggleCharge(bool isOn)
        {
            //DebugUtil.LogErrorFormat("toggle charge {0}", isOn);
            if (isOn)
            {
                Sys_Mall.Instance.SelectShopId = 0u;
                foreach (var data in dicCells)
                    data.Value.OnSelect(false);
            }
            mlistener?.OnToggleCharge(isOn);
        }

        public void RegisterListener(IListener listener)
        {
            mlistener = listener;
        }

        public void UpdateInfo(uint mallId, uint defaultShopId, bool isCharge = false)
        {
            _toggleCharge?.gameObject.SetActive(mallId == 101u);

            //cal shop ids
            int count = 0;
            shopIds.Clear();
            shopNames.Clear();

            CSVMall.Data mallData = CSVMall.Instance.GetConfData(mallId);
            if (mallData.shop_id.Count <= mallData.tab_name.Count)
            {
                for (int i = 0; i < mallData.shop_id.Count; ++i)
                {
                    shopIds.Add(mallData.shop_id[i]);
                    shopNames.Add(mallData.tab_name[i]);
                }
                count = mallData.shop_id.Count;
            }
            else
            {
                Debug.LogErrorFormat("商店和名字数量不匹配..mallId={0}", mallId);
            }

            Sys_Mall.Instance.SelectShopId = defaultShopId != 0 ? defaultShopId : shopIds[0];

            _infinityGrid.CellCount = count;
            _infinityGrid.ForceRefreshActiveCell();

            if (isCharge)
            {
                _toggleCharge.SetSelected(true, true);
                return;
            }


            if (handler != null)
            {
                CoroutineManager.Instance.Stop(handler);
                handler = null;
            }

            //设置默认选中商店
            if (count > 0)
            {
                int index = shopIds.IndexOf(Sys_Mall.Instance.SelectShopId);
                _infinityGrid.MoveToIndex(index);

                //handler = CoroutineManager.Instance.StartHandler(CheckNeedScroll(Sys_Mall.Instance.SelectShopId));
            }
        }

       public void RefreshRedDot()
        {
            foreach (var data in dicCells)
                data.Value.RefreshRedDot();
        }

        //private IEnumerator CheckNeedScroll(uint shopId)
        //{
        //    yield return new WaitForSeconds(0.2f);

        //    int index = shopIds.IndexOf(shopId);
        //    _infinityGrid.MoveToIndex(index);

        //    //foreach (var data in dicCells)
        //    //{
        //    //    if (data.Value.mShopId == selectShopId)
        //    //    {
        //    //        data.Value.OnSelect(true);
        //    //    }
        //    //}
        //}

        public interface IListener
        {
            void OnSelectShop(uint shopId);

            void OnToggleCharge(bool toggle);
        }
    }
}


