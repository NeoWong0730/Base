using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Lib.Core;

namespace Logic
{
    public class UI_Ornament_Recast : UIComponent, UI_Ornament_Recast_ViewLeft.IListener
    {
        public uint PageType { get; } = (uint)EOrnamentPageType.Recast;
        private bool isOpen;

        private UI_Ornament_Recast_ViewLeft viewLeft;
        private UI_Ornament_Recast_ViewMiddle viewMiddle;
        private GameObject goViewNpc;
        private Text txtNpcShow;
        #region 系统函数
        protected override void Loaded()
        {
            Parse();
        }

        public override void Show()
        {
            base.Show();
            viewLeft.selectedUuid = 0;
            viewLeft.Show();
            viewMiddle.Show();
            UpdateView();
        }
        public override void Hide()
        {
            base.Hide();
            viewLeft.Hide();
            viewMiddle.Hide();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Ornament.Instance.eventEmitter.Handle(Sys_Ornament.EEvents.OnRecastResBack, OnRecastResBack, toRegister);
        }
        public override void OnDestroy()
        {
            viewLeft.OnDestroy();
            viewMiddle.OnDestroy();
            base.OnDestroy();
        }
        #endregion

        #region func
        private void Parse()
        {
            viewLeft = AddComponent<UI_Ornament_Recast_ViewLeft>(transform.Find("Scroll_Jewelry"));
            viewLeft.Register(this);
            viewMiddle = AddComponent<UI_Ornament_Recast_ViewMiddle>(transform.Find("View_Right"));
            goViewNpc = transform.Find("View_Npc022").gameObject;
            txtNpcShow = transform.Find("View_Npc022/Text").GetComponent<Text>();
        }
        private void UpdateView()
        {
            var itemUuidList = Sys_Ornament.Instance.GetCanRecastItemList();
            goViewNpc.SetActive(true);
            viewMiddle.Hide();
            if (itemUuidList.Count <= 0)
            {
                txtNpcShow.text = LanguageHelper.GetTextContent(680000566);//暂无可重铸的饰品
            }
            else
            {
                txtNpcShow.text = LanguageHelper.GetTextContent(680000557);//请先选中要重铸的饰品
            }
        }
        public void OnRecastOpen(ulong uid)
        {
            if (uid > 0 && !isOpen)
            {
                isOpen = true;
                viewLeft.OnItemSelect(uid);
                viewLeft.MoveToSelectCell();
            }
        }
        #endregion

        #region event
        public void OnItemSelect(ulong itemUuid)
        {
            goViewNpc.SetActive(false);
            viewMiddle.Show();
            viewMiddle.UpdateView(itemUuid);
        }
        private void OnRecastResBack()
        {
            viewMiddle.RefreshCacheShowlist();
            viewMiddle.UpdateView(Sys_Ornament.Instance.LastRecastUuid);
            viewLeft.UpdateView();
        }
        #endregion
    }
}
