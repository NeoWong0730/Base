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

	sealed public partial class CSVTitleSeries : Framework.Table.TableBase<CSVTitleSeries.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint seriesLan;
			public readonly List<List<uint>> seriesProperty;
			public readonly int seriesPoint;
			public readonly List<uint> seriesTitle;
			public readonly List<List<uint>> seriesCollect;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				seriesLan = ReadHelper.ReadUInt(binaryReader);
				seriesProperty = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				seriesPoint = ReadHelper.ReadInt(binaryReader);
				seriesTitle = shareData.GetShareData<List<uint>>(binaryReader, 0);
				seriesCollect = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVTitleSeries.bytes";
		}

		private static CSVTitleSeries instance = null;			
		public static CSVTitleSeries Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTitleSeries 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTitleSeries forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTitleSeries();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTitleSeries");

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

    sealed public partial class CSVTitleSeries : FCSVTitleSeries
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVTitleSeries.bytes";
		}

		private static CSVTitleSeries instance = null;			
		public static CSVTitleSeries Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTitleSeries 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTitleSeries forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTitleSeries();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTitleSeries");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}