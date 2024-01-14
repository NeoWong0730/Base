//
#define USE_HOTFIX_LOGIC

using Lib.AssetLoader;
using Lib.Core;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Framework.Table;

namespace Table
{
#if USE_HOTFIX_LOGIC

	sealed public partial class CSVAction : Framework.Table.TableBase<CSVAction.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly string dirPath;
			public readonly uint hero_id;
			public readonly uint weapon_type;
			public readonly uint weapon_id;
			public bool role { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public readonly string npc_show1;
			public readonly string npc_show2;
			public readonly string npc_show3;
			public readonly string action_login;
			public readonly string action_collection;
			public readonly string action_collection2;
			public readonly string action_collection3;
			public readonly string action_LossLife;
			public readonly string action_EndLife;
			public readonly string action_detect;
			public readonly string action_detect_end;
			public readonly string action_idle;
			public readonly string action_show_idle;
			public readonly string action_career;
			public readonly string mount_stand;
			public readonly string mount_sprint;
			public readonly string action_walk;
			public readonly string action_run;
			public readonly string action_escape;
			public readonly string action_escape_failure;
			public readonly string action_die1;
			public readonly string action_die2;
			public readonly string action_die3;
			public readonly string action_defense;
			public readonly string action_be_hit;
			public readonly string action_alliance;
			public readonly string action_hit_fly;
			public readonly string action_hit_down;
			public readonly string action_sprit_prepare;
			public readonly string action_battle_move;
			public readonly string action_battle_move01;
			public readonly string action_normal_attack;
			public readonly string action_normal_attack01;
			public readonly string action_normal_attack02;
			public readonly string action_normal_attack03;
			public readonly string action_normal_attack04;
			public readonly string action_normal_attack05;
			public readonly string action_normal_attack06;
			public readonly string action_special_attack01;
			public readonly string action_special_attack02;
			public readonly string action_cast;
			public readonly string action_randomshot_start;
			public readonly string action_randomshot_loop;
			public readonly string action_randomshot_loop_end;
			public readonly string ui_show_entrance;
			public readonly string ui_show_idle;
			public readonly string ui_show_idleshow01;
			public readonly string ui_show_idleshow02;
			public readonly string action_depressed;
			public readonly string action_laugh;
			public readonly string action_sad;
			public readonly string action_agree;
			public readonly string action_nervous;
			public readonly string action_shy;
			public readonly string action_uplift;
			public readonly string action_ponder;
			public readonly string action_resolve;
			public readonly string action_cry;
			public readonly string action_toothache;
			public readonly string action_mad;
			public readonly string action_book;
			public readonly string action_pray;
			public readonly string action_however;
			public readonly string action_angry;
			public readonly string action_doubt;
			public readonly string action_satisfy;
			public readonly string action_jump;
			public readonly string action_wave;
			public readonly string action_think1;
			public readonly string action_surprise;
			public readonly string action_talk;
			public readonly string action_dialogue_idle;
			public readonly string action_special;
			public readonly string action_special_02;
			public readonly string action_special_03;
			public readonly string action_special_04;
			public readonly string ui_show;
			public readonly string action_special_05;
			public readonly string boss_born;
			public readonly string boss_failure;
			public readonly string boss_fighting;
			public readonly string boss_win;
			public readonly string chara_Motion_1;
			public readonly string chara_Motion_2;
			public readonly string chara_Motion_3;
			public readonly string chara_Motion_4;
			public readonly string chara_Motion_5;
			public readonly string chara_Motion_6;
			public readonly string chara_Motion_7;
			public readonly string chara_Motion_8;
			public readonly string chara_Motion_9;
			public readonly string action_mining;
			public readonly string action_logging;
			public readonly string action_fish;
			public readonly string action_hunt;
			public readonly string action_mount_1_inquiry;
			public readonly string action_mount_1_idle;
			public readonly string action_mount_1_show_idle;
			public readonly string action_mount_1_walk;
			public readonly string action_mount_1_run;
			public readonly string action_mount_2_inquiry;
			public readonly string action_mount_2_idle;
			public readonly string action_mount_2_show_idle;
			public readonly string action_mount_2_walk;
			public readonly string action_mount_2_run;
			public readonly string action_collected;
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				dirPath = shareData.GetShareData<string>(binaryReader, 0);
				hero_id = ReadHelper.ReadUInt(binaryReader);
				weapon_type = ReadHelper.ReadUInt(binaryReader);
				weapon_id = ReadHelper.ReadUInt(binaryReader);
				npc_show1 = shareData.GetShareData<string>(binaryReader, 0);
				npc_show2 = shareData.GetShareData<string>(binaryReader, 0);
				npc_show3 = shareData.GetShareData<string>(binaryReader, 0);
				action_login = shareData.GetShareData<string>(binaryReader, 0);
				action_collection = shareData.GetShareData<string>(binaryReader, 0);
				action_collection2 = shareData.GetShareData<string>(binaryReader, 0);
				action_collection3 = shareData.GetShareData<string>(binaryReader, 0);
				action_LossLife = shareData.GetShareData<string>(binaryReader, 0);
				action_EndLife = shareData.GetShareData<string>(binaryReader, 0);
				action_detect = shareData.GetShareData<string>(binaryReader, 0);
				action_detect_end = shareData.GetShareData<string>(binaryReader, 0);
				action_idle = shareData.GetShareData<string>(binaryReader, 0);
				action_show_idle = shareData.GetShareData<string>(binaryReader, 0);
				action_career = shareData.GetShareData<string>(binaryReader, 0);
				mount_stand = shareData.GetShareData<string>(binaryReader, 0);
				mount_sprint = shareData.GetShareData<string>(binaryReader, 0);
				action_walk = shareData.GetShareData<string>(binaryReader, 0);
				action_run = shareData.GetShareData<string>(binaryReader, 0);
				action_escape = shareData.GetShareData<string>(binaryReader, 0);
				action_escape_failure = shareData.GetShareData<string>(binaryReader, 0);
				action_die1 = shareData.GetShareData<string>(binaryReader, 0);
				action_die2 = shareData.GetShareData<string>(binaryReader, 0);
				action_die3 = shareData.GetShareData<string>(binaryReader, 0);
				action_defense = shareData.GetShareData<string>(binaryReader, 0);
				action_be_hit = shareData.GetShareData<string>(binaryReader, 0);
				action_alliance = shareData.GetShareData<string>(binaryReader, 0);
				action_hit_fly = shareData.GetShareData<string>(binaryReader, 0);
				action_hit_down = shareData.GetShareData<string>(binaryReader, 0);
				action_sprit_prepare = shareData.GetShareData<string>(binaryReader, 0);
				action_battle_move = shareData.GetShareData<string>(binaryReader, 0);
				action_battle_move01 = shareData.GetShareData<string>(binaryReader, 0);
				action_normal_attack = shareData.GetShareData<string>(binaryReader, 0);
				action_normal_attack01 = shareData.GetShareData<string>(binaryReader, 0);
				action_normal_attack02 = shareData.GetShareData<string>(binaryReader, 0);
				action_normal_attack03 = shareData.GetShareData<string>(binaryReader, 0);
				action_normal_attack04 = shareData.GetShareData<string>(binaryReader, 0);
				action_normal_attack05 = shareData.GetShareData<string>(binaryReader, 0);
				action_normal_attack06 = shareData.GetShareData<string>(binaryReader, 0);
				action_special_attack01 = shareData.GetShareData<string>(binaryReader, 0);
				action_special_attack02 = shareData.GetShareData<string>(binaryReader, 0);
				action_cast = shareData.GetShareData<string>(binaryReader, 0);
				action_randomshot_start = shareData.GetShareData<string>(binaryReader, 0);
				action_randomshot_loop = shareData.GetShareData<string>(binaryReader, 0);
				action_randomshot_loop_end = shareData.GetShareData<string>(binaryReader, 0);
				ui_show_entrance = shareData.GetShareData<string>(binaryReader, 0);
				ui_show_idle = shareData.GetShareData<string>(binaryReader, 0);
				ui_show_idleshow01 = shareData.GetShareData<string>(binaryReader, 0);
				ui_show_idleshow02 = shareData.GetShareData<string>(binaryReader, 0);
				action_depressed = shareData.GetShareData<string>(binaryReader, 0);
				action_laugh = shareData.GetShareData<string>(binaryReader, 0);
				action_sad = shareData.GetShareData<string>(binaryReader, 0);
				action_agree = shareData.GetShareData<string>(binaryReader, 0);
				action_nervous = shareData.GetShareData<string>(binaryReader, 0);
				action_shy = shareData.GetShareData<string>(binaryReader, 0);
				action_uplift = shareData.GetShareData<string>(binaryReader, 0);
				action_ponder = shareData.GetShareData<string>(binaryReader, 0);
				action_resolve = shareData.GetShareData<string>(binaryReader, 0);
				action_cry = shareData.GetShareData<string>(binaryReader, 0);
				action_toothache = shareData.GetShareData<string>(binaryReader, 0);
				action_mad = shareData.GetShareData<string>(binaryReader, 0);
				action_book = shareData.GetShareData<string>(binaryReader, 0);
				action_pray = shareData.GetShareData<string>(binaryReader, 0);
				action_however = shareData.GetShareData<string>(binaryReader, 0);
				action_angry = shareData.GetShareData<string>(binaryReader, 0);
				action_doubt = shareData.GetShareData<string>(binaryReader, 0);
				action_satisfy = shareData.GetShareData<string>(binaryReader, 0);
				action_jump = shareData.GetShareData<string>(binaryReader, 0);
				action_wave = shareData.GetShareData<string>(binaryReader, 0);
				action_think1 = shareData.GetShareData<string>(binaryReader, 0);
				action_surprise = shareData.GetShareData<string>(binaryReader, 0);
				action_talk = shareData.GetShareData<string>(binaryReader, 0);
				action_dialogue_idle = shareData.GetShareData<string>(binaryReader, 0);
				action_special = shareData.GetShareData<string>(binaryReader, 0);
				action_special_02 = shareData.GetShareData<string>(binaryReader, 0);
				action_special_03 = shareData.GetShareData<string>(binaryReader, 0);
				action_special_04 = shareData.GetShareData<string>(binaryReader, 0);
				ui_show = shareData.GetShareData<string>(binaryReader, 0);
				action_special_05 = shareData.GetShareData<string>(binaryReader, 0);
				boss_born = shareData.GetShareData<string>(binaryReader, 0);
				boss_failure = shareData.GetShareData<string>(binaryReader, 0);
				boss_fighting = shareData.GetShareData<string>(binaryReader, 0);
				boss_win = shareData.GetShareData<string>(binaryReader, 0);
				chara_Motion_1 = shareData.GetShareData<string>(binaryReader, 0);
				chara_Motion_2 = shareData.GetShareData<string>(binaryReader, 0);
				chara_Motion_3 = shareData.GetShareData<string>(binaryReader, 0);
				chara_Motion_4 = shareData.GetShareData<string>(binaryReader, 0);
				chara_Motion_5 = shareData.GetShareData<string>(binaryReader, 0);
				chara_Motion_6 = shareData.GetShareData<string>(binaryReader, 0);
				chara_Motion_7 = shareData.GetShareData<string>(binaryReader, 0);
				chara_Motion_8 = shareData.GetShareData<string>(binaryReader, 0);
				chara_Motion_9 = shareData.GetShareData<string>(binaryReader, 0);
				action_mining = shareData.GetShareData<string>(binaryReader, 0);
				action_logging = shareData.GetShareData<string>(binaryReader, 0);
				action_fish = shareData.GetShareData<string>(binaryReader, 0);
				action_hunt = shareData.GetShareData<string>(binaryReader, 0);
				action_mount_1_inquiry = shareData.GetShareData<string>(binaryReader, 0);
				action_mount_1_idle = shareData.GetShareData<string>(binaryReader, 0);
				action_mount_1_show_idle = shareData.GetShareData<string>(binaryReader, 0);
				action_mount_1_walk = shareData.GetShareData<string>(binaryReader, 0);
				action_mount_1_run = shareData.GetShareData<string>(binaryReader, 0);
				action_mount_2_inquiry = shareData.GetShareData<string>(binaryReader, 0);
				action_mount_2_idle = shareData.GetShareData<string>(binaryReader, 0);
				action_mount_2_show_idle = shareData.GetShareData<string>(binaryReader, 0);
				action_mount_2_walk = shareData.GetShareData<string>(binaryReader, 0);
				action_mount_2_run = shareData.GetShareData<string>(binaryReader, 0);
				action_collected = shareData.GetShareData<string>(binaryReader, 0);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVAction.bytes";
		}

