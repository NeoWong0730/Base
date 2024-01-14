using System.Collections;
using System.Collections.Generic;
using Table;
using Net;
using Packet;
using Lib.Core;
using UnityEngine;
using Logic.Core;
using Google.Protobuf.Collections;
using System;

namespace Logic
{
    public class Sys_ElementalCrystal : SystemModuleBase<Sys_ElementalCrystal>
    {
        public const uint crystalBaseDebrisId = 432900;
        public const uint crystalBaseId = 432000;

        public const uint LAND_DEBRIS = 432901;
        public const uint WATER_DEBRIS = 432902;
        public const uint FIRE_DEBRIS = 432903;
        public const uint WIND_DEBRIS = 432904;

        public const uint LAND = 432010;
        public const uint WATER = 432020;
        public const uint FIRE = 432030;
        public const uint WIND = 432040;

        public uint durabilityType = 0;      //0 不足10%  1消耗完了
        private uint m_FormulaA;
        private uint m_FormulaB;

        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public uint curEquipCrystalDurability
        {
            get
            {
                ItemData itemData = GetEquipedCrystal();
                if (itemData != null)
                {
                    return itemData.crystal.Durability;
                }
                else
                {
                    return 0;
                }
            }
        }

        public enum EEvents
        {
            OnSetActiveMenuCrystal,
            OnRefreshExchangeProp,
        }

        public override void Init()
        {
            m_FormulaA = CSVFormulaParam.Instance.GetConfData(13).value;
            m_FormulaB = CSVFormulaParam.Instance.GetConfData(14).value;
        }

        public override void OnLogin()
        {
            durabilityType = 1;
        }

        public bool bEquiped
        {
            get { return GetEquipedCrystal() != null; }
        }

        public void OpenTips()
        {
            UIManager.OpenUI(EUIID.UI_ElementalCrystal_Tip, false, new Tuple<uint, uint>(GetEquipedCrystal().Id, durabilityType));
        }

        public ItemData GetEquipedCrystal()
        {
            int boxId = (int)BoxIDEnum.BoxIdCrystal;
            if (!Sys_Bag.Instance.BagItems.TryGetValue(boxId, out List<ItemData> itemDatas))
            {
                return null;
            }
            if (itemDatas.Count == 0)
            {
                return null;
            }
            if (itemDatas.Count > 1)
            {
                DebugUtil.LogErrorFormat("穿戴的元素水晶大于1");
                return null;
            }
            return itemDatas[0];
        }


        //元素伤害加成=(10000+A* (攻击方风属性* 受击方地属性+攻击方地属性* 受击方水属性+攻击方水属性* 受击方火属性+
        //攻击方火属性* 受击方风属性-受击方风属性* 攻击方地属性-受击方地属性* 攻击方水属性-受击方水属性* 攻击方火属性-
        //受击方火属性* 攻击方风属性)+A*10*(攻击方风属性+攻击方地属性+攻击方水属性+攻击方火属性-受击方风属性-受击方地属性-受击方水属性-受击方火属性))/10000*100%
        public float GetDamage_Physical(int land_A, int water_A, int fire_A, int wind_A, int land_B, int water_B, int fire_B, int wind_B)
        {
            float res = 0;
            int value1 = wind_A * land_B + land_A * water_B + water_A * fire_B + fire_A * wind_B - wind_B * land_A - land_B * water_A - water_B * fire_A - fire_B * wind_A;
            int value2 = wind_A + land_A + water_A + fire_A - land_B - water_B - fire_B - wind_B;
            res = (10000 + m_FormulaA * value1 + m_FormulaA * 10 * value2) / 10000f;
            return res;
        }


        //元素伤害加成=(10000+A*(攻击方风属性*受击方地属性+攻击方地属性*受击方水属性+攻击方水属性*受击方火属性+
        //攻击方火属性*受击方风属性-受击方风属性*攻击方地属性-受击方地属性*攻击方水属性-受击方水属性*攻击方火属性-受击方火属性*攻击方风属性)
        //+A*10*(攻击方风属性+攻击方地属性+攻击方水属性+攻击方火属性-受击方风属性-受击方地属性-受击方水属性-受击方火属性))/10000*100%+
        //B*(技能风属性*受击方地属性+技能地属性*受击方水属性+技能水属性*受击方火属性+技能火属性*受击方风属性-受击方风属性*技能地属性-受击方地属性*技能水属性-
        //受击方水属性*技能火属性-受击方火属性*技能风属性)/10000*100%
        public float GetDamage_Magic(int land_A, int water_A, int fire_A, int wind_A, int land_B, int water_B, int fire_B, int wind_B,
            int landMagic, int waterMagic, int fireMagic, int windMagic)
        {
            float res = 0;
            float value1 = GetDamage_Physical(land_A, water_A, fire_A, wind_A, land_B, water_B, fire_B, wind_B);
            float value2 = windMagic * land_B + landMagic * water_B + waterMagic * fire_B + fireMagic * wind_B
                - wind_B * landMagic - land_B * waterMagic - water_B * fireMagic - fire_B * windMagic;
            res = value1 + m_FormulaB * value2 / 10000f;
            return res;
        }
    }
}


