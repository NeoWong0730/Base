using Logic;
using System.Collections.Generic;

namespace Table
{
    public partial class CSVParam
    {
        public float UIDialogueAutoCD
        {
            get
            {
                if (TryGetValue(281, out CSVParam.Data data))
                {
                    return float.Parse(data.str_value) / 1000f;
                }
                return 0;
            }
        }

        public float UIMenuDialogueAutoCD
        {
            get
            {
                if (TryGetValue(282, out CSVParam.Data data))
                {
                    return float.Parse(data.str_value) / 1000f;
                }
                return 0;
            }
        }

        uint _upgradeSkillItemID;

        public uint UpgradeSkillItemID
        {
            get
            {
                if (_upgradeSkillItemID == 0)
                {
                    if (TryGetValue(264, out CSVParam.Data data))
                    {
                        _upgradeSkillItemID = uint.Parse((data.str_value).Split('|')[0]);
                    }
                }
                return _upgradeSkillItemID;
            }
        }

        uint _upgradeSkillItemAddNum;

        public uint UpgradeSkillItemAddNum
        {
            get
            {
                if (_upgradeSkillItemAddNum == 0)
                {
                    if (TryGetValue(264, out CSVParam.Data data))
                    {
                        _upgradeSkillItemAddNum = uint.Parse((data.str_value).Split('|')[1]);
                    }
                }
                return _upgradeSkillItemAddNum;
            }
        }

        uint _rankCostCoinPrice;

        public uint RankCostCoinPrice
        {
            get
            {
                if (_rankCostCoinPrice == 0)
                {
                    if (TryGetValue(1554, out CSVParam.Data data))
                    {
                        _rankCostCoinPrice = uint.Parse(data.str_value.Split('|')[0].Split('&')[1]);
                    }
                }
                return _rankCostCoinPrice;
            }
        }

        uint _levelCostCoinPrice;

        public uint LevelCostCoinPrice
        {
            get
            {
                if (_levelCostCoinPrice == 0)
                {
                    if (TryGetValue(1554, out CSVParam.Data data))
                    {
                        _levelCostCoinPrice = uint.Parse(data.str_value.Split('|')[1].Split('&')[1]);
                    }
                }
                return _levelCostCoinPrice;
            }
        }
    }
}