		private static CSVAction instance = null;			
		public static CSVAction Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVAction 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVAction forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVAction();
                instance.ReadByFilePath(ConfigPath(), OnCreat, OnReadShareData);
            }
            else if (forceReload)
            {
                instance.Clear();
                instance.ReadByFilePath(ConfigPath(), OnCreat, OnReadShareData);
            }
        }

        public static void Unload()
        {
            DebugUtil.Log(ELogType.eTable, "卸载CSVAction");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }

        private static Data OnCreat(uint id, BinaryReader binaryReader, TableShareData shareData)
        {
            Data data = new Data(id, binaryReader, shareData);
            return data;
        }

        private static TableShareData OnReadShareData(BinaryReader binaryReader)
		{
			TableShareData shareData = new TableShareData(1);
			shareData.ReadStrings(binaryReader, 0);

			return shareData;
		}
	}

#else

    sealed public partial class CSVAction : FCSVAction
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVAction.bytes";
		}

		private static CSVAction instance = null;			
		public static CSVAction Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVAction 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVAction forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVAction();
                instance.ReadByFilePath(ConfigPath());
            }
            else if (forceReload)
            {
                instance.Clear();
                instance.ReadByFilePath(ConfigPath());
            }
        }

        public static void Unload()
        {
            DebugUtil.Log(ELogType.eTable, "卸载CSVAction");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}