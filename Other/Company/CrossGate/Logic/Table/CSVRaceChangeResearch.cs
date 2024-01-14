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

	sealed public partial class CSVRaceChangeResearch : Framework.Table.TableBase<CSVRaceChangeResearch.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly List<List<uint>> upgrade_cost;
			public readonly uint world_level_restrict;
			public readonly List<uint> upgrade_restrict;
			public readonly List<List<uint>> up_rank_cost;
			public readonly uint score;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				upgrade_cost = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				world_level_restrict = ReadHelper.ReadUInt(binaryReader);
				upgrade_restrict = shareData.GetShareData<List<uint>>(binaryReader, 0);
				up_rank_cost = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				score = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVRaceChangeResearch.bytes";
		}

		private static CSVRaceChangeResearch instance = null;			
		public static CSVRaceChangeResearch Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVRaceChangeResearch 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVRaceChangeResearch forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVRaceChangeResearch();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVRaceChangeResearch");

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

    sealed public partial class CSVRaceChangeResearch : FCSVRaceChangeResearch
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVRaceChangeResearch.bytes";
		}

		private static CSVRaceChangeResearch instance = null;			
		public static CSVRaceChangeResearch Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVRaceChangeResearch 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVRaceChangeResearch forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVRaceChangeResearch();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVRaceChangeResearch");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}