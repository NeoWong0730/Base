using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Net;
using Packet;
using Table;
using Google.Protobuf.Collections;

namespace Logic
{
    public partial class Sys_Equip : SystemModuleBase<Sys_Equip>
    {

        #region 评分计算
        //private double CalEquipScore(uint infoId, Equipment equip)
        //{
        //    double basicScore = CalBasicScore(infoId, equip);
        //    double greenScore = CalGreenScore(infoId, equip);
        //    //double smeltScore = CalSmeltScore(infoId, equip);
        //    double effectScore = CalEffectScore(infoId, equip);
        //    return basicScore + greenScore + effectScore;
        //}

        //private double CalBasicScore(uint infoId, Equipment equip)
        //{
        //    double result = 0;
        //    CSVEquipment.Data equipInfo = CSVEquipment.Instance.GetConfData(infoId);
        //    if (equipInfo.attr != null)
        //    {
        //        for (int i = 0; i < equip.BaseAttr.Count; ++i)
        //        {
        //            for (int j = 0; j < equipInfo.attr.Count; ++j)
        //            {
        //                if (equipInfo.attr[j][0] == equip.BaseAttr[i].Attr2.Id)
        //                {
        //                    result += 1.0f * equip.BaseAttr[i].Attr2.Value * equipInfo.attr[j][3] / equipInfo.attr[j][2];
        //                }
        //            }
        //        }
        //    }

        //    return result;
        //}

        //private double CalGreenScore(uint infoId, Equipment equip)
        //{
        //    double result = 0;
        //    CSVEquipment.Data equipInfo = CSVEquipment.Instance.GetConfData(infoId);

        //    double valueSum = 0;
        //    for (int i = 0; i < equip.GreenAttr.Count; ++i)
        //    {
        //        valueSum += equip.GreenAttr[i].Attr2.Value;
        //    }

        //    result = valueSum * equipInfo.score_coe;

        //    return result;
        //}

        //private double CalEffectScore(uint infoId, Equipment equip)
        //{
        //    double result = 0;

        //    for (int i = 0; i < equip.EffectAttr.Count; ++i)
        //    {
        //        CSVEquipmentEffect.Data effectInfo = CSVEquipmentEffect.Instance.GetConfData(equip.EffectAttr[i].Attr2.Id);
        //        if (effectInfo != null)
        //            result += effectInfo.score;
        //    }

        //    return result;
        //}

        /// <summary>
        /// 熔炼评分
        /// </summary>
        /// <param name="infoId"></param>
        /// <param name="equip"></param>
        /// <returns></returns>
        private double CalSmeltScore(uint infoId, Equipment equip)
        {
            double result = 0;
            CSVEquipment.Data equipInfo = CSVEquipment.Instance.GetConfData(infoId);

            List<AttributeRow> basicSmelt = new List<AttributeRow>();
            List<AttributeRow> leftSmelt = new List<AttributeRow>();

            for (int i = 0; i < equip.SmeltAttr.Count; ++i)
            {
                if (equip.SmeltAttr[i].Attr.Attr2.Id == 5
                    || equip.SmeltAttr[i].Attr.Attr2.Id == 7
                    || equip.SmeltAttr[i].Attr.Attr2.Id == 9
                    || equip.SmeltAttr[i].Attr.Attr2.Id == 11
                    || equip.SmeltAttr[i].Attr.Attr2.Id == 13) //策划说写死一级属性
                {
                    basicSmelt.Add(equip.SmeltAttr[i].Attr.Attr2);
                }
                else
                {
                    leftSmelt.Add(equip.SmeltAttr[i].Attr.Attr2);
                }
            }

            //计算一级属性评分 按绿字规则计算
            double basicResult = 0;
            for (int i = 0; i < basicSmelt.Count; ++i)
            {
                basicResult += basicSmelt[i].Value;
            }
            basicResult *= equipInfo.score_coe;

            //计算剩余评分
            double leftSum = 0;
            if (equipInfo.attr != null)
            {
                for (int i = 0; i < leftSmelt.Count; ++i)
                {
                    for (int j = 0; j < equipInfo.attr.Count; ++j)
                    {
                        if (equipInfo.attr[j][0] == leftSmelt[i].Id)
                        {
                            leftSum += 1.0f * leftSmelt[i].Value * equipInfo.attr[j][3] / equipInfo.attr[j][2];
                        }
                    }
                }
            }

            result = basicResult + leftSum;

            return result;
        }

