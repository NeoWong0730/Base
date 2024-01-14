using System.ComponentModel;

public enum CombatBehaveAIEnum
{
	None = 0,

    #region 1-1000是node节点枚举
    [Description("播放特效")]
    PlayEffect = 1,
    [Description("删除特效")]
    DelEffect = 2,
    [Description("等待时间")]
    WaitTime = 3,
    [Description("播放动作")]
    PlayAnimation = 4,
    [Description("向目标点位移[T]")]
    MoveToTargetPos = 5,
    [Description("生成飞弹")]
    CreateBullets = 7,
    [Description("播放飙血数字")]
    ShowDmg = 8,
    [Description("受击方后退")]
    TargetBack = 9,
    [Description("按类型单位向初始点位移[T]")]
    MoveToOriginByTypeMob = 10,
    [Description("模型消失")]
    HideModel = 11,
    [Description("向初始点位移")]
    GoBackToStart = 12,
    [Description("时间缩放")]
    TimeScale = 13,
    [Description("mob抖动")]
    MobShake = 14,
    [Description("朝向目标")]
    LookAtTarget = 16,
    [Description("显示释放的技能内容")]
    ShowSkillContent = 17,
    [Description("等待攻击_合击状态下")]
    WaitAttack_CombineAttack = 18,
    [Description("通知攻击_合击状态下")]
    InformAttack_CombineAttack = 19,
    [Description("播放音效")]
    PlayAudio = 20,
    [Description("相机抖动")]
    CameraShake = 21,
    [Description("自身偏移[T]")]
    LocalOffset = 22,
    [Description("冒气泡")]
    PopupBubble = 23,
    [Description("播放残影")]
    PlayGhostShadow = 24,
    [Description("删除残影")]
    DelGhostShadow = 25,
    [Description("收集行为数据【目标带跳转块】")]
    CollectBehaveInfo = 26,
    [Description("被击飞死（怪物和宠物）")]
    BeHitToFlyDead = 27,
    [Description("轮询数量标记")]
    PollingNodeMark = 28,
    [Description("返回轮询数量标记")]
    BackToPollingNodeMark = 29,
    [Description("播放延迟删除特效")]
    PlayDelayEffect = 30,
    [Description("增加Y轴角度")]
    RotationYAngle = 31,
    [Description("朝正前方移动")]
    MoveForward = 32,
    [Description("改变动画播放速度")]
    ChangeAnimationSpeed = 33,
    [Description("执行技能行为")]
    DoSkillBehave = 34,
    [Description("模型缩放")]
    ModelScale = 35,
    [Description("对位置生成飞弹")]
    CreateBulletsToNum = 36,
    [Description("停止残影")]
    StopGhostShadow = 37,
    [Description("匹配战斗单位类型")]
    MatchingCombatUnitType = 38,
    [Description("技能等级判断")]
    SkillLvOption = 39,
    [Description("旋转")]
    RotationByTime = 40,
    [Description("武器类型")]
    WeaponType = 41,
    [Description("随机节点")]
    RandomNode = 42,
    [Description("怪物状态")]
    MonsterState = 43,
    [Description("结束当前回合演示")]
    OverShow = 44,
    [Description("播放CutScene")]
    PlayCutScene = 45,
    [Description("判断目标状态")]
    SelectTargetState = 46,
    [Description("更新武器动作")]
    UpdateWeaponAnimation = 47,
    [Description("怪物级别")]
    MonsterRank = 48,
    [Description("匹配怪物Id")]
    MatchMonsterId = 49,
    [Description("切换场景")]
    SwitchCombatScene = 50,
    [Description("结束当前块")]
    OverCurBlock = 51,
    [Description("怪物执行死亡WorkId")]
    MonsterDoWorkInDeath = 52,
    [Description("进入战斗阶段设置")]
    SetBattleStage = 53,
    [Description("隐藏所有战斗单位")]
    HideBattleUnits = 54,
    [Description("抛物线轨迹")]
    FlyParabolaTrack = 55,
    [Description("删除对象成功判断")]
    IsDelObjSuccess = 56,
    [Description("是否加入新对象")]
    IsAddNewMob = 57,
    [Description("攻击效果")]
    HitEffect = 58,
    [Description("直接跳WorkStrem块")]
    JumpToWorkStreamBlock = 59,
    [Description("循环节点标记")]
    LoopStart = 60,
    [Description("返回循环节点标记")]
    BackToLoopStart = 61,
    [Description("判断当前遇敌组")]
    CheckCurMonsterGroup = 62,
    [Description("冒指定气泡")]
    PopupSpecificBubble = 63,
    [Description("隐藏Boss Mesh")]
    HideBossMesh = 64,
    [Description("向站位坐标类型点移动[T]")]
    MoveToPosType = 65,
    [Description("设置强制回合结束时间")]
    SetForceDoExcuteTurnTime = 66,
    [Description("判断主动技能表active_skill_behavior_id")]
    CheckSkillTbBehaviorId = 68,
    [Description("匹配施放者的HitEffectType")]
    MatchingAttackHitEffectType = 70,
    [Description("显示被动名称飘字")]
    ShowPassiveNameFont = 71,
    [Description("伤害类型飘字")]
    ShowHitEffectTypeFont = 72,
    [Description("施放者自己是否有蓝量变化")]
    IsChangeMpForAttack = 73,
    [Description("技能喊话")]
    SkillShout = 74,
    [Description("站位类型")]
    BattlePosType = 75,
    [Description("Stop行为")]
    StopWorkStreamStyle = 76,
    [Description("被护卫者后退")]
    BeProtectedToBack = 77,
    [Description("战斗类型的进场效果")]
    EnterSceneEffectInBattleType = 78,
    [Description("更新行为次数[T]")]
    DoUpdateBehaveCount = 79,
    [Description("飞弹执行击中后的处理")]
    DoProcessAfterHit = 80,
    [Description("飞弹取消击中后的处理")]
    CancelProcessAfterHit = 81,
    [Description("公用的等待时间")]
    CommonWaitTime = 82,
    [Description("执行换位")]
    DoUnitChangePos = 83,
    [Description("服务器战斗单位改变时的类型")]
    BattleUnitChangeType = 84,
    [Description("黑屏过渡")]
    ScreenBlackTransition = 85,
    [Description("生成闪现战斗单位")]
    CreateBattleUnitForFlash = 86,
    [Description("通知护卫")]
    InformReadyProject = 87,
    [Description("只是收集行为数据【不带跳转】")]
    CollectBehaveInfoNoSkip = 88,
    [Description("护卫是否结束")]
    ProjectIsOver = 89,
    [Description("记录N次后等待一帧")]
    WaitFrameAfterRecordNTimes = 90,
    [Description("暂停/解除动画")]
    PauseOrBreakAnimation = 91,
    [Description("反击阻碍")]
    FightBackHinder = 92,
    [Description("反击阻碍解除")]
    RemoveFightBackHinder = 93,
    [Description("目标依据状态跳Block")]
    SkipByState = 94,
    [Description("服务器执行数据的类型")]
    ServerExcuteDataExtendType = 95,
    [Description("是否有反击行为")]
    IsHaveFightBackBehave = 96,
    [Description("从主队列中执行反击行为")]
    DoFightBackFromMainControllerQueue = 97,
    [Description("是否为主角")]
    IsPlayer = 98,
    [Description("设为初始状态")]
    IsSetOriginState = 99,
    [Description("判断目标身上的BUFF状态")]
    CheckTargetBuffEffective = 100,
    [Description("动画启动或关闭")]
    EnableAnimation = 101,
    [Description("匹配buffId")]
    MatchBuffId = 102,
    [Description("判断当前遇敌组(多个)")]
    CheckCurMonsterMutliGroup = 103,

