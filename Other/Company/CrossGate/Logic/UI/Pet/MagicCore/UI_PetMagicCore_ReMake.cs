using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Lib.Core;

namespace Logic
{
    public class UI_PetMagicCore_ReMake : UIComponent, UI_PetMagicCore_ReMake_ViewLeft.IListener
    {
        public uint PageType { get; } = (uint)EMagicCore.Remake;

        private UI_PetMagicCore_ReMake_ViewLeft viewLeft;
        private UI_PetMagicCore_ReMake_ViewMiddle viewMiddle;
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
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnSmeltPetEquipEnd, OnRecastResBack, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle<uint>(Sys_Pet.EEvents.OnPetRemakeRecastTipsEntry, OnRightBtnClicked, toRegister);
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
            viewLeft = AddComponent<UI_PetMagicCore_ReMake_ViewLeft>(transform.Find("Scroll_Jewelry"));
            viewLeft.Register(this);
            viewMiddle = AddComponent<UI_PetMagicCore_ReMake_ViewMiddle>(transform.Find("View_Right"));
            goViewNpc = transform.Find("View_Npc022").gameObject;
        }
        private void UpdateView()
        {
            goViewNpc.SetActive(true);
            viewMiddle.Hide();
            viewLeft.UpdateView();
        }
        #endregion

        #region event
        public void OnItemSelect(ItemData itemUuid)
        {
            goViewNpc.SetActive(false);
            viewMiddle.Show();
            viewMiddle.UpdateView(itemUuid);
        }

        private void OnRecastResBack()
        {
            viewMiddle.UpdateView();
            viewLeft.UpdateView();
        }

        private void OnRightBtnClicked(uint type)
        {
            if (type == 4 || type == 5)
            {
                viewMiddle.OnSure();
            }
        }
        #endregion
    }
}
