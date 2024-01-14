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

	sealed public partial class CSVMonster : Framework.Table.TableBase<CSVMonster.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint monster_name;
			public readonly uint monster_id;
			public readonly uint monster_rank;
			public readonly uint monster_behavior_rank;
			public readonly uint genus;
			public readonly uint level;
			public readonly List<uint> dynamic_level;
			public readonly List<List<uint>> template_attr;
			public readonly uint is_dynamic;
			public readonly uint attr_id;
			public readonly uint body_part;
			public readonly List<uint> init_buff;
			public readonly uint dead_behavior;
			public readonly string model;
			public readonly uint weapon_id;
			public readonly List<uint> weapon_action_id;
			public readonly uint ex_name;
			public readonly uint unlock_pet;
			public readonly uint template_id;
			public readonly uint name_colour;
			public readonly uint zooming;
			public readonly uint approach_bubble;
			public readonly uint die_bubble;
			public readonly uint refresh_action;
			public readonly uint show_name;
			public readonly uint target_select_type;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				monster_name = ReadHelper.ReadUInt(binaryReader);
				monster_id = ReadHelper.ReadUInt(binaryReader);
				monster_rank = ReadHelper.ReadUInt(binaryReader);
				monster_behavior_rank = ReadHelper.ReadUInt(binaryReader);
				genus = ReadHelper.ReadUInt(binaryReader);
				level = ReadHelper.ReadUInt(binaryReader);
				dynamic_level = shareData.GetShareData<List<uint>>(binaryReader, 1);
				template_attr = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				is_dynamic = ReadHelper.ReadUInt(binaryReader);
				attr_id = ReadHelper.ReadUInt(binaryReader);
				body_part = ReadHelper.ReadUInt(binaryReader);
				init_buff = shareData.GetShareData<List<uint>>(binaryReader, 1);
				dead_behavior = ReadHelper.ReadUInt(binaryReader);
				model = shareData.GetShareData<string>(binaryReader, 0);
				weapon_id = ReadHelper.ReadUInt(binaryReader);
				weapon_action_id = shareData.GetShareData<List<uint>>(binaryReader, 1);
				ex_name = ReadHelper.ReadUInt(binaryReader);
				unlock_pet = ReadHelper.ReadUInt(binaryReader);
				template_id = ReadHelper.ReadUInt(binaryReader);
				name_colour = ReadHelper.ReadUInt(binaryReader);
				zooming = ReadHelper.ReadUInt(binaryReader);
				approach_bubble = ReadHelper.ReadUInt(binaryReader);
				die_bubble = ReadHelper.ReadUInt(binaryReader);
				refresh_action = ReadHelper.ReadUInt(binaryReader);
				show_name = ReadHelper.ReadUInt(binaryReader);
				target_select_type = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVMonster.bytes";
		}

		private static CSVMonster instance = null;			
		public static CSVMonster Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVMonster 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVMonster forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVMonster();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVMonster");

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
			TableShareData shareData = new TableShareData(3);
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadArrays<uint>(binaryReader, 1, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArray2s<uint>(binaryReader, 2, 1);

			return shareData;
		}
	}

#else

    sealed public partial class CSVMonster : FCSVMonster
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVMonster.bytes";
		}

		private static CSVMonster instance = null;			
		public static CSVMonster Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVMonster 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVMonster forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVMonster();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVMonster");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}