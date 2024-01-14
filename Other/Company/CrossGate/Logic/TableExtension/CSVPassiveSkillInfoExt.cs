using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Table
{
    public static class CSVPassiveSkillInfoExt
    {
        private static Dictionary<uint, Dictionary<uint, uint>> SkillCareerLevelConds = new Dictionary<uint, Dictionary<uint, uint>>();

        public static Dictionary<uint, uint> SkillCareerLevelCond(this CSVPassiveSkillInfo.Data data)
        {
            if (!SkillCareerLevelConds.TryGetValue(data.id, out Dictionary<uint, uint> _skillCareerLevelCond))
            {
                _skillCareerLevelCond = new Dictionary<uint, uint>();

                if (data.need_career_lv != null)
                {
                    foreach (var pair in data.need_career_lv)
                    {
                        _skillCareerLevelCond[pair[0]] = pair[1];
                    }
                }

                SkillCareerLevelConds.Add(data.id, _skillCareerLevelCond);
            }

            return _skillCareerLevelCond;
        }
    }
}