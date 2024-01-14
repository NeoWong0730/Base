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

	sealed public partial class CSVActiveSkill : Framework.Table.TableBase<CSVActiveSkill.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint main_skill_id;
			public readonly uint active_skill_behavior_id;
			public readonly uint active_skill_lv;
			public readonly uint new_behavior;
			public readonly uint behavior_tool;
			public readonly uint show_skill_name;
			public readonly uint min_spirit;
			public readonly uint skill_type;
			public readonly uint no_mana_behavior;
			public readonly uint have_behavior;
			public readonly uint attack_range;
			public readonly uint type_cold_time;
			public readonly uint cold_time;
			public readonly List<uint> before_action;
			public readonly uint buff_limit_condition;
			public readonly uint buff_condition;
			public readonly uint mana_cost;
			public readonly uint energy_cost;
			public readonly uint attack_type;
			public readonly uint second_target;
			public readonly uint min_attack_num;
			public readonly uint max_attack_num;
			public readonly List<uint> skill_effect_id;
			public readonly int choose_type;
			public readonly List<uint> choose_skill_condition;
			public readonly List<uint> choose_wrong_target;
			public readonly List<uint> choose_req;
			public readonly uint choose_AI;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				main_skill_id = ReadHelper.ReadUInt(binaryReader);
				active_skill_behavior_id = ReadHelper.ReadUInt(binaryReader);
				active_skill_lv = ReadHelper.ReadUInt(binaryReader);
				new_behavior = ReadHelper.ReadUInt(binaryReader);
				behavior_tool = ReadHelper.ReadUInt(binaryReader);
				show_skill_name = ReadHelper.ReadUInt(binaryReader);
				min_spirit = ReadHelper.ReadUInt(binaryReader);
				skill_type = ReadHelper.ReadUInt(binaryReader);
				no_mana_behavior = ReadHelper.ReadUInt(binaryReader);
				have_behavior = ReadHelper.ReadUInt(binaryReader);
				attack_range = ReadHelper.ReadUInt(binaryReader);
				type_cold_time = ReadHelper.ReadUInt(binaryReader);
				cold_time = ReadHelper.ReadUInt(binaryReader);
				before_action = shareData.GetShareData<List<uint>>(binaryReader, 0);
				buff_limit_condition = ReadHelper.ReadUInt(binaryReader);
				buff_condition = ReadHelper.ReadUInt(binaryReader);
				mana_cost = ReadHelper.ReadUInt(binaryReader);
				energy_cost = ReadHelper.ReadUInt(binaryReader);
				attack_type = ReadHelper.ReadUInt(binaryReader);
				second_target = ReadHelper.ReadUInt(binaryReader);
				min_attack_num = ReadHelper.ReadUInt(binaryReader);
				max_attack_num = ReadHelper.ReadUInt(binaryReader);
				skill_effect_id = shareData.GetShareData<List<uint>>(binaryReader, 0);
				choose_type = ReadHelper.ReadInt(binaryReader);
				choose_skill_condition = shareData.GetShareData<List<uint>>(binaryReader, 0);
				choose_wrong_target = shareData.GetShareData<List<uint>>(binaryReader, 0);
				choose_req = shareData.GetShareData<List<uint>>(binaryReader, 0);
				choose_AI = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVActiveSkill.bytes";
		}

		private static CSVActiveSkill instance = null;			
		public static CSVActiveSkill Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVActiveSkill 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVActiveSkill forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVActiveSkill();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVActiveSkill");

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

    sealed public partial class CSVActiveSkill : FCSVActiveSkill
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVActiveSkill.bytes";
		}

		private static CSVActiveSkill instance = null;			
		public static CSVActiveSkill Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVActiveSkill 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVActiveSkill forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVActiveSkill();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVActiveSkill");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}