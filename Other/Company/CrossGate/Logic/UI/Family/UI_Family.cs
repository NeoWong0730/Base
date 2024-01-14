using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using Lib.Core;
using System;
using System.Text;

namespace Logic
{
    /// <summary> 家族主界面打开参数 </summary>
    public class UI_FamilyOpenParam
    {
        public uint familyMenuEnum;
        public uint activeId;
    }
    /// <summary> 家族主界面 </summary>
    public class UI_Family : UIBase
    {
        #region 界面组件
        /// <summary> 货币标题通用界面 </summary>
        private UI_CurrencyTitle ui_CurrencyTitle;
        /// <summary> 菜单列表 </summary>
        private List<Toggle> list_Menu = new List<Toggle>();
        /// <summary> 子界面列表 </summary>
        private List<UIComponent> list_ChildView = new List<UIComponent>();
        private Toggle familyPVP;
        /// <summary> 当前子界面 </summary>
        private UIComponent curChildView { set; get; }
        /// <summary> 家族红点 </summary>
        private UI_Family_RedPoint redPoint;
        /// <summary> 家族主界面打开参数 </summary>
        private UI_FamilyOpenParam openParam;
        #endregion
        #region 数据定义
        /// <summary> 家族菜单 </summary>
        public enum EFamilyMenu
        {
            Hall = 0, //大厅
            Member = 1, //成员
            Building = 2, //建筑
            Activity = 3, //活动
            FamilyPVP = 4, //家族pvp
        }
        /// <summary> 当前菜单 </summary>
        private EFamilyMenu eFamilyMenu;
        /// <summary> 菜单改变事件 </summary>
        public UnityEngine.Events.UnityAction<bool> onValueChanged;
        /// <summary> 计时器 </summary>
        private List<Timer> list_Timers = new List<Timer>();
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
            ui_CurrencyTitle?.Dispose();
            for (int i = 0, count = list_ChildView.Count; i < count; i++)
            {
                var x = list_ChildView[i];
                x?.Hide();
                x?.OnDestroy();
            }
        }
        protected override void OnOpen(object arg)
        {
            if (null != arg)
            {
                if (arg.GetType() == typeof(UI_FamilyOpenParam))
                {
                    openParam = arg as UI_FamilyOpenParam;
                    eFamilyMenu = (EFamilyMenu)openParam.familyMenuEnum;
                }
                else if (arg.GetType() == typeof(uint))
                {
                    eFamilyMenu = (EFamilyMenu)Convert.ToUInt32(arg);
                }
            }
            else
            {
                eFamilyMenu = EFamilyMenu.Hall;
            }
            Sys_Family.Instance.OnUpdateGuildSceneInfo();
        }
        protected override void OnOpened()
        {
            Sys_Family.Instance.SendGuildGetGuildInfoReq();
            Sys_Family.Instance.GuildPetGetInfoReq();
            for (int i = 0, count = list_ChildView.Count; i < count; i++)
            {
                var x = list_ChildView[i];
                x?.Reset();
            } 
        }
        protected override void OnShow()
        {
            ui_CurrencyTitle?.SetData(new List<uint>() { 1,2,3},20);

            // 暂时只在show的时候刷新，不监听条件
            var familyFightList = Sys_Family.Instance.GetFamilyFightDataList();
            familyPVP.gameObject.SetActive(familyFightList.Count > 0);

            if (eFamilyMenu == EFamilyMenu.Hall)
            {
                curChildView?.OnRefresh();
            }
        }

        protected override void OnShowEnd()
        {
            if (Sys_Family.Instance.needShowFraudTip)
            {
                Sys_Family.Instance.needShowFraudTip = false;
                UIManager.OpenUI(EUIID.UI_Family_Sure, parentID: EUIID.UI_Family);
            }
        }
        protected override void OnHide()
        {
            ClearTimers();
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
            var values = System.Enum.GetValues(typeof(EFamilyMenu));
            for (int i = 0, count = values.Length; i < count; i++)
            {
                EFamilyMenu type = (EFamilyMenu)values.GetValue(i);
                UIComponent uiComponent = null;
                Toggle toggle = null;
                switch (type)
                {
                    case EFamilyMenu.Hall:
                        {
                            toggle = transform.Find("Animator/Toggle_Tab/Toggle_Hall").GetComponent<Toggle>();
                            uiComponent = AddComponent<UI_Family_Hall>(transform.Find("Animator/View_Hall"));
                        }
                        break;
                    case EFamilyMenu.Member:
                        {
                            toggle = transform.Find("Animator/Toggle_Tab/Toggle_Member").GetComponent<Toggle>();
                            uiComponent = AddComponent<UI_Family_Member>(transform.Find("Animator/View_Member"));
                        }
                        break;
                    case EFamilyMenu.Building:
                        {
                            toggle = transform.Find("Animator/Toggle_Tab/Toggle_Build").GetComponent<Toggle>();
                            uiComponent = AddComponent<UI_Family_Building>(transform.Find("Animator/View_Build"));
                        }
                        break;
                    case EFamilyMenu.Activity:
                        {
                            toggle = transform.Find("Animator/Toggle_Tab/Toggle_Activity").GetComponent<Toggle>();
                            uiComponent = AddComponent<UI_Family_Activity>(transform.Find("Animator/View_Activity"));
                        }
                        break;
                    case EFamilyMenu.FamilyPVP: {
                            familyPVP = toggle = transform.Find("Animator/Toggle_Tab/Toggle_PVPActivity").GetComponent<Toggle>();
                            uiComponent = AddComponent<UI_Family_PVP>(transform.Find("Animator/View_PVPActivity"));
                        }
                        break;
                }
                if (eFamilyMenu == type)
                    toggle.SetIsOnWithoutNotify(true);//默认prefab上菜单显示不正确，需要强制刷新下。 
                toggle.onValueChanged.AddListener((bool value) => OnClick_Menu(uiComponent, type, value));
                list_Menu.Add(toggle);
                list_ChildView.Add(uiComponent);
            }
            ui_CurrencyTitle = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);
            redPoint = gameObject.AddComponent<UI_Family_RedPoint>();
            redPoint?.Init(this);

