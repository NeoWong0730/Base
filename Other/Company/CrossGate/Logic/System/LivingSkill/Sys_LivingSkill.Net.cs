using Packet;
using Logic.Core;
using Lib.Core;
using Net;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google.Protobuf.Collections;
using Table;
using System;

namespace Logic
{
    public partial class Sys_LivingSkill : SystemModuleBase<Sys_LivingSkill>
    {
        public Dictionary<uint, LivingSkill> livingSkills = new Dictionary<uint, LivingSkill>();
        private List<uint> openFormulas = new List<uint>();
        private uint grade;
        public uint Grade
        {
            get { return grade; }
            set
            {
                if (grade != value)
                {
                    grade = value;
                }
            }
        }
        private bool canGradeLevelUp = false;
        public bool CanGradeLevelUp
        {
            get { return canGradeLevelUp; }
            set
            {
                if (canGradeLevelUp != value)
                {
                    canGradeLevelUp = value;
                }
            }
        }
        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        public List<uint> UnfixedFomulaItems = new List<uint>();                        //不固定配方
        public bool bIntensify;            //否强化打造
        public ulong uuid;     //淬炼精华uuid
        public bool bHasItem;
        public Button refrenceButton;
        public uint canLearnNum;        //手动解锁可学习的个数(不包括除初始的可学习的数量)
        public uint freeLearnNum;
        public List<List<int>> cost = new List<List<int>>();

        public uint SkillId { get; private set; }

        public enum EEvents
        {
            OnUpdateExp,            //更新经验
            OnUpdateGrade,          //更新段位
            OnLevelUp,              //升级
            OnUpdateLevelUpButtonState,//更新升级按钮状态
            OnRefreshUnfixFormulaSelectItems,   //更新不固定配方方案
            OnSetHardenItem,      //选择不固定配方方案(更新右上角按钮)
            OnLearnedSkill,          //学习新技能
            OnMakeSuccess,           //制造成功
            OnPlayLevelUpFx,        //技能等级提升
            OnPlayGradeUpFx,        //段位等级提升
            OnRefreshLifeSkillMessage,//刷新生活技能界面
            OnSkipToFormula,        //跳转到对应配方
            OnEquip,                //穿戴装备
            OnCloseEquipTips,       //关闭装备tips
            OnUpdateCanLearn,       //更新制造系可学习状态
            OnForgetSkill,          //遗忘技能
            OnUpdateLuckyValue,     //更新幸运值
        }

        public override void Init()
        {
            ParseClientData();
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdLifeSkill.LearnFormulaBookNtf, OnLearnFormulaBookNtf, CmdLifeSkillLearnFormulaBookNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdLifeSkill.AddSkillExpNtf, OnAddSkillExpNtf, CmdLifeSkillAddSkillExpNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdLifeSkill.SkillGetNtf, OnSkillGetNtf, CmdLifeSkillSkillGetNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdLifeSkill.PullLifeSkillReq, (ushort)CmdLifeSkill.PullLifeSkillRes, OnPullLifeSkillRes, CmdLifeSkillPullLifeSkillRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdLifeSkill.FormulaBuildReq, (ushort)CmdLifeSkill.FormulaBuildRes, OnFormulaBuildRes, CmdLifeSkillFormulaBuildRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdLifeSkill.SegmentLevelUpReq, (ushort)CmdLifeSkill.SegmentLevelUpRes, OnSegmentLevelUpRes, CmdLifeSkillSegmentLevelUpRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdLifeSkill.SkillLevelUpReq, (ushort)CmdLifeSkill.SkillLevelUpRes, OnSkillLevelUpRes, CmdLifeSkillSkillLevelUpRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdLifeSkill.FunctionOpenReq, (ushort)CmdLifeSkill.FunctionOpenRes, OnFunctionOpenRes, CmdLifeSkillFunctionOpenRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdLifeSkill.CanLearnNumAddReq, (ushort)CmdLifeSkill.CanLearnNumAddRes, LifeSkillCanLearnNumAddRes, CmdLifeSkillCanLearnNumAddRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdLifeSkill.SkillForgetReq, (ushort)CmdLifeSkill.SkillForgetRes, LifeSkillSkillForgetRes, CmdLifeSkillSkillForgetRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdLifeSkill.AddLuckyValueNtf, OnAddLuckyValueNtf, CmdLifeSkillAddLuckyValueNtf.Parser);
            Sys_FunctionOpen.Instance.eventEmitter.Handle<Sys_FunctionOpen.FunctionOpenData>(Sys_FunctionOpen.EEvents.CompletedFunctionOpen, OnCompletedFunctionOpen, true);
            Sys_Net.Instance.eventEmitter.Handle<bool>(Sys_Net.EEvents.OnReconnectResult, OnReconnectResult, true);
        }

