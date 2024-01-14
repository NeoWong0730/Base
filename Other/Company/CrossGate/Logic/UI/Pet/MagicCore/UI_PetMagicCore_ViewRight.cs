using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;

namespace Logic
{
    public class UI_PetMagicCore_ViewRight : UIComponent
    {
        #region 界面组件
        private MagicCoreRightCell cellUpgrade;
        private MagicCoreRightCell cellRecast;
        #endregion
        private IListener listener;
        private Dictionary<uint, MagicCoreRightCell> dicCells = new Dictionary<uint, MagicCoreRightCell>();

        #region 系统函数
        protected override void Loaded()
        {
            base.Loaded();
            Init();
        }
        #endregion

        #region function
        private void Init()
        {
            cellUpgrade = AddComponent<MagicCoreRightCell>(transform.Find("Label_Scroll01/TabList/TabItem01"));
            cellUpgrade.PageType = (uint)EMagicCore.Make;
            cellUpgrade.Register(OnTabSelect);
            dicCells.Add((uint)cellUpgrade.PageType, cellUpgrade);

            cellRecast = AddComponent<MagicCoreRightCell>(transform.Find("Label_Scroll01/TabList/TabItem02"));
            cellRecast.PageType = (uint)EMagicCore.Remake;
            cellRecast.Register(OnTabSelect);
            dicCells.Add((uint)cellRecast.PageType, cellRecast);

        }
        public void OnPageBtnInit(uint openType)
        {
            dicCells[openType].SetSelected(true);
        }
        public void Register(IListener _listener)
        {
            listener = _listener;
        }
        private void OnTabSelect(uint type)
        {
            listener?.OnPageSelect(type);
        }
        #endregion

        #region 响应事件

        #endregion
        public interface IListener
        {
            void OnPageSelect(uint type);
        }

        //cell类
        public class MagicCoreRightCell : UIComponent
        {
            public uint PageType;
            #region 界面组件
            private CP_Toggle toggle;
            #endregion
            private System.Action<uint> _action;

            #region 系统函数
            protected override void Loaded()
            {
                Init();
            }
            #endregion

            #region function
            private void Init()
            {
                toggle = transform.GetComponent<CP_Toggle>();
                toggle.onValueChanged.AddListener(OnClickToggle);
            }
            public void SetSelected(bool isOn)
            {
                toggle.SetSelected(isOn, true);
            }
            public void Register(System.Action<uint> action)
            {
                _action = action;
            }
            #endregion

            #region 响应事件
            private void OnClickToggle(bool isOn)
            {
                if (isOn)
                {
                    _action?.Invoke((uint)PageType);
                }
            }
            #endregion
        }

    }
}