using System;
using System.Collections.Generic;
using Logic.Core;
using Table;
using UnityEngine;
using static Logic.Sys_Map;

namespace Logic {
    /// <summary>
    /// 想设计为：对话，或者其他外部关联任务的系统回调任务的接口
    /// 比如：对话按钮点击之后执行的任务回调
    /// </summary>
    public partial class Sys_Task : SystemModuleBase<Sys_Task>, ISystemModuleUpdate {
        // 地图挂载的所有任务
        private Dictionary<uint, List<CSVTask.Data>> _map2Tasks = null;
        public Dictionary<uint, List<CSVTask.Data>> map2Tasks {
            get {
                if (this._map2Tasks == null) {
                    this._map2Tasks = new Dictionary<uint, List<CSVTask.Data>>();
                    this.PopulateNpsTasks();
                }

                return this._map2Tasks;
            }
        }

        public void PopulateNpsTasks() {
            var csvDict = CSVTask.Instance.GetAll();
            foreach (var kvp in csvDict) {
                this.GetByMapId(kvp.taskMap, kvp);
                if (kvp.taskTriggerMap != null) {
                    for (int i = 0, length = kvp.taskTriggerMap.Count; i < length; ++i) {
                        this.GetByMapId(kvp.taskTriggerMap[i], kvp);
                    }
                }
                if (kvp.taskTraceMap != null) {
                    for (int i = 0, length = kvp.taskTraceMap.Count; i < length; ++i) {
                        this.GetByMapId(kvp.taskTraceMap[i], kvp);
                    }
                }
            }
        }
        private void GetByMapId(uint mapId, CSVTask.Data csv) {
            List<CSVTask.Data>  ls;
            if (!this.map2Tasks.TryGetValue(mapId, out ls)) {
                ls = new List<CSVTask.Data>();
                this.map2Tasks.Add(mapId, ls);
            }
            if (!ls.Contains(csv)) {
                ls.Add(csv);
            }
        }

        public List<CSVTask.Data>  GetTasksByMapId(uint mapId, Func<CSVTask.Data, bool> func = null, bool includeInactive = false) {
            List<CSVTask.Data>  tmpMapTasks = new List<CSVTask.Data>();
            if (this.map2Tasks.TryGetValue(mapId, out var mapTasks)) {
                if (!includeInactive) {
                    for (int i = mapTasks.Count - 1; i >= 0; i--) {
                        TaskTab tab = this.GetTab(mapTasks[i].taskCategory);
                        if (Sys_FunctionOpen.Instance.IsOpen(tab.funcOpenId, false)) {
                            tmpMapTasks.Add(mapTasks[i]);
                        }
                    }
                }
                if (func != null) {
                    tmpMapTasks = tmpMapTasks.FindAll((e) => { return func.Invoke(e); });
                }
            }

            return tmpMapTasks;
        }

        public List<CSVTask.Data>  GetTasksByMapId(uint mapId, ETaskCategory taskCategory, Func<CSVTask.Data, bool> func = null) {
            List<CSVTask.Data>  tmpMapTasks = new List<CSVTask.Data>();
            if (this.map2Tasks.TryGetValue(mapId, out var mapTasks)) {
                tmpMapTasks = mapTasks.FindAll((e) => { return (e.taskCategory == (int)taskCategory) && func.Invoke(e); });
            }
            return tmpMapTasks;
        }
        public List<CSVTask.Data>  GetCanReceivedTasksByMapId(uint mapId) {
            List<CSVTask.Data>  tmpMapTasks = this.GetTasksByMapId(mapId, (csvTask) => {
                return TaskHelper.CheckForCanReceive(csvTask);
            });
            return tmpMapTasks;
        }
        public List<CSVTask.Data>  GetCanReceivedTasksByMapId(uint mapId, ETaskCategory taskCategory) {
            List<CSVTask.Data>  tmpMapTasks = this.GetTasksByMapId(mapId, taskCategory, (csvTask) => {
                return TaskHelper.CheckForCanReceive(csvTask);
            });
            return tmpMapTasks;
        }
        
        public List<CSVTask.Data>  GetReceivedTasksByMapId(uint mapId, ETaskCategory taskCategory) {
            List<CSVTask.Data>  tmpMapTasks = this.GetTasksByMapId(mapId, taskCategory, (csvTask) => {
                return TaskHelper.HasReceived(csvTask.id);
            });
            return tmpMapTasks;
        }

