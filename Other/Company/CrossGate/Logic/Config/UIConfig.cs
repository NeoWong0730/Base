using System;
using System.Collections.Generic;
using Logic;
using EUIOption = Framework.Core.UI.EUIOption;
using UIConfigData = Framework.Core.UI.UIConfigData;

namespace Logic
{
    //public class UIConfigData : Framework.Core.UI.UIConfigData
    //{
    //    /// <summary>
    //    /// UI配置
    //    /// </summary>
    //    /// <param name="prefabPath">预制体加载路径</param>
    //    /// <param name="script">逻辑脚本类型</param>
    //    /// <param name="options">UI设置组</param>
    //    /// <param name="order">
    //    /// 深度值
    //    /// 当设置包含eIgnoreStack时:
    //    /// order 大于等于 0 : UI栈层级上限 + order
    //    /// order 小于 0 : UI栈层级下限 + order
    //    ///
    //    /// 当设置不包含eIgnoreStack时:
    //    /// order暂时无效
    //    /// </param>
    //    public UIConfigData(Type script, string prefabPath, Framework.Core.UI.EUIOption options = Framework.Core.UI.EUIOption.eInvalid, int order = 0)
    //        : base(script, prefabPath, options, order) { }
    //}

    /// <summary>
    /// 添加界面 依次添加!
    /// 添加界面 依次添加!
    /// 添加界面 依次添加!
    /// </summary>
    public enum EUIID
    {
        Invalid = 0,
        UI_Loading = 1,         // 加载界面
        UI_Login = 2,           // 登录界面
        UI_Server = 3,          // 服务器
        UI_CreateCharacter = 4, // 创角
        UI_Hint = 5,            // 提示
        UI_Joystick = 6,        //遥杆
        UI_MainInterface = 7,    //主界面面板管理容器
        UI_Career = 8,              //选择职业
        UI_ChoosePet = 9,           //选择宠物
        UI_Menu = 10,                //主界面
        UI_MainBattle = 11,          //战斗界面
        UI_MainBattle_Pet = 12,      //选择宠物
        UI_MainBattle_Skills = 13,   //选择技能
        UI_Buff = 14,                //战斗 buff信息面板
        HUD = 15,                    //HUD
        UI_Setting = 16,             //设置界面
        UI_Reconnection = 17,        //断线重连界面
        UI_PromptBox = 18,           //确认界面(通用)
        UI_Bag = 19,                 //主背包
        UI_Prop_Message = 20,        //主背包物品详情界面
        UI_Sale_Prop = 21,           //出售界面
        UI_TemporaryBag = 22,        //临时背包
        UI_Message_Box = 23,         //临时背包物品详情界面
        UI_BatchUse_Box = 24,        //批量使用
        UI_Exchange_Coin = 25,       //金币兑换
        UI_SafeBox = 26,             //银行
        UI_PerForm = 27,             //道具飞入特效界面（后续可以作为各种界面拓展表现的root）
        UI_NPC = 28,                 //NPC功能面板
        UI_TaskList = 29,            // 任务总览界面
        UI_TaskSelectBox = 30,   // 任务多选一
        UI_TaskNormalResult = 31,    // 任务提交UI
        UI_SharedTaskList = 32,  // 任务分享
        UI_TaskSpecialResult = 33, // 爱心/挑战结算
        UI_Team_Member = 34,//组队
        UI_Team_Player = 35,//
        UI_Team_WarCommand = 36,//组队战斗指挥指令
        UI_Team_Fast = 37,//快捷组队
        UI_Dialogue = 38,           //对话面板
        UI_Attribute = 39,         //属性加点
        UI_Preinstall = 40,       //加点预设
        UI_Reset = 41,      //加点重置
        UI_ExpTips = 42,       //经验tips
        UI_TipsEquipment = 43,       //装备tips
        UI_Equipment = 44,           //装备
        UI_JewelCompound = 45,       //宝石合成
        UI_Chat = 46,                //聊天
        UI_ChatSimplify = 47,        //聊天(精简界面)
        UI_ChatInput = 48,           //聊天(精简输入)
        UI_Map = 49,                 //地图界面
        UI_Horn = 50,                //广播
        UI_HornHUD = 51,             //广播
        UI_ExploreReward = 53,       //探索奖励
        UI_Loading2 = 55,         // 加载界面2
        UI_Paint = 56,               //画图界面
        UI_SeekItem = 57,             //寻找物品
        UI_SkillUpgrade = 58,         //技能升级
        UI_PathFind = 59,             // 寻路表现
        UI_ClueTaskMain = 60,           // 线索任务总览
        UI_ClueTaskWall = 61,           // 线索任务墙
        UI_ClueTaskResult = 62,         // 线索任务完成面板
        UI_ClueTaskLevelUp = 63,         // 任务等级提升
        UI_Team_Target = 64,          //组队目标
        UI_SkillPlay = 65,    //技能播放界面
        UI_ClueTaskWallTips = 66,    // 线索任务墙tips
        UI_ForceGuide = 67,               //强制引导界面
        UI_Element = 68,     //属性相克界面
        UI_Newfunction = 69,         //新功能界面

        //伙伴
        UI_Partner = 70,        //伙伴界面
        UI_PartnerReview = 71,  //伙伴预览
        UI_PartnerGet = 72,     //伙伴获得
        UI_PartnerUnlock = 73,  //伙伴解锁
        UI_PartnerLevelUp = 74, //伙伴升级
        UI_Ring = 75,

        UI_TaskComplete = 76, // 任务完成
        UI_UnForceGuide = 77, //非强制引导界面

        
        UI_Treasure = 80, //宝藏库
        UI_Treasure_Tips = 81, //宝藏库宝藏tips


        UI_NumInput = 84,//数字输入界面

        //Common Rule
        UI_Rule = 85,        //通用规则提示
        UI_MapTips = 86,    // 跳转地图的提示
        UI_LittleGmae_2048 = 87, //小游戏2048
        UI_LittleGame_OneTouchDraw = 88, // 一笔画
        UI_CountDown = 89, // 倒计时
        UI_LittleGame_Result = 90, // 小游戏结算
        UI_LittleGame_Tips = 91, // 一笔画的tips
        UI_Chapter = 92, // 章节开启
        UI_Subtitle = 93, // 任务字幕

        UI_Society,     //好友面板
        UI_Society_FriendSearch,    //好友面板(好友搜索)
        UI_Society_FriendGroupOperation,    //好友面板(自定义好友分组操作)
        UI_Society_FriendGroupCreate,    //好友面板(自定义好友分组创建)
        UI_Society_FriendGroupSetting,    //好友面板(自定义好友分组设置)
        UI_Society_GroupMasterOperation,    //好友面板(组长分组操作)
        UI_Society_GroupMemberOperation,    //好友面板(组员分组操作)
        UI_Society_GroupCreate,    //好友面板(分组创建)
        UI_Society_GroupMasterSetting,    //好友面板(组长分组设置)
        UI_Society_GroupMemberSetting,    //好友面板(组员分组设置)
        UI_Society_InviteFriends,    //好友面板(邀请好友)
        UI_Society_RecommendFriend,    //好友面板(好友推荐)
        UI_SocietyChatInput,    //好友面板(表情输入)

        UI_Onedungeons = 108,        //单人副本
        UI_Onedungeons_Tips = 109,   //单人副本提示
        UI_Onedungeons_Ranking = 110, //单人副本排行


        UI_FrightingClick = 111,//战斗点击人物菜单
        UI_Fashion = 112,

        //宠物
        UI_Modification_Name = 113, //宠物改名
        UI_Pet_AddPoint = 114, //宠物加点
        UI_Pet_BookList = 115,//宠物图鉴（废弃）
        UI_Pet_BookReview = 116,//宠物图鉴预览
        UI_Pet_Details = 117, //宠物详情
        UI_Pet_Develop = 118,    //宠物培养
        UI_Pet_Free = 119,     
        UI_Pet_Friend = 120, //宠物好感度
        UI_Pet_Get = 121, //宠物获取1
        UI_Pet_GradeUp = 122,
        UI_Pet_Automatical = 123,    //宠物自动加点
        UI_Pet_Develope_SelecItem = 124,      //宠物强化加点选择道具
        UI_Pet_Message = 125,//宠物主界面
        UI_Pet_MixPreview = 126,//宠物融合预览（废弃）
        UI_Pet_Point = 127,
        UI_Pet_Practice = 128,//宠物培养（废弃）
        UI_Pet_Practicefinish = 129,//宠物培养结束（废弃）
        UI_Pet_Remake = 130,//宠物改造 （废弃）
        UI_Pet_Seal = 131, //宠物封印（废弃）
        UI_Pet_SealUse = 132,//宠物封印道具提示（废弃）
        UI_Pet_Story = 133,//宠物故事
        UI_Pet_Tips01 = 134,//宠物属性提示   
        UI_Skill_Tips = 135,//宠物技能提示
        UI_SelectItem = 136,//宠物选择道具
        UI_SelectPet = 137,//选择宠物

        UI_CutScenePre = 138,  // CutScenePre
        UI_Fashion_SuitChange = 139,
        UI_SubmitItem = 140, // 上交物品

        UI_Multi_Ready = 141,
        UI_ServerNotify = 142, // server推送通知

        UI_Jewel_Upgrade = 143, //宝石升级
        UI_Tips_Preview = 144, //熔炼范围
        UI_Quenching_Success = 145, //粹炼成功
        UI_UseItem = 146, //装备提示

        UI_Multi_Info = 147,
        UI_Multi_PlayType = 148,
        UI_Multi_Reward = 149,
        UI_Cost = 150,    //
        UI_MainBattle_Good = 151, //战斗内道具使用
        UI_Compete = 152,     //切磋

        UI_Instance_Reuslt = 153,//副本结算
        UI_Loading3 = 154,         // 加载界面3
        UI_Pet_GetMix = 155,         // 融合宠物获得界面
        UI_TalentReset = 160, // 天赋重置
        UI_TalentExchange = 161, // 天赋炼化
        UI_Mall = 162,    //商城

        UI_PreviewMap = 169,//预览过场地图
        UI_Weather = 170,   //主界面天气面板

        UI_DailyActivites = 172, // 日常
        UI_DailyActivites_Detail = 173,//日常玩法详情
        UI_DailyActivites_Activity = 174,//活跃度详情
        UI_DailyActivites_Set = 175,//日常设置
        UI_DailyActivites_Calendar = 176,//日常日历

        UI_LifeSkill_Message = 178,//生活技能
        UI_PromptItemBox = 180, //通用道具确认框
        UI_LifeSkill_MedicineSelect = 181,//不固定配方道具选择
        UI_ChooseItem = 182,      //不固定配方道具选择
        UI_ExpUp_SelectItem = 183,        //升级经验道具选择
        UI_LifeSkill_MakeTips = 184,      //装备预览
        UI_LifeSkill_MedicineTips = 185,  //道具预览
        UI_MainBattle_SealItem = 186,    //战斗中封印宠物

        UI_AreaProtectionTips = 193, //地域防范提示
        UI_AreaProtection = 194,   //地域防范
        UI_Eraser = 195, //橡皮擦小游戏

        UI_QTEClick_1 = 196, // cutscene中点击qte
        UI_QTELongPress_1 = 197,  // cutscene中长按qte
        UI_QTESlide_1 = 198,  // cutscene中水平滑动qte
        UI_QTELongPress_2 = 199, // 采集 长按
        UI_QTESlide_2 = 200, // cutscene中垂直滑动qte

        UI_DailyActivites_Limite = 201,//日常限时活动
        UI_QTEClick_2 = 202,  // 点击开锁
        UI_QTEClick_3 = 203, // 点击qte
        UI_QTESlide_2_1 = 204, // 信封
        UI_QTESlide_2_2 = 205, // 密码锁

        UI_Pub = 209,//酒吧事件
        UI_Probe_Report = 210, //调查报告

        UI_Temple_Storage = 211, //宠物临时背包
        UI_Make_Success = 212,  //装备打造套装成功

        UI_Patrol = 213, //巡逻表现
        UI_QTEEye = 214, // 打开眼睛
        UI_QTETriangle = 215, // 打开三角形
        UI_CutSceneTop = 216, // cutsceneMask遮罩

        UI_FunctionMenu = 217,//战斗内功能界面

        UI_FavorabilityMain = 220, // 好感度主界面
        UI_FavorabilityTip = 221, // 好感度小提示

        UI_FavorabilityFete = 222, // 好感：满汉全席
        UI_FavorabilityNPCChanged = 223, // 好感：值变化
        UI_FavorabilityMenu = 224, // 好感：菜单
        UI_FavorabilityDanceList = 225, // 好感：跳舞
        UI_FavorabilityMusicList = 226, // 好感：歌曲
        UI_FavorabilityPlay = 227, // 好感：歌舞
        UI_FavorabilityFilterGifts = 228, // 好感：礼物过滤
        UI_FavorabilityPlayerChanged = 229, //  好感：度变化
        UI_FavorabilityStageRewardPreview = 230, // 好感：阶段奖励预览
        UI_FavorabilityNPCShow = 232, // 好感：npc展示
        UI_FavorabilityNPCCharacterDesc = 233, // npc性格描述
        UI_FavorabilityHealth = 234, // 好感：治疗预览
        UI_FavorabilitySendGift = 235, // 赠礼
        UI_FavorabilityThanks = 236, // 好感：感谢信
        UI_FavorabilityClue = 237, // 治疗
        UI_FavorabilityDialogue = 238, // 对话
        UI_PureHint = 239, // 提示

        UI_Pvp_Single = 240,//荣耀竞技场
        UI_Pvp_RiseInRank = 241,//荣耀竞技场段位升级奖励
        UI_Pvp_SingleLoading = 242,//荣耀竞技场加载
        UI_Pvp_SingleMatch = 243,//荣耀竞技场匹配
        UI_Pvp_SingleRank = 244,//荣耀竞技场排行榜
        UI_Pvp_SingleFinale = 245,//荣耀竞技场结算
        UI_Pvp_SingleNewSeason = 246,//荣耀竞技场新赛季
        UI_Pvp_SingleRankReward = 247,//荣耀竞技场排行奖励

        UI_Decompose = 248,       //道具分解
        UI_Reputation = 249,     //声望

        UI_Make_Preview = 251, //装备锻铸预览

        UI_Energyspar = 253,  //晶石:主面板
        UI_Advancedl_Result = 254,  //晶石:进阶成功
        UI_Energyspar_Advanced = 255,  //晶石:进阶
        UI_Energyspar_Changeover = 256,  //晶石:技能逆转
        UI_Energyspar_Refine = 257,  //晶石:炼晶
        UI_Energyspar_Resolve = 258,  //晶石:分解
        UI_Energyspar_Tips = 259,  //晶石:技能详情
        UI_Upgrade_Result = 260,  //晶石:升级成功

        UI_MainBattle_SparSkills = 262,   //战斗内晶石技能

        UI_TitleSeriesSelect = 264,   //系列称号更换
        UI_TitleSeries = 265,    //称号系列
        UI_TitleTips = 266,          //称号tips
        UI_TitleAward = 267,          //称号奖励
        UI_Fashion_Tips = 268,        //时装tips

        UI_WorldBossCampChallenge = 270, // 阵营挑战
        UI_WorldBossCampInfo = 271, // 阵营信息
        UI_WorldBossCampPreview = 272, // 阵营预览
        UI_WorldBossManualShow = 273, // boss信息展示
        UI_WorldBossSignUp = 274, // boss战报名
        UI_WorldBossChallengeRule = 275,  // 挑战规则
        UI_WorldBossSignUpFail = 276,  // 报名失败提示
        UI_WorldBossUnlockCampOrBoss = 277,   // 解锁boss,解锁阵营提示
        UI_WorldBossRank = 278, // 世界boss排行榜
        UI_WorldBossRewardList = 279,  //世界boss奖励预览

        UI_Trade = 280,         //交易行
        UI_Trade_Sell = 281,    //交易行出售(条件满足)
        UI_Trade_Buy = 282,     //交易行购买
        UI_Trade_Search = 283, //交易行搜索
        UI_Trade_SellDetail_OutOfDate = 284, //交易行摊位商品过期详情
        UI_Trade_SellDetail_Pricing = 285,  //交易行商品上架
        UI_Trade_SellDetail_Publicity = 286, //交易行摊位 公示商品详情
        UI_Trade_SellDetail_Sale = 287,      //交易行摊位 非公示商品详情
        UI_Trade_SellDetail_Bargin = 288,    //交易行摊位 议价商品详情
        UI_Trade_Record = 289,                //交易记录

        UI_Terrorist = 290,         //黑色祈祷
        UI_TerroristWeek = 291,     //黑色祈祷周常 
        UI_TerroristEnter = 292,    //黑色祈祷投票

        UI_Lotto = 293,//大地鼠彩票
        UI_Lotto_Preview = 294,//大地鼠彩票预览
        UI_Lotto_RewardGet = 295,//大地鼠彩票奖励获得
        UI_Lotto_2 = 296,//大地鼠彩票
        UI_Lotto_3 = 297,//大地鼠彩票

        UI_SceneNubber = 300,//场景橡皮擦

        UI_ApplyFamily = 310,              //申请家族
        UI_Family = 311,                   //家族界面
        UI_Family_Empowerment = 312,       //家族功勋
        UI_Family_DeedsLv_Popup = 313,     //家族功勋升级
        UI_Family_ModifyName = 314,        //修改家族名称
        UI_Family_ModifyDeclaration = 315, //修改宣言
        UI_Family_GroupSending = 316,      //群发消息
        UI_Family_Merge = 317,             //家族合并
        UI_Family_MergeApply = 318,        //家族申请合并
        UI_Family_MergeLaunch = 319,       //家族发起合并
        UI_Family_MergeConfirm = 320,      //家族合并确认
        UI_Family_MergeAgreement = 321,    //家族合并协议
        UI_Family_PromptBox_Leave = 322,   //请离家族对话框
        UI_Family_Branch = 323,            //分会管理
        UI_Family_BranchMember = 324,      //分会成员
        UI_Family_BranchMemberManage = 325,//分会成员管理
        UI_Family_BranchModifyName = 326,  //分会改名
        UI_Family_BranchMerge = 327,       //分会合并
        UI_Family_FamilyList = 328,        //家族列表
        UI_Family_Authority = 329,         //家族权限
        UI_Family_ApplyList = 330,         //申请列表
        UI_Family_BuildLvUpgrade = 331,    //建筑升级
        UI_Family_BuildSkillUpgrade = 332, //建筑技能升级
        UI_Family_Bank = 333,              //家族金库
        UI_Family_BankAward = 334,         //家族金库奖励
        UI_Family_Construct = 335,         //家族建设
        UI_Family_Construct_Tips = 336,         //家族建设提示
        UI_Family_Construct_Submit = 337, //家族建设提交道具

        UI_Compose = 341,// 合成界面
        UI_GodPet = 342,//金宠兑换 && 重生
        UI_GodPetReview = 343,//金宠预览

        UI_Uplifted = 344,      //提升界面

        UI_PartnerRune = 350, // 伙伴符文&背包&合成
        UI_PartnerRune_Tips, // 伙伴符文详情tips
        UI_Rune_Result, // 符文合成展示
        UI_Exchange_Rune = 353, //符文合成数量选择框

        UI_RewardsShow = 360, // 通用奖励展示

        UI_Team_Invite = 361,//组队邀请

        UI_Advance_Warning = 362, //进阶警告
        UI_Advance_Tips = 363,    //进阶提示
        UI_Advance_TeamTips=364,    //进阶组队确认

        UI_TaskHint = 365, // 任务提示
        UI_MapAward = 366, //地图奖励

        UI_Trade_Assign_Info = 367,  //指定提示
        UI_Trade_Record_Recovery = 368, //交易记录，取回保证金
        UI_Trade_SaleSuccess_Tip = 369, //上架成功提示
        //UI_Trade_MessageBox = 370,  //议价道具tips

        UI_Vitality = 371,   //活力
        UI_Vitality_Tips = 372,  //活力提示

        UI_BlockClickNetwork = 373,  //阻挡点击操作
        UI_MainBattle_PetDetail = 374,  //  战斗内宠物详情
        UI_BlockClickHttp = 375, // 转菊花

        UI_MagicBook = 377,         //魔力宝典
        UI_FunctionPreview = 378,   //魔力宝典 -功能预告
        UI_MagicBook_Detail = 379,  //魔力宝典-功能详情
        UI_MagicBook_Tips = 380,    //魔力宝典-章节奖励预览
        UI_MapCondition_Tips = 381,  //地图进入条件提示
        UI_ScreenLock = 382,  //屏幕锁定
        UI_FamilyMapCondition_Tips = 383,  //家族领地进入条件提示

        UI_Qa = 385,              // 调查问卷

        UI_Trade_Search_Pet_Tips = 386,   //宠物搜索tips
        UI_FadeInOut = 391,          //fadeinout

        UI_Trade_Publicity_Buy = 392, //交易行商品购买(或预购)
        UI_Trade_Sure_Tip = 393, //上架确认二次弹框提示

        UI_Tips_Pet = 394, //宠物tips

        UI_PointMall = 395, //积分商城
        UI_PointRuleTips = 396, //积分规则提示
        UI_PointGetTips = 397,  //积分获取详情提示

        UI_Cooking_Main = 402,    //料理效果
        UI_Cooking_Choose = 403,//烹饪选择
        UI_Cooking_Loading = 404,//组队烹饪等待
        UI_Cooking_Single = 405,//单人烹饪
        UI_Cooking_Multiple = 406,//多人烹饪
        UI_Cooking = 407,//烹饪
        UI_Knowledge_Cooking = 408,//烹饪图册
        UI_Knowledge_Cooking_Award = 409,//烹饪奖励
        UI_Cooking_Point = 410,   //烹饪评分
        UI_Cooking_Collect = 411,  //烹饪收集
        UI_Knowledge_RecipeCooking = 412,//食材列表
        UI_TipsAutoClose,         //换位通知(自动关闭)

        UI_Rank = 414,  //排行榜主界面
        UI_Rank_Detail01 = 415,   //排行详情
        UI_Rank_Setting = 416,  //排行设置

        UI_MenuDialogue = 417,  //弹窗对话面板

        UI_HangupFight = 418,         //挂机打怪
        UI_UnLockHangup = 419,        //解锁挂机
        UI_OperationalActivity = 420, //运营活动

        UI_Adventure = 421, //冒险手册
        UI_Adventure_LevelUp = 422, //冒险等级提升
        UI_Adventure_RewardGetTips = 423, //冒险手册悬赏令接取tips
        UI_Adventure_RewardInfo = 424, //冒险手册悬赏令详情
        UI_Adventure_RewardFinishTips = 425, //冒险手册悬赏令完成tips

        UI_ElementalCrystal = 430,    //元素水晶
        UI_ElementalCrystal_Exchange = 431,//水晶兑换
        UI_Tips_ElementalCrystal = 432,//水晶tips
        UI_ElementalCrystal_Tip = 433,

        UI_GoddessTrial = 441, //女神试炼
        UI_Goddess_Enter = 442, //女神试炼
        UI_Goddess_Select = 443, //女神试炼
        UI_Goddess_Rank = 444, //女神试炼
        UI_Goddess_Ending = 445, //女神试炼
        UI_Goddess_Result = 446, //女神试炼
        UI_Goddess_EndCollect = 447, //女神试炼
        UI_Goddess_Difficult = 448, //女神试炼
        UI_Goddness_Charac = 449, //女神试炼
        UI_Goddness_EndAward = 450,
        UI_Goddness_FristAward = 451,//首通奖励

        UI_PartnerRuneReplace = 460, // 伙伴符文替换
        UI_VendorList = 461, // 摆摊
        UI_VendorPurchase = 462, // 摆摊
        UI_ResourceGetPath = 463, // 引导
        UI_LoginLineUp = 464, // 登录排队

        UI_Knowledge_Main = 470, //旅人志主界面
        UI_Knowledge_Brave = 471, //旅人志勇者
        UI_Knowledge_Brave_Detail = 472,//旅人志勇者 详情
        UI_Knowledge_Brave_Stroy = 473, //旅人志勇者故事 
        UI_Knowledge_Annals = 474,  //旅人志魔力纪年
        UI_Knowledge_Annals_Detail = 475, //旅人志魔力纪年详情
        UI_Knowledge_Fragment = 476,   //旅人志记忆碎片
        UI_Knowledge_Fragment_Detail = 477, //旅人志记忆碎片详情
        UI_Knowledge_Gleanings = 478, //旅人志物品
        UI_Knowledge_Gleanings_Info = 479,//旅人志物品详情 
        UI_Knowledge_Award = 480,//旅人志物品奖励
        UI_Knowledge_Unlock = 481,  //旅人志未解锁提示

        UI_Pet_List = 485, //宠物改造概率界面
        UI_Chat_VoiceInput = 486,
        UI_Pet_SkillStudy = 487,//宠物技能升级界面

        UI_ClassicBossWar = 490, //经典Boss战
        UI_ClassicBossWarEnterConfirm = 491, //进入经典Boss战确认界面
        UI_ClassicBossWarResult = 492, //经典Boss战结果
        UI_Pet_Exchange = 493,//宠物兑换界面
        UI_Pet_Help = 494,//宠物援助界面
        UI_Pet_BookReview_Tip = 495,//宠物增加图鉴属性界面

        UI_Team_Apply = 500,//组队申请列表
        UI_Team_Tips = 501,//组队申请提示
        UI_ExternalNotice,

        UI_Attribute_Tips = 505,    //属性提示
        UI_MapExploreDetail = 506,  //地图探索详情
        UI_Card_Tips = 507,  //宠物品质弹窗小片
        UI_MapAreaDetail = 508, //地图区域详情
        UI_UserPartition = 510,     //用户分层
        UI_Pet_Usage = 511,     //宠物道具数量选择

        UI_Ornament = 515,          //主角饰品
        UI_Tips_Ornament = 516,     //主角饰品装备tips
        UI_Ornament_Select = 517,    //主角饰品选择界面
        UI_Ornament_Result = 518,    //主角饰品结算界面

        UI_Head = 520,    //头像外显
        UI_FavorabilityMoodDesc = 521, // 好感度心情介绍
        UI_Pet_Domestication = 522, // 坐骑驯化
        UI_HelpRule = 523, // 通用帮助规则UI
        UI_TaskFailHint = 524, // 任务失败提示

        UI_HundredPeopleArea = 525, //百人道场
        UI_HundredPeopleAreaRank = 526, //百人道场排行
        UI_HundredPeopleAreaResult = 527, //百人道场结算
        UI_HangupResult = 528, // 挂机结算
        UI_BlockClickTime = 529,  //阻挡点击操作

