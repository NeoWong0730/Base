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

	sealed public partial class CSVTianTiSegmentInformation : Framework.Table.TableBase<CSVTianTiSegmentInformation.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Score;
			public readonly uint KeepScore;
			public readonly uint RankDisplay;
			public readonly string RankIcon;
			public readonly uint RankIcon1;
			public readonly uint Tittle;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Score = ReadHelper.ReadUInt(binaryReader);
				KeepScore = ReadHelper.ReadUInt(binaryReader);
				RankDisplay = ReadHelper.ReadUInt(binaryReader);
				RankIcon = shareData.GetShareData<string>(binaryReader, 0);
				RankIcon1 = ReadHelper.ReadUInt(binaryReader);
				Tittle = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVTianTiSegmentInformation.bytes";
		}

		private static CSVTianTiSegmentInformation instance = null;			
		public static CSVTianTiSegmentInformation Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTianTiSegmentInformation 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTianTiSegmentInformation forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTianTiSegmentInformation();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTianTiSegmentInformation");

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

    sealed public partial class CSVTianTiSegmentInformation : FCSVTianTiSegmentInformation
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVTianTiSegmentInformation.bytes";
		}

		private static CSVTianTiSegmentInformation instance = null;			
		public static CSVTianTiSegmentInformation Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTianTiSegmentInformation 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTianTiSegmentInformation forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTianTiSegmentInformation();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTianTiSegmentInformation");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}