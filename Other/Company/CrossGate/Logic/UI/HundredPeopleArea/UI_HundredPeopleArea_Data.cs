using System.Collections.Generic;
using Framework;
using Logic;
using Table;

/// <summary> 百人道场界面数据 </summary>
public class UI_HundredPeopleArea_Data {
    public enum EStatus {
        OverAll,   // 全部通关
        Locked,   // 未解锁
        SomeUnlock,    // 解锁部分挂卡
    }

    #region 数据定义
    /// <summary> 所有关卡 </summary>
    public Dictionary<int, List<uint>> dict_AllStage = new Dictionary<int, List<uint>>();
    /// <summary> 选中关卡下标 </summary>
    public int selectStageIndex = 0;
    /// <summary> 选中菜单下标 </summary>
    public int selectSubIndex = 0;
    /// <summary> 当前关卡列表 </summary>
    public List<int> list_StageID = new List<int>();
    /// <summary> 当前菜单列表 </summary>
    public List<uint> list_Sub = new List<uint>();
    public Dictionary<int, Dictionary<int, CSVInstanceDaily.Data>> map = new Dictionary<int, Dictionary<int, CSVInstanceDaily.Data>>();

    /// <summary> 当前关卡Id </summary>
    public int curStageID {
        get {
            return this.GetStageID(this.selectStageIndex);
        }
    }
    /// <summary> 当前菜单Id </summary>
    public uint curSubId {
        get {
            return this.GetSubID(this.selectSubIndex);
        }
    }
    #endregion

    #region 功能接口
    /// <summary> 重置 </summary>
    public void Reset() {
        this.selectStageIndex = 0;
        this.selectSubIndex = 0;
        this.list_StageID.Clear();
        this.list_Sub.Clear();
        this.map.Clear();
    }
    // 当前应该挑战的关卡
    public uint unfinishedFirstStageId = 0;

    public void LoadData() {
        this.dict_AllStage.Clear();
        var dict = CSVInstanceDaily.Instance.GetAll();
        foreach (var kvp in dict) {
            if (kvp.InstanceId != Sys_HundredPeopleArea.Instance.activityid) {
                continue;
            }

            if (!this.dict_AllStage.TryGetValue((int)kvp.LayerStage, out List<uint> ls)) {
                ls = new List<uint>();
                this.dict_AllStage.Add((int)kvp.LayerStage, ls);
            }
            this.dict_AllStage[(int)kvp.LayerStage].Add(kvp.id);

            if (!this.map.TryGetValue((int)kvp.LayerStage, out Dictionary<int, CSVInstanceDaily.Data> dt)) {
                dt = new Dictionary<int, CSVInstanceDaily.Data>();
                this.map.Add((int)kvp.LayerStage, dt);
            }
            dt.Add((int)kvp.Layerlevel, kvp);
        }

        bool found = false;
        foreach (var kvp in this.map) {
            if (!found) {
                foreach (var innerKVP in kvp.Value) {
                    if (innerKVP.Value.id > Sys_HundredPeopleArea.Instance.passedInstanceId) {
                        this.unfinishedFirstStageId = innerKVP.Value.id;
                        found = true;
                        break;
                    }
                }
            }
        }
        if (!found) {
            this.unfinishedFirstStageId = 0;
        }
    }

    public void UpdateStageIDList() {
        this.list_StageID.Clear();
        foreach (var kvp in this.dict_AllStage) {
            this.list_StageID.Add(kvp.Key);
        }
    }

    public void UpdateSubList() {
        this.list_Sub.Clear();
        if (this.dict_AllStage.TryGetValue(this.curStageID, out List<uint> list)) {
            this.list_Sub.AddRange(list);
        }
    }

    public void SetSelectStageIndex(int index) {
        this.selectStageIndex = index;
        this.UpdateSubList();
    }
    public void SetSelectSubIndex(int index) {
        this.selectSubIndex = index;
    }

