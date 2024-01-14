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
    public class UI_Trade_Panel_Publicity_Category
    {
        private class UI_Trade_Category_Cell
        {
            private Transform transform;

            private CP_Toggle m_Toggle;
            private Text m_Name;
            private Text m_LightName;

            public uint _categoryType; //            

            public void Init(Transform trans)
            {
                transform = trans;

                m_Toggle = transform.GetComponent<CP_Toggle>();
                m_Toggle.onValueChanged.AddListener(OnClickToggle);

                m_Name = transform.Find("Label").GetComponent<Text>();
                m_LightName = transform.Find("Label (1)").GetComponent<Text>();
            }

            private void OnClickToggle(bool isOn)
            {
                if (isOn)
                {
                    Sys_Trade.Instance.SetPublicityCatergory(_categoryType);
                }
            }

            public void OnSelectCategory(bool isOn)
            {
                m_Toggle.SetSelected(isOn, true);
            }

            public void Update(int index, uint categoryId)
            {
                _categoryType = categoryId;

                CSVCommodityList.Data data = CSVCommodityList.Instance.GetConfData(_categoryType);
                if (data != null)
                    m_Name.text = m_LightName.text = LanguageHelper.GetTextContent(data.name);

                m_Toggle.SetSelected(_categoryType == Sys_Trade.Instance.CurPublicityCategory, true);
            }
        }

        private Transform transform;

        //private InfinityGridLayoutGroup gridGroup;
        //private GridLayoutGroup layoutGroup;
        //private int visualGridCount;
        //private Dictionary<GameObject, UI_Trade_Category_Cell> dicCells = new Dictionary<GameObject, UI_Trade_Category_Cell>();
        //private InfinityGrid _infinityGrid;

        //private Lib.Core.CoroutineHandler handler;
        public Widget_List_Trade listCategory;

        private List<uint> listIds = new List<uint>();
        private uint _CurCategoryId = 0u;
        public void Init(Transform trans)
        {
            transform = trans;

            //_infinityGrid = transform.GetComponent<InfinityGrid>();
            //_infinityGrid.onCreateCell += OnCreateCell;
            //_infinityGrid.onCellChange += OnCellChange;
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
            Sys_Trade.Instance.eventEmitter.Handle(Sys_Trade.EEvents.OnViewPublicityServerType, OnSelectServer, register);
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_Trade_Category_Cell entry = new UI_Trade_Category_Cell();

            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);

            //dicCells.Add(cell.mRootTransform.gameObject, entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_Trade_Category_Cell entry = cell.mUserData as UI_Trade_Category_Cell;
            entry.Update(index, listIds[index]);
        }

        private void OnSelectServer()
        {
            UpdaeInfo();
        }


        private void OnClickItem(uint arg1, uint arg2)
        {
            uint cateId = arg2 == 0u ? arg1 : arg2;
            Sys_Trade.Instance.SetPublicityCatergory(cateId);
            //Debug.LogErrorFormat("type = {0}", arg1);
            //Debug.LogErrorFormat("subType = {0}", arg2);
            //_listener?.OnSelect(arg1, arg2);
        }

        private void UpdaeInfo()
        {
            var itemList = Sys_Trade.Instance.GetTypesData(Sys_Trade.PageType.Publicity);

            listCategory = new Widget_List_Trade(transform, OnClickItem, itemList);

            listIds = Sys_Trade.Instance.GetPublicityCategoryList();

            if (Sys_Trade.Instance.SearchParam.isSearch && Sys_Trade.Instance.SearchParam.showType == TradeShowType.Publicity)
            {
                if (listIds.Contains(Sys_Trade.Instance.SearchParam.Category))
                {
                    Sys_Trade.Instance.CurPublicityCategory = Sys_Trade.Instance.SearchParam.Category;
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011229));
                    Sys_Trade.Instance.SearchParam.Reset();
                    if (listIds.Count > 0)
                        Sys_Trade.Instance.CurPublicityCategory = listIds[0];
                }
            }
            else
            {
                if (listIds.Count > 0 && Sys_Trade.Instance.CurPublicityCategory == 0)
                {
                    //if (Sys_Trade.Instance.CurPublicityCategory == 0u)
                        Sys_Trade.Instance.CurPublicityCategory = listIds[0];
                }
            }

            listCategory.OnSelect(Sys_Trade.Instance.CurPublicityCategory);
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

        //    int index = listIds.IndexOf(Sys_Trade.Instance.CurPublicityCategory);
        //    if (index > 7)
        //    {
        //        _infinityGrid.MoveToIndex(index);
        //    }
        //}
    }
}


