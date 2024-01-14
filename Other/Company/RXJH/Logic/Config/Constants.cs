using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    /// <summary>
    /// Logic中的常量类///
    /// </summary>
    public class Constants
    {
        #region ShowWeaponID

        public const uint UMARMEDID = 1001;
        public const uint SWORDID = 2001;

        #endregion

        #region Animator

        public const int CHARPARAM = 100000;
        public const int WEAPONTYPEPARAM = 10000;

        public const float CORSSFADETIME = 0.4f;

        #endregion

        #region Suffix

        public const string ANIMSUFFIX = ".anim";

        #endregion     

        #region Skill

        public const uint DEFFENSESKILLID = 1;
        public const uint ESCAPESKILLID = 2;
        public const uint PETINSKILLID = 3;
        public const uint PETOUTSKILLID = 4;
        public const uint NORMALATTACKSKILLID = 100;

        public const uint DEFFENSEID = 101;
        public const uint ESCAPEID = 102;
        public const uint PETINID = 103;
        public const uint PETOUTID = 104;
        public const uint NEARNORMALATTACKID = 1001;
        public const uint FARNORMALATTACKID = 1002;

        #endregion

        #region Fight

        public const int OPERATIONTIME = 30;
        public const int AUTOTIME = 3;
        public const int BUFFSHOWCLICKTIME = 1;

        #endregion

        #region ChatColors
        public const string gChatColor_Name = "BE6E39";
        public const string gChatColorAddFriend = "0000ff";
        public const string gChatColorTask = "861c36";
        public const string gChatColorJoinTeam = "00ff00";
        public const string gChatColorTitle = "009f3c";
        public const string gChatColorPet = "00ff00";
        public const string gChatColorAchievement = "30dbe1";
        public static readonly string[] gChatColors_Item = new string[5] { "858E92", "4EAB85", "3291BD", "AE7AC9", "CC923E" };

        //白 绿 蓝 紫 橙
        public static readonly string[] gChatColors_Items = new string[5] { "96bdca", "3ebd2e", "3d9fd2", "b341d9", "d67e3c" };//系统频道道具品质字色
        public static readonly string[] gHintColors_Items = new string[5] { "b5b0c1", "44e462", "48b7f1", "ce4ff8", "e48844" };//道具飘字品质字色
        #endregion

        #region ChatChannelIcon
        public const int ChatChannelIconID_World = 992101;      //世界
        public const int ChatChannelIconID_Person = 992102;     //个人
        public const int ChatChannelIconID_Local = 992103;      //当前
        public const int ChatChannelIconID_Guild = 992104;      //家族
        public const int ChatChannelIconID_Team = 992105;       //队伍
        public const int ChatChannelIconID_LookForTeam = 992106;//组队
        public const int ChatChannelIconID_System = 992107;     //系统
        public const int ChatChannelIconID_Notice = 992107;     //公告
        public const int ChatChannelIconID_Horn = 992108;       //喇叭
        public const int ChatChannelIconID_Career = 992112;     //职业
        public const int ChatChannelIconID_BraveGroup = 992113; //TODO：112893 //职业 


        public const int ChatChannelIconID_Hero = 992111;        //剧情(玩家)
        public const int ChatChannelIconID_Monster = 992109;    //怪物
        public const int ChatChannelIconID_Partner = 992110;    //伙伴

        #endregion

        #region EquipTipQualityIcon
        public const string TipBgWhite = "Texture/Big/Common_tips_white.png";
        public const string TipBgGreen = "Texture/Big/Common_tips_green.png";
        public const string TipBgBlue = "Texture/Big/Common_tips_blue.png";
        public const string TipBgPurple = "Texture/Big/Common_tips_purple.png";
        public const string TipBgOrange = "Texture/Big/Common_tips_orange.png";
        #endregion

        #region SkillPlay
        public const string SceneModelPath = "Prefab/ShowScene/skillShowScene.prefab";

        #endregion

        public const int NOCAREERID = 100;

        #region Society

        public const uint CUSTOMFRIENDGROUPCOUNTMAX = 4;

        #endregion

        public const string FANXIEGANG = "/";

        #region CSVWordStytle ID
        public const uint WORD_STYLE_22 = 22;           //不满足文字（数字字体）
        public const uint OUTFIGHTHERONAMR = 30;        //主角、队友（战斗外）
        public const uint OUTFIGHTOTHERHERONAME = 31;   //其他玩家（战斗外）
        public const uint OUTFIGHTNPCNAME = 32;         //npc、怪物（战斗外）
        public const uint OUTFIGHTNPCAPPEL = 33;        //npc称谓（战斗外）
        public const uint OUTFIGHTAPPEL = 34;           //玩家称谓（战斗外）临时
        public const uint INFIGHTHERONAME = 37;         //主角、队友、npc（战斗内）
        public const uint INFIGHTHMONSTERNAME = 38;     //怪物（战斗内）
        public const uint INFIGHTPARTNERNAME = 39;      //伙伴（战斗内）临时   
        public const uint INFIGHTHPETNAME = 40;         //宠物（战斗内）临时
        #endregion

        #region Weather
        public const uint CSVID_RainAudio = 5000;
        public const uint CSVID_SnowAudio = 5001;
        public const uint CSVID_ThunderstormAudio = 5002;
        public const uint CSVID_FlashingAudio = 5005;
        #endregion

        public const string FxPetBule = "Prefab/Fx/fx_PetGear_lan.prefab";
        public const string FxPetPurple = "Prefab/Fx/fx_PetGear_zi.prefab";
        public const string FxPetOrange = "Prefab/Fx/fx_PetGear_cheng.prefab";

        #region Family

        public const uint FamilyMapId = 1510;
        public const uint FamilyCastleMapId = 1521;//家族城堡

        #endregion

        #region Pet
        /// <summary> 安全锁限制此品种以下不处理 </summary>
        public const uint PetPurpleNum = 4;
        public const uint Max_DeficiencyGear = 20;
        public const string PETWEAR_EQUIP = "Equip";

        #endregion

        #region Tint
        public static readonly int[] kShaderTintIDs = { Shader.PropertyToID("_ColorR"), Shader.PropertyToID("_ColorG"), Shader.PropertyToID("_ColorB"), Shader.PropertyToID("_ColorA") };
        public static readonly int kUseTintColor = Shader.PropertyToID("_UseTintColor");
        public static readonly int kHairShaderOutLineID = Shader.PropertyToID("_Outline_Color");
        #endregion
    }
}