    public int GetStageID(int index) {
        if (index < 0 || index >= this.list_StageID.Count)
            return 0;

        return this.list_StageID[index];
    }

    public uint GetSubID(int index) {
        if (index < 0 || index >= this.list_Sub.Count)
            return 0;

        return this.list_Sub[index];
    }

    public CSVInstanceDaily.Data GetStageCofig(int index) {
        uint id = this.GetSubID(index);
        return CSVInstanceDaily.Instance.GetConfData(id);
    }

    // 该挂卡全部layer是否通关
    // 该stage的最后一个id比较即可
    public bool IsAllLayerOver(int stageLevel, uint passedInstanceId) {
        if (stageLevel <= 0) {
            return true;
        }
        else if (stageLevel >= this.dict_AllStage.Count) {
            return false;
        }

        var subIds = this.dict_AllStage[stageLevel];
        return subIds[subIds.Count - 1] <= passedInstanceId;
    }
    // 某一layer是否通关
    public bool IsOver(int stageLevel, int layerLevel, uint passedInstanceId) {
        if (stageLevel <= 0) {
            return true;
        }
        //else if (stageLevel >= this.dict_AllStage.Count) {
        //    return false;
        //}
        if (layerLevel <= 0) {
            return true;
        }

        var subIds = this.dict_AllStage[stageLevel];
        return subIds[layerLevel - 1] <= passedInstanceId;
    }
    // 某一layer是否解锁
    public bool IsLock(int stageLevel, int layerLevel) {
        if (stageLevel <= 0) {
            return true;
        }
        else if (stageLevel >= this.dict_AllStage.Count) {
            return false;
        }

        var subIds = this.dict_AllStage[stageLevel];
        return CSVInstanceDaily.Instance.GetConfData(subIds[layerLevel - 1]).LevelLimited > Sys_Role.Instance.Role.Level;
    }
    public bool IsPreStageLayerOver(int stageLevel, int layerLevel, uint passedInstanceId) {
        if (layerLevel <= 1) {
            layerLevel = this.map[stageLevel].Count;
            --stageLevel;
        }
        else {
            --layerLevel;
        }

        return this.IsOver(stageLevel, layerLevel, passedInstanceId);
    }

    // 该stage是否封锁
    public bool IsUnlock(int stageLevel, uint passedInstanceId) {
        var subIds = this.dict_AllStage[stageLevel];
        CSVInstanceDaily.Data csv = CSVInstanceDaily.Instance.GetConfData(subIds[0]);
        return Sys_Role.Instance.Role.Level >= csv.LevelLimited && this.IsAllLayerOver(stageLevel - 1, passedInstanceId);
    }
    public enum EStageLockReason {
        ConditionNotValid, // 条件不满足
        PreNotOver, // 上一关未完成
        AllOver,  // 全部完成
        AnyFinish, // 完成一部分
    }

    // 该stage解锁到的instanceId
    public int UnlockLayerLevelId(int stageLevel, uint passedInstanceId, out EStageLockReason reason) {
        reason = EStageLockReason.PreNotOver;
        CSVInstanceDaily.Data csv = this.map[stageLevel][1];
        bool valid = csv.LevelLimited <= Sys_Role.Instance.Role.Level;
        if (!valid) {
            reason = EStageLockReason.ConditionNotValid;
            return 0;
        }

        foreach (var kvp in this.map[stageLevel]) {
            if (kvp.Key <= 1) {
                valid &= this.IsAllLayerOver(stageLevel - 1, passedInstanceId);
                if (!valid) {
                    reason = EStageLockReason.PreNotOver;
                    return 0;
                }
            }

            valid &= this.IsOver(stageLevel, kvp.Key, passedInstanceId);
            if (!valid) {
                reason = EStageLockReason.AnyFinish;
                return (int)kvp.Value.id;
            }
        }

        reason = EStageLockReason.AllOver;
        // 全部解锁
        return 0;
    }
    #endregion
}
