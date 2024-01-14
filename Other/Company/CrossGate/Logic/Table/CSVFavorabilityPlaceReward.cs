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

	sealed public partial class CSVFavorabilityPlaceReward : Framework.Table.TableBase<CSVFavorabilityPlaceReward.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint PlaceName;
			public readonly uint RewardName;
			public readonly uint RewardDes;
			public readonly uint Num;
			public readonly uint Reward;
			public readonly uint SortID;
			public readonly uint BulletinId;
			public readonly string TownIcon;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				PlaceName = ReadHelper.ReadUInt(binaryReader);
				RewardName = ReadHelper.ReadUInt(binaryReader);
				RewardDes = ReadHelper.ReadUInt(binaryReader);
				Num = ReadHelper.ReadUInt(binaryReader);
				Reward = ReadHelper.ReadUInt(binaryReader);
				SortID = ReadHelper.ReadUInt(binaryReader);
				BulletinId = ReadHelper.ReadUInt(binaryReader);
				TownIcon = shareData.GetShareData<string>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVFavorabilityPlaceReward.bytes";
		}

		private static CSVFavorabilityPlaceReward instance = null;			
		public static CSVFavorabilityPlaceReward Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFavorabilityPlaceReward 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFavorabilityPlaceReward forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFavorabilityPlaceReward();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFavorabilityPlaceReward");

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
			shareData.ReadStrings(binaryReader, 0);

			return shareData;
		}
	}

#else

    sealed public partial class CSVFavorabilityPlaceReward : FCSVFavorabilityPlaceReward
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVFavorabilityPlaceReward.bytes";
		}

		private static CSVFavorabilityPlaceReward instance = null;			
		public static CSVFavorabilityPlaceReward Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFavorabilityPlaceReward 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFavorabilityPlaceReward forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFavorabilityPlaceReward();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFavorabilityPlaceReward");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}