            transform.Find("Animator/View_Title08/Btn_Close").GetComponent<Button>().onClick.AddListener(OnClick_Close);
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateFamilyInfo, OnUpdateFamilyInfo, toRegister);
            //Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.QuitFamily, OnClick_Close, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateBuildState, OnUpdateBuildState, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateBuildSkillState, OnUpdateBuildSkillState, toRegister);
            Sys_Time.Instance.eventEmitter.Handle<uint, uint>(Sys_Time.EEvents.OnTimeNtf, OnTimeNtf, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateMergeResult, OnMergeFamilyResult, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateApplyMergedList, OnMergeFamilyResult, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.OnAuctionAckEnd, OnUpdateFamilyInfo, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.FamilyPetGetUIEvent, GetFamilyCreatureEvent, toRegister);
            
        }
        /// <summary>
        /// 设置定时器
        /// </summary>
        private void SetTimers()
        {
            ClearTimers();
            if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return;
            var info = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info;
            var allBuilds = info.AllBuildings;

            if (info.NowUpgrade > 0)
            {
                float upgradeTime = Sys_Family.Instance.GetBuildUpgradeResidueTime(info.UpgradeFinishTime);

                Timer timer = Timer.Register(upgradeTime, () =>
                {
                    Sys_Family.Instance.SendGuildGetGuildInfoReq();
                });
                list_Timers.Add(timer);
            }

            for (int i = 0; i < allBuilds.Count; i++)
            {
                var building = allBuilds[i];
                CSVFamilySkillUp.Data cSVFamilySkillUpData = CSVFamilySkillUp.Instance.GetConfData((uint)building.NowUpgrade);
                if (null == cSVFamilySkillUpData) continue;

                float upgradeTime = Sys_Family.Instance.GetBuildUpgradeResidueTime(cSVFamilySkillUpData.UpgradeTime);
                float time = Sys_Family.Instance.GetBuildUpgradeTotalTime(building.UpgradeFinishTime);

                Timer timer = Timer.Register(upgradeTime, () =>
                {
                    Sys_Family.Instance.SendGuildGetGuildInfoReq();
                });
                list_Timers.Add(timer);
            }
        }
        /// <summary>
        /// 清理定时器
        /// </summary>
        private void ClearTimers()
        {
            for (int i = 0; i < list_Timers.Count; i++)
            {
                var timer = list_Timers[i];
                timer?.Cancel();
                timer = null;
            }
            list_Timers.Clear();
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
            UIManager.HitButton(EUIID.UI_Family, "OnClick_Close");
            CloseSelf();
        }
        /// <summary>
        /// 点击菜单
        /// </summary>
        /// <param name="uiComponent"></param>
        /// <param name="menu"></param>
        /// <param name="value"></param>
        private void OnClick_Menu(UIComponent uiComponent, EFamilyMenu menu, bool value)
        {
            if (value)
            {
                StringBuilder btnEventStr = StringBuilderPool.GetTemporary();
                btnEventStr.Append("OnClick_Menu:");
                btnEventStr.Append(((int)menu).ToString());
                UIManager.HitButton(EUIID.UI_Family, StringBuilderPool.ReleaseTemporaryAndToString(btnEventStr));
                eFamilyMenu = menu;
                uiComponent?.Show();
                if(null != openParam && menu == EFamilyMenu.Activity)
                {
                    uiComponent.SetData(openParam.activeId);
                    openParam = null;
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
        private void GetFamilyCreatureEvent()
        {
            openParam = new UI_FamilyOpenParam();
            openParam.activeId = 60;
            eFamilyMenu = EFamilyMenu.Activity;
            OnUpdateFamilyInfo();
        }
        private void OnMergeFamilyResult()
        {
            Sys_Family.Instance.SendFamilyInfo();
        }

        /// <summary>
        /// 更新家族信息
        /// </summary>
        private void OnUpdateFamilyInfo()
        {
            SetMenu(eFamilyMenu);
            SetTimers();
        }
        /// <summary>
        /// 更新建筑状态
        /// </summary>
        private void OnUpdateBuildState()
        {
            SetTimers();
        }
        /// <summary>
        /// 更改建筑技能状态
        /// </summary>
        private void OnUpdateBuildSkillState()
        {
            SetTimers();
        }
        /// <summary>
        /// 时间刷新
        /// </summary>
        private void OnTimeNtf(uint oldTime, uint newTime)
        {
            SetTimers();
        }
        #endregion
        #region 提供功能
        /// <summary>
        /// 设置菜单
        /// </summary>
        /// <param name="menu"></param>
        private void SetMenu(EFamilyMenu menu)
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