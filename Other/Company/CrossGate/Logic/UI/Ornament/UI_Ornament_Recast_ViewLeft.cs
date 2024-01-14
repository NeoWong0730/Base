using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Lib.Core;
using System;

namespace Logic
{
    public class UI_Ornament_Recast_ViewLeft : UIComponent
    {
        public ulong selectedUuid = 0;
        private InfinityGrid infinity;
        private Dictionary<GameObject, UI_Ornament_Recast_CellView> CeilGrids = new Dictionary<GameObject, UI_Ornament_Recast_CellView>();
        private List<ulong> itemUuidList = new List<ulong>();
        private IListener listener;
        private GameObject goTips;

        private Timer timer;
        #region 系统函数
        protected override void Loaded()
        {
            Parse();
        }

        public override void Show()
        {
            base.Show();
            UpdateView();
        }
        public override void Hide()
        {
            base.Hide();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
        }
        public override void OnDestroy()
        {
            timer?.Cancel();
            base.OnDestroy();
        }
        #endregion

        #region func
        private void Parse()
        {
            goTips = transform.parent.Find("Text_Tips").gameObject;
            infinity = transform.GetNeedComponent<InfinityGrid>();
            infinity.onCreateCell += OnCreateCell;
            infinity.onCellChange += OnCellChange;
        }
        public void UpdateView()
        {
            itemUuidList = Sys_Ornament.Instance.GetCanRecastItemList();
            if (selectedUuid > 0)
            {
                for (int i = 0; i < itemUuidList.Count; i++)
                {
                    if(itemUuidList[i] == selectedUuid)
                    {
                        listener?.OnItemSelect(itemUuidList[i]);
                        break;
                    }
                }
            }
            //else if (itemUuidList.Count > 0 && selectedUuid == 0)
            //{
            //    selectedUuid = itemUuidList[0];
            //    listener?.OnItemSelect(itemUuidList[0]);
            //}
            goTips.SetActive(itemUuidList.Count <= 0);
            infinity.CellCount = itemUuidList.Count;
            infinity.ForceRefreshActiveCell();
        }
        public void Register(IListener _listener)
        {
            listener = _listener;
        }
        public void OnItemSelect(ulong itemUuid)
        {
            selectedUuid = itemUuid;
            foreach (var cell in CeilGrids)
            {
                if (cell.Value.gameObject.activeInHierarchy)
                {
                    cell.Value.SetSelectedState(selectedUuid);
                }
            }
            listener?.OnItemSelect(selectedUuid);
        }
        public void MoveToSelectCell()
        {
            if (selectedUuid > 0)
            {
                for (int i = 0; i < itemUuidList.Count; i++)
                {
                    if (itemUuidList[i] == selectedUuid)
                    {
                        Debug.Log(i);
                        timer = Timer.Register(0.2f, () =>
                        {
                            infinity.MoveToIndex(i);
                        });
                        break;
                    }
                }
            }
        }
        #endregion

        #region event
        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_Ornament_Recast_CellView mCell = new UI_Ornament_Recast_CellView();
            mCell.Init(cell.mRootTransform.transform);
            cell.BindUserData(mCell);
            mCell.Register(OnItemSelect);
            CeilGrids.Add(cell.mRootTransform.gameObject, mCell);
        }
        private void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_Ornament_Recast_CellView mCell = cell.mUserData as UI_Ornament_Recast_CellView;
            mCell.UpdateCellView(itemUuidList[index], selectedUuid);
        }
        #endregion
        public interface IListener
        {
            void OnItemSelect(ulong itemUuid);
        }
        public class UI_Ornament_Recast_CellView : UIComponent
        {
            private ItemData itemData;
            private Button btnItem;
            private ulong uuid;
            private System.Action<ulong> _action;
            private Image imgIcon;
            private Image imgQuality;
            private Text txtName;
            private Text txtLv;
            private Text txtPoint;
            private GameObject goSelected;
            private GameObject goRedPoint;
            private GameObject goEquiped;
            protected override void Loaded()
            {
                btnItem = transform.GetComponent<Button>();
                btnItem.onClick.AddListener(OnBtnItemClick);
                imgIcon = transform.Find("EquipItem/Btn_Item/Image_Icon").GetComponent<Image>();
                imgQuality = transform.Find("EquipItem/Btn_Item/Image_Quality").GetComponent<Image>();
                txtName = transform.Find("Text_Name").GetComponent<Text>();
                txtLv = transform.Find("View_Score/Text_LevelNum").GetComponent<Text>();
                txtPoint = transform.Find("View_Score/Text_Point/Text_PointNum").GetComponent<Text>();
                goRedPoint = transform.Find("Image_Up").gameObject;
                goSelected = transform.Find("Image_Select").gameObject;
                goEquiped = transform.Find("EquipItem/Image_Equiped").gameObject;
            }
            public void UpdateCellView(ulong itemUuid,ulong selectedUuid)
            {
                uuid = itemUuid;
                itemData = Sys_Bag.Instance.GetItemDataByUuid(itemUuid);
                ImageHelper.SetIcon(imgIcon, itemData.cSVItemData.icon_id);
                ImageHelper.GetQualityColor_Frame(imgQuality, (int)itemData.Quality);
                goEquiped.SetActive(Sys_Ornament.Instance.IsEquiped(itemData));
                txtName.text = LanguageHelper.GetTextContent(itemData.cSVItemData.name_id);
                txtLv.text = LanguageHelper.GetTextContent(4811, itemData.cSVItemData.lv.ToString());
                txtPoint.text = itemData.ornament.Score.ToString();
                SetSelectedState(selectedUuid);
            }
            public void SetSelectedState(ulong _uuid)
            {
                goSelected.SetActive(uuid == _uuid);
            }
            public void Register(System.Action<ulong> action)
            {
                _action = action;
            }
            private void OnBtnItemClick()
            {
                _action?.Invoke(uuid);
            }
        }
    }
}
