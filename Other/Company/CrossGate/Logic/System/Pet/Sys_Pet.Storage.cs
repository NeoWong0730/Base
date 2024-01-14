using Logic.Core;
using Net;
using Packet;
using System;
using System.Collections.Generic;
using Table;

namespace Logic
{
    public partial class Sys_Pet : SystemModuleBase<Sys_Pet>
    {
        private List<ClientPet> petTempPackUnits = new List<ClientPet>();
        public uint PetBankCount { get; set; }

        private Dictionary<uint, List<ClientPet>> storagePetData = new Dictionary<uint, List<ClientPet>>();

        private uint bankNpcId;
        public uint BankNpcId
        {
            get
            {
                if(bankNpcId == 0)
                {
                    CSVParam.Data paramData = CSVParam.Instance.GetConfData(243);
                    bankNpcId = Convert.ToUInt32(paramData.str_value);
                }
                return bankNpcId;
            }
        }
        /// <summary>
        /// 临时宠物背包协议
        /// </summary>
        public void OnPetOutFromPetTempPackReq()
        {
            CmdPetOutFromPetTempPackReq cmdPetOutFromPetTempPackReq = new CmdPetOutFromPetTempPackReq();
            NetClient.Instance.SendMessage((ushort)CmdPet.OutFromPetTempPackReq, cmdPetOutFromPetTempPackReq);
        }

        /// <summary>
        /// 未完成 协议
        /// </summary>
        /// <param name="msg"></param>
        private void PetOutFromPetTempPackRes(NetMsg msg)
        {
            CmdPetOutFromPetTempPackRes dataNtf = NetMsgUtil.Deserialize<CmdPetOutFromPetTempPackRes>(CmdPetOutFromPetTempPackRes.Parser, msg);
            for (int i = 0; i < dataNtf.Pets.Count; i++)
            {
                uint petUid = dataNtf.Pets[i];
                for (int j = 0; j < petTempPackUnits.Count; j++)
                {
                    ClientPet clientData = petTempPackUnits[j];
                    if (clientData.GetPetUid() == petUid)
                    {
                        petsList.Add(clientData);
                        petTempPackUnits.RemoveAt(j);
                        break;
                    }
                }
            }
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnTemplePetBagChange, petTempPackUnits.Count);
            Sys_Pet.Instance.OnGetPetInfoReq();
        }

