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

	sealed public partial class CSVTianTiRankingAward : Framework.Table.TableBase<CSVTianTiRankingAward.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Division;
			public readonly List<uint> Ranking;
			public readonly List<uint> RankingPer;
			public readonly uint Reward;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Division = ReadHelper.ReadUInt(binaryReader);
				Ranking = shareData.GetShareData<List<uint>>(binaryReader, 0);
				RankingPer = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Reward = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVTianTiRankingAward.bytes";
		}

		private static CSVTianTiRankingAward instance = null;			
		public static CSVTianTiRankingAward Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTianTiRankingAward 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTianTiRankingAward forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTianTiRankingAward();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTianTiRankingAward");

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

    sealed public partial class CSVTianTiRankingAward : FCSVTianTiRankingAward
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVTianTiRankingAward.bytes";
		}

		private static CSVTianTiRankingAward instance = null;			
		public static CSVTianTiRankingAward Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTianTiRankingAward 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTianTiRankingAward forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTianTiRankingAward();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTianTiRankingAward");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}