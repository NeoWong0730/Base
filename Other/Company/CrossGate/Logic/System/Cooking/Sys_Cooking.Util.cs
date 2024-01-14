using System.Collections;
using System.Collections.Generic;
using Table;
using Net;
using Packet;
using Lib.Core;
using UnityEngine;
using Logic.Core;
using static Packet.CmdCookPrepareConfirmNtf.Types;

namespace Logic
{
    public partial class Sys_Cooking : SystemModuleBase<Sys_Cooking>
    {
        private uint m_CookFunId;
        private uint cookFunId
        {
            get { return m_CookFunId; }
            set
            {
                if (m_CookFunId != value)
                {
                    m_CookFunId = value;
                    UpdateCookFun();
                }
            }
        }
        public List<uint> supportModels = new List<uint>();
        public List<uint> supportKitchens = new List<uint>();

        private int m_CurCookingLevel;
        public int curCookingLevel
        {
            get
            {
                return m_CurCookingLevel;
            }
            set
            {
                if (m_CurCookingLevel != value)
                {
                    m_CurCookingLevel = value;
                }
            }
        }


        public int maxCookingLevel;

        private void UpdateCookFun()
        {
            CSVCookFunction.Data cSVCookFunctionData = CSVCookFunction.Instance.GetConfData(m_CookFunId);
            if (cSVCookFunctionData != null)
            {
                supportModels = CSVCookFunction.Instance.GetConfData(m_CookFunId).allow_type;
                supportKitchens = CSVCookFunction.Instance.GetConfData(m_CookFunId).allow_tool;
            }
        }


        public void StartCooking(uint cookingFunId)
        {
            cookFunId = cookingFunId;
            UIManager.OpenUI(EUIID.UI_Cooking_Choose, false, cookingFunId);
        }

        public void UpdateAllCookingSubmitState()
        {
            for (int i = 0; i < cookings.Count; i++)
            {
                cookings[i].UpdateSubmitableItem();
            }
        }

        public bool HasSubmitItem(uint type)
        {
            bool has = false;
            if (type == 0)
            {
                for (int i = 0; i < cookings.Count; i++)
                {
                    if (cookings[i].CanSubmit())
                    {
                        has = true;
                        break;
                    }
                }
            }
            else if (type == 1)
            {
                for (int i = 0; i < type_1.Count; i++)
                {
                    if (type_1[i].CanSubmit())
                    {
                        has = true;
                        break;
                    }
                }
            }
            else if (type == 2)
            {
                for (int i = 0; i < type_2.Count; i++)
                {
                    if (type_2[i].CanSubmit())
                    {
                        has = true;
                        break;
                    }
                }
            }
            else if (type == 3)
            {
                for (int i = 0; i < type_3.Count; i++)
                {
                    if (type_3[i].CanSubmit())
                    {
                        has = true;
                        break;
                    }
                }
            }
            else if (type == 4)
            {
                for (int i = 0; i < type_4.Count; i++)
                {
                    if (type_4[i].CanSubmit())
                    {
                        has = true;
                        break;
                    }
                }
            }
            return has;
        }

        public CookStage GetCookStage(ulong roleId)
        {
            for (int i = 0; i < cookStages.Count; i++)
            {
                if (cookStages[i].RoleId == roleId)
                {
                    return cookStages[i];
                }
            }
            return null;
        }

        public string GetToolName(uint toolType)
        {
            string toolName = string.Empty;
            switch (toolType)
            {
                case 1:
                    toolName = LanguageHelper.GetTextContent(1003053);
                    break;
                case 2:
                    toolName = LanguageHelper.GetTextContent(1003054);
                    break;
                case 3:
                    toolName = LanguageHelper.GetTextContent(1003055);
                    break;
                case 4:
                    toolName = LanguageHelper.GetTextContent(1003056);
                    break;
                default:
                    break;
            }
            return toolName;
        }

        public uint GetToolIcon(uint toolType)
        {
            uint toolIcon = 0;
            switch (toolType)
            {
                case 1:
                    toolIcon = 320101;
                    break;
                case 2:
                    toolIcon = 320102;
                    break;
                case 3:
                    toolIcon = 320103;
                    break;
                case 4:
                    toolIcon = 320104;
                    break;
                default:
                    break;
            }
            return toolIcon;
        }

