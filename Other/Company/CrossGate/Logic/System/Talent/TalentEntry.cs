using Logic.Core;
using Lib.Core;
using Table;
using System.Collections.Generic;
using System;

namespace Logic {
    public class TalentBranch {
        public int schemeIndex;
        public uint id;
        public uint name;
        public uint iconId;

        public uint usedPoint {
            get {
                uint count = 0;
                foreach (var kvp in talents) {
                    count += kvp.level;
                }
                return count;
            }
        }
        private List<TalentEntry> _talents;
        public List<TalentEntry> talents {
            get {
                if (_talents == null) {
                    _talents = new List<TalentEntry>();
                    foreach (var kvp in Sys_Talent.Instance.schemes[schemeIndex].talents) {
                        if (kvp.Value.csv.branch_id == id) {
                            _talents.Add(kvp.Value);
                        }
                    }
                }
                return _talents;
            }
        }

        public TalentBranch(uint id, uint name, uint iconId) {
            this.id = id;
            this.name = name;
            this.iconId = iconId;
            this._talents = null;
        }
    }

    public class TalentEntry {
        public int schemeIndex;
        // client
        public uint id;
        public CSVTalent.Data csv;

        // server
        public uint level;

        public TalentBranch ownerBranch {
            get {
                return Sys_Talent.Instance.schemes[schemeIndex].branchs[csv.branch_id];
            }
        }

        public CSVPassiveSkillInfo.Data CurrentPassiveSkill {
            get {
                if (level <= 0 || level > csv.skill_id.Count) {
                    return null;
                }
                return CSVPassiveSkillInfo.Instance.GetConfData(csv.skill_id[(int)level - 1]);
            }
        }
        public CSVPassiveSkillInfo.Data NextPassiveSkill {
            get {
                if (level < 0 || level >= csv.skill_id.Count) {
                    return null;
                }
                return CSVPassiveSkillInfo.Instance.GetConfData(csv.skill_id[(int)level]);
            }
        }
        public CSVActiveSkillInfo.Data CurrentActiveSkill {
            get {
                if (level <= 0 || level > csv.skill_id.Count) {
                    return null;
                }
                return CSVActiveSkillInfo.Instance.GetConfData(csv.skill_id[(int)level - 1]);
            }
        }
        public CSVActiveSkillInfo.Data NextActiveSkill {
            get {
                if (level < 0 || level >= csv.skill_id.Count) {
                    return null;
                }
                return CSVActiveSkillInfo.Instance.GetConfData(csv.skill_id[(int)level]);
            }
        }

        // 所在分支使用的天赋点是否满足 >= pre_unm
        public bool IsBranchPointReached {
            get {
                return ownerBranch.usedPoint >= csv.pre_unm;
            }
        }
        // 剩余可使用天赋是否满足
        public bool IsPointEnough {
            get {
                return Sys_Talent.Instance.schemes[schemeIndex].canUseTalentPoint >= Sys_Talent.UpgradeTalentLevelCostPoint;
            }
        }
        // 前置天赋等级是否满足
        public bool IsPreReached {
            get {
                bool ret = true;
                var preLimit = PreLimits;
                for (int i = 0, length = preLimit.Count; i < length; ++i) {
                    ret &= (preLimit[i].Item1.level >= preLimit[i].Item2);
                }
                return ret;
            }
        }
        public bool CanUpgrade {
            get { return level < csv.lev && IsPointEnough && IsBranchPointReached && IsPreReached; }
        }

        public int PositionIndex {
            get {
                return (int)((csv.position[0] - 1u) * 4 + csv.position[1] - 1u);
            }
        }
        public string PreIds() {
            string ret = "";
            var pers = PreLimits;
            for (int i = 0, length = pers.Count; i < length; ++i) {
                ret += " " + pers[i].Item1.id.ToString();
            }
            return ret;
        }

        private List<Tuple<TalentEntry, uint>> _preLimits;
        public List<Tuple<TalentEntry, uint>> PreLimits {
            get {
                if (_preLimits == null) {
                    _preLimits = new List<Tuple<TalentEntry, uint>>();
                    if (csv.pre_skill != null) {
                        for (int i = 0, length = csv.pre_skill.Count; i < length; ++i) {
                            TalentEntry entry = Sys_Talent.Instance.schemes[schemeIndex].GetTalent(csv.pre_skill[i][0]);
                            if(entry != null) {
                                Tuple<TalentEntry, uint> entryLevel = new Tuple<TalentEntry, uint>(entry, csv.pre_skill[i][1]);
                                _preLimits.Add(entryLevel);
                            }
                        }
                    }
                }
                return _preLimits;
            }
        }

        public TalentEntry(uint id) : this(CSVTalent.Instance.GetConfData(id)) { }
        public TalentEntry(uint id, CSVTalent.Data csv) : this(csv) { }
        public TalentEntry(CSVTalent.Data csv) {
            this.id = csv.id;
            this.csv = csv;
        }

        // 刷新server数据
        public TalentEntry Refresh(uint level) {
            this.level = level;
            return this;
        }
    }
}