    [Description("阻碍【只在本次行为中生效】")]
    HinderNode = 401,
    [Description("解除阻碍【只在本次行为中生效】")]
    RemoveHinderNode = 402,

    [Description("被动执行时机")]
    DoPassiveTiming = 501,
    [Description("触发时机效果")]
    DoTriggerTimingEffect = 502,

    [Description("切换【控制器】类型")]
    SwitchWorkStreamStyle = 997,
    [Description("生成节点子WorkStream")]
    CreateChildWorkStream = 998,
    [Description("辅助节点")]
    AssistNode = 999,
    [Description("测试节点")]
    JustTest = 1000,
    #endregion

    #region 1001+是block枚举
    [Description("技能释放(前)")]
    B_ReleaseSkill_Before = 1101,
    [Description("技能释放(中)")]
    B_ReleaseSkill_Ing = 1201,
    [Description("技能释放(结束)")]
    B_ReleaseSkill_End = 1301,

    [Description("技能释放(前)立即执行")]
    B_ReleaseSkillImmediate_Before = 1401,
    
    [Description("命中(正常状态)敌人时")]
    B_BeHit_Normal = 2001,
    [Description("命中(防御状态)敌人时")]
    B_BeHit_Def = 2002,
    [Description("命中(圣盾状态)敌人时")]
    B_BeHit_DivineShield = 2003,
    [Description("命中(护卫状态)敌人时")]
    B_BeHit_Guard = 2004,
    [Description("命中(阳炎状态)敌人时")]
    B_BeHit_Sunburst = 2005,
    [Description("命中(反击状态)敌人时")]
    B_BeHit_FightBack = 2006,
    [Description("命中(攻击无效[物理免疫])敌人时")]
    B_BeHit_AttackInvalid = 2008,
    [Description("命中(魔法无效[魔法免疫])敌人时")]
    B_BeHit_MagicInvalid = 2009,
    [Description("命中(石化状态)敌人时")]
    B_BeHit_RockState = 2010,
    
