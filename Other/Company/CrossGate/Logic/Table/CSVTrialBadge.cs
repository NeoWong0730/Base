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

	sealed public partial class CSVTrialBadge : Framework.Table.TableBase<CSVTrialBadge.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint badge_name;
			public readonly uint bigIcon_id;
			public readonly uint badge_description;
			public readonly uint badge_source;
			public readonly List<List<uint>> firstLevelGradeBoss;
			public readonly List<List<uint>> secendLevelGradeBoss;
			public readonly List<List<uint>> thirdLevelGradeBoss;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				badge_name = ReadHelper.ReadUInt(binaryReader);
				bigIcon_id = ReadHelper.ReadUInt(binaryReader);
				badge_description = ReadHelper.ReadUInt(binaryReader);
				badge_source = ReadHelper.ReadUInt(binaryReader);
				firstLevelGradeBoss = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				secendLevelGradeBoss = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				thirdLevelGradeBoss = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVTrialBadge.bytes";
		}

		private static CSVTrialBadge instance = null;			
		public static CSVTrialBadge Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTrialBadge 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTrialBadge forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTrialBadge();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTrialBadge");

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

    sealed public partial class CSVTrialBadge : FCSVTrialBadge
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVTrialBadge.bytes";
		}

		private static CSVTrialBadge instance = null;			
		public static CSVTrialBadge Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTrialBadge 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTrialBadge forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTrialBadge();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTrialBadge");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}