        public uint GetToolIcon2(uint toolType)
        {
            uint toolIcon = 0;
            switch (toolType)
            {
                case 1:
                    toolIcon = 320105;
                    break;
                case 2:
                    toolIcon = 320106;
                    break;
                case 3:
                    toolIcon = 320107;
                    break;
                case 4:
                    toolIcon = 320108;
                    break;
                default:
                    break;
            }
            return toolIcon;
        }


        public float GetCookTime(uint toolId)
        {
            float cookTime = 0;
            CSVCookAttr.Data cSVCookAttrData = null;
            if (toolId == 1)
            {
                cSVCookAttrData = CSVCookAttr.Instance.GetConfData(1);
            }
            else if (toolId == 2)
            {
                cSVCookAttrData = CSVCookAttr.Instance.GetConfData(2);
            }
            else if (toolId == 3)
            {
                cSVCookAttrData = CSVCookAttr.Instance.GetConfData(3);
            }
            else if (toolId == 4)
            {
                cSVCookAttrData = CSVCookAttr.Instance.GetConfData(4);
            }
            if (cSVCookAttrData != null)
            {
                cookTime = cSVCookAttrData.value;
            }
            return cookTime;
        }

        public void UpdateCookingPrepareMember()
        {
            roles.Clear();
            for (int i = 0; i < m_CmdCookPrepareNtf.Mems.Count; i++)
            {
                roles.Add(m_CmdCookPrepareNtf.Mems[i].RoleId);
            }
            bIsCookingCaptain = Sys_Role.Instance.RoleId == roles[0];
        }

        public void UpdateCookingMember()
        {
            roles.Clear();
            for (int i = 0; i < m_CmdCookPrepareConfirmNtf.Mems.Count; i++)
            {
                roles.Add(m_CmdCookPrepareConfirmNtf.Mems[i].RoleId);
            }
            bIsCookingCaptain = Sys_Role.Instance.RoleId == roles[0];
        }

        public Cooking GetCooking(uint id)
        {
            if (m_CookingMap.TryGetValue(id, out Cooking cooking) && cooking != null)
            {
                return cooking;
            }
            return null;
        }

        /// <summary>
        /// 更新奖励排序
        /// </summary>
        public void UpdateRewards()
        {
            showRewards.Clear();
            m_ShowRewardsMap.Clear();
            List<uint> unGetRewards = new List<uint>();
            for (int i = 0; i < m_ConfigRewards.Count; i++)
            {
                if (!m_RewardsGet.Contains(m_ConfigRewards[i]))
                {
                    unGetRewards.Add(m_ConfigRewards[i]);
                }
            }
            if (unGetRewards.Count > 0)
            {
                unGetRewards.Sort();
                for (int i = 0; i < unGetRewards.Count; i++)
                {
                    showRewards.Add(unGetRewards[i]);
                    m_ShowRewardsMap.Add(unGetRewards[i], false);
                }
            }
            if (m_RewardsGet.Count > 0)
            {
                m_RewardsGet.Sort();
                for (int i = 0; i < m_RewardsGet.Count; i++)
                {
                    showRewards.Add(m_RewardsGet[i]);
                    m_ShowRewardsMap.Add(m_RewardsGet[i], true);
                }
            }
        }

        /// <summary>
        /// state 0 可领取 1 已领取 -1不能领取
        /// </summary>
        /// <param name="state"></param>
        /// <param name="index"></param>
        public void GetRewardState(out int state, uint rewardId)
        {
            state = 0;
            if (!m_ShowRewardsMap.ContainsKey(rewardId))
            {
                state = -1;
                return;
            }
            bool get = m_ShowRewardsMap[rewardId];
            if (get)
            {
                state = 1;
            }
            else
            {
                CSVCookBookReward.Data cSVFoodBookRewardData = CSVCookBookReward.Instance.GetConfData(rewardId);
                if (null != cSVFoodBookRewardData)
                {
                    uint needScore = cSVFoodBookRewardData.score;
                    state = curScore >= needScore ? 0 : -1;
                }
                else
                {
                    DebugUtil.LogFormat(ELogType.eCooking, $"找不到烹饪奖励{rewardId}");
                }
            }
        }

