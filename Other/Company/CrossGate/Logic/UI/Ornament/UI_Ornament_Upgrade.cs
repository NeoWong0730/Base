using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Lib.Core;

namespace Logic
{

    public class UI_Ornament_Upgrade : UIComponent, UI_Ornament_Upgrade_ViewLeft.IListener
    {
        public uint PageType { get; } = (uint)EOrnamentPageType.Upgrade;
        private uint lastType;
        private uint lastLv;
        private bool needDefault = false;

        private UI_Ornament_Upgrade_ViewLeft viewLeft;
        private UI_Ornament_Upgrade_ViewMiddle viewMiddle;
        private GameObject goViewNpc;

        #region 系统函数
        protected override void Loaded()
        {
            Parse();
        }

        public override void Show()
        {
            base.Show();
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
            Sys_Ornament.Instance.eventEmitter.Handle<ulong, bool>(Sys_Ornament.EEvents.OnUpgradeResBack, OnUpgradeResBack, toRegister);
            Sys_Ornament.Instance.eventEmitter.Handle<uint, bool>(Sys_Ornament.EEvents.OnUpgradeAllResBack, OnUpgradeAllResBack, toRegister);
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
            viewLeft = AddComponent<UI_Ornament_Upgrade_ViewLeft>(transform.Find("Scroll01"));
            viewLeft.RegisterListener(this);
            viewMiddle = AddComponent<UI_Ornament_Upgrade_ViewMiddle>(transform.Find("View_Right"));
            goViewNpc = transform.Find("View_Npc022").gameObject;
        }
        private void UpdateView()
        {
            if (needDefault)
            {
                DefaultView();
            }
            else
            {
                viewLeft.UpdateView();
            }
        }
        private void DefaultView()
        {
            viewLeft.DefaultView();
            viewMiddle.Hide();
            goViewNpc.SetActive(true);
        }
        public void ResetDefaultState()
        {
            needDefault = true;
        }
        #endregion

        #region event
        public void OnSelectClick(uint type, uint lv)
        {
            lastType = type;
            lastLv = lv;
            viewMiddle.Show();
            goViewNpc.SetActive(false);
            viewMiddle.UpdateView(type, lv);
            needDefault = false;
        }
        private void OnUpgradeResBack(ulong uuid, bool success)
        {
            if (success)
            {
                OnSelectClick(lastType, lastLv);
            }
            else
            {
                viewMiddle.OnFailUpdate();
            }
            viewMiddle.PlayParticle(success);
            viewLeft.RefreshRedPoint();
        }
        private void OnUpgradeAllResBack(uint infoId,bool hasNew)
        {
            if (hasNew)
            {
                viewMiddle.PlayParticle(true);
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000564));//背包已满，请先清理背包
            }
            OnSelectClick(lastType, lastLv);
            viewLeft.RefreshRedPoint();
        }
        #endregion
    }
}
