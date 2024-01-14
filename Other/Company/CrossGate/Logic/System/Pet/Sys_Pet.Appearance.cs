using Google.Protobuf;
using Google.Protobuf.Collections;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;


namespace Logic
{
    public partial class Sys_Pet : SystemModuleBase<Sys_Pet>
    {

        private List<uint> petAppearanceDropDownList;
        /// <summary>
        /// 宠物外观下拉删选列表
        /// </summary>
        public List<uint> PetAppearanceDropDownList
        {
            get
            {
                if(null == petAppearanceDropDownList)
                {
                    var petParam = CSVPetNewParam.Instance.GetConfData(93u);
                    if(null != petParam)
                    {
                        petAppearanceDropDownList = ReadHelper.ReadArray_ReadUInt(petParam.str_value, '|');
                    }
                }
                return petAppearanceDropDownList;
            }
        }

        public List<PetFashionData> petFashionInfos = new List<PetFashionData>();

        public void PetActiveFashionReq(uint petAppearanceId)
        {
            CmdPetActiveFashionReq req = new CmdPetActiveFashionReq();
            req.FashionId = petAppearanceId;
            NetClient.Instance.SendMessage((ushort)CmdPet.ActiveFashionReq, req);
        }

        private void OnPetActiveFashionRes(NetMsg msg)
        {
            CmdPetActiveFashionRes res = NetMsgUtil.Deserialize<CmdPetActiveFashionRes>(CmdPetActiveFashionRes.Parser, msg);
            PetFashionData petFashionEntry = new PetFashionData();
            petFashionEntry.FashionId = res.FashionId;
            petFashionEntry.ColorActive = 1;
            petFashionInfos.Add(petFashionEntry);
            eventEmitter.Trigger(EEvents.OnBuyOrActivePetAppearance, 1u);
        }

        public void PetActiveFashionColorReq(uint petAppearanceId, uint index)
        {
            CmdPetActiveFashionColorReq req = new CmdPetActiveFashionColorReq();
            req.FashionId = petAppearanceId;
            req.ColorIndex = index;
            NetClient.Instance.SendMessage((ushort)CmdPet.ActiveFashionColorReq, req);
        }

        private void OnPetActiveFashionColorRes(NetMsg msg)
        {
            CmdPetActiveFashionColorRes res = NetMsgUtil.Deserialize<CmdPetActiveFashionColorRes>(CmdPetActiveFashionColorRes.Parser, msg);
            bool noActive = true;
            for (int i = 0; i < petFashionInfos.Count; i++)
            {
                var info = petFashionInfos[i];
                if(info.FashionId == res.FashionId)
                {
                    noActive = false;
                    uint bit = info.ColorActive;
                    SetBitvalue(res.ColorIndex, ref bit);
                    info.ColorActive = bit;
                }
            }
            if(noActive)
            {
                PetFashionData petFashionEntry = new PetFashionData();
                petFashionEntry.FashionId = res.FashionId;
                uint bit = 1;
                SetBitvalue(res.ColorIndex, ref bit);
                petFashionEntry.ColorActive = bit;
                petFashionInfos.Add(petFashionEntry);
            }
            eventEmitter.Trigger(EEvents.OnBuyOrActivePetAppearance, 2u);
        }

        public void PetDressOnOffFashionReq(uint type ,uint petAppearanceId, uint petUid, uint colorIndex)
        {
            CmdPetDressOnOffFashionReq req = new CmdPetDressOnOffFashionReq();
            req.FashionId = petAppearanceId;
            req.Type = type;
            req.ColorIndex = colorIndex;
            req.PetUid = petUid;
            NetClient.Instance.SendMessage((ushort)CmdPet.DressOnOffFashionReq, req);
        }

        private void OnPetDressOnOffFashionRes(NetMsg msg)
        {
            CmdPetDressOnOffFashionRes res = NetMsgUtil.Deserialize<CmdPetDressOnOffFashionRes>(CmdPetDressOnOffFashionRes.Parser, msg);
            for (int i = 0; i < petsList.Count; i++)
            {
                var pet = petsList[i];
                if(pet.GetPetUid() == res.PetUid)
                {
                    pet.petUnit.SimpleInfo.FashionId = res.FashionId;
                    pet.petUnit.SimpleInfo.FashionColorIndex = res.ColorIndex;
                    //if (res.Type == 1) // 1穿 2 脱
                }
            }
            eventEmitter.Trigger(EEvents.OnPetEquipOrDownPetAppearance);
        }

