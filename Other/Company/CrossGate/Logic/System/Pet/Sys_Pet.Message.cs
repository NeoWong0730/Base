using Google.Protobuf;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using Table;
using static Packet.PetPkAttr.Types;
using System.Collections;
namespace Logic
{

    public partial class Sys_Pet : SystemModuleBase<Sys_Pet>
    {
        private EPetUiType petUiType;
        private MessageEx practiceEx;
        bool isInitData = false;

        public uint tipsPetUid;
        private uint maxLoyalty = 0;
        public uint MaxLoyalty
        {
            get
            {
                if(maxLoyalty == 0)
                {
                    CSVPetNewParam.Data paramData = CSVPetNewParam.Instance.GetConfData(20u);
                    if(null != paramData)
                    {
                        maxLoyalty = paramData.value;
                    }
                }
                return maxLoyalty;                
            }

            private set { }
        }

        private List<uint> exchangeScoreList;
        public List<uint> ExchangeScoreList
        {
            get
            {
                if (null == exchangeScoreList)
                {
                    exchangeScoreList = new List<uint>(CSVPetNewExchange.Instance.GetKeys());
                    //for (int i = 0; i < CSVPetNewExchange.Instance.Count; i++)
                    //{
                    //    exchangeScoreList.Add(CSVPetNewExchange.Instance[i].id);
                    //}
                }
                return exchangeScoreList;
            }

            private set { }
        }

        public uint MaxAutoBlinkNum
        {
            get
            {
                CSVCharacterAttribute.Data data = CSVCharacterAttribute.Instance.GetConfData(Sys_Role.Instance.Role.Level);
                if(null != data)
                {
                    return data.pet_summon_num;
                }
                return 0;
            }
            private set { }
        }

        private uint maxLevel2Battle = 0;
        public uint MaxLevel2Battle
        {
            get
            {
                if (maxLevel2Battle == 0)
                {
                    CSVPetNewParam.Data paramData = CSVPetNewParam.Instance.GetConfData(7u);
                    if (null != paramData)
                    {
                        maxLevel2Battle = paramData.value;
                    }
                }
                return maxLevel2Battle;
            }

            private set { }
        }

        public uint devicesCount
        {
            get
            {
                return bagNum + costBagNum;
            }
            set
            {
            }
        }
        public uint bagNum
        {
            get
            {
                return GetBagLevelUnLockNum();
            }
            set
            {
            }
        }
        public uint costBagNum;

        private int maxLevelListCount;
        private int maxCostListCout;

        public int MaxLevelListCount
        {
            get
            {
                if (maxLevelListCount == 0)
                {
                    CSVPetNewParam.Data paramData = CSVPetNewParam.Instance.GetConfData(14u);
                    if (null != paramData)
                    {
                        maxLevelListCount = paramData.str_value.Split('|').Length;;
                    }
                }
                return maxLevelListCount;
            }

            private set { }
        }

        public int MaxCostListCout
        {
            get
            {
                if (maxCostListCout == 0)
                {
                    CSVPetNewParam.Data paramData = CSVPetNewParam.Instance.GetConfData(15u);
                    if (null != paramData)
                    {
                        maxCostListCout = paramData.str_value.Split('|').Length; ;
                    }
                }
                return maxCostListCout;
            }

            private set { }
        }

        private List<uint> bagLevelList;
        public uint GetBagLevel(int index)
        {
            if(null == bagLevelList)
            {
                bagLevelList = new List<uint>();
                CSVPetNewParam.Data paramData = CSVPetNewParam.Instance.GetConfData(14u);
                if (null != paramData)
                {                   
                    string[] strs = paramData.str_value.Split('|') ;
                    for (int i = 0; i < strs.Length; i++)
                    {
                        bagLevelList.Add(Convert.ToUInt32(strs[i]));
                    }
                }                
            }

            if (0 <= index && index < bagLevelList.Count)
            {
                return bagLevelList[index];
            }
            else
            {
                return 0;
            }
        }

        public uint[] petLevelUnLockData;

        public uint[] PetLevelUnLockData
        {
            get
            {
                if (null == petLevelUnLockData)
                {
                    CSVPetNewParam.Data paramData = CSVPetNewParam.Instance.GetConfData(14u);
                    if (null != paramData)
                    {
                        string[] strs = paramData.str_value.Split('|');
                        petLevelUnLockData = new uint[strs.Length];
                        for (int i = 0; i < strs.Length; i++)
                        {
                            petLevelUnLockData[i] = Convert.ToUInt32(strs[i]);
                        }
                    }
                    else
                    {
                        petLevelUnLockData = new uint[1];
                    }
                    
                }
                return petLevelUnLockData;
            }

            private set { }
        }

        public uint GetBagLevelUnLockNum()
        {
            uint index = 0;
            for (int i = 0; i < PetLevelUnLockData.Length; i++)
            {
                if (Sys_Role.Instance.Role.Level >= PetLevelUnLockData[i])
                {
                    index = (uint)i;
                }
            }
            return index + 1;
        }

        public List<uint> GetBagCountUnLockData()
        {
            CSVPetNewParam.Data paramData = CSVPetNewParam.Instance.GetConfData(15u);
            if (null != paramData)
            {
                string[] strs = paramData.str_value.Split('|');
                if(0<= costBagNum && costBagNum < strs.Length)
                {
                    string[] cost = strs[costBagNum].Split('&');
                    List<uint> levelAndCost = new List<uint>();
                    for (int i = 0; i < cost.Length; i++)
                    {
                        levelAndCost.Add(Convert.ToUInt32(cost[i]));
                    }
                    return levelAndCost;
                }
            }
            return null;
        }

