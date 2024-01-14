using Packet;
using Logic.Core;
using Lib.Core;
using Net;
using System.Collections.Generic;
using Table;
using System.Json;
using UnityEngine;

namespace Logic
{
    public partial class Sys_Trade : SystemModuleBase<Sys_Trade>
    {

        /// <summary>
        /// 搜索类型
        /// </summary>
        public enum SearchPageType
        {
            None = 0,
            Normal = 1, //普通搜索
            Equipment = 2, //珍品装备搜索
            Pet = 3,        //珍品宠物
            MagicCore = 4, //宠物元核
            Ornament = 5, //饰品
        }

        public SearchPageType CurSearchPageType { get; set; } = SearchPageType.None;

        /// <summary>
        /// 设置搜索类型
        /// </summary>
        /// <param name="type"></param>
        public void SetSearchPageType(SearchPageType type)
        {
            CurSearchPageType = type;
            eventEmitter.Trigger(EEvents.OnSearchPageType, type);
        }

        public class SearchData
        {
            public bool isSearch = false;
            public bool bCross = false;
            public TradeShowType showType = TradeShowType.OnSale;
            public TradeSearchType searchType = TradeSearchType.Category;
            public ulong goodsUId = 0;
            public uint infoId = 0u;
            public uint Category = 0u;
            public uint SubCategory = 0u;
            public uint SubClass = 0u;
            public TradeSearchEquipParam equipParam = null;
            public TradeSearchPetParam petParam = null;
            public TradeSearchPetEquipParam coreParam = null;
            public TradeSearchOrnamentParam ornamentParam = null;

            private int clickCount = 0;

            public void Reset()
            {
                isSearch = false;
                bCross = false;
                showType = TradeShowType.OnSale;
                goodsUId = 0;
                infoId = 0;
                Category = 0u;
                SubCategory = 0u;
                SubClass = 0u;
                equipParam = null;
                petParam = null;
                coreParam = null;
                ornamentParam = null;

                clickCount = 0;
            }

            public void OnClick()
            {
                clickCount++;
                if (clickCount > 1)
                    Reset();
            }
        }
        public SearchData SearchParam = new SearchData();

        #region 装备搜索
        /// <summary>
        /// 装备搜索，等级搜索
        /// </summary>
        public uint EquipLevel = 0u;

        /// <summary>
        /// 选中的装备
        /// </summary>
        public uint SelectedEquipId = 0u;

        /// <summary>
        /// 装备搜索，装备价格范围
        /// </summary>
        public uint[] EquipPriceRange = new uint[2];

        /// <summary>
        /// 装备特殊效果 最多同时选中2条
        /// </summary>
        public List<uint> EquipSpeicalAttrIds = new List<uint>(2);

        /// <summary>
        /// 装备选择的基础属性, //最多三条
        /// </summary>
        public class EquipBasicAttr
        {
            public uint attrId = 0u;
            public uint attrValue = 0u;
            public bool addSmelt = false;

            public void Reset()
            {
                attrId = 0u;
                attrValue = 0u;
                addSmelt = false;
            }
        }
        public List<EquipBasicAttr> EquipBasicAttrArray = new List<EquipBasicAttr>();

        /// <summary>
        /// 装备附加效果，绿字属性 最多同时选中2条
        /// </summary>
        public class EquipAddition
        {
            public List<uint> attrIds = new List<uint>(2);
            public uint totalValue = 0u;
            public bool addSmelt = false;

            public void Reset()
            {
                attrIds.Clear();
                totalValue = 0u;
                addSmelt = false;
            }
        }
        public EquipAddition EquipAddtionData = new EquipAddition();

        /// <summary>
        /// 搜索装备，装备评分
        /// </summary>
        public uint EquipScore = 0u;

        /// <summary>
        /// 搜索装备，装备交易状态
        /// </summary>
        public uint EquipTradeState = 0u; //0 上架，1，公使，2议价
        /// <summary>
        /// 清理装备搜索数据
        /// </summary>
        public void ClearSearchEquipData()
        {
            EquipLevel = 0u;
            SelectedEquipId = 0u;

            EquipSpeicalAttrIds.Clear();

            for (int i = 0; i < EquipPriceRange.Length; ++i)
                EquipPriceRange[i] = 0u;

            for (int i = 0; i < EquipBasicAttrArray.Count; ++i)
                EquipBasicAttrArray[i].Reset();

            EquipAddtionData.Reset();

            EquipScore = 0u;

            EquipTradeState = (uint)TradeShowType.OnSaleAndDiscuss;
        }
        #endregion



