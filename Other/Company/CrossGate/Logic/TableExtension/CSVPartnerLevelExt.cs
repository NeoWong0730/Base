using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Table
{
    public partial class CSVPartnerLevel
    {
        public CSVPartnerLevel.Data GetUniqData(uint _infoId, uint _level)
        {
            uint uniqId = _infoId * 1000 + _level;
            return GetConfData(uniqId);
        }
    }
}


