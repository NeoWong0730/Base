using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Lib.Core;
using UnityEngine.EventSystems;
using System;

namespace Logic
{
    
    public class UI_PetMagicCore_Make : UIComponent, UI_PetMagicCore_Make_ViewLeft.IListener
    {
        public uint PageType { get; } = (uint)EMagicCore.Make;
        private uint lastType;
        private uint lastId;
        private bool needDefault = false;

        private UI_PetMagicCore_Make_ViewLeft viewLeft;
        private UI_PetMagicCore_Make_ViewMiddle viewMiddle;
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
            viewLeft = AddComponent<UI_PetMagicCore_Make_ViewLeft>(transform.Find("Scroll01"));
            viewLeft.RegisterListener(this);
            viewMiddle = AddComponent<UI_PetMagicCore_Make_ViewMiddle>(transform.Find("View_Right"));
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

        public void InitItemInfo(uint type, uint id)
        {
            lastType = type;
            lastId = id;
            viewLeft.InitItemInfo(lastType, lastId);
        }
        #endregion

        #region event
        public void OnSelectClick(uint type, uint id)
        {
            lastType = type;
            lastId = id;
            viewMiddle.Show();
            goViewNpc.SetActive(false);
            viewMiddle.UpdateView(type, id);
            needDefault = false;
        }
        #endregion
    }
}
