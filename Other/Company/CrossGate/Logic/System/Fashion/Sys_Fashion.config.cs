using Packet;
using Logic.Core;
using Lib.Core;
using Net;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Collections;
using Table;
using System;

namespace Logic
{
    public partial class Sys_Fashion : SystemModuleBase<Sys_Fashion>, ISystemModuleUpdate
    {
        private List<dressData> GetInitialDressDataByFashionId(uint fashionId, uint heroId)
        {
            FashionClothes fashionClothes = _FashionClothes.Find(x => x.Id == fashionId);
            if (fashionClothes != null)
            {
                List<dressData> tempList = new List<dressData>();
                for (int i = 0; i < 4; i++)
                {
                    ETintIndex eTintIndex = (ETintIndex)i;

                    dressData dressData = new dressData();
                    dressData.tintIndex = eTintIndex;
                    dressData.color = GetClothesFirstColor(fashionId, eTintIndex, heroId);
                    tempList.Add(dressData);
                }
                return tempList;
            }

            FashionWeapon fashionWeapon = _FashionWeapons.Find(x => x.Id == fashionId);
            if (fashionWeapon != null)
            {
                List<dressData> tempList = new List<dressData>();
                for (int i = 0; i < 1; i++)
                {
                    ETintIndex eTintIndex = (ETintIndex)i;

                    dressData dressData = new dressData();
                    dressData.tintIndex = eTintIndex;
                    dressData.color = GetWeaponFirstColor(fashionId, eTintIndex);
                    tempList.Add(dressData);
                }
                return tempList;
            }

            FashionAccessory fashionAccessory = _FashionAccessories.Find(x => x.Id == fashionId);
            if (fashionAccessory != null)
            {
                List<dressData> tempList = new List<dressData>();
                for (int i = 0; i < 1; i++)
                {
                    ETintIndex eTintIndex = (ETintIndex)i;

                    dressData dressData = new dressData();
                    dressData.tintIndex = eTintIndex;
                    dressData.color = GetAcceFirstColor(fashionId, eTintIndex);
                    tempList.Add(dressData);
                }
                return tempList;
            }

            return new List<dressData>();
        }

        public uint GetFirstFashionClothesId()
        {
            foreach (var item in _FashionClothes)
            {
                if (item.cSVFashionClothesData.LimitedTime == 0)
                {
                    return item.Id;
                }
            }
            return 0;
        }

        public Color GetClothesFirstColor(uint clothesId, ETintIndex tintIndex, uint heroId)
        {
            List<uint> colors = new List<uint>();
            uint dyeId = clothesId * 10000 + heroId;
            CSVFashionColour.Data cSVFashionColourData = CSVFashionColour.Instance.GetConfData(dyeId);
            if (cSVFashionColourData == null)
            {
                DebugUtil.LogErrorFormat("找不到id={0}的数据 ： clothesId={1}, heroId={2}", dyeId, clothesId, heroId);
                return Color.red;
            }
            switch (tintIndex)
            {
                case ETintIndex.R:
                    colors = CSVFashionColour.Instance.GetConfData(dyeId).FashionColour1[0];
                    break;
                case ETintIndex.G:
                    colors = CSVFashionColour.Instance.GetConfData(dyeId).FashionColour2[0];
                    break;
                case ETintIndex.B:
                    colors = CSVFashionColour.Instance.GetConfData(dyeId).FashionColour3[0];
                    break;
                case ETintIndex.A:
                    colors = CSVFashionColour.Instance.GetConfData(dyeId).HairColour[0];
                    break;
                default:
                    break;
            }
            return new Color(colors[0] / 255f, colors[1] / 255f, colors[2] / 255f, 1);
        }

        public Color GetWeaponFirstColor(uint weaponId, ETintIndex tintIndex)
        {
            List<uint> colors = new List<uint>();
            CSVFashionWeapon.Data cSVFashionWeaponData = CSVFashionWeapon.Instance.GetConfData(weaponId);
            switch (tintIndex)
            {
                case ETintIndex.R:
                    colors = cSVFashionWeaponData.WeaponColour[0];
                    break;
            }
            return new Color(colors[0] / 255f, colors[1] / 255f, colors[2] / 255f, 1);
        }

        public Color GetAcceFirstColor(uint acceId, ETintIndex tintIndex)
        {
            List<uint> colors = new List<uint>();
            CSVFashionAccessory.Data cSVFashionAccessoryData = CSVFashionAccessory.Instance.GetConfData(acceId);
            switch (tintIndex)
            {
                case ETintIndex.R:
                    colors = cSVFashionAccessoryData.AccColour[0];
                    break;
            }
            return new Color(colors[0] / 255f, colors[1] / 255f, colors[2] / 255f, 1);
        }