        private ushort minExChangeScore;

        public ushort MinExChangeScore
        {
            get
            {
                if(minExChangeScore == 0)
                {
                    minExChangeScore = (ushort)ExchangeScoreList.Min();
                }
                return minExChangeScore;
            }
        }

        private uint bindPetAbandonCoin = 0;

        public uint BindPetAbandonCoin
        {
            get
            {
                if (bindPetAbandonCoin == 0)
                {
                    CSVPetNewParam.Data param = CSVPetNewParam.Instance.GetConfData(58u);
                    if (null != param)
                    {
                        bindPetAbandonCoin = param.value;
                    }
                }
                return bindPetAbandonCoin;
            }
        }

        //骑术能量
        public uint RidingEnergy { get; set; }
        public uint SkillCostResetTick { get; set; }

        private List<uint> advancedLevel;
        public List<uint> AdvancedLevel
        {
            get
            {
                if(null == advancedLevel)
                {
                    CSVPetNewParam.Data paramData = CSVPetNewParam.Instance.GetConfData(70u);
                    if(null != paramData)
                    {
                        advancedLevel = ReadHelper.ReadArray_ReadUInt(paramData.str_value, '|');
                    }
                    else
                    {
                        advancedLevel = new List<uint>();
                    }
                }
                return advancedLevel;
            }
        }

        public void ShowPetTip(ClientPet pet, uint type, uint labelId = 0)
        {
            if (pet.HasMinutePetInfo())
            {
                ShowDesc(pet);
                tipsPetUid = 0;
            }
            else
            {
                tipsPetUid = pet.GetPetUid();
                CmdPetGetPetInfoReq cmdPetGetPetInfoReq = new CmdPetGetPetInfoReq();
                cmdPetGetPetInfoReq.PetUid.Add(pet.GetPetUid());
                cmdPetGetPetInfoReq.Type = type;
                cmdPetGetPetInfoReq.Banklabel = labelId;
                NetClient.Instance.SendMessage((ushort)CmdPet.GetPetInfoReq, cmdPetGetPetInfoReq);
            }
        }

        private void ShowDesc(ClientPet pet)
        {
            UIManager.OpenUI(EUIID.UI_Pet_Details, false, pet);
        }

        public void OnGetPetInfoReq(MessageEx _practiceEx = null, EPetUiType _ePetUiType = EPetUiType.UI_None, uint type = 0, uint labelId = 0)
        {
            petUiType = _ePetUiType;
            practiceEx = _practiceEx;
            List<ClientPet> notMinList = HasClientNotMin(type);
            if (!isInitData)
            {
                CmdPetGetPetInfoReq cmdPetGetPetInfoReq = new CmdPetGetPetInfoReq();
                cmdPetGetPetInfoReq.Banklabel = labelId;
                cmdPetGetPetInfoReq.Type = type;
                NetClient.Instance.SendMessage((ushort)CmdPet.GetPetInfoReq, cmdPetGetPetInfoReq);
                isInitData = true;
            }
            else if(isInitData && notMinList.Count > 0)
            {
                List<uint> petUids = new List<uint>();
                for (int i = 0; i < notMinList.Count; i++)
                {
                    petUids.Add(notMinList[i].GetPetUid());
                }
                CmdPetGetPetInfoReq cmdPetGetPetInfoReq = new CmdPetGetPetInfoReq();
                cmdPetGetPetInfoReq.PetUid.AddRange(petUids);
                cmdPetGetPetInfoReq.Type = type;
                cmdPetGetPetInfoReq.Banklabel = labelId;
                NetClient.Instance.SendMessage((ushort)CmdPet.GetPetInfoReq, cmdPetGetPetInfoReq);
            }
            else
            {
                CheckPet();
            }
        }

        private List<ClientPet> HasClientNotMin(uint type)
        {
            List<ClientPet> clients = new List<ClientPet>();
            List<ClientPet> checkList = null;
            if(type == 0)
            {
                checkList = petsList;
            }
            else
            {
                checkList = petTempPackUnits;
            }
            for (int i = 0; i < checkList.Count; i++)
            {
                ClientPet client = checkList[i];
                if (!client.HasMinutePetInfo())
                {
                    clients.Add(client);
                }
            }
            return clients;
        }