        public void InitPetFashionInfos(RepeatedField<PetFashionData> petFashionDatas)
        {
            petFashionInfos.Clear();
            petFashionInfos.AddRange(petFashionDatas);
        }

        /// <summary>
        /// 获取拥有宠物外观的宠物通过种族id
        /// id = 0 表示全部
        /// </summary>
        /// <returns></returns>
        public List<uint> GetPossessAppearancePetIdsByPetGenu(uint genuType)
        {
            List<uint> petIds = new List<uint>();
            var count = CSVPetFashion.Instance.Count;
            for (int i = 0; i < count; i++)
            {
                var data = CSVPetFashion.Instance.GetByIndex(i);
                if(null != data)
                {
                    var dataPetData = CSVPetNew.Instance.GetConfData(data.Petid);
                    if (null != dataPetData && (genuType == 0 || genuType == dataPetData.race))
                    {
                        if (!petIds.Contains(data.Petid))
                        {
                            petIds.Add(data.Petid);
                        }
                            
                    }
                }
            }
            return petIds;
        }

        /// <summary>
        /// 获取宠物外观的id
        /// </summary>
        /// <returns></returns>
        public List<uint> GetPetAppearanceIdsByPetId(uint petId)
        {
            List<uint> petAppearanceIds = new List<uint>();
            var count = CSVPetFashion.Instance.Count;
            for (int i = 0; i < count; i++)
            {
                var data = CSVPetFashion.Instance.GetByIndex(i);
                if (null != data && !data.Hide && petId == data.Petid)
                {
                    petAppearanceIds.Add(data.id);
                }
            }
            return petAppearanceIds;
        }

        /// <summary>
        /// 获取宠物外观的染色方案
        /// </summary>
        /// <returns></returns>
        public List<uint> GetPetAppearanceColorIdsByPetIdAndAppearanceId(uint petId, uint petAppearanceId)
        {
            List<uint> petAppearanceColorIds = new List<uint>();
            var appearamceData = CSVPetFashion.Instance.GetConfData(petAppearanceId);
            if(null != appearamceData)
            {
                for (int i = 0; i < appearamceData.WeaponColour.Count; i++)
                {
                    petAppearanceColorIds.Add((uint)i);
                }
            }
            return petAppearanceColorIds;
        }

        /// <summary>
        /// 返回是否拥有对应宠物外观
        /// </summary>
        /// <param name="petAppearanceId"> 外观表id </param>
        /// <param name="index">外观染色 0 表示初始颜色</param>
        /// <returns></returns>
        public bool IsHasPetFashin(uint petAppearanceId, int index)
        {
            for (int i = 0; i < petFashionInfos.Count; i++)
            {
                var info = petFashionInfos[i];
                if (info.FashionId == petAppearanceId && GetActiveState((uint)index, info.ColorActive))
                {
                    return true;
                }
            }
            return false;
        }

        public List<uint> GetSamePetsByPetId(uint petId)
        {
            var petUids = new List<uint>(petsList.Count / 2);
            for (int i = 0; i < petsList.Count; i++)
            {
                var pet = petsList[i];
                if(pet.petData.id == petId)
                {
                    petUids.Add(pet.GetPetUid());
                }
            }
            return petUids;
        }

        /// <summary>
        /// 通过位比较获得状态
        /// </summary>
        /// <param name="setBit">子类型id</param>
        /// <returns>true 参选打勾</returns>
        public bool GetActiveState(uint setBit, uint value)
        {
            return GetBit32value((int)setBit, value);
        }

        /// <summary>
        /// 获取bit 是否为1
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool GetBit32value(int index, uint value)
        {
            if (index > 32)
                return false;

            uint _value = (value & (1u << index));
            return _value > 0;
        }

        /// <summary>
        /// 将bit位设为1
        /// </summary>
        /// <param name="setBit"></param>
        public void SetBitvalue(uint setBit, ref uint value)
        {
            int bit = (int)setBit;
            if (bit >= 32)
            {
                return;
            }
            value = value | (1u << bit);
        }

        /// <summary>
        /// 将bit位设为0
        /// </summary>
        /// <param name="setBit"></param>
        public void SetBitValueZeo(uint setBit, ref uint value)
        {
            int bit = (int)setBit;
            if (bit >= 32)
            {
                return;
            }
            value = value & (~(1u << bit));
        }
    }
}