        /// <summary>
        /// 计算 当前积分 小于 积分列表的哪个位置
        /// </summary>
        /// <param name="unGetPointIndex"></param>
        public void GetTargetScore(out uint targetScore)
        {
            targetScore = 0;
            for (int i = 0; i < m_ConfigRewards.Count; i++)
            {
                CSVCookBookReward.Data cSVFoodBookRewardData = CSVCookBookReward.Instance.GetConfData(m_ConfigRewards[i]);
                targetScore = cSVFoodBookRewardData.score;
                if (curScore <= cSVFoodBookRewardData.score)
                    return;
            }
        }

        public bool HasReward()
        {
            bool has = false;
            foreach (var item in CSVCookBookReward.Instance.GetAll())
            {
                GetRewardState(out int state, item.id);
                if (state == 0)
                {
                    has = true;
                    break;
                }
            }
            return has;
        }

        public bool GetAllReward()
        {
            bool get = true;
            foreach (var item in CSVCookBookReward.Instance.GetAll())
            {
                GetRewardState(out int state, item.id);
                if (state != 1)
                {
                    get = false;
                    break;
                }
            }
            return get;
        }


        public void SortCooking()
        {
            cookings.Sort(Compare);
            type_1.Clear();
            type_2.Clear();
            type_3.Clear();
            type_4.Clear();
            cookings_Special.Clear();
            type_1_Special.Clear();
            type_2_Special.Clear();
            type_3_Special.Clear();
            type_4_Special.Clear();

            for (int i = 0; i < cookings.Count; i++)
            {
                if (cookings[i].cSVCookData.is_special)
                {
                    cookings_Special.Add(cookings[i]);
                }

                if (cookings[i].foodType == 1)
                {
                    type_1.Add(cookings[i]);
                    if (cookings[i].cSVCookData.is_special)
                    {
                        type_1_Special.Add(cookings[i]);
                    }
                }
                if (cookings[i].foodType == 2)
                {
                    type_2.Add(cookings[i]);
                    if (cookings[i].cSVCookData.is_special)
                    {
                        type_2_Special.Add(cookings[i]);
                    }
                }
                if (cookings[i].foodType == 3)
                {
                    type_3.Add(cookings[i]);
                    if (cookings[i].cSVCookData.is_special)
                    {
                        type_3_Special.Add(cookings[i]);
                    }
                }
                if (cookings[i].foodType == 4)
                {
                    type_4.Add(cookings[i]);
                    if (cookings[i].cSVCookData.is_special)
                    {
                        type_4_Special.Add(cookings[i]);
                    }
                }
            }
        }

        private int Compare(Cooking cook1, Cooking cook2)
        {
            if (cook1.active < cook2.active)
            {
                return 1;
            }
            else if (cook1.active > cook2.active)
            {
                return -1;
            }
            else
            {
                if (cook1.watch && !cook2.watch)
                {
                    return -1;
                }
                else if (!cook1.watch && cook2.watch)
                {
                    return 1;
                }
                else
                {
                    if (cook1.sort < cook2.sort)
                    {
                        return -1;
                    }
                    else if (cook1.sort > cook2.sort)
                    {
                        return 1;
                    }
                    else
                    {
                        if (cook1.id < cook2.id)
                        {
                            return -1;
                        }
                        else
                        {
                            return 1;
                        }
                    }
                }
            }
        }

        public uint CalFree1CookId(uint tool, List<uint> items)
        {
            uint cookId = 0;
            foreach (var item in CSVCook.Instance.GetAll())
            {
                Cooking cooking = GetCooking(item.id);
                if (cooking != null)
                {
                    if (cooking.CanMake(tool, 1, items))
                    {
                        cookId = cooking.id;
                        break;
                    }
                }
            }
            return cookId;
        }

        public uint CalFree2CookId(uint too11, List<uint> items1, uint tool2, List<uint> items2)
        {
            uint cookId = 0;
            foreach (var item in CSVCook.Instance.GetAll())
            {
                Cooking cooking = GetCooking(item.id);
                if (cooking != null)
                {
                    if (cooking.CanMake(too11, 1, items1) && cooking.CanMake(tool2, 2, items2))
                    {
                        cookId = cooking.id;
                        break;
                    }
                }
            }
            return cookId;
        }

