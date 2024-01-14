using Logic.Core;
using System;
using UnityEngine;
using Framework;
using Lib.Core;
using System.Collections.Generic;
using Table;

namespace Logic
{
    public class Sys_MainMenu : SystemModuleBase<Sys_MainMenu>
    {
        #region 数据定义
        public enum EEvents
        {
            OnRefreshMenuRedPoint, //刷新主界面菜单红点
            OnRefreshFunctionRedPoint,
        }
        public enum Enum_RedPointUI
        {
            None,
            Knowledge = 4, //旅人志
            Adventure = 7, //冒险者手册
            Fashion = 10, //外观
            MagicBook = 17, //魔力宝典
            Awaken = 18, //觉醒
            Cook = 27, //烹饪
            Skill = 2, // 技能
            WarriorGroup = 29,  //勇者团
            TownTask = 30,  //城镇任务
            MerchantFleet=31,//法兰商队
            PetDometicate=32,//宠物驯养
        }

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public static RedPointNode MenuRedPoint = new RedPointNode();
        public static RedPointNode FunctionRedPoint_Fashion = new RedPointNode(MenuRedPoint);
        public static RedPointNode FunctionRedPoint_Knowledge = new RedPointNode(MenuRedPoint);
        public static RedPointNode FunctionRedPoint_MagicBook = new RedPointNode(MenuRedPoint);
        public static RedPointNode FunctionRedPoint_Awaken = new RedPointNode(MenuRedPoint);
        public static RedPointNode FunctionRedPoint_AdventureNum = new RedPointNode(); //冒险手册 计数红点 不显示于主界面红点
        public static RedPointNode FunctionRedPoint_Cook = new RedPointNode(MenuRedPoint);
        public static RedPointNode FunctionRedPoint_Skill = new RedPointNode(MenuRedPoint);
        public static RedPointNode FunctionRedPoint_TownTask = new RedPointNode(MenuRedPoint);
		public static RedPointNode FunctionRedPoint_WarriorGroup = new RedPointNode(MenuRedPoint);
        public static RedPointNode FunctionRedPoint_MerchantFleet = new RedPointNode(MenuRedPoint);
        public static RedPointNode FunctionRedPoint_PetDometicate = new RedPointNode(MenuRedPoint);
        public List<uint> list_ShowFunction = new List<uint>();
        #endregion

        #region 系统函数
        public override void Init()
        {
            list_ShowFunction.Clear();
            ProcessEvents(true);
            MenuRedPoint.OnChanged += UpdateMenuRedPoint;
        }

        public override void Dispose()
        {
            ProcessEvents(false);
            MenuRedPoint.OnChanged -= UpdateMenuRedPoint;
        }

        public override void OnLogout()
        {
            ClearRedPointData();
        }

        private void ProcessEvents(bool toRegister)
        {
            Sys_Adventure.Instance.eventEmitter.Handle(Sys_Adventure.EEvents.OnTipsNumUpdate, OnAdventureStateUpdate, toRegister);
            Sys_Knowledge.Instance.eventEmitter.Handle(Sys_Knowledge.EEvents.OnActiveKnowledge, CheckKnowledgeRedPoint, toRegister);
            Sys_MagicBook.Instance.eventEmitter.Handle(Sys_MagicBook.EEvents.OnNeedCheckItemRed, MagicBookRedState, toRegister);
            Sys_Fashion.Instance.eventEmitter.Handle(Sys_Fashion.EEvents.OnRefreshFreeDrawState, OnRefreshFreeDrawState, toRegister);
            Sys_TravellerAwakening.Instance.eventEmitter.Handle(Sys_TravellerAwakening.EEvents.OnAwakeRedPoint, OnAwakeRedPoint, toRegister);
            Sys_Cooking.Instance.eventEmitter.Handle(Sys_Cooking.EEvents.OnUpdateBookRightSubmitState, OnCookRedPoint, toRegister);
            
            #region 技能天赋
            Sys_Talent.Instance.eventEmitter.Handle(Sys_Talent.EEvents.OnLianghuaChanged, OnSkillRedPoint, toRegister);
            Sys_Talent.Instance.eventEmitter.Handle(Sys_Talent.EEvents.OnExchangeTalentPoint, OnSkillRedPoint, toRegister);
            #endregion
            Sys_MerchantFleet.Instance.eventEmitter.Handle(Sys_MerchantFleet.EEvents.UpdateMerchantInfo, OnUpdateMerchantInfo, toRegister);
            Sys_MerchantFleet.Instance.eventEmitter.Handle(Sys_MerchantFleet.EEvents.UpdateFamilyMerchantHelp, OnUpdateMerchantInfo, toRegister);
            // 道具个数变更
            // 道具变更 可能触发太频繁，有需要的话，可以屏蔽这里
            void __OnItemCountChanged(int _, int __) {
                OnSkillRedPoint();
            }

            void __OnPlayerLevelChanged() {
                OnSkillRedPoint();
            }
            
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateLevel, __OnPlayerLevelChanged, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<int, int>(Sys_Bag.EEvents.OnRefreshChangeData, __OnItemCountChanged, toRegister);
        }
        #endregion

