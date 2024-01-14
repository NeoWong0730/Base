using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// 条件功能类
    /// </summary>
    public class ConditionManager : Singleton<ConditionManager>
    {

        private static readonly Dictionary<EConditionType, ConditionBase> dict_Condition = new Dictionary<EConditionType, ConditionBase>()
        {
                {EConditionType.HaveTask,new HaveTaskCondition()},
                {EConditionType.Time,new TimeCondition()},
                {EConditionType.Weather,new WeatherCondition()},
                {EConditionType.Season,new SeasonCondition()},
                {EConditionType.HaveItem,new HaveItemCondition()},
                {EConditionType.TaskUnReceived,new TaskUnReceivedCondition()},
                {EConditionType.TaskUnCompleted,new TaskUnCompletedCondition()},
                {EConditionType.TaskCompleted,new TaskCompletedCondition()},
                {EConditionType.TaskSubmitted,new TaskSubmittedCondition()},
                {EConditionType.EqualCareer,new EqualCareerCondition()},
                {EConditionType.PassStage,new PassStageCondition()},
                {EConditionType.DialogueChoose,new DialogueChooseCondition()},
                {EConditionType.TaskTargetUnCompleted,new TaskTargetUnCompletedCondition()},
                {EConditionType.TaskTargetCompleted,new TaskTargetCompletedCondition()},
                {EConditionType.GreaterThanLv,new GreaterThanLvCondition()},
                {EConditionType.GreaterOrEqualLv,new GreaterOrEqualLvCondition()},
                {EConditionType.LessThanLv,new LessThanLvCondition()},
                {EConditionType.EqualLv,new EqualLvCondition()},
                {EConditionType.FunctionOpen,new FunctionOpenCondition()},
                {EConditionType.ActivedNPC,new ActivedNPCCondition()},
                {EConditionType.HasInquiryedFavorabilityNpc,new HasInquiryedFavorabilityNpcCondition()},
                {EConditionType.GodTest,new GodTestCondition()},
                {EConditionType.Secret,new SecretCondition()},
                {EConditionType.NoSecret,new NoSecretCondition()},
                {EConditionType.InTeam,new InTeamCondition()},
                {EConditionType.TimeLimitTaskGoalOn,new TimeLimitTaskGoalOnCondition()},
                {EConditionType.TimeLimitTaskGoalOff,new TimeLimitTaskGoalOffCondition()},
                {EConditionType.Escort,new EscortCondition()},
                {EConditionType.NpcFollow,new NpcFollowCondition()},
                {EConditionType.HavePartner,new HavePartnerCondition()},
                {EConditionType.LoginDay,new LoginDayCondition()},
                {EConditionType.HaveTreasure, new HaveTreasureCondition()},
                {EConditionType.FamilyBossOpen, new FamilyBossCondition()},
                {EConditionType.FamilyType, new FamilyTypeCondition()},
                {EConditionType.FamilyStage, new FamilyStageCondition()},
                {EConditionType.FamilyTrainOpenTime, new FamilyTrainTimeCondition()},
                {EConditionType.FamilyTrainCreature, new FamilyTrainCondition()},
                {EConditionType.AwakenImprint, new AwakenImprintCondition()},
                {EConditionType.TeamMemberCount, new TeamMemberCountCondition()},
                {EConditionType.GreaterFamilyConstructLevel, new GreaterFamilyConstructLevel()},
                {EConditionType.LessFamilyConstructLevel, new LessFamilyConstructLevel()},
                {EConditionType.FamilyCOnstructExpMax, new FamilyCOnstructExpMax()},
                {EConditionType.GreaterFamilyBuildLevel, new GreaterFamilyBuildLevel()},
                {EConditionType.LessFamilyMainBuildLevel, new LessFamilyMainBuildLevel()},
                {EConditionType.ActivityGMSetting, new ActivityGMSettingCondition()},
                {EConditionType.WorldLevel, new WorldLevelCondition()},
                {EConditionType.TownTaskAvaiable, new TownTaskAvaiableCondition()},
                {EConditionType.CheckTimeInYear, new CheckTimeInYearCondition()},
                {EConditionType.CheckTimeMonth, new CheckTimeInMonthCondition()},
                {EConditionType.CheckTimeOnDay, new CheckTimeOnDayCondition()},
                {EConditionType.CheckTimeOnWeek, new CheckTimeOnWeekCondition()},
                {EConditionType.CheckBossTowerState, new CheckBossTowerStateCondition()},
                {EConditionType.CheckBlessState, new CheckBlessStateCondition()},
        };

        private static List<int> _tempValues = new List<int>(4);
        public static bool IsValid(List<int> condition)
        {
            EConditionType eConditionType = (EConditionType)condition[0];
            ConditionBase conditionBase = null;
            dict_Condition.TryGetValue(eConditionType, out conditionBase);
            if (null == conditionBase) return true;

            _tempValues.Clear();
            _tempValues.AddRange(condition);
            _tempValues.RemoveAt(0);

            conditionBase.DeserializeObject(_tempValues);
            bool result = conditionBase.IsValid();
            return result;
        }

        public static ConditionBase CreateCondition(EConditionType type)
        {
            ConditionBase condition = null;

            switch (type)
            {
                case EConditionType.HaveTask:
                    condition = PoolManager.Fetch(typeof(HaveTaskCondition)) as ConditionBase;
                    break;
                case EConditionType.Time:
                    condition = PoolManager.Fetch(typeof(TimeCondition)) as ConditionBase;
                    break;
                case EConditionType.Weather:
                    condition = PoolManager.Fetch(typeof(WeatherCondition)) as ConditionBase;
                    break;
                case EConditionType.Season:
                    condition = PoolManager.Fetch(typeof(SeasonCondition)) as ConditionBase;
                    break;
                case EConditionType.HaveItem:
                    condition = PoolManager.Fetch(typeof(HaveItemCondition)) as ConditionBase;
                    break;
                case EConditionType.TaskUnReceived:
                    condition = PoolManager.Fetch(typeof(TaskUnReceivedCondition)) as ConditionBase;
                    break;
                case EConditionType.TaskUnCompleted:
                    condition = PoolManager.Fetch(typeof(TaskUnCompletedCondition)) as ConditionBase;
                    break;
                case EConditionType.TaskCompleted:
                    condition = PoolManager.Fetch(typeof(TaskCompletedCondition)) as ConditionBase;
                    break;
                case EConditionType.TaskSubmitted:
                    condition = PoolManager.Fetch(typeof(TaskSubmittedCondition)) as ConditionBase;
                    break;
                case EConditionType.EqualCareer:
                    condition = PoolManager.Fetch(typeof(EqualCareerCondition)) as ConditionBase;
                    break;
                case EConditionType.PassStage:
                    condition = PoolManager.Fetch(typeof(PassStageCondition)) as ConditionBase;
                    break;
                case EConditionType.DialogueChoose:
                    condition = PoolManager.Fetch(typeof(DialogueChooseCondition)) as ConditionBase;
                    break;
                case EConditionType.TaskTargetUnCompleted:
                    condition = PoolManager.Fetch(typeof(TaskTargetUnCompletedCondition)) as ConditionBase;
                    break;
                case EConditionType.TaskTargetCompleted:
                    condition = PoolManager.Fetch(typeof(TaskTargetCompletedCondition)) as ConditionBase;
                    break;
                case EConditionType.GreaterThanLv:
                    condition = PoolManager.Fetch(typeof(GreaterThanLvCondition)) as ConditionBase;
                    break;
                case EConditionType.LessThanLv:
                    condition = PoolManager.Fetch(typeof(LessThanLvCondition)) as ConditionBase;
                    break;
                case EConditionType.EqualLv:
                    condition = PoolManager.Fetch(typeof(EqualLvCondition)) as ConditionBase;
                    break;
                case EConditionType.GreaterOrEqualLv:
                    condition = PoolManager.Fetch(typeof(GreaterOrEqualLvCondition)) as ConditionBase;
                    break;
                case EConditionType.FunctionOpen:
                    condition = PoolManager.Fetch(typeof(FunctionOpenCondition)) as ConditionBase;
                    break;
                case EConditionType.ActivedNPC:
                    condition = PoolManager.Fetch(typeof(ActivedNPCCondition)) as ConditionBase;
                    break;
                case EConditionType.HasInquiryedFavorabilityNpc:
                    condition = PoolManager.Fetch(typeof(HasInquiryedFavorabilityNpcCondition)) as ConditionBase;
                    break;
                case EConditionType.GodTest:
                    condition = PoolManager.Fetch(typeof(GodTestCondition)) as ConditionBase;
                    break;
                case EConditionType.Secret:
                    condition = PoolManager.Fetch(typeof(SecretCondition)) as ConditionBase;
                    break;
                case EConditionType.NoSecret:
                    condition = PoolManager.Fetch(typeof(NoSecretCondition)) as ConditionBase;
                    break;
                case EConditionType.Escort:
                    condition = PoolManager.Fetch(typeof(EscortCondition)) as ConditionBase;
                    break;
                case EConditionType.NpcFollow:
                    condition = PoolManager.Fetch(typeof(NpcFollowCondition)) as ConditionBase;
                    break;
                case EConditionType.HavePartner:
                    condition = PoolManager.Fetch(typeof(HavePartnerCondition)) as ConditionBase;
                    break;
                case EConditionType.InTeam:
                    condition = PoolManager.Fetch(typeof(InTeamCondition)) as ConditionBase;
                    break;
                case EConditionType.TimeLimitTaskGoalOn:
                    condition = PoolManager.Fetch(typeof(TimeLimitTaskGoalOnCondition)) as ConditionBase;
                    break;
                case EConditionType.TimeLimitTaskGoalOff:
                    condition = PoolManager.Fetch(typeof(TimeLimitTaskGoalOffCondition)) as ConditionBase;
                    break;
                case EConditionType.LoginDay:
                    condition = PoolManager.Fetch(typeof(LoginDayCondition)) as ConditionBase;
                    break;
                case EConditionType.HaveTreasure:
                    condition = PoolManager.Fetch(typeof(HaveTreasureCondition)) as ConditionBase;
                    break;
                case EConditionType.FamilyBossOpen:
                    condition = PoolManager.Fetch(typeof(FamilyBossCondition)) as ConditionBase;
                    break;
                case EConditionType.FamilyType:
                    condition = PoolManager.Fetch(typeof(FamilyTypeCondition)) as ConditionBase;
                    break;
                case EConditionType.FamilyStage:
                    condition = PoolManager.Fetch(typeof(FamilyStageCondition)) as ConditionBase;
                    break;
                case EConditionType.FamilyTrainOpenTime:
                    condition = PoolManager.Fetch(typeof(FamilyTrainTimeCondition)) as ConditionBase;
                    break;
                case EConditionType.FamilyTrainCreature:
                    condition = PoolManager.Fetch(typeof(FamilyTrainCondition)) as ConditionBase;
                    break;
                case EConditionType.AwakenImprint:
                    condition = PoolManager.Fetch(typeof(AwakenImprintCondition)) as ConditionBase;
                    break;
                case EConditionType.TeamMemberCount:
                    condition = PoolManager.Fetch(typeof(TeamMemberCountCondition)) as ConditionBase;
                    break;
                case EConditionType.GreaterFamilyConstructLevel:
                    condition = PoolManager.Fetch(typeof(GreaterFamilyConstructLevel)) as ConditionBase;
                    break;
                case EConditionType.LessFamilyConstructLevel:
                    condition = PoolManager.Fetch(typeof(LessFamilyConstructLevel)) as ConditionBase;
                    break;
                case EConditionType.FamilyCOnstructExpMax:
                    condition = PoolManager.Fetch(typeof(FamilyCOnstructExpMax)) as ConditionBase;
                    break;
                case EConditionType.GreaterFamilyBuildLevel:
                    condition = PoolManager.Fetch(typeof(GreaterFamilyBuildLevel)) as ConditionBase;
                    break;
                case EConditionType.LessFamilyMainBuildLevel:
                    condition = PoolManager.Fetch(typeof(LessFamilyMainBuildLevel)) as ConditionBase;
                    break;
                case EConditionType.ActivityGMSetting:
                    condition = PoolManager.Fetch(typeof(ActivityGMSettingCondition)) as ConditionBase;
                    break;
                case EConditionType.WorldLevel:
                    condition = PoolManager.Fetch(typeof(WorldLevelCondition)) as ConditionBase;
                    break;
                case EConditionType.TownTaskAvaiable:
                    condition = PoolManager.Fetch(typeof(TownTaskAvaiableCondition)) as ConditionBase;
                    break;
                case EConditionType.CheckTimeInYear:
                    condition = PoolManager.Fetch(typeof(CheckTimeInYearCondition)) as ConditionBase;
                    break;
                case EConditionType.CheckTimeMonth:
                    condition = PoolManager.Fetch(typeof(CheckTimeInMonthCondition)) as ConditionBase;
                    break;
                case EConditionType.CheckTimeOnDay:
                    condition = PoolManager.Fetch(typeof(CheckTimeOnDayCondition)) as ConditionBase;
                    break;
                case EConditionType.CheckTimeOnWeek:
                    condition = PoolManager.Fetch(typeof(CheckTimeOnWeekCondition)) as ConditionBase;
                    break;
                case EConditionType.CheckBossTowerState:
                    condition = PoolManager.Fetch(typeof(CheckBossTowerStateCondition)) as ConditionBase;
                    break;
                case EConditionType.CheckBlessState:
                    condition = PoolManager.Fetch(typeof(CheckBlessStateCondition)) as ConditionBase;
                    break;
                default:
                    Lib.Core.DebugUtil.LogError($"ERROR!!! Create Condition failed, ConditionType:{type.ToString()}");
                    break;
            }

            return condition;
        }
    }
}