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

	sealed public partial class CSVExperienceLevel : Framework.Table.TableBase<CSVExperienceLevel.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly List<List<uint>> cost;
			public readonly uint total_gongxun;
			public readonly uint score;
			public readonly uint need_level;
			public readonly uint need_technology;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				cost = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				total_gongxun = ReadHelper.ReadUInt(binaryReader);
				score = ReadHelper.ReadUInt(binaryReader);
				need_level = ReadHelper.ReadUInt(binaryReader);
				need_technology = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVExperienceLevel.bytes";
		}

		private static CSVExperienceLevel instance = null;			
		public static CSVExperienceLevel Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVExperienceLevel 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVExperienceLevel forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVExperienceLevel();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVExperienceLevel");

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

    sealed public partial class CSVExperienceLevel : FCSVExperienceLevel
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVExperienceLevel.bytes";
		}

		private static CSVExperienceLevel instance = null;			
		public static CSVExperienceLevel Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVExperienceLevel 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVExperienceLevel forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVExperienceLevel();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVExperienceLevel");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}