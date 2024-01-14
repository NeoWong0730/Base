using UnityEngine;
using System.Collections;
using Logic.Core;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Table;
using Lib.Core;

namespace Logic
{
    public class UI_Pet_BookListPetList
    {
        public Transform transform;
        private List<uint> listIds = new List<uint>();
        private GameObject petListPageGo;
        private Text numText;
        private Dictionary<GameObject, UI_Pet_BookList_PetCeil> dicCells = new Dictionary<GameObject, UI_Pet_BookList_PetCeil>();
        private List<UI_Pet_BookList_PetCeil> cellList = new List<UI_Pet_BookList_PetCeil>();
        private int visualGridCount;
        private InfinityGrid _infinityGrid;

        public void Init(Transform _transform)
        {
            transform = _transform;
            numText = transform.Find("ContentProto/Text_Number").GetComponent<Text>();
            petListPageGo = transform.Find("ContentProto/Text_Number").gameObject;

            _infinityGrid = transform.Find("ContentProto/Scroll_View").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;

        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_Pet_BookList_PetCeil entry = new UI_Pet_BookList_PetCeil();
            entry.Init(cell.mRootTransform);
            entry.Register(OnCeilClick);
            cellList.Add(entry);
            dicCells.Add(cell.mRootTransform.gameObject, entry);

            cell.BindUserData(entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_Pet_BookList_PetCeil entry = cell.mUserData as UI_Pet_BookList_PetCeil;
            entry.SetData(listIds[index]);
        }

        public void RefreshListView(List<uint> _listIds)
        {
            listIds = _listIds;
            visualGridCount = _listIds.Count;
            SetGridGroup();
            numText.text = LanguageHelper.GetTextContent(2009404, Sys_Pet.Instance.GetPetActiveNum(listIds).ToString(), visualGridCount.ToString()); 
        }

        private void SetGridGroup()
        {
            _infinityGrid.CellCount = visualGridCount;
            _infinityGrid.ForceRefreshActiveCell();
            if (visualGridCount >= 0)
                _infinityGrid.MoveToIndex(0);
        }

        public void Refresh2(List<uint> _listIds)
        {
            listIds = _listIds;
            visualGridCount = _listIds.Count;
            _infinityGrid.CellCount = visualGridCount;
            _infinityGrid.ForceRefreshActiveCell();
            numText.text = LanguageHelper.GetTextContent(2009404, Sys_Pet.Instance.GetPetActiveNum(listIds).ToString(), visualGridCount.ToString());
        }

        public void Refresh()
        {
            for (int i = 0; i < cellList.Count; i++)
            {
                cellList[i].RefreshView();
            }
        }

        public void OnCeilClick(uint petId)
        {
            PetBookListPar petBookListPar = new PetBookListPar();
            petBookListPar.petId = petId;
            petBookListPar.showPetList = new List<uint>();
            for (int i = 0; i < listIds.Count; i++)
            {
                petBookListPar.showPetList.Add(listIds[i]);
            }
            petBookListPar.ePetReviewPageType = EPetBookPageType.Seal;
            UIManager.OpenUI(EUIID.UI_Pet_BookReview, false, petBookListPar);
        }
    }

}

