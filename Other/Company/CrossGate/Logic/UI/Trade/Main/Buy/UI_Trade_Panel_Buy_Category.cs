using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Packet;
using Lib.Core;

namespace Logic
{
    public class UI_Trade_Panel_Buy_Category
    {
        private Transform transform;

        //private Dictionary<GameObject, UI_Trade_Category_Cell> dicCells = new Dictionary<GameObject, UI_Trade_Category_Cell>();
        //private InfinityGrid _infinityGrid;

        //private Lib.Core.CoroutineHandler handler;
        public Widget_List_Trade listCategory;

        private List<uint> listIds = new List<uint>();
        private uint _CurCategoryId = 0u;
        public void Init(Transform trans)
        {
            transform = trans;
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }

        public void ProcessEvents(bool register)
        {
            Sys_Trade.Instance.eventEmitter.Handle(Sys_Trade.EEvents.OnViewBuyServerType, OnSelectServer, register);
        }
       
        private void OnSelectServer()
        {
            UpdaeInfo();
        }

        private void OnClickItem(uint arg1, uint arg2)
        {
            uint cateId = arg2 == 0u ? arg1 : arg2;
            Sys_Trade.Instance.SetBuyCatergory(cateId);
            //Debug.LogErrorFormat("type = {0}", arg1);
            //Debug.LogErrorFormat("subType = {0}", arg2);
            //_listener?.OnSelect(arg1, arg2);
        }

        private void UpdaeInfo()
        {
            var itemList = Sys_Trade.Instance.GetTypesData(Sys_Trade.PageType.Buy);

            listCategory = new Widget_List_Trade(transform, OnClickItem, itemList);

            listIds = Sys_Trade.Instance.GetBuyCategoryList();

            if (Sys_Trade.Instance.SearchParam.isSearch && Sys_Trade.Instance.SearchParam.showType != TradeShowType.Publicity)
            {
                if (listIds.Contains(Sys_Trade.Instance.SearchParam.Category))
                {
                    Sys_Trade.Instance.CurBuyCategory = Sys_Trade.Instance.SearchParam.Category;
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011229));
                    Sys_Trade.Instance.SearchParam.Reset();
                    if (listIds.Count > 0)
                        Sys_Trade.Instance.CurBuyCategory = listIds[0];
                }
            }
            else
            {
                if (listIds.Count > 0 && Sys_Trade.Instance.CurBuyCategory == 0u)
                {
                    //if (Sys_Trade.Instance.CurBuyCategory == 0u)
                        Sys_Trade.Instance.CurBuyCategory = listIds[0];
                }
            }

            //Debug.LogError(Sys_Trade.Instance.CurBuyCategory.ToString());
            listCategory.OnSelect(Sys_Trade.Instance.CurBuyCategory);

            //_infinityGrid.CellCount = listIds.Count;
            //_infinityGrid.ForceRefreshActiveCell();
            //_infinityGrid.MoveToIndex(0);

            //if (handler != null)
            //{
            //    CoroutineManager.Instance.Stop(handler);
            //    handler = null;
            //}

            //handler = CoroutineManager.Instance.StartHandler(CheckNeedScroll());
        }

        //private IEnumerator CheckNeedScroll()
        //{
        //    yield return new WaitForSeconds(0.2f);

        //    int index = listIds.IndexOf(Sys_Trade.Instance.CurBuyCategory);
        //    if (index > 7)
        //    {
        //        _infinityGrid.MoveToIndex(index);
        //    }
        //}
    }
}


