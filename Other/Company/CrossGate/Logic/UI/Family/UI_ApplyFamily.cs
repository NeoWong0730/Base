using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using System;
using System.Text;
using Lib.Core;

namespace Logic
{
    /// <summary> 申请家族主界面 </summary>
    public class UI_ApplyFamily : UIBase
    {
        #region 界面组件
        /// <summary> 货币标题通用界面 </summary>
        private UI_CurrencyTitle ui_CurrencyTitle;
        /// <summary> 菜单列表 </summary>
        private List<Toggle> list_Menu = new List<Toggle>();
        /// <summary> 全部子界面 </summary>
        private List<UIComponent> list_ChildView = new List<UIComponent>();
        /// <summary> 当前子界面 </summary>
        private UIComponent curChildView { set; get; }
        #endregion
        #region 数据定义
        /// <summary> 申请家族菜单 </summary>
        public enum EApplyFamilyMenu
        {
            Create = 0, //创建家族
            Join = 1, //加入家族
        }
        /// <summary> 当前菜单 </summary>
        private EApplyFamilyMenu eApplyFamilyMenu;
        /// <summary> 默认搜索 </summary>
        private ulong seartchId_text;
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
            ui_CurrencyTitle.Dispose();
            for (int i = 0, count = list_ChildView.Count; i < count; i++)
            {
                var x = list_ChildView[i];
                x?.Hide();
                x?.OnDestroy();
            }
        }
        protected override void OnOpen(object arg)
        {
            if (arg is Tuple<uint, object>)
            {
                Tuple<uint, object> tuple = arg as Tuple<uint, object>;
                eApplyFamilyMenu = (EApplyFamilyMenu)System.Convert.ToUInt32(tuple.Item1);
                seartchId_text = System.Convert.ToUInt64(tuple.Item2);
            }
            else
            {
                eApplyFamilyMenu = null == arg ? EApplyFamilyMenu.Join : (EApplyFamilyMenu)System.Convert.ToInt32(arg);
            }
        }
        protected override void OnOpened()
        {
            Sys_Family.Instance.SendGuildGetMyApplyListReq();
            for (int i = 0, count = list_ChildView.Count; i < count; i++)
            {
                var x = list_ChildView[i];
                x?.Reset();
            }
        }
        protected override void OnShow()
        {
            ui_CurrencyTitle.InitUi();
            SetMenu(eApplyFamilyMenu);
        }
        protected override void OnHide()
        {

        }
        protected override void OnUpdate()
        {
            curChildView?.ExecUpdate();
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
            var values = System.Enum.GetValues(typeof(EApplyFamilyMenu));
            for (int i = 0, count = values.Length; i < count; i++)
            {
                EApplyFamilyMenu type = (EApplyFamilyMenu)values.GetValue(i);
                UIComponent uiComponent = null;
                Toggle toggle = null;
                switch (type)
                {
                    case EApplyFamilyMenu.Create:
                        {
                            uiComponent = AddComponent<UI_ApplyFamily_Create>(transform.Find("Animator/View_Create"));
                            toggle = transform.Find("Animator/Toggle_Tab/Toggle_Creat").GetComponent<Toggle>();
                        }
                        break;
                    case EApplyFamilyMenu.Join:
                        {
                            uiComponent = AddComponent<UI_ApplyFamily_Join>(transform.Find("Animator/View_Join"));
                            toggle = transform.Find("Animator/Toggle_Tab/Toggle_Join").GetComponent<Toggle>();
                        }
                        break;
                }
                toggle.SetIsOnWithoutNotify(true);//处理下默认界面显示效果
                toggle.onValueChanged.AddListener((bool value) =>
                {
                    OnClick_Menu(uiComponent, type, value);
                });
                list_Menu.Add(toggle);
                if (null != uiComponent) list_ChildView.Add(uiComponent);
            }

            ui_CurrencyTitle = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);
            transform.Find("Animator/View_Title08/Btn_Close").GetComponent<Button>().onClick.AddListener(OnClick_Close);
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_Bag.Instance.eventEmitter.Handle<int, int>(Sys_Bag.EEvents.OnRefreshChangeData, OnItemChange, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.CreateFamily, OnEnterFamily, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.JoinFamily, OnEnterFamily, toRegister);
        }
        #endregion
        #region 界面显示
        #endregion
        #region 响应事件
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public void OnClick_Close()
        {
            UIManager.HitButton(EUIID.UI_ApplyFamily, "Close");
            CloseSelf();
        }
        /// <summary>
        /// 点击菜单
        /// </summary>
        /// <param name="uiComponent"></param>
        /// <param name="menu"></param>
        /// <param name="value"></param>
        public void OnClick_Menu(UIComponent uiComponent, EApplyFamilyMenu menu, bool value)
        {
            if (value)
            {
                StringBuilder btnEventStr = StringBuilderPool.GetTemporary();
                btnEventStr.Append("OnClick_Menu:");
                btnEventStr.Append(((int)menu).ToString());
                UIManager.HitButton(EUIID.UI_ApplyFamily, StringBuilderPool.ReleaseTemporaryAndToString(btnEventStr));
                eApplyFamilyMenu = menu;
                uiComponent?.Show();
                if (seartchId_text != 0)
                {
                    uiComponent.SetData(seartchId_text);
                    seartchId_text = 0;
                }
                if (curChildView != uiComponent)
                    curChildView = uiComponent;
            }
            else
            {
                uiComponent?.Hide();
                if (curChildView == uiComponent)
                    curChildView = null;
            }
            
        }
        /// <summary>
        /// 进入家族
        /// </summary>
        private void OnEnterFamily()
        {
            OnClick_Close();
            Sys_Family.Instance.OpenUI_Family();
        }
        /// <summary>
        /// 兑换界面兑换货币完之后
        /// </summary>
        private void OnItemChange(int changeType, int curBoxId)
        {
            SetMenu(eApplyFamilyMenu);
        }
        #endregion
        #region 提供功能
        /// <summary>
        /// 设置菜单
        /// </summary>
        /// <param name="menu"></param>
        private void SetMenu(EApplyFamilyMenu menu)
        {
            Toggle toggle = list_Menu[(int)menu];
            if (!toggle.isOn)
            {
                toggle.isOn = true;
            }
            else
            {
                toggle.onValueChanged.Invoke(true);
            }
        }
        #endregion
    }
}