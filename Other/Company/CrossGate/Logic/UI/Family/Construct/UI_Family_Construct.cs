using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using Lib.Core;
using UnityEngine;

namespace Logic
{
    /// <summary> 家族建设 </summary>
    public class UI_Family_Construct : UIBase
    {
        #region 界面组件
        /// <summary> 规则按钮 </summary>
        private Button ruleBtn;
        /// <summary> 货币标题通用界面 </summary>
        private UI_CurrencyTitle ui_CurrencyTitle;
        /// <summary> 菜单列表 </summary>
        private List<Toggle> list_Menu = new List<Toggle>();
        /// <summary> 子界面列表 </summary>
        private List<UIComponent> list_ChildView = new List<UIComponent>();
        /// <summary> 当前子界面 </summary>
        private UIComponent curChildView { set; get; }
        #endregion
        #region 数据定义
        /// <summary> 家族菜单 </summary>
        public enum EFamilyConstruct
        {
            Construct = 0, //建设
            Prosperity = 1, //繁荣
        }
        /// <summary> 当前菜单 </summary>
        private EFamilyConstruct eFamilyMenu;
        /// <summary> 菜单改变事件 </summary>
        public UnityEngine.Events.UnityAction<bool> onValueChanged;

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
            eFamilyMenu = EFamilyConstruct.Construct;            
        }

        protected override void OnShow()
        {
            ui_CurrencyTitle.InitUi();
            SetMenu(eFamilyMenu);
        }

        protected override void OnHide()
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
            var values = System.Enum.GetValues(typeof(EFamilyConstruct));
            for (int i = 0, count = values.Length; i < count; i++)
            {
                EFamilyConstruct type = (EFamilyConstruct)values.GetValue(i);
                UIComponent uiComponent = null;
                Toggle toggle = null;
                switch (type)
                {
                    case EFamilyConstruct.Construct:
                        {
                            toggle = transform.Find("Animator/View_Left_Tabs/Label_Scroll01/TabList/TabItem").GetComponent<Toggle>();
                            uiComponent = AddComponent<UI_Family_Construct_Info>(transform.Find("Animator/View_Construction"));
                        }
                        break;
                    case EFamilyConstruct.Prosperity:
                        {
                            toggle = transform.Find("Animator/View_Left_Tabs/Label_Scroll01/TabList/TabItem (1)").GetComponent<Toggle>();
                            uiComponent = AddComponent<UI_Family_Construct_Level>(transform.Find("Animator/View_Level"));
                        }
                        break;
                }
                if (eFamilyMenu == type)
                    toggle.SetIsOnWithoutNotify(true);//默认prefab上菜单显示不正确，需要强制刷新下。 
                toggle.onValueChanged.AddListener((bool value) => OnClick_Menu(uiComponent, type, value));
                list_Menu.Add(toggle);
                list_ChildView.Add(uiComponent);
            }
            ruleBtn = transform.Find("Animator/Btn_Details").GetComponent<Button>();
            ruleBtn.onClick.AddListener(OnClick_Rule);
            ui_CurrencyTitle = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);
            ui_CurrencyTitle.SetData(new List<uint>() { 17 });
            transform.Find("Animator/View_Title01/Btn_Close").GetComponent<Button>().onClick.AddListener(OnClick_Close);
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_Bag.Instance.eventEmitter.Handle<uint, long>(Sys_Bag.EEvents.OnCurrencyChanged, OnUpdateFamilyInfo, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.GuildCurrencyChange, OnUpdateFamilyInfo, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.GuildConstructLvChange, OnUpdateFamilyInfo, toRegister);
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
            UIManager.HitButton(EUIID.UI_Family_Construct, "OnClick_Close");
            CloseSelf();
        }

        public void OnClick_Rule()
        {
            UIManager.OpenUI(EUIID.UI_Construt_Rule, false, new UIRuleParam { StrContent = LanguageHelper.GetTextContent(3290000005)});
        }
        /// <summary>
        /// 点击菜单
        /// </summary>
        /// <param name="uiComponent"></param>
        /// <param name="menu"></param>
        /// <param name="value"></param>
        private void OnClick_Menu(UIComponent uiComponent, EFamilyConstruct menu, bool value)
        {
            if (value)
            {
                eFamilyMenu = menu;
                uiComponent?.Show();
                UIManager.HitButton(EUIID.UI_Family_Construct, "OnClick_Menu:"+ ((uint)eFamilyMenu).ToString());
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
        /// 更新家族信息
        /// </summary>
        private void OnUpdateFamilyInfo()
        {
            SetMenu(eFamilyMenu);
        }

        private void OnUpdateFamilyInfo(uint id, long value)
        {
            if (id != (uint)ECurrencyType.FamilyStamina)
            {
                return;
            }
            OnUpdateFamilyInfo();
        }
        #endregion
        #region 提供功能
        /// <summary>
        /// 设置菜单
        /// </summary>
        /// <param name="menu"></param>
        private void SetMenu(EFamilyConstruct menu)
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