        // true为激活
        public List<Tuple<uint, bool>> GetNpcsByMapId(List<uint> mapIds, ETaskCategory taskCategory) {
            List<Tuple<uint, bool>> ls = new List<Tuple<uint, bool>>();
            for (int i = 0, length = mapIds.Count; i < length; ++i) {
                ls.AddRange(this.GetNpcsByMapId(mapIds[i], taskCategory));
            }
            return ls;
        }
        public List<Tuple<uint, bool>> GetNpcsByMapId(uint mapId, ETaskCategory taskCategory) {
            List<Tuple<uint, bool>> ls = new List<Tuple<uint, bool>>();
            List<CSVTask.Data>  tmpMapTasks = this.GetTasksByMapId(mapId, taskCategory);
            for (int i = 0, length = tmpMapTasks.Count; i < length; ++i) {
                ls.Add(new Tuple<uint, bool>(tmpMapTasks[i].id, TaskHelper.HasSubmited(tmpMapTasks[i].id)));
            }
            return ls;
        }

        // 返回任务完成，准备提交的一系列action组合
        public List<ActionBase> GetTaskSubmitActionList(TaskEntry taskEntry) {
            List<ActionBase> actionBases = new List<ActionBase>();
            if (taskEntry != null) {
                uint submitNpcId = taskEntry.csvTask.submitNpc;
                if (submitNpcId != 0) {
                    if (GameCenter.uniqueNpcs.ContainsKey(submitNpcId)) {
                        InteractiveWithNPCAction interactiveWithNPC = ActionCtrl.Instance.CreateAction(typeof(InteractiveWithNPCAction)) as InteractiveWithNPCAction;
                        if (interactiveWithNPC != null) {
                            interactiveWithNPC.npc = GameCenter.uniqueNpcs[submitNpcId];
                            interactiveWithNPC.currentTaskEntry = taskEntry;

                            actionBases.Add(interactiveWithNPC);
                        }
                    }
                }
                else {
                    Task_SubmitAction submitAction = ActionCtrl.Instance.CreateAction(typeof(Task_SubmitAction)) as Task_SubmitAction;
                    if (submitAction != null) {
                        submitAction.taskEntry = taskEntry;

                        actionBases.Add(submitAction);
                    }
                }
            }

            return actionBases;
        }
    }

    public static class TaskHelper {
        /// <summary>
        /// 能够接任务?
        /// </summary>
        /// <param name="taskData"></param>
        /// <returns></returns>
        public static bool CanReceive(CSVTask.Data taskData) {
            bool result = true;
            if (taskData != null) {
                if (HasReceived(taskData.id)) { return false; }
                if (HasSubmited(taskData.id)) { return false; }

                result = CheckForCanReceive(taskData);
                if (!result) { return false; }
            }
            return result;
        }
        // 是否表格條件滿足
        public static bool IsMatchCsvCondition(ETaskState taskState, CSVTask.Data taskData) {
            bool result = true;
            if (taskState == ETaskState.Finished) { result = true; }
            else if (taskState == ETaskState.UnFinished) { result = true; }
            else if (taskState == ETaskState.UnReceived) { result = CheckForCanReceive(taskData); }
            return result;
        }
        public static bool CheckForCanReceive(CSVTask.Data taskData) {
            bool result = true;
            if (taskData != null) {
                // 等级判断
                if (taskData.taskLvLowerLimit != 0 || taskData.taskLvUpperLimit != 0) {
                    int level = (int)Sys_Role.Instance.Role.Level;
                    if (taskData.taskLvLowerLimit != 0) {
                        result &= (taskData.taskLvLowerLimit <= level);
                    }
                    if (taskData.taskLvUpperLimit != 0) {
                        result &= (level <= taskData.taskLvUpperLimit);
                    }
                }

                if (!result) { return false; }

                if (taskData.LifeSkillLv != 0) {
                    int level = (int)Sys_LivingSkill.Instance.GetMaxSkillLifeLevel();
                    result &= (taskData.LifeSkillLv >= level);
                }

                if (!result) { return false; }

                // 其他判断
                // 前置判断
                if (taskData.preposeTask != null) {
                    for (int i = 0, length = taskData.preposeTask.Count; i < length; ++i) {
                        if (!HasSubmited(taskData.preposeTask[i])) {
                            result = false;
                            break;
                        }
                    }
                }
                if (!result) { return false; }

                if (GameCenter.mainHero != null && GameCenter.mainHero.careerComponent != null) {
                    // 职业判断
                    ECareerType careerType = GameCenter.mainHero.careerComponent.CurCarrerType;
                    if (taskData.occupationLimit != 0 && careerType != (ECareerType)taskData.occupationLimit) { result = false; }
                    if (!result) { return false; }
                }

                // 是否调查过
                if (taskData.InvestigateCondition != 0) {
                    bool isInquiryed = Sys_Inquiry.Instance.IsInquiryed(taskData.InvestigateCondition);
                    if (!isInquiryed) {
                        return false;
                    }
                }

                TaskTab tab = Sys_Task.Instance.GetTab(taskData.taskCategory);
                if (tab != null) {
                    if (!tab.IsOpen()) {
                        return false;
                    }
                }
                else {
                    return false;
                }

                // 冒险等级
                if (taskData.taskAdventureLv != 0) {
                    int level = (int)Sys_ClueTask.Instance.adventureLevel;
                    result &= level >= taskData.taskAdventureLv;
                }
                if (!result) { return false; }

                // 侦探等级
                if (taskData.taskDetectiveLv != 0) {
                    int level = (int)Sys_ClueTask.Instance.detectiveLevel;
                    result &= level >= taskData.taskDetectiveLv;
                }
                if (!result) { return false; }
            }
            return result;
        }
        public static bool HasReceived(uint id) {
            TaskEntry taskEntry = Sys_Task.Instance.GetReceivedTask(id);
            return taskEntry != null;
        }
        public static bool HasTracked(uint id) {
            TaskEntry taskEntry = Sys_Task.Instance.trackedTaskList.Find((e) => { return e.id == id; });
            return taskEntry != null;
        }
        public static bool HasFinished(uint id) {
            ETaskState status = Sys_Task.Instance.GetTaskState(id);
            return status == ETaskState.Finished;
        }
        public static bool HasSubmited(uint id) {
            bool result = false;
            for (int i = 0, length = Sys_Task.Instance.finishedTasks.Count; i < length; ++i) {
                if (Sys_Task.Instance.finishedTasks[i] == id) {
                    return true;
                }
            }
            return result;
        }
        public static bool InCurrentMapArea(TaskEntry taskEntry, CSVTask.Data taskdata) {
            bool inArea = Sys_Npc.Instance.IsInNpcArea(Sys_Map.Instance.CurMapId, taskdata.receiveNpc, GameCenter.mainHero.transform);
            return inArea;
        }

