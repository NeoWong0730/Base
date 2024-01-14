namespace Logic
{
    public enum ERedPointShowMode
    {
        Show,
        Hide,
        Ignore,
    }

    public enum ERedPointType
    {
        Pure,
        WithNumber,
    }

    public enum ESwitch
    {
        On,
        Off,
    }

    public enum EDir
    {
        SelfOrUnknown = 0,
        Up = 1,
        Left = 2,
        Right = 3,
        Down = 4,
        Max = 5, // 同一个轴上，和为Max
    }

    public enum EDialogueType
    {
        Dialogue,
        MenuDialogue,
    }

    /// <summary>
    /// 功能类型///
    /// </summary>
    public enum EFunctionType
    {
        None = 0,
        Group = 1,          //功能组
        Dialogue = 2,       //对话
        Task = 3,           //任务
        DirectlyFight = 4,  //交互战斗
        Collection = 5,     //采集
        Transmit = 6,       //传送
        TriggerFight = 7,   //范围战斗
        DialogueChoose = 8, //对话选择
        Task_PathFindOpenUI = 9,         //打开ui
        ExecuteSystemModule = 10, //执行系统功能
        SubmitItem = 11,    //上交道具
        Inquiry = 12,       //调查
        PetTransformation = 13,//宠物改造
        Bar = 14,           //酒吧事件
        Visit = 15,         //拜访功能
        Prestige = 16,      //声望功能
        SignUp = 17, // 世界boss报名功能
        Shop = 18,  //商店功能
        Advance = 19, //进阶功能
        Cook = 20, //烹饪功能
        CutScene = 21,  //过场动画功能
        InterfaceBubble = 22, //界面气泡
        TaskEnterArea = 23, //  任务进入区域
        OpenTeam = 24,      //打开快捷组队
        ClassicBoss = 25, //经典BOSS战
        SecretMessage = 26, //密语
        ClockIn = 27,       //打卡
        ActiveKnowledge = 28,   //激活旅人志
        LearnActiveSkill = 29,  //学习主动技能
        LearnPassiveSkill = 30, //学习被动技能
        HpMpUp = 31,        //血蓝回复
        ResourceSubmit = 32,    //资源上交
        FamilyBattle = 33,      //家族战斗
        ReciveFamilyTask = 34,
        CreateWarriorGroup = 35, //创建勇者团
    }

    public enum EDailogueAnswerRightOrWrong
    {
        Right = 0,
        Wrong = 1,
    }

    public enum EDialogueWrongResult
    {
        Close,
        ReChoose,
    }

    public enum EDialogueWrongDetach
    {
        UnDetach,
        Detach,
    }

    public enum EDialogueActorType
    {
        Player,
        NPC,
        Parnter,
        PangBai,
        Monster,
    }

    public enum ETaskShowType
    {
        None,
        //Clue_Sumbited,
        //Clue_UnFinish,
        //Clue_Finish,
        Clue_NoReceive,
        Love_Sumbited,
        Love_UnFinish,
        Love_Finish,
        Love_NoReceive,
        Challenge_Sumbited,
        Challenge_UnFinish,
        Challenge_Finish,
        Challenge_NoReceive,
        //Arrest_Sumbited,
        //Arrest_UnFinish,
        //Arrest_Finish,
        Arrest_NoReceive,
    }

    /// <summary>
    /// NPC类型///
    /// </summary>
    public enum ENPCType
    {
        None,
        Common = 1,         //对话
        Collection = 2,     //采集              
        Note = 3,           //记录
        Transmit = 4,       //传送
        ActiveMonster = 5,  //明怪
        LittleKnow = 6,     //小知识
        WorldBoss = 7,      //世界Boss
        EventNPC = 8,       //事件点
    }

    /// <summary>
    /// 记录npc子类型///
    /// </summary>
    public enum ENPCNoteSubType
    {
        None,
        Transmit,       //传送
        Discovery,      //探索
        Resource,       //资源    
        Trade,          //贸易
    }

    /// <summary>
    /// Npc标记类型
    /// </summary>
    public enum ENPCMarkType
    {
        None = 0,           //无
        Transmit = 1,       //传送点
        Exploration = 2,    //探索点
        Resources = 3,      //资源点
        Trade = 4,          //贸易点
        LoveTask = 5,       //爱心任务
        ChallengeTask = 6,  //挑战任务
        Collection = 7,     //采集资源点
        Lumbering = 8,      //伐木资源点
        Fishing = 9,        //钓鱼资源点
        Mining = 10,        //采矿资源点
        Hunting = 11,       //狩猎资源点
        PathWayTransmit = 12,//寻路用传送点
        GoWay = 13,          //前往点
        FamilyResBattle = 14,          //家族资源战
        
        ResourcesNew = 15,
        MiningNew = 16,     //采集资源点
        LumberingNew = 17,      //伐木资源点
        CollectionNew = 18,        //采矿资源点
        
        Food = 19,         //食材点
        Food_Fishing = 20,         //食材点
        Food_LianDan = 21,         //食材点
        Food_Hunting = 22,         //食材点
        Food_Water = 23,         //食材点

        CatchPet = 24,          //抓宠点
    }

    /// <summary>
    /// 交互类型///
    /// </summary>
    public enum EInteractiveType
    {
        None,

        #region 主动型

        Click,              //单击
        DoubleClick,        //双击
        LongPress,          //长按
        UIButton,           //按钮

        #endregion

        #region 被动型

        AreaCheck,        //进入范围触发
        DistanceCheck,    //直线距离检测触发

        #endregion              
    }

    /// <summary>
    /// 交互目的类型///
    /// </summary>
    public enum EInteractiveAimType
    {
        None,
        NPCFunction,            //NPC功能
        ChooseActorInFight      //战斗中选中Actor
    }

    /// <summary>
    /// 职业类型///
    /// </summary>
    public enum ECareerType
    {
        None = 100,           //无职业
        Archer = 101,       //弓箭手
        Magician = 201,     //法师
        Swordman = 301,     //剑士
        Missionary = 401,   //传教士
        Axe = 501,          //战斧
        Conjurer = 601,     //咒术师
        Fighter = 701,      //格斗家
    }

    /// <summary>
    /// 战斗角色类型///
    /// </summary>
    public enum EFightActorType
    {
        None,
        Hero = 1,           //玩家自己
        Pet = 2,            //宠物
        Monster = 3,        //怪
        Partner = 4,        //同伴
    }

    public enum EFightOutActorType
    {
        None,
        MainHero,  //玩家自己
        Teammate,  //队友 
        OtherHero, //其他玩家
        Npc,       //Npc
        Monster,   //怪物
        Partner,   //伙伴
    }


    /// <summary>
    /// 装备种类///
    /// </summary>
    public enum EEquipType
    {
        Weapon = 1,         //武器
        Armor = 2,          //防具
    }

    ///武器种类///
    public enum EWeaponType
    {
        Unarmed = 1,        //空手
        Sword = 2,          //剑
    }

    /// <summary>
    /// 玩家动作状态///
    /// </summary>
    public enum EStateType
    {
        Idle = 1001,            //待机
        Walk = 1002,            //移动
        Run = 1003,             //跑步动作
        Login = 1004,           //进场
        DialogueIdle = 1007,    //对话待机
        Stand = 1005,
        Sprint = 1006,

        NormalAttack = 1101,    //普通攻击

        Escape = 1201,          //逃跑
        EscapeFailure = 1202,   //逃跑失败

        Die1 = 1301,            //死亡动作1
        Die2 = 1302,            //死亡动作2
        Die3 = 1303,            //死亡动作3

        Defense = 1401,         //防御受击
        BeHit = 1402,           //正常受击
        Alliance = 1403,        //合击攻击动作

        BattleMove = 1501,      //位移动作

        Cast = 1601,            //魔法技能

        RandomShotStart = 2001,     //乱射准备
        RandomShotLoop = 2002,      //乱射攻击
        RandomShotLoopEnd = 2003,   //乱射结束

        NPCShow1 = 3001,        //npc表演1
        NPCShow2 = 3002,        //npc表演2
        NPCShow3 = 3003,        //npc表演3

        Collection = 3101,      //采集动作
        Collection2 = 3102,     //采集动作1
        //Collection3 = 3103,     //采集动作2
        CollectEnd = 3104,
        LossLife = 3201,        //采集物生命消耗
        EndLife = 3202,         //采集物生命结束

        Inquiry = 3301,         //调查
        InquiryEnd = 3302,     //调查结束

        Mining = 3401,
        Logging = 3402,
        fishing = 3403,
        hunting = 3404,

        mount_1_inquiry = 3501,
        mount_1_idle = 3502,
        mount_1_show_idle = 3503,
        mount_1_walk = 3504,
        mount_1_run = 3505,

        mount_2_inquiry = 3601,
        mount_2_idle = 3602,
        mount_2_show_idle = 3603,
        mount_2_walk = 3604,
        mount_2_run = 3605,

        Depressed = 4001,       //沮丧
        Laugh = 4002,           //大笑
        Sad = 4003,             //忧愁
        Agree = 4004,           //同意
        Nervous = 4005,         //紧张防备
        Shy = 4006,             //不好意思
        Uplift = 4007,          //振奋
        Ponder = 4008,          //沉思
        Resolve = 4009,         //决断

        Cry = 4101,             //哭
        Toothache = 4102,       //牙疼
        Mad = 4103,
        Book = 4104,
        Pray = 4105,
        However = 4106,
        Angry = 4107,
        Doubt = 4108,
        Satisfy = 4109,
        Jump = 4110,
        Wave = 4111,
        Think1 = 4112,
        Surprise = 4113,
        Talk = 4114,
        Dialogue_Idle = 4115,

        UI_Show_Entrance = 6001,
        UI_Show_Idle = 6002,
        UI_Show_Idleshow01 = 6003,
        UI_Show_Idleshow02 = 6004,      

        None = 99999,
    }

    /// <summary>
    /// 地图类型
    /// </summary>
    public enum EMapType
    {
        None = 0,     //无
        Country = 1,  //国家
        Island = 2,   //岛屿
        Map = 3,      //地图
        Maze = 4,     //迷宫
    }

    /// <summary>
    /// 个人采集限制类型///
    /// </summary>
    public enum EICollectionLimitType
    {
        Day = 1,
        Week,
        Month,
        Forever,
    }

    /// <summary>
    /// 属性表，属性类型
    /// </summary>
    public enum EAttryType
    {
        None = 0,
        Basic = 1,
        High = 2,
        Element = 3
    }

    /// <summary>
    /// 道具品质类型
    /// </summary>
    public enum EItemQuality
    {
        White = 1,
        Green = 2,
        Blue = 3,
        Purple = 4,
        Orange = 5,
    }

    public enum EGetBriefInfoClassify
    {
        SocialSearchRole = 0,
        Sys_Role_Info = 1,
    }
    /// <summary>
    /// 活动类型
    /// </summary>
    public enum EActiveType
    {
        Daily = 1,     //日常
        TimeLimit = 2, //限时
    }
    /// <summary>
    /// 活动类型2
    /// </summary>
    public enum EActiveType2
    {
        Instance = 1, //副本类型
    }
    /// <summary>
    /// 副本类型
    /// </summary>
    public enum EInstanceType
    {
        None = 0,        //无
        SingleDaily = 1, //单人日常本
        MultiDaily = 2,  //多人日常本
        TerroristWeek = 3, //恐怖旅团多人副本
        HundreadPeolle = 5, //百人道场
    }

    /// <summary>
    /// 流程事件///
    /// </summary>
    public enum EProcedureEvent
    {
        EnterNormal = 0,
        EnterInteractive,
        EnterFight,
        EnterCutScene,

    }

    public enum EFunctionSourceType
    {
        None,   //常驻
        Task,   //任务
    }

    public enum EBubbleType
    {
        PureWord,
        EmojiText,
        Pic,
    }

    public enum EEquipmentQuality
    {
        None = 0,
        White = 1, //白
        Green = 2, //绿
        Blue = 3,  //蓝
        Purple = 4, //紫
        Orange = 5, //橙
    }

    public enum ECurrencyType
    {
        None = 0,
        Diamonds = 1,    //钻石
        GoldCoin = 2,    //金币
        SilverCoin = 3,  //银币
        Experience = 4,  //经验
        Vitality = 5,    //活力
        Spar = 6,        //晶石晶源
        Prestige = 7,    //声望
        Feats = 8,       //功勋
        Runefragment = 9,//符文碎片
        PersonalContribution = 10, //个人贡献
        FamilyCoin = 11,  //家族资金
        CompetitiveIntegral = 12,//竞技积分
        Aid = 13,         //援助值
        CaptainPoint = 14,   //队长积分
        Knowledge = 15,      //旅人币
        AdventuresExp = 16,   //冒险者经验
        FamilyStamina = 17,  //家族令牌
        AkaDyaAncientGoldenCoin = 18,//阿卡迪亚古钱币
        War = 19,              //战令币
        GuildCurrency=20,          //家族货币
        FashionPoint=21,        //时装积分
        ActivityToken=22,       //活动代币
        CurrencyPoint=23,      //货币点卡点数
        TransformationPoint=24,    //变身积分
        Currency_25,    //秘医
        Currency_26,    //阿尔卡迪亚
        Currency_27,    //卡拉班
        Currency_28,    //恐怖旅团
        Currency_29,    //阿斯提亚
        Currency_30,    //魔物
        Currency_31,    //龙族
        Currency_32,    //诅咒
        Currency_33,    //万能
        Currency_34,    //天梯
        Currency_35,    //卡拉班的黑市金币
        Currency_36,    //卡拉版黑市古代币
        Domesticate,    //驯养值
        Max ,
    }

    public enum EFamilyConstruct
    {
        FarmingProsperity = 433001,  //农业繁荣度
        BusinessProsperity = 433002,//商业繁荣度
        SecurityProsperity = 433003,//治安繁荣度
        ReligiousProsperity = 433004,//宗教繁荣度
        TechProsperity = 433005,//科技繁荣度
    }
}