        /// <summary>
        /// 切磋获得头像使用
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public uint GetClothId(Dictionary<uint, List<dressData>> dressData)
        {
            uint clothId = 0;
            foreach (var data in dressData)
            {
                FashionClothes fashionClothes = _FashionClothes.Find(x => x.Id == data.Key);
                if (fashionClothes != null)
                {
                    clothId = data.Key;
                    break;
                }
            }

            return clothId;
        }


        //返回 temp[0]: 武器资源路径    temp[1]:武器equipPos绑点
        public List<string> GetWeaponModelPath(uint fashionId, uint equipId, bool highModel)
        {
            List<string> temp = new List<string>();
            if (equipId == Constants.UMARMEDID)
            {
                //DebugUtil.Log(ELogType.eFashion, "当前身上没有装备");
                return temp;
            }
            CSVEquipment.Data cSVEquipmentData = CSVEquipment.Instance.GetConfData(equipId);
            if (cSVEquipmentData != null)
            {
                FashionWeapon fashionWeapon = Sys_Fashion.Instance._FashionWeapons.Find(x => x.Id == fashionId);
                string modelPath = null;
                string socketName = null;
                if (fashionWeapon != null)
                {
                    uint _weaponfashionModeId = fashionId * 10 + cSVEquipmentData.equipment_type;
                    CSVFashionWeaponModel.Data _cSVFashionWeaponModelData = CSVFashionWeaponModel.Instance.GetConfData(_weaponfashionModeId);
                    if (_cSVFashionWeaponModelData != null)
                    {
                        if (highModel)
                        {
                            modelPath = _cSVFashionWeaponModelData.model_show;
                        }
                        else
                        {
                            modelPath = _cSVFashionWeaponModelData.model;
                        }
                        socketName = _cSVFashionWeaponModelData.equip_pos;
                    }
                    else
                    {
                        //DebugUtil.LogErrorFormat("找不到武器模型id{0}", _weaponfashionModeId);
                        return temp;
                    }
                    temp.Add(modelPath);
                    temp.Add(socketName);
                    return temp;
                }
                else
                {
                    //DebugUtil.LogErrorFormat("找不到武器时装id{0}", fashionId);
                    modelPath = highModel ? cSVEquipmentData.show_model : cSVEquipmentData.model;
                    socketName = cSVEquipmentData.equip_pos;
                    temp.Add(modelPath);
                    temp.Add(socketName);
                    return temp;
                }
            }
            else
            {
                DebugUtil.LogErrorFormat("找不到装备id{0}", equipId);
                return temp;
            }
        }
        /// <summary>
        /// 获取时装武器csv数据
        /// </summary>
        public CSVFashionWeaponModel.Data GetWeaponModelData(uint fashionId, uint equipId)
        {
            if (equipId == Constants.UMARMEDID)
            {
                //DebugUtil.Log(ELogType.eFashion, "当前身上没有装备");
                return null;
            }
            CSVEquipment.Data cSVEquipmentData = CSVEquipment.Instance.GetConfData(equipId);
            if (cSVEquipmentData != null)
            {
                FashionWeapon fashionWeapon = Sys_Fashion.Instance._FashionWeapons.Find(x => x.Id == fashionId);
                if (fashionWeapon != null)
                {
                    uint _weaponfashionModeId = fashionId * 10 + cSVEquipmentData.equipment_type;
                    CSVFashionWeaponModel.Data _cSVFashionWeaponModelData = CSVFashionWeaponModel.Instance.GetConfData(_weaponfashionModeId);

                    return _cSVFashionWeaponModelData;
                }
            }
            DebugUtil.LogErrorFormat("找不到时装武器csv数据 fashionId {0} | equipId {1}", fashionId, equipId);
            return null;
        }
        public uint GetDressedWeaponFashionId(List<uint> _dressList)
        {
            foreach (var item in _dressList)
            {
                CSVFashionWeapon.Data cSVFashionWeaponData = CSVFashionWeapon.Instance.GetConfData(item);
                if (cSVFashionWeaponData != null)
                {
                    return item;
                }
            }
            return 0;
        }

        public uint GetDressedClothesFashionId(List<uint> dressList)
        {
            foreach (var item in dressList)
            {
                CSVFashionClothes.Data cSVFashionClothesData = CSVFashionClothes.Instance.GetConfData(item);
                if (cSVFashionClothesData != null)
                {
                    return item;
                }
            }
            return 0;
        }

        public List<uint> GetDressedAcceFashionId(List<uint> dressList)
        {
            List<uint> lists = new List<uint>();
            foreach (var item in dressList)
            {
                CSVFashionAccessory.Data cSVFashionAccessory = CSVFashionAccessory.Instance.GetConfData(item);
                if (cSVFashionAccessory != null)
                {
                    lists.Add(item);
                }
            }
            return lists;
        }
    }
}