        // 给地图盛继刚的接口
        public static CSVTask.Data GetTaskDataByNPCId(uint npcId) {
            CSVTask.Data ret = null;
            CSVNpc.Data csvNpcData = CSVNpc.Instance.GetConfData(npcId);

            if (csvNpcData == null)
                return ret;

            List<FunctionBase> filteredFunctions;
            NPCFunctionComponent.FilterFunctions(csvNpcData, out filteredFunctions);
            if (filteredFunctions != null) {
                for (int index = 0, len = filteredFunctions.Count; index < len; index++) {
                    if (filteredFunctions[index].Type == EFunctionType.Task) {
                        ret = CSVTask.Instance.GetConfData(filteredFunctions[index].ID);
                        break;
                    }
                }
            }

            return ret;
        }

        public static void GetNpcRegion(uint npcId, List<int> region) {
            region.Clear();

            var csvNpc = CSVNpc.Instance.GetConfData(npcId);
            if (csvNpc != null) {
                var csvMap = CSVMapInfo.Instance.GetConfData(csvNpc.mapId);
                if (csvMap != null && csvMap.map_node != null) {
                    for (int i = 1, length = Math.Min(csvMap.map_node.Count, 4); i < length; ++i) {
                        region.Add(csvMap.map_node[i]);
                    }
                }
            }
        }
        public static Dictionary<int, List<uint>> GetNpcsByTrackedTask() {
            Dictionary<int, List<uint>> dict = new Dictionary<int, List<uint>>();
            if (Sys_Ini.Instance.Get<IniElement_IntArray>(1045, out IniElement_IntArray taskCatList)) {
                for (int i = 0, length = Sys_Task.Instance.trackedTaskList.Count; i < length; ++i) {
                    var taskEntry = Sys_Task.Instance.trackedTaskList[i];
                    int taskCategory = taskEntry.csvTask.taskCategory;
                    if (!taskEntry.IsFinish()) {
                        int index = Array.IndexOf<int>(taskCatList.value, taskEntry.csvTask.taskCategory);
                        if (index != -1) {
                            if (!dict.TryGetValue(taskCategory, out List<uint> ls)) {
                                ls = new List<uint>();
                                dict.Add(taskCategory, ls);
                            }

                            for (int j = 0, lengthJ = taskEntry.taskGoals.Count; j < lengthJ; ++j) {
                                if (j > 0) {
                                    break;
                                }
                                var npcId = taskEntry.taskGoals[j].csv.PathfindingTargetID;
                                string reason = null;
                                if (taskEntry.CanDoOnlyCSVCondition(ref reason)) {
                                    if (npcId != 0 && !ls.Contains(npcId)) {
                                        ls.Add(npcId);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return dict;
        }

        public static List<MapNpc> GetMapNpcsInMap(Dictionary<uint, MapNpc> mapNpcs, uint mapId) {
            List<MapNpc> ls = new List<MapNpc>();
            foreach (var kvp in mapNpcs) {
                if (kvp.Value.csvMap.id == mapId) {
                    ls.Add(kvp.Value);
                }
            }

            return ls;
        }

        public class MapNpc {
            public uint npcId;
            public List<int> taskCatagaries = new List<int>();

            public CSVNpc.Data csvNpc {
                get {
                    return CSVNpc.Instance.GetConfData(this.npcId);
                }
            }
            public CSVMapInfo.Data csvMap {
                get {
                    return CSVMapInfo.Instance.GetConfData(this.csvNpc.mapId);
                }
            }

            public CSVEditorMap.Data csvEditorMap {
                get {
                    return CSVEditorMap.Instance.GetConfData(this.csvNpc.mapId);
                }
            }

            public bool IsInMap(uint mapId) {
                return this.csvMap.map_node.IndexOf((int)mapId) != -1;
            }

            public bool TryGetPos(uint map, out Vector3 pos, out bool inThisMap) {
                pos = default;
                inThisMap = false;
                if (this.csvMap != null) {
                    if (this.csvMap.map_node != null) {
                        int index = this.csvMap.map_node.IndexOf((int)map);
                        if (index == -1) {
                            return false;
                        }
                        else {
                            if (map == this.csvMap.id) {
                                inThisMap = true;

                                // 就在当前地图
                                //GameCenter.FindNearestNpc(this.npcId, out Npc npc, out ulong guid);
                                //if (npc != null && npc.gameObject != null) {
                                //    pos = npc.gameObject.transform.position;
                                //}

                                Quaternion eular = default;
                                Sys_Map.Instance.GetNpcPos(this.csvMap.id, this.npcId, ref pos, ref eular);
                            }
                            else if (index < this.csvMap.map_node.Count - 1) {
                                inThisMap = false;

                                // 查找传送点
                                uint nextMapId = (uint)this.csvMap.map_node[index + 1];
                                TelData telData = Sys_Map.Instance.GetTelData(map, nextMapId);
                                if (telData != null) {
                                    pos.x = telData.pos.X;
                                    pos.y = telData.pos.Y;
                                }
                            }
                        }
                    }
                }
                return true;
            }

            public MapNpc(uint npcId) {
                this.npcId = npcId;
            }
        }

        public static Dictionary<uint, MapNpc> GetTaskMapNpcs(Dictionary<int, List<uint>> dict) {
            Dictionary<uint, MapNpc> npcs = new Dictionary<uint, MapNpc>();
            foreach (var kvp in dict) {
                for (int i = 0, length = kvp.Value.Count; i < length; ++i) {
                    uint npcId = kvp.Value[i];
                    if (!npcs.TryGetValue(kvp.Value[i], out MapNpc npc)) {
                        npc = new MapNpc(npcId);
                        npcs.Add(npcId, npc);
                    }
                    npc.taskCatagaries.Add(kvp.Key);
                }
            }
            return npcs;
        }
        public static List<MapNpc> GetMapNpcs(Dictionary<uint, MapNpc> dict, uint mapId) {
            List<MapNpc> ls = new List<MapNpc>();
            foreach (var kvp in dict) {
                if (kvp.Value.IsInMap(mapId)) {
                    ls.Add(kvp.Value);
                }
            }
            return ls;
        }

        public static void IsNpcInThisMap(Dictionary<int, List<uint>> npcs, EMapType mapType, uint id, ref List<int> taskCatagaries) {
            taskCatagaries.Clear();
            foreach (var kvp in npcs) {
                for (int i = 0, length = kvp.Value.Count; i < length; ++i) {
                    var csvNpc = CSVNpc.Instance.GetConfData(kvp.Value[i]);
                    if (csvNpc != null) {
                        var csvMap = CSVMapInfo.Instance.GetConfData(csvNpc.mapId);
                        if (csvMap != null && csvMap.map_node != null) {
                            if (mapType == EMapType.Island) {
                                if (csvMap.map_node.Count >= 2 && csvMap.map_node[1] == id && !taskCatagaries.Contains(kvp.Key)) {
                                    taskCatagaries.Add(kvp.Key);
                                }
                            }
                            else if (mapType == EMapType.Map) {
                                if (csvMap.map_node.Count >= 3 && csvMap.map_node[2] == id && !taskCatagaries.Contains(kvp.Key)) {
                                    taskCatagaries.Add(kvp.Key);
                                }
                            }
                            else if (mapType == EMapType.Maze) {
                                if (csvMap.map_node.Count >= 4 && csvMap.map_node[3] == id && !taskCatagaries.Contains(kvp.Key)) {
                                    taskCatagaries.Add(kvp.Key);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
