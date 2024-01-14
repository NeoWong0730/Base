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
    /// <summary>
    /// 特殊处理 枚举值代表的是位, 与预设上的节点名称对应
    /// 需要修改预设显示顺序，直接修改预设
    /// </summary>
    public enum EmountScreemType
    {
        /// <summary> 已拥有的骑宠 </summary>
        MountDomestication = 0,
        /// <summary> 契约位 1个 </summary>
        Contract_1,
        /// <summary> 契约位 2个 </summary>
        Contract_2,
        /// <summary> 契约位 3个 </summary>
        Contract_3,
        /// <summary> 技能槽位 1个 </summary>
        MountSkillGrid_1,
        /// <summary> 技能槽位 2个 </summary>
        MountSkillGrid_2,
        /// <summary> 技能槽位 3个 </summary>
        MountSkillGrid_3,
        /// <summary> 技能槽位 4个 </summary>
        MountSkillGrid_4,
    }

    public partial class Sys_Pet : SystemModuleBase<Sys_Pet>
    {
        public uint mountPetUid;
        public uint followPetUid;
        public uint nextLimitTime;
        private Timer limitPetTimer;

        private uint mountSkillOpenLevel = 0;
        public uint MountSkillOpenLevel
        {
            get
            {
                if (mountSkillOpenLevel == 0)
                {
                    CSVPetNewParam.Data param = CSVPetNewParam.Instance.GetConfData(64u);
                    if (null != param)
                    {
                        mountSkillOpenLevel = param.value;
                    }
                }
                return mountSkillOpenLevel;
            }
        }

        private uint mountDomeOpenLevel = 0;
        public uint MountDomeOpenLevel
        {
            get
            {
                if (mountDomeOpenLevel == 0)
                {
                    CSVPetNewParam.Data param = CSVPetNewParam.Instance.GetConfData(65u);
                    if (null != param)
                    {
                        mountDomeOpenLevel = param.value;
                    }
                }
                return mountDomeOpenLevel;
            }
        }

        private uint mountEnergyItemId = 0;
        public uint MountEnergyItemId
        {
            get
            {
                if (mountEnergyItemId == 0)
                {
                    CSVPetNewParam.Data param = CSVPetNewParam.Instance.GetConfData(60u);
                    if (null != param)
                    {
                        var list = ReadHelper.ReadArray_ReadUInt(param.str_value, '|');
                        if (null != list && list.Count > 0)
                        {
                            mountEnergyItemId = list[0];
                        }
                    }
                }
                return mountEnergyItemId;
            }
        }

        public bool MountIsOpen
        {
            get
            {
                return Sys_Role.Instance.Role.Level >= MountSkillOpenLevel;
            }
        }

        public bool MountDomeIsOpen
        {
            get
            {
                return Sys_Role.Instance.Role.Level >= MountDomeOpenLevel;
            }
        }

        private uint energyLimit;
        public uint EnergyLimit
        {
            get
            {
                if(energyLimit == 0)
                {
                    var currentMaxParam = CSVPetNewParam.Instance.GetConfData(61);
                    if(null != currentMaxParam)
                    {
                        energyLimit = currentMaxParam.value;
                    }
                }

                return energyLimit;
            }
        }

        private uint[] levelBackLimit;
        public uint[] LevelBackLimit
        {
            get
            {
                if(null == levelBackLimit)
                {
                    var limitParam = CSVPetNewParam.Instance.GetConfData(76u);
                    if (null != limitParam)
                    {
                        levelBackLimit = ReadHelper.ReadArray_ReadUInt(limitParam.str_value, '|').ToArray();
                    }
                }
                return levelBackLimit;
            }
        }

        #region 服务器消息
        /// <summary>
        /// 请求上下坐骑
        /// </summary>
        /// <param name="uid">宠物唯一标识</param>
        public void OnPetSetCurrentMountReq(uint uid)
        {
            CmdPetSetCurrentMountReq req = new CmdPetSetCurrentMountReq();
            req.Uid = uid;
            NetClient.Instance.SendMessage((ushort)CmdPet.SetCurrentMountReq, req);
        }

        private void PetSetCurrentMountRes(NetMsg msg)
        {
            CmdPetSetCurrentMountRes dataRes = NetMsgUtil.Deserialize<CmdPetSetCurrentMountRes>(CmdPetSetCurrentMountRes.Parser, msg);
            uint uid = dataRes.Currentuid;
            mountPetUid = uid;
            if (uid != 0)
            {
                if (mountPetUid == followPetUid)
                {
                    followPetUid = 0;
                    GameCenter.mainHero.RemovePet();
                }
                SimplePet simplePet = GetSimplePetByUid(uid);
                if (null != simplePet)
                {
                    GameCenter.mainHero.animationComponent.UpdateHoldingAnimations(GameCenter.mainHero.heroBaseComponent.HeroID, GameCenter.mainHero.weaponComponent.CurWeaponID, CSVActionState.Instance.GetHeroPreLoadActions(), GameCenter.mainHero.stateComponent.CurrentState, GameCenter.mainHero.modelGameObject);

                    if (CheckMountGradeIsFullByUid(mountPetUid))
                    {
                        GameCenter.mainHero.OnMount(simplePet.PetId * 10 + 1, Sys_Role.Instance.RoleId * 1000000 + simplePet.PetId * 10 + 1, GetMountPetSuitFashionId(), (uint)GetMountPerfectRemakeCount(), IsMountPetShowDemonSpiritFx());
                    }                    else
                    {
                        GameCenter.mainHero.OnMount(simplePet.PetId * 10, Sys_Role.Instance.RoleId * 1000000 + simplePet.PetId * 10 + 1, GetMountPetSuitFashionId(), (uint)GetMountPerfectRemakeCount(), IsMountPetShowDemonSpiritFx());
                    }
                }
            }
            else
            {
                GameCenter.mainHero.OffMount();
            }
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnChangeStatePet);
        }

        /// <summary>
        /// 驯化坐骑
        /// </summary>
        /// <param name="uid">宠物唯一标识</param>
        public void OnPetMountdomesticationReq(uint uid)
        {
            CmdPetMountdomesticationReq req = new CmdPetMountdomesticationReq();
            req.Uid = uid;
            NetClient.Instance.SendMessage((ushort)CmdPet.MountdomesticationReq, req);
        }

        private void PetMountdomesticationRes(NetMsg msg)
        {
            CmdPetMountdomesticationRes dataRes = NetMsgUtil.Deserialize<CmdPetMountdomesticationRes>(CmdPetMountdomesticationRes.Parser, msg);
            uint uid = dataRes.Uid;
            for (int i = 0; i < petsList.Count; i++)
            {
                ClientPet pet = petsList[i];
                if (pet.GetPetUid() == uid)
                {
                    pet.petUnit.SimpleInfo.MountDomestication = 1;
                    Instance.eventEmitter.Trigger(EEvents.OnPetMountDomestication, uid);
                    break;
                }
            }

        }

        /// <summary>
        /// 坐骑过期请求删除
        /// </summary>
        public void OnPetMountExpiredReq()
        {
            nextLimitTime = 0;
            CmdPetMountExpiredReq req = new CmdPetMountExpiredReq();
            NetClient.Instance.SendMessage((ushort)CmdPet.MountExpiredReq, req);
        }

        private void PetMountExpiredRes(NetMsg msg)
        {
            CmdPetMountExpiredRes dataRes = NetMsgUtil.Deserialize<CmdPetMountExpiredRes>(CmdPetMountExpiredRes.Parser, msg);
            for (int i = 0; i < dataRes.Uid.Count; i++)
            {
                uint removeUid = dataRes.Uid[i];
                for (int j = 0; j < petsList.Count; j++)
                {
                    ClientPet pet = petsList[j];
                    if (removeUid == pet.GetPetUid())
                    {
                        if (mountPetUid == removeUid)
                        {
                            mountPetUid = 0;
                            GameCenter.mainHero.OffMount();
                        }
                        if (followPetUid == removeUid)
                        {
                            followPetUid = 0;
                            GameCenter.mainHero.RemovePet();
                        }
                        petsList.RemoveAt(j);
                        break;
                    }
                }
            }
            CheckNextLimitPet();
            Instance.eventEmitter.Trigger(EEvents.OnNumberChangePet);
        }

        public void OnPetSetFollowPetReq(uint uid)
        {
            CmdPetSetFollowPetReq req = new CmdPetSetFollowPetReq();
            req.Uid = uid;
            NetClient.Instance.SendMessage((ushort)CmdPet.SetFollowPetReq, req);
        }

        private void PetSetFollowPetRes(NetMsg msg)
        {
            CmdPetSetFollowPetRes dataRes = NetMsgUtil.Deserialize<CmdPetSetFollowPetRes>(CmdPetSetFollowPetRes.Parser, msg);
            uint uid = dataRes.Uid;
            followPetUid = uid;
            if (uid != 0)
            {
                if (mountPetUid == followPetUid)
                {
                    mountPetUid = 0;
                    GameCenter.mainHero.OffMount();
                }
                ClientPet simplePet = GetFightPetClient(uid);
                if (null != simplePet)
                {
                    GameCenter.mainHero.AddPet(simplePet.GetFollowPetInfo(), Sys_Role.Instance.RoleId * 1000000 + simplePet.petData.id * 10 + 2, GetFollowPetSuitFashionId(), (uint)GetFollowPerfectRemakeCount(), IsFollowPetShowDemonSpiritFx());
                }
            }
            else
            {
                GameCenter.mainHero.RemovePet();
            }
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnChangeStatePet);
        }

        /// <summary>
        /// 设置契约位宠物
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="index"></param>
        /// <param name="contractUid"></param>
        public void OnPetContractSetUpReq(uint uid, uint index, uint contractUid)
        {
            CmdPetContractSetUpReq req = new CmdPetContractSetUpReq();
            req.Uid = uid;
            req.Pos = index;
            req.ContractUid = contractUid;
            NetClient.Instance.SendMessage((ushort)CmdPet.ContractSetUpReq, req);
        }

        private void PetContractSetUpRes(NetMsg msg)
        {
            CmdPetContractSetUpRes dataRes = NetMsgUtil.Deserialize<CmdPetContractSetUpRes>(CmdPetContractSetUpRes.Parser, msg);
            for (int i = 0; i < petsList.Count; i++)
            {
                var _pet = petsList[i];
                if (dataRes.Uid == _pet.GetPetUid())
                {
                    var willPet = _pet.GetSubByIndex((int)dataRes.Pos);
                    if (willPet != 0)
                    {
                        var willClientPet = GetPetByUId(willPet);
                        if (null != willClientPet)
                        {
                            willClientPet.PartnerUid = 0;
                        }
                    }
                    _pet.SetContractInfoByPos(dataRes.Pos, dataRes.ContractUid);
                    var subPet = GetPetByUId(dataRes.ContractUid);
                    if (null != subPet)
                    {
                        subPet.PartnerUid = dataRes.Uid;
                    }
                    break;
                }
            }
            Sys_Pet.Instance.eventEmitter.Trigger(EEvents.OnContractChange);
        }

        /// <summary>
        /// 取消契约位宠物-将契约位上的宠物清除
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="index"></param>
        public void OnPetContractCancleReq(uint uid, List<uint> indexs)
        {
            CmdPetContractCancleReq req = new CmdPetContractCancleReq();
            req.Uid = uid;
            req.PosList.AddRange(indexs);
            NetClient.Instance.SendMessage((ushort)CmdPet.ContractCancleReq, req);
        }

        private void PetContractCancleRes(NetMsg msg)
        {
            CmdPetContractCancleRes dataRes = NetMsgUtil.Deserialize<CmdPetContractCancleRes>(CmdPetContractCancleRes.Parser, msg);

            for (int i = 0; i < petsList.Count; i++)
            {
                var _pet = petsList[i];
                if (dataRes.Uid == _pet.GetPetUid())
                {
                    for (int j = 0; j < dataRes.PosList.Count; j++)
                    {
                        _pet.SetContractInfoByPos(dataRes.PosList[j], 0);
                    }
                    
                    break;
                }
            }
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000229u));
            Sys_Pet.Instance.eventEmitter.Trigger(EEvents.OnContractChange);
            /*for (int i = 0; i < dataRes.Infos.Count; i++)
            {
                var info = dataRes.Infos[i];
                for (int j = 0; j < petsList.Count; j++)
                {
                    var _pet = petsList[j];
                    if (info.Uid == _pet.GetPetUid())
                    {
                        _pet.SetContractInfo(info.ContractPetUid, info.ContractUids);
                    }
                }
            }*/
        }

        /// <summary>
        /// 骑术技能学习
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="index"></param>
        /// <param name="itemId"></param>
        public void OnPetRidingSkillLearnReq(uint uid, uint itemId)
        {
            CmdPetRidingSkillLearnReq req = new CmdPetRidingSkillLearnReq();
            req.Uid = uid;
            req.ItemId = itemId;
            NetClient.Instance.SendMessage((ushort)CmdPet.RidingSkillLearnReq, req);
        }

        private void PetRidingSkillLearnRes(NetMsg msg)
        {
            CmdPetRidingSkillLearnRes dataRes = NetMsgUtil.Deserialize<CmdPetRidingSkillLearnRes>(CmdPetRidingSkillLearnRes.Parser, msg);
            uint skillId = dataRes.SkillId;
            /*for (int i = 0; i < petsList.Count; i++)
            {
                var _pet = petsList[i];
                if (dataRes.Uid == _pet.GetPetUid())
                {
                    _pet.SetMountSkillByIndex((int)dataRes.Pos, dataRes.SkillId);
                    break;
                }
            }*/
            bool isActiveSkill = Sys_Skill.Instance.IsActiveSkill(dataRes.SkillId);
            if (isActiveSkill) //主动技能
            {
                CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(skillId);
                if (skillInfo != null)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000211, LanguageHelper.GetTextContent(skillInfo.name)));//
                }
                else
                {
                    Debug.LogErrorFormat("not found skillId={0} in skillInfo", skillId);
                }
            }
            else
            {
                CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                if (skillInfo != null)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000211, LanguageHelper.GetTextContent(skillInfo.name)));
                }
                else
                {
                    Debug.LogErrorFormat("not found skillId={0}", skillId);
                }
            }

            Instance.eventEmitter.Trigger(EEvents.OnMountSkillChange);
        }

        /// <summary>
        /// 骑术技能遗忘
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="index"></param>
        public void OnPetRidingSkillForgetReq(uint uid, uint skillId)
        {
            CmdPetRidingSkillForgetReq req = new CmdPetRidingSkillForgetReq();
            req.Uid = uid;
            req.SkillId = skillId;
            NetClient.Instance.SendMessage((ushort)CmdPet.RidingSkillForgetReq, req);
        }

        private void PetRidingSkillForgetRes(NetMsg msg)
        {
            CmdPetRidingSkillForgetRes dataRes = NetMsgUtil.Deserialize<CmdPetRidingSkillForgetRes>(CmdPetRidingSkillForgetRes.Parser, msg);
            /*for (int i = 0; i < petsList.Count; i++)
            {
                var _pet = petsList[i];
                if (dataRes.Uid == _pet.GetPetUid())
                {
                    var skillId = _pet.GetMountSkillByIndex((int)dataRes.Pos);
                    
                    _pet.SetMountSkillByIndex((int)dataRes.Pos, 0);
                    break;
                }
            }*/
            uint skillId = dataRes.SkillId;
            bool isActiveSkill = Sys_Skill.Instance.IsActiveSkill(skillId);
            if (isActiveSkill) //主动技能
            {
                CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(skillId);
                if (skillInfo != null)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000225, LanguageHelper.GetTextContent(skillInfo.name)));//
                }
                else
                {
                    Debug.LogErrorFormat("not found skillId={0} in skillInfo", skillId);
                }
            }
            else
            {
                CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                if (skillInfo != null)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000225, LanguageHelper.GetTextContent(skillInfo.name)));
                }
                else
                {
                    Debug.LogErrorFormat("not found skillId={0}", skillId);
                }
            }
            Instance.eventEmitter.Trigger(EEvents.OnMountSkillChange);
        }

        /// <summary>
        /// 能量充能
        /// </summary>
        /// <param name="type"></param>
        public void OnPetRidingSkillAddEnergyReq(uint type)
        {
            CmdPetRidingSkillAddEnergyReq req = new CmdPetRidingSkillAddEnergyReq();
            req.Type = type;
            NetClient.Instance.SendMessage((ushort)CmdPet.RidingSkillAddEnergyReq, req);
        }

        /// <summary>
        /// 骑术能量变化通知
        /// </summary>
        /// <param name="msg"></param>
        private void PetRidingSkillEnergyChangeNtf(NetMsg msg)
        {
            CmdPetRidingSkillEnergyChangeNtf ntf = NetMsgUtil.Deserialize<CmdPetRidingSkillEnergyChangeNtf>(CmdPetRidingSkillEnergyChangeNtf.Parser, msg);
            RidingEnergy = ntf.EnergyNum;

            Instance.eventEmitter.Trigger(EEvents.OnEnergyChargeEnd);
        }

        /// <summary>
        /// 兑换坐骑技能书
        /// </summary>
        /// <param name="uid"></param>
        public void OnPetMountExchangeReq(uint uid)
        {
            CmdPetMountExchangeReq req = new CmdPetMountExchangeReq();
            req.Uid = uid;
            NetClient.Instance.SendMessage((ushort)CmdPet.MountExchangeReq, req);
        }

        /// <summary>
        /// 获取消耗
        /// </summary>
        /// <param name="uid"></param>
        public void OnPetGetRidingSkillCostReq()
        {
            CmdPetGetRidingSkillCostReq req = new CmdPetGetRidingSkillCostReq();
            for (int i = 0; i < petsList.Count; i++)
            {
                if (petsList[i].petData.mount && petsList[i].GetPetIsDomestication())
                {
                    req.PetUids.Add(petsList[i].GetPetUid());
                }
            }
            NetClient.Instance.SendMessage((ushort)CmdPet.GetRidingSkillCostReq, req);
        }

        /// <summary>
        /// 契约位发生变化通知
        /// </summary>
        /// <param name="msg"></param>
        private void PetContractInfoUpdateNtf(NetMsg msg)
        {
            CmdPetContractInfoUpdateNtf ntf = NetMsgUtil.Deserialize<CmdPetContractInfoUpdateNtf>(CmdPetContractInfoUpdateNtf.Parser, msg);
            for (int i = 0; i < ntf.Infos.Count; i++)
            {
                var info = ntf.Infos[i];
                for (int j = 0; j < petsList.Count; j++)
                {
                    var _pet = petsList[j];
                    if (info.Uid == _pet.GetPetUid())
                    {
                        _pet.SetContractInfo(info.ContractPetUid, info.ContractUids);
                    }
                }
            }

            Instance.eventEmitter.Trigger(EEvents.OnContractChange);
        }

        /// <summary>
        /// 定向兑换金宠
        /// </summary>
        public void OnPetExchangTargetGoldPetReq(uint keyId)
        {
            CmdPetExchangTargetGoldPetReq req = new CmdPetExchangTargetGoldPetReq();
            req.KeyId = keyId;
            NetClient.Instance.SendMessage((ushort)CmdPet.ExchangTargetGoldPetReq, req);
        }

        private void PetExchangTargetGoldPetRes(NetMsg msg)
        {
            CmdPetExchangTargetGoldPetRes dataRes = NetMsgUtil.Deserialize<CmdPetExchangTargetGoldPetRes>(CmdPetExchangTargetGoldPetRes.Parser, msg);
            
            Instance.eventEmitter.Trigger(EEvents.OnExchangTargetGoldPetSuccess);
        }

        private void PetFightGetLuckyBagNtf(NetMsg msg)
        {
            CmdPetFightGetLuckyBagNtf ntf = NetMsgUtil.Deserialize<CmdPetFightGetLuckyBagNtf>(CmdPetFightGetLuckyBagNtf.Parser, msg);


            string content = string.Empty;

            CSVItem.Data item = CSVItem.Instance.GetConfData(ntf.ItemId);
            if(null != item)
            {
                var skillId = ntf.SkillId;
                bool isActiveSkill = Sys_Skill.Instance.IsActiveSkill(skillId);
                if (isActiveSkill) //主动技能
                {
                    CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(skillId);
                    if (skillInfo != null)
                    {
                        content = LanguageHelper.GetTextContent(skillInfo.name);

                    }
                    else
                    {
                        DebugUtil.LogFormat(ELogType.eNone, "not found skillId={0} in skillInfo", skillId);
                    }
                }
                else
                {
                    CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                    if (skillInfo != null)
                    {
                        content = LanguageHelper.GetTextContent(skillInfo.name);
                    }
                    else
                    {
                        DebugUtil.LogFormat(ELogType.eNone, "not found skillId={0}", skillId);
                    }
                }

                content = LanguageHelper.GetErrorCodeContent(680001024, content, LanguageHelper.GetTextContent(item.name_id), ntf.CurNum.ToString(), ntf.MaxNum.ToString());
            }
            
            ErrorCodeHelper.PushErrorCode(CSVErrorCode.Instance.GetConfData(680001024).pos, content, content);
        }

        public void OnPetLevelDownReq(uint petUid)
        {
            CmdPetLevelDownReq req = new CmdPetLevelDownReq();
            req.PetUid = petUid;
            NetClient.Instance.SendMessage((ushort)CmdPet.LevelDownReq, req);
        }

        public void OnPetContractLevelUpReq(uint petUid, List<ulong> itemUuid)
        {
            CmdPetContractLevelUpReq req = new CmdPetContractLevelUpReq();
            req.Uid = petUid;
            for (int i = 0; i < itemUuid.Count; i++)
            {
                var uuid = itemUuid[i];
                var itemData = Sys_Bag.Instance.GetItemDataByUuid(uuid);
                if(null != itemData)
                {
                    SimpleItem simpleItem = new SimpleItem();
                    simpleItem.ItemInfoId = itemData.Id;
                    simpleItem.Count = 1;
                    req.CostItem.Add(simpleItem);
                }
            }
            NetClient.Instance.SendMessage((ushort)CmdPet.ContractLevelUpReq, req);
        }

        private void PetContractLevelUpRes(NetMsg msg)
        {
            Instance.eventEmitter.Trigger(EEvents.OnContractLevelUpRes);
        }
        #endregion

        #region Util
        public ClientPet GetMountPet()
        {
            if (mountPetUid != 0)
            {
                for (int i = 0; i < petsList.Count; i++)
                {
                    ClientPet pet = petsList[i];
                    if (pet.GetPetUid() == mountPetUid)
                    {
                        return pet;
                    }
                }
            }
            return null;
        }

        public long GetMountPetSpeed()
        {
            if (mountPetUid != 0)
            {
                for (int i = 0; i < petsList.Count; i++)
                {
                    ClientPet pet = petsList[i];
                    if (pet.GetPetUid() == mountPetUid)
                    {
                        return pet.GetAttrValueByAttrId(101);
                    }
                }
            }
            return 0;
        }

        public ClientPet GetFollwPet()
        {
            if (followPetUid != 0)
            {
                for (int i = 0; i < petsList.Count; i++)
                {
                    ClientPet pet = petsList[i];
                    if (pet.GetPetUid() == followPetUid)
                    {
                        return pet;
                    }
                }
            }
            return null;
        }

        private bool CheckIsLimitPet(uint petUid)
        {
            ClientPet checkPet = GetPetByUId(petUid);
            if (null != checkPet)
            {
                if (checkPet.petUnit.SimpleInfo.ExpiredTick != 0)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000654));
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 检查是否刷新过期定时
        /// </summary>
        /// <param name="timer">替换时间</param>
        /// <param name="checkReplace">true 为判断检测， false 为直接替换</param>
        public void ResetLimitPetTimer(uint timer, bool checkReplace)
        {
            if (checkReplace)
            {
                if (nextLimitTime == 0 && timer > 0)
                {
                    nextLimitTime = timer;
                }
                else if (timer > 0 && nextLimitTime > 0 && timer < nextLimitTime)
                {
                    nextLimitTime = timer;
                }
            }
            else
            {
                nextLimitTime = timer;
            }

            if (nextLimitTime > 0)
            {
                limitPetTimer?.Cancel();
                uint serverTime = Sys_Time.Instance.GetServerTime();
                if (nextLimitTime > serverTime)
                {
                    limitPetTimer = Timer.Register(nextLimitTime - serverTime, OnPetMountExpiredReq, useRealTime: true);
                }
                else
                {
                    OnPetMountExpiredReq();
                }
            }
            else
            {
                limitPetTimer?.Cancel();
            }
        }

        public void CheckNextLimitPet()
        {
            uint minTime = 0;
            for (int i = 0; i < petsList.Count; i++)
            {
                ClientPet pet = petsList[i];
                uint limit = pet.petUnit.SimpleInfo.ExpiredTick;
                if ((limit > 0 && minTime == 0) || (limit > 0 && minTime < limit))
                {
                    minTime = limit;
                }
            }
            ResetLimitPetTimer(minTime, false);
        }

        public bool IsLastPetEntExpiredTick(bool ExpiredTick)
        {
            if (null != petsList && petsList.Count > 0)
            {
                uint limitNum = 0;
                for (int i = 0; i < petsList.Count; i++)
                {
                    ClientPet pet = petsList[i];
                    if (pet.petUnit.SimpleInfo.ExpiredTick > 0)
                    {
                        limitNum += 1;
                    }
                }
                if (ExpiredTick)
                {
                    return false;
                }
                else
                {
                    return (petsList.Count - limitNum - 1) <= 0;
                }
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// 是否有出战宠物 未点数量达到N点 全局表 1057
        /// </summary>
        /// <returns></returns>
        public bool IsHasFightPetPointNotUse()
        {
            if (currentPetUid == 0 || null == fightPet)
            {
                return false;
            }
            if (Sys_Ini.Instance.Get(1057u, out IniElement_Int level))
            {
                ClientPet pet = GetFightPetClient(currentPetUid);
                if (null != pet)
                {
                    pet.baseAttrs.TryGetValue(EBaseAttr.SurplusPoint, out long has);
                    if (has >= level.value)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 排序规则：已驯化的且没有建立契约的坐骑；未驯化的坐骑；已驯化且建立契约的坐骑
        /// </summary>
        public List<ClientPet> GetMountConversionList()
        {
            List<ClientPet> domesAndNotHasSub = new List<ClientPet>(petsList.Count);
            List<ClientPet> noDomes = new List<ClientPet>(petsList.Count);
            List<ClientPet> domesAndHasSub = new List<ClientPet>(petsList.Count);
            List<ClientPet> fightPetOrRidPet = new List<ClientPet>(2);
            for (int i = 0; i < petsList.Count; i++)
            {
                var pet = petsList[i];
                if (pet.petData.mount)
                {
                    if (pet.IsHasMountUnique())
                    {
                        var _uid = pet.GetPetUid();
                        if (Sys_Pet.Instance.fightPet.IsSamePet(_uid) || Sys_Pet.Instance.mountPetUid == _uid)
                        {
                            fightPetOrRidPet.Add(pet);
                        }
                        else if (pet.GetPetIsDomestication())
                        {
                            //是否有契约宠物
                            if (pet.HasPartnerPet() || pet.HasSubPet())
                            {
                                domesAndHasSub.Add(pet);
                            }
                            else
                            {
                                domesAndNotHasSub.Add(pet);
                            }
                        }
                        else
                        {
                            noDomes.Add(pet);
                        }
                    }
                }
            }

            domesAndNotHasSub.AddRange(noDomes);
            domesAndNotHasSub.AddRange(domesAndHasSub);
            domesAndNotHasSub.AddRange(fightPetOrRidPet);
            return domesAndNotHasSub;
        }

        /// <summary>
        /// 获取属于坐骑的宠物
        /// </summary>
        public List<ClientPet> GetMountClientPets()
        {
            List<ClientPet> domeMouts = new List<ClientPet>(petsList.Count);
            List<ClientPet> mouts = new List<ClientPet>(petsList.Count);
            for (int i = 0; i < petsList.Count; i++)
            {
                var pet = petsList[i];
                if (pet.petData.mount)
                {
                    if (pet.GetPetIsDomestication())
                    {
                        domeMouts.Add(pet);
                    }
                    else
                    {
                        mouts.Add(pet);
                    }
                }
            }
            if (domeMouts.Count > 1)
            {
                domeMouts.Sort(MountComp);
            }

            domeMouts.AddRange(mouts);
            return domeMouts;
        }


        /// <summary>
        /// 获取属于坐骑的图鉴宠物
        /// </summary>
        public List<uint> GetMountPetByConfigs()
        {
            List<uint> mouts = new List<uint>(32);
            var petNew = CSVPetNew.Instance.GetAll();
            for (int i = 0, len = petNew.Count; i < len; i++)
            {
                var pet = petNew[i];
                if (PetBookChannelShow(pet) &&  pet.mount && !CheckMoutCanShowWithView(pet))
                {
                    mouts.Add(pet.id);
                }
            }
            if (mouts.Count > 1)
            {
                mouts.Sort(MountSort);
            }
            return mouts;
        }

        public int MountComp(ClientPet a, ClientPet b)
        {
            int aH = a.HasSubPet() ? 1 : 0;
            int bH = b.HasSubPet() ? 1 : 0;
            int re = -aH.CompareTo(bH);
            if (re == 0)
            {
                re = -a.mountData.skill_grid.CompareTo(b.mountData.skill_grid);
                if (re == 0)
                {
                    return -a.petData.id.CompareTo(b.petData.id);
                }
            }
            return re;
        }

        /// <summary>
        /// 获取骑宠契约列表
        /// </summary>
        public List<ClientPet> GetPetsExceptParamId(uint petId)
        {
            List<ClientPet> mouts = new List<ClientPet>(petsList.Count);
            for (int i = 0; i < petsList.Count; i++)
            {
                var pet = petsList[i];
                if (pet.GetPetUid() != petId)
                {
                    mouts.Add(pet);
                }
            }
            if (mouts.Count > 1)
            {
                mouts.Sort(MountLevelComp);
            }

            return mouts;
        }

        public int MountLevelComp(ClientPet a, ClientPet b)
        {
            int re = -a.petUnit.SimpleInfo.Level.CompareTo(b.petUnit.SimpleInfo.Level);
            if (re == 0)
            {
                return -a.petData.id.CompareTo(b.petData.id);
            }
            return re;
        }

        public CSVPetMountAttr.Data GetAttrListByGreatAndId(int id, uint gread)
        {
            var petMountAttrDatas = CSVPetMountAttr.Instance.GetAll();
            for (int i = 0, len = petMountAttrDatas.Count; i < len; i++)
            {
                var data = petMountAttrDatas[i];
                if (id == data.group_id && gread <= data.gear_param)
                {
                    return data;
                }
            }
            return null;
        }

        /// <summary>
        /// 检查 是否过了技能获取消耗的时间
        /// </summary>
        public void CheckGetSkillCostReset()
        {
            if (Sys_Time.Instance.GetServerTime() >= Sys_Pet.Instance.SkillCostResetTick + 86400)
            {
                Sys_Pet.Instance.SkillCostResetTick = Sys_Pet.Instance.SkillCostResetTick + 86400;
                Sys_Pet.Instance.OnPetGetRidingSkillCostReq();
            }
        }

        /// <summary>
        /// 检查 坐骑是否满档
        /// </summary>
        public bool CheckMountGradeIsFullByUid(uint petUid)
        {
            for (int i = 0; i < petsList.Count; i++)
            {
                var _pet = petsList[i];

                if(_pet.GetPetUid() == petUid && _pet.GetPetMaxGradeCount() == _pet.GetPetGradeCount())
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region 坐骑删选

        /// <summary>
        /// 通过位比较获得状态
        /// </summary>
        /// <param name="setBit">子类型id</param>
        /// <returns>true 参选打勾</returns>
        public bool GetScreemTypeState(uint setBit)
        {
            return GetBitvalue((int)setBit);
        }

        /// <summary>
        /// 获取bit 是否为1
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool GetBitvalue(int index)
        {
            if ((ulong)index > 64)
                return false;
            
            ulong value = (mountScreemBitEntry.mountScreemBit & (1ul << index));
            return value > 0;
        }

        /// <summary>
        /// 将bit位设为1
        /// </summary>
        /// <param name="setBit"></param>
        public void SetBitvalue(uint setBit, ref ulong setValue)
        {
            int bit = (int)setBit;
            if (bit >= 64)
            {
                return;
            }
            setValue = setValue | (1ul << bit);
        }

        /// <summary>
        /// 将bit位设为0
        /// </summary>
        /// <param name="setBit"></param>
        public void SetBitValueZeo(uint setBit, ref ulong setValue)
        {
            int bit = (int)setBit;
            if (bit >= 64)
            {
                return;
            }
            setValue = setValue & (~(1ul << bit));
        }
        public class MountScreemBitEntry
        {
            public ulong mountScreemBit;
        }
        private MountScreemBitEntry mountScreemBitEntry = new MountScreemBitEntry();
        public readonly string MountScreeFileName = "MountScreemSaveFile";
        private void ParseScreemJson(JsonObject jsonvalue)
        {
            JsonHeler.DeserializeObject(jsonvalue, mountScreemBitEntry);
        }

        private void LoadMountScreeDB()
        {
            var jsonValue = FileStore.ReadJson(MountScreeFileName);
            if (jsonValue == null)
            {
                mountScreemBitEntry.mountScreemBit = 0;
                var petParam = CSVPetNewParam.Instance.GetConfData(62u);
                if (null != petParam)
                {
                    List<List<uint>> data = ReadHelper.ReadArray2_ReadUInt(petParam.str_value, '|', '&');
                    for (int i = 0; i < data.Count; i++)
                    {
                        if (data[i][1] == 1)
                        {
                            Sys_Pet.Instance.SetBitvalue((uint)i, ref mountScreemBitEntry.mountScreemBit);
                        }
                    }
                }
                SaveMountScreeDB();
                return;
            }
            ParseScreemJson(jsonValue);
        }

        private void SaveMountScreeDB()
        {
            FileStore.WriteJson(MountScreeFileName, mountScreemBitEntry);
        }

        public void SaveMountScreeDB(ulong bitValue)
        {
            mountScreemBitEntry.mountScreemBit = bitValue;
            SaveMountScreeDB();
        }

        public ulong GetScreemBit()
        {
            return mountScreemBitEntry.mountScreemBit;
        }
        #endregion

        #region 推荐骑宠删选条件

        private bool CheckMoutCanShowWithView(CSVPetNew.Data checkData)
        {
            return IsHasMount(checkData) || ContractNum_1(checkData) || ContractNum_2(checkData)
                || ContractNum_3(checkData) || MountSkillGrid_1(checkData) ||MountSkillGrid_2(checkData)
                || MountSkillGrid_3(checkData) || MountSkillGrid_4(checkData);
        }

        /// <summary>
        /// 已拥有的骑宠
        /// </summary>
        /// <param name="cSVPetNew"></param>
        /// <returns></returns>
        private bool IsHasMount(CSVPetNew.Data checkData)
        {
            bool _value = GetScreemTypeState((uint)EmountScreemType.MountDomestication);
            bool hasPet = IsHasSamePet(checkData);
            return (!_value && hasPet);
        }

        private bool IsHasSamePet(CSVPetNew.Data checkData)
        {
            if (null == checkData)
                return false;
            for (int i = 0; i < petsList.Count; i++)
            {
                if(petsList[i].petData.id == checkData.id)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 契约位数量
        /// </summary>
        /// <param name="cSVPetNew"></param>
        /// <returns></returns>
        private bool ContractNum_1(CSVPetNew.Data checkData)
        {
            bool _value = GetScreemTypeState((uint)EmountScreemType.Contract_1);

            return !_value && ContractNum(checkData, 1);
        }

        private bool ContractNum_2(CSVPetNew.Data checkData)
        {
            bool _value = GetScreemTypeState((uint)EmountScreemType.Contract_2);

            return !_value && ContractNum(checkData, 2);
        }

        private bool ContractNum_3(CSVPetNew.Data checkData)
        {
            bool _value = GetScreemTypeState((uint)EmountScreemType.Contract_3);

            return !_value && ContractNum(checkData, 3);
        }

        private bool ContractNum(CSVPetNew.Data checkData, int num)
        {
            if (null == checkData)
                return false;
            CSVPetMount.Data moutData = CSVPetMount.Instance.GetConfData(checkData.id);
            if(null == moutData)
                return false;

            return null != moutData.indenture_effect && moutData.indenture_effect.Count == num;
        }

        // <summary>
        /// 契约位数量
        /// </summary>
        /// <param name="cSVPetNew"></param>
        /// <returns></returns>
        private bool MountSkillGrid_1(CSVPetNew.Data checkData)
        {
            bool _value = GetScreemTypeState((uint)EmountScreemType.MountSkillGrid_1);

            return !_value && MountSkillGrid(checkData, 1);
        }

        private bool MountSkillGrid_2(CSVPetNew.Data checkData)
        {
            bool _value = GetScreemTypeState((uint)EmountScreemType.MountSkillGrid_2);

            return !_value && MountSkillGrid(checkData, 2);
        }

        private bool MountSkillGrid_3(CSVPetNew.Data checkData)
        {
            bool _value = GetScreemTypeState((uint)EmountScreemType.MountSkillGrid_3);

            return !_value && MountSkillGrid(checkData, 3);
        }

        private bool MountSkillGrid_4(CSVPetNew.Data checkData)
        {
            bool _value = GetScreemTypeState((uint)EmountScreemType.MountSkillGrid_4);

            return !_value && MountSkillGrid(checkData, 4);
        }

        private bool MountSkillGrid(CSVPetNew.Data checkData, int num)
        {
            if (null == checkData)
                return false;
            CSVPetMount.Data moutData = CSVPetMount.Instance.GetConfData(checkData.id);
            if (null == moutData)
                return false;

            return moutData.skill_grid == num;
        }

        #endregion
    }
}