        #region 宠物搜索
        /// <summary>
        /// 宠物搜索，等级搜索
        /// </summary>
        public uint PetLevel = 0u;

        /// <summary>
        /// 选中的宠物
        /// </summary>
        public uint SelectedPetId = 0u;

        /// <summary>
        /// 宠物搜索，宠物价格范围
        /// </summary>
        public uint[] PetPriceRange = new uint[2];

        /// <summary>
        /// 宠物资质档位
        /// </summary>
        public enum EPetQuality
        {
            vitGrade = 2011135, //体力
            snhGrade = 2011136, //力量
            intenGrade = 2011137, //强度
            speedGrade = 2011138, //速度
            magicGrade = 2011139, //魔法
            lostGrade = 2011235,  //掉档数
            //growthGrade = 2011140, //成长
        }

        /// <summary>
        /// 宠物搜索，宠物总资质
        /// </summary>
        public uint[] TotalPetQualitys = new uint[] 
        {
            //(uint)EPetQuality.vitGrade,
            //(uint)EPetQuality.snhGrade,
            //(uint)EPetQuality.intenGrade,
            //(uint)EPetQuality.speedGrade,
            //(uint)EPetQuality.magicGrade,
            (uint)EPetQuality.lostGrade,
            //(uint)EPetQuality.growthGrade,
        };

        public class PetQualityData
        {
            public uint qualityId = 0;
            public uint qualityValue = 0;

            public void Reset()
            {
                qualityId = 0;
                qualityValue = 0;
            }
        }
        /// <summary>
        ///宠物搜索，选中的资质
        /// </summary>
        public List<PetQualityData> SelectedQualitys = new List<PetQualityData>(8);

        /// <summary>
        /// 宠物搜索，选中的技能类型
        /// </summary>
        public uint SelectedSkillType = 0;

        /// <summary>
        /// 宠物搜索，选中的包含技能
        /// </summary>
        public List<uint> SelectedSkillIds = new List<uint>(16);

        /// <summary>
        /// 宠物搜索，技能数量
        /// </summary>
        public uint PetSkillsNum = 0;

        /// <summary>
        /// 宠物搜索，选中的新的宠物技能
        /// </summary>
        public List<uint> SelectedNewSkillIds = new List<uint>(16);

        /// <summary>
        /// 宠物搜索，评分限制
        /// </summary>
        public uint PetScore = 0;

        /// <summary>
        /// 宠物搜索，交易状态
        /// </summary>
        public uint PetTradeState = 0u; //0 上架，1，公使，2议价

        /// <summary>
        /// 宠物搜索，重置数据
        /// </summary>
        public void ClearSearchPetData()
        {
            PetLevel = 0u;
            SelectedPetId = 0u;

            for (int i = 0; i < PetPriceRange.Length; ++i)
                PetPriceRange[i] = 0u;

            for (int i = 0; i < SelectedQualitys.Count; ++i)
            {
                if (SelectedQualitys[i] != null)
                    SelectedQualitys[i].Reset();
            }

            SelectedSkillType = 0u;
            SelectedSkillIds.Clear();
            PetSkillsNum = 0;
            SelectedNewSkillIds.Clear();
            PetScore = 0u;
            PetTradeState = 0u;
        }
        #endregion
        
         #region 元核搜索
         /// <summary>
        /// 选中的元核
        /// </summary>
        public uint SelectedCoreId = 0u;

        /// <summary>
        /// 元核价格范围
        /// </summary>
        public uint[] CorePriceRange = new uint[2];

        public class CoreAttrData
        {
            public uint attrId = 0;
            public uint attrValue = 0;

            public void Reset()
            {
                attrId = 0;
                attrValue = 0;
            }
        }
        /// <summary>
        ///元核搜索，选中的基础属性
        /// </summary>
        public List<CoreAttrData> SelectedCoreAttrs = new List<CoreAttrData>(8);

        /// <summary>
        /// 元核搜索，选中的套装
        /// </summary>
        public uint SelectedCoreSuitSkillId = 0;

        /// <summary>
        /// 元核搜索，选中外观
        /// </summary>
        public uint SelectedCoreDressId = 0;
        