        /// <summary>
        /// 未完成 协议
        /// </summary>
        /// <param name="msg"></param>
        private void OnPetTempPackNtf(NetMsg msg)
        {
            CmdPetTempPackNtf addNtf = NetMsgUtil.Deserialize<CmdPetTempPackNtf>(CmdPetTempPackNtf.Parser, msg);
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnTemplePetBagChange, petTempPackUnits.Count);
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1000942));
        }

        public void CheckTempPetBag()
        {
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnTemplePetBagChange, petTempPackUnits.Count);
        }

        /// <summary>
        ///  获取银行分页的宠物
        /// </summary>
        public void OnPetGetBankInfoReq(uint bankLabel)
        {
            if(storagePetData.ContainsKey(bankLabel))
            {
                Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnGetPetBankData, bankLabel);
            }
            else
            {
                CmdPetGetBankInfoReq cmdPetGetBankInfoReq = new CmdPetGetBankInfoReq();
                cmdPetGetBankInfoReq.LabelId = bankLabel;
                cmdPetGetBankInfoReq.NpcUId = Sys_Bag.Instance.curInteractiveNPC;
                NetClient.Instance.SendMessage((ushort)CmdPet.GetBankInfoReq, cmdPetGetBankInfoReq);
            }            
        }

        /// <summary>
        ///  获取银行分页的宠物 返回
        /// </summary>
        private void PetGetBankInfoRes(NetMsg msg)
        {
            CmdPetGetBankInfoRes dataData = NetMsgUtil.Deserialize<CmdPetGetBankInfoRes>(CmdPetGetBankInfoRes.Parser, msg);
            PetBankCount = dataData.BankCount;
            List<ClientPet> tempList = new List<ClientPet>();
            for (int i = 0; i < dataData.PetInfo.Count; i++)
            {
                ClientPet clientPet = new ClientPet(dataData.PetInfo[i]);
                tempList.Add(clientPet);
            }

            if(storagePetData.ContainsKey(dataData.LabelId))
            {
                storagePetData[dataData.LabelId] = tempList;
            }
            else
            {
                storagePetData.Add(dataData.LabelId, tempList);
            }
            
            eventEmitter.Trigger(EEvents.OnGetPetBankData, dataData.LabelId);
        }

        /// <summary>
        /// 解锁宠物银行分页
        /// </summary>
        public void OnPetBankUnlockReq()
        {
            CmdPetBankUnlockReq cmdPetBankUnlockReq = new CmdPetBankUnlockReq();            
            NetClient.Instance.SendMessage((ushort)CmdPet.BankUnlockReq, cmdPetBankUnlockReq);
        }

        /// <summary>
        /// 解锁宠物银行分页 返回
        /// </summary>
        private void PetBankUnlockRes(NetMsg msg)
        {
            CmdPetBankUnlockRes dataData = NetMsgUtil.Deserialize<CmdPetBankUnlockRes>(CmdPetBankUnlockRes.Parser, msg);
            PetBankCount = dataData.BankCount;
            eventEmitter.Trigger(EEvents.OnPetBankUnlock);
        }

        /// <summary>
        /// 宠物存入或者取出
        /// </summary>
        /// <param name="moveType">1存2取</param>
        /// <param name="labelId">页签编号</param>
        /// <param name="petUid">宠物uid</param>
        public void PetBankMoveReq(uint moveType, uint labelId, uint petUid)
        {
            if(!Sys_Hint.Instance.PushForbidOprationInFight())
            {
                CmdPetBankMoveReq cmdPetBankMoveReq = new CmdPetBankMoveReq();
                cmdPetBankMoveReq.MoveType = moveType;
                cmdPetBankMoveReq.LabelId = labelId;
                cmdPetBankMoveReq.PetUid = petUid;
                cmdPetBankMoveReq.NpcUId = Sys_Bag.Instance.curInteractiveNPC;
                NetClient.Instance.SendMessage((ushort)CmdPet.BankMoveReq, cmdPetBankMoveReq);
            }
        }

        private void PetBankMoveRes(NetMsg msg)
        {
            CmdPetBankMoveRes dataData = NetMsgUtil.Deserialize<CmdPetBankMoveRes>(CmdPetBankMoveRes.Parser, msg);
            if(dataData.MoveType == 1)  // 1存 2取
            {
                uint uid = dataData.Uid;
                int petConut = petsList.Count;
                for (int i = petConut - 1; i >= 0; i--)
                {
                    if (petsList[i].petUnit.Uid == uid)
                    {
                        storagePetData[dataData.LabelId].Add(petsList[i]);
                        petsList.RemoveAt(i);
                    }
                }
                if (fightPet.IsSamePet(uid))
                {
                    fightPet.RemoveFightPetData();
                }
                if (uid == mountPetUid)
                {
                    GameCenter.mainHero.OffMount();
                }
                if (uid == followPetUid)
                {
                    GameCenter.mainHero.RemovePet();
                }                    
                petdevelopdic.Remove(dataData.Uid);               
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1000938));                
            }
            else
            {
                if(storagePetData.ContainsKey(dataData.LabelId))
                {
                    int storageConut = storagePetData[dataData.LabelId].Count;
                    for (int i = storageConut - 1; i >= 0; i--)
                    {
                        if (storagePetData[dataData.LabelId][i].GetPetUid() == dataData.Uid)
                        {
                            petsList.Add(storagePetData[dataData.LabelId][i]);
                            storagePetData[dataData.LabelId].Remove(storagePetData[dataData.LabelId][i]);
                        }
                    }
                }
                OnGetPetInfoReq();
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1000940));
            }
            eventEmitter.Trigger(EEvents.OnStorageChang, dataData.LabelId);
        }

        public void PetTempBagBatchAbandonPetReq(List<uint> uids)
        {
            CmdPetTempBagBatchAbandonPetReq req = new CmdPetTempBagBatchAbandonPetReq();
            req.Uid.AddRange(uids);
            NetClient.Instance.SendMessage((ushort)CmdPet.TempBagBatchAbandonPetReq, req);
        }

        public void OnPetTempBagBatchAbandonPetRes(NetMsg msg)
        {
            CmdPetTempBagBatchAbandonPetRes res = NetMsgUtil.Deserialize<CmdPetTempBagBatchAbandonPetRes>(CmdPetTempBagBatchAbandonPetRes.Parser, msg);
            for (int i = 0; i < res.Uid.Count; i++)
            {
                uint petUid = res.Uid[i];
                for (int j = 0; j < petTempPackUnits.Count; j++)
                {
                    ClientPet clientData = petTempPackUnits[j];
                    if (clientData.GetPetUid() == petUid)
                    {
                        petTempPackUnits.RemoveAt(j);
                        break;
                    }
                }
            }
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnTemplePetBagChange, petTempPackUnits.Count);
        }


        /// <summary>
        /// 数据相关
        /// </summary>

        public bool PetIsFull()
        {
            return devicesCount == petsList.Count;
        }

        public List<ClientPet> GetTemplePetBagData()
        {
            return petTempPackUnits;
        }

        public List<ClientPet> GetBankPetByLabel(uint label)
        {
            storagePetData.TryGetValue(label, out List<ClientPet> data);
            return data;            
        }

        public int GetBankLabelNum()
        {
            CSVPetBank.Data LabelData = CSVPetBank.Instance.GetConfData(1);
            if(null != LabelData)
            {
                return LabelData.uarray2_value.Count;
            }
            return 0;
        }

        public uint GetBankLabelStorageNumByLabel(uint label)
        {
            CSVPetBank.Data LabelData = CSVPetBank.Instance.GetConfData(1);
            if (null != LabelData)
            {
                if(label > 0 && label <= LabelData.uarray2_value.Count)
                {
                    if(label == 1)
                    {
                        if(LabelData.uarray2_value[0].Count >= 2)
                        {
                            return LabelData.uarray2_value[0][0];
                        }                        
                    }
                    else
                    {
                        if (LabelData.uarray2_value[(int)label - 2].Count >= 2 && LabelData.uarray2_value[(int)label - 1].Count >= 2)
                        {
                            return LabelData.uarray2_value[(int)label - 1][0] - LabelData.uarray2_value[(int)label - 2][0];
                        }
                    }
                }
            }
            return 0;
        }

        public bool IsBankLabelIsFull(uint label)
        {
            int thisLabelStorageNum = (int)GetBankLabelStorageNumByLabel(label);
            List<ClientPet> tempPets = GetBankPetByLabel(label);
            int bankHasNum = 0;
            if (null != tempPets)
            {
                bankHasNum = GetBankPetByLabel(label).Count;
            }
            return thisLabelStorageNum == bankHasNum;
        }
    }
}
