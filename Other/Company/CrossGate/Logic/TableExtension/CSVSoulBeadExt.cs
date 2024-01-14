using Logic;
using System.Collections.Generic;

namespace Table
{
    public partial class CSVSoulBead
    {
        /// <summary>
        /// 取得类型技能解锁等级
        /// </summary>
        /// <param name="type">魔珠类型</param>
        /// <returns>返回解锁等级数据</returns>
        public CSVSoulBead.Data GetSkillLockLevelData(uint type)
        {
            int count = Count;
            CSVSoulBead.Data data = null;
            for (int index = 0; index < count; index++)
            {
                var soulBeadData = GetByIndex(index);

                if (soulBeadData.type == type && null != soulBeadData.special_skill_group)
                {
                    if(data == null)
                    {
                        data = soulBeadData;
                    }
                    else
                    {
                        if(data.level > soulBeadData.level)
                        {
                            data = soulBeadData;
                        }
                    }
                }
            }
            return data;
        }
    }
}
