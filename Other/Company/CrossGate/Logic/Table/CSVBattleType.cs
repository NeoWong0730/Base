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

	sealed public partial class CSVBattleType : Framework.Table.TableBase<CSVBattleType.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint battle_type;
			public readonly uint battle_type_param;
			public readonly uint is_escape;
			public readonly uint show_battle_victory;
			public readonly uint IntoAnimationTime;
			public readonly uint max_round;
			public readonly uint battle_end_recovery;
			public readonly uint victory_condition_type;
			public readonly List<uint> victory_condition;
			public readonly uint auto_fight_mode;
			public readonly uint setup_time;
			public bool is_seal { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public bool is_order_cancel { get { return ReadHelper.GetBoolByIndex(boolArray0, 1); } }
			public readonly uint init_show_speed;
			public bool is_speed_up { get { return ReadHelper.GetBoolByIndex(boolArray0, 2); } }
			public readonly uint show_boss_id;
			public bool Auto_Fight_Func { get { return ReadHelper.GetBoolByIndex(boolArray0, 3); } }
			public readonly uint battle_type_name;
			public bool show_self_hp { get { return ReadHelper.GetBoolByIndex(boolArray0, 4); } }
			public bool show_self_mp { get { return ReadHelper.GetBoolByIndex(boolArray0, 5); } }
			public bool show_enemy_hp { get { return ReadHelper.GetBoolByIndex(boolArray0, 6); } }
			public bool show_enemy_mp { get { return ReadHelper.GetBoolByIndex(boolArray0, 7); } }
			public bool show_self_BuffHUD { get { return ReadHelper.GetBoolByIndex(boolArray1, 0); } }
			public bool show_enemy_BuffHUD { get { return ReadHelper.GetBoolByIndex(boolArray1, 1); } }
			public bool show_self_name { get { return ReadHelper.GetBoolByIndex(boolArray1, 2); } }
			public bool show_enemy_name { get { return ReadHelper.GetBoolByIndex(boolArray1, 3); } }
			public bool show_UI_hp { get { return ReadHelper.GetBoolByIndex(boolArray1, 4); } }
			public bool show_instruct_info { get { return ReadHelper.GetBoolByIndex(boolArray1, 5); } }
			public bool show_self_element { get { return ReadHelper.GetBoolByIndex(boolArray1, 6); } }
			public bool show_enemy_element { get { return ReadHelper.GetBoolByIndex(boolArray1, 7); } }
			public readonly uint show_hp_tie;
			public bool mirror_position { get { return ReadHelper.GetBoolByIndex(boolArray2, 0); } }
			public readonly uint position_type;
			public bool running_enter_self { get { return ReadHelper.GetBoolByIndex(boolArray2, 1); } }
			public readonly uint enter_battle_effect;
			public readonly uint exit_battle_effect;
			public bool is_quickfight { get { return ReadHelper.GetBoolByIndex(boolArray2, 2); } }
			public readonly uint battle_end_workid;
			public readonly uint barrage;
			public readonly uint ob;
			public readonly uint CrystalDurability;
			public readonly uint durability;
			public readonly uint MountEnergy;
			public readonly List<uint> normal_medic;
			public readonly uint normal_medic_num;
			public readonly List<uint> special_medic;
			public readonly uint special_medic_num;
			public readonly uint pet_battletimes;
			public readonly List<uint> forbid_medic;
			public bool proficiency { get { return ReadHelper.GetBoolByIndex(boolArray2, 3); } }
			public readonly List<uint> AidBasePoint;
			public readonly uint AidUpperLimit;
			public readonly uint AidLevel;
			public readonly int AidLevelScope;
			public readonly uint AidMultiple;
			public readonly List<uint> CaptainBasePoint;
			public readonly uint CaptainUpperLimit;
			public readonly uint battle_bgm;
			public readonly uint battle_Bubble;
			public readonly uint CloseReward;
			public readonly uint Battle_Watch;
			public readonly uint enter_battle_effectON;
			public readonly uint enter_battle_effectOFF;
			public readonly uint exit_battle_effectON;
			public readonly uint exit_battle_effectOFF;
			public readonly uint enter_battle_bgmON;
			public readonly uint enter_battle_bgmOFF;
			public readonly uint block_family_advice;
			public readonly uint block_friend_advice;
			public readonly uint videoFirst;
			public readonly List<uint> stageSwitch_id;
			public readonly List<uint> Limit_Skill;
			public readonly List<uint> Limit_Item;
			public readonly uint isTrialGate;
			public readonly uint BlessCloseReward;
		private readonly byte boolArray0;
		private readonly byte boolArray1;
		private readonly byte boolArray2;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				battle_type = ReadHelper.ReadUInt(binaryReader);
				battle_type_param = ReadHelper.ReadUInt(binaryReader);
				is_escape = ReadHelper.ReadUInt(binaryReader);
				show_battle_victory = ReadHelper.ReadUInt(binaryReader);
				IntoAnimationTime = ReadHelper.ReadUInt(binaryReader);
				max_round = ReadHelper.ReadUInt(binaryReader);
				battle_end_recovery = ReadHelper.ReadUInt(binaryReader);
				victory_condition_type = ReadHelper.ReadUInt(binaryReader);
				victory_condition = shareData.GetShareData<List<uint>>(binaryReader, 0);
				auto_fight_mode = ReadHelper.ReadUInt(binaryReader);
				setup_time = ReadHelper.ReadUInt(binaryReader);
				init_show_speed = ReadHelper.ReadUInt(binaryReader);
				show_boss_id = ReadHelper.ReadUInt(binaryReader);
				battle_type_name = ReadHelper.ReadUInt(binaryReader);
				show_hp_tie = ReadHelper.ReadUInt(binaryReader);
				position_type = ReadHelper.ReadUInt(binaryReader);
				enter_battle_effect = ReadHelper.ReadUInt(binaryReader);
				exit_battle_effect = ReadHelper.ReadUInt(binaryReader);
				battle_end_workid = ReadHelper.ReadUInt(binaryReader);
				barrage = ReadHelper.ReadUInt(binaryReader);
				ob = ReadHelper.ReadUInt(binaryReader);
				CrystalDurability = ReadHelper.ReadUInt(binaryReader);
				durability = ReadHelper.ReadUInt(binaryReader);
				MountEnergy = ReadHelper.ReadUInt(binaryReader);
				normal_medic = shareData.GetShareData<List<uint>>(binaryReader, 0);
				normal_medic_num = ReadHelper.ReadUInt(binaryReader);
				special_medic = shareData.GetShareData<List<uint>>(binaryReader, 0);
				special_medic_num = ReadHelper.ReadUInt(binaryReader);
				pet_battletimes = ReadHelper.ReadUInt(binaryReader);
				forbid_medic = shareData.GetShareData<List<uint>>(binaryReader, 0);
				AidBasePoint = shareData.GetShareData<List<uint>>(binaryReader, 0);
				AidUpperLimit = ReadHelper.ReadUInt(binaryReader);
				AidLevel = ReadHelper.ReadUInt(binaryReader);
				AidLevelScope = ReadHelper.ReadInt(binaryReader);
				AidMultiple = ReadHelper.ReadUInt(binaryReader);
				CaptainBasePoint = shareData.GetShareData<List<uint>>(binaryReader, 0);
				CaptainUpperLimit = ReadHelper.ReadUInt(binaryReader);
				battle_bgm = ReadHelper.ReadUInt(binaryReader);
				battle_Bubble = ReadHelper.ReadUInt(binaryReader);
				CloseReward = ReadHelper.ReadUInt(binaryReader);
				Battle_Watch = ReadHelper.ReadUInt(binaryReader);
				enter_battle_effectON = ReadHelper.ReadUInt(binaryReader);
				enter_battle_effectOFF = ReadHelper.ReadUInt(binaryReader);
				exit_battle_effectON = ReadHelper.ReadUInt(binaryReader);
				exit_battle_effectOFF = ReadHelper.ReadUInt(binaryReader);
				enter_battle_bgmON = ReadHelper.ReadUInt(binaryReader);
				enter_battle_bgmOFF = ReadHelper.ReadUInt(binaryReader);
				block_family_advice = ReadHelper.ReadUInt(binaryReader);
				block_friend_advice = ReadHelper.ReadUInt(binaryReader);
				videoFirst = ReadHelper.ReadUInt(binaryReader);
				stageSwitch_id = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Limit_Skill = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Limit_Item = shareData.GetShareData<List<uint>>(binaryReader, 0);
				isTrialGate = ReadHelper.ReadUInt(binaryReader);
				BlessCloseReward = ReadHelper.ReadUInt(binaryReader);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
			boolArray1 = ReadHelper.ReadByte(binaryReader);
			boolArray2 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVBattleType.bytes";
		}

		private static CSVBattleType instance = null;			
		public static CSVBattleType Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBattleType 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBattleType forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBattleType();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBattleType");

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
			shareData.ReadArrays<uint>(binaryReader, 0, ReadHelper.ReadArray_ReadUInt);

			return shareData;
		}
	}

#else

    sealed public partial class CSVBattleType : FCSVBattleType
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVBattleType.bytes";
		}

		private static CSVBattleType instance = null;			
		public static CSVBattleType Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBattleType 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBattleType forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBattleType();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBattleType");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}