        /// <summary>
        /// 元核搜索，选中特效
        /// </summary>
        public uint SelectedCoreEffectId = 0;

        /// <summary>
        /// 元核搜索，交易状态
        /// </summary>
        public uint CoreTradeState = 0u; //0 上架，1，公使，2议价

        /// <summary>
        /// 元核搜索，重置数据
        /// </summary>
        public void ClearSearchCoreData()
        {
            SelectedCoreId = 0u;

            for (int i = 0; i < CorePriceRange.Length; ++i)
                CorePriceRange[i] = 0u;

            for (int i = 0; i < SelectedCoreAttrs.Count; ++i)
            {
                if (SelectedCoreAttrs[i] != null)
                    SelectedCoreAttrs[i].Reset();
            }

            SelectedCoreSuitSkillId = 0u;
            SelectedCoreDressId = 0;
            SelectedCoreEffectId = 0;
            CoreTradeState = 0u;
        }
        #endregion
        
        #region =========饰品搜索========

        ///选中饰品类型
        public uint SelectOraType = 0u;
        
        /// <summary>
        /// 选中的饰品
        /// </summary>
        public uint SelectedOraId = 0u;

        /// <summary>
        /// 饰品价格范围
        /// </summary>
        public uint[] OraPriceRange = new uint[2];

        public class OraExtraAttrData
        {
            public uint attrId = 0;
            public uint attrValue = 0;
            public bool isSkill = false;

            public void Reset()
            {
                attrId = 0;
                attrValue = 0;
                isSkill = false;
            }
        }
        /// <summary>
        ///饰品搜索，选中的基础属性
        /// </summary>
        public List<OraExtraAttrData> SelectedOraAttrs = new List<OraExtraAttrData>(8);

        /// <summary>
        /// 饰品搜索，交易状态
        /// </summary>
        public uint OraTradeState = 0u; //0 上架，1，公使，2议价

        /// <summary>
        /// 饰品搜索，重置数据
        /// </summary>
        public void ClearSearchOraData()
        {
            SelectOraType = 0u;
            SelectedOraId = 0u;

            for (int i = 0; i < OraPriceRange.Length; ++i)
                OraPriceRange[i] = 0u;

            for (int i = 0; i < SelectedOraAttrs.Count; ++i)
            {
                if (SelectedOraAttrs[i] != null)
                    SelectedOraAttrs[i].Reset();
            }
            
            OraTradeState = 0u;
        }
        #endregion =========饰品搜索========

        /// <summary>
        /// 根据等级获取装备列表(可上架)
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public List<uint> GetEquipByLevel(uint level)
        {
            List<uint> tempList = new List<uint>(32);
            foreach(var data in CSVEquipment.Instance.GetAll())
            {
                if (data.sale_least != 0 && data.equipment_level == level)
                {
                    tempList.Add(data.id);
                }
            }

            return tempList;
        }

        /// <summary>
        /// 根据等级获取宠物列表(可上架)， 宠物搜索
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public List<uint> GetPetByLevel(uint level)
        {
            List<uint> tempList = new List<uint>(32);
            foreach (var data in CSVPetNew.Instance.GetAll())
            {
                CSVItem.Data item = CSVItem.Instance.GetConfData(data.id);
                if(item != null && item.on_sale && data.search_pet)
                {
                    if (data.card_lv == level)
                    {
                        tempList.Add(data.id);
                    }
                }
            }

            return tempList;
        }

        /// <summary>
        /// 宠物搜索，获得宠物剩余资质
        /// </summary>
        /// <returns></returns>
        public List<uint> LeftPetQualitys()
        {
            List<uint> temp = new List<uint>(6);
            temp.AddRange(TotalPetQualitys);
            for (int i = 0; i < SelectedQualitys.Count; ++i)
            {
                if (SelectedQualitys[i].qualityId != 0u)
                    temp.Remove(SelectedQualitys[i].qualityId);
            }

            return temp;
        }
        
        /// <summary>
        /// 元核搜索，获得元核剩余属性
        /// </summary>
        /// <returns></returns>
        public List<uint> LeftCoreAttrIds(uint attrLibId)
        {
            List<uint> temp = new List<uint>(6);
            int count = CSVPetEquipAttr.Instance.Count;
            for (int i = 0; i < count; ++i)
            {
                CSVPetEquipAttr.Data data = CSVPetEquipAttr.Instance.GetByIndex(i);
                if (data.group_id == attrLibId)
                {
                    if (temp.IndexOf(data.attr_id) < 0)
                        temp.Add(data.attr_id);
                }
            }

            return temp;
        }

