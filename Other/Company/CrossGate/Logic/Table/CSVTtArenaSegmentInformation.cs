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

	sealed public partial class CSVTtArenaSegmentInformation : Framework.Table.TableBase<CSVTtArenaSegmentInformation.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Rank;
			public readonly uint RankSubordinate;
			public readonly uint RankInherit;
			public readonly uint RankDisplay;
			public readonly string RankIcon;
			public readonly uint RankIcon1;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Rank = ReadHelper.ReadUInt(binaryReader);
				RankSubordinate = ReadHelper.ReadUInt(binaryReader);
				RankInherit = ReadHelper.ReadUInt(binaryReader);
				RankDisplay = ReadHelper.ReadUInt(binaryReader);
				RankIcon = shareData.GetShareData<string>(binaryReader, 0);
				RankIcon1 = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVTtArenaSegmentInformation.bytes";
		}

		private static CSVTtArenaSegmentInformation instance = null;			
		public static CSVTtArenaSegmentInformation Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTtArenaSegmentInformation 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTtArenaSegmentInformation forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTtArenaSegmentInformation();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTtArenaSegmentInformation");

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

    sealed public partial class CSVTtArenaSegmentInformation : FCSVTtArenaSegmentInformation
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVTtArenaSegmentInformation.bytes";
		}

		private static CSVTtArenaSegmentInformation instance = null;			
		public static CSVTtArenaSegmentInformation Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTtArenaSegmentInformation 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTtArenaSegmentInformation forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTtArenaSegmentInformation();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTtArenaSegmentInformation");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}