using System;
using System.Collections.Generic;

using Table;

namespace Logic
{
    public static class ItemQualityEffectHelper
    {

        public static CSVAwardQualityEffect.Data GetItemQualityEffectData(uint awardID)
        {
            //var awardID = Sys_pub.Instance.GetAwardID(itemID);

            //if (awardID == 0)
            //    return null;

            var awardData = CSVAward.Instance.GetConfData(awardID);

            if (awardData == null)
                return null;

            var data =  CSVAwardQualityEffect.Instance.GetConfData(awardData.quality);

            return data;
        }
    }
}