        UI_Setting_Tips = 530,   //宠物封印设置详情
        UI_Welfare_Main = 531,  //特权buff
        UI_SignBuy = 532,  //签到购买

        UI_Fashion_Buy = 540,       //时装购买
        UI_FirstCharge = 550,   //首充-运营活动
        UI_FirstCharge_AlyIOS = 551,   //首充-运营活动-IOS支付宝版

        UI_Trade_AppealTip = 560, //交易行申诉界面
        UI_HundreadPeoplePopReward = 561, // 百人道场 弹出式的奖励预览领取界面
        UI_Hundred_Result = 565, // 百人道场 胜负界面
        UI_PrompBox_Long = 566,//确认界面长提示(通用)
        UI_HundredPeopelAwakenTip = 567, // 百人道场buff弹出框提示

        UI_SurvivalPvp = 570,
        UI_SurvivalPvp_Rank = 571,
        UI_SurvivalPvp_Result = 572,
        UI_Trade_Box_Com = 575,     //交易行道具tips
        UI_Trade_Box_Equip = 576,   //交易行装备tips
        UI_Trade_Box_Ornament = 577, //交易行饰品tips
        UI_Trade_Box_PetEquip = 578, //交易行宠物元核tips

        UI_Awaken = 580,//旅人觉醒界面
        UI_Awaken_Tips = 581,//旅人觉醒tips

        UI_FamilyParty_Popup = 590, //家族酒会活动弹窗
        UI_FamilyParty_Submit = 591, //家族酒会上缴界面
        UI_FamilyParty_Record = 592, //家族酒会上缴记录界面
        UI_FamilyParty_StarReward = 593, //家族酒会星级奖励详情界面

        UI_PetSkill_Tips = 595, //宠物档位技能弹框特殊显示
        UI_ChatPCExpand = 596, //PC扩展窗口
        UI_OptionalGift = 600,    //自选礼包


        UI_Rewards_Result = 605,  //通用道具奖励

        UI_HangupTip = 601, // 挂机提示

        UI_BravePromotion = 602,//我要变强
        UI_PromotionDefeat = 603,//战败提示提升面板
        UI_PromotionList = 604,//我要变强子页签分支

        UI_Pet_GetCatch = 606, // 封印专用获取界面- -三个一样的界面但是会涉及到其他系统需求
        UI_Unlock_Prop = 615,
        UI_SetPassword_Prop = 616,
        UI_ChangePassword_Prop = 617,
        UI_SubmitPet = 618, // 上交宠物

        UI_Tips_Attribute = 620,   //宠物属性加成提示框

        UI_Equipment_Preview = 621, //装备预览

        UI_BatchRelease = 622, //宠物临时批量丢弃

        UI_PowerSaving = 625, //省电模式遮罩界面

        UI_FamilySale = 626, // 家族拍卖

        UI_Mine_Result = 627,//暗雷结算

        UI_Bio_PlayType = 628,//人物传记
        UI_Bio_Info = 629,//人物传记章节
        UI_Bio_Vote = 630,//人物传记投票
        UI_Bio_LevelReward = 631,//人物传记奖励展示
        UI_Bio_Result = 632,//人物传记结算

        UI_FamilySale_My = 633, // 家族拍卖 -个人参与
        UI_FamilySale_Record = 634, // 家族拍卖-历史记录
        UI_FamilySale_Detail = 635, // 家族拍卖-出价

        UI_WorldbossBattleResult = 636,//世界boss战斗结算

        UI_TutorGratitude = 637,//导师答谢弹窗

        // 家族资源战
        UI_FamilyResBattleActorList = 638,  // 叠到一起的角色，点击出现的列表
        UI_FamilyResBattleIntroduce = 639, // 家族资源站规则介绍
        UI_FamilyResBattleMain = 640, //  家族资源站入口
        UI_FamilyResBattleMap = 641, // 家族资源站地图
        UI_FamilyResBattleRank = 642,   // 家族资源站排行榜
        UI_FamilyResBattleReborn = 643, // 家族资源站 失败死亡重生
        UI_FamilyResBattleReward = 644,   // 家族资源站 奖励预览
        UI_FamilyResBattleResult = 645,  // 家族资源站 战场结果
        UI_FamilyResBattleSubmitResult = 646, // 家族资源站 资源提交结果 
        UI_FamilyResBattleTeam = 647,   // 家族资源站 组队界面
        UI_FamilyResBattleTop = 648,  //  家族资源站 战场内UI
        UI_FamilyResBattleBeginTip = 649,  // 家族资源站 战场内开战提示

        //家族Boss
        UI_FamilyBoss_Info = 650,   //牛鬼来袭，活动详情
        UI_FamilyBoss = 651, //牛鬼来袭，主界面

        UI_FamilyResBattleSignup = 652,  // 家族资源站 报名UI
        UI_FamilyResBattleMapHoldResPlayer = 653,   //  家族资源站 持矿玩家界面
        UI_FamilyResBattleCDRes = 654,  // 家族资源站 资源列表
        UI_MessageBag = 655,
        UI_FamilyResBattlePreWin = 656,   // 家族资源站 阶段性 胜利预告
        
        UI_FamilyResBattleFamilyRank = 657,   // 家族资源站排行榜
        UI_FamilyResBattleTeamMember = 658,   // 家族资源站某队伍队员展示

        //七日目标
        UI_SevenDaysTargetPopup = 659,  //七日目标跳脸弹窗界面
        UI_SevenDaysTarget = 660,
        UI_Family_RedPacket = 661,//家族红包
        UI_Family_PacketRecord = 662,//家族红包记录
        UI_Family_GivePacket = 663,//红包发送界面

        UI_FamilyWorkshop = 665,//家族委托
        UI_FamilyWorkshop_Detail = 666,//委托条目详情
        UI_FamilyWorkshop_entrust = 667,//委托发布打造
        UI_FamilyWorkshop_Tips = 668,//委托提tips

        UI_FamilyCreatures = 670,           //家族兽-信息界面
        UI_FamilyCreatures_Feed = 671,      //家族兽-喂养界面
        UI_FamilyCreatures_Get = 672,       //家族兽-领取查看界面
        UI_FamilyCreatures_SetTime = 673,   //家族兽-训练时间设置
        UI_FamilyCreatures_Train = 674,     //家族兽-训练
        UI_FamilyCreatures_Notice = 675,       //家族兽-公告
        UI_FamilyCreatures_Popup = 676,       //家族兽-训练开始结束提示暴击提示
        UI_FamilyCreatures_Reward = 677,       //家族兽-星级奖励
        UI_FamilyCreatures_Rank = 678,       //家族兽-排行奖励
        UI_FamilyCreatures_Rename = 679,       //家族兽-改名
        UI_FamilyCreatures_SetTrain = 680,       //家族兽-设置训练
        UI_FamilyCreatures_681 = 681,       //家族兽-占位
        UI_FamilyCreatures_682 = 682,       //家族兽-占位

        UI_Battle_Explain = 690,    //关卡提示
        UI_Sequence = 691,      //回合敏序
        UI_Post_Prop = 692,     //发布留言

        UI_RewardPanel = 693,   //多个道具礼包展示界面

        UI_Subtitle2 = 694,   //效果不一样的字幕

        UI_SendGift = 699,  //  赠礼界面

        UI_BattlePass = 700,//战令主界面
        UI_BattlePass_Collect = 701,//战令奖励
        UI_BattlePass_LevelBuy = 702,//战令购买等级
        UI_BattlePass_Pay = 703,//战令购买vip
        UI_BattlePass_Popup = 704,//战令开启拍脸
        UI_BattlePass_SpecialLv = 705,//战令提升

        UI_FamilyBoss_Seize = 706,  //牛鬼来袭，抢夺
        UI_FamilyBoss_Result = 707, //牛鬼来袭战斗结束
        UI_AwakenImprint = 708,//觉醒印记
        UI_Awaken_Addition = 709,//觉醒加成

        UI_FamilyBoss_RankingReward = 710, //牛鬼来袭，奖励预览

        UI_Pet_RemakeTips = 711, //改造重塑提示带单选框

        UI_Team_TalkMessage = 712, //队伍内队群发消息界面 
        UI_FamilyBoss_Ranking = 713, //牛鬼来排名
        UI_Friend_Attribute = 714,     //好友人物详情

        UI_FamilyCreatures_Result = 715, // 家族兽训练结算界面

        UI_Construct_Tips_Agri = 716,// 家族建设玩法教学界面-农业
        UI_Construct_Tips_Business = 717,// 家族建设玩法教学界面-商业
        UI_Construct_Tips_Rei = 718,// 家族建设玩法教学界面-宗教
        UI_Construct_Tips_Safe = 719,// 家族建设玩法教学界面-安全
        UI_Construct_Tips_Science = 720,// 家族建设玩法教学界面-科学
        UI_Construt_Rule = 721,// 家族建设规则提示界面


        UI_Teaming = 722, //队伍内队员有移动操作时弹出飘字提示 

        UI_Fashion_LuckyDraw = 730,//时装抽奖界面
        UI_LuckyDraw_Result = 731,//时装抽奖结算
        UI_Fashion_Point = 732, //时装领取奖励界面

        UI_Video = 735,     //录像站
        UI_Video_Friend = 736,    //录像分享好友
        UI_Video_Upload = 737,    //录相上传
        UI_VideoDetails = 738,     //录像详情

        UI_FamilyBoss_Sure, //牛鬼来袭未开启，倒计时弹框 

        UI_Property_Tips = 740,//货币栏-货币列表

        UI_TimeLimitGift_Reward = 744,    //限时礼包奖励界面
        UI_TimelimitGift = 745, //限时礼包界面(水蓝鼠宝藏)
        UI_Face_ExpRetrieve = 746,//福利-经验找回-拍脸提示

        UI_JSBattle = 747,//决胜竞技场-主界面
        UI_JSBattle_Rank = 748,//决胜竞技场-排行
        UI_JSBattle_Record = 749,//决胜竞技场-记录
        UI_JSBattle_Result = 750,//决胜竞技场-战斗结算
        UI_JSBattle_Reward = 751,//决胜竞技场-奖励领取
        UI_JSBattle_Tips = 752,//决胜竞技场-种族提示
        UI_JSBattle_Quick_Tip = 753,//决胜竞技场-扫荡成功界面

        UI_RedEnvelopeRain = 755,//红包雨记录
        UI_RedEnvelopeRain_Main = 756,//主界面红包雨
        UI_RedEnvelopeRain_Popup = 757,//红包雨点击打开界面
        UI_RedEnvelopeRain_Remind = 758,//红包雨开启倒计时界面
        UI_RedEnvelopeRain_Face = 759,//红包雨拍脸图

        UI_ChatTaskInfo = 765, // 聊天面板打开任务介绍

        UI_ReadMapTip = 766, // 阅读地图说明
        UI_MapFirstEnter = 767, // 首次进入地图
        UI_MapFilterNpc = 768, // 筛选npc

        UI_CommonCourse = 770,   //通用教程
        UI_MessageBox_Tip = 775,  //带勾选框的提示框

        UI_Construct_Continue = 776,//继续家族建设

        UI_Pet_Sale = 777, // 宠物珍贵宠物出售提示框
        UI_Bag_Clear = 780,//背包清理

        UI_Pet_MountCharge = 781, //坐骑充能界面
        UI_Pet_MountContract = 782, //坐骑契约位详情界面
        UI_Pet_MountConversion = 783, // 坐骑兑换技能书界面
        UI_Pet_MountScreem = 784, // 坐骑筛选界面
        UI_Pet_MountSelectItem = 785, //坐骑选择宠物界面
        UI_MountSkill_Tips = 786, //坐骑技能详情
        UI_Pet_MountSkillSelectItem = 787, //坐骑技能选择界面

        UI_SpecialCardRule = 789,       //特权卡规则说明tips
        UI_SpecialCardPresent = 790,    //特权卡赠送界面

        UI_ReName = 791,//改名系统
        UI_ReName_Tips = 792,//改名系统-tips

        UI_ListNumber = 793,//福利-夺宝-获奖列表

        UI_FavorabilityFirstUnlock = 800, // 好感度 首次解锁
        UI_FavorabilityNpcLikeDesc = 801,
        UI_FavorabilityNpcLikeFood = 802,

        UI_LuckyPet = 805, //金宠抽奖

        UI_Partner_Fetter_Detail = 806, //伙伴羁绊组详情

        UI_Bio_StageReward = 807, //副本，小关奖励展示
        UI_MainMenu=809, //战斗外主界面菜单

        UI_RideLotto = 810,  //骑宠抽奖主界面
        UI_RideLotto_Preview = 811,  //骑宠抽奖详情预览
        UI_RideLotto_RewardGet = 812, //骑宠抽奖获得界面

        UI_Reputation_Tips = 813,      //声望tip
        UI_Family_JoinTips = 814,//无家族提示加入家族界面

        UI_PartnerSkillGet = 815, //伙伴技能解锁展示
        UI_CurrencyRuneTips = 816, //伙伴符文碎片货币信息

        UI_Pet_SkillTips = 817, //宠物改造技能预览界面
        UI_PetAttributeTips_Left = 818, //改造技能提示左侧
        UI_PetAttributeTips_Right = 819, //改造技能提示右侧

        UI_Achievement = 820,//成就主界面
        UI_Achievement_Menu = 821,//成就菜单界面
        UI_Achievement_Reward = 822,//成就等级奖励预览界面
        UI_Achievement_RewardList = 823,//具体成就奖励预览界面
        UI_Achievement_Share = 824,//好友成就分享界面
        UI_Achievement_ShareList = 825,//成就分享选择列表
        UI_Achievement_AccessTip = 826,//成就详情弹窗

        UI_Common_Tip = 830,//通用提示(旅人志解锁，成就达成)
        UI_Skip = 831, //等待界面(转圈圈)

        UI_UndergroundArena = 835,//地下竞技场
        UI_UndergroundArena_Opponent = 836,//地下竞技场对手情报
        UI_UndergroundArena_Reward = 837,//地下竞技场参赛奖励
        UI_UndergroundArena_Vote = 838,//地下竞技场投票
        UI_UndergroundArena_Result = 839,//地下竞技场结算
        UI_UndergroundArena_Rank = 840,//地下竞技场排行榜

        UI_PetExpedition = 850, //宠物探险
        UI_PetExpedition_RewardList = 851, //宠物探险宝箱奖励列表
        UI_PetExpedition_Task = 852, //宠物探险任务详情
        UI_PetExpedition_Result = 853, //宠物探险奖励结算界面

        UI_Activity_Exchange_HeFu = 854,//合服道具兑换
        UI_Activity_Exchange = 855,//道具兑换
        
        UI_Activity_Mall = 856, //活动商城
        UI_Activity_MallBuy = 857, //活动商城购买

        UI_PetMagicCore = 860, // 宠物装备制作炼化界面
        UI_PetSelectMagicMakeItem = 861, // 宠物装备炼化选择改造图纸界面
        UI_PetMagicCore_MakePreview = 862, //宠物装备制作预览界面
        UI_PetMagicCore_ActivateResult = 863, // 宠物套装激活提示
        UI_PetMagicCore_ArtificeResult = 864, //宠物装备炼化成功界面
        UI_Tips_PetMagicCore = 865, //宠物装备详情界面
        UI_SelectPetEquip = 866, // 宠物装备选择界面

        UI_Activity_Summer =870,//夏日活动主界面
        UI_Activity_SummerSign=871,//夏日活动签到(已变成通用签到)

        UI_DescWarriorGroup = 880,  //勇者团介绍
        UI_CreateWarriorGroup = 881,    //创建勇者团
        UI_WarriorGroup = 882,      //勇者团
        UI_WarriorGroup_Invite = 883,   //勇者团邀请好友
        UI_WarriorGroup_Transfer = 884, //勇者团转移团长

        UI_ActivityShortcut =885,//活动一条龙快捷方式

        UI_Advance_Level = 886, //进阶卡级提示
        
        UI_HangupFightTriedTips = 890, // 挂机疲劳度tips
        UI_HangupFightOption = 891, // 挂机选项说明

        UI_WarriorGroup_Sign = 900, //勇者团邀请函
        UI_WarriorGroup_CreateMeeting = 901,    //勇者团会议创建
        UI_WarriorGroup_MeetingInfo = 902, //勇者团会议信息
		UI_CarrerTrans = 905,  //转职		
        UI_ActivitySavingBank = 906,//鼠王存钱活动
        UI_Pet_Exchange_Stamp = 910, //兑换宠物印记
        UI_Pet_Advanced = 911, //金宠进阶    

        UI_Activity_Topic=915,//活动面板通用
        UI_Pet_SealSetting=916,  //宠物封印设置
        
        UI_KingPetReview=920,//金宠预览
        UI_KingPet=921,//金宠兑换

        UI_LadderPvp = 925,//天梯主界面
        UI_LadderPvp_LevelReward = 926,//天梯升级奖励
        UI_LadderPvp_Match = 927,//天梯匹配
        UI_LadderPvp_Rank = 928,//天梯排行
        UI_LadderPvp_RankReward = 929,//天梯排行奖励
        UI_LadderPvp_TaskReward = 930,//天梯任务奖励
        UI_LadderPvp_Laoding = 931,//天梯加载
        UI_LadderPvp_FightResult = 932,//天梯结算
        UI_LadderPvp_NewSeason = 933,//天梯新赛季

        UI_Transfiguration_Unlock_Result = 935,    //全系研究解锁技能
        UI_Transfiguration_Study =936,   //种族研究变身
        UI_Transfiguration_BookList = 937,  //变身卡图鉴
        UI_Transfiguration_Result = 938,   // 激活变身卡
        UI_Transfiguration_SkillTips = 939,    //变身卡技能查看

        UI_TownTaskMain = 940, // 城镇任务主界面
        UI_TownTaskDetail = 941, // 城镇任务某个城镇的任务
        UI_TownTaskReward = 942, // 城镇任务奖励

        UI_Family_Sure = 943,// 家族防诈骗提示
        UI_ChangeSchemeName = 945, // 方案更名
        UI_TownTaskSubmitResult = 946, // 城镇任务提交结果
        
        UI_BackActivity = 950,//回归活动主界面
        UI_BackWalfare = 951,//回归福利界面
        
        UI_ExchangePointChange = 955, //伙伴属性点数转化
        UI_ExchangePointReset = 956, //伙伴属性点

        UI_TrialGateMain = 960,//试炼之门活动主界面
        UI_TrialSkillDeploy = 961,//试炼技能配置详情界面
        UI_TrialTeamDeploy = 962,//试炼队伍配置界面
        UI_TrialRank = 963,       //试炼排行界面
        UI_TrialBattleConfirm = 964, //试炼备战战斗确认界面
        UI_TrialBadgeTips = 965, //徽章详情弹窗
        UI_TrialResult = 966, //试炼结算界面
        UI_TrialGate_StageTip = 967, //战斗内试炼阶段

        UI_PKCompetition = 970,//PK大赛报名
        UI_PKCompetitionCreate = 971,//P创建战队
        UI_PKCompetition_TeamWords = 972,//战队宣言界面
        UI_PvP_Result = 973,//pk战斗结果界面

        UI_Battle_Decide =976, //胜负判定
        UI_Activity_FestivalTask = 977,//活动——节日任务引导
        UI_Rewards_GetNew=978,//道具——特效礼包
        UI_Plan = 990,  //方案切换

        UI_Activity2048 = 995, //活动2048主界面
        UI_Activity2048Result = 996, //活动2048结算界面

        UI_Family_Funds = 999, // 家族奖金预览界面
        
        UI_Activity_Nien = 1000, //年兽活动
        UI_Activity_Nien_Challenge = 1001, //年兽挑战
        UI_Activity_Nien_ChallengeAward = 1002, //年兽挑战预览
        UI_Activity_Nien_Rank = 1003, //年兽挑战预览

		UI_MerchantFleet=1010,//法兰商队主界面
        UI_MerchantFleet_FamilyHelp= 1011,//法兰商队-家族求助
        UI_MerchantFleet_GradeAward=1012,//法兰商队-等级奖励
        UI_MerchantFleet_Settlement =1013,//法兰商队-结算		

		UI_Pet_LevelDown = 1020, //宠物等级回退预览界面    

        UI_MagicChange = 1030,//魔法变形

        UI_Pet_DemonSkill_Tips = 1035,//魔魂技能弹框
        UI_Pet_Demon_Bag = 1036,//魔魂背包
        UI_Pet_DemonReform = 1037,//魔魂改造激活
        UI_Pet_Demon = 1038,//专属魔魂
        UI_Pet_DemonPet = 1039,//魔魂宠物选择弹框
        UI_Pet_DemonPreview = 1040,//魔魂技能总描述
        UI_Pet_DemonSelect = 1041,//魔魂魂珠选择弹框
        UI_Pet_Demon_SkillPreview = 1042,//魔魂技能预览
        UI_Pet_DemonUpgrade = 1043,//魔珠升级弹框
        UI_Pet_DemonSkill_Refresh = 1044,//魔珠技能刷新界面
        UI_Pet_Demon_Detail = 1045, //魂珠卸下升级界面
        UI_Pet_DemonSuccess = 1046,// 解锁 升级tips

        UI_EquipSlot_Upgrade = 1050, //装备部位升级
        UI_EquipSlot_Upgrade_Items = 1051, //装备部位升级材料选择
        UI_EquipSlot_Upgrade_SkillPreview = 1052, //装备部位升级技能预览
        UI_Equipment_Remake_Preview = 1053, //装备重铸预览
        UI_Equipment_Inlay_GemTip = 1054, //装备镶嵌 宝石达到等级属性预览
        UI_Pet_FirstStart=1055,//首发宠物选择


        UI_BossTower_Feature = 1056,       //Boss特性界面
        UI_BossTower_FightMain = 1057,      //Boss战主界面
        UI_BossTower_Stages = 1058,        //Boss阶段预览界面
        UI_BossTower_QualifierRank = 1059,  //资格赛排行界面
        UI_BossTower_BossFightRank = 1060, //Boss战排行界面
        UI_BossTower_Result = 1061,        //Boss资格挑战结算界面
        UI_BossTower_EnterFightVote = 1062,//Boss资格进战投票界面
        UI_BossTower_QualifierMain = 1063, //资格赛主界面
        UI_BossTower_QualifierExplain = 1064,//资格赛挑战名额说明

        UI_PetDomesticate = 1070,   //宠物驯养
        UI_PetDomesticateTask = 1071,   //宠物驯养任务
        UI_PetDomesticateResult = 1072,   //宠物驯养任务

        UI_Pet_MountIntensify = 1075,//宠物坐骑契约强化界面
		UI_zhuanshuSkill_Tips=1080,  //宠物图鉴技能tip		
        UI_Pet_Appearance = 1081, // 宠物外观
        UI_Pet_FashionApparel = 1082, //宠物外观穿戴页面


        UI_Blessing = 1085,
        UI_Blessing_Info = 1086,
        UI_Blessing_Result = 1087,

        UI_Transfiguration_Tips = 1090,  //变身添加方案确认
    }

    //max Line:2148(新加配置的时候 更新下 方便下个人快速定位到位置添加界面)

    public static class UIConfig
    {
        public static UIConfigData GetConfData(int id)
        {
            UIConfigData data = null;
            datas.TryGetValue(id, out data);
            return data;
        }

        public static UIConfigData GetConfData(EUIID id)
        {
            UIConfigData data = null;
            datas.TryGetValue((int)id, out data);
            return data;
        }

