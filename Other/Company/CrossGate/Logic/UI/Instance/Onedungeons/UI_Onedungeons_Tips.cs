using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Logic
{
    /// <summary> 单人副本提示 </summary>
    public class UI_Onedungeons_Tips : UIBase
    {
        #region 界面组件
        /// <summary> 菜单 </summary>
        private List<Toggle> list_Menu = new List<Toggle>();
        /// <summary> 内容 </summary>
        private Text text_Content;
        #endregion
        #region 数据
        /// <summary> 下标 </summary>
        private int index = 0;
        #endregion
        #region 系统函数

        protected override void OnLoaded()
        {
            OnParseComponent();
        }

        protected override void OnShow()
        {
            RefreshView();
        }

        #endregion
        #region 初始化
        /// <summary>
        /// 检测组件 
        /// </summary>
        private void OnParseComponent()
        {
            Transform toggleGroup = transform.Find("Animator/ToggleGroup");

            for (int i = 0, count = toggleGroup.childCount; i < count; i++)
            {
                list_Menu.Add(toggleGroup.GetChild(i).GetComponent<Toggle>());
            }

            text_Content = transform.Find("Animator/Scroll_View/Text").GetComponent<Text>();

            for (int i = 0, count = list_Menu.Count; i < count; i++)
            {
                var item = list_Menu[i];
                item.onValueChanged.AddListener((bool value) =>
                {
                    if (value) OnClick_Menu(item);
                });
            }

            Lib.Core.EventTrigger.Get(transform.Find("Animator/Image_Mask")).AddEventListener(EventTriggerType.PointerClick, OnClick_Close);
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            switch (index)
            {
                case 0:
                    text_Content.text = "0";
                    break;
                case 1:
                    text_Content.text = "1";
                    break;
            }
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 关闭界面
        /// </summary>
        /// <param name="eventData"></param>
        public void OnClick_Close(BaseEventData eventData)
        {
            CloseSelf();
        }
        /// <summary>
        /// 点击菜单
        /// </summary>
        /// <param name="toggle"></param>
        public void OnClick_Menu(Toggle toggle)
        {
            index = list_Menu.IndexOf(toggle);
            RefreshView();
        }
        #endregion
        #region 提供功能
        #endregion
    }
}
