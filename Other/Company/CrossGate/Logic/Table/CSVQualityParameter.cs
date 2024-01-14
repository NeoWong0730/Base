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

	sealed public partial class CSVQualityParameter : Framework.Table.TableBase<CSVQualityParameter.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly List<uint> base_cor;
			public readonly List<List<uint>> green_weight;
			public readonly List<uint> green_range;
			public readonly List<List<uint>> special_weight;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				base_cor = shareData.GetShareData<List<uint>>(binaryReader, 0);
				green_weight = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				green_range = shareData.GetShareData<List<uint>>(binaryReader, 0);
				special_weight = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVQualityParameter.bytes";
		}

		private static CSVQualityParameter instance = null;			
		public static CSVQualityParameter Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVQualityParameter 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVQualityParameter forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVQualityParameter();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVQualityParameter");

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

    sealed public partial class CSVQualityParameter : FCSVQualityParameter
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVQualityParameter.bytes";
		}

		private static CSVQualityParameter instance = null;			
		public static CSVQualityParameter Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVQualityParameter 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVQualityParameter forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVQualityParameter();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVQualityParameter");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}