        private void OnReconnectResult(bool res)
        {
            UIManager.CloseUI(EUIID.UI_LifeSkill_Message);
        }

        public void RegisterButton(Button button)
        {
            refrenceButton = button;
        }

        public void UnRegisterButton()
        {
            refrenceButton = null;
        }


        public override void Dispose()
        {
            Sys_FunctionOpen.Instance.eventEmitter.Handle<Sys_FunctionOpen.FunctionOpenData>(Sys_FunctionOpen.EEvents.CompletedFunctionOpen, OnCompletedFunctionOpen, false);
            Sys_Net.Instance.eventEmitter.Handle<bool>(Sys_Net.EEvents.OnReconnectResult, OnReconnectResult, false);
        }

        private void OnCompletedFunctionOpen(Sys_FunctionOpen.FunctionOpenData functionOpenData)
        {
            if (functionOpenData.id == 10701)
            {
                FunctionOpenReq();
            }
        }

        private void FunctionOpenReq()
        {
            CmdLifeSkillFunctionOpenReq cmdLifeSkillFunctionOpenReq = new CmdLifeSkillFunctionOpenReq();
            NetClient.Instance.SendMessage((ushort)CmdLifeSkill.FunctionOpenReq, cmdLifeSkillFunctionOpenReq);
        }


        private void OnFunctionOpenRes(NetMsg netMsg)
        {

        }



        public override void OnLogin()
        {
            PullLifeSkillReq();
        }

        private void ParseClientData()
        {
            for (uint i = 1; i < (uint)LifeSkillType.Collect + 1; i++)
            {
                LivingSkill livingSkill = new LivingSkill();
                livingSkill.SetSkillType((LifeSkillType)i);
                livingSkill.Level = 0;
                livingSkill.Proficiency = 0;
                livingSkills.Add(i, livingSkill);
            }
            freeLearnNum = uint.Parse(CSVParam.Instance.GetConfData(651).str_value);
            Sys_Ini.Instance.Get<IniElement_IntArray2>(652, out IniElement_IntArray2 array);
            cost = array.value;
        }


        private void PullLifeSkillReq()
        {
            CmdLifeSkillPullLifeSkillReq cmdLifeSkillPullLifeSkillReq = new CmdLifeSkillPullLifeSkillReq();
            NetClient.Instance.SendMessage((ushort)CmdLifeSkill.PullLifeSkillReq, cmdLifeSkillPullLifeSkillReq);
        }

        private void OnPullLifeSkillRes(NetMsg netMsg)
        {
            openFormulas.Clear();
            CmdLifeSkillPullLifeSkillRes res = NetMsgUtil.Deserialize<CmdLifeSkillPullLifeSkillRes>(CmdLifeSkillPullLifeSkillRes.Parser, netMsg);
            Grade = res.LifeSkillData.Segment;
            RepeatedField<uint> skillLevels = res.LifeSkillData.PerSkillLevel;
            RepeatedField<uint> skillExps = res.LifeSkillData.PerSkillExp;
            RepeatedField<uint> openedFormulas = res.LifeSkillData.OpenedFormula;
            RepeatedField<uint> skillLucky = res.LifeSkillData.PerSkillLucky;
            var orangeCounts = res.LifeSkillData.OrangeCount;
            for (int i = 1, count = skillLevels.Count; i < count; i++)
            {
                if (livingSkills.TryGetValue((uint)i, out LivingSkill livingSkill))
                {
                    livingSkill.Level = skillLevels[i];
                    livingSkill.Proficiency = skillExps[i];
                    livingSkill.luckyValue = skillLucky[i];
                    livingSkill.orangeCount = orangeCounts[i];
                    if (livingSkill.Level > 0)
                    {
                        List<uint> addlists = GetUnlockSkillFormulasByLevel((uint)i, livingSkill.Level);
                        foreach (var item in addlists)
                        {
                            openFormulas.AddOnce(item);
                        }
                    }
                }
                else
                {
                    DebugUtil.LogErrorFormat($"找不到技能id:{i}");
                }
            }
            for (int i = 0, count = openedFormulas.Count; i < count; i++)
            {
                openFormulas.AddOnce(openedFormulas[i]);
            }
            canLearnNum = res.LifeSkillData.CanLearnNum;
        }


