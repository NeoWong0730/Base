using System;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using Logic.Core;
using Table;
using Lib.Core;
using Packet;

namespace Logic
{
    /// CollectItemCounter = 7,    // 达成收集次数
    public class TaskGoal_CollectItemCounter : TaskGoal_CollectItem
    {
        public override TaskGoal Init(TaskEntry taskEntry, CSVTaskGoal.Data csv, int goalIndex)
        {
            this.limit = (int)csv.TargetParameter2;
            return this.InitTaskEntry(taskEntry, csv, goalIndex);
        }
    }
}