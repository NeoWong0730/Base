using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;
using Lib.Core;

namespace Logic
{
    /// <summary> 家族列表 </summary>
    public class UI_Family_Member_FamilyList : UIBase
    {
        #region 界面组件
        /// <summary> 查询列表列表 </summary>
        private UIComponent uiComponent;
        #endregion
        #region 数据定义
        #endregion
        #region 系统函数
        protected override void OnInit()
        {
        }
        protected override void OnLoaded()
        {
            OnParseComponent();
        }
        protected override void OnDestroy()
        {
            uiComponent?.Hide();
            uiComponent?.OnDestroy();
        }
        protected override void OnOpen(object arg)
        {

        }
        protected override void OnOpened()
        {
            uiComponent?.Reset();
        }
        protected override void OnShow()
        {
            SetData();
            RefreshView();
        }
        protected override void OnHide()
        {
         
        }
        protected override void OnUpdate()
        {

        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            OnProcessEventsForEnable(toRegister);
        }
        #endregion
        #region 初始化
        /// <summary>
        /// 检测组件 
        /// </summary>
        private void OnParseComponent()
        {
            uiComponent = AddComponent<UI_ApplyFamily_Join>(transform.Find("Animator"));
            transform.Find("Animator/View_Title/Btn_Close").GetComponent<Button>().onClick.AddListener(OnClick_Close);
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {

        }
        /// <summary>
        /// 设置数据
        /// </summary>
        private void SetData()
        {
            uiComponent?.SetData();
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            uiComponent?.Show();
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public void OnClick_Close()
        {
            UIManager.HitButton(EUIID.UI_Family_FamilyList, "OnClick_Close");
            CloseSelf();
        }
        #endregion
        #region 提供功能
        #endregion
    }
}