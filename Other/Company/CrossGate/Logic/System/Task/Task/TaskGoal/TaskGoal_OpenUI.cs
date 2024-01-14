using Logic.Core;
using Table;

namespace Logic {
    public class TaskGoal_NavToNpcInteractive : TaskGoal {
        public uint npcID = 0;

        public override TaskGoal Init(TaskEntry taskEntry, CSVTaskGoal.Data csv, int goalIndex) {
            this.limit = 1;
            this.npcID = csv.PathfindingTargetID;
            return base.Init(taskEntry, csv, goalIndex);
        }

        public override void Refresh(uint crt) {
            this.current = (int)crt;
        }

        public override string GetTaskContent() {
            int finalIndex = this.taskEntry.csvTask.conditionType ? 0 : this.goalIndex;
            return LanguageHelper.GetTaskTextContent(this.taskEntry.csvTask.taskContent[finalIndex]);
        }

        protected override void DoExec(bool auto = true) {
            if (this.csv.PathfindingType == 1) {
                this.NavToNpc(this.csv.PathfindingTargetID, auto);
            }
            else {
                this.PartrolNpc(this.csv.PathfindingTargetID, this.csv.PathfindingMap, auto);
            }
        }
    }

    public class TaskGoal_OpenUI29 : TaskGoal_NavToNpcInteractive {
    }
    public class TaskGoal_OpenUI30 : TaskGoal_NavToNpcInteractive {
    }
    public class TaskGoal_OpenUI31 : TaskGoal_NavToNpcInteractive {
    }
    public class TaskGoal_OpenUI32 : TaskGoal_NavToNpcInteractive {
    }
    public class TaskGoal_OpenUI33 : TaskGoal_NavToNpcInteractive {
    }
    public class TaskGoal_OpenUI34 : TaskGoal_NavToNpcInteractive {
        public override TaskGoal Init(TaskEntry taskEntry, CSVTaskGoal.Data csv, int goalIndex) {
            this.limit = (int)csv.TargetParameter2;
            return this.InitTaskEntry(taskEntry, csv, goalIndex);
        }
    }
    public class TaskGoal_OpenUI35 : TaskGoal_NavToNpcInteractive {
    }
    public class TaskGoal_OpenUI36 : TaskGoal_NavToNpcInteractive {
    }
    public class TaskGoal_OpenUI37 : TaskGoal_NavToNpcInteractive {
    }
    public class TaskGoal_OpenUI38 : TaskGoal_NavToNpcInteractive {
    }
    public class TaskGoal_OpenUI39 : TaskGoal_NavToNpcInteractive {
    }
    public class TaskGoal_OpenUI40 : TaskGoal_NavToNpcInteractive {
    }
    public class TaskGoal_OpenUI41 : TaskGoal_NavToNpcInteractive {
        public override TaskGoal Init(TaskEntry taskEntry, CSVTaskGoal.Data csv, int goalIndex) {
            this.limit = (int)csv.TargetParameter1;
            return this.InitTaskEntry(taskEntry, csv, goalIndex);
        }
    }
    public class TaskGoal_OpenUI42 : TaskGoal_NavToNpcInteractive {
    }
    public class TaskGoal_OpenUI43 : TaskGoal_NavToNpcInteractive {
    }
    public class TaskGoal_OpenUI44 : TaskGoal_NavToNpcInteractive {
    }
    public class TaskGoal_OpenUI45 : TaskGoal_NavToNpcInteractive {
    }
    public class TaskGoal_OpenUI46 : TaskGoal_NavToNpcInteractive {
    }
    public class TaskGoal_OpenUI47 : TaskGoal_NavToNpcInteractive {
    }
    public class TaskGoal_OpenUI52 : TaskGoal_NavToNpcInteractive {
    }

    public class TaskGoal_ReachAwakenLevel : TaskGoal_NavToNpcInteractive {
    }
    public class TaskGoal_PetPropertyLevel : TaskGoal_NavToNpcInteractive {
    }
    public class TaskGoal_JionFamily : TaskGoal_NavToNpcInteractive {
    }
    public class TaskGoal_ConsuseFamilyCoin : TaskGoal_NavToNpcInteractive {
        public override TaskGoal Init(TaskEntry taskEntry, CSVTaskGoal.Data csv, int goalIndex) {
            this.limit = (int)csv.TargetParameter1;
            return this.InitTaskEntry(taskEntry, csv, goalIndex);
        }

        protected override void DoExec(bool auto = true) {
            if (!Sys_FunctionOpen.Instance.IsOpen(30401, true)) return;

            UI_FamilyOpenParam openParam = new UI_FamilyOpenParam() {
                familyMenuEnum = (int)UI_Family.EFamilyMenu.Activity, // 0 - 3 大厅 家族成员 家族建筑， 家族活动
                activeId = 1 // 家族活动表id 
            };

            Sys_Family.Instance.OpenUI_Family(openParam);

            //if (Sys_Family.Instance.familyData.isInFamily) {
            //    UIManager.OpenUI(EUIID.UI_Family, false, UI_Family.EFamilyMenu.Activity);
            //    UIManager.OpenUI(EUIID.UI_Family_Construct, false, UI_Family_Construct.EFamilyConstruct.Construct);
            //}
            //else {
            //    UIManager.OpenUI(EUIID.UI_ApplyFamily, false);
            //}
        }
    }
    public class TaskGoal_GotLilianPoint : TaskGoal_NavToNpcInteractive {
        public override TaskGoal Init(TaskEntry taskEntry, CSVTaskGoal.Data csv, int goalIndex) {
            this.limit = (int)csv.TargetParameter1;
            return this.InitTaskEntry(taskEntry, csv, goalIndex);
        }
    }
    public class TaskGoal_SubmitFamilyMenu : TaskGoal_NavToNpcInteractive {
        public override TaskGoal Init(TaskEntry taskEntry, CSVTaskGoal.Data csv, int goalIndex) {
            this.limit = (int)csv.TargetParameter1;
            return this.InitTaskEntry(taskEntry, csv, goalIndex);
        }

        protected override void DoExec(bool auto = true) {
            UI_FamilyOpenParam openParam = new UI_FamilyOpenParam() {
                familyMenuEnum = (int)UI_Family.EFamilyMenu.Activity, // 0 - 3 大厅 家族成员 家族建筑， 家族活动
                activeId = 10 // 家族活动表id 
            };

            Sys_Family.Instance.OpenUI_Family(openParam);
        }
    }
    public class TaskGoal_LiftSkillLevel : TaskGoal_NavToNpcInteractive {
    }
    public class TaskGoal_UploadMallItem : TaskGoal_NavToNpcInteractive {
    }
    public class TaskGoal_EquipBuilt : TaskGoal_NavToNpcInteractive {
    }
    public class TaskGoal_EquipTakeOn : TaskGoal_NavToNpcInteractive {
        public override TaskGoal Init(TaskEntry taskEntry, CSVTaskGoal.Data csv, int goalIndex) {
            this.limit = (int)csv.TargetParameter1;
            return this.InitTaskEntry(taskEntry, csv, goalIndex);
        }
    }
}