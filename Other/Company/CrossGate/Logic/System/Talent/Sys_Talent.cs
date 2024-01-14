using Logic.Core;
using Lib.Core;
using System.Collections.Generic;
using Table;

namespace Logic {
    /// <summary>
    /// 总天赋点数：
    /// 1： 正在炼化 +
    /// 2： 未炼化 +
    /// 3： 炼化出来的
    ///     4：已经使用的 +
    ///     5：剩下可使用的
    /// </summary>
    public partial class Sys_Talent : SystemModuleBase<Sys_Talent> {
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        public enum EEvents {
            OnUpdateTalentLevel,
            OnResetTalentPoint,
            OnExchangeTalentPoint,
            OnTalentLimitChanged, // 天赋点上限变化
            OnCanRemainTalentChanged, // 剩余天赋点数变化
            OnLianghuaChanged, // 炼化变化
            ChangePlan,
        }
        
        public class Scheme {
            public string name;
            
            public Dictionary<uint, TalentEntry> talents = new Dictionary<uint, TalentEntry>();
            public SortedDictionary<uint, TalentBranch> branchs = new SortedDictionary<uint, TalentBranch>();
            
            public TalentEntry GetTalent(uint talentId) {
                talents.TryGetValue(talentId, out TalentEntry entry);
                return entry;
            }
            
            // 已经使用的天赋点
            public uint usedTalentPoint {
                get {
                    uint count = 0;
                    foreach (var kvp in branchs) {
                        count += kvp.Value.usedPoint;
                    }
                    return count;
                }
            }
            // 可使用的天赋点
            public long canUseTalentPoint { get; set; }

            public uint withoutLianhuaing {
                get {
                    if (canUseTalentPoint >= 0) {
                        return (uint)((int)usedTalentPoint + canUseTalentPoint);
                    }
                    else {
                        return (uint)((int)usedTalentPoint);
                    }
                }
            }
        }
        
        public int curSchemeIndex = 0;
        public List<Scheme> schemes = new List<Scheme>();

        public Scheme usingScheme {
            get {
                if (0 <= curSchemeIndex && curSchemeIndex < schemes.Count) {
                    return schemes[curSchemeIndex];
                }

                return null;
            }
        }

        public static readonly int UpgradeTalentLevelCostPoint = 1;

        #region 升级
        // 天赋点上限： 会随着升级变化 
        public uint currentLimitTalentPoint { get; private set; }

        #endregion

        #region 炼化
        // 正在炼化的点
        public int lianhuaingPoint {
            get {
                return lianhuaTimeStamps.Count;
            }
        }
        // 最后一个炼化的时间戳
        public uint LastLianhuaTimeStamp {
            get {
                if (lianhuaingPoint > 0) {
                    return lianhuaTimeStamps[lianhuaTimeStamps.Count - 1];
                }
                return 0u;
            }
        }
        public List<Timer> lianhuaTimers = new List<Timer>();
        public List<uint> lianhuaTimeStamps = new List<uint>();
        // 可能表格中找不到
        public uint nextLianhuaId {
            get {
                return (uint)usingScheme.withoutLianhuaing + (uint)lianhuaingPoint + 1;
            }
        }
        // 剩余可炼化点数
        public int remainCanLianhuaPoint {
            get {
                return (int)(currentLimitTalentPoint - usingScheme.usedTalentPoint - lianhuaingPoint);
            }
        }
        public ulong RemainLianhuaTotalTime {
            get {
                return LastLianhuaTimeStamp - Sys_Time.Instance.GetServerTime();
            }
        }
        #endregion

        public uint SelectedTalentId = 0;
        
        private void ClearTimer() {
            for (int i = 0, length = lianhuaTimers.Count; i < length; ++i) {
                lianhuaTimers[i]?.Cancel();
            }
            lianhuaTimers.Clear();
        }

        // 炼化红点
        public bool CanLianhua() {
            if (usingScheme == null) {
                return false;
            }

            bool canShowPoint = false;
            uint nextLianhuaId = Sys_Talent.Instance.nextLianhuaId;
            CSVTalentExchange.Data csv = CSVTalentExchange.Instance.GetConfData(nextLianhuaId);
            if (csv != null) {
                canShowPoint = Sys_Role.Instance.Role.Level >= csv.level;
                canShowPoint &= Sys_Talent.Instance.lianhuaingPoint < this.MaxLianHua;
                if (!canShowPoint) {
                    return canShowPoint;
                }

                canShowPoint = (Sys_Bag.Instance.GetItemCount(2) >= csv.gold);
                if (!canShowPoint) {
                    return canShowPoint;
                }

                canShowPoint = (Sys_Bag.Instance.GetItemCount(csv.item[0]) >= csv.item[1]);
                if (!canShowPoint) {
                    return canShowPoint;
                }

                canShowPoint = (Sys_Talent.Instance.currentLimitTalentPoint - Sys_Talent.Instance.lianhuaingPoint - usingScheme.usedTalentPoint - usingScheme.canUseTalentPoint > 0);
                if (!canShowPoint) {
                    return canShowPoint;
                }
            }

            return canShowPoint;
        }
    }
}
