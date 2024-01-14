using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// 条件:家族兽当前阶段是否拥有
    /// </summary>
    public class FamilyTypeCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.FamilyType;
            }
        }

        uint type;

        public override void DeserializeObject(List<int> data)
        {
            type = (uint)data[0];
        }

        public override bool IsValid()
        {
            if (Sys_Family.Instance.IsHasCreatureState(type))
            {
                return true;
            }
            return false;
        }

        protected override void OnDispose()
        {
            type = 0;
        }
    }

    /// <summary>
    /// 条件:家族兽是否拥有
    /// </summary>
    public class FamilyStageCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.FamilyStage;
            }
        }

        uint stage;

        public override void DeserializeObject(List<int> data)
        {
            stage = (uint)data[0];
        }

        public override bool IsValid()
        {
            if (Sys_Family.Instance.IsHasCreatureState(stage))
            {
                return true;
            }
            return false;
        }

        protected override void OnDispose()
        {
            stage = 0;
        }
    }

    /// <summary>
    /// 条件:家族兽是否是训练中的
    /// </summary>
    public class FamilyTrainCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.FamilyTrainCreature;
            }
        }

        uint id;

        public override void DeserializeObject(List<int> data)
        {
            id = (uint)data[0];
        }

        public override bool IsValid()
        {
            if (Sys_Family.Instance.IsCurrentTrainCreature(id))
            {
                return true;
            }
            return false;
        }

        protected override void OnDispose()
        {
            id = 0;
        }
    }

    /// <summary>
    /// 条件:家族兽是否是训练时间中
    /// </summary>
    public class FamilyTrainTimeCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.FamilyTrainOpenTime;
            }
        }


        public override void DeserializeObject(List<int> data)
        {
        }

        public override bool IsValid()
        {
            if (Sys_Family.Instance.ShowTrainInfo())
            {
                return true;
            }
            return false;
        }

        protected override void OnDispose()
        {
        }
    }

    /// <summary>
    /// 条件:家族建设繁荣度等级大于等于
    /// </summary>
    public class GreaterFamilyConstructLevel : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.GreaterFamilyConstructLevel;
            }
        }

        uint level;
        public override void DeserializeObject(List<int> data)
        {
            level = (uint)data[0];
        }

        public override bool IsValid()
        {
            return Sys_Family.Instance.familyData.GetConstructLevel() >= level;
        }

        protected override void OnDispose()
        {
        }
    }

    /// <summary>
    /// 条件:家族建设繁荣度等级小于
    /// </summary>
    public class LessFamilyConstructLevel : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.LessFamilyConstructLevel;
            }
        }

        uint level;
        public override void DeserializeObject(List<int> data)
        {
            level = (uint)data[0];
        }

        public override bool IsValid()
        {
            return Sys_Family.Instance.familyData.GetConstructLevel() < level;
        }

        protected override void OnDispose()
        {
        }
    }

    /// <summary>
    /// 条件:家族建设繁荣度经验是否已满
    /// </summary>
    public class FamilyCOnstructExpMax : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.FamilyCOnstructExpMax;
            }
        }
        uint id;
        uint isMax;
        public override void DeserializeObject(List<int> data)
        {
            id = (uint)data[0];
            isMax = (uint)data[1];
        }

        public override bool IsValid()
        {
            bool _isMax = Sys_Family.Instance.familyData.GetCurrentConstructExpIsMax((EConstructs)id);
            return (_isMax ? 0u : 1u) == isMax;
        }

        protected override void OnDispose()
        {
        }
    }

    /// <summary>
    /// 条件:家族建筑等级大于等于
    /// </summary>
    public class GreaterFamilyBuildLevel : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.GreaterFamilyBuildLevel;
            }
        }
        uint index;
        uint level;
        public override void DeserializeObject(List<int> data)
        {
            index = (uint)data[0];
            level = (uint)data[1];
        }

        public override bool IsValid()
        {
            uint _level = Sys_Family.Instance.familyData.GetLevelByBuildIndex(index);
            return _level >= level;
        }

        protected override void OnDispose()
        {
        }
    }

    /// <summary>
    /// 条件:家族建筑等级小于
    /// </summary>
    public class LessFamilyMainBuildLevel : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.LessFamilyMainBuildLevel;
            }
        }
        uint index;
        uint level;
        public override void DeserializeObject(List<int> data)
        {
            index = (uint)data[0];
            level = (uint)data[1];
        }

        public override bool IsValid()
        {
            uint _level = Sys_Family.Instance.familyData.GetLevelByBuildIndex(index);
            return _level < level;
        }

        protected override void OnDispose()
        {
        }
    }
}

