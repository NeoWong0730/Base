using Google.Protobuf;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using System;
using System.Collections.Generic;
using System.Json;
using Table;
using UnityEngine;
namespace Logic
{
    public partial class Sys_Pet : SystemModuleBase<Sys_Pet>
    {
        public bool isDemonSpiritSkillRefresh = false;
        private List<uint> demonSpiritRemakeTimesByEquipLevel;
        public List<uint> DemonSpiritRemakeTimesByEquipLevel
        {
            get
            {
                if (null == demonSpiritRemakeTimesByEquipLevel)
                {
                    CSVPetNewParam.Data param = CSVPetNewParam.Instance.GetConfData(79u);
                    if(null != param)
                    {
                        demonSpiritRemakeTimesByEquipLevel = ReadHelper.ReadArray_ReadUInt(param.str_value, '|');
                    }
                }
                return demonSpiritRemakeTimesByEquipLevel;
            }
        }

        public List<uint> GetDemonSkillGroup(uint type)
        {
            var petParam = CSVPetNewParam.Instance.GetConfData(81u);
            List<uint> grouds = new List<uint>(2);
            if(null != petParam)
            {
                var allGroud = ReadHelper.ReadArray2_ReadUInt(petParam.str_value, '|', '&');
                if(null != allGroud)
                {
                    var count = allGroud.Count;
                    for (int i = 0; i < allGroud.Count; i++)
                    {
                        var _goruds = allGroud[i];
                        if (null != _goruds && _goruds.Count >= 2 && _goruds[0] == type)
                        {
                            grouds.AddRange(_goruds.GetRange(1, 2));
                            break;
                        }
                    }
                }
            }
            return grouds;
        }

        private List<uint> equipActiveLevelLimit;
        public List<uint> EquipActiveLevelLimit
        {
            get
            {
                if(null == equipActiveLevelLimit)
                {
                    CSVPetNewParam.Data param = CSVPetNewParam.Instance.GetConfData(82);
                    if (null != param)
                    {
                        equipActiveLevelLimit = ReadHelper.ReadArray_ReadUInt(param.str_value, '|');
                    }
                }
                return equipActiveLevelLimit;
            }
        }

        private List<uint> costPetExpList;
        public List<uint> CostPetExpList
        {
            get
            {
                if (null == costPetExpList)
                {
                    costPetExpList = new List<uint>(CSVSoulBeadPetAddexp.Instance.GetKeys());
                    
                    costPetExpList.Sort();
                }
                return costPetExpList;
            }
        }

        private int showDemonSpiritFxLevel = -1;
        public int ShowDemonSpiritFxLevel
        {
            get
            {
                if(showDemonSpiritFxLevel == -1)
                {
                    CSVPetNewParam.Data param = CSVPetNewParam.Instance.GetConfData(80u);
                    if (null != param)
                    {
                        showDemonSpiritFxLevel = (int)param.value;
                    }
                }
                return showDemonSpiritFxLevel;
            }
        }

        private string demonSpiritFxString = string.Empty;
        public string DemonSpiritFxString
        {
            get
            {
                if (demonSpiritFxString.Equals(string.Empty))
                {
                    CSVParam.Data param = CSVParam.Instance.GetConfData(1575);
                    if (null != param)
                    {
                        demonSpiritFxString = param.str_value;
                    }
                }
                return demonSpiritFxString;
            }
        }

        public List<PetSoulBeadInfo> petSoulBeadInfos = new List<PetSoulBeadInfo>();

        public PetUpgradeBeadInfo petUpgradeBeadInfo;

        public List<ClientPet> GetPetDemonSpiritActives(ClientPet targetClientPet)
        {
            List<ClientPet> samePets = new List<ClientPet>(petsList.Count);
            var targetUid = targetClientPet.GetPetUid();
            if(null != targetClientPet.petData.soul_activate_cost && targetClientPet.petData.soul_activate_cost.Count >= 3)
            {
                var targetPoint = targetClientPet.petData.soul_activate_cost[2];
                var targetGrade = targetClientPet.petData.soul_activate_cost[1];
                
                for (int i = 0; i < petsList.Count; i++)
                {
                    var pet = petsList[i];
                    var lowGrade = pet.GetPetMaxGradeCount() - pet.GetPetGradeCount();
                    if (pet.petUnit.SimpleInfo.ExpiredTick == 0 //非限时宠物
                        && pet.GetPetUid() != targetUid //非自身
                        && targetClientPet.petData.id == pet.petData.id //同名
                        /*&& pet.petUnit.SimpleInfo.Score >= targetPoint*/ //评分达标
                        && ((targetGrade == 0 && lowGrade == 0) || targetGrade != 0)) //是否满档需求
                    {
                        samePets.Add(pet);
                    }
                }
            }
            
            if (samePets.Count > 1)
            {
                samePets.Sort(DemonActivePetLowPointComp);
            }

            return samePets;
        }

