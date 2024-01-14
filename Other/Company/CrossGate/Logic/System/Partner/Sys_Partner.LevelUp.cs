using Packet;
using Logic.Core;
using System.Collections.Generic;
using System;
using Net;
using Lib.Core;
using Table;

namespace Logic
{
    public partial class Sys_Partner : SystemModuleBase<Sys_Partner>
    {
        public class LevelUpProgress
        {
            public bool isMaxLevel = false;
            public bool showCurProgress = true;
            public uint targetLevel = 0;
            public uint curLevel = 0;
            public float curProgress = 0f;
            public float addProgress = 0f;
            public uint totalExp = 0;

            public void Reset()
            {
                isMaxLevel = false;
                showCurProgress = true;
                targetLevel = 0u;
                curLevel = 0u;
                curProgress = 0f;
                addProgress = 0f;
                totalExp = 0u;
            }
        }
        #region Logic

        LevelUpProgress levelUpdata = new LevelUpProgress();
        public LevelUpProgress CalProgress(uint _infoId, uint _useNum, uint _itemId)
        {
            levelUpdata.Reset();

            //add exp
            CSVItem.Data itemData = CSVItem.Instance.GetConfData(_itemId);
            uint addExp = itemData.fun_value[1] * _useNum;
            
            Partner partner = GetPartnerInfo(_infoId);
            CSVPartner.Data partnerData = CSVPartner.Instance.GetConfData(_infoId);
            CSVPartnerLevel.Data levelUpData = CSVPartnerLevel.Instance.GetUniqData(partner.InfoId, partner.Level);

            uint totalExp = levelUpData.totol_exp + (uint)partner.Exp + addExp;
            levelUpdata.totalExp = totalExp;

            levelUpdata.isMaxLevel = IsMax(_infoId, totalExp);
            CalTargetLevel(partnerData.inti_attr, totalExp, ref levelUpdata);
            levelUpdata.showCurProgress = levelUpdata.targetLevel <= partner.Level + 1;
            levelUpdata.curProgress = partner.Exp / (levelUpData.upgrade_exp * 1.0f);

            if (levelUpdata.isMaxLevel)
            {
                levelUpdata.addProgress = 1.0f;
                levelUpdata.totalExp = CSVPartnerLevel.Instance.GetUniqData(partner.InfoId, levelUpdata.targetLevel).totol_exp;
            }
            else
            {
                CSVPartnerLevel.Data targetLevelData = CSVPartnerLevel.Instance.GetUniqData(partner.InfoId, levelUpdata.targetLevel);
                uint targetLevelTotalExp = targetLevelData.totol_exp;
                if (totalExp < targetLevelTotalExp)
                {
                    uint diff = targetLevelTotalExp - totalExp;
                    uint upExp = CSVPartnerLevel.Instance.GetUniqData(partner.InfoId, levelUpdata.targetLevel).upgrade_exp;
                    levelUpdata.addProgress = (upExp - diff) / (upExp * 1.0f);
                        //= 1.0f - diff / (upExp * 1.0f);
                }
                else if (totalExp == targetLevelTotalExp)
                {
                    levelUpdata.addProgress = 1f;
                    //if (partner.Level == levelUpdata.targetLevel)
                    //{
                    //    levelUpdata.addProgress = 1f;
                    //}
                    //else
                    //{
                    //    levelUpdata.addProgress = 1f;
                    //}

                    levelUpdata.totalExp = CSVPartnerLevel.Instance.GetUniqData(partner.InfoId, levelUpdata.targetLevel).totol_exp;
                }
            }
            
            return levelUpdata;
        }

        private bool IsMax(uint _infoId, uint _totalExp)
        {
            bool isMax = true;
            var dictDatas = CSVPartnerLevel.Instance.GetAll();
            foreach (var data in dictDatas)
            {
                if (data.attr_id == _infoId)
                {
                    if (_totalExp < data.totol_exp)
                    {
                        isMax = false;
                        break;
                    }
                }
            }

            return isMax;
        }

        private void CalTargetLevel(uint _attrId, uint _totalExp, ref LevelUpProgress progressData)
        {
            uint maxLevel = 0;
            var dictDatas = CSVPartnerLevel.Instance.GetAll();
            foreach (var data in dictDatas)
            {
                if (data.attr_id == _attrId)
                {
                    maxLevel = data.level;
                    if (_totalExp <= data.totol_exp)
                    {
                        progressData.targetLevel = maxLevel;
                        progressData.curLevel = maxLevel;

                        if (_totalExp < data.totol_exp)
                            progressData.curLevel -= 1;

                        break;
                    }
                }
            }

            if (progressData.targetLevel == 0)
            {
                progressData.targetLevel = maxLevel;
                progressData.curLevel = maxLevel;
            }
        }
        #endregion
    }
}