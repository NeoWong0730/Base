//
//#define USE_HOTFIX_LOGIC

using Lib.AssetLoader;
using Lib.Core;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Framework.Table;

namespace Table
{
#if USE_HOTFIX_LOGIC

	sealed public partial class CSVLifeSkillLevel : Framework.Table.TableBase<CSVLifeSkillLevel.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint skill_id;
			public readonly uint level;
			public readonly uint proficiency;
			public readonly uint role_level;
			public readonly List<List<uint>> cost_item;
			public readonly List<uint> collection_item_id;
			public readonly uint map_id;
			public readonly uint collection_npc;
			public readonly List<uint> active_npc;
			public readonly uint cost_vitality;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				skill_id = ReadHelper.ReadUInt(binaryReader);
				level = ReadHelper.ReadUInt(binaryReader);
				proficiency = ReadHelper.ReadUInt(binaryReader);
				role_level = ReadHelper.ReadUInt(binaryReader);
				cost_item = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				collection_item_id = shareData.GetShareData<List<uint>>(binaryReader, 0);
				map_id = ReadHelper.ReadUInt(binaryReader);
				collection_npc = ReadHelper.ReadUInt(binaryReader);
				active_npc = shareData.GetShareData<List<uint>>(binaryReader, 0);
				cost_vitality = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVLifeSkillLevel.bytes";
		}

		private static CSVLifeSkillLevel instance = null;			
		public static CSVLifeSkillLevel Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVLifeSkillLevel 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVLifeSkillLevel forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVLifeSkillLevel();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVLifeSkillLevel");

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
			shareData.ReadArrays<uint>(binaryReader, 0, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArray2s<uint>(binaryReader, 1, 0);

			return shareData;
		}
	}

#else

    sealed public partial class CSVLifeSkillLevel : FCSVLifeSkillLevel
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVLifeSkillLevel.bytes";
		}

		private static CSVLifeSkillLevel instance = null;			
		public static CSVLifeSkillLevel Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVLifeSkillLevel 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVLifeSkillLevel forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVLifeSkillLevel();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVLifeSkillLevel");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}