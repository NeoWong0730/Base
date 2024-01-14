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

	sealed public partial class CSVDecisiveArenaRankingRedward : Framework.Table.TableBase<CSVDecisiveArenaRankingRedward.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Ranking;
			public readonly uint Reward;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Ranking = ReadHelper.ReadUInt(binaryReader);
				Reward = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVDecisiveArenaRankingRedward.bytes";
		}

		private static CSVDecisiveArenaRankingRedward instance = null;			
		public static CSVDecisiveArenaRankingRedward Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVDecisiveArenaRankingRedward 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVDecisiveArenaRankingRedward forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVDecisiveArenaRankingRedward();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVDecisiveArenaRankingRedward");

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

    sealed public partial class CSVDecisiveArenaRankingRedward : FCSVDecisiveArenaRankingRedward
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVDecisiveArenaRankingRedward.bytes";
		}

		private static CSVDecisiveArenaRankingRedward instance = null;			
		public static CSVDecisiveArenaRankingRedward Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVDecisiveArenaRankingRedward 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVDecisiveArenaRankingRedward forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVDecisiveArenaRankingRedward();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVDecisiveArenaRankingRedward");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}