        /// <summary>
        /// 计算搜索装备数据
        /// </summary>
        /// <returns></returns>
        public TradeSearchEquipParam CalEquipParam()
        {
            TradeSearchEquipParam equipParam = new TradeSearchEquipParam();

            //price change
            if (EquipPriceRange[0] != 0u)
            {
                equipParam.MinPrice = new UInt32Value();
                equipParam.MinPrice.Value = EquipPriceRange[0];
            }

            if (EquipPriceRange[1] != 0u)
            {
                equipParam.MaxPrice = new UInt32Value();
                equipParam.MaxPrice.Value = EquipPriceRange[1];
            }

            //特效属性
            for (int i = 0; i < EquipSpeicalAttrIds.Count; ++i)
            {
                CSVEquipmentEffect.Data data = CSVEquipmentEffect.Instance.GetConfData(EquipSpeicalAttrIds[i]);
                equipParam.EffectPassiveId.Add(data.effect);
            }

            //基础属性
            for (int i = 0; i < EquipBasicAttrArray.Count; ++i)
            {
                EquipBasicAttr basicAttr = EquipBasicAttrArray[i];
                if (basicAttr != null && basicAttr.attrId != 0u)
                {
                    SearchAttrParam param = new SearchAttrParam();
                    param.AttrId = basicAttr.attrId;
                    CSVAttr.Data data = CSVAttr.Instance.GetConfData(basicAttr.attrId);
                    if (data != null)
                    {
                        param.Value = (int)basicAttr.attrValue;
                        if (data.show_type == 2u)
                            param.Value = param.Value * 10;
                    }
                    
                    param.AddSmelt = basicAttr.addSmelt;

                    equipParam.BaseParams.Add(param);
                }
            }

            //绿字属性(附加属性)
            equipParam.GreeParams = new SearchGreenParam();
            equipParam.GreeParams.Value = (int)EquipAddtionData.totalValue;
            equipParam.GreeParams.AddSmelt = EquipAddtionData.addSmelt;
            for (int i = 0; i < EquipAddtionData.attrIds.Count; ++i)
            {
                if (EquipAddtionData.attrIds[i] != 0u)
                {
                    equipParam.GreeParams.AttrId.Add(EquipAddtionData.attrIds[i]);
                }
            }

            //评分
            //if (EquipScore != 0u)
            //{
            //    equipParam.Score = new UInt32Value();
            //    equipParam.Score.Value = EquipScore;
            //}
            StartSearchCD();

            return equipParam;
        }

