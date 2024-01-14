using Lib.Core;
using Logic.Core;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Framework;
using Logic;
using Table;
using UnityEngine.EventSystems;

namespace Logic
{
    public class UI_KingPetReview : UIBase
    {
        private Button m_CloseButton;

        private InfinityGrid m_InfinityGrid;
        private Dictionary<GameObject, UI_Pet_BookList_PetCeil> m_Grids = new Dictionary<GameObject, UI_Pet_BookList_PetCeil>();

        private List<uint> m_Ids = new List<uint>();

        protected override void OnInit()
        {
            m_Ids.Clear();
            var datas = CSVSpecialGoldPetExchange.Instance.GetAll();

            for (int i = 0; i < datas.Count; i++)
            {
                if (datas[i].isShow == 0)
                    continue;
                m_Ids.Add(datas[i].id);
            }
        }

        protected override void OnLoaded()
        {
            m_CloseButton = transform.Find("Animator/View_TipsBg01_Big/Btn_Close").GetComponent<Button>();

            m_InfinityGrid = transform.Find("Animator/Scroll_View").GetComponent<InfinityGrid>();
            m_InfinityGrid.onCreateCell += OnCreateCell;
            m_InfinityGrid.onCellChange += OnCellChange;
            m_CloseButton.onClick.AddListener(onCloseButtonClicked);
        }

        protected override void OnShow()
        {
            UpdateInfoUI();
        }

        private void UpdateInfoUI()
        {
            m_InfinityGrid.CellCount = m_Ids.Count;
            m_InfinityGrid.ForceRefreshActiveCell();
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_Pet_BookList_PetCeil grid = new UI_Pet_BookList_PetCeil();
            grid.Init(cell.mRootTransform);
            grid.Register(OnPetGridSelect);
            cell.BindUserData(grid);
            m_Grids.Add(cell.mRootTransform.gameObject, grid);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_Pet_BookList_PetCeil grid = cell.mUserData as UI_Pet_BookList_PetCeil;

            CSVSpecialGoldPetExchange.Data data = CSVSpecialGoldPetExchange.Instance.GetConfData(m_Ids[index]);

            grid.SetData(data.Pet_id, true);
        }

        private void OnPetGridSelect(uint petId)
        {
            PetBookListPar petBookListPar = new PetBookListPar();
            petBookListPar.petId = petId;
            petBookListPar.showChangeBtn = true;
            petBookListPar.ePetReviewPageType = EPetBookPageType.Seal;
            UIManager.OpenUI(EUIID.UI_Pet_BookReview, false, petBookListPar);
        }

        private void onCloseButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_KingPetReview);
        }
    }
}