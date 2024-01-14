using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using Net;
using Packet;

namespace Logic
{
    /// <summary>
    /// 方案系统///
    /// </summary>
    public class Sys_Plan : SystemModuleBase<Sys_Plan>
    {
        /// <summary>
        /// 方案类型///
        /// </summary>
        public enum EPlanType
        {
            None = 0,
            Family, //家族历练///
            PetAttribute,  //宠物加点///
            PetAttributeCorrect,    //宠物属性修正           
            RoleAttribute,   //人物加点///
            Talent, //天赋///
            Partner,    //伙伴///
            TransfigurationStudy,    //种族研究
        }

        /// <summary>
        /// 方案类///
        /// </summary>
        public class Plan
        {
            /// <summary>
            /// 类型///
            /// </summary>
            public EPlanType PlanType
            {
                get;
                private set;
            }

            /// <summary>
            /// 方案序号///
            /// </summary>
            public uint Index
            {
                get;
                private set;
            }

            /// <summary>
            /// 方案名///
            /// </summary>
            public string Name
            {
                get;
                private set;
            }

            public Plan(EPlanType _planType, uint _index)
            {
                PlanType = _planType;
                Index = _index;

                if (PlanType == EPlanType.RoleAttribute)
                {
                    Name = LanguageHelper.GetTextContent(10013503, (Index + 1).ToString());
                }
                else if (PlanType == EPlanType.Family)
                {
                    Name = LanguageHelper.GetTextContent(10013502, (Index + 1).ToString());
                }
                else if (PlanType == EPlanType.Talent)
                {
                    Name = LanguageHelper.GetTextContent(10013501, (Index + 1).ToString());
                }
                else if (PlanType == EPlanType.PetAttribute)
                {
                    Name = LanguageHelper.GetTextContent(10013504, (Index + 1).ToString());
                }
                else if (PlanType == EPlanType.PetAttributeCorrect)
                {
                    Name = LanguageHelper.GetTextContent(10013505, (Index + 1).ToString());
                }                
                else
                {
                    Name = string.Empty;
                }
            }

            /// <summary>
            /// 改变方案名///
            /// </summary>
            /// <param name="_name"></param>
            /// <returns></returns>
            public Plan SetName(string _name)
            {
                Name = _name;
                return this;
            }

            public static string GetUITitleStr(EPlanType planType)
            {
                string str = string.Empty;
                if (planType == EPlanType.RoleAttribute)
                    return LanguageHelper.GetTextContent(10013601);
                else if (planType == EPlanType.Family)
                    return LanguageHelper.GetTextContent(10013602);
                else if (planType == EPlanType.Talent)
                    return LanguageHelper.GetTextContent(10013603);
                else if (planType == EPlanType.PetAttribute)
                    return LanguageHelper.GetTextContent(10013604);
                else if (planType == EPlanType.PetAttributeCorrect)
                    return LanguageHelper.GetTextContent(10013605);
                else if (planType == EPlanType.Partner)
                    return LanguageHelper.GetTextContent(10013600);

                return str;
            }
        }

        /// <summary>
        /// 事件///
        /// </summary>
        public enum EEvents
        {
            AddNewPlan, //添加一个新方案///
            ChangePlanName, //重命名方案///
            ChangePlan, //改变方案///
            OnChangePlanSuccess,    //改变方案成功///
            OnChangeFightPet, //战宠被换///


            OnClickPartner,
        }

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        /// <summary>
        /// 所有方案集合///
        /// </summary>
        public Dictionary<uint, Dictionary<uint, Plan>> allPlans = new Dictionary<uint, Dictionary<uint, Plan>>();

