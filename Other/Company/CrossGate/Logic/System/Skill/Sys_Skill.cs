using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using Table;
using Net;
using Packet;

namespace Logic
{
    public class Sys_Skill : SystemModuleBase<Sys_Skill>
    {
        public const uint EACHLEVELRANKNUM = 10;

        /// <summary>
        /// 技能类型///
        /// </summary>
        public enum ESkillType
        {
            /// <summary>
            /// 主动技能///
            /// </summary>
            Active,

            /// <summary>
            /// 被动技能///
            /// </summary>
            Passive,
        }

        /// <summary>
        /// 技能副类型///
        /// </summary>
        public enum ESkillSubType
        {
            /// <summary>
            /// 得意技能///
            /// </summary>
            Best,

            /// <summary>
            /// 通用技能///
            /// </summary>
            Common,
        }

        public class SkillInfo
        {
            /// <summary>
            /// 技能ID///
            /// </summary>
            public uint SkillID
            {
                get;
                set;
            }

            /// <summary>
            /// 当前熟练度///
            /// </summary>
            public uint CurProficiency
            {
                get;
                set;
            }

            /// <summary>
            /// 当前技能阶级///
            /// </summary>
            public uint Level
            {
                get;
                set;
            }

            /// <summary>
            /// 当前技能等级///
            /// </summary>
            public uint Rank
            {
                get;
                set;
            }

            /// <summary>
            /// 技能类型///
            /// </summary>
            public ESkillType ESkillType
            {
                get;
                set;
            }

            public ESkillSubType ESkillSubType
            {
                get;
                set;
            }

            /// <summary>
            /// 技能可升到的最高阶级
            /// </summary>
            public uint MaxLevel
            {
                get;
                set;
            }

            public List<uint> occUpgradeLimits = new List<uint>();

            public int SortIndex
            {
                get;
                set;
            }

            /// <summary>
            /// 当前阶级等级对应的表ID///
            /// </summary>
            public uint CurInfoID
            {
                get
                {
                    if (Level > 0)
                    {
                        return SkillID * 1000 + (Level - 1) * EACHLEVELRANKNUM + Rank;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }

            /// <summary>
            /// 第一阶第一级的ID///
            /// </summary>
            public uint FirstInfoID
            {
                get
                {
                    return SkillID * 1000 + 1;
                }
            }

            /// <summary>
            /// 获取下一等级的Infoid///
            /// </summary>
            /// <returns></returns>
            public uint GetNextRankInfoID()
            {
                if (Level == 0)
                {
                    return FirstInfoID;
                }
                else if (Level > MaxLevel)
                {
                    return 0;
                }
                else
                {
                    if (Rank >= EACHLEVELRANKNUM)
                    {
                        if (Level == MaxLevel)
                        {
                            return 0;
                        }
                        else
                        {
                            return SkillID * 1000 + Level * EACHLEVELRANKNUM + 1;
                        }
                    }
                    else
                    {
                        return CurInfoID + 1;
                    }
                }
            }
        }

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public enum EEvents
        {
            /// <summary>
            /// 选中某个技能///
            /// </summary>
            ChooseSkillItem,

            /// <summary>
            /// 更新技能等级///
            /// </summary>
            UpdateSkillLevel,

            /// <summary>
            /// 更新技能阶级和熟练度///
            /// </summary>
            UpdateSkillRankExp,

            /// <summary>
            /// 使用熟练度道具///
            /// </summary>
            UseItem,

            SkillRankUp,

            ClickedItem,

            ForgetSkill,
        }

        /// <summary>
        /// 得意技能集合///
        /// </summary>
        public Dictionary<uint, SkillInfo> bestSkillInfos = new Dictionary<uint, SkillInfo>();

        /// <summary>
        /// 通用技能集合///
        /// </summary>
        public Dictionary<uint, SkillInfo> commonSkillInfos = new Dictionary<uint, SkillInfo>();

        public List<uint> clickedSkillIDs = new List<uint>();

        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener((ushort)CmdSkill.ClickSkillReq, (ushort)CmdSkill.ClickSkillRes, OnClickSkillRes, CmdSkillClickSkillRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdSkill.SkillListNtf, OnSkillListNtf, CmdSkillSkillListNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdSkill.UpdateSkillLevelReq, (ushort)CmdSkill.UpdateSkillLevelRes, OnUpdateSkillLevelRes, CmdSkillUpdateSkillLevelRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdSkill.PassiveSkillUpdateReq, (ushort)CmdSkill.PassiveSkillUpdateRes, OnPassiveSkillUpdateRes, CmdSkillPassiveSkillUpdateRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdSkill.UseItemAddAsexpReq, (ushort)CmdSkill.UseItemAddAsexpRes, OnUseItemAddASExpRes, CmdSkillUseItemAddASExpRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdSkill.UseItemAddPsexpReq, (ushort)CmdSkill.UseItemAddPsexpRes, OnUseItemAddPSExpRes, CmdSkillUseItemAddPSExpRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdSkill.ActiveSkillRankExptNtf, OnActiveSkillRankExptNtf, CmdSkillActiveSkillRankExptNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdSkill.PassiveSkillRankExptNtf, OnPassiveSkillRankExptNtf, CmdSkillPassiveSkillRankExptNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdSkill.ForgetReq, (ushort)CmdSkill.ForgetRes, OnForgetRes, CmdSkillForgetRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdSkill.InfoNtf, OnCmdSkillInfoNtf, CmdSkillInfoNtf.Parser);
        }

        public override void OnLogout()
        {
            bestSkillInfos.Clear();
            commonSkillInfos.Clear();

            base.OnLogout();
        }