        /// <summary>
        /// 增加改造次数
        /// </summary>
        /// <param name="petUid"></param>
        /// <param name="index">左边为0右边为1 或者相反也可以</param>
        /// <param name="costPetUid">消耗宠物的Uid 如果没有填0</param>
        public void PetSoulAddRemakeCountReq(uint petUid, uint index, uint costPetUid)
        {
            CmdPetSoulAddRemakeCountReq req = new CmdPetSoulAddRemakeCountReq();
            req.PetUid = petUid;
            req.Index = index;
            req.CostPetUid = costPetUid;
            NetClient.Instance.SendMessage((ushort)CmdPet.SoulAddRemakeCountReq, req);
        }

        private void OnPetSoulAddRemakeCountRes(NetMsg msg)
        {
            //CmdPetSoulAddRemakeCountRes res = NetMsgUtil.Deserialize<CmdPetSoulAddRemakeCountRes>(CmdPetSoulAddRemakeCountRes.Parser, msg);
            /*for (int i = 0; i < petsList.Count; i++)
            {
                var pet = petsList[i];

                if (pet.GetPetUid() == res.PetUid)
                {
                    pet.petUnit.PetSoulUnit.RemakeCount = res.RemakeCount;
                }
            }*/
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnActiveDemonSpiritRemake);
        }

        /// <summary>
        /// 激活专属魔魂
        /// </summary>
        /// <param name="petUid"></param>
        /// <param name="costPetUid">消耗宠物的Uid 如果没有填0</param>
        public void PetSoulActiveReq(uint petUid, uint costPetUid)
        {
            CmdPetSoulActiveReq req = new CmdPetSoulActiveReq();
            req.PetUid = petUid;
            req.CostPetUid = costPetUid;
            NetClient.Instance.SendMessage((ushort)CmdPet.SoulActiveReq, req);
        }

