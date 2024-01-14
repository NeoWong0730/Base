using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public partial class UI_Knowledge_Cooking : UIBase
    {
        private Button m_PointButton;
        private GameObject m_RedPoint;
        private Image m_RewardIcon;
        private Text m_PointText;
        private Slider m_Slider;
        private InfinityGrid m_InfinityGrid;
        private int m_CurRecipeSelectIndex = -1;
        private Dictionary<GameObject, GridWarp> m_Grids = new Dictionary<GameObject, GridWarp>();
        private List<Cooking> m_Cookings = new List<Cooking>();

        private Transform m_ToggleParent;
        private CP_ToggleRegistry m_CP_ToggleRegistry_AtlasTab;
        private int m_CurAtlasTab;
        private int curAtlasTab
        {
            get { return m_CurAtlasTab; }
            set
            {
                if (m_CurAtlasTab != value)
                {
                    m_CurAtlasTab = value;
                    OnAtlasTabChanged();
                }
            }
        }
        private Cooking m_CurSelectCooking;
        private UI_CurrencyTitle m_UI_CurrencyTitle;
        private Timer m_ScorllMoveTimer;

        private CP_ToggleRegistry m_CP_ToggleRegistry_Special;
        private int m_CurSpecialTab;

        private void RegisterLeft()
        {
            m_PointButton = transform.Find("Animator/Bottom/Image_BG (1)/Center/Image_Bottom/Button").GetComponent<Button>();
            m_RewardIcon = transform.Find("Animator/Bottom/Image_BG (1)/Center/Image_Bottom/Button").GetComponent<Image>();
            m_RedPoint = transform.Find("Animator/Bottom/Image_BG (1)/Center/Image_Bottom/Button/Image_Red").gameObject;
            m_PointText = transform.Find("Animator/Bottom/Image_BG (1)/Center/Image_Bottom/Text_num").GetComponent<Text>();
            m_Slider = transform.Find("Animator/Bottom/Image_BG (1)/Center/Image_Bottom/Slider").GetComponent<Slider>();

            m_InfinityGrid = transform.Find("Animator/Bottom/Image_BG (1)/Center/Scroll View").GetComponent<InfinityGrid>();
            m_InfinityGrid.onCellChange = OnCellUpdateCallback;
            m_InfinityGrid.onCreateCell = OnCreateCellCallback;
            m_ToggleParent = transform.Find("Animator/Bottom/Image_BG/Image_BG (2)/Left/Toggle_Tab");

            m_CP_ToggleRegistry_AtlasTab = transform.Find("Animator/Bottom/Image_BG/Image_BG (2)/Left/Toggle_Tab").GetComponent<CP_ToggleRegistry>();
            m_CP_ToggleRegistry_AtlasTab.onToggleChange = OnAtlasTabChanged;

            m_CP_ToggleRegistry_Special = transform.Find("Animator/Bottom/Image_BG (1)/Center/Toggle").GetComponent<CP_ToggleRegistry>();
            m_CP_ToggleRegistry_Special.onToggleChange = OnSpecialTabChanged;

            m_PointButton.onClick.AddListener(OnPointButtonClicked);
            m_UI_CurrencyTitle = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);
        }


        private void SetCookingData_Normal()
        {
            if (m_CurAtlasTab == 0)
            {
                m_Cookings = Sys_Cooking.Instance.cookings;
            }
            else if (m_CurAtlasTab == 1)
            {
                m_Cookings = Sys_Cooking.Instance.type_1;
            }
            else if (m_CurAtlasTab == 2)
            {
                m_Cookings = Sys_Cooking.Instance.type_2;
            }
            else if (m_CurAtlasTab == 3)
            {
                m_Cookings = Sys_Cooking.Instance.type_3;
            }
            else if (m_CurAtlasTab == 4)
            {
                m_Cookings = Sys_Cooking.Instance.type_4;
            }
            if (m_Cookings.Count > 0)
            {
                m_CurSelectCooking = m_Cookings[m_CurRecipeSelectIndex];
                curSelectSubmitIndex = m_CurSelectCooking.submitIndex;
            }
        }

        private void SetCookingData_Special()
        {
            if (m_CurAtlasTab == 0)
            {
                m_Cookings = Sys_Cooking.Instance.cookings_Special;
            }
            else if (m_CurAtlasTab == 1)
            {
                m_Cookings = Sys_Cooking.Instance.type_1_Special;
            }
            else if (m_CurAtlasTab == 2)
            {
                m_Cookings = Sys_Cooking.Instance.type_2_Special;
            }
            else if (m_CurAtlasTab == 3)
            {
                m_Cookings = Sys_Cooking.Instance.type_3_Special;
            }
            else if (m_CurAtlasTab == 4)
            {
                m_Cookings = Sys_Cooking.Instance.type_4_Special;
            }
            if (m_Cookings.Count > 0)
            {
                m_CurSelectCooking = m_Cookings[m_CurRecipeSelectIndex];
                curSelectSubmitIndex = m_CurSelectCooking.submitIndex;
            }
        }

        private void RefreshLeftView()
        {
            m_InfinityGrid.CellCount = m_Cookings.Count;
            m_InfinityGrid.ForceRefreshActiveCell();
            m_InfinityGrid.StopMovement();
            UpdateRecipeSelect();
            RefreshScore();
            RefreshReward(0);
            RefreshLabelRedPoint();
            if (m_CurRecipeSelectIndex == 0)
            {
                m_InfinityGrid.MoveToIndex(m_CurRecipeSelectIndex);
            }
            else
            {
                m_ScorllMoveTimer?.Cancel();
                m_ScorllMoveTimer = Timer.Register(0.2f, ScorllMoveTo);
            }
        }

        private void ScorllMoveTo()
        {
            m_InfinityGrid.MoveToIndex(m_CurRecipeSelectIndex);
        }


        private void OnCellUpdateCallback(InfinityGridCell cell, int index)
        {
            GridWarp gridWarp = cell.mUserData as GridWarp;
            gridWarp.SetData(m_Cookings[index], index);
            if (m_CurRecipeSelectIndex == index)
            {
                gridWarp.Select();
            }
            else
            {
                gridWarp.Release();
            }
        }

        private void OnCreateCellCallback(InfinityGridCell cell)
        {
            GridWarp gridWarp = new GridWarp();
            gridWarp.BindGameObject(cell.mRootTransform.gameObject);
            gridWarp.AddEvent(OnItemSelected);
            cell.BindUserData(gridWarp);
            m_Grids[cell.mRootTransform.gameObject] = gridWarp;
        }

        private void OnItemSelected(GridWarp gridWarp)
        {
            if (m_CurRecipeSelectIndex != gridWarp.dataIndex)
            {
                m_CurRecipeSelectIndex = gridWarp.dataIndex;
                UpdateRecipeSelect();
                m_CurSelectCooking = m_Cookings[m_CurRecipeSelectIndex];
                //m_CurSelectCooking.UpdateSubmitableItem();
                curSelectSubmitIndex = m_CurSelectCooking.submitIndex;
                RefreshRightView();
            }
        }

        private void UpdateRecipeSelect()
        {
            foreach (var item in m_Grids)
            {
                if (item.Value.dataIndex == m_CurRecipeSelectIndex)
                {
                    item.Value.Select();
                }
                else
                {
                    item.Value.Release();
                }
            }
        }

        private void OnAtlasTabChanged(int curToggle, int old)
        {
            curAtlasTab = curToggle;
        }

        private void OnSpecialTabChanged(int current, int old)
        {
            if (current == old)
            {
                return;
            }

            m_CurSpecialTab = current;

            m_CurRecipeSelectIndex = 0;

            if (m_CurSpecialTab == 0)
            {
                SetCookingData_Normal();
            }
            else if (m_CurSpecialTab == 1)
            {
                SetCookingData_Special();
            }

            RefreshLeftView();
            RefreshRightView();
        }

        private void OnAtlasTabChanged()
        {
            m_CurRecipeSelectIndex = 0;

            if (m_CurSpecialTab == 0)
            {
                SetCookingData_Normal();
            }
            else if (m_CurSpecialTab == 1)
            {
                SetCookingData_Special();
            }
            RefreshLeftView();
            RefreshRightView();
        }

        private void OnPointButtonClicked()
        {
            Sys_Cooking.Instance.UpdateRewards();
            UIManager.OpenUI(EUIID.UI_Knowledge_Cooking_Award);
        }

        private void OnActiveCook(uint cookId)
        {
            foreach (var item in m_Grids)
            {
                if (!item.Key.activeSelf)
                    continue;
                if (item.Value.cooking.id == cookId)
                {
                    item.Value.RefreshActive();
                }
            }
        }

        private void OnRefreshWatch(uint cookId)
        {
            foreach (var item in m_Grids)
            {
                if (item.Value.cooking.id == cookId)
                {
                    item.Value.RefreshWatchState();
                }
            }
        }

        private void OnRefreshLeftSubmitRedPoint()
        {
            foreach (var item in m_Grids)
            {
                if (!item.Key.activeSelf)
                    continue;

                item.Value.RefreshSubmit();
                //if (item.Value.cooking.id == cookId)
                //{
                   
                //}
            }
            RefreshLabelRedPoint();
        }


        private void RefreshScore()
        {
            Sys_Cooking.Instance.GetTargetScore(out uint targetScore);
            TextHelper.SetText(m_PointText, string.Format($"{Sys_Cooking.Instance.curScore}/{targetScore}"));
            float rate = (float)Sys_Cooking.Instance.curScore / (float)targetScore;
            m_Slider.value = rate;
            RefreshReward(0);
        }

        private void RefreshReward(uint rewardId)
        {
            m_RedPoint.SetActive(Sys_Cooking.Instance.HasReward());
            uint iconId = 0;
            if (Sys_Cooking.Instance.GetAllReward())
            {
                iconId = 993102;
            }
            else
            {
                iconId = 993101;
            }
            ImageHelper.SetIcon(m_RewardIcon, iconId);
        }

        private void RefreshLabelRedPoint()
        {
            for (int i = 0; i < m_ToggleParent.childCount; i++)
            {
                m_ToggleParent.GetChild(i).Find("Image_Red").gameObject.SetActive(Sys_Cooking.Instance.HasSubmitItem((uint)i));
            }
        }


        public class GridWarp
        {
            public int dataIndex;
            public Cooking cooking;
            private Image m_Icon;
            private Text m_Name;
            public GameObject gameObject;
            private Button m_ClickButton;
            private Action<GridWarp> m_SelectAction;
            private GameObject m_Select;
            private GameObject m_Lock;
            private GameObject m_SubmitRedPoint;
            private GameObject m_AllSubmitFx;
            private GameObject m_Mutli;
            private GameObject m_Watch;
            private GameObject m_Advance;


            public void BindGameObject(GameObject @object)
            {
                gameObject = @object;
                ParseCp();
            }

            private void ParseCp()
            {
                m_SubmitRedPoint = gameObject.transform.Find("Image_Red").gameObject;
                m_ClickButton = gameObject.transform.Find("Image_bottom").GetComponent<Button>();
                m_Icon = gameObject.transform.Find("Image_icon_bg/Image_icon").GetComponent<Image>();
                m_Name = gameObject.transform.Find("Text").GetComponent<Text>();
                m_Select = gameObject.transform.Find("Image").gameObject;
                m_Lock = gameObject.transform.Find("Image_Black").gameObject;
                m_AllSubmitFx = gameObject.transform.Find("Fx_ui_Knowledge_Cooking").gameObject;
                m_Mutli = gameObject.transform.Find("Image_Label").gameObject;
                m_Watch = gameObject.transform.Find("Image_Collect_bg").gameObject;
                m_Advance = gameObject.transform.Find("Image_Label2").gameObject;
                m_ClickButton.onClick.AddListener(OnButtonClicked);
            }

            public void SetData(Cooking cooking, int dataIndex)
            {
                this.cooking = cooking;
                this.dataIndex = dataIndex;
                Refresh();
            }

            public void Refresh()
            {
                ImageHelper.SetIcon(m_Icon, cooking.cSVCookData.icon);
                TextHelper.SetText(m_Name, cooking.cSVCookData.name);

                m_Mutli.SetActive(cooking.cookType == 3);
                m_Advance.SetActive(cooking.cookType == 2);
                RefreshWatchState();
                RefreshActive();
                RefreshSubmit();
            }

            public void RefreshWatchState()
            {
                m_Watch.SetActive(cooking.watch);
            }

            public void RefreshActive()
            {
                m_Lock.SetActive(cooking.active == 0);
            }

            public void RefreshSubmit()
            {
                m_SubmitRedPoint.SetActive(cooking.CanSubmit());
                m_AllSubmitFx.SetActive(cooking.allSubmit);
            }

            public void AddEvent(Action<GridWarp> action)
            {
                m_SelectAction = action;
            }

            private void OnButtonClicked()
            {
                m_SelectAction.Invoke(this);
            }

            public void Release()
            {
                m_Select.SetActive(false);
            }

            public void Select()
            {
                m_Select.SetActive(true);
            }
        }
    }
}


