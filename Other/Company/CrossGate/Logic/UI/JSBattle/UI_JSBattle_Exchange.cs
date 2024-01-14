using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
namespace Logic
{
    public class UI_JSBattle_Exchange_Layout
    {
        private Button closeBtn;
        /// <summary> 无限滚动 </summary>
        private InfinityGrid infinityGrid;
        public void Init(Transform transform)
        {
            infinityGrid = transform.Find("Scroll_View_Gem").GetComponent<InfinityGrid>();
            closeBtn = transform.Find("Animator/View_Title08/Btn_Close").GetComponent<Button>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.CloseBtnClicked);
            infinityGrid.onCreateCell += listener.OnCreateCell;
            infinityGrid.onCellChange += listener.OnCellChange;
        }

        public void SetInfinityGridCell(int count)
        {
            infinityGrid.CellCount = count;
            infinityGrid.ForceRefreshActiveCell();
        }

        public interface IListener
        {
            void CloseBtnClicked();
            void OnCreateCell(InfinityGridCell cell);
            void OnCellChange(InfinityGridCell cell, int index);
        }
    }

    public class UI_JSBattle_Exchange : UIBase, UI_JSBattle_Exchange_Layout.IListener
    {
        private UI_JSBattle_Exchange_Layout layout = new UI_JSBattle_Exchange_Layout();
        private UI_CurrencyTitle currency;
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
            currency = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
        }

        protected override void OnOpen(object arg = null)
        {
            uint param = Convert.ToUInt32(arg);
        }

        protected override void OnShow()
        {
            currency.InitUi();
        }

        protected override void OnHide()
        {
        }

        protected override void OnDestroy()
        {
            currency.Dispose();
        }

        /// <summary>
        /// 滚动列表创建回调
        /// </summary>
        /// <param name="cell"></param>
        public void OnCreateCell(InfinityGridCell cell)
        {
        }

        /// <summary>
        /// 滚动列表滚动回调
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        public void OnCellChange(InfinityGridCell cell, int index)
        {
        }

        public void CloseBtnClicked()
        {
            //UIManager.CloseUI(EUIID.UI_JSBattle_Exchange);
        }

    }
}