        private void OnPetSoulActiveRes(NetMsg msg)
        {
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnActiveDemonSpirit);
        }

        /// <summary>
        /// 装配/卸下魂珠
        /// </summary>
        /// <param name="petUid"></param>
        /// <param name="index"></param>
        /// <param name="beadType">类型(1-4)</param>
        /// <param name="opType">1:穿戴 2:卸下</param>
        public void PetSoulAssembleBeadReq(uint petUid, uint index, uint beadType, uint opType)
        {
            CmdPetSoulAssembleBeadReq req = new CmdPetSoulAssembleBeadReq();
            req.PetUid = petUid;
            req.Index = index;
            req.BeadType = beadType;
            req.OpType = opType;
            NetClient.Instance.SendMessage((ushort)CmdPet.SoulAssembleBeadReq, req);
        }

        private void OnPetSoulAssembleBeadRes(NetMsg msg)
        {
            CmdPetSoulAssembleBeadRes res = NetMsgUtil.Deserialize<CmdPetSoulAssembleBeadRes>(CmdPetSoulAssembleBeadRes.Parser, msg);
            for (int i = 0; i < petsList.Count; i++)
            {
                //服务器的数据为互相标记 即在宠物标记魂珠的位置 也在魂珠标记宠物uid
                //故需要处理数据的赋值
                if (petsList[i].GetPetUid() == res.PetUid)
                {
                    petsList[i].petUnit.PetSoulUnit.SoulBeads[(int)res.BeadType - 1] = res.Index;
                    uint setUid = 0;
                    //穿戴类型 分2种情况 原本位置上有 以及没有的
                    if (res.OpType == 1)
                    {
                        setUid = res.PetUid;
                        var index = petsList[i].petUnit.PetSoulUnit.SoulBeads[(int)res.BeadType - 1];
                        if(index != 0)
                        {
                            var petSoulBead = GetPetDemonSpiritSphereInfo(res.BeadType, res.Index);
                            petSoulBead.PetUid = 0;
                        }
                        petsList[i].petUnit.PetSoulUnit.SoulBeads[(int)res.BeadType - 1] = res.Index;
                    }
                    else if(res.OpType == 2)
                    {
                        setUid = 0;
                        petsList[i].petUnit.PetSoulUnit.SoulBeads[(int)res.BeadType - 1] = 0;
                    }

                    for (int j = 0; j < petSoulBeadInfos.Count; j++)
                    {
                        if (petSoulBeadInfos[j].Type == res.BeadType && petSoulBeadInfos[j].Index == res.Index)
                        {
                            petSoulBeadInfos[j].PetUid = setUid;
                        }
                    }
                    Sys_Pet.Instance.eventEmitter.Trigger<uint,bool>(Sys_Pet.EEvents.OnEquipDemonSpiritSphereLevelChange, res.PetUid, Sys_Pet.Instance.IsNeedShowDemonSpiritFx(res.PetUid)); 
                }
            }
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnEquipDemonSpiritSphere);
        }

        //PetSoulBeadOpType
        //SoulBeadOpType_Null = 0;
        //SoulBeadOpType_Active = 1;//激活魂珠
        //SoulBeadOpType_LevelUpItem = 2;//升级魂珠-道具
        //SoulBeadOpType_LevelUpPet = 3;//升级魂珠-宠物
        //SoulBeadOpType_RandSkill = 4;//随机魂珠技能
        //SoulBeadOpType_SaveSkill = 5;//保存魂珠技能
        //SoulBeadOpType_DeleteSkill = 6;//放弃魂珠技能

        /// <summary>
        /// 魂珠的各种操作
        /// </summary>
        /// <param name="opType">enum  // SoulBeadOpType_Null = 0;
        ///SoulBeadOpType_Active = 1;//激活魂珠
        ///SoulBeadOpType_LevelUpItem = 2;//升级魂珠-道具
        ///SoulBeadOpType_LevelUpPet = 3;//升级魂珠-宠物
        ///SoulBeadOpType_RandSkill = 4;//随机魂珠技能
        ///SoulBeadOpType_SaveSkill = 5;//保存魂珠技能
        ///SoulBeadOpType_DeleteSkill = 6;//放弃魂珠技能 </param>
        /// <param name="index"></param>
        /// <param name="beadType"></param>
        /// <param name="param"></param>
        public void PetSoulBeadOperateReq(uint opType, uint index, uint beadType, uint param)
        {
            CmdPetSoulBeadOperateReq req = new CmdPetSoulBeadOperateReq();
            req.OpType = opType;
            req.Index = index;
            req.BeadType = beadType;
            req.Param = param;
            NetClient.Instance.SendMessage((ushort)CmdPet.SoulBeadOperateReq, req);
        }

        private void OnPetSoulBeadOperateRes(NetMsg msg)
        {
            CmdPetSoulBeadOperateRes res = NetMsgUtil.Deserialize<CmdPetSoulBeadOperateRes>(CmdPetSoulBeadOperateRes.Parser, msg);
            if (res.OpType == (uint)PetSoulBeadOpType.SoulBeadOpTypeActive)
            {
                UI_Pet_DemonSuccessParam param = new UI_Pet_DemonSuccessParam();
                param.petSoulBeadOpType = (uint)PetSoulBeadOpType.SoulBeadOpTypeActive;
                param.skill1 = 0;
                param.skill2 = 0;
                param.petSoulBeadInfo = res.Info;
                UIManager.OpenUI(EUIID.UI_Pet_DemonSuccess, false, param);
                petSoulBeadInfos.Add(res.Info);
                Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnActiveDemonSpiritSphere);
            }
            else
            {
                for (int i = 0; i < petSoulBeadInfos.Count; i++)
                {
                    if (res.Info.Type == petSoulBeadInfos[i].Type && res.Info.Index == petSoulBeadInfos[i].Index)
                    {
                        uint changeSkill = 0;

                        if (res.OpType == (uint)PetSoulBeadOpType.SoulBeadOpTypeSaveSkill)
                        {
                            for (int j = 0; j < petSoulBeadInfos[i].SkillIds.Count; j++)
                            {
                                if (petSoulBeadInfos[i].SkillIds[j] != res.Info.SkillIds[j])
                                {
                                    changeSkill = res.Info.SkillIds[j];
                                }
                            }
                            if (Sys_Skill.Instance.IsActiveSkill(changeSkill)) //主动技能
                            {
                                CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(changeSkill);
                                if (skillInfo != null)
                                {
                                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002022, LanguageHelper.GetTextContent(skillInfo.name)));
                                }
                                else
                                {
                                    Debug.LogErrorFormat("not found skillId={0} in  CSVActiveSkillInfoData", changeSkill);
                                }
                            }
                            else
                            {
                                CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(changeSkill);
                                if (skillInfo != null)
                                {
                                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002022, LanguageHelper.GetTextContent(skillInfo.name)));
                                }
                                else
                                {
                                    Debug.LogErrorFormat("not found skillId={0} in CSVPassiveSkillInfoData", changeSkill);
                                }
                            }
                            petSoulBeadInfos[i] = res.Info;
                            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnSaveOrDelectDemonSpiritSphereSkill);
                        }
                        else if (res.OpType == (uint)PetSoulBeadOpType.SoulBeadOpTypeDeleteSkill)
                        {
                            for (int j = 0; j < petSoulBeadInfos[i].TempSkillIds.Count; j++)
                            {
                                if (petSoulBeadInfos[i].TempSkillIds[j] != res.Info.TempSkillIds[j])
                                {
                                    changeSkill = petSoulBeadInfos[i].TempSkillIds[j];
                                }
                            }
                            if (Sys_Skill.Instance.IsActiveSkill(changeSkill)) //主动技能
                            {
                                CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(changeSkill);
                                if (skillInfo != null)
                                {
                                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002024, LanguageHelper.GetTextContent(skillInfo.name)));
                                }
                                else
                                {
                                    Debug.LogErrorFormat("not found skillId={0} in  CSVActiveSkillInfoData", changeSkill);
                                }
                            }
                            else
                            {
                                CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(changeSkill);
                                if (skillInfo != null)
                                {
                                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002024, LanguageHelper.GetTextContent(skillInfo.name)));
                                }
                                else
                                {
                                    Debug.LogErrorFormat("not found skillId={0} in CSVPassiveSkillInfoData", changeSkill);
                                }
                            }
                            petSoulBeadInfos[i] = res.Info;
                            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnSaveOrDelectDemonSpiritSphereSkill);
                        }
                        else if (res.OpType == (uint)PetSoulBeadOpType.SoulBeadOpTypeLevelUpItem || res.OpType == (uint)PetSoulBeadOpType.SoulBeadOpTypeLevelUpPet)
                        {
                            if(res.Info.Level > petSoulBeadInfos[i].Level)
                            {
                                UI_Pet_DemonSuccessParam param = new UI_Pet_DemonSuccessParam();
                                param.petSoulBeadOpType = (uint)PetSoulBeadOpType.SoulBeadOpTypeLevelUpItem;
                                param.skill1 = petSoulBeadInfos[i].SkillIds[0];
                                param.skill2 = petSoulBeadInfos[i].SkillIds[1];
                                param.petSoulBeadInfo = res.Info;
                                UIManager.OpenUI(EUIID.UI_Pet_DemonSuccess, false, param);
                            }
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002015, LanguageHelper.GetTextContent(680003010u + res.Info.Type - 1), res.AddExp.ToString()));
                            petSoulBeadInfos[i] = res.Info;
                            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnDemonSpiritUpgrade, res.ExpRate); 
                        }
                        else
                        {
                            petSoulBeadInfos[i] = res.Info;
                            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnRefreshDemonSpiritSkill);
                        }
                    }
                }

            }
            
        }

        private void PetSoulBeadInitNtf(NetMsg msg)
        {
            CmdPetSoulBeadInitNtf ntf = NetMsgUtil.Deserialize<CmdPetSoulBeadInitNtf>(CmdPetSoulBeadInitNtf.Parser, msg);
            petSoulBeadInfos.Clear();
            petSoulBeadInfos.AddRange(ntf.AllBeads);
            petUpgradeBeadInfo = ntf.UpgradeInfo;
        }

        private void PetSoulBeadUpgradeInfoNtf(NetMsg msg)
        {
            CmdPetSoulBeadUpgradeInfoNtf ntf = NetMsgUtil.Deserialize<CmdPetSoulBeadUpgradeInfoNtf>(CmdPetSoulBeadUpgradeInfoNtf.Parser, msg);
            petUpgradeBeadInfo = ntf.UpgradeInfo;
        }

        public List<ClientPet> GetPetDemonSpiritRemakeActives(ClientPet targetClientPet, uint type)
        {
            List<ClientPet> samePets = new List<ClientPet>(petsList.Count);
            var targetUid = targetClientPet.GetPetUid();
            List<uint> activeData = null;
            if(type == 1)
            {
                activeData = targetClientPet.petData.extra1_remake_cost;
            }
            else if(type ==2 )
            {
                activeData = targetClientPet.petData.extra2_remake_cost;
            }
            if (null != activeData && activeData.Count >= 3)
            {
                var targetPoint = activeData[2];
                var targetGrade = activeData[1];

                for (int i = 0; i < petsList.Count; i++)
                {
                    var pet = petsList[i];
                    var lowGrade = pet.GetPetMaxGradeCount() - pet.GetPetGradeCount();
                    if (pet.petUnit.SimpleInfo.ExpiredTick == 0 //非限时宠物
                        && pet.GetPetUid() != targetUid //非自身
                        && targetClientPet.petData.id == pet.petData.id //同名
                        && pet.petUnit.SimpleInfo.Score >= targetPoint //评分达标
                        && ((targetGrade == 0 && lowGrade == 0) || targetGrade != 0)) //是否满档需求
                    {
                        samePets.Add(pet);
                    }
                }
            }

            if (samePets.Count > 1)
            {
                samePets.Sort(DemonActivePetLowPointComp);
            }

            return samePets;
        }

        public List<ClientPet> GetPetDemonSpiritUpgrades(uint sphereId)
        {
            List<ClientPet> samePets = new List<ClientPet>(petsList.Count);
            var sphereData = CSVSoulBead.Instance.GetConfData(sphereId);
            if(null != sphereData)
            {
                var targetPoint = sphereData.pet_score;
                var targetType = sphereData.pet_type;

                for (int i = 0; i < petsList.Count; i++)
                {
                    var pet = petsList[i];
                    if (pet.petUnit.SimpleInfo.ExpiredTick == 0 //非限时宠物
                        && pet.petUnit.SimpleInfo.Score >= targetPoint //评分达标
                        && targetType <= pet.petData.card_type //卡片类型正确
                        && !IsUniquePet(pet.petData.id)) //不能是唯一宠物

                    {
                        samePets.Add(pet);
                    }
                }
            }
            if (samePets.Count > 1)
            {
                samePets.Sort(DemonActivePetLowPointComp);
            }

            return samePets;
        }

        public int DemonActivePetLowPointComp(ClientPet a, ClientPet b)
        {
            return a.petUnit.SimpleInfo.Score.CompareTo(b.petUnit.SimpleInfo.Score);
        }

        public uint GetAddDemonSpiritExpId(uint score)
        {
            for (int i = CostPetExpList.Count -1; i >= 0; i--)
            {
                if(score >= CostPetExpList[i])
                {
                    return CostPetExpList[i];
                }
            }
            return 0;
        }

        //获取指定类型的魂珠
        public List<PetSoulBeadInfo> GetMySpheres(uint type, bool getAll)
        {
            List<PetSoulBeadInfo> vs = new List<PetSoulBeadInfo>(16);
            int allCount = petSoulBeadInfos.Count;
            uint typeCount = 0;
            for (int i = 0; i < allCount; i++)
            {
                var _petSoulBeadInfo = petSoulBeadInfos[i];

                if (_petSoulBeadInfo.Type == type)
                {
                    typeCount++;
                    bool isNotEquip = _petSoulBeadInfo.PetUid == 0;
                    if (getAll || (!getAll && isNotEquip))
                    {
                        vs.Add(petSoulBeadInfos[i]);
                    }
                }
            }
            CSVSoulBeadUnlock.Data unLock = CSVSoulBeadUnlock.Instance.GetConfData(type * 10000 + (uint)typeCount + 1);
            if(null != unLock)
            {
                PetSoulBeadInfo temp = new PetSoulBeadInfo();
                temp.Index = typeCount + 1;
                temp.Type = type;
                temp.Level = 0;
                vs.Add(temp);
            }
            return vs;
        }

        /// <summary>
        /// 获取一个数据通过 类型和下标
        /// </summary>
        /// <param name="type"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public PetSoulBeadInfo GetPetDemonSpiritSphereInfo(uint type, uint index)
        {
            int allCount = petSoulBeadInfos.Count;
            for (int i = 0; i < allCount; i++)
            {
                var _petSoulBeadInfo = petSoulBeadInfos[i];
                if(_petSoulBeadInfo.Type == type && index == _petSoulBeadInfo.Index)
                {
                    return _petSoulBeadInfo;
                }
            }
            return null;
        }

         
    }
}
