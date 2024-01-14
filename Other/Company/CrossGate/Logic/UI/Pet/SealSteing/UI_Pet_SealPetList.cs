using UnityEngine;
using System.Collections.Generic;
using Logic.Core;

namespace Logic
{
    public class UI_Pet_SealPetList
    {
        public Transform transform;
        private List<uint> listIds = new List<uint>();
        private GameObject petListPageGo;
        private List<UI_Pet_Seal_PetCeil> cellList = new List<UI_Pet_Seal_PetCeil>();
        private int visualGridCount;
        private InfinityGrid _infinityGrid;
        private int index;

        public void Init(Transform _transform)
        {
            transform = _transform;
            _infinityGrid = transform.Find("ContentProto/Scroll_View").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_Pet_Seal_PetCeil entry = new UI_Pet_Seal_PetCeil();
            entry.Init(cell.mRootTransform);
            entry.Register(OnCeilClick);
            cellList.Add(entry);
            cell.BindUserData(entry);     
        }

        public void OnCeilClick(uint petId)
        {
            PetBookListPar petBookListPar = new PetBookListPar();
            petBookListPar.petId = petId;
            petBookListPar.showChangeBtn = false;
            petBookListPar.ePetReviewPageType = EPetBookPageType.Seal;
            UIManager.OpenUI(EUIID.UI_Pet_BookReview, false, petBookListPar);
            index = SelectCurIndex(petId);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_Pet_Seal_PetCeil entry = cell.mUserData as UI_Pet_Seal_PetCeil;
            entry.SetData(listIds[index]);
        }

        public void RefreshListView(List<uint> _listIds, uint petId)
        {
            listIds = _listIds;
            visualGridCount = _listIds.Count;
            if (petId != 0)
            {
                index = SelectCurIndex(petId);
            }
            else
            {
                index = 0;
            }
            SetGridGroup(index);
        }

        public int SelectCurIndex(uint curPetId)
        {
            if (curPetId == 0)
            {
                return 0;
            }
            for (int i = 0; i < listIds.Count; ++i)
            {
                if (listIds[i] == curPetId)
                {
                    return i;
                }
            }
            return 0;
        }


        private void SetGridGroup( int _index)
        {
            index = _index;
            _infinityGrid.CellCount = visualGridCount;
            _infinityGrid.Apply();
            if (visualGridCount >= 0)
                _infinityGrid.MoveToIndex(index);
            _infinityGrid.ForceRefreshActiveCell();
    
        }

        public void Refresh()
        {
            for (int i = 0; i < cellList.Count; i++)
            {
                cellList[i].RefreshView();
            }
        }
    }
}