        public void FormulaBuildReq(uint formulaId, bool useIntensifyBuild, ulong essenceuuId, List<uint> inputId)
        {
            CmdLifeSkillFormulaBuildReq cmdLifeSkillFormulaBuildReq = new CmdLifeSkillFormulaBuildReq();
            cmdLifeSkillFormulaBuildReq.FormulaID = formulaId;
            cmdLifeSkillFormulaBuildReq.UseIntensifyBuild = useIntensifyBuild;
            cmdLifeSkillFormulaBuildReq.EssenceUUID = essenceuuId;
            foreach (var item in inputId)
            {
                cmdLifeSkillFormulaBuildReq.InputItemID.Add(item);
            }
            NetClient.Instance.SendMessage((ushort)CmdLifeSkill.FormulaBuildReq, cmdLifeSkillFormulaBuildReq);
        }

        private void OnFormulaBuildRes(NetMsg netMsg)
        {
            CmdLifeSkillFormulaBuildRes res = NetMsgUtil.Deserialize<CmdLifeSkillFormulaBuildRes>(CmdLifeSkillFormulaBuildRes.Parser, netMsg);

            uint skillid = res.SkillId;
            string skillName = CSVLanguage.Instance.GetConfData(CSVLifeSkill.Instance.GetConfData(skillid).name_id).words;

            string itemId_get = CSVLanguage.Instance.GetConfData(CSVItem.Instance.GetConfData(res.ItemInfoId[0]).name_id).words;
            int itemId_getCount = res.ItemInfoId.Count;
            uint quality = 1;
            CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(res.ItemInfoId[0]);
            if (cSVItemData.type_id == (uint)EItemType.Equipment)
            {
                ItemData itemData = Sys_Bag.Instance.GetItemDataByUuid(res.Equipuuid[0]);
                if (itemData == null)
                {
                    DebugUtil.LogErrorFormat("找不到uuid={0}的装备", res.Equipuuid[0]);
                }
                else
                {
                    quality = itemData.Quality;
                }
            }
            else
            {
                quality = cSVItemData.quality;
            }
            if (res.Result == 0)
            {
                string content = LanguageHelper.GetTextContent(2010126, skillName, Constants.gHintColors_Items[quality - 1], itemId_get, itemId_getCount.ToString());
                Sys_Hint.Instance.PushContent_Normal(content);
            }

            else
            {
                string content = LanguageHelper.GetTextContent(2010124, skillName, Constants.gHintColors_Items[quality - 1], itemId_get, itemId_getCount.ToString());
                Sys_Hint.Instance.PushContent_Normal(content);
            }

            if (res.ExtraItemInfoId.Count > 0)
            {
                string itemId_extra_get = CSVLanguage.Instance.GetConfData(CSVItem.Instance.GetConfData(res.ExtraItemInfoId[0]).name_id).words;
                int itemId_extragetCount = res.ExtraItemInfoId.Count;
                string content = LanguageHelper.GetTextContent(2010125, skillName, Constants.gHintColors_Items[quality - 1], itemId_extra_get, itemId_extragetCount.ToString());
                Sys_Hint.Instance.PushContent_Normal(content);
            }
            if (res.Equipuuid.Count == 0)
            {
                eventEmitter.Trigger<ulong>(EEvents.OnMakeSuccess, 0);
            }
            else
            {
                eventEmitter.Trigger<ulong>(EEvents.OnMakeSuccess, res.Equipuuid[0]);
            }
        }

        public void SegmentLevelUpReq()
        {
            CmdLifeSkillSegmentLevelUpReq cmdLifeSkillSegmentLevelUpReq = new CmdLifeSkillSegmentLevelUpReq();
            NetClient.Instance.SendMessage((ushort)CmdLifeSkill.SegmentLevelUpReq, cmdLifeSkillSegmentLevelUpReq);
        }