        #region 功能函数
        public void InitMenuRedPoint()
        {
            FunctionRedPoint(Enum_RedPointUI.Knowledge);
            FunctionRedPoint(Enum_RedPointUI.Adventure);
            FunctionRedPoint(Enum_RedPointUI.Fashion);
            FunctionRedPoint(Enum_RedPointUI.MagicBook);
            FunctionRedPoint(Enum_RedPointUI.Awaken);
            FunctionRedPoint(Enum_RedPointUI.Cook);
            FunctionRedPoint(Enum_RedPointUI.Skill);
            FunctionRedPoint(Enum_RedPointUI.TownTask);
            FunctionRedPoint(Enum_RedPointUI.WarriorGroup);
            FunctionRedPoint(Enum_RedPointUI.PetDometicate);
        }

        public bool FunctionRedPoint(Enum_RedPointUI eRedPoint)
        {
            CSVBattleMenuFunction.Data data;
            bool functionOpen = false;
            if (CSVBattleMenuFunction.Instance.TryGetValue((uint)eRedPoint, out data))
                functionOpen = Sys_FunctionOpen.Instance.IsOpen(data.functionId, false);
            bool active = false;
            switch (eRedPoint)
            {
                case Enum_RedPointUI.Knowledge:
                    {
                        if (functionOpen)
                            active = Sys_Knowledge.Instance.IsRedPoint();
                        FunctionRedPoint_Knowledge.Value = active ? 1 : 0;
                    }
                    break;
                case Enum_RedPointUI.Adventure:
                    {
                        uint num = 0;
                        if (functionOpen)
                            num = Sys_Adventure.Instance.GetAllCheckNum();
                        FunctionRedPoint_AdventureNum.Value = (int)num;
                    }
                    break;
                case Enum_RedPointUI.Fashion:
                    {
                        if (functionOpen)
                        {
                            if (Sys_Fashion.Instance.activeId == 0)
                            {
                                active = false;
                            }
                            else
                            {
                                active = Sys_Fashion.Instance.freeDraw || Sys_Fashion.Instance.HasReward();
                            }
                        }
                        FunctionRedPoint_Fashion.Value = active ? 1 : 0;
                    }
                    break;
                case Enum_RedPointUI.MagicBook:
                    {
                        if (functionOpen)
                            active = Sys_MagicBook.Instance.CheckMagicBookAndTeachRedPoint();
                        FunctionRedPoint_MagicBook.Value = active ? 1 : 0;
                    }
                    break;
                case Enum_RedPointUI.Awaken:
                    {
                        if (functionOpen)
                            active = Sys_TravellerAwakening.Instance.CheckTarget();
                        FunctionRedPoint_Awaken.Value = active ? 1 : 0;
                    }
                    break;
                case Enum_RedPointUI.Cook:
                    {
                        if (functionOpen)
                            active = Sys_Cooking.Instance.HasSubmitItem(0) || Sys_Cooking.Instance.HasReward();
                        FunctionRedPoint_Awaken.Value = active ? 1 : 0;
                    }
                    break;
                case Enum_RedPointUI.Skill: {
                    if (functionOpen)
                        active = Sys_Talent.Instance.usingScheme != null && Sys_Talent.Instance.CanLianhua();
                    FunctionRedPoint_Skill.Value = active ? 1 : 0;
                }
                
                    break;
                case Enum_RedPointUI.WarriorGroup:
                    {
                        if (functionOpen)
                            active = Sys_WarriorGroup.Instance.HavingWarriorGroupRedPoint();
                        FunctionRedPoint_WarriorGroup.Value = active ? 1 : 0;
                    }
                    break;
                case Enum_RedPointUI.TownTask: {
                    if (functionOpen)
                        active = Sys_TownTask.Instance.CanAnyGot;
                    FunctionRedPoint_TownTask.Value = active ? 1 : 0;
                }
                    break;
                case Enum_RedPointUI.MerchantFleet:
                    {
                        if (functionOpen)
                            active = Sys_MerchantFleet.Instance.CheckMerchantFleetRedPoint();
                        FunctionRedPoint_TownTask.Value = active ? 1 : 0;
                    }
                    break;
                case Enum_RedPointUI.PetDometicate:
                    {
                        if (functionOpen)
                            active = Sys_PetDomesticate.Instance.CheckRedPoint();
                        FunctionRedPoint_PetDometicate.Value = active ? 1 : 0;
                    }
                    break;
                default:
                    return false;
            }
            return active;
        }

