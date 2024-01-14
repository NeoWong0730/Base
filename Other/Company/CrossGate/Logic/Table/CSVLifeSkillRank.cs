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

	sealed public partial class CSVLifeSkillRank : Framework.Table.TableBase<CSVLifeSkillRank.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint desc_id;
			public readonly uint level_max;
			public readonly uint role_level;
			public readonly uint npc_id;
			public readonly uint task_id;
			public readonly List<List<uint>> cost_item;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				desc_id = ReadHelper.ReadUInt(binaryReader);
				level_max = ReadHelper.ReadUInt(binaryReader);
				role_level = ReadHelper.ReadUInt(binaryReader);
				npc_id = ReadHelper.ReadUInt(binaryReader);
				task_id = ReadHelper.ReadUInt(binaryReader);
				cost_item = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVLifeSkillRank.bytes";
		}

		private static CSVLifeSkillRank instance = null;			
		public static CSVLifeSkillRank Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVLifeSkillRank 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVLifeSkillRank forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVLifeSkillRank();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVLifeSkillRank");

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

    sealed public partial class CSVLifeSkillRank : FCSVLifeSkillRank
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVLifeSkillRank.bytes";
		}

		private static CSVLifeSkillRank instance = null;			
		public static CSVLifeSkillRank Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVLifeSkillRank 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVLifeSkillRank forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVLifeSkillRank();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVLifeSkillRank");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}