        private static readonly Dictionary<int, UIConfigData> datas = new Dictionary<int, UIConfigData>()
        {
            {(int)EUIID.UI_Loading, new UIConfigData(
                    typeof(UI_Loading),
                    "UI/Loading/UI_Loading.prefab",
                    EUIOption.eHideMainCamera | EUIOption.eIgnoreStack | EUIOption.eIgnoreClear | EUIOption.eReduceFrameRate | EUIOption.eHideBeforeUI, 10000)},

            {(int)EUIID.UI_Login, new UIConfigData(
                    typeof(UI_Login),
                    "UI/UI_Login.prefab",
                    EUIOption.eHideBeforeUI)},

            {(int)EUIID.UI_Server, new UIConfigData(
                    typeof(UI_ServerList),
                    "UI/UI_Server.prefab",
                    EUIOption.eHideBeforeUI)},
            {(int)EUIID.UI_CreateCharacter, new UIConfigData(
                    typeof(UI_CreateCharacter),
                    "UI/UI_Create_Role.prefab",
                    EUIOption.eHideBeforeUI)},

            {(int)EUIID.UI_Hint, new UIConfigData(
                    typeof(UI_Hint),
                    "UI/Common/UI_Hint.prefab",
                    EUIOption.eIgnoreClear | EUIOption.eIgnoreStack, 9450)},

            {(int)EUIID.UI_ChoosePet, new UIConfigData(
                    typeof(UI_ChoosePet),
                    "UI/UI_Choose_Pet.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate|EUIOption.eReduceMainCameraQuality)},

            {(int)EUIID.UI_Career, new UIConfigData(
                    typeof(UI_Career),
                    "UI/Attribute/UI_Profession.prefab",
                    EUIOption.eHideBeforeUI | EUIOption.eHideMainCamera | EUIOption.eReduceFrameRate)},

            {(int)EUIID.UI_MainInterface, new UIConfigData(
                    typeof(UI_MainInterface),
                    "UI/MainInterface/UI_MainInterface.prefab",
                    EUIOption.eHideBeforeUI)},

            {(int)EUIID.UI_Joystick, new UIConfigData(
                    typeof(UI_Joystick),
                    "UI/MainInterface/UI_Joystick.prefab"
                    )},

            {(int)EUIID.UI_Menu, new UIConfigData(
                    typeof(UI_Menu),
                    "UI/MainInterface/UI_Menu.prefab"
                    )},

            {(int)EUIID.UI_UseItem, new UIConfigData(
                    typeof(UI_UseItem),
                    "UI/MainInterface/UI_UseItem.prefab"
                    )},

            {(int)EUIID.UI_MainBattle, new UIConfigData(
                    typeof(UI_MainBattle),
                    "UI/MainBattle/UI_MainBattle.prefab",
                   EUIOption.eIgnoreStack, -10)},

            {(int)EUIID.UI_MainBattle_Pet, new UIConfigData(
                    typeof(UI_MainBattle_Pet),
                    "UI/MainBattle/UI_MainBattle_Pet.prefab"
                    )},

            {(int)EUIID.UI_MainBattle_Skills, new UIConfigData(
                    typeof(UI_MainBattle_Skills),
                    "UI/MainBattle/UI_MainBattle_Skills.prefab"
                    )},

            {(int)EUIID.HUD, new UIConfigData(
                    typeof(HUD),
                    "UI/HUD.prefab",
                    EUIOption.eIgnoreStack|EUIOption.eInvalid,-10)},

            {(int)EUIID.UI_Setting, new UIConfigData(
                    typeof(UI_Setting),
                    "UI/UI_Setting.prefab",
                    EUIOption.eHideBeforeUI)},

            {(int)EUIID.UI_Reconnection, new UIConfigData(
                    typeof(UI_Reconnection),
                    "UI/UI_Reconnection.prefab",
                    EUIOption.eHideBeforeUI | EUIOption.eIgnoreStack, 9500)}, //重连在除loading所有UI之上,在UI_ServerNotify之下
            {(int)EUIID.UI_ServerNotify, new UIConfigData(
                    typeof(UI_ServerNotify),
                    "UI/Common/UI_Tips01.prefab",
                    EUIOption.eHideBeforeUI | EUIOption.eIgnoreStack, 9600)}, //在所有UI之上

            {(int)EUIID.UI_PromptBox, new UIConfigData(
                    typeof(UI_PromptBox),
                    "UI/Common/UI_Tips.prefab",
                    EUIOption.eIgnoreStack,9455)},
            {(int)EUIID.UI_HangupTip, new UIConfigData(
                    typeof(UI_HangupTip),
                    "UI/Common/UI_Tips.prefab",
                    EUIOption.eIgnoreStack,9455)},
            {(int)EUIID.UI_Cost, new UIConfigData(
                    typeof(UI_Cost),
                    "UI/UI_Cost.prefab",
                    EUIOption.eInvalid)},

            {(int)EUIID.UI_Bag, new UIConfigData(
                    typeof(UI_Bag),
                    "UI/Bag/UI_Bag.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate|EUIOption.eReduceMainCameraQuality)},

            {(int)EUIID.UI_Prop_Message, new UIConfigData(
                    typeof(UI_Prop_Message),
                    "UI/Bag/UI_Prop_Message.prefab"
                    )},

            {(int)EUIID.UI_Sale_Prop, new UIConfigData(
                    typeof(UI_Sale_Prop),
                    "UI/Bag/UI_Sale_Prop.prefab")},

            {(int)EUIID.UI_TemporaryBag, new UIConfigData(
                    typeof(UI_TemporaryBag),
                    "UI/Bag/UI_Temple_Bag.prefab",
                    EUIOption.eHideBeforeUI)},

            {(int)EUIID.UI_SafeBox, new UIConfigData(
                    typeof(UI_SafeBox),
                    "UI/Bag/UI_SafeBox.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate|EUIOption.eReduceMainCameraQuality)},

            {(int)EUIID.UI_BatchUse_Box, new UIConfigData(
                    typeof(UI_BatchUse_Box),
                    "UI/Bag/UI_BatchUse_Box.prefab")},

            {(int)EUIID.UI_Exchange_Coin, new UIConfigData(
                    typeof(UI_Exchange_Coin),
                    "UI/Bag/UI_Exchange_Coin.prefab")},

            {(int)EUIID.UI_Message_Box, new UIConfigData(
                    typeof(UI_Message_Box),
                    "UI/Bag/UI_Message_Box.prefab",
                    EUIOption.eInvalid)},

            {(int)EUIID.UI_PerForm, new UIConfigData(
                    typeof(UI_PerForm),
                    "UI/Common/UI_PerForm.prefab",
                    EUIOption.eIgnoreClear | EUIOption.eIgnoreStack,30)},

            {(int)EUIID.UI_TownTaskMain, new UIConfigData(
                typeof(UI_TownTaskMain),
                "UI/TownTask/UI_TownTask.prefab",
                EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eReduceMainCameraQuality)},
            {(int)EUIID.UI_TownTaskDetail, new UIConfigData(
                typeof(UI_TownTaskDetail),
                "UI/TownTask/UI_TownTask_Board.prefab",
                EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eReduceMainCameraQuality)},
            {(int)EUIID.UI_TownTaskReward, new UIConfigData(
                typeof(UI_TownTaskReward),
                "UI/TownTask/UI_TownTask_Reward.prefab"/*,
                EUIOption.eHideBeforeUI*/)},

            {(int)EUIID.UI_TaskList, new UIConfigData(
                    typeof(UI_TaskList),
                    "UI/Task/UI_TaskList.prefab",
                    EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eReduceMainCameraQuality)},
            {(int)EUIID.UI_ChatTaskInfo, new UIConfigData(
                typeof(UI_ChatTaskInfo),
                "UI/Attribute/UI_ChatTaskInfo.prefab")},

            {(int)EUIID.UI_TaskHint, new UIConfigData(
                    typeof(UI_TaskHint),
                    "UI/Task/UI_TaskHint.prefab")},
            {(int)EUIID.UI_TaskFailHint, new UIConfigData(
                    typeof(UI_TaskFailHint),
                    "UI/Task/UI_TaskHint.prefab")},

            {(int)EUIID.UI_TaskComplete, new UIConfigData(
                    typeof(UI_TaskComplete),
                    "UI/Task/UI_Task_Complete.prefab",
                    EUIOption.eReduceFrameRate | EUIOption.eIgnoreStack, 31)},

            {(int)EUIID.UI_TaskSelectBox, new UIConfigData(
                    typeof(UI_TaskSelectBox),
                    "UI/Task/UI_Task_SelectBox.prefab",
                    EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate)},

            {(int)EUIID.UI_TaskNormalResult, new UIConfigData(
                    typeof(UI_TaskNormalResult),
                    "UI/Task/UI_Task_Normal_Result.prefab",
                    EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eReduceMainCameraQuality)},

            {(int)EUIID.UI_RewardsShow, new UIConfigData(
                    typeof(UI_RewardsShow),
                    "UI/Task/UI_Task_Normal_Result.prefab",
                    EUIOption.eHideBeforeUI)},

            {(int)EUIID.UI_TaskSpecialResult, new UIConfigData(
                    typeof(UI_TaskSpecialResult),
                    "UI/Task/UI_Task_Special_Result.prefab",
                    EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eReduceMainCameraQuality)},

            {(int)EUIID.UI_SharedTaskList, new UIConfigData(
                    typeof(UI_TaskShareList),
                    "UI/Task/UI_TaskShareList.prefab",
                    EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eReduceMainCameraQuality)},

            {(int)EUIID.UI_Team_Member, new UIConfigData(
                    typeof(UI_Team_Member),
                    "UI/Team/UI_TeamList.prefab",
                    EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eReduceMainCameraQuality|EUIOption.eHideMainCamera)},

            {(int)EUIID.UI_Team_Player, new UIConfigData(
                    typeof(UI_Team_Player),
                    "UI/Team/UI_Team_Player.prefab"
                    )},

            {(int)EUIID.UI_Team_WarCommand, new UIConfigData(
                    typeof(UI_Team_OrderCompiler),
                    "UI/Team/UI_Order_Compile.prefab"
                    )},

            {(int)EUIID.UI_Team_Fast, new UIConfigData(
                    typeof(UI_Team_Fast),
                    "UI/Team/UI_Team_Fast.prefab"
                    )},

            {(int)EUIID.UI_Team_Target, new UIConfigData(
                    typeof(UI_Team_Target),
                    "UI/Team/UI_Team_Target.prefab"
                    )},

            {(int)EUIID.UI_NPC, new UIConfigData(
                    typeof(UI_NPC),
                    "UI/UI_NPC.prefab",
                    EUIOption.eIgnoreStack | EUIOption.eHideBeforeUI, 9455)},

            {(int)EUIID.UI_Dialogue, new UIConfigData(
                    typeof(UI_Dialogue),
                    "UI/UI_Dialogue.prefab",
                    EUIOption.eIgnoreStack | EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate, 9450)},

            {(int)EUIID.UI_TipsEquipment, new UIConfigData(
                    typeof(UI_Tips_Equipment),
                    "UI/Equip/UI_Tips_Equipment.prefab"
                    )},

            {(int)EUIID.UI_Equipment, new UIConfigData(
                    typeof(UI_Equipment),
                    "UI/Equip/UI_Equip.prefab",
                    EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eReduceMainCameraQuality)},

            { (int)EUIID.UI_Tips_Preview, new UIConfigData(
                    typeof(UI_Tips_Preview),
                    "UI/Equip/UI_Tips_Preview.prefab",
                    EUIOption.eInvalid)},

            { (int)EUIID.UI_Quenching_Success, new UIConfigData(
                    typeof(UI_Quenching_Success),
                    "UI/Equip/UI_Quenching_Success.prefab",
                    EUIOption.eInvalid)},

            {(int)EUIID.UI_JewelCompound, new UIConfigData(
                    typeof(UI_JewelCompound),
                    "UI/Equip/UI_Gem_Compound.prefab",
                    EUIOption.eInvalid)},

            { (int)EUIID.UI_Jewel_Upgrade, new UIConfigData(
                    typeof(UI_Jewel_Upgrade),
                    "UI/Equip/UI_Gem_Upgrade1.prefab",
                    EUIOption.eInvalid)},

            {(int)EUIID.UI_Buff, new UIConfigData(
                    typeof(UI_Buff),
                    "UI/MainBattle/UI_Buff.prefab",
                    EUIOption.eReduceFrameRate)},

            {(int)EUIID.UI_Attribute, new UIConfigData(
                    typeof(UI_Attribute),
                    "UI/Attribute/UI_AttributeNew.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate|EUIOption.eReduceMainCameraQuality)},

            {(int)EUIID.UI_Preinstall, new UIConfigData(
                    typeof(UI_Preinstall),
                    "UI/Attribute/UI_Preinstall.prefab",
                    EUIOption.eReduceFrameRate)},

            {(int)EUIID.UI_Reset, new UIConfigData(
                    typeof(UI_Reset),
                    "UI/Attribute/UI_Reset.prefab",
                    EUIOption.eReduceFrameRate)},

            {(int)EUIID.UI_ExpTips, new UIConfigData(
                    typeof(UI_ExpTips),
                    "UI/Attribute/UI_ExpTips.prefab",
                    EUIOption.eReduceFrameRate)},

            {(int)EUIID.UI_Chat, new UIConfigData(
                    typeof(UI_Chat),
                    "UI/Chat/UI_Chat.prefab",
                    EUIOption.eReduceFrameRate)},

            {(int)EUIID.UI_ChatSimplify, new UIConfigData(
                    typeof(UI_ChatSimplify),
                    "UI/Chat/UI_ChatSimplify.prefab"
                    )},

            {(int)EUIID.UI_ChatInput, new UIConfigData(
                    typeof(UI_ChatInput),
                    "UI/Chat/UI_ChatInput.prefab",
                    EUIOption.eReduceFrameRate)},

            {(int)EUIID.UI_Map, new UIConfigData(
                    typeof(UI_Map),
                    "UI/Map/UI_Map.prefab",
                    EUIOption.eHideMainCamera|EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate|EUIOption.eReduceMainCameraQuality)},

            {(int)EUIID.UI_MapFirstEnter, new UIConfigData(
                typeof(UI_MapFirstEnter),
                "UI/Map/UI_MapNew.prefab")},

            {(int)EUIID.UI_ReadMapTip, new UIConfigData(
                typeof(UI_ReadMapTip),
                "UI/Map/UI_MapMail.prefab",
                EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eReduceMainCameraQuality)},

            {(int)EUIID.UI_MapFilterNpc, new UIConfigData(
                typeof(UI_MapFilterNpc),
                "UI/Map/UI_Map_Npc.prefab")},

            {(int)EUIID.UI_Horn, new UIConfigData(
                    typeof(UI_Horn),
                    "UI/Chat/UI_Horn.prefab",
                    EUIOption.eHideBeforeUI)},

            {(int)EUIID.UI_HornHUD, new UIConfigData(
                    typeof(UI_HornHUD),
                    "UI/Chat/UI_HornHUD.prefab",
                    EUIOption.eIgnoreStack, 99)},

            {(int)EUIID.UI_ExploreReward, new UIConfigData(
                    typeof(UI_ExploreReward),
                    "UI/Task/UI_ExploreReward.prefab")},

            {(int)EUIID.UI_Loading2, new UIConfigData(
                    typeof(UI_Loading2),
                    "UI/Loading/UI_Loading2.prefab",
                    EUIOption.eIgnoreStack | EUIOption.eIgnoreClear | EUIOption.eReduceFrameRate | EUIOption.eHideBeforeUI, 100)},

            {(int)EUIID.UI_Loading3, new UIConfigData(
                    typeof(UI_Loading3),
                    "UI/Loading/UI_Loading3.prefab",
                    EUIOption.eIgnoreStack | EUIOption.eIgnoreClear | EUIOption.eReduceFrameRate | EUIOption.eHideBeforeUI, 100)},

            {(int)EUIID.UI_Paint, new UIConfigData(
                    typeof(UI_PaintBoard),
                    "UI/UI_PaintBoard.prefab",
                    EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eReduceMainCameraQuality, 100)},

            {(int)EUIID.UI_SeekItem, new UIConfigData(
                    typeof(UI_SeekItem),
                    "UI/UI_SeekItem.prefab",
                    EUIOption.eHideBeforeUI)},

            {(int)EUIID.UI_SkillUpgrade, new UIConfigData(
                    typeof(UI_SkillNew),
                    "UI/Skill/UI_Skill_1.prefab",
                    EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eReduceMainCameraQuality)},

            {(int)EUIID.UI_PathFind, new UIConfigData(
                    typeof(UI_PathFind),
                    "UI/UI_PathFind.prefab",
                    EUIOption.eInvalid, -1)},//EUIOption.eIgnoreStack

            {(int)EUIID.UI_ClueTaskMain, new UIConfigData(
                    typeof(UI_ClueTaskMain),
                    "UI/Task/Clue/UI_ClueTaskMain.prefab",
                    EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eReduceMainCameraQuality)},

            {(int)EUIID.UI_ClueTaskWall, new UIConfigData(
                    typeof(UI_ClueTaskWall),
                    "UI/Task/Clue/UI_ClueTaskWall.prefab",
                    EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eReduceMainCameraQuality)},

            {(int)EUIID.UI_ClueTaskResult, new UIConfigData(
                    typeof(UI_ClueTaskResult),
                    "UI/Task/Clue/UI_ClueTaskResult.prefab",
                    EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eReduceMainCameraQuality)},

            {(int)EUIID.UI_ClueTaskLevelUp, new UIConfigData(
                    typeof(UI_ClueTaskLevelUp),
                    "UI/Task/Clue/UI_ClueTaskLevelUp.prefab",
                    EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eReduceMainCameraQuality)},

            {(int)EUIID.UI_ClueTaskWallTips, new UIConfigData(
                    typeof(UI_ClueTaskWallTips),
                    "UI/Task/Clue/UI_ClueTaskWallTips.prefab",
                    EUIOption.eReduceFrameRate)},

            {(int)EUIID.UI_SkillPlay, new UIConfigData(
                    typeof(UI_SkillPlay),
                    "UI/Attribute/UI_SkillPlay.prefab",
                    EUIOption.eHideBeforeUI| EUIOption.eReduceFrameRate | EUIOption.eReduceMainCameraQuality)},

            {(int)EUIID.UI_ForceGuide, new UIConfigData(
                    typeof(UI_ForceGuide),
                    "UI/Guide/UI_GuideArrow.prefab",
                    EUIOption.eIgnoreClear|EUIOption.eIgnoreStack, 9502)},

            {(int)EUIID.UI_UnForceGuide, new UIConfigData(
                    typeof(UI_UnForceGuide),
                    "UI/Guide/UI_GuideWeak.prefab",
                    EUIOption.eIgnoreClear|EUIOption.eIgnoreStack, 9501)},

            {(int)EUIID.UI_Newfunction, new UIConfigData(
                    typeof(UI_Newfunction),
                    "UI/Newfunction/UI_Newfunction.prefab",
                    EUIOption.eIgnoreClear|EUIOption.eIgnoreStack, 95)},

            {(int)EUIID.UI_Partner, new UIConfigData(
                    typeof(UI_Partner),
                    "UI/Partner/UI_Partner.prefab",
                    EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eReduceMainCameraQuality)},

            {(int)EUIID.UI_PartnerReview, new UIConfigData(
                    typeof(UI_Partner_Review),
                    "UI/Partner/UI_PartnerReview.prefab",
                    EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eHideMainCamera)},

            {(int)EUIID.UI_PartnerGet, new UIConfigData(
                    typeof(UI_Partner_Get),
                    "UI/Partner/UI_PartnerGet.prefab",
                    EUIOption.eHideBeforeUI,100)},

            {(int)EUIID.UI_PartnerUnlock, new UIConfigData(
                    typeof(UI_Partner_Unlock),
                    "UI/Partner/UI_PartnerUnlock.prefab",
                    EUIOption.eInvalid)},

            {(int)EUIID.UI_PartnerLevelUp, new UIConfigData(
                    typeof(UI_Partner_LevelUp),
                    "UI/Partner/UI_PartnerLvUp.prefab",
                    EUIOption.eInvalid)},

            {(int)EUIID.UI_Ring, new UIConfigData(
                    typeof(UI_Ring),
                    "UI/UI_Ring.prefab",
                    EUIOption.eHideBeforeUI)},

            {(int)EUIID.UI_Treasure, new UIConfigData(
                    typeof(UI_Treasure),
                    "UI/Treasure/UI_Treasure.prefab",
                    EUIOption.eHideBeforeUI | EUIOption.eHideMainCamera)},

            {(int)EUIID.UI_Treasure_Tips, new UIConfigData(
                    typeof(UI_Treasure_Tips),
                    "UI/Treasure/UI_Treasure_Tips.prefab",
                    EUIOption.eInvalid)},

            {(int)EUIID.UI_NumInput, new UIConfigData(
                    typeof(UI_NumInput),
                    "UI/Common/UI_Num_Input.prefab",
                    EUIOption.eInvalid)},

            {(int)EUIID.UI_Rule, new UIConfigData(
                    typeof(UI_Rule),
                    "UI/UI_Rule.prefab",
                    EUIOption.eInvalid)},

            {(int)EUIID.UI_Element, new UIConfigData(
                    typeof(UI_Element),
                    "UI/MainBattle/UI_Element.prefab",
                    EUIOption.eReduceFrameRate)},
            {(int)EUIID.UI_MapTips, new UIConfigData(
                    typeof(UI_MapTips),
                    "UI/Common/UI_MapTips.prefab",
                    EUIOption.eIgnoreClear | EUIOption.eIgnoreStack, 80)},
            {(int)EUIID.UI_LittleGmae_2048, new UIConfigData(
                    typeof(UI_LittleGame2048),
                    "UI/LittleGame/UI_Littlegame_2048.prefab",
                    EUIOption.eHideBeforeUI)},
            {(int)EUIID.UI_LittleGame_OneTouchDraw, new UIConfigData(
                    typeof(UI_LittleGame_OneTouchDraw),
                    "UI/LittleGame/UI_LittleGame_OneTouchDraw.prefab",
                    EUIOption.eHideBeforeUI | EUIOption.eReduceMainCameraQuality | EUIOption.eReduceFrameRate)},
            {(int)EUIID.UI_CountDown, new UIConfigData(
                    typeof(UI_CountDown),
                    "UI/Common/UI_CountDown.prefab",
                    EUIOption.eInvalid)},
            {(int)EUIID.UI_LittleGame_Result, new UIConfigData(
                    typeof(UI_LittleGame_Result),
                    "UI/LittleGame/UI_LittleGame_Result.prefab",
                    EUIOption.eIgnoreStack, 1000)},
            {(int)EUIID.UI_LittleGame_Tips, new UIConfigData(
                    typeof(UI_LittleGame_Tips),
                    "UI/LittleGame/UI_Littlegame_Tips.prefab",
                    EUIOption.eIgnoreStack, 1000)},
            {(int)EUIID.UI_Chapter, new UIConfigData(
                    typeof(UI_Chapter),
                    "UI/Chapter/UI_Chapter.prefab",
                    EUIOption.eHideBeforeUI | EUIOption.eIgnoreStack | EUIOption.eReduceFrameRate | EUIOption.eReduceMainCameraQuality, 9200)},
            {(int)EUIID.UI_Subtitle, new UIConfigData(
                    typeof(UI_Subtitle),
                    "UI/Subtitle/UI_Subtitle.prefab",
                    EUIOption.eHideBeforeUI | EUIOption.eIgnoreStack | EUIOption.eReduceFrameRate, 9250)},
            {(int)EUIID.UI_CutSceneTop, new UIConfigData(
                    typeof(UI_CutSceneTop),
                    "UI/Cutscene/UI_CutSceneTop.prefab",
                    EUIOption.eHideBeforeUI)},
             {(int)EUIID.UI_Society, new UIConfigData(
                    typeof(UI_Society),
                    "UI/Society/UI_Society.prefab",
                    EUIOption.eHideBeforeUI)},

             {(int)EUIID.UI_SocietyChatInput, new UIConfigData(
                    typeof(UI_SocietyChatInput),
                    "UI/Chat/UI_ChatInput.prefab",
                    EUIOption.eReduceFrameRate)},

             {(int)EUIID.UI_Society_FriendSearch, new UIConfigData(
                    typeof(UI_Society_FriendSearch),
                    "UI/Society/UI_FriendSearch.prefab",
                    EUIOption.eInvalid)},

             {(int)EUIID.UI_Society_FriendGroupCreate, new UIConfigData(
                    typeof(UI_Society_FriendGroupCreate),
                    "UI/Society/UI_FriendGroupCreate.prefab",
                    EUIOption.eInvalid)},

             {(int)EUIID.UI_Society_FriendGroupOperation, new UIConfigData(
                    typeof(UI_Scciety_FriendGroupOperation),
                    "UI/Society/UI_FriendGroupOperation.prefab",
                    EUIOption.eInvalid)},

             {(int)EUIID.UI_Society_FriendGroupSetting, new UIConfigData(
                    typeof(UI_Society_FriendGroupSetting),
                    "UI/Society/UI_FriendGroupSetting.prefab",
                    EUIOption.eInvalid)},

             {(int)EUIID.UI_Society_GroupCreate, new UIConfigData(
                    typeof(UI_Society_GroupCreate),
                    "UI/Society/UI_GroupCreate.prefab",
                    EUIOption.eInvalid)},

             {(int)EUIID.UI_Society_GroupMasterOperation, new UIConfigData(
                    typeof(UI_Society_GroupMasterOperation),
                    "UI/Society/UI_GroupMasterOperation.prefab",
                    EUIOption.eInvalid)},

             {(int)EUIID.UI_Society_GroupMemberOperation, new UIConfigData(
                    typeof(UI_Society_GroupMemberOperation),
                    "UI/Society/UI_GroupMemberOperation.prefab",
                    EUIOption.eInvalid)},

             {(int)EUIID.UI_Society_GroupMasterSetting, new UIConfigData(
                    typeof(UI_Society_GroupMasterSetting),
                    "UI/Society/UI_GroupMasterSetting.prefab",
                    EUIOption.eInvalid)},

             {(int)EUIID.UI_Society_GroupMemberSetting, new UIConfigData(
                    typeof(UI_Society_GroupMemberSetting),
                    "UI/Society/UI_GroupMemberSetting.prefab",
                    EUIOption.eInvalid)},

              {(int)EUIID.UI_Society_InviteFriends, new UIConfigData(
                    typeof(UI_Society_InviteFriends),
                    "UI/Society/UI_InviteFriends.prefab",
                    EUIOption.eInvalid)},

             {(int)EUIID.UI_Onedungeons, new UIConfigData(
                    typeof(UI_Onedungeons),
                    "UI/Onedungeons/UI_Onedungeons.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate|EUIOption.eReduceMainCameraQuality)},

             {(int)EUIID.UI_Onedungeons_Tips, new UIConfigData(
                    typeof(UI_Onedungeons_Tips),
                    "UI/Onedungeons/UI_Onedungeons_Tips.prefab",
                    EUIOption.eReduceFrameRate)},

             {(int)EUIID.UI_Onedungeons_Ranking, new UIConfigData(
                    typeof(UI_Onedungeons_Ranking),
                    "UI/Onedungeons/UI_Onedungeons_Ranking.prefab",
                    EUIOption.eReduceFrameRate)},

             {(int)EUIID.UI_FrightingClick, new UIConfigData(
                    typeof(UI_Team_FrightingClick),
                    "UI/Team/UI_Frighting_Click.prefab",
                    EUIOption.eInvalid)},
             {(int)EUIID.UI_Fashion, new UIConfigData(
                    typeof(UI_Fashion),
                    "UI/Fashion/UI_Fashion.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate|EUIOption.eHideMainCamera)},
             {(int)EUIID.UI_CutScenePre, new UIConfigData(
                    typeof(UI_CutScenePre),
                    "UI/Cutscene/UI_CutScenePre.prefab",
                    EUIOption.eIgnoreStack | EUIOption.eHideBeforeUI, 90)},
             {(int)EUIID.UI_Pet_Message, new UIConfigData(
                    typeof(UI_Pet_Message),
                    "UI/Pet/UI_Pet_Message3.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate|EUIOption.eHideMainCamera|EUIOption.eReduceMainCameraQuality)},
             {(int)EUIID.UI_Pet_Tips01, new UIConfigData(
                    typeof(UI_Pet_Tips01),
                    "UI/Pet/UI_Pet_Tips01.prefab"
                    )},

              {(int)EUIID.UI_Fashion_SuitChange, new UIConfigData(
                    typeof(UI_Fashion_SuitChange),
                    "UI/Fashion/UI_Fashion_SuitChange.prefab"
                    )},
              {(int)EUIID.UI_Modification_Name, new UIConfigData(
                    typeof(UI_Modification_Name),
                    "UI/Pet/UI_Modification_Name.prefab"
                    )},
              {(int)EUIID.UI_SubmitItem, new UIConfigData(
                    typeof(UI_SubmitItem),
                    "UI/Task/UI_SubmitItem.prefab",
                    EUIOption.eHideBeforeUI)},
              {(int)EUIID.UI_SubmitPet, new UIConfigData(
                    typeof(UI_SubmitPet),
                    "UI/Pet/UI_SubmitPet.prefab",
                    EUIOption.eHideBeforeUI)},

               {(int)EUIID.UI_Pet_AddPoint, new UIConfigData(
                    typeof(UI_Pet_AddPoint),
                    "UI/Pet/UI_Pet_AddPoint2.prefab"
                    )},
               {(int)EUIID.UI_Pet_Develop, new UIConfigData(
                    typeof(UI_Pet_Develop),
                    "UI/Pet/UI_Pet_Develop.prefab",
                   EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate| EUIOption.eReduceMainCameraQuality)},
               {(int)EUIID.UI_Pet_GradeUp, new UIConfigData(
                    typeof(UI_Pet_GradeUp),
                    "UI/Pet/UI_Pet_GradeUp.prefab"
                    )},
               {(int)EUIID.UI_Pet_Automatical, new UIConfigData(
                    typeof(UI_Pet_Automatical),
                    "UI/Pet/UI_Pet_Automatical.prefab"
                    )},
               {(int)EUIID.UI_Pet_Develope_SelecItem, new UIConfigData(
                    typeof(UI_Pet_Develope_SelecItem),
                    "UI/Pet/UI_SelectObject.prefab"
                    )},
               {(int)EUIID.UI_Pet_Details, new UIConfigData(
                    typeof(UI_Pet_Details),
                    "UI/Pet/UI_Pet_Details.prefab"
                    )},
                {(int)EUIID.UI_Pet_Get, new UIConfigData(
                    typeof(UI_Pet_Get),
                    "UI/Pet/UI_Pet_Get.prefab",
                    EUIOption.eHideBeforeUI)},

                {(int)EUIID.UI_Tips_Pet, new UIConfigData(
                    typeof(UI_Tips_Pet),
                    "UI/Trade/UI_Tip_Pet.prefab")},

               {(int)EUIID.UI_Multi_Ready, new UIConfigData(
                    typeof(UI_Multi_Ready),
                    "UI/Team/UI_Copy_Enter.prefab",
                     EUIOption.eHideBeforeUI
                    )},
               {(int)EUIID.UI_Multi_Info, new UIConfigData(
                    typeof(UI_Multi_Info),
                    "UI/Manydungeons/UI_Manydungeons_Info.prefab",
                     EUIOption.eInvalid
                    )},
               {(int)EUIID.UI_Multi_PlayType, new UIConfigData(
                    typeof(UI_Multi_PlayType),
                    "UI/Manydungeons/UI_Manydungeons.prefab",
                     EUIOption.eHideBeforeUI
                    )},
               {(int)EUIID.UI_Multi_Reward, new UIConfigData(
                    typeof(UI_Multi_Reward),
                    "UI/Manydungeons/UI_Manydungeons_Reward.prefab",
                     EUIOption.eInvalid
                    )},
                {(int)EUIID.UI_MainBattle_Good, new UIConfigData(
                    typeof(UI_MainBattle_Good),
                    "UI/MainBattle/UI_MainBattle_Good.prefab"
                    )},
                {(int)EUIID.UI_Compete, new UIConfigData(
                    typeof(UI_Compete),
                    "UI/UI_Compete.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate| EUIOption.eReduceMainCameraQuality)},
                { (int)EUIID.UI_SelectItem, new UIConfigData(
                    typeof(UI_SelectItem),
                    "UI/Pet/UI_SelectItem.prefab"
                    )},
                { (int)EUIID.UI_SelectPet, new UIConfigData(
                    typeof(UI_SelectPet),
                    "UI/Pet/UI_SelectPet.prefab"
                    )},

                {(int)EUIID.UI_Skill_Tips, new UIConfigData(
                    typeof(UI_Skill_Tips),
                    "UI/Common/UI_Skill_Tips.prefab"
                    )},
                {(int)EUIID.UI_Instance_Reuslt, new UIConfigData(
                    typeof(UI_Multi_Result),
                    "UI/CopyResult/UI_CopyResult.prefab",
                    EUIOption.eHideBeforeUI
                    )},
                {(int)EUIID.UI_Pet_GetMix, new UIConfigData(
                    typeof(UI_Pet_GetMix),
                    "UI/Pet/UI_Pet_GetMix.prefab")},
                {(int)EUIID.UI_TalentReset, new UIConfigData(
                    typeof(UI_TalentReset),
                    "UI/Common/UI_Tips03.prefab")},
                {(int)EUIID.UI_TalentExchange, new UIConfigData(
                    typeof(UI_TalentExchange),
                    "UI/Skill/UI_Skill_Exchange.prefab",
                    EUIOption.eInvalid
                    )},
                {(int)EUIID.UI_QTEClick_1, new UIConfigData(
                    typeof(QTE_Click),
                    "UI/QTE/UI_QTE_Once_1.prefab",
                    EUIOption.eInvalid
                    )},
                {(int)EUIID.UI_QTEClick_3, new UIConfigData(
                    typeof(QTE_Click),
                    "UI/QTE/UI_QTE_Once_2.prefab",
                    EUIOption.eInvalid
                    )},
                {(int)EUIID.UI_QTEClick_2, new UIConfigData(
                    typeof(QTE_Click_Unlock),
                    "UI/UI_Plotlock_Click.prefab",
                    EUIOption.eHideBeforeUI
                )},
                {(int)EUIID.UI_QTELongPress_1, new UIConfigData(
                    typeof(QTE_LongPress),
                    "UI/QTE/UI_QTE_Long_1.prefab",
                    EUIOption.eInvalid
                    )},
                {(int)EUIID.UI_QTESlide_1, new UIConfigData(
                    typeof(QTE_Slide),
                    "UI/QTE/UI_QTE_Slider_1.prefab",
                    EUIOption.eInvalid
                    )},
                {(int)EUIID.UI_QTELongPress_2, new UIConfigData(
                        typeof(QTE_LongPress),
                        "UI/UI_Plotlock.prefab",
                        EUIOption.eHideBeforeUI
                )},
                {(int)EUIID.UI_QTESlide_2, new UIConfigData(
                        typeof(QTE_Slide),
                        "UI/QTE/UI_QTE_Slider_2.prefab",
                        EUIOption.eHideBeforeUI
                )},
                {(int)EUIID.UI_QTESlide_2_1, new UIConfigData(
                        typeof(QTE_Slide),
                        "UI/UI_Plotlock2.prefab",
                        EUIOption.eHideBeforeUI
                )},
                {(int)EUIID.UI_QTESlide_2_2, new UIConfigData(
                        typeof(QTE_Slide),
                        "UI/UI_Plotlock3.prefab",
                        EUIOption.eHideBeforeUI
                )},
                {(int)EUIID.UI_QTEEye, new UIConfigData(
                        typeof(UI_Empty),
                        "UI/Cutscene/UI_CutScene_Camera01.prefab"
                )},
                {(int)EUIID.UI_QTETriangle, new UIConfigData(
                        typeof(UI_Empty),
                        "UI/Cutscene/UI_CutScene_Camera02.prefab"
                )},
                {(int)EUIID.UI_Mall, new UIConfigData(
                    typeof(UI_Mall),
                    "UI/Mall/UI_Mall.prefab",
                   EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate| EUIOption.eReduceMainCameraQuality | EUIOption.eHideMainCamera)},

                {(int)EUIID.UI_Pet_BookReview, new UIConfigData(
                    typeof(UI_Pet_BookReview),
                    "UI/Pet/UI_Pet_BookReview 1.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate|EUIOption.eReduceMainCameraQuality|EUIOption.eHideMainCamera)},
                {(int)EUIID.UI_Pet_Story, new UIConfigData(
                    typeof(UI_Pet_Story),
                    "UI/Pet/UI_Pet_Story.prefab"
                   )},
                {(int)EUIID.UI_PreviewMap, new UIConfigData(
                    typeof(UI_PreviewMap),
                    "UI/Map/UI_Map_Guide.prefab",
                    EUIOption.eHideBeforeUI| EUIOption.eIgnoreStack|EUIOption.eReduceFrameRate| EUIOption.eHideMainCamera, 9450)},
                { (int)EUIID.UI_Weather, new UIConfigData(
                    typeof(UI_Weather),
                    "UI/MainInterface/UI_Weather.prefab"
                    )},
                {(int)EUIID.UI_LifeSkill_Message, new UIConfigData(
                    typeof(UI_LifeSkill_Message),
                    "UI/LifeSkill/UI_LifeSkill.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate|EUIOption.eReduceMainCameraQuality
                    )},
                { (int)EUIID.UI_DailyActivites, new UIConfigData(
                    typeof(UI_DailyActivites),
                    "UI/Activity/UI_Activity.prefab",
                    EUIOption.eHideBeforeUI
                    )},
                {(int)EUIID.UI_LifeSkill_MedicineSelect, new UIConfigData(
                    typeof(UI_LifeSkill_MedicineSelect),
                    "UI/LifeSkill/UI_LifeSkill_MedicineSelect.prefab",
                    EUIOption.eReduceFrameRate|EUIOption.eReduceMainCameraQuality
                    )},
                { (int)EUIID.UI_DailyActivites_Calendar, new UIConfigData(
                    typeof(UI_Daily_Calendar),
                    "UI/Activity/UI_Activity_Calendar.prefab"
                    )},
                {(int)EUIID.UI_ExpUp_SelectItem, new UIConfigData(
                    typeof(UI_ExpUp_SelectItem),
                    "UI/LifeSkill/UI_ExpUp_SelectItem.prefab",
                    EUIOption.eInvalid)},
                {(int)EUIID.UI_PromptItemBox, new UIConfigData(
                    typeof(UI_PromptItemBox),
                    "UI/LifeSkill/UI_LifeSkill_LevelUp.prefab",
                    EUIOption.eInvalid)},
                {(int)EUIID.UI_LifeSkill_MedicineTips, new UIConfigData(
                    typeof(UI_LifeSkill_MedicineTips),
                    "UI/LifeSkill/UI_LifeSkill_MedicineTips.prefab",
                    EUIOption.eInvalid)},
                {(int)EUIID.UI_LifeSkill_MakeTips, new UIConfigData(
                    typeof(UI_LifeSkill_MakePreview),
                    "UI/LifeSkill/UI_LifeSkill_MakePreview.prefab",
                    EUIOption.eInvalid)},
                {(int)EUIID.UI_ChooseItem, new UIConfigData(
                    typeof(UI_ChooseItem),
                    "UI/LifeSkill/UI_ChooseItem.prefab",
                    EUIOption.eInvalid)},
                { (int)EUIID.UI_DailyActivites_Set, new UIConfigData(
                    typeof(UI_Daily_Bell),
                    "UI/Activity/UI_Activity_Bell.prefab"
                    )},
               {(int)EUIID.UI_Pet_BookList, new UIConfigData(
                    typeof(UI_Pet_BookList),
                    "UI/Pet/UI_Pet_BookList.prefab",
                   EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate| EUIOption.eReduceMainCameraQuality | EUIOption.eHideMainCamera)},
                {(int)EUIID.UI_Eraser, new UIConfigData(
                    typeof(UI_Earser),
                    "UI/Nubber/UI_Nubber.prefab",
                    EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eReduceMainCameraQuality, 100)},
                {(int)EUIID.UI_AreaProtection, new UIConfigData(
                    typeof(UI_AreaProtection),
                    "UI/Event/UI_Event.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate|EUIOption.eReduceMainCameraQuality)},
                {(int)EUIID.UI_AreaProtectionTips, new UIConfigData(
                    typeof(UI_AreaProtectionTips),
                    "UI/Event/UI_Event_Tips.prefab",
                    EUIOption.eIgnoreStack)},
                {(int)EUIID.UI_MainBattle_SealItem, new UIConfigData(
                    typeof(UI_MainBattle_Seal),
                    "UI/MainBattle/UI_MainBattle_SealItem.prefab"
                    )},
                {(int)EUIID.UI_DailyActivites_Activity, new UIConfigData(
                    typeof(UI_Daily_ActivityAward),
                    "UI/Activity/UI_Activity_Award.prefab")},
                {(int)EUIID.UI_DailyActivites_Detail, new UIConfigData(
                    typeof(UI_Daily_DetailCom),
                    "UI/Activity/UI_Activity_Message.prefab")},

                {(int)EUIID.UI_DailyActivites_Limite, new UIConfigData(
                    typeof(UI_Daily_LimitActivity),
                    "UI/Activity/UI_Activity_LimitedTime.prefab")},
                {(int)EUIID.UI_Pub, new UIConfigData(
                    typeof(UI_Pub),
                    "UI/Pub/UI_Pub.prefab", EUIOption.eHideBeforeUI)},
                {(int)EUIID.UI_Probe_Report, new UIConfigData(
                    typeof(UI_Probe_Report),
                    "UI/MainInterface/UI_Probe_Report.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate|EUIOption.eReduceMainCameraQuality)},
                {(int)EUIID.UI_Temple_Storage, new UIConfigData(
                    typeof(UI_Temple_Storage),
                    "UI/Bag/UI_Temple_Storage.prefab",
                    EUIOption.eHideBeforeUI)},

                {(int)EUIID.UI_Make_Success, new UIConfigData(
                    typeof(UI_Make_Success),
                    "UI/Equip/UI_Make_Success.prefab")},

                {(int)EUIID.UI_Patrol, new UIConfigData(
                    typeof(UI_Patrol),
                    "UI/UI_Patrol.prefab",
                    EUIOption.eInvalid, -1)},//EUIOption.eIgnoreStack

                {(int)EUIID.UI_FunctionMenu, new UIConfigData(
                    typeof(UI_Mainbattle_Function),
                    "UI/MainBattle/UI_FunctionMenu.prefab")},

                {(int)EUIID.UI_Pvp_Single, new UIConfigData(
                    typeof(UI_Pvp_Single),
                    "UI/PVP/UI_PVP.prefab",
                    EUIOption.eHideBeforeUI)},
                {(int)EUIID.UI_Pvp_RiseInRank, new UIConfigData(
                    typeof(UI_Pvp_RiseInRank),
                    "UI/PVP/UI_PVP_LevelReward.prefab")},
                {(int)EUIID.UI_Pvp_SingleLoading, new UIConfigData(
                    typeof(UI_Pvp_SingleLoading),
                    "UI/PVP/UI_PVP_Loading.prefab",
                    EUIOption.eHideBeforeUI)},
                {(int)EUIID.UI_Pvp_SingleMatch, new UIConfigData(
                    typeof(UI_Pvp_SingleMatch),
                    "UI/PVP/UI_PVP_Match.prefab",
                    EUIOption.eHideBeforeUI)},
                {(int)EUIID.UI_Pvp_SingleRank, new UIConfigData(
                    typeof(UI_Pvp_SingleRank),
                    "UI/PVP/UI_PVP_Rank.prefab")},

                {(int)EUIID.UI_Pvp_SingleNewSeason, new UIConfigData(
                    typeof(UI_Pvp_NewSeason),
                    "UI/PVP/UI_PVP_NewSeason.prefab"
                    )},
                {(int)EUIID.UI_Pvp_SingleFinale, new UIConfigData(
                    typeof(UI_Pvp_Single_Over),
                    "UI/PVP/UI_PVP_Finish.prefab",
                     EUIOption.eHideBeforeUI)},

                {(int)EUIID.UI_Pvp_SingleRankReward, new UIConfigData(
                    typeof(UI_PvpSingle_RankReward),
                    "UI/PVP/UI_PVP_RankReward.prefab")},

                {(int)EUIID.UI_Decompose, new UIConfigData(
                    typeof(UI_Decompose),
                    "UI/Bag/UI_Decompose.prefab")},

                {(int)EUIID.UI_FavorabilityMoodDesc, new UIConfigData(
                    typeof(UI_FavorabilityMoodDesc),
                    "UI/NpcFavorability/UI_NPC_Mood.prefab")},

                {(int)EUIID.UI_FavorabilityFirstUnlock, new UIConfigData(
                    typeof(UI_FavorabilityFirstUnlock),
                    "UI/NpcFavorability/UI_NPC_Tips1.prefab")},
                {(int)EUIID.UI_FavorabilityNpcLikeDesc, new UIConfigData(
                    typeof(UI_FavorabilityNpcLikeDesc),
                    "UI/NpcFavorability/UI_NPC_LikeThing.prefab")},
                {(int)EUIID.UI_FavorabilityNpcLikeFood, new UIConfigData(
                    typeof(UI_FavorabilityNpcLikeFood),
                    "UI/NpcFavorability/UI_NPC_Like.prefab")},

                {(int)EUIID.UI_FavorabilityMain, new UIConfigData(
                    typeof(UI_FavorabilityMain),
                    "UI/NpcFavorability/UI_NPC_Favorability.prefab", EUIOption.eHideBeforeUI | EUIOption.eHideMainCamera)},
                {(int)EUIID.UI_FavorabilityNPCShow, new UIConfigData(
                    typeof(UI_FavorabilityNPCShow),
                    "UI/NpcFavorability/UI_NPC_Introduce.prefab", EUIOption.eHideBeforeUI | EUIOption.eHideMainCamera)},
                {(int)EUIID.UI_FavorabilityNPCCharacterDesc, new UIConfigData(
                    typeof(UI_FavorabilityNPCCharacterDesc),
                    "UI/NpcFavorability/UI_NPC_Character.prefab")},
                {(int)EUIID.UI_FavorabilityStageRewardPreview, new UIConfigData(
                    typeof(UI_FavorabilityStageRewardPreview),
                    "UI/NpcFavorability/UI_NPC_Award.prefab")},
                {(int)EUIID.UI_FavorabilityFilterGifts, new UIConfigData(
                    typeof(UI_FavorabilityFilterGifts),
                    "UI/NpcFavorability/UI_NPC_Screen.prefab")},
                {(int)EUIID.UI_FavorabilityNPCChanged, new UIConfigData(
                    typeof(UI_FavorabilityNPCChanged),
                    "UI/NpcFavorability/UI_NPC_Tips.prefab", EUIOption.eIgnoreStack)},

                {(int)EUIID.UI_FavorabilityMenu, new UIConfigData(
                    typeof(UI_FavorabilityMenu),
                    "UI/NpcFavorability/UI_NPC_Dialogue.prefab")},
                {(int)EUIID.UI_FavorabilityDialogue, new UIConfigData(
                    typeof(UI_FavorabilityDialogue),
                    "UI/UI_Dialogue.prefab", EUIOption.eHideBeforeUI)},

                {(int)EUIID.UI_FavorabilityHealth, new UIConfigData(
                    typeof(UI_FavorabilityHealth),
                    "UI/NpcFavorability/UI_NPC_Health.prefab")},
                {(int)EUIID.UI_FavorabilityClue, new UIConfigData(
                    typeof(UI_FavorabilityClue),
                    "UI/NpcFavorability/UI_NPC_Clue.prefab")},
                 {(int)EUIID.UI_FavorabilitySendGift, new UIConfigData(
                    typeof(UI_FavorabilitySendGift),
                    "UI/NpcFavorability/UI_NPC_GiveGift.prefab")},
                 {(int)EUIID.UI_FavorabilityDanceList, new UIConfigData(
                    typeof(UI_FavorabilityDanceList),
                    "UI/NpcFavorability/UI_NPC_Play.prefab")},
                 {(int)EUIID.UI_FavorabilityFete, new UIConfigData(
                    typeof(UI_FavorabilityFete),
                    "UI/NpcFavorability/UI_NPC_Fete.prefab")},
                 {(int)EUIID.UI_FavorabilityMusicList, new UIConfigData(
                    typeof(UI_FavorabilityMusicList),
                    "UI/NpcFavorability/UI_NPC_Play.prefab")},
                 {(int)EUIID.UI_FavorabilityThanks, new UIConfigData(
                    typeof(UI_FavorabilityThanks),
                    "UI/NpcFavorability/UI_NPC_ThanksLetter.prefab")},
                 {(int)EUIID.UI_PureHint, new UIConfigData(
                    typeof(UI_PureHint),
                    "UI/Common/UI_PureHint.prefab", EUIOption.eIgnoreStack)},
                 {(int)EUIID.UI_FavorabilityTip, new UIConfigData(
                    typeof(UI_FavorabilityTip),
                    "UI/UI_Rule1.prefab")},

                 // 世界boss相关
                 {(int)EUIID.UI_WorldBossCampChallenge, new UIConfigData(
                    typeof(UI_CampChallenge),
                    "UI/WorldBoss/UI_WorldBoss.prefab", EUIOption.eHideBeforeUI/* | EUIOption.eHideMainCamera*/)},
                 {(int)EUIID.UI_WorldBossCampInfo, new UIConfigData(
                    typeof(UI_CampInfo),
                    "UI/WorldBoss/UI_WorldBoss_Challenge.prefab", EUIOption.eHideBeforeUI)},
                 {(int)EUIID.UI_WorldBossCampPreview, new UIConfigData(
                    typeof(UI_CampPreview),
                    "UI/WorldBoss/UI_WorldBoss_Camp.prefab", EUIOption.eHideBeforeUI)},
                 {(int)EUIID.UI_WorldBossManualShow, new UIConfigData(
                    typeof(UI_BossManualInfo),
                    "UI/WorldBoss/UI_WorldBoss_CampDetail.prefab", EUIOption.eHideBeforeUI)},
                 {(int)EUIID.UI_WorldBossChallengeRule, new UIConfigData(
                    typeof(UI_WorldBossChallengeRule),
                    "UI/WorldBoss/UI_WorldBoss_Info.prefab")},
                 {(int)EUIID.UI_WorldBossSignUp, new UIConfigData(
                    typeof(UI_BossSignUp),
                    "UI/WorldBoss/UI_WorldBoss_Signup.prefab", EUIOption.eHideBeforeUI)},
                {(int)EUIID.UI_WorldBossSignUpFail, new UIConfigData(
                    typeof(UI_BossSignUpFail),
                    "UI/WorldBoss/UI_WorldBoss_SignupFail.prefab", EUIOption.eHideBeforeUI)},
                {(int)EUIID.UI_WorldBossUnlockCampOrBoss, new UIConfigData(
                    typeof(UI_BossUnlockCampOrBoss),
                    "UI/WorldBoss/UI_WorldBoss_Tips.prefab")},
                {(int)EUIID.UI_WorldBossRank, new UIConfigData(
                    typeof(UI_WorldBossRank),
                    "UI/WorldBoss/UI_WorldBoss_Rank.prefab"/*,
                    EUIOption.eHideBeforeUI | EUIOption.eHideMainCamera | EUIOption.eReduceFrameRate*/)},
                 {(int)EUIID.UI_WorldBossRewardList, new UIConfigData(
                    typeof(UI_WorldBossRewardList),
                    "UI/WorldBoss/UI_WorldBoss_RewardList.prefab"
                   )},

                {(int)EUIID.UI_Reputation, new UIConfigData(
                    typeof(UI_Reputation),
                    "UI/Attribute/UI_Reputation1.prefab",
                   EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate|EUIOption.eHideMainCamera|EUIOption.eReduceMainCameraQuality)},

                { (int)EUIID.UI_Make_Preview, new UIConfigData(
                    typeof(UI_Make_Preview),
                    "UI/Equip/UI_Make_preview1.prefab",
                    EUIOption.eInvalid)},

                {(int)EUIID.UI_TitleSeriesSelect, new UIConfigData(
                    typeof(UI_TitleSeriesSelect),
                    "UI/Attribute/UI_TitleSeriesSelect.prefab")},
                {(int)EUIID.UI_TitleSeries, new UIConfigData(
                    typeof(UI_TitleSeries),
                    "UI/Attribute/UI_TitleSeries.prefab")},
                {(int)EUIID.UI_TitleTips, new UIConfigData(
                    typeof(UI_TitleTips),
                    "UI/Attribute/UI_TitleTips.prefab")},
                {(int)EUIID.UI_TitleAward, new UIConfigData(
                    typeof(UI_TitleAward),
                    "UI/Attribute/UI_TitleAward.prefab")},
                {(int)EUIID.UI_Energyspar, new UIConfigData(
                    typeof(UI_Energyspar),
                    "UI/Energyspar/UI_Energyspar.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate|EUIOption.eReduceMainCameraQuality)},
                {(int)EUIID.UI_Advancedl_Result, new UIConfigData(
                    typeof(UI_Advancedl_Result),
                    "UI/Energyspar/UI_Advancedl_Result.prefab")},
                {(int)EUIID.UI_Energyspar_Advanced, new UIConfigData(
                    typeof(UI_Energyspar_Advanced),
                    "UI/Energyspar/UI_Energyspar_Advanced.prefab")},
                {(int)EUIID.UI_Energyspar_Changeover, new UIConfigData(
                    typeof(UI_Energyspar_Changeover),
                    "UI/Energyspar/UI_Energyspar_Changeover.prefab")},
                {(int)EUIID.UI_Energyspar_Refine, new UIConfigData(
                    typeof(UI_Energyspar_Refine),
                    "UI/Energyspar/UI_Energyspar_Refine.prefab")},
                {(int)EUIID.UI_Energyspar_Resolve, new UIConfigData(
                    typeof(UI_Energyspar_Resolve),
                    "UI/Energyspar/UI_Energyspar_Resolve.prefab")},
                {(int)EUIID.UI_Energyspar_Tips, new UIConfigData(
                    typeof(UI_Energyspar_Tips),
                    "UI/Energyspar/UI_Energyspar_Tips.prefab")},
                {(int)EUIID.UI_Upgrade_Result, new UIConfigData(
                    typeof(UI_Upgrade_Result),
                    "UI/Energyspar/UI_Upgrade_Result.prefab" )},
                {(int)EUIID.UI_MainBattle_SparSkills, new UIConfigData(
                    typeof(UI_MainBattle_SparSkills),
                    "UI/MainBattle/UI_MainBattle_SparSkills.prefab")},
                {(int)EUIID.UI_Fashion_Tips, new UIConfigData(
                    typeof(UI_Fashion_Tips),
                    "UI/Fashion/UI_Fashion_Tips.prefab",
                    EUIOption.eInvalid)},

                {(int)EUIID.UI_Trade, new UIConfigData(
                    typeof(UI_Trade),
                    "UI/Trade/UI_Trade.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate|EUIOption.eHideMainCamera)},

                {(int)EUIID.UI_Trade_Sell, new UIConfigData(
                    typeof(UI_Trade_Sell),
                    "UI/Trade/UI_Trade_View_Sell.prefab",
                    EUIOption.eInvalid)},

                {(int)EUIID.UI_Trade_Sure_Tip, new UIConfigData(
                    typeof(UI_Trade_Sure_Tip),
                    "UI/Trade/UI_Trade_SureTip.prefab",
                    EUIOption.eInvalid)},

                {(int)EUIID.UI_Trade_Buy, new UIConfigData(
                    typeof(UI_Trade_Buy),
                    "UI/Trade/UI_Trade_Buy.prefab",
                    EUIOption.eInvalid)},

                {(int)EUIID.UI_Trade_Publicity_Buy, new UIConfigData(
                    typeof(UI_Trade_Publicity_Buy),
                    "UI/Trade/UI_Trade_Publicity_Buy.prefab",
                    EUIOption.eInvalid)},

                {(int)EUIID.UI_Trade_Search, new UIConfigData(
                    typeof(UI_Trade_Search),
                    "UI/Trade/UI_Trade_Search.prefab",
                    EUIOption.eInvalid)},

                  {(int)EUIID.UI_Trade_SellDetail_OutOfDate, new UIConfigData(
                    typeof(UI_Trade_SellDetail_OutOfDate),
                    "UI/Trade/UI_Trade_Sell_Detail_OutOfDate.prefab",
                    EUIOption.eInvalid)},

                 {(int)EUIID.UI_Trade_SellDetail_Pricing, new UIConfigData(
                    typeof(UI_Trade_SellDetail_Pricing),
                    "UI/Trade/UI_Trade_Sell_Detail_Pricing.prefab",
                    EUIOption.eInvalid)},

                  {(int)EUIID.UI_Trade_SellDetail_Publicity, new UIConfigData(
                    typeof(UI_Trade_SellDetail_Publicity),
                    "UI/Trade/UI_Trade_Sell_Detail_Publicity.prefab",
                    EUIOption.eInvalid)},

                {(int)EUIID.UI_Trade_SellDetail_Sale, new UIConfigData(
                    typeof(UI_Trade_SellDetail_Sale),
                    "UI/Trade/UI_Trade_Sell_Detail_Sale.prefab",
                    EUIOption.eInvalid)},

                {(int)EUIID.UI_Trade_SellDetail_Bargin, new UIConfigData(
                    typeof(UI_Trade_SellDetail_Bargin),
                    "UI/Trade/UI_Trade_Sell_DetailTip.prefab",
                    EUIOption.eInvalid)},

                {(int)EUIID.UI_Trade_Record, new UIConfigData(
                    typeof(UI_Trade_Record),
                    "UI/Trade/UI_Trade_Record.prefab",
                    EUIOption.eInvalid)},

                {(int)EUIID.UI_Terrorist, new UIConfigData(
                    typeof(UI_Terrorist),
                    "UI/TerroristHotel/UI_TerroristMain.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate|EUIOption.eReduceMainCameraQuality)},

                {(int)EUIID.UI_TerroristWeek, new UIConfigData(
                    typeof(UI_TerroristWeek),
                    "UI/TerroristHotel/UI_TerroristDetail.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate|EUIOption.eReduceMainCameraQuality)},

                {(int)EUIID.UI_TerroristEnter, new UIConfigData(
                    typeof(UI_TerroristEnter),
                    "UI/TerroristHotel/UI_TerroristEnter.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate|EUIOption.eReduceMainCameraQuality)},

                {(int)EUIID.UI_SceneNubber, new UIConfigData(
                    typeof(UI_SceneNubber),
                    "UI/Nubber/UI_SceneNubber.prefab",
                    EUIOption.eHideBeforeUI, 100)},

                {(int)EUIID.UI_ApplyFamily, new UIConfigData(
                    typeof(UI_ApplyFamily),
                    "UI/Family/UI_Family_Join.prefab",
                    EUIOption.eHideMainCamera| EUIOption.eHideBeforeUI)},
                {(int)EUIID.UI_Family, new UIConfigData(
                    typeof(UI_Family),
                    "UI/Family/UI_Family_Hall.prefab",
                    EUIOption.eHideMainCamera|EUIOption.eHideBeforeUI)},
                {(int)EUIID.UI_Family_Empowerment, new UIConfigData(
                    typeof(UI_Family_Empowerment),
                    "UI/Family/UI_Family_Empowerment.prefab")},
                {(int)EUIID.UI_Family_DeedsLv_Popup, new UIConfigData(
                    typeof(UI_Family_DeedsLv_Popup),
                    "UI/Family/UI_Family_DeedsLv_Popup.prefab")},
                {(int)EUIID.UI_Family_ModifyName, new UIConfigData(
                    typeof(UI_Family_Hall_ModifyName),
                    "UI/Family/UI_Family_Rename.prefab",
                     EUIOption.eHideMainCamera)},
                {(int)EUIID.UI_Family_ModifyDeclaration, new UIConfigData(
                    typeof(UI_Family_Hall_ModifyDeclaration),
                    "UI/Family/UI_Family_Declaration.prefab",
                     EUIOption.eHideMainCamera)},
                {(int)EUIID.UI_Family_GroupSending, new UIConfigData(
                    typeof(UI_Family_Hall_GroupSending),
                    "UI/Family/UI_Family_Send_News.prefab",
                     EUIOption.eHideMainCamera)},
                {(int)EUIID.UI_Family_Merge, new UIConfigData(
                    typeof(UI_Family_Hall_Merge),
                    "UI/Family/UI_Family_Merge.prefab",
                     EUIOption.eHideMainCamera)},
                {(int)EUIID.UI_Family_MergeApply, new UIConfigData(
                    typeof(UI_Family_Hall_Merge_Apply),
                    "UI/Family/UI_Family_Merge_Apply.prefab",
                     EUIOption.eHideMainCamera)},
                {(int)EUIID.UI_Family_MergeLaunch, new UIConfigData(
                    typeof(UI_Family_Hall_Merge_Launch),
                    "UI/Family/UI_Family_Merge_Launch.prefab",
                     EUIOption.eHideMainCamera)},
                 {(int)EUIID.UI_Family_MergeConfirm, new UIConfigData(
                    typeof(UI_Family_Hall_Merge_Launch_Confirm),
                    "UI/Family/UI_Family_Merge_Confirm.prefab",
                     EUIOption.eHideMainCamera)},
                {(int)EUIID.UI_Family_MergeAgreement, new UIConfigData(
                    typeof(UI_Family_Hall_Merge_Agreement),
                    "UI/Family/UI_Family_Agreement.prefab",
                     EUIOption.eHideMainCamera)},
                {(int)EUIID.UI_Family_PromptBox_Leave, new UIConfigData(
                    typeof(UI_Family_PromptBox_Leave),
                    "UI/Family/UI_Family_Leave.prefab",
                     EUIOption.eHideMainCamera)},
                {(int)EUIID.UI_Family_Branch, new UIConfigData(
                    typeof(UI_Family_Member_Branch),
                    "UI/Family/UI_Family_Branch.prefab",
                     EUIOption.eHideMainCamera)},
                {(int)EUIID.UI_Family_BranchMember, new UIConfigData(
                    typeof(UI_Family_Member_Branch_Member),
                    "UI/Family/UI_Family_Member.prefab",
                     EUIOption.eHideMainCamera)},
                {(int)EUIID.UI_Family_BranchMemberManage, new UIConfigData(
                    typeof(UI_Family_Member_Branch_Member_Manage),
                    "UI/Family/UI_Family_Manage.prefab",
                     EUIOption.eHideMainCamera)},
                {(int)EUIID.UI_Family_BranchModifyName, new UIConfigData(
                    typeof(UI_Family_Member_Branch_ModifyName),
                    "UI/Family/UI_Family_Branch_Rename.prefab",
                     EUIOption.eHideMainCamera)},
                {(int)EUIID.UI_Family_BranchMerge, new UIConfigData(
                    typeof(UI_Family_Member_Branch_Merge),
                    "UI/Family/UI_Family_Branch_Merge.prefab",
                     EUIOption.eHideMainCamera)},
                {(int)EUIID.UI_Family_FamilyList, new UIConfigData(
                    typeof(UI_Family_Member_FamilyList),
                    "UI/Family/UI_Family_List.prefab",
                     EUIOption.eReduceMainCameraQuality|EUIOption.eHideBeforeUI)},
                {(int)EUIID.UI_Family_Authority, new UIConfigData(
                    typeof(UI_Family_Member_Authority),
                    "UI/Family/UI_Family_Limit.prefab",
                     EUIOption.eHideMainCamera)},
                {(int)EUIID.UI_Family_ApplyList, new UIConfigData(
                    typeof(UI_Family_Member_ApplyList),
                    "UI/Family/UI_Family_Apply.prefab",
                     EUIOption.eHideMainCamera)},
                {(int)EUIID.UI_Family_BuildLvUpgrade, new UIConfigData(
                    typeof(UI_Family_Building_LvUpgrade),
                    "UI/Family/UI_Family_Build.prefab",
                     EUIOption.eHideMainCamera)},
                {(int)EUIID.UI_Family_BuildSkillUpgrade, new UIConfigData(
                    typeof(UI_Family_Building_SkilUpgrade),
                    "UI/Family/UI_Family_Build_Skill.prefab",
                     EUIOption.eHideMainCamera)},
                {(int)EUIID.UI_Family_Bank, new UIConfigData(
                    typeof(UI_Family_Building_Bank),
                    "UI/Family/UI_Family_Storage.prefab",
                     EUIOption.eHideMainCamera)},
                {(int)EUIID.UI_Family_BankAward, new UIConfigData(
                    typeof(UI_Family_Building_BankAward),
                    "UI/Family/UI_Family_Award.prefab",
                     EUIOption.eHideMainCamera)},

                {(int)EUIID.UI_Compose, new UIConfigData(
                    typeof(UI_Compose),
                    "UI/Lotto/UI_Compose.prefab")},
                {(int)EUIID.UI_Lotto, new UIConfigData(
                    typeof(UI_Lotto),
                    "UI/Lotto/UI_Lotto.prefab",
                     EUIOption.eHideBeforeUI|EUIOption.eHideMainCamera)},
                {(int)EUIID.UI_Lotto_Preview, new UIConfigData(
                    typeof(UI_LottoRewardPreview),
                    "UI/Lotto/UI_LottoTips.prefab")},
                {(int)EUIID.UI_Lotto_RewardGet, new UIConfigData(
                    typeof(UI_LottoRewardGet),
                    "UI/Lotto/UI_LottoGet.prefab",
                     EUIOption.eHideBeforeUI)},
                {(int)EUIID.UI_GodPet, new UIConfigData(
                    typeof(UI_GodPet),
                    "UI/Lotto/UI_GodPet.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate|EUIOption.eReduceMainCameraQuality)},
                {(int)EUIID.UI_GodPetReview, new UIConfigData(
                    typeof(UI_GodPetReview),
                    "UI/Lotto/UI_GodPetReview.prefab")},


                {(int)EUIID.UI_RideLotto, new UIConfigData(
                    typeof(UI_RideLotto),
                    "UI/RideLotto/UI_RideLotto.prefab",
                     EUIOption.eHideBeforeUI|EUIOption.eHideMainCamera)},
                {(int)EUIID.UI_RideLotto_Preview, new UIConfigData(
                    typeof(UI_RideLottoRewardPreview),
                    "UI/RideLotto/UI_RideLottoTips.prefab")},
                {(int)EUIID.UI_RideLotto_RewardGet, new UIConfigData(
                    typeof(UI_RideLottoRewardGet),
                    "UI/RideLotto/UI_RideLottoGet.prefab",
                     EUIOption.eHideBeforeUI)},

                {(int)EUIID.UI_Uplifted, new UIConfigData(
                    typeof(UI_Uplifted),
                    "UI/Uplifted/UI_Uplifted.prefab")},

                //{(int)EUIID.UI_PartnerRune, new UIConfigData(
                //    typeof(UI_PartnerRune),
                //    "UI/Partner/UI_PartnerRune.prefab",
                //    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate|EUIOption.eReduceMainCameraQuality)},

                {(int)EUIID.UI_PartnerRune_Tips, new UIConfigData(
                    typeof(UI_PartnerRune_Tips),
                    "UI/Partner/UI_PartnerRune_Tips.prefab")},

                {(int)EUIID.UI_Rune_Result, new UIConfigData(
                    typeof(UI_Rune_Result),
                    "UI/Partner/UI_Rune_Result.prefab")},

                {(int)EUIID.UI_Exchange_Rune, new UIConfigData(
                    typeof(UI_Exchange_Rune),
                    "UI/Partner/UI_Exchange_Rune.prefab")},

                {(int)EUIID.UI_Advance_Warning, new UIConfigData(
                    typeof(UI_Advance_Warning),
                    "UI/Attribute/UI_Advance_Warning.prefab",
                     EUIOption.eReduceFrameRate|EUIOption.eReduceMainCameraQuality)},

                {(int)EUIID.UI_Advance_Tips, new UIConfigData(
                    typeof(UI_Advance_Tips),
                    "UI/Attribute/UI_Advance_Tips.prefab",
                    EUIOption.eReduceFrameRate|EUIOption.eReduceMainCameraQuality)},

                {(int)EUIID.UI_Advance_TeamTips, new UIConfigData(
                    typeof(UI_Advance_TeamTips),
                    "UI/Attribute/UI_Advance_TeamTips.prefab",
                    EUIOption.eReduceFrameRate|EUIOption.eReduceMainCameraQuality)},

                {(int)EUIID.UI_MapAward, new UIConfigData(
                    typeof(UI_MapAward),
                    "UI/Map/UI_MapAward.prefab")},

                {(int)EUIID.UI_Trade_Assign_Info, new UIConfigData(
                    typeof(UI_Trade_Assign_Info),
                    "UI/Trade/UI_Trade_View_Zhiding.prefab")},

                {(int)EUIID.UI_Trade_Record_Recovery, new UIConfigData(
                    typeof(UI_Trade_Record_Recovery),
                    "UI/Trade/UI_Trade_Deposit.prefab")},

                {(int)EUIID.UI_Trade_SaleSuccess_Tip, new UIConfigData(
                    typeof(UI_Trade_SaleSuccess_Tip),
                    "UI/Trade/UI_Trade_CompleteTip.prefab")},

                {(int)EUIID.UI_Trade_Box_Com, new UIConfigData(
                    typeof(UI_Trade_MessageBox_Com),
                    "UI/Trade/UI_Trade_Box.prefab")},

                {(int)EUIID.UI_Trade_Box_Equip, new UIConfigData(
                    typeof(UI_Trade_MessageBox_Equip),
                    "UI/Trade/UI_Trade_Box1.prefab")},

                 {(int)EUIID.UI_Trade_Box_Ornament, new UIConfigData(
                    typeof(UI_Trade_MessageBox_Ornament),
                    "UI/Trade/UI_Trade_Box2.prefab")},

                 {(int)EUIID.UI_Trade_Box_PetEquip, new UIConfigData(
                     typeof(UI_Trade_MessageBox_PetEquip),
                     "UI/Trade/UI_Trade_Box3.prefab")},

                {(int)EUIID.UI_Trade_Search_Pet_Tips, new UIConfigData(
                    typeof(UI_Trade_Search_Pet_Tips),
                    "UI/Trade/UI_Trade_PreviewTips.prefab")},

                {(int)EUIID.UI_Vitality, new UIConfigData(
                    typeof(UI_Vitality),
                    "UI/Vitality/UI_Vitality.prefab")},

                {(int)EUIID.UI_Vitality_Tips, new UIConfigData(
                    typeof(UI_Vitality_Tips),
                    "UI/Vitality/UI_Vitality_Tips.prefab")},

                {(int)EUIID.UI_BlockClickNetwork, new UIConfigData(
                    typeof(UI_BlockClickNetwork),
                    "UI/UI_BlockClick.prefab")},
                {(int)EUIID.UI_BlockClickTime, new UIConfigData(
                    typeof(UI_BlockClickTime),
                    "UI/UI_BlockClickTime.prefab")},
                {(int)EUIID.UI_BlockClickHttp, new UIConfigData(
                    typeof(UI_BlockClickHttp),
                    "UI/UI_BlockClick.prefab")},

               {(int)EUIID.UI_MainBattle_PetDetail, new UIConfigData(
                    typeof(UI_MainBattle_PetDetail),
                    "UI/MainBattle/UI_MainBattle_PetDetail.prefab")},

               {(int)EUIID.UI_MagicBook, new UIConfigData(
                    typeof(UI_MagicBook),
                    "UI/FunctionPreview/UI_MagicBook.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate|EUIOption.eReduceMainCameraQuality)},

                {(int)EUIID.UI_FunctionPreview, new UIConfigData(
                    typeof(UI_FunctionPreview),
                    "UI/FunctionPreview/UI_FunctionPreview.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate|EUIOption.eReduceMainCameraQuality)},

                {(int)EUIID.UI_MagicBook_Detail, new UIConfigData(
                    typeof(UI_MagicBook_Detail),
                    "UI/FunctionPreview/UI_MagicBook_Detail.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate|EUIOption.eReduceMainCameraQuality)},

                {(int)EUIID.UI_MagicBook_Tips, new UIConfigData(
                    typeof(UI_MagicBook_Tips),
                    "UI/FunctionPreview/UI_MagicBook_Tips.prefab")},
                {(int)EUIID.UI_Team_Invite, new UIConfigData(
                    typeof(UI_Team_Invite),
                    "UI/Team/UI_Team_Invite.prefab")},
                 {(int)EUIID.UI_Qa, new UIConfigData(
                    typeof(UI_Qa),
                    "UI/QA/UI_Qa.prefab")},
                { (int)EUIID.UI_MapCondition_Tips , new UIConfigData(
                    typeof(UI_MapConditionTips),
                    "UI/Map/UI_MapCondition.prefab")},
                { (int)EUIID.UI_FamilyMapCondition_Tips , new UIConfigData(
                    typeof(UI_FamilyMapCondition),
                    "UI/Map/UI_FamilyMapCondition.prefab")},
                { (int)EUIID.UI_ScreenLock , new UIConfigData(
                    typeof(UI_ScreenLock),
                    "UI/UI_Screenlock.prefab")},
                {(int)EUIID.UI_FadeInOut, new UIConfigData(
                    typeof(UI_FadeInOut),
                    "UI/Cutscene/UI_CutScenePre.prefab", EUIOption.eHideBeforeUI)},
                {(int)EUIID.UI_PointMall, new UIConfigData(
                    typeof(UI_PointMall),
                    "UI/Mall/UI_PointMall.prefab", EUIOption.eHideBeforeUI)},
                {(int)EUIID.UI_PointGetTips, new UIConfigData(
                    typeof(UI_PointGetTips),
                    "UI/Mall/UI_PointGet_Tips.prefab")},
                {(int)EUIID.UI_PointRuleTips, new UIConfigData(
                    typeof(UI_PointRuleTips),
                    "UI/Mall/UI_Point_Tips.prefab")},
                {(int)EUIID.UI_Cooking_Main, new UIConfigData(
                    typeof(UI_Cooking_Main),
                    "UI/Cooking/UI_Cooking_Main.prefab")},
                {(int)EUIID.UI_Cooking_Single, new UIConfigData(
                    typeof(UI_Cooking_Single),
                    "UI/Cooking/UI_Cooking_Single.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate)},
                {(int)EUIID.UI_Cooking_Choose, new UIConfigData(
                    typeof(UI_Cooking_Choose),
                     "UI/Cooking/UI_Cooking_Choose.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate)},
                {(int)EUIID.UI_Cooking_Loading, new UIConfigData(
                    typeof(UI_Cooking_Loading),
                     "UI/Cooking/UI_Cooking_Loading.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate)},
                {(int)EUIID.UI_Cooking_Multiple, new UIConfigData(
                    typeof(UI_Cooking_Multiple),
                     "UI/Cooking/UI_Cooking_Multiple.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate)},
                {(int)EUIID.UI_Cooking, new UIConfigData(
                    typeof(UI_Cooking),
                     "UI/Cooking/UI_Cooking.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate)},
                {(int)EUIID.UI_Knowledge_Cooking, new UIConfigData(
                    typeof(UI_Knowledge_Cooking),
                     "UI/Knowledge/UI_Knowledge_Cooking.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate)},
                {(int)EUIID.UI_Knowledge_Cooking_Award, new UIConfigData(
                    typeof(UI_Knowledge_Cooking_Award),
                     "UI/Knowledge/UI_Knowledge_Cooking_Award.prefab")},
                {(int)EUIID.UI_Cooking_Point, new UIConfigData(
                    typeof(UI_Cooking_Point),
                     "UI/Cooking/UI_Cooking_Point.prefab")},
                {(int)EUIID.UI_TipsAutoClose, new UIConfigData(
                    typeof(UI_TipsAutoClose),
                    "UI/Common/UI_Tips02.prefab")},
                {(int)EUIID.UI_Rank, new UIConfigData(
                    typeof(UI_Rank),
                     "UI/Rank/UI_Rank.prefab",
                     EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eHideMainCamera)},
                {(int)EUIID.UI_Rank_Detail01, new UIConfigData(
                    typeof(UI_Rank_Detail01),
                     "UI/Rank/UI_Rank_Detail01.prefab")},
                {(int)EUIID.UI_Rank_Setting, new UIConfigData(
                    typeof(UI_Rank_Setting),
                     "UI/Rank/UI_Rank_Setting.prefab")},
                {(int)EUIID.UI_MenuDialogue, new UIConfigData(
                    typeof(UI_MenuDialogue),
                    "UI/MainInterface/UI_task_Dialogue.prefab")},
                {(int)EUIID.UI_HangupFight, new UIConfigData(
                    typeof(UI_HangupFight),
                    "UI/HangUp/UI_MoppingUp.prefab", EUIOption.eHideBeforeUI)},
                {(int)EUIID.UI_HangupFightTriedTips, new UIConfigData(
                    typeof(UI_HangupFightTriedTips),
                    "UI/HangUp/UI_MoppingUp_TriedTips.prefab")},
                {(int)EUIID.UI_HangupFightOption, new UIConfigData(
                    typeof(UI_HangupFightOption),
                    "UI/HangUp/UI_MoppingUp_Tips.prefab")},
                {(int)EUIID.UI_HangupResult, new UIConfigData(
                    typeof(UI_HangupResult),
                    "UI/HangUp/UI_MoppingUp_OffLine.prefab")},
                {(int)EUIID.UI_Hundred_Result, new UIConfigData(
                    typeof(UI_Hundred_Result),
                    "UI/HundredPeople/UI_Hundred_Result.prefab",
                    EUIOption.eHideBeforeUI)},
                {(int)EUIID.UI_HundreadPeoplePopReward, new UIConfigData(
                    typeof(UI_HundreadPeoplePopReward),
                    "UI/HundredPeople/UI_HundreadPeoplePopReward.prefab",
                    EUIOption.eInvalid)},
                {(int)EUIID.UI_UnLockHangup, new UIConfigData(
                    typeof(UI_UnLockHangup),
                    "UI/HangUp/UI_MoppingUp_UnLock.prefab")},
                {(int)EUIID.UI_OperationalActivity, new UIConfigData(
                    typeof(UI_OperationalActivity),
                    "UI/SevenDays/UI_Activity_Operation.prefab", EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eHideMainCamera) },
                {(int)EUIID.UI_Adventure, new UIConfigData(
                    typeof(UI_Adventure),
                    "UI/AdventureBook/UI_AdventureBook.prefab", EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eHideMainCamera)},
                {(int)EUIID.UI_Adventure_LevelUp, new UIConfigData(
                    typeof(UI_Adventure_LevelUp),
                    "UI/AdventureBook/UI_AdventureLevelUp.prefab")},
                {(int)EUIID.UI_Adventure_RewardGetTips, new UIConfigData(
                    typeof(UI_Adventure_RewardGetTips),
                    "UI/AdventureBook/UI_RewardPost.prefab")},
                {(int)EUIID.UI_Adventure_RewardInfo, new UIConfigData(
                    typeof(UI_Adventure_RewardInfo),
                    "UI/AdventureBook/UI_RewardPostDetails.prefab")},
                {(int)EUIID.UI_Adventure_RewardFinishTips, new UIConfigData(
                    typeof(UI_Adventure_RewardFinishTips),
                    "UI/AdventureBook/UI_Reward_Result.prefab")},
                {(int)EUIID.UI_ElementalCrystal, new UIConfigData(
                    typeof(UI_ElementalCrystal),
                     "UI/ElementalCrystal/UI_ElementalCrystal.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate)},
                {(int)EUIID.UI_ElementalCrystal_Exchange, new UIConfigData(
                    typeof(UI_ElementalCrystal_Exchange),
                     "UI/ElementalCrystal/UI_ElementalCrystal_Exchange.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate)},
                 {(int)EUIID.UI_Tips_ElementalCrystal, new UIConfigData(
                    typeof(UI_Tips_ElementalCrystal),
                    "UI/ElementalCrystal/UI_Tips_ElementalCrystal.prefab",
                    EUIOption.eInvalid)},

                {(int)EUIID.UI_GoddessTrial, new UIConfigData(
                    typeof(UI_GoddnessTrial),
                    "UI/GoddessTrial/UI_GoddessTrial.prefab",EUIOption.eHideBeforeUI)},
                {(int)EUIID.UI_Goddess_Enter, new UIConfigData(
                    typeof(UI_Goddness_Enter),
                    "UI/GoddessTrial/UI_Goddess_Enter.prefab")},
                {(int)EUIID.UI_Goddess_Select, new UIConfigData(
                    typeof(UI_Goddness_Select),
                    "UI/GoddessTrial/UI_GoddessSelect.prefab",EUIOption.eHideBeforeUI)},//

                {(int)EUIID.UI_Goddess_Rank, new UIConfigData(
                    typeof(UI_Goddness_Rank),
                    "UI/GoddessTrial/UI_Goddess_Rank.prefab")},

                {(int)EUIID.UI_Goddess_Ending, new UIConfigData(
                    typeof(UI_Goddness_Ending),
                    "UI/GoddessTrial/UI_GoddessEnding.prefab")},//
                {(int)EUIID.UI_Goddess_Result, new UIConfigData(
                    typeof(UI_Goddness_Result),
                    "UI/GoddessTrial/UI_Goddess_Result.prefab")},
                {(int)EUIID.UI_Goddess_Difficult, new UIConfigData(
                    typeof(UI_Goddness_Difficult),
                    "UI/GoddessTrial/UI_Goddess_DifficultSelect.prefab")},
                {(int)EUIID.UI_Goddess_EndCollect, new UIConfigData(
                    typeof(UI_Goddness_EndCollect),
                    "UI/GoddessTrial/UI_Goddess_EndingCollect.prefab")},//
                {(int)EUIID.UI_Goddness_Charac, new UIConfigData(
                    typeof(UI_Goddness_Charac),
                    "UI/GoddessTrial/UI_Goddess_Characteristic.prefab")},

               {(int)EUIID.UI_Goddness_EndAward, new UIConfigData(
                    typeof(UI_Goddness_EndAward),
                    "UI/GoddessTrial/UI_Goddess_EndingAward.prefab")},
               {(int)EUIID.UI_Goddness_FristAward, new UIConfigData(
                    typeof(UI_Goddness_FristReward),
                    "UI/GoddessTrial/UI_RankAward.prefab")},

                {(int)EUIID.UI_PartnerRuneReplace, new UIConfigData(
                    typeof(UI_PartnerRuneReplace),
                    "UI/Partner/UI_PartnerRuneReplace.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate)},

                {(int)EUIID.UI_VendorList, new UIConfigData(
                    typeof(UI_VendorList),
                    "UI/Stall/UI_Stall.prefab")},

                {(int)EUIID.UI_VendorPurchase, new UIConfigData(
                    typeof(UI_VendorPurchase),
                    "UI/Stall/UI_Stall_Buy.prefab")},

                {(int)EUIID.UI_ResourceGetPath, new UIConfigData(
                    typeof(UI_ResourceGetPath),
                    "UI/Experience/UI_Experience.prefab",
                    EUIOption.eHideBeforeUI)},

                {(int)EUIID.UI_LoginLineUp, new UIConfigData(
                    typeof(UI_LoginLineUp),
                    "UI/Login/UI_Login_LineUp.prefab")},

                 {(int)EUIID.UI_Knowledge_Main, new UIConfigData(
                    typeof(UI_Knowledge_Main),
                     "UI/Knowledge/UI_Knowledge_Main.prefab",
                     EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eHideMainCamera)},

                {(int)EUIID.UI_Knowledge_Annals, new UIConfigData(
                    typeof(UI_Knowledge_Annals),
                     "UI/Knowledge/UI_Knowledge_Annals.prefab",
                     EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eHideMainCamera)},

                {(int)EUIID.UI_Knowledge_Annals_Detail, new UIConfigData(
                    typeof(UI_Knowledge_Annals_Detail),
                     "UI/Knowledge/UI_Knowledge_Annals_Detail.prefab",
                     EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eHideMainCamera)},

                {(int)EUIID.UI_Knowledge_Brave, new UIConfigData(
                    typeof(UI_Knowledge_Brave),
                     "UI/Knowledge/UI_Knowledge_Brave.prefab",
                     EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eHideMainCamera)},

                {(int)EUIID.UI_Knowledge_Brave_Detail, new UIConfigData(
                    typeof(UI_Knowledge_Brave_Detail),
                     "UI/Knowledge/UI_Knowledge_Brave_Detail.prefab",
                     EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eHideMainCamera)},

                {(int)EUIID.UI_Knowledge_Brave_Stroy, new UIConfigData(
                    typeof(UI_Knowledge_Brave_Story),
                     "UI/Knowledge/UI_Knowledge_Brave_Story.prefab")},

                {(int)EUIID.UI_Knowledge_Fragment, new UIConfigData(
                    typeof(UI_Knowledge_Fragment),
                     "UI/Knowledge/UI_Knowledge_Fragment.prefab",
                     EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eHideMainCamera)},

                {(int)EUIID.UI_Knowledge_Fragment_Detail, new UIConfigData(
                    typeof(UI_Knowledge_Fragment_Detail),
                     "UI/Knowledge/UI_Knowledge_Fragment_Detail.prefab",
                     EUIOption.eReduceFrameRate | EUIOption.eHideMainCamera)},

                {(int)EUIID.UI_Knowledge_Gleanings, new UIConfigData(
                    typeof(UI_Knowledge_Gleanings),
                     "UI/Knowledge/UI_Knowledge_Gleanings.prefab",
                     EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eHideMainCamera)},

                {(int)EUIID.UI_Knowledge_Gleanings_Info, new UIConfigData(
                    typeof(UI_Knowledge_Gleanings_Info),
                     "UI/Knowledge/UI_Knowledge_Gleanings_Info.prefab")},

                {(int)EUIID.UI_Knowledge_Award, new UIConfigData(
                    typeof(UI_Knowledge_Award),
                     "UI/Knowledge/UI_Knowledge_Award.prefab")},

                {(int)EUIID.UI_Knowledge_Unlock, new UIConfigData(
                    typeof(UI_Knowledge_Unlock),
                     "UI/Knowledge/UI_Konwledge_UnLock.prefab")},

                {(int)EUIID.UI_Pet_List, new UIConfigData(
                    typeof(UI_Pet_List),
                     "UI/Pet/UI_Pet_List.prefab")},
                {(int)EUIID.UI_Pet_SkillStudy, new UIConfigData(
                    typeof(UI_Pet_SkillStudy),
                     "UI/Pet/UI_Pet_SkillStudy.prefab")},

                {(int)EUIID.UI_Chat_VoiceInput, new UIConfigData(
                    typeof(UI_Chat_VoiceInput),
                    "UI/Chat/UI_Chat_VoiceInput.prefab") },

                 {(int)EUIID.UI_ElementalCrystal_Tip, new UIConfigData(
                    typeof(UI_ElementalCrystal_Tip),
                    "UI/ElementalCrystal/UI_ElementalCrystal_Tip.prefab") },

                 {(int)EUIID.UI_ClassicBossWar, new UIConfigData(
                    typeof(UI_ClassicBossWar),
                    "UI/LeaderWar/UI_LeaderWar.prefab",
                    EUIOption.eHideBeforeUI) },
                 {(int)EUIID.UI_ClassicBossWarEnterConfirm, new UIConfigData(
                    typeof(UI_ClassicBossWarEnterConfirm),
                    "UI/LeaderWar/UI_LeaderWar_Enter.prefab") },
                 {(int)EUIID.UI_ClassicBossWarResult, new UIConfigData(
                    typeof(UI_ClassicBossWarResult),
                    "UI/LeaderWar/UI_LeaderWarResult.prefab") },
                 {(int)EUIID.UI_Pet_Exchange, new UIConfigData(
                    typeof(UI_Pet_Exchange),
                    "UI/Pet/UI_Pet_Exchange.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate|EUIOption.eHideMainCamera|EUIOption.eReduceMainCameraQuality)},
                 {(int)EUIID.UI_Pet_Help, new UIConfigData(
                    typeof(UI_Pet_Help),
                    "UI/Pet/UI_Pet_Help.prefab")},
                  {(int)EUIID.UI_Pet_BookReview_Tip, new UIConfigData(
                    typeof(UI_Pet_BookReview_Tip),
                    "UI/Pet/UI_Pet_BookReview_Tip.prefab")},
                  {(int)EUIID.UI_ExternalNotice, new UIConfigData(
                    typeof(UI_ExternalNotice),
                    "UI/UI_ExternalNotice.prefab",
                    EUIOption.eHideBeforeUI)},

                  {(int)EUIID.UI_Team_Apply, new UIConfigData(
                    typeof(UI_Team_Member_Apply),
                    "UI/Team/UI_Team_Apply.prefab")},
                   {(int)EUIID.UI_Team_Tips, new UIConfigData(
                    typeof(UI_Team_Tips),
                    "UI/Team/UI_Team_Tips.prefab"/*,EUIOption.eIgnoreStack,9400*/)},

                  {(int)EUIID.UI_Attribute_Tips, new UIConfigData(
                    typeof(UI_Attribute_Tips),
                    "UI/Attribute/UI_Attribute_Tips.prefab")},

                  {(int)EUIID.UI_MapExploreDetail, new UIConfigData(
                    typeof(UI_MapExploreDetail),
                    "UI/Map/UI_Map_ExploreDetail.prefab")},
                  {(int)EUIID.UI_MapAreaDetail, new UIConfigData(
                    typeof(UI_MapAreaDetail),
                    "UI/Map/UI_Map_AreaDetail.prefab")},

                  {(int)EUIID.UI_Card_Tips, new UIConfigData(
                    typeof(UI_Card_Tips),
                    "UI/Pet/UI_Card_Tips.prefab")},
                  {(int)EUIID.UI_UserPartition, new UIConfigData(
                    typeof(UI_UserPartition),
                    "UI/UI_Users.prefab", EUIOption.eIgnoreStack | EUIOption.eHideMainCamera | EUIOption.eHideBeforeUI, 9410)},
                  {(int)EUIID.UI_Pet_Usage, new UIConfigData(
                    typeof(UI_Pet_Usage),
                    "UI/Pet/UI_Pet_Usage.prefab")},
                  {(int)EUIID.UI_Ornament, new UIConfigData(
                    typeof(UI_Ornament),
                    "UI/Equip/UI_Jewelry.prefab", EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eReduceMainCameraQuality)},
                  {(int)EUIID.UI_Tips_Ornament, new UIConfigData(
                    typeof(UI_Tips_Ornament),
                    "UI/Equip/UI_Tips_Jewelry.prefab")},
                  {(int)EUIID.UI_Ornament_Select, new UIConfigData(
                    typeof(UI_Ornament_Select),
                    "UI/Equip/UI_Jewelry_Select.prefab")},
                  {(int)EUIID.UI_Ornament_Result, new UIConfigData(
                    typeof(UI_Ornament_Result),
                    "UI/Equip/UI_Jewelry_Result.prefab")},
                  { (int)EUIID.UI_Head, new UIConfigData(
                    typeof(UI_Head),
                    "UI/ChangeHead/UI_ChangeHead_Main.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate)},
                  {(int)EUIID.UI_Pet_Domestication, new UIConfigData(
                    typeof(UI_Pet_Domestication),
                     "UI/Pet/UI_Pet_Domestication.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate)},
                  {(int)EUIID.UI_HelpRule, new UIConfigData(
                    typeof(UI_HelpRule),
                     "UI/UI_RuleNew.prefab")},
                  {(int)EUIID.UI_HundredPeopleArea, new UIConfigData(
                    typeof(UI_HundredPeopleArea),
                    "UI/HundredPeople/UI_HundredPeople.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate | EUIOption.eHideMainCamera)},
                  {(int)EUIID.UI_HundredPeopleAreaRank, new UIConfigData(
                    typeof(UI_HundredPeopleAreaRank),
                     "UI/HundredPeople/UI_HundredPeople_Ranking.prefab")},
                  {(int)EUIID.UI_HundredPeopelAwakenTip, new UIConfigData(
                      typeof(UI_HundredPeopelAwakenTip),
                      "UI/HundredPeople/UI_HundredPeopelAwakenTip.prefab")},
                  {(int)EUIID.UI_Setting_Tips, new UIConfigData(
                    typeof(UI_Setting_Tips),
                     "UI/UI_Setting_Tips.prefab")},
                  {(int)EUIID.UI_Fashion_Buy, new UIConfigData(
                    typeof(UI_Fashion_Buy),
                     "UI/Fashion/UI_Fashion_Buy.prefab")},

                  { (int)EUIID.UI_FirstCharge, new UIConfigData(
                    typeof(UI_FirstCharge),
                     "UI/FirstCharge/UI_FirstCharge.prefab", EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eReduceMainCameraQuality)},
                  { (int)EUIID.UI_FirstCharge_AlyIOS, new UIConfigData(
                    typeof(UI_FirstCharge_AlyIOS),
                     "UI/FirstCharge/UI_FirstCharge_IOS.prefab", EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eReduceMainCameraQuality)},

                  {(int)EUIID.UI_Trade_AppealTip, new UIConfigData(
                    typeof(UI_Trade_Appeal_Tip),
                     "UI/Trade/UI_Trade_AppealTip.prefab")},

                 {(int)EUIID.UI_Welfare_Main, new UIConfigData(
                    typeof(UI_Welfare_Main),
                     "UI/MainInterface/UI_Welfare_Main.prefab")},

                  {(int)EUIID.UI_PrompBox_Long, new UIConfigData(
                    typeof(UI_PrompBox_Long),
                    "UI/Common/UI_Tips_Long.prefab",
                    EUIOption.eIgnoreStack,9399)},
                  {(int)EUIID.UI_SurvivalPvp, new UIConfigData(
                    typeof(UI_SurvivalPvp),
                    "UI/SurvivalPvP/UI_SurvivalPvP_Enter.prefab")},
                  {(int)EUIID.UI_SurvivalPvp_Rank, new UIConfigData(
                    typeof(UI_SurvivalPvp_Rank),
                    "UI/SurvivalPvP/UI_SurvivalPvP_Rank.prefab")},
                  {(int)EUIID.UI_SurvivalPvp_Result, new UIConfigData(
                    typeof(UI_SurvivalPvp_Result),
                    "UI/SurvivalPvP/UI_SurvivalPvP_Result.prefab")},
                   {(int)EUIID.UI_Family_Construct, new UIConfigData(
                    typeof(UI_Family_Construct),
                    "UI/Family/FamilyBuild/UI_Construct.prefab",
                    EUIOption.eReduceFrameRate|EUIOption.eReduceMainCameraQuality)},
                   {(int)EUIID.UI_Family_Construct_Tips, new UIConfigData(
                    typeof(UI_Family_Construct_Tips),
                    "UI/Family/FamilyBuild/UI_Construct_Tips.prefab")},

                   {(int)EUIID.UI_SignBuy, new UIConfigData(
                    typeof(UI_SignBuy),
                     "UI/SevenDays/UI_SignBuy.prefab")},
                   {(int)EUIID.UI_Family_Construct_Submit, new UIConfigData(
                    typeof(UI_Family_Construct_Submit),
                    "UI/Family/FamilyBuild/UI_Submit.prefab")},

                   {(int)EUIID.UI_Awaken, new UIConfigData(
                    typeof(UI_Awaken),
                    "UI/Awaken/UI_Awaken.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate | EUIOption.eHideMainCamera)},

                   {(int)EUIID.UI_Awaken_Tips, new UIConfigData(
                    typeof(UI_Awaken_Tips),
                    "UI/Awaken/UI_Awaken_Tips.prefab")},
                   { (int)EUIID.UI_FamilyParty_Popup, new UIConfigData(
                    typeof(UI_FamilyParty_Popup),
                    "UI/Family/FamilyParty/UI_FamilyParty_Popup.prefab")},
                   {(int)EUIID.UI_FamilyParty_Submit, new UIConfigData(
                    typeof(UI_FamilyParty_Submit),
                    "UI/Family/FamilyParty/UI_FamilyParty_Submit.prefab", EUIOption.eHideBeforeUI )},
                   {(int)EUIID.UI_FamilyParty_Record, new UIConfigData(
                    typeof(UI_FamilyParty_Record),
                    "UI/Family/FamilyParty/UI_FamilyParty_Record.prefab")},
                   {(int)EUIID.UI_FamilyParty_StarReward, new UIConfigData(
                    typeof(UI_FamilyParty_StarReward),
                    "UI/Family/FamilyParty/UI_FamilyParty_StarReward.prefab")},
                   {(int)EUIID.UI_PetSkill_Tips, new UIConfigData(
                    typeof(UI_PetSkill_Tips),
                    "UI/Pet/UI_PetSkill_Tips.prefab")},
                    { (int)EUIID.UI_OptionalGift, new UIConfigData(
                    typeof(UI_OptionalGift),
                     "UI/Bag/UI_OptionalGift.prefab")},
                    {(int)EUIID.UI_ChatPCExpand, new UIConfigData(
                    typeof(UI_Chat),
                    "UI/Chat/UI_Chat_Pc.prefab",
                    EUIOption.eReduceFrameRate)},
                    { (int)EUIID.UI_Rewards_Result, new UIConfigData(
                    typeof(UI_Rewards_Result),
                     "UI/Bag/UI_Rewards_Result.prefab")},
                    {(int)EUIID.UI_BravePromotion, new UIConfigData(
                    typeof(UI_BravePromotion),
                    "UI/BravePromotion/UI_BravePromotion.prefab",EUIOption.eHideBeforeUI)},
                    {(int)EUIID.UI_PromotionDefeat, new UIConfigData(
                    typeof(UI_Promotion_Defeat),
                    "UI/BravePromotion/UI_Promotion_Defeat.prefab",EUIOption.eHideBeforeUI)},
                    {(int)EUIID.UI_Pet_GetCatch, new UIConfigData(
                    typeof(UI_Pet_GetCatch),
                    "UI/Pet/UI_Pet_GetCatch.prefab",EUIOption.eHideBeforeUI)},
                     {(int)EUIID.UI_PromotionList, new UIConfigData(
                    typeof(UI_Promotion_List),
                    "UI/BravePromotion/UI_Promotion_List.prefab")},
                     {(int)EUIID.UI_Unlock_Prop, new UIConfigData(
                    typeof(UI_Unlock_Prop),
                    "UI/UI_Unlock_Prop.prefab")},
                     {(int)EUIID.UI_SetPassword_Prop, new UIConfigData(
                    typeof(UI_SetPassword_Prop),
                    "UI/UI_SetPassword_Prop.prefab")},
                     {(int)EUIID.UI_ChangePassword_Prop, new UIConfigData(
                    typeof(UI_ChangePassword_Prop),
                    "UI/UI_ChangePassword_Prop.prefab")},
                     {(int)EUIID.UI_Tips_Attribute, new UIConfigData(
                    typeof(UI_Tips_Attribute),
                    "UI/Pet/UI_Tips_Attribute.prefab")},

                    {(int)EUIID.UI_Equipment_Preview, new UIConfigData(
                    typeof(UI_Equipment_Preview),
                    "UI/Equip/UI_Preview.prefab"
                    )},
                    {(int)EUIID.UI_BatchRelease, new UIConfigData(
                    typeof(UI_BatchRelease),
                    "UI/Bag/UI_BatchRelease.prefab",
                    EUIOption.eHideBeforeUI)},
                    {(int)EUIID.UI_PowerSaving, new UIConfigData(
                    typeof(UI_PowerSaving),//省电模式，层级在重连之上
                    "UI/UI_PowerSaving.prefab",EUIOption.eIgnoreStack, 9550)},

                    {(int)EUIID.UI_Cooking_Collect, new UIConfigData(
                    typeof(UI_Cooking_Collect),
                    "UI/Cooking/UI_Cooking_Collect.prefab",
                    EUIOption.eHideBeforeUI)},

                     {(int)EUIID.UI_Knowledge_RecipeCooking, new UIConfigData(
                    typeof(UI_Knowledge_RecipeCooking),
                    "UI/Knowledge/UI_Knowledge_RecipeCooking.prefab",
                    EUIOption.eHideBeforeUI)},
                     {(int)EUIID.UI_FamilySale, new UIConfigData(
                    typeof(UI_FamilySale),
                    "UI/Family/FamilySale/UI_FamilySale.prefab"
                    )},
                     { (int)EUIID.UI_Mine_Result, new UIConfigData(
                    typeof(UI_Mine_Result),
                    "UI/MainBattle/UI_Mine_Result.prefab",
                     EUIOption.eHideBeforeUI)},
                     {(int)EUIID.UI_FamilySale_My, new UIConfigData(
                    typeof(UI_FamilySale_My),
                    "UI/Family/FamilySale/UI_FamilySale_My.prefab"
                    )},
                    {(int)EUIID.UI_FamilySale_Record, new UIConfigData(
                    typeof(UI_FamilySale_Record),
                    "UI/Family/FamilySale/UI_FamilySale_Record.prefab"
                    )},
                    { (int)EUIID.UI_FamilySale_Detail, new UIConfigData(
                    typeof(UI_FamilySale_Detail),
                    "UI/Family/FamilySale/UI_FamilySale_Detail.prefab"
                    )},

                    { (int)EUIID.UI_Bio_PlayType, new UIConfigData(
                    typeof(UI_Multi_PlayTypeNew),
                    "UI/Manydungeons/UI_Manydungeons_New.prefab",EUIOption.eHideBeforeUI)},
                    { (int)EUIID.UI_Bio_Info, new UIConfigData(
                    typeof(UI_Multi_InfoNew),
                    "UI/Manydungeons/UI_Manydungeons_Inter.prefab")},
                    { (int)EUIID.UI_Bio_Vote, new UIConfigData(
                    typeof(UI_Multi_ReadyNew),
                    "UI/Team/UI_Copy_Enter.prefab",
                     EUIOption.eHideBeforeUI)},
                    { (int)EUIID.UI_Bio_LevelReward, new UIConfigData(
                    typeof(UI_Multi_LevelReward),
                    "UI/Manydungeons/UI_Manydungeons_LevelReward.prefab")},
                    {(int)EUIID.UI_Bio_Result, new UIConfigData(
                    typeof(UI_Multi_ResultNew),
                    "UI/CopyResult/UI_CopyResult.prefab",
                    EUIOption.eHideBeforeUI
                    )},

                    {(int)EUIID.UI_TutorGratitude, new UIConfigData(
                    typeof(UI_TutorGratitude),
                    "UI/UI_TutorGratitude.prefab",EUIOption.eReduceFrameRate)},
                    { (int)EUIID.UI_WorldbossBattleResult, new UIConfigData(
                    typeof(UI_WorldbossBattleResult),
                    "UI/WorldBoss/UI_WorldBoss_Result.prefab"
                    )},

                    { (int)EUIID.UI_FamilyResBattleActorList, new UIConfigData(
                    typeof(UI_FamilyResBattleActorList),
                    "UI/Family/FamilyBattle/UI_FamilyResBattleActorList.prefab"
                    )},
                    { (int)EUIID.UI_FamilyResBattleIntroduce, new UIConfigData(
                    typeof(UI_FamilyResBattleIntroduce),
                    "UI/Family/FamilyBattle/UI_FamilyResBattleIntroduce.prefab"
                    )},
                    { (int)EUIID.UI_FamilyResBattleMain, new UIConfigData(
                    typeof(UI_FamilyResBattleMain),
                    "UI/Family/FamilyBattle/UI_FamilyResBattleMain.prefab"
                    )},
                    { (int)EUIID.UI_FamilyResBattleMap, new UIConfigData(
                    typeof(UI_FamilyResBattleMap),
                    "UI/Family/FamilyBattle/UI_FamilyResBattleMap.prefab"
                    , EUIOption.eHideBeforeUI)},
                    { (int)EUIID.UI_FamilyResBattleRank, new UIConfigData(
                    typeof(UI_FamilyResBattleRank),
                    "UI/Family/FamilyBattle/UI_FamilyResBattleRank.prefab"
                    )},
                    { (int)EUIID.UI_FamilyResBattleFamilyRank, new UIConfigData(
                        typeof(UI_FamilyResBattleFamilyRank),
                        "UI/Family/FamilyBattle/UI_FamilyResBattle_FamilyRank.prefab"
                    )},
                    { (int)EUIID.UI_FamilyResBattleTeamMember, new UIConfigData(
                        typeof(UI_FamilyResBattleTeamMember),
                        "UI/Family/FamilyBattle/UI_FamilyResBattle_TeamList.prefab", 
                        EUIOption.eHideMainCamera | EUIOption.eHideBeforeUI | EUIOption.eReduceMainCameraQuality)},
                    { (int)EUIID.UI_FamilyResBattleReborn, new UIConfigData(
                    typeof(UI_FamilyResBattleReborn),
                    "UI/Family/FamilyBattle/UI_FamilyResBattleReborn.prefab"
                    )},
                    { (int)EUIID.UI_FamilyResBattleResult, new UIConfigData(
                    typeof(UI_FamilyResBattleResult),
                    "UI/Family/FamilyBattle/UI_FamilyResBattleResult.prefab"
                    )},
                    { (int)EUIID.UI_FamilyResBattleReward, new UIConfigData(
                    typeof(UI_FamilyResBattleReward),
                    "UI/Family/FamilyBattle/UI_FamilyResBattleReward.prefab"
                    )},
                    { (int)EUIID.UI_FamilyResBattleSubmitResult, new UIConfigData(
                    typeof(UI_FamilyResBattleSubmitResult),
                    "UI/Family/FamilyBattle/UI_FamilyResBattleSubmitResult.prefab"
                    )},
                    { (int)EUIID.UI_FamilyResBattleTeam, new UIConfigData(
                    typeof(UI_FamilyResBattleTeam),
                    "UI/Family/FamilyBattle/UI_FamilyResBattleTeam.prefab"
                    )},
                    { (int)EUIID.UI_FamilyResBattleTop, new UIConfigData(
                    typeof(UI_FamilyResBattleTop),
                    "UI/Family/FamilyBattle/UI_FamilyResBattleTop.prefab"
                    )},

                    { (int)EUIID.UI_FamilyResBattleBeginTip, new UIConfigData(
                    typeof(UI_FamilyResBattleBeginTip),
                    "UI/Family/FamilyBattle/UI_FamilyResBattleBeginTip.prefab"
                    )},
                     { (int)EUIID.UI_FamilyResBattleSignup, new UIConfigData(
                    typeof(UI_FamilyResBattleSignup),
                    "UI/Family/FamilyBattle/UI_FamilyResBattleSignup.prefab"
                    )},
                    { (int)EUIID.UI_FamilyResBattleMapHoldResPlayer, new UIConfigData(
                    typeof(UI_FamilyResBattleMapHoldResPlayer),
                    "UI/Family/FamilyBattle/UI_FamilyResBattleMapHoldResPlayer.prefab"
                    )},
                    { (int)EUIID.UI_FamilyResBattleCDRes, new UIConfigData(
                    typeof(UI_FamilyResBattleCDRes),
                    "UI/Family/FamilyBattle/UI_FamilyResBattleDetails.prefab"
                    )},
                    { (int)EUIID.UI_FamilyResBattlePreWin, new UIConfigData(
                    typeof(UI_FamilyResBattlePreWin),
                    "UI/Family/FamilyBattle/UI_FamilyResBattleWinDeclaration.prefab"
                    )},

                    {(int)EUIID.UI_FamilyBoss_Info, new UIConfigData(
                    typeof(UI_FamilyBoss_Info),
                    "UI/Family/FamilyBoss/UI_FamilyBoss_tips.prefab"
                    )},

                    {(int)EUIID.UI_FamilyBoss, new UIConfigData(
                    typeof(UI_FamilyBoss),
                    "UI/Family/FamilyBoss/UI_FamilyBoss.prefab",
                    EUIOption.eHideBeforeUI | EUIOption.eHideMainCamera
                    )},

                    {(int)EUIID.UI_FamilyBoss_Seize, new UIConfigData(
                    typeof(UI_FamilyBoss_Seize),
                    "UI/Family/FamilyBoss/UI_FamilyBoss_Seize.prefab")},

                     {(int)EUIID.UI_FamilyBoss_Result, new UIConfigData(
                    typeof(UI_FamilyBoss_Result),
                    "UI/Family/FamilyBoss/UI_FamilyBoss_Result.prefab",
                    EUIOption.eHideBeforeUI)},

                    {(int)EUIID.UI_FamilyBoss_RankingReward, new UIConfigData(
                    typeof(UI_FamilyBoss_RankingReward),
                    "UI/Family/FamilyBoss/UI_FamilyBoss_RankReward.prefab")},

                    {(int)EUIID.UI_FamilyBoss_Ranking, new UIConfigData(
                    typeof(UI_FamilyBoss_Ranking),
                    "UI/Family/FamilyBoss/UI_FamilyBoss_Ranking.prefab")},

                    {(int)EUIID.UI_FamilyBoss_Sure, new UIConfigData(
                    typeof(UI_FamilyBoss_Sure),
                    "UI/Family/FamilyBoss/UI_FamilyBoss_Sure.prefab")},

                    {(int)EUIID.UI_MessageBag, new UIConfigData(
                    typeof(UI_MessageBag),
                    "UI/MainInterface/UI_MessageBag.prefab"
                    )},
                    {(int)EUIID.UI_SevenDaysTarget, new UIConfigData(
                    typeof(UI_SevenDaysTarget),
                    "UI/SevenDays/UI_SevendayTarget.prefab",
                    EUIOption.eHideBeforeUI
                    )},
                    {(int)EUIID.UI_FamilyWorkshop, new UIConfigData(
                    typeof(UI_FamilyWorkshop),
                    "UI/Family/FamilyWorkshop/UI_FamilyWorkshop.prefab",
                    EUIOption.eHideBeforeUI
                    )},
                    {(int)EUIID.UI_FamilyWorkshop_Detail, new UIConfigData(
                    typeof(UI_FamilyWorkshop_Detail),
                    "UI/Family/FamilyWorkshop/UI_FamilyWorkshop_Detail.prefab"
                    )},
                    {(int)EUIID.UI_FamilyWorkshop_entrust, new UIConfigData(
                    typeof(UI_FamilyWorkshop_entrust),
                    "UI/Family/FamilyWorkshop/UI_FamilyWorkshop_entrust.prefab"
                    )},
                    { (int)EUIID.UI_FamilyWorkshop_Tips, new UIConfigData(
                    typeof(UI_FamilyWorkshop_Tips),
                    "UI/Family/FamilyWorkshop/UI_FamilyWorkshop_Tips.prefab",
                    EUIOption.eHideBeforeUI
                    )},
                   {(int)EUIID.UI_Battle_Explain, new UIConfigData(
                    typeof(UI_Battle_Explain),
                    "UI/MainBattle/UI_Battle_Explain.prefab"
                    )},
                   {(int)EUIID.UI_Sequence, new UIConfigData(
                    typeof(UI_Sequence),
                   "UI/MainBattle/UI_Sequence.prefab"
                    )},
                   {(int)EUIID.UI_Post_Prop, new UIConfigData(
                    typeof(UI_Post_Prop),
                    "UI/MainBattle/UI_Post_Prop.prefab"
                    )},
                    { (int)EUIID.UI_FamilyCreatures, new UIConfigData(
                    typeof(UI_FamilyCreatures),
                    "UI/Family/FamilyCreatures/UI_FamilyCreatures.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate|EUIOption.eHideMainCamera|EUIOption.eReduceMainCameraQuality
                    )},

                    { (int)EUIID.UI_FamilyCreatures_Feed, new UIConfigData(
                    typeof(UI_FamilyCreatures_Feed),
                    "UI/Family/FamilyCreatures/UI_FamilyCreatures_Feed.prefab",
                    EUIOption.eHideBeforeUI | EUIOption.eHideMainCamera
                    )},

                    { (int)EUIID.UI_FamilyCreatures_Get, new UIConfigData(
                    typeof(UI_FamilyCreatures_Get),
                    "UI/Family/FamilyCreatures/UI_FamilyCreatures_Get.prefab"
                    )},
                    { (int)EUIID.UI_FamilyCreatures_Notice, new UIConfigData(
                    typeof(UI_FamilyCreatures_Notice),
                    "UI/Family/FamilyCreatures/UI_FamilyCreatures_Notice.prefab"
                    )},
                    { (int)EUIID.UI_FamilyCreatures_Reward, new UIConfigData(
                    typeof(UI_FamilyCreatures_Reward),
                    "UI/Family/FamilyCreatures/UI_FamilyCreatures_Reward.prefab"
                    )},
                    { (int)EUIID.UI_FamilyCreatures_Popup, new UIConfigData(
                    typeof(UI_FamilyCreatures_Popup),
                    "UI/Family/FamilyCreatures/UI_FamilyCreatures_Popup.prefab"
                    )},
                    { (int)EUIID.UI_FamilyCreatures_Rename, new UIConfigData(
                    typeof(UI_FamilyCreatures_Rename),
                    "UI/Family/FamilyCreatures/UI_FamilyCreatures_Rename.prefab"
                    )},
                    { (int)EUIID.UI_FamilyCreatures_SetTime, new UIConfigData(
                    typeof(UI_FamilyCreatures_SetTime),
                    "UI/Family/FamilyCreatures/UI_FamilyCreatures_SetTime.prefab"
                    )},
                    { (int)EUIID.UI_FamilyCreatures_Train, new UIConfigData(
                    typeof(UI_FamilyCreatures_Train),
                    "UI/Family/FamilyCreatures/UI_FamilyCreatures_Train.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate|EUIOption.eHideMainCamera|EUIOption.eReduceMainCameraQuality
                    )},
                     { (int)EUIID.UI_FamilyCreatures_SetTrain, new UIConfigData(
                    typeof(UI_FamilyCreatures_SetTrain),
                    "UI/Family/FamilyCreatures/UI_FamilyCreatures_SetTrain.prefab"
                    )},
                       { (int)EUIID.UI_FamilyCreatures_Rank, new UIConfigData(
                    typeof(UI_FamilyCreatures_Rank),
                    "UI/Family/FamilyCreatures/UI_FamilyCreatures_Rank.prefab"
                    )},
                    { (int)EUIID.UI_RewardPanel, new UIConfigData(
                    typeof(UI_RewardPanel),
                    "UI/Common/View_RewardList.prefab"
                    )},
                    {(int)EUIID.UI_Subtitle2, new UIConfigData(
                    typeof(UI_Subtitle2),
                    "UI/Subtitle/UI_Subtitle2.prefab",
                    EUIOption.eHideBeforeUI | EUIOption.eIgnoreStack | EUIOption.eReduceFrameRate, 9250)},

                    {(int)EUIID.UI_Family_RedPacket, new UIConfigData(
                    typeof(UI_Family_RedPacket),
                    "UI/Family/FamilyRedPacket/UI_Family_RedPacket.prefab")},
                    {(int)EUIID.UI_Family_PacketRecord, new UIConfigData(
                    typeof(UI_Family_PacketRecord),
                    "UI/Family/FamilyRedPacket/UI_Family_PacketRecord.prefab",
                    EUIOption.eHideBeforeUI)},
                    {(int)EUIID.UI_Family_GivePacket, new UIConfigData(
                    typeof(UI_Family_GivePacket),
                    "UI/Family/FamilyRedPacket/UI_Family_GivePacket.prefab")},
                    {(int)EUIID.UI_SendGift, new UIConfigData(
                    typeof(UI_SendGift),
                    "UI/Society/UI_Friend_SendGift.prefab",
                    EUIOption.eReduceFrameRate | EUIOption.eReduceMainCameraQuality)},
                    { (int)EUIID.UI_BattlePass, new UIConfigData(
                    typeof(UI_BattlePass),
                    "UI/BattlePass/UI_BattlePass.prefab",
                    EUIOption.eHideBeforeUI
                    )},
                   { (int)EUIID.UI_BattlePass_Pay, new UIConfigData(
                    typeof(UI_BattlePass_VIP),
                    "UI/BattlePass/UI_BattlePass_Pay.prefab",
                    EUIOption.eHideBeforeUI
                    )},
                   { (int)EUIID.UI_BattlePass_LevelBuy, new UIConfigData(
                    typeof(UI_BattlePass_BuyLevel),
                    "UI/BattlePass/UI_BattlePass_LevelBuy.prefab"
                    )},

                   { (int)EUIID.UI_BattlePass_SpecialLv, new UIConfigData(
                    typeof(UI_BattlePass_LevelUp),
                    "UI/BattlePass/UI_BattlePass_SpecialLv.prefab"
                    )},
                   { (int)EUIID.UI_BattlePass_Collect, new UIConfigData(
                    typeof(UI_BattlePass_GotReward),
                    "UI/BattlePass/UI_BattlePass_CollectByOnce.prefab"
                    )},
                   { (int)EUIID.UI_AwakenImprint, new UIConfigData(
                    typeof(UI_AwakenImprint),
                    "UI/Awaken/Awaken_Imprint/UI_Aawken_Imprint.prefab"
                    )},
                   { (int)EUIID.UI_Awaken_Addition, new UIConfigData(
                    typeof(UI_Awaken_Addition),
                    "UI/Awaken/Awaken_Imprint/UI_Awaken_Imprint_Tips.prefab")},
                   {(int)EUIID.UI_Pet_RemakeTips, new UIConfigData(
                    typeof(UI_Pet_RemakeTips),
                    "UI/Common/UI_Tips.prefab",
                    EUIOption.eIgnoreStack,9400)},
                   { (int)EUIID.UI_Team_TalkMessage, new UIConfigData(
                    typeof(UI_Team_TalkMessage),
                    "UI/Team/UI_Team_SendNews.prefab"
                    )},
                   { (int)EUIID.UI_Friend_Attribute, new UIConfigData(
                    typeof(UI_Friend_Attribute),
                    "UI/Society/UI_Friend_Attribute.prefab",
                    EUIOption.eReduceFrameRate | EUIOption.eReduceMainCameraQuality)},
                   {(int)EUIID.UI_FamilyCreatures_Result, new UIConfigData(
                    typeof(UI_FamilyCreatures_Result),
                    "UI/Family/FamilyCreatures/UI_FamilyCreatures_Result.prefab") },
                   { (int)EUIID.UI_BattlePass_Popup, new UIConfigData(
                    typeof(UI_BattlePass_Popup),
                    "UI/BattlePass/UI_BattlePass_Popup.prefab")},
                   { (int)EUIID.UI_SevenDaysTargetPopup, new UIConfigData(
                    typeof(UI_SevenDaysTargetPopup),
                    "UI/SevenDays/UI_Face.prefab", EUIOption.eHideBeforeUI)},
                   {(int)EUIID.UI_Construct_Tips_Agri, new UIConfigData(
                    typeof(UI_Family_Construct_Tips),
                    "UI/Family/FamilyBuild/UI_Construct_Tips_Agri.prefab")},
                   { (int)EUIID.UI_Construct_Tips_Business, new UIConfigData(
                    typeof(UI_Family_Construct_Tips),
                    "UI/Family/FamilyBuild/UI_Construct_Tips_Business.prefab")},
                   { (int)EUIID.UI_Construct_Tips_Rei, new UIConfigData(
                            typeof(UI_Family_Construct_Tips),
                            "UI/Family/FamilyBuild/UI_Construct_Tips_Rei.prefab")},
                   { (int)EUIID.UI_Construct_Tips_Safe, new UIConfigData(
                            typeof(UI_Family_Construct_Tips),
                            "UI/Family/FamilyBuild/UI_Construct_Tips_Safe.prefab")},
                   { (int)EUIID.UI_Construct_Tips_Science, new UIConfigData(
                    typeof(UI_Family_Construct_Tips),
                    "UI/Family/FamilyBuild/UI_Construct_Tips_Science.prefab")},
                   {(int)EUIID.UI_Construt_Rule, new UIConfigData(
                    typeof(UI_Rule),
                    "UI/Family/FamilyBuild/UI_Construt_Rule.prefab",
                    EUIOption.eInvalid)},


                   {(int)EUIID.UI_Teaming, new UIConfigData(
                    typeof(UI_Teaming),
                    "UI/UI_Teaming.prefab",
                    EUIOption.eInvalid, -1)},

                   { (int)EUIID.UI_Fashion_LuckyDraw, new UIConfigData(
                    typeof(UI_Fashion_LuckyDraw),
                    "UI/Fashion/UI_Fashion_LuckyDraw.prefab",EUIOption.eHideMainCamera|EUIOption.eHideBeforeUI)},
                   { (int)EUIID.UI_LuckyDraw_Result, new UIConfigData(
                  typeof(UI_LuckyDraw_Result),
                    "UI/Fashion/UI_LuckyDraw_Result.prefab", EUIOption.eInvalid|EUIOption.eHideBeforeUI)},
                   { (int)EUIID.UI_Fashion_Point, new UIConfigData(
                    typeof(UI_Fashion_Point),
                    "UI/Fashion/UI_Fashion_Point.prefab", EUIOption.eInvalid)},

                   { (int)EUIID.UI_Video, new UIConfigData(
                    typeof(UI_Video_Station),
                    "UI/Video/UI_Video.prefab",
                    EUIOption.eHideBeforeUI|  EUIOption.eReduceFrameRate | EUIOption.eReduceMainCameraQuality)},
                    { (int)EUIID.UI_VideoDetails, new UIConfigData(
                    typeof(UI_Video_Message),
                    "UI/Video/UI_VideoDetails.prefab")},
                    { (int)EUIID.UI_Video_Friend, new UIConfigData(
                    typeof(UI_Share_Friend),
                    "UI/Video/UI_Video_Friend.prefab")},
                     { (int)EUIID.UI_Video_Upload, new UIConfigData(
                    typeof(UI_Video_Upload),
                    "UI/Video/UI_Video_Upload.prefab")},
                     { (int)EUIID.UI_Property_Tips, new UIConfigData(
                    typeof(UI_Property_Tips),
                    "UI/UI_Property_Tips.prefab")},

                     {(int)EUIID.UI_TimelimitGift, new UIConfigData(
                    typeof(UI_TimelimitGift),
                    "UI/SevenDays/UI_Condition_Gift.prefab") },
                     {(int)EUIID.UI_TimeLimitGift_Reward, new UIConfigData(
                    typeof(UI_TimelimitGift_Reward),
                    "UI/SevenDays/UI_Condition_Get.prefab") },
                     {(int)EUIID.UI_Face_ExpRetrieve, new UIConfigData(
                    typeof(UI_Face_ExpRetrieve),
                    "UI/SevenDays/UI_Face_ExpRetrieve.prefab") },

                    { (int)EUIID.UI_JSBattle, new UIConfigData(
                    typeof(UI_JSBattle),
                    "UI/JSBattle/UI_JSBattle.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate|EUIOption.eHideMainCamera|EUIOption.eReduceMainCameraQuality
                    )},
                    { (int)EUIID.UI_JSBattle_Rank, new UIConfigData(
                    typeof(UI_JSBattle_Rank),
                    "UI/JSBattle/UI_JSBattle_Rank.prefab")},
                    { (int)EUIID.UI_JSBattle_Record, new UIConfigData(
                    typeof(UI_JSBattle_Record),
                    "UI/JSBattle/UI_JSBattle_Record.prefab")},
                    { (int)EUIID.UI_JSBattle_Result, new UIConfigData(
                    typeof(UI_JSBattle_Result),
                    "UI/JSBattle/UI_JSBattle_Result.prefab")},
                    { (int)EUIID.UI_JSBattle_Reward, new UIConfigData(
                    typeof(UI_JSBattle_Reward),
                    "UI/JSBattle/UI_JSBattle_Reward.prefab")},
                     { (int)EUIID.UI_RedEnvelopeRain, new UIConfigData(
                    typeof(UI_RedEnvelopeRain),
                    "UI/RedEnvelopeRain/UI_RedEnvelopeRain.prefab",EUIOption.eIgnoreStack,1000)},
                     { (int)EUIID.UI_RedEnvelopeRain_Main, new UIConfigData(
                    typeof(UI_RedEnvelopeRainMain),
                    "UI/RedEnvelopeRain/UI_RedEnvelopeRain_Main.prefab")},
                     { (int)EUIID.UI_RedEnvelopeRain_Popup, new UIConfigData(
                    typeof(UI_RedEnvelopeRainPopup),
                    "UI/RedEnvelopeRain/UI_RedEnvelopeRain_Popup.prefab")},
                     { (int)EUIID.UI_RedEnvelopeRain_Remind, new UIConfigData(
                    typeof(UI_RedEnvelopeRainRemind),
                    "UI/RedEnvelopeRain/UI_RedEnvelopeRain_Remind.prefab",EUIOption.eIgnoreStack,1001)},
                     { (int)EUIID.UI_RedEnvelopeRain_Face, new UIConfigData(
                    typeof(UI_RedEnvelopeRainFace),
                    "UI/RedEnvelopeRain/UI_Face_RedEnvelopeRain.prefab",EUIOption.eHideBeforeUI)},
                     { (int)EUIID.UI_JSBattle_Tips, new UIConfigData(
                    typeof(UI_JSBattle_Tips),
                    "UI/JSBattle/UI_JSBattle_Tips.prefab")},
                    {(int)EUIID.UI_CommonCourse, new UIConfigData(
                    typeof(UI_CommonCourse),
                    "UI/UI_Course.prefab", EUIOption.eHideBeforeUI) },

                    {(int)EUIID.UI_MessageBox_Tip, new UIConfigData(
                    typeof(UI_Message_Box_Tip),
                    "UI/Common/UI_Event_sure.prefab") },
                    {(int)EUIID.UI_Construct_Continue, new UIConfigData(
                    typeof(UI_Construct_Continue),
                    "UI/Family/FamilyBuild/UI_Construct_Continue.prefab") },
                    {(int)EUIID.UI_Pet_Sale, new UIConfigData(
                    typeof(UI_Pet_Sale),
                    "UI/Pet/UI_Pet_Sale.prefab") },
                     {(int)EUIID.UI_Bag_Clear, new UIConfigData(
                    typeof(UI_Bag_Clear),
                    "UI/Bag/UI_Bag_Clear.prefab") },
                     { (int)EUIID.UI_Pet_MountSkillSelectItem, new UIConfigData(
                    typeof(UI_Pet_MountSkillSelectItem),
                    "UI/Pet/UI_Pet_MountSkillSelectItem.prefab"
                    )},
                    {(int)EUIID.UI_SpecialCardRule, new UIConfigData(
                    typeof(UI_SpecialCardRule),
                    "UI/SevenDays/UI_Specialcard_Rule.prefab") },
                    {(int)EUIID.UI_SpecialCardPresent, new UIConfigData(
                    typeof(UI_SpecialCardPresent),
                    "UI/SevenDays/UI_Present.prefab") },
                    {(int)EUIID.UI_ReName, new UIConfigData(
                    typeof(UI_ReName),
                    "UI/Attribute/UI_Rename.prefab") },
                    {(int)EUIID.UI_ChangeSchemeName, new UIConfigData(
                        typeof(UI_ChangeSchemeName),
                        "UI/UI_Plan_Rename.prefab") },
                    {(int)EUIID.UI_TownTaskSubmitResult, new UIConfigData(
                        typeof(UI_TownTaskSubmitResult),
                        "UI/TownTask/UI_TownTask_Result.prefab") },
                    {(int)EUIID.UI_ReName_Tips, new UIConfigData(
                    typeof(UI_ReName_Tips),
                    "UI/Attribute/UI_Rename_Tips.prefab") },
                    {(int)EUIID.UI_ListNumber, new UIConfigData(
                    typeof(UI_ListNumber),
                    "UI/SevenDays/UI_ListNumber.prefab") },
                    { (int)EUIID.UI_Pet_MountCharge, new UIConfigData(
                    typeof(UI_Pet_MountCharge),
                    "UI/Pet/UI_Pet_MountCharge.prefab"
                    )},
                      { (int)EUIID.UI_Pet_MountContract, new UIConfigData(
                    typeof(UI_Pet_MountContract),
                    "UI/Pet/UI_Pet_MountContract.prefab"
                    )},
                       { (int)EUIID.UI_Pet_MountConversion, new UIConfigData(
                    typeof(UI_Pet_MountConversion),
                    "UI/Pet/UI_Pet_MountConversion.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate)},
                        { (int)EUIID.UI_Pet_MountSelectItem, new UIConfigData(
                    typeof(UI_Pet_MountSelectItem),
                    "UI/Pet/UI_Pet_MountSelectItem.prefab"
                    )},
                         { (int)EUIID.UI_MountSkill_Tips, new UIConfigData(
                    typeof(UI_MountSkill_Tips),
                    "UI/Pet/UI_MountSkill_Tips.prefab"
                    )},
                    { (int)EUIID.UI_LuckyPet, new UIConfigData(
                    typeof(UI_LuckyPet),
                    "UI/SevenDays/UI_LuckyPet.prefab"
                    )},
                    { (int)EUIID.UI_Pet_MountScreem, new UIConfigData(
                    typeof(UI_Pet_MountScreem),
                    "UI/Pet/UI_Pet_MountScreem.prefab"
                    )},

                    { (int)EUIID.UI_Partner_Fetter_Detail, new UIConfigData(
                    typeof(UI_Partner_Fetter_Detail),
                    "UI/Partner/UI_FetterDetail.prefab"
                    )},

                    { (int)EUIID.UI_Bio_StageReward, new UIConfigData(
                    typeof(UI_Multi_StageReward),
                    "UI/Manydungeons/UI_Manydungeons_CopyReward.prefab"
                    )},
                    { (int)EUIID.UI_MainMenu, new UIConfigData(
                    typeof(UI_MainMenu),
                    "UI/MainInterface/UI_MainMenu.prefab"
                    )},
                    { (int)EUIID.UI_Family_JoinTips, new UIConfigData(
                    typeof(UI_Family_JoinTips),
                    "UI/Family/UI_Family_JoinTips.prefab")},

                { (int)EUIID.UI_PartnerSkillGet, new UIConfigData(
                    typeof(UI_PartnerSkillGet),
                    "UI/Partner/UI_PartnerSkillGet.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate)},

                   { (int)EUIID.UI_CurrencyRuneTips, new UIConfigData(
                    typeof(UI_CurrencyRuneTips),
                    "UI/Partner/UI_Rune_Tips.prefab")},

                   { (int)EUIID.UI_Reputation_Tips, new UIConfigData(
                    typeof(UI_Reputation_Tips),
                    "UI/Attribute/UI_Reputation_Tips.prefab")},
                    { (int)EUIID.UI_Pet_SkillTips, new UIConfigData(
                    typeof(UI_Pet_SkillTips),
                    "UI/Pet/UI_Pet_SkillTips.prefab")},

                    { (int)EUIID.UI_PetAttributeTips_Left, new UIConfigData(
                    typeof(UI_PetAttributeTips),
                    "UI/Pet/UI_PetAttributeTips_Left.prefab")},
                   { (int)EUIID.UI_PetAttributeTips_Right, new UIConfigData(
                    typeof(UI_PetAttributeTips),
                    "UI/Pet/UI_PetAttributeTips_Right.prefab")},

                    {(int)EUIID.UI_Achievement, new UIConfigData(
                    typeof(UI_Achievement),
                    "UI/Achievement/UI_Achievement.prefab", EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eReduceMainCameraQuality|EUIOption.eHideMainCamera)},
                    {(int)EUIID.UI_Achievement_Menu, new UIConfigData(
                    typeof(UI_Achievement_Menu),
                    "UI/Achievement/UI_Achievement_Menu.prefab") },
                    {(int)EUIID.UI_Achievement_Reward, new UIConfigData(
                    typeof(UI_Achievement_Reward),
                    "UI/Achievement/UI_Achievement_Reward.prefab") },
                    {(int)EUIID.UI_Achievement_RewardList, new UIConfigData(
                    typeof(UI_Achievement_RewardList),
                    "UI/Achievement/UI_Achievement_RewardList.prefab") },
                    {(int)EUIID.UI_Achievement_Share, new UIConfigData(
                    typeof(UI_Achievement_Share),
                    "UI/Achievement/UI_Achievement_Share.prefab") },
                    {(int)EUIID.UI_Achievement_ShareList, new UIConfigData(
                    typeof(UI_Achievement_ShareList),
                    "UI/Achievement/UI_Achievement_ShareList.prefab") },
                    {(int)EUIID.UI_Achievement_AccessTip, new UIConfigData(
                    typeof(UI_Achievement_AccessTip),
                    "UI/Achievement/UI_Achievement_AccessTip.prefab") },
                     { (int)EUIID.UI_JSBattle_Quick_Tip, new UIConfigData(
                    typeof(UI_JSBattle_Quick_Tip),
                    "UI/JSBattle/UI_JSBattle_Quick_Tip.prefab")},
                    {(int)EUIID.UI_Common_Tip, new UIConfigData(
                    typeof(UI_CommonTip),
                    "UI/MainInterface/View_AchievementTip.prefab") },
                    {(int)EUIID.UI_Skip, new UIConfigData(
                    typeof(UI_Skip),
                    "UI/UI_Skip.prefab") },

                    {(int)EUIID.UI_UndergroundArena, new UIConfigData(
                    typeof(UI_UndergroundArena),
                    "UI/Underground/UI_Underground.prefab",
                    EUIOption.eHideBeforeUI)},
                    {(int)EUIID.UI_UndergroundArena_Opponent, new UIConfigData(
                    typeof(UI_Underground_Opponent),
                    "UI/Underground/UI_Underground_news.prefab") },
                    {(int)EUIID.UI_UndergroundArena_Reward, new UIConfigData(
                    typeof(UI_Underground_Reward),
                    "UI/Underground/UI_Underground_Award.prefab") },
                    {(int)EUIID.UI_UndergroundArena_Rank, new UIConfigData(
                    typeof(UI_Underground_Rank),
                    "UI/Underground/UI_Underground_Rank.prefab") },
                    {(int)EUIID.UI_UndergroundArena_Result, new UIConfigData(
                    typeof(UI_Underground_Result),
                    "UI/Underground/UI_Underground_Result.prefab") },
                    {(int)EUIID.UI_UndergroundArena_Vote, new UIConfigData(
                    typeof(UI_Underground_Vote),
                    "UI/Team/UI_Copy_Enter.prefab") },

                    {(int)EUIID.UI_PetExpedition, new UIConfigData(
                    typeof(UI_PetExpedition),
                    "UI/PetExpedition/UI_PetExpedition.prefab") },
                    {(int)EUIID.UI_PetExpedition_RewardList, new UIConfigData(
                    typeof(UI_PetExpedition_RewardList),
                    "UI/PetExpedition/UI_PetExpedition_RewardList.prefab") },
                    {(int)EUIID.UI_PetExpedition_Task, new UIConfigData(
                    typeof(UI_PetExpedition_Task),
                    "UI/PetExpedition/UI_PetExpedition_Task.prefab") },
                    {(int)EUIID.UI_PetExpedition_Result, new UIConfigData(
                    typeof(UI_PetExpedition_Result),
                    "UI/PetExpedition/UI_PetExpedition_Result.prefab") },
                     {(int)EUIID.UI_Activity_Exchange, new UIConfigData(
                    typeof(UI_Activity_Exchange),
                    "UI/SevenDays/UI_Activity_Exchange.prefab") },
                      {(int)EUIID.UI_Activity_Exchange_HeFu, new UIConfigData(
                    typeof(UI_Activity_Exchange_HeFu),
                    "UI/SevenDays/UI_Activity_Exchange.prefab") },
                     {(int)EUIID.UI_Activity_Mall, new UIConfigData(
                         typeof(UI_Activity_Mall),
                         "UI/SevenDays/UI_Activity_Mall.prefab",  EUIOption.eReduceFrameRate) },
                     {(int)EUIID.UI_Activity_MallBuy, new UIConfigData(
                         typeof(UI_Activity_MallBuy),
                         "UI/SevenDays/UI_Activity_MallBuy.prefab") },

                     {(int)EUIID.UI_PetMagicCore , new UIConfigData(
                    typeof(UI_PetMagicCore ),
                    "UI/PetMagicCore/UI_PetMagicCore.prefab",EUIOption.eHideBeforeUI)},
                    {(int)EUIID.UI_PetSelectMagicMakeItem , new UIConfigData(
                    typeof(UI_PetSelectMagicMakeItem),
                    "UI/Pet/UI_Pet_MountSkillSelectItem.prefab")},
                    {(int)EUIID.UI_PetMagicCore_MakePreview , new UIConfigData(
                    typeof(UI_PetMagicCore_MakePreview),
                    "UI/PetMagicCore/UI_PetMagicCore_MakePreview.prefab")},
                     {(int)EUIID.UI_PetMagicCore_ActivateResult , new UIConfigData(
                    typeof(UI_PetMagicCore_ActivateResult),
                    "UI/PetMagicCore/UI_PetMagicCore_ActivateResult.prefab")},
                      {(int)EUIID.UI_PetMagicCore_ArtificeResult , new UIConfigData(
                    typeof(UI_PetMagicCore_ArtificeResult),
                    "UI/PetMagicCore/UI_PetMagicCore_ArtificeResult.prefab")},
                    {(int)EUIID.UI_Tips_PetMagicCore , new UIConfigData(
                    typeof(UI_Tips_PetMagicCore),
                    "UI/PetMagicCore/UI_Tips_PetMagicCore.prefab")},
                    {(int)EUIID.UI_SelectPetEquip , new UIConfigData(
                    typeof(UI_SelectPetEquip),
                    "UI/Pet/UI_Pet_MountSkillSelectItem.prefab")},
                     {(int) EUIID.UI_DescWarriorGroup, new UIConfigData(
                    typeof(UI_DescWarriorGroup),
                    "UI/BraveTeam/UI_BraveTeam_Initial.prefab",
                    EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate)},
                    {(int)EUIID.UI_CreateWarriorGroup, new UIConfigData(
                    typeof(UI_CreateWarriorGroup),
                    "UI/BraveTeam/UI_BraveTeam_Create.prefab",
                    EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate)},
                    {(int)EUIID.UI_WarriorGroup, new UIConfigData(
                    typeof(UI_WarriorGroup),
                    "UI/BraveTeam/UI_BraveTeam.prefab",
                    EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eReduceMainCameraQuality|EUIOption.eHideMainCamera)},
                     {(int)EUIID.UI_WarriorGroup_Invite, new UIConfigData(
                    typeof(UI_WarriorGroup_Invite),
                    "UI/BraveTeam/UI_BraveTeam_InviteFriend.prefab",
                    EUIOption.eReduceFrameRate)},
                      {(int)EUIID.UI_WarriorGroup_Transfer, new UIConfigData(
                    typeof(UI_WarriorGroup_Transfer),
                    "UI/BraveTeam/UI_BraveTeam_Transfer.prefab",
                    EUIOption.eReduceFrameRate)},
                    {(int)EUIID.UI_ActivityShortcut, new UIConfigData(
                    typeof(UI_ActivityShortcut),
                    "UI/Activity/UI_Activity_Tips.prefab")},
                    {(int)EUIID.UI_Activity_Summer, new UIConfigData(
                    typeof(UI_Activity_Summer),
                    "UI/SevenDays/UI_Activity_Summer.prefab") },
                    {(int)EUIID.UI_Activity_SummerSign, new UIConfigData(
                    typeof(UI_Activity_TopicSign),
                    "UI/SevenDays/UI_Activity_SummerSign.prefab") },

                    {(int)EUIID.UI_Advance_Level, new UIConfigData(
                    typeof(UI_Advance_Level),
                    "UI/Attribute/UI_Advance_Level.prefab")},

                     {(int)EUIID.UI_WarriorGroup_Sign, new UIConfigData(
                    typeof(UI_WarriorGroup_Sign),
                    "UI/BraveTeam/UI_BraveTeam_Sign.prefab",
                    EUIOption.eReduceFrameRate)},

                     {(int)EUIID.UI_WarriorGroup_CreateMeeting, new UIConfigData(
                    typeof(UI_WarriorGroup_CreateMeeting),
                    "UI/BraveTeam/UI_BraveTeam_Launch.prefab",
                    EUIOption.eReduceFrameRate)},

                     {(int)EUIID.UI_WarriorGroup_MeetingInfo, new UIConfigData(
                    typeof(UI_WarriorGroup_MeetingInfo),
                    "UI/BraveTeam/UI_BraveTeam_Vote.prefab",
                    EUIOption.eReduceFrameRate)},
                     {(int)EUIID.UI_CarrerTrans, new UIConfigData(
                     typeof(UI_CareerTrans),
                     "UI/Attribute/UI_Profession.prefab",
                     EUIOption.eHideBeforeUI | EUIOption.eHideMainCamera | EUIOption.eReduceFrameRate)},

                    {(int)EUIID.UI_Pet_Exchange_Stamp, new UIConfigData(
                    typeof(UI_Pet_Exchange_Stamp),
                    "UI/Pet/UI_Pet_Exchange_Stamp.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate|EUIOption.eHideMainCamera|EUIOption.eReduceMainCameraQuality)},
                      {(int)EUIID.UI_Pet_Advanced, new UIConfigData(
                    typeof(UI_Pet_Advanced),
                    "UI/Pet/UI_Pet_Advanced.prefab",
                    EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate|EUIOption.eHideMainCamera|EUIOption.eReduceMainCameraQuality)},

                    {(int)EUIID.UI_ActivitySavingBank, new UIConfigData(
                    typeof(UI_ActivitySavingBank),
                    "UI/SevenDays/UI_Activity_PiggyBank.prefab",
                    EUIOption.eReduceFrameRate)},
                    {(int)EUIID.UI_Activity_Topic, new UIConfigData(
                    typeof(UI_Activity_Topic),
                    "UI/SevenDays/UI_Activity_Topic.prefab",EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate | EUIOption.eHideMainCamera)},

                     {(int)EUIID.UI_Pet_SealSetting, new UIConfigData(
                       typeof(UI_Pet_SealSetting),
                    "UI/Pet/UI_Pet_SealSetting.prefab",
                     EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate|EUIOption.eReduceMainCameraQuality)},

                     {(int)EUIID.UI_KingPet, new UIConfigData(
                         typeof(UI_KingPet),
                         "UI/Lotto/UI_KingPet.prefab",EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate)},

                     {(int)EUIID.UI_KingPetReview, new UIConfigData(
                         typeof(UI_KingPetReview),
                         "UI/Lotto/UI_KingPetReview.prefab")},

                     {(int)EUIID.UI_Transfiguration_Study, new UIConfigData(
                      typeof(UI_Transfiguration_Study),
                    "UI/Transfiguration/UI_Transfiguration_Study.prefab",
                    EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eReduceMainCameraQuality|EUIOption.eHideMainCamera)},

                     {(int)EUIID.UI_Transfiguration_BookList, new UIConfigData(
                      typeof(UI_Transfiguration_BookList),
                    "UI/Transfiguration/UI_Transfiguration_BookList.prefab",
                    EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eReduceMainCameraQuality|EUIOption.eHideMainCamera)},

                     {(int)EUIID.UI_Transfiguration_Result, new UIConfigData(
                      typeof(UI_Transfiguration_Result),
                    "UI/Transfiguration/UI_Transfiguration_Result.prefab")},

                     {(int)EUIID.UI_Transfiguration_Unlock_Result, new UIConfigData(
                      typeof(UI_Transfiguration_Unlock_Result),
                    "UI/Transfiguration/UI_Transfiguration_Unlock_Result.prefab")},

                      {(int)EUIID.UI_Transfiguration_SkillTips, new UIConfigData(
                      typeof(UI_Transfiguration_SkillTips),
                    "UI/Transfiguration/UI_Transfiguration_SkillTips.prefab")},

                     {(int)EUIID.UI_Family_Sure, new UIConfigData(
                      typeof(UI_Family_Sure),
                    "UI/Family/UI_Family_Sure.prefab")},
                     {(int)EUIID.UI_BackActivity, new UIConfigData(
                      typeof(UI_BackActivity),
                    "UI/SevenDays/UI_ComeBack_Operation.prefab",EUIOption.eHideBeforeUI)},
                     {(int)EUIID.UI_BackWalfare, new UIConfigData(
                      typeof(UI_BackWalfare),
                    "UI/SevenDays/UI_ComeBack_Welfare.prefab")},
                     {(int)EUIID.UI_ExchangePointChange, new UIConfigData(
                     typeof(UI_ExchangePointChange),
                     "UI/Partner/UI_PointChange.prefab")},
                     {(int)EUIID.UI_ExchangePointReset, new UIConfigData(
                     typeof(UI_ExchangePointReset),
                     "UI/Partner/UI_PointReset.prefab")},
                     {(int)EUIID.UI_LadderPvp, new UIConfigData(
                      typeof(UI_LadderPvp),
                      "UI/TTS_Battle/UI_TTS.prefab" ,EUIOption.eHideBeforeUI)
                     },
                     {(int)EUIID.UI_LadderPvp_LevelReward, new UIConfigData(
                      typeof(UI_LadderPvp_RiseInRank),
                      "UI/TTS_Battle/UI_TTS_LevelReward.prefab")},
                     {(int)EUIID.UI_LadderPvp_Match, new UIConfigData(
                      typeof(UI_LadderPvp_Match),
                      "UI/TTS_Battle/UI_TTS_Match.prefab",EUIOption.eHideBeforeUI)
                      },
                     {(int)EUIID.UI_LadderPvp_Rank, new UIConfigData(
                      typeof(UI_LadderPvp_Rank),
                      "UI/TTS_Battle/UI_TTS_Rank.prefab")},
                     {(int)EUIID.UI_LadderPvp_RankReward, new UIConfigData(
                      typeof(UI_LadderPvp_RankReward),
                      "UI/TTS_Battle/UI_TTS_RankReward.prefab")},
                     {(int)EUIID.UI_LadderPvp_TaskReward, new UIConfigData(
                      typeof(UI_LadderPvp_Task),
                      "UI/TTS_Battle/UI_TTS_TaskReward.prefab")},
                     {(int)EUIID.UI_LadderPvp_Laoding, new UIConfigData(
                      typeof(UI_LadderPvp_Loading),
                      "UI/TTS_Battle/UI_TTS_Loading.prefab")},
                     {(int)EUIID.UI_LadderPvp_FightResult, new UIConfigData(
                      typeof(UI_LadderPvp_Over),
                      "UI/TTS_Battle/UI_TTS_Finish.prefab")},
                     {(int)EUIID.UI_LadderPvp_NewSeason, new UIConfigData(
                      typeof(UI_LadderPvp_NewSeason),
                      "UI/TTS_Battle/UI_TTS_NewSeason.prefab")},

                     {(int)EUIID.UI_TrialGateMain, new UIConfigData(
                      typeof(UI_TrialGateMain),
                    "UI/TrialGate/UI_TrialGate.prefab",
                    EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eReduceMainCameraQuality | EUIOption.eHideMainCamera)},
                     {(int)EUIID.UI_TrialSkillDeploy, new UIConfigData(
                      typeof(UI_TrialSkillDeploy),
                    "UI/TrialGate/UI_TrialGate_SkilSetting.prefab")},
                    {(int)EUIID.UI_TrialTeamDeploy, new UIConfigData(
                      typeof(UI_TrialTeamDeploy),
                    "UI/TrialGate/UI_TrialGate_TeamSetting.prefab")},
                     {(int)EUIID.UI_TrialBattleConfirm, new UIConfigData(
                      typeof(UI_TrialBattleConfirm),
                    "UI/TrialGate/UI_TrialGate_Enter.prefab")},
                     {(int)EUIID.UI_TrialRank, new UIConfigData(
                      typeof(UI_TrialRank),
                    "UI/TrialGate/UI_TrialGate_Rank.prefab")},
                     {(int)EUIID.UI_TrialBadgeTips, new UIConfigData(
                      typeof(UI_TrialBadgeTips),
                    "UI/TrialGate/UI_TrialGate_BadgeSource.prefab")}, 
                    {(int)EUIID.UI_TrialResult, new UIConfigData(
                      typeof(UI_TrialResult),
                    "UI/WorldBoss/UI_WorldBoss_Result.prefab")},
                       {(int)EUIID.UI_TrialGate_StageTip, new UIConfigData(
                      typeof(UI_TrialGate_StageTip),
                    "UI/TrialGate/UI_TrialGate_StageTip.prefab")},

                     {(int)EUIID.UI_PKCompetition, new UIConfigData(
                      typeof(UI_PKCompetition),
                      "UI/PKCompetition/UI_PKCompetition.prefab",
                      EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate | EUIOption.eHideMainCamera)},
                      {(int)EUIID.UI_PKCompetitionCreate, new UIConfigData(
                      typeof(UI_PKCompetitionCreate),
                      "UI/PKCompetition/UI_PKCompetitionCreate.prefab")},
                      {(int)EUIID.UI_PKCompetition_TeamWords, new UIConfigData(
                      typeof(UI_PKCompetition_TeamWords),
                      "UI/PKCompetition/UI_PKCompetition_TeamWords.prefab")},
                      {(int)EUIID.UI_PvP_Result, new UIConfigData(
                      typeof(UI_PvP_Result),
                      "UI/Common/UI_PvP_Result.prefab")},

                       {(int)EUIID.UI_Battle_Decide, new UIConfigData(
                      typeof(UI_Battle_Decide),
                      "UI/MainBattle/UI_Battle_Decide.prefab")},
                       {(int)EUIID.UI_Plan, new UIConfigData(
                      typeof(UI_Plan),
                      "UI/MainInterface/UI_Plan.prefab")},
                       {(int)EUIID.UI_Activity_FestivalTask, new UIConfigData(
                      typeof(UI_Activity_FestivalTask),
                      "UI/SevenDays/UI_Christmas_stock.prefab")},
                       {(int)EUIID.UI_Rewards_GetNew, new UIConfigData(
                      typeof(UI_Rewards_GetNew),
                      "UI/Bag/UI_Rewards_GetNew.prefab",EUIOption.eHideBeforeUI)},
                      { (int)EUIID.UI_Activity2048, new UIConfigData(
                      typeof(UI_Activity2048),
                      "UI/LittleGame/UI_Activity_2048.prefab",EUIOption.eHideBeforeUI)},
                       {(int)EUIID.UI_Activity2048Result, new UIConfigData(
                      typeof(UI_Activity2048Result),
                      "UI/LittleGame/UI_2048_Result.prefab")},
                       {(int)EUIID.UI_Family_Funds, new UIConfigData(
                      typeof(UI_Family_Funds),
                      "UI/Family/UI_Family_Funds.prefab")},
                       
                       {(int)EUIID.UI_Activity_Nien, new UIConfigData(
                           typeof(UI_Activity_Nien),
                           "UI/SevenDays/UI_Activity_Nien.prefab",
                           EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate | EUIOption.eReduceMainCameraQuality)},
                       
                       {(int)EUIID.UI_Activity_Nien_Challenge, new UIConfigData(
                           typeof(UI_Activity_Nien_Challenge),
                           "UI/SevenDays/UI_Activity_NienChallenge.prefab",
                           EUIOption.eHideBeforeUI|EUIOption.eReduceFrameRate | EUIOption.eReduceMainCameraQuality)},

                       {(int)EUIID.UI_Activity_Nien_ChallengeAward, new UIConfigData(
                           typeof(UI_Activity_Nien_ChallengeAward),
                           "UI/SevenDays/UI_Activity_NienChallengeAward.prefab")},
                       
                       {(int)EUIID.UI_Activity_Nien_Rank, new UIConfigData(
                           typeof(UI_Activity_Nien_Rank),
                           "UI/SevenDays/UI_Activity_NienRank.prefab")},
                       {(int)EUIID.UI_MerchantFleet, new UIConfigData(
                           typeof(UI_MerchantFleet),
                           "UI/MerchantFleet/UI_MerchantFleet.prefab")},
                       {(int)EUIID.UI_MerchantFleet_FamilyHelp, new UIConfigData(
                           typeof(UI_MerchantFleet_FamilyHelp),
                           "UI/MerchantFleet/UI_MerchantFleet_FamilyHelp.prefab")},
                       {(int)EUIID.UI_MerchantFleet_GradeAward, new UIConfigData(
                           typeof(UI_MerchantFleet_GradeAward),
                           "UI/MerchantFleet/UI_MerchantFleet_LevelReward.prefab")},
                       {(int)EUIID.UI_MerchantFleet_Settlement, new UIConfigData(
                           typeof(UI_MerchantFleet_Settlement),
                           "UI/MerchantFleet/UI_MerchantFleet_Result.prefab")},
						{(int)EUIID.UI_Pet_LevelDown, new UIConfigData(
                           typeof(UI_Pet_LevelDown),
                           "UI/Pet/UI_Pet_LevelDown.prefab")},
                        {(int)EUIID.UI_MagicChange, new UIConfigData(
                        typeof(UI_MagicChange),
                        "UI/MagicChange/UI_MagicChange.prefab",
                        EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eReduceMainCameraQuality|EUIOption.eHideMainCamera)},
                       {(int)EUIID.UI_Pet_DemonSkill_Tips, new UIConfigData(
                        typeof(UI_Pet_DemonSkill_Tips),
                        "UI/Pet/UI_Pet_DemonSkill_Tips.prefab")},
                       {(int)EUIID.UI_EquipSlot_Upgrade, new UIConfigData(
                           typeof(UI_Field_Upgrade),
                           "UI/Equip/UI_Field_Upgrade.prefab",
                           EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eReduceMainCameraQuality)},
                       {(int)EUIID.UI_EquipSlot_Upgrade_Items, new UIConfigData(
                           typeof(UI_Field_Upgrade_Items),
                           "UI/Equip/UI_FieldSelectItem.prefab")},
                       {(int)EUIID.UI_EquipSlot_Upgrade_SkillPreview, new UIConfigData(
                           typeof(UI_Field_Upgrade_SkillPreview),
                           "UI/Equip/UI_Field_Tips.prefab")},
                       {(int)EUIID.UI_Equipment_Remake_Preview, new UIConfigData(
                           typeof(UI_Equipment_Remake_Preview),
                           "UI/Equip/UI_Preview.prefab")},
                       {(int)EUIID.UI_Equipment_Inlay_GemTip, new UIConfigData(
                           typeof(UI_Equipment_Inlay_GemTip),
                           "UI/Equip/UI_Field_Tips1.prefab")},
                       {(int)EUIID.UI_Pet_FirstStart, new UIConfigData(
                           typeof(UI_Pet_FirstStart),
                           "UI/Pet/UI_Pet_FirstStart.prefab")},
                            {(int)EUIID.UI_Pet_Demon, new UIConfigData(
                        typeof(UI_Pet_Demon),
                        "UI/Pet/UI_Pet_Demon.prefab")},

                         {(int)EUIID.UI_Pet_DemonPet, new UIConfigData(
                        typeof(UI_Pet_DemonPet),
                        "UI/Pet/UI_Pet_DemonPet.prefab")},

                          {(int)EUIID.UI_Pet_DemonPreview, new UIConfigData(
                        typeof(UI_Pet_DemonPreview),
                        "UI/Pet/UI_Pet_DemonPreview.prefab")},

                           {(int)EUIID.UI_Pet_DemonSelect, new UIConfigData(
                        typeof(UI_Pet_DemonSelect),
                        "UI/Pet/UI_Pet_DemonSelect.prefab")},
                            {(int)EUIID.UI_Pet_Demon_SkillPreview, new UIConfigData(
                        typeof(UI_Pet_Demon_SkillPreview),
                        "UI/Pet/UI_Pet_Demon_SkillPreview.prefab")},
                             {(int)EUIID.UI_Pet_DemonUpgrade, new UIConfigData(
                        typeof(UI_Pet_DemonUpgrade),
                        "UI/Pet/UI_Pet_DemonUpgrade.prefab")},
                              {(int)EUIID.UI_Pet_DemonSkill_Refresh, new UIConfigData(
                        typeof(UI_Pet_DemonSkill_Refresh),
                        "UI/Pet/UI_Pet_DemonSkill_Refresh.prefab")},
                               {(int)EUIID.UI_Pet_DemonReform, new UIConfigData(
                        typeof(UI_Pet_DemonReform),
                        "UI/Pet/UI_Pet_DemonReform.prefab")},
                       {(int)EUIID.UI_Pet_Demon_Bag, new UIConfigData(
                        typeof(UI_Pet_Demon_Bag),
                        "UI/Pet/UI_Pet_Demon_Bag.prefab")},
                       {(int)EUIID.UI_Pet_Demon_Detail, new UIConfigData(
                        typeof(UI_Pet_Demon_Detail),
                        "UI/Pet/UI_Pet_Demon_Detail.prefab")},
                       {(int)EUIID.UI_Pet_DemonSuccess, new UIConfigData(
                        typeof(UI_Pet_DemonSuccess),
                        "UI/Pet/UI_Pet_DemonSuccess.prefab")},
                       {(int)EUIID.UI_PetDomesticate, new UIConfigData(
                           typeof(UI_PetDomesticate),
                           "UI/Pet/UI_Pet_DomesticatioMain.prefab",EUIOption.eHideBeforeUI)},
                       {(int)EUIID.UI_PetDomesticateTask, new UIConfigData(
                           typeof(UI_PetDomesticateTask),
                           "UI/Pet/UI_Pet_DomesticatioTask.prefab")},
                       {(int)EUIID.UI_PetDomesticateResult, new UIConfigData(
                           typeof(UI_PetDomesticateResult),
                           "UI/Pet/UI_Pet_DomesticatioResult.prefab")},
                        {(int)EUIID.UI_BossTower_Feature, new UIConfigData(
                          typeof(UI_BossTower_Feature),
                        "UI/BossGame/UI_BossGame_Specific.prefab")},
                        {(int)EUIID.UI_BossTower_FightMain, new UIConfigData(
                          typeof(UI_BossTower_FightMain),
                        "UI/BossGame/UI_BossGame_Boss.prefab",EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eReduceMainCameraQuality | EUIOption.eHideMainCamera)},
                        {(int)EUIID.UI_BossTower_Stages, new UIConfigData(
                          typeof(UI_BossTower_Stages),
                        "UI/BossGame/UI_BossGameTips.prefab")},
                        {(int)EUIID.UI_BossTower_QualifierRank, new UIConfigData(
                          typeof(UI_BossTower_QualifierRank),
                        "UI/BossGame/UI_BossGame_Rank2.prefab")},
                        {(int)EUIID.UI_BossTower_BossFightRank, new UIConfigData(
                          typeof(UI_BossTower_BossFightRank),
                        "UI/BossGame/UI_BossGame_Rank.prefab")},
                        {(int)EUIID.UI_BossTower_EnterFightVote, new UIConfigData(
                          typeof(UI_BossTower_EnterFightVote),
                        "UI/Team/UI_Copy_Enter.prefab")},
                        {(int)EUIID.UI_BossTower_QualifierMain, new UIConfigData(
                          typeof(UI_BossTower_QualifierMain),
                        "UI/BossGame/UI_BossGame_Qualifier.prefab",EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eReduceMainCameraQuality | EUIOption.eHideMainCamera)},
                        {(int)EUIID.UI_BossTower_Result, new UIConfigData(
                          typeof(UI_BossTower_Result),
                        "UI/BossGame/UI_BossGame_Result.prefab")},
                        {(int)EUIID.UI_BossTower_QualifierExplain, new UIConfigData(
                          typeof(UI_BossTower_QualifierExplain),
                        "UI/BossGame/UI_BossGameTips2.prefab")},
                         {(int)EUIID.UI_Pet_MountIntensify, new UIConfigData(
                          typeof(UI_Pet_MountIntensify),
                        "UI/Pet/UI_Pet_MountIntensify.prefab")},
						{(int)EUIID.UI_zhuanshuSkill_Tips, new UIConfigData(
                          typeof(UI_zhuanshuSkill_Tips),
                        "UI/Pet/UI_zhuanshuSkill_Tips.prefab")},
                        { (int)EUIID.UI_Pet_Appearance, new UIConfigData(
                          typeof(UI_Pet_Appearance),
                        "UI/PetAppearance/UI_Pet_Fashion.prefab",
                          EUIOption.eHideBeforeUI | EUIOption.eReduceFrameRate | EUIOption.eHideMainCamera | EUIOption.eReduceMainCameraQuality)},
                        { (int)EUIID.UI_Pet_FashionApparel, new UIConfigData(
                          typeof(UI_Pet_FashionApparel),
                        "UI/PetAppearance/UI_Pet_FashionApparel.prefab")},
                        {(int)EUIID.UI_Blessing, new UIConfigData(
                          typeof(UI_Blessing),
                        "UI/Blessing/UI_Blessing.prefab")},
                        {(int)EUIID.UI_Blessing_Info, new UIConfigData(
                          typeof(UI_BlessingInfo),
                        "UI/Blessing/UI_Blessing_Ex.prefab")},

                        {(int)EUIID.UI_Blessing_Result, new UIConfigData(
                          typeof(UI_BlessingResult),
                        "UI/Blessing/UI_Blessing_Result.prefab")},

                        {(int)EUIID.UI_Transfiguration_Tips, new UIConfigData(
                          typeof(UI_Transfiguration_Tips),
                        "UI/Transfiguration/UI_UI_Transfiguration_Tips.prefab")},

        };
    }
}