        //5910 多阶段;一阶段 5911 多阶段 :二阶段 5908 单阶段
        public void FoodNotEnough(int stage)
        {
            uint id = 0;
            if (stage == 1)
            {
                id = 5910;
            }
            else if (stage == 2)
            {
                id = 5911;
            }
            else
            {
                id = 5908;
            }
            string content = LanguageHelper.GetTextContent(id);
            Sys_Hint.Instance.PushContent_Normal(content);
        }

        public void KitchenNotMatch()
        {
            string content = LanguageHelper.GetTextContent(5909);
            Sys_Hint.Instance.PushContent_Normal(content);
        }

        public void KitchenNotSupport()
        {
            string content = LanguageHelper.GetTextContent(5905);
            Sys_Hint.Instance.PushContent_Normal(content);
        }

        public void FixKitchen()
        {
            string content = LanguageHelper.GetTextContent(5919);
            Sys_Hint.Instance.PushContent_Normal(content);
        }

        public bool HasRecipeActive()
        {
            bool has = false;
            for (int i = 0; i < cookings.Count; i++)
            {
                if (cookings[i].active == 1)
                {
                    has = true;
                    break;
                }
            }
            return has;
        }

        public void CancelCooking()
        {
            if (UIManager.IsOpen(EUIID.UI_Cooking_Choose))
            {
                UIManager.CloseUI(EUIID.UI_Cooking_Choose, true);
            }
            if (UIManager.IsOpen(EUIID.UI_Cooking_Single))
            {
                UIManager.CloseUI(EUIID.UI_Cooking_Single, true);
            }
            if (UIManager.IsOpen(EUIID.UI_Cooking_Multiple))
            {
                UIManager.CloseUI(EUIID.UI_Cooking_Multiple, true);
            }
            if (UIManager.IsOpen(EUIID.UI_Cooking))
            {
                UIManager.CloseUI(EUIID.UI_Cooking);
            }
        }

        public void UpdateCurCookingLevel()
        {
            if (curScore == 0)
            {
                curCookingLevel = 1;
                return;
            }
            bool reachMax = true;
            for (int i = 0; i < CSVCookLv.Instance.Count; i++)
            {
                CSVCookLv.Data cSVCookLvData = CSVCookLv.Instance.GetByIndex(i);
                if (curScore < cSVCookLvData.need_score)
                {
                    reachMax = false;
                    curCookingLevel = (int)cSVCookLvData.id - 1;
                    break;
                }
            }
            if (m_CurCookingLevel < 1)
            {
                curCookingLevel = 1;
            }
            if (reachMax)
            {
                curCookingLevel = CSVCookLv.Instance.Count;
            }
        }

        public bool Free1StageValid()
        {
            CSVCookLv.Data cSVCookLvData = CSVCookLv.Instance.GetConfData((uint)m_CurCookingLevel);
            return cSVCookLvData.free_cook;
        }

        public bool Free2StageValid()
        {
            CSVCookLv.Data cSVCookLvData = CSVCookLv.Instance.GetConfData((uint)m_CurCookingLevel);
            return cSVCookLvData.stage2_cook;
        }

        public bool Free3StageValid()
        {
            CSVCookLv.Data cSVCookLvData = CSVCookLv.Instance.GetConfData((uint)m_CurCookingLevel);
            return cSVCookLvData.multi_cook;
        }

        //若玩家烹饪等级不足无法使用该厨具时，无法切换厨具，点击飘字提示“需要烹饪等级达到“厨师”才能解锁该厨具”（语言表id：[新增限制提示]5934）
        public bool ToolValid(uint toolId)
        {
            CSVCookLv.Data cSVCookLvData = CSVCookLv.Instance.GetConfData((uint)m_CurCookingLevel);
            bool valid = cSVCookLvData.allow_tool.Contains(toolId);
            if (!valid)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5934)); ;
            }
            return valid;
        }
        /// <summary>
        /// 检测是否解锁批量料理
        /// </summary>
        public bool CheckBatchCookingIsOpen()
        {
            var cData = CSVCookLv.Instance.GetConfData((uint)m_CurCookingLevel);
            if (!cData.batch_cook)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5940)); 
                return false;
            }
            return true;
        }
    }
}


