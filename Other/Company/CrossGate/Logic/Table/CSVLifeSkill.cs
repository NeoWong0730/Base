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

	sealed public partial class CSVLifeSkill : Framework.Table.TableBase<CSVLifeSkill.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint name_id;
			public readonly uint icon_id;
			public readonly uint type;
			public readonly uint lucky__value_max;
			public readonly uint equip_orange_times;
			public readonly uint learn_npc;
			public readonly uint task_id;
			public readonly uint icon_id_learn;
			public readonly uint picture;
			public readonly string npc_image;
			public readonly uint desc_id;
			public readonly List<uint> add_proficiency_item;
			public readonly uint max_level;
			public readonly uint function_id;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				name_id = ReadHelper.ReadUInt(binaryReader);
				icon_id = ReadHelper.ReadUInt(binaryReader);
				type = ReadHelper.ReadUInt(binaryReader);
				lucky__value_max = ReadHelper.ReadUInt(binaryReader);
				equip_orange_times = ReadHelper.ReadUInt(binaryReader);
				learn_npc = ReadHelper.ReadUInt(binaryReader);
				task_id = ReadHelper.ReadUInt(binaryReader);
				icon_id_learn = ReadHelper.ReadUInt(binaryReader);
				picture = ReadHelper.ReadUInt(binaryReader);
				npc_image = shareData.GetShareData<string>(binaryReader, 0);
				desc_id = ReadHelper.ReadUInt(binaryReader);
				add_proficiency_item = shareData.GetShareData<List<uint>>(binaryReader, 1);
				max_level = ReadHelper.ReadUInt(binaryReader);
				function_id = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVLifeSkill.bytes";
		}

		private static CSVLifeSkill instance = null;			
		public static CSVLifeSkill Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVLifeSkill 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVLifeSkill forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVLifeSkill();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVLifeSkill");

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
			TableShareData shareData = new TableShareData(2);
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadArrays<uint>(binaryReader, 1, ReadHelper.ReadArray_ReadUInt);

			return shareData;
		}
	}

#else

    sealed public partial class CSVLifeSkill : FCSVLifeSkill
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVLifeSkill.bytes";
		}

		private static CSVLifeSkill instance = null;			
		public static CSVLifeSkill Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVLifeSkill 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVLifeSkill forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVLifeSkill();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVLifeSkill");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}