        /// <summary>
        /// 计算套装评分
        /// </summary>
        /// <param name="infoId"></param>
        /// <param name="equip"></param>
        /// <returns></returns>
        private double CalSuitScore(uint infoId, Equipment equip)
        {
            CSVEquipment.Data equipInfo = CSVEquipment.Instance.GetConfData(infoId);

            double result = 0;
            if (equipInfo != null && equip.SuitTypeId != 0u)
            {
                CSVSuit.Data suitData = CSVSuit.Instance.GetSuitData(equip.SuitTypeId, equipInfo.slot_id[0]);
                if (equip.SuitAttr != null)
                {
                    for (int i = 0; i < equip.SuitAttr.Count; ++i)
                    {
                        AttributeElem element = equip.SuitAttr[i];
                        for (int j = 0; j < suitData.attr.Count; ++j)
                        {
                            if (element.Attr2.Id == suitData.attr[j][0])
                            {
                                result += 1.0f * element.Attr2.Value / suitData.attr[j][2] * suitData.attr[j][3];
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 宝石评分
        /// </summary>
        /// <param name="infoId"></param>
        /// <param name="equip"></param>
        /// <returns></returns>
        public double CalJewelScore(uint infoId, Equipment equip)
        {
            double result = 0;
            for (int i = 0; i < equip.JewelinfoId.Count; ++i)
            {
                if (equip.JewelinfoId[i] != 0)
                {
                    CSVJewel.Data jewelInfo = CSVJewel.Instance.GetConfData(equip.JewelinfoId[i]);
                    if (jewelInfo != null)
                    {
                        result += jewelInfo.score;
                    }
                }
            }

            return result;
        }

        public double CalJewelLevScore(ItemData equip)
        {
            CSVEquipment.Data equipData = CSVEquipment.Instance.GetConfData(equip.Id);
            uint gemSumLev = 0;
            for (int i = 0; i < equip.Equip.JewelinfoId.Count; ++i)
            {
                uint jewelInfoId = equip.Equip.JewelinfoId[i];
                if (jewelInfoId != 0)
                {
                    CSVJewel.Data jewInfo = CSVJewel.Instance.GetConfData(jewelInfoId);
                    gemSumLev += jewInfo.level;
                }
            }

            double score = 0;
            if (equipData.jew_lev_score != null) //没配置 就没有选项
            {
                for (int i = equipData.jew_lev_score.Count - 1; i >= 0; --i)
                {
                    if (gemSumLev >= equipData.jew_lev_score[i][0])
                    {
                        score = equipData.jew_lev_score[i][1];
                        break;
                    }
                }
            }

            return score;
        }

        /// <summary>
        ///正常评分(和装备对比有关)
        /// 总评分(用于评分显示):　正常评分　＋　宝石评分
        /// </summary>
        /// <param name="equipItem"></param>
        /// <returns></returns>
        public long CalEquipTotalScore(ItemData equipItem)
        {
            double result = 0;
            result = equipItem.Equip.Score + CalJewelScore(equipItem.Id, equipItem.Equip) + CalJewelLevScore(equipItem);
            return  (long)result;
        }

        /// <summary>
        /// 品质评分(和交易相关): 正常评分-熔炼评分
        /// </summary>
        /// <param name="equipItem"></param>
        /// <returns></returns>
        public long CalEquipQualityScore(ItemData equipItem)
        {
           return equipItem.Equip.Score - (long)CalSmeltScore(equipItem.Id, equipItem.Equip) - (long)CalSuitScore(equipItem.Id, equipItem.Equip);
        }

        /// <summary>
        /// 计算装备预览品质
        /// </summary>
        /// <param name="equipPara"></param>
        /// <returns></returns>
        public uint CalPreviewQuality(uint equipPara)
        {
            uint quality = 1;
            CSVEquipmentParameter.Data paraData = CSVEquipmentParameter.Instance.GetConfData(equipPara);
            if (paraData != null)
            {
                for (int i = paraData.quality_weight.Count - 1; i >= 0; --i)
                {
                    if (paraData.quality_weight[i] != 0)
                    {
                        quality = (uint)i + 1;
                        break;
                    }
                }
            }

            return quality;
        }

        /// <summary>
        /// 计算装备预览最低品质
        /// </summary>
        /// <param name="equipPara"></param>
        /// <returns></returns>
        public uint CalPreLowerQuality(uint equipPara)
        {
            uint quality = 1;
            CSVEquipmentParameter.Data paraData = CSVEquipmentParameter.Instance.GetConfData(equipPara);
            if (paraData != null)
            {
                for (int i = 0; i < paraData.quality_weight.Count; ++i)
                {
                    if (paraData.quality_weight[i] != 0)
                    {
                        quality = (uint)i + 1;
                        break;
                    }
                }
            }

            return quality;
        }

        #endregion

    }
}

