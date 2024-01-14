using Google.Protobuf;
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
        public string GetPetGearFxPath(CSVPetNew.Data petData, bool IsMax)
        {
            if(null != petData && IsMax)
            {
                EPetCard cardType = (EPetCard)petData.card_type;
                switch (cardType)
                {
                    case EPetCard.Normal:
                        return Constants.FxPetBule;
                    case EPetCard.Silver:
                        return Constants.FxPetPurple;
                    case EPetCard.Gold:
                        return Constants.FxPetOrange;
                    default:
                        return Constants.FxPetOrange;
                }
            }
            return null;
        }

        public string GetPetGearFxPath(uint petId, bool IsMax)
        {
            return GetPetGearFxPath(CSVPetNew.Instance.GetConfData(petId), IsMax);
        }

        private List<string> perfectFxPath;
        
        public List<string> PerfectFxPath
        {
            get
            {
                if (null == perfectFxPath)
                {
                    CSVPetNewParam.Data petParam = CSVPetNewParam.Instance.GetConfData(73);
                    if(null != petParam)
                    {
                        perfectFxPath = ReadHelper.ReadArray_ReadString(petParam.str_value, '|');
                    }
                }
                return perfectFxPath;
            }
        }

        public string GetPetRemakePerfectFxPath(ClientPet pet)
        {
            return GetPetRemakePerfectFxPath(GetPetPerfectRemakeCount(pet));
        }

        public string GetPetRemakePerfectFxPath(int count)
        {
            if (count > 0)
            {
                var list = PerfectFxPath;
                var fxCount = PerfectFxPath.Count;
                if (count > fxCount)
                    return list[fxCount - 1];
                return list[count - 1];
            }
            return null;
        }
        
        /// <summary>
        /// 获取宠物完美改造对应的次数
        /// </summary>
        /// <param name="pet"></param>
        /// <returns></returns>
        public int GetPetPerfectRemakeCount(ClientPet pet)
        {
            int remakeTime = pet.GetPeBuildCount();
            int count = 0;
            for (int i = 0; i < remakeTime; i++)
            {
                if (Sys_Pet.Instance.isPerfectRemakePoint((uint)i + 1, pet.GetBuildPointByIndex(i)))
                {
                    count++;
                }
            }
            return count;
        }
        /// <summary>
        /// 坐骑完美改次数
        /// </summary>
        /// <returns></returns>
        public int GetMountPerfectRemakeCount()
        {
            ClientPet pet = GetMountPet();
            if(null != pet)
            {
                return GetPetPerfectRemakeCount(pet);
            }
            return 0;
        }
        /// <summary>
        /// 跟随宠完美改次数
        /// </summary>
        /// <returns></returns>
        public int GetFollowPerfectRemakeCount()
        {
            ClientPet pet = GetFollwPet();
            if (null != pet)
            {
                return GetPetPerfectRemakeCount(pet);
            }
            return 0;
        }

        /// <summary>
        /// 坐骑显示魔魂特效
        /// </summary>
        /// <returns></returns>
        public bool IsMountPetShowDemonSpiritFx()
        {
            ClientPet pet = GetMountPet();
            if (null != pet)
            {
                return IsNeedShowDemonSpiritFx(pet);
            }
            return false;
        }
        /// <summary>
        /// 跟随宠显示魔魂特效
        /// </summary>
        /// <returns></returns>
        public bool IsFollowPetShowDemonSpiritFx()
        {
            ClientPet pet = GetFollwPet();
            if (null != pet)
            {
                return IsNeedShowDemonSpiritFx(pet);
            }
            return false;
        }

        /// <summary>
        /// 坐骑的外观模型路径
        /// </summary>
        /// <returns></returns>
        public string GetMountPetModelPath(bool showHight = true)
        {
            ClientPet pet = GetMountPet();
            if (null != pet)
            {
                return GetPetModelPath(pet, showHight);
            }
            return string.Empty;
        }

        /// <summary>
        /// 跟随宠的外观模型路径
        /// </summary>
        /// <returns></returns>
        public string GetFollowPetModelPath(bool showHight = true)
        {
            ClientPet pet = GetFollwPet();
            if (null != pet)
            {
                return GetPetModelPath(pet, showHight);
            }
            return string.Empty;
        }

        /// <summary>
        /// 只有本地宠物调用有效
        /// </summary>
        /// <param name="pet"></param>
        /// <param name="showHight"></param>
        /// <returns></returns>
        public string GetPetModelPath(ClientPet pet, bool showHight = true)
        {
            uint id = pet.GetAppearanId();
            if(id == 0)
            {
                return GetNormalPetAppearanceModelPath(pet.petData);
            }
            uint appearanId = id / 10;
            int index = (int)id % ((int)appearanId * 10);
            return GetPetModelPath(appearanId, index, showHight);
        }

        public string GetPetModelPath(PetUnit petUnit, bool showHight = true)
        {
            return GetPetModelPath(new ClientPet(petUnit), showHight);
        }

        public string GetPetModelPathByUid(uint petUid, bool showHight = true)
        {
            return GetPetModelPath(GetPetByUId(petUid), showHight);
        }

        /// <summary>
        /// 获取模型路径
        /// </summary>
        /// <param name="petId"> 宠物表格id </param>
        /// <param name="appearanceTempId"> 时装id*10+index </param>
        /// <param name="showHight"></param>
        /// <returns></returns>
        public string GetPetModelPath(uint petId, uint appearanceTempId, bool showHight = true)
        {
            if (appearanceTempId == 0)
            {
                return GetNormalPetAppearanceModelPath(petId, showHight);
            }
            uint appearanId = appearanceTempId / 10;
            int index = (int)appearanceTempId % ((int)appearanId * 10);
            return GetPetModelPath(appearanId, index, showHight);
        }

        /// <summary>
        /// 获取模型的外观路径
        /// </summary>
        /// <param name="petAppearanceId"></param>
        /// <param name="index"></param>
        /// <param name="showHight"></param>
        /// <returns></returns>
        public string GetPetModelPath(uint petAppearanceId, int index, bool showHight = true)
        {
            uint appearanId = petAppearanceId;
            if(appearanId == 0)
            {
                var petId = appearanId / 10;
                return GetNormalPetAppearanceModelPath(petId, showHight);
            }
            else
            {
                CSVPetFashion.Data fashionData = CSVPetFashion.Instance.GetConfData(appearanId);
                if (null != fashionData)
                {
                    if (showHight)
                    {
                        if (index >= 0 && index < fashionData.model_show.Count)
                        {
                            return fashionData.model_show[index];
                        }
                    }
                    else
                    {
                        if (index >= 0 && index < fashionData.model.Count)
                        {
                            return fashionData.model[index];
                        }
                    }
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取普通的模型路径
        /// </summary>
        /// <param name="petId"></param>
        /// <param name="showHight"></param>
        /// <returns></returns>
        private string GetNormalPetAppearanceModelPath(uint petId, bool showHight = true)
        {
            var petData = CSVPetNew.Instance.GetConfData(petId);
            return GetNormalPetAppearanceModelPath(petData, showHight);
        }

        /// <summary>
        /// 获取普通的模型路径
        /// </summary>
        /// <param name="petId"></param>
        /// <param name="showHight"></param>
        /// <returns></returns>
        private string GetNormalPetAppearanceModelPath(CSVPetNew.Data petData, bool showHight = true)
        {
            if (null != petData)
            {
                return showHight ? petData.model_show : petData.model;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 返回图鉴卡片的底图
        /// </summary>
        private readonly uint bookBackGroudBaseId = 2529; // 图标表id段开头        
        public uint SetPetBookQuality(uint cardType)
        {
            return (bookBackGroudBaseId + cardType);
        }

        /// <summary>
        /// 返回图鉴卡片底图的圆圈
        /// </summary>
        private readonly uint bookCircleBaseId = 2539;// 图标表id段开头
        public uint SetPetBookCircleQuality(uint cardType)
        {
            return bookCircleBaseId + cardType;
        }

        public bool IsNeedShowDemonSpiritFx(ClientPet pet)
        {
            if (null != pet && (pet.GetPetMaxGradeCount() - pet.GetPetGradeCount()) == 0)
            {
                var equipLevel = pet.GetEquipSphereTotalLevel();
                return equipLevel >= ShowDemonSpiritFxLevel;
            }
            return false;
        }

        public bool IsNeedShowDemonSpiritFx(uint petUid)
        {
            return IsNeedShowDemonSpiritFx(Sys_Pet.Instance.GetPetByUId(petUid));
        }

        public bool HasEquipDemonSpiritSphere(PetUnit pet)
        {
            if(null != pet)
            {
                ClientPet clientPet = new ClientPet(pet);

                if (null != clientPet && clientPet.GetDemonSpiritIsActive())
                {
                    return clientPet.HasEquipDemonSpiritSphere();
                }
            }
            return false;
        }

        public bool HasEquipDemonSpiritSphere(uint petUid)
        {
            ClientPet clientPet = GetPetByUId(petUid);
            if(null != clientPet && clientPet.GetDemonSpiritIsActive())
            {
                return clientPet.HasEquipDemonSpiritSphere();
            }
            return false;
        }
    }
}