        /// <summary>
        /// 当前选中的方案///
        /// </summary>
        public Dictionary<uint, uint> curPlanIndexs = new Dictionary<uint, uint>();

        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdRole.AttrPlanNtf, OnAttrPlanNtf, CmdRoleAttrPlanNtf.Parser);
            eventEmitter.Handle<uint, uint>(EEvents.AddNewPlan, OnAddNewPlan, true);
            eventEmitter.Handle<uint, uint, string>(EEvents.ChangePlanName, OnChangePlanName, true);
            eventEmitter.Handle<uint, uint>(EEvents.OnChangePlanSuccess, OnChangePlanSuccess, true);
            eventEmitter.Handle<ulong, uint, uint>(EEvents.OnChangeFightPet, OnChangeFightPet, true);
        }

        public override void OnLogout()
        {
            allPlans.Clear();
            curPlanIndexs.Clear();
        }

        /// <summary>
        /// 数据初始化///
        /// </summary>
        /// <param name="msg"></param>
        void OnAttrPlanNtf(NetMsg msg)
        {
            CmdRoleAttrPlanNtf ntf = NetMsgUtil.Deserialize<CmdRoleAttrPlanNtf>(CmdRoleAttrPlanNtf.Parser, msg);
            if (ntf != null)
            {
                for (int index = 0, len = ntf.Datas.Count; index < len; index++)
                {
                    Dictionary<uint, Plan> plans = new Dictionary<uint, Plan>();
                    for (int index2 = 0, len2 = ntf.Datas[index].Plans.Count; index2 < len2; index2++)
                    {
                        plans[ntf.Datas[index].Plans[index2].Index] = new Plan((EPlanType)ntf.Datas[index].Type, ntf.Datas[index].Plans[index2].Index);
                        if (!string.IsNullOrEmpty(ntf.Datas[index].Plans[index2].PlanName.ToStringUtf8()))
                        {
                            plans[ntf.Datas[index].Plans[index2].Index].SetName(ntf.Datas[index].Plans[index2].PlanName.ToStringUtf8());
                        }
                    }
                    allPlans[ntf.Datas[index].Type] = plans;
                    curPlanIndexs[ntf.Datas[index].Type] = ntf.Datas[index].SelectIndex;
                }
            }
        }

        /// <summary>
        /// 添加一个新方案///
        /// </summary>
        /// <param name="planType"></param>
        /// <param name="index"></param>
        /// <param name="name"></param>
        void OnAddNewPlan(uint planType, uint index)
        {
            Plan plan = new Plan((EPlanType)planType, index);
            if (allPlans.ContainsKey(planType))
            {
                allPlans[planType][index] = plan;
            }
            else
            {
                Dictionary<uint, Plan> plans = new Dictionary<uint, Plan>();
                plans[index] = plan;
                allPlans[planType] = plans;
            }
        }

        /// <summary>
        /// 改变方案名///
        /// </summary>
        /// <param name="planType"></param>
        /// <param name="index"></param>
        /// <param name="name"></param>
        void OnChangePlanName(uint planType, uint index, string name)
        {
            if (allPlans.ContainsKey(planType))
            {
                if (allPlans[planType].ContainsKey(index))
                {
                    allPlans[planType][index].SetName(name);
                }
                else
                {
                    DebugUtil.LogError($"allPlans[{planType}] didn't contain key {index}");
                }
            }
            else
            {
                DebugUtil.LogError($"allPlans didn't contain key {planType}");
            }
        }

        /// <summary>
        /// 改变方案///
        /// </summary>
        /// <param name="planType"></param>
        /// <param name="index"></param>
        public void ChangePlan(EPlanType planType, uint index)
        {
            eventEmitter.Trigger<uint, uint>(EEvents.ChangePlan, (uint)planType, index);
        }

        /// <summary>
        /// 改变方案成功///
        /// </summary>
        /// <param name="planType"></param>
        /// <param name="index"></param>
        public void OnChangePlanSuccess(uint planType, uint index)
        {
            if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Fight)
            {
                if (planType == (uint)EPlanType.Talent)
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10013801));
                if (planType == (uint)EPlanType.Family)
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10013802));
                if (planType == (uint)EPlanType.RoleAttribute)
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10013803));
                if (planType == (uint)EPlanType.PetAttribute)
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10013804));
                if (planType == (uint)EPlanType.PetAttributeCorrect)
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10013805));
                if (planType == (uint)EPlanType.Partner)
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10013811));
            }
            curPlanIndexs[planType] = index;
        }

        /// <summary>
        /// 战宠被换///
        /// </summary>
        /// <param name="petID"></param>
        /// <param name="planIndex"></param>
        public void OnChangeFightPet(ulong petID, uint petAttributeIndex, uint PetAttributeCorrectIndex)
        {
            if (petID != 0)
            {
                PetPointPlansData petPointPlansData = Sys_Pet.Instance.GetPetAddPointPlanDataByUid(petID);
                Dictionary<uint, Plan> plans = new Dictionary<uint, Plan>();
                for (int index = 0, len = petPointPlansData.Plans.Count; index < len; index++)
                {
                    plans[(uint)index] = new Plan(EPlanType.PetAttribute, (uint)index);
                    if (!string.IsNullOrEmpty(petPointPlansData.Plans[index].PlanName.ToStringUtf8()))
                    {
                        plans[(uint)index].SetName(petPointPlansData.Plans[index].PlanName.ToStringUtf8());
                    }
                }
                allPlans[(uint)EPlanType.PetAttribute] = plans;

                curPlanIndexs[(uint)EPlanType.PetAttribute] = petAttributeIndex;

                EnhancePlansData enhancePlansData = Sys_Pet.Instance.GetPetEnHanPointPlanDataByUid(petID);
                Dictionary<uint, Plan> plans2 = new Dictionary<uint, Plan>();
                for (int index = 0, len = enhancePlansData.Plans.Count; index < len; index++)
                {
                    plans2[(uint)index] = new Plan(EPlanType.PetAttributeCorrect, (uint)index);
                    if (!string.IsNullOrEmpty(enhancePlansData.Plans[index].PlanName.ToStringUtf8()))
                    {
                        plans2[(uint)index].SetName(enhancePlansData.Plans[index].PlanName.ToStringUtf8());
                    }
                }
                allPlans[(uint)EPlanType.PetAttributeCorrect] = plans2;

                curPlanIndexs[(uint)EPlanType.PetAttributeCorrect] = PetAttributeCorrectIndex;
            }         
            else
            {
                if (allPlans.ContainsKey((uint)EPlanType.PetAttribute))
                    allPlans[(uint)EPlanType.PetAttribute].Clear();
                if (allPlans.ContainsKey((uint)EPlanType.PetAttributeCorrect))
                    allPlans[(uint)EPlanType.PetAttributeCorrect].Clear();
                if (curPlanIndexs.ContainsKey((uint)EPlanType.PetAttribute))
                    curPlanIndexs[(uint)EPlanType.PetAttribute] = 0;
                if (curPlanIndexs.ContainsKey((uint)EPlanType.PetAttributeCorrect))
                    curPlanIndexs[(uint)EPlanType.PetAttributeCorrect] = 0;
            }
        }
    }
}