        private void OnSegmentLevelUpRes(NetMsg netMsg)
        {
            CmdLifeSkillSegmentLevelUpRes res = NetMsgUtil.Deserialize<CmdLifeSkillSegmentLevelUpRes>(CmdLifeSkillSegmentLevelUpRes.Parser, netMsg);
            Grade = res.CurrentSegment;
            UpdateGradeCanLock();
            eventEmitter.Trigger(EEvents.OnUpdateGrade);
            eventEmitter.Trigger(EEvents.OnPlayGradeUpFx);
            CSVLifeSkillRank.Data cSVLifeSkillRankData = CSVLifeSkillRank.Instance.GetConfData(Sys_LivingSkill.Instance.Grade);
            string content = string.Format(CSVLanguage.Instance.GetConfData(2010105).words, Sys_LivingSkill.Instance.Grade.ToString(), cSVLifeSkillRankData.level_max.ToString());
            Sys_Hint.Instance.PushContent_Normal(content);
        }

        public void SkillLevelUpReq(uint skillIndex)
        {
            CmdLifeSkillSkillLevelUpReq cmdLifeSkillSkillLevelUpReq = new CmdLifeSkillSkillLevelUpReq();
            cmdLifeSkillSkillLevelUpReq.SkillIndex = skillIndex;
            NetClient.Instance.SendMessage((ushort)CmdLifeSkill.SkillLevelUpReq, cmdLifeSkillSkillLevelUpReq);
        }

        private void OnSkillLevelUpRes(NetMsg netMsg)
        {
            CmdLifeSkillSkillLevelUpRes res = NetMsgUtil.Deserialize<CmdLifeSkillSkillLevelUpRes>(CmdLifeSkillSkillLevelUpRes.Parser, netMsg);
            uint skillId = res.SkillIndex;
            if (livingSkills.ContainsKey(skillId))
            {
                if (res.Level >= 1)
                {
                    List<uint> addlists = GetUnlockSkillFormulasByLevel(skillId, res.Level);
                    foreach (var item in addlists)
                    {
                        openFormulas.AddOnce(item);
                    }
                    eventEmitter.Trigger(EEvents.OnPlayLevelUpFx);
                    //Sys_MagicBook.Instance.eventEmitter.Trigger<EMagicAchievement, uint>(Sys_MagicBook.EEvents.MagicTaskCheckEvent2, EMagicAchievement.Event21, res.Level);
                }
                else
                {
                    DebugUtil.LogErrorFormat($"技能 {skillId} 升级失败");
                }
                livingSkills[skillId].Level = res.Level;
                livingSkills[skillId].Proficiency = res.NowExp;
            }
            else
            {
                DebugUtil.LogErrorFormat($"找不到技能id:{skillId}");
            }
        }

        private void OnLearnFormulaBookNtf(NetMsg netMsg)
        {
            CmdLifeSkillLearnFormulaBookNtf res = NetMsgUtil.Deserialize<CmdLifeSkillLearnFormulaBookNtf>(CmdLifeSkillLearnFormulaBookNtf.Parser, netMsg);
            openFormulas.AddOnce(res.FormulaID);
        }

        private void OnAddSkillExpNtf(NetMsg netMsg)
        {
            CmdLifeSkillAddSkillExpNtf res = NetMsgUtil.Deserialize<CmdLifeSkillAddSkillExpNtf>(CmdLifeSkillAddSkillExpNtf.Parser, netMsg);
            LivingSkill livingSkill = livingSkills[res.SkillIndex];
            //使用道具
            if (res.Reason == 1)
            {
                if (livingSkill.bExpFull)
                {
                    //string ExpFullcontent = CSVLanguage.Instance.GetConfData(2010110).words;
                    //Sys_Hint.Instance.PushContent_Normal(ExpFullcontent);
                }
                livingSkill.Proficiency = res.CurrentExp;
            }
            else
            {
                uint extra = res.CurrentExp - livingSkill.Proficiency;
                string skillName = CSVLanguage.Instance.GetConfData(livingSkill.cSVLifeSkillData.name_id).words;
                string content = string.Format(CSVLanguage.Instance.GetConfData(2010117).words, extra.ToString(), skillName);
                Sys_Hint.Instance.PushContent_Normal(content);
                livingSkill.Proficiency = res.CurrentExp;
            }
        }

