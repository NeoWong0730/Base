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

	sealed public partial class CSVSurvivalRankReward : Framework.Table.TableBase<CSVSurvivalRankReward.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint RewardLevel;
			public readonly List<int> RankRange;
			public readonly uint DropView;
			public readonly uint KfDropView;
			public readonly uint Rewardshow;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				RewardLevel = ReadHelper.ReadUInt(binaryReader);
				RankRange = shareData.GetShareData<List<int>>(binaryReader, 0);
				DropView = ReadHelper.ReadUInt(binaryReader);
				KfDropView = ReadHelper.ReadUInt(binaryReader);
				Rewardshow = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVSurvivalRankReward.bytes";
		}

		private static CSVSurvivalRankReward instance = null;			
		public static CSVSurvivalRankReward Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVSurvivalRankReward 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVSurvivalRankReward forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVSurvivalRankReward();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVSurvivalRankReward");

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
			shareData.ReadArrays<int>(binaryReader, 0, ReadHelper.ReadArray_ReadInt);

			return shareData;
		}
	}

#else

    sealed public partial class CSVSurvivalRankReward : FCSVSurvivalRankReward
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVSurvivalRankReward.bytes";
		}

		private static CSVSurvivalRankReward instance = null;			
		public static CSVSurvivalRankReward Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVSurvivalRankReward 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVSurvivalRankReward forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVSurvivalRankReward();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVSurvivalRankReward");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}