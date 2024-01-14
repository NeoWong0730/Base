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

	sealed public partial class CSVTtArenaRandAward : Framework.Table.TableBase<CSVTtArenaRandAward.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Division;
			public readonly uint Rank;
			public readonly uint Reward;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Division = ReadHelper.ReadUInt(binaryReader);
				Rank = ReadHelper.ReadUInt(binaryReader);
				Reward = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVTtArenaRandAward.bytes";
		}

		private static CSVTtArenaRandAward instance = null;			
		public static CSVTtArenaRandAward Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTtArenaRandAward 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTtArenaRandAward forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTtArenaRandAward();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTtArenaRandAward");

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
			TableShareData shareData = null;

			return shareData;
		}
	}

#else

    sealed public partial class CSVTtArenaRandAward : FCSVTtArenaRandAward
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVTtArenaRandAward.bytes";
		}

		private static CSVTtArenaRandAward instance = null;			
		public static CSVTtArenaRandAward Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTtArenaRandAward 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTtArenaRandAward forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTtArenaRandAward();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTtArenaRandAward");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}