using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// 条件类型///
    /// </summary>
    public enum EConditionType
    {
        HaveTask = 10001,       //接受了任务
        Time = 10011,           //时间是否相符 (白天晚上)
        Weather = 10021,        //天气是否相符
        Season = 10031,         //季节是否相符
        HaveItem = 10041,       //拥有相应数量的物品
        TaskUnReceived = 10071,     //任务状态是未接受
        TaskUnCompleted = 10081,    //任务状态是未完成
        TaskCompleted = 10051,      //任务状态是已完成
        TaskSubmitted = 10091,      //任务状态是已提交
        EqualCareer = 10111,  //等于职业
        PassStage = 10121,          //通过关卡
        DialogueChoose = 10131,
        TaskTargetUnCompleted = 10151,      //任务目标未完成
        TaskTargetCompleted = 10161,        //任务目标已完成        
        GreaterThanLv = 10181,    //大于等级
        LessThanLv = 10191,       //小于等级
        EqualLv = 10201,        //等于等级 
        GreaterOrEqualLv = 10202,
        FunctionOpen = 10301,   //功能推送
        ActivedNPC = 10401,     //激活NPC
        HasInquiryedFavorabilityNpc = 10501,    //有调查过有好感度的NPC
        GodTest = 10701,        //女神试炼
        Secret = 10801,         //密语
        NoSecret = 10811,       //未完成密语
        InTeam = 10901,         //组队
        TimeLimitTaskGoalOn = 11001,//限时任务
        TimeLimitTaskGoalOff = 11002,
        LoginDay = 11201,               //活跃天数
        HaveTreasure = 11401,      //拥有宝藏库
        FamilyType = 11501,     //家族兽类型
        FamilyStage = 11601,     //家族兽阶段
        FamilyBossOpen = 11701,    //牛鬼来袭(家族boss)是否开启
        FamilyTrainCreature = 11801,    //是否是训练中的对象
        FamilyTrainOpenTime = 11901,    //是否是训练中的时间
        TeamMemberCount = 12101,     //队伍人数大于等于
        GreaterFamilyConstructLevel = 12401,     //家族建设繁荣度等级大于等于
        LessFamilyConstructLevel = 12501,     //家族建设繁荣度等级小于
        FamilyCOnstructExpMax = 12601,     //家族建设繁荣度经验是否已满
        GreaterFamilyBuildLevel = 12701,     //家族建筑等级大于等于
        LessFamilyMainBuildLevel = 12801,   //家族城堡等级小于
        ActivityGMSetting = 12901,  //活动紧急开关
        WorldLevel = 13001,     //世界等级
        Escort = 1000002,       //护送    
        NpcFollow = 1000003,    //Npc跟随
        NpcTrack = 1000004,     //Npc跟踪
        HavePartner,            //拥有伙伴 
        AwakenImprint=12001,
        TownTaskAvaiable = 13201,   //城镇任务开启
        CheckTimeInYear = 13301,    //某年的某个时间段
        CheckTimeMonth = 13311,     //某月的某个时间段
        CheckTimeOnDay = 13321,    //某日的某个时间段
        CheckTimeOnWeek = 13322,    //某周的某个时间段
        CheckBossTowerState = 13323, //检查boss资格赛的状态
        CheckBlessState = 13324, //检查boss资格赛的状态
    }

    /// <summary>
    /// 条件基类///
    /// </summary>
    public abstract class ConditionBase
    {
        public virtual EConditionType ConditionType
        {
            get;
        }

        public enum ESourceType
        {
            Common,
            ConditionGroup,
        }

        private ESourceType _eSourceType;
        public ESourceType SourceType
        {
            get
            {
                return _eSourceType;
            }
            set
            {
                _eSourceType = value;
            }
        }

        private uint _handlerID;

        public uint HandlerID
        {
            get
            {
                return _handlerID;
            }
            set
            {
                _handlerID = value;
            }
        }

        public abstract void DeserializeObject(List<int> data);

        public abstract bool IsValid();

        public void Dispose()
        {
            _eSourceType = ESourceType.Common;
            _handlerID = 0;
            OnDispose();
            PoolManager.Recycle(this);
        }

        protected abstract void OnDispose();
    }
}