        private void OnSkillGetNtf(NetMsg netMsg)
        {
            CmdLifeSkillSkillGetNtf res = NetMsgUtil.Deserialize<CmdLifeSkillSkillGetNtf>(CmdLifeSkillSkillGetNtf.Parser, netMsg);
            livingSkills[res.Skillid].Level = 1;
            List<uint> addlists = GetUnlockSkillFormulasByLevel(res.Skillid, 1);
            foreach (var item in addlists)
            {
                openFormulas.AddOnce(item);
            }
            eventEmitter.Trigger(EEvents.OnLearnedSkill);
            eventEmitter.Trigger(EEvents.OnUpdateCanLearn);
            //Sys_MagicBook.Instance.eventEmitter.Trigger<EMagicAchievement>(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.Event20);
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010153, livingSkills[res.Skillid].name));
        }

        public void LifeSkillLearnSkillReq(uint skillId)
        {
            CmdLifeSkillLearnSkillReq cmdLifeSkillLearnSkillReq = new CmdLifeSkillLearnSkillReq();
            cmdLifeSkillLearnSkillReq.SkillId = skillId;
            NetClient.Instance.SendMessage((ushort)CmdLifeSkill.LearnSkillReq, cmdLifeSkillLearnSkillReq);
        }

        public void LifeSkillCanLearnNumAddReq()
        {
            CmdLifeSkillCanLearnNumAddReq cmdLifeSkillCanLearnNumAddReq = new CmdLifeSkillCanLearnNumAddReq();
            NetClient.Instance.SendMessage((ushort)CmdLifeSkill.CanLearnNumAddReq, cmdLifeSkillCanLearnNumAddReq);
        }

        private void LifeSkillCanLearnNumAddRes(NetMsg netMsg)
        {
            CmdLifeSkillCanLearnNumAddRes cmdLifeSkillCanLearnNumAddRes = NetMsgUtil.Deserialize<CmdLifeSkillCanLearnNumAddRes>(CmdLifeSkillCanLearnNumAddRes.Parser, netMsg);
            canLearnNum = cmdLifeSkillCanLearnNumAddRes.LearnNum;
            eventEmitter.Trigger(EEvents.OnUpdateCanLearn);
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010155, (canLearnNum + freeLearnNum).ToString()));
        }

        public void LifeSkill_SkillForgetReq(uint skillId)
        {
            CmdLifeSkillSkillForgetReq cmdLifeSkillSkillForgetReq = new CmdLifeSkillSkillForgetReq();
            cmdLifeSkillSkillForgetReq.SkillId = skillId;
            NetClient.Instance.SendMessage((ushort)CmdLifeSkill.SkillForgetReq, cmdLifeSkillSkillForgetReq);
        }

        private void LifeSkillSkillForgetRes(NetMsg netMsg)
        {
            CmdLifeSkillSkillForgetRes cmdLifeSkillSkillForgetRes = NetMsgUtil.Deserialize<CmdLifeSkillSkillForgetRes>(CmdLifeSkillSkillForgetRes.Parser, netMsg);

            livingSkills[cmdLifeSkillSkillForgetRes.SkillId].Level = 0;
            eventEmitter.Trigger(EEvents.OnForgetSkill);
            eventEmitter.Trigger(EEvents.OnUpdateLuckyValue, cmdLifeSkillSkillForgetRes.SkillId, 0);
        }

        private void OnAddLuckyValueNtf(NetMsg netMsg)
        {
            CmdLifeSkillAddLuckyValueNtf cmdLifeSkillSkillForgetRes = NetMsgUtil.Deserialize<CmdLifeSkillAddLuckyValueNtf>(CmdLifeSkillAddLuckyValueNtf.Parser, netMsg);
            livingSkills[cmdLifeSkillSkillForgetRes.SkillId].luckyValue = cmdLifeSkillSkillForgetRes.LuckyValue;
            livingSkills[cmdLifeSkillSkillForgetRes.SkillId].orangeCount = cmdLifeSkillSkillForgetRes.OrangeCount;
        }
    }
}


