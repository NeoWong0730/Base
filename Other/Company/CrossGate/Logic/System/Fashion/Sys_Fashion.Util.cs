using Packet;
using Logic.Core;
using Lib.Core;
using Net;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Collections;
using Table;
using System;
using System.Text;

namespace Logic
{

    public partial class Sys_Fashion : SystemModuleBase<Sys_Fashion>, ISystemModuleUpdate
    {
        public void CheckUnlockedList(uint changeFashionId, bool add)
        {
            if (add)
            {
                _UnLockedFashions.AddOnce<uint>(changeFashionId);
            }
            else
            {
                _UnLockedFashions.TryRemove<uint>(changeFashionId);
            }
            for (int i = 0, count = _FashionSuits.Count; i < count; i++)
            {
                bool bSuitUnlock = true;
                for (int j = 0; j < _FashionSuits[i].associated.Count; j++)
                {
                    if (!_UnLockedFashions.Contains(_FashionSuits[i].associated[j]))
                    {
                        bSuitUnlock = false;
                        _FashionSuits[i].TryRemoveAsso(_FashionSuits[i].associated[j]);
                    }
                    else
                    {
                        _FashionSuits[i].TryAddAsso(_FashionSuits[i].associated[j]);
                    }
                }
                if (bSuitUnlock)
                {
                    _FashionSuits[i].UnLock = true;
                    _UnLockedSuits.AddOnce<uint>(_FashionSuits[i].Id);
                }
                else
                {
                    _FashionSuits[i].UnLock = false;
                    _UnLockedSuits.TryRemove<uint>(_FashionSuits[i].Id);
                }
            }
            foreach (var item in _UnLockedSuits)
            {
                _UnLockedSuitAttr.Add(CSVFashionSuit.Instance.GetConfData(item).attr_id);
            }
        }


        public List<FashionAccessory> GetFashionAcce(int acceType)
        {
            List<FashionAccessory> fashionAccessories = new List<FashionAccessory>();
            foreach (var item in _FashionAccessories)
            {
                if (item.GetAcceType() == acceType)
                {
                    fashionAccessories.Add(item);
                }
            }
            return fashionAccessories;
        }

        public uint GetDressedId(EHeroModelParts eHeroModelParts)
        {
            // return dressedList.Find(x => parts[x] == eHeroModelParts);
            return dressedList[(int)eHeroModelParts];
        }

        public uint GetOldFashionId(uint fashionId)
        {
            uint oldfashionId = 0;
            EHeroModelParts eHeroModelParts = parts[fashionId];
            switch (eHeroModelParts)
            {
                case EHeroModelParts.None:
                    break;
                case EHeroModelParts.Main:
                    var _fashion = _FashionClothes.Find(x => x.Dress);
                    if (_fashion != null) { oldfashionId = _fashion.Id; }
                    break;
                case EHeroModelParts.Weapon:
                    var _fashion1 = _FashionWeapons.Find(x => x.Dress);
                    if (_fashion1 != null) { oldfashionId = _fashion1.Id; }
                    break;
                case EHeroModelParts.Jewelry_Head:
                    var _fashion2 = _FashionAcce_head_2.Find(x => x.Dress);
                    if (_fashion2 != null) { oldfashionId = _fashion2.Id; }
                    break;
                case EHeroModelParts.Jewelry_Back:
                    var _fashion3 = _FashionAcce_back_3.Find(x => x.Dress);
                    if (_fashion3 != null) { oldfashionId = _fashion3.Id; }
                    break;
                case EHeroModelParts.Jewelry_Waist:
                    var _fashion4 = _FashionAcce_waist_4.Find(x => x.Dress);
                    if (_fashion4 != null) { oldfashionId = _fashion4.Id; }
                    break;
                case EHeroModelParts.Jewelry_Face:
                    var _fashion5 = _FashionAcce_face_5.Find(x => x.Dress);
                    if (_fashion5 != null) { oldfashionId = _fashion5.Id; }
                    break;
                case EHeroModelParts.Count:
                    break;
                default:
                    break;
            }
            if (oldfashionId == fashionId)
                oldfashionId = 0;
            return oldfashionId;
        }