    [Description("命中(死亡状态)敌人时")]
    B_BeHit_Death = 2901,
    [Description("未命中(死亡状态)敌人时")]
    B_NoBeHit_Death = 2902,

    [Description("未命中(正常状态)敌人时")]
    B_NoBeHit_Normal = 3001,
    [Description("未命中(防御状态)敌人时")]
    B_NoBeHit_Def = 3002,
    [Description("未命中(圣盾状态)敌人时")]
    B_NoBeHit_DivineShield = 3003,
    [Description("未命中(护卫状态)敌人时")]
    B_NoBeHit_Guard = 3004,
    [Description("未命中(阳炎状态)敌人时")]
    B_NoBeHit_Sunburst = 3005,
    [Description("未命中(反击状态)敌人时")]
    B_NoBeHit_FightBack = 3006,
    [Description("未命中(攻击无效[物理免疫])敌人时")]
    B_NoBeHit_AttackInvalid = 3008,
    [Description("未命中(魔法无效[魔法免疫])敌人时")]
    B_NoBeHit_MagicInvalid = 3009,
    [Description("未命中(石化状态)敌人时")]
    B_NoBeHit_RockState = 3010,
    
    [Description("未生效")]
    B_NoTakeEffect = 3999,

    [Description("命中后")]
    AfterBeHit = 4001,
    [Description("倒地死亡")]
    B_FallDead = 4101,
    [Description("消失死亡")]
    B_OutDead = 4201,

    [Description("[溅射]攻击效果")]
    B_HitEffectTypeSpurting = 4904,
    [Description("[护卫]攻击效果")]
    B_HitEffectTypeProtect = 4906,
    [Description("[附加伤害]攻击效果")]
    B_HitEffectTypeAddtion = 4907,
    [Description("[吸血]攻击效果")]
    B_HitEffectTypeVmpare = 4909,
    [Description("[吸收]攻击效果")]
    B_HitEffectTypeAbsorb = 4910,

    [Description("进入场景")]
    B_EnterScene = 5001,

    [Description("逃跑成功")]
    B_RunAwaySuccess = 5002,
    [Description("逃跑失败")]
    B_RunAwayFail = 5003,
    [Description("逃跑")]
    RunAway = 5004,

    [Description("回合结束")]
    RoundOver = 5010,

    [Description("添加战斗单位块")]
    AddCombatUnitBlock = 6001,
    [Description("连战进场")]
    ContinueCombatBlock = 6002,

    [Description("护卫准备")]
    ReadyProtectBlock = 6101,
    [Description("护卫结束")]
    OverProtectBlock = 6102,

    [Description("反击准备")]
    ReadyFightBackBlock = 6201,

    [Description("通用模块")]
    B_Common = 100001,

    [Description("【WorkStream】切换控制器")]
    B_SwitchWorkStreamController = 1000001,
    #endregion
}