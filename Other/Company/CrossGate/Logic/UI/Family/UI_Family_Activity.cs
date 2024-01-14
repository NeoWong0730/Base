using Logic.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;
using System;

namespace Logic
{
    /// <summary> 家族活动子界面 </summary>
    public class UI_Family_Activity : UIComponent
    {
        #region 界面组件
        /// <summary> 无限滚动 </summary>
        private InfinityGrid _infinityGrid;
        private UI_FamilyParty_Entry viewFamilyPartyEntry;
        #endregion
        #region 数据定义
        /// <summary> 家族活动列表 </summary>
        private List<CSVFamilyActive.Data>  activeList = new List<CSVFamilyActive.Data>();
        #endregion
        #region 系统函数        
        protected override void Loaded()
        {
            OnParseComponent();
        }
        public override void Show()
        {
            base.Show();
            RefreshView();
        }
        public override void Hide()
        {
            viewFamilyPartyEntry.Hide();
            base.Hide();
        }
        public override void OnDestroy()
        {
            viewFamilyPartyEntry.OnDestroy();
            base.OnDestroy();
        }
        /// <summary>
        /// 此函数为了不破坏外部结构，使用为特殊设置值
        /// </summary>
        /// <param name="arg"></param>
        public override void SetData(params object[] arg)
        {
            if (null != arg && arg.Length > 0)
            {
                ActiveResponById(Convert.ToUInt32(arg[0]));
            }
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        public void SetData()
        {
            activeList = Sys_Family.Instance.GetFamilyActiveDataList();
        }

        /// <summary>
        /// 刷新界面
        /// </summary>
        public void RefreshView()
        {
            var count = activeList.Count;
            _infinityGrid.CellCount = count;
            _infinityGrid.ForceRefreshActiveCell();
        }

        #endregion
        #region 初始化
        /// <summary>
        /// 检测组件
        /// </summary>
        private void OnParseComponent()
        {
            _infinityGrid = transform.Find("Scroll_View_Gem").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;
            SetData();
            viewFamilyPartyEntry = AddComponent<UI_FamilyParty_Entry>(transform.Find("UI_Activity_Message"));
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 设置获得界面子物体
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="constructInfo"></param>
        private void SetActiveView(Transform tr, int index)
        {
            var data = activeList[index];
            tr.name = data.id.ToString();
            ImageHelper.SetIcon(tr.Find("Image2").GetComponent<Image>(), data.familyActiveIcon);
            TextHelper.SetText(tr.Find("Text1").GetComponent<Text>(), data.familyActiveName);
            Transform openTime = tr.Find("Image_Time");
            GameObject redPoint = tr.Find("Image_Dot").gameObject;
            if (data.id == 40)
            {
                redPoint.SetActive(Sys_Family.Instance.HasConsignReward());
            }
            else if(data.id == 20)
            {
                redPoint.SetActive(Sys_Family.Instance.HasAuction()); 
            }
            openTime.gameObject.SetActive(false);
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 滚动列表创建回调
        /// </summary>
        /// <param name="cell"></param>
        private void OnCreateCell(InfinityGridCell cell)
        {
            GameObject go = cell.mRootTransform.gameObject;
            go.transform.Find("Image1").GetComponent<Button>().onClick.AddListener(() => { OnClick_Active(go.transform); });
        }

        /// <summary>
        /// 滚动列表滚动回调
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        private void OnCellChange(InfinityGridCell cell, int index)
        {
            SetActiveView(cell.mRootTransform.transform, index);
        }

        /// <summary>
        /// 活动点击事件
        /// </summary>
        /// <param name="tr"></param>
        private void OnClick_Active(Transform tr)
        {
            
            uint.TryParse(tr.name, out uint id);
            UIManager.HitButton(EUIID.UI_Family, "OnClick_Active:" + id.ToString());
            tr.Find("Image_Dot").gameObject.SetActive(false);
            ActiveResponById(id);
        }

        /// <summary>
        /// 活动点击事件处理-特殊接口，外部可直接响应
        /// </summary>
        /// <param name="tr"></param>
        private void ActiveResponById(uint id)
        {
            switch (id)
            {
                case 1:
                    UIManager.OpenUI(EUIID.UI_Family_Construct);
                    break;
                case 10:
                    viewFamilyPartyEntry.Show();
                    break;
                case 20:
                    UIManager.OpenUI(EUIID.UI_FamilySale);
                    break;
                case 30:
                    UIManager.OpenUI(EUIID.UI_FamilyBoss_Info);
                    break;
                case 40:
                    UIManager.OpenUI(EUIID.UI_FamilyWorkshop);
                    break;
                case 60:
                    Sys_Family.Instance.OpenFamilyCreatureUI();
                    break;
                case 70:
                    UIManager.OpenUI(EUIID.UI_MerchantFleet_FamilyHelp);
                    break;
            }
        }
        #endregion
        #region 提供功能
        #endregion
    }
}