        private List<dressData> CalDressData(uint dressId, DyeScheme dyeScheme, uint heroId)
        {
            List<dressData> tempList = new List<dressData>();
            if (dyeScheme.DyeInfo.Count == 0)
            {
                FashionClothes fashionClothes = _FashionClothes.Find(x => x.Id == dressId);
                if (fashionClothes != null)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        ETintIndex eTintIndex = (ETintIndex)i;
                        dressData dressData = new dressData();
                        dressData.tintIndex = eTintIndex;
                        dressData.color = GetClothesFirstColor(fashionClothes.Id, eTintIndex, heroId);
                        tempList.Add(dressData);
                    }
                }
                FashionWeapon fashionWeapon = _FashionWeapons.Find(x => x.Id == dressId);
                if (fashionWeapon != null)
                {
                    for (int i = 0; i < 1; i++)
                    {
                        ETintIndex eTintIndex = (ETintIndex)i;
                        dressData dressData = new dressData();
                        dressData.tintIndex = eTintIndex;
                        dressData.color = GetClothesFirstColor(fashionWeapon.Id, eTintIndex, heroId);
                        tempList.Add(dressData);
                    }
                }
                FashionAccessory fashionAccessory = _FashionAccessories.Find(x => x.Id == dressId);
                if (fashionAccessory != null)
                {
                    for (int i = 0; i < 1; i++)
                    {
                        ETintIndex eTintIndex = (ETintIndex)i;
                        dressData dressData = new dressData();
                        dressData.tintIndex = eTintIndex;
                        dressData.color = GetClothesFirstColor(fashionAccessory.Id, eTintIndex, heroId);
                        tempList.Add(dressData);
                    }
                }
            }
            else
            {
                foreach (DyeInfo dye in dyeScheme.DyeInfo)
                {
                    dressData dress = new dressData();
                    dress.tintIndex = (ETintIndex)(dye.DyeIndex);
                    //if (dye.DyeValue.Count == 0)
                    //{
                    //    dress.color = Color.white;
                    //    Debug.LogErrorFormat("dressData dyeValue error, tintIndex={0}", dye.DyeIndex);
                    //}
                    //else
                    //{
                    //    Color32 color = Color32Extensions.FromUInt32(dye.DyeValue[0]);
                    //    dress.color = color;
                    //}
                    Color32 color = Color32Extensions.FromUInt32(dye.Value);
                    dress.color = color;
                    tempList.Add(dress);
                }
            }
            return tempList;
        }

        public Dictionary<uint, List<dressData>> GetDressData()
        {
            Dictionary<uint, List<dressData>> _dressData = new Dictionary<uint, List<dressData>>();
            //dressedList.Sort();
            uint fashionClothesId = dressedList[0];
            if (fashionClothesId != 0)
            {
                FashionClothes fashionClothes = _FashionClothes.Find(x => x.Id == fashionClothesId);
                if (fashionClothes != null)
                {
                    List<dressData> fashionClothesDressDatas = new List<dressData>();
                    for (int j = 0; j < 4; j++)
                    {
                        ETintIndex eTintIndex = (ETintIndex)j;
                        Color color = fashionClothes.GetLastUseColor(eTintIndex);
                        dressData dressData = new dressData();
                        dressData.tintIndex = eTintIndex;
                        dressData.color = color;
                        fashionClothesDressDatas.Add(dressData);
                    }
                    _dressData.Add(fashionClothesId, fashionClothesDressDatas);
                }
            }

            uint fashionWeaponId = dressedList[1];
            if (fashionWeaponId != 0)
            {
                FashionWeapon fashionWeapon = _FashionWeapons.Find(x => x.Id == fashionWeaponId);
                if (fashionWeapon != null)
                {
                    List<dressData> fashionWeaponDressDatas = new List<dressData>();
                    for (int j = 0; j < 1; j++)
                    {
                        ETintIndex eTintIndex = (ETintIndex)j;
                        dressData dressData = new dressData();
                        dressData.tintIndex = eTintIndex;
                        dressData.color = fashionWeapon.GetLastUseColor(eTintIndex);
                        fashionWeaponDressDatas.Add(dressData);
                    }
                    _dressData.Add(fashionWeaponId, fashionWeaponDressDatas);
                }
            }

            for (int i = 2; i < 6; i++)
            {
                uint fashionAcceId = dressedList[i];
                if (fashionAcceId != 0)
                {
                    FashionAccessory fashionAccessory = _FashionAccessories.Find(x => x.Id == fashionAcceId);
                    if (fashionAccessory != null)
                    {
                        List<dressData> fashionAcceDressDatas = new List<dressData>();
                        for (int j = 0; j < 1; j++)
                        {
                            ETintIndex eTintIndex = (ETintIndex)j;
                            dressData dressData = new dressData();
                            dressData.tintIndex = eTintIndex;
                            dressData.color = fashionAccessory.GetLastUseColor(eTintIndex);
                            fashionAcceDressDatas.Add(dressData);
                        }
                        _dressData.Add(fashionAcceId, fashionAcceDressDatas);
                    }
                }
            }

            //for (int i = 0; i < dressedList.Count; i++)
            //{
            //    uint dressId = dressedList[i];
            //    FashionClothes fashionClothes = _FashionClothes.Find(x => x.Id == dressId);
            //    if (fashionClothes != null)
            //    {
            //        List<dressData> fashionClothesDressDatas = new List<dressData>();
            //        for (int j = 0; j < 4; j++)
            //        {
            //            ETintIndex eTintIndex = (ETintIndex)j;
            //            Color color = fashionClothes.GetLastUseColor(eTintIndex);
            //            dressData dressData = new dressData();
            //            dressData.tintIndex = eTintIndex;
            //            dressData.color = color;
            //            fashionClothesDressDatas.Add(dressData);
            //        }
            //        _dressData.Add(dressId, fashionClothesDressDatas);
            //    }

            //    FashionWeapon fashionWeapon = _FashionWeapons.Find(x => x.Id == dressId);
            //    if (fashionWeapon != null)
            //    {
            //        List<dressData> fashionWeaponDressDatas = new List<dressData>();
            //        for (int j = 0; j < 1; j++)
            //        {
            //            ETintIndex eTintIndex = (ETintIndex)j;
            //            dressData dressData = new dressData();
            //            dressData.tintIndex = eTintIndex;
            //            dressData.color = fashionWeapon.GetLastUseColor(eTintIndex);
            //            fashionWeaponDressDatas.Add(dressData);
            //        }
            //        _dressData.Add(dressId, fashionWeaponDressDatas);
            //    }

            //    FashionAccessory fashionAccessory = _FashionAccessories.Find(x => x.Id == dressId);
            //    if (fashionAccessory != null)
            //    {
            //        List<dressData> fashionAcceDressDatas = new List<dressData>();
            //        for (int j = 0; j < 1; j++)
            //        {
            //            ETintIndex eTintIndex = (ETintIndex)j;
            //            dressData dressData = new dressData();
            //            dressData.tintIndex = eTintIndex;
            //            dressData.color = fashionAccessory.GetLastUseColor(eTintIndex);
            //            fashionAcceDressDatas.Add(dressData);
            //        }
            //        _dressData.Add(dressId, fashionAcceDressDatas);
            //    }
            //}
            return _dressData;
        }

        public Dictionary<uint, List<dressData>> GetDressData(RepeatedField<MapRoleFashionInfo> fashInfo, uint heroId)
        {
            Dictionary<uint, List<dressData>> _dressData = new Dictionary<uint, List<dressData>>();
            foreach (MapRoleFashionInfo dataInfo in fashInfo)
            {
                if (dataInfo.FashionId == 0)
                {
                    continue;
                }
                if (!_dressData.ContainsKey(dataInfo.FashionId))
                {
                    List<dressData> tempList = new List<dressData>();

                    if (dataInfo.DyeScheme == null)
                    {
                        DebugUtil.LogErrorFormat(" MapRoleFashionInfo.DyeScheme = null");
                    }
                    else
                    {
                        if (dataInfo.DyeScheme.DyeInfo.Count == 0)
                        {
                            tempList = GetInitialDressDataByFashionId(dataInfo.FashionId, heroId);
                        }

                        foreach (var dye in dataInfo.DyeScheme.DyeInfo)
                        {
                            dressData dress = new dressData();
                            dress.tintIndex = (ETintIndex)(dye.DyeIndex);
                            Color32 color = Color32Extensions.FromUInt32(dye.Value);
                            dress.color = color;

                            tempList.Add(dress);
                        }

                        _dressData.Add(dataInfo.FashionId, tempList);
                    }
                }
            }

            return _dressData;
        }

        public uint GetCurDressedFashionWeapon()
        {
            return dressedList[1];
            //return GetDressedWeaponFashionId(dressedList);
        }


        public int GetDressFashionIndex()
        {
            //int index = -1;
            //uint id = 0;
            //EHeroModelParts eHeroModelParts = EHeroModelParts.None;
            //for (int i = 0; i <dressedList.Count; i++)
            //{
            //    id =dressedList[i];
            //    if (!parts.TryGetValue(id, out eHeroModelParts))
            //    {
            //        Debug.LogErrorFormat("不存在时装时装部件，id={0}", id);
            //        return index;
            //    }
            //    if (eHeroModelParts == EHeroModelParts.Main)
            //    {
            //        FashionClothes fashionClothes =_FashionClothes.Find(x => x.Id == id);
            //        index =_FashionClothes.IndexOf(fashionClothes);
            //        break;
            //    }
            //}
            uint id = dressedList[0];
            FashionClothes fashionClothes = _FashionClothes.Find(x => x.Id == id);
            int index = _FashionClothes.IndexOf(fashionClothes);
            return index;
        }

        public int GetDressWeaponIndex()
        {
            //int index = -1;
            //uint id = 0;
            //EHeroModelParts eHeroModelParts = EHeroModelParts.None;
            //for (int i = 0; i <dressedList.Count; i++)
            //{
            //    id =dressedList[i];
            //    eHeroModelParts =parts[id];
            //    if (eHeroModelParts == EHeroModelParts.Weapon)
            //    {
            //        FashionWeapon fashionWeapon =_FashionWeapons.Find(x => x.Id == id);
            //        index =_FashionWeapons.IndexOf(fashionWeapon);
            //        break;
            //    }
            //}
            //if (index == -1)
            //{
            //    index = 0;
            //}
            uint id = dressedList[1];
            FashionWeapon fashionWeapon = _FashionWeapons.Find(x => x.Id == id);
            int index = _FashionWeapons.IndexOf(fashionWeapon);
            if (index == -1)
            {
                index = 0;
            }
            return index;
        }

        public int GetDressSuitIndex()
        {
            //int index = -1;
            //uint id = 0;
            //EHeroModelParts eHeroModelParts = EHeroModelParts.None;
            //for (int i = 0; i <dressedList.Count; i++)
            //{
            //    id =dressedList[i];
            //    eHeroModelParts =parts[id];
            //    if (eHeroModelParts == EHeroModelParts.Main)
            //    {
            //        for (int j = 0; j <_FashionSuits.Count; j++)
            //        {
            //            FashionSuit fashionSuit =_FashionSuits[j];
            //            if (fashionSuit.cSVFashionSuitData.FashionId == id)
            //            {
            //                index =_FashionSuits.IndexOf(fashionSuit);
            //                break;
            //            }
            //        }
            //    }
            //}
            int index = -1;
            uint id = dressedList[0];
            for (int i = 0; i < _FashionSuits.Count; i++)
            {
                FashionSuit fashionSuit = _FashionSuits[i];
                if (fashionSuit.cSVFashionSuitData.FashionId == id)
                {
                    index = _FashionSuits.IndexOf(fashionSuit);
                    break;
                }
            }
            if (index == -1)
            {
                index = 0;
            }
            return index;
        }


        public int GetDressAcceIndex(EHeroModelParts eHeroModelParts)
        {
            //int index = -1;
            //uint id = 0;
            //EHeroModelParts part;
            //for (int i = 0; i < dressedList.Count; i++)
            //{
            //    id = dressedList[i];
            //    part = parts[id];
            //    if (part == eHeroModelParts)
            //    {
            //        if (eHeroModelParts == EHeroModelParts.Jewelry_Head)
            //        {
            //            FashionAccessory fashionAccessory = _FashionAcce_head_2.Find(x => x.Id == id);
            //            index = _FashionAcce_head_2.IndexOf(fashionAccessory);
            //            break;
            //        }
            //        else if (eHeroModelParts == EHeroModelParts.Jewelry_Back)
            //        {
            //            FashionAccessory fashionAccessory = _FashionAcce_back_3.Find(x => x.Id == id);
            //            index = _FashionAcce_back_3.IndexOf(fashionAccessory);
            //            break;
            //        }
            //        else if (eHeroModelParts == EHeroModelParts.Jewelry_Waist)
            //        {
            //            FashionAccessory fashionAccessory = _FashionAcce_waist_4.Find(x => x.Id == id);
            //            index = _FashionAcce_waist_4.IndexOf(fashionAccessory);
            //            break;
            //        }
            //        else if (eHeroModelParts == EHeroModelParts.Jewelry_Face)
            //        {
            //            FashionAccessory fashionAccessory = _FashionAcce_face_5.Find(x => x.Id == id);
            //            index = _FashionAcce_face_5.IndexOf(fashionAccessory);
            //            break;
            //        }
            //    }
            //}

            int index = -1;
            uint id = dressedList[(int)eHeroModelParts];

            if (eHeroModelParts == EHeroModelParts.Jewelry_Head)
            {
                FashionAccessory fashionAccessory = _FashionAcce_head_2.Find(x => x.Id == id);
                index = _FashionAcce_head_2.IndexOf(fashionAccessory);
            }
            else if (eHeroModelParts == EHeroModelParts.Jewelry_Back)
            {
                FashionAccessory fashionAccessory = _FashionAcce_back_3.Find(x => x.Id == id);
                index = _FashionAcce_back_3.IndexOf(fashionAccessory);
            }
            else if (eHeroModelParts == EHeroModelParts.Jewelry_Waist)
            {
                FashionAccessory fashionAccessory = _FashionAcce_waist_4.Find(x => x.Id == id);
                index = _FashionAcce_waist_4.IndexOf(fashionAccessory);
            }
            else if (eHeroModelParts == EHeroModelParts.Jewelry_Face)
            {
                FashionAccessory fashionAccessory = _FashionAcce_face_5.Find(x => x.Id == id);
                index = _FashionAcce_face_5.IndexOf(fashionAccessory);
            }
            if (index == -1)
            {
                index = 0;
            }
            return index;
        }


        public bool DressSuit
        {
            get
            {
                bool ret = false;
                foreach (var item in _FashionSuits)
                {
                    if (item.Dress)
                    {
                        ret = true;
                        continue;
                    }
                }
                return ret;
            }
        }

        public void SetFashionClothesDyeSheme(FashionClothes fashionClothes, RepeatedField<DyeScheme> dyeSchemes)
        {
            for (int i = 0; i < dyeSchemes.Count; i++)
            {
                DyeScheme dyeScheme = dyeSchemes[i];
                RepeatedField<DyeInfo> d = dyeScheme.DyeInfo;
                if (d != null && d.Count > 0)
                {
                    fashionClothes.schemes[i] = 1;
                    for (int j = 0; j < d.Count; j++)
                    {
                        DyeInfo dyeInfo = d[j];
                        ColorCache colorcache = fashionClothes.OwnTintWarp[(ETintIndex)dyeInfo.DyeIndex];
                        Color32 color32 = Color32Extensions.FromUInt32(dyeInfo.Value);
                        colorcache.SetColor(i, color32);
                    }
                }
                else
                {
                    fashionClothes.schemes[i] = 0;
                }
            }

            DebugUtil.LogFormat(ELogType.eFashion, "时装衣服: {0}有 {1}套染色方案 ,当前使用方案{2}", fashionClothes.Id,
           fashionClothes.SchemeCount, fashionClothes.curUseScheme);
        }

        public void SetFashionWeaponDyeSheme(FashionWeapon fashionWeapon, RepeatedField<DyeScheme> dyeSchemes)
        {
            for (int i = 0; i < dyeSchemes.Count; i++)
            {
                DyeScheme dyeScheme = dyeSchemes[i];
                RepeatedField<DyeInfo> d = dyeScheme.DyeInfo;
                if (d != null && d.Count > 0)
                {
                    fashionWeapon.schemes[i] = 1;
                    for (int j = 0; j < d.Count; j++)
                    {
                        DyeInfo dyeInfo = d[j];
                        ColorCache colorcache = fashionWeapon.OwnTintWarp[(ETintIndex)dyeInfo.DyeIndex];
                        Color32 color32 = Color32Extensions.FromUInt32(dyeInfo.Value);
                        colorcache.SetColor(i, color32);
                    }
                }
                else
                {
                    fashionWeapon.schemes[i] = 0;
                }
            }

            DebugUtil.LogFormat(ELogType.eFashion, "时装武器 :{0} 有{1}套染色方案,当前使用方案{2}", fashionWeapon.Id,
                fashionWeapon.SchemeCount, fashionWeapon.curUseScheme);
        }

        public void SetFashionAcceDyeSheme(FashionAccessory fashionAccessory, RepeatedField<DyeScheme> dyeSchemes)
        {
            for (int i = 0; i < dyeSchemes.Count; i++)
            {
                DyeScheme dyeScheme = dyeSchemes[i];
                RepeatedField<DyeInfo> d = dyeScheme.DyeInfo;
                if (d != null && d.Count > 0)
                {
                    fashionAccessory.schemes[i] = 1;
                    for (int j = 0; j < d.Count; j++)
                    {
                        DyeInfo dyeInfo = d[j];
                        ColorCache colorcache = fashionAccessory.OwnTintWarp[(ETintIndex)dyeInfo.DyeIndex];
                        Color32 color32 = Color32Extensions.FromUInt32(dyeInfo.Value);
                        colorcache.SetColor(i, color32);
                    }
                }
                else
                {
                    fashionAccessory.schemes[i] = 0;
                }
            }
            DebugUtil.LogFormat(ELogType.eFashion, "时装挂饰 {0}有{1}套染色方案,当前使用方案{2}", fashionAccessory.Id,
                fashionAccessory.SchemeCount, fashionAccessory.curUseScheme);
        }


        private List<uint> canGet = new List<uint>();

        public bool HasReward()
        {
            canGet.Clear();
            var fashionScroes = CSVFashionScroe.Instance.GetAll();
            for (int i = 0, len = fashionScroes.Count; i < len; i++)
            {
                if (fashionPoint >= fashionScroes[i].score)
                {
                    canGet.Add(fashionScroes[i].id);
                }
            }
            for (int i = 0; i < canGet.Count; i++)
            {
                if (!rewardsGet.Contains(canGet[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public string DressListToString()
        {
            StringBuilder stringBuilder = StringBuilderPool.GetTemporary();
            stringBuilder.Append("穿戴列表: ");
            for (int i = 0; i < dressedList.Length; i++)
            {
                if (dressedList[i] != 0)
                {
                    stringBuilder.Append(string.Format("{0} ", dressedList[i].ToString()));
                }
            }
            return StringBuilderPool.ReleaseTemporaryAndToString(stringBuilder);
        }

        public string FashionClothesDyeToString(FashionClothes fashionClothes)
        {
            StringBuilder stringBuilder = StringBuilderPool.GetTemporary();
            if (fashionClothes.curUseScheme > -1)
            {
                if (fashionClothes.SchemeCount == 1)
                {
                    Color32 r = fashionClothes.GetColor(fashionClothes.curUseScheme, ETintIndex.R);
                    Color32 g = fashionClothes.GetColor(fashionClothes.curUseScheme, ETintIndex.G);
                    Color32 b = fashionClothes.GetColor(fashionClothes.curUseScheme, ETintIndex.B);
                    Color32 a = fashionClothes.GetColor(fashionClothes.curUseScheme, ETintIndex.A);

                    stringBuilder.AppendFormat("方案{0}颜色, r:{1}  g:{2}  b:{3}  a{4}", fashionClothes.curUseScheme, r.ToString(),
                        g.ToString(), b.ToString(), a.ToString());
                }
                else if (fashionClothes.SchemeCount == 2)
                {
                    Color32 r = fashionClothes.GetColor(fashionClothes.curUseScheme, ETintIndex.R);
                    Color32 g = fashionClothes.GetColor(fashionClothes.curUseScheme, ETintIndex.G);
                    Color32 b = fashionClothes.GetColor(fashionClothes.curUseScheme, ETintIndex.B);
                    Color32 a = fashionClothes.GetColor(fashionClothes.curUseScheme, ETintIndex.A);

                    stringBuilder.AppendFormat("方案{0}颜色, r:{1}  g:{2}  b:{3}  a{4}", fashionClothes.curUseScheme, r.ToString(),
                        g.ToString(), b.ToString(), a.ToString());

                    Color32 _r = fashionClothes.GetColor(1 - fashionClothes.curUseScheme, ETintIndex.R);
                    Color32 _g = fashionClothes.GetColor(1 - fashionClothes.curUseScheme, ETintIndex.G);
                    Color32 _b = fashionClothes.GetColor(1 - fashionClothes.curUseScheme, ETintIndex.B);
                    Color32 _a = fashionClothes.GetColor(1 - fashionClothes.curUseScheme, ETintIndex.A);

                    stringBuilder.AppendFormat("方案{0}颜色:  r:{1}  g:{2}  b:{3}  a{4}", (1 - fashionClothes.curUseScheme).ToString(), _r.ToString(),
                        _g.ToString(), _b.ToString(), _a.ToString());
                }
                return StringBuilderPool.ReleaseTemporaryAndToString(stringBuilder);
            }
            else
            {
                return string.Format("时装衣服{0}没有染色方案", fashionClothes.Id);
            }
        }

        public FashionClothes GetFashionClothes(uint fashionId)
        {
            FashionClothes fashionClothes = _FashionClothes.Find(x => x.Id == fashionId);
            return fashionClothes;
        }

        public FashionWeapon GetFashionWeapon(uint fashionId)
        {
            FashionWeapon fashionWeapon = _FashionWeapons.Find(x => x.Id == fashionId);
            return fashionWeapon;
        }

        public FashionAccessory GetFashionAcce(uint fashionId)
        {
            EHeroModelParts eHeroModelParts = parts[fashionId];
            FashionAccessory fashionAccessory = null;
            if (eHeroModelParts == EHeroModelParts.Jewelry_Head)
            {
                fashionAccessory = _FashionAcce_head_2.Find(x => x.Id == fashionId);
            }
            else if (eHeroModelParts == EHeroModelParts.Jewelry_Back)
            {
                fashionAccessory = _FashionAcce_back_3.Find(x => x.Id == fashionId);
            }
            else if (eHeroModelParts == EHeroModelParts.Jewelry_Waist)
            {
                fashionAccessory = _FashionAcce_waist_4.Find(x => x.Id == fashionId);
            }
            else if (eHeroModelParts == EHeroModelParts.Jewelry_Face)
            {
                fashionAccessory = _FashionAcce_face_5.Find(x => x.Id == fashionId);
            }
            return fashionAccessory;
        }
    }
}