        public bool IsInBag(PetUnit unit) {
            for (int j = 0, length = petsList.Count; j < length; j++) {
                ClientPet clientPet = petsList[j];
                if (clientPet.petUnit.Uid == unit.Uid) {
                    return true;
                }
            }
            return true;
        }
        private void OnGetPetInfoNtf(NetMsg msg)
        {
            CmdPetPetInfoNtf dataNtf = NetMsgUtil.Deserialize<CmdPetPetInfoNtf>(CmdPetPetInfoNtf.Parser, msg);
            if (dataNtf.Type == 0)
            {
                for (int i = 0; i < dataNtf.PetInfo.Count; i++)
                {
                    PetUnit v = dataNtf.PetInfo[i];
                    for (int j = 0; j < petsList.Count; j++)
                    {
                        ClientPet clientPet = petsList[j];
                        if (clientPet.petUnit.Uid == v.Uid)
                        {
                            if(null != clientPet.petUnit.SimpleInfo && null != v.SimpleInfo && v.SimpleInfo.Level < clientPet.petUnit.SimpleInfo.Level)
                            {
                                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(15193, clientPet.GetPetNmae()));
                            }
                            clientPet.ResetPetModel(v);
                        }
                        if (fightPet.IsSamePet(v.Uid))
                            fightPet.InitFightPetData(v.Uid, v.SimpleInfo, (long)clientPet.GetAttrValueByAttrId((int)EPkAttr.MaxHp), null == v.PkAttr? (long)clientPet.GetAttrValueByAttrId((int)EPkAttr.CurHp):v.PkAttr.CurHp, (long)clientPet.GetAttrValueByAttrId((int)EPkAttr.MaxMp), null == v.PkAttr ? (long)clientPet.GetAttrValueByAttrId((int)EPkAttr.CurMp) : v.PkAttr.CurMp);
                    }
                }
                CheckPet();
                RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnPetAddPonit, null);
                eventEmitter.Trigger(EEvents.OnUpdatePetInfo);
            }
            else if (dataNtf.Type == 1)
            {
                for (int i = 0; i < dataNtf.PetInfo.Count; i++)
                {
                    PetUnit v = dataNtf.PetInfo[i];
                    for (int j = 0; j < petTempPackUnits.Count; j++)
                    {
                        ClientPet clientPet = petTempPackUnits[j];
                        if (clientPet.petUnit.Uid == v.Uid)
                        {
                            clientPet.ResetPetModel(v);
                            if (tipsPetUid == v.Uid)
                            {
                                ShowDesc(clientPet);
                                tipsPetUid = 0;
                            }
                        }
                    }
                }
            }
            else if (dataNtf.Type == 2)
            {
                List<ClientPet> bankPetlisr = storagePetData[dataNtf.Banklabel];

                for (int i = 0; i < dataNtf.PetInfo.Count; i++)
                {
                    PetUnit v = dataNtf.PetInfo[i];
                    for (int j = 0; j < bankPetlisr.Count; j++)
                    {
                        ClientPet clientPet = bankPetlisr[j];
                        if (clientPet.petUnit.Uid == v.Uid)
                        {
                            clientPet.ResetPetModel(v);
                            if (tipsPetUid == v.Uid)
                            {
                                ShowDesc(clientPet);
                                tipsPetUid = 0;
                            }
                        }
                    }
                }
            }
        }

        public void OnAutoBlinkInfoReq()
        {
            CmdPetAutoBlinkInfoReq req = new CmdPetAutoBlinkInfoReq();
            NetClient.Instance.SendMessage((ushort)CmdPet.AutoBlinkInfoReq, req);
            openAutoBlinkView = true;
        }

        public List<uint> petAutoBlinkList = new List<uint>();
        public bool useAutoBlink;
        private bool openAutoBlinkView;

        public void PetAutoBlinkInfoRes(NetMsg msg)
        {
            CmdPetAutoBlinkInfoRes dataNtf = NetMsgUtil.Deserialize<CmdPetAutoBlinkInfoRes>(CmdPetAutoBlinkInfoRes.Parser, msg);
            petAutoBlinkList.Clear();
            petAutoBlinkList.AddRange(dataNtf.Uid);
            useAutoBlink = dataNtf.UseAutoBlink;
            if(openAutoBlinkView)
            {
                eventEmitter.Trigger(EEvents.OnGetAutoBlinkDataEnd);
                openAutoBlinkView = false;
            }  
        }

        public void OnPetAutoBlinkSetReq(List<uint> petUids, bool isUseAutoBlink)
        {
            CmdPetAutoBlinkSetReq req = new CmdPetAutoBlinkSetReq();
            req.Uid.AddRange(petUids);
            req.Useautoblink = isUseAutoBlink;
            NetClient.Instance.SendMessage((ushort)CmdPet.AutoBlinkSetReq, req);
        }

        private void CheckPet()
        {
            if (petUiType == EPetUiType.UI_Message)
            {
                petUiType = EPetUiType.UI_None;
                if (!Sys_FunctionOpen.Instance.IsOpen(10501, true))//宠物功能开启条件
                    return;
                UIManager.OpenUI(EUIID.UI_Pet_Message, false, practiceEx);
            }
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnGetPetInfoForUplift);
        }
        

        public string GetPetName(ClientPet messageClientPet)
        {
            CSVPetNew.Data cSVPetData = CSVPetNew.Instance.GetConfData(messageClientPet.petUnit.SimpleInfo.PetId);
            return !messageClientPet.petUnit.SimpleInfo.Name.IsEmpty ? messageClientPet.petUnit.SimpleInfo.Name.ToStringUtf8() : LanguageHelper.GetTextContent(cSVPetData.name);
        }

        public string GetPetNmaeByClient(ClientPet messageClientPet)
        {
            if(null == messageClientPet)
            {
                return LanguageHelper.GetTextContent(540000028);
            }
            return GetPetNmaeBySeverName(messageClientPet.petData.id, messageClientPet.petUnit.SimpleInfo.Name);
        }

        public string GetPetNmaeBySeverName(uint PetId, ByteString name)
        {
            if(!name.IsEmpty)
            {
                return name.ToStringUtf8();
            }
            CSVPetNew.Data cSVPetData = CSVPetNew.Instance.GetConfData(PetId);
            if(null == cSVPetData)
            {
                return LanguageHelper.GetTextContent(540000028);
            }
            return LanguageHelper.GetTextContent(cSVPetData.name);
        }

        public int GetPetSkillNum(ClientPet messageClientPet)
        {
            int skillNum = 0;
            //skillNum = messageClientPet.petUnit.BaseSkillInfo.Skills.Count + (messageClientPet.petUnit.BaseSkillInfo.UniqueSkills.Count);
            return skillNum;
        }

        public void OnPetAbandonPetReq(uint id)
        {
            CmdPetAbandonPetReq cmdPetAbandonPetReq = new CmdPetAbandonPetReq();
            cmdPetAbandonPetReq.Uid = id;
            NetClient.Instance.SendMessage((ushort)CmdPet.AbandonPetReq, cmdPetAbandonPetReq);
        }

        private void PetAbandonPetRes(NetMsg msg)
        {
            CmdPetAbandonPetRes dataRes = NetMsgUtil.Deserialize<CmdPetAbandonPetRes>(CmdPetAbandonPetRes.Parser, msg);
            for (int i = 0; i < petsList.Count; i++)
            {
                uint petUid = petsList[i].petUnit.Uid;
                if (dataRes.Uid == petUid)
                {
                    if (fightPet.IsSamePet(petUid))
                    {
                        fightPet.RemoveFightPetData();
                    }                        
                    if(mountPetUid == petUid)
                    {
                        mountPetUid = 0;
                        GameCenter.mainHero.OffMount();
                    }
                    if (followPetUid == petUid)
                    {
                        followPetUid = 0;
                        GameCenter.mainHero.RemovePet();
                    }
                    if (null != remakePet && remakePet.petUnit.Uid == petsList[i].petUnit.Uid)
                        remakePet = null;
                    petsList.RemoveAt(i);
                    break;
                }
            }
            /*if(dataRes.Type == 2)
            {
                Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.Event43);
            }*/
            CheckNextLimitPet();
            // 宠物放生了 刷新数据
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnNumberChangePet);
        }

        public void OnPetRenameReq(uint id, string newNmae)
        {
            CmdPetRenameReq cmdPetRenameReq = new CmdPetRenameReq();
            cmdPetRenameReq.Uid = id;
            cmdPetRenameReq.NewName = FrameworkTool.ConvertToGoogleByteString(newNmae);
            NetClient.Instance.SendMessage((ushort)CmdPet.RenameReq, cmdPetRenameReq);
        }

        private void PetRenameRes(NetMsg msg)
        {
            CmdPetRenameRes dataRes = NetMsgUtil.Deserialize<CmdPetRenameRes>(CmdPetRenameRes.Parser, msg);

            for (int i = 0; i < petsList.Count; i++)
            {
                ClientPet pet = petsList[i];
                if (pet.petUnit.Uid == dataRes.Uid)
                {
                    pet.petUnit.SimpleInfo.Name = dataRes.NewName;
  
                    //TODO 添加出战宠物名称
                    break;
                }
            }

            // 宠物改名了 刷新数据
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnReNamePet);
        }

        /// <summary>
        /// 宠物换位
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newPostion">值为宠物列表下标</param>
        public void OnPetChangePositionReq(uint id, uint newPostion)
        {
            CmdPetChangePositionReq cmdPetChangePositionReq = new CmdPetChangePositionReq();
            cmdPetChangePositionReq.Uid = id;
            cmdPetChangePositionReq.NewPosition = newPostion;
            NetClient.Instance.SendMessage((ushort)CmdPet.ChangePositionReq, cmdPetChangePositionReq);
        }

        private void PetChangePositionRes(NetMsg msg)
        {
            CmdPetChangePositionRes dataRes = NetMsgUtil.Deserialize<CmdPetChangePositionRes>(CmdPetChangePositionRes.Parser, msg);
            int newIndex = (int)dataRes.NewPosition;
            int oldIndex = (int)dataRes.OldPetNewPosition;
            ClientPet tempNewPet = petsList[newIndex];
            ClientPet tempOldPet = petsList[oldIndex];
            petsList[newIndex] = tempOldPet;
            petsList[oldIndex] = tempNewPet;            
            Sys_Hint.Instance.PushEffectInNextFight();  //战斗内提示：当前战斗不生效，将在下一场战斗生效
            //换位置了
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnChangePostion);
        }

        /// <summary>
        /// 给技能增加经验
        /// </summary>
        /// <param name="petUid"></param>
        /// <param name="SkillId"></param>
        /// <param name="itemUid"></param>
        /// <param name="itemNum"></param>
        public void OnCmdPetSkillAddExpReq(uint petUid, uint SkillId, List<ulong> itemUid, List<long> itemNum, uint sliverNum)
        {
            CmdPetSkillAddExpReq req = new CmdPetSkillAddExpReq();
            req.Uid = petUid;
            req.SkillId = SkillId;
            for (int i = 0; i < itemUid.Count; i++)
            {
                ulong uid = itemUid[i];
                if (uid != 0)
                {
                    req.ItemUid.Add(itemUid[i]);
                    req.ItemNum.Add((uint)itemNum[i]);
                }
            }
            req.UseSilverNum = sliverNum;
            NetClient.Instance.SendMessage((ushort)CmdPet.SkillAddExpReq, req);
        }

        private void PetSkillAddExpRes(NetMsg msg)
        {
            CmdPetSkillAddExpRes dataRes = NetMsgUtil.Deserialize<CmdPetSkillAddExpRes>(CmdPetSkillAddExpRes.Parser, msg);
            uint uid = dataRes.Uid;
            for (int i = 0; i < petsList.Count; i++)
            {
                ClientPet pet = petsList[i];
                if (pet.GetPetUid() == uid)
                {
                    pet.SetPetSkillExp(dataRes.SkillId, dataRes.SkillExp);
                }
            }
            Sys_Hint.Instance.PushEffectInNextFight();  //战斗内提示：当前战斗不生效，将在下一场战斗生效
            //技能经验变化
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnUpdatePetInfo);
        }

        /// <summary>
        /// 遗忘技能
        /// </summary>
        /// <param name="petUid"></param>
        /// <param name="SkillId"></param>
        /// <param name="type">1 专属技能 2 一般技能 3 改造技能</param>
        public void OnCmdPetRemoveSkillReq(uint petUid, uint SkillId, uint type)
        {
           CmdPetRemoveSkillReq req = new CmdPetRemoveSkillReq();
            req.Uid = petUid;
            req.Skillid = SkillId;
            req.Type = type;
            NetClient.Instance.SendMessage((ushort)CmdPet.RemoveSkillReq, req);
        }

        private void PetRemoveSkillRes(NetMsg msg)
        {
            CmdPetRemoveSkillRes dataRes = NetMsgUtil.Deserialize<CmdPetRemoveSkillRes>(CmdPetRemoveSkillRes.Parser, msg);
            uint uid = dataRes.Uid;
            for (int i = 0; i < petsList.Count; i++)
            {
                ClientPet pet = petsList[i];
                if (pet.GetPetUid() == uid)
                {
                    pet.RemoveSkill(dataRes.Skillid, dataRes.Skillexp, dataRes.Type);
                }
            }
            Sys_Hint.Instance.PushEffectInNextFight();  //战斗内提示：当前战斗不生效，将在下一场战斗生效
            //技能遗忘
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnUpdatePetInfo);
        }

       /// <summary>
       /// 技能升级
       /// </summary>
       /// <param name="petUid"></param>
       /// <param name="SkillId"></param>
        public void OnPetSkillLevelUpReq(uint petUid, uint SkillId)
        {
            if (CheckIsLimitPet(petUid))
                return;
            CmdPetSkillLevelUpReq req = new CmdPetSkillLevelUpReq();
            req.Uid = petUid;
            req.SkillId = SkillId;
            NetClient.Instance.SendMessage((ushort)CmdPet.SkillLevelUpReq, req);
        }

        private void PetSkillLevelUpRes(NetMsg msg)
        {
            CmdPetSkillLevelUpRes dataRes = NetMsgUtil.Deserialize<CmdPetSkillLevelUpRes>(CmdPetSkillLevelUpRes.Parser, msg);
            uint uid = dataRes.Uid;
            for (int i = 0; i < petsList.Count; i++)
            {
                ClientPet pet = petsList[i];
                if (pet.GetPetUid() == uid)
                {
                    pet.ReSetSkill(dataRes.OskillId, dataRes.NskillId, dataRes.SkillExp);
                }
            }
            Sys_Hint.Instance.PushEffectInNextFight();  //战斗内提示：当前战斗不生效，将在下一场战斗生效
            //技能升级
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnUpdatePetInfo);
        }

        public void OnPetDeComposeReq(uint petUid)
        {
            if (CheckIsLimitPet(petUid))
                return;
            CmdPetDeComposeReq req = new CmdPetDeComposeReq();
            req.Uid = petUid;
            NetClient.Instance.SendMessage((ushort)CmdPet.DeComposeReq, req);
        }

        public void OnPetBagUnlockReq()
        {
            CmdPetBagUnlockReq req = new CmdPetBagUnlockReq();
            NetClient.Instance.SendMessage((ushort)CmdPet.BagUnlockReq, req);
        }

        private void PetBagUnlockRes(NetMsg msg)
        {
            CmdPetBagUnlockRes dataRes = NetMsgUtil.Deserialize<CmdPetBagUnlockRes>(CmdPetBagUnlockRes.Parser, msg);
            costBagNum = dataRes.CostGrids;
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnUpdatePetInfo);
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10925));
        }

        List<uint> uniquePetList = new List<uint>();

        private void PetUniquePetAddNtf(NetMsg msg)
        {
            CmdPetUniquePetAddNtf ntf = NetMsgUtil.Deserialize<CmdPetUniquePetAddNtf>(CmdPetUniquePetAddNtf.Parser, msg);
            uniquePetList.Add(ntf.AddInfoId);
        }

        public void PetStampExchangeReq(uint petUid)
        {
            CmdPetStampExchangeReq req = new CmdPetStampExchangeReq();
            req.Uid = petUid;
            NetClient.Instance.SendMessage((ushort)CmdPet.StampExchangeReq, req);
        }

        public void PetUpStageReq(uint petUid)
        {
            CmdPetUpStageReq req = new CmdPetUpStageReq();
            req.Uid = petUid;
            NetClient.Instance.SendMessage((ushort)CmdPet.UpStageReq, req);
        }

        private void OnPetUpStageRes(NetMsg msg)
        {
            CmdPetUpStageRes res = NetMsgUtil.Deserialize<CmdPetUpStageRes>(CmdPetUpStageRes.Parser, msg);
            for (int i = 0; i < petsList.Count; i++)
            {
                ClientPet pet = petsList[i];
                if (pet.GetPetUid() == res.Uid)
                {
                    pet.petUnit.SimpleInfo.PetStage = res.PetStage;
                    Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnUpdatePetInfo);
                    break;
                }
            }
        }


        public bool PetBagCostRedState()
        {
            List<uint> costData = Sys_Pet.Instance.GetBagCountUnLockData();
            if (null != costData)
            {
                if (costData.Count >= 3)
                {
                    uint level = costData[0];
                    uint resId = costData[1];
                    uint resCout = costData[2];
                    return (Sys_Bag.Instance.GetItemCount(resId) >= resCout) && (level <= Sys_Role.Instance.Role.Level);
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

        public uint SetPetQuality(uint cardType)
        {
            uint id = 0;
            switch (cardType)
            {
                case 1: { id = 992911; } break;
                case 2: { id = 992912; } break;
                case 3: { id = 992913; } break;
                case 4: { id = 992913; } break;
                default:
                    break;
            }
            return id;
        }

        public List<ClientPet> GetCarryPetList()
        {
            return petsList;
        }

        public ClientPet GetPetByUId(uint uid)
        {
            for (int i = 0; i < petsList.Count; i++)
            {
                if (Instance.petsList[i].petUnit.Uid == uid)
                {
                    return petsList[i];
                }
            }
            return null;
        }

        public List<PetUnit> GetGodPetList()
        {
            List<PetUnit> petlist = new List<PetUnit>();
            for (int i = 0; i < Sys_Pet.Instance.petsList.Count; i++)
            {
                ClientPet pet = Sys_Pet.Instance.petsList[i];
                CSVPetNew.Data petData = CSVPetNew.Instance.GetConfData(pet.petUnit.SimpleInfo.PetId);
                if (null != petData && petData.card_type == 3 && petData.reborn && !pet.petUnit.SimpleInfo.Bind && pet.petUnit.SimpleInfo.LockPeriod < Sys_Time.Instance.GetServerTime())
                {
                    petlist.Add(pet.petUnit);
                }

            }
            return petlist;
        }

        public List<uint> GetGodReviewPetid()
        {
            List<uint> petlist = new List<uint>();

            var goldPetExchangeDatas = CSVGoldPetExchange.Instance.GetAll();
            for (int i = 0, len = goldPetExchangeDatas.Count; i < len; i++)
            {
                CSVGoldPetExchange.Data godPetData = goldPetExchangeDatas[i];
                if (godPetData.isShow == 1 && !petlist.Contains(godPetData.Pet_id))
                {
                    petlist.Add(godPetData.Pet_id);
                }
            }
            return petlist;
        }

        public ClientPet GetPostion2ClientPet()
        {
            return GetClientPet2Postion(0);
        }

        public ClientPet GetClientPet2Postion(int index)
        {
            if (index < 0 || index >= petsList.Count)
                return null;
            if (petsList.Count > 0)
                return Sys_Pet.Instance.petsList[index];
            return null;
        }

        public void ChangeFightPetIndex()
        {
            if (petsList.Count > 1)
            {
                petsList.Sort(FightCom);

                if (currentPetUid == 0 || null == fightPet)
                    return;
                for (int i = 0; i < petsList.Count; i++)
                {
                    if (fightPet.IsSamePet(petsList[i].petUnit) && i != 0)
                    {
                        var TempClientPet = petsList[i];
                        petsList.Remove(petsList[i]);
                        petsList.Insert(0, TempClientPet);
                        break;
                    }
                }
            }
            
        }

        private int FightCom(ClientPet a, ClientPet b)
        {
            if (a.petUnit.SimpleInfo.Level < b.petUnit.SimpleInfo.Level)
            {
                return 1;
            }
            else if (a.petUnit.SimpleInfo.Level > b.petUnit.SimpleInfo.Level)
            {
                return -1;
            }
            else if (a.GetPetUid() > b.GetPetUid())
            {
                return 1;
            }
            else if (a.GetPetUid() < b.GetPetUid())
            {
                return -1;
            }
            return 0;
        }

        public string CardName(uint cardType)
        {
            string cardNmae = string.Empty;

            if (cardType == (uint)EPetCard.Normal)
            {
                cardNmae = LanguageHelper.GetTextContent(2009303);
            }
            else if (cardType == (uint)EPetCard.Silver)
            {
                cardNmae = LanguageHelper.GetTextContent(2009302);
            }
            else if (cardType == (uint)EPetCard.Gold)
            {
                cardNmae = LanguageHelper.GetTextContent(2009301);
            }
            return cardNmae;

        }

        public int GetPetQuality(PetUnit petUnit)
        {
            CSVPetNew.Data petData = CSVPetNew.Instance.GetConfData(petUnit.SimpleInfo.PetId);
            int index = 4;
            if (null != petData && null != petData.quality_score && petData.quality_score.Count > 0)
            {
                for (int i = 0; i < petData.quality_score.Count; ++i)
                {
                    if (petUnit.SimpleInfo.Score <= petData.quality_score[i])
                    {
                        index = i;
                        break;
                    }
                }
            }

            return index + 1;
        }

        public uint GetQuality_ScoreImage(ClientPet clientPet)
        {
            int quality = GetPetQuality(clientPet.petUnit);
            uint paramId = 0u;
            switch (quality)
            {
                case 1: { paramId = 836u; } break;
                case 2: { paramId = 835u; } break;
                case 3: { paramId = 834u; } break;
                case 4: { paramId = 833u; } break;
                case 5: { paramId = 832u; } break;
                default:
                    break;
            }            
            return uint.Parse(CSVParam.Instance.GetConfData(paramId).str_value); 
        }

        public Dictionary<uint, float> GetExchangeItemData(uint petScore)
        {
            int index = -1;
            for (int i = 0; i < ExchangeScoreList.Count; i++)
            {
                if (petScore >= ExchangeScoreList[i])
                {
                    index = i;
                }
            }
            if (0<= index && index <= ExchangeScoreList.Count)
            {
                List<List<uint>>  data = CSVPetNewExchange.Instance.GetConfData(ExchangeScoreList[index]).result; ;

                uint weight = 0;
                Dictionary<uint, float> itemAndWeight = new Dictionary<uint, float>();
                for (int i = 0; i < data.Count; i++)
                {
                    if (null != data[i] && data[i].Count >= 2)
                    {
                        weight += data[i][1];
                    }
                }

                for (int i = 0; i < data.Count; i++)
                {
                    if (null != data[i] && data[i].Count >= 2)
                    {
                        if (!itemAndWeight.ContainsKey(data[i][0]))
                        {
                            itemAndWeight.Add(data[i][0], (data[i][1] + 0f) / weight);
                        }
                    }
                }
                return itemAndWeight;
            }
            else
            {
                return null;
            }
        }

        public List<List<uint>> GetExchangeStampItemData(uint petScore, out uint weight)
        {
            int index = -1;
            for (int i = 0; i < ExchangeScoreList.Count; i++)
            {
                if (petScore >= ExchangeScoreList[i])
                {
                    index = i;
                }
            }
            if (0 <= index && index <= ExchangeScoreList.Count)
            {
                List<List<uint>> data = CSVPetNewExchange.Instance.GetConfData(ExchangeScoreList[index]).resolve_result;

                weight = 0;
                List<List<uint>> itemAndWeight = new List<List<uint>>();
                for (int i = 0; i < data.Count; i++)
                {
                    if (null != data[i] && data[i].Count >= 3)
                    {
                        weight += data[i][2];
                    }
                }
                return data;
            }
            else
            {
                weight = 0;
                return null;
            }
        }

        private List<uint> petAutoBlinkSkill;
        public List<uint> PetAutoBlinkSkill
        {
            get
            {
                if(null == petAutoBlinkSkill)
                {
                    CSVPetNewParam.Data param = CSVPetNewParam.Instance.GetConfData(23u);
                    if(null != param)
                    {
                        petAutoBlinkSkill = new List<uint>();
                        string[] strs = param.str_value.Split('|');
                        for (int i = 0; i < strs.Length; i++)
                        {
                            petAutoBlinkSkill.Add(uint.Parse(strs[i]));
                        }

                    }
                }
                return petAutoBlinkSkill;
            }
            private set { }
        }

        public List<ClientPet> GetHaveAutoBlinkSkillPets()
        {
            List<ClientPet> pets = new List<ClientPet>();
            for (int i = 0; i < petsList.Count; i++)
            {
                ClientPet pet = petsList[i];
                if(pet.IsHasAutoBlinkSkill() && Sys_Role.Instance.Role.Level >= pet.petData.participation_lv && (Sys_Role.Instance.Role.Level + MaxLevel2Battle >= pet.petUnit.SimpleInfo.Level))
                {
                    pets.Add(pet);
                }
            }
            return pets;
        }

        public long GetPetUnitPkAttrValue(PetUnit pet, int id)
        {
            if (null == pet || (null != pet && null == pet.PkAttr))
                return 0;

            for (int i = 0; i < pet.PkAttr.Attr.Count; i++)
            {
                AttrPair attrPair = pet.PkAttr.Attr[i];
                if(attrPair.AttrId == id)
                {
                    return (long)attrPair.AttrValue;
                }
            }
            return 0;
        }

        public ClientPet GetHightPowerPet()
        {
            int index = 0;
            uint maxPower = 0;
            for (int i = 0; i < petsList.Count; i++)
            {
                uint petPower = petsList[i].petUnit.SimpleInfo.Score;
                if (petPower > maxPower)
                {
                    index = i;
                    maxPower = petPower;
                }
            }

            if(index < petsList.Count)
            {
                return petsList[index];
            }

            return null;
        }

        private List<uint> gradeAdd;
        public List<uint> GradeAdd
        {
            get
            {
                if (null == gradeAdd)
                {
                    CSVPetNewParam.Data paramData = CSVPetNewParam.Instance.GetConfData(4u);
                    if (null != paramData)
                    {
                        gradeAdd = new List<uint>(8);
                        string[] strs = paramData.str_value.Split('|');
                        for (int i = 0; i < strs.Length; i++)
                        {
                            gradeAdd.Add(Convert.ToUInt32(strs[i]));
                        }
                        
                    }
                }
                return gradeAdd;
            }

            private set { }
        }

        /// <summary>
        /// 档位生效最低档 小于这个数组才生效
        /// </summary>
        private uint gradeLowEffectNum = 0;
        public uint GradeLowEffectNum
        {
            get
            {
                if (gradeLowEffectNum == 0)
                {
                    CSVPetNewParam.Data paramData = CSVPetNewParam.Instance.GetConfData(40u);
                    if (null != paramData)
                    {
                        gradeLowEffectNum = paramData.value;
                    }
                }
                return gradeLowEffectNum;
            }

            private set { }
        }

        public bool ISPetGradehaveEffect(uint grade)
        {
            return GradeLowEffectNum > grade;
        }

        public float GetPetGradeNum(int grade)
        {
            float num = 0f;
            if (0 <= grade && grade < GradeAdd.Count)
            {
                num = GradeAdd[grade] / 100.0f; //万分比 显示百分比 n / 10000 * 100
            }
            return num;
        }

        public string GetPetGradeDesc(int grade)
        {
            float num= GetPetGradeNum(grade);
            return LanguageHelper.GetTextContent(271099999, num.ToString());
        }

        public List<int> GetDomesticationListIndex()
        {
            List<int> petIndex = new List<int>();
            for (int i = 0; i < petsList.Count; i++)
            {
                ClientPet pet = petsList[i];
                if (null != pet && pet.petData.mount && !pet.GetPetIsDomestication())
                {
                    petIndex.Add(i);
                }
            }
            return petIndex;
        }

        public List<int> GetAdvancedPetList()
        {
            List<int> petIndex = new List<int>();
            for (int i = 0; i < petsList.Count; i++)
            {
                ClientPet pet = petsList[i];
                if (null != pet && pet.petData.is_gold_adv)
                {
                    petIndex.Add(i);
                }
            }
            return petIndex;
        }

        public ClientPet GetFirstDomesticationPet()
        {
            if (petsList.Count > 0)
            {
                for (int i = 0; i < petsList.Count; i++)
                {
                    ClientPet pet = petsList[i];
                    if (null != pet && pet.petData.mount && !pet.GetPetIsDomestication())
                    {
                        return pet;
                    }
                }
            }
            return null;
        }

        public ClientPet GetFirstAdvancedPet()
        {
            if (petsList.Count > 0)
            {
                for (int i = 0; i < petsList.Count; i++)
                {
                    ClientPet pet = petsList[i];
                    if (null != pet && pet.petData.is_gold_adv)
                    {
                        return pet;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 返回宠物是否受安全锁影响
        /// </summary>
        /// <param name="pet"></param>
        /// <returns></returns>
        public bool IsPetBeEffectWithSecureLock(PetUnit pet, bool jumpTips = true)
        {
            if (null == pet)
                return false;
            if (Sys_SecureLock.Instance.lockState && GetPetIsHightScore(pet))
            {
                if(jumpTips)
                    Sys_SecureLock.Instance.JumpToSecureLock();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 返回宠物是否评分大于等于某个品质，默认紫色
        /// </summary>
        /// <param name="pet"></param>
        /// <param name="quality"></param>
        /// <returns></returns>
        public bool GetPetIsHightScore(PetUnit pet, uint quality = Constants.PetPurpleNum)
        {
            if (null == pet)
                return false;
            return GetPetQuality(pet) >= quality;
        }

        /// <summary>
        /// 宠物装备是否受到安全锁影响
        /// </summary>
        /// <param name="petEquip"></param>
        /// <returns></returns>
        public bool IsPetEquipBeEffectWithSecureLock(PetEquip petEquip, bool jumpTips = true)
        {
            if (null == petEquip)
                return false;
            if (Sys_SecureLock.Instance.lockState && petEquip.Color >= 5)
            {
                if (jumpTips)
                    Sys_SecureLock.Instance.JumpToSecureLock();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 是否是唯一宠物，只能获得一只
        /// </summary>
        /// <param name="petId"></param>
        /// <returns></returns>
        public bool IsUniquePet(uint petId)
        {
            if(Sys_Ini.Instance.Get<IniElement_IntArray>(1407, out IniElement_IntArray arr))
            {
                var uniquePetIds = arr.value;
                for (int i = 0; i < uniquePetIds.Length; i++)
                {
                    if(uniquePetIds[i] == petId)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 返回是否已有唯一宠物
        /// </summary>
        /// <param name="petId"></param>
        /// <returns></returns>
        public bool HasUniquePet(uint petId)
        {
            return uniquePetList.Contains(petId);
        }

        public bool PetCanAdvanced(ClientPet pet)
        {
            if (null != pet)
            {
                int currentAdvance = (int)pet.GetAdvancedNum();
                var advancelevels = Sys_Pet.Instance.AdvancedLevel;
                if (currentAdvance >= advancelevels.Count)
                {
                    return false;
                }
                uint nextAdvanceLevel = advancelevels[currentAdvance];
                if(nextAdvanceLevel > pet.petUnit.SimpleInfo.Level)
                {
                    return false;
                }
                if(null != pet.petData.required_skills_money)
                {
                    if(currentAdvance < pet.petData.required_skills_money.Count)
                    {
                        var itemAndId = pet.petData.required_skills_money[currentAdvance];
                        if(itemAndId.Count >= 2)
                        {
                            ItemIdCount itemIdCount = new ItemIdCount(itemAndId[0], itemAndId[1]);
                            return itemIdCount.Enough;
                        }
                    }
                }
            }
            return false;
        }
    }
}