        /// <summary>
        /// 计算搜索宠物数据
        /// </summary>
        /// <returns></returns>
        public TradeSearchPetParam CalPetParam()
        {
            TradeSearchPetParam petParam = new TradeSearchPetParam();

            //price change
            if (PetPriceRange[0] != 0u)
            {
                petParam.MinPrice = new UInt32Value();
                petParam.MinPrice.Value = PetPriceRange[0];
            }

            if (PetPriceRange[1] != 0u)
            {
                petParam.MaxPrice = new UInt32Value();
                petParam.MaxPrice.Value = PetPriceRange[1];
            }

            //宠物资质
            for (int i = 0; i < SelectedQualitys.Count; ++i)
            {
                if (SelectedQualitys[i] != null && SelectedQualitys[i].qualityId != 0u)
                {
                    switch ((EPetQuality)SelectedQualitys[i].qualityId)
                    {
                        //case EPetQuality.vitGrade:
                        //    if (SelectedQualitys[i].qualityValue != 0u)
                        //    {
                        //        petParam.VitGrade = new UInt32Value();
                        //        petParam.VitGrade.Value = SelectedQualitys[i].qualityValue * 1000;
                        //    }
                        //    break;
                        //case EPetQuality.snhGrade:
                        //    if (SelectedQualitys[i].qualityValue != 0u)
                        //    {
                        //        petParam.SnhGrade = new UInt32Value();
                        //        petParam.SnhGrade.Value = SelectedQualitys[i].qualityValue * 1000;
                        //    }
                        //    break;
                        //case EPetQuality.intenGrade:
                        //    if (SelectedQualitys[i].qualityValue != 0u)
                        //    {
                        //        petParam.IntenGrade = new UInt32Value();
                        //        petParam.IntenGrade.Value = SelectedQualitys[i].qualityValue * 1000;
                        //    }
                        //    break;
                        //case EPetQuality.speedGrade:
                        //    if (SelectedQualitys[i].qualityValue != 0u)
                        //    {
                        //        petParam.SpeedGrade = new UInt32Value();
                        //        petParam.SpeedGrade.Value = SelectedQualitys[i].qualityValue * 1000;
                        //    }
                        //    break;
                        //case EPetQuality.magicGrade:
                        //    if (SelectedQualitys[i].qualityValue != 0u)
                        //    {
                        //        petParam.MagicGrade = new UInt32Value();
                        //        petParam.MagicGrade.Value = SelectedQualitys[i].qualityValue * 1000;
                        //    }
                        //    break;
                        case EPetQuality.lostGrade:
                            if (SelectedQualitys[i].qualityValue >= 0u)
                            {
                                petParam.LostGrade = new UInt32Value();
                                petParam.LostGrade.Value = SelectedQualitys[i].qualityValue;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            //包含技能
            for (int i = 0; i < SelectedSkillIds.Count; ++i)
            {
                if (SelectedSkillIds[i] != 0u)
                    petParam.ContainSkills.Add(SelectedSkillIds[i]);
            }

            //宠物技能
            for (int i = 0; i < SelectedNewSkillIds.Count; ++i)
            {
                if (SelectedNewSkillIds[i] != 0u)
                    petParam.PetSkills.Add(SelectedNewSkillIds[i]);
            }

            //技能总数
            if (PetSkillsNum != 0u)
            {
                petParam.SkillCount = new UInt32Value();
                petParam.SkillCount.Value = PetSkillsNum;
            }

            //评分
            //if (PetScore != 0u)
            //{
            //    petParam.Score = new UInt32Value();
            //    petParam.Score.Value = PetScore;
            //}

            StartSearchCD();

            return petParam;
        }

        /// <summary>
        /// 计算搜索宠物元核数据
        /// </summary>
        /// <returns></returns>
        public TradeSearchPetEquipParam CalCoreParam()
        {
            TradeSearchPetEquipParam coreParam = new TradeSearchPetEquipParam();

            //price change
            if (CorePriceRange[0] != 0u)
            {
                coreParam.MinPrice = new UInt32Value();
                coreParam.MinPrice.Value = CorePriceRange[0];
            }

            if (CorePriceRange[1] != 0u)
            {
                coreParam.MaxPrice = new UInt32Value();
                coreParam.MaxPrice.Value = CorePriceRange[1];
            }

            //基础属性
            for (int i = 0; i < SelectedCoreAttrs.Count; i++)
            {
                if (SelectedCoreAttrs[i] != null
                    && SelectedCoreAttrs[i].attrId != 0)
                {
                    TradeSearchPetEquipParam.Types.AttrParam param = new TradeSearchPetEquipParam.Types.AttrParam();
                    param.AttrId = SelectedCoreAttrs[i].attrId;
                    param.Value = (int)SelectedCoreAttrs[i].attrValue;
                    coreParam.BaseParams.Add(param);
                }
            }
            
            //套装
            if (SelectedCoreSuitSkillId != 0u)
            {
                coreParam.SuitSkill = new UInt32Value();
                coreParam.SuitSkill.Value = SelectedCoreSuitSkillId;
            }
            
            //外观
            if (SelectedCoreDressId != 0u)
            {
                coreParam.SuitAppearance = new UInt32Value(); 
                coreParam.SuitAppearance.Value = SelectedCoreDressId;
            }
            
            //特效
            if (SelectedCoreEffectId != 0u)
            {
                CSVPetEquipEffect.Data data = CSVPetEquipEffect.Instance.GetConfData(SelectedCoreEffectId);
                if (data != null)
                {
                    coreParam.EffectPassiveId = new UInt32Value();
                    coreParam.EffectPassiveId.Value = data.effect;
                }
            }


            StartSearchCD();

            return coreParam;
        }
        
        /// <summary>
        /// 计算搜索饰品数据
        /// </summary>
        /// <returns></returns>
        public TradeSearchOrnamentParam CalOraParam()
        {
            TradeSearchOrnamentParam oraParam = new TradeSearchOrnamentParam();

            //price change
            if (OraPriceRange[0] != 0u)
            {
                oraParam.MinPrice = new UInt32Value();
                oraParam.MinPrice.Value = OraPriceRange[0];
            }

            if (OraPriceRange[1] != 0u)
            {
                oraParam.MaxPrice = new UInt32Value();
                oraParam.MaxPrice.Value = OraPriceRange[1];
            }

            //额外属性
            for (int i = 0; i < SelectedOraAttrs.Count; i++)
            {
                if (SelectedOraAttrs[i] != null
                    && SelectedOraAttrs[i].attrId != 0)
                {
                    TradeSearchOrnamentParam.Types.ExtParam param = new TradeSearchOrnamentParam.Types.ExtParam();
                    param.InfoId = SelectedOraAttrs[i].attrId;
                    param.Value = (int)SelectedOraAttrs[i].attrValue;
                    param.IsSkill = SelectedOraAttrs[i].isSkill;
                    if (!param.IsSkill)
                    {
                        CSVAttr.Data data = CSVAttr.Instance.GetConfData(param.InfoId);
                        if (data != null && data.show_type == 2u)
                        {
                            param.Value = param.Value * 10;
                        }
                    }

                    oraParam.ExtParams.Add(param);
                }
            }


            StartSearchCD();

            return oraParam;
        }

        private void StartSearchCD()
        {
            searchCD = true;
            searchTimer?.Cancel();
            searchTimer = Timer.Register(10f, () =>
            {
                searchCD = false;
            });
        }

        public bool IsInSearchCD()
        {
            return searchCD;
        }

        #region 普通搜索
        public class HistoryInfo
        {
            public List<uint> idList = new List<uint>();

            public void DeserializeObject(JsonObject jo)
            {
                JsonHeler.DeserializeObject(jo, this);

                if (jo.ContainsKey("idList"))
                {
                    Clear();
                    JsonArray ja = (JsonArray)jo["idList"];
                    foreach (var item in ja)
                    {
                        idList.Add(item);
                    }
                }
            }

            public void Clear()
            {
                idList.Clear();
            }
        }


        private string SearchHistoryPath = "TradeSearchHistory";
        public HistoryInfo historyInfo = new HistoryInfo();

        public void LoadSearchHistory()
        {
            historyInfo.Clear();

            JsonObject json = FileStore.ReadJson(SearchHistoryPath);
            if (json != null)
            {
                historyInfo.DeserializeObject(json);
            }
        }

        public void SaveHistory()
        {
            FileStore.WriteJson(SearchHistoryPath, historyInfo);
        }

        public List<uint> GetSearchHistory()
        {
            return historyInfo.idList;
        }

        public void PushHistory(uint goodId)
        {
            if (historyInfo.idList.Contains(goodId))
                return;
            historyInfo.idList.Add(goodId);
            if (historyInfo.idList.Count > 6) //最多6个
                historyInfo.idList.RemoveAt(0);
        }
        #endregion

        #region 跳转交易行
        public class TelData
        {
            public uint telType = 0u; //0, 查找, 1 上架, 2 跳转到某个分类
            public ulong goodsUId = 0;
            public uint itemInfoId = 0u;
            public bool bCross = false;
            public TradeShowType tradeShowType = TradeShowType.OnSale;
            public ItemData itemData = null;
        }

        public void TradeFind(uint itemInfoId)
        {
            TelData telData = new TelData();
            telData.telType = 0u;
            telData.itemInfoId = itemInfoId;

            if (Sys_FamilyResBattle.Instance.InFamilyBattle) {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3230000059));
                return;
            }

            UIManager.OpenUI(EUIID.UI_Trade, false, telData);
        }

        public void SaleItem(ItemData item)
        {
            TelData telData = new TelData();
            telData.telType = 1u;
            telData.itemData = item;

            if (Sys_FamilyResBattle.Instance.InFamilyBattle) {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3230000059));
                return;
            }

            UIManager.OpenUI(EUIID.UI_Trade, false, telData);
        }

        public void FindCategory(uint categoryId)
        {
            TelData telData = new TelData();
            telData.telType = 2u;
            telData.itemInfoId = categoryId;

            if (Sys_FamilyResBattle.Instance.InFamilyBattle) {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3230000059));
                return;
            }

            UIManager.OpenUI(EUIID.UI_Trade, false, telData);
        }
        #endregion
    }
}