        public int GetFunctionRaw()
        {
            int count = list_ShowFunction.Count;
            int raw = count % 5 == 0 ? count / 5 : (count / 5) + 1;
            return raw;
        }

        private void ClearRedPointData()
        {
            FunctionRedPoint_Fashion.Value = 0;
            FunctionRedPoint_Knowledge.Value = 0;
            FunctionRedPoint_MagicBook.Value = 0;
            FunctionRedPoint_Awaken.Value = 0;
            FunctionRedPoint_AdventureNum.Value = 0;
            FunctionRedPoint_Cook.Value = 0;
            FunctionRedPoint_Skill.Value = 0;
            FunctionRedPoint_WarriorGroup.Value = 0;
            FunctionRedPoint_TownTask.Value = 0;
            FunctionRedPoint_PetDometicate.Value = 0;
        }

        #region 红点更新事件
        private void MagicBookRedState()
        {
            FunctionRedPoint(Enum_RedPointUI.MagicBook);
            eventEmitter.Trigger<Enum_RedPointUI>(EEvents.OnRefreshFunctionRedPoint, Enum_RedPointUI.MagicBook);
        }

        private void CheckKnowledgeRedPoint()
        {
            FunctionRedPoint(Enum_RedPointUI.Knowledge);
            eventEmitter.Trigger<Enum_RedPointUI>(EEvents.OnRefreshFunctionRedPoint, Enum_RedPointUI.Knowledge);
        }

        private void OnAdventureStateUpdate()
        {
            FunctionRedPoint(Enum_RedPointUI.Adventure);
            eventEmitter.Trigger<Enum_RedPointUI>(EEvents.OnRefreshFunctionRedPoint, Enum_RedPointUI.Adventure);
        }

        private void OnRefreshFreeDrawState()
        {
            FunctionRedPoint(Enum_RedPointUI.Fashion);
            eventEmitter.Trigger<Enum_RedPointUI>(EEvents.OnRefreshFunctionRedPoint, Enum_RedPointUI.Fashion);
        }

        private void OnAwakeRedPoint()
        {
            FunctionRedPoint(Enum_RedPointUI.Awaken);
            eventEmitter.Trigger<Enum_RedPointUI>(EEvents.OnRefreshFunctionRedPoint, Enum_RedPointUI.Awaken);
        }

        private void OnCookRedPoint()
        {
            FunctionRedPoint(Enum_RedPointUI.Cook);
            eventEmitter.Trigger<Enum_RedPointUI>(EEvents.OnRefreshFunctionRedPoint, Enum_RedPointUI.Cook);
        }

        private void OnSkillRedPoint()
        {
            FunctionRedPoint(Enum_RedPointUI.Skill);
            eventEmitter.Trigger<Enum_RedPointUI>(EEvents.OnRefreshFunctionRedPoint, Enum_RedPointUI.Skill);
        }
        private void OnUpdateMerchantInfo()
        {
            FunctionRedPoint(Enum_RedPointUI.MerchantFleet);
            eventEmitter.Trigger<Enum_RedPointUI>(EEvents.OnRefreshFunctionRedPoint, Enum_RedPointUI.MerchantFleet);
        }
        private void UpdateMenuRedPoint()
        {
            eventEmitter.Trigger(EEvents.OnRefreshMenuRedPoint);
        }
        
        #endregion      
        #endregion

    }
}