        /// <summary>
        /// 初始化当前职业所有可学技能///
        /// </summary>
        public void InitSkillDatas()
        {
            bestSkillInfos.Clear();
            commonSkillInfos.Clear();

            CSVCareer.Data cSVCareerData = CSVCareer.Instance.GetConfData(Sys_Role.Instance.Role.Career);
            if (cSVCareerData == null)
                return;

            ///初始化所有得意技能///
            for (int i = 0, len = cSVCareerData.proud_skill.Count; i < len; i++)
            {
                SkillInfo bestSkillInfo = new SkillInfo();
                bestSkillInfo.SkillID = cSVCareerData.proud_skill[i][0];
                bestSkillInfo.CurProficiency = 0;
                bestSkillInfo.MaxLevel = cSVCareerData.proud_skill[i][4];
                bestSkillInfo.ESkillSubType = ESkillSubType.Best;
                bestSkillInfo.Level = 0;
                bestSkillInfo.Rank = 0;
                bestSkillInfo.SortIndex = i;

                for (int index = 1, len2 = cSVCareerData.proud_skill[i].Count; index < len2; index++)
                {
                    bestSkillInfo.occUpgradeLimits.Add(cSVCareerData.proud_skill[i][index]);
                }

                if (CSVActiveSkillInfo.Instance.ContainsKey(bestSkillInfo.FirstInfoID))
                {
                    bestSkillInfo.ESkillType = ESkillType.Active;
                }
                else if (CSVPassiveSkillInfo.Instance.ContainsKey(bestSkillInfo.FirstInfoID))
                {
                    bestSkillInfo.ESkillType = ESkillType.Passive;
                }
                else
                {
                    DebugUtil.LogError($"Can not find skillInfo in active and Passive id: {bestSkillInfo.SkillID}");
                }

                bestSkillInfos[cSVCareerData.proud_skill[i][0]] = bestSkillInfo;
            }

            //初始化所有通用技能///
            for (int i = 0, len = cSVCareerData.currency_skill.Count; i < len; i++)
            {
                SkillInfo commonSkillInfo = new SkillInfo();
                commonSkillInfo.SkillID = cSVCareerData.currency_skill[i][0];
                commonSkillInfo.CurProficiency = 0;
                commonSkillInfo.MaxLevel = cSVCareerData.currency_skill[i][4];
                commonSkillInfo.ESkillSubType = ESkillSubType.Common;
                commonSkillInfo.Level = 0;
                commonSkillInfo.Rank = 0;
                commonSkillInfo.SortIndex = i;

                for (int index = 1, len2 = cSVCareerData.currency_skill[i].Count; index < len2; index++)
                {
                    commonSkillInfo.occUpgradeLimits.Add(cSVCareerData.currency_skill[i][index]);
                }

                if (CSVActiveSkillInfo.Instance.ContainsKey(commonSkillInfo.FirstInfoID))
                {
                    commonSkillInfo.ESkillType = ESkillType.Active;
                }
                else if (CSVPassiveSkillInfo.Instance.ContainsKey(commonSkillInfo.FirstInfoID))
                {
                    commonSkillInfo.ESkillType = ESkillType.Passive;
                }
                else
                {
                    DebugUtil.LogError($"Can not find skillInfo in active and Passive id: {commonSkillInfo.SkillID}");
                }

                commonSkillInfos[cSVCareerData.currency_skill[i][0]] = commonSkillInfo;
            }

            ///初始话初始得意技能///
            if (cSVCareerData.proud_initial != null)
            {
                foreach (var id in cSVCareerData.proud_initial)
                {
                    CSVActiveSkillInfo.Data cSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(id);
                    if (cSVActiveSkillInfoData != null)
                    {
                        SkillInfo bestSkillInfo;
                        if (bestSkillInfos.TryGetValue(cSVActiveSkillInfoData.skill_id, out bestSkillInfo))
                        {
                            bestSkillInfo.Level = 1;
                        }
                        else
                        {
                            DebugUtil.LogError($"初始主动技能不存在可学技能中 skillID: {cSVActiveSkillInfoData.skill_id}");
                        }
                    }
                    else
                    {
                        CSVPassiveSkillInfo.Data cSVPassiveSkillInfoData = CSVPassiveSkillInfo.Instance.GetConfData(id);
                        if (cSVPassiveSkillInfoData != null)
                        {
                            SkillInfo bestSkillInfo;
                            if (bestSkillInfos.TryGetValue(cSVPassiveSkillInfoData.skill_id, out bestSkillInfo))
                            {
                                bestSkillInfo.Level = 1;
                            }
                            else
                            {
                                DebugUtil.LogError($"初始被动技能不存在可学技能中 skillID: {cSVPassiveSkillInfoData.skill_id}");
                            }
                        }
                        else
                        {
                            DebugUtil.LogError($"找不到该得意技能ID id: {id}");
                        }
                    }
                }
            }

            ///初始化初始通用技能///
            if (cSVCareerData.currency_initial != null)
            {
                foreach (var id in cSVCareerData.currency_initial)
                {
                    CSVActiveSkillInfo.Data cSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(id);
                    if (cSVActiveSkillInfoData != null)
                    {
                        SkillInfo commonSkillInfo;
                        if (commonSkillInfos.TryGetValue(cSVActiveSkillInfoData.skill_id, out commonSkillInfo))
                        {
                            commonSkillInfo.Level = 1;
                        }
                        else
                        {
                            DebugUtil.LogError($"初始主动技能不存在可学技能中 skillID: {cSVActiveSkillInfoData.skill_id}");
                        }
                    }
                    else
                    {
                        CSVPassiveSkillInfo.Data cSVPassiveSkillInfoData = CSVPassiveSkillInfo.Instance.GetConfData(id);
                        if (cSVPassiveSkillInfoData != null)
                        {
                            SkillInfo commonSkillInfo;
                            if (commonSkillInfos.TryGetValue(cSVPassiveSkillInfoData.skill_id, out commonSkillInfo))
                            {
                                commonSkillInfo.Level = 1;
                            }
                            else
                            {
                                DebugUtil.LogError($"初始被动技能不存在可学技能中 skillID: {cSVPassiveSkillInfoData.skill_id}");
                            }
                        }
                        else
                        {
                            DebugUtil.LogError($"找不到该通用技能ID id: {id}");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 更新技能信息///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="eSkillType"></param>
        /// <param name="level"></param>
        /// <param name="rank"></param>
        /// <param name="proficiency"></param>
        SkillInfo UpdateSkillData(uint id, uint level, uint rank, uint proficiency, bool Init = true)
        {
            SkillInfo skillInfo;
            if (bestSkillInfos.TryGetValue(id, out skillInfo))
            {
                if (!Init)
                {
                    if (skillInfo.Level == 0 && level == 1)
                    {
                        if (skillInfo.ESkillType == ESkillType.Active)
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000751, LanguageHelper.GetTextContent(CSVActiveSkillInfo.Instance.GetConfData(skillInfo.FirstInfoID).name)));
                        }
                        else if (skillInfo.ESkillType == ESkillType.Passive)
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000751, LanguageHelper.GetTextContent(CSVPassiveSkillInfo.Instance.GetConfData(skillInfo.FirstInfoID).name)));
                        }
                    }
                }
                skillInfo.Level = level;
                skillInfo.Rank = rank;
                skillInfo.CurProficiency = proficiency;
            }
            else
            {
                if (commonSkillInfos.TryGetValue(id, out skillInfo))
                {
                    if (!Init)
                    {
                        if (skillInfo.Level == 0 && level == 1)
                        {
                            if (skillInfo.ESkillType == ESkillType.Active)
                            {
                                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000751, LanguageHelper.GetTextContent(CSVActiveSkillInfo.Instance.GetConfData(skillInfo.FirstInfoID).name)));
                            }
                            else if (skillInfo.ESkillType == ESkillType.Passive)
                            {
                                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000751, LanguageHelper.GetTextContent(CSVPassiveSkillInfo.Instance.GetConfData(skillInfo.FirstInfoID).name)));
                            }
                        }
                    }
                    skillInfo.Level = level;
                    skillInfo.Rank = rank;
                    skillInfo.CurProficiency = proficiency;
                }
                else
                {
                    DebugUtil.LogError($"该技能不可学 SkillID: {id}");
                }
            }

            return skillInfo;
        }

        /// <summary>
        /// 技能是否可以升级///
        /// </summary>
        /// <param name="skillID"></param>
        /// <param name="eSkillType"></param>
        /// <returns></returns>
        uint CanUpgradeRank(uint skillID, ESkillSubType eSkillSubType, uint num = 1)
        {
            long count = Sys_Bag.Instance.GetItemCount(202002);
            if (count <= 0)
            {
                Sys_Hint.Instance.PushContent_Normal(CSVLanguage.Instance.GetConfData(4408).words);
            }

            if (eSkillSubType == ESkillSubType.Best)
            {
                SkillInfo bestSkillInfo;
                if (bestSkillInfos.TryGetValue(skillID, out bestSkillInfo))
                {
                    if (bestSkillInfo.Rank <= EACHLEVELRANKNUM)
                    {
                        if (count >= num)
                            return num;
                        else
                            return (uint)count;
                    }
                }
            }
            else if (eSkillSubType == ESkillSubType.Common)
            {
                SkillInfo commonSkillInfo;
                if (commonSkillInfos.TryGetValue(skillID, out commonSkillInfo))
                {
                    if (commonSkillInfo.Rank <= EACHLEVELRANKNUM)
                    {
                        if (count >= num)
                            return num;
                        else
                            return (uint)count;
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// 技能是否可以升阶///
        /// </summary>
        /// <param name="skillID"></param>
        /// <param name="eSkillType"></param>
        /// <returns></returns>
        public bool CanUpgradeLevel(SkillInfo skillInfo, bool checkCost = true, bool tip = true)
        {
            if (skillInfo.ESkillType == ESkillType.Active)
            {
                CSVActiveSkillInfo.Data cSVActiveSkillInfoData;
                if (skillInfo.Level == 0)
                {
                    cSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(skillInfo.FirstInfoID);
                }
                else
                {
                    cSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(skillInfo.SkillID * 1000 + skillInfo.Level * Sys_Skill.EACHLEVELRANKNUM + 1);
                }
                if (cSVActiveSkillInfoData == null)
                    return false;

                if (cSVActiveSkillInfoData.need_lv != 0 && Sys_Role.Instance.Role.Level < cSVActiveSkillInfoData.need_lv)
                {
                    if (tip)
                    {
                        Sys_Hint.Instance.PushContent_Normal(string.Format(CSVLanguage.Instance.GetConfData(4406).words, cSVActiveSkillInfoData.need_lv.ToString()));
                    }
                    return false;
                }

                if (cSVActiveSkillInfoData.SkillCareerLevelCond().ContainsKey(Sys_Role.Instance.Role.Career))
                {
                    if (Sys_Role.Instance.Role.CareerRank < cSVActiveSkillInfoData.SkillCareerLevelCond()[Sys_Role.Instance.Role.Career])
                    {
                        if (tip)
                        {
                            Sys_Hint.Instance.PushContent_Normal(string.Format(CSVLanguage.Instance.GetConfData(4409).words, LanguageHelper.GetTextContent(CSVPromoteCareer.Instance.GetConfData(Sys_Role.Instance.Role.Career * 100 + cSVActiveSkillInfoData.SkillCareerLevelCond()[Sys_Role.Instance.Role.Career]).professionLan)));
                        }
                        return false;
                    }
                }

                if (cSVActiveSkillInfoData.pre_task != 0 && Sys_Task.Instance.GetTaskState(cSVActiveSkillInfoData.pre_task) != ETaskState.Submited)
                {
                    if (tip)
                    {
                        Sys_Hint.Instance.PushContent_Normal(string.Format(CSVLanguage.Instance.GetConfData(4407).words, CSVTaskLanguage.Instance.GetConfData(CSVTask.Instance.GetConfData(cSVActiveSkillInfoData.pre_task).taskName).words));
                    }
                    return false;
                }

                if (checkCost && cSVActiveSkillInfoData.upgrade_cost != null)
                {
                    for (int index = 0, len = cSVActiveSkillInfoData.upgrade_cost.Count; index < len; index++)
                    {
                        if (Sys_Bag.Instance.GetItemCount(cSVActiveSkillInfoData.upgrade_cost[index][0]) < cSVActiveSkillInfoData.upgrade_cost[index][1])
                        {
                            if (tip)
                            {
                                if (cSVActiveSkillInfoData.level == 1)
                                {
                                    Sys_Hint.Instance.PushContent_Normal(CSVLanguage.Instance.GetConfData(680000750).words);
                                }
                                else
                                {
                                    Sys_Hint.Instance.PushContent_Normal(CSVLanguage.Instance.GetConfData(4408).words);
                                }
                            }
                            return false;
                        }
                    }
                }
            }
            else if (skillInfo.ESkillType == ESkillType.Passive)
            {
                CSVPassiveSkillInfo.Data cSVPassiveSkillInfoData;
                if (skillInfo.Level == 0)
                {
                    cSVPassiveSkillInfoData = CSVPassiveSkillInfo.Instance.GetConfData(skillInfo.FirstInfoID);
                }
                else
                {
                    cSVPassiveSkillInfoData = CSVPassiveSkillInfo.Instance.GetConfData(skillInfo.SkillID * 1000 + skillInfo.Level * Sys_Skill.EACHLEVELRANKNUM + 1);
                }
                if (cSVPassiveSkillInfoData == null)
                    return false;

                if (cSVPassiveSkillInfoData.need_lv != 0 && Sys_Role.Instance.Role.Level < cSVPassiveSkillInfoData.need_lv)
                {
                    if (tip)
                    {
                        Sys_Hint.Instance.PushContent_Normal(string.Format(CSVLanguage.Instance.GetConfData(4406).words, cSVPassiveSkillInfoData.need_lv.ToString()));
                    }
                    return false;
                }

                if (cSVPassiveSkillInfoData.SkillCareerLevelCond().ContainsKey(Sys_Role.Instance.Role.Career))
                {
                    if (Sys_Role.Instance.Role.CareerRank < cSVPassiveSkillInfoData.SkillCareerLevelCond()[Sys_Role.Instance.Role.Career])
                    {
                        if (tip)
                        {
                            Sys_Hint.Instance.PushContent_Normal(string.Format(CSVLanguage.Instance.GetConfData(4409).words, LanguageHelper.GetTextContent(CSVPromoteCareer.Instance.GetConfData(Sys_Role.Instance.Role.Career * 100 + cSVPassiveSkillInfoData.SkillCareerLevelCond()[Sys_Role.Instance.Role.Career]).professionLan)));
                        }
                        return false;
                    }
                }

                if (cSVPassiveSkillInfoData.pre_task != 0 && Sys_Task.Instance.GetTaskState(cSVPassiveSkillInfoData.pre_task) != ETaskState.Submited)
                {
                    if (tip)
                    {
                        Sys_Hint.Instance.PushContent_Normal(string.Format(CSVLanguage.Instance.GetConfData(4407).words, CSVTaskLanguage.Instance.GetConfData(CSVTask.Instance.GetConfData(cSVPassiveSkillInfoData.pre_task).taskName).words));
                    }
                    return false;
                }

                if (checkCost && cSVPassiveSkillInfoData.upgrade_cost != null)
                {
                    for (int index = 0, len = cSVPassiveSkillInfoData.upgrade_cost.Count; index < len; index++)
                    {
                        if (Sys_Bag.Instance.GetItemCount(cSVPassiveSkillInfoData.upgrade_cost[index][0]) < cSVPassiveSkillInfoData.upgrade_cost[index][1])
                        {
                            if (tip)
                            {
                                if (cSVPassiveSkillInfoData.level == 1)
                                {
                                    Sys_Hint.Instance.PushContent_Normal(CSVLanguage.Instance.GetConfData(680000750).words);
                                }
                                else
                                {
                                    Sys_Hint.Instance.PushContent_Normal(CSVLanguage.Instance.GetConfData(4408).words);
                                }
                            }
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        #region Call

        /// <summary>
        /// 遗忘技能///
        /// </summary>
        /// <param name="skillID"></param>
        public void ReqForgetSKill(uint skillID)
        {
            CmdSkillForgetReq req = new CmdSkillForgetReq();
            req.SkillId = skillID;

            NetClient.Instance.SendMessage((ushort)CmdSkill.ForgetReq, req);
        }

        /// <summary>
        /// 升阶主动技能等级请求///
        /// </summary>
        /// <param name="skillID"></param>
        public void ReqUpdateSkillLevel(SkillInfo skillInfo)
        {
            if (CanUpgradeLevel(skillInfo))
            {
                CSVActiveSkillInfo.Data cSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(skillInfo.FirstInfoID);
                if (cSVActiveSkillInfoData != null && cSVActiveSkillInfoData.learn_npc != 0 && skillInfo.Level == 0)
                {
                    if (UIManager.IsOpen(EUIID.UI_SkillUpgrade))
                    {
                        UIManager.CloseUI(EUIID.UI_SkillUpgrade);
                    }
                    ActionCtrl.Instance.MoveToTargetNPCAndInteractive(cSVActiveSkillInfoData.learn_npc);
                }
                else
                {
                    CmdSkillUpdateSkillLevelReq req = new CmdSkillUpdateSkillLevelReq();
                    req.SkillId = skillInfo.SkillID;

                    NetClient.Instance.SendMessage((ushort)CmdSkill.UpdateSkillLevelReq, req);
                }
            }
        }

        /// <summary>
        /// 使用道具增加主动技能熟练度请求///
        /// </summary>
        /// <param name="skillID"></param>
        public void ReqUseItemAddASExp(SkillInfo skillInfo, uint num = 1)
        {
            uint count = CanUpgradeRank(skillInfo.SkillID, skillInfo.ESkillSubType, num);
            if (count > 0)
            {
                CmdSkillUseItemAddASExpReq req = new CmdSkillUseItemAddASExpReq();
                req.SkillId = skillInfo.SkillID;
                req.ItemCount = count;

                NetClient.Instance.SendMessage((ushort)CmdSkill.UseItemAddAsexpReq, req);
            }
        }

        /// <summary>
        /// 学习(升阶)被动技能请求///
        /// </summary>
        /// <param name="skillInfo"></param>
        public void ReqPassiveSkillUpdate(SkillInfo skillInfo)
        {
            if (CanUpgradeLevel(skillInfo))
            {
                CSVPassiveSkillInfo.Data cSVPassiveSkillInfoData = CSVPassiveSkillInfo.Instance.GetConfData(skillInfo.FirstInfoID);
                if (cSVPassiveSkillInfoData != null && cSVPassiveSkillInfoData.learn_npc != 0 && skillInfo.Level == 0)
                {
                    if (UIManager.IsOpen(EUIID.UI_SkillUpgrade))
                    {
                        UIManager.CloseUI(EUIID.UI_SkillUpgrade);
                    }
                    ActionCtrl.Instance.MoveToTargetNPCAndInteractive(cSVPassiveSkillInfoData.learn_npc);
                }
                else
                {
                    CmdSkillPassiveSkillUpdateReq req = new CmdSkillPassiveSkillUpdateReq();
                    req.SkillId = skillInfo.SkillID;

                    NetClient.Instance.SendMessage((ushort)CmdSkill.PassiveSkillUpdateReq, req);
                }
            }
        }

        /// <summary>
        /// 使用道具增加被动技能熟练度请求///
        /// </summary>
        /// <param name="skillInfo"></param>
        public void ReqUseItemAddPSExp(SkillInfo skillInfo, uint num = 1)
        {
            uint count = CanUpgradeRank(skillInfo.SkillID, skillInfo.ESkillSubType, num);
            if (count > 0)
            {
                CmdSkillUseItemAddPSExpReq req = new CmdSkillUseItemAddPSExpReq();
                req.SkillId = skillInfo.SkillID;
                req.ItemCount = num;

                NetClient.Instance.SendMessage((ushort)CmdSkill.UseItemAddPsexpReq, req);
            }
        }

        public void ReqOnSwitchUpdate(SkillInfo skillInfo)
        {
            CmdSkillOnSwitchUpdateReq req = new CmdSkillOnSwitchUpdateReq();
            req.SkillId = skillInfo.SkillID;

            NetClient.Instance.SendMessage((ushort)CmdSkill.OnSwitchUpdateReq, req);
        }

        #endregion

        #region Callback

        void OnForgetRes(NetMsg msg)
        {
            CmdSkillForgetRes res = NetMsgUtil.Deserialize<CmdSkillForgetRes>(CmdSkillForgetRes.Parser, msg);
            if (res != null)
            {
                if (bestSkillInfos.ContainsKey(res.SkillId))
                {
                    bestSkillInfos[res.SkillId].Level = 0;
                    bestSkillInfos[res.SkillId].Rank = 0;
                    bestSkillInfos[res.SkillId].CurProficiency = 0;
                }
                else if (commonSkillInfos.ContainsKey(res.SkillId))
                {
                    commonSkillInfos[res.SkillId].Level = 0;
                    commonSkillInfos[res.SkillId].Rank = 0;
                    commonSkillInfos[res.SkillId].CurProficiency = 0;
                }
                eventEmitter.Trigger(EEvents.ForgetSkill);
            }
        }

        void OnClickSkillRes(NetMsg msg)
        {
            CmdSkillClickSkillRes res = NetMsgUtil.Deserialize<CmdSkillClickSkillRes>(CmdSkillClickSkillRes.Parser, msg);
            if (res != null)
            {
                if (!clickedSkillIDs.Contains(res.SkillId))
                {
                    clickedSkillIDs.Add(res.SkillId);
                    eventEmitter.Trigger(EEvents.ClickedItem);
                }
            }
        }

        /// <summary>
        /// 技能列表通知///
        /// </summary>
        /// <param name="msg"></param>
        void OnSkillListNtf(NetMsg msg)
        {
            InitSkillDatas();

            CmdSkillSkillListNtf ntf = NetMsgUtil.Deserialize<CmdSkillSkillListNtf>(CmdSkillSkillListNtf.Parser, msg);
            if (ntf.SkillList != null)
            {
                foreach (var info in ntf.SkillList)
                {
                    UpdateSkillData(info.SkillId, info.SkillLevel, info.Rank, info.Exp);
                }

                foreach (var info in ntf.PassiveSkillList)
                {
                    UpdateSkillData(info.SkillId, info.SkillLevel, info.Rank, info.Exp);
                }

                clickedSkillIDs.Clear();
                for (int index = 0, len = ntf.ClickedSkillList.Count; index < len; index++)
                {
                    clickedSkillIDs.Add(ntf.ClickedSkillList[index]);
                }
            }
        }

        /// <summary>
        /// 升阶主动技能等级请求应答///
        /// </summary>
        /// <param name="msg"></param>
        void OnUpdateSkillLevelRes(NetMsg msg)
        {
            CmdSkillUpdateSkillLevelRes res = NetMsgUtil.Deserialize<CmdSkillUpdateSkillLevelRes>(CmdSkillUpdateSkillLevelRes.Parser, msg);
            if (res != null)
            {
                SkillInfo skillInfo = UpdateSkillData(res.SkillId, res.SkillLevel, 1, 0, false);
                if (skillInfo != null)
                {
                    eventEmitter.Trigger<SkillInfo>(EEvents.UpdateSkillLevel, skillInfo);
                }

                //Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.SkillUp);
            }
        }

        /// <summary>
        /// 学习(升阶)被动技能请求应答///
        /// </summary>
        /// <param name="msg"></param>
        void OnPassiveSkillUpdateRes(NetMsg msg)
        {
            CmdSkillPassiveSkillUpdateRes res = NetMsgUtil.Deserialize<CmdSkillPassiveSkillUpdateRes>(CmdSkillPassiveSkillUpdateRes.Parser, msg);
            if (res != null)
            {
                SkillInfo skillInfo = UpdateSkillData(res.SkillId, res.SkillLevel, 1, 0, false);
                if (skillInfo != null)
                {
                    eventEmitter.Trigger<SkillInfo>(EEvents.UpdateSkillLevel, skillInfo);
                }

                //Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.SkillUp);
            }
        }

        /// <summary>
        /// 使用道具增加主动技能熟练度返回///
        /// </summary>
        /// <param name="msg"></param>
        void OnUseItemAddASExpRes(NetMsg msg)
        {
            eventEmitter.Trigger(EEvents.UseItem);
        }

        /// <summary>
        /// 使用道具增加被动技能熟练度返回///
        /// </summary>
        /// <param name="msg"></param>
        void OnUseItemAddPSExpRes(NetMsg msg)
        {
            eventEmitter.Trigger(EEvents.UseItem);
        }

        /// <summary>
        /// 主动技能升级和熟练更新通知///
        /// </summary>
        /// <param name="msg"></param>
        void OnActiveSkillRankExptNtf(NetMsg msg)
        {
            CmdSkillActiveSkillRankExptNtf ntf = NetMsgUtil.Deserialize<CmdSkillActiveSkillRankExptNtf>(CmdSkillActiveSkillRankExptNtf.Parser, msg);
            if (ntf != null)
            {
                foreach (var pair in ntf.SkillList)
                {
                    SkillInfo skillInfo;
                    if (bestSkillInfos.TryGetValue(pair.SkillId, out skillInfo))
                    {
                        if (pair.Rank > skillInfo.Rank)
                        {
                            //Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.SkillUp);
                            eventEmitter.Trigger(EEvents.SkillRankUp);
                        }

                        if (ntf.BattleChange)
                        {
                            CSVActiveSkillInfo.Data cSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(skillInfo.CurInfoID);
                            if (cSVActiveSkillInfoData != null)
                            {
                                if (pair.Rank >= EACHLEVELRANKNUM && pair.Exp >= cSVActiveSkillInfoData.need_proficiency && skillInfo.Level >= skillInfo.MaxLevel)
                                {

                                }
                                else
                                {
                                    if (pair.Rank > skillInfo.Rank)
                                    {
                                        Sys_Hint.Instance.PushContent_Property(cSVActiveSkillInfoData.id, pair.Rank - skillInfo.Rank, EPropertyType.ActiveSkill);
                                        AudioUtil.PlayAudio(4002);
                                    }

                                    skillInfo.Rank = pair.Rank;
                                    skillInfo.CurProficiency = pair.Exp;

                                    if (skillInfo.Rank >= EACHLEVELRANKNUM && skillInfo.CurProficiency >= cSVActiveSkillInfoData.need_proficiency)
                                    {
                                        string content = LanguageHelper.GetErrorCodeContent(680000003, LanguageHelper.GetTextContent(cSVActiveSkillInfoData.name));
                                        ErrorCodeHelper.PushErrorCode(CSVErrorCode.Instance.GetConfData(680000003).pos, content, content);
                                    }
                                    else
                                    {
                                        if (pair.ProudAdd > 0)
                                        {
                                            string content = LanguageHelper.GetErrorCodeContent(680000001, LanguageHelper.GetTextContent(cSVActiveSkillInfoData.name), pair.Add.ToString(), pair.ProudAdd.ToString());
                                            ErrorCodeHelper.PushErrorCode(CSVErrorCode.Instance.GetConfData(680000001).pos, content, content);
                                        }
                                        else
                                        {
                                            string content = LanguageHelper.GetErrorCodeContent(680000002, LanguageHelper.GetTextContent(cSVActiveSkillInfoData.name), pair.Add.ToString());
                                            ErrorCodeHelper.PushErrorCode(CSVErrorCode.Instance.GetConfData(680000002).pos, content, content);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            skillInfo.Rank = pair.Rank;
                            skillInfo.CurProficiency = pair.Exp;
                        }
                    }
                    else
                    {
                        if (commonSkillInfos.TryGetValue(pair.SkillId, out skillInfo))
                        {
                            if (pair.Rank > skillInfo.Rank)
                            {
                                //Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.SkillUp);
                                eventEmitter.Trigger(EEvents.SkillRankUp);
                            }

                            if (ntf.BattleChange)
                            {
                                CSVActiveSkillInfo.Data cSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(skillInfo.CurInfoID);
                                if (cSVActiveSkillInfoData != null)
                                {
                                    if (pair.Rank >= EACHLEVELRANKNUM && pair.Exp >= cSVActiveSkillInfoData.need_proficiency && skillInfo.Level >= skillInfo.MaxLevel)
                                    {

                                    }
                                    else
                                    {
                                        skillInfo.Rank = pair.Rank;
                                        skillInfo.CurProficiency = pair.Exp;

                                        if (skillInfo.Rank >= EACHLEVELRANKNUM && skillInfo.CurProficiency >= cSVActiveSkillInfoData.need_proficiency)
                                        {
                                            string content = LanguageHelper.GetErrorCodeContent(680000003, LanguageHelper.GetTextContent(cSVActiveSkillInfoData.name));
                                            ErrorCodeHelper.PushErrorCode(CSVErrorCode.Instance.GetConfData(680000003).pos, content, content);
                                        }
                                        else
                                        {
                                            if (pair.ProudAdd > 0)
                                            {
                                                string content = LanguageHelper.GetErrorCodeContent(680000001, LanguageHelper.GetTextContent(cSVActiveSkillInfoData.name), pair.Add.ToString(), pair.ProudAdd.ToString());
                                                ErrorCodeHelper.PushErrorCode(CSVErrorCode.Instance.GetConfData(680000001).pos, content, content);
                                            }
                                            else
                                            {
                                                string content = LanguageHelper.GetErrorCodeContent(680000002, LanguageHelper.GetTextContent(cSVActiveSkillInfoData.name), pair.Add.ToString());
                                                ErrorCodeHelper.PushErrorCode(CSVErrorCode.Instance.GetConfData(680000002).pos, content, content);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                skillInfo.Rank = pair.Rank;
                                skillInfo.CurProficiency = pair.Exp;
                            }
                        }
                    }
                    if (skillInfo != null)
                    {
                        eventEmitter.Trigger<SkillInfo>(EEvents.UpdateSkillRankExp, skillInfo);
                    }
                }
            }
        }

        /// <summary>
        /// 被动技能升级和熟练更新通知///
        /// </summary>
        /// <param name="msg"></param>
        void OnPassiveSkillRankExptNtf(NetMsg msg)
        {
            CmdSkillPassiveSkillRankExptNtf ntf = NetMsgUtil.Deserialize<CmdSkillPassiveSkillRankExptNtf>(CmdSkillPassiveSkillRankExptNtf.Parser, msg);
            if (ntf != null)
            {
                SkillInfo skillInfo;
                if (bestSkillInfos.TryGetValue(ntf.SkillId, out skillInfo))
                {
                    if (ntf.Rank > skillInfo.Rank)
                    {
                        //Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.SkillUp);
                        eventEmitter.Trigger(EEvents.SkillRankUp);
                    }
                    skillInfo.Rank = ntf.Rank;
                    skillInfo.CurProficiency = ntf.Exp;
                }
                else
                {
                    if (commonSkillInfos.TryGetValue(ntf.SkillId, out skillInfo))
                    {
                        if (ntf.Rank > skillInfo.Rank)
                        {
                            //Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.SkillUp);
                            eventEmitter.Trigger(EEvents.SkillRankUp);
                        }
                        skillInfo.Rank = ntf.Rank;
                        skillInfo.CurProficiency = ntf.Exp;
                    }
                }
                if (skillInfo != null)
                {
                    eventEmitter.Trigger<SkillInfo>(EEvents.UpdateSkillRankExp, skillInfo);
                }
            }
        }

        void OnCmdSkillInfoNtf(NetMsg msg)
        {
            CmdSkillInfoNtf ntf = NetMsgUtil.Deserialize<CmdSkillInfoNtf>(CmdSkillInfoNtf.Parser, msg);
            if (ntf != null)
            {
                var skillInfo =  UpdateSkillData(ntf.SkillId, ntf.Level, ntf.Rank, ntf.Exp);
                if (skillInfo != null)
                {
                    eventEmitter.Trigger<SkillInfo>(EEvents.UpdateSkillLevel, skillInfo);
                }
            }
        }

        #endregion

        #region Logic function

        public bool IsActiveSkill(uint skillId)
        {
            return skillId <= 999999 && skillId > 0; //六位数主动技能
        }

        public bool IsCanLearnSkill(SkillInfo skillInfo)
        {
            bool result;

            if (bestSkillInfos.Count == 0 && commonSkillInfos.Count == 0)
                return false;

            if (skillInfo.Level > 0)
                return false;

            if (clickedSkillIDs.Contains(skillInfo.SkillID))
                return false;

            if (skillInfo.ESkillType == ESkillType.Active)
            {
                CSVActiveSkillInfo.Data nextdata = CSVActiveSkillInfo.Instance.GetConfData(skillInfo.GetNextRankInfoID());
                if (nextdata != null)
                {
                    result = true;

                    if (nextdata.need_lv != 0 && Sys_Role.Instance.Role.Level < nextdata.need_lv)
                    {
                        result = false;
                    }

                    if (nextdata.SkillCareerLevelCond().ContainsKey(Sys_Role.Instance.Role.Career))
                    {
                        if (Sys_Role.Instance.Role.CareerRank < nextdata.SkillCareerLevelCond()[Sys_Role.Instance.Role.Career])
                        {
                            result = false;
                        }
                    }

                    if (nextdata.pre_task != 0 && Sys_Task.Instance.GetTaskState(nextdata.pre_task) != ETaskState.Submited)
                    {
                        result = false;
                    }

                    if (result)
                    {
                        return true;
                    }
                }
            }
            else if (skillInfo.ESkillType == ESkillType.Passive)
            {
                CSVPassiveSkillInfo.Data nextdata = CSVPassiveSkillInfo.Instance.GetConfData(skillInfo.GetNextRankInfoID());
                if (nextdata != null)
                {
                    result = true;

                    if (nextdata.need_lv != 0 && Sys_Role.Instance.Role.Level < nextdata.need_lv)
                    {
                        result = false;
                    }

                    if (nextdata.SkillCareerLevelCond().ContainsKey(Sys_Role.Instance.Role.Career))
                    {
                        if (Sys_Role.Instance.Role.CareerRank < nextdata.SkillCareerLevelCond()[Sys_Role.Instance.Role.Career])
                        {
                            result = false;
                        }
                    }

                    if (nextdata.pre_task != 0 && Sys_Task.Instance.GetTaskState(nextdata.pre_task) != ETaskState.Submited)
                    {
                        result = false;
                    }

                    if (result)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool  ExistedLearnShowTipCommonSkill()
        {
            bool result;

            if (commonSkillInfos.Count == 0)
                return false;

            foreach (var skillInfo in commonSkillInfos.Values)
            {
                if (skillInfo.ESkillType == ESkillType.Active)
                {
                    CSVActiveSkillInfo.Data nextdata = CSVActiveSkillInfo.Instance.GetConfData(skillInfo.GetNextRankInfoID());
                    if (nextdata != null)
                    {
                        result = true;

                        if (skillInfo.Level > 0)
                            result = false;

                        if (clickedSkillIDs.Contains(skillInfo.SkillID))
                            result = false;

                        if (nextdata.need_lv != 0 && Sys_Role.Instance.Role.Level < nextdata.need_lv)
                        {
                            result = false;
                        }

                        if (nextdata.SkillCareerLevelCond().ContainsKey(Sys_Role.Instance.Role.Career))
                        {
                            if (Sys_Role.Instance.Role.CareerRank < nextdata.SkillCareerLevelCond()[Sys_Role.Instance.Role.Career])
                            {
                                result = false;
                            }
                        }

                        if (nextdata.pre_task != 0 && Sys_Task.Instance.GetTaskState(nextdata.pre_task) != ETaskState.Submited)
                        {
                            result = false;
                        }

                        if (result)
                        {
                            return true;
                        }
                    }
                }
                else if (skillInfo.ESkillType == ESkillType.Passive)
                {
                    CSVPassiveSkillInfo.Data nextdata = CSVPassiveSkillInfo.Instance.GetConfData(skillInfo.GetNextRankInfoID());
                    if (nextdata != null)
                    {
                        result = true;

                        if (skillInfo.Level > 0)
                            result = false;

                        if (clickedSkillIDs.Contains(skillInfo.SkillID))
                            result = false;

                        if (nextdata.need_lv != 0 && Sys_Role.Instance.Role.Level < nextdata.need_lv)
                        {
                            result = false;
                        }

                        if (nextdata.SkillCareerLevelCond().ContainsKey(Sys_Role.Instance.Role.Career))
                        {
                            if (Sys_Role.Instance.Role.CareerRank < nextdata.SkillCareerLevelCond()[Sys_Role.Instance.Role.Career])
                            {
                                result = false;
                            }
                        }

                        if (nextdata.pre_task != 0 && Sys_Task.Instance.GetTaskState(nextdata.pre_task) != ETaskState.Submited)
                        {
                            result = false;
                        }

                        if (result)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool ExistedLearnShowTipBestSkill()
        {
            bool result;

            if (bestSkillInfos.Count == 0)
                return false;

            foreach (var skillInfo in bestSkillInfos.Values)
            {
                if (skillInfo.ESkillType == ESkillType.Active)
                {
                    CSVActiveSkillInfo.Data nextdata = CSVActiveSkillInfo.Instance.GetConfData(skillInfo.GetNextRankInfoID());
                    if (nextdata != null)
                    {
                        result = true;

                        if (skillInfo.Level > 0)
                            result = false;

                        if (clickedSkillIDs.Contains(skillInfo.SkillID))
                            result = false;

                        if (nextdata.need_lv != 0 && Sys_Role.Instance.Role.Level < nextdata.need_lv)
                        {
                            result = false;
                        }

                        if (nextdata.SkillCareerLevelCond().ContainsKey(Sys_Role.Instance.Role.Career))
                        {
                            if (Sys_Role.Instance.Role.CareerRank < nextdata.SkillCareerLevelCond()[Sys_Role.Instance.Role.Career])
                            {
                                result = false;
                            }
                        }

                        if (nextdata.pre_task != 0 && Sys_Task.Instance.GetTaskState(nextdata.pre_task) != ETaskState.Submited)
                        {
                            result = false;
                        }

                        if (result)
                        {
                            return true;
                        }
                    }
                }
                else if (skillInfo.ESkillType == ESkillType.Passive)
                {
                    CSVPassiveSkillInfo.Data nextdata = CSVPassiveSkillInfo.Instance.GetConfData(skillInfo.GetNextRankInfoID());
                    if (nextdata != null)
                    {
                        result = true;

                        if (skillInfo.Level > 0)
                            result = false;

                        if (clickedSkillIDs.Contains(skillInfo.SkillID))
                            result = false;

                        if (nextdata.need_lv != 0 && Sys_Role.Instance.Role.Level < nextdata.need_lv)
                        {
                            result = false;
                        }

                        if (nextdata.SkillCareerLevelCond().ContainsKey(Sys_Role.Instance.Role.Career))
                        {
                            if (Sys_Role.Instance.Role.CareerRank < nextdata.SkillCareerLevelCond()[Sys_Role.Instance.Role.Career])
                            {
                                result = false;
                            }
                        }

                        if (nextdata.pre_task != 0 && Sys_Task.Instance.GetTaskState(nextdata.pre_task) != ETaskState.Submited)
                        {
                            result = false;
                        }

                        if (result)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool ExistedLearnShowTipSkill()
        {
            bool result;

            if (bestSkillInfos.Count == 0 && commonSkillInfos.Count == 0)
                return false;

            foreach (var skillInfo in bestSkillInfos.Values)
            {
                if (skillInfo.ESkillType == ESkillType.Active)
                {
                    CSVActiveSkillInfo.Data nextdata = CSVActiveSkillInfo.Instance.GetConfData(skillInfo.GetNextRankInfoID());
                    if (nextdata != null)
                    {
                        result = true;

                        if (skillInfo.Level > 0)
                            result = false;

                        if (clickedSkillIDs.Contains(skillInfo.SkillID))
                            result = false;

                        if (nextdata.need_lv != 0 && Sys_Role.Instance.Role.Level < nextdata.need_lv)
                        {
                            result = false;
                        }

                        if (nextdata.SkillCareerLevelCond().ContainsKey(Sys_Role.Instance.Role.Career))
                        {
                            if (Sys_Role.Instance.Role.CareerRank < nextdata.SkillCareerLevelCond()[Sys_Role.Instance.Role.Career])
                            {
                                result = false;
                            }
                        }

                        if (nextdata.pre_task != 0 && Sys_Task.Instance.GetTaskState(nextdata.pre_task) != ETaskState.Submited)
                        {
                            result = false;
                        }

                        if (result)
                        {
                            return true;
                        }
                    }
                }
                else if (skillInfo.ESkillType == ESkillType.Passive)
                {
                    CSVPassiveSkillInfo.Data nextdata = CSVPassiveSkillInfo.Instance.GetConfData(skillInfo.GetNextRankInfoID());
                    if (nextdata != null)
                    {
                        result = true;

                        if (skillInfo.Level > 0)
                            result = false;

                        if (clickedSkillIDs.Contains(skillInfo.SkillID))
                            result = false;

                        if (nextdata.need_lv != 0 && Sys_Role.Instance.Role.Level < nextdata.need_lv)
                        {
                            result = false;
                        }

                        if (nextdata.SkillCareerLevelCond().ContainsKey(Sys_Role.Instance.Role.Career))
                        {
                            if (Sys_Role.Instance.Role.CareerRank < nextdata.SkillCareerLevelCond()[Sys_Role.Instance.Role.Career])
                            {
                                result = false;
                            }
                        }

                        if (nextdata.pre_task != 0 && Sys_Task.Instance.GetTaskState(nextdata.pre_task) != ETaskState.Submited)
                        {
                            result = false;
                        }

                        if (result)
                        {
                            return true;
                        }
                    }
                }
            }

            uint levelLimit = uint.Parse(CSVParam.Instance.GetConfData(953).str_value);
            if (Sys_Role.Instance.Role.Level >= levelLimit)
            {
                foreach (var skillInfo in commonSkillInfos.Values)
                {
                    if (skillInfo.ESkillType == ESkillType.Active)
                    {
                        CSVActiveSkillInfo.Data nextdata = CSVActiveSkillInfo.Instance.GetConfData(skillInfo.GetNextRankInfoID());
                        if (nextdata != null)
                        {
                            result = true;

                            if (skillInfo.Level > 0)
                                result = false;

                            if (clickedSkillIDs.Contains(skillInfo.SkillID))
                                result = false;

                            if (nextdata.need_lv != 0 && Sys_Role.Instance.Role.Level < nextdata.need_lv)
                            {
                                result = false;
                            }

                            if (nextdata.SkillCareerLevelCond().ContainsKey(Sys_Role.Instance.Role.Career))
                            {
                                if (Sys_Role.Instance.Role.CareerRank < nextdata.SkillCareerLevelCond()[Sys_Role.Instance.Role.Career])
                                {
                                    result = false;
                                }
                            }

                            if (nextdata.pre_task != 0 && Sys_Task.Instance.GetTaskState(nextdata.pre_task) != ETaskState.Submited)
                            {
                                result = false;
                            }

                            if (result)
                            {
                                return true;
                            }
                        }
                    }
                    else if (skillInfo.ESkillType == ESkillType.Passive)
                    {
                        CSVPassiveSkillInfo.Data nextdata = CSVPassiveSkillInfo.Instance.GetConfData(skillInfo.GetNextRankInfoID());
                        if (nextdata != null)
                        {
                            result = true;

                            if (skillInfo.Level > 0)
                                result = false;

                            if (clickedSkillIDs.Contains(skillInfo.SkillID))
                                result = false;

                            if (nextdata.need_lv != 0 && Sys_Role.Instance.Role.Level < nextdata.need_lv)
                            {
                                result = false;
                            }

                            if (nextdata.SkillCareerLevelCond().ContainsKey(Sys_Role.Instance.Role.Career))
                            {
                                if (Sys_Role.Instance.Role.CareerRank < nextdata.SkillCareerLevelCond()[Sys_Role.Instance.Role.Career])
                                {
                                    result = false;
                                }
                            }

                            if (nextdata.pre_task != 0 && Sys_Task.Instance.GetTaskState(nextdata.pre_task) != ETaskState.Submited)
                            {
                                result = false;
                            }

                            if (result)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 是否存在可以升阶的技能///
        /// </summary>
        /// <returns></returns>
        public bool ExistedUpgradeLevel(out int id)
        {
            bool result;

            if (bestSkillInfos.Count == 0 && commonSkillInfos.Count == 0)
            {
                id = 0;
                return false;
            }
            foreach (var skillInfo in bestSkillInfos.Values)
            {
                if (skillInfo.Level < skillInfo.MaxLevel && skillInfo.Rank >= EACHLEVELRANKNUM)
                {
                    if (skillInfo.ESkillType == ESkillType.Active)
                    {
                        CSVActiveSkillInfo.Data curData = CSVActiveSkillInfo.Instance.GetConfData(skillInfo.CurInfoID);
                        CSVActiveSkillInfo.Data nextdata = CSVActiveSkillInfo.Instance.GetConfData(skillInfo.GetNextRankInfoID());
                        if (nextdata != null && curData != null)
                        {
                            result = true;

                            if (skillInfo.CurProficiency < curData.need_proficiency)
                                result = false;

                            if (nextdata.need_lv != 0 && Sys_Role.Instance.Role.Level < nextdata.need_lv)
                            {
                                result = false;
                            }

                            if (nextdata.SkillCareerLevelCond().ContainsKey(Sys_Role.Instance.Role.Career))
                            {
                                if (Sys_Role.Instance.Role.CareerRank < nextdata.SkillCareerLevelCond()[Sys_Role.Instance.Role.Career])
                                {
                                    result = false;
                                }
                            }

                            if (nextdata.pre_task != 0 && Sys_Task.Instance.GetTaskState(nextdata.pre_task) != ETaskState.Submited)
                            {
                                result = false;
                            }

                            if (result)
                            {
                                id = (int)skillInfo.SkillID;
                                return true;
                            }
                        }
                    }
                    else if (skillInfo.ESkillType == ESkillType.Passive)
                    {
                        CSVPassiveSkillInfo.Data curData = CSVPassiveSkillInfo.Instance.GetConfData(skillInfo.CurInfoID);
                        CSVPassiveSkillInfo.Data nextdata = CSVPassiveSkillInfo.Instance.GetConfData(skillInfo.GetNextRankInfoID());
                        if (nextdata != null && curData != null)
                        {
                            result = true;

                            if (skillInfo.CurProficiency < curData.max_adept)
                                result = false;

                            if (nextdata.need_lv != 0 && Sys_Role.Instance.Role.Level < nextdata.need_lv)
                            {
                                result = false;
                            }

                            if (nextdata.SkillCareerLevelCond().ContainsKey(Sys_Role.Instance.Role.Career))
                            {
                                if (Sys_Role.Instance.Role.CareerRank < nextdata.SkillCareerLevelCond()[Sys_Role.Instance.Role.Career])
                                {
                                    result = false;
                                }
                            }

                            if (nextdata.pre_task != 0 && Sys_Task.Instance.GetTaskState(nextdata.pre_task) != ETaskState.Submited)
                            {
                                result = false;
                            }

                            if (result)
                            {
                                id = (int)skillInfo.SkillID;
                                return true;
                            }
                        }
                    }
                }
            }

            foreach (var skillInfo in commonSkillInfos.Values)
            {
                if (skillInfo.Level < skillInfo.MaxLevel && skillInfo.Rank >= EACHLEVELRANKNUM)
                {
                    if (skillInfo.ESkillType == ESkillType.Active)
                    {
                        CSVActiveSkillInfo.Data curData = CSVActiveSkillInfo.Instance.GetConfData(skillInfo.CurInfoID);
                        CSVActiveSkillInfo.Data nextdata = CSVActiveSkillInfo.Instance.GetConfData(skillInfo.GetNextRankInfoID());
                        if (nextdata != null && curData != null)
                        {
                            result = true;

                            if (skillInfo.CurProficiency < curData.need_proficiency)
                                result = false;

                            if (nextdata.need_lv != 0 && Sys_Role.Instance.Role.Level < nextdata.need_lv)
                            {
                                result = false;
                            }

                            if (nextdata.SkillCareerLevelCond().ContainsKey(Sys_Role.Instance.Role.Career))
                            {
                                if (Sys_Role.Instance.Role.CareerRank < nextdata.SkillCareerLevelCond()[Sys_Role.Instance.Role.Career])
                                {
                                    result = false;
                                }
                            }

                            if (nextdata.pre_task != 0 && Sys_Task.Instance.GetTaskState(nextdata.pre_task) != ETaskState.Submited)
                            {
                                result = false;
                            }

                            if (result)
                            {
                                id = (int)skillInfo.SkillID;
                                return true;
                            }
                        }
                    }
                    else if (skillInfo.ESkillType == ESkillType.Passive)
                    {
                        CSVPassiveSkillInfo.Data curData = CSVPassiveSkillInfo.Instance.GetConfData(skillInfo.CurInfoID);
                        CSVPassiveSkillInfo.Data nextdata = CSVPassiveSkillInfo.Instance.GetConfData(skillInfo.GetNextRankInfoID());
                        if (nextdata != null && curData != null)
                        {
                            result = true;

                            if (skillInfo.CurProficiency < curData.max_adept)
                                result = false;

                            if (nextdata.need_lv != 0 && Sys_Role.Instance.Role.Level < nextdata.need_lv)
                            {
                                result = false;
                            }

                            if (nextdata.SkillCareerLevelCond().ContainsKey(Sys_Role.Instance.Role.Career))
                            {
                                if (Sys_Role.Instance.Role.CareerRank < nextdata.SkillCareerLevelCond()[Sys_Role.Instance.Role.Career])
                                {
                                    result = false;
                                }
                            }

                            if (nextdata.pre_task != 0 && Sys_Task.Instance.GetTaskState(nextdata.pre_task) != ETaskState.Submited)
                            {
                                result = false;
                            }

                            if (result)
                            {
                                id = (int)skillInfo.SkillID;
                                return true;
                            }
                        }
                    }
                }
            }

            id = 0;
            return false;
        }

        /// <summary>
        /// 是否存在可以升级的技能///
        /// </summary>
        /// <returns></returns>
        public bool ExistedUpgradeRank(out int id)
        {
            if (Sys_Role.Instance.Role.Level < uint.Parse(CSVParam.Instance.GetConfData(932).str_value))
            {
                id = 0;
                return false;
            }

            if (bestSkillInfos.Count == 0 && commonSkillInfos.Count == 0)
            {
                id = 0;
                return false;
            }

            foreach (var skillInfo in bestSkillInfos.Values)
            {
                if (skillInfo.Level > 0 && skillInfo.Rank < EACHLEVELRANKNUM)
                {
                    if (skillInfo.ESkillType == ESkillType.Active)
                    {
                        CSVActiveSkillInfo.Data cSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(skillInfo.CurInfoID);
                        uint needpoint = cSVActiveSkillInfoData.need_proficiency - skillInfo.CurProficiency;
                        if (Sys_Bag.Instance.GetItemCount(CSVParam.Instance.UpgradeSkillItemID) * CSVParam.Instance.UpgradeSkillItemAddNum >= needpoint)
                        {
                            id = (int)skillInfo.SkillID;
                            return true;
                        }
                    }
                    else if (skillInfo.ESkillType == ESkillType.Passive)
                    {
                        CSVPassiveSkillInfo.Data CSVPassiveSkillInfoData = CSVPassiveSkillInfo.Instance.GetConfData(skillInfo.CurInfoID);
                        uint needpoint = CSVPassiveSkillInfoData.max_adept - skillInfo.CurProficiency;
                        if (Sys_Bag.Instance.GetItemCount(CSVParam.Instance.UpgradeSkillItemID) * CSVParam.Instance.UpgradeSkillItemAddNum >= needpoint)
                        {
                            id = (int)skillInfo.SkillID;
                            return true;
                        }
                    }
                }
            }

            foreach (var skillInfo in commonSkillInfos.Values)
            {
                if (skillInfo.Level > 0 && skillInfo.Rank < EACHLEVELRANKNUM)
                {
                    if (skillInfo.ESkillType == ESkillType.Active)
                    {
                        CSVActiveSkillInfo.Data cSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(skillInfo.CurInfoID);
                        uint needpoint = cSVActiveSkillInfoData.need_proficiency - skillInfo.CurProficiency;
                        if (Sys_Bag.Instance.GetItemCount(CSVParam.Instance.UpgradeSkillItemID) * CSVParam.Instance.UpgradeSkillItemAddNum >= needpoint)
                        {
                            id = (int)skillInfo.SkillID;
                            return true;
                        }
                    }
                    else if (skillInfo.ESkillType == ESkillType.Passive)
                    {
                        CSVPassiveSkillInfo.Data CSVPassiveSkillInfoData = CSVPassiveSkillInfo.Instance.GetConfData(skillInfo.CurInfoID);
                        uint needpoint = CSVPassiveSkillInfoData.max_adept - skillInfo.CurProficiency;
                        if (Sys_Bag.Instance.GetItemCount(CSVParam.Instance.UpgradeSkillItemID) * CSVParam.Instance.UpgradeSkillItemAddNum >= needpoint)
                        {
                            id = (int)skillInfo.SkillID;
                            return true;
                        }
                    }
                }
            }

            id = 0;
            return false;
        }

        public string GetSkillDesc(uint skillID)
        {
            string desc = string.Empty;
            CSVActiveSkillInfo.Data cSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(skillID);
            if (cSVActiveSkillInfoData != null)
            {
                CSVActiveSkillEffective.Data cSVActiveSkillEffectiveData = CSVActiveSkillEffective.Instance.GetConfData(cSVActiveSkillInfoData.effect_id);
                if (cSVActiveSkillEffectiveData != null)
                {
                    if (cSVActiveSkillEffectiveData.skill_effect_type == 11)
                    {
                        desc = LanguageHelper.GetTextContent(CSVLanguage.Instance.GetConfData(cSVActiveSkillInfoData.desc), cSVActiveSkillEffectiveData.effect_to_target.ToString(), System.Math.Round(cSVActiveSkillEffectiveData.effect_to_target2 / 100.0f, 1).ToString());
                    }
                    else
                    {
                        if (cSVActiveSkillEffectiveData.damage_type == 1)
                        {
                            desc = LanguageHelper.GetTextContent(CSVLanguage.Instance.GetConfData(cSVActiveSkillInfoData.desc), System.Math.Round(cSVActiveSkillEffectiveData.effect_to_target / 100.0f, 1).ToString(), cSVActiveSkillEffectiveData.fixed_damage.ToString());
                        }
                        else if (cSVActiveSkillEffectiveData.damage_type == 2)
                        {
                            desc = LanguageHelper.GetTextContent(CSVLanguage.Instance.GetConfData(cSVActiveSkillInfoData.desc), cSVActiveSkillEffectiveData.fixed_damage.ToString(), System.Math.Round(cSVActiveSkillEffectiveData.base_magic_attack / 100.0f, 1).ToString());
                        }
                        else
                        {
                            CSVBuff.Data cSVBuffData = CSVBuff.Instance.GetConfData(cSVActiveSkillEffectiveData.effect_to_target);
                            if (cSVBuffData != null)
                            {
                                if (cSVBuffData.buff_type == 1)
                                {
                                    desc = LanguageHelper.GetTextContent(CSVLanguage.Instance.GetConfData(cSVActiveSkillInfoData.desc), System.Math.Round(cSVBuffData.success_probability / 100.0f, 1).ToString());
                                }
                                if (cSVBuffData.buff_type == 10 || cSVBuffData.buff_type == 2 || cSVBuffData.buff_type == 15)
                                {
                                    desc = LanguageHelper.GetTextContent(CSVLanguage.Instance.GetConfData(cSVActiveSkillInfoData.desc), System.Math.Round(cSVBuffData.ui_param1 / 100.0f, 1).ToString(), System.Math.Round(cSVBuffData.ui_param2 / 100.0f, 1).ToString());
                                }
                                else if (cSVBuffData.buff_type == 14)
                                {
                                    desc = LanguageHelper.GetTextContent(CSVLanguage.Instance.GetConfData(cSVActiveSkillInfoData.desc), System.Math.Round(cSVBuffData.ui_param1 / 100.0f, 1).ToString());
                                }
                            }
                            else
                            {
                                DebugUtil.LogError($"cSVBuffData is null id:{cSVActiveSkillEffectiveData.effect_to_target}");
                            }
                        }
                    }
                }
                else
                {
                    DebugUtil.LogError($"currentActiveSkillEffectiveData is null id:{cSVActiveSkillInfoData.effect_id}");
                }
            }
            else
            {
                DebugUtil.LogError($"CSVActiveSkillInfo.Data is null skillID:{skillID}");
            }

            return desc;
        }

        /// <summary>
        /// 宠物技能书显示(跟技能相关，暂且放这里)
        /// </summary>
        /// <param name="img"></param>
        /// <param name="itemData"></param>
        public void ShowPetSkillBook(UnityEngine.UI.Image img, CSVItem.Data itemData)
        {
            if (itemData == null || img == null)
                return;

            bool isSkillBook = (itemData.type_id == (uint)EItemType.PetSkillBook) || (itemData.type_id == (uint)EItemType.PetRemakeSkillBook || (itemData.type_id == (uint)EItemType.PetMountSkillBook));
            img.gameObject.SetActive(isSkillBook);
            if (isSkillBook)
            {
                if (itemData.fun_value != null)
                {
                    uint skillId = itemData.fun_value[1];
                    if (Sys_Skill.Instance.IsActiveSkill(skillId))
                    {
                        CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(skillId);
                        ImageHelper.SetIcon(img, skillInfo.icon);
                    }
                    else
                    {
                        CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                        ImageHelper.SetIcon(img, skillInfo.icon);
                    }
                }
                else
                {
                    DebugUtil.LogErrorFormat("技能书{0}没有功能参数", itemData.id);
                }

                img.transform.Find("Text_Num").GetComponent<UnityEngine.UI.Text>().text = LanguageHelper.GetTextContent(2007280 + itemData.lv);
            }
        }


        public void Get(SkillInfo skillInfo, out uint levelCostNum, out uint rankCostNum, out uint finalLevel, out uint finalRank)
        {
            levelCostNum = 0;
            rankCostNum = 0;
            finalLevel = skillInfo.Level;
            finalRank = skillInfo.Rank;
            CSVActiveSkillInfo.Data cSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(skillInfo.CurInfoID);
            if (cSVActiveSkillInfoData != null)
            {
                uint allNeedProficiency = 0;
                allNeedProficiency += (cSVActiveSkillInfoData.need_proficiency - skillInfo.CurProficiency);
                //DebugUtil.LogError($"SkillID:{skillInfo.SkillID} Add Proficiency: CurrentLevel:{skillInfo.Level}, CurrentRank:{skillInfo.Rank}, AddProficiencyValue:{cSVActiveSkillInfoData.need_proficiency - skillInfo.CurProficiency}");
                for (uint rank = skillInfo.Rank + 1; rank <= EACHLEVELRANKNUM; rank++)
                {
                    uint tempSkillID = skillInfo.SkillID * 1000 + (skillInfo.Level - 1) * EACHLEVELRANKNUM + rank;
                    var tempSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(tempSkillID);
                    allNeedProficiency += tempSkillInfoData.need_proficiency;
                    //DebugUtil.LogError($"SkillID:{skillInfo.SkillID} Add Proficiency: CurrentLevel:{skillInfo.Level}, CurrentRank:{rank}, AddProficiencyValue:{tempSkillInfoData.need_proficiency}");

                    finalRank = rank;
                }

                for (uint level = cSVActiveSkillInfoData.level + 1, maxLevel = skillInfo.MaxLevel; level <= maxLevel; level++)
                {
                    uint tempLevelOneRank = skillInfo.SkillID * 1000 + (level - 1) * EACHLEVELRANKNUM + 1;
                    var tempLevelOneRankData = CSVActiveSkillInfo.Instance.GetConfData(tempLevelOneRank);
                    if (tempLevelOneRankData.need_lv != 0 && Sys_Role.Instance.Role.Level < tempLevelOneRankData.need_lv)
                        break;
                    if (tempLevelOneRankData.SkillCareerLevelCond().ContainsKey(Sys_Role.Instance.Role.Career))
                    {
                        if (Sys_Role.Instance.Role.CareerRank < tempLevelOneRankData.SkillCareerLevelCond()[Sys_Role.Instance.Role.Career])
                            break;
                    }

                    if (tempLevelOneRankData.pre_task != 0 && Sys_Task.Instance.GetTaskState(tempLevelOneRankData.pre_task) != ETaskState.Submited)
                        break;

                    if (tempLevelOneRankData.upgrade_cost != null)
                    {
                        levelCostNum += tempLevelOneRankData.upgrade_cost[0][1];
                        //DebugUtil.LogError($"SkillID:{skillInfo.SkillID} Add levelCost: CurrentLevel:{level}, CurrentRank:{1}, AddlevelCostVaule:{tempLevelOneRankData.upgrade_cost[0][1]}");
                        finalLevel = level;
                    }

                    for (uint rank = 1; rank <= EACHLEVELRANKNUM; rank++)
                    {
                        uint tempSkillID = skillInfo.SkillID * 1000 + (level - 1) * EACHLEVELRANKNUM + rank;
                        var tempSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(tempSkillID);                       
                        allNeedProficiency += tempSkillInfoData.need_proficiency;
                        //DebugUtil.LogError($"SkillID:{skillInfo.SkillID} Add Proficiency: CurrentLevel:{level}, CurrentRank:{rank}, AddProficiencyValue:{tempSkillInfoData.need_proficiency}");

                        finalRank = rank;
                    }
                }

                //DebugUtil.LogError($"SkillID:{skillInfo.SkillID} All Proficiency:{allNeedProficiency}");
                rankCostNum = (uint)UnityEngine.Mathf.CeilToInt((float)allNeedProficiency / CSVParam.Instance.UpgradeSkillItemAddNum);
                //DebugUtil.LogError($"SkillID:{skillInfo.SkillID} All rankCostNum:{rankCostNum}");
                //DebugUtil.LogError($"SkillID:{skillInfo.SkillID} All levelCostNum:{levelCostNum}");
            }
            else
            {
                CSVPassiveSkillInfo.Data cSVPassiveSkillInfoData = CSVPassiveSkillInfo.Instance.GetConfData(skillInfo.CurInfoID);
                if (cSVPassiveSkillInfoData != null)
                {
                    uint allNeedProficiency = 0;
                    allNeedProficiency += (cSVPassiveSkillInfoData.max_adept - skillInfo.CurProficiency);
                    //DebugUtil.LogError($"SkillID:{skillInfo.SkillID} Add Proficiency: CurrentLevel:{skillInfo.Level}, CurrentRank:{skillInfo.Rank}, AddProficiencyValue:{cSVPassiveSkillInfoData.max_adept - skillInfo.CurProficiency}");
                    for (uint rank = skillInfo.Rank + 1; rank <= EACHLEVELRANKNUM; rank++)
                    {
                        uint tempSkillID = skillInfo.SkillID * 1000 + (skillInfo.Level - 1) * EACHLEVELRANKNUM + rank;
                        var tempSkillInfoData = CSVPassiveSkillInfo.Instance.GetConfData(tempSkillID);
                        allNeedProficiency += tempSkillInfoData.max_adept;
                        //DebugUtil.LogError($"SkillID:{skillInfo.SkillID} Add Proficiency: CurrentLevel:{skillInfo.Level}, CurrentRank:{rank}, AddProficiencyValue:{tempSkillInfoData.max_adept}");

                        finalRank = rank;
                    }

                    for (uint level = cSVPassiveSkillInfoData.level + 1, maxLevel = skillInfo.MaxLevel; level <= maxLevel; level++)
                    {
                        uint tempLevelOneRank = skillInfo.SkillID * 1000 + (level - 1) * EACHLEVELRANKNUM + 1;
                        var tempLevelOneRankData = CSVPassiveSkillInfo.Instance.GetConfData(tempLevelOneRank);
                        if (tempLevelOneRankData.need_lv != 0 && Sys_Role.Instance.Role.Level < tempLevelOneRankData.need_lv)
                            break;
                        if (tempLevelOneRankData.SkillCareerLevelCond().ContainsKey(Sys_Role.Instance.Role.Career))
                        {
                            if (Sys_Role.Instance.Role.CareerRank < tempLevelOneRankData.SkillCareerLevelCond()[Sys_Role.Instance.Role.Career])
                                break;
                        }

                        if (tempLevelOneRankData.pre_task != 0 && Sys_Task.Instance.GetTaskState(tempLevelOneRankData.pre_task) != ETaskState.Submited)
                            break;

                        if (tempLevelOneRankData.upgrade_cost != null)
                        {
                            levelCostNum += tempLevelOneRankData.upgrade_cost[0][1];
                            //DebugUtil.LogError($"SkillID:{skillInfo.SkillID} Add levelCost: CurrentLevel:{level}, CurrentRank:{1}, AddlevelCostVaule:{tempLevelOneRankData.upgrade_cost[0][1]}");
                            finalLevel = level;
                        }

                        for (uint rank = 1; rank <= EACHLEVELRANKNUM; rank++)
                        {
                            uint tempSkillID = skillInfo.SkillID * 1000 + (level - 1) * EACHLEVELRANKNUM + rank;
                            var tempSkillInfoData = CSVPassiveSkillInfo.Instance.GetConfData(tempSkillID);
                            allNeedProficiency += tempSkillInfoData.max_adept;
                            //DebugUtil.LogError($"SkillID:{skillInfo.SkillID} Add Proficiency: CurrentLevel:{level}, CurrentRank:{rank}, AddProficiencyValue:{tempSkillInfoData.max_adept}");

                            finalRank = rank;
                        }
                    }

                    //DebugUtil.LogError($"SkillID:{skillInfo.SkillID} All Proficiency:{allNeedProficiency}");
                    rankCostNum = (uint)UnityEngine.Mathf.CeilToInt((float)allNeedProficiency / CSVParam.Instance.UpgradeSkillItemAddNum);
                    //DebugUtil.LogError($"SkillID:{skillInfo.SkillID} All rankCostNum:{rankCostNum}");
                    //DebugUtil.LogError($"SkillID:{skillInfo.SkillID} All levelCostNum:{levelCostNum}");
                }
            }
        